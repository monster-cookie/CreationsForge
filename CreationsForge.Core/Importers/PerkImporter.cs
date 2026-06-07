using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class PerkImporter : ITypedRecordImporter
{
    private readonly IPerkRepository PerkRepository;
    private readonly IScriptingAdapterImportService ScriptingAdapterImportService;

    public PerkImporter(
        IPerkRepository perkRepository,
        IScriptingAdapterImportService scriptingAdapterImportService)
    {
        PerkRepository = perkRepository;
        ScriptingAdapterImportService = scriptingAdapterImportService;
    }

    public string RecordType => RecordTypeCatalog.Perk.RecordID;

    public string TableName => RecordTypeCatalog.Perk.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not PerkDTO perk) throw new ArgumentException($"Expected {nameof(PerkDTO)}.", nameof(recordDTO));

        perk.ImportedAtUTC = importedAtUTC;
        PerkRepository.Save(perk);
        ScriptingAdapterImportService.ReplaceRecordScriptingAdapters(perk, RecordTypeCatalog.Perk.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        PerkRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
