using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.ViewModels;

namespace CreationsForge.Views;

/// <summary>
/// Hosts application-level workspace actions and the record editor content region.
/// </summary>
public sealed class WorkspaceShellView : UserControl
{
    /// <summary>The workspace shell state and commands.</summary>
    private readonly WorkspaceShellViewModel ViewModel;

    /// <summary>The FormList browser hosted for the current navigation scope.</summary>
    private readonly FormListBrowserView FormListBrowserView;

    /// <summary>The read-only major-record browser hosted for the current navigation scope.</summary>
    private readonly MajorRecordBrowserView MajorRecordBrowserView;

    /// <summary>Initializes the workspace shell view.</summary>
    /// <param name="viewModel">The shell state and commands.</param>
    /// <param name="formListBrowserView">The FormList browser for this navigation scope.</param>
    /// <param name="majorRecordBrowserView">The read-only major-record browser for this navigation scope.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public WorkspaceShellView(
        WorkspaceShellViewModel viewModel,
        FormListBrowserView formListBrowserView,
        MajorRecordBrowserView majorRecordBrowserView)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(formListBrowserView);
        ArgumentNullException.ThrowIfNull(majorRecordBrowserView);
        ViewModel = viewModel;
        FormListBrowserView = formListBrowserView;
        MajorRecordBrowserView = majorRecordBrowserView;
        DataContext = ViewModel;
        AutomationProperties.SetAutomationId(this, "WorkspaceShellView");
        Content = BuildContent();
    }

    /// <summary>Builds the shell toolbar, plugin content region, and active workspace status.</summary>
    /// <returns>The complete shell control tree.</returns>
    private Control BuildContent()
    {
        var toolbar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                CreateCommandButton("Open Plugin...", nameof(WorkspaceShellViewModel.OpenWorkspaceCommand), "OpenWorkspaceShellButton"),
                CreateCommandButton("Close Plugin", nameof(WorkspaceShellViewModel.CloseWorkspaceCommand), "CloseWorkspaceShellButton"),
                CreateCommandButton("Review Changes...", nameof(WorkspaceShellViewModel.ReviewChangesCommand), "WorkspaceReviewChangesButton"),
                CreateCommandButton("Save Changes...", nameof(WorkspaceShellViewModel.SaveChangesCommand), "WorkspaceSaveChangesButton"),
                CreateCommandButton("Discard Changes...", nameof(WorkspaceShellViewModel.DiscardChangesCommand), "WorkspaceDiscardChangesButton"),
                CreateCommandButton("Settings", nameof(WorkspaceShellViewModel.SettingsCommand), "WorkspaceSettingsButton")
            }
        };

        var header = new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(18, 12),
            Child = toolbar
        };

        var recordsTab = new TabItem
        {
            Header = "All Records",
            Content = MajorRecordBrowserView
        };
        AutomationProperties.SetAutomationId(recordsTab, "MajorRecordBrowserTab");
        var formListsTab = new TabItem
        {
            Header = "FormList Authoring",
            Content = FormListBrowserView
        };
        AutomationProperties.SetAutomationId(formListsTab, "FormListBrowserTab");
        var browsers = new TabControl
        {
            ItemsSource = new[] { recordsTab, formListsTab },
            SelectedIndex = 0
        };
        AutomationProperties.SetAutomationId(browsers, "WorkspaceBrowserTabs");
        var contentHost = new Border
        {
            Background = App.GetApplicationBrush(App.ApplicationSurfaceBrushKey),
            Padding = new Thickness(24),
            Child = browsers
        };
        AutomationProperties.SetAutomationId(contentHost, "WorkspaceContentHost");

        var status = new TextBlock
        {
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap
        };
        App.ApplyApplicationTextForeground(status);
        status.Bind(TextBlock.TextProperty, new Binding(nameof(WorkspaceShellViewModel.WorkspaceStatusText)));
        AutomationProperties.SetAutomationId(status, "WorkspaceStatusText");

        var activePluginCount = CreateBrowserStatusText(
            nameof(FormListBrowserViewModel.ActivePluginRecordCountText),
            "ActivePluginRecordCountText");
        var loadedCount = CreateBrowserStatusText(
            nameof(FormListBrowserViewModel.LoadedRecordCountText),
            "LoadedRecordCountText");
        var statusFields = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 18,
            VerticalAlignment = VerticalAlignment.Center,
            Children =
            {
                status,
                activePluginCount,
                loadedCount
            }
        };
        AutomationProperties.SetAutomationId(statusFields, "WorkspaceStatusFields");

        var legend = BuildComparisonLegend();
        legend.HorizontalAlignment = HorizontalAlignment.Right;
        Grid.SetColumn(legend, 1);

        var statusBar = new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 1, 0, 0),
            Padding = new Thickness(18, 8),
            Child = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("*,Auto"),
                ColumnSpacing = 18,
                Children =
                {
                    statusFields,
                    legend
                }
            }
        };

        Grid.SetRow(header, 0);
        Grid.SetRow(contentHost, 1);
        Grid.SetRow(statusBar, 2);
        return new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*,Auto"),
            Children =
            {
                header,
                contentHost,
                statusBar
            }
        };
    }

    /// <summary>Creates one footer count bound directly to the hosted browser's unfiltered snapshot.</summary>
    /// <param name="propertyName">The formatted browser property to display.</param>
    /// <param name="automationId">The stable automation identity.</param>
    /// <returns>The configured footer text.</returns>
    private TextBlock CreateBrowserStatusText(string propertyName, string automationId)
    {
        var text = new TextBlock
        {
            FontSize = 13,
            VerticalAlignment = VerticalAlignment.Center
        };
        App.ApplyApplicationTextForeground(text);
        text.Bind(TextBlock.TextProperty, new Binding(propertyName)
        {
            Source = FormListBrowserView.BrowserViewModel
        });
        AutomationProperties.SetAutomationId(text, automationId);
        return text;
    }

    /// <summary>Builds the persistent comparison-color legend restored from the established status bar.</summary>
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
        AutomationProperties.SetAutomationId(legend, "WorkspaceComparisonLegend");
        return legend;
    }

    /// <summary>Creates one text-labelled comparison-state swatch for the persistent legend.</summary>
    /// <param name="label">The visible state label.</param>
    /// <param name="state">The field state represented by the swatch.</param>
    /// <returns>The configured legend item.</returns>
    private static Control CreateLegendItem(string label, ComparisonFieldState state)
    {
        var labelText = new TextBlock
        {
            Text = label,
            FontSize = 12,
            VerticalAlignment = VerticalAlignment.Center
        };
        App.ApplyApplicationTextForeground(labelText);
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

    /// <summary>Creates a toolbar button bound to a shell command.</summary>
    /// <param name="text">The visible button label.</param>
    /// <param name="commandProperty">The view-model command property.</param>
    /// <param name="automationId">The stable automation identity.</param>
    /// <returns>The configured command button.</returns>
    private static Button CreateCommandButton(string text, string commandProperty, string automationId)
    {
        var button = new Button
        {
            Content = text,
            Padding = new Thickness(14, 8)
        };
        button.Bind(Button.CommandProperty, new Binding(commandProperty));
        AutomationProperties.SetAutomationId(button, automationId);
        return button;
    }
}
