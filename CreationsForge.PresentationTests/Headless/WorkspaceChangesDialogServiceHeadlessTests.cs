using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.Headless;

/// <summary>Verifies the production plugin changes dialog owns asynchronous modal work and queued close requests on Avalonia's UI thread.</summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class WorkspaceChangesDialogServiceHeadlessTests
{
    /// <summary>Bounds asynchronous modal lifecycle assertions.</summary>
    private static readonly TimeSpan AsyncDeadline = TimeSpan.FromSeconds(5);

    /// <summary>Verifies Review opens and dismisses the real modal after preview resumes on a worker thread.</summary>
    /// <returns>A task that completes after the real modal and transition have drained.</returns>
    [AvaloniaFact]
    public Task ReviewChangesAsync_DelayedPreview_ShowsRealModalAndReleasesTransition()
    {
        return VerifyToolbarModalAsync(
            (viewModel, cancellationToken) => viewModel.ReviewChangesAsync(cancellationToken),
            WorkspaceChangesDialogPurpose.Review,
            "Done");
    }

    /// <summary>Verifies Save opens and dismisses the real modal after preview resumes on a worker thread.</summary>
    /// <returns>A task that completes after the real modal and transition have drained.</returns>
    [AvaloniaFact]
    public Task ShowSaveChangesDialogAsync_DelayedPreview_ShowsRealModalAndReleasesTransition()
    {
        return VerifyToolbarModalAsync(
            (viewModel, cancellationToken) => viewModel.ShowSaveChangesDialogAsync(cancellationToken),
            WorkspaceChangesDialogPurpose.Save,
            "Keep Editing");
    }

    /// <summary>Verifies Discard opens and dismisses the real modal after preview resumes on a worker thread.</summary>
    /// <returns>A task that completes after the real modal and transition have drained.</returns>
    [AvaloniaFact]
    public Task ShowDiscardChangesDialogAsync_DelayedPreview_ShowsRealModalAndReleasesTransition()
    {
        return VerifyToolbarModalAsync(
            (viewModel, cancellationToken) => viewModel.ShowDiscardChangesDialogAsync(cancellationToken),
            WorkspaceChangesDialogPurpose.Discard,
            "Keep Editing");
    }

    /// <summary>Verifies dirty Close uses the real close handler after preview resumes on a worker thread.</summary>
    /// <returns>A task that completes after the real modal and transition have drained.</returns>
    [AvaloniaFact]
    public async Task ReserveLeaveAsync_DelayedDirtyPreview_CloseGestureReleasesTransition()
    {
        var uiThreadId = Environment.CurrentManagedThreadId;
        var owner = new Window();
        var windowService = new HeadlessApplicationWindowService(owner);
        var dialogService = new WorkspaceChangesDialogService(
            windowService,
            new LoggerConfiguration().CreateLogger());
        await using var context = WorkspaceChangesTestContext.CreateReadyWithPresentationServices(
            dialogService,
            new AvaloniaUiDispatcher(),
            hasStagedChanges: true);
        var preview = DelayPreview(context);
        Task<WorkspaceLeaveReservation?>? leaveTask = null;
        Window? dialog = null;

        try
        {
            owner.Show();
            leaveTask = context.ViewModel.ReserveLeaveAsync(
                WorkspaceLeaveReason.CloseWorkspace,
                TestContext.Current.CancellationToken).AsTask();
            await ReleasePreviewFromWorkerAsync(preview, uiThreadId);
            dialog = await windowService.DialogShown.Task.WaitAsync(AsyncDeadline);

            AssertProductionDialog(dialog, context.ViewModel, uiThreadId, windowService);
            dialog.Close();

            (await leaveTask.WaitAsync(AsyncDeadline)).ShouldBeNull();
            AssertTransitionReleased(context);
        }
        finally
        {
            preview.Release.TrySetResult(true);
            DismissForCleanup(dialog);
            await DrainForCleanupAsync(leaveTask);
            owner.Close();
        }
    }

    /// <summary>Verifies the real modal accepts Discard and Proceed after a definitive staged-save validation failure.</summary>
    /// <returns>A task that completes after the modal closes and a clean leave reservation is transferred.</returns>
    [AvaloniaFact]
    public async Task ReserveLeaveAsync_FailedSaveThenDiscardButton_LeavesCleanWorkspace()
    {
        var owner = new Window();
        var windowService = new HeadlessApplicationWindowService(owner);
        var dialogService = new WorkspaceChangesDialogService(
            windowService,
            new LoggerConfiguration().CreateLogger());
        await using var context = WorkspaceChangesTestContext.CreateReadyWithPresentationServices(
            dialogService,
            new AvaloniaUiDispatcher(),
            hasStagedChanges: true);
        var workspace = context.Workspace.ShouldNotBeNull();
        var originalState = workspace.State;
        var discardedRevision = originalState.Revision.Next();
        workspace.OnSaveAsync = (request, _) => ValueTask.FromResult(new SaveResult(
            workspace.WorkspaceId,
            request.OperationId,
            request.ExpectedRevision,
            request.ExpectedRevision,
            SaveCommitStatus.NotCommitted,
            committedBaseline: null,
            recoveryEvidenceToken: null,
            resolvedEvidence: null,
            new EngineError(EngineErrorCode.ValidationFailed, "The staged output changed a translated field."),
            warnings: []));
        workspace.OnDiscardAsync = (request, _) =>
        {
            workspace.State = new WorkspaceState(
                originalState.Game,
                originalState.Release,
                originalState.Output,
                originalState.OutputBaseline,
                new OutputSynchronizationState(OutputSynchronizationStatus.Ready, null),
                discardedRevision);
            workspace.Preview = new WorkspacePreview([], 0, []);
            return ValueTask.FromResult(EngineResult<OperationReceipt>.Success(
                new OperationReceipt(request.OperationId, discardedRevision),
                workspace.WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                discardedRevision));
        };
        context.EditParticipant.OnRefreshAsync = (_, _, _) => Task.FromResult(
            EngineResult<WorkspaceState>.Success(
                workspace.State,
                workspace.WorkspaceId,
                baseRevision: workspace.State.Revision,
                resultRevision: workspace.State.Revision));
        var saveRejected = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        context.ViewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(WorkspaceChangesViewModel.ErrorCode)
                && context.ViewModel.ErrorCode == EngineErrorCode.ValidationFailed)
            {
                saveRejected.TrySetResult(true);
            }
        };
        Task<WorkspaceLeaveReservation?>? leaveTask = null;
        Window? dialog = null;

        try
        {
            owner.Show();
            leaveTask = context.ViewModel.ReserveLeaveAsync(
                WorkspaceLeaveReason.CloseWorkspace,
                TestContext.Current.CancellationToken).AsTask();
            dialog = await windowService.DialogShown.Task.WaitAsync(AsyncDeadline);
            var view = dialog.Content.ShouldBeOfType<WorkspaceChangesView>();
            var save = ControlFinder.FindByAutomationId<Button>(view, "WorkspaceSaveAndProceedButton").ShouldNotBeNull();
            var discard = ControlFinder.FindByAutomationId<Button>(view, "WorkspaceDiscardAndProceedButton").ShouldNotBeNull();

            save.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            await saveRejected.Task.WaitAsync(AsyncDeadline);
            discard.IsEnabled.ShouldBeTrue();
            discard.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            using var reservation = (await leaveTask.WaitAsync(AsyncDeadline)).ShouldNotBeNull();
            reservation.Disposition.ShouldBe(WorkspaceLeaveDisposition.ReadyAndClean);
            workspace.DiscardRequests.Count.ShouldBe(1);
        }
        finally
        {
            DismissForCleanup(dialog);
            await DrainForCleanupAsync(leaveTask);
            owner.Close();
        }
    }

    /// <summary>Verifies a window close before any drain choice keeps the captured editor active and uncanceled.</summary>
    /// <returns>A task that completes after the real modal abandons its pending transition request.</returns>
    [AvaloniaFact]
    public async Task ReserveLeaveAsync_ActiveEditor_WindowCloseBeforeChoiceKeepsEditing()
    {
        await using var fixture = await ActiveEditorDialogFixture.CreateAsync();

        fixture.Dialog.Close();

        (await fixture.LeaveTask.WaitAsync(AsyncDeadline)).ShouldBeNull();
        fixture.EditorLease.IsActive.ShouldBeTrue();
        fixture.EditorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
        fixture.Context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies queued caller cancellation rejects a synchronously raised cancel-and-wait choice.</summary>
    /// <returns>A task that completes after caller cancellation closes the real modal without touching the editor token.</returns>
    [AvaloniaFact]
    public async Task ReserveLeaveAsync_ActiveEditor_CallerCancellationBeforeCancelAndWaitRejectsChoice()
    {
        using var callerCancellation = new CancellationTokenSource();
        await using var fixture = await ActiveEditorDialogFixture.CreateAsync(callerCancellation.Token);
        var cancelAndWait = ControlFinder.FindByAutomationId<Button>(
            fixture.View,
            "WorkspaceCancelEditorOperationAndWaitButton").ShouldNotBeNull();

        callerCancellation.Cancel();
        cancelAndWait.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        fixture.EditorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
        (await fixture.LeaveTask.WaitAsync(AsyncDeadline)).ShouldBeNull();
        fixture.EditorLease.IsActive.ShouldBeTrue();
        fixture.EditorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
        fixture.Context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies a close during natural wait drains first and overrides a newly proved clean leave.</summary>
    /// <returns>A task that completes after the captured editor drains and the queued close keeps editing.</returns>
    [AvaloniaFact]
    public async Task ReserveLeaveAsync_ActiveEditor_WindowCloseDuringWaitOverridesProceed()
    {
        await using var fixture = await ActiveEditorDialogFixture.CreateAsync();
        var wait = ControlFinder.FindByAutomationId<Button>(
            fixture.View,
            "WorkspaceWaitForEditorOperationButton").ShouldNotBeNull();

        wait.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        fixture.Context.ViewModel.OperationState.ShouldBe(WorkspaceChangesOperationState.WaitingForEditorOperation);
        fixture.EditorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
        fixture.Dialog.Close();

        fixture.LeaveTask.IsCompleted.ShouldBeFalse();
        fixture.CompleteEditorOperation();
        (await fixture.LeaveTask.WaitAsync(AsyncDeadline)).ShouldBeNull();
        fixture.EditorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
        AssertTransitionReleased(fixture.Context);
    }

    /// <summary>Verifies caller cancellation during cancel-and-wait cannot truncate the exact editor drain.</summary>
    /// <returns>A task that completes after cancellation is latched, editor publication drains, and Keep Editing wins.</returns>
    [AvaloniaFact]
    public async Task ReserveLeaveAsync_ActiveEditor_CallerCancellationDuringCancelAndWaitDrainsThenKeepsEditing()
    {
        using var callerCancellation = new CancellationTokenSource();
        await using var fixture = await ActiveEditorDialogFixture.CreateAsync(callerCancellation.Token);
        var cancelAndWait = ControlFinder.FindByAutomationId<Button>(
            fixture.View,
            "WorkspaceCancelEditorOperationAndWaitButton").ShouldNotBeNull();

        cancelAndWait.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        fixture.EditorLease.CancellationToken.IsCancellationRequested.ShouldBeTrue();
        fixture.Context.ViewModel.OperationState.ShouldBe(WorkspaceChangesOperationState.CancelRequestedWaitingForResult);
        callerCancellation.Cancel();

        fixture.LeaveTask.IsCompleted.ShouldBeFalse();
        fixture.CompleteEditorOperation();
        (await fixture.LeaveTask.WaitAsync(AsyncDeadline)).ShouldBeNull();
        AssertTransitionReleased(fixture.Context);
    }

    /// <summary>Runs one toolbar modal through delayed production preview and its actual keep-editing button.</summary>
    /// <param name="showDialogAsync">The public toolbar modal entry point.</param>
    /// <param name="expectedPurpose">The exact dialog purpose expected in the visible control.</param>
    /// <param name="expectedButtonText">The purpose-specific non-destructive button text.</param>
    /// <returns>A task that completes after the modal and transition have both drained.</returns>
    private static async Task VerifyToolbarModalAsync(
        Func<WorkspaceChangesViewModel, CancellationToken, Task> showDialogAsync,
        WorkspaceChangesDialogPurpose expectedPurpose,
        string expectedButtonText)
    {
        var uiThreadId = Environment.CurrentManagedThreadId;
        var owner = new Window();
        var windowService = new HeadlessApplicationWindowService(owner);
        var dialogService = new WorkspaceChangesDialogService(
            windowService,
            new LoggerConfiguration().CreateLogger());
        await using var context = WorkspaceChangesTestContext.CreateReadyWithPresentationServices(
            dialogService,
            new AvaloniaUiDispatcher(),
            hasStagedChanges: true);
        var preview = DelayPreview(context);
        Task? showTask = null;
        Window? dialog = null;

        try
        {
            owner.Show();
            showTask = showDialogAsync(context.ViewModel, TestContext.Current.CancellationToken);
            await ReleasePreviewFromWorkerAsync(preview, uiThreadId);
            dialog = await windowService.DialogShown.Task.WaitAsync(AsyncDeadline);

            var view = AssertProductionDialog(dialog, context.ViewModel, uiThreadId, windowService);
            ControlFinder.FindByAutomationId<TextBlock>(view, "WorkspaceChangesPurposeText")!
                .Text.ShouldBe(ExpectedPurposeText(expectedPurpose));
            var keepEditing = ControlFinder.FindByAutomationId<Button>(
                view,
                "WorkspaceKeepEditingButton").ShouldNotBeNull();
            keepEditing.Content.ShouldBe(expectedButtonText);
            keepEditing.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            await showTask.WaitAsync(AsyncDeadline);
            AssertTransitionReleased(context);
        }
        finally
        {
            preview.Release.TrySetResult(true);
            DismissForCleanup(dialog);
            await DrainForCleanupAsync(showTask);
            owner.Close();
        }
    }

    /// <summary>Installs one preview that cannot finish until a worker thread releases it.</summary>
    /// <param name="context">The ready workspace context whose preview is delayed.</param>
    /// <returns>The preview synchronization signals.</returns>
    private static DelayedPreview DelayPreview(WorkspaceChangesTestContext context)
    {
        var preview = new DelayedPreview();
        var workspace = context.Workspace.ShouldNotBeNull();
        workspace.OnPreviewAsync = async cancellationToken =>
        {
            preview.Started.TrySetResult(true);
            await preview.Release.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
            preview.Continuation.TrySetResult(new PreviewContinuation(
                Environment.CurrentManagedThreadId,
                Dispatcher.UIThread.CheckAccess()));
            return EngineResult<WorkspacePreview>.Success(
                workspace.Preview,
                workspace.WorkspaceId,
                baseRevision: workspace.Revision,
                resultRevision: workspace.Revision);
        };
        return preview;
    }

    /// <summary>Releases delayed preview on a worker and proves its continuation did not run on the UI thread.</summary>
    /// <param name="preview">The preview synchronization signals.</param>
    /// <param name="uiThreadId">The Avalonia UI thread's managed identity.</param>
    /// <returns>A task that completes after the worker continuation is observed.</returns>
    private static async Task ReleasePreviewFromWorkerAsync(DelayedPreview preview, int uiThreadId)
    {
        await preview.Started.Task.WaitAsync(AsyncDeadline);
        await Task.Run(() => preview.Release.TrySetResult(true));
        var continuation = await preview.Continuation.Task.WaitAsync(AsyncDeadline);
        continuation.ThreadId.ShouldNotBe(uiThreadId);
        continuation.HasUiThreadAccess.ShouldBeFalse();
    }

    /// <summary>Verifies the production service supplied and presented its real view on the recorded UI thread.</summary>
    /// <param name="dialog">The visible production dialog.</param>
    /// <param name="viewModel">The change lifecycle expected as the dialog data context.</param>
    /// <param name="uiThreadId">The test's Avalonia UI-thread identity.</param>
    /// <param name="windowService">The headless owner service that recorded presentation.</param>
    /// <returns>The real workspace changes view.</returns>
    private static WorkspaceChangesView AssertProductionDialog(
        Window dialog,
        WorkspaceChangesViewModel viewModel,
        int uiThreadId,
        HeadlessApplicationWindowService windowService)
    {
        Dispatcher.UIThread.CheckAccess().ShouldBeTrue();
        windowService.PresentationThreadId.ShouldBe(uiThreadId);
        dialog.IsVisible.ShouldBeTrue();
        var view = dialog.Content.ShouldBeOfType<WorkspaceChangesView>();
        view.DataContext.ShouldBeSameAs(viewModel);
        ControlFinder.FindByAutomationId<WorkspaceChangesView>(
            view,
            "WorkspaceChangesView").ShouldBeSameAs(view);
        return view;
    }

    /// <summary>Verifies transition admission reopened after the complete modal session returned.</summary>
    /// <param name="context">The completed modal test context.</param>
    private static void AssertTransitionReleased(WorkspaceChangesTestContext context)
    {
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
        using var nextEditorOperation = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
    }

    /// <summary>Gets the user-visible text belonging to one toolbar dialog purpose.</summary>
    /// <param name="purpose">The closed toolbar dialog purpose.</param>
    /// <returns>The exact purpose text rendered by the production view.</returns>
    private static string ExpectedPurposeText(WorkspaceChangesDialogPurpose purpose)
    {
        return purpose switch
        {
            WorkspaceChangesDialogPurpose.Review => "Review form changes and changes applied to the workspace.",
            WorkspaceChangesDialogPurpose.Save => "Review every staged change before saving the complete selected output.",
            WorkspaceChangesDialogPurpose.Discard => "Review form changes and changes applied to the workspace before discarding them.",
            _ => throw new ArgumentOutOfRangeException(nameof(purpose), purpose, "The test requires a toolbar dialog purpose.")
        };
    }

    /// <summary>Best-effort dismisses a still-visible changes modal during failed-test cleanup.</summary>
    /// <param name="dialog">The optional dialog to dismiss.</param>
    private static void DismissForCleanup(Window? dialog)
    {
        if (dialog?.IsVisible != true || dialog.Content is not WorkspaceChangesView view)
        {
            return;
        }

        var keepEditing = ControlFinder.FindByAutomationId<Button>(view, "WorkspaceKeepEditingButton");
        keepEditing?.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
    }

    /// <summary>Best-effort drains an optional task within the bounded test deadline.</summary>
    /// <param name="task">The task to drain, or <see langword="null"/> when it never started.</param>
    /// <returns>A task that completes after cleanup or timeout.</returns>
    private static async Task DrainForCleanupAsync(Task? task)
    {
        if (task is null)
        {
            return;
        }

        try
        {
            await task.WaitAsync(AsyncDeadline);
        }
        catch
        { }
    }

    /// <summary>Owns the signals that prove preview crossed a real asynchronous worker boundary.</summary>
    private sealed class DelayedPreview
    {
        /// <summary>Signals that the workspace preview request began.</summary>
        internal TaskCompletionSource<bool> Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        /// <summary>Releases the pending workspace preview.</summary>
        internal TaskCompletionSource<bool> Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        /// <summary>Records the thread on which the delayed preview continuation ran.</summary>
        internal TaskCompletionSource<PreviewContinuation> Continuation { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Records the delayed preview continuation's thread identity and dispatcher access.</summary>
    private sealed class PreviewContinuation
    {
        /// <summary>Initializes one preview continuation observation.</summary>
        /// <param name="threadId">The managed thread identity.</param>
        /// <param name="hasUiThreadAccess">Whether the continuation ran on Avalonia's UI thread.</param>
        internal PreviewContinuation(int threadId, bool hasUiThreadAccess)
        {
            ThreadId = threadId;
            HasUiThreadAccess = hasUiThreadAccess;
        }

        /// <summary>Gets the managed thread identity.</summary>
        internal int ThreadId { get; }

        /// <summary>Gets whether the continuation ran on Avalonia's UI thread.</summary>
        internal bool HasUiThreadAccess { get; }
    }

    /// <summary>Owns one real modal, active editor lease, and ready workspace for close-latch integration tests.</summary>
    private sealed class ActiveEditorDialogFixture : IAsyncDisposable
    {
        /// <summary>Initializes the fully presented active-editor fixture.</summary>
        /// <param name="owner">The visible headless modal owner.</param>
        /// <param name="context">The complete plugin changes presentation graph.</param>
        /// <param name="editorLease">The exact captured editor operation.</param>
        /// <param name="dialog">The visible production modal.</param>
        /// <param name="view">The real production dialog content.</param>
        /// <param name="leaveTask">The active public leave request.</param>
        private ActiveEditorDialogFixture(
            Window owner,
            WorkspaceChangesTestContext context,
            WorkspaceEditorOperationLease editorLease,
            Window dialog,
            WorkspaceChangesView view,
            Task<WorkspaceLeaveReservation?> leaveTask)
        {
            Owner = owner;
            Context = context;
            EditorLease = editorLease;
            Dialog = dialog;
            View = view;
            LeaveTask = leaveTask;
        }

        /// <summary>Gets the visible modal owner.</summary>
        private Window Owner { get; }

        /// <summary>Gets the complete plugin changes presentation graph.</summary>
        internal WorkspaceChangesTestContext Context { get; }

        /// <summary>Gets the exact editor operation captured by the pending transition.</summary>
        internal WorkspaceEditorOperationLease EditorLease { get; }

        /// <summary>Gets the visible production modal.</summary>
        internal Window Dialog { get; }

        /// <summary>Gets the real production dialog content.</summary>
        internal WorkspaceChangesView View { get; }

        /// <summary>Gets the active public leave request.</summary>
        internal Task<WorkspaceLeaveReservation?> LeaveTask { get; }

        /// <summary>Creates and presents one real active-editor modal around a clean ready workspace.</summary>
        /// <param name="cancellationToken">The caller token passed to the public leave request.</param>
        /// <returns>The fully presented fixture.</returns>
        internal static async Task<ActiveEditorDialogFixture> CreateAsync(
            CancellationToken cancellationToken = default)
        {
            var uiThreadId = Environment.CurrentManagedThreadId;
            var owner = new Window();
            var windowService = new HeadlessApplicationWindowService(owner);
            var dialogService = new WorkspaceChangesDialogService(
                windowService,
                new LoggerConfiguration().CreateLogger());
            var context = WorkspaceChangesTestContext.CreateReadyWithPresentationServices(
                dialogService,
                new AvaloniaUiDispatcher());
            var editorLease = context.Arbiter.TryBeginEditorOperation().ShouldNotBeNull();
            context.EditParticipant.SetState(
                hasDraftChanges: false,
                isEditorBusy: true,
                operationState: FormListEditorOperationState.Applying);
            owner.Show();
            var leaveTask = context.ViewModel.ReserveLeaveAsync(
                WorkspaceLeaveReason.CloseWorkspace,
                cancellationToken).AsTask();
            var dialog = await windowService.DialogShown.Task.WaitAsync(AsyncDeadline);
            var view = AssertProductionDialog(dialog, context.ViewModel, uiThreadId, windowService);
            context.ViewModel.ShowActiveEditorOperationDecision.ShouldBeTrue();
            return new ActiveEditorDialogFixture(owner, context, editorLease, dialog, view, leaveTask);
        }

        /// <summary>Publishes a definitive idle editor state and releases the exact captured operation.</summary>
        internal void CompleteEditorOperation()
        {
            Context.EditParticipant.SetState(hasDraftChanges: false);
            EditorLease.Dispose();
        }

        /// <summary>Releases retained UI and lifecycle resources after each modal scenario.</summary>
        /// <returns>A task that completes after best-effort modal and leave-request drain.</returns>
        public async ValueTask DisposeAsync()
        {
            CompleteEditorOperation();
            DismissForCleanup(Dialog);
            await DrainForCleanupAsync(LeaveTask);
            await Context.DisposeAsync();
            Owner.Close();
        }
    }

    /// <summary>Shows modal dialogs against a headless owner while recording their presentation thread.</summary>
    private sealed class HeadlessApplicationWindowService : IApplicationWindowService
    {
        /// <summary>The visible owner used by Avalonia modal lifetime.</summary>
        private readonly Window Owner;

        /// <summary>Initializes a headless modal presenter.</summary>
        /// <param name="owner">The visible modal owner.</param>
        internal HeadlessApplicationWindowService(Window owner)
        {
            Owner = owner;
        }

        /// <summary>Signals the modal dialog supplied to the presenter.</summary>
        internal TaskCompletionSource<Window> DialogShown { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        /// <summary>Gets the managed thread that presented the modal.</summary>
        internal int PresentationThreadId { get; private set; }

        /// <inheritdoc />
        public void RegisterMainWindow(MainWindow mainWindow)
        { }

        /// <inheritdoc />
        public void SetContent(Control content)
        { }

        /// <inheritdoc />
        public void ClearContent(Control content)
        { }

        /// <inheritdoc />
        public void ApplyTheme(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
        { }

        /// <inheritdoc />
        public Task<TResult> ShowDialogAsync<TResult>(Window dialog)
        {
            Dispatcher.UIThread.VerifyAccess();
            PresentationThreadId = Environment.CurrentManagedThreadId;
            DialogShown.TrySetResult(dialog);
            return dialog.ShowDialog<TResult>(Owner);
        }

        /// <inheritdoc />
        public void Quit()
        { }
    }
}
