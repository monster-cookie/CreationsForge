using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

/// <summary>
/// Tests persisted application settings behavior for <see cref="ApplicationSettingsService"/>.
/// </summary>
public class ApplicationSettingsServiceTests
{
    /// <summary>
    /// Verifies the configured theme family is returned.
    /// </summary>
    [Fact]
    public void GetThemeFamily_ReturnsConfiguredThemeFamily()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration { ThemeFamily = ApplicationThemeFamily.Fluent }
        };
        var service = new ApplicationSettingsService(store);

        var themeFamily = service.GetThemeFamily();

        themeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
    }

    /// <summary>
    /// Verifies the configured theme mode is returned.
    /// </summary>
    [Fact]
    public void GetThemeMode_ReturnsConfiguredThemeMode()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration { ThemeMode = ApplicationThemeMode.Light }
        };
        var service = new ApplicationSettingsService(store);

        var themeMode = service.GetThemeMode();

        themeMode.ShouldBe(ApplicationThemeMode.Light);
    }

    /// <summary>
    /// Verifies the configured NifSkope executable path is returned.
    /// </summary>
    [Fact]
    public void GetNifSkopeExecutablePath_ReturnsConfiguredPath()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration { NifSkopeExecutablePath = "nifskope.exe" }
        };
        var service = new ApplicationSettingsService(store);

        var path = service.GetNifSkopeExecutablePath();

        path.ShouldBe("nifskope.exe");
    }

    /// <summary>
    /// Verifies the configured ESP-over-ESM plugin selector preference is returned.
    /// </summary>
    [Fact]
    public void GetPreferEspOverMatchingEsm_ReturnsConfiguredPreference()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration { PreferEspOverMatchingEsm = false }
        };
        var service = new ApplicationSettingsService(store);

        var preference = service.GetPreferEspOverMatchingEsm();

        preference.ShouldBeFalse();
    }

    /// <summary>
    /// Verifies invalid record text language configuration falls back to English.
    /// </summary>
    [Fact]
    public void GetRecordTextLanguage_WithInvalidConfiguredLanguage_ReturnsEnglish()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration { RecordTextLanguage = "NotALanguage" }
        };
        var service = new ApplicationSettingsService(store);

        var language = service.GetRecordTextLanguage();

        language.ShouldBe(Language.English);
    }

    /// <summary>
    /// Verifies a theme mode save preserves unrelated settings.
    /// </summary>
    [Fact]
    public void SetThemeMode_PreservesActiveGameAndExistingSettings()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration
            {
                ActiveGame = "Skyrim",
                ThemeFamily = ApplicationThemeFamily.Fluent,
                RecordTextLanguage = "German",
                NifSkopeExecutablePath = "nifskope.exe",
                PreferEspOverMatchingEsm = false,
                ApplicationDataDirectory = "app-data",
                DatabaseDirectory = "database",
                LoggingDirectory = "logs"
            }
        };
        var service = new ApplicationSettingsService(store);

        service.SetThemeMode(ApplicationThemeMode.Light);

        store.Current.ActiveGame.ShouldBe("Skyrim");
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Light);
        store.Current.RecordTextLanguage.ShouldBe("German");
        store.Current.NifSkopeExecutablePath.ShouldBe("nifskope.exe");
        store.Current.PreferEspOverMatchingEsm.ShouldBeFalse();
        store.Current.ApplicationDataDirectory.ShouldBe("app-data");
        store.Current.DatabaseDirectory.ShouldBe("database");
        store.Current.LoggingDirectory.ShouldBe("logs");
    }

    /// <summary>
    /// Verifies a theme family save preserves unrelated settings.
    /// </summary>
    [Fact]
    public void SetThemeFamily_PreservesActiveGameThemeModeAndExistingSettings()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration
            {
                ActiveGame = "Skyrim",
                ThemeMode = ApplicationThemeMode.Light,
                RecordTextLanguage = "German",
                NifSkopeExecutablePath = "nifskope.exe",
                PreferEspOverMatchingEsm = false,
                ApplicationDataDirectory = "app-data",
                DatabaseDirectory = "database",
                LoggingDirectory = "logs"
            }
        };
        var service = new ApplicationSettingsService(store);

        service.SetThemeFamily(ApplicationThemeFamily.Fluent);

        store.Current.ActiveGame.ShouldBe("Skyrim");
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Light);
        store.Current.RecordTextLanguage.ShouldBe("German");
        store.Current.NifSkopeExecutablePath.ShouldBe("nifskope.exe");
        store.Current.PreferEspOverMatchingEsm.ShouldBeFalse();
        store.Current.ApplicationDataDirectory.ShouldBe("app-data");
        store.Current.DatabaseDirectory.ShouldBe("database");
        store.Current.LoggingDirectory.ShouldBe("logs");
    }

    /// <summary>
    /// Verifies theme saves preserve unrelated settings.
    /// </summary>
    [Fact]
    public void SetTheme_PreservesActiveGameAndExistingSettings()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration
            {
                ActiveGame = "Fallout4",
                RecordTextLanguage = "German",
                NifSkopeExecutablePath = "nifskope.exe",
                PreferEspOverMatchingEsm = false,
                ApplicationDataDirectory = "app-data",
                DatabaseDirectory = "database",
                LoggingDirectory = "logs"
            }
        };
        var service = new ApplicationSettingsService(store);

        service.SetTheme(ApplicationThemeFamily.Fluent, ApplicationThemeMode.Light);

        store.Current.ActiveGame.ShouldBe("Fallout4");
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Light);
        store.Current.RecordTextLanguage.ShouldBe("German");
        store.Current.NifSkopeExecutablePath.ShouldBe("nifskope.exe");
        store.Current.PreferEspOverMatchingEsm.ShouldBeFalse();
        store.Current.ApplicationDataDirectory.ShouldBe("app-data");
        store.Current.DatabaseDirectory.ShouldBe("database");
        store.Current.LoggingDirectory.ShouldBe("logs");
    }

    /// <summary>
    /// Verifies NifSkope path saves trim user input.
    /// </summary>
    [Fact]
    public void SetThemeRecordTextLanguageNifSkopeExecutablePathAndPluginSelectionPreference_SavesTrimmedPath()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration
            {
                ActiveGame = "Starfield",
                ApplicationDataDirectory = "app-data",
                DatabaseDirectory = "database",
                LoggingDirectory = "logs"
            }
        };
        var service = new ApplicationSettingsService(store);

        service.SetThemeRecordTextLanguageNifSkopeExecutablePathAndPluginSelectionPreference(ApplicationThemeFamily.Fluent, ApplicationThemeMode.Light, Language.German, "  nifskope.exe  ", false);

        store.Current.ActiveGame.ShouldBe("Starfield");
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Light);
        store.Current.RecordTextLanguage.ShouldBe("German");
        store.Current.NifSkopeExecutablePath.ShouldBe("nifskope.exe");
        store.Current.PreferEspOverMatchingEsm.ShouldBeFalse();
        store.Current.ApplicationDataDirectory.ShouldBe("app-data");
        store.Current.DatabaseDirectory.ShouldBe("database");
        store.Current.LoggingDirectory.ShouldBe("logs");
    }

    /// <summary>
    /// Verifies blank NifSkope paths are saved as null.
    /// </summary>
    [Fact]
    public void SetThemeAndNifSkopeExecutablePath_BlankPathSavesNull()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration
            {
                ActiveGame = "Fallout4",
                RecordTextLanguage = "German",
                PreferEspOverMatchingEsm = false,
                ApplicationDataDirectory = "app-data",
                DatabaseDirectory = "database",
                LoggingDirectory = "logs"
            }
        };
        var service = new ApplicationSettingsService(store);

        service.SetThemeAndNifSkopeExecutablePath(ApplicationThemeFamily.Fluent, ApplicationThemeMode.Light, " ");

        store.Current.ActiveGame.ShouldBe("Fallout4");
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Light);
        store.Current.RecordTextLanguage.ShouldBe("German");
        store.Current.NifSkopeExecutablePath.ShouldBeNull();
        store.Current.PreferEspOverMatchingEsm.ShouldBeFalse();
        store.Current.ApplicationDataDirectory.ShouldBe("app-data");
        store.Current.DatabaseDirectory.ShouldBe("database");
        store.Current.LoggingDirectory.ShouldBe("logs");
    }

    /// <summary>
    /// Verifies the combined active-game and settings save updates every intended value.
    /// </summary>
    [Fact]
    public void SetActiveGameThemeRecordTextLanguageNifSkopeExecutablePathAndPluginSelectionPreference_SavesAllSettings()
    {
        var store = new TestApplicationConfigurationStore
        {
            Current = new ApplicationConfiguration
            {
                ApplicationDataDirectory = "app-data",
                DatabaseDirectory = "database",
                LoggingDirectory = "logs"
            }
        };
        var service = new ApplicationSettingsService(store);

        service.SetActiveGameThemeRecordTextLanguageNifSkopeExecutablePathAndPluginSelectionPreference(SupportedGame.Skyrim, ApplicationThemeFamily.Fluent, ApplicationThemeMode.Light, Language.German, "nifskope.exe", false);

        store.Current.ActiveGame.ShouldBe("Skyrim");
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Light);
        store.Current.RecordTextLanguage.ShouldBe("German");
        store.Current.NifSkopeExecutablePath.ShouldBe("nifskope.exe");
        store.Current.PreferEspOverMatchingEsm.ShouldBeFalse();
        store.Current.ApplicationDataDirectory.ShouldBe("app-data");
        store.Current.DatabaseDirectory.ShouldBe("database");
        store.Current.LoggingDirectory.ShouldBe("logs");
    }

    /// <summary>
    /// Provides in-memory configuration state for application settings service tests.
    /// </summary>
    private sealed class TestApplicationConfigurationStore : IApplicationConfigurationStore
    {
        /// <inheritdoc />
        public string ConfigurationPath => "test.json";

        /// <inheritdoc />
        public ApplicationConfiguration Current { get; set; } = new();

        /// <inheritdoc />
        public void Load()
        { }

        /// <inheritdoc />
        public void Save(ApplicationConfiguration configuration)
        {
            Current = configuration;
        }
    }
}
