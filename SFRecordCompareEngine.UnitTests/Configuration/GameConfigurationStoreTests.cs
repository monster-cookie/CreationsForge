using SFRecordCompareEngine.Core.Configuration;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Configuration;

public class GameConfigurationStoreTests
{
    [Fact]
    public void Constructor_ExposesSupportedGames()
    {
        var sut = new GameConfigurationStore();

        sut.SupportedGames.ShouldBe(["None", "Starfield", "Skyrim", "Fallout 4"]);
    }

    [Fact]
    public void SelectGame_WhenGameIsNone_ClearsActiveGame()
    {
        var sut = new GameConfigurationStore();
        sut.SelectGame("Starfield");

        sut.SelectGame("None");

        sut.SelectedGame.ShouldBeNull();
        sut.Game.ShouldBeNull();
    }

    [Fact]
    public void SelectGame_WhenGameIsStarfield_SetsSelectedGameAndEnvironment()
    {
        var sut = new GameConfigurationStore();

        sut.SelectGame("Starfield");

        sut.SelectedGame.ShouldBe("Starfield");
        sut.Game.ShouldNotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Oblivion")]
    public void SelectGame_WhenGameIsUnsupported_ClearsActiveGame(string? game)
    {
        var sut = new GameConfigurationStore();
        sut.SelectGame("Starfield");

        sut.SelectGame(game);

        sut.SelectedGame.ShouldBeNull();
        sut.Game.ShouldBeNull();
    }

    [Fact]
    public void ClearActiveGame_ClearsSelectedGameAndEnvironment()
    {
        var sut = new GameConfigurationStore();
        sut.SelectGame("Starfield");

        sut.ClearActiveGame();

        sut.SelectedGame.ShouldBeNull();
        sut.Game.ShouldBeNull();
    }
}
