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
    public void SelectedGame_CanBeAssigned()
    {
        var sut = new ApplicationConfiguration
        {
            SelectedGame = "Starfield"
        };

        sut.SelectedGame.ShouldBe("Starfield");
    }
}
