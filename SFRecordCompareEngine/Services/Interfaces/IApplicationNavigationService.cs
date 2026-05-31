namespace SFRecordCompareEngine.Services.Interfaces;

public interface IApplicationNavigationService
{
    Task ShowMainPageAsync();
    Task ShowOpenDialogAsync();
    Task ShowSettingsDialogAsync();
    Task CloseOpenDialogAsync();
    void Quit();
}