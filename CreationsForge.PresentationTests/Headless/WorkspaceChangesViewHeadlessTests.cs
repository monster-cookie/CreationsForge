using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.PresentationTests.Support;
using CreationsForge.PresentationTests.ViewModels;
using CreationsForge.Services;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Shouldly;

namespace CreationsForge.PresentationTests.Headless;

/// <summary>Verifies the shared plugin changes dialog renders active-editor, review, and blocked-recovery states headlessly.</summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class WorkspaceChangesViewHeadlessTests
{
    /// <summary>Verifies a populated review exposes detached comparison trees and writes its opt-in rendered artifact.</summary>
    [AvaloniaFact]
    public async Task WorkspaceChangesView_PopulatedReview_ExposesComparisonAndWritesScreenshot()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(
            hasStagedChanges: true,
            hasDraftChanges: true);
        var wasPresented = false;
        context.DialogService.OnShowAsync = (viewModel, request) =>
        {
            wasPresented = true;
            request.Purpose.ShouldBe(WorkspaceChangesDialogPurpose.Review);
            ShowPopulatedReview(viewModel, request);
            return Task.FromResult(WorkspaceChangesDialogResult.KeepEditing);
        };

        await context.ViewModel.ReviewChangesAsync();

        wasPresented.ShouldBeTrue();
        context.ViewModel.CurrentReview.ShouldNotBeNull().Items.Count.ShouldBe(1);
    }

    /// <summary>Verifies an active editor shows only the three closed choices and forwards both drain decisions.</summary>
    /// <returns>A task that completes after the active-editor dialog session releases its pending request.</returns>
    [AvaloniaFact]
    public async Task WorkspaceChangesView_ActiveEditor_ShowsClosedChoicesAndWritesScreenshot()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        using var editorLease = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        var forwardedChoices = new List<WorkspaceChangesDialogChoice>();
        var wasPresented = false;
        context.DialogService.OnShowAsync = (viewModel, request) =>
        {
            wasPresented = true;
            request.Purpose.ShouldBe(WorkspaceChangesDialogPurpose.Leave);
            viewModel.ShowActiveEditorOperationDecision.ShouldBeTrue();
            var view = new WorkspaceChangesView(viewModel, request, choice =>
            {
                forwardedChoices.Add(choice);
                return Task.CompletedTask;
            });
            var window = CreateWindow(view);
            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();

                var explanation = ControlFinder.FindByAutomationId<TextBlock>(
                    view,
                    "WorkspaceActiveEditorOperationText").ShouldNotBeNull();
                explanation.IsEffectivelyVisible.ShouldBeTrue();
                explanation.Text.ShouldBe("An editor operation is still running. Wait for it, request cancellation and wait for a safe result, or keep editing.");
                var wait = ControlFinder.FindByAutomationId<Button>(
                    view,
                    "WorkspaceWaitForEditorOperationButton").ShouldNotBeNull();
                wait.IsEffectivelyVisible.ShouldBeTrue();
                wait.IsEnabled.ShouldBeTrue();
                wait.Content.ShouldBe("Wait for operation");
                var cancelAndWait = ControlFinder.FindByAutomationId<Button>(
                    view,
                    "WorkspaceCancelEditorOperationAndWaitButton").ShouldNotBeNull();
                cancelAndWait.IsEffectivelyVisible.ShouldBeTrue();
                cancelAndWait.IsEnabled.ShouldBeTrue();
                cancelAndWait.Content.ShouldBe("Cancel operation and wait");
                var keepEditing = ControlFinder.FindByAutomationId<Button>(
                    view,
                    "WorkspaceKeepEditingButton").ShouldNotBeNull();
                keepEditing.IsEffectivelyVisible.ShouldBeTrue();
                keepEditing.IsDefault.ShouldBeTrue();
                keepEditing.Content.ShouldBe("Keep Editing");
                AssertHidden(view, "WorkspaceSaveAndProceedButton");
                AssertHidden(view, "WorkspaceDiscardAndProceedButton");
                AssertHidden(view, "WorkspaceReturnToEditorButton");
                AssertHidden(view, "WorkspaceConfirmAbandonmentButton");
                AssertHidden(view, "WorkspaceRetryCommittedRefreshButton");
                AssertHidden(view, "WorkspaceInspectRecoveryButton");
                AssertHidden(view, "WorkspaceCompletePreparedButton");
                AssertHidden(view, "WorkspaceRestoreBaselineButton");
                AssertHidden(view, "WorkspaceResumeStagedButton");
                AssertHidden(view, "WorkspaceReopenResolvedButton");

                wait.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                cancelAndWait.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                Dispatcher.UIThread.RunJobs();
                forwardedChoices.ShouldBe([
                    WorkspaceChangesDialogChoice.WaitForEditorOperation,
                    WorkspaceChangesDialogChoice.CancelEditorOperationAndWait]);
                WriteScreenshot(window, "plugin-workspace-changes-active-editor.png");
            }
            finally
            {
                window.Close();
            }

            return Task.FromResult(WorkspaceChangesDialogResult.KeepEditing);
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(WorkspaceLeaveReason.CloseWorkspace);

        reservation.ShouldBeNull();
        wasPresented.ShouldBeTrue();
        editorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
        context.Arbiter.IsEditorOperationActive.ShouldBeTrue();
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies an uncertain post-drain editor outcome leaves only Return to editor and Keep Editing.</summary>
    /// <returns>A task that completes after the pending editor outcome is rendered and the session is dismissed.</returns>
    [AvaloniaFact]
    public async Task WorkspaceChangesView_PendingEditorOutcome_HidesPersistenceAndDrainChoices()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        using var editorLease = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        context.EditParticipant.SetState(
            hasDraftChanges: false,
            isEditorBusy: true,
            operationState: FormListEditorOperationState.Applying);
        context.DialogService.OnShowAsync = async (viewModel, request) =>
        {
            var view = new WorkspaceChangesView(viewModel, request, _ => Task.CompletedTask);
            var window = CreateWindow(view);
            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                var choiceTask = viewModel.ApplyLeaveChoiceAsync(
                    WorkspaceChangesDialogChoice.CancelEditorOperationAndWait);
                context.EditParticipant.SetState(
                    hasDraftChanges: false,
                    hasPendingOperation: true,
                    operationState: FormListEditorOperationState.PendingOutcome);
                editorLease.Dispose();

                (await choiceTask).ShouldBe(WorkspaceLeaveChoiceOutcome.ContinueDialog);
                Dispatcher.UIThread.RunJobs();

                ControlFinder.FindByAutomationId<TextBlock>(
                    view,
                    "WorkspaceActiveEditorOperationText")!
                    .IsEffectivelyVisible.ShouldBeFalse();
                AssertHidden(view, "WorkspaceWaitForEditorOperationButton");
                AssertHidden(view, "WorkspaceCancelEditorOperationAndWaitButton");
                AssertHidden(view, "WorkspaceSaveAndProceedButton");
                AssertHidden(view, "WorkspaceDiscardAndProceedButton");
                AssertHidden(view, "WorkspaceConfirmAbandonmentButton");
                var returnToEditor = ControlFinder.FindByAutomationId<Button>(
                    view,
                    "WorkspaceReturnToEditorButton").ShouldNotBeNull();
                returnToEditor.IsEffectivelyVisible.ShouldBeTrue();
                returnToEditor.IsEnabled.ShouldBeTrue();
                var keepEditing = ControlFinder.FindByAutomationId<Button>(
                    view,
                    "WorkspaceKeepEditingButton").ShouldNotBeNull();
                keepEditing.IsEffectivelyVisible.ShouldBeTrue();
                keepEditing.IsDefault.ShouldBeTrue();
            }
            finally
            {
                window.Close();
            }

            return WorkspaceChangesDialogResult.KeepEditing;
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(WorkspaceLeaveReason.ShowSettings);

        reservation.ShouldBeNull();
        context.Workspace!.PreviewCount.ShouldBe(0);
    }

    /// <summary>Verifies a definitive successful post-drain outcome replaces drain choices with fresh persistence actions.</summary>
    /// <returns>A task that completes after the staged post-drain review is rendered and dismissed.</returns>
    [AvaloniaFact]
    public async Task WorkspaceChangesView_SuccessfulEditorOutcome_ShowsFreshPersistenceChoices()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        using var editorLease = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        context.EditParticipant.SetState(
            hasDraftChanges: false,
            isEditorBusy: true,
            operationState: FormListEditorOperationState.Applying);
        context.DialogService.OnShowAsync = async (viewModel, request) =>
        {
            var view = new WorkspaceChangesView(viewModel, request, _ => Task.CompletedTask);
            var window = CreateWindow(view);
            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                var choiceTask = viewModel.ApplyLeaveChoiceAsync(
                    WorkspaceChangesDialogChoice.WaitForEditorOperation);
                context.EditParticipant.SetState(hasDraftChanges: false);
                editorLease.Dispose();

                (await choiceTask).ShouldBe(WorkspaceLeaveChoiceOutcome.ContinueDialog);
                Dispatcher.UIThread.RunJobs();

                ControlFinder.FindByAutomationId<TextBlock>(
                    view,
                    "WorkspaceActiveEditorOperationText")!
                    .IsEffectivelyVisible.ShouldBeFalse();
                AssertHidden(view, "WorkspaceWaitForEditorOperationButton");
                AssertHidden(view, "WorkspaceCancelEditorOperationAndWaitButton");
                AssertHidden(view, "WorkspaceReturnToEditorButton");
                var save = ControlFinder.FindByAutomationId<Button>(
                    view,
                    "WorkspaceSaveAndProceedButton").ShouldNotBeNull();
                save.IsEffectivelyVisible.ShouldBeTrue();
                save.IsEnabled.ShouldBeTrue();
                var discard = ControlFinder.FindByAutomationId<Button>(
                    view,
                    "WorkspaceDiscardAndProceedButton").ShouldNotBeNull();
                discard.IsEffectivelyVisible.ShouldBeTrue();
                discard.IsEnabled.ShouldBeTrue();
                ControlFinder.FindByAutomationId<Button>(
                    view,
                    "WorkspaceKeepEditingButton")!
                    .IsDefault.ShouldBeTrue();
            }
            finally
            {
                window.Close();
            }

            return WorkspaceChangesDialogResult.KeepEditing;
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(WorkspaceLeaveReason.CloseWorkspace);

        reservation.ShouldBeNull();
        context.Workspace!.PreviewCount.ShouldBe(1);
    }

    /// <summary>Verifies an unknown save outcome exposes recovery without destructive choices and writes its opt-in rendered artifact.</summary>
    [AvaloniaFact]
    public async Task WorkspaceChangesView_RecoveryRequired_ExposesInspectionAndWritesScreenshot()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady();
        SetRecoveryRequired(context);
        var wasPresented = false;
        context.DialogService.OnShowAsync = (viewModel, request) =>
        {
            wasPresented = true;
            request.Purpose.ShouldBe(WorkspaceChangesDialogPurpose.Leave);
            request.LeaveReason.ShouldBe(WorkspaceLeaveReason.ExitApplication);
            ShowRecoveryRequired(viewModel, request);
            return Task.FromResult(WorkspaceChangesDialogResult.KeepEditing);
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(WorkspaceLeaveReason.ExitApplication);

        reservation.ShouldBeNull();
        wasPresented.ShouldBeTrue();
        context.Workspace!.PreviewCount.ShouldBe(0);
    }

    /// <summary>Verifies committed recovery labels its terminal action as reopening saved output.</summary>
    [AvaloniaFact]
    public async Task WorkspaceChangesView_CommittedRecovery_LabelsReopenSavedOutput()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        WorkspaceChangesViewModelTests.SetRecoveryRequired(
            context,
            context.Workspace!.WorkspaceId,
            out var pending);
        var baseline = WorkspaceChangesViewModelTests.ReplacementBaseline(context.Workspace.State.Output!);
        var evidence = WorkspaceChangesViewModelTests.Evidence(
            pending,
            baseline,
            RecoverSaveStatus.Committed,
            new RecoveryEvidenceToken("headless-committed-token"));
        context.SaveCoordinator.OnRecoverAsync = (_, _) => ValueTask.FromResult(
            CreateTerminalRecovery(pending, evidence, RecoverSaveStatus.Committed));

        await VerifyRecoveryReopenLabelAsync(
            context,
            viewModel => viewModel.InspectSaveOutcomeAsync(),
            "Reopen Saved Output");
    }

    /// <summary>Verifies original not-committed recovery names the staged-candidate discard explicitly.</summary>
    [AvaloniaFact]
    public async Task WorkspaceChangesView_OriginalNotCommittedRecovery_LabelsDiscardAndReopen()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        WorkspaceChangesViewModelTests.SetRecoveryRequired(
            context,
            context.Workspace!.WorkspaceId,
            out var pending);
        var baseline = WorkspaceChangesViewModelTests.ReplacementBaseline(context.Workspace.State.Output!);
        var evidence = WorkspaceChangesViewModelTests.Evidence(pending, baseline);
        context.SaveCoordinator.OnRecoverAsync = (_, _) => ValueTask.FromResult(
            WorkspaceChangesViewModelTests.TerminalRecovery(pending, evidence));

        await VerifyRecoveryReopenLabelAsync(
            context,
            viewModel => viewModel.InspectSaveOutcomeAsync(),
            "Discard Staged Changes and Reopen Output");
    }

    /// <summary>Verifies fresh not-committed recovery uses resolved-output wording when no staged candidate can resume.</summary>
    [AvaloniaFact]
    public async Task WorkspaceChangesView_FreshNotCommittedRecovery_LabelsReopenResolvedOutput()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        WorkspaceChangesViewModelTests.SetRecoveryRequired(context, Guid.NewGuid(), out var pending);
        var baseline = WorkspaceChangesViewModelTests.ReplacementBaseline(context.Workspace!.State.Output!);
        var evidence = WorkspaceChangesViewModelTests.Evidence(pending, baseline);
        context.SaveCoordinator.OnRecoverAsync = (_, _) => ValueTask.FromResult(
            WorkspaceChangesViewModelTests.TerminalRecovery(pending, evidence));

        await VerifyRecoveryReopenLabelAsync(
            context,
            viewModel => viewModel.InspectSaveOutcomeAsync(),
            "Reopen Resolved Output");
    }

    /// <summary>Verifies baseline restoration keeps the retained staged-candidate discard explicit.</summary>
    [AvaloniaFact]
    public async Task WorkspaceChangesView_BaselineRestoredRecovery_LabelsDiscardAndReopen()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        WorkspaceChangesViewModelTests.SetRecoveryRequired(
            context,
            context.Workspace!.WorkspaceId,
            out var pending);
        var token = new RecoveryEvidenceToken("headless-baseline-restored-token");
        var baseline = WorkspaceChangesViewModelTests.ReplacementBaseline(context.Workspace.State.Output!);
        var evidence = WorkspaceChangesViewModelTests.Evidence(
            pending,
            baseline,
            RecoverSaveStatus.NotCommitted,
            token);
        context.SaveCoordinator.OnRecoverAsync = (_, _) => ValueTask.FromResult(
            WorkspaceChangesViewModelTests.RepairRequiredRecovery(pending, token));
        context.SaveCoordinator.OnRepairAsync = (request, _) => ValueTask.FromResult(new RepairSaveResult(
            request.WorkspaceId,
            request.SaveOperationId,
            request.RepairOperationId,
            RepairSaveStatus.BaselineRestored,
            baseline,
            evidence.EvidenceToken,
            evidence,
            error: null));

        await VerifyRecoveryReopenLabelAsync(
            context,
            async viewModel =>
            {
                await viewModel.InspectSaveOutcomeAsync();
                await viewModel.RestorePreviousOutputAsync();
            },
            "Discard Staged Changes and Reopen Output");
    }

    /// <summary>Shows the bound recovery action before publishing one terminal outcome and verifies its updated label.</summary>
    /// <param name="context">The recovery fixture whose fake dialog retains transition ownership.</param>
    /// <param name="publishOutcomeAsync">The recovery operation that publishes the tested terminal outcome.</param>
    /// <param name="expectedLabel">The exact visible action label after publication.</param>
    /// <returns>A task that completes after the fake modal session releases its transition.</returns>
    private static async Task VerifyRecoveryReopenLabelAsync(
        WorkspaceChangesTestContext context,
        Func<WorkspaceChangesViewModel, Task> publishOutcomeAsync,
        string expectedLabel)
    {
        var wasPresented = false;
        context.DialogService.OnShowAsync = async (viewModel, request) =>
        {
            wasPresented = true;
            var view = new WorkspaceChangesView(viewModel, request, _ => Task.CompletedTask);
            var window = CreateWindow(view);
            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                var reopen = ControlFinder.FindByAutomationId<Button>(
                    view,
                    "WorkspaceReopenResolvedButton").ShouldNotBeNull();

                await publishOutcomeAsync(viewModel);
                Dispatcher.UIThread.RunJobs();

                reopen.IsEffectivelyVisible.ShouldBeTrue();
                reopen.IsEnabled.ShouldBeTrue();
                reopen.Content.ShouldBe(expectedLabel);
            }
            finally
            {
                window.Close();
            }

            return WorkspaceChangesDialogResult.KeepEditing;
        };

        var reservation = await context.ViewModel.ReserveLeaveAsync(WorkspaceLeaveReason.CloseWorkspace);

        reservation.ShouldBeNull();
        wasPresented.ShouldBeTrue();
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Creates one terminal recovery result for the supplied reviewed status and evidence.</summary>
    /// <param name="pending">The original pending save identity.</param>
    /// <param name="evidence">The complete terminal resolved evidence.</param>
    /// <param name="status">The exact committed or not-committed outcome.</param>
    /// <returns>The adoptable terminal recovery result.</returns>
    private static RecoverSaveResult CreateTerminalRecovery(
        PendingSaveIdentity pending,
        ResolvedOutputEvidence evidence,
        RecoverSaveStatus status)
    {
        return new RecoverSaveResult(
            pending.OriginalWorkspaceId,
            pending.SaveOperationId,
            status,
            pending.SaveBaseRevision,
            repairRequired: false,
            evidence.EvidenceToken,
            evidence,
            error: null);
    }

    /// <summary>Shows and verifies the complete detached comparison surface while the review dialog owns admission.</summary>
    /// <param name="viewModel">The active review lifecycle.</param>
    /// <param name="request">The exact active review request.</param>
    private static void ShowPopulatedReview(
        WorkspaceChangesViewModel viewModel,
        WorkspaceChangesDialogRequest request)
    {
        var view = new WorkspaceChangesView(viewModel, request, _ => Task.CompletedTask);
        var window = CreateWindow(view);
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            ControlFinder.FindByAutomationId<TextBlock>(view, "WorkspaceChangesTitle")!
                .Text.ShouldBe("Review Changes");
            ControlFinder.FindByAutomationId<TextBlock>(view, "WorkspaceChangesPurposeText")!
                .Text.ShouldBe("Review form changes and changes applied to the workspace.");
            ControlFinder.FindByAutomationId<TextBlock>(view, "WorkspaceUnappliedDraftWarning")!
                .IsEffectivelyVisible.ShouldBeTrue();
            ControlFinder.FindByAutomationId<ItemsControl>(view, "WorkspaceChangeItems")!
                .ItemCount.ShouldBe(1);
            var item = viewModel.CurrentReview.ShouldNotBeNull().Items.ShouldHaveSingleItem();
            var stableId = CreateStableFormKeyId(item.FormKey.ToString());
            ControlFinder.FindByAutomationId<Expander>(view, $"WorkspaceChange_{stableId}")!
                .IsExpanded.ShouldBeTrue();
            ControlFinder.FindByAutomationId<TreeDataGrid>(view, $"WorkspaceChange_{stableId}_PriorTree")
                .ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TreeDataGrid>(view, $"WorkspaceChange_{stableId}_ResultTree")
                .ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "WorkspaceKeepEditingButton")!
                .IsDefault.ShouldBeTrue();
            ControlFinder.FindByAutomationId<Button>(view, "WorkspaceSaveAndProceedButton")!
                .IsVisible.ShouldBeFalse();
            ControlFinder.FindByAutomationId<Button>(view, "WorkspaceDiscardAndProceedButton")!
                .IsVisible.ShouldBeFalse();
            WriteScreenshot(window, "plugin-workspace-changes-review.png");
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Shows and verifies the blocked recovery surface while the leave dialog owns admission.</summary>
    /// <param name="viewModel">The active blocked lifecycle.</param>
    /// <param name="request">The exact active leave request.</param>
    private static void ShowRecoveryRequired(
        WorkspaceChangesViewModel viewModel,
        WorkspaceChangesDialogRequest request)
    {
        var view = new WorkspaceChangesView(viewModel, request, _ => Task.CompletedTask);
        var window = CreateWindow(view);
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            viewModel.OutputSynchronizationStatus.ShouldBe(OutputSynchronizationStatus.RecoveryRequired);
            ControlFinder.FindByAutomationId<TextBlock>(view, "WorkspaceChangesTitle")!
                .Text.ShouldBe("Review Changes");
            ControlFinder.FindByAutomationId<TextBlock>(view, "WorkspaceChangesPurposeText")!
                .Text.ShouldBe("Resolve current work before exiting CreationsForge.");
            var inspect = ControlFinder.FindByAutomationId<Button>(view, "WorkspaceInspectRecoveryButton")
                .ShouldNotBeNull();
            inspect.IsEffectivelyVisible.ShouldBeTrue();
            inspect.IsEnabled.ShouldBeTrue();
            ControlFinder.FindByAutomationId<Button>(view, "WorkspaceCompletePreparedButton")!
                .IsVisible.ShouldBeFalse();
            ControlFinder.FindByAutomationId<Button>(view, "WorkspaceRestoreBaselineButton")!
                .IsVisible.ShouldBeFalse();
            var save = ControlFinder.FindByAutomationId<Button>(view, "WorkspaceSaveAndProceedButton")
                .ShouldNotBeNull();
            save.IsEffectivelyVisible.ShouldBeTrue();
            save.IsEnabled.ShouldBeFalse();
            var discard = ControlFinder.FindByAutomationId<Button>(view, "WorkspaceDiscardAndProceedButton")
                .ShouldNotBeNull();
            discard.IsEffectivelyVisible.ShouldBeTrue();
            discard.IsEnabled.ShouldBeFalse();
            ControlFinder.FindByAutomationId<TextBlock>(view, "WorkspaceSaveDisabledReason")!
                .Text.ShouldNotBeNullOrWhiteSpace();
            ControlFinder.FindByAutomationId<TextBlock>(view, "WorkspaceDiscardDisabledReason")!
                .Text.ShouldNotBeNullOrWhiteSpace();
            ControlFinder.FindByAutomationId<Button>(view, "WorkspaceKeepEditingButton")!
                .IsDefault.ShouldBeTrue();
            WriteScreenshot(window, "plugin-workspace-changes-recovery-required.png");
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Creates a fixed-size review host for deterministic control measurement and rendering.</summary>
    /// <param name="view">The active changes control.</param>
    /// <returns>The configured headless window.</returns>
    private static Window CreateWindow(WorkspaceChangesView view)
    {
        return new Window
        {
            Width = 1280,
            Height = 900,
            Content = view
        };
    }

    /// <summary>Writes an opt-in real-Skia rendered frame under the task-owned screenshot directory.</summary>
    /// <param name="window">The visible headless review host.</param>
    /// <param name="fileName">The stable artifact file name.</param>
    private static void WriteScreenshot(Window window, string fileName)
    {
        if (!HeadlessTestApp.UsesRenderedArtifactRenderer)
        {
            return;
        }

        using var bitmap = window.CaptureRenderedFrame().ShouldNotBeNull();
        var screenshotPath = GetScreenshotPath(fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath)!);
        bitmap.Save(screenshotPath, PngBitmapEncoderOptions.Default);
        new FileInfo(screenshotPath).Length.ShouldBeGreaterThan(0L);
    }

    /// <summary>Verifies one closed action is absent from the effective active-editor layout.</summary>
    /// <param name="view">The active changes view.</param>
    /// <param name="automationId">The closed action automation identity.</param>
    private static void AssertHidden(WorkspaceChangesView view, string automationId)
    {
        ControlFinder.FindByAutomationId<Button>(view, automationId)
            .ShouldNotBeNull()
            .IsEffectivelyVisible.ShouldBeFalse();
    }

    /// <summary>Gets the repository-relative destination for one changes-dialog review artifact.</summary>
    /// <param name="fileName">The stable PNG file name.</param>
    /// <returns>The fully qualified artifact path.</returns>
    private static string GetScreenshotPath(string fileName)
    {
        return Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            ".work",
            "PluginAuthoring",
            "screenshots",
            fileName));
    }

    /// <summary>Publishes an unresolved original save identity for the blocked recovery dialog.</summary>
    /// <param name="context">The ready workspace context to move into recovery-required state.</param>
    private static void SetRecoveryRequired(WorkspaceChangesTestContext context)
    {
        var workspace = context.Workspace!;
        var state = workspace.State;
        var pending = new PendingSaveIdentity(
            workspace.WorkspaceId,
            Guid.NewGuid(),
            state.Revision,
            state.Game,
            state.Release,
            state.Output!);
        workspace.State = new WorkspaceState(
            state.Game,
            state.Release,
            state.Output,
            state.OutputBaseline,
            new OutputSynchronizationState(OutputSynchronizationStatus.RecoveryRequired, pending),
            state.Revision);
    }

    /// <summary>Creates the same deterministic automation-safe FormKey suffix as the production view.</summary>
    /// <param name="formKey">The exact FormKey text.</param>
    /// <returns>A non-empty suffix containing only letters, digits, and underscores.</returns>
    private static string CreateStableFormKeyId(string formKey)
    {
        var value = new string(formKey.Select(character => char.IsLetterOrDigit(character) ? character : '_').ToArray());
        return string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
    }
}
