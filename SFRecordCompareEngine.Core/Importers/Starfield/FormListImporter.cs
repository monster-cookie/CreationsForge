using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Importers.Starfield;

public class FormListImporter : ITypedRecordDetailImporter
{
    private readonly IFormListItemRepository FormListItemRepository;
    private readonly IFormListRepository FormListRepository;

    public FormListImporter(
        IFormListRepository formListRepository,
        IFormListItemRepository formListItemRepository
    )
    {
        FormListRepository = formListRepository;
        FormListItemRepository = formListItemRepository;
    }

    public GameRelease GameRelease => GameRelease.Starfield;

    public RecordType RecordType => new(RecordTypeCatalog.FormList.RecordID);

    public string TableName => RecordTypeCatalog.FormList.TableName;

    public void Import(object recordDTO, RecordTypeImportResultDTO resultDTO)
    {
        var record = (FormListDTO)recordDTO;
        record.ImportedAtUTC = DateTime.UtcNow;

        FormListRepository.Save(record);
        resultDTO.DetailRowsImported++;
        FormListItemRepository.DeleteByFormList(record.ModKey, record.FormKey);

        var itemIndex = 0;
        foreach (var item in record.Items)
        {
            var formListItemDTO = new FormListItemDTO
            {
                ModKey = record.ModKey,
                FormKey = record.FormKey,
                ItemModKey = item.ItemModKey,
                ItemFormKey = item.ItemFormKey,
                ItemIndex = itemIndex,
                ImportedAtUTC = DateTime.UtcNow
            };
            FormListItemRepository.Save(formListItemDTO);
            resultDTO.FormListItemsImported++;
            itemIndex++;
        }
    }
}