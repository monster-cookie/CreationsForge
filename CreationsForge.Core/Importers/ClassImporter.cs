using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class ClassImporter : ITypedRecordImporter
{
    private readonly IClassRepository ClassRepository;
    private readonly IRecordChildImportService RecordChildImportService;

    public ClassImporter(
        IClassRepository classRepository,
        IRecordChildImportService recordChildImportService)
    {
        ClassRepository = classRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.Class.RecordID;

    public string TableName => RecordTypeCatalog.Class.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not ClassDTO classRecord) throw new ArgumentException($"Expected {nameof(ClassDTO)}.", nameof(recordDTO));

        classRecord.ImportedAtUTC = importedAtUTC;
        ClassRepository.Save(classRecord);
        RecordChildImportService.ReplaceRecordChildren(classRecord, RecordTypeCatalog.Class.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        ClassRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
