using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.ViewModels;

namespace CreationsForge.Views;

/// <summary>
/// Presents explicit native source, load-order, resource, and output selection without reading records itself.
/// </summary>
public sealed class NativeWorkspaceSelectionView : UserControl
{
    /// <summary>The selection and activation workflow displayed by this view.</summary>
    private readonly NativeWorkspaceSelectionViewModel ViewModel;

    /// <summary>Closes the owning dialog with the final activation result.</summary>
    private readonly Action<bool> CloseAction;

    /// <summary>The load-order list whose selection is used by order controls.</summary>
    private ListBox? LoadOrderList;

    /// <summary>The string-directory list whose selection is used by removal controls.</summary>
    private ListBox? StringDirectoryList;

    /// <summary>Initializes the native workspace selection view.</summary>
    /// <param name="viewModel">The explicit path selection and activation workflow.</param>
    /// <param name="closeAction">The callback used to close the owning dialog.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public NativeWorkspaceSelectionView(NativeWorkspaceSelectionViewModel viewModel, Action<bool> closeAction)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(closeAction);
        ViewModel = viewModel;
        CloseAction = closeAction;
        DataContext = ViewModel;
        AutomationProperties.SetAutomationId(this, "NativeWorkspaceSelectionView");
        Content = BuildContent();
    }

    /// <summary>Builds the scrollable selection form and footer actions.</summary>
    /// <returns>The complete native workspace selection control tree.</returns>
    private Control BuildContent()
    {
        var form = new StackPanel
        {
            Spacing = 14,
            Children =
            {
                CreateHeading("Native Workspace", 22),
                CreateDescription("Choose explicit read-only source inputs and a separate output. Paths remain in memory for this workspace only."),
                BuildGameSelection(),
                BuildPathSelection("Source plugin", nameof(NativeWorkspaceSelectionViewModel.SourcePluginPath), "Browse...", ViewModel.BrowseSourcePluginAsync, "SourcePluginPath"),
                BuildLoadOrderSelection(),
                BuildPathSelection("Game data directory", nameof(NativeWorkspaceSelectionViewModel.DataDirectoryPath), "Browse...", ViewModel.BrowseDataDirectoryAsync, "DataDirectoryPath"),
                BuildStringDirectorySelection(),
                BuildOutputSelection(),
                BuildStatusPanel()
            }
        };

        var scrollViewer = new ScrollViewer
        {
            Content = form,
            Padding = new Thickness(24),
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
        Grid.SetRow(scrollViewer, 0);
        var footer = BuildFooter();
        Grid.SetRow(footer, 1);
        return new Grid
        {
            RowDefinitions = new RowDefinitions("*,Auto"),
            Children =
            {
                scrollViewer,
                footer
            }
        };
    }

    /// <summary>Builds the game and exact native-release selector.</summary>
    /// <returns>The labeled game selection control.</returns>
    private Control BuildGameSelection()
    {
        var selector = new ComboBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        AutomationProperties.SetAutomationId(selector, "NativeWorkspaceGameSelector");
        selector.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.Games)));
        selector.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.SelectedGame))
        {
            Mode = BindingMode.TwoWay
        });
        selector.Bind(IsEnabledProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.CanOpen)));
        return CreateLabeledControl("Game", selector);
    }

    /// <summary>Builds the explicit ordered load-order list and its reorder controls.</summary>
    /// <returns>The load-order selection controls.</returns>
    private Control BuildLoadOrderSelection()
    {
        LoadOrderList = new ListBox
        {
            MinHeight = 120,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        AutomationProperties.SetAutomationId(LoadOrderList, "NativeWorkspaceLoadOrderList");
        LoadOrderList.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.LoadOrderPluginPaths)));

        var controls = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                CreateActionButton("Select load order...", "BrowseLoadOrderButton", ViewModel.BrowseLoadOrderAsync),
                CreateActionButton("Move up", "MoveLoadOrderUpButton", () =>
                {
                    ViewModel.MoveLoadOrderPluginUp(LoadOrderList.SelectedIndex);
                    return Task.CompletedTask;
                }),
                CreateActionButton("Move down", "MoveLoadOrderDownButton", () =>
                {
                    ViewModel.MoveLoadOrderPluginDown(LoadOrderList.SelectedIndex);
                    return Task.CompletedTask;
                }),
                CreateActionButton("Remove", "RemoveLoadOrderButton", () =>
                {
                    ViewModel.RemoveLoadOrderPlugin(LoadOrderList.SelectedIndex);
                    return Task.CompletedTask;
                })
            }
        };

        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                CreateLabel("Explicit load order (masters first; source included)"),
                LoadOrderList,
                controls
            }
        };
    }

    /// <summary>Builds the optional explicit localized-string directory list.</summary>
    /// <returns>The string-directory selection controls.</returns>
    private Control BuildStringDirectorySelection()
    {
        StringDirectoryList = new ListBox
        {
            MinHeight = 72,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        AutomationProperties.SetAutomationId(StringDirectoryList, "NativeWorkspaceStringDirectoryList");
        StringDirectoryList.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.StringDirectoryPaths)));
        var controls = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                CreateActionButton("Select directories...", "BrowseStringDirectoriesButton", ViewModel.BrowseStringDirectoriesAsync),
                CreateActionButton("Remove", "RemoveStringDirectoryButton", () =>
                {
                    ViewModel.RemoveStringDirectory(StringDirectoryList.SelectedIndex);
                    return Task.CompletedTask;
                })
            }
        };
        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                CreateLabel("Localized-string directories (optional)"),
                StringDirectoryList,
                controls
            }
        };
    }

    /// <summary>Builds output acquisition and native format selectors.</summary>
    /// <returns>The complete output selection controls.</returns>
    private Control BuildOutputSelection()
    {
        var outputMode = CreateBoundSelector(
            nameof(NativeWorkspaceSelectionViewModel.OutputModeOptions),
            nameof(NativeWorkspaceSelectionViewModel.OutputMode),
            "OutputModeSelector");
        var localizationMode = CreateBoundSelector(
            nameof(NativeWorkspaceSelectionViewModel.LocalizedOutputModeOptions),
            nameof(NativeWorkspaceSelectionViewModel.LocalizedOutputMode),
            "LocalizedOutputModeSelector");
        var masterStyle = CreateBoundSelector(
            nameof(NativeWorkspaceSelectionViewModel.OutputMasterStyleOptions),
            nameof(NativeWorkspaceSelectionViewModel.OutputMasterStyle),
            "OutputMasterStyleSelector");

        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                CreateHeading("Output", 17),
                CreateLabeledControl("Open mode", outputMode),
                BuildPathSelection("Output plugin", nameof(NativeWorkspaceSelectionViewModel.OutputPluginPath), "Browse...", ViewModel.BrowseOutputPluginAsync, "OutputPluginPath"),
                CreateLabeledControl("Localized strings", localizationMode),
                CreateLabeledControl("Master style", masterStyle)
            }
        };
    }

    /// <summary>Builds bound status and error feedback.</summary>
    /// <returns>The status feedback panel.</returns>
    private Control BuildStatusPanel()
    {
        var progress = new ProgressBar
        {
            IsIndeterminate = true,
            MinHeight = 4
        };
        progress.Bind(IsVisibleProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.IsBusy)));
        var status = CreateDescription(string.Empty);
        status.Bind(TextBlock.TextProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.StatusText)));
        var error = CreateDescription(string.Empty);
        error.Foreground = new SolidColorBrush(Color.FromRgb(204, 73, 73));
        error.Bind(TextBlock.TextProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.ErrorText)));
        error.Bind(IsVisibleProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.HasError)));
        AutomationProperties.SetAutomationId(error, "NativeWorkspaceErrorText");
        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                progress,
                status,
                error
            }
        };
    }

    /// <summary>Builds dialog cancellation, close-workspace, and activation actions.</summary>
    /// <returns>The footer action bar.</returns>
    private Control BuildFooter()
    {
        var closeWorkspace = CreateActionButton("Close workspace", "CloseNativeWorkspaceButton", ViewModel.CloseWorkspaceAsync);
        var cancel = new Button
        {
            Content = "Cancel",
            MinWidth = 100,
            Padding = new Thickness(16, 8)
        };
        AutomationProperties.SetAutomationId(cancel, "CancelNativeWorkspaceButton");
        cancel.Click += async (_, _) =>
        {
            if (ViewModel.IsBusy)
            {
                var activated = await ViewModel.CancelAndWaitForOpenAsync();
                CloseAction(activated);
                return;
            }

            CloseAction(false);
        };

        var open = new Button
        {
            Content = "Open workspace",
            MinWidth = 140,
            Padding = new Thickness(16, 8)
        };
        AutomationProperties.SetAutomationId(open, "OpenNativeWorkspaceButton");
        open.Bind(IsEnabledProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.CanOpen)));
        open.Click += async (_, _) =>
        {
            if (await ViewModel.OpenWorkspaceAsync())
            {
                CloseAction(true);
            }
        };

        return new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 1, 0, 0),
            Padding = new Thickness(18, 12),
            Child = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Spacing = 8,
                Children =
                {
                    closeWorkspace,
                    cancel,
                    open
                }
            }
        };
    }

    /// <summary>Builds one read-only bound path field with a picker action.</summary>
    /// <param name="label">The field label.</param>
    /// <param name="pathProperty">The bound view-model path property.</param>
    /// <param name="buttonText">The picker button text.</param>
    /// <param name="browseAction">The picker workflow.</param>
    /// <param name="automationId">The path field automation identity.</param>
    /// <returns>The labeled path picker control.</returns>
    private Control BuildPathSelection(
        string label,
        string pathProperty,
        string buttonText,
        Func<Task> browseAction,
        string automationId)
    {
        var path = new TextBox
        {
            IsReadOnly = true,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        AutomationProperties.SetAutomationId(path, automationId);
        path.Bind(TextBox.TextProperty, new Binding(pathProperty));
        Grid.SetColumn(path, 0);
        var browse = CreateActionButton(buttonText, $"Browse{automationId}Button", browseAction);
        Grid.SetColumn(browse, 1);
        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                CreateLabel(label),
                new Grid
                {
                    ColumnDefinitions = new ColumnDefinitions("*,Auto"),
                    ColumnSpacing = 8,
                    Children =
                    {
                        path,
                        browse
                    }
                }
            }
        };
    }

    /// <summary>Builds a selector bound to view-model item and selected-value properties.</summary>
    /// <param name="itemsProperty">The bound item-source property.</param>
    /// <param name="selectedProperty">The two-way selected-item property.</param>
    /// <param name="automationId">The selector automation identity.</param>
    /// <returns>The configured selector.</returns>
    private ComboBox CreateBoundSelector(string itemsProperty, string selectedProperty, string automationId)
    {
        var selector = new ComboBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        AutomationProperties.SetAutomationId(selector, automationId);
        selector.Bind(ItemsControl.ItemsSourceProperty, new Binding(itemsProperty));
        selector.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(selectedProperty)
        {
            Mode = BindingMode.TwoWay
        });
        selector.Bind(IsEnabledProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.CanOpen)));
        return selector;
    }

    /// <summary>Builds a button whose asynchronous failures remain contained by the view model.</summary>
    /// <param name="text">The button label.</param>
    /// <param name="automationId">The automation identity.</param>
    /// <param name="action">The asynchronous action to invoke.</param>
    /// <returns>The configured action button.</returns>
    private Button CreateActionButton(string text, string automationId, Func<Task> action)
    {
        var button = new Button
        {
            Content = text,
            Padding = new Thickness(12, 6)
        };
        AutomationProperties.SetAutomationId(button, automationId);
        button.Bind(IsEnabledProperty, new Binding(nameof(NativeWorkspaceSelectionViewModel.CanOpen)));
        button.Click += async (_, _) => await action();
        return button;
    }

    /// <summary>Builds a label stacked above its related input control.</summary>
    /// <param name="label">The field label.</param>
    /// <param name="control">The related input control.</param>
    /// <returns>The labeled control.</returns>
    private static Control CreateLabeledControl(string label, Control control)
    {
        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                CreateLabel(label),
                control
            }
        };
    }

    /// <summary>Creates a section heading using the application's foreground brush.</summary>
    /// <param name="text">The heading text.</param>
    /// <param name="fontSize">The heading font size.</param>
    /// <returns>The styled heading.</returns>
    private static TextBlock CreateHeading(string text, double fontSize)
    {
        var heading = new TextBlock
        {
            Text = text,
            FontSize = fontSize,
            FontWeight = FontWeight.SemiBold
        };
        App.ApplyApplicationTextForeground(heading);
        return heading;
    }

    /// <summary>Creates a field label using the application's foreground brush.</summary>
    /// <param name="text">The label text.</param>
    /// <returns>The styled label.</returns>
    private static TextBlock CreateLabel(string text)
    {
        return CreateHeading(text, 13);
    }

    /// <summary>Creates wrapping descriptive text using the application's foreground brush.</summary>
    /// <param name="text">The initial text.</param>
    /// <returns>The styled description.</returns>
    private static TextBlock CreateDescription(string text)
    {
        var description = new TextBlock
        {
            Text = text,
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap
        };
        App.ApplyApplicationTextForeground(description);
        return description;
    }
}
