using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.PresentationTests.Support;
using CreationsForge.PresentationTests.ViewModels;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.Headless;

/// <summary>Verifies the native reference picker control surface and modal cancellation lifecycle headlessly.</summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class NativeReferencePickerViewHeadlessTests
{
    /// <summary>Bounds asynchronous modal lifecycle assertions.</summary>
    private static readonly TimeSpan AsyncDeadline = TimeSpan.FromSeconds(5);

    /// <summary>Verifies search, bounded paging, provenance, explicit null, cancellation, and selection controls render.</summary>
    [AvaloniaFact]
    public async Task NativeReferencePickerView_ShowHeadlessly_ExposesCompleteBoundedSelectionSurface()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 2);
        var match = NativeReferencePickerViewModelTests.CreateMatch(0x820, "VisibleKeyword", recordType: "KYWD");
        var workspace = new ReferencePickerTestWorkspace(workspaceId, revision)
        {
            SearchAction = (_, _) => ValueTask.FromResult(
                EngineResult<ReferenceSearchPage>.Success(
                    new ReferenceSearchPage([match], "next"),
                    workspaceId: workspaceId,
                    resultRevision: revision))
        };
        var dispatcher = new InlineUiDispatcher();
        var coordinator = new ReferencePickerTestCoordinator(
            workspace,
            NativeReferencePickerViewModelTests.CreateDescriptor(workspaceId, revision));
        await using var viewModel = new NativeReferencePickerViewModel(
            coordinator,
            dispatcher,
            CreateRequest(workspaceId, revision, allowNull: true),
            new LoggerConfiguration().CreateLogger());
        var view = new NativeReferencePickerView(viewModel, _ => { });
        var window = new Window
        {
            Width = 960,
            Height = 720,
            Content = view
        };

        try
        {
            window.Show();
            viewModel.Query = "Visible";
            await viewModel.SearchAsync();
            Dispatcher.UIThread.RunJobs();

            ControlFinder.FindByAutomationId<NativeReferencePickerView>(view, "NativeReferencePickerView").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBox>(view, "NativeReferencePickerQuery")!.MaxLength.ShouldBe(ReferenceSearchRequest.MaximumQueryLength);
            ControlFinder.FindByAutomationId<Button>(view, "NativeReferencePickerSearchButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ListBox>(view, "NativeReferencePickerResults")!.ItemCount.ShouldBe(1);
            ControlFinder.FindByAutomationId<Button>(view, "NativeReferencePickerNextPageButton")!.IsEnabled.ShouldBeTrue();
            ControlFinder.FindByAutomationId<Button>(view, "NativeReferencePickerNullButton")!.IsVisible.ShouldBeTrue();
            ControlFinder.FindByAutomationId<Button>(view, "NativeReferencePickerCancelButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "NativeReferencePickerSelectButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBlock>(view, "NativeReferencePickerStatusText").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBlock>(view, "NativeReferencePickerErrorText").ShouldNotBeNull();
            var renderedText = string.Join(
                "\n",
                view.GetVisualDescendants().OfType<TextBlock>().Select(block => block.Text));
            renderedText.ShouldContain("VisibleKeyword");
            renderedText.ShouldContain("KYWD");
            renderedText.ShouldContain(match.FormKey.ToString());
            renderedText.ShouldContain(match.ContainingModKey!.Value.FileName.ToString());
            renderedText.ShouldContain(match.SourcePath!);
            renderedText.ShouldContain(match.Role!.Value.ToString());
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies title-bar dismissal cancels and drains an in-flight native search before the modal returns.</summary>
    [AvaloniaFact]
    public async Task NativeReferencePickerService_CloseWhileSearching_DrainsBeforeReturningCancellation()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 3);
        var searchStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var cancellationObserved = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseCleanup = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var workspace = CreateBlockingWorkspace(workspaceId, revision, searchStarted, cancellationObserved, releaseCleanup);
        var coordinator = new ReferencePickerTestCoordinator(
            workspace,
            NativeReferencePickerViewModelTests.CreateDescriptor(workspaceId, revision));
        var owner = new Window();
        var windowService = new HeadlessApplicationWindowService(owner);
        var service = new NativeReferencePickerService(
            windowService,
            coordinator,
            new AvaloniaUiDispatcher(),
            new LoggerConfiguration().CreateLogger());
        Task<NativeReferencePickerSelection?>? pickTask = null;
        Task? searchTask = null;
        Window? dialog = null;

        try
        {
            owner.Show();
            pickTask = service.PickAsync(CreateRequest(workspaceId, revision, allowNull: true));
            dialog = await windowService.DialogShown.Task.WaitAsync(AsyncDeadline);
            var picker = (NativeReferencePickerView)dialog.Content!;
            var viewModel = (NativeReferencePickerViewModel)picker.DataContext!;
            viewModel.Query = "blocked";
            searchTask = viewModel.SearchAsync();
            await searchStarted.Task.WaitAsync(AsyncDeadline);

            dialog.Close((object?)null);
            await cancellationObserved.Task.WaitAsync(AsyncDeadline);
            pickTask.IsCompleted.ShouldBeFalse();
            releaseCleanup.TrySetResult(true);

            (await pickTask.WaitAsync(AsyncDeadline)).ShouldBeNull();
            await searchTask.WaitAsync(AsyncDeadline);
        }
        finally
        {
            releaseCleanup.TrySetResult(true);
            await DrainForCleanupAsync(searchTask);
            if (dialog?.IsVisible == true)
            {
                dialog.Close((object?)null);
            }

            await DrainForCleanupAsync(pickTask);
            owner.Close();
        }
    }

    /// <summary>Verifies an external cancellation token drains active work and preserves the cancellation contract.</summary>
    [AvaloniaFact]
    public async Task NativeReferencePickerService_ExternalCancellation_DrainsThenThrows()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 4);
        var searchStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var cancellationObserved = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseCleanup = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var workspace = CreateBlockingWorkspace(workspaceId, revision, searchStarted, cancellationObserved, releaseCleanup);
        var coordinator = new ReferencePickerTestCoordinator(
            workspace,
            NativeReferencePickerViewModelTests.CreateDescriptor(workspaceId, revision));
        var owner = new Window();
        var windowService = new HeadlessApplicationWindowService(owner);
        var service = new NativeReferencePickerService(
            windowService,
            coordinator,
            new AvaloniaUiDispatcher(),
            new LoggerConfiguration().CreateLogger());
        using var cancellation = new CancellationTokenSource();
        Task<NativeReferencePickerSelection?>? pickTask = null;
        Task? searchTask = null;
        Window? dialog = null;

        try
        {
            owner.Show();
            pickTask = service.PickAsync(CreateRequest(workspaceId, revision, allowNull: false), cancellation.Token);
            dialog = await windowService.DialogShown.Task.WaitAsync(AsyncDeadline);
            var picker = (NativeReferencePickerView)dialog.Content!;
            var viewModel = (NativeReferencePickerViewModel)picker.DataContext!;
            viewModel.Query = "blocked";
            searchTask = viewModel.SearchAsync();
            await searchStarted.Task.WaitAsync(AsyncDeadline);

            cancellation.Cancel();
            Dispatcher.UIThread.RunJobs();
            await cancellationObserved.Task.WaitAsync(AsyncDeadline);
            pickTask.IsCompleted.ShouldBeFalse();
            releaseCleanup.TrySetResult(true);

            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await pickTask.WaitAsync(AsyncDeadline));
            await searchTask.WaitAsync(AsyncDeadline);
        }
        finally
        {
            releaseCleanup.TrySetResult(true);
            await DrainForCleanupAsync(searchTask);
            if (dialog?.IsVisible == true)
            {
                dialog.Close((object?)null);
            }

            await DrainForCleanupAsync(pickTask);
            owner.Close();
        }
    }

    /// <summary>Creates a picker request against one exact test workspace revision.</summary>
    /// <param name="workspaceId">The active workspace identity.</param>
    /// <param name="revision">The exact workspace revision.</param>
    /// <param name="allowNull">Whether the dialog exposes explicit null selection.</param>
    /// <returns>The immutable picker request.</returns>
    private static NativeReferencePickerRequest CreateRequest(
        Guid workspaceId,
        WorkspaceRevision revision,
        bool allowNull)
    {
        return new NativeReferencePickerRequest(
            workspaceId,
            revision,
            currentFormKey: null,
            RecordScope.WinningOverrides,
            containingModKey: null,
            allowNull,
            "Choose a reference for the selected native field.");
    }

    /// <summary>Creates a workspace whose search observes cancellation but delays completion until cleanup is released.</summary>
    /// <param name="workspaceId">The workspace identity.</param>
    /// <param name="revision">The exact workspace revision.</param>
    /// <param name="searchStarted">The signal completed when search begins.</param>
    /// <param name="cancellationObserved">The signal completed when search observes cancellation.</param>
    /// <param name="releaseCleanup">The signal that allows canceled search cleanup to complete.</param>
    /// <returns>The blocking test workspace.</returns>
    private static ReferencePickerTestWorkspace CreateBlockingWorkspace(
        Guid workspaceId,
        WorkspaceRevision revision,
        TaskCompletionSource<bool> searchStarted,
        TaskCompletionSource<bool> cancellationObserved,
        TaskCompletionSource<bool> releaseCleanup)
    {
        return new ReferencePickerTestWorkspace(workspaceId, revision)
        {
            SearchAction = async (_, cancellationToken) =>
            {
                searchStarted.TrySetResult(true);
                try
                {
                    await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
                    throw new InvalidOperationException("The blocked search completed without cancellation.");
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    cancellationObserved.TrySetResult(true);
                    await releaseCleanup.Task;
                    throw;
                }
            }
        };
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

    /// <summary>Shows modal dialogs against a headless owner and exposes the shown dialog to tests.</summary>
    private sealed class HeadlessApplicationWindowService : IApplicationWindowService
    {
        /// <summary>The visible owner used by Avalonia modal lifetime.</summary>
        private readonly Window Owner;

        /// <summary>Initializes a headless modal presenter.</summary>
        /// <param name="owner">The visible modal owner.</param>
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
