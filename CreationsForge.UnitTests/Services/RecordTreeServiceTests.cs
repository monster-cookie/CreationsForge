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
    public void GetRecordTreeEntries_DelegatesToRecordInstanceRepository()
    {
        var modKey = CreateModKey("Example", "Example.esm");
        var repository = new TestRecordInstanceRepository
        {
            Entries =
            [
                CreateEntry("FLST", "FormListEditorID"),
                CreateEntry("GMST", "GameSettingEditorID"),
                CreateEntry("GLOB", "GlobalEditorID")
            ]
        };
        var service = new RecordTreeService(repository);

        var entries = service.GetRecordTreeEntries(SupportedGame.Starfield, modKey);

        entries.ShouldBe(repository.Entries);
        repository.TreeRequest.ShouldBe((SupportedGame.Starfield, modKey));
    }

    private static RecordTreeEntryDTO CreateEntry(string recordType, string editorId)
    {
        return new RecordTreeEntryDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey("Example", "Example.esm"),
            FormKey = new FormKeyDTO
            {
                ModKey = CreateModKey("Example", "Example.esm"),
                Id = 123
            },
            EditorID = editorId,
            RecordType = recordType,
            PluginCount = 2
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

    private sealed class TestRecordInstanceRepository : IRecordInstanceRepository
    {
        public IReadOnlyList<RecordTreeEntryDTO> Entries { get; set; } = [];

        public (SupportedGame Game, ModKeyDTO ModKey)? TreeRequest { get; private set; }

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            TreeRequest = (game, modKey);
            return Entries;
        }

        public void Save(RecordInstanceDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, string recordType, DateTime importedAtUTC)
        { }
    }
}
