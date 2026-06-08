using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class NPCImporter : ITypedRecordImporter
{
    private readonly INPCRepository NPCRepository;
    private readonly IRecordKeywordImportService RecordKeywordImportService;
    private readonly IScriptingAdapterImportService ScriptingAdapterImportService;

    public NPCImporter(
        INPCRepository npcRepository,
        IScriptingAdapterImportService scriptingAdapterImportService,
        IRecordKeywordImportService recordKeywordImportService)
    {
        NPCRepository = npcRepository;
        ScriptingAdapterImportService = scriptingAdapterImportService;
        RecordKeywordImportService = recordKeywordImportService;
    }

    public string RecordType => RecordTypeCatalog.NPC.RecordID;

    public string TableName => RecordTypeCatalog.NPC.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not NPCDTO npc) throw new ArgumentException($"Expected {nameof(NPCDTO)}.", nameof(recordDTO));

        npc.ImportedAtUTC = importedAtUTC;
        NPCRepository.Save(npc);
        RecordKeywordImportService.ReplaceRecordKeywords(npc, RecordTypeCatalog.NPC.RecordID);
        ScriptingAdapterImportService.ReplaceRecordScriptingAdapters(npc, RecordTypeCatalog.NPC.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        NPCRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
