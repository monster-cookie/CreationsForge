using System.Text.Json;
using System.Text.Json.Serialization;
using CreationsForge.Core.Configuration;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using Shouldly;

namespace CreationsForge.UnitTests.Configuration;

/// <summary>
/// Verifies durable application configuration defaults, compatibility, and persistence.
/// </summary>
public class ApplicationConfigurationStoreTests : IDisposable
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string DirectoryPath = Path.Combine(Path.GetTempPath(), "CreationsForge.Tests", Guid.NewGuid().ToString("N"));

    [Fact]
    public void Load_WhenConfigurationFileDoesNotExist_LoadsDefaultConfiguration()
    {
        var configurationPath = Path.Combine(DirectoryPath, "Missing.Config.json");

        var store = new ApplicationConfigurationStore(configurationPath);

        store.Current.ActiveGame.ShouldBeNull();
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Semi);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Dark);
        store.Current.NifSkopeExecutablePath.ShouldBeNull();
        store.Current.PreferEspOverMatchingEsm.ShouldBeTrue();
    }

    [Fact]
    public void Load_WhenConfigurationFileIsEmpty_LoadsDefaultConfiguration()
    {
        var configurationPath = CreateConfigurationFile(string.Empty);

        var store = new ApplicationConfigurationStore(configurationPath);

        store.Current.ActiveGame.ShouldBeNull();
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Semi);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Dark);
        store.Current.NifSkopeExecutablePath.ShouldBeNull();
        store.Current.PreferEspOverMatchingEsm.ShouldBeTrue();
    }

    [Fact]
    public void Load_WhenConfigurationFileContainsInvalidJson_LoadsDefaultConfiguration()
    {
        var configurationPath = CreateConfigurationFile("{not json");

        var store = new ApplicationConfigurationStore(configurationPath);

        store.Current.ActiveGame.ShouldBeNull();
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Semi);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Dark);
        store.Current.NifSkopeExecutablePath.ShouldBeNull();
        store.Current.PreferEspOverMatchingEsm.ShouldBeTrue();
    }

    [Fact]
    public void Save_WritesConfigurationAndUpdatesCurrentConfiguration()
    {
        var configurationPath = Path.Combine(DirectoryPath, "Saved.Config.json");
        var store = new ApplicationConfigurationStore(configurationPath);
        var configuration = new ApplicationConfiguration
        {
            ActiveGame = nameof(SupportedGame.Starfield),
            ThemeFamily = ApplicationThemeFamily.Fluent,
            ThemeMode = ApplicationThemeMode.Light,
            NifSkopeExecutablePath = @"C:\Tools\NifSkope.exe",
            PreferEspOverMatchingEsm = false
        };

        store.Save(configuration);

        store.Current.ShouldBeSameAs(configuration);
        var saved = JsonSerializer.Deserialize<ApplicationConfiguration>(File.ReadAllText(configurationPath), SerializerOptions);
        saved.ShouldNotBeNull();
        saved.ActiveGame.ShouldBe(nameof(SupportedGame.Starfield));
        saved.ThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        saved.ThemeMode.ShouldBe(ApplicationThemeMode.Light);
        saved.NifSkopeExecutablePath.ShouldBe(@"C:\Tools\NifSkope.exe");
        saved.PreferEspOverMatchingEsm.ShouldBeFalse();
    }

    [Fact]
    public void Load_WhenConfigurationFileDoesNotContainThemeFields_DefaultsToSemiDark()
    {
        var configurationPath = CreateConfigurationFile("""{"ActiveGame":"Fallout4"}""");

        var store = new ApplicationConfigurationStore(configurationPath);

        store.Current.ActiveGame.ShouldBe("Fallout4");
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Semi);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Dark);
        store.Current.NifSkopeExecutablePath.ShouldBeNull();
        store.Current.PreferEspOverMatchingEsm.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies an obsolete database-directory value is ignored while retained settings survive a load and save.
    /// </summary>
    [Fact]
    public void Load_WithLegacyDatabaseDirectory_IgnoresItAndPreservesRetainedSettings()
    {
        var legacyDatabaseDirectory = Path.Combine(DirectoryPath, "LegacyDatabase");
        var configurationPath = CreateConfigurationFile(JsonSerializer.Serialize(new
        {
            ActiveGame = "Skyrim",
            ThemeFamily = "Fluent",
            ThemeMode = "Light",
            RecordTextLanguage = "German",
            NifSkopeExecutablePath = "nifskope.exe",
            PreferEspOverMatchingEsm = false,
            ApplicationDataDirectory = "app-data",
            DatabaseDirectory = legacyDatabaseDirectory,
            LoggingDirectory = "logs"
        }));
        var store = new ApplicationConfigurationStore(configurationPath);

        store.Save(store.Current);

        store.Current.ActiveGame.ShouldBe("Skyrim");
        store.Current.ThemeFamily.ShouldBe(ApplicationThemeFamily.Fluent);
        store.Current.ThemeMode.ShouldBe(ApplicationThemeMode.Light);
        store.Current.RecordTextLanguage.ShouldBe("German");
        store.Current.NifSkopeExecutablePath.ShouldBe("nifskope.exe");
        store.Current.PreferEspOverMatchingEsm.ShouldBeFalse();
        store.Current.ApplicationDataDirectory.ShouldBe("app-data");
        store.Current.LoggingDirectory.ShouldBe("logs");
        File.ReadAllText(configurationPath).ShouldNotContain("DatabaseDirectory");
        Directory.Exists(legacyDatabaseDirectory).ShouldBeFalse();
    }

    public void Dispose()
    {
        if (Directory.Exists(DirectoryPath))
        {
            Directory.Delete(DirectoryPath, true);
        }
    }

    private string CreateConfigurationFile(string contents)
    {
        Directory.CreateDirectory(DirectoryPath);
        var configurationPath = Path.Combine(DirectoryPath, "CreationsForge.Config.json");
        File.WriteAllText(configurationPath, contents);
        return configurationPath;
    }
}
