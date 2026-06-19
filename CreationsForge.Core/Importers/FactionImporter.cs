using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class FactionImporter : ITypedRecordImporter
{
    private readonly IFactionRepository FactionRepository;
    private readonly IRecordChildImportService RecordChildImportService;

    public FactionImporter(
        IFactionRepository factionRepository,
        IRecordChildImportService recordChildImportService)
    {
        FactionRepository = factionRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.Faction.RecordID;

    public string TableName => RecordTypeCatalog.Faction.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not FactionDTO faction) throw new ArgumentException($"Expected {nameof(FactionDTO)}.", nameof(recordDTO));

        faction.ImportedAtUTC = importedAtUTC;
        FactionRepository.Save(faction);
        RecordChildImportService.ReplaceRecordChildren(faction, RecordTypeCatalog.Faction.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        FactionRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
