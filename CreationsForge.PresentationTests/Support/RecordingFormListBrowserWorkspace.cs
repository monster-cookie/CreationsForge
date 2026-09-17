using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.PresentationTests.Support;

/// <summary>
/// Supplies configurable plugin browser reads while rejecting mutation and persistence operations outside browser tests.
/// </summary>
internal sealed class RecordingFormListBrowserWorkspace : IPluginWorkspace
{
    /// <summary>The callback that supplies atomic workspace state.</summary>
    private readonly Func<CancellationToken, ValueTask<EngineResult<WorkspaceState>>> ReadStateAction;

    /// <summary>The callback that supplies participating plugins.</summary>
    private readonly Func<CancellationToken, ValueTask<EngineResult<IReadOnlyList<PluginSummary>>>> ListPluginsAction;

    /// <summary>The callback that supplies FormList enumeration.</summary>
    private readonly Func<RecordScope, CancellationToken, ValueTask<EngineResult<IReadOnlyList<FormListSummary>>>> ListFormListsAction;

    /// <summary>The callback that supplies exact context comparison.</summary>
    private readonly Func<CompareFormListRequest, CancellationToken, ValueTask<EngineResult<FormListComparison>>> CompareAction;

    /// <summary>The optional callback that supplies bounded major-record pages.</summary>
    private readonly Func<MajorRecordListRequest, CancellationToken, ValueTask<EngineResult<MajorRecordListPage>>>? ListMajorRecordsAction;

    /// <summary>The optional callback that supplies exact context search pages.</summary>
    private readonly Func<ReferenceSearchRequest, CancellationToken, ValueTask<EngineResult<ReferenceSearchPage>>>? SearchReferencesAction;

    /// <summary>The optional callback that supplies native major-record comparisons.</summary>
    private readonly Func<CompareMajorRecordRequest, CancellationToken, ValueTask<EngineResult<MajorRecordComparison>>>? CompareMajorRecordAction;

    /// <summary>Initializes configurable browser reads for one deterministic workspace.</summary>
    /// <param name="workspaceId">The non-empty workspace identity.</param>
    /// <param name="revision">The current exact workspace revision.</param>
    /// <param name="readStateAction">The callback that supplies atomic workspace state.</param>
    /// <param name="listPluginsAction">The callback that supplies participating plugins.</param>
    /// <param name="listFormListsAction">The callback that supplies FormList enumeration.</param>
    /// <param name="compareAction">The callback that supplies exact context comparison.</param>
    /// <param name="listMajorRecordsAction">An optional callback that supplies bounded major-record pages.</param>
    /// <param name="searchReferencesAction">An optional callback that supplies exact context search pages.</param>
    /// <param name="compareMajorRecordAction">An optional callback that supplies native major-record comparisons.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="workspaceId"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when any callback is <see langword="null"/>.</exception>
    public RecordingFormListBrowserWorkspace(
        Guid workspaceId,
        WorkspaceRevision revision,
        Func<CancellationToken, ValueTask<EngineResult<WorkspaceState>>> readStateAction,
        Func<CancellationToken, ValueTask<EngineResult<IReadOnlyList<PluginSummary>>>> listPluginsAction,
        Func<RecordScope, CancellationToken, ValueTask<EngineResult<IReadOnlyList<FormListSummary>>>> listFormListsAction,
        Func<CompareFormListRequest, CancellationToken, ValueTask<EngineResult<FormListComparison>>> compareAction,
        Func<MajorRecordListRequest, CancellationToken, ValueTask<EngineResult<MajorRecordListPage>>>? listMajorRecordsAction = null,
        Func<ReferenceSearchRequest, CancellationToken, ValueTask<EngineResult<ReferenceSearchPage>>>? searchReferencesAction = null,
        Func<CompareMajorRecordRequest, CancellationToken, ValueTask<EngineResult<MajorRecordComparison>>>? compareMajorRecordAction = null)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("A browser test workspace requires a non-empty identity.", nameof(workspaceId));
        }

        ArgumentNullException.ThrowIfNull(readStateAction);
        ArgumentNullException.ThrowIfNull(listPluginsAction);
        ArgumentNullException.ThrowIfNull(listFormListsAction);
        ArgumentNullException.ThrowIfNull(compareAction);
        WorkspaceId = workspaceId;
        Revision = revision;
        ReadStateAction = readStateAction;
        ListPluginsAction = listPluginsAction;
        ListFormListsAction = listFormListsAction;
        CompareAction = compareAction;
        ListMajorRecordsAction = listMajorRecordsAction;
        SearchReferencesAction = searchReferencesAction;
        CompareMajorRecordAction = compareMajorRecordAction;
    }

    /// <inheritdoc />
    public Guid WorkspaceId { get; }

    /// <inheritdoc />
    public WorkspaceRevision Revision { get; }

    /// <summary>Gets the ready synchronization state used by browser-only tests.</summary>
    public OutputSynchronizationState OutputSynchronization { get; } = new(OutputSynchronizationStatus.Ready, null);

    /// <summary>Gets state-read cancellation tokens in call order.</summary>
    public List<CancellationToken> StateTokens { get; } = [];

    /// <summary>Gets FormList enumeration scopes in call order.</summary>
    public List<RecordScope> ListScopes { get; } = [];

    /// <summary>Gets comparison requests in call order.</summary>
    public List<CompareFormListRequest> ComparisonRequests { get; } = [];

    /// <summary>Gets how many times disposal was requested.</summary>
    public int DisposeCount { get; private set; }

    /// <inheritdoc />
    public ValueTask<EngineResult<WorkspaceState>> ReadStateAsync(CancellationToken cancellationToken = default)
    {
        StateTokens.Add(cancellationToken);
        return ReadStateAction(cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<IReadOnlyList<PluginSummary>>> ListPluginsAsync(CancellationToken cancellationToken = default)
    {
        return ListPluginsAction(cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<IReadOnlyList<FormListSummary>>> ListFormListsAsync(
        RecordScope scope,
        CancellationToken cancellationToken = default)
    {
        ListScopes.Add(scope);
        return ListFormListsAction(scope, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<FormListComparison>> CompareFormListAsync(
        CompareFormListRequest request,
        CancellationToken cancellationToken = default)
    {
        ComparisonRequests.Add(request);
        return CompareAction(request, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OutputSelectionReceipt>> SelectOutputAsync(
        SelectOutputRequest request,
        CancellationToken cancellationToken = default)
    {
        throw Unsupported();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<IMajorRecordGetter>> ReadFormListAsync(
        FormKey formKey,
        RecordScope scope,
        CancellationToken cancellationToken = default)
    {
        throw Unsupported();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<FormListReadView>> ReadFormListViewAsync(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        throw Unsupported();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<ReferenceSearchPage>> SearchReferencesAsync(
        ReferenceSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        return SearchReferencesAction is null
            ? throw Unsupported()
            : SearchReferencesAction(request, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<MajorRecordListPage>> ListMajorRecordsAsync(
        MajorRecordListRequest request,
        CancellationToken cancellationToken = default)
    {
        return ListMajorRecordsAction is null
            ? throw Unsupported()
            : ListMajorRecordsAction(request, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<MajorRecordComparison>> CompareMajorRecordAsync(
        CompareMajorRecordRequest request,
        CancellationToken cancellationToken = default)
    {
        return CompareMajorRecordAction is null
            ? throw Unsupported()
            : CompareMajorRecordAction(request, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<ReferenceResolution>> ResolveReferenceAsync(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        throw Unsupported();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<EditReceipt>> BeginEditAsync(
        BeginEditRequest request,
        CancellationToken cancellationToken = default)
    {
        throw Unsupported();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OperationReceipt>> ApplyFormListEditAsync(
        FormListEditRequest request,
        CancellationToken cancellationToken = default)
    {
        throw Unsupported();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<WorkspacePreview>> PreviewAsync(CancellationToken cancellationToken = default)
    {
        throw Unsupported();
    }

    /// <inheritdoc />
    public ValueTask<SaveResult> SaveAsync(
        SaveRequest request,
        CancellationToken cancellationToken = default)
    {
        throw Unsupported();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OutputSelectionReceipt>> ResolveOutputRecoveryAsync(
        ResolveOutputRecoveryRequest request,
        CancellationToken cancellationToken = default)
    {
        throw Unsupported();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OutputSelectionReceipt>> ReopenOutputAsync(
        ReopenOutputRequest request,
        CancellationToken cancellationToken = default)
    {
        throw Unsupported();
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OperationReceipt>> DiscardChangesAsync(
        DiscardChangesRequest request,
        CancellationToken cancellationToken = default)
    {
        throw Unsupported();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        DisposeCount++;
        return ValueTask.CompletedTask;
    }

    /// <summary>Creates the stable exception for engine behavior outside browser tests.</summary>
    /// <returns>The unsupported-operation exception.</returns>
    private static NotSupportedException Unsupported()
    {
        return new NotSupportedException("This presentation fake supports FormList browser reads only.");
    }
}
