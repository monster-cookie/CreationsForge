using CreationsForge.Engine.Interfaces;
using CreationsForge.Engine.Records;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Engine.Workspaces;

/// <summary>Owns one complete Mutagen plugin source graph, mutable output, link cache, and lifetime lock set.</summary>
public sealed class PluginWorkspace : IDisposable
{
    private readonly IGameIntegration _integration;
    private readonly IReadOnlyList<IModDisposeGetter> _ownedSources;
    private readonly WorkspaceFileLockSet _fileLocks;
    private readonly IMod _output;
    private PluginWorkspaceState _state;
    private bool _disposed;

    internal PluginWorkspace(
        IGameIntegration integration,
        IReadOnlyList<IModDisposeGetter> sources,
        IMod output,
        ILinkCache linkCache,
        WorkspaceFileLockSet fileLocks,
        PluginWorkspaceState state)
    {
        _integration = integration;
        _ownedSources = sources;
        _output = output;
        LinkCache = linkCache;
        _fileLocks = fileLocks;
        _state = state;
        Records = new RecordEditor(this, integration.RecordFamilies);
    }

    /// <summary>Gets immutable source plugins in masters-first, low-to-high priority order.</summary>
    public IReadOnlyList<IModGetter> Sources => _ownedSources;

    /// <summary>Gets a read-only view of the complete Mutagen plugin output.</summary>
    public IModGetter Output => _output;

    /// <summary>Gets the mutable Mutagen plugin output for engine-owned editing operations.</summary>
    internal IMod MutableOutput => _output;

    /// <summary>Throws when an engine-owned operation is attempted after the workspace has closed.</summary>
    internal void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    /// <summary>Gets the Mutagen link cache whose immutable base is <see cref="Sources"/> and whose mutable layer is <see cref="Output"/>.</summary>
    public ILinkCache LinkCache { get; }

    /// <summary>Gets the family-independent record editing boundary owned by this workspace.</summary>
    public RecordEditor Records { get; }

    /// <summary>Gets constant-time workspace and output state.</summary>
    public PluginWorkspaceState State => _state;

    /// <summary>Resolves both an exact containing-plugin record context and its winning context.</summary>
    /// <param name="formKey">The record's origin identity.</param>
    /// <param name="recordType">The Mutagen record getter type.</param>
    /// <param name="containingModKey">The plugin containing the exact requested record version.</param>
    /// <returns>Both Mutagen contexts without flattening their parent chains.</returns>
    public PluginRecordResolution ResolveRecord(FormKey formKey, Type recordType, ModKey containingModKey)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(recordType);
        if (!typeof(IMajorRecordGetter).IsAssignableFrom(recordType))
        {
            throw new ArgumentException($"'{recordType}' is not a Mutagen major-record getter type.", nameof(recordType));
        }

        var exactContext = _integration.ResolveContextFromMod(LinkCache, formKey, recordType, containingModKey);
        var winningContext = _integration.ResolveWinningContext(LinkCache, formKey, recordType);
        return new PluginRecordResolution(exactContext, winningContext);
    }

    /// <summary>Browses winning Mutagen contexts from only this workspace's explicit source closure and output.</summary>
    /// <param name="recordType">The Mutagen record getter type.</param>
    /// <returns>A materialized snapshot of winning contexts with Mutagen parent chains.</returns>
    public IReadOnlyList<IModContext> BrowseWinningRecords(Type recordType)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(recordType);
        if (!typeof(IMajorRecordGetter).IsAssignableFrom(recordType))
        {
            throw new ArgumentException($"'{recordType}' is not a Mutagen major-record getter type.", nameof(recordType));
        }

        return _integration
            .EnumerateWinningContexts(LinkCache, recordType)
            .ToArray();
    }

    /// <summary>Records one successfully committed logical output mutation.</summary>
    internal void MarkOutputChanged()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _state = new PluginWorkspaceState(
            _state.Release,
            _state.OutputModKey,
            _state.OutputPath,
            _state.MasterStyle,
            _state.TextStorageMode,
            _state.IsNewOutput,
            isDirty: true,
            checked(_state.Revision + 1));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        var exceptions = new List<Exception>();
        TryDispose(LinkCache as IDisposable, exceptions);
        TryDispose(_output as IDisposable, exceptions);

        for (var index = _ownedSources.Count - 1; index >= 0; index--)
        {
            TryDispose(_ownedSources[index], exceptions);
        }

        TryDispose(_fileLocks, exceptions);
        if (exceptions.Count > 0)
        {
            throw new AggregateException("One or more plugin workspace resources failed to close.", exceptions);
        }
    }

    private static void TryDispose(IDisposable? resource, ICollection<Exception> exceptions)
    {
        try
        {
            resource?.Dispose();
        }
        catch (Exception exception)
        {
            exceptions.Add(exception);
        }
    }
}
