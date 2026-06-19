using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class RecordChildImportService : IRecordChildImportService
{
    private readonly IModelImportService ModelImportService;
    private readonly IRecordKeywordImportService RecordKeywordImportService;
    private readonly IRecordComponentImportService RecordComponentImportService;
    private readonly IConditionRuleImportService ConditionRuleImportService;
    private readonly IRawRecordPayloadImportService RawRecordPayloadImportService;
    private readonly IRecordSoundImportService RecordSoundImportService;
    private readonly IScriptingAdapterImportService ScriptingAdapterImportService;
    private readonly ITerminalMarkerParameterImportService TerminalMarkerParameterImportService;

    public RecordChildImportService(
        IModelImportService modelImportService,
        IRecordKeywordImportService recordKeywordImportService,
        IRecordComponentImportService recordComponentImportService,
        IConditionRuleImportService conditionRuleImportService,
        IRawRecordPayloadImportService rawRecordPayloadImportService,
        IRecordSoundImportService recordSoundImportService,
        IScriptingAdapterImportService scriptingAdapterImportService,
        ITerminalMarkerParameterImportService terminalMarkerParameterImportService)
    {
        ModelImportService = modelImportService;
        RecordKeywordImportService = recordKeywordImportService;
        RecordComponentImportService = recordComponentImportService;
        ConditionRuleImportService = conditionRuleImportService;
        RawRecordPayloadImportService = rawRecordPayloadImportService;
        RecordSoundImportService = recordSoundImportService;
        ScriptingAdapterImportService = scriptingAdapterImportService;
        TerminalMarkerParameterImportService = terminalMarkerParameterImportService;
    }

    public void ReplaceRecordChildren(RecordDTO record, string recordType)
    {
        if (record is IHasModelsRecordDTO modelRecord)
        {
            ModelImportService.ReplaceRecordModels(modelRecord, recordType);
        }

        if (record is IHasKeywordsRecordDTO keywordRecord)
        {
            RecordKeywordImportService.ReplaceRecordKeywords(keywordRecord, recordType);
        }

        if (record is IHasComponentsRecordDTO componentRecord)
        {
            RecordComponentImportService.ReplaceRecordComponents(componentRecord, recordType);
        }

        if (record is IHasConditionsRecordDTO conditionRecord)
        {
            ConditionRuleImportService.ReplaceConditionRules(conditionRecord, recordType);
        }

        if (record is IHasSoundsRecordDTO soundRecord)
        {
            RecordSoundImportService.ReplaceRecordSounds(soundRecord, recordType);
        }

        if (record is IHasRawRecordPayloadsRecordDTO rawPayloadRecord)
        {
            RawRecordPayloadImportService.ReplaceRawRecordPayloads(rawPayloadRecord, recordType);
        }

        if (record is IHasScriptingAdaptersRecordDTO scriptingAdapterRecord)
        {
            ScriptingAdapterImportService.ReplaceRecordScriptingAdapters(scriptingAdapterRecord, recordType);
        }

        if (record is IHasTerminalMarkerParametersRecordDTO terminalMarkerParameterRecord)
        {
            TerminalMarkerParameterImportService.ReplaceRecordMarkerParameters(terminalMarkerParameterRecord);
        }
    }
}
