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
/// Presents native FormLists as winning roots with exact contexts and an independent two-context typed comparison.
/// </summary>
public sealed class NativeFormListBrowserView : UserControl
{
    /// <summary>The navigation-scope browser workflow and detached presentation state.</summary>
    private readonly NativeFormListBrowserViewModel ViewModel;

    /// <summary>Tracks whether initial browser loading has started for this attached view.</summary>
    private bool Started;

    /// <summary>Initializes the native FormList browser view.</summary>
    /// <param name="viewModel">The navigation-scope browser workflow.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is <see langword="null"/>.</exception>
    public NativeFormListBrowserView(NativeFormListBrowserViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ViewModel = viewModel;
        DataContext = ViewModel;
        AutomationProperties.SetAutomationId(this, "NativeFormListBrowserView");
        Content = BuildContent();
    }

    /// <summary>Starts native loading once this navigation-owned view enters the visual tree.</summary>
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
        AutomationProperties.SetAutomationId(layout, "NativeFormListBrowserLayout");
        return layout;
    }

    /// <summary>Builds retained comparison and typed editing as sibling tabs in the right pane.</summary>
    /// <returns>The complete right-side details pane.</returns>
    private Control BuildDetailsPane()
    {
        var compare = new TabItem
        {
            Header = "Compare",
            Content = BuildComparisonPane()
        };
        AutomationProperties.SetAutomationId(compare, "NativeFormListCompareTab");
        var edit = new TabItem
        {
            Header = "Edit",
            Content = new NativeFormListEditorView(ViewModel.Editor)
        };
        AutomationProperties.SetAutomationId(edit, "NativeFormListEditTab");
        var tabs = new TabControl
        {
            ItemsSource = new[] { compare, edit },
            SelectedIndex = 0
        };
        AutomationProperties.SetAutomationId(tabs, "NativeFormListDetailsTabs");
        return tabs;
    }

    /// <summary>Builds native record discovery and exact-context tree selection.</summary>
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
        pickerButton.Bind(Button.CommandProperty, new Binding(nameof(NativeFormListBrowserViewModel.PickReferenceCommand)));
        AutomationProperties.SetAutomationId(pickerButton, "NativeFormListReferencePickerButton");

        var refreshButton = new Button
        {
            Content = "Refresh",
            Padding = new Thickness(12, 7),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        refreshButton.Bind(Button.CommandProperty, new Binding(nameof(NativeFormListBrowserViewModel.RefreshCommand)));
        AutomationProperties.SetAutomationId(refreshButton, "NativeFormListRefreshButton");

        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                pickerButton,
                refreshButton
            }
        };

        var filters = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            ColumnSpacing = 8,
            Children =
            {
                CreateFilterTextBox(
                    nameof(NativeFormListBrowserViewModel.FormIdFilter),
                    "FormID",
                    "NativeFormListFormIdFilter",
                    0),
                CreateFilterTextBox(
                    nameof(NativeFormListBrowserViewModel.EditorIdFilter),
                    "EditorID",
                    "NativeFormListEditorIdFilter",
                    1)
            }
        };

        var selectedReference = CreateBoundText(
            nameof(NativeFormListBrowserViewModel.SelectedReferenceText),
            12,
            FontWeight.Normal);
        selectedReference.TextWrapping = TextWrapping.Wrap;
        AutomationProperties.SetAutomationId(selectedReference, "NativeFormListSelectedReferenceText");

        var recordTree = new TreeDataGrid
        {
            CanUserResizeColumns = true,
            CanUserSortColumns = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        recordTree.Bind(TreeDataGrid.SourceProperty, new Binding(nameof(NativeFormListBrowserViewModel.RecordTreeSource)));
        recordTree.SelectionChanged += async (_, eventArgs) =>
        {
            var selectedRecord = eventArgs.SelectedItems
                .OfType<NativeFormListRecordViewModel>()
                .LastOrDefault();
            if (selectedRecord is not null)
            {
                await ViewModel.SelectRecordAsync(selectedRecord);
            }
        };
        AutomationProperties.SetAutomationId(recordTree, "NativeFormListRecordTree");

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

    /// <summary>Creates one two-way native record filter text box.</summary>
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

    /// <summary>Builds selectors, exact provenance, independent JSON hierarchies, and semantic diagnostics.</summary>
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
                    nameof(NativeFormListBrowserViewModel.SelectedBeforeContext),
                    nameof(NativeFormListBrowserViewModel.BeforeProvenanceText),
                    "NativeFormListBeforeContextSelector",
                    ViewModel.SelectBeforeContextAsync,
                    0),
                BuildContextSelector(
                    "Resulting context",
                    nameof(NativeFormListBrowserViewModel.SelectedAfterContext),
                    nameof(NativeFormListBrowserViewModel.AfterProvenanceText),
                    "NativeFormListAfterContextSelector",
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
                    nameof(NativeFormListBrowserViewModel.BeforeFieldSource),
                    "NativeFormListBeforeFieldTree",
                    0),
                BuildFieldPane(
                    "Resulting fields",
                    nameof(NativeFormListBrowserViewModel.AfterFieldSource),
                    "NativeFormListAfterFieldTree",
                    1)
            }
        };

        var diagnostics = BuildDiagnosticsPane();
        var legend = BuildComparisonLegend();
        Grid.SetRow(contextSelectors, 0);
        Grid.SetRow(legend, 1);
        Grid.SetRow(fieldTrees, 2);
        Grid.SetRow(diagnostics, 3);
        return new Grid
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
                CreateLegendItem("Identical", NativeComparisonFieldState.Identical),
                CreateLegendItem("Conflict", NativeComparisonFieldState.Conflict),
                CreateLegendItem("Winning Override", NativeComparisonFieldState.WinningOverride)
            }
        };
        AutomationProperties.SetAutomationId(legend, "NativeFormListComparisonLegend");
        return legend;
    }

    /// <summary>Creates one text-labelled field-state swatch.</summary>
    /// <param name="label">The visible state label.</param>
    /// <param name="state">The field state represented by the swatch.</param>
    /// <returns>The configured legend item.</returns>
    private static Control CreateLegendItem(string label, NativeComparisonFieldState state)
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
                    Background = NativeFormListBrowserViewModel.GetComparisonFieldBrush(state),
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
        Func<NativeFormListContextOption?, Task> selectionAction,
        int column)
    {
        var selector = new ComboBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MaxDropDownHeight = 260,
            ItemTemplate = new FuncDataTemplate<NativeFormListContextOption>(
                (option, _) => new TextBlock { Text = option?.Label ?? string.Empty })
        };
        selector.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(NativeFormListBrowserViewModel.ContextOptions)));
        selector.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(selectedProperty)
        {
            Mode = BindingMode.OneWay
        });
        selector.SelectionChanged += async (_, _) =>
            await selectionAction(selector.SelectedItem as NativeFormListContextOption);
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

    /// <summary>Builds semantic changes, warnings, typed errors, retry, and operation status.</summary>
    /// <returns>The comparison diagnostics pane.</returns>
    private Control BuildDiagnosticsPane()
    {
        var changes = new ItemsControl
        {
            ItemTemplate = new FuncDataTemplate<SemanticChangeDescriptor>(
                (change, _) => CreateWrappedText(FormatChange(change), 12))
        };
        changes.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(NativeFormListBrowserViewModel.SemanticChanges)));
        AutomationProperties.SetAutomationId(changes, "NativeFormListSemanticChanges");

        var warnings = new ItemsControl
        {
            ItemTemplate = new FuncDataTemplate<EngineWarning>(
                (warning, _) => CreateWrappedText(
                    warning is null ? string.Empty : $"{warning.Code}: {warning.Message}",
                    12))
        };
        warnings.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(NativeFormListBrowserViewModel.Warnings)));
        AutomationProperties.SetAutomationId(warnings, "NativeFormListWarnings");

        var error = CreateBoundText(nameof(NativeFormListBrowserViewModel.ErrorMessage), 12, FontWeight.Normal);
        error.TextWrapping = TextWrapping.Wrap;
        AutomationProperties.SetAutomationId(error, "NativeFormListErrorText");
        var retry = new Button
        {
            Content = "Retry",
            Padding = new Thickness(12, 6),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        retry.Bind(Button.CommandProperty, new Binding(nameof(NativeFormListBrowserViewModel.RetryCommand)));
        AutomationProperties.SetAutomationId(retry, "NativeFormListRetryButton");
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
        errorPanel.Bind(IsVisibleProperty, new Binding(nameof(NativeFormListBrowserViewModel.HasError)));
        AutomationProperties.SetAutomationId(errorPanel, "NativeFormListErrorPanel");

        var status = CreateBoundText(nameof(NativeFormListBrowserViewModel.StatusText), 12, FontWeight.Normal);
        status.TextWrapping = TextWrapping.Wrap;
        AutomationProperties.SetAutomationId(status, "NativeFormListBrowserStatusText");
        var diagnosticDetails = new StackPanel
        {
            Spacing = 6,
            Children =
            {
                CreateText("Semantic changes", 13, FontWeight.SemiBold),
                changes,
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
        AutomationProperties.SetAutomationId(diagnosticsScroller, "NativeFormListDiagnosticsScroller");
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

    /// <summary>Formats one semantic descriptor without interpreting its field identifier as a JSON path.</summary>
    /// <param name="change">The exact engine descriptor.</param>
    /// <returns>The verbatim field identifier, kind, and optional positions.</returns>
    private static string FormatChange(SemanticChangeDescriptor? change)
    {
        if (change is null)
        {
            return string.Empty;
        }

        var before = change.BeforePosition.HasValue ? change.BeforePosition.Value.ToString() : "-";
        var after = change.AfterPosition.HasValue ? change.AfterPosition.Value.ToString() : "-";
        return $"{change.FieldIdentifier} | {change.Kind} | before {before} | after {after}";
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
