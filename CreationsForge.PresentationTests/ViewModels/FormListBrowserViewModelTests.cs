using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.RecordEditing.Drafts;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <summary>
/// Verifies plugin browser ordering, exact selections, typed outcomes, retry, picker, and generation safety.
/// </summary>
public sealed partial class FormListBrowserViewModelTests
{
    /// <summary>Verifies the browser groups records by plugin and major type, supports FormID and EditorID ordering, preserves context order, and compares the earliest context with the winner.</summary>
    /// <returns>A task that completes after the comparison is published.</returns>
    [Fact]
    public async Task StartAndSelectRecord_AllContexts_PreserveOrderProvenanceAndDetachedComparison()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 4);
        var sourceMod = ModKey.FromNameAndExtension("Source.esm");
        var outputMod = ModKey.FromNameAndExtension("Output.esm");
        var otherMod = ModKey.FromNameAndExtension("Other.esm");
        var firstFormKey = new FormKey(sourceMod, 0x0123);
        var secondFormKey = new FormKey(otherMod, 0x0456);
        var sourcePath = AbsolutePath("Source.esm");
        var outputPath = AbsolutePath("Output.esm");
        var otherPath = AbsolutePath("Other.esm");
        var semanticChange = new SemanticChangeDescriptor("Items", SemanticChangeKind.ItemInserted, afterPosition: 2);
        var workspace = CreateWorkspace(
            workspaceId,
            revision,
            [
                new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source, 8, 8),
                new PluginSummary(otherMod, otherPath, 1, PluginRole.LoadOrder, 5, 4),
                new PluginSummary(outputMod, outputPath, 2, PluginRole.Output, 3, 1)
            ],
            [
                Summary(firstFormKey, "OriginalList", sourceMod, sourcePath, 0, PluginRole.Source, 2),
                Summary(firstFormKey, "IntermediateList", otherMod, otherPath, 1, PluginRole.LoadOrder, 2),
                Summary(secondFormKey, "OtherList", otherMod, otherPath, 1, PluginRole.LoadOrder, 1),
                Summary(firstFormKey, "WinningList", outputMod, outputPath, 2, PluginRole.Output, 2),
                Summary(secondFormKey, "AlphaList", outputMod, outputPath, 2, PluginRole.Output, 1)
            ],
            request => SuccessfulComparison(
                workspaceId,
                revision,
                request,
                sourceMod,
                sourcePath,
                outputMod,
                outputPath,
                [semanticChange]));
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision), workspace);
        using var viewModel = CreateViewModel(coordinator);

        await viewModel.StartAsync();

        workspace.ListScopes.ShouldBe([RecordScope.AllContexts]);
        viewModel.Plugins.Select(plugin => plugin.ModKey).ShouldBe([sourceMod, otherMod, outputMod]);
        viewModel.RecordSortMode.ShouldBe(FormListRecordSortMode.FormId);
        viewModel.Records.Select(record => record.FormKey).ShouldBe([firstFormKey, secondFormKey]);
        viewModel.PluginGroups.Select(group => group.Plugin.ModKey).ShouldBe([sourceMod, otherMod, outputMod]);
        viewModel.PluginGroups[0].Children.ShouldBeEmpty();
        viewModel.PluginGroups[1].Children.ShouldBeEmpty();
        var outputPluginGroup = viewModel.PluginGroups[2];
        outputPluginGroup.IsExpanded.ShouldBeFalse();
        var recordTypeGroup = outputPluginGroup.Children.Cast<RecordTypeGroupViewModel>().ShouldHaveSingleItem();
        recordTypeGroup.Label.ShouldBe("Form Lists (FLST)");
        recordTypeGroup.IsExpanded.ShouldBeTrue();
        recordTypeGroup.Children.Cast<FormListRecordViewModel>()
            .Select(record => record.FormKey)
            .ShouldBe([firstFormKey, secondFormKey]);
        viewModel.RecordSortMode = FormListRecordSortMode.EditorId;
        viewModel.Records.Select(record => record.FormKey).ShouldBe([secondFormKey, firstFormKey]);
        viewModel.PluginGroups[2].Children.Cast<RecordTypeGroupViewModel>().ShouldHaveSingleItem()
            .Children.Cast<FormListRecordViewModel>()
            .Select(record => record.FormKey)
            .ShouldBe([secondFormKey, firstFormKey]);
        viewModel.ActivePluginRecordCountText.ShouldBe("Plugin records: 3");
        viewModel.LoadedRecordCountText.ShouldBe("Unique loaded records: 13");
        viewModel.RecordSortMode = FormListRecordSortMode.FormId;
        var firstRoot = viewModel.Records.Single(record => record.FormKey == firstFormKey);
        firstRoot.PrimaryText.ShouldBe("00000123");
        firstRoot.Context.Selection.Scope.ShouldBe(RecordScope.WinningOverrides);
        firstRoot.EditorId.ShouldBe("WinningList");
        firstRoot.TreeChildren.Count.ShouldBe(3);
        firstRoot.HasChildren.ShouldBeTrue();
        firstRoot.Contexts.Select(context => context.Context.ContainingModKey).ShouldBe([sourceMod, otherMod, outputMod]);
        firstRoot.Contexts.Select(context => context.Context.LoadOrderIndex).ShouldBe([0, 1, 2]);
        firstRoot.Contexts.Select(context => context.Context.Role).ShouldBe([PluginRole.Source, PluginRole.LoadOrder, PluginRole.Output]);
        firstRoot.ContextOptions[0].Label.ShouldBe("Winning override (Output.esm)");
        firstRoot.ContextOptions.Skip(1).Select(option => option.ContainingModKey).ShouldBe([sourceMod, otherMod, outputMod]);

        await viewModel.SelectRecordAsync(firstRoot);

        var winningSelection = viewModel.Selection.ShouldNotBeNull();
        winningSelection.FormKey.ShouldBe(firstFormKey);
        winningSelection.IsStagedOutput.ShouldBeTrue();
        winningSelection.ExactReferenceRequest.ShouldBeNull();
        viewModel.Editor.CanBeginExistingOutput.ShouldBeTrue();
        var request = workspace.ComparisonRequests.ShouldHaveSingleItem();
        request.Before.Scope.ShouldBe(RecordScope.AllContexts);
        request.Before.ContainingModKey.ShouldBe(sourceMod);
        request.After.Scope.ShouldBe(RecordScope.WinningOverrides);
        request.After.ContainingModKey.ShouldBeNull();
        viewModel.SelectedBeforeContext!.ContainingModKey.ShouldBe(sourceMod);
        viewModel.BeforeContext!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        viewModel.AfterContext!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        viewModel.BeforeProvenanceText.ShouldContain(sourceMod.FileName);
        viewModel.AfterProvenanceText.ShouldContain(outputMod.FileName);
        var beforeRoot = viewModel.BeforeFields.ShouldHaveSingleItem();
        var afterRoot = viewModel.AfterFields.ShouldHaveSingleItem();
        beforeRoot.Children.Select(node => node.Name)
            .ShouldBe(["$type", "Items", "Name"]);
        afterRoot.Children.Select(node => node.Name)
            .ShouldBe(["$type", "Items", "Name"]);
        beforeRoot.ComparisonState.ShouldBe(ComparisonFieldState.Conflict);
        afterRoot.ComparisonState.ShouldBe(ComparisonFieldState.WinningOverride);
        beforeRoot.Children.Single(node => node.Name == "$type").ComparisonState
            .ShouldBe(ComparisonFieldState.Identical);
        afterRoot.Children.Single(node => node.Name == "$type").ComparisonState
            .ShouldBe(ComparisonFieldState.Identical);
        beforeRoot.Children.Single(node => node.Name == "Items").ComparisonState
            .ShouldBe(ComparisonFieldState.Conflict);
        afterRoot.Children.Single(node => node.Name == "Items").ComparisonState
            .ShouldBe(ComparisonFieldState.WinningOverride);
        afterRoot.Children.Single(node => node.Name == "Items").Children[^1].ComparisonState
            .ShouldBe(ComparisonFieldState.WinningOverride);
        beforeRoot.Children.Single(node => node.Name == "Name").Children.ShouldHaveSingleItem().ComparisonState
            .ShouldBe(ComparisonFieldState.Conflict);
        afterRoot.Children.Single(node => node.Name == "Name").Children.ShouldHaveSingleItem().ComparisonState
            .ShouldBe(ComparisonFieldState.WinningOverride);
        viewModel.SemanticChanges.ShouldHaveSingleItem().ShouldBeSameAs(semanticChange);
        viewModel.StatusText.ShouldBeEmpty();
        viewModel.HasStatusText.ShouldBeFalse();

        await viewModel.SelectRecordAsync(firstRoot.Contexts[0]);
        var sourceSelection = viewModel.Selection.ShouldNotBeNull();
        sourceSelection.WorkspaceId.ShouldBe(workspaceId);
        sourceSelection.Revision.ShouldBe(revision);
        sourceSelection.FormKey.ShouldBe(firstFormKey);
        sourceSelection.ExactReferenceRequest.ShouldBeSameAs(firstRoot.Contexts[0].Context.Selection);
        sourceSelection.IsStagedOutput.ShouldBeFalse();

        await viewModel.SelectRecordAsync(firstRoot.Contexts[^1]);
        var outputSelection = viewModel.Selection.ShouldNotBeNull();
        outputSelection.WorkspaceId.ShouldBe(workspaceId);
        outputSelection.Revision.ShouldBe(revision);
        outputSelection.FormKey.ShouldBe(firstFormKey);
        outputSelection.ExactReferenceRequest.ShouldBeNull();
        outputSelection.IsStagedOutput.ShouldBeTrue();
    }

    /// <summary>Verifies selecting a visible source-only winning row exposes its exact context for override authoring.</summary>
    /// <returns>A task that completes after the source row is selected.</returns>
    [Fact]
    public async Task SelectWinningSourceRoot_EnablesOverrideWithoutSelectingHiddenContext()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 1);
        var sourceMod = ModKey.FromNameAndExtension("SourceOnly.esm");
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var formKey = new FormKey(sourceMod, 0x0123);
        var workspace = CreateWorkspace(
            workspaceId,
            revision,
            [new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source)],
            [Summary(formKey, "SourceOnlyList", sourceMod, sourcePath, 0, PluginRole.Source, 0)],
            request => SuccessfulComparison(workspaceId, revision, request, sourceMod, sourcePath, sourceMod, sourcePath, []));
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision), workspace);
        using var viewModel = CreateViewModel(coordinator);
        await viewModel.StartAsync();

        var root = viewModel.Records.ShouldHaveSingleItem();
        root.TreeChildren.ShouldHaveSingleItem();
        await viewModel.SelectRecordAsync(root);

        var selection = viewModel.Selection.ShouldNotBeNull();
        selection.ExactReferenceRequest.ShouldBeSameAs(root.Contexts[0].Context.Selection);
        selection.IsStagedOutput.ShouldBeFalse();
        viewModel.Editor.CanBeginOverride.ShouldBeTrue();
        viewModel.Editor.CanBeginExistingOutput.ShouldBeFalse();
    }

    /// <summary>Verifies all reference resolution outcomes remain explicit and only inspectable outcomes carry JSON.</summary>
    /// <param name="status">The engine-reported context outcome.</param>
    /// <returns>A task that completes after the comparison is published.</returns>
    [Theory]
    [InlineData(ReferenceResolutionStatus.Resolved)]
    [InlineData(ReferenceResolutionStatus.Unresolved)]
    [InlineData(ReferenceResolutionStatus.Unsupported)]
    [InlineData(ReferenceResolutionStatus.Deleted)]
    [InlineData(ReferenceResolutionStatus.Ambiguous)]
    [InlineData(ReferenceResolutionStatus.UnknownFamily)]
    public async Task SelectRecord_EveryResolutionStatus_RemainsExplicit(
        ReferenceResolutionStatus status)
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 1);
        var sourceMod = ModKey.FromNameAndExtension("StatusSource.esm");
        var formKey = new FormKey(sourceMod, 0x0100);
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var inspectable = status is ReferenceResolutionStatus.Resolved or ReferenceResolutionStatus.Deleted;
        var workspace = CreateWorkspace(
            workspaceId,
            revision,
            [new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source)],
            [Summary(formKey, "StatusList", sourceMod, sourcePath, 0, PluginRole.Source, 0)],
            request =>
            {
                ModKey? containingModKey = inspectable ? (ModKey?)sourceMod : null;
                var before = new FormListContext(
                    request.Before,
                    status,
                    containingModKey,
                    inspectable ? sourcePath : null,
                    inspectable ? 0 : null,
                    inspectable ? PluginRole.Source : null);
                var after = new FormListContext(
                    request.After,
                    status,
                    containingModKey,
                    inspectable ? sourcePath : null,
                    inspectable ? 0 : null,
                    inspectable ? PluginRole.Source : null);
                JsonElement? json = inspectable
                    ? Element("{\"$type\":\"FormList\",\"Items\":[]}")
                    : null;
                return EngineResult<FormListComparison>.Success(
                    new FormListComparison(before, after, json, json, [], []),
                    workspaceId,
                    resultRevision: revision);
            });
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision), workspace);
        using var viewModel = CreateViewModel(coordinator);
        await viewModel.StartAsync();

        await viewModel.SelectRecordAsync(viewModel.Records.ShouldHaveSingleItem());

        viewModel.BeforeContext!.Status.ShouldBe(status);
        viewModel.AfterContext!.Status.ShouldBe(status);
        viewModel.BeforeProvenanceText.ShouldStartWith(status.ToString());
        viewModel.AfterProvenanceText.ShouldStartWith(status.ToString());
        if (inspectable)
        {
            viewModel.BeforeFields.ShouldHaveSingleItem();
            viewModel.AfterFields.ShouldHaveSingleItem();
        }
        else
        {
            viewModel.BeforeFields.ShouldBeEmpty();
            viewModel.AfterFields.ShouldBeEmpty();
        }
    }

    /// <summary>Verifies a typed load failure is retryable and a successful retry clears its stale failure state.</summary>
    /// <returns>A task that completes after the retry succeeds.</returns>
    [Fact]
    public async Task Retry_LoadFailure_PublishesFreshSuccessfulSnapshot()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 2);
        var sourceMod = ModKey.FromNameAndExtension("RetrySource.esm");
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var formKey = new FormKey(sourceMod, 0x0200);
        var listAttempt = 0;
        var workspace = new RecordingFormListBrowserWorkspace(
            workspaceId,
            revision,
            _ => ValueTask.FromResult(SuccessState(workspaceId, revision)),
            _ => ValueTask.FromResult(EngineResult<IReadOnlyList<PluginSummary>>.Success(
                new[] { new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source) },
                workspaceId,
                resultRevision: revision)),
            (_, _) => ValueTask.FromResult(++listAttempt == 1
                ? EngineResult<IReadOnlyList<FormListSummary>>.Failure(
                    new EngineError(EngineErrorCode.SourceOpenFailed, "The source index is temporarily unavailable."),
                    workspaceId,
                    resultRevision: revision)
                : EngineResult<IReadOnlyList<FormListSummary>>.Success(
                    new[] { Summary(formKey, "RetryList", sourceMod, sourcePath, 0, PluginRole.Source, 0) },
                    workspaceId,
                    resultRevision: revision)),
            (_, _) => throw new NotSupportedException());
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision), workspace);
        using var viewModel = CreateViewModel(coordinator);

        await viewModel.StartAsync();

        viewModel.ErrorCode.ShouldBe(EngineErrorCode.SourceOpenFailed);
        viewModel.ErrorMessage.ShouldBe("The source index is temporarily unavailable.");
        viewModel.HasError.ShouldBeTrue();

        await viewModel.RetryAsync();

        listAttempt.ShouldBe(2);
        viewModel.HasError.ShouldBeFalse();
        viewModel.ErrorCode.ShouldBeNull();
        viewModel.Records.ShouldHaveSingleItem().FormKey.ShouldBe(formKey);
    }

    /// <summary>Verifies a workspace replacement cancels the prior generation and prevents its delayed records from publishing.</summary>
    /// <returns>A task that completes after the replacement generation is visible.</returns>
    [Fact]
    public async Task WorkspaceReplacement_CancelsAndSuppressesStaleLoad()
    {
        var firstWorkspaceId = Guid.NewGuid();
        var secondWorkspaceId = Guid.NewGuid();
        var firstRevision = new WorkspaceRevision(Guid.NewGuid(), 1);
        var secondRevision = new WorkspaceRevision(Guid.NewGuid(), 1);
        var firstStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var firstWorkspace = new RecordingFormListBrowserWorkspace(
            firstWorkspaceId,
            firstRevision,
            async token =>
            {
                firstStarted.TrySetResult();
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
                throw new InvalidOperationException("Canceled plugin state read resumed unexpectedly.");
            },
            _ => throw new InvalidOperationException("The canceled generation must not list plugins."),
            (_, _) => throw new InvalidOperationException("The canceled generation must not list FormLists."),
            (_, _) => throw new NotSupportedException());
        var secondMod = ModKey.FromNameAndExtension("Replacement.esm");
        var secondPath = AbsolutePath(secondMod.FileName);
        var secondFormKey = new FormKey(secondMod, 0x0300);
        var secondWorkspace = CreateWorkspace(
            secondWorkspaceId,
            secondRevision,
            [new PluginSummary(secondMod, secondPath, 0, PluginRole.Source)],
            [Summary(secondFormKey, "ReplacementList", secondMod, secondPath, 0, PluginRole.Source, 0)],
            _ => throw new NotSupportedException());
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(firstWorkspaceId, firstRevision), firstWorkspace);
        using var viewModel = CreateViewModel(coordinator);
        var firstLoad = viewModel.StartAsync();
        await firstStarted.Task;

        coordinator.Publish(CreateDescriptor(secondWorkspaceId, secondRevision), secondWorkspace);
        await WaitUntilAsync(() => viewModel.Records.Count == 1);
        await firstLoad;

        firstWorkspace.StateTokens.ShouldHaveSingleItem().IsCancellationRequested.ShouldBeTrue();
        viewModel.Records.ShouldHaveSingleItem().FormKey.ShouldBe(secondFormKey);
        viewModel.Plugins.ShouldHaveSingleItem().ModKey.ShouldBe(secondMod);
    }

    /// <summary>Verifies close clears detached JSON, contexts, changes, warnings, and selections synchronously.</summary>
    /// <returns>A task that completes after an initial comparison.</returns>
    [Fact]
    public async Task WorkspaceClose_ClearsEveryDerivedRecordAndJsonValue()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 1);
        var sourceMod = ModKey.FromNameAndExtension("CloseSource.esm");
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var formKey = new FormKey(sourceMod, 0x0400);
        var workspace = CreateWorkspace(
            workspaceId,
            revision,
            [new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source)],
            [Summary(formKey, "CloseList", sourceMod, sourcePath, 0, PluginRole.Source, 0)],
            request => SuccessfulComparison(
                workspaceId,
                revision,
                request,
                sourceMod,
                sourcePath,
                sourceMod,
                sourcePath,
                [new SemanticChangeDescriptor("EditorID", SemanticChangeKind.ValueChanged)]));
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision), workspace);
        using var viewModel = CreateViewModel(coordinator);
        await viewModel.StartAsync();
        await viewModel.SelectRecordAsync(viewModel.Records.ShouldHaveSingleItem());
        viewModel.BeforeFields.ShouldNotBeEmpty();

        coordinator.PublishClosed();

        viewModel.Records.ShouldBeEmpty();
        viewModel.Plugins.ShouldBeEmpty();
        viewModel.ContextOptions.ShouldBeEmpty();
        viewModel.SelectedRecord.ShouldBeNull();
        viewModel.BeforeContext.ShouldBeNull();
        viewModel.AfterContext.ShouldBeNull();
        viewModel.BeforeFields.ShouldBeEmpty();
        viewModel.AfterFields.ShouldBeEmpty();
        viewModel.SemanticChanges.ShouldBeEmpty();
        viewModel.Warnings.ShouldBeEmpty();
        viewModel.StatusText.ShouldBe("No workspace is open.");
    }

    /// <summary>Verifies refresh can reselect the exact staged-output context for a newly edited FormList.</summary>
    /// <returns>A task that completes after refresh and comparison.</returns>
    [Fact]
    public async Task Refresh_WithFormKeyAndOutputPreference_ReselectsExactOutputContext()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 8);
        var sourceMod = ModKey.FromNameAndExtension("RefreshSource.esm");
        var outputMod = ModKey.FromNameAndExtension("RefreshOutput.esm");
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var outputPath = AbsolutePath(outputMod.FileName);
        var formKey = new FormKey(sourceMod, 0x0500);
        var workspace = CreateWorkspace(
            workspaceId,
            revision,
            [
                new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source),
                new PluginSummary(outputMod, outputPath, 1, PluginRole.Output)
            ],
            [
                Summary(formKey, "RefreshSource", sourceMod, sourcePath, 0, PluginRole.Source, 1),
                Summary(formKey, "RefreshOutput", outputMod, outputPath, 1, PluginRole.Output, 1)
            ],
            request => SuccessfulComparison(
                workspaceId,
                revision,
                request,
                outputMod,
                outputPath,
                outputMod,
                outputPath,
                []));
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision), workspace);
        using var viewModel = CreateViewModel(coordinator);
        await viewModel.StartAsync();

        await viewModel.RefreshAsync(formKey);

        viewModel.SelectedRecord!.FormKey.ShouldBe(formKey);
        viewModel.SelectedBeforeContext!.Role.ShouldBe(PluginRole.Output);
        workspace.ComparisonRequests.ShouldHaveSingleItem().Before.ContainingModKey.ShouldBe(outputMod);
    }

    /// <summary>Verifies a completed refresh releases its caller token so later comparisons retain the workspace generation lifetime.</summary>
    /// <returns>A task that completes after the post-refresh comparison is published.</returns>
    [Fact]
    public async Task Refresh_AfterCompletion_CallerCancellationDoesNotCancelLaterComparison()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 6);
        var sourceMod = ModKey.FromNameAndExtension("RefreshLifetime.esm");
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var formKey = new FormKey(sourceMod, 0x0350);
        var workspace = CreateWorkspace(
            workspaceId,
            revision,
            [new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source)],
            [Summary(formKey, "RefreshLifetimeList", sourceMod, sourcePath, 0, PluginRole.Source, 0)],
            request => SuccessfulComparison(
                workspaceId,
                revision,
                request,
                sourceMod,
                sourcePath,
                sourceMod,
                sourcePath,
                []));
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision), workspace);
        using var viewModel = CreateViewModel(coordinator);
        await viewModel.StartAsync();
        using var refreshCancellation = new CancellationTokenSource();

        await viewModel.RefreshAsync(cancellationToken: refreshCancellation.Token);
        refreshCancellation.Cancel();
        await viewModel.SelectRecordAsync(viewModel.Records.ShouldHaveSingleItem());

        workspace.ComparisonRequests.ShouldHaveSingleItem();
        viewModel.BeforeFields.ShouldNotBeEmpty();
        viewModel.AfterFields.ShouldNotBeEmpty();
    }

    /// <summary>Verifies caller cancellation at the queued UI boundary prevents a refresh snapshot and reselect from publishing.</summary>
    /// <returns>A task that completes after the canceled publication has been suppressed.</returns>
    [Fact]
    public async Task Refresh_CanceledBeforeQueuedPublication_DoesNotPublishSnapshotOrReselect()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 7);
        var sourceMod = ModKey.FromNameAndExtension("QueuedRefresh.esm");
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var formKey = new FormKey(sourceMod, 0x0360);
        var workspace = CreateWorkspace(
            workspaceId,
            revision,
            [new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source)],
            [Summary(formKey, "QueuedRefreshList", sourceMod, sourcePath, 0, PluginRole.Source, 0)],
            _ => throw new InvalidOperationException("A canceled queued refresh must not start comparison."));
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision), workspace);
        var dispatcher = new QueuedUiDispatcher();
        var picker = new RecordingReferencePickerService();
        var operationArbiter = new WorkspacePresentationOperationArbiter();
        using var viewModel = new FormListBrowserViewModel(
            coordinator,
            operationArbiter,
            new RecordJsonTreeProjectionService(),
            picker,
            dispatcher,
            CreateEditorFactory(coordinator, operationArbiter, picker, dispatcher));
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var startTask = viewModel.StartAsync();
        await dispatcher.WaitForInvocationAsync(timeout.Token);
        dispatcher.RunNextInvocation();
        await startTask;
        viewModel.Records.ShouldHaveSingleItem();
        using var refreshCancellation = new CancellationTokenSource();

        var refreshTask = viewModel.RefreshAsync(formKey, refreshCancellation.Token);
        await dispatcher.WaitForInvocationAsync(timeout.Token);
        refreshCancellation.Cancel();
        dispatcher.RunNextInvocation();
        await dispatcher.WaitForInvocationAsync(timeout.Token);
        dispatcher.RunNextInvocation();
        await refreshTask;

        viewModel.Records.ShouldBeEmpty();
        viewModel.SelectedRecord.ShouldBeNull();
        workspace.ComparisonRequests.ShouldBeEmpty();
        viewModel.StatusText.ShouldBe("FormList refresh canceled.");
    }

    /// <summary>Verifies caller cancellation at a queued refresh-comparison boundary suppresses its projected JSON publication.</summary>
    /// <returns>A task that completes after the canceled comparison publication has been suppressed.</returns>
    [Fact]
    public async Task Refresh_CanceledBeforeQueuedComparisonPublication_DoesNotPublishJson()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 8);
        var sourceMod = ModKey.FromNameAndExtension("QueuedComparison.esm");
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var formKey = new FormKey(sourceMod, 0x0361);
        var workspace = CreateWorkspace(
            workspaceId,
            revision,
            [new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source)],
            [Summary(formKey, "QueuedComparisonList", sourceMod, sourcePath, 0, PluginRole.Source, 0)],
            request => SuccessfulComparison(
                workspaceId,
                revision,
                request,
                sourceMod,
                sourcePath,
                sourceMod,
                sourcePath,
                []));
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision), workspace);
        var dispatcher = new QueuedUiDispatcher();
        var picker = new RecordingReferencePickerService();
        var operationArbiter = new WorkspacePresentationOperationArbiter();
        using var viewModel = new FormListBrowserViewModel(
            coordinator,
            operationArbiter,
            new RecordJsonTreeProjectionService(),
            picker,
            dispatcher,
            CreateEditorFactory(coordinator, operationArbiter, picker, dispatcher));
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var startTask = viewModel.StartAsync();
        await dispatcher.WaitForInvocationAsync(timeout.Token);
        dispatcher.RunNextInvocation();
        await startTask;
        using var refreshCancellation = new CancellationTokenSource();

        var refreshTask = viewModel.RefreshAsync(formKey, refreshCancellation.Token);
        await dispatcher.WaitForInvocationAsync(timeout.Token);
        dispatcher.RunNextInvocation();
        await dispatcher.WaitForInvocationAsync(timeout.Token);
        refreshCancellation.Cancel();
        dispatcher.RunNextInvocation();
        await refreshTask;

        workspace.ComparisonRequests.ShouldHaveSingleItem();
        viewModel.SelectedRecord.ShouldNotBeNull();
        viewModel.BeforeFields.ShouldBeEmpty();
        viewModel.AfterFields.ShouldBeEmpty();
        viewModel.SemanticChanges.ShouldBeEmpty();
    }

    /// <summary>Verifies caller cancellation during refresh-owned reselect cancels that comparison without publishing JSON.</summary>
    /// <returns>A task that completes after the refresh-owned comparison observes cancellation.</returns>
    [Fact]
    public async Task Refresh_CanceledDuringReselect_CancelsOwnedComparison()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 9);
        var sourceMod = ModKey.FromNameAndExtension("ReselectCancellation.esm");
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var formKey = new FormKey(sourceMod, 0x0370);
        var comparisonStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        CancellationToken comparisonToken = default;
        var workspace = new RecordingFormListBrowserWorkspace(
            workspaceId,
            revision,
            _ => ValueTask.FromResult(SuccessState(workspaceId, revision)),
            _ => ValueTask.FromResult(EngineResult<IReadOnlyList<PluginSummary>>.Success(
                [new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source)],
                workspaceId,
                resultRevision: revision)),
            (_, _) => ValueTask.FromResult(EngineResult<IReadOnlyList<FormListSummary>>.Success(
                [Summary(formKey, "ReselectCancellationList", sourceMod, sourcePath, 0, PluginRole.Source, 0)],
                workspaceId,
                resultRevision: revision)),
            async (_, token) =>
            {
                comparisonToken = token;
                comparisonStarted.TrySetResult();
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
                throw new InvalidOperationException("The canceled refresh-owned comparison resumed unexpectedly.");
            });
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision), workspace);
        using var viewModel = CreateViewModel(coordinator);
        await viewModel.StartAsync();
        using var refreshCancellation = new CancellationTokenSource();

        var refreshTask = viewModel.RefreshAsync(formKey, refreshCancellation.Token);
        await comparisonStarted.Task;
        viewModel.IsComparisonBusy.ShouldBeTrue();
        refreshCancellation.Cancel();
        await refreshTask;

        comparisonToken.IsCancellationRequested.ShouldBeTrue();
        viewModel.IsComparisonBusy.ShouldBeFalse();
        workspace.ComparisonRequests.ShouldHaveSingleItem();
        viewModel.BeforeFields.ShouldBeEmpty();
        viewModel.AfterFields.ShouldBeEmpty();
    }

    /// <summary>Verifies FormID and EditorID filters retain winning-plugin grouping, matching contexts, and an exact visible selection.</summary>
    /// <returns>A task that completes after the selected filtered record is compared.</returns>
    [Fact]
    public async Task Filters_MatchRootsAndContextsWhilePreservingExactVisibleSelection()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 10);
        var sourceMod = ModKey.FromNameAndExtension("FilterSource.esm");
        var outputMod = ModKey.FromNameAndExtension("FilterOutput.esm");
        var otherMod = ModKey.FromNameAndExtension("FilterOther.esm");
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var outputPath = AbsolutePath(outputMod.FileName);
        var otherPath = AbsolutePath(otherMod.FileName);
        var formKey = new FormKey(sourceMod, 0x1234);
        var otherFormKey = new FormKey(otherMod, 0x5678);
        var workspace = CreateWorkspace(
            workspaceId,
            revision,
            [
                new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source),
                new PluginSummary(outputMod, outputPath, 1, PluginRole.Output),
                new PluginSummary(otherMod, otherPath, 2, PluginRole.LoadOrder)
            ],
            [
                Summary(formKey, "SourceNeedle", sourceMod, sourcePath, 0, PluginRole.Source, 1),
                Summary(formKey, "WinningName", outputMod, outputPath, 1, PluginRole.Output, 1),
                Summary(otherFormKey, "OtherList", otherMod, otherPath, 2, PluginRole.LoadOrder, 0)
            ],
            request => SuccessfulComparison(
                workspaceId,
                revision,
                request,
                sourceMod,
                sourcePath,
                outputMod,
                outputPath,
                []));
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision), workspace);
        using var viewModel = CreateViewModel(coordinator);
        await viewModel.StartAsync();

        viewModel.EditorIdFilter = "sourceneedle";

        var filteredRoot = viewModel.Records.ShouldHaveSingleItem();
        filteredRoot.FormKey.ShouldBe(formKey);
        filteredRoot.Contexts.Select(context => context.EditorId).ShouldBe(["SourceNeedle", "WinningName"]);
        filteredRoot.TreeChildren.Count.ShouldBe(2);
        filteredRoot.ContextOptions.Count.ShouldBe(3);
        viewModel.PluginGroups[0].Children.ShouldBeEmpty();
        viewModel.PluginGroups[1].Children.Cast<RecordTypeGroupViewModel>().ShouldHaveSingleItem()
            .Children.Cast<FormListRecordViewModel>().ShouldHaveSingleItem()
            .ShouldBeSameAs(filteredRoot);
        await viewModel.SelectRecordAsync(filteredRoot.Contexts[0]);
        var selectedBefore = viewModel.SelectedBeforeContext.ShouldNotBeNull();
        var selectedAfter = viewModel.SelectedAfterContext.ShouldNotBeNull();

        viewModel.FormIdFilter = "001234";

        viewModel.Records.ShouldHaveSingleItem().FormKey.ShouldBe(formKey);
        viewModel.SelectedRecord.ShouldNotBeNull().FormKey.ShouldBe(formKey);
        viewModel.SelectedBeforeContext.ShouldBeSameAs(selectedBefore);
        viewModel.SelectedAfterContext.ShouldBeSameAs(selectedAfter);
    }

    /// <summary>Verifies hiding the selected FormList cancels its comparison and clears all exact selection state.</summary>
    /// <returns>A task that completes after the in-flight comparison observes cancellation.</returns>
    [Fact]
    public async Task Filter_HidesSelectedRoot_CancelsComparisonAndClearsSelection()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 11);
        var selectedMod = ModKey.FromNameAndExtension("SelectedFilter.esm");
        var visibleMod = ModKey.FromNameAndExtension("VisibleFilter.esm");
        var selectedPath = AbsolutePath(selectedMod.FileName);
        var visiblePath = AbsolutePath(visibleMod.FileName);
        var selectedFormKey = new FormKey(selectedMod, 0x0400);
        var visibleFormKey = new FormKey(visibleMod, 0x0401);
        var comparisonStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        CancellationToken comparisonToken = default;
        var workspace = new RecordingFormListBrowserWorkspace(
            workspaceId,
            revision,
            _ => ValueTask.FromResult(SuccessState(workspaceId, revision)),
            _ => ValueTask.FromResult(EngineResult<IReadOnlyList<PluginSummary>>.Success(
                [
                    new PluginSummary(selectedMod, selectedPath, 0, PluginRole.Source),
                    new PluginSummary(visibleMod, visiblePath, 1, PluginRole.LoadOrder)
                ],
                workspaceId,
                resultRevision: revision)),
            (_, _) => ValueTask.FromResult(EngineResult<IReadOnlyList<FormListSummary>>.Success(
                [
                    Summary(selectedFormKey, "SelectedList", selectedMod, selectedPath, 0, PluginRole.Source, 0),
                    Summary(visibleFormKey, "VisibleList", visibleMod, visiblePath, 1, PluginRole.LoadOrder, 0)
                ],
                workspaceId,
                resultRevision: revision)),
            async (_, token) =>
            {
                comparisonToken = token;
                comparisonStarted.TrySetResult();
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
                throw new InvalidOperationException("The filtered comparison resumed unexpectedly.");
            });
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision), workspace);
        using var viewModel = CreateViewModel(coordinator);
        await viewModel.StartAsync();
        var comparisonTask = viewModel.SelectRecordAsync(
            viewModel.Records.Single(record => record.FormKey == selectedFormKey));
        await comparisonStarted.Task;

        viewModel.EditorIdFilter = "VisibleList";
        await comparisonTask;

        comparisonToken.IsCancellationRequested.ShouldBeTrue();
        viewModel.Records.ShouldHaveSingleItem().FormKey.ShouldBe(visibleFormKey);
        viewModel.SelectedRecord.ShouldBeNull();
        viewModel.SelectedBeforeContext.ShouldBeNull();
        viewModel.SelectedAfterContext.ShouldBeNull();
        viewModel.BeforeFields.ShouldBeEmpty();
        viewModel.AfterFields.ShouldBeEmpty();
        viewModel.StatusText.ShouldBe("The selected FormList is hidden by the current filters.");
    }

    /// <summary>Verifies a newer context selection cancels and suppresses an in-flight comparison result.</summary>
    /// <returns>A task that completes after the newer exact-context comparison publishes.</returns>
    [Fact]
    public async Task ContextChange_CancelsAndSuppressesPriorComparisonGeneration()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 3);
        var sourceMod = ModKey.FromNameAndExtension("CompareSource.esm");
        var outputMod = ModKey.FromNameAndExtension("CompareOutput.esm");
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var outputPath = AbsolutePath(outputMod.FileName);
        var formKey = new FormKey(sourceMod, 0x0600);
        var firstComparisonStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        CancellationToken firstComparisonToken = default;
        var compareAttempt = 0;
        var workspace = new RecordingFormListBrowserWorkspace(
            workspaceId,
            revision,
            _ => ValueTask.FromResult(SuccessState(workspaceId, revision)),
            _ => ValueTask.FromResult(EngineResult<IReadOnlyList<PluginSummary>>.Success(
                [
                    new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source),
                    new PluginSummary(outputMod, outputPath, 1, PluginRole.Output)
                ],
                workspaceId,
                resultRevision: revision)),
            (_, _) => ValueTask.FromResult(EngineResult<IReadOnlyList<FormListSummary>>.Success(
                [
                    Summary(formKey, "CompareSource", sourceMod, sourcePath, 0, PluginRole.Source, 1),
                    Summary(formKey, "CompareOutput", outputMod, outputPath, 1, PluginRole.Output, 1)
                ],
                workspaceId,
                resultRevision: revision)),
            async (request, token) =>
            {
                compareAttempt++;
                if (compareAttempt == 1)
                {
                    firstComparisonToken = token;
                    firstComparisonStarted.TrySetResult();
                    await Task.Delay(Timeout.InfiniteTimeSpan, token);
                }

                return SuccessfulComparison(
                    workspaceId,
                    revision,
                    request,
                    outputMod,
                    outputPath,
                    outputMod,
                    outputPath,
                    []);
            });
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision), workspace);
        using var viewModel = CreateViewModel(coordinator);
        await viewModel.StartAsync();
        var firstComparison = viewModel.SelectRecordAsync(viewModel.Records.ShouldHaveSingleItem());
        await firstComparisonStarted.Task;
        viewModel.IsComparisonBusy.ShouldBeTrue();

        var outputOption = viewModel.ContextOptions.Single(option => option.Role == PluginRole.Output);
        var secondComparison = viewModel.SelectAfterContextAsync(outputOption);
        await secondComparison;
        await firstComparison;

        firstComparisonToken.IsCancellationRequested.ShouldBeTrue();
        compareAttempt.ShouldBe(2);
        viewModel.IsComparisonBusy.ShouldBeFalse();
        viewModel.AfterContext!.Selection.Scope.ShouldBe(RecordScope.AllContexts);
        viewModel.AfterContext.Selection.ContainingModKey.ShouldBe(outputMod);
        viewModel.AfterFields.ShouldHaveSingleItem()
            .Children.Single(node => node.Name == "Name")
            .Children.ShouldHaveSingleItem()
            .ComparisonState.ShouldBe(ComparisonFieldState.Conflict);
    }

    /// <summary>Verifies a changed live revision rejects comparison before record traversal.</summary>
    /// <returns>A task that completes after the typed revision failure publishes.</returns>
    [Fact]
    public async Task SelectRecord_WhenLiveRevisionChanged_ReportsTypedConflictBeforeComparison()
    {
        var workspaceId = Guid.NewGuid();
        var loadedRevision = new WorkspaceRevision(Guid.NewGuid(), 1);
        var changedRevision = loadedRevision.Next();
        var sourceMod = ModKey.FromNameAndExtension("RevisionSource.esm");
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var formKey = new FormKey(sourceMod, 0x0700);
        var stateRead = 0;
        var compareCalled = false;
        var workspace = new RecordingFormListBrowserWorkspace(
            workspaceId,
            loadedRevision,
            _ => ValueTask.FromResult(SuccessState(
                workspaceId,
                ++stateRead == 1 ? loadedRevision : changedRevision)),
            _ => ValueTask.FromResult(EngineResult<IReadOnlyList<PluginSummary>>.Success(
                [new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source)],
                workspaceId,
                resultRevision: loadedRevision)),
            (_, _) => ValueTask.FromResult(EngineResult<IReadOnlyList<FormListSummary>>.Success(
                [Summary(formKey, "RevisionList", sourceMod, sourcePath, 0, PluginRole.Source, 0)],
                workspaceId,
                resultRevision: loadedRevision)),
            (_, _) =>
            {
                compareCalled = true;
                throw new InvalidOperationException("Revision guard failed to stop plugin comparison traversal.");
            });
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, loadedRevision), workspace);
        using var viewModel = CreateViewModel(coordinator);
        await viewModel.StartAsync();

        await viewModel.SelectRecordAsync(viewModel.Records.ShouldHaveSingleItem());

        compareCalled.ShouldBeFalse();
        viewModel.ErrorCode.ShouldBe(EngineErrorCode.RevisionConflict);
        viewModel.ErrorMessage.ShouldNotBeNull().ShouldContain("changed after this browser selection");
        viewModel.BeforeFields.ShouldBeEmpty();
        viewModel.AfterFields.ShouldBeEmpty();
    }

    /// <summary>Verifies picker selections are revision-bound, display every record family, and navigate only FormLists.</summary>
    /// <returns>A task that completes after both picker results are handled.</returns>
    [Fact]
    public async Task PickReference_ExactCurrentSelection_DisplaysAnyFamilyAndNavigatesOnlyFormList()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 5);
        var sourceMod = ModKey.FromNameAndExtension("PickerSource.esm");
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var formListKey = new FormKey(sourceMod, 0x0800);
        var bookKey = new FormKey(sourceMod, 0x0801);
        var workspace = CreateWorkspace(
            workspaceId,
            revision,
            [new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source)],
            [Summary(formListKey, "PickerList", sourceMod, sourcePath, 0, PluginRole.Source, 0)],
            request => SuccessfulComparison(
                workspaceId,
                revision,
                request,
                sourceMod,
                sourcePath,
                sourceMod,
                sourcePath,
                []));
        ReferencePickerSelection? nextSelection = new(
            workspaceId,
            revision,
            isNull: false,
            match: new ReferenceSearchMatch(
                bookKey,
                "Book",
                "PickerBook",
                sourceMod,
                sourcePath,
                0,
                PluginRole.Source));
        var picker = new RecordingReferencePickerService((_, _) =>
            Task.FromResult<ReferencePickerSelection?>(nextSelection));
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision), workspace);
        var dispatcher = new InlineUiDispatcher();
        var operationArbiter = new WorkspacePresentationOperationArbiter();
        using var viewModel = new FormListBrowserViewModel(
            coordinator,
            operationArbiter,
            new RecordJsonTreeProjectionService(),
            picker,
            dispatcher,
            CreateEditorFactory(coordinator, operationArbiter, picker, dispatcher));
        await viewModel.StartAsync();

        await viewModel.PickReferenceAsync();

        viewModel.SelectedReferenceText.ShouldContain(bookKey.ToString());
        viewModel.SelectedReferenceText.ShouldContain("Book");
        viewModel.SelectedRecord.ShouldBeNull();
        var request = picker.Requests.ShouldHaveSingleItem();
        request.WorkspaceId.ShouldBe(workspaceId);
        request.ExpectedRevision.ShouldBe(revision);
        request.RecordScope.ShouldBe(RecordScope.WinningOverrides);
        request.ContainingModKey.ShouldBeNull();
        request.AllowNull.ShouldBeFalse();

        nextSelection = new ReferencePickerSelection(
            workspaceId,
            revision,
            isNull: false,
            match: new ReferenceSearchMatch(
                formListKey,
                "FormList",
                "PickerList",
                sourceMod,
                sourcePath,
                0,
                PluginRole.Source));
        await viewModel.PickReferenceAsync();

        viewModel.SelectedReferenceText.ShouldContain("FormList");
        viewModel.SelectedRecord!.FormKey.ShouldBe(formListKey);
        workspace.ComparisonRequests.ShouldHaveSingleItem();
    }

    /// <summary>Verifies a picker result from another revision cannot change browser selection or displayed identity.</summary>
    /// <returns>A task that completes after the stale selection is rejected.</returns>
    [Fact]
    public async Task PickReference_StaleRevision_IsIgnored()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 1);
        var sourceMod = ModKey.FromNameAndExtension("StalePicker.esm");
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var formKey = new FormKey(sourceMod, 0x0900);
        var workspace = CreateWorkspace(
            workspaceId,
            revision,
            [new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source)],
            [Summary(formKey, "StalePickerList", sourceMod, sourcePath, 0, PluginRole.Source, 0)],
            _ => throw new NotSupportedException());
        var picker = new RecordingReferencePickerService((_, _) => Task.FromResult<ReferencePickerSelection?>(new(
            workspaceId,
            revision.Next(),
            isNull: false,
            match: new ReferenceSearchMatch(formKey, "FormList", "StalePickerList"))));
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision), workspace);
        var dispatcher = new InlineUiDispatcher();
        var operationArbiter = new WorkspacePresentationOperationArbiter();
        using var viewModel = new FormListBrowserViewModel(
            coordinator,
            operationArbiter,
            new RecordJsonTreeProjectionService(),
            picker,
            dispatcher,
            CreateEditorFactory(coordinator, operationArbiter, picker, dispatcher));
        await viewModel.StartAsync();

        await viewModel.PickReferenceAsync();

        viewModel.SelectedReferenceText.ShouldBe("No reference selected.");
        viewModel.SelectedRecord.ShouldBeNull();
        workspace.ComparisonRequests.ShouldBeEmpty();
    }

    /// <summary>Creates a browser workspace with successful state, plugin, and all-context reads.</summary>
    /// <param name="workspaceId">The workspace identity.</param>
    /// <param name="revision">The exact workspace revision.</param>
    /// <param name="plugins">The participating plugins.</param>
    /// <param name="summaries">The FormList contexts.</param>
    /// <param name="compare">The comparison result callback.</param>
    /// <returns>The configurable browser workspace.</returns>
    private static RecordingFormListBrowserWorkspace CreateWorkspace(
        Guid workspaceId,
        WorkspaceRevision revision,
        IReadOnlyList<PluginSummary> plugins,
        IReadOnlyList<FormListSummary> summaries,
        Func<CompareFormListRequest, EngineResult<FormListComparison>> compare)
    {
        return new RecordingFormListBrowserWorkspace(
            workspaceId,
            revision,
            _ => ValueTask.FromResult(SuccessState(workspaceId, revision)),
            _ => ValueTask.FromResult(EngineResult<IReadOnlyList<PluginSummary>>.Success(
                plugins,
                workspaceId,
                resultRevision: revision)),
            (_, _) => ValueTask.FromResult(EngineResult<IReadOnlyList<FormListSummary>>.Success(
                summaries,
                workspaceId,
                resultRevision: revision)),
            (request, _) => ValueTask.FromResult(compare(request)));
    }

    /// <summary>Creates a successful atomic state result at an exact revision.</summary>
    /// <param name="workspaceId">The workspace identity.</param>
    /// <param name="revision">The exact workspace revision.</param>
    /// <returns>The successful state result.</returns>
    private static EngineResult<WorkspaceState> SuccessState(Guid workspaceId, WorkspaceRevision revision)
    {
        return EngineResult<WorkspaceState>.Success(
            new WorkspaceState(
                SupportedGame.Starfield,
                GameRelease.Starfield,
                output: null,
                outputBaseline: null,
                new OutputSynchronizationState(OutputSynchronizationStatus.Ready, null),
                revision),
            workspaceId,
            resultRevision: revision);
    }

    /// <summary>Creates one all-context FormList summary.</summary>
    /// <param name="formKey">The FormList identity.</param>
    /// <param name="editorId">The context EditorID.</param>
    /// <param name="containingModKey">The containing plugin.</param>
    /// <param name="path">The canonical plugin path.</param>
    /// <param name="loadOrderIndex">The exact load-order index.</param>
    /// <param name="role">The workspace plugin role.</param>
    /// <param name="overrideCount">The plugin override count.</param>
    /// <returns>The exact-context summary.</returns>
    private static FormListSummary Summary(
        FormKey formKey,
        string? editorId,
        ModKey containingModKey,
        string path,
        int loadOrderIndex,
        PluginRole role,
        int overrideCount)
    {
        return new FormListSummary(
            formKey,
            editorId,
            overrideCount,
            RecordScope.AllContexts,
            containingModKey,
            path,
            loadOrderIndex,
            role);
    }

    /// <summary>Creates a successful resolved comparison with complete detached JSON and exact provenance.</summary>
    /// <param name="workspaceId">The workspace identity.</param>
    /// <param name="revision">The exact workspace revision.</param>
    /// <param name="request">The exact selected contexts.</param>
    /// <param name="beforeMod">The resolved prior containing plugin.</param>
    /// <param name="beforePath">The prior plugin path.</param>
    /// <param name="afterMod">The resolved resulting containing plugin.</param>
    /// <param name="afterPath">The resulting plugin path.</param>
    /// <param name="changes">The semantic change descriptors.</param>
    /// <returns>The successful comparison result.</returns>
    private static EngineResult<FormListComparison> SuccessfulComparison(
        Guid workspaceId,
        WorkspaceRevision revision,
        CompareFormListRequest request,
        ModKey beforeMod,
        string beforePath,
        ModKey afterMod,
        string afterPath,
        IReadOnlyList<SemanticChangeDescriptor> changes)
    {
        var beforeContext = new FormListContext(
            request.Before,
            ReferenceResolutionStatus.Resolved,
            beforeMod,
            beforePath,
            0,
            PluginRole.Source);
        var afterContext = new FormListContext(
            request.After,
            ReferenceResolutionStatus.Resolved,
            afterMod,
            afterPath,
            beforeMod == afterMod ? 0 : 2,
            beforeMod == afterMod ? PluginRole.Source : PluginRole.Output);
        var before = Element("{\"$type\":\"FormList\",\"Items\":[{\"link\":\"A\"},{\"link\":null}],\"Name\":{\"English\":\"Before\"}}");
        var after = Element("{\"$type\":\"FormList\",\"Items\":[{\"link\":\"A\"},{\"link\":\"A\"},{\"link\":null}],\"Name\":{\"English\":\"After\"}}");
        return EngineResult<FormListComparison>.Success(
            new FormListComparison(beforeContext, afterContext, before, after, changes, []),
            workspaceId,
            resultRevision: revision);
    }

    /// <summary>Creates a navigation-safe workspace descriptor.</summary>
    /// <param name="workspaceId">The workspace identity.</param>
    /// <param name="revision">The exact workspace revision.</param>
    /// <returns>The immutable descriptor.</returns>
    private static WorkspaceDescriptor CreateDescriptor(Guid workspaceId, WorkspaceRevision revision)
    {
        var sourcePath = AbsolutePath("BrowserSource.esm");
        var outputPath = AbsolutePath("BrowserOutput.esm");
        return new WorkspaceDescriptor(
            workspaceId,
            SupportedGame.Starfield,
            GameRelease.Starfield,
            sourcePath,
            [sourcePath],
            new OutputAssociation(
                outputPath,
                ModKey.FromNameAndExtension("BrowserOutput.esm"),
                LocalizedOutputMode.Embedded,
                OutputMasterStyle.Full),
            revision);
    }

    /// <summary>Creates a browser view model with deterministic presentation dependencies.</summary>
    /// <param name="coordinator">The recording workspace coordinator.</param>
    /// <returns>The configured browser view model.</returns>
    private static FormListBrowserViewModel CreateViewModel(RecordingWorkspaceCoordinator coordinator)
    {
        var picker = new RecordingReferencePickerService();
        var dispatcher = new InlineUiDispatcher();
        var operationArbiter = new WorkspacePresentationOperationArbiter();
        return new FormListBrowserViewModel(
            coordinator,
            operationArbiter,
            new RecordJsonTreeProjectionService(),
            picker,
            dispatcher,
            CreateEditorFactory(coordinator, operationArbiter, picker, dispatcher));
    }

    /// <summary>Creates the browser-owned editor factory with deterministic presentation dependencies.</summary>
    /// <param name="coordinator">The recording workspace coordinator.</param>
    /// <param name="operationArbiter">The shared editor and workspace-transition admission boundary.</param>
    /// <param name="picker">The recording reference picker.</param>
    /// <param name="dispatcher">The deterministic presentation dispatcher.</param>
    /// <returns>The configured editor factory.</returns>
    private static IFormListEditorViewModelFactory CreateEditorFactory(
        IWorkspaceCoordinator coordinator,
        IWorkspacePresentationOperationArbiter operationArbiter,
        IReferencePickerService picker,
        IUiDispatcher dispatcher)
    {
        var validator = new FormListDraftValidator();
        return new FormListEditorViewModelFactory(
            coordinator,
            operationArbiter,
            new FormListWireCatalogResolver([], []),
            new FormListDraftFactory(),
            validator,
            new FormListDraftSerializer(validator),
            picker,
            dispatcher);
    }

    /// <summary>Parses and detaches one JSON value for an engine result.</summary>
    /// <param name="json">The complete JSON text.</param>
    /// <returns>The detached root element.</returns>
    private static JsonElement Element(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

    /// <summary>Creates a fully qualified test-only plugin path.</summary>
    /// <param name="fileName">The plugin file name.</param>
    /// <returns>The fully qualified path.</returns>
    private static string AbsolutePath(string fileName)
    {
        return Path.GetFullPath(Path.Combine(Path.GetTempPath(), "CreationsForge-BrowserTests", fileName));
    }

    /// <summary>Waits without sleeping until a deterministic asynchronous condition becomes true.</summary>
    /// <param name="condition">The condition to observe.</param>
    /// <returns>A task that completes when the condition becomes true.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the condition remains false for five seconds.</exception>
    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        while (!condition())
        {
            timeout.Token.ThrowIfCancellationRequested();
            await Task.Yield();
        }
    }
}
