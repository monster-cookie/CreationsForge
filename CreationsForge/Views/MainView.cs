using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.ViewModels;

namespace CreationsForge.Views;

public class MainView : UserControl
{
    private readonly MainViewModel ViewModel;
    private readonly AssetPreviewPaneView AssetPreviewPaneView;
    private bool Started;

    public MainView(MainViewModel viewModel, AssetPreviewPaneView assetPreviewPaneView)
    {
        ViewModel = viewModel;
        AssetPreviewPaneView = assetPreviewPaneView;
        DataContext = ViewModel;
        AutomationProperties.SetAutomationId(this, "MainView");
        Content = BuildContent();
    }

    public void Configure(SupportedGameDTO? selectedGame, bool runConfiguredGameImport, PluginDTO? selectedPlugin = null, IList<RecordTreeItemViewModel>? recordTreeItems = null)
    {
        Started = false;
        ViewModel.Configure(selectedGame, runConfiguredGameImport, selectedPlugin, recordTreeItems);
    }

    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (Started)
        {
            return;
        }

        Started = true;
        await ViewModel.StartAsync();
    }

    private Control BuildContent()
    {
        var root = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*,Auto"),
            ColumnDefinitions = new ColumnDefinitions("3*,7*")
        };

        var header = BuildHeader();
        Grid.SetRow(header, 0);
        Grid.SetColumnSpan(header, 2);
        root.Children.Add(header);

        var recordTreePane = BuildRecordTreePane();
        Grid.SetRow(recordTreePane, 1);
        Grid.SetColumn(recordTreePane, 0);
        root.Children.Add(recordTreePane);

        var workspace = BuildWorkspace();
        Grid.SetRow(workspace, 1);
        Grid.SetColumn(workspace, 1);
        root.Children.Add(workspace);

        var statusBar = BuildStatusBar();
        Grid.SetRow(statusBar, 2);
        Grid.SetColumnSpan(statusBar, 2);
        root.Children.Add(statusBar);

        return new Border
        {
            Background = App.GetApplicationBrush(App.ApplicationSurfaceBrushKey),
            Child = root
        };
    }

    private Control BuildHeader()
    {
        return new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            Padding = new Thickness(20, 14),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Child = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"),
                ColumnSpacing = 18,
                Children =
                {
                    CreateToolbarButton("Open Plugin", nameof(MainViewModel.OpenPluginCommand), "OpenPluginButton"),
                    CreateActiveSelectionSummary(),
                    CreateToolbar()
                }
            }
        };
    }

    private Control BuildRecordTreePane()
    {
        var toggleButton = new Button
        {
            Width = 36,
            Height = 32,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        toggleButton.Bind(ContentControl.ContentProperty, new Binding(nameof(MainViewModel.RecordTreePaneToggleText)));
        toggleButton.Bind(Button.CommandProperty, new Binding(nameof(MainViewModel.ToggleRecordTreePaneCommand)));

        var filterGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            ColumnSpacing = 8,
            Children =
            {
                CreateFilterTextBox(nameof(MainViewModel.FormIDFilter), "FormID", 0),
                CreateFilterTextBox(nameof(MainViewModel.EditorIDFilter), "EditorID", 1)
            }
        };
        var activePluginLoading = BuildActivePluginLoadingIndicator();
        var recordSections = new ItemsControl
        {
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        recordSections.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(MainViewModel.RecordTreeItems)));
        recordSections.ItemTemplate = new FuncDataTemplate<RecordTreeItemViewModel>(
            (item, _) => BuildRecordTypeSection(item));

        var sectionsScrollViewer = new ScrollViewer
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = recordSections
        };

        var content = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto,*"),
            RowSpacing = 10
        };
        Grid.SetRow(toggleButton, 0);
        content.Children.Add(toggleButton);
        Grid.SetRow(filterGrid, 1);
        content.Children.Add(filterGrid);
        Grid.SetRow(activePluginLoading, 2);
        content.Children.Add(activePluginLoading);
        Grid.SetRow(sectionsScrollViewer, 3);
        content.Children.Add(sectionsScrollViewer);
        content.Bind(IsVisibleProperty, new Binding(nameof(MainViewModel.IsRecordTreePaneContentVisible)));

        var border = new Border
        {
            Background = App.GetApplicationBrush(App.ApplicationSurfaceBrushKey),
            Padding = new Thickness(12),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 0, 1, 0),
            Child = content
        };
        return border;
    }

    private static Control BuildActivePluginLoadingIndicator()
    {
        var loadingText = new TextBlock
        {
            FontSize = 12
        };
        App.ApplyApplicationTextForeground(loadingText);
        loadingText.Bind(TextBlock.TextProperty, new Binding(nameof(MainViewModel.ActivePluginLoadingText)));

        var progress = new ProgressBar
        {
            IsIndeterminate = true,
            MinHeight = 4
        };

        var panel = new StackPanel
        {
            Spacing = 6,
            Children =
            {
                loadingText,
                progress
            }
        };
        panel.Bind(IsVisibleProperty, new Binding(nameof(MainViewModel.IsActivePluginLoading)));
        return panel;
    }

    private Control BuildRecordTypeSection(RecordTreeItemViewModel item)
    {
        var records = BuildRecordTreeDataGrid(item);
        records.IsVisible = false;

        var header = BuildRecordTypeSectionHeader(item.DisplayFormIDText);
        header.GetObservable(ToggleButton.IsCheckedProperty)
            .Subscribe(isChecked => records.IsVisible = isChecked == true);

        return new StackPanel
        {
            Margin = new Thickness(0, 0, 0, 8),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Children =
            {
                header,
                records
            }
        };
    }

    private static ToggleButton BuildRecordTypeSectionHeader(string title)
    {
        var textBlock = new TextBlock
        {
            Text = title,
            FontWeight = FontWeight.SemiBold,
            VerticalAlignment = VerticalAlignment.Center
        };
        App.ApplyApplicationTextForeground(textBlock);

        var arrow = new TextBlock
        {
            Text = "v",
            FontWeight = FontWeight.SemiBold,
            VerticalAlignment = VerticalAlignment.Center
        };
        App.ApplyApplicationTextForeground(arrow);

        var headerContent = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            Children =
            {
                textBlock,
                arrow
            }
        };
        Grid.SetColumn(arrow, 1);

        var header = new ToggleButton
        {
            Background = new SolidColorBrush(Colors.Transparent),
            BorderThickness = new Thickness(0),
            Padding = new Thickness(4, 6),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            Content = headerContent
        };
        header.GetObservable(ToggleButton.IsCheckedProperty)
            .Subscribe(isChecked => arrow.Text = isChecked == true ? "^" : "v");
        return header;
    }

    private Control BuildRecordTreeDataGrid(RecordTreeItemViewModel item)
    {
        var selectionChangedSincePointerPress = false;
        var source = new FlatTreeDataGridSource<RecordTreeItemViewModel>(item.Children)
            .WithTextColumn("FormID", record => record.FormIDText, options => options.BeginEditGestures = BeginEditGestures.None)
            .WithTextColumn("EditorID", record => record.EditorID, options => options.BeginEditGestures = BeginEditGestures.None)
            .WithTextColumn("Plugins", record => record.PluginCountText, options => options.BeginEditGestures = BeginEditGestures.None);
        var records = new TreeDataGrid
        {
            CanUserResizeColumns = true,
            CanUserSortColumns = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MinHeight = 44,
            Source = source
        };
        records.PointerPressed += (_, _) => selectionChangedSincePointerPress = false;
        records.SelectionChanged += (_, e) =>
        {
            var selectedRecord = e.SelectedItems
                .OfType<RecordTreeItemViewModel>()
                .LastOrDefault();
            if (selectedRecord is not null)
            {
                selectionChangedSincePointerPress = true;
                ViewModel.SelectRecordForComparison(selectedRecord);
            }
        };
        records.Tapped += (_, _) =>
        {
            if (!selectionChangedSincePointerPress &&
                source.RowSelection?.SelectedItem is RecordTreeItemViewModel selectedRecord)
            {
                ViewModel.SelectRecordForComparison(selectedRecord);
            }
        };

        return records;
    }

    private Control BuildWorkspace()
    {
        var comparisonTitle = CreateBoundText(nameof(MainViewModel.RecordComparisonTitleText), 20, FontWeight.SemiBold);
        var comparisonGrid = new TreeDataGrid
        {
            CanUserResizeColumns = true,
            CanUserSortColumns = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        AutomationProperties.SetAutomationId(comparisonGrid, "RecordComparisonGrid");
        comparisonGrid.Bind(TreeDataGrid.SourceProperty, new Binding(nameof(MainViewModel.RecordComparisonSource)));
        Grid.SetRow(comparisonTitle, 0);
        Grid.SetRow(comparisonGrid, 1);

        var workspaceGrid = new Grid
        {
            ColumnDefinitions = GetWorkspaceColumnDefinitions(),
            ColumnSpacing = 0
        };
        var comparisonPane = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*"),
            RowSpacing = 12,
            Children =
            {
                comparisonTitle,
                comparisonGrid
            }
        };
        Grid.SetColumn(comparisonPane, 0);
        workspaceGrid.Children.Add(comparisonPane);
        AssetPreviewPaneView.Bind(IsVisibleProperty, new Binding(nameof(AssetPreviewPaneViewModel.HasPreviewCandidates)));
        Grid.SetColumn(AssetPreviewPaneView, 1);
        workspaceGrid.Children.Add(AssetPreviewPaneView);
        ViewModel.AssetPreviewPane.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(AssetPreviewPaneViewModel.HasPreviewCandidates))
            {
                workspaceGrid.ColumnDefinitions = GetWorkspaceColumnDefinitions();
            }
        };

        return new Border
        {
            Background = App.GetApplicationBrush(App.ApplicationSurfaceBrushKey),
            Padding = new Thickness(28),
            Child = workspaceGrid
        };
    }

    private ColumnDefinitions GetWorkspaceColumnDefinitions()
    {
        return ViewModel.AssetPreviewPane.HasPreviewCandidates
            ? new ColumnDefinitions("7*,3*")
            : new ColumnDefinitions("*,0");
    }

    private Control BuildStatusBar()
    {
        return new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            Padding = new Thickness(14, 6),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 1, 0, 0),
            Child = new StackPanel
            {
                Spacing = 4,
                Children =
                {
                    BuildComparisonLegend(),
                    CreateBoundText(nameof(MainViewModel.StatusText), 14, FontWeight.Normal)
                }
            }
        };
    }

    private static Control BuildComparisonLegend()
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 14,
            Children =
            {
                CreateLegendItem("Identical", RecordComparisonValueState.Identical),
                CreateLegendItem("Conflict", RecordComparisonValueState.Conflict),
                CreateLegendItem("Winning Override", RecordComparisonValueState.WinningOverride)
            }
        };
    }

    private static Control CreateLegendItem(string label, RecordComparisonValueState state)
    {
        var textBlock = new TextBlock
        {
            Text = label,
            FontSize = 12,
            VerticalAlignment = VerticalAlignment.Center
        };
        App.ApplyApplicationTextForeground(textBlock);

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
                    Background = GetComparisonValueBrush(state),
                    BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
                    BorderThickness = new Thickness(1),
                    VerticalAlignment = VerticalAlignment.Center
                },
                textBlock
            }
        };
    }

    private static Control CreateSearchPanel(string label, Control searchBox, int column)
    {
        var labelBlock = new TextBlock
        {
            Text = label,
            FontWeight = FontWeight.SemiBold
        };
        App.ApplyApplicationTextForeground(labelBlock);

        var panel = new StackPanel
        {
            Spacing = 6,
            Children =
            {
                labelBlock,
                searchBox
            }
        };
        Grid.SetColumn(panel, column);
        return panel;
    }

    private Control CreateToolbar()
    {
        var toolbar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            VerticalAlignment = VerticalAlignment.Bottom,
            Children =
            {
                CreateToolbarButton("Reimport", nameof(MainViewModel.ReimportSelectedGameCommand), "ReimportButton"),
                CreateToolbarButton("Reset & Import All", nameof(MainViewModel.ResetAndImportAllCommand), "ResetAndImportAllButton"),
                CreateToolbarButton("Settings", nameof(MainViewModel.ShowSettingsCommand), "SettingsButton")
            }
        };
        AutomationProperties.SetAutomationId(toolbar, "MainToolbar");
        Grid.SetColumn(toolbar, 2);
        return toolbar;
    }

    private static Control CreateActiveSelectionSummary()
    {
        var activeGame = CreateBoundText(nameof(MainViewModel.ActiveGameStatusText), 13, FontWeight.SemiBold);
        var activePlugin = CreateBoundText(nameof(MainViewModel.ActivePluginStatusText), 13, FontWeight.Normal);
        var importedRecords = CreateBoundText(nameof(MainViewModel.ImportedRecordCountText), 12, FontWeight.Normal);
        var panel = new StackPanel
        {
            Spacing = 2,
            VerticalAlignment = VerticalAlignment.Center,
            Children =
            {
                activeGame,
                activePlugin,
                importedRecords
            }
        };
        Grid.SetColumn(panel, 1);
        return panel;
    }

    private static Button CreateToolbarButton(string content, string commandProperty, string automationId)
    {
        var button = new Button
        {
            Content = content,
            Padding = new Thickness(16, 8)
        };
        AutomationProperties.SetAutomationId(button, automationId);
        button.Bind(Button.CommandProperty, new Binding(commandProperty));
        return button;
    }

    private ComboBox CreateComboBox(
        string itemsSourceProperty,
        string textProperty,
        string selectedItemProperty,
        bool isEditable,
        Func<string, Task> selectionAction,
        Func<string, Task> submitAction,
        Action<string>? textChangedAction,
        Func<string, bool>? exactMatchPredicate = null)
    {
        var comboBox = new ComboBox
        {
            IsEditable = isEditable,
            MaxDropDownHeight = 220,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MinWidth = 360
        };
        comboBox.Bind(ItemsControl.ItemsSourceProperty, new Binding(itemsSourceProperty));
        comboBox.Bind(ComboBox.TextProperty, new Binding(textProperty)
        {
            Mode = BindingMode.TwoWay
        });
        comboBox.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(selectedItemProperty)
        {
            Mode = BindingMode.TwoWay
        });
        comboBox.GetObservable(SelectingItemsControl.SelectedItemProperty)
            .Subscribe(async selectedItem =>
            {
                if (selectedItem is string selectedText)
                {
                    await selectionAction(selectedText);
                }
            });
        comboBox.KeyDown += async (_, e) =>
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            await submitAction(comboBox.Text ?? string.Empty);
            e.Handled = true;
        };
        comboBox.LostFocus += async (_, _) =>
        {
            await submitAction(comboBox.Text ?? string.Empty);
        };

        if (textChangedAction is not null)
        {
            comboBox.GetObservable(ComboBox.TextProperty)
                .Subscribe(async text =>
                {
                    var searchText = text ?? string.Empty;
                    textChangedAction(searchText);
                    if (string.IsNullOrWhiteSpace(searchText))
                    {
                        comboBox.IsDropDownOpen = false;
                        return;
                    }

                    if (exactMatchPredicate?.Invoke(searchText) == true)
                    {
                        await submitAction(searchText);
                        comboBox.IsDropDownOpen = false;
                        return;
                    }

                    comboBox.IsDropDownOpen = true;
                });
        }

        return comboBox;
    }

    private ComboBox CreatePluginComboBox()
    {
        var comboBox = CreateComboBox(
            nameof(MainViewModel.PluginSuggestions),
            nameof(MainViewModel.ActivePluginSearchText),
            nameof(MainViewModel.SelectedPluginFileName),
            isEditable: true,
            text =>
            {
                ViewModel.ChoosePluginSuggestion(text);
                return Task.CompletedTask;
            },
            text =>
            {
                ViewModel.SubmitPluginQuery(text);
                return Task.CompletedTask;
            },
            ViewModel.UpdatePluginSearchText,
            ViewModel.IsExactPluginSuggestion);

        comboBox.ItemTemplate = new FuncDataTemplate<PluginSuggestionViewModel>(
            (plugin, _) => plugin is null
                ? new TextBlock()
                : CreatePluginSuggestionItem(plugin));
        comboBox.GetObservable(SelectingItemsControl.SelectedItemProperty)
            .Subscribe(selectedItem =>
            {
                if (selectedItem is PluginSuggestionViewModel plugin)
                {
                    ViewModel.ChoosePluginSuggestion(plugin.FileName);
                }
            });
        return comboBox;
    }

    private static Control CreatePluginSuggestionItem(PluginSuggestionViewModel plugin)
    {
        return new TextBlock
        {
            Text = plugin.FileName,
            Foreground = plugin.StatusBrush,
            TextTrimming = TextTrimming.CharacterEllipsis,
            VerticalAlignment = VerticalAlignment.Center
        };
    }

    private TextBox CreateFilterTextBox(string boundProperty, string watermark, int column)
    {
        var textBox = CreateLabeledTextBox(boundProperty, watermark);
        Grid.SetColumn(textBox, column);
        return textBox;
    }

    private TextBox CreateLabeledTextBox(string boundProperty, string watermark)
    {
        var textBox = new TextBox
        {
            PlaceholderText = watermark
        };
        textBox.Bind(TextBox.TextProperty, new Binding(boundProperty));
        return textBox;
    }

    private static TextBlock CreateBoundText(string boundProperty, double fontSize, FontWeight fontWeight)
    {
        var textBlock = new TextBlock
        {
            FontSize = fontSize,
            FontWeight = fontWeight,
            TextWrapping = TextWrapping.Wrap
        };
        App.ApplyApplicationTextForeground(textBlock);
        textBlock.Bind(TextBlock.TextProperty, new Binding(boundProperty));
        return textBlock;
    }

    private static IBrush GetComparisonValueBrush(RecordComparisonValueState state)
    {
        return state switch
        {
            RecordComparisonValueState.Identical => new SolidColorBrush(Color.FromArgb(80, 0, 128, 0)),
            RecordComparisonValueState.Conflict => new SolidColorBrush(Color.FromArgb(80, 192, 0, 0)),
            RecordComparisonValueState.WinningOverride => new SolidColorBrush(Color.FromArgb(80, 192, 160, 0)),
            _ => new SolidColorBrush(Colors.Transparent)
        };
    }
}
