using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.Headless;

/// <summary>
/// Verifies the native workspace selection form exposes every required picker and output capability control headlessly.
/// </summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class NativeWorkspaceSelectionViewHeadlessTests
{
    /// <summary>Bounds asynchronous UI and engine coordination so a regression cannot hang the test process.</summary>
    private static readonly TimeSpan AsyncDeadline = TimeSpan.FromSeconds(5);

    /// <summary>Verifies the complete explicit input and output selection surface can be rendered by Avalonia.</summary>
    [AvaloniaFact]
    public void NativeWorkspaceSelectionView_ShowHeadlessly_ExposesRequiredSelectionControls()
    {
        var dispatcher = new InlineUiDispatcher();
        var factory = new FakeFormListWorkspaceFactory((request, _) =>
            ValueTask.FromResult(EngineResult<IFormListWorkspace>.Failure(
                new EngineError(EngineErrorCode.SourceOpenFailed, "Test factory does not open records."),
                workspaceId: request.WorkspaceId)));
        var coordinator = new NativeWorkspaceCoordinator(factory, dispatcher, new LoggerConfiguration().CreateLogger());
        var viewModel = new NativeWorkspaceSelectionViewModel(
            coordinator,
            new FakeNativeWorkspacePathPicker(),
            new FakeGameSelectionService(),
            dispatcher,
            new LoggerConfiguration().CreateLogger());
        var view = new NativeWorkspaceSelectionView(viewModel, _ => { });
        var window = new Window
        {
            Width = 980,
            Height = 820,
            Content = view
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            ControlFinder.FindByAutomationId<NativeWorkspaceSelectionView>(view, "NativeWorkspaceSelectionView").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ComboBox>(view, "NativeWorkspaceGameSelector")!.ItemCount.ShouldBe(3);
            ControlFinder.FindByAutomationId<TextBox>(view, "SourcePluginPath").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ListBox>(view, "NativeWorkspaceLoadOrderList").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBox>(view, "DataDirectoryPath").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ListBox>(view, "NativeWorkspaceStringDirectoryList").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ComboBox>(view, "OutputModeSelector")!.ItemCount.ShouldBe(2);
            ControlFinder.FindByAutomationId<TextBox>(view, "OutputPluginPath").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ComboBox>(view, "LocalizedOutputModeSelector")!.ItemCount.ShouldBe(2);
            ControlFinder.FindByAutomationId<ComboBox>(view, "OutputMasterStyleSelector")!.ItemCount.ShouldBe(3);
            ControlFinder.FindByAutomationId<Button>(view, "OpenNativeWorkspaceButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "CancelNativeWorkspaceButton").ShouldNotBeNull();
        }
        finally
        {
            window.Close();
            coordinator.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }

    /// <summary>Verifies title-bar dismissal cancels and drains acquisition before the modal dialog returns.</summary>
    [AvaloniaFact]
    public async Task NativeWorkspaceSelectionDialog_CloseWhileOpening_PreservesPriorWorkspaceAndDisposesCandidate()
    {
        var original = CreateSuccessfulWorkspace();
        var selectionStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var cancellationObserved = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseSelection = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseCleanup = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var candidate = new FakeFormListWorkspace(
            Guid.NewGuid(),
            new WorkspaceRevision(Guid.NewGuid(), 0),
            async (_, cancellationToken) =>
            {
                selectionStarted.TrySetResult(true);
                try
                {
                    var cancellationWait = Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
                    var completed = await Task.WhenAny(cancellationWait, releaseSelection.Task);
                    if (ReferenceEquals(completed, releaseSelection.Task))
                    {
                        return EngineResult<OutputSelectionReceipt>.Failure(
                            new EngineError(EngineErrorCode.OutputOpenFailed, "The test released output selection during cleanup."));
                    }

                    await cancellationWait;
                    throw new InvalidOperationException("The test output selection was released without cancellation.");
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    cancellationObserved.TrySetResult(true);
                    await releaseCleanup.Task;
                    throw;
                }
            });
        var workspaces = new Queue<IFormListWorkspace>([original, candidate]);
        var coordinator = new NativeWorkspaceCoordinator(
            new FakeFormListWorkspaceFactory((_, _) =>
                ValueTask.FromResult(EngineResult<IFormListWorkspace>.Success(workspaces.Dequeue()))),
            new InlineUiDispatcher(),
            new LoggerConfiguration().CreateLogger());
        var viewModel = new NativeWorkspaceSelectionViewModel(
            coordinator,
            new FakeNativeWorkspacePathPicker(),
            new FakeGameSelectionService(),
            new InlineUiDispatcher(),
            new LoggerConfiguration().CreateLogger());
        PopulateRequiredPaths(viewModel);
        var owner = new Window();
        var windowService = new HeadlessApplicationWindowService(owner);
        var dialogService = new NativeWorkspaceSelectionDialogService(windowService);
        Task<bool>? showTask = null;
        Task<bool>? openTask = null;
        Window? dialog = null;

        try
        {
            var initialResult = await coordinator.OpenAsync(CreateRequest()).AsTask().WaitAsync(AsyncDeadline);
            owner.Show();
            showTask = dialogService.ShowAsync(viewModel);
            dialog = await windowService.DialogShown.Task.WaitAsync(AsyncDeadline);
            openTask = viewModel.OpenWorkspaceAsync();
            await selectionStarted.Task.WaitAsync(AsyncDeadline);

            dialog.Close(false);
            await cancellationObserved.Task.WaitAsync(AsyncDeadline);
            showTask.IsCompleted.ShouldBeFalse();
            releaseCleanup.TrySetResult(true);
            var dialogResult = await showTask.WaitAsync(AsyncDeadline);

            dialogResult.ShouldBeFalse();
            (await openTask.WaitAsync(AsyncDeadline)).ShouldBeFalse();
            coordinator.CurrentWorkspace.ShouldBeSameAs(initialResult.Value);
            original.DisposeCount.ShouldBe(0);
            candidate.DisposeCount.ShouldBe(1);
        }
        finally
        {
            viewModel.CancelOpen();
            releaseSelection.TrySetResult(true);
            releaseCleanup.TrySetResult(true);
            await DrainForCleanupAsync(openTask);
            if (dialog?.IsVisible == true)
            {
                dialog.Close(false);
            }

            await DrainForCleanupAsync(showTask);
            owner.Close();
            await DrainForCleanupAsync(coordinator.DisposeAsync().AsTask());
        }
    }

    /// <summary>Verifies dismissal after UI publication drains the operation and returns its committed activation.</summary>
    [AvaloniaFact]
    public async Task NativeWorkspaceSelectionDialog_CloseAfterPublication_KeepsCommittedWorkspaceAndReturnsSuccess()
    {
        var candidate = CreateSuccessfulWorkspace();
        var dispatcher = new QueuedUiDispatcher();
        var coordinator = new NativeWorkspaceCoordinator(
            new FakeFormListWorkspaceFactory((_, _) =>
                ValueTask.FromResult(EngineResult<IFormListWorkspace>.Success(candidate))),
            dispatcher,
            new LoggerConfiguration().CreateLogger());
        var viewModel = new NativeWorkspaceSelectionViewModel(
            coordinator,
            new FakeNativeWorkspacePathPicker(),
            new FakeGameSelectionService(),
            dispatcher,
            new LoggerConfiguration().CreateLogger());
        PopulateRequiredPaths(viewModel);
        var owner = new Window();
        var windowService = new HeadlessApplicationWindowService(owner);
        var dialogService = new NativeWorkspaceSelectionDialogService(windowService);
        Task<bool>? showTask = null;
        Task<bool>? openTask = null;
        Window? dialog = null;
        var publicationActionRunning = false;

        try
        {
            owner.Show();
            showTask = dialogService.ShowAsync(viewModel);
            dialog = await windowService.DialogShown.Task.WaitAsync(AsyncDeadline);
            openTask = viewModel.OpenWorkspaceAsync();
            await WaitForInvocationAsync(dispatcher);
            dispatcher.RunNextActionWithoutCompleting();
            publicationActionRunning = true;
            coordinator.CurrentWorkspace.ShouldNotBeNull();

            dialog.Close(false);
            showTask.IsCompleted.ShouldBeFalse();
            dispatcher.CompleteRunningInvocation();
            publicationActionRunning = false;

            (await showTask.WaitAsync(AsyncDeadline)).ShouldBeTrue();
            (await openTask.WaitAsync(AsyncDeadline)).ShouldBeTrue();
            coordinator.CurrentWorkspace!.WorkspaceId.ShouldBe(candidate.WorkspaceId);
            candidate.DisposeCount.ShouldBe(0);
        }
        finally
        {
            viewModel.CancelOpen();
            if (publicationActionRunning)
            {
                dispatcher.CompleteRunningInvocation();
            }

            await DrainForCleanupAsync(openTask);
            if (dialog?.IsVisible == true)
            {
                dialog.Close(false);
            }

            await DrainForCleanupAsync(showTask);
            var disposal = coordinator.DisposeAsync().AsTask();
            if (!disposal.IsCompleted)
            {
                try
                {
                    await WaitForInvocationAsync(dispatcher);
                    dispatcher.RunNextInvocation();
                }
                catch (OperationCanceledException)
                { }
            }

            await DrainForCleanupAsync(disposal);
            owner.Close();
        }
    }

    /// <summary>Waits for a queued dispatcher invocation without leaving an abandoned signal waiter after timeout.</summary>
    /// <param name="dispatcher">The deterministic dispatcher whose next invocation is required.</param>
    /// <returns>A task that completes when an invocation is queued.</returns>
    private static async Task WaitForInvocationAsync(QueuedUiDispatcher dispatcher)
    {
        using var cancellation = new CancellationTokenSource(AsyncDeadline);
        await dispatcher.WaitForInvocationAsync(cancellation.Token);
    }

    /// <summary>Best-effort drains a task during test cleanup without permitting a regression to hang the test process.</summary>
    /// <param name="task">The task to drain, or <see langword="null"/> when the operation never started.</param>
    /// <returns>A task that completes when cleanup finishes or its bounded deadline expires.</returns>
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

    /// <summary>Creates a workspace whose output selection succeeds with the requested association.</summary>
    /// <returns>The successful test workspace.</returns>
    private static FakeFormListWorkspace CreateSuccessfulWorkspace()
    {
        var initialRevision = new WorkspaceRevision(Guid.NewGuid(), 0);
        return new FakeFormListWorkspace(
            Guid.NewGuid(),
            initialRevision,
            (request, _) =>
            {
                var resultRevision = initialRevision.Next();
                var baseline = new OutputArtifactSetBaseline(
                    Guid.NewGuid(),
                    [new NativeArtifactAssociation(
                        Path.GetFullPath(request.Output.PluginPath),
                        NativeArtifactRole.Plugin,
                        null,
                        new NativeArtifactFingerprint(false, 0, null))]);
                return ValueTask.FromResult(EngineResult<OutputSelectionReceipt>.Success(
                    new OutputSelectionReceipt(request.Output, baseline, resultRevision),
                    operationId: request.OperationId,
                    baseRevision: request.ExpectedRevision,
                    resultRevision: resultRevision));
            });
    }

    /// <summary>Creates a complete Starfield workspace activation request without touching the file system.</summary>
    /// <returns>The complete coordinator request.</returns>
    private static NativeWorkspaceOpenRequest CreateRequest()
    {
        var root = CreateTestRoot();
        var source = Path.Combine(root, "Source.esm");
        return new NativeWorkspaceOpenRequest(
            new WorkspaceOpenRequest(
                Guid.NewGuid(),
                SupportedGame.Starfield,
                GameRelease.Starfield,
                source,
                [source],
                root,
                []),
            OutputSelectionMode.CreateNew,
            new OutputAssociation(
                Path.Combine(root, "Output.esp"),
                ModKey.FromNameAndExtension("Output.esp"),
                LocalizedOutputMode.Embedded,
                OutputMasterStyle.Full));
    }

    /// <summary>Populates structurally complete paths for a replacement acquisition.</summary>
    /// <param name="viewModel">The selection view model to populate.</param>
    private static void PopulateRequiredPaths(NativeWorkspaceSelectionViewModel viewModel)
    {
        var root = CreateTestRoot();
        var source = Path.Combine(root, "Replacement.esm");
        viewModel.SourcePluginPath = source;
        viewModel.LoadOrderPluginPaths.Add(source);
        viewModel.DataDirectoryPath = root;
        viewModel.OutputPluginPath = Path.Combine(root, "Replacement.esp");
    }

    /// <summary>Creates a unique absolute test path without writing machine-specific state.</summary>
    /// <returns>The unique path.</returns>
    private static string CreateTestRoot()
    {
        return Path.Combine(Path.GetTempPath(), "CreationsForge-PresentationTests", Guid.NewGuid().ToString("N"));
    }

    /// <summary>Shows modal dialogs against a headless owner while exposing the shown dialog to the test.</summary>
    private sealed class HeadlessApplicationWindowService : IApplicationWindowService
    {
        /// <summary>The visible owner used by Avalonia's modal-window lifetime.</summary>
        private readonly Window Owner;

        /// <summary>Initializes a headless modal presenter.</summary>
        /// <param name="owner">The visible owner for the modal dialog.</param>
        public HeadlessApplicationWindowService(Window owner)
        {
            Owner = owner;
        }

        /// <summary>Signals the modal dialog supplied to the presenter.</summary>
        public TaskCompletionSource<Window> DialogShown { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

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
            DialogShown.TrySetResult(dialog);
            return dialog.ShowDialog<TResult>(Owner);
        }

        /// <inheritdoc />
        public void Quit()
        { }
    }
}
