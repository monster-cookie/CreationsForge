using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class ConstructibleObjectImporter : ITypedRecordImporter
{
    private readonly IConstructibleObjectRepository ConstructibleObjectRepository;
    private readonly IRecordChildImportService RecordChildImportService;

    public ConstructibleObjectImporter(
        IConstructibleObjectRepository constructibleObjectRepository,
        IRecordChildImportService recordChildImportService)
    {
        ConstructibleObjectRepository = constructibleObjectRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.ConstructibleObject.RecordID;

    public string TableName => RecordTypeCatalog.ConstructibleObject.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not ConstructibleObjectDTO constructibleObject) throw new ArgumentException($"Expected {nameof(ConstructibleObjectDTO)}.", nameof(recordDTO));

        constructibleObject.ImportedAtUTC = importedAtUTC;
        ConstructibleObjectRepository.Save(constructibleObject);
        RecordChildImportService.ReplaceRecordChildren(constructibleObject, RecordTypeCatalog.ConstructibleObject.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        ConstructibleObjectRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
