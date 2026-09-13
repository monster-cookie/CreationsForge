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

    /// <summary>Initializes the workspace shell view.</summary>
    /// <param name="viewModel">The shell state and commands.</param>
    /// <param name="formListBrowserView">The FormList browser for this navigation scope.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public WorkspaceShellView(
        WorkspaceShellViewModel viewModel,
        FormListBrowserView formListBrowserView)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(formListBrowserView);
        ViewModel = viewModel;
        FormListBrowserView = formListBrowserView;
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

        var contentHost = new Border
        {
            Background = App.GetApplicationBrush(App.ApplicationSurfaceBrushKey),
            Padding = new Thickness(24),
            Child = FormListBrowserView
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

        var statusBar = new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 1, 0, 0),
            Padding = new Thickness(18, 8),
            Child = status
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
