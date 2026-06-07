using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class PluginSelectionServiceTests
{
    [Fact]
    public void SearchOpenablePluginsByFilename_WithBlankSearch_ReturnsOpenablePlugins()
    {
        var openablePlugins = new List<PluginDTO> { CreatePlugin("Example.esm") };
        var repository = new TestPluginRepository { OpenablePlugins = openablePlugins };
        var service = new PluginSelectionService(repository);

        var plugins = service.SearchOpenablePluginsByFilename(SupportedGame.Starfield, string.Empty);

        plugins.ShouldBe(openablePlugins);
        repository.OpenableGame.ShouldBe(SupportedGame.Starfield);
        repository.SearchGame.ShouldBeNull();
    }

    [Fact]
    public void SearchOpenablePluginsByFilename_WithSearchText_ReturnsSearchResults()
    {
        var searchResults = new List<PluginDTO> { CreatePlugin("SearchResult.esm") };
        var repository = new TestPluginRepository { SearchResults = searchResults };
        var service = new PluginSelectionService(repository);

        var plugins = service.SearchOpenablePluginsByFilename(SupportedGame.Fallout4, "search");

        plugins.ShouldBe(searchResults);
        repository.SearchGame.ShouldBe(SupportedGame.Fallout4);
        repository.SearchFilename.ShouldBe("search");
    }

    [Fact]
    public void GetImportedRecordCount_ReturnsRepositoryCount()
    {
        var repository = new TestPluginRepository { ImportedRecordCount = 42 };
        var service = new PluginSelectionService(repository);

        var count = service.GetImportedRecordCount(SupportedGame.Skyrim);

        count.ShouldBe(42);
        repository.RecordCountGame.ShouldBe(SupportedGame.Skyrim);
    }

    private static PluginDTO CreatePlugin(string fileName)
    {
        return new PluginDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = new ModKeyDTO
            {
                Name = Path.GetFileNameWithoutExtension(fileName),
                Type = 0,
                FileName = fileName
            },
            LoadOrderIndex = 0,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current,
            HeaderFlags = 0,
            FormVersion = 0,
            RecordCount = 1,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow
        };
    }

    private sealed class TestPluginRepository : IPluginRepository
    {
        public IReadOnlyList<PluginDTO> OpenablePlugins { get; set; } = [];

        public IReadOnlyList<PluginDTO> SearchResults { get; set; } = [];

        public long ImportedRecordCount { get; set; }

        public SupportedGame? OpenableGame { get; private set; }

        public SupportedGame? SearchGame { get; private set; }

        public string? SearchFilename { get; private set; }

        public SupportedGame? RecordCountGame { get; private set; }

        public int CountByGame(SupportedGame game)
        {
            return 0;
        }

        public long GetImportedRecordCountByGame(SupportedGame game)
        {
            RecordCountGame = game;
            return ImportedRecordCount;
        }

        public PluginDTO? GetByModKey(SupportedGame game, ModKeyDTO modKey)
        {
            return null;
        }

        public IReadOnlyList<PluginDTO> GetOpenablePlugins(SupportedGame game)
        {
            OpenableGame = game;
            return OpenablePlugins;
        }

        public IReadOnlyList<PluginDTO> SearchOpenablePluginsByFilename(SupportedGame game, string searchFilename)
        {
            SearchGame = game;
            SearchFilename = searchFilename;
            return SearchResults;
        }

        public void Save(PluginDTO dto)
        { }
    }
}
