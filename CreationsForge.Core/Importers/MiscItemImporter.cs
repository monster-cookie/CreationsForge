using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class MiscItemImporter : ITypedRecordImporter
{
    private readonly IMiscItemRepository MiscItemRepository;
    private readonly IRecordChildImportService RecordChildImportService;

    public MiscItemImporter(
        IMiscItemRepository miscItemRepository,
        IRecordChildImportService recordChildImportService)
    {
        MiscItemRepository = miscItemRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.MiscItem.RecordID;

    public string TableName => RecordTypeCatalog.MiscItem.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not MiscItemDTO miscItem) throw new ArgumentException($"Expected {nameof(MiscItemDTO)}.", nameof(recordDTO));

        miscItem.ImportedAtUTC = importedAtUTC;
        MiscItemRepository.Save(miscItem);
        RecordChildImportService.ReplaceRecordChildren(miscItem, RecordTypeCatalog.MiscItem.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        MiscItemRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
