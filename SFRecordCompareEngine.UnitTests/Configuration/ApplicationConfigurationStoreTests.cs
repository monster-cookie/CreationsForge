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
        ConfigurationPath = Path.Combine(TestDirectory, "SFRecordCompareEngine.config.json");
    }

    public void Dispose()
    {
        if (Directory.Exists(TestDirectory))
        {
            Directory.Delete(TestDirectory, true);
        }
    }

    [Fact]
    public void Constructor_WhenConfigurationFileIsMissing_RequiresConfiguration()
    {
        var sut = new ApplicationConfigurationStore(ConfigurationPath);

        sut.IsConfigurationRequired.ShouldBeTrue();
        sut.Current.SelectedGame.ShouldBeNull();
    }

    [Fact]
    public void Constructor_UsesProgramDataConfigurationPath()
    {
        var sut = new ApplicationConfigurationStore();

        sut.ConfigurationPath.ShouldBe(Path.Combine(ApplicationConfigurationStore.DefaultApplicationDataDirectory, "SFRecordCompareEngine.config.json"));
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
    public void Constructor_WhenConfigurationFileIsEmpty_RequiresConfiguration()
    {
        Directory.CreateDirectory(TestDirectory);
        File.WriteAllText(ConfigurationPath, string.Empty);

        var sut = new ApplicationConfigurationStore(ConfigurationPath);

        sut.IsConfigurationRequired.ShouldBeTrue();
    }

    [Fact]
    public void Save_WritesConfigurationAndUpdatesCurrent()
    {
        var sut = new ApplicationConfigurationStore(ConfigurationPath);

        sut.Save(new ApplicationConfiguration
        {
            SelectedGame = "Starfield"
        });

        sut.IsConfigurationRequired.ShouldBeFalse();
        sut.Current.SelectedGame.ShouldBe("Starfield");
        File.Exists(ConfigurationPath).ShouldBeTrue();
    }

    [Fact]
    public void Constructor_WhenConfigurationFileIsValid_LoadsConfiguration()
    {
        Directory.CreateDirectory(TestDirectory);
        File.WriteAllText(ConfigurationPath, """
            {
              "SelectedGame": "Starfield"
            }
            """);

        var sut = new ApplicationConfigurationStore(ConfigurationPath);

        sut.IsConfigurationRequired.ShouldBeFalse();
        sut.Current.SelectedGame.ShouldBe("Starfield");
    }
}
