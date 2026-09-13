using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.PresentationTests.Support;
using CreationsForge.ViewModels;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <summary>
/// Verifies settings preserve application preferences while returning through native navigation.
/// </summary>
public sealed class SettingsViewModelTests
{
    /// <summary>Verifies Save persists editable preferences, preserves the platform-specific NifSkope contract, applies the theme, and returns to the native shell.</summary>
    [Fact]
    public void SaveCommand_WithSelectedGame_PreservesSettingsBehaviorAndShowsNativeShell()
    {
        var game = new SupportedGameDTO
        {
            Game = SupportedGame.Fallout4,
            Name = nameof(SupportedGame.Fallout4),
            DisplayName = "Fallout 4"
        };
        var gameSelection = new FakeGameSelectionService
        {
            ActiveGame = SupportedGame.Fallout4,
            SupportedGames = [game]
        };
        var settings = new FakeApplicationSettingsService();
        var navigation = new FakeNativeApplicationNavigationService();
        var window = new FakeApplicationWindowService();
        var viewModel = new SettingsViewModel(gameSelection, settings, navigation, window)
        {
            SelectedThemeFamily = nameof(ApplicationThemeFamily.Fluent),
            SelectedThemeMode = nameof(ApplicationThemeMode.Light),
            SelectedRecordTextLanguage = nameof(Language.German),
            NifSkopeExecutablePath = "  NifSkope.exe  ",
            PreferEspOverMatchingEsm = false
        };

        viewModel.SaveCommand.Execute(null);

        settings.SavedGame.ShouldBe(SupportedGame.Fallout4);
        settings.SavedThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        settings.SavedThemeMode.ShouldBe(ApplicationThemeMode.Light);
        settings.SavedRecordTextLanguage.ShouldBe(Language.German);
        settings.SavedNifSkopeExecutablePath.ShouldBe(OperatingSystem.IsWindows() ? "NifSkope.exe" : null);
        settings.SavedPreferEspOverMatchingEsm.ShouldBeFalse();
        window.ThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        window.ThemeMode.ShouldBe(ApplicationThemeMode.Light);
        navigation.WorkspaceShellCount.ShouldBe(1);
    }

    /// <summary>Verifies Cancel leaves persisted settings unchanged and returns to the native shell.</summary>
    [Fact]
    public void CancelCommand_WhenExecuted_ShowsNativeShellWithoutSaving()
    {
        var settings = new FakeApplicationSettingsService();
        var navigation = new FakeNativeApplicationNavigationService();
        var viewModel = new SettingsViewModel(
            new FakeGameSelectionService(),
            settings,
            navigation,
            new FakeApplicationWindowService());

        viewModel.CancelCommand.Execute(null);

        settings.SaveCount.ShouldBe(0);
        navigation.WorkspaceShellCount.ShouldBe(1);
    }

    /// <summary>Stores settings calls for deterministic presentation verification.</summary>
    private sealed class FakeApplicationSettingsService : IApplicationSettingsService
    {
        /// <summary>Gets how many combined settings writes were received.</summary>
        public int SaveCount { get; private set; }

        /// <summary>Gets the game supplied to the most recent combined write.</summary>
        public SupportedGame? SavedGame { get; private set; }

        /// <summary>Gets the theme family supplied to the most recent combined write.</summary>
        public ApplicationThemeFamily? SavedThemeFamily { get; private set; }

        /// <summary>Gets the theme mode supplied to the most recent combined write.</summary>
        public ApplicationThemeMode? SavedThemeMode { get; private set; }

        /// <summary>Gets the record text language supplied to the most recent combined write.</summary>
        public Language? SavedRecordTextLanguage { get; private set; }

        /// <summary>Gets the NifSkope path supplied to the most recent combined write.</summary>
        public string? SavedNifSkopeExecutablePath { get; private set; }

        /// <summary>Gets the plugin-selection preference supplied to the most recent combined write.</summary>
        public bool SavedPreferEspOverMatchingEsm { get; private set; }

        /// <inheritdoc />
        public ApplicationThemeMode GetThemeMode()
        {
            return ApplicationThemeMode.Dark;
        }

        /// <inheritdoc />
        public ApplicationThemeFamily GetThemeFamily()
        {
            return ApplicationThemeFamily.Semi;
        }

        /// <inheritdoc />
        public IReadOnlyList<Language> GetRecordTextLanguages()
        {
            return [Language.English, Language.German];
        }

        /// <inheritdoc />
        public Language GetRecordTextLanguage()
        {
            return Language.English;
        }

        /// <inheritdoc />
        public string? GetNifSkopeExecutablePath()
        {
            return null;
        }

        /// <inheritdoc />
        public bool GetPreferEspOverMatchingEsm()
        {
            return true;
        }

        /// <inheritdoc />
        public void SetThemeMode(ApplicationThemeMode themeMode)
        {
            SavedThemeMode = themeMode;
        }

        /// <inheritdoc />
        public void SetThemeFamily(ApplicationThemeFamily themeFamily)
        {
            SavedThemeFamily = themeFamily;
        }

        /// <inheritdoc />
        public void SetTheme(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
        {
            SavedThemeFamily = themeFamily;
            SavedThemeMode = themeMode;
        }

        /// <inheritdoc />
        public void SetThemeAndNifSkopeExecutablePath(
            ApplicationThemeFamily themeFamily,
            ApplicationThemeMode themeMode,
            string? nifSkopeExecutablePath)
        {
            SavedThemeFamily = themeFamily;
            SavedThemeMode = themeMode;
            SavedNifSkopeExecutablePath = nifSkopeExecutablePath;
        }

        /// <inheritdoc />
        public void SetThemeRecordTextLanguageNifSkopeExecutablePathAndPluginSelectionPreference(
            ApplicationThemeFamily themeFamily,
            ApplicationThemeMode themeMode,
            Language recordTextLanguage,
            string? nifSkopeExecutablePath,
            bool preferEspOverMatchingEsm)
        {
            Save(null, themeFamily, themeMode, recordTextLanguage, nifSkopeExecutablePath, preferEspOverMatchingEsm);
        }

        /// <inheritdoc />
        public void SetActiveGameThemeRecordTextLanguageNifSkopeExecutablePathAndPluginSelectionPreference(
            SupportedGame game,
            ApplicationThemeFamily themeFamily,
            ApplicationThemeMode themeMode,
            Language recordTextLanguage,
            string? nifSkopeExecutablePath,
            bool preferEspOverMatchingEsm)
        {
            Save(game, themeFamily, themeMode, recordTextLanguage, nifSkopeExecutablePath, preferEspOverMatchingEsm);
        }

        /// <summary>Records a combined settings write.</summary>
        /// <param name="game">The active game, or <see langword="null"/> when unchanged.</param>
        /// <param name="themeFamily">The selected theme family.</param>
        /// <param name="themeMode">The selected theme mode.</param>
        /// <param name="recordTextLanguage">The selected record text language.</param>
        /// <param name="nifSkopeExecutablePath">The optional NifSkope path.</param>
        /// <param name="preferEspOverMatchingEsm">The plugin-selection preference.</param>
        private void Save(
            SupportedGame? game,
            ApplicationThemeFamily themeFamily,
            ApplicationThemeMode themeMode,
            Language recordTextLanguage,
            string? nifSkopeExecutablePath,
            bool preferEspOverMatchingEsm)
        {
            SaveCount++;
            SavedGame = game;
            SavedThemeFamily = themeFamily;
            SavedThemeMode = themeMode;
            SavedRecordTextLanguage = recordTextLanguage;
            SavedNifSkopeExecutablePath = nifSkopeExecutablePath;
            SavedPreferEspOverMatchingEsm = preferEspOverMatchingEsm;
        }
    }
}
