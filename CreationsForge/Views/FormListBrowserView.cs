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

/// <summary>
/// Presents FormLists as winning roots with exact contexts and an independent two-context typed comparison.
/// </summary>
public sealed class FormListBrowserView : UserControl
{
    /// <summary>The navigation-scope browser workflow and detached presentation state.</summary>
    private readonly FormListBrowserViewModel ViewModel;

    /// <summary>Tracks whether initial browser loading has started for this attached view.</summary>
    private bool Started;

    /// <summary>Initializes the FormList browser view.</summary>
    /// <param name="viewModel">The navigation-scope browser workflow.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is <see langword="null"/>.</exception>
    public FormListBrowserView(FormListBrowserViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ViewModel = viewModel;
        DataContext = ViewModel;
        AutomationProperties.SetAutomationId(this, "FormListBrowserView");
        Content = BuildContent();
    }

    /// <summary>Starts plugin loading once this navigation-owned view enters the visual tree.</summary>
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

    /// <summary>Builds the three-to-seven record and Compare/Edit layout.</summary>
    /// <returns>The complete browser control tree.</returns>
    private Control BuildContent()
    {
        var recordPane = BuildRecordPane();
        Grid.SetColumn(recordPane, 0);
        var detailsPane = BuildDetailsPane();
        Grid.SetColumn(detailsPane, 1);
        var layout = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("3*,7*"),
            ColumnSpacing = 18,
            Children =
            {
                recordPane,
                detailsPane
            }
        };
        AutomationProperties.SetAutomationId(layout, "FormListBrowserLayout");
        return layout;
    }

    /// <summary>Builds retained comparison and conditionally visible typed editing as sibling tabs in the right pane.</summary>
    /// <returns>The complete right-side details pane.</returns>
    private Control BuildDetailsPane()
    {
        var compare = new TabItem
        {
            Header = "Compare",
            Content = BuildComparisonPane()
        };
        AutomationProperties.SetAutomationId(compare, "FormListCompareTab");
        var edit = new TabItem
        {
            Header = "Edit",
            Content = new FormListEditorView(ViewModel.Editor)
        };
        AutomationProperties.SetAutomationId(edit, "FormListEditTab");
        edit.Bind(IsVisibleProperty, new Binding(nameof(FormListBrowserViewModel.IsEditingWorkspace)));
        var tabs = new TabControl
        {
            ItemsSource = new[] { compare, edit },
            SelectedIndex = 0
        };
        AutomationProperties.SetAutomationId(tabs, "FormListDetailsTabs");
        return tabs;
    }

    /// <summary>Builds record discovery and exact-context tree selection.</summary>
    /// <returns>The left browser pane.</returns>
    private Control BuildRecordPane()
    {
        var title = CreateText("FormLists", 18, FontWeight.SemiBold);
        var pickerButton = new Button
        {
            Content = "Find Reference...",
            Padding = new Thickness(12, 7),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        pickerButton.Bind(Button.CommandProperty, new Binding(nameof(FormListBrowserViewModel.PickReferenceCommand)));
        AutomationProperties.SetAutomationId(pickerButton, "FormListReferencePickerButton");

        var refreshButton = new Button
        {
            Content = "Refresh",
            Padding = new Thickness(12, 7),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        refreshButton.Bind(Button.CommandProperty, new Binding(nameof(FormListBrowserViewModel.RefreshCommand)));
        AutomationProperties.SetAutomationId(refreshButton, "FormListRefreshButton");

        var sortLabel = CreateText("Sort records:", 12, FontWeight.Normal);
        sortLabel.VerticalAlignment = VerticalAlignment.Center;
        var sortSelector = new ComboBox
        {
            MinWidth = 100,
            ItemsSource = Enum.GetValues<FormListRecordSortMode>(),
            ItemTemplate = new FuncDataTemplate<FormListRecordSortMode>(
                (mode, _) => CreateText(DescribeRecordSortMode(mode), 12, FontWeight.Normal))
        };
        sortSelector.Bind(
            SelectingItemsControl.SelectedItemProperty,
            new Binding(nameof(FormListBrowserViewModel.RecordSortMode))
            {
                Mode = BindingMode.TwoWay
            });
        AutomationProperties.SetAutomationId(sortSelector, "FormListRecordSortSelector");

        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                pickerButton,
                refreshButton,
                sortLabel,
                sortSelector
            }
        };

        var filters = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            ColumnSpacing = 8,
            Children =
            {
                CreateFilterTextBox(
                    nameof(FormListBrowserViewModel.FormIdFilter),
                    "FormID",
                    "FormListFormIdFilter",
                    0),
                CreateFilterTextBox(
                    nameof(FormListBrowserViewModel.EditorIdFilter),
                    "EditorID",
                    "FormListEditorIdFilter",
                    1)
            }
        };

        var selectedReference = CreateBoundText(
            nameof(FormListBrowserViewModel.SelectedReferenceText),
            12,
            FontWeight.Normal);
        selectedReference.TextWrapping = TextWrapping.Wrap;
        AutomationProperties.SetAutomationId(selectedReference, "FormListSelectedReferenceText");

        var recordTree = new TreeDataGrid
        {
            CanUserResizeColumns = true,
            CanUserSortColumns = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        recordTree.Bind(TreeDataGrid.SourceProperty, new Binding(nameof(FormListBrowserViewModel.RecordTreeSource)));
        recordTree.SelectionChanged += async (_, eventArgs) =>
        {
            var selectedRecord = eventArgs.SelectedItems
                .OfType<FormListRecordViewModel>()
                .LastOrDefault();
            if (selectedRecord is not null)
            {
                await ViewModel.SelectRecordAsync(selectedRecord);
            }
        };
        AutomationProperties.SetAutomationId(recordTree, "FormListRecordTree");

        Grid.SetRow(title, 0);
        Grid.SetRow(actions, 1);
        Grid.SetRow(filters, 2);
        Grid.SetRow(selectedReference, 3);
        Grid.SetRow(recordTree, 4);
        return new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 0, 1, 0),
            Padding = new Thickness(12),
            Child = new Grid
            {
                RowDefinitions = new RowDefinitions("Auto,Auto,Auto,Auto,*"),
                RowSpacing = 10,
                Children =
                {
                    title,
                    actions,
                    filters,
                    selectedReference,
                    recordTree
                }
            }
        };
    }

    /// <summary>Gets the user-facing label for one supported record sort mode.</summary>
    /// <param name="mode">The record sort mode.</param>
    /// <returns>The canonical FormID or EditorID field label.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="mode"/> is undefined.</exception>
    private static string DescribeRecordSortMode(FormListRecordSortMode mode)
    {
        return mode switch
        {
            FormListRecordSortMode.FormId => "FormID",
            FormListRecordSortMode.EditorId => "EditorID",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unsupported FormList record sort mode.")
        };
    }

    /// <summary>Creates one two-way record filter text box.</summary>
    /// <param name="propertyName">The bound browser filter property.</param>
    /// <param name="watermark">The input watermark.</param>
    /// <param name="automationId">The stable automation identity.</param>
    /// <param name="column">The containing filter-grid column.</param>
    /// <returns>The configured filter input.</returns>
    private static TextBox CreateFilterTextBox(
        string propertyName,
        string watermark,
        string automationId,
        int column)
    {
        var textBox = new TextBox
        {
            PlaceholderText = watermark
        };
        textBox.Bind(TextBox.TextProperty, new Binding(propertyName));
        AutomationProperties.SetAutomationId(textBox, automationId);
        Grid.SetColumn(textBox, column);
        return textBox;
    }

    /// <summary>Builds selectors, exact provenance, independent JSON hierarchies, diagnostics, and loading feedback.</summary>
    /// <returns>The right comparison pane.</returns>
    private Control BuildComparisonPane()
    {
        var contextSelectors = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            ColumnSpacing = 12,
            Children =
            {
                BuildContextSelector(
                    "Prior context",
                    nameof(FormListBrowserViewModel.SelectedBeforeContext),
                    nameof(FormListBrowserViewModel.BeforeProvenanceText),
                    "FormListBeforeContextSelector",
                    ViewModel.SelectBeforeContextAsync,
                    0),
                BuildContextSelector(
                    "Resulting context",
                    nameof(FormListBrowserViewModel.SelectedAfterContext),
                    nameof(FormListBrowserViewModel.AfterProvenanceText),
                    "FormListAfterContextSelector",
                    ViewModel.SelectAfterContextAsync,
                    1)
            }
        };

        var fieldTrees = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            ColumnSpacing = 12,
            Children =
            {
                BuildFieldPane(
                    "Prior fields",
                    nameof(FormListBrowserViewModel.BeforeFieldSource),
                    "FormListBeforeFieldTree",
                    0),
                BuildFieldPane(
                    "Resulting fields",
                    nameof(FormListBrowserViewModel.AfterFieldSource),
                    "FormListAfterFieldTree",
                    1)
            }
        };

        var diagnostics = BuildDiagnosticsPane();
        var legend = BuildComparisonLegend();
        Grid.SetRow(contextSelectors, 0);
        Grid.SetRow(legend, 1);
        Grid.SetRow(fieldTrees, 2);
        Grid.SetRow(diagnostics, 3);
        var comparisonContent = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto,*,Auto"),
            RowSpacing = 12,
            Children =
            {
                contextSelectors,
                legend,
                fieldTrees,
                diagnostics
            }
        };
        var progress = new ProgressBar
        {
            IsIndeterminate = true,
            MinWidth = 280,
            MinHeight = 4
        };
        AutomationProperties.SetAutomationId(progress, "FormListComparisonProgress");
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
                    CreateText("Resolving record hierarchy...", 16, FontWeight.SemiBold),
                    progress
                }
            }
        };
        loading.Bind(IsVisibleProperty, new Binding(nameof(FormListBrowserViewModel.IsComparisonBusy)));
        AutomationProperties.SetAutomationId(loading, "FormListComparisonLoadingView");
        return new Grid
        {
            Children =
            {
                comparisonContent,
                loading
            }
        };
    }

    /// <summary>Builds the text-labelled retained comparison-color legend.</summary>
    /// <returns>The complete field-state legend.</returns>
    private static Control BuildComparisonLegend()
    {
        var legend = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 14,
            Children =
            {
                CreateLegendItem("Identical", ComparisonFieldState.Identical),
                CreateLegendItem("Conflict", ComparisonFieldState.Conflict),
                CreateLegendItem("Winning Override", ComparisonFieldState.WinningOverride)
            }
        };
        AutomationProperties.SetAutomationId(legend, "FormListComparisonLegend");
        return legend;
    }

    /// <summary>Creates one text-labelled field-state swatch.</summary>
    /// <param name="label">The visible state label.</param>
    /// <param name="state">The field state represented by the swatch.</param>
    /// <returns>The configured legend item.</returns>
    private static Control CreateLegendItem(string label, ComparisonFieldState state)
    {
        var labelText = CreateText(label, 12, FontWeight.Normal);
        labelText.VerticalAlignment = VerticalAlignment.Center;
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            Children =
            {
                new Border
                {
                    Width = 18,
                    Height = 10,
                    Background = FormListBrowserViewModel.GetComparisonFieldBrush(state),
                    BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
                    BorderThickness = new Thickness(1),
                    VerticalAlignment = VerticalAlignment.Center
                },
                labelText
            }
        };
    }

    /// <summary>Builds one exact-context selector and its engine-reported provenance.</summary>
    /// <param name="title">The selector title.</param>
    /// <param name="selectedProperty">The selected context property.</param>
    /// <param name="provenanceProperty">The engine provenance property.</param>
    /// <param name="automationId">The stable selector automation identity.</param>
    /// <param name="selectionAction">The asynchronous selection workflow.</param>
    /// <param name="column">The parent grid column.</param>
    /// <returns>The configured selector panel.</returns>
    private Control BuildContextSelector(
        string title,
        string selectedProperty,
        string provenanceProperty,
        string automationId,
        Func<FormListContextOption?, Task> selectionAction,
        int column)
    {
        var selector = new ComboBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MaxDropDownHeight = 260,
            ItemTemplate = new FuncDataTemplate<FormListContextOption>(
                (option, _) => new TextBlock { Text = option?.Label ?? string.Empty })
        };
        selector.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(FormListBrowserViewModel.ContextOptions)));
        selector.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(selectedProperty)
        {
            Mode = BindingMode.OneWay
        });
        selector.SelectionChanged += async (_, _) =>
            await selectionAction(selector.SelectedItem as FormListContextOption);
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

    /// <summary>Builds one independent hierarchical JSON record pane.</summary>
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

    /// <summary>Builds warnings, typed errors, retry, and operation status.</summary>
    /// <returns>The comparison diagnostics pane.</returns>
    private Control BuildDiagnosticsPane()
    {
        var warnings = new ItemsControl
        {
            ItemTemplate = new FuncDataTemplate<EngineWarning>(
                (warning, _) => CreateWrappedText(
                    warning is null ? string.Empty : $"{warning.Code}: {warning.Message}",
                    12))
        };
        warnings.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(FormListBrowserViewModel.Warnings)));
        AutomationProperties.SetAutomationId(warnings, "FormListWarnings");

        var error = CreateBoundText(nameof(FormListBrowserViewModel.ErrorMessage), 12, FontWeight.Normal);
        error.TextWrapping = TextWrapping.Wrap;
        AutomationProperties.SetAutomationId(error, "FormListErrorText");
        var retry = new Button
        {
            Content = "Retry",
            Padding = new Thickness(12, 6),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        retry.Bind(Button.CommandProperty, new Binding(nameof(FormListBrowserViewModel.RetryCommand)));
        AutomationProperties.SetAutomationId(retry, "FormListRetryButton");
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
        errorPanel.Bind(IsVisibleProperty, new Binding(nameof(FormListBrowserViewModel.HasError)));
        AutomationProperties.SetAutomationId(errorPanel, "FormListErrorPanel");

        var status = CreateBoundText(nameof(FormListBrowserViewModel.StatusText), 12, FontWeight.Normal);
        status.TextWrapping = TextWrapping.Wrap;
        AutomationProperties.SetAutomationId(status, "FormListBrowserStatusText");
        var diagnosticDetails = new StackPanel
        {
            Spacing = 6,
            Children =
            {
                CreateText("Warnings", 13, FontWeight.SemiBold),
                warnings
            }
        };
        var diagnosticsScroller = new ScrollViewer
        {
            MaxHeight = 120,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = diagnosticDetails
        };
        AutomationProperties.SetAutomationId(diagnosticsScroller, "FormListDiagnosticsScroller");
        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                diagnosticsScroller,
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
        var textBlock = new TextBlock
        {
            Text = text,
            FontSize = fontSize,
            FontWeight = fontWeight
        };
        App.ApplyApplicationTextForeground(textBlock);
        return textBlock;
    }

    /// <summary>Creates application-styled diagnostic text that wraps inside its fixed comparison pane.</summary>
    /// <param name="text">The visible diagnostic text.</param>
    /// <param name="fontSize">The font size.</param>
    /// <returns>The configured wrapping text block.</returns>
    private static TextBlock CreateWrappedText(string text, double fontSize)
    {
        var textBlock = CreateText(text, fontSize, FontWeight.Normal);
        textBlock.TextWrapping = TextWrapping.Wrap;
        textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
        textBlock.Bind(TextBlock.MaxWidthProperty, new Binding("Viewport.Width")
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
            {
                AncestorType = typeof(ScrollViewer)
            }
        });
        return textBlock;
    }

    /// <summary>Creates application-styled text bound to one browser property.</summary>
    /// <param name="propertyName">The source property name.</param>
    /// <param name="fontSize">The font size.</param>
    /// <param name="fontWeight">The font weight.</param>
    /// <returns>The configured bound text block.</returns>
    private static TextBlock CreateBoundText(string propertyName, double fontSize, FontWeight fontWeight)
    {
        var textBlock = CreateText(string.Empty, fontSize, fontWeight);
        textBlock.Bind(TextBlock.TextProperty, new Binding(propertyName));
        return textBlock;
    }
}
