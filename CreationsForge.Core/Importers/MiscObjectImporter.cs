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
    private readonly IRecordChildImportService RecordChildImportService;

    public MiscObjectImporter(
        IMiscObjectRepository miscObjectRepository,
        IRecordChildImportService recordChildImportService)
    {
        MiscObjectRepository = miscObjectRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.MiscObject.RecordID;

    public string TableName => RecordTypeCatalog.MiscObject.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not MiscObjectDTO miscObject) throw new ArgumentException($"Expected {nameof(MiscObjectDTO)}.", nameof(recordDTO));

        miscObject.ImportedAtUTC = importedAtUTC;
        MiscObjectRepository.Save(miscObject);
        RecordChildImportService.ReplaceRecordChildren(miscObject, RecordTypeCatalog.MiscObject.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        MiscObjectRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
