using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.ViewModels;

namespace CreationsForge.Views;

/// <summary>
/// Hosts application-level native workspace actions and the native editor content region.
/// </summary>
public sealed class NativeWorkspaceShellView : UserControl
{
    /// <summary>The native workspace shell state and commands.</summary>
    private readonly NativeWorkspaceShellViewModel ViewModel;

    /// <summary>The native FormList browser hosted for the current navigation scope.</summary>
    private readonly NativeFormListBrowserView FormListBrowserView;

    /// <summary>Initializes the native workspace shell view.</summary>
    /// <param name="viewModel">The shell state and commands.</param>
    /// <param name="formListBrowserView">The native FormList browser for this navigation scope.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public NativeWorkspaceShellView(
        NativeWorkspaceShellViewModel viewModel,
        NativeFormListBrowserView formListBrowserView)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(formListBrowserView);
        ViewModel = viewModel;
        FormListBrowserView = formListBrowserView;
        DataContext = ViewModel;
        AutomationProperties.SetAutomationId(this, "NativeWorkspaceShellView");
        Content = BuildContent();
    }

    /// <summary>Builds the shell toolbar, native content region, and active workspace status.</summary>
    /// <returns>The complete shell control tree.</returns>
    private Control BuildContent()
    {
        var toolbar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                CreateCommandButton("Open Plugin...", nameof(NativeWorkspaceShellViewModel.OpenWorkspaceCommand), "OpenNativeWorkspaceShellButton"),
                CreateCommandButton("Close Plugin", nameof(NativeWorkspaceShellViewModel.CloseWorkspaceCommand), "CloseNativeWorkspaceShellButton"),
                CreateCommandButton("Review Changes...", nameof(NativeWorkspaceShellViewModel.ReviewChangesCommand), "NativeWorkspaceReviewChangesButton"),
                CreateCommandButton("Save Changes...", nameof(NativeWorkspaceShellViewModel.SaveChangesCommand), "NativeWorkspaceSaveChangesButton"),
                CreateCommandButton("Discard Changes...", nameof(NativeWorkspaceShellViewModel.DiscardChangesCommand), "NativeWorkspaceDiscardChangesButton"),
                CreateCommandButton("Settings", nameof(NativeWorkspaceShellViewModel.SettingsCommand), "NativeWorkspaceSettingsButton")
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
        AutomationProperties.SetAutomationId(contentHost, "NativeWorkspaceContentHost");

        var status = new TextBlock
        {
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap
        };
        App.ApplyApplicationTextForeground(status);
        status.Bind(TextBlock.TextProperty, new Binding(nameof(NativeWorkspaceShellViewModel.WorkspaceStatusText)));
        AutomationProperties.SetAutomationId(status, "NativeWorkspaceStatusText");

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
