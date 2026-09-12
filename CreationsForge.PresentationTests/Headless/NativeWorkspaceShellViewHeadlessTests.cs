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
/// Verifies the native workspace shell control tree in headless Avalonia.
/// </summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class NativeWorkspaceShellViewHeadlessTests
{
    /// <summary>Verifies the shell exposes native workspace actions, status, and the editor content host.</summary>
    [AvaloniaFact]
    public void NativeWorkspaceShellView_ShowHeadlessly_ExposesNativeRootControls()
    {
        using var context = NativeWorkspaceChangesTestContext.CreateNoWorkspace();
        using var viewModel = CreateViewModel(context);
        var picker = new RecordingNativeReferencePickerService();
        var dispatcher = new InlineUiDispatcher();
        using var browserViewModel = new NativeFormListBrowserViewModel(
            context.Coordinator,
            context.Arbiter,
            new NativeJsonTreeProjectionService(),
            picker,
            dispatcher,
            HeadlessNativeEditorFactory.Create(context.Coordinator, context.Arbiter, picker, dispatcher));
        var browserView = new NativeFormListBrowserView(browserViewModel);
        var view = new NativeWorkspaceShellView(viewModel, browserView);
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

            ControlFinder.FindByAutomationId<NativeWorkspaceShellView>(view, "NativeWorkspaceShellView").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "OpenNativeWorkspaceShellButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "CloseNativeWorkspaceShellButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "NativeWorkspaceReviewChangesButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "NativeWorkspaceSaveChangesButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "NativeWorkspaceDiscardChangesButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "NativeWorkspaceSettingsButton").ShouldNotBeNull();
            var contentHost = ControlFinder.FindByAutomationId<Border>(view, "NativeWorkspaceContentHost").ShouldNotBeNull();
            contentHost.Child.ShouldBeSameAs(browserView);
            ControlFinder.FindByAutomationId<NativeFormListBrowserView>(view, "NativeFormListBrowserView").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBlock>(view, "NativeWorkspaceStatusText")!.Text.ShouldBe("No native workspace is open.");
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Creates a shell view model backed by deterministic test doubles.</summary>
    /// <param name="context">The shared workspace lifecycle and presentation admission context.</param>
    /// <returns>The configured shell view model.</returns>
    private static NativeWorkspaceShellViewModel CreateViewModel(NativeWorkspaceChangesTestContext context)
    {
        NativeWorkspaceSelectionViewModel CreateSelectionViewModel()
        {
            return new NativeWorkspaceSelectionViewModel(
                context.Coordinator,
                new FakeNativeWorkspacePathPicker(),
                new FakeGameSelectionService(),
                context.Dispatcher,
                new LoggerConfiguration().CreateLogger());
        }

        return new NativeWorkspaceShellViewModel(
            context.Coordinator,
            CreateSelectionViewModel,
            new FakeNativeWorkspaceSelectionDialogService(),
            context.ViewModel,
            new FakeNativeApplicationNavigationService());
    }
}
