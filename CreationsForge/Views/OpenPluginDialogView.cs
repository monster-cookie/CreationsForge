using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.ViewModels;

namespace CreationsForge.Views;

public class OpenPluginDialogView : UserControl
{
    private readonly OpenPluginDialogViewModel ViewModel;
    private readonly Action<bool> CloseAction;
    private readonly IList<Button> GameButtons = new List<Button>();
    private Button? PrimaryActionButton;
    private CancellationTokenSource? PrimaryActionPulseCancellationTokenSource;
    private bool LastPrimaryActionWasImport;

    public OpenPluginDialogView(OpenPluginDialogViewModel viewModel, Action<bool> closeAction)
    {
        ViewModel = viewModel;
        CloseAction = closeAction;
        DataContext = ViewModel;
        AutomationProperties.SetAutomationId(this, "OpenPluginDialogView");
        Content = BuildContent();
        LastPrimaryActionWasImport = ViewModel.HasNoPlugins;
        ViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(OpenPluginDialogViewModel.SelectedGame))
            {
                RefreshGameButtons();
            }

            if (e.PropertyName == nameof(OpenPluginDialogViewModel.PrimaryActionText) ||
                e.PropertyName == nameof(OpenPluginDialogViewModel.HasNoPlugins))
            {
                PulsePrimaryActionButtonWhenImportBecomesAvailable();
            }
        };
        RefreshGameButtons();
    }

    private Control BuildContent()
    {
        var root = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto,*,Auto"),
            RowSpacing = 12,
            Margin = new Thickness(16)
        };

        var title = CreateText("Open Plugin", 20, FontWeight.SemiBold);
        Grid.SetRow(title, 0);
        root.Children.Add(title);

        var gameSelector = BuildGameSelector();
        Grid.SetRow(gameSelector, 1);
        root.Children.Add(gameSelector);

        var body = BuildBody();
        Grid.SetRow(body, 2);
        root.Children.Add(body);

        var footer = BuildFooter();
        Grid.SetRow(footer, 3);
        root.Children.Add(footer);

        return root;
    }

    private Control BuildGameSelector()
    {
        var label = CreateText("Game:", 14, FontWeight.Normal);
        label.VerticalAlignment = VerticalAlignment.Center;
        var gameButtons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 4
        };

        foreach (var game in ViewModel.SupportedGames)
        {
            var button = new Button
            {
                Content = game.DisplayName,
                MinWidth = 120,
                Padding = new Thickness(16, 6),
                Tag = game
            };
            button.Click += (_, _) => ViewModel.SelectGame(game);
            GameButtons.Add(button);
            gameButtons.Children.Add(button);
        }

        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 10,
            Children =
            {
                label,
                new Border
                {
                    BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(4),
                    Child = gameButtons
                }
            }
        };
    }

    private Control BuildBody()
    {
        var pluginPane = BuildPluginPane();
        var detailsPane = BuildDetailsPane();

        return new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("7*,3*"),
            ColumnSpacing = 14,
            Children =
            {
                pluginPane,
                detailsPane
            }
        };
    }

    private Control BuildPluginPane()
    {
        var searchBox = new TextBox
        {
            PlaceholderText = "Search plugins...",
            MinHeight = 34
        };
        searchBox.Bind(TextBox.TextProperty, new Binding(nameof(OpenPluginDialogViewModel.SearchText))
        {
            Mode = BindingMode.TwoWay
        });

        var stateFilter = new ComboBox
        {
            MinWidth = 180
        };
        stateFilter.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(OpenPluginDialogViewModel.ImportStateFilters)));
        stateFilter.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(OpenPluginDialogViewModel.SelectedImportStateFilter))
        {
            Mode = BindingMode.TwoWay
        });

        var sortBox = new ComboBox
        {
            MinWidth = 150
        };
        sortBox.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(OpenPluginDialogViewModel.SortOptions)));
        sortBox.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(OpenPluginDialogViewModel.SelectedSortOption))
        {
            Mode = BindingMode.TwoWay
        });

        var refreshButton = new Button
        {
            Content = "Refresh",
            Padding = new Thickness(14, 6)
        };
        refreshButton.Bind(Button.CommandProperty, new Binding(nameof(OpenPluginDialogViewModel.RefreshCommand)));

        var toolbar = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto,Auto,Auto"),
            ColumnSpacing = 8,
            Children =
            {
                searchBox,
                stateFilter,
                sortBox,
                refreshButton
            }
        };
        Grid.SetColumn(stateFilter, 1);
        Grid.SetColumn(sortBox, 2);
        Grid.SetColumn(refreshButton, 3);

        var header = BuildPluginListHeader();
        var listBox = new ListBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            MinHeight = 0
        };
        ScrollViewer.SetVerticalScrollBarVisibility(listBox, ScrollBarVisibility.Auto);
        ScrollViewer.SetHorizontalScrollBarVisibility(listBox, ScrollBarVisibility.Disabled);
        AutomationProperties.SetAutomationId(listBox, "OpenPluginList");
        listBox.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(OpenPluginDialogViewModel.PluginRows)));
        listBox.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(OpenPluginDialogViewModel.SelectedPluginRow))
        {
            Mode = BindingMode.TwoWay
        });
        listBox.ItemTemplate = new FuncDataTemplate<OpenPluginRowViewModel>(
            (plugin, _) => plugin is null ? new TextBlock() : BuildPluginRow(plugin));

        var emptyListText = CreateText(string.Empty, 14, FontWeight.Normal);
        emptyListText.HorizontalAlignment = HorizontalAlignment.Center;
        emptyListText.VerticalAlignment = VerticalAlignment.Center;
        emptyListText.Bind(TextBlock.TextProperty, new Binding(nameof(OpenPluginDialogViewModel.EmptyPluginListText)));
        emptyListText.Bind(IsVisibleProperty, new Binding(nameof(OpenPluginDialogViewModel.HasNoPlugins)));

        var listBody = new Grid
        {
            Children =
            {
                listBox,
                emptyListText
            }
        };

        var listSurface = new Border
        {
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(1),
            Child = new Grid
            {
                RowDefinitions = new RowDefinitions("Auto,*"),
                Children =
                {
                    header,
                    listBody
                }
            }
        };
        Grid.SetRow(listBody, 1);
        Grid.SetRow(listSurface, 1);

        return new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*"),
            RowSpacing = 8,
            Children =
            {
                toolbar,
                listSurface
            }
        };
    }

    private static Control BuildPluginListHeader()
    {
        return new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(8, 6),
            Child = CreatePluginRowGrid(
                CreateHeaderText("Plugin"),
                CreateHeaderText("Load Order"),
                CreateHeaderText("Records"),
                CreateHeaderText("State"),
                CreateHeaderText("Last Imported"))
        };
    }

    private Control BuildPluginRow(OpenPluginRowViewModel plugin)
    {
        var row = new Border
        {
            Padding = new Thickness(8, 4),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Child = CreatePluginRowGrid(
                CreateRowText(plugin.FileName),
                CreateRowText(plugin.LoadOrderText),
                CreateRowText(plugin.RecordCountText),
                CreateStatusPill(plugin),
                CreateRowText(plugin.LastImportedText))
        };
        ToolTip.SetTip(row, plugin.DiagnosticTooltip);
        row.DoubleTapped += (_, _) =>
        {
            if (plugin.CanOpen)
            {
                ViewModel.SelectedPluginRow = plugin;
                CloseAction(true);
            }
        };
        return row;
    }

    private static Grid CreatePluginRowGrid(Control plugin, Control loadOrder, Control records, Control state, Control lastImported)
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("3*,1.1*,1.1*,1.2*,1.7*"),
            ColumnSpacing = 10,
            Children =
            {
                plugin,
                loadOrder,
                records,
                state,
                lastImported
            }
        };
        Grid.SetColumn(loadOrder, 1);
        Grid.SetColumn(records, 2);
        Grid.SetColumn(state, 3);
        Grid.SetColumn(lastImported, 4);
        return grid;
    }

    private static TextBlock CreateHeaderText(string text)
    {
        return CreateText(text, 13, FontWeight.SemiBold);
    }

    private static TextBlock CreateRowText(string text)
    {
        var textBlock = CreateText(text, 13, FontWeight.Normal);
        textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
        textBlock.VerticalAlignment = VerticalAlignment.Center;
        return textBlock;
    }

    private static Control CreateStatusPill(OpenPluginRowViewModel plugin)
    {
        return new Border
        {
            Background = plugin.StatusBrush,
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(8, 2),
            HorizontalAlignment = HorizontalAlignment.Left,
            Child = new TextBlock
            {
                Text = plugin.ImportStateText,
                FontSize = 12,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
    }

    private Control BuildDetailsPane()
    {
        var detailsGrid = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto,*"),
            RowSpacing = 12,
            Children =
            {
                CreateText("Plugin Details", 16, FontWeight.SemiBold),
                BuildPluginDetails(),
                BuildErrorSummary(),
                BuildErrorDetails()
            }
        };
        Grid.SetRow(detailsGrid.Children[1], 1);
        Grid.SetRow(detailsGrid.Children[2], 2);
        Grid.SetRow(detailsGrid.Children[3], 3);

        var border = new Border
        {
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(12),
            Child = detailsGrid
        };
        Grid.SetColumn(border, 1);
        return border;
    }

    private static Control BuildPluginDetails()
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*"),
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto,Auto,Auto,Auto,Auto"),
            ColumnSpacing = 16,
            RowSpacing = 8
        };

        AddDetailRow(grid, 0, "Plugin", "SelectedPluginRow.FileName");
        AddDetailRow(grid, 1, "Load Order", "SelectedPluginRow.LoadOrderText");
        AddDetailRow(grid, 2, "Records", "SelectedPluginRow.RecordCountText");
        AddDetailRow(grid, 3, "Import State", "SelectedPluginRow.ImportStateText");
        AddDetailRow(grid, 4, "Last Attempt", "SelectedPluginRow.LastCheckedText");
        AddDetailRow(grid, 5, "Last Imported", "SelectedPluginRow.LastImportedText");
        AddDetailRow(grid, 6, "Source Modified", "SelectedPluginRow.SourceModifiedText");
        return grid;
    }

    private static void AddDetailRow(Grid grid, int row, string label, string bindingPath)
    {
        var labelText = CreateText(label, 13, FontWeight.Normal);
        var valueText = CreateText(string.Empty, 13, FontWeight.Normal);
        valueText.TextTrimming = TextTrimming.CharacterEllipsis;
        valueText.Bind(TextBlock.TextProperty, new Binding(bindingPath));
        Grid.SetRow(labelText, row);
        Grid.SetRow(valueText, row);
        Grid.SetColumn(valueText, 1);
        grid.Children.Add(labelText);
        grid.Children.Add(valueText);
    }

    private static Control BuildErrorSummary()
    {
        var summary = CreateText(string.Empty, 13, FontWeight.Normal);
        summary.TextWrapping = TextWrapping.Wrap;
        summary.Bind(TextBlock.TextProperty, new Binding(nameof(OpenPluginDialogViewModel.SelectedPluginDiagnosticSummary)));

        var border = new Border
        {
            BorderBrush = new SolidColorBrush(Color.FromRgb(162, 66, 66)),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(10),
            Child = summary
        };
        border.Bind(IsVisibleProperty, new Binding(nameof(OpenPluginDialogViewModel.HasSelectedDiagnostics)));
        return border;
    }

    private static Control BuildErrorDetails()
    {
        var details = new TextBox
        {
            IsReadOnly = true,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            FontFamily = FontFamily.Parse("Consolas"),
            FontSize = 12,
            MinHeight = 120
        };
        details.Bind(TextBox.TextProperty, new Binding(nameof(OpenPluginDialogViewModel.SelectedPluginDiagnosticDetails)));
        details.Bind(IsVisibleProperty, new Binding(nameof(OpenPluginDialogViewModel.HasSelectedDiagnostics)));
        ScrollViewer.SetVerticalScrollBarVisibility(details, ScrollBarVisibility.Auto);
        return details;
    }

    private Control BuildFooter()
    {
        var summary = CreateText(string.Empty, 13, FontWeight.Normal);
        summary.VerticalAlignment = VerticalAlignment.Center;
        summary.TextTrimming = TextTrimming.CharacterEllipsis;
        summary.Bind(TextBlock.TextProperty, new Binding(nameof(OpenPluginDialogViewModel.PluginSummaryText)));

        var cancelButton = new Button
        {
            Content = "Cancel",
            MinWidth = 110,
            Padding = new Thickness(16, 8)
        };
        cancelButton.Click += (_, _) => CloseAction(false);

        var openButton = new Button
        {
            MinWidth = 110,
            Padding = new Thickness(16, 8)
        };
        PrimaryActionButton = openButton;
        AutomationProperties.SetAutomationId(openButton, "OpenPluginDialogOpenButton");
        openButton.Bind(ContentControl.ContentProperty, new Binding(nameof(OpenPluginDialogViewModel.PrimaryActionText)));
        openButton.Bind(IsEnabledProperty, new Binding(nameof(OpenPluginDialogViewModel.CanRunPrimaryAction)));
        openButton.Click += (_, _) =>
        {
            if (ViewModel.CanRunPrimaryAction)
            {
                CloseAction(true);
            }
        };

        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            HorizontalAlignment = HorizontalAlignment.Right,
            Children =
            {
                cancelButton,
                openButton
            }
        };

        Grid.SetColumn(buttons, 1);
        return new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            Children =
            {
                summary,
                buttons
            }
        };
    }

    private void PulsePrimaryActionButtonWhenImportBecomesAvailable()
    {
        var isImportAction = ViewModel.HasNoPlugins;
        if (isImportAction && !LastPrimaryActionWasImport)
        {
            _ = PulsePrimaryActionButtonAsync();
        }

        LastPrimaryActionWasImport = isImportAction;
    }

    private async Task PulsePrimaryActionButtonAsync()
    {
        if (PrimaryActionButton is null)
        {
            return;
        }

        PrimaryActionPulseCancellationTokenSource?.Cancel();
        PrimaryActionPulseCancellationTokenSource?.Dispose();
        PrimaryActionPulseCancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = PrimaryActionPulseCancellationTokenSource.Token;
        var button = PrimaryActionButton;
        try
        {
            await SetPrimaryActionPulseStateAsync(button, 1.05, Color.FromRgb(64, 122, 205), cancellationToken);
            await Task.Delay(120, cancellationToken);
            await SetPrimaryActionPulseStateAsync(button, 1.0, Color.FromRgb(45, 95, 168), cancellationToken);
            await Task.Delay(80, cancellationToken);
            await SetPrimaryActionPulseStateAsync(button, 1.03, Color.FromRgb(64, 122, 205), cancellationToken);
            await Task.Delay(120, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        finally
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    button.ClearValue(BackgroundProperty);
                    button.ClearValue(BorderBrushProperty);
                    button.ClearValue(RenderTransformProperty);
                });
            }
        }
    }

    private static Task SetPrimaryActionPulseStateAsync(Button button, double scale, Color backgroundColor, CancellationToken cancellationToken)
    {
        return Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            button.RenderTransformOrigin = RelativePoint.Center;
            button.RenderTransform = new ScaleTransform(scale, scale);
            button.Background = new SolidColorBrush(backgroundColor);
            button.BorderBrush = new SolidColorBrush(Color.FromRgb(122, 172, 255));
        }).GetTask();
    }

    private void RefreshGameButtons()
    {
        foreach (var button in GameButtons)
        {
            if (button.Tag is not SupportedGameDTO game)
            {
                continue;
            }

            button.Background = ViewModel.IsSelectedGame(game)
                ? new SolidColorBrush(Color.FromRgb(45, 95, 168))
                : App.GetApplicationBrush(App.PanelSurfaceBrushKey);
        }
    }

    private static TextBlock CreateText(string text, double fontSize, FontWeight fontWeight)
    {
        var textBlock = new TextBlock
        {
            Text = text,
            FontSize = fontSize,
            FontWeight = fontWeight,
            VerticalAlignment = VerticalAlignment.Center
        };
        App.ApplyApplicationTextForeground(textBlock);
        return textBlock;
    }
}
