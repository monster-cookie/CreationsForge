using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Starfield.Importers;
using Shouldly;

namespace CreationsForge.UnitTests.Importers;

public class StarfieldModelRecordImporterTests
{
    [Fact]
    public void Import_SavesRecordInstanceAndModels()
    {
        var recordInstanceRepository = new TestRecordInstanceRepository();
        var modelImportService = new TestModelImportService();
        var importer = new StarfieldModelRecordImporter("STAT", recordInstanceRepository, modelImportService);
        var plugin = CreatePlugin();
        var record = new ModelRecordDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            FormKey = new FormKeyDTO
            {
                ModKey = plugin.ModKey,
                Id = 0x123456
            },
            EditorID = "PreviewStatic",
            FormVersion = 582,
            MajorRecordFlags = 1,
            ImportedAtUTC = DateTime.MinValue,
            Models =
            [
                new ModelDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = plugin.ModKey,
                    RecordType = "STAT",
                    FormKey = new FormKeyDTO
                    {
                        ModKey = plugin.ModKey,
                        Id = 0x123456
                    },
                    ModelSlot = "Model",
                    File = "Meshes\\Preview\\Static.nif",
                    ImportedAtUTC = DateTime.MinValue
                }
            ]
        };
        var importedAtUTC = DateTime.UtcNow;
        var result = new RecordTypeImportResultDTO
        {
            RecordType = "STAT"
        };

        importer.Import(record, result, importedAtUTC);

        record.ImportedAtUTC.ShouldBe(importedAtUTC);
        result.DetailRowsImported.ShouldBe(1);
        recordInstanceRepository.Saved.ShouldNotBeNull();
        recordInstanceRepository.Saved.RecordType.ShouldBe("STAT");
        recordInstanceRepository.Saved.EditorID.ShouldBe("PreviewStatic");
        modelImportService.Record.ShouldBeSameAs(record);
        modelImportService.RecordType.ShouldBe("STAT");
    }

    [Fact]
    public void DeleteStaleRecords_DeletesRecordInstancesForRecordType()
    {
        var recordInstanceRepository = new TestRecordInstanceRepository();
        var importer = new StarfieldModelRecordImporter("BOOK", recordInstanceRepository, new TestModelImportService());
        var plugin = CreatePlugin();
        var importedAtUTC = DateTime.UtcNow;

        importer.DeleteStaleRecords(plugin, importedAtUTC);

        recordInstanceRepository.DeletedGame.ShouldBe(SupportedGame.Starfield);
        recordInstanceRepository.DeletedModKey.ShouldBe(plugin.ModKey);
        recordInstanceRepository.DeletedRecordType.ShouldBe("BOOK");
        recordInstanceRepository.DeletedImportedAtUTC.ShouldBe(importedAtUTC);
    }

    private static PluginDTO CreatePlugin()
    {
        return new PluginDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = new ModKeyDTO
            {
                Name = "Plugin",
                Type = 0,
                FileName = "Plugin.esm"
            },
            LoadOrderIndex = 0,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current,
            HeaderFlags = 0,
            FormVersion = 0,
            RecordCount = 0,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow
        };
    }

    private class TestRecordInstanceRepository : IRecordInstanceRepository
    {
        public RecordInstanceDTO? Saved { get; private set; }

        public SupportedGame? DeletedGame { get; private set; }

        public ModKeyDTO? DeletedModKey { get; private set; }

        public string? DeletedRecordType { get; private set; }

        public DateTime? DeletedImportedAtUTC { get; private set; }

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public void Save(RecordInstanceDTO dto)
        {
            Saved = dto;
        }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, string recordType, DateTime importedAtUTC)
        {
            DeletedGame = game;
            DeletedModKey = modKey;
            DeletedRecordType = recordType;
            DeletedImportedAtUTC = importedAtUTC;
        }
    }

    private class TestModelImportService : IModelImportService
    {
        public IHasModelsDTO? Record { get; private set; }

        public string? RecordType { get; private set; }

        public void ReplaceRecordModels(IHasModelsDTO record, string recordType)
        {
            Record = record;
            RecordType = recordType;
        }
    }
}
