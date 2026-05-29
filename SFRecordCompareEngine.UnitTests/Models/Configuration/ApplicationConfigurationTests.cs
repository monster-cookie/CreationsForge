using SFRecordCompareEngine.Core.Models.Configuration;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Models.Configuration;

public class ApplicationConfigurationTests
{
    [Fact]
    public void Constructor_DefaultsSelectedGameToNull()
    {
        var sut = new ApplicationConfiguration();

        sut.SelectedGame.ShouldBeNull();
    }

    [Fact]
    public void Constructor_DefaultsThemeToDark()
    {
        var sut = new ApplicationConfiguration();

        sut.Theme.ShouldBe(ApplicationThemeMode.Dark);
    }

    [Fact]
    public void SelectedGame_CanBeAssigned()
    {
        var sut = new ApplicationConfiguration
        {
            SelectedGame = "Starfield"
        };

        sut.SelectedGame.ShouldBe("Starfield");
    }

    [Fact]
    public void Theme_CanBeAssigned()
    {
        var sut = new ApplicationConfiguration
        {
            Theme = ApplicationThemeMode.Light
        };

        sut.Theme.ShouldBe(ApplicationThemeMode.Light);
    }
}
