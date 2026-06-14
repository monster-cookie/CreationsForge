using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class FormListImporter : ITypedRecordImporter
{
    private readonly IFormListRepository FormListRepository;
    private readonly IFormListItemRepository FormListItemRepository;
    private readonly IRecordChildImportService RecordChildImportService;

    public FormListImporter(
        IFormListRepository formListRepository,
        IFormListItemRepository formListItemRepository,
        IRecordChildImportService recordChildImportService)
    {
        FormListRepository = formListRepository;
        FormListItemRepository = formListItemRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.FormList.RecordID;

    public string TableName => RecordTypeCatalog.FormList.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame>
    {
        SupportedGame.Starfield,
        SupportedGame.Fallout4,
        SupportedGame.Skyrim
    };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not FormListDTO formList) throw new ArgumentException($"Expected {nameof(FormListDTO)}.", nameof(recordDTO));

        formList.ImportedAtUTC = importedAtUTC;
        FormListRepository.Save(formList);
        RecordChildImportService.ReplaceRecordChildren(formList, RecordTypeCatalog.FormList.RecordID);
        result.DetailRowsImported++;

        var itemIndex = 0;
        foreach (var item in formList.Items)
        {
            item.ImportedAtUTC = importedAtUTC;
            item.ItemIndex = itemIndex;
            FormListItemRepository.Save(item);
            result.FormListItemsImported++;
            itemIndex++;
        }
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        FormListItemRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
        FormListRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
