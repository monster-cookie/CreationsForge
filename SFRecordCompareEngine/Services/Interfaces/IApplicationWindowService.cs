using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SFRecordCompareEngine.Core.Models.Configuration;
using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine.Services.Interfaces;

public interface IApplicationWindowService
{
    void RegisterMainWindow(MainWindow mainWindow);
    void ShowMainCommandSurface(MainPageViewModel viewModel);
    void HideMainCommandSurface();
    void ApplyTheme(ApplicationThemeMode theme);
    void SetContent(UIElement content);
    Task ShowDialogAsync(ContentDialog dialog);
    void CloseDialog();
    void MaximizeMainWindow();
    void Quit();
}
