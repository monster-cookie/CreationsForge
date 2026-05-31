using SFRecordCompareEngine.Core.Configuration;
using SFRecordCompareEngine.Core.Models.Configuration;
using SFRecordCompareEngine.Core.Models.Database;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Configuration;

public class ApplicationConfigurationStoreTests : IDisposable
{
    private readonly string TestDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));
    private readonly string ConfigurationPath;

    public ApplicationConfigurationStoreTests()
    {
        ConfigurationPath = Path.Combine(TestDirectory, "SFRecordCompareEngine.Config.json");
    }

    public void Dispose()
    {
        if (Directory.Exists(TestDirectory))
        {
            Directory.Delete(TestDirectory, true);
        }
    }

    [Fact]
    public void Constructor_UsesProgramDataConfigurationPath()
    {
        var sut = new ApplicationConfigurationStore();

        var expectedDirectory = Path.Combine(ApplicationConfigurationStore.DefaultApplicationDataDirectory, "SFRecordCompareEngine.Config.json");
        sut.ConfigurationPath.ShouldBe(expectedDirectory);
    }

    [Fact]
    public void SqliteDatabaseOptions_UsesDefaultApplicationConfigurationDirectory()
    {
        var sut = new SqliteDatabaseOptions();

        sut.DatabaseDirectory.ShouldBe(ApplicationConfigurationStore.DefaultApplicationDataDirectory);
        sut.DatabasePath.ShouldBe(Path.Combine(ApplicationConfigurationStore.DefaultApplicationDataDirectory, "SFRecordCompareEngine.sqlite"));
        sut.LogDirectory.ShouldBe(Path.Combine(ApplicationConfigurationStore.DefaultApplicationDataDirectory, "Logs"));
    }

    [Fact]
    public void Save_WritesConfigurationAndUpdatesCurrent()
    {
        var sut = new ApplicationConfigurationStore(ConfigurationPath);

        sut.Save(new ApplicationConfiguration
        {
            Theme = ApplicationThemeMode.Light,
            ApplicationDataDirectory = "C:\\Temp\\SFRecordCompareEngine",
            DatabaseDirectory = "C:\\Temp\\SFRecordCompareEngine\\Database",
            LoggingDirectory = "C:\\Temp\\SFRecordCompareEngine\\Logs"
        });

        sut.Current.Theme.ShouldBe(ApplicationThemeMode.Light);
        sut.Current.ApplicationDataDirectory.ShouldBe("C:\\Temp\\SFRecordCompareEngine");
        sut.Current.DatabaseDirectory.ShouldBe("C:\\Temp\\SFRecordCompareEngine\\Database");
        sut.Current.LoggingDirectory.ShouldBe("C:\\Temp\\SFRecordCompareEngine\\Logs");
        File.Exists(ConfigurationPath).ShouldBeTrue();

        var reloaded = new ApplicationConfigurationStore(ConfigurationPath);
        reloaded.Current.Theme.ShouldBe(ApplicationThemeMode.Light);
        reloaded.Current.ApplicationDataDirectory.ShouldBe("C:\\Temp\\SFRecordCompareEngine");
        reloaded.Current.DatabaseDirectory.ShouldBe("C:\\Temp\\SFRecordCompareEngine\\Database");
        reloaded.Current.LoggingDirectory.ShouldBe("C:\\Temp\\SFRecordCompareEngine\\Logs");
    }

    [Fact]
    public void Constructor_WhenConfigurationFileIsValid_LoadsConfiguration()
    {
        Directory.CreateDirectory(TestDirectory);
        File.WriteAllText(ConfigurationPath, """
            {
              "ApplicationDataDirectory": "C:\\Temp\\SFRecordCompareEngine",
              "DatabaseDirectory": "C:\\Temp\\SFRecordCompareEngine\\Database",
              "LoggingDirectory": "C:\\Temp\\SFRecordCompareEngine\\Logs"
            }
            """);

        var sut = new ApplicationConfigurationStore(ConfigurationPath);

        sut.Current.Theme.ShouldBe(ApplicationThemeMode.Dark);
        sut.Current.ApplicationDataDirectory.ShouldBe("C:\\Temp\\SFRecordCompareEngine");
        sut.Current.DatabaseDirectory.ShouldBe("C:\\Temp\\SFRecordCompareEngine\\Database");
        sut.Current.LoggingDirectory.ShouldBe("C:\\Temp\\SFRecordCompareEngine\\Logs");
    }

    [Fact]
    public void Constructor_WhenConfigurationFileIncludesTheme_LoadsTheme()
    {
        Directory.CreateDirectory(TestDirectory);
        File.WriteAllText(ConfigurationPath, """
            {
              "Theme": "Light"
            }
            """);

        var sut = new ApplicationConfigurationStore(ConfigurationPath);

        sut.Current.Theme.ShouldBe(ApplicationThemeMode.Light);
    }
}
