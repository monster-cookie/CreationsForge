using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.PresentationTests.Support;

/// <summary>Supplies programmable native reference search and resolution behavior to picker presentation tests.</summary>
internal sealed class ReferencePickerTestWorkspace : IFormListWorkspace
{
    /// <summary>Initializes a reference picker test workspace.</summary>
    /// <param name="workspaceId">The non-empty workspace identity.</param>
    /// <param name="revision">The exact test revision.</param>
    public ReferencePickerTestWorkspace(Guid workspaceId, WorkspaceRevision revision)
    {
        WorkspaceId = workspaceId;
        Revision = revision;
    }

    /// <inheritdoc />
    public Guid WorkspaceId { get; }

    /// <inheritdoc />
    public WorkspaceRevision Revision { get; set; }

    /// <inheritdoc />
    public OutputSynchronizationState OutputSynchronization { get; } = new(OutputSynchronizationStatus.Ready, null);

    /// <summary>Gets or sets the callback used for native reference searches.</summary>
    public Func<ReferenceSearchRequest, CancellationToken, ValueTask<EngineResult<ReferenceSearchPage>>> SearchAction { get; set; } =
        (_, _) => throw new NotSupportedException("No reference search behavior was configured.");

    /// <summary>Gets or sets the callback used for exact native reference resolution.</summary>
    public Func<ReferenceRequest, CancellationToken, ValueTask<EngineResult<ReferenceResolution>>> ResolveAction { get; set; } =
        (_, _) => throw new NotSupportedException("No reference resolution behavior was configured.");

    /// <summary>Gets the ordered reference search requests received by this workspace.</summary>
    public List<ReferenceSearchRequest> SearchRequests { get; } = [];

    /// <summary>Gets the ordered exact reference requests received by this workspace.</summary>
    public List<ReferenceRequest> ResolveRequests { get; } = [];

    /// <inheritdoc />
    public ValueTask<EngineResult<ReferenceSearchPage>> SearchReferencesAsync(
        ReferenceSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        SearchRequests.Add(request);
        return SearchAction(request, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<ReferenceResolution>> ResolveReferenceAsync(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        ResolveRequests.Add(request);
        return ResolveAction(request, cancellationToken);
    }

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
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<IReadOnlyList<PluginSummary>>> ListPluginsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<IReadOnlyList<FormListSummary>>> ListFormListsAsync(
        RecordScope scope,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<IMajorRecordGetter>> ReadFormListAsync(
        FormKey formKey,
        RecordScope scope,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<FormListReadView>> ReadFormListViewAsync(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<EditReceipt>> BeginEditAsync(
        BeginEditRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OperationReceipt>> ApplyFormListEditAsync(
        FormListEditRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<FormListComparison>> CompareFormListAsync(
        CompareFormListRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<WorkspacePreview>> PreviewAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<SaveResult> SaveAsync(
        SaveRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OutputSelectionReceipt>> ResolveOutputRecoveryAsync(
        ResolveOutputRecoveryRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OutputSelectionReceipt>> ReopenOutputAsync(
        ReopenOutputRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OperationReceipt>> DiscardChangesAsync(
        DiscardChangesRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
