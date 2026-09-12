using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Engine.NativeReading;
using Mutagen.Bethesda.Fallout4;

namespace CreationsForge.Fallout4.Native;

/// <summary>
/// Owns fully materialized Fallout 4 source plugins together with their verified native input baseline.
/// </summary>
public sealed class Fallout4NativeSourceSet : INativeSourceSet
{
    /// <summary>The workspace that owns this source lifetime and its reference cursors.</summary>
    private readonly Guid _workspaceId;

    /// <summary>The shared validated input lifetime, including resource fingerprints.</summary>
    private readonly NativeSourceInputs _inputs;

    /// <summary>The fully materialized native Fallout 4 plugins in explicit load-order order.</summary>
    private readonly IReadOnlyList<IFallout4ModGetter> _nativeMods;

    /// <summary>The bounded reader that borrows the materialized native plugin handles.</summary>
    private readonly NativeReferenceReader _referenceReader;

    /// <summary>Tracks whether disposal has already released the source lifetime.</summary>
    private int _disposed;

    /// <summary>
    /// Initializes an owned Fallout 4 native source set after parsing and baseline verification complete.
    /// </summary>
    /// <param name="workspaceId">The workspace that owns this source lifetime.</param>
    /// <param name="inputs">The shared native input lifetime transferred to this source set.</param>
    /// <param name="nativeMods">The fully materialized Fallout 4 plugins in explicit load-order order.</param>
    /// <param name="baseline">The complete verified baseline established after native parsing.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="workspaceId"/> is empty or the input and native plugin counts disagree.</exception>
    /// <exception cref="ArgumentNullException">Thrown when a required lifetime or collection is <see langword="null"/>.</exception>
    internal Fallout4NativeSourceSet(
        Guid workspaceId,
        NativeSourceInputs inputs,
        IReadOnlyList<IFallout4ModGetter> nativeMods,
        NativeSourceInputBaseline baseline)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("A Fallout 4 native source lifetime requires a non-empty workspace identifier.", nameof(workspaceId));
        }

        ArgumentNullException.ThrowIfNull(inputs);
        ArgumentNullException.ThrowIfNull(nativeMods);
        ArgumentNullException.ThrowIfNull(baseline);
        if (inputs.Plugins.Count != nativeMods.Count)
        {
            throw new ArgumentException("Every admitted Fallout 4 plugin must have exactly one materialized native source.", nameof(nativeMods));
        }

        _workspaceId = workspaceId;
        _inputs = inputs;
        _nativeMods = Array.AsReadOnly(nativeMods.ToArray());
        _referenceReader = new NativeReferenceReader(
            inputs.Plugins
                .Select((plugin, index) => new NativeReferenceSource(
                    _nativeMods[index],
                    plugin.Path,
                    plugin.LoadOrderIndex,
                    plugin.Role))
                .ToArray(),
            EnumerateFormListLinks);
        Baseline = baseline;
        Revision = new WorkspaceRevision(baseline.BaselineId, 0);
    }

    /// <summary>
    /// Gets the complete verified source and localized-resource baseline established during open.
    /// </summary>
    public NativeSourceInputBaseline Baseline { get; }

    /// <summary>
    /// Gets the source-only revision bound to the verified input baseline.
    /// </summary>
    public WorkspaceRevision Revision { get; }

    /// <summary>
    /// Gets the workspace that owns this source lifetime and its continuation tokens.
    /// </summary>
    public Guid WorkspaceId => _workspaceId;

    /// <summary>Lists the explicit Fallout 4 source plugins in their supplied load-order order.</summary>
    /// <param name="cancellationToken">A token observed while producing the immutable summaries.</param>
    /// <returns>The source plugin summaries, or a typed disposed failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<IReadOnlyList<PluginSummary>> ListPlugins(
        CancellationToken cancellationToken = default)
    {
        var readerResult = GetReferenceReader();
        if (!readerResult.Succeeded)
        {
            return EngineResult<IReadOnlyList<PluginSummary>>.Failure(
                readerResult.Error!,
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        var summaries = new List<PluginSummary>(_inputs.Plugins.Count);
        foreach (var plugin in _inputs.Plugins)
        {
            cancellationToken.ThrowIfCancellationRequested();
            summaries.Add(new PluginSummary(
                plugin.ModKey,
                plugin.Path,
                plugin.LoadOrderIndex,
                plugin.Role));
        }

        return EngineResult<IReadOnlyList<PluginSummary>>.Success(
            Array.AsReadOnly(summaries.ToArray()),
            workspaceId: WorkspaceId,
            resultRevision: Revision);
    }

    /// <summary>Lists typed Fallout 4 FormList contexts without building or retaining a record index.</summary>
    /// <param name="scope">The source-only, all-contexts, winning-override, or unavailable staged-output view.</param>
    /// <param name="cancellationToken">A token observed throughout native group traversal and override counting.</param>
    /// <returns>FormList summaries in containing load-order and native group order, or a typed request or disposed failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<IReadOnlyList<FormListSummary>> ListFormLists(
        RecordScope scope,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(scope))
        {
            return EngineResult<IReadOnlyList<FormListSummary>>.Failure(
                new EngineError(EngineErrorCode.InvalidRequest, $"The Fallout 4 FormList scope '{scope}' is undefined."),
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        var readerResult = GetReferenceReader();
        if (!readerResult.Succeeded)
        {
            return EngineResult<IReadOnlyList<FormListSummary>>.Failure(
                readerResult.Error!,
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        var summaries = new List<FormListSummary>();
        if (scope == RecordScope.StagedOutput)
        {
            return EngineResult<IReadOnlyList<FormListSummary>>.Success(
                Array.AsReadOnly(summaries.ToArray()),
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        for (var sourceIndex = 0; sourceIndex < _nativeMods.Count; sourceIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var plugin = _inputs.Plugins[sourceIndex];
            if (scope == RecordScope.Source && plugin.Role != PluginRole.Source)
            {
                continue;
            }

            foreach (var formList in _nativeMods[sourceIndex].FormLists)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (scope == RecordScope.WinningOverrides
                    && HasLaterFormListContext(sourceIndex, formList.FormKey, cancellationToken))
                {
                    continue;
                }

                summaries.Add(new FormListSummary(
                    formList.FormKey,
                    formList.EditorID,
                    readerResult.Value!.CountOverrides(
                        formList.FormKey,
                        typeof(IFormListGetter),
                        cancellationToken),
                    scope,
                    plugin.ModKey,
                    plugin.Path,
                    plugin.LoadOrderIndex,
                    plugin.Role));
            }
        }

        return EngineResult<IReadOnlyList<FormListSummary>>.Success(
            Array.AsReadOnly(summaries.ToArray()),
            workspaceId: WorkspaceId,
            resultRevision: Revision);
    }

    /// <summary>Reads one exact Fallout 4 FormList context as an independent native copy.</summary>
    /// <param name="request">The native FormList identity, scope, and optional containing-plugin selection.</param>
    /// <param name="cancellationToken">A token observed during selection, copying, and direct-link validation.</param>
    /// <returns>The contextual detached read with provenance and missing-reference warnings, or a typed disposed failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<NativeRecordRead> ReadFormListContext(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var readerResult = GetReferenceReader();
        if (!readerResult.Succeeded)
        {
            return EngineResult<NativeRecordRead>.Failure(
                readerResult.Error!,
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        var readResult = readerResult.Value!.Read(request, cancellationToken);
        if (!readResult.Succeeded)
        {
            return EngineResult<NativeRecordRead>.Failure(
                readResult.Error!,
                workspaceId: WorkspaceId,
                resultRevision: Revision,
                warnings: readResult.Warnings);
        }

        var read = readResult.Value!;
        if (read.Record is not null && read.Record is not IFormListGetter)
        {
            read = CreateUnsupportedFormListRead(read);
        }

        return EngineResult<NativeRecordRead>.Success(
            read,
            workspaceId: WorkspaceId,
            resultRevision: Revision,
            warnings: readResult.Warnings);
    }

    /// <summary>
    /// Resolves one native reference and returns a detached getter that cannot mutate the loaded sources.
    /// </summary>
    /// <param name="request">The native identity and source scope to resolve.</param>
    /// <param name="cancellationToken">A token that cancels the bounded native scan.</param>
    /// <returns>The native resolution status and detached record when resolved.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<ReferenceResolution> Resolve(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var readerResult = GetReferenceReader();
        if (!readerResult.Succeeded)
        {
            return EngineResult<ReferenceResolution>.Failure(
                readerResult.Error!,
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        return BindResult(readerResult.Value!.Resolve(request, cancellationToken));
    }

    /// <summary>
    /// Searches native references in deterministic bounded pages tied to this source baseline.
    /// </summary>
    /// <param name="request">The query, scope, page size, and optional continuation token.</param>
    /// <param name="cancellationToken">A token that cancels the bounded native scan.</param>
    /// <returns>One deterministic page of native references or a stable token/request failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<ReferenceSearchPage> Search(
        ReferenceSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var readerResult = GetReferenceReader();
        if (!readerResult.Succeeded)
        {
            return EngineResult<ReferenceSearchPage>.Failure(
                readerResult.Error!,
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        return BindResult(readerResult.Value!.Search(request, WorkspaceId, Revision, cancellationToken));
    }

    /// <summary>
    /// Revalidates every source and localized-string artifact against the baseline established during open.
    /// </summary>
    /// <param name="cancellationToken">A token that cancels source verification.</param>
    /// <returns>The unchanged baseline or an external-change failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public Task<EngineResult<NativeSourceInputBaseline>> VerifyUnchangedAsync(
        CancellationToken cancellationToken = default)
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            return Task.FromResult(EngineResult<NativeSourceInputBaseline>.Failure(
                new EngineError(EngineErrorCode.WorkspaceDisposed, "The Fallout 4 native source lifetime has been disposed."),
                workspaceId: WorkspaceId,
                resultRevision: Revision));
        }

        return _inputs.VerifyUnchangedAsync(cancellationToken);
    }

    /// <summary>
    /// Borrows the validated native source inputs for same-assembly output preparation.
    /// </summary>
    /// <returns>The source inputs owned by this source-set lifetime.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this source set has been disposed.</exception>
    internal NativeSourceInputs BorrowInputs()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        return _inputs;
    }

    /// <summary>
    /// Gets the fully materialized native plugins for same-assembly game-adapter composition.
    /// </summary>
    /// <returns>The native plugins in explicit load-order order.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this source set has been disposed.</exception>
    internal IReadOnlyList<IFallout4ModGetter> GetNativeMods()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        return _nativeMods;
    }

    /// <summary>
    /// Releases native source handles and localized-string lookups while closing every public source operation.
    /// </summary>
    /// <returns>A task that completes after the shared input lifetime is released.</returns>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        await _inputs.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the borrowed shared reference reader or a typed disposed failure.
    /// </summary>
    /// <returns>The shared reader while this source lifetime remains active.</returns>
    private EngineResult<NativeReferenceReader> GetReferenceReader()
    {
        return Volatile.Read(ref _disposed) != 0
            ? EngineResult<NativeReferenceReader>.Failure(
                new EngineError(EngineErrorCode.WorkspaceDisposed, "The Fallout 4 native source lifetime has been disposed."),
                workspaceId: WorkspaceId,
                resultRevision: Revision)
            : EngineResult<NativeReferenceReader>.Success(_referenceReader);
    }

    /// <summary>Checks later typed FormList groups for an overriding or deleting context.</summary>
    /// <param name="sourceIndex">The containing plugin index of the candidate context.</param>
    /// <param name="formKey">The native origin identity shared by the override chain.</param>
    /// <param name="cancellationToken">A token observed throughout later native group traversal.</param>
    /// <returns><see langword="true"/> when a later typed FormList context exists.</returns>
    private bool HasLaterFormListContext(
        int sourceIndex,
        Mutagen.Bethesda.Plugins.FormKey formKey,
        CancellationToken cancellationToken)
    {
        for (var laterIndex = sourceIndex + 1; laterIndex < _nativeMods.Count; laterIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var laterFormList in _nativeMods[laterIndex].FormLists)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (laterFormList.FormKey == formKey)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>Enumerates non-null Fallout 4 FormList item targets with exact typed field positions.</summary>
    /// <param name="record">The borrowed native record selected for missing-reference inspection.</param>
    /// <returns>Immutable real FormList targets with exact item positions; native null sentinels and other record families are omitted.</returns>
    private static IReadOnlyList<NativeFormLinkReference> EnumerateFormListLinks(
        Mutagen.Bethesda.Plugins.Records.IMajorRecordGetter record)
    {
        if (record is not IFormListGetter formList)
        {
            return Array.Empty<NativeFormLinkReference>();
        }

        var references = new List<NativeFormLinkReference>();
        for (var index = 0; index < formList.Items.Count; index++)
        {
            if (formList.Items[index].FormKeyNullable is { } formKey
                && !formKey.IsNull)
            {
                references.Add(new NativeFormLinkReference(formKey, $"Items[{index}]"));
            }
        }

        return Array.AsReadOnly(references.ToArray());
    }

    /// <summary>Converts a selected non-FormList context into an explicit unsupported FormList read.</summary>
    /// <param name="read">The contextual native read whose selected record belongs to another family.</param>
    /// <returns>An unsupported read retaining the exact selection and containing-plugin provenance.</returns>
    private static NativeRecordRead CreateUnsupportedFormListRead(NativeRecordRead read)
    {
        var context = read.Context;
        return new NativeRecordRead(
            new FormListContext(
                context.Selection,
                ReferenceResolutionStatus.Unsupported,
                context.ContainingModKey,
                context.Path,
                context.LoadOrderIndex,
                context.Role),
            read.RecordType,
            null);
    }

    /// <summary>
    /// Binds a shared reference result to the workspace and immutable source revision that produced it.
    /// </summary>
    /// <typeparam name="T">The successful reference value type.</typeparam>
    /// <param name="result">The shared reference result to bind.</param>
    /// <returns>An equivalent result carrying this source set's workspace and revision metadata.</returns>
    private EngineResult<T> BindResult<T>(EngineResult<T> result)
    {
        return result.Succeeded
            ? EngineResult<T>.Success(
                result.Value!,
                workspaceId: WorkspaceId,
                resultRevision: Revision,
                warnings: result.Warnings)
            : EngineResult<T>.Failure(
                result.Error!,
                workspaceId: WorkspaceId,
                resultRevision: Revision,
                warnings: result.Warnings);
    }
}
