using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class StaticImporter : ITypedRecordImporter
{
    private readonly IRecordChildImportService RecordChildImportService;
    private readonly IStaticRepository StaticRepository;

    public StaticImporter(
        IStaticRepository staticRepository,
        IRecordChildImportService recordChildImportService)
    {
        StaticRepository = staticRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.Static.RecordID;

    public string TableName => RecordTypeCatalog.Static.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not StaticDTO staticRecord) throw new ArgumentException($"Expected {nameof(StaticDTO)}.", nameof(recordDTO));

        staticRecord.ImportedAtUTC = importedAtUTC;
        StaticRepository.Save(staticRecord);
        RecordChildImportService.ReplaceRecordChildren(staticRecord, RecordTypeCatalog.Static.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        StaticRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
