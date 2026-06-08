using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class MiscObjectImporter : ITypedRecordImporter
{
    private readonly IMiscObjectRepository MiscObjectRepository;
    private readonly IModelImportService ModelImportService;
    private readonly IRecordKeywordImportService RecordKeywordImportService;
    private readonly IRecordSoundImportService RecordSoundImportService;
    private readonly IScriptingAdapterImportService ScriptingAdapterImportService;

    public MiscObjectImporter(
        IMiscObjectRepository miscObjectRepository,
        IScriptingAdapterImportService scriptingAdapterImportService,
        IModelImportService modelImportService,
        IRecordKeywordImportService recordKeywordImportService,
        IRecordSoundImportService recordSoundImportService)
    {
        MiscObjectRepository = miscObjectRepository;
        ScriptingAdapterImportService = scriptingAdapterImportService;
        ModelImportService = modelImportService;
        RecordKeywordImportService = recordKeywordImportService;
        RecordSoundImportService = recordSoundImportService;
    }

    public string RecordType => RecordTypeCatalog.MiscObject.RecordID;

    public string TableName => RecordTypeCatalog.MiscObject.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not MiscObjectDTO miscObject) throw new ArgumentException($"Expected {nameof(MiscObjectDTO)}.", nameof(recordDTO));

        miscObject.ImportedAtUTC = importedAtUTC;
        MiscObjectRepository.Save(miscObject);
        ModelImportService.ReplaceRecordModels(miscObject, RecordTypeCatalog.MiscObject.RecordID);
        RecordKeywordImportService.ReplaceRecordKeywords(miscObject, RecordTypeCatalog.MiscObject.RecordID);
        RecordSoundImportService.ReplaceRecordSounds(miscObject, RecordTypeCatalog.MiscObject.RecordID);
        ScriptingAdapterImportService.ReplaceRecordScriptingAdapters(miscObject, RecordTypeCatalog.MiscObject.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        MiscObjectRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
