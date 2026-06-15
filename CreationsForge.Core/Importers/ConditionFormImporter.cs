using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class ConditionFormImporter : ITypedRecordImporter
{
    private readonly IConditionFormRepository ConditionFormRepository;
    private readonly IRecordChildImportService RecordChildImportService;

    public ConditionFormImporter(
        IConditionFormRepository conditionFormRepository,
        IRecordChildImportService recordChildImportService)
    {
        ConditionFormRepository = conditionFormRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.ConditionForm.RecordID;

    public string TableName => RecordTypeCatalog.ConditionForm.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not ConditionFormDTO conditionForm) throw new ArgumentException($"Expected {nameof(ConditionFormDTO)}.", nameof(recordDTO));

        conditionForm.ImportedAtUTC = importedAtUTC;
        ConditionFormRepository.Save(conditionForm);
        RecordChildImportService.ReplaceRecordChildren(conditionForm, RecordTypeCatalog.ConditionForm.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        ConditionFormRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
