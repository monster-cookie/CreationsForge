using SFRecordCompareEngine.Commands;
using SFRecordCompareEngine.Services.Interfaces;

namespace SFRecordCompareEngine.ViewModels;

public class OpenPluginDialogViewModel : ViewModelBase
{
    private readonly IApplicationNavigationService ApplicationNavigationService;

    public OpenPluginDialogViewModel(IApplicationNavigationService applicationNavigationService)
    {
        ApplicationNavigationService = applicationNavigationService;
        CloseCommand = new AsyncRelayCommand(CloseAsync);
    }

    public AsyncRelayCommand CloseCommand { get; }

    private async Task CloseAsync()
    {
        await ApplicationNavigationService.CloseOpenDialogAsync();
    }
}
