using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class GlobalImporter : ITypedRecordImporter
{
    private readonly IGlobalRepository GlobalRepository;
    private readonly IRecordChildImportService RecordChildImportService;

    public GlobalImporter(
        IGlobalRepository globalRepository,
        IRecordChildImportService recordChildImportService)
    {
        GlobalRepository = globalRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.Global.RecordID;

    public string TableName => RecordTypeCatalog.Global.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame>
    {
        SupportedGame.Starfield,
        SupportedGame.Fallout4,
        SupportedGame.Skyrim
    };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not GlobalDTO global) throw new ArgumentException($"Expected {nameof(GlobalDTO)}.", nameof(recordDTO));

        global.ImportedAtUTC = importedAtUTC;
        GlobalRepository.Save(global);
        RecordChildImportService.ReplaceRecordChildren(global, RecordTypeCatalog.Global.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        GlobalRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
