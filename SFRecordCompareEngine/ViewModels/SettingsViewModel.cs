using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.Models.Configuration;
using SFRecordCompareEngine.Services.Interfaces;

namespace SFRecordCompareEngine.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly IApplicationConfigurationStore ApplicationConfigurationStore;
    private readonly IApplicationWindowService ApplicationWindowService;

    public SettingsViewModel(
        IApplicationConfigurationStore applicationConfigurationStore,
        IApplicationWindowService applicationWindowService
    )
    {
        ApplicationConfigurationStore = applicationConfigurationStore;
        ApplicationWindowService = applicationWindowService;
        ThemeOptions = Enum.GetValues<ApplicationThemeMode>();
        SelectedTheme = applicationConfigurationStore.Current.Theme;
    }

    public IReadOnlyList<ApplicationThemeMode> ThemeOptions { get; }

    public ApplicationThemeMode SelectedTheme
    {
        get;
        set => SetProperty(ref field, value);
    }

    public void Save()
    {
        var configuration = new ApplicationConfiguration
        {
            SelectedGame = ApplicationConfigurationStore.Current.SelectedGame,
            Theme = SelectedTheme
        };

        ApplicationConfigurationStore.Save(configuration);
        ApplicationWindowService.ApplyTheme(configuration.Theme);
    }
}
