using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Starfield.Importers;

public class StarfieldModelRecordImporter : ITypedRecordImporter
{
    private readonly IModelImportService ModelImportService;
    private readonly IRecordInstanceRepository RecordInstanceRepository;

    public StarfieldModelRecordImporter(
        string recordType,
        IRecordInstanceRepository recordInstanceRepository,
        IModelImportService modelImportService)
    {
        RecordType = recordType;
        RecordInstanceRepository = recordInstanceRepository;
        ModelImportService = modelImportService;
    }

    public string RecordType { get; }

    public string TableName => "RecordInstances";

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not ModelRecordDTO modelRecord) throw new ArgumentException($"Expected {nameof(ModelRecordDTO)}.", nameof(recordDTO));

        modelRecord.ImportedAtUTC = importedAtUTC;
        RecordInstanceRepository.Save(new RecordInstanceDTO
        {
            Game = modelRecord.Game,
            ModKey = modelRecord.ModKey,
            RecordType = RecordType,
            FormKey = modelRecord.FormKey,
            EditorID = modelRecord.EditorID,
            FormVersion = modelRecord.FormVersion,
            MajorRecordFlags = modelRecord.MajorRecordFlags,
            ImportedAtUTC = modelRecord.ImportedAtUTC
        });
        ModelImportService.ReplaceRecordModels(modelRecord, RecordType);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        RecordInstanceRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, RecordType, importedAtUTC);
    }
}
