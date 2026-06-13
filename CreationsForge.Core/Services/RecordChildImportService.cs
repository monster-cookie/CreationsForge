using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class RecordChildImportService : IRecordChildImportService
{
    private readonly IModelImportService ModelImportService;
    private readonly IRecordKeywordImportService RecordKeywordImportService;
    private readonly IRecordSoundImportService RecordSoundImportService;
    private readonly IScriptingAdapterImportService ScriptingAdapterImportService;

    public RecordChildImportService(
        IModelImportService modelImportService,
        IRecordKeywordImportService recordKeywordImportService,
        IRecordSoundImportService recordSoundImportService,
        IScriptingAdapterImportService scriptingAdapterImportService)
    {
        ModelImportService = modelImportService;
        RecordKeywordImportService = recordKeywordImportService;
        RecordSoundImportService = recordSoundImportService;
        ScriptingAdapterImportService = scriptingAdapterImportService;
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

        if (record is IHasSoundsRecordDTO soundRecord)
        {
            RecordSoundImportService.ReplaceRecordSounds(soundRecord, recordType);
        }

        if (record is IHasScriptingAdaptersRecordDTO scriptingAdapterRecord)
        {
            ScriptingAdapterImportService.ReplaceRecordScriptingAdapters(scriptingAdapterRecord, recordType);
        }
    }
}
