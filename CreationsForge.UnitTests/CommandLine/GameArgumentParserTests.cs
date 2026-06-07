using CreationsForge.Console.CommandLine;
using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using Shouldly;

namespace CreationsForge.UnitTests.CommandLine;

public class GameArgumentParserTests
{
    [Fact]
    public void Parse_WithLongGameArgument_ReturnsGameAndStoresActiveGame()
    {
        var store = new TestConfigurationStore();
        var parser = new GameArgumentParser(store);

        var result = parser.Parse(["--game", "Starfield"]);

        result.IsSuccess.ShouldBeTrue();
        result.Game.ShouldBe(SupportedGame.Starfield);
        result.ForceFullReimport.ShouldBeFalse();
        store.Current.ActiveGame.ShouldBe(nameof(SupportedGame.Starfield));
    }

    [Fact]
    public void Parse_WithForceArgument_ReturnsForceFullReimport()
    {
        var parser = new GameArgumentParser(new TestConfigurationStore());

        var result = parser.Parse(["--game", "Starfield", "--force"]);

        result.IsSuccess.ShouldBeTrue();
        result.Game.ShouldBe(SupportedGame.Starfield);
        result.ForceFullReimport.ShouldBeTrue();
    }

    [Fact]
    public void Parse_WithFullArgument_ReturnsForceFullReimport()
    {
        var parser = new GameArgumentParser(new TestConfigurationStore());

        var result = parser.Parse(["--full", "--game", "Starfield"]);

        result.IsSuccess.ShouldBeTrue();
        result.Game.ShouldBe(SupportedGame.Starfield);
        result.ForceFullReimport.ShouldBeTrue();
    }

    [Fact]
    public void Parse_WithResetAllArgument_ReturnsResetAllWithoutRequiringGame()
    {
        var parser = new GameArgumentParser(new TestConfigurationStore());

        var result = parser.Parse(["--reset-all"]);

        result.IsSuccess.ShouldBeTrue();
        result.ResetAll.ShouldBeTrue();
        result.Game.ShouldBeNull();
        result.ForceFullReimport.ShouldBeTrue();
    }

    [Fact]
    public void Parse_WithShortGameArgument_ReturnsGame()
    {
        var parser = new GameArgumentParser(new TestConfigurationStore());

        var result = parser.Parse(["-g", "Fallout4"]);

        result.IsSuccess.ShouldBeTrue();
        result.Game.ShouldBe(SupportedGame.Fallout4);
    }

    [Fact]
    public void Parse_WithSingleDashGameArgument_ReturnsGame()
    {
        var parser = new GameArgumentParser(new TestConfigurationStore());

        var result = parser.Parse(["-game", "Skyrim"]);

        result.IsSuccess.ShouldBeTrue();
        result.Game.ShouldBe(SupportedGame.Skyrim);
    }

    [Fact]
    public void Parse_WithoutArgument_UsesStoredActiveGame()
    {
        var store = new TestConfigurationStore
        {
            Current = new ApplicationConfiguration { ActiveGame = nameof(SupportedGame.Skyrim) }
        };
        var parser = new GameArgumentParser(store);

        var result = parser.Parse([]);

        result.IsSuccess.ShouldBeTrue();
        result.Game.ShouldBe(SupportedGame.Skyrim);
    }

    [Fact]
    public void Parse_WithoutArgumentOrStoredGame_ReturnsFailure()
    {
        var parser = new GameArgumentParser(new TestConfigurationStore());

        var result = parser.Parse([]);

        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("A game is required");
    }

    [Fact]
    public void Parse_WithUnsupportedGame_ReturnsFailure()
    {
        var parser = new GameArgumentParser(new TestConfigurationStore());

        var result = parser.Parse(["--game", "Oblivion"]);

        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNull();
        result.ErrorMessage.ShouldContain("Unsupported game");
    }

    private sealed class TestConfigurationStore : IApplicationConfigurationStore
    {
        public string ConfigurationPath => "Test.Config.json";

        public ApplicationConfiguration Current { get; set; } = new();

        public void Load()
        { }

        public void Save(ApplicationConfiguration configuration)
        {
            Current = configuration;
        }
    }
}
