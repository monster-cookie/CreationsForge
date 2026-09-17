using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.PresentationTests.Support;

/// <summary>Provides deterministic workspace state and records change-lifecycle requests.</summary>
internal sealed class RecordingWorkspaceChangesWorkspace : IPluginWorkspace
{
    /// <summary>Initializes a recording workspace with a ready state and empty preview.</summary>
    /// <param name="workspaceId">The deterministic workspace identity.</param>
    /// <param name="state">The initial atomic workspace state.</param>
    internal RecordingWorkspaceChangesWorkspace(Guid workspaceId, WorkspaceState state)
    {
        WorkspaceId = workspaceId;
        State = state;
        Preview = new WorkspacePreview([], 0, []);
    }

    /// <inheritdoc />
    public Guid WorkspaceId { get; }

    /// <inheritdoc />
    public WorkspaceRevision Revision => State.Revision;

    /// <inheritdoc />
    public OutputSynchronizationState OutputSynchronization => State.OutputSynchronization;

    /// <summary>Gets or sets the state returned by the next ordinary state read.</summary>
    internal WorkspaceState State { get; set; }

    /// <summary>Gets or sets the preview returned by the next ordinary preview.</summary>
    internal WorkspacePreview Preview { get; set; }

    /// <summary>Gets or sets a custom state-read implementation.</summary>
    internal Func<CancellationToken, ValueTask<EngineResult<WorkspaceState>>>? OnReadStateAsync { get; set; }

    /// <summary>Gets or sets a custom preview implementation.</summary>
    internal Func<CancellationToken, ValueTask<EngineResult<WorkspacePreview>>>? OnPreviewAsync { get; set; }

    /// <summary>Gets or sets a custom save implementation.</summary>
    internal Func<SaveRequest, CancellationToken, ValueTask<SaveResult>>? OnSaveAsync { get; set; }

    /// <summary>Gets or sets a custom discard implementation.</summary>
    internal Func<DiscardChangesRequest, CancellationToken, ValueTask<EngineResult<OperationReceipt>>>? OnDiscardAsync { get; set; }

    /// <summary>Gets or sets a custom recovery-adoption implementation.</summary>
    internal Func<ResolveOutputRecoveryRequest, CancellationToken, ValueTask<EngineResult<OutputSelectionReceipt>>>? OnResolveOutputRecoveryAsync { get; set; }

    /// <summary>Gets or sets a custom output-reopen implementation.</summary>
    internal Func<ReopenOutputRequest, CancellationToken, ValueTask<EngineResult<OutputSelectionReceipt>>>? OnReopenOutputAsync { get; set; }

    /// <summary>Gets the number of atomic state reads.</summary>
    internal int ReadStateCount { get; private set; }

    /// <summary>Gets the number of preview reads.</summary>
    internal int PreviewCount { get; private set; }

    /// <summary>Gets the exact save requests in invocation order.</summary>
    internal List<SaveRequest> SaveRequests { get; } = [];

    /// <summary>Gets the exact discard requests in invocation order.</summary>
    internal List<DiscardChangesRequest> DiscardRequests { get; } = [];

    /// <summary>Gets the exact recovery-adoption requests in invocation order.</summary>
    internal List<ResolveOutputRecoveryRequest> ResolveOutputRecoveryRequests { get; } = [];

    /// <summary>Gets the exact reopen requests in invocation order.</summary>
    internal List<ReopenOutputRequest> ReopenOutputRequests { get; } = [];

    /// <summary>Gets the number of disposal requests.</summary>
    internal int DisposeCount { get; private set; }

    /// <inheritdoc />
    public ValueTask<EngineResult<WorkspaceState>> ReadStateAsync(CancellationToken cancellationToken = default)
    {
        ReadStateCount++;
        cancellationToken.ThrowIfCancellationRequested();
        return OnReadStateAsync?.Invoke(cancellationToken)
            ?? ValueTask.FromResult(Success(State));
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<WorkspacePreview>> PreviewAsync(CancellationToken cancellationToken = default)
    {
        PreviewCount++;
        cancellationToken.ThrowIfCancellationRequested();
        return OnPreviewAsync?.Invoke(cancellationToken)
            ?? ValueTask.FromResult(EngineResult<WorkspacePreview>.Success(
                Preview,
                WorkspaceId,
                baseRevision: Revision,
                resultRevision: Revision));
    }

    /// <inheritdoc />
    public ValueTask<SaveResult> SaveAsync(SaveRequest request, CancellationToken cancellationToken = default)
    {
        SaveRequests.Add(request);
        cancellationToken.ThrowIfCancellationRequested();
        return OnSaveAsync?.Invoke(request, cancellationToken)
            ?? ValueTask.FromResult(new SaveResult(
                WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                request.ExpectedRevision,
                SaveCommitStatus.NotCommitted,
                null,
                null,
                null,
                new EngineError(EngineErrorCode.ValidationFailed, "The recording save was not configured."),
                []));
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OperationReceipt>> DiscardChangesAsync(
        DiscardChangesRequest request,
        CancellationToken cancellationToken = default)
    {
        DiscardRequests.Add(request);
        cancellationToken.ThrowIfCancellationRequested();
        return OnDiscardAsync?.Invoke(request, cancellationToken)
            ?? ValueTask.FromResult(EngineResult<OperationReceipt>.Failure(
                new EngineError(EngineErrorCode.ValidationFailed, "The recording discard was not configured."),
                WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                request.ExpectedRevision));
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OutputSelectionReceipt>> ResolveOutputRecoveryAsync(
        ResolveOutputRecoveryRequest request,
        CancellationToken cancellationToken = default)
    {
        ResolveOutputRecoveryRequests.Add(request);
        cancellationToken.ThrowIfCancellationRequested();
        return OnResolveOutputRecoveryAsync?.Invoke(request, cancellationToken)
            ?? ValueTask.FromResult(Unsupported<OutputSelectionReceipt>());
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OutputSelectionReceipt>> ReopenOutputAsync(
        ReopenOutputRequest request,
        CancellationToken cancellationToken = default)
    {
        ReopenOutputRequests.Add(request);
        cancellationToken.ThrowIfCancellationRequested();
        return OnReopenOutputAsync?.Invoke(request, cancellationToken)
            ?? ValueTask.FromResult(Unsupported<OutputSelectionReceipt>());
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OutputSelectionReceipt>> SelectOutputAsync(SelectOutputRequest request, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Unsupported<OutputSelectionReceipt>());

    /// <inheritdoc />
    public ValueTask<EngineResult<IReadOnlyList<PluginSummary>>> ListPluginsAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Unsupported<IReadOnlyList<PluginSummary>>());

    /// <inheritdoc />
    public ValueTask<EngineResult<IReadOnlyList<FormListSummary>>> ListFormListsAsync(RecordScope scope, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Unsupported<IReadOnlyList<FormListSummary>>());

    /// <inheritdoc />
    public ValueTask<EngineResult<IMajorRecordGetter>> ReadFormListAsync(FormKey formKey, RecordScope scope, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Unsupported<IMajorRecordGetter>());

    /// <inheritdoc />
    public ValueTask<EngineResult<FormListReadView>> ReadFormListViewAsync(ReferenceRequest request, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Unsupported<FormListReadView>());

    /// <inheritdoc />
    public ValueTask<EngineResult<ReferenceSearchPage>> SearchReferencesAsync(ReferenceSearchRequest request, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Unsupported<ReferenceSearchPage>());

    /// <inheritdoc />
    public ValueTask<EngineResult<ReferenceResolution>> ResolveReferenceAsync(ReferenceRequest request, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Unsupported<ReferenceResolution>());

    /// <inheritdoc />
    public ValueTask<EngineResult<EditReceipt>> BeginEditAsync(BeginEditRequest request, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Unsupported<EditReceipt>());

    /// <inheritdoc />
    public ValueTask<EngineResult<OperationReceipt>> ApplyFormListEditAsync(FormListEditRequest request, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Unsupported<OperationReceipt>());

    /// <inheritdoc />
    public ValueTask<EngineResult<FormListComparison>> CompareFormListAsync(CompareFormListRequest request, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(Unsupported<FormListComparison>());

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        DisposeCount++;
        return ValueTask.CompletedTask;
    }

    /// <summary>Creates an exact successful state envelope.</summary>
    /// <param name="state">The atomic state value.</param>
    /// <returns>The successful engine result.</returns>
    private EngineResult<WorkspaceState> Success(WorkspaceState state)
        => EngineResult<WorkspaceState>.Success(
            state,
            WorkspaceId,
            baseRevision: state.Revision,
            resultRevision: state.Revision);

    /// <summary>Creates a deterministic failure for an operation outside this fake's change-lifecycle scope.</summary>
    /// <typeparam name="T">The unavailable result value type.</typeparam>
    /// <returns>The typed failure.</returns>
    private EngineResult<T> Unsupported<T>()
        => EngineResult<T>.Failure(
            new EngineError(EngineErrorCode.InvalidRequest, "The recording workspace operation was not configured."),
            WorkspaceId,
            baseRevision: Revision,
            resultRevision: Revision);
}
