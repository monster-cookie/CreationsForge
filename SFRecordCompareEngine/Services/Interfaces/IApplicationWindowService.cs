namespace SFRecordCompareEngine.Services.Interfaces;

public interface IApplicationWindowService
{
    void RegisterMainWindow(MainWindow mainWindow);
    void ShowMainCommandSurface(ViewModels.MainPageViewModel viewModel);
    void ApplyTheme(Core.Models.Configuration.ApplicationThemeMode theme);
    void SetContent(Microsoft.UI.Xaml.UIElement content);
    Task ShowDialogAsync(Microsoft.UI.Xaml.Controls.ContentDialog dialog);
    void CloseDialog();
    void MaximizeMainWindow();
    void Quit();
}
