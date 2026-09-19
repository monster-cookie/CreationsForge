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

/// <summary>Collects the game, file type, and master size before creating a plugin workspace.</summary>
public sealed class NewPluginView : UserControl
{
    /// <summary>The workspace selection workflow that owns discovery and plugin creation.</summary>
    private readonly WorkspaceSelectionViewModel ViewModel;

    /// <summary>Completes the owning creation dialog.</summary>
    private readonly Action<bool> CloseAction;

    /// <summary>Initializes the creation choices and action controls.</summary>
    /// <param name="viewModel">The workspace selection workflow.</param>
    /// <param name="closeAction">The callback for creation or cancellation.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public NewPluginView(WorkspaceSelectionViewModel viewModel, Action<bool> closeAction)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(closeAction);
        ViewModel = viewModel;
        CloseAction = closeAction;
        DataContext = viewModel;
        AutomationProperties.SetAutomationId(this, "NewPluginView");
        Content = BuildContent();
    }

    /// <summary>Builds creation-specific choices, destination context, and actions.</summary>
    /// <returns>The complete creation dialog content.</returns>
    private Control BuildContent()
    {
        var game = new ComboBox { MinWidth = 220 };
        game.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(WorkspaceSelectionViewModel.Games)));
        game.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(WorkspaceSelectionViewModel.SelectedGame))
        {
            Mode = BindingMode.TwoWay
        });
        game.Bind(IsEnabledProperty, new Binding(nameof(WorkspaceSelectionViewModel.CanOpen)));
        game.SelectionChanged += async (_, _) => await ViewModel.RefreshPluginsAsync();
        AutomationProperties.SetAutomationId(game, "NewPluginGameSelector");

        var extension = new ComboBox { MinWidth = 110 };
        extension.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(WorkspaceSelectionViewModel.NewPluginExtensionOptions)));
        extension.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(WorkspaceSelectionViewModel.NewPluginExtension))
        {
            Mode = BindingMode.TwoWay
        });
        extension.Bind(IsEnabledProperty, new Binding(nameof(WorkspaceSelectionViewModel.CanOpen)));
        AutomationProperties.SetAutomationId(extension, "NewPluginExtensionSelector");

        var masterSize = new ComboBox { MinWidth = 110 };
        masterSize.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(WorkspaceSelectionViewModel.NewPluginMasterStyleOptions)));
        masterSize.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(WorkspaceSelectionViewModel.OutputMasterStyle))
        {
            Mode = BindingMode.TwoWay
        });
        masterSize.Bind(IsEnabledProperty, new Binding(nameof(WorkspaceSelectionViewModel.CanOpen)));
        masterSize.Bind(IsVisibleProperty, new Binding(nameof(WorkspaceSelectionViewModel.NewPluginSupportsMasterSize)));
        AutomationProperties.SetAutomationId(masterSize, "NewPluginMasterStyleSelector");
        var masterSizeLabel = Label("Master size");
        masterSizeLabel.Bind(IsVisibleProperty, new Binding(nameof(WorkspaceSelectionViewModel.NewPluginSupportsMasterSize)));

        var textStorage = new ComboBox
        {
            MinWidth = 220,
            ItemTemplate = new FuncDataTemplate<LocalizedOutputMode>((mode, _) =>
                new TextBlock
                {
                    Text = mode == LocalizedOutputMode.Embedded ? "Embedded in plugin" : "Localized string files"
                })
        };
        textStorage.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(WorkspaceSelectionViewModel.LocalizedOutputModeOptions)));
        textStorage.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(WorkspaceSelectionViewModel.NewPluginLocalizedOutputMode))
        {
            Mode = BindingMode.TwoWay
        });
        textStorage.Bind(IsEnabledProperty, new Binding(nameof(WorkspaceSelectionViewModel.CanOpen)));
        AutomationProperties.SetAutomationId(textStorage, "NewPluginTextStorageSelector");

        var fileName = new TextBox { PlaceholderText = "MyPlugin", MinHeight = 34 };
        fileName.Bind(TextBox.TextProperty, new Binding(nameof(WorkspaceSelectionViewModel.NewPluginFileName))
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
        fileName.Bind(IsEnabledProperty, new Binding(nameof(WorkspaceSelectionViewModel.CanOpen)));
        AutomationProperties.SetAutomationId(fileName, "NewPluginNameBox");
        var extensionSuffix = new TextBlock { VerticalAlignment = VerticalAlignment.Center, FontWeight = FontWeight.SemiBold };
        extensionSuffix.Bind(TextBlock.TextProperty, new Binding(nameof(WorkspaceSelectionViewModel.NewPluginExtension)));
        App.ApplyApplicationTextForeground(extensionSuffix);
        AutomationProperties.SetAutomationId(extensionSuffix, "NewPluginExtensionSuffix");
        Grid.SetColumn(extensionSuffix, 1);
        var fileNameField = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            ColumnSpacing = 8,
            Children = { fileName, extensionSuffix }
        };

        var dataDirectory = new TextBlock { TextWrapping = TextWrapping.Wrap };
        dataDirectory.Bind(TextBlock.TextProperty, new Binding(nameof(WorkspaceSelectionViewModel.DetectedDataDirectoryText)));
        App.ApplyApplicationTextForeground(dataDirectory);
        AutomationProperties.SetAutomationId(dataDirectory, "NewPluginDataDirectory");

        var hint = new TextBlock
        {
            Text = "Only the selected game's base plugin is added as an initial master. Choose localized string files to save names in more than one language. Localized output currently supports FormList records only.",
            TextWrapping = TextWrapping.Wrap
        };
        App.ApplyApplicationTextForeground(hint);

        var progress = new ProgressBar { IsIndeterminate = true, Height = 3 };
        progress.Bind(IsVisibleProperty, new Binding(nameof(WorkspaceSelectionViewModel.IsBusy)));
        var status = new TextBlock { TextWrapping = TextWrapping.Wrap };
        status.Bind(TextBlock.TextProperty, new Binding(nameof(WorkspaceSelectionViewModel.StatusText)));
        App.ApplyApplicationTextForeground(status);
        var error = new TextBlock { Foreground = Brushes.IndianRed, TextWrapping = TextWrapping.Wrap };
        error.Bind(TextBlock.TextProperty, new Binding(nameof(WorkspaceSelectionViewModel.ErrorText)));
        error.Bind(IsVisibleProperty, new Binding(nameof(WorkspaceSelectionViewModel.HasError)));

        var cancel = new Button { Content = "Cancel", MinWidth = 100 };
        cancel.Click += async (_, _) => CloseAction(ViewModel.IsBusy
            ? await ViewModel.CancelAndWaitForOpenAsync()
            : false);
        AutomationProperties.SetAutomationId(cancel, "CancelNewPluginButton");
        var create = new Button { Content = "Create Plugin...", MinWidth = 140, IsDefault = true };
        create.Bind(IsEnabledProperty, new Binding(nameof(WorkspaceSelectionViewModel.CanOpen)));
        create.Click += async (_, _) =>
        {
            if (await ViewModel.CreateNewPluginAsync())
            {
                CloseAction(true);
            }
        };
        AutomationProperties.SetAutomationId(create, "ConfirmNewPluginButton");

        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Children = { cancel, create }
        };
        return new ScrollViewer
        {
            Content = new StackPanel
            {
                Margin = new Thickness(24),
                Spacing = 10,
                Children =
                {
                    Label("New Plugin", 22),
                    Label("Game"), game,
                    Label("Plugin file type"), extension,
                    masterSizeLabel, masterSize,
                    Label("Text storage"), textStorage,
                    Label("Plugin name (without extension)"), fileNameField,
                    Label("Game Data directory"), dataDirectory,
                    hint, progress, status, error, actions
                }
            }
        };
    }

    /// <summary>Creates a readable heading for one creation choice.</summary>
    /// <param name="text">The visible label.</param>
    /// <param name="fontSize">The label size.</param>
    /// <returns>A styled text label.</returns>
    private static TextBlock Label(string text, double fontSize = 12)
    {
        var label = new TextBlock { Text = text, FontSize = fontSize, FontWeight = FontWeight.SemiBold };
        App.ApplyApplicationTextForeground(label);
        return label;
    }
}
