using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class GameImportReadinessServiceTests
{
    [Fact]
    public void HasImportedData_WhenPluginRowsExist_ReturnsTrue()
    {
        var repository = new TestPluginRepository { Count = 1 };
        var service = new GameImportReadinessService(repository);

        service.HasImportedData(SupportedGame.Starfield).ShouldBeTrue();
        repository.CountedGame.ShouldBe(SupportedGame.Starfield);
    }

    [Fact]
    public void HasImportedData_WhenNoPluginRowsExist_ReturnsFalse()
    {
        var repository = new TestPluginRepository { Count = 0 };
        var service = new GameImportReadinessService(repository);

        service.HasImportedData(SupportedGame.Fallout4).ShouldBeFalse();
        repository.CountedGame.ShouldBe(SupportedGame.Fallout4);
    }

    private sealed class TestPluginRepository : IPluginRepository
    {
        public int Count { get; set; }

        public SupportedGame? CountedGame { get; private set; }

        public int CountByGame(SupportedGame game)
        {
            CountedGame = game;
            return Count;
        }

        public long GetImportedRecordCountByGame(SupportedGame game)
        {
            return 0;
        }

        public PluginDTO? GetByModKey(SupportedGame game, ModKeyDTO modKey)
        {
            return null;
        }

        public IReadOnlyList<PluginDTO> GetOpenablePlugins(SupportedGame game)
        {
            return [];
        }

        public IReadOnlyList<PluginDTO> SearchOpenablePluginsByFilename(SupportedGame game, string searchFilename)
        {
            return [];
        }

        public void Save(PluginDTO dto)
        { }
    }
}
