using SFRecordCompareEngine.Core.Configuration;
using SFRecordCompareEngine.Core.Models.Configuration;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Models.Configuration;

public class ApplicationConfigurationTests
{
    [Fact]
    public void Constructor_DefaultsThemeToDark()
    {
        var sut = new ApplicationConfiguration();

        sut.Theme.ShouldBe(ApplicationThemeMode.Dark);
    }

    [Fact]
    public void Constructor_DefaultsApplicationDirectory()
    {
        var sut = new ApplicationConfiguration();

        sut.ApplicationDataDirectory.ShouldBe(ApplicationConfigurationStore.DefaultApplicationDataDirectory);
    }

    [Fact]
    public void Constructor_DefaultsDatabaseDirectory()
    {
        var sut = new ApplicationConfiguration();

        sut.DatabaseDirectory.ShouldBe(ApplicationConfigurationStore.DefaultDatabaseDirectory);
    }

    [Fact]
    public void Constructor_DefaultsLoggingDirectory()
    {
        var sut = new ApplicationConfiguration();

        sut.LoggingDirectory.ShouldBe(ApplicationConfigurationStore.DefaultLoggingDirectory);
    }
}
