using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class DoorImporter : ITypedRecordImporter
{
    private readonly IDoorRepository DoorRepository;
    private readonly IRecordChildImportService RecordChildImportService;

    public DoorImporter(IDoorRepository doorRepository, IRecordChildImportService recordChildImportService)
    {
        DoorRepository = doorRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.Door.RecordID;

    public string TableName => RecordTypeCatalog.Door.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not DoorDTO door) throw new ArgumentException($"Expected {nameof(DoorDTO)}.", nameof(recordDTO));

        door.ImportedAtUTC = importedAtUTC;
        DoorRepository.Save(door);
        RecordChildImportService.ReplaceRecordChildren(door, RecordTypeCatalog.Door.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        DoorRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
