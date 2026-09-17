using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.ViewModels;

namespace CreationsForge.Views;

/// <summary>Presents complete major-record families with exact context selection and native field comparison.</summary>
public sealed class MajorRecordBrowserView : UserControl
{
    /// <summary>The navigation-scope read-only browser workflow.</summary>
    private readonly MajorRecordBrowserViewModel ViewModel;

    /// <summary>Tracks whether initial loading started for this attached view.</summary>
    private bool Started;

    /// <summary>Initializes the major-record browser view.</summary>
    /// <param name="viewModel">The navigation-scope browser workflow.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is <see langword="null"/>.</exception>
    public MajorRecordBrowserView(MajorRecordBrowserViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ViewModel = viewModel;
        DataContext = ViewModel;
        AutomationProperties.SetAutomationId(this, "MajorRecordBrowserView");
        Content = BuildContent();
    }

    /// <summary>Gets the browser presentation state hosted by this view for shell-level status bindings.</summary>
    internal MajorRecordBrowserViewModel BrowserViewModel => ViewModel;

    /// <summary>Starts record loading once this navigation-owned view enters the visual tree.</summary>
    /// <param name="eventArgs">The visual-tree attachment event.</param>
    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs eventArgs)
    {
        base.OnAttachedToVisualTree(eventArgs);
        if (Started)
        {
            return;
        }

        Started = true;
        await ViewModel.StartAsync();
    }

    /// <summary>Builds the family list and comparison layout.</summary>
    /// <returns>The complete browser control tree.</returns>
    private Control BuildContent()
    {
        var records = BuildRecordPane();
        Grid.SetColumn(records, 0);
        var comparison = BuildComparisonPane();
        Grid.SetColumn(comparison, 1);
        var content = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("3*,7*"),
            ColumnSpacing = 18,
            Children =
            {
                records,
                comparison
            }
        };
        AutomationProperties.SetAutomationId(content, "MajorRecordBrowserLayout");
        return content;
    }

    /// <summary>Builds complete family-grouped record discovery with visible loading progress.</summary>
    /// <returns>The record navigation pane.</returns>
    private Control BuildRecordPane()
    {
        var refresh = new Button
        {
            Content = "Refresh",
            Padding = new Thickness(12, 7)
        };
        refresh.Bind(Button.CommandProperty, new Binding(nameof(MajorRecordBrowserViewModel.RefreshCommand)));
        AutomationProperties.SetAutomationId(refresh, "MajorRecordRefreshButton");
        var count = CreateBoundText(nameof(MajorRecordBrowserViewModel.LoadedRecordCountText), 12, FontWeight.Normal);
        count.VerticalAlignment = VerticalAlignment.Center;
        AutomationProperties.SetAutomationId(count, "MajorRecordLoadedCountText");
        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                refresh,
                count
            }
        };

        var tree = new TreeDataGrid
        {
            CanUserResizeColumns = true,
            CanUserSortColumns = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        tree.Bind(TreeDataGrid.SourceProperty, new Binding(nameof(MajorRecordBrowserViewModel.RecordTreeSource)));
        tree.SelectionChanged += async (_, eventArgs) =>
        {
            var selected = eventArgs.SelectedItems.OfType<MajorRecordViewModel>().LastOrDefault();
            if (selected is not null)
            {
                await ViewModel.SelectRecordAsync(selected);
            }
        };
        AutomationProperties.SetAutomationId(tree, "MajorRecordTree");
        var progressText = CreateBoundText(nameof(MajorRecordBrowserViewModel.StatusText), 14, FontWeight.SemiBold);
        progressText.TextWrapping = TextWrapping.Wrap;
        AutomationProperties.SetAutomationId(progressText, "MajorRecordLoadingStatus");
        var loading = new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            Child = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 12,
                Children =
                {
                    progressText,
                    new ProgressBar { IsIndeterminate = true, MinWidth = 280, MinHeight = 4 }
                }
            }
        };
        loading.Bind(IsVisibleProperty, new Binding(nameof(MajorRecordBrowserViewModel.IsBusy)));
        AutomationProperties.SetAutomationId(loading, "MajorRecordLoadingScreen");
        var treeArea = new Grid { Children = { tree, loading } };
        Grid.SetRow(actions, 1);
        Grid.SetRow(treeArea, 2);
        return new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 0, 1, 0),
            Padding = new Thickness(12),
            Child = new Grid
            {
                RowDefinitions = new RowDefinitions("Auto,Auto,*"),
                RowSpacing = 10,
                Children =
                {
                    CreateText("All Major Records", 18, FontWeight.SemiBold),
                    actions,
                    treeArea
                }
            }
        };
    }

    /// <summary>Builds exact context selectors, complete field trees, changes, warnings, and progress.</summary>
    /// <returns>The comparison pane.</returns>
    private Control BuildComparisonPane()
    {
        var selectors = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            ColumnSpacing = 12,
            Children =
            {
                BuildContextSelector(
                    "Prior context",
                    nameof(MajorRecordBrowserViewModel.SelectedBeforeContext),
                    nameof(MajorRecordBrowserViewModel.BeforeProvenanceText),
                    "MajorRecordBeforeContextSelector",
                    ViewModel.SelectBeforeContextAsync,
                    0),
                BuildContextSelector(
                    "Resulting context",
                    nameof(MajorRecordBrowserViewModel.SelectedAfterContext),
                    nameof(MajorRecordBrowserViewModel.AfterProvenanceText),
                    "MajorRecordAfterContextSelector",
                    ViewModel.SelectAfterContextAsync,
                    1)
            }
        };
        var fields = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            ColumnSpacing = 12,
            Children =
            {
                BuildFieldPane("Prior fields", nameof(MajorRecordBrowserViewModel.BeforeFieldSource), "MajorRecordBeforeFieldTree", 0),
                BuildFieldPane("Resulting fields", nameof(MajorRecordBrowserViewModel.AfterFieldSource), "MajorRecordAfterFieldTree", 1)
            }
        };
        var diagnostics = BuildDiagnosticsPane();
        Grid.SetRow(selectors, 0);
        Grid.SetRow(fields, 1);
        Grid.SetRow(diagnostics, 2);
        var body = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*,Auto"),
            RowSpacing = 12,
            Children =
            {
                selectors,
                fields,
                diagnostics
            }
        };

        var progress = new ProgressBar
        {
            IsIndeterminate = true,
            MinWidth = 280,
            MinHeight = 4
        };
        var loading = new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            Child = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 12,
                Children =
                {
                    CreateText("Resolving native record fields...", 16, FontWeight.SemiBold),
                    progress
                }
            }
        };
        loading.Bind(IsVisibleProperty, new Binding(nameof(MajorRecordBrowserViewModel.IsComparisonBusy)));
        AutomationProperties.SetAutomationId(loading, "MajorRecordComparisonLoadingView");
        return new Grid
        {
            Children =
            {
                body,
                loading
            }
        };
    }

    /// <summary>Builds one exact-context selector and provenance label.</summary>
    /// <param name="title">The selector title.</param>
    /// <param name="selectedProperty">The selected context property.</param>
    /// <param name="provenanceProperty">The provenance text property.</param>
    /// <param name="automationId">The stable selector automation identity.</param>
    /// <param name="selectionAction">The asynchronous selection workflow.</param>
    /// <param name="column">The parent grid column.</param>
    /// <returns>The configured selector panel.</returns>
    private Control BuildContextSelector(
        string title,
        string selectedProperty,
        string provenanceProperty,
        string automationId,
        Func<MajorRecordContextOption?, Task> selectionAction,
        int column)
    {
        var selector = new ComboBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MaxDropDownHeight = 260,
            ItemTemplate = new FuncDataTemplate<MajorRecordContextOption>(
                (option, _) => new TextBlock { Text = option?.Label ?? string.Empty })
        };
        selector.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(MajorRecordBrowserViewModel.ContextOptions)));
        selector.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(selectedProperty) { Mode = BindingMode.OneWay });
        selector.SelectionChanged += async (_, _) => await selectionAction(selector.SelectedItem as MajorRecordContextOption);
        AutomationProperties.SetAutomationId(selector, automationId);
        var provenance = CreateBoundText(provenanceProperty, 12, FontWeight.Normal);
        provenance.TextWrapping = TextWrapping.Wrap;
        AutomationProperties.SetAutomationId(provenance, automationId + "Provenance");
        var panel = new StackPanel
        {
            Spacing = 6,
            Children =
            {
                CreateText(title, 14, FontWeight.SemiBold),
                selector,
                provenance
            }
        };
        Grid.SetColumn(panel, column);
        return panel;
    }

    /// <summary>Builds one independent hierarchical field pane.</summary>
    /// <param name="title">The field-pane title.</param>
    /// <param name="sourceProperty">The hierarchical source property.</param>
    /// <param name="automationId">The stable tree automation identity.</param>
    /// <param name="column">The parent grid column.</param>
    /// <returns>The configured field pane.</returns>
    private static Control BuildFieldPane(string title, string sourceProperty, string automationId, int column)
    {
        var tree = new TreeDataGrid
        {
            CanUserResizeColumns = true,
            CanUserSortColumns = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        tree.Bind(TreeDataGrid.SourceProperty, new Binding(sourceProperty));
        AutomationProperties.SetAutomationId(tree, automationId);
        Grid.SetRow(tree, 1);
        var pane = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*"),
            RowSpacing = 6,
            Children =
            {
                CreateText(title, 14, FontWeight.SemiBold),
                tree
            }
        };
        Grid.SetColumn(pane, column);
        return pane;
    }

    /// <summary>Builds warnings, typed errors, retry, and status without duplicating field-level comparison details.</summary>
    /// <returns>The complete diagnostics pane.</returns>
    private Control BuildDiagnosticsPane()
    {
        var warnings = new ItemsControl
        {
            ItemTemplate = new FuncDataTemplate<EngineWarning>(
                (warning, _) => CreateWrappedText(
                    warning is null ? string.Empty : $"{warning.Code}: {warning.Message}",
                    12))
        };
        warnings.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(MajorRecordBrowserViewModel.Warnings)));
        var warningDetails = new StackPanel
        {
            Spacing = 6,
            Children =
            {
                CreateText("Warnings", 13, FontWeight.SemiBold),
                warnings
            }
        };
        AutomationProperties.SetAutomationId(warningDetails, "MajorRecordWarnings");
        var scroller = new ScrollViewer
        {
            MaxHeight = 130,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = warningDetails
        };
        scroller.Bind(IsVisibleProperty, new Binding(nameof(MajorRecordBrowserViewModel.HasWarnings)));
        var error = CreateBoundText(nameof(MajorRecordBrowserViewModel.ErrorMessage), 12, FontWeight.Normal);
        error.TextWrapping = TextWrapping.Wrap;
        var retry = new Button
        {
            Content = "Retry",
            Padding = new Thickness(12, 6)
        };
        retry.Bind(Button.CommandProperty, new Binding(nameof(MajorRecordBrowserViewModel.RetryCommand)));
        var errorPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                error,
                retry
            }
        };
        errorPanel.Bind(IsVisibleProperty, new Binding(nameof(MajorRecordBrowserViewModel.HasError)));
        var status = CreateBoundText(nameof(MajorRecordBrowserViewModel.StatusText), 12, FontWeight.Normal);
        status.TextWrapping = TextWrapping.Wrap;
        status.Bind(IsVisibleProperty, new Binding(nameof(MajorRecordBrowserViewModel.HasStatusText)));
        AutomationProperties.SetAutomationId(status, "MajorRecordBrowserStatusText");
        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                scroller,
                errorPanel,
                status
            }
        };
    }

    /// <summary>Creates unbound application-styled text.</summary>
    /// <param name="text">The visible text.</param>
    /// <param name="fontSize">The font size.</param>
    /// <param name="fontWeight">The font weight.</param>
    /// <returns>The configured text block.</returns>
    private static TextBlock CreateText(string text, double fontSize, FontWeight fontWeight)
    {
        var block = new TextBlock
        {
            Text = text,
            FontSize = fontSize,
            FontWeight = fontWeight
        };
        App.ApplyApplicationTextForeground(block);
        return block;
    }

    /// <summary>Creates application-styled text bound to one view-model property.</summary>
    /// <param name="propertyName">The bound property.</param>
    /// <param name="fontSize">The font size.</param>
    /// <param name="fontWeight">The font weight.</param>
    /// <returns>The configured text block.</returns>
    private static TextBlock CreateBoundText(string propertyName, double fontSize, FontWeight fontWeight)
    {
        var block = new TextBlock
        {
            FontSize = fontSize,
            FontWeight = fontWeight
        };
        block.Bind(TextBlock.TextProperty, new Binding(propertyName));
        App.ApplyApplicationTextForeground(block);
        return block;
    }

    /// <summary>Creates wrapped application-styled text.</summary>
    /// <param name="text">The visible text.</param>
    /// <param name="fontSize">The font size.</param>
    /// <returns>The configured text block.</returns>
    private static TextBlock CreateWrappedText(string text, double fontSize)
    {
        var block = CreateText(text, fontSize, FontWeight.Normal);
        block.TextWrapping = TextWrapping.Wrap;
        return block;
    }
}
