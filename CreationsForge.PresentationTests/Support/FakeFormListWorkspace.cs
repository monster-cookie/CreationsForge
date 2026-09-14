using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.PresentationTests.Support;

/// <summary>
/// Supplies deterministic output selection and disposal while rejecting unrelated engine operations in presentation tests.
/// </summary>
internal sealed class FakeFormListWorkspace : IFormListWorkspace
{
    /// <summary>The callback that supplies output-selection behavior.</summary>
    private readonly Func<SelectOutputRequest, CancellationToken, ValueTask<EngineResult<OutputSelectionReceipt>>> SelectOutputAction;

    /// <summary>Initializes a test workspace.</summary>
    /// <param name="workspaceId">The deterministic workspace identity.</param>
    /// <param name="revision">The initial workspace revision.</param>
    /// <param name="selectOutputAction">The callback that supplies output-selection behavior.</param>
    public FakeFormListWorkspace(
        Guid workspaceId,
        WorkspaceRevision revision,
        Func<SelectOutputRequest, CancellationToken, ValueTask<EngineResult<OutputSelectionReceipt>>> selectOutputAction)
    {
        WorkspaceId = workspaceId;
        Revision = revision;
        SelectOutputAction = selectOutputAction;
    }

    /// <inheritdoc />
    public Guid WorkspaceId { get; }

    /// <inheritdoc />
    public WorkspaceRevision Revision { get; }

    /// <summary>Gets the ready state used by presentation tests that do not perform plugin saves.</summary>
    public OutputSynchronizationState OutputSynchronization { get; } = new(OutputSynchronizationStatus.Ready, null);

    /// <summary>Gets the last output-selection request received by the fake.</summary>
    public SelectOutputRequest? LastSelectOutputRequest { get; private set; }

    /// <summary>Gets how many times this workspace was disposed.</summary>
    public int DisposeCount { get; private set; }

    /// <inheritdoc />
    public ValueTask<EngineResult<WorkspaceState>> ReadStateAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OutputSelectionReceipt>> SelectOutputAsync(
        SelectOutputRequest request,
        CancellationToken cancellationToken = default)
    {
        LastSelectOutputRequest = request;
        return SelectOutputAction(request, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<IReadOnlyList<PluginSummary>>> ListPluginsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<IReadOnlyList<FormListSummary>>> ListFormListsAsync(RecordScope scope, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<IMajorRecordGetter>> ReadFormListAsync(FormKey formKey, RecordScope scope, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<FormListReadView>> ReadFormListViewAsync(ReferenceRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<ReferenceSearchPage>> SearchReferencesAsync(ReferenceSearchRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<ReferenceResolution>> ResolveReferenceAsync(ReferenceRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<EditReceipt>> BeginEditAsync(BeginEditRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OperationReceipt>> ApplyFormListEditAsync(FormListEditRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<FormListComparison>> CompareFormListAsync(CompareFormListRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<WorkspacePreview>> PreviewAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<SaveResult> SaveAsync(SaveRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OutputSelectionReceipt>> ReopenOutputAsync(ReopenOutputRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OperationReceipt>> DiscardChangesAsync(DiscardChangesRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <summary>Rejects recovery adoption because this fake only models output selection and disposal.</summary>
    /// <param name="request">The unsupported recovery-adoption request.</param>
    /// <param name="cancellationToken">The unused cancellation token.</param>
    /// <returns>This method always throws.</returns>
    /// <exception cref="NotSupportedException">Thrown because recovery adoption is outside this fake's scope.</exception>
    public ValueTask<EngineResult<OutputSelectionReceipt>> ResolveOutputRecoveryAsync(
        ResolveOutputRecoveryRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        DisposeCount++;
        return ValueTask.CompletedTask;
    }
}
