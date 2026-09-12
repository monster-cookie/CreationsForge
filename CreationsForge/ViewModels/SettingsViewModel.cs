using System.Windows.Input;
using CreationsForge.Commands;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services.Interfaces;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.ViewModels;

/// <summary>
/// Edits persisted application preferences and returns to the native workspace shell.
/// </summary>
public class SettingsViewModel : ViewModelBase
{
    /// <summary>The native shell/settings navigation boundary.</summary>
    private readonly INativeApplicationNavigationService NativeApplicationNavigationService;

    /// <summary>The application window owner and theme service.</summary>
    private readonly IApplicationWindowService ApplicationWindowService;

    /// <summary>The persisted application settings service.</summary>
    private readonly IApplicationSettingsService ApplicationSettingsService;

    /// <summary>The supported and active game settings service.</summary>
    private readonly IGameSelectionService GameSelectionService;
    private string? SelectedGameDisplayNameValue;
    private string SelectedThemeFamilyValue;
    private string SelectedThemeModeValue;
    private string SelectedRecordTextLanguageValue;
    private string? NifSkopeExecutablePathValue;
    private bool PreferEspOverMatchingEsmValue;

    /// <summary>Initializes editable settings from the current persisted configuration.</summary>
    /// <param name="gameSelectionService">The supported and active game settings service.</param>
    /// <param name="applicationSettingsService">The persisted display, localization, and tool settings service.</param>
    /// <param name="nativeApplicationNavigationService">The native shell/settings navigation boundary.</param>
    /// <param name="applicationWindowService">The application window owner and theme service.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public SettingsViewModel(
        IGameSelectionService gameSelectionService,
        IApplicationSettingsService applicationSettingsService,
        INativeApplicationNavigationService nativeApplicationNavigationService,
        IApplicationWindowService applicationWindowService)
    {
        ArgumentNullException.ThrowIfNull(gameSelectionService);
        ArgumentNullException.ThrowIfNull(applicationSettingsService);
        ArgumentNullException.ThrowIfNull(nativeApplicationNavigationService);
        ArgumentNullException.ThrowIfNull(applicationWindowService);
        GameSelectionService = gameSelectionService;
        ApplicationSettingsService = applicationSettingsService;
        NativeApplicationNavigationService = nativeApplicationNavigationService;
        ApplicationWindowService = applicationWindowService;
        SupportedGames = GameSelectionService.GetSupportedGames();
        GameOptions = SupportedGames.Select(game => game.DisplayName).ToList();
        ThemeFamilyOptions = ["Semi", "Fluent"];
        ThemeModeOptions = ["Dark", "Light"];
        RecordTextLanguageOptions = ApplicationSettingsService.GetRecordTextLanguages().Select(language => language.ToString()).ToList();
        SelectedGameDisplayNameValue = GetConfiguredGame()?.DisplayName;
        SelectedThemeFamilyValue = ApplicationSettingsService.GetThemeFamily().ToString();
        SelectedThemeModeValue = ApplicationSettingsService.GetThemeMode().ToString();
        SelectedRecordTextLanguageValue = ApplicationSettingsService.GetRecordTextLanguage().ToString();
        NifSkopeExecutablePathValue = ApplicationSettingsService.GetNifSkopeExecutablePath();
        PreferEspOverMatchingEsmValue = ApplicationSettingsService.GetPreferEspOverMatchingEsm();
        SaveCommand = new RelayCommand(Save);
        BrowseNifSkopeExecutableCommand = new AsyncRelayCommand(BrowseNifSkopeExecutableAsync, () => IsNifSkopeSettingVisible);
        CancelCommand = new RelayCommand(Cancel);
    }

    public IReadOnlyList<SupportedGameDTO> SupportedGames { get; }

    public IList<string> GameOptions { get; }

    public IList<string> ThemeModeOptions { get; }

    public IList<string> ThemeFamilyOptions { get; }

    public IList<string> RecordTextLanguageOptions { get; }

    public ICommand SaveCommand { get; }

    public ICommand BrowseNifSkopeExecutableCommand { get; }

    public ICommand CancelCommand { get; }

    public bool IsNifSkopeSettingVisible => OperatingSystem.IsWindows();

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

    public string SelectedRecordTextLanguage
    {
        get => SelectedRecordTextLanguageValue;
        set => SetProperty(ref SelectedRecordTextLanguageValue, value);
    }

    public string? NifSkopeExecutablePath
    {
        get => NifSkopeExecutablePathValue;
        set => SetProperty(ref NifSkopeExecutablePathValue, value);
    }

    /// <summary>
    /// Gets or sets whether plugin selectors hide a matching ESM when an ESP with the same base filename exists.
    /// </summary>
    public bool PreferEspOverMatchingEsm
    {
        get => PreferEspOverMatchingEsmValue;
        set => SetProperty(ref PreferEspOverMatchingEsmValue, value);
    }

    /// <summary>Saves every editable preference, applies the theme, and returns to the native workspace shell.</summary>
    private void Save()
    {
        var selectedGame = SupportedGames.FirstOrDefault(game =>
            string.Equals(game.DisplayName, SelectedGameDisplayName, StringComparison.OrdinalIgnoreCase));
        var themeFamily = GetSelectedThemeFamily();
        var themeMode = GetSelectedThemeMode();
        var recordTextLanguage = GetSelectedRecordTextLanguage();
        if (selectedGame is not null)
        {
            ApplicationSettingsService.SetActiveGameThemeRecordTextLanguageNifSkopeExecutablePathAndPluginSelectionPreference(selectedGame.Game, themeFamily, themeMode, recordTextLanguage, GetSelectedNifSkopeExecutablePath(), PreferEspOverMatchingEsm);
        }
        else
        {
            ApplicationSettingsService.SetThemeRecordTextLanguageNifSkopeExecutablePathAndPluginSelectionPreference(themeFamily, themeMode, recordTextLanguage, GetSelectedNifSkopeExecutablePath(), PreferEspOverMatchingEsm);
        }

        ApplicationWindowService.ApplyTheme(themeFamily, themeMode);
        NativeApplicationNavigationService.ShowWorkspaceShell();
    }

    /// <summary>Discards unsaved edits and returns to the native workspace shell.</summary>
    private void Cancel()
    {
        NativeApplicationNavigationService.ShowWorkspaceShell();
    }

    private async Task BrowseNifSkopeExecutableAsync()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var selectedPath = await ApplicationWindowService.ShowNifSkopeExecutablePickerAsync();
        if (!string.IsNullOrWhiteSpace(selectedPath))
        {
            NifSkopeExecutablePath = selectedPath;
        }
    }

    private SupportedGameDTO? GetConfiguredGame()
    {
        var activeGame = GameSelectionService.GetActiveGame();
        return activeGame.HasValue
            ? SupportedGames.FirstOrDefault(game => game.Game == activeGame.Value)
            : null;
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

    private Language GetSelectedRecordTextLanguage()
    {
        return Enum.TryParse<Language>(SelectedRecordTextLanguage, true, out var language)
            ? language
            : Language.English;
    }

    private string? GetSelectedNifSkopeExecutablePath()
    {
        if (!OperatingSystem.IsWindows())
        {
            return ApplicationSettingsService.GetNifSkopeExecutablePath();
        }

        return string.IsNullOrWhiteSpace(NifSkopeExecutablePath)
            ? null
            : NifSkopeExecutablePath.Trim();
    }
}
