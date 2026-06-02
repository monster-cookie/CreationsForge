using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.Models.Configuration;
using SFRecordCompareEngine.Services.Interfaces;
using SFRecordCompareEngine.ViewModels;
using SFRecordCompareEngine.Views;

namespace SFRecordCompareEngine;

public sealed partial class MainWindow : Window
{
    private ContentDialog? ActiveDialog;

    public MainWindow(
        StartupImportView startupImportView,
        IApplicationConfigurationStore applicationConfigurationStore,
        IApplicationWindowService applicationWindowService)
    {
        InitializeComponent();

        applicationWindowService.RegisterMainWindow(this);
        ApplyTheme(applicationConfigurationStore.Current.Theme);
        SetContent(startupImportView);
        applicationWindowService.MaximizeMainWindow();
    }

    public void SetContent(UIElement content)
    {
        RootHost.Children.Clear();
        RootHost.Children.Add(content);
    }

    public void ShowMainCommandSurface(MainPageViewModel viewModel)
    {
        OpenMenuItem.Command = viewModel.OpenCommand;
        OptionsMenuItem.Command = viewModel.OptionsCommand;
        ExitMenuItem.Command = viewModel.ExitCommand;
        ReimportAllPluginsMenuItem.Command = viewModel.ReimportAllPluginsCommand;
        OpenCommandButton.Command = viewModel.OpenCommand;
        SettingsCommandButton.Command = viewModel.OptionsCommand;
        ReimportAllPluginsCommandButton.Command = viewModel.ReimportAllPluginsCommand;
        MainMenuBar.Visibility = Visibility.Visible;
        MainCommandBar.Visibility = Visibility.Visible;
    }

    public void HideMainCommandSurface()
    {
        MainMenuBar.Visibility = Visibility.Collapsed;
        MainCommandBar.Visibility = Visibility.Collapsed;
    }

    public void ApplyTheme(ApplicationThemeMode theme)
    {
        ShellRoot.RequestedTheme = theme == ApplicationThemeMode.Light
            ? ElementTheme.Light
            : ElementTheme.Dark;
    }

    public async Task ShowDialogAsync(ContentDialog dialog)
    {
        dialog.XamlRoot = RootHost.XamlRoot;
        ActiveDialog = dialog;
        await dialog.ShowAsync();
        ActiveDialog = null;
    }

    public void CloseDialog()
    {
        ActiveDialog?.Hide();
        ActiveDialog = null;
    }
}
