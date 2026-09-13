using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.PresentationTests.Headless;

/// <summary>
/// Verifies the FormList browser exposes its comparison workflow in headless Avalonia.
/// </summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class FormListBrowserViewHeadlessTests
{
    /// <summary>Verifies the browser retains the required three-to-seven pane ratio and discoverable workflow controls.</summary>
    [AvaloniaFact]
    public void FormListBrowserView_ShowHeadlessly_ExposesPluginComparisonControls()
    {
        var coordinator = new FakeWorkspaceCoordinator();
        var picker = new RecordingReferencePickerService();
        var dispatcher = new InlineUiDispatcher();
        var operationArbiter = new WorkspacePresentationOperationArbiter();
        using var viewModel = new FormListBrowserViewModel(
            coordinator,
            operationArbiter,
            new RecordJsonTreeProjectionService(),
            picker,
            dispatcher,
            HeadlessRecordEditorFactory.Create(coordinator, operationArbiter, picker, dispatcher));
        var view = new FormListBrowserView(viewModel);
        var window = new Window
        {
            Width = 1400,
            Height = 900,
            Content = view
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var layout = ControlFinder.FindByAutomationId<Grid>(view, "FormListBrowserLayout").ShouldNotBeNull();
            layout.ColumnDefinitions.Count.ShouldBe(2);
            layout.ColumnDefinitions[0].Width.Value.ShouldBe(3d);
            layout.ColumnDefinitions[1].Width.Value.ShouldBe(7d);
            var detailsTabs = ControlFinder.FindByAutomationId<TabControl>(
                view,
                "FormListDetailsTabs").ShouldNotBeNull();
            var tabItems = detailsTabs.ItemsSource.ShouldNotBeNull().Cast<TabItem>().ToArray();
            tabItems.Select(tab => tab.Header).ShouldBe(["Compare", "Edit"]);
            tabItems[1].IsVisible.ShouldBeFalse();
            detailsTabs.SelectedIndex.ShouldBe(0);
            ControlFinder.FindByAutomationId<TreeDataGrid>(view, "FormListRecordTree").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBox>(view, "FormListFormIdFilter").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBox>(view, "FormListEditorIdFilter").ShouldNotBeNull();
            var sortSelector = ControlFinder.FindByAutomationId<ComboBox>(
                view,
                "FormListRecordSortSelector").ShouldNotBeNull();
            sortSelector.ItemsSource.ShouldNotBeNull().Cast<FormListRecordSortMode>()
                .ShouldBe([FormListRecordSortMode.FormId, FormListRecordSortMode.EditorId]);
            sortSelector.SelectedItem.ShouldBe(FormListRecordSortMode.FormId);
            sortSelector.SelectedItem = FormListRecordSortMode.EditorId;
            Dispatcher.UIThread.RunJobs();
            viewModel.RecordSortMode.ShouldBe(FormListRecordSortMode.EditorId);
            ControlFinder.FindByAutomationId<ComboBox>(view, "FormListBeforeContextSelector").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ComboBox>(view, "FormListAfterContextSelector").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TreeDataGrid>(view, "FormListBeforeFieldTree").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TreeDataGrid>(view, "FormListAfterFieldTree").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<StackPanel>(view, "FormListComparisonLegend").ShouldBeNull();
            var loadingView = ControlFinder.FindByAutomationId<Border>(
                view,
                "FormListComparisonLoadingView").ShouldNotBeNull();
            loadingView.IsVisible.ShouldBeFalse();
            ControlFinder.FindByAutomationId<ProgressBar>(view, "FormListComparisonProgress").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ItemsControl>(view, "FormListSemanticChanges").ShouldBeNull();
            ControlFinder.FindByAutomationId<StackPanel>(view, "FormListWarningsPanel")!
                .IsVisible.ShouldBeFalse();
            var diagnosticsScroller = ControlFinder.FindByAutomationId<ScrollViewer>(
                view,
                "FormListDiagnosticsScroller").ShouldNotBeNull();
            diagnosticsScroller.MaxHeight.ShouldBe(120d);
            diagnosticsScroller.HorizontalScrollBarVisibility.ShouldBe(ScrollBarVisibility.Disabled);
            diagnosticsScroller.VerticalScrollBarVisibility.ShouldBe(ScrollBarVisibility.Auto);
            ControlFinder.FindByAutomationId<Button>(view, "FormListReferencePickerButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "FormListRefreshButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "FormListRetryButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBlock>(view, "FormListBrowserStatusText")!
                .Text.ShouldBe("No workspace is open.");

            ControlFinder.FindByAutomationId<TabItem>(view, "FormListEditTab")!.IsVisible.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies crowded warnings remain bounded at a compact desktop size and writes the rendered review artifact.</summary>
    /// <remarks>The PNG is retained under the task-owned <c>.work/PluginAuthoring/screenshots</c> directory for visual inspection.</remarks>
    [AvaloniaFact]
    public async Task FormListBrowserView_CrowdedDiagnostics_PreserveFieldHeightAndWriteScreenshot()
    {
        using var viewModel = await CreatePopulatedBrowserAsync();
        var view = new FormListBrowserView(viewModel);
        var window = new Window
        {
            Width = 960,
            Height = 600,
            Content = view
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var warnings = ControlFinder.FindByAutomationId<ItemsControl>(
                view,
                "FormListWarnings").ShouldNotBeNull();
            var warningsPanel = ControlFinder.FindByAutomationId<StackPanel>(
                view,
                "FormListWarningsPanel").ShouldNotBeNull();
            var errorPanel = ControlFinder.FindByAutomationId<StackPanel>(
                view,
                "FormListErrorPanel").ShouldNotBeNull();
            var errorText = ControlFinder.FindByAutomationId<TextBlock>(
                view,
                "FormListErrorText").ShouldNotBeNull();
            warnings.ItemsSource = Enumerable.Range(0, 48).Select(index => new EngineWarning(
                $"Crowded-{index}",
                "A long plugin warning message that must wrap within the fixed comparison pane without reducing either field tree to zero height."));
            warningsPanel.ClearValue(Visual.IsVisibleProperty);
            warningsPanel.IsVisible = true;
            errorPanel.ClearValue(Visual.IsVisibleProperty);
            errorPanel.IsVisible = true;
            errorText.Text = "A compact-layout failure keeps Retry visible while diagnostics scroll independently.";
            Dispatcher.UIThread.RunJobs();

            var diagnosticsScroller = ControlFinder.FindByAutomationId<ScrollViewer>(
                view,
                "FormListDiagnosticsScroller").ShouldNotBeNull();
            var beforeTree = ControlFinder.FindByAutomationId<TreeDataGrid>(
                view,
                "FormListBeforeFieldTree").ShouldNotBeNull();
            var afterTree = ControlFinder.FindByAutomationId<TreeDataGrid>(
                view,
                "FormListAfterFieldTree").ShouldNotBeNull();
            var status = ControlFinder.FindByAutomationId<TextBlock>(
                view,
                "FormListBrowserStatusText").ShouldNotBeNull();
            var retry = ControlFinder.FindByAutomationId<Button>(
                view,
                "FormListRetryButton").ShouldNotBeNull();
            var detailsTabs = ControlFinder.FindByAutomationId<TabControl>(
                view,
                "FormListDetailsTabs").ShouldNotBeNull();
            detailsTabs.ItemsSource.ShouldNotBeNull().Cast<TabItem>()
                .Single(tab => Equals(tab.Header, "Edit"))
                .IsVisible.ShouldBeTrue();
            if (HeadlessTestApp.UsesRenderedArtifactRenderer)
            {
                using var bitmap = window.CaptureRenderedFrame().ShouldNotBeNull();
                var screenshotPath = GetScreenshotPath();
                Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath)!);
                bitmap.Save(screenshotPath, PngBitmapEncoderOptions.Default);
                new FileInfo(screenshotPath).Length.ShouldBeGreaterThan(0L);
            }

            diagnosticsScroller.Bounds.Height.ShouldBeLessThanOrEqualTo(120d);
            diagnosticsScroller.Extent.Height.ShouldBeGreaterThan(diagnosticsScroller.Viewport.Height);
            beforeTree.Bounds.Height.ShouldBeGreaterThan(100d);
            afterTree.Bounds.Height.ShouldBeGreaterThan(100d);
            var firstWarning = warnings.GetVisualDescendants().OfType<TextBlock>().First();
            firstWarning.Bounds.Width.ShouldBeLessThanOrEqualTo(diagnosticsScroller.Viewport.Width);
            firstWarning.Bounds.Height.ShouldBeGreaterThan(20d);
            status.IsEffectivelyVisible.ShouldBeTrue();
            retry.IsEffectivelyVisible.ShouldBeTrue();
            GetBottomWithinView(status, view).ShouldBeLessThanOrEqualTo(view.Bounds.Height);
            GetBottomWithinView(retry, view).ShouldBeLessThanOrEqualTo(view.Bounds.Height);
            beforeTree.GetVisualDescendants()
                .OfType<Border>()
                .Select(border => border.GetValue(AutomationProperties.NameProperty))
                .OfType<string>()
                .ShouldContain(name => name.StartsWith("Identical:", StringComparison.Ordinal));
            beforeTree.GetVisualDescendants()
                .OfType<Border>()
                .Select(border => border.GetValue(AutomationProperties.NameProperty))
                .OfType<string>()
                .ShouldContain(name => name.StartsWith("Conflict:", StringComparison.Ordinal));
            afterTree.GetVisualDescendants()
                .OfType<Border>()
                .Select(border => border.GetValue(AutomationProperties.NameProperty))
                .OfType<string>()
                .ShouldContain(name => name.StartsWith("Winning Override:", StringComparison.Ordinal));

        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Creates a deterministic populated plugin browser with identical, conflicting, null, empty, and duplicate JSON fields.</summary>
    /// <returns>The started browser after its default winning-override comparison is published.</returns>
    private static async Task<FormListBrowserViewModel> CreatePopulatedBrowserAsync()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 3);
        var sourceMod = ModKey.FromNameAndExtension("VisualSource.esm");
        var outputMod = ModKey.FromNameAndExtension("VisualOutput.esm");
        var sourcePath = GetTestPluginPath(sourceMod.FileName);
        var outputPath = GetTestPluginPath(outputMod.FileName);
        var formKey = new FormKey(sourceMod, 0x0123);
        var workspace = new RecordingFormListBrowserWorkspace(
            workspaceId,
            revision,
            _ => ValueTask.FromResult(EngineResult<WorkspaceState>.Success(
                new WorkspaceState(
                    SupportedGame.Starfield,
                    GameRelease.Starfield,
                    output: null,
                    outputBaseline: null,
                    new OutputSynchronizationState(OutputSynchronizationStatus.Ready, null),
                    revision),
                workspaceId,
                resultRevision: revision)),
            _ => ValueTask.FromResult(EngineResult<IReadOnlyList<PluginSummary>>.Success(
                [
                    new PluginSummary(sourceMod, sourcePath, 0, PluginRole.Source),
                    new PluginSummary(outputMod, outputPath, 1, PluginRole.Output)
                ],
                workspaceId,
                resultRevision: revision)),
            (_, _) => ValueTask.FromResult(EngineResult<IReadOnlyList<FormListSummary>>.Success(
                [
                    new FormListSummary(formKey, "VisualSourceList", 1, RecordScope.AllContexts, sourceMod, sourcePath, 0, PluginRole.Source),
                    new FormListSummary(formKey, "VisualWinningList", 1, RecordScope.AllContexts, outputMod, outputPath, 1, PluginRole.Output)
                ],
                workspaceId,
                resultRevision: revision)),
            (request, _) => ValueTask.FromResult(EngineResult<FormListComparison>.Success(
                new FormListComparison(
                    new FormListContext(request.Before, ReferenceResolutionStatus.Resolved, sourceMod, sourcePath, 0, PluginRole.Source),
                    new FormListContext(request.After, ReferenceResolutionStatus.Resolved, outputMod, outputPath, 1, PluginRole.Output),
                    ParseElement("{\"$type\":\"FormList\",\"Items\":[{\"link\":\"A\"},{\"link\":null}],\"Empty\":[],\"Nullable\":null,\"Name\":{\"English\":\"Before\"}}"),
                    ParseElement("{\"$type\":\"FormList\",\"Items\":[{\"link\":\"A\"},{\"link\":\"A\"},{\"link\":null}],\"Empty\":[],\"Nullable\":null,\"Name\":{\"English\":\"After\"}}"),
                    [new SemanticChangeDescriptor("Items", SemanticChangeKind.ItemInserted, afterPosition: 1)],
                    []),
                workspaceId,
                resultRevision: revision)));
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(
            new WorkspaceDescriptor(
                workspaceId,
                SupportedGame.Starfield,
                GameRelease.Starfield,
                sourcePath,
                [sourcePath, outputPath],
                new OutputAssociation(
                    outputPath,
                    outputMod,
                    LocalizedOutputMode.Embedded,
                    OutputMasterStyle.Full),
                revision),
            workspace);
        var picker = new RecordingReferencePickerService();
        var dispatcher = new AvaloniaUiDispatcher();
        var operationArbiter = new WorkspacePresentationOperationArbiter();
        var viewModel = new FormListBrowserViewModel(
            coordinator,
            operationArbiter,
            new RecordJsonTreeProjectionService(),
            picker,
            dispatcher,
            HeadlessRecordEditorFactory.Create(coordinator, operationArbiter, picker, dispatcher));
        await viewModel.StartAsync();
        await viewModel.SelectRecordAsync(viewModel.Records.ShouldHaveSingleItem());
        viewModel.FormIdFilter = "000123";
        viewModel.EditorIdFilter = "Visual";
        ExpandAll(viewModel.BeforeFields);
        ExpandAll(viewModel.AfterFields);
        return viewModel;
    }

    /// <summary>Expands every projected JSON container for visual-review coverage.</summary>
    /// <param name="fields">The projected fields to expand recursively.</param>
    private static void ExpandAll(IReadOnlyList<RecordJsonFieldNodeViewModel> fields)
    {
        foreach (var field in fields)
        {
            field.IsExpanded = true;
            ExpandAll(field.Children);
        }
    }

    /// <summary>Translates a control's lower edge into browser-view coordinates.</summary>
    /// <param name="control">The visible control to measure.</param>
    /// <param name="view">The browser view that owns the available bounds.</param>
    /// <returns>The lower edge in view coordinates.</returns>
    private static double GetBottomWithinView(Control control, Control view)
    {
        return control.TranslatePoint(new Point(0, control.Bounds.Height), view).ShouldNotBeNull().Y;
    }

    /// <summary>Parses and detaches one JSON element for the visual fixture.</summary>
    /// <param name="json">The complete JSON text.</param>
    /// <returns>The detached JSON root.</returns>
    private static System.Text.Json.JsonElement ParseElement(string json)
    {
        using var document = System.Text.Json.JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

    /// <summary>Creates a fully qualified test-only plugin path.</summary>
    /// <param name="fileName">The plugin file name.</param>
    /// <returns>The fully qualified test path.</returns>
    private static string GetTestPluginPath(string fileName)
    {
        return Path.GetFullPath(Path.Combine(Path.GetTempPath(), "CreationsForge-VisualTests", fileName));
    }

    /// <summary>Gets the stable repository-relative path for the plugin browser visual review artifact.</summary>
    /// <returns>The fully qualified PNG destination under the task-owned work directory.</returns>
    private static string GetScreenshotPath()
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
            "plugin-form-list-browser-crowded.png"));
    }
}
