using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services;
using CreationsForge.Core.Services.Interfaces;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class RecordChildImportServiceTests
{
    [Fact]
    public void ReplaceRecordChildren_DispatchesByRecordCapabilities()
    {
        var modelImportService = new TestModelImportService();
        var keywordImportService = new TestRecordKeywordImportService();
        var soundImportService = new TestRecordSoundImportService();
        var scriptingAdapterImportService = new TestScriptingAdapterImportService();
        var service = new RecordChildImportService(
            modelImportService,
            keywordImportService,
            soundImportService,
            scriptingAdapterImportService);
        var record = CreateCompositeRecord();

        service.ReplaceRecordChildren(record, "TEST");

        modelImportService.ReplaceRequests.ShouldBe([(record, "TEST")]);
        keywordImportService.ReplaceRequests.ShouldBe([(record, "TEST")]);
        soundImportService.ReplaceRequests.ShouldBe([(record, "TEST")]);
        scriptingAdapterImportService.ReplaceRequests.ShouldBe([(record, "TEST")]);
    }

    [Fact]
    public void ReplaceRecordChildren_WhenRecordHasNoSharedChildren_DoesNotDispatch()
    {
        var modelImportService = new TestModelImportService();
        var keywordImportService = new TestRecordKeywordImportService();
        var soundImportService = new TestRecordSoundImportService();
        var scriptingAdapterImportService = new TestScriptingAdapterImportService();
        var service = new RecordChildImportService(
            modelImportService,
            keywordImportService,
            soundImportService,
            scriptingAdapterImportService);

        service.ReplaceRecordChildren(CreateFlatRecord(), "FLAT");

        modelImportService.ReplaceRequests.ShouldBeEmpty();
        keywordImportService.ReplaceRequests.ShouldBeEmpty();
        soundImportService.ReplaceRequests.ShouldBeEmpty();
        scriptingAdapterImportService.ReplaceRequests.ShouldBeEmpty();
    }

    private static CompositeRecordDTO CreateCompositeRecord()
    {
        return new CompositeRecordDTO
        {
            Game = SupportedGame.Fallout4,
            ModKey = CreateModKey(),
            FormKey = new FormKeyDTO { ModKey = CreateModKey(), Id = 10 },
            EditorID = "Composite",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static FlatRecordDTO CreateFlatRecord()
    {
        return new FlatRecordDTO
        {
            Game = SupportedGame.Skyrim,
            ModKey = CreateModKey(),
            FormKey = new FormKeyDTO { ModKey = CreateModKey(), Id = 11 },
            EditorID = "Flat",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static ModKeyDTO CreateModKey()
    {
        return new ModKeyDTO
        {
            Name = "Test",
            Type = 0,
            FileName = "Test.esm"
        };
    }

    private sealed class CompositeRecordDTO : RecordDTO, IHasModelsRecordDTO, IHasKeywordsRecordDTO, IHasSoundsRecordDTO, IHasScriptingAdaptersRecordDTO
    {
        public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

        public IList<RecordKeywordDTO> Keywords { get; set; } = new List<RecordKeywordDTO>();

        public IList<RecordSoundDTO> Sounds { get; set; } = new List<RecordSoundDTO>();

        public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
    }

    private sealed class FlatRecordDTO : RecordDTO
    { }

    private sealed class TestModelImportService : IModelImportService
    {
        public IList<(IHasModelsRecordDTO Record, string RecordType)> ReplaceRequests { get; } = new List<(IHasModelsRecordDTO Record, string RecordType)>();

        public void ReplaceRecordModels(IHasModelsRecordDTO record, string recordType)
        {
            ReplaceRequests.Add((record, recordType));
        }
    }

    private sealed class TestRecordKeywordImportService : IRecordKeywordImportService
    {
        public IList<(IHasKeywordsRecordDTO Record, string RecordType)> ReplaceRequests { get; } = new List<(IHasKeywordsRecordDTO Record, string RecordType)>();

        public void ReplaceRecordKeywords(IHasKeywordsRecordDTO record, string recordType)
        {
            ReplaceRequests.Add((record, recordType));
        }
    }

    private sealed class TestRecordSoundImportService : IRecordSoundImportService
    {
        public IList<(IHasSoundsRecordDTO Record, string RecordType)> ReplaceRequests { get; } = new List<(IHasSoundsRecordDTO Record, string RecordType)>();

        public void ReplaceRecordSounds(IHasSoundsRecordDTO record, string recordType)
        {
            ReplaceRequests.Add((record, recordType));
        }
    }

    private sealed class TestScriptingAdapterImportService : IScriptingAdapterImportService
    {
        public IList<(IHasScriptingAdaptersRecordDTO Record, string RecordType)> ReplaceRequests { get; } = new List<(IHasScriptingAdaptersRecordDTO Record, string RecordType)>();

        public void ReplaceRecordScriptingAdapters(IHasScriptingAdaptersRecordDTO record, string recordType)
        {
            ReplaceRequests.Add((record, recordType));
        }
    }
}
