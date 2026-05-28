using SFRecordCompareEngine.Commands;
using SFRecordCompareEngine.Services.Interfaces;

namespace SFRecordCompareEngine.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    private readonly IApplicationNavigationService ApplicationNavigationService;

    public MainPageViewModel(IApplicationNavigationService applicationNavigationService)
    {
        ApplicationNavigationService = applicationNavigationService;
        OpenCommand = new AsyncRelayCommand(OpenAsync);
        ExitCommand = new RelayCommand(ApplicationNavigationService.Quit);
        StatusText = "Ready.";
    }

    public AsyncRelayCommand OpenCommand { get; }
    public RelayCommand ExitCommand { get; }

    public string StatusText
    {
        get;
        private set => SetProperty(ref field, value);
    }

    private async Task OpenAsync()
    {
        await ApplicationNavigationService.ShowOpenDialogAsync();
    }
}
