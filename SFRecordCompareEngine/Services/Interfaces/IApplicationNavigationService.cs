namespace SFRecordCompareEngine.Services.Interfaces;

public interface IApplicationNavigationService
{
    Task ShowMainPageAsync();
    Task ShowOpenDialogAsync();
    Task CloseOpenDialogAsync();
    void Quit();
}
