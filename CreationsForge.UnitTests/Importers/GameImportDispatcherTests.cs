using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Importers.Interfaces;
using Shouldly;

namespace CreationsForge.UnitTests.Importers;

public class GameImportDispatcherTests
{
    [Fact]
    public void Import_WithRegisteredGame_DispatchesToMatchingImporter()
    {
        var importer = new TestGameImporter(SupportedGame.Fallout4);
        var dispatcher = new GameImportDispatcher([importer]);

        var result = dispatcher.Import(SupportedGame.Fallout4);

        result.Game.ShouldBe(SupportedGame.Fallout4);
        importer.ImportWasCalled.ShouldBeTrue();
    }

    [Fact]
    public void Import_WithoutRegisteredGame_Throws()
    {
        var dispatcher = new GameImportDispatcher([new TestGameImporter(SupportedGame.Starfield)]);

        Should.Throw<InvalidOperationException>(() => dispatcher.Import(SupportedGame.Skyrim))
            .Message.ShouldContain("No importer is registered");
    }

    private sealed class TestGameImporter : IGameImporter
    {
        public TestGameImporter(SupportedGame game)
        {
            Game = game;
        }

        public SupportedGame Game { get; }

        public bool ImportWasCalled { get; private set; }

        public GameImportResultDTO Import(bool forceFullReimport = false, IProgress<GameImportProgressDTO>? progress = null, CancellationToken cancellationToken = default)
        {
            ImportWasCalled = true;
            return new GameImportResultDTO { Game = Game };
        }
    }
}
