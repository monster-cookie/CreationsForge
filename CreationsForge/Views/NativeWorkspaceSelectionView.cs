using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.ViewModels;

namespace CreationsForge.Views;

/// <summary>Presents installed plugins as the entry point into native record editing.</summary>
public sealed class NativeWorkspaceSelectionView : UserControl
{
    /// <summary>The installed-plugin selection state.</summary>
    private readonly NativeWorkspaceSelectionViewModel ViewModel;

    /// <summary>Closes the owning dialog with its activation result.</summary>
    private readonly Action<bool> CloseAction;

    /// <summary>Initializes the installed-plugin selection surface.</summary>
    /// <param name="viewModel">The discovery and native workspace activation state.</param>
    /// <param name="closeAction">The callback used after cancellation or successful activation.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    public NativeWorkspaceSelectionView(NativeWorkspaceSelectionViewModel viewModel, Action<bool> closeAction)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(closeAction);
        ViewModel = viewModel;
        CloseAction = closeAction;
        DataContext = ViewModel;
        AutomationProperties.SetAutomationId(this, "NativeWorkspaceSelectionView");
        Content = BuildContent();
        Loaded += async (_, _) => await ViewModel.RefreshPluginsAsync();
    }

    /// <summary>Builds the complete plugin selection layout.</summary>
    /// <returns>The configured control tree.</returns>
    private Control BuildContent()
    {
        var heading = CreateCell("Open Plugin", FontWeight.SemiBold, 22);
        var guidance = CreateCell(
            "Choose the plugin you want to edit. CreationsForge opens its declared masters read-only and stages changes safely until you save.");
        guidance.TextWrapping = TextWrapping.Wrap;

        var selectors = BuildSelectors();
        var body = BuildBody();
        var status = BuildStatus();
        var footer = BuildFooter();

        Grid.SetRow(guidance, 1);
        Grid.SetRow(selectors, 2);
        Grid.SetRow(body, 3);
        Grid.SetRow(status, 4);
        Grid.SetRow(footer, 5);
        return new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto,*,Auto,Auto"),
            RowSpacing = 12,
            Margin = new Thickness(24),
            Children = { heading, guidance, selectors, body, status, footer }
        };
    }

    /// <summary>Builds the plugin list and selected-plugin details as a stable two-pane layout.</summary>
    /// <returns>The complete selector body.</returns>
    private Control BuildBody()
    {
        var pluginList = BuildPluginList();
        var details = BuildPluginDetails();
        Grid.SetColumn(details, 1);
        return new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("7*,3*"),
            ColumnSpacing = 14,
            Children = { pluginList, details }
        };
    }

    /// <summary>Builds the game, search, and refresh controls.</summary>
    /// <returns>The selector toolbar.</returns>
    private Control BuildSelectors()
    {
        var game = new ComboBox { MinWidth = 210 };
        game.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.Games)));
        game.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.SelectedGame))
        {
            Mode = BindingMode.TwoWay
        });
        game.Bind(IsEnabledProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.CanOpen)));
        AutomationProperties.SetAutomationId(game, "NativePluginGameSelector");
        game.SelectionChanged += async (_, _) => await ViewModel.RefreshPluginsAsync();

        var search = new TextBox { PlaceholderText = "Search plugins...", MinHeight = 34 };
        search.Bind(TextBox.TextProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.PluginSearchText))
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
        AutomationProperties.SetAutomationId(search, "NativePluginSearchBox");

        var refresh = new Button { Content = "Refresh", Padding = new Thickness(14, 7) };
        refresh.Click += async (_, _) => await ViewModel.RefreshPluginsAsync();
        refresh.Bind(IsEnabledProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.CanOpen)));
        AutomationProperties.SetAutomationId(refresh, "RefreshNativePluginsButton");

        Grid.SetColumn(search, 1);
        Grid.SetColumn(refresh, 2);
        return new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"),
            ColumnSpacing = 10,
            Children = { game, search, refresh }
        };
    }

    /// <summary>Builds the installed plugin table.</summary>
    /// <returns>The bordered table.</returns>
    private Control BuildPluginList()
    {
        var header = CreatePluginRow(
            CreateCell("Plugin", FontWeight.SemiBold),
            CreateCell("Load Order", FontWeight.SemiBold),
            CreateCell("Type", FontWeight.SemiBold),
            CreateCell("State", FontWeight.SemiBold),
            CreateCell("Availability", FontWeight.SemiBold));
        var headerBorder = new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(10, 7),
            Child = header
        };

        var list = new ListBox { MinHeight = 260 };
        list.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.PluginRows)));
        list.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.SelectedPlugin))
        {
            Mode = BindingMode.TwoWay
        });
        list.ItemTemplate = new FuncDataTemplate<NativePluginSelectionRowViewModel>(
            (row, _) => row is null ? new TextBlock() : BuildPluginRow(row));
        AutomationProperties.SetAutomationId(list, "NativePluginList");

        Grid.SetRow(list, 1);
        return new Border
        {
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(1),
            Child = new Grid
            {
                RowDefinitions = new RowDefinitions("Auto,*"),
                Children = { headerBorder, list }
            }
        };
    }

    /// <summary>Builds one installed-plugin row.</summary>
    /// <param name="row">The detached plugin row.</param>
    /// <returns>The complete table row.</returns>
    private static Control BuildPluginRow(NativePluginSelectionRowViewModel row)
    {
        var border = new Border
        {
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(10, 7),
            Child = CreatePluginRow(
                CreateCell(row.FileName),
                CreateCell(row.LoadOrderText),
                CreateCell(row.PluginTypeText),
                CreateCell(row.EnabledText),
                CreateCell(row.AvailabilityText))
        };
        ToolTip.SetTip(border, row.DetailsText);
        return border;
    }

    /// <summary>Creates the shared five-column plugin row.</summary>
    /// <param name="plugin">The plugin filename cell.</param>
    /// <param name="loadOrder">The load-order cell.</param>
    /// <param name="type">The native type cell.</param>
    /// <param name="state">The enabled-state cell.</param>
    /// <param name="access">The editability cell.</param>
    /// <returns>The aligned row grid.</returns>
    private static Grid CreatePluginRow(Control plugin, Control loadOrder, Control type, Control state, Control access)
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("3*,1.1*,1.1*,1.1*,1.2*"),
            ColumnSpacing = 12,
            Children = { plugin, loadOrder, type, state, access }
        };
        Grid.SetColumn(loadOrder, 1);
        Grid.SetColumn(type, 2);
        Grid.SetColumn(state, 3);
        Grid.SetColumn(access, 4);
        return grid;
    }

    /// <summary>Creates one text cell.</summary>
    /// <param name="text">The visible text.</param>
    /// <param name="weight">The requested font weight.</param>
    /// <param name="fontSize">The requested font size.</param>
    /// <returns>The configured text cell.</returns>
    private static TextBlock CreateCell(string text, FontWeight? weight = null, double fontSize = 13)
    {
        var cell = new TextBlock
        {
            Text = text,
            FontSize = fontSize,
            FontWeight = weight ?? FontWeight.Normal,
            TextTrimming = TextTrimming.CharacterEllipsis
        };
        App.ApplyApplicationTextForeground(cell);
        return cell;
    }

    /// <summary>Builds the selected plugin's native header and availability details.</summary>
    /// <returns>The bordered details pane.</returns>
    private Control BuildPluginDetails()
    {
        var contents = new StackPanel
        {
            Spacing = 12,
            Children =
            {
                CreateCell("Plugin Details", FontWeight.SemiBold, 16),
                CreateBoundDetail("Plugin", "SelectedPlugin.FileName", "SelectedPluginFileName"),
                CreateBoundDetail("Type", "SelectedPlugin.PluginTypeText", "SelectedPluginType"),
                CreateBoundDetail("Parent Masters", "SelectedPlugin.ParentMastersText", "SelectedPluginParentMasters", true),
                CreateBoundDetail("Load Order Position", "SelectedPlugin.LoadOrderText", "SelectedPluginLoadOrder"),
                CreateBoundDetail("State", "SelectedPlugin.EnabledText", "SelectedPluginState"),
                CreateBoundDetail("Availability", "SelectedPlugin.AvailabilityText", "SelectedPluginAvailability"),
                CreateBoundDetail("Localized Strings", "SelectedPlugin.LocalizationText", "SelectedPluginLocalization"),
                CreateBoundDetail("Author", "SelectedPlugin.AuthorText", "SelectedPluginAuthor", true),
                CreateBoundDetail("Description", "SelectedPlugin.DescriptionText", "SelectedPluginDescription", true),
                CreateBoundDetail("Path", "SelectedPlugin.PluginPathText", "SelectedPluginPath", true),
                CreateBoundDetail("Editing Context", nameof(NativeWorkspaceSelectionViewModel.SelectedPluginDetails), "SelectedPluginEditingContext", true),
                CreateBoundDetail("Data Directory", nameof(NativeWorkspaceSelectionViewModel.DetectedDataDirectoryText), "DetectedPluginDataDirectory", true)
            }
        };
        var scroll = new ScrollViewer { Content = contents };
        ScrollViewer.SetVerticalScrollBarVisibility(scroll, ScrollBarVisibility.Auto);
        var border = new Border
        {
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(14),
            Child = scroll
        };
        AutomationProperties.SetAutomationId(border, "NativePluginDetailsPanel");
        return border;
    }

    /// <summary>Creates one labeled, bound plugin detail.</summary>
    /// <param name="label">The visible detail label.</param>
    /// <param name="bindingPath">The view-model binding path.</param>
    /// <param name="automationId">The value control's automation identity.</param>
    /// <param name="wrap">Whether the value may wrap across lines.</param>
    /// <returns>The labeled detail control.</returns>
    private static Control CreateBoundDetail(string label, string bindingPath, string automationId, bool wrap = false)
    {
        var labelText = CreateCell(label, FontWeight.SemiBold, 12);
        var valueText = CreateCell(string.Empty);
        valueText.TextWrapping = wrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
        valueText.Bind(TextBlock.TextProperty, new Binding(bindingPath));
        AutomationProperties.SetAutomationId(valueText, automationId);
        return new StackPanel { Spacing = 3, Children = { labelText, valueText } };
    }

    /// <summary>Builds progress, status, and error feedback.</summary>
    /// <returns>The feedback surface.</returns>
    private Control BuildStatus()
    {
        var progress = new ProgressBar { IsIndeterminate = true, Height = 3 };
        progress.Bind(IsVisibleProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.IsBusy)));
        var status = CreateCell(string.Empty);
        status.TextWrapping = TextWrapping.Wrap;
        status.Bind(TextBlock.TextProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.StatusText)));
        var error = new TextBlock { Foreground = Brushes.IndianRed, TextWrapping = TextWrapping.Wrap };
        error.Bind(TextBlock.TextProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.ErrorText)));
        error.Bind(IsVisibleProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.HasError)));
        return new StackPanel { Spacing = 6, Children = { progress, status, error } };
    }

    /// <summary>Builds cancel, create, and open actions.</summary>
    /// <returns>The right-aligned footer.</returns>
    private Control BuildFooter()
    {
        var cancel = new Button { Content = "Cancel", MinWidth = 110, Padding = new Thickness(16, 8) };
        AutomationProperties.SetAutomationId(cancel, "CancelNativePluginButton");
        cancel.Click += async (_, _) =>
        {
            if (ViewModel.IsBusy)
            {
                CloseAction(await ViewModel.CancelAndWaitForOpenAsync());
                return;
            }

            CloseAction(false);
        };

        var create = new Button { Content = "New Plugin...", MinWidth = 130, Padding = new Thickness(16, 8) };
        create.Bind(IsEnabledProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.CanOpen)));
        create.Click += async (_, _) =>
        {
            if (await ViewModel.CreateNewPluginAsync())
            {
                CloseAction(true);
            }
        };
        AutomationProperties.SetAutomationId(create, "CreateNativePluginButton");

        var open = new Button { Content = "Open for Editing", MinWidth = 150, Padding = new Thickness(16, 8) };
        open.Bind(IsEnabledProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.CanOpenSelectedPlugin)));
        open.Click += async (_, _) =>
        {
            if (await ViewModel.OpenSelectedPluginAsync())
            {
                CloseAction(true);
            }
        };
        AutomationProperties.SetAutomationId(open, "OpenNativePluginButton");

        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            HorizontalAlignment = HorizontalAlignment.Right,
            Children = { cancel, create, open }
        };
    }
}
