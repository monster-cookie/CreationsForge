using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
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
        var projector = new RecordJsonTreeProjectionService();
        using var browserViewModel = new FormListBrowserViewModel(
            context.Coordinator,
            context.Arbiter,
            projector,
            picker,
            dispatcher,
            HeadlessRecordEditorFactory.Create(context.Coordinator, context.Arbiter, picker, dispatcher));
        using var majorRecordViewModel = new MajorRecordBrowserViewModel(context.Coordinator, projector, dispatcher);
        var browserView = new FormListBrowserView(browserViewModel);
        var majorRecordView = new MajorRecordBrowserView(majorRecordViewModel);
        var view = new WorkspaceShellView(viewModel, browserView, majorRecordView);
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
            var tabs = contentHost.Child.ShouldBeOfType<TabControl>();
            tabs.ItemCount.ShouldBe(2);
            ControlFinder.FindByAutomationId<TabItem>(view, "MajorRecordBrowserTab").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TabItem>(view, "FormListBrowserTab").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<MajorRecordBrowserView>(view, "MajorRecordBrowserView").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBox>(view, "MajorRecordFormIdFilter").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBox>(view, "MajorRecordEditorIdFilter").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ComboBox>(view, "MajorRecordSortSelector").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ItemsControl>(view, "MajorRecordSemanticChanges").ShouldBeNull();
            var activePluginCount = ControlFinder.FindByAutomationId<TextBlock>(view, "ActivePluginRecordCountText")!;
            var loadedFormListCount = ControlFinder.FindByAutomationId<TextBlock>(view, "LoadedRecordCountText")!;
            var loadedMajorRecordCount = ControlFinder.FindByAutomationId<TextBlock>(view, "WorkspaceMajorRecordLoadedCountText")!;
            activePluginCount.IsVisible.ShouldBeFalse();
            loadedFormListCount.IsVisible.ShouldBeFalse();
            loadedMajorRecordCount.IsVisible.ShouldBeTrue();
            loadedMajorRecordCount.Text.ShouldBe("Loaded records: 0");
            tabs.SelectedIndex = 1;
            Dispatcher.UIThread.RunJobs();
            activePluginCount.IsVisible.ShouldBeTrue();
            loadedFormListCount.IsVisible.ShouldBeTrue();
            loadedMajorRecordCount.IsVisible.ShouldBeFalse();
            ControlFinder.FindByAutomationId<FormListBrowserView>(view, "FormListBrowserView").ShouldNotBeNull();
            var legend = ControlFinder.FindByAutomationId<StackPanel>(
                view,
                "WorkspaceComparisonLegend").ShouldNotBeNull();
            legend.HorizontalAlignment.ShouldBe(Avalonia.Layout.HorizontalAlignment.Right);
            var legendItems = legend.Children.OfType<StackPanel>().ToArray();
            legendItems.Select(item => item.Children.OfType<TextBlock>().Single().Text)
                .ShouldBe(["Identical", "Conflict", "Winning Override"]);
            legendItems.Select(item => item.Children.OfType<Border>().Single().Background)
                .Cast<SolidColorBrush>()
                .Select(brush => brush.Color)
                .ShouldBe(
                [
                    Color.FromArgb(80, 0, 128, 0),
                    Color.FromArgb(80, 192, 0, 0),
                    Color.FromArgb(80, 192, 160, 0)
                ]);
            ControlFinder.FindByAutomationId<TextBlock>(view, "WorkspaceStatusText")!.Text.ShouldBe("No plugin is open.");
            activePluginCount.Text.ShouldBe("Plugin records: 0");
            loadedFormListCount.Text.ShouldBe("Unique loaded records: 0");
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
