using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class RecordChildImportService : IRecordChildImportService
{
    private readonly IModelImportService ModelImportService;
    private readonly IKeywordMappingImportService KeywordMappingImportService;
    private readonly IRecordComponentImportService RecordComponentImportService;
    private readonly IConditionRuleImportService ConditionRuleImportService;
    private readonly IRawRecordPayloadImportService RawRecordPayloadImportService;
    private readonly ISoundMappingImportService SoundMappingImportService;
    private readonly IScriptingAdapterImportService ScriptingAdapterImportService;
    private readonly ITerminalMarkerParameterImportService TerminalMarkerParameterImportService;
    private readonly IRecordLocalizedStringImportService RecordLocalizedStringImportService;

    public RecordChildImportService(
        IModelImportService modelImportService,
        IKeywordMappingImportService keywordMappingImportService,
        IRecordComponentImportService recordComponentImportService,
        IConditionRuleImportService conditionRuleImportService,
        IRawRecordPayloadImportService rawRecordPayloadImportService,
        ISoundMappingImportService soundMappingImportService,
        IScriptingAdapterImportService scriptingAdapterImportService,
        ITerminalMarkerParameterImportService terminalMarkerParameterImportService,
        IRecordLocalizedStringImportService recordLocalizedStringImportService)
    {
        ModelImportService = modelImportService;
        KeywordMappingImportService = keywordMappingImportService;
        RecordComponentImportService = recordComponentImportService;
        ConditionRuleImportService = conditionRuleImportService;
        RawRecordPayloadImportService = rawRecordPayloadImportService;
        SoundMappingImportService = soundMappingImportService;
        ScriptingAdapterImportService = scriptingAdapterImportService;
        TerminalMarkerParameterImportService = terminalMarkerParameterImportService;
        RecordLocalizedStringImportService = recordLocalizedStringImportService;
    }

    public void ReplaceRecordChildren(RecordDTO record, string recordType)
    {
        if (record is IHasModelsRecordDTO modelRecord)
        {
            ModelImportService.ReplaceRecordModels(modelRecord, recordType);
        }

        if (record is IKeywords keywordRecord)
        {
            KeywordMappingImportService.ReplaceKeywordMappings(keywordRecord, recordType);
        }

        if (record is IHasComponentsRecordDTO componentRecord)
        {
            RecordComponentImportService.ReplaceRecordComponents(componentRecord, recordType);
        }

        if (record is IHasConditionsRecordDTO conditionRecord)
        {
            ConditionRuleImportService.ReplaceConditionRules(conditionRecord, recordType);
        }

        if (record is ISounds soundRecord)
        {
            SoundMappingImportService.ReplaceSoundMappings(soundRecord, recordType);
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

        if (record is IHasLocalizedStringsRecordDTO localizedStringRecord)
        {
            RecordLocalizedStringImportService.ReplaceRecordLocalizedStrings(localizedStringRecord, recordType);
        }
    }
}
