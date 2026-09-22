using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Engine;

/// <summary>Owns one complete native Mutagen source graph, mutable output, cache, and lifetime lock set.</summary>
public sealed class NativeWorkspace : IDisposable
{
    private readonly IGameIntegration _integration;
    private readonly IReadOnlyList<IModDisposeGetter> _ownedSources;
    private readonly WorkspaceFileLockSet _fileLocks;
    private readonly IMod _output;
    private NativeWorkspaceState _state;
    private bool _disposed;

    internal NativeWorkspace(
        IGameIntegration integration,
        IReadOnlyList<IModDisposeGetter> sources,
        IMod output,
        ILinkCache linkCache,
        WorkspaceFileLockSet fileLocks,
        NativeWorkspaceState state)
    {
        _integration = integration;
        _ownedSources = sources;
        _output = output;
        LinkCache = linkCache;
        _fileLocks = fileLocks;
        _state = state;
    }

    /// <summary>Gets immutable source plugins in masters-first, low-to-high priority order.</summary>
    public IReadOnlyList<IModGetter> Sources => _ownedSources;

    /// <summary>Gets a read-only view of the complete native output.</summary>
    public IModGetter Output => _output;

    /// <summary>Gets the mutable native output for engine-owned editing operations.</summary>
    internal IMod MutableOutput => _output;

    /// <summary>Gets the native cache whose immutable base is <see cref="Sources"/> and whose mutable layer is <see cref="Output"/>.</summary>
    public ILinkCache LinkCache { get; }

    /// <summary>Gets constant-time workspace and output state.</summary>
    public NativeWorkspaceState State => _state;

    /// <summary>Resolves both an exact containing-plugin record context and its winning context.</summary>
    /// <param name="formKey">The record's origin identity.</param>
    /// <param name="recordType">The native Mutagen record getter type.</param>
    /// <param name="containingModKey">The plugin containing the exact requested record version.</param>
    /// <returns>Both native contexts without flattening their parent chains.</returns>
    public NativeRecordResolution ResolveRecord(FormKey formKey, Type recordType, ModKey containingModKey)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(recordType);
        if (!typeof(IMajorRecordGetter).IsAssignableFrom(recordType))
        {
            throw new ArgumentException($"'{recordType}' is not a native Mutagen major-record getter type.", nameof(recordType));
        }

        var exactContext = _integration.ResolveContextFromMod(LinkCache, formKey, recordType, containingModKey);
        var winningContext = _integration.ResolveWinningContext(LinkCache, formKey, recordType);
        return new NativeRecordResolution(exactContext, winningContext);
    }

    /// <summary>Browses winning native contexts from only this workspace's explicit source closure and output.</summary>
    /// <param name="recordType">The native Mutagen record getter type.</param>
    /// <returns>A materialized snapshot of winning contexts with native parent chains.</returns>
    public IReadOnlyList<IModContext> BrowseWinningRecords(Type recordType)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(recordType);
        if (!typeof(IMajorRecordGetter).IsAssignableFrom(recordType))
        {
            throw new ArgumentException($"'{recordType}' is not a native Mutagen major-record getter type.", nameof(recordType));
        }

        return _integration
            .EnumerateWinningContexts(LinkCache, recordType)
            .ToArray();
    }

    /// <summary>Records one successfully committed logical output mutation.</summary>
    internal void MarkOutputChanged()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _state = new NativeWorkspaceState(
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
            throw new AggregateException("One or more native workspace resources failed to close.", exceptions);
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
