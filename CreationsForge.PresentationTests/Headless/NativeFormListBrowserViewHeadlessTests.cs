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
/// Verifies the native FormList browser exposes its comparison workflow in headless Avalonia.
/// </summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class NativeFormListBrowserViewHeadlessTests
{
    /// <summary>Verifies the browser retains the required three-to-seven pane ratio and discoverable workflow controls.</summary>
    [AvaloniaFact]
    public void NativeFormListBrowserView_ShowHeadlessly_ExposesNativeComparisonControls()
    {
        var coordinator = new FakeNativeWorkspaceCoordinator();
        var picker = new RecordingNativeReferencePickerService();
        var dispatcher = new InlineUiDispatcher();
        var operationArbiter = new NativeWorkspacePresentationOperationArbiter();
        using var viewModel = new NativeFormListBrowserViewModel(
            coordinator,
            operationArbiter,
            new NativeJsonTreeProjectionService(),
            picker,
            dispatcher,
            HeadlessNativeEditorFactory.Create(coordinator, operationArbiter, picker, dispatcher));
        var view = new NativeFormListBrowserView(viewModel);
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

            var layout = ControlFinder.FindByAutomationId<Grid>(view, "NativeFormListBrowserLayout").ShouldNotBeNull();
            layout.ColumnDefinitions.Count.ShouldBe(2);
            layout.ColumnDefinitions[0].Width.Value.ShouldBe(3d);
            layout.ColumnDefinitions[1].Width.Value.ShouldBe(7d);
            var detailsTabs = ControlFinder.FindByAutomationId<TabControl>(
                view,
                "NativeFormListDetailsTabs").ShouldNotBeNull();
            var tabItems = detailsTabs.ItemsSource.ShouldNotBeNull().Cast<TabItem>().ToArray();
            tabItems.Select(tab => tab.Header).ShouldBe(["Compare", "Edit"]);
            detailsTabs.SelectedIndex.ShouldBe(0);
            ControlFinder.FindByAutomationId<TreeDataGrid>(view, "NativeFormListRecordTree").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBox>(view, "NativeFormListFormIdFilter").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBox>(view, "NativeFormListEditorIdFilter").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ComboBox>(view, "NativeFormListBeforeContextSelector").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ComboBox>(view, "NativeFormListAfterContextSelector").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TreeDataGrid>(view, "NativeFormListBeforeFieldTree").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TreeDataGrid>(view, "NativeFormListAfterFieldTree").ShouldNotBeNull();
            var legend = ControlFinder.FindByAutomationId<StackPanel>(
                view,
                "NativeFormListComparisonLegend").ShouldNotBeNull();
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
            ControlFinder.FindByAutomationId<ItemsControl>(view, "NativeFormListSemanticChanges").ShouldNotBeNull();
            var diagnosticsScroller = ControlFinder.FindByAutomationId<ScrollViewer>(
                view,
                "NativeFormListDiagnosticsScroller").ShouldNotBeNull();
            diagnosticsScroller.MaxHeight.ShouldBe(120d);
            diagnosticsScroller.HorizontalScrollBarVisibility.ShouldBe(ScrollBarVisibility.Disabled);
            diagnosticsScroller.VerticalScrollBarVisibility.ShouldBe(ScrollBarVisibility.Auto);
            ControlFinder.FindByAutomationId<Button>(view, "NativeFormListReferencePickerButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "NativeFormListRefreshButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "NativeFormListRetryButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBlock>(view, "NativeFormListBrowserStatusText")!
                .Text.ShouldBe("No native workspace is open.");

            detailsTabs.SelectedIndex = 1;
            Dispatcher.UIThread.RunJobs();
            ControlFinder.FindByAutomationId<NativeFormListEditorView>(
                view,
                "NativeFormListEditorView").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "NativeFormListBeginNewButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "NativeFormListBeginOverrideButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "NativeFormListBeginExistingOutputButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ComboBox>(view, "NativeFormListCommandGroupSelector").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ComboBox>(view, "NativeFormListCommandSelector").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ScrollViewer>(view, "NativeFormListTypedDraftScroller").ShouldNotBeNull();
            var clearComponents = ControlFinder.FindByAutomationId<Button>(view, "NativeFormListClearComponentsButton").ShouldNotBeNull();
            clearComponents.IsVisible.ShouldBeFalse();
            ControlFinder.FindByAutomationId<Button>(view, "NativeFormListEditorApplyButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(view, "NativeFormListEditorRetryPendingButton").ShouldNotBeNull();
            var discardFormChanges = ControlFinder.FindByAutomationId<Button>(
                view,
                "NativeFormListDiscardFormChangesButton").ShouldNotBeNull();
            discardFormChanges.IsEnabled.ShouldBeFalse();
            ControlFinder.FindByAutomationId<TextBlock>(view, "NativeFormListDiscardFormChangesHelp")!
                .Text.ShouldBe("Discards unapplied form input only; staged changes remain.");
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies crowded diagnostics remain bounded at a compact desktop size and writes the rendered review artifact.</summary>
    /// <remarks>The PNG is retained under the task-owned <c>.work/NativeReplacement/screenshots</c> directory for visual inspection.</remarks>
    [AvaloniaFact]
    public async Task NativeFormListBrowserView_CrowdedDiagnostics_PreserveFieldHeightAndWriteScreenshot()
    {
        using var viewModel = await CreatePopulatedBrowserAsync();
        var view = new NativeFormListBrowserView(viewModel);
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
            var changes = ControlFinder.FindByAutomationId<ItemsControl>(
                view,
                "NativeFormListSemanticChanges").ShouldNotBeNull();
            var warnings = ControlFinder.FindByAutomationId<ItemsControl>(
                view,
                "NativeFormListWarnings").ShouldNotBeNull();
            var errorPanel = ControlFinder.FindByAutomationId<StackPanel>(
                view,
                "NativeFormListErrorPanel").ShouldNotBeNull();
            var errorText = ControlFinder.FindByAutomationId<TextBlock>(
                view,
                "NativeFormListErrorText").ShouldNotBeNull();
            changes.ItemsSource = Enumerable.Range(0, 48).Select(index => new SemanticChangeDescriptor(
                $"Items[{index}].A_very_long_native_field_identifier_that_must_wrap_inside_the_comparison_pane_and_continue_across_multiple_visual_lines_without_horizontal_clipping_or_inaccessible_tail_content",
                SemanticChangeKind.ItemChanged,
                index,
                index + 1));
            warnings.ItemsSource = Enumerable.Range(0, 48).Select(index => new EngineWarning(
                $"Crowded-{index}",
                "A long native warning message that must wrap within the fixed comparison pane without reducing either field tree to zero height."));
            errorPanel.ClearValue(Visual.IsVisibleProperty);
            errorPanel.IsVisible = true;
            errorText.Text = "A compact-layout failure keeps Retry visible while diagnostics scroll independently.";
            Dispatcher.UIThread.RunJobs();

            var diagnosticsScroller = ControlFinder.FindByAutomationId<ScrollViewer>(
                view,
                "NativeFormListDiagnosticsScroller").ShouldNotBeNull();
            var beforeTree = ControlFinder.FindByAutomationId<TreeDataGrid>(
                view,
                "NativeFormListBeforeFieldTree").ShouldNotBeNull();
            var afterTree = ControlFinder.FindByAutomationId<TreeDataGrid>(
                view,
                "NativeFormListAfterFieldTree").ShouldNotBeNull();
            var status = ControlFinder.FindByAutomationId<TextBlock>(
                view,
                "NativeFormListBrowserStatusText").ShouldNotBeNull();
            var retry = ControlFinder.FindByAutomationId<Button>(
                view,
                "NativeFormListRetryButton").ShouldNotBeNull();
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
            var firstChange = changes.GetVisualDescendants().OfType<TextBlock>().First();
            firstChange.Bounds.Width.ShouldBeLessThanOrEqualTo(diagnosticsScroller.Viewport.Width);
            firstChange.Bounds.Height.ShouldBeGreaterThan(20d);
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

    /// <summary>Creates a deterministic populated native browser with identical, conflicting, null, empty, and duplicate JSON fields.</summary>
    /// <returns>The started browser after its default winning-override comparison is published.</returns>
    private static async Task<NativeFormListBrowserViewModel> CreatePopulatedBrowserAsync()
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
        var coordinator = new RecordingNativeWorkspaceCoordinator();
        coordinator.Publish(
            new NativeWorkspaceDescriptor(
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
        var picker = new RecordingNativeReferencePickerService();
        var dispatcher = new AvaloniaUiDispatcher();
        var operationArbiter = new NativeWorkspacePresentationOperationArbiter();
        var viewModel = new NativeFormListBrowserViewModel(
            coordinator,
            operationArbiter,
            new NativeJsonTreeProjectionService(),
            picker,
            dispatcher,
            HeadlessNativeEditorFactory.Create(coordinator, operationArbiter, picker, dispatcher));
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
    private static void ExpandAll(IReadOnlyList<NativeJsonFieldNodeViewModel> fields)
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

    /// <summary>Gets the stable repository-relative path for the native browser visual review artifact.</summary>
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
            "NativeReplacement",
            "screenshots",
            "native-form-list-browser-crowded.png"));
    }
}
