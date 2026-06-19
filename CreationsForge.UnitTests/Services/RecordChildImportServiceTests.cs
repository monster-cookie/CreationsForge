using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
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
        var componentImportService = new TestRecordComponentImportService();
        var conditionRuleImportService = new TestConditionRuleImportService();
        var rawRecordPayloadImportService = new TestRawRecordPayloadImportService();
        var soundImportService = new TestRecordSoundImportService();
        var scriptingAdapterImportService = new TestScriptingAdapterImportService();
        var terminalMarkerParameterImportService = new TestTerminalMarkerParameterImportService();
        var localizedStringImportService = new TestRecordLocalizedStringImportService();
        var service = new RecordChildImportService(
            modelImportService,
            keywordImportService,
            componentImportService,
            conditionRuleImportService,
            rawRecordPayloadImportService,
            soundImportService,
            scriptingAdapterImportService,
            terminalMarkerParameterImportService,
            localizedStringImportService);
        var record = CreateCompositeRecord();

        service.ReplaceRecordChildren(record, "TEST");

        modelImportService.ReplaceRequests.ShouldBe([(record, "TEST")]);
        keywordImportService.ReplaceRequests.ShouldBe([(record, "TEST")]);
        componentImportService.ReplaceRequests.ShouldBe([(record, "TEST")]);
        conditionRuleImportService.ReplaceRequests.ShouldBe([(record, "TEST")]);
        rawRecordPayloadImportService.ReplaceRequests.ShouldBe([(record, "TEST")]);
        soundImportService.ReplaceRequests.ShouldBe([(record, "TEST")]);
        scriptingAdapterImportService.ReplaceRequests.ShouldBe([(record, "TEST")]);
        terminalMarkerParameterImportService.ReplaceRequests.ShouldBe([record]);
        localizedStringImportService.ReplaceRequests.ShouldBe([(record, "TEST")]);
    }

    [Fact]
    public void ReplaceRecordChildren_WhenRecordHasOnlyLocalizedStrings_DispatchesOnlyLocalizedStrings()
    {
        var modelImportService = new TestModelImportService();
        var keywordImportService = new TestRecordKeywordImportService();
        var componentImportService = new TestRecordComponentImportService();
        var conditionRuleImportService = new TestConditionRuleImportService();
        var rawRecordPayloadImportService = new TestRawRecordPayloadImportService();
        var soundImportService = new TestRecordSoundImportService();
        var scriptingAdapterImportService = new TestScriptingAdapterImportService();
        var terminalMarkerParameterImportService = new TestTerminalMarkerParameterImportService();
        var localizedStringImportService = new TestRecordLocalizedStringImportService();
        var service = new RecordChildImportService(
            modelImportService,
            keywordImportService,
            componentImportService,
            conditionRuleImportService,
            rawRecordPayloadImportService,
            soundImportService,
            scriptingAdapterImportService,
            terminalMarkerParameterImportService,
            localizedStringImportService);

        var record = CreateFlatRecord();

        service.ReplaceRecordChildren(record, "FLAT");

        modelImportService.ReplaceRequests.ShouldBeEmpty();
        keywordImportService.ReplaceRequests.ShouldBeEmpty();
        componentImportService.ReplaceRequests.ShouldBeEmpty();
        conditionRuleImportService.ReplaceRequests.ShouldBeEmpty();
        rawRecordPayloadImportService.ReplaceRequests.ShouldBeEmpty();
        soundImportService.ReplaceRequests.ShouldBeEmpty();
        scriptingAdapterImportService.ReplaceRequests.ShouldBeEmpty();
        terminalMarkerParameterImportService.ReplaceRequests.ShouldBeEmpty();
        localizedStringImportService.ReplaceRequests.ShouldBe([(record, "FLAT")]);
    }

    [Fact]
    public void ReplaceRecordChildren_WhenConditionForm_DoesNotDispatchRawPayloads()
    {
        var modelImportService = new TestModelImportService();
        var keywordImportService = new TestRecordKeywordImportService();
        var componentImportService = new TestRecordComponentImportService();
        var conditionRuleImportService = new TestConditionRuleImportService();
        var rawRecordPayloadImportService = new TestRawRecordPayloadImportService();
        var soundImportService = new TestRecordSoundImportService();
        var scriptingAdapterImportService = new TestScriptingAdapterImportService();
        var terminalMarkerParameterImportService = new TestTerminalMarkerParameterImportService();
        var localizedStringImportService = new TestRecordLocalizedStringImportService();
        var service = new RecordChildImportService(
            modelImportService,
            keywordImportService,
            componentImportService,
            conditionRuleImportService,
            rawRecordPayloadImportService,
            soundImportService,
            scriptingAdapterImportService,
            terminalMarkerParameterImportService,
            localizedStringImportService);

        var conditionForm = CreateConditionForm();

        service.ReplaceRecordChildren(conditionForm, RecordTypeCatalog.ConditionForm.RecordID);

        conditionRuleImportService.ReplaceRequests.ShouldBe([(conditionForm, RecordTypeCatalog.ConditionForm.RecordID)]);
        rawRecordPayloadImportService.ReplaceRequests.ShouldBeEmpty();
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

    private static ConditionFormDTO CreateConditionForm()
    {
        return new ConditionFormDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(),
            FormKey = new FormKeyDTO { ModKey = CreateModKey(), Id = 12 },
            EditorID = "ConditionForm",
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

    private sealed class CompositeRecordDTO : RecordDTO, IHasModelsRecordDTO, IHasKeywordsRecordDTO, IHasComponentsRecordDTO, IHasConditionsRecordDTO, IHasRawRecordPayloadsRecordDTO, IHasSoundsRecordDTO, IHasScriptingAdaptersRecordDTO, IHasTerminalMarkerParametersRecordDTO
    {
        public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

        public IList<RecordKeywordDTO> Keywords { get; set; } = new List<RecordKeywordDTO>();

        public IList<RecordComponentDTO> Components { get; set; } = new List<RecordComponentDTO>();

        public IList<ConditionFormConditionDTO> Conditions { get; set; } = new List<ConditionFormConditionDTO>();

        public IList<RawRecordPayloadDTO> RawPayloads { get; set; } = new List<RawRecordPayloadDTO>();

        public IList<RecordSoundDTO> Sounds { get; set; } = new List<RecordSoundDTO>();

        public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

        public IList<TerminalMarkerParameterDTO> MarkerParameters { get; set; } = new List<TerminalMarkerParameterDTO>();
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

    private sealed class TestRawRecordPayloadImportService : IRawRecordPayloadImportService
    {
        public IList<(IHasRawRecordPayloadsRecordDTO Record, string RecordType)> ReplaceRequests { get; } = new List<(IHasRawRecordPayloadsRecordDTO Record, string RecordType)>();

        public void ReplaceRawRecordPayloads(IHasRawRecordPayloadsRecordDTO record, string recordType)
        {
            ReplaceRequests.Add((record, recordType));
        }
    }

    private sealed class TestRecordComponentImportService : IRecordComponentImportService
    {
        public IList<(IHasComponentsRecordDTO Record, string RecordType)> ReplaceRequests { get; } = new List<(IHasComponentsRecordDTO Record, string RecordType)>();

        public void ReplaceRecordComponents(IHasComponentsRecordDTO record, string recordType)
        {
            ReplaceRequests.Add((record, recordType));
        }
    }

    private sealed class TestConditionRuleImportService : IConditionRuleImportService
    {
        public IList<(IHasConditionsRecordDTO Record, string RecordType)> ReplaceRequests { get; } = new List<(IHasConditionsRecordDTO Record, string RecordType)>();

        public void ReplaceConditionRules(IHasConditionsRecordDTO record, string recordType)
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

    private sealed class TestTerminalMarkerParameterImportService : ITerminalMarkerParameterImportService
    {
        public IList<IHasTerminalMarkerParametersRecordDTO> ReplaceRequests { get; } = new List<IHasTerminalMarkerParametersRecordDTO>();

        public void ReplaceRecordMarkerParameters(IHasTerminalMarkerParametersRecordDTO record)
        {
            ReplaceRequests.Add(record);
        }
    }

    private sealed class TestRecordLocalizedStringImportService : IRecordLocalizedStringImportService
    {
        public IList<(IHasLocalizedStringsRecordDTO Record, string RecordType)> ReplaceRequests { get; } = new List<(IHasLocalizedStringsRecordDTO Record, string RecordType)>();

        public void ReplaceRecordLocalizedStrings(IHasLocalizedStringsRecordDTO record, string recordType)
        {
            ReplaceRequests.Add((record, recordType));
        }
    }
}
