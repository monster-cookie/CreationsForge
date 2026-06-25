using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

/// <summary>
/// Coordinates replacement imports for typed record child tables.
/// </summary>
public class RecordChildImportService : IRecordChildImportService
{
    private readonly IModelImportService ModelImportService;
    private readonly IKeywordMappingImportService KeywordMappingImportService;
    private readonly IRecordComponentImportService RecordComponentImportService;
    private readonly IConditionRuleImportService ConditionRuleImportService;
    private readonly IRawRecordPayloadImportService RawRecordPayloadImportService;
    /// <summary>
    /// Imports first-class component reflection rows for records with Spriggit <c>REFL</c> data.
    /// </summary>
    private readonly IReflectionImportService ReflectionImportService;
    private readonly ISoundMappingImportService SoundMappingImportService;
    private readonly IScriptingAdapterImportService ScriptingAdapterImportService;
    private readonly ITerminalMarkerParameterImportService TerminalMarkerParameterImportService;
    private readonly IRecordLocalizedStringImportService RecordLocalizedStringImportService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordChildImportService"/> class.
    /// </summary>
    /// <param name="modelImportService">The model child-row import service.</param>
    /// <param name="keywordMappingImportService">The keyword mapping child-row import service.</param>
    /// <param name="recordComponentImportService">The record component child-row import service.</param>
    /// <param name="conditionRuleImportService">The condition rule child-row import service.</param>
    /// <param name="rawRecordPayloadImportService">The raw payload child-row import service.</param>
    /// <param name="reflectionImportService">The reflection child-row import service.</param>
    /// <param name="soundMappingImportService">The sound mapping child-row import service.</param>
    /// <param name="scriptingAdapterImportService">The scripting adapter child-row import service.</param>
    /// <param name="terminalMarkerParameterImportService">The terminal marker parameter child-row import service.</param>
    /// <param name="recordLocalizedStringImportService">The localized string child-row import service.</param>
    public RecordChildImportService(
        IModelImportService modelImportService,
        IKeywordMappingImportService keywordMappingImportService,
        IRecordComponentImportService recordComponentImportService,
        IConditionRuleImportService conditionRuleImportService,
        IRawRecordPayloadImportService rawRecordPayloadImportService,
        IReflectionImportService reflectionImportService,
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
        ReflectionImportService = reflectionImportService;
        SoundMappingImportService = soundMappingImportService;
        ScriptingAdapterImportService = scriptingAdapterImportService;
        TerminalMarkerParameterImportService = terminalMarkerParameterImportService;
        RecordLocalizedStringImportService = recordLocalizedStringImportService;
    }

    /// <summary>
    /// Replaces all supported child rows for an imported parent record.
    /// </summary>
    /// <param name="record">The imported parent record DTO.</param>
    /// <param name="recordType">The Bethesda record type identifier for the parent record.</param>
    public void ReplaceRecordChildren(RecordDTO record, string recordType)
    {
        if (record is IHasModelsDTO modelRecord)
        {
            ModelImportService.ReplaceRecordModels(modelRecord, recordType);
        }

        if (record is IKeywords keywordRecord)
        {
            KeywordMappingImportService.ReplaceKeywordMappings(keywordRecord, recordType);
        }

        if (record is IHasComponentsDTO componentRecord)
        {
            RecordComponentImportService.ReplaceRecordComponents(componentRecord, recordType);
        }

        if (record is IHasConditionsDTO conditionRecord)
        {
            ConditionRuleImportService.ReplaceConditionRules(conditionRecord, recordType);
        }

        if (record is ISounds soundRecord)
        {
            SoundMappingImportService.ReplaceSoundMappings(soundRecord, recordType);
        }

        if (record is IHasRawRecordPayloadsDTO rawPayloadRecord)
        {
            RawRecordPayloadImportService.ReplaceRawRecordPayloads(rawPayloadRecord, recordType);
        }

        if (record is IHasReflectionDTO reflectionRecord)
        {
            ReflectionImportService.ReplaceReflections(reflectionRecord, recordType);
        }

        if (record is IHasScriptingAdaptersDTO scriptingAdapterRecord)
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
