using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services.Interfaces;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Core.Services;

/// <summary>
/// Provides persisted application settings for display, localization, external tools, and plugin selector behavior.
/// </summary>
public class ApplicationSettingsService : IApplicationSettingsService
{
    private readonly IApplicationConfigurationStore ConfigurationStore;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationSettingsService"/> class.
    /// </summary>
    /// <param name="configurationStore">The configuration store used to read and save application settings.</param>
    public ApplicationSettingsService(IApplicationConfigurationStore configurationStore)
    {
        ConfigurationStore = configurationStore;
    }

    /// <inheritdoc />
    public ApplicationThemeMode GetThemeMode()
    {
        return ConfigurationStore.Current.ThemeMode;
    }

    /// <inheritdoc />
    public ApplicationThemeFamily GetThemeFamily()
    {
        return ConfigurationStore.Current.ThemeFamily;
    }

    /// <inheritdoc />
    public IReadOnlyList<Language> GetRecordTextLanguages()
    {
        return Enum.GetValues<Language>().OrderBy(language => language.ToString(), StringComparer.OrdinalIgnoreCase).ToList();
    }

    /// <inheritdoc />
    public Language GetRecordTextLanguage()
    {
        return NormalizeRecordTextLanguage(ConfigurationStore.Current.RecordTextLanguage);
    }

    /// <inheritdoc />
    public string? GetNifSkopeExecutablePath()
    {
        return ConfigurationStore.Current.NifSkopeExecutablePath;
    }

    /// <inheritdoc />
    public bool GetPreferEspOverMatchingEsm()
    {
        return ConfigurationStore.Current.PreferEspOverMatchingEsm;
    }

    /// <inheritdoc />
    public void SetThemeMode(ApplicationThemeMode themeMode)
    {
        SaveConfiguration(ConfigurationStore.Current.ActiveGame, ConfigurationStore.Current.ThemeFamily, themeMode, GetRecordTextLanguage().ToString(), ConfigurationStore.Current.NifSkopeExecutablePath);
    }

    /// <inheritdoc />
    public void SetThemeFamily(ApplicationThemeFamily themeFamily)
    {
        SaveConfiguration(ConfigurationStore.Current.ActiveGame, themeFamily, ConfigurationStore.Current.ThemeMode, GetRecordTextLanguage().ToString(), ConfigurationStore.Current.NifSkopeExecutablePath);
    }

    /// <inheritdoc />
    public void SetTheme(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
    {
        SaveConfiguration(ConfigurationStore.Current.ActiveGame, themeFamily, themeMode, GetRecordTextLanguage().ToString(), ConfigurationStore.Current.NifSkopeExecutablePath);
    }

    /// <inheritdoc />
    public void SetThemeAndNifSkopeExecutablePath(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, string? nifSkopeExecutablePath)
    {
        SaveConfiguration(ConfigurationStore.Current.ActiveGame, themeFamily, themeMode, GetRecordTextLanguage().ToString(), NormalizeOptionalPath(nifSkopeExecutablePath));
    }

    /// <inheritdoc />
    public void SetThemeRecordTextLanguageNifSkopeExecutablePathAndPluginSelectionPreference(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, Language recordTextLanguage, string? nifSkopeExecutablePath, bool preferEspOverMatchingEsm)
    {
        SaveConfiguration(ConfigurationStore.Current.ActiveGame, themeFamily, themeMode, recordTextLanguage.ToString(), NormalizeOptionalPath(nifSkopeExecutablePath), preferEspOverMatchingEsm);
    }

    /// <inheritdoc />
    public void SetActiveGameThemeRecordTextLanguageNifSkopeExecutablePathAndPluginSelectionPreference(SupportedGame game, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, Language recordTextLanguage, string? nifSkopeExecutablePath, bool preferEspOverMatchingEsm)
    {
        SaveConfiguration(game.ToString(), themeFamily, themeMode, recordTextLanguage.ToString(), NormalizeOptionalPath(nifSkopeExecutablePath), preferEspOverMatchingEsm);
    }

    /// <summary>
    /// Persists application settings while preserving configured paths and plugin-list preferences by default.
    /// </summary>
    /// <param name="activeGame">The active game name to store, or <see langword="null"/> when none is selected.</param>
    /// <param name="themeFamily">The Avalonia theme family to store.</param>
    /// <param name="themeMode">The light or dark display mode to store.</param>
    /// <param name="recordTextLanguage">The localized record text language to store.</param>
    /// <param name="nifSkopeExecutablePath">The optional external NifSkope executable path to store.</param>
    /// <param name="preferEspOverMatchingEsm">Optional plugin-list preference override.</param>
    private void SaveConfiguration(string? activeGame, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, string recordTextLanguage, string? nifSkopeExecutablePath, bool? preferEspOverMatchingEsm = null)
    {
        var configuration = new ApplicationConfiguration
        {
            ActiveGame = activeGame,
            ThemeFamily = themeFamily,
            ThemeMode = themeMode,
            RecordTextLanguage = recordTextLanguage,
            NifSkopeExecutablePath = nifSkopeExecutablePath,
            PreferEspOverMatchingEsm = preferEspOverMatchingEsm ?? ConfigurationStore.Current.PreferEspOverMatchingEsm,
            ApplicationDataDirectory = ConfigurationStore.Current.ApplicationDataDirectory,
            DatabaseDirectory = ConfigurationStore.Current.DatabaseDirectory,
            LoggingDirectory = ConfigurationStore.Current.LoggingDirectory
        };

        ConfigurationStore.Save(configuration);
    }

    /// <summary>
    /// Normalizes an optional path from user input before saving it.
    /// </summary>
    /// <param name="path">The optional path to normalize.</param>
    /// <returns>The trimmed path, or <see langword="null"/> for blank values.</returns>
    private static string? NormalizeOptionalPath(string? path)
    {
        return string.IsNullOrWhiteSpace(path)
            ? null
            : path.Trim();
    }

    /// <summary>
    /// Normalizes a persisted record text language value.
    /// </summary>
    /// <param name="language">The stored language name.</param>
    /// <returns>The parsed language, or English when the stored value is invalid.</returns>
    private static Language NormalizeRecordTextLanguage(string? language)
    {
        return Enum.TryParse<Language>(language, true, out var parsedLanguage)
            ? parsedLanguage
            : Language.English;
    }
}
