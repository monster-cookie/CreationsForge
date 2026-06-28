using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Core.Services.Interfaces;

/// <summary>
/// Provides UI-neutral access to persisted application display, localization, and plugin selector preferences.
/// </summary>
public interface IApplicationSettingsService
{
    /// <summary>
    /// Gets the configured theme mode.
    /// </summary>
    /// <returns>The configured light or dark theme mode.</returns>
    ApplicationThemeMode GetThemeMode();

    /// <summary>
    /// Gets the configured theme family.
    /// </summary>
    /// <returns>The configured application theme family.</returns>
    ApplicationThemeFamily GetThemeFamily();

    /// <summary>
    /// Gets the record text languages available for localized record display.
    /// </summary>
    /// <returns>The supported Mutagen language values sorted for display.</returns>
    IReadOnlyList<Language> GetRecordTextLanguages();

    /// <summary>
    /// Gets the configured record text language.
    /// </summary>
    /// <returns>The configured language, or English when the stored value is invalid.</returns>
    Language GetRecordTextLanguage();

    /// <summary>
    /// Gets the configured external NifSkope executable path.
    /// </summary>
    /// <returns>The configured path, or <see langword="null"/> when no path is configured.</returns>
    string? GetNifSkopeExecutablePath();

    /// <summary>
    /// Gets whether plugin selectors should hide a matching ESM when an ESP with the same base filename exists.
    /// </summary>
    /// <returns><see langword="true"/> when matching ESP rows are preferred over ESM rows.</returns>
    bool GetPreferEspOverMatchingEsm();

    /// <summary>
    /// Saves a theme mode while preserving the other persisted application settings.
    /// </summary>
    /// <param name="themeMode">The theme mode to save.</param>
    void SetThemeMode(ApplicationThemeMode themeMode);

    /// <summary>
    /// Saves a theme family while preserving the other persisted application settings.
    /// </summary>
    /// <param name="themeFamily">The theme family to save.</param>
    void SetThemeFamily(ApplicationThemeFamily themeFamily);

    /// <summary>
    /// Saves the configured theme while preserving the other persisted application settings.
    /// </summary>
    /// <param name="themeFamily">The theme family to save.</param>
    /// <param name="themeMode">The theme mode to save.</param>
    void SetTheme(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode);

    /// <summary>
    /// Saves display and external-tool settings without changing the active game.
    /// </summary>
    /// <param name="themeFamily">The theme family to save.</param>
    /// <param name="themeMode">The theme mode to save.</param>
    /// <param name="nifSkopeExecutablePath">The optional external NifSkope executable path to save.</param>
    void SetThemeAndNifSkopeExecutablePath(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, string? nifSkopeExecutablePath);

    /// <summary>
    /// Saves display, localization, external-tool, and plugin-selector settings without changing the active game.
    /// </summary>
    /// <param name="themeFamily">The theme family to save.</param>
    /// <param name="themeMode">The theme mode to save.</param>
    /// <param name="recordTextLanguage">The record text language to save.</param>
    /// <param name="nifSkopeExecutablePath">The optional external NifSkope executable path to save.</param>
    /// <param name="preferEspOverMatchingEsm">Whether matching ESP rows should be preferred over ESM rows.</param>
    void SetThemeRecordTextLanguageNifSkopeExecutablePathAndPluginSelectionPreference(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, Language recordTextLanguage, string? nifSkopeExecutablePath, bool preferEspOverMatchingEsm);

    /// <summary>
    /// Saves active-game, display, localization, external-tool, and plugin-selector settings.
    /// </summary>
    /// <param name="game">The active game to save.</param>
    /// <param name="themeFamily">The theme family to save.</param>
    /// <param name="themeMode">The theme mode to save.</param>
    /// <param name="recordTextLanguage">The record text language to save.</param>
    /// <param name="nifSkopeExecutablePath">The optional external NifSkope executable path to save.</param>
    /// <param name="preferEspOverMatchingEsm">Whether matching ESP rows should be preferred over ESM rows.</param>
    void SetActiveGameThemeRecordTextLanguageNifSkopeExecutablePathAndPluginSelectionPreference(SupportedGame game, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode, Language recordTextLanguage, string? nifSkopeExecutablePath, bool preferEspOverMatchingEsm);
}
