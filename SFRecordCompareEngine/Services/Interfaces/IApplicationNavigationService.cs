namespace SFRecordCompareEngine.Services.Interfaces;

public interface IApplicationNavigationService
{
    Task ShowMainPageAsync();
    Task ShowStartupImportAsync(bool forceFullReimport);
    Task ShowOpenDialogAsync();
    Task ShowSettingsDialogAsync();
    Task CloseOpenDialogAsync();
    void Quit();
}
