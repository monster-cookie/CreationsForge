using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class RecordTreeServiceTests
{
    [Fact]
    public void GetRecordTreeEntries_AggregatesSharedRecordTypesInOrder()
    {
        var modKey = CreateModKey("Example", "Example.esm");
        var formListEntries = new List<RecordTreeEntryDTO> { CreateEntry("FLST", "FormListEditorID") };
        var gameSettingEntries = new List<RecordTreeEntryDTO> { CreateEntry("GMST", "GameSettingEditorID") };
        var globalEntries = new List<RecordTreeEntryDTO> { CreateEntry("GLOB", "GlobalEditorID") };
        var service = new RecordTreeService([
            new TestFormListRepository { Entries = formListEntries },
            new TestGameSettingRepository { Entries = gameSettingEntries },
            new TestGlobalRepository { Entries = globalEntries }]);

        var entries = service.GetRecordTreeEntries(SupportedGame.Starfield, modKey);

        entries.ShouldBe([formListEntries[0], gameSettingEntries[0], globalEntries[0]]);
    }

    [Fact]
    public void GetRecordTreeEntries_PassesGameAndModKeyToRepositories()
    {
        var modKey = CreateModKey("Example", "Example.esm");
        var formListRepository = new TestFormListRepository();
        var gameSettingRepository = new TestGameSettingRepository();
        var globalRepository = new TestGlobalRepository();
        var service = new RecordTreeService([formListRepository, gameSettingRepository, globalRepository]);

        service.GetRecordTreeEntries(SupportedGame.Fallout4, modKey);

        formListRepository.Request.ShouldBe((SupportedGame.Fallout4, modKey));
        gameSettingRepository.Request.ShouldBe((SupportedGame.Fallout4, modKey));
        globalRepository.Request.ShouldBe((SupportedGame.Fallout4, modKey));
    }

    [Fact]
    public void GetRecordTreeEntries_UsesRepositoryProvidedPluginCounts()
    {
        var modKey = CreateModKey("Example", "Example.esm");
        var formListEntry = CreateEntry("FLST", "FormListEditorID", 123, 2);
        var gameSettingEntry = CreateEntry("GMST", "GameSettingEditorID", 234, 3);
        var globalEntry = CreateEntry("GLOB", "GlobalEditorID", 345, 4);
        var service = new RecordTreeService([
            new TestFormListRepository
            {
                Entries = [formListEntry],
                PluginCounts = new Dictionary<string, int> { [GetFormKeyKey(formListEntry.FormKey)] = 2 }
            },
            new TestGameSettingRepository
            {
                Entries = [gameSettingEntry],
                PluginCounts = new Dictionary<string, int> { [GetFormKeyKey(gameSettingEntry.FormKey)] = 3 }
            },
            new TestGlobalRepository
            {
                Entries = [globalEntry],
                PluginCounts = new Dictionary<string, int> { [GetFormKeyKey(globalEntry.FormKey)] = 4 }
            }]);

        var entries = service.GetRecordTreeEntries(SupportedGame.Starfield, modKey);

        entries[0].PluginCount.ShouldBe(2);
        entries[1].PluginCount.ShouldBe(3);
        entries[2].PluginCount.ShouldBe(4);
    }

    [Fact]
    public void GetRecordTreeEntries_DoesNotRequestFullGamePluginCounts()
    {
        var modKey = CreateModKey("Example", "Example.esm");
        var formListEntry = CreateEntry("FLST", "FormListEditorID", pluginCount: 5);
        var formListRepository = new TestFormListRepository { Entries = [formListEntry] };
        var gameSettingRepository = new TestGameSettingRepository();
        var globalRepository = new TestGlobalRepository();
        var service = new RecordTreeService([
            formListRepository,
            gameSettingRepository,
            globalRepository]);

        var entries = service.GetRecordTreeEntries(SupportedGame.Starfield, modKey);

        entries.Single().PluginCount.ShouldBe(5);
        formListRepository.PluginCountRequestCount.ShouldBe(0);
        gameSettingRepository.PluginCountRequestCount.ShouldBe(0);
        globalRepository.PluginCountRequestCount.ShouldBe(0);
    }

    private static RecordTreeEntryDTO CreateEntry(string recordType, string editorId, uint formKeyId = 123, int pluginCount = 0)
    {
        return new RecordTreeEntryDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey("Example", "Example.esm"),
            FormKey = new FormKeyDTO
            {
                ModKey = CreateModKey("Example", "Example.esm"),
                Id = formKeyId
            },
            EditorID = editorId,
            RecordType = recordType,
            PluginCount = pluginCount
        };
    }

    private static ModKeyDTO CreateModKey(string name, string fileName)
    {
        return new ModKeyDTO
        {
            Name = name,
            Type = 0,
            FileName = fileName
        };
    }

    private static string GetFormKeyKey(FormKeyDTO formKey)
    {
        return $"{formKey.ModKey.Name}|{formKey.ModKey.Type}|{formKey.ModKey.FileName}|{formKey.Id}";
    }

    private sealed class TestFormListRepository : IFormListRepository, IRecordTreeRepository
    {
        public string RecordType => "FLST";

        public IReadOnlyList<RecordTreeEntryDTO> Entries { get; set; } = [];

        public IReadOnlyDictionary<string, int> PluginCounts { get; set; } = new Dictionary<string, int>();

        public (SupportedGame Game, ModKeyDTO ModKey)? Request { get; private set; }

        public int PluginCountRequestCount { get; private set; }

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            Request = (game, modKey);
            return Entries;
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            PluginCountRequestCount++;
            return PluginCounts;
        }

        public IReadOnlyList<FormListDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return [];
        }

        public void Save(FormListDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestGameSettingRepository : IGameSettingRepository, IRecordTreeRepository
    {
        public string RecordType => "GMST";

        public IReadOnlyList<RecordTreeEntryDTO> Entries { get; set; } = [];

        public IReadOnlyDictionary<string, int> PluginCounts { get; set; } = new Dictionary<string, int>();

        public (SupportedGame Game, ModKeyDTO ModKey)? Request { get; private set; }

        public int PluginCountRequestCount { get; private set; }

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            Request = (game, modKey);
            return Entries;
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            PluginCountRequestCount++;
            return PluginCounts;
        }

        public IReadOnlyList<GameSettingDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return [];
        }

        public void Save(GameSettingDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestGlobalRepository : IGlobalRepository, IRecordTreeRepository
    {
        public string RecordType => "GLOB";

        public IReadOnlyList<RecordTreeEntryDTO> Entries { get; set; } = [];

        public IReadOnlyDictionary<string, int> PluginCounts { get; set; } = new Dictionary<string, int>();

        public (SupportedGame Game, ModKeyDTO ModKey)? Request { get; private set; }

        public int PluginCountRequestCount { get; private set; }

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            Request = (game, modKey);
            return Entries;
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            PluginCountRequestCount++;
            return PluginCounts;
        }

        public IReadOnlyList<GlobalDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return [];
        }

        public void Save(GlobalDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }
}
