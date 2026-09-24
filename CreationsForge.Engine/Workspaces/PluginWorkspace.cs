using System.Diagnostics;
using CreationsForge.Engine.Interfaces;
using CreationsForge.Engine.Persistence;
using CreationsForge.Engine.Records;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Engine.Workspaces;

/// <summary>Owns one complete Mutagen plugin source graph, mutable output, link cache, and lifetime lock set.</summary>
public sealed class PluginWorkspace : IDisposable
{
    private readonly IGameIntegration _integration;
    private readonly IReadOnlyList<IModDisposeGetter> _ownedSources;
    private readonly WorkspaceFileLockSet _fileLocks;
    private readonly PluginSaveService _saveService;
    private readonly SemaphoreSlim _operationGate = new(1, 1);
    private readonly CancellationTokenSource _closeCancellation = new();
    private readonly Dictionary<PendingSnapshotKey, RecordSnapshot> _pendingSaveSnapshots = [];
    private IMod _output;
    private IMod _savedBaseline;
    private ILinkCache _linkCache;
    private PluginDestinationStamp? _expectedDestinationStamp;
    private PluginWorkspaceState _state;
    private ulong _savedRevision;
    private int _closeRequested;
    private bool _disposed;

    internal PluginWorkspace(
        IGameIntegration integration,
        IReadOnlyList<IModDisposeGetter> sources,
        IMod output,
        IMod savedBaseline,
        ILinkCache linkCache,
        WorkspaceFileLockSet fileLocks,
        PluginWorkspaceState state,
        string dataDirectory,
        Language targetLanguage,
        PluginDestinationStamp? expectedDestinationStamp,
        IPluginPersistenceBackend persistenceBackend)
    {
        _integration = integration;
        _ownedSources = sources;
        _output = output;
        _savedBaseline = savedBaseline;
        _linkCache = linkCache;
        _fileLocks = fileLocks;
        _state = state;
        DataDirectory = dataDirectory;
        TargetLanguage = targetLanguage;
        _expectedDestinationStamp = expectedDestinationStamp;
        _saveService = new PluginSaveService(persistenceBackend);
        _savedRevision = state.Revision;
        Records = new RecordEditor(this, integration.RecordFamilies);
    }

    /// <summary>Gets immutable source plugins in masters-first, low-to-high priority order.</summary>
    public IReadOnlyList<IModGetter> Sources => _ownedSources;

    /// <summary>Gets a read-only view of the complete Mutagen plugin output.</summary>
    public IModGetter Output => _output;

    /// <summary>Gets the mutable Mutagen plugin output for engine-owned editing operations.</summary>
    internal IMod MutableOutput => _output;

    /// <summary>Gets the exact data-directory context admitted when this workspace opened.</summary>
    internal string DataDirectory { get; }

    /// <summary>Gets the active translated-string language admitted when this workspace opened.</summary>
    internal Language TargetLanguage { get; }

    /// <summary>Gets the game-specific native integration that owns this output.</summary>
    internal IGameIntegration Integration => _integration;

    /// <summary>Gets native source/master objects in explicit masters-first load-order order.</summary>
    internal IReadOnlyList<IModMasterStyledGetter> SourceMasters => _ownedSources;

    /// <summary>Gets the destination identity expected from the last open or confirmed save.</summary>
    internal PluginDestinationStamp? ExpectedDestinationStamp => _expectedDestinationStamp;

    /// <summary>Gets the latest registered values for records changed since the saved baseline.</summary>
    internal IReadOnlyList<RecordSnapshot> PendingSaveSnapshots => _pendingSaveSnapshots.Values.ToArray();

    /// <summary>Throws when an engine-owned operation is attempted after the workspace has closed.</summary>
    internal void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed || Volatile.Read(ref _closeRequested) != 0, this);
    }

    /// <summary>Gets the Mutagen link cache whose immutable base is <see cref="Sources"/> and whose mutable layer is <see cref="Output"/>.</summary>
    public ILinkCache LinkCache => _linkCache;

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
        return ExecuteExclusive(() => ResolveRecordCore(formKey, recordType, containingModKey));
    }

    /// <summary>Browses winning Mutagen contexts from only this workspace's explicit source closure and output.</summary>
    /// <param name="recordType">The Mutagen record getter type.</param>
    /// <returns>A materialized snapshot of winning contexts with Mutagen parent chains.</returns>
    public IReadOnlyList<IModContext> BrowseWinningRecords(Type recordType)
    {
        return ExecuteExclusive(() =>
        {
            ArgumentNullException.ThrowIfNull(recordType);
            if (!typeof(IMajorRecordGetter).IsAssignableFrom(recordType))
            {
                throw new ArgumentException($"'{recordType}' is not a Mutagen major-record getter type.", nameof(recordType));
            }

            return _integration
                .EnumerateWinningContexts(LinkCache, recordType)
                .ToArray();
        });
    }

    /// <summary>Exports, verifies, publishes, and reopens the complete native output file set.</summary>
    /// <param name="progress">Optional UI-neutral lifecycle diagnostics whose report calls are serialized outside the workspace operation gate and completed before this method returns.</param>
    /// <param name="cancellationToken">Cancellation honored before destination publication begins.</param>
    /// <returns>The bounded save and publication result.</returns>
    public async Task<PluginSaveResult> SaveAsync(
        IProgress<PluginSaveProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var progressDispatcher = new PluginSaveProgressDispatcher(progress);
        try
        {
            try
            {
                return await ExecuteExclusiveAsync(
                    token => _saveService.SaveAsync(this, progressDispatcher, token),
                    cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested || _closeCancellation.IsCancellationRequested)
            {
                return _saveService.CanceledBeforeStart(this, stopwatch.Elapsed, progressDispatcher);
            }
        }
        finally
        {
            await progressDispatcher.CompleteAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Restores the last native saved baseline without writing or deleting destination files.</summary>
    /// <returns>The resulting revision and dirty state.</returns>
    public PluginDiscardResult Discard()
    {
        return ExecuteExclusive(() =>
        {
            if (_state.RequiresReopen)
            {
                throw new PluginWorkspaceException("The workspace requires a fresh reopen before discard because the last publication state was not safely adopted.");
            }

            if (_state.Revision == _savedRevision)
            {
                return new PluginDiscardResult(
                    _state.Revision,
                    changed: false,
                    _state.IsDirty,
                    _state.IsDirty
                        ? "The new empty output already matches its unsaved native baseline and still needs a physical first save."
                        : "The workspace already matches its saved native baseline.");
            }

            if (_state.Revision == ulong.MaxValue)
            {
                throw new PluginWorkspaceException("The workspace revision cannot be incremented beyond UInt64.MaxValue.");
            }

            var replacement = _integration.CloneOutput(_savedBaseline);
            ILinkCache? replacementCache = null;
            try
            {
                replacementCache = _integration.CreateLinkCache(_ownedSources, replacement);
            }
            catch
            {
                TryDisposeResource(replacementCache as IDisposable);
                TryDisposeResource(replacement as IDisposable);
                throw;
            }

            var oldOutput = _output;
            var oldLinkCache = _linkCache;
            _output = replacement;
            _linkCache = replacementCache;
            _pendingSaveSnapshots.Clear();
            var revision = checked(_state.Revision + 1);
            _savedRevision = revision;
            var remainsDirty = _expectedDestinationStamp is null;
            UpdateState(remainsDirty, revision, requiresReopen: false);
            var cleanupFailures = new List<Exception>();
            DisposeResource(oldLinkCache as IDisposable, cleanupFailures);
            DisposeResource(oldOutput as IDisposable, cleanupFailures);
            return new PluginDiscardResult(
                revision,
                changed: true,
                remainsDirty,
                cleanupFailures.Count == 0
                    ? "The native saved baseline was restored without touching destination files."
                    : "The native saved baseline was restored, but one or more replaced in-memory resources did not close cleanly.");
        });
    }

    /// <summary>Records one successfully committed logical output mutation.</summary>
    internal void MarkOutputChanged(IEnumerable<RecordSnapshot> snapshots)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(snapshots);
        foreach (var snapshot in snapshots)
        {
            _pendingSaveSnapshots[new PendingSnapshotKey(snapshot.FamilyId, snapshot.FormKey)] = snapshot;
        }

        UpdateState(isDirty: true, checked(_state.Revision + 1), requiresReopen: false);
    }

    /// <summary>Serializes one synchronous workspace operation with save, discard, and close.</summary>
    internal TResult ExecuteExclusive<TResult>(Func<TResult> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ThrowIfDisposed();
        _operationGate.Wait();
        try
        {
            ThrowIfDisposed();
            return operation();
        }
        finally
        {
            _operationGate.Release();
        }
    }

    /// <summary>Resolves a context while the caller already owns the workspace operation gate.</summary>
    internal PluginRecordResolution ResolveRecordCore(FormKey formKey, Type recordType, ModKey containingModKey)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(recordType);
        if (!typeof(IMajorRecordGetter).IsAssignableFrom(recordType))
        {
            throw new ArgumentException($"'{recordType}' is not a Mutagen major-record getter type.", nameof(recordType));
        }

        var exactContext = _integration.ResolveContextFromMod(LinkCache, formKey, recordType, containingModKey);
        var winningContext = _integration.ResolveWinningContext(LinkCache, formKey, recordType);
        return new PluginRecordResolution(exactContext, winningContext);
    }

    /// <summary>Adopts a reopened published output as both the live output and saved native baseline.</summary>
    internal bool AcceptSavedOutput(IMod publishedOutput, PluginDestinationStamp publishedStamp)
    {
        ArgumentNullException.ThrowIfNull(publishedOutput);
        ArgumentNullException.ThrowIfNull(publishedStamp);
        var savedBaseline = _integration.CloneOutput(publishedOutput);
        ILinkCache? linkCache = null;
        try
        {
            linkCache = _integration.CreateLinkCache(_ownedSources, publishedOutput);
        }
        catch
        {
            TryDisposeResource(linkCache as IDisposable);
            TryDisposeResource(savedBaseline as IDisposable);
            throw;
        }

        var oldOutput = _output;
        var oldBaseline = _savedBaseline;
        var oldLinkCache = _linkCache;
        _output = publishedOutput;
        _savedBaseline = savedBaseline;
        _linkCache = linkCache;
        _expectedDestinationStamp = publishedStamp;
        _pendingSaveSnapshots.Clear();
        _savedRevision = _state.Revision;
        UpdateState(isDirty: false, _state.Revision, requiresReopen: false);

        var cleanupFailures = new List<Exception>();
        DisposeResource(oldLinkCache as IDisposable, cleanupFailures);
        DisposeResource(oldOutput as IDisposable, cleanupFailures);
        DisposeResource(oldBaseline as IDisposable, cleanupFailures);
        return cleanupFailures.Count == 0;
    }

    /// <summary>Prevents blind persistence retries after an unadopted or partial publication.</summary>
    internal void MarkRequiresReopen()
    {
        UpdateState(isDirty: true, _state.Revision, requiresReopen: true);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _closeRequested, 1) != 0)
        {
            return;
        }

        _closeCancellation.Cancel();
        _operationGate.Wait();
        try
        {
            _disposed = true;
            var exceptions = new List<Exception>();
            DisposeResource(LinkCache as IDisposable, exceptions);
            DisposeResource(_output as IDisposable, exceptions);
            DisposeResource(_savedBaseline as IDisposable, exceptions);

            for (var index = _ownedSources.Count - 1; index >= 0; index--)
            {
                DisposeResource(_ownedSources[index], exceptions);
            }

            DisposeResource(_fileLocks, exceptions);
            if (exceptions.Count > 0)
            {
                throw new AggregateException("One or more plugin workspace resources failed to close.", exceptions);
            }
        }
        finally
        {
            _operationGate.Release();
            _closeCancellation.Dispose();
        }
    }

    private async Task<TResult> ExecuteExclusiveAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ThrowIfDisposed();
        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            _closeCancellation.Token);
        await _operationGate.WaitAsync(linkedCancellation.Token).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            return await operation(linkedCancellation.Token).ConfigureAwait(false);
        }
        finally
        {
            _operationGate.Release();
        }
    }

    private void UpdateState(bool isDirty, ulong revision, bool requiresReopen)
    {
        _state = new PluginWorkspaceState(
            _state.Release,
            _state.OutputModKey,
            _state.OutputPath,
            _state.MasterStyle,
            _state.TextStorageMode,
            _state.IsNewOutput,
            isDirty,
            revision,
            requiresReopen);
    }

    private static void DisposeResource(IDisposable? resource, ICollection<Exception> exceptions)
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

    private static void TryDisposeResource(IDisposable? resource)
    {
        try
        {
            resource?.Dispose();
        }
        catch
        {
            // Preserve the actionable construction failure while still attempting each owned cleanup.
        }
    }

    private readonly record struct PendingSnapshotKey(string FamilyId, FormKey FormKey);
}
