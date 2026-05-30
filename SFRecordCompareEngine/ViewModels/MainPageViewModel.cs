using SFRecordCompareEngine.Commands;
using SFRecordCompareEngine.Services.Interfaces;

namespace SFRecordCompareEngine.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    private readonly IApplicationNavigationService ApplicationNavigationService;
    private readonly IActivePluginSelectionService ActivePluginSelectionService;

    public MainPageViewModel(
        IApplicationNavigationService applicationNavigationService,
        IActivePluginSelectionService activePluginSelectionService)
    {
        ApplicationNavigationService = applicationNavigationService;
        ActivePluginSelectionService = activePluginSelectionService;
        OpenCommand = new AsyncRelayCommand(OpenAsync);
        OptionsCommand = new AsyncRelayCommand(ShowOptionsAsync);
        ExitCommand = new RelayCommand(ApplicationNavigationService.Quit);
        StatusText = GetStatusText();
        ActivePluginSelectionService.ActivePluginChanged += OnActivePluginChanged;
    }

    public AsyncRelayCommand OpenCommand { get; }
    public AsyncRelayCommand OptionsCommand { get; }
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

    private async Task ShowOptionsAsync()
    {
        await ApplicationNavigationService.ShowSettingsDialogAsync();
    }

    private void OnActivePluginChanged(object? sender, EventArgs e)
    {
        StatusText = GetStatusText();
    }

    private string GetStatusText()
    {
        return ActivePluginSelectionService.ActivePlugin == null
            ? "No active plugin selected."
            : $"Active plugin: {ActivePluginSelectionService.ActivePlugin.ModKey.FileName}";
    }
}
