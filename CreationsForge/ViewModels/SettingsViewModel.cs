using System.Windows.Input;
using CreationsForge.Commands;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly IApplicationNavigationService ApplicationNavigationService;
    private readonly IApplicationWindowService ApplicationWindowService;
    private readonly IGameSelectionService GameSelectionService;
    private readonly SupportedGameDTO? InitialSelectedGame;
    private string? SelectedGameDisplayNameValue;
    private string SelectedThemeFamilyValue;
    private string SelectedThemeModeValue;

    public SettingsViewModel(
        IGameSelectionService gameSelectionService,
        IApplicationNavigationService applicationNavigationService,
        IApplicationWindowService applicationWindowService)
    {
        GameSelectionService = gameSelectionService;
        ApplicationNavigationService = applicationNavigationService;
        ApplicationWindowService = applicationWindowService;
        SupportedGames = GameSelectionService.GetSupportedGames();
        GameOptions = SupportedGames.Select(game => game.DisplayName).ToList();
        ThemeFamilyOptions = ["Semi", "Fluent"];
        ThemeModeOptions = ["Dark", "Light"];
        InitialSelectedGame = GetConfiguredGame();
        SelectedGameDisplayNameValue = InitialSelectedGame?.DisplayName;
        SelectedThemeFamilyValue = GameSelectionService.GetThemeFamily().ToString();
        SelectedThemeModeValue = GameSelectionService.GetThemeMode().ToString();
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    public IReadOnlyList<SupportedGameDTO> SupportedGames { get; }

    public IList<string> GameOptions { get; }

    public IList<string> ThemeModeOptions { get; }

    public IList<string> ThemeFamilyOptions { get; }

    public ICommand SaveCommand { get; }

    public ICommand CancelCommand { get; }

    public string? SelectedGameDisplayName
    {
        get => SelectedGameDisplayNameValue;
        set => SetProperty(ref SelectedGameDisplayNameValue, value);
    }

    public string SelectedThemeMode
    {
        get => SelectedThemeModeValue;
        set => SetProperty(ref SelectedThemeModeValue, value);
    }

    public string SelectedThemeFamily
    {
        get => SelectedThemeFamilyValue;
        set => SetProperty(ref SelectedThemeFamilyValue, value);
    }

    private void Save()
    {
        var selectedGame = SupportedGames.FirstOrDefault(game =>
            string.Equals(game.DisplayName, SelectedGameDisplayName, StringComparison.OrdinalIgnoreCase));
        var themeFamily = GetSelectedThemeFamily();
        var themeMode = GetSelectedThemeMode();
        if (selectedGame is not null)
        {
            GameSelectionService.SetActiveGameAndTheme(selectedGame.Game, themeFamily, themeMode);
        }
        else
        {
            GameSelectionService.SetTheme(themeFamily, themeMode);
        }

        ApplicationWindowService.ApplyTheme(themeFamily, themeMode);
        _ = ApplicationNavigationService.ShowMainViewAsync(selectedGame, ShouldRunImport(selectedGame));
    }

    private void Cancel()
    {
        _ = ApplicationNavigationService.ShowMainViewAsync(InitialSelectedGame, runConfiguredGameImport: false);
    }

    private SupportedGameDTO? GetConfiguredGame()
    {
        var activeGame = GameSelectionService.GetActiveGame();
        return activeGame.HasValue
            ? SupportedGames.FirstOrDefault(game => game.Game == activeGame.Value)
            : null;
    }

    private bool ShouldRunImport(SupportedGameDTO? selectedGame)
    {
        return selectedGame is not null && InitialSelectedGame?.Game != selectedGame.Game;
    }

    private ApplicationThemeFamily GetSelectedThemeFamily()
    {
        return Enum.TryParse<ApplicationThemeFamily>(SelectedThemeFamily, true, out var themeFamily)
            ? themeFamily
            : ApplicationThemeFamily.Semi;
    }

    private ApplicationThemeMode GetSelectedThemeMode()
    {
        return Enum.TryParse<ApplicationThemeMode>(SelectedThemeMode, true, out var themeMode)
            ? themeMode
            : ApplicationThemeMode.Dark;
    }

}
