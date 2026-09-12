using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.ViewModels;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <summary>Verifies bounded native reference search, exact resolution, cancellation, and stale-state suppression.</summary>
public sealed class NativeReferencePickerViewModelTests
{
    /// <summary>Bounds asynchronous cancellation and dispatcher tests.</summary>
    private static readonly TimeSpan AsyncDeadline = TimeSpan.FromSeconds(5);

    /// <summary>Verifies each explicit page is capped at 100 and replaces the prior visible page.</summary>
    [Fact]
    public async Task SearchAndNextPage_WithContinuation_ReplacesBoundedVisiblePage()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 7);
        var workspace = new ReferencePickerTestWorkspace(workspaceId, revision);
        var firstPage = Enumerable.Range(0, NativeReferencePickerViewModel.PageSize)
            .Select(index => CreateMatch((uint)(0x800 + index), $"Needle{index:D3}"))
            .ToArray();
        var secondPage = new[]
        {
            CreateMatch(0x900, "NeedleNextA"),
            CreateMatch(0x901, "NeedleNextB")
        };
        workspace.SearchAction = (request, _) => ValueTask.FromResult(
            EngineResult<ReferenceSearchPage>.Success(
                new ReferenceSearchPage(request.ContinuationToken is null ? firstPage : secondPage, request.ContinuationToken is null ? "next-page" : null),
                workspaceId: workspaceId,
                resultRevision: revision));
        var dispatcher = new InlineUiDispatcher();
        await using var viewModel = CreateViewModel(workspace, dispatcher);
        viewModel.Query = "  Needle  ";

        await viewModel.SearchAsync();

        workspace.SearchRequests[0].Query.ShouldBe("Needle");
        workspace.SearchRequests[0].MaximumResults.ShouldBe(NativeReferencePickerViewModel.PageSize);
        workspace.SearchRequests[0].ContinuationToken.ShouldBeNull();
        viewModel.Matches.Count.ShouldBe(NativeReferencePickerViewModel.PageSize);
        viewModel.PageNumber.ShouldBe(1);
        viewModel.HasNextPage.ShouldBeTrue();

        await viewModel.LoadNextPageAsync();

        workspace.SearchRequests.Count.ShouldBe(2);
        workspace.SearchRequests[1].ContinuationToken.ShouldBe("next-page");
        viewModel.Matches.ShouldBe(secondPage);
        viewModel.Matches.ShouldNotContain(firstPage[0]);
        viewModel.PageNumber.ShouldBe(2);
        viewModel.HasNextPage.ShouldBeFalse();
        dispatcher.InvokeCount.ShouldBe(4);
    }

    /// <summary>Verifies a non-FormList record remains selectable only after exact native provenance is re-resolved.</summary>
    [Fact]
    public async Task ConfirmSelectionAsync_WithNonFormListMatch_ReturnsExactProvenance()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 3);
        var match = CreateMatch(0x812, "NonFormListKeyword", recordType: "KYWD");
        var workspace = new ReferencePickerTestWorkspace(workspaceId, revision)
        {
            SearchAction = (_, _) => ValueTask.FromResult(SuccessfulPage(workspaceId, revision, [match], null)),
            ResolveAction = (_, _) => ValueTask.FromResult(SuccessfulResolution(workspaceId, revision, match))
        };
        await using var viewModel = CreateViewModel(
            workspace,
            new InlineUiDispatcher(),
            recordScope: RecordScope.AllContexts,
            containingModKey: match.ContainingModKey);
        viewModel.Query = "keyword";
        await viewModel.SearchAsync();
        viewModel.SelectedMatch = match;

        var selection = await viewModel.ConfirmSelectionAsync();

        selection.ShouldNotBeNull();
        selection.IsNull.ShouldBeFalse();
        selection.WorkspaceId.ShouldBe(workspaceId);
        selection.Revision.ShouldBe(revision);
        selection.Match.ShouldBeSameAs(match);
        selection.Match!.RecordType.ShouldBe("KYWD");
        selection.Match.ContainingModKey.ShouldBe(match.ContainingModKey);
        selection.Match.SourcePath.ShouldBe(match.SourcePath);
        selection.Match.LoadOrderIndex.ShouldBe(match.LoadOrderIndex);
        selection.Match.Role.ShouldBe(match.Role);
        viewModel.ResolutionStatus.ShouldBe(ReferenceResolutionStatus.Resolved);
        workspace.SearchRequests.Single().Scope.ShouldBe(RecordScope.AllContexts);
        workspace.SearchRequests.Single().ContainingModKey.ShouldBe(match.ContainingModKey);
        workspace.ResolveRequests.Single().FormKey.ShouldBe(match.FormKey);
        workspace.ResolveRequests.Single().Scope.ShouldBe(RecordScope.AllContexts);
        workspace.ResolveRequests.Single().ContainingModKey.ShouldBe(match.ContainingModKey);
    }

    /// <summary>Verifies changed source provenance prevents a stale search row from being returned.</summary>
    [Fact]
    public async Task ConfirmSelectionAsync_WhenProvenanceChanges_BlocksSelectionAndRequiresRefresh()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 4);
        var match = CreateMatch(0x813, "MovedKeyword", recordType: "KYWD");
        var moved = new ReferenceSearchMatch(
            match.FormKey,
            match.RecordType,
            match.EditorId,
            match.ContainingModKey,
            Path.Combine(Path.GetTempPath(), "CreationsForge-PresentationTests", "Moved.esm"),
            match.LoadOrderIndex,
            match.Role,
            match.IsDeleted);
        var workspace = new ReferencePickerTestWorkspace(workspaceId, revision)
        {
            SearchAction = (_, _) => ValueTask.FromResult(SuccessfulPage(workspaceId, revision, [match], null)),
            ResolveAction = (_, _) => ValueTask.FromResult(SuccessfulResolution(workspaceId, revision, moved))
        };
        await using var viewModel = CreateViewModel(workspace, new InlineUiDispatcher());
        viewModel.Query = "Moved";
        await viewModel.SearchAsync();
        viewModel.SelectedMatch = match;

        var selection = await viewModel.ConfirmSelectionAsync();

        selection.ShouldBeNull();
        viewModel.RequiresRefresh.ShouldBeTrue();
        viewModel.ErrorText.ShouldNotBeNull().ShouldContain("provenance changed");
        viewModel.IsBusy.ShouldBeFalse();
    }

    /// <summary>Verifies each unavailable native resolution status is preserved instead of returning a record.</summary>
    /// <param name="status">The explicit engine resolution status.</param>
    [Theory]
    [InlineData(ReferenceResolutionStatus.Unresolved)]
    [InlineData(ReferenceResolutionStatus.Deleted)]
    [InlineData(ReferenceResolutionStatus.Unsupported)]
    [InlineData(ReferenceResolutionStatus.Ambiguous)]
    [InlineData(ReferenceResolutionStatus.UnknownFamily)]
    public async Task ConfirmSelectionAsync_WithUnavailableResolution_PreservesStatusAndRejectsSelection(
        ReferenceResolutionStatus status)
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 5);
        var match = CreateMatch(0x814, "UnavailableKeyword", recordType: "KYWD", isDeleted: status == ReferenceResolutionStatus.Deleted);
        var workspace = new ReferencePickerTestWorkspace(workspaceId, revision)
        {
            SearchAction = (_, _) => ValueTask.FromResult(SuccessfulPage(workspaceId, revision, [match], null)),
            ResolveAction = (_, _) => ValueTask.FromResult(
                EngineResult<ReferenceResolution>.Success(
                    new ReferenceResolution(
                        status,
                        match.FormKey,
                        match.RecordType,
                        null,
                        match.ContainingModKey,
                        match.SourcePath,
                        match.LoadOrderIndex,
                        match.Role),
                    workspaceId: workspaceId,
                    resultRevision: revision))
        };
        await using var viewModel = CreateViewModel(workspace, new InlineUiDispatcher());
        viewModel.Query = "Unavailable";
        await viewModel.SearchAsync();
        viewModel.SelectedMatch = match;

        var selection = await viewModel.ConfirmSelectionAsync();

        selection.ShouldBeNull();
        viewModel.ResolutionStatus.ShouldBe(status);
        viewModel.ErrorText.ShouldNotBeNull().ShouldContain(status.ToString());
        viewModel.IsBusy.ShouldBeFalse();
    }

    /// <summary>Verifies an allowed null is a successful explicit selection distinct from modal cancellation.</summary>
    [Fact]
    public async Task SelectNullAsync_WhenAllowed_ReturnsExplicitNullWithExactRevision()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 6);
        var workspace = new ReferencePickerTestWorkspace(workspaceId, revision);
        await using var viewModel = CreateViewModel(workspace, new InlineUiDispatcher(), allowNull: true);

        var selection = await viewModel.SelectNullAsync();

        selection.ShouldNotBeNull();
        selection.IsNull.ShouldBeTrue();
        selection.Match.ShouldBeNull();
        selection.WorkspaceId.ShouldBe(workspaceId);
        selection.Revision.ShouldBe(revision);
        workspace.SearchRequests.ShouldBeEmpty();
        workspace.ResolveRequests.ShouldBeEmpty();
    }

    /// <summary>Verifies query invalidation cancels and drains the active native search before clearing busy state.</summary>
    [Fact]
    public async Task QueryChange_DuringSearch_CancelsAndDrainsCurrentOperation()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 8);
        var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var cancellationObserved = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var workspace = new ReferencePickerTestWorkspace(workspaceId, revision)
        {
            SearchAction = async (_, cancellationToken) =>
            {
                started.TrySetResult(true);
                try
                {
                    await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
                    throw new InvalidOperationException("The blocked search completed without cancellation.");
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    cancellationObserved.TrySetResult(true);
                    throw;
                }
            }
        };
        await using var viewModel = CreateViewModel(workspace, new InlineUiDispatcher());
        viewModel.Query = "first";
        var searchTask = viewModel.SearchAsync();
        await started.Task.WaitAsync(AsyncDeadline);

        viewModel.Query = "second";
        await cancellationObserved.Task.WaitAsync(AsyncDeadline);
        await viewModel.CancelAndDrainAsync().WaitAsync(AsyncDeadline);
        await searchTask.WaitAsync(AsyncDeadline);

        viewModel.IsBusy.ShouldBeFalse();
        viewModel.Matches.ShouldBeEmpty();
        viewModel.PageNumber.ShouldBe(0);
        workspace.SearchRequests.Count.ShouldBe(1);
    }

    /// <summary>Verifies a result queued before a query generation change cannot publish stale matches.</summary>
    [Fact]
    public async Task SearchAsync_WhenQueryChangesBeforeDispatch_SuppressesStaleResults()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 9);
        var staleMatch = CreateMatch(0x815, "StaleKeyword", recordType: "KYWD");
        var workspace = new ReferencePickerTestWorkspace(workspaceId, revision)
        {
            SearchAction = (_, _) => ValueTask.FromResult(SuccessfulPage(workspaceId, revision, [staleMatch], null))
        };
        var dispatcher = new QueuedUiDispatcher();
        await using var viewModel = CreateViewModel(workspace, dispatcher);
        viewModel.Query = "stale";
        var searchTask = viewModel.SearchAsync();
        await dispatcher.WaitForInvocationAsync().WaitAsync(AsyncDeadline);

        viewModel.Query = "fresh";
        dispatcher.RunNextInvocation();
        await dispatcher.WaitForInvocationAsync().WaitAsync(AsyncDeadline);
        dispatcher.RunNextInvocation();
        await searchTask.WaitAsync(AsyncDeadline);

        viewModel.Matches.ShouldBeEmpty();
        viewModel.PageNumber.ShouldBe(0);
        viewModel.IsBusy.ShouldBeFalse();
        viewModel.CanSearch.ShouldBeTrue();
    }

    /// <summary>Verifies cancellation invalidates a selection whose UI publication was already queued.</summary>
    [Fact]
    public async Task CancelAndDrainAsync_WithQueuedResolutionPublication_PreventsSelectionReturn()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 13);
        var match = CreateMatch(0x818, "QueuedKeyword", recordType: "KYWD");
        var workspace = new ReferencePickerTestWorkspace(workspaceId, revision)
        {
            ResolveAction = (_, _) => ValueTask.FromResult(SuccessfulResolution(workspaceId, revision, match))
        };
        var dispatcher = new QueuedUiDispatcher();
        await using var viewModel = CreateViewModel(workspace, dispatcher);
        viewModel.SelectedMatch = match;
        var selectionTask = viewModel.ConfirmSelectionAsync();
        await dispatcher.WaitForInvocationAsync().WaitAsync(AsyncDeadline);

        var drainTask = viewModel.CancelAndDrainAsync();
        dispatcher.RunNextInvocation();
        await dispatcher.WaitForInvocationAsync().WaitAsync(AsyncDeadline);
        dispatcher.RunNextInvocation();

        (await selectionTask.WaitAsync(AsyncDeadline)).ShouldBeNull();
        await drainTask.WaitAsync(AsyncDeadline);
        viewModel.IsBusy.ShouldBeFalse();
    }

    /// <summary>Verifies an engine result from another revision is rejected and permanently invalidates the picker.</summary>
    [Fact]
    public async Task SearchAsync_WithChangedResultRevision_RequiresRefreshWithoutPublishingMatches()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 10);
        var workspace = new ReferencePickerTestWorkspace(workspaceId, revision)
        {
            SearchAction = (_, _) => ValueTask.FromResult(SuccessfulPage(
                workspaceId,
                revision.Next(),
                [CreateMatch(0x816, "ChangedRevision")],
                null))
        };
        await using var viewModel = CreateViewModel(workspace, new InlineUiDispatcher());
        viewModel.Query = "Changed";

        await viewModel.SearchAsync();

        viewModel.Matches.ShouldBeEmpty();
        viewModel.RequiresRefresh.ShouldBeTrue();
        viewModel.ErrorText.ShouldNotBeNull().ShouldContain("workspace or revision changed");
        viewModel.IsBusy.ShouldBeFalse();
    }

    /// <summary>Verifies presentation refuses an engine response that exceeds the fixed visible page bound.</summary>
    [Fact]
    public async Task SearchAsync_WhenEngineExceedsPageBound_RejectsThePage()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 12);
        var oversizedPage = Enumerable.Range(0, NativeReferencePickerViewModel.PageSize + 1)
            .Select(index => CreateMatch((uint)(0xA00 + index), $"Oversized{index:D3}"))
            .ToArray();
        var workspace = new ReferencePickerTestWorkspace(workspaceId, revision)
        {
            SearchAction = (_, _) => ValueTask.FromResult(SuccessfulPage(workspaceId, revision, oversizedPage, null))
        };
        await using var viewModel = CreateViewModel(workspace, new InlineUiDispatcher());
        viewModel.Query = "Oversized";

        await viewModel.SearchAsync();

        viewModel.Matches.ShouldBeEmpty();
        viewModel.ErrorText.ShouldNotBeNull().ShouldContain("bounded 100-record page");
        viewModel.IsBusy.ShouldBeFalse();
    }

    /// <summary>Verifies coordinator replacement invalidates generations and removes prior results from selection.</summary>
    [Fact]
    public async Task WorkspaceReplacement_AfterSearch_ClearsResultsAndRequiresRefresh()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 11);
        var match = CreateMatch(0x817, "PriorWorkspace");
        var workspace = new ReferencePickerTestWorkspace(workspaceId, revision)
        {
            SearchAction = (_, _) => ValueTask.FromResult(SuccessfulPage(workspaceId, revision, [match], null))
        };
        var dispatcher = new InlineUiDispatcher();
        var coordinator = new ReferencePickerTestCoordinator(workspace, CreateDescriptor(workspaceId, revision));
        await using var viewModel = new NativeReferencePickerViewModel(
            coordinator,
            dispatcher,
            new NativeReferencePickerRequest(
                workspaceId,
                revision,
                currentFormKey: null,
                RecordScope.WinningOverrides,
                containingModKey: null,
                allowNull: false,
                "Choose a native record for the test."),
            new LoggerConfiguration().CreateLogger());
        viewModel.Query = "Prior";
        await viewModel.SearchAsync();

        coordinator.PublishWorkspace(null);

        viewModel.RequiresRefresh.ShouldBeTrue();
        viewModel.Matches.ShouldBeEmpty();
        viewModel.SelectedMatch.ShouldBeNull();
        viewModel.CanSearch.ShouldBeFalse();
        viewModel.StatusText.ShouldContain("Workspace changed");
    }

    /// <summary>Creates a picker view model and matching coordinator for one exact test workspace.</summary>
    /// <param name="workspace">The programmable test workspace.</param>
    /// <param name="dispatcher">The deterministic UI dispatcher.</param>
    /// <param name="allowNull">Whether explicit null selection is enabled.</param>
    /// <param name="recordScope">The native contexts used by search and resolution.</param>
    /// <param name="containingModKey">The optional containing-plugin filter.</param>
    /// <returns>The configured picker view model.</returns>
    private static NativeReferencePickerViewModel CreateViewModel(
        ReferencePickerTestWorkspace workspace,
        CreationsForge.Services.Interfaces.IUiDispatcher dispatcher,
        bool allowNull = false,
        RecordScope recordScope = RecordScope.WinningOverrides,
        ModKey? containingModKey = null)
    {
        var descriptor = CreateDescriptor(workspace.WorkspaceId, workspace.Revision);
        var coordinator = new ReferencePickerTestCoordinator(workspace, descriptor);
        return new NativeReferencePickerViewModel(
            coordinator,
            dispatcher,
            new NativeReferencePickerRequest(
                workspace.WorkspaceId,
                workspace.Revision,
                currentFormKey: null,
                recordScope,
                containingModKey,
                allowNull,
                "Choose a native record for the test."),
            new LoggerConfiguration().CreateLogger());
    }

    /// <summary>Creates an immutable active-workspace descriptor without accessing game files.</summary>
    /// <param name="workspaceId">The workspace identity.</param>
    /// <param name="revision">The exact workspace revision.</param>
    /// <returns>The matching active workspace descriptor.</returns>
    internal static NativeWorkspaceDescriptor CreateDescriptor(Guid workspaceId, WorkspaceRevision revision)
    {
        var root = Path.Combine(Path.GetTempPath(), "CreationsForge-PresentationTests", workspaceId.ToString("N"));
        var source = Path.Combine(root, "Source.esm");
        return new NativeWorkspaceDescriptor(
            workspaceId,
            SupportedGame.Starfield,
            GameRelease.Starfield,
            source,
            [source],
            new OutputAssociation(
                Path.Combine(root, "Output.esp"),
                ModKey.FromNameAndExtension("Output.esp"),
                LocalizedOutputMode.Embedded,
                OutputMasterStyle.Full),
            revision);
    }

    /// <summary>Creates one search match with complete deterministic source provenance.</summary>
    /// <param name="formId">The local native form identifier.</param>
    /// <param name="editorId">The native EditorID.</param>
    /// <param name="recordType">The stable native record type.</param>
    /// <param name="isDeleted">Whether the exact source context is deleted.</param>
    /// <returns>The immutable search match.</returns>
    internal static ReferenceSearchMatch CreateMatch(
        uint formId,
        string editorId,
        string recordType = "FLST",
        bool isDeleted = false)
    {
        var modKey = ModKey.FromNameAndExtension("Source.esm");
        return new ReferenceSearchMatch(
            new FormKey(modKey, formId),
            recordType,
            editorId,
            modKey,
            Path.Combine(Path.GetTempPath(), "CreationsForge-PresentationTests", "Source.esm"),
            loadOrderIndex: 2,
            PluginRole.Source,
            isDeleted);
    }

    /// <summary>Creates a successful contextualized native search page.</summary>
    /// <param name="workspaceId">The result workspace identity.</param>
    /// <param name="revision">The unchanged result revision.</param>
    /// <param name="matches">The page matches.</param>
    /// <param name="continuationToken">The next-page token.</param>
    /// <returns>The successful engine result.</returns>
    private static EngineResult<ReferenceSearchPage> SuccessfulPage(
        Guid workspaceId,
        WorkspaceRevision revision,
        IReadOnlyList<ReferenceSearchMatch> matches,
        string? continuationToken)
    {
        return EngineResult<ReferenceSearchPage>.Success(
            new ReferenceSearchPage(matches, continuationToken),
            workspaceId: workspaceId,
            resultRevision: revision);
    }

    /// <summary>Creates a resolved non-FormList result with exact match provenance.</summary>
    /// <param name="workspaceId">The result workspace identity.</param>
    /// <param name="revision">The unchanged result revision.</param>
    /// <param name="match">The search match whose identity and provenance are resolved.</param>
    /// <returns>The successful exact resolution.</returns>
    private static EngineResult<ReferenceResolution> SuccessfulResolution(
        Guid workspaceId,
        WorkspaceRevision revision,
        ReferenceSearchMatch match)
    {
        var record = new Keyword(match.FormKey, StarfieldRelease.Starfield)
        {
            EditorID = match.EditorId
        };
        return EngineResult<ReferenceResolution>.Success(
            new ReferenceResolution(
                ReferenceResolutionStatus.Resolved,
                match.FormKey,
                match.RecordType,
                record,
                match.ContainingModKey,
                match.SourcePath,
                match.LoadOrderIndex,
                match.Role),
            workspaceId: workspaceId,
            resultRevision: revision);
    }
}
