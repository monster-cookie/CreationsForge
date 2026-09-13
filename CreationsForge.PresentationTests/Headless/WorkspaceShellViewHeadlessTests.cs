using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.Headless;

/// <summary>
/// Verifies the workspace shell control tree in headless Avalonia.
/// </summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class WorkspaceShellViewHeadlessTests
{
    /// <summary>Verifies the shell exposes workspace actions, status, and the editor content host.</summary>
    [AvaloniaFact]
    public void WorkspaceShellView_ShowHeadlessly_ExposesPluginRootControls()
    {
        using var context = WorkspaceChangesTestContext.CreateNoWorkspace();
        using var viewModel = CreateViewModel(context);
        var picker = new RecordingReferencePickerService();
        var dispatcher = new InlineUiDispatcher();
        using var browserViewModel = new FormListBrowserViewModel(
            context.Coordinator,
            context.Arbiter,
            new RecordJsonTreeProjectionService(),
            picker,
            dispatcher,
            HeadlessRecordEditorFactory.Create(context.Coordinator, context.Arbiter, picker, dispatcher));
        var browserView = new FormListBrowserView(browserViewModel);
        var view = new WorkspaceShellView(viewModel, browserView);
        var window = new Window
        {
            Width = 1200,
            Height = 800,
            Content = view
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            ControlFinder.FindByAutomationId<WorkspaceShellView>(view, "WorkspaceShellView").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "OpenWorkspaceShellButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "CloseWorkspaceShellButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "WorkspaceReviewChangesButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "WorkspaceSaveChangesButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "WorkspaceDiscardChangesButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "WorkspaceSettingsButton").ShouldNotBeNull();
            var contentHost = ControlFinder.FindByAutomationId<Border>(view, "WorkspaceContentHost").ShouldNotBeNull();
            contentHost.Child.ShouldBeSameAs(browserView);
            ControlFinder.FindByAutomationId<FormListBrowserView>(view, "FormListBrowserView").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBlock>(view, "WorkspaceStatusText")!.Text.ShouldBe("No plugin is open.");
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Creates a shell view model backed by deterministic test doubles.</summary>
    /// <param name="context">The shared workspace lifecycle and presentation admission context.</param>
    /// <returns>The configured shell view model.</returns>
    private static WorkspaceShellViewModel CreateViewModel(WorkspaceChangesTestContext context)
    {
        WorkspaceSelectionViewModel CreateSelectionViewModel()
        {
            return new WorkspaceSelectionViewModel(
                context.Coordinator,
                new FakeWorkspacePathPicker(),
                new FakeGameSelectionService(),
                context.Dispatcher,
                new LoggerConfiguration().CreateLogger());
        }

        return new WorkspaceShellViewModel(
            context.Coordinator,
            CreateSelectionViewModel,
            new FakeWorkspaceSelectionDialogService(),
            context.ViewModel,
            new FakeApplicationNavigationService());
    }
}
