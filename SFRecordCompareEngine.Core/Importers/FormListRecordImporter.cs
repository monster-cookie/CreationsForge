using System.Collections;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;

namespace SFRecordCompareEngine.Core.Importers;

public class FormListRecordImporter(IFormListRepository formListRepository)
{
    public int Import(NPoco.IDatabase database, string modKey, string formId, object record, string importedAtUtc)
    {
        formListRepository.UpsertFormList(database, new FormListDTO
        {
            ModKey = modKey,
            FormID = formId,
            AddToListFormKey = ExtractReferenceText(RecordHeaderMapper.GetPropertyValue(record, "AddToList")),
            ImportedAtUtc = importedAtUtc
        });

        var items = GetItems(record, modKey, formId, importedAtUtc);
        formListRepository.ReplaceItems(database, modKey, formId, items);

        return items.Count;
    }

    private static IList<FormListItemDTO> GetItems(object record, string modKey, string formId, string importedAtUtc)
    {
        if (RecordHeaderMapper.GetPropertyValue(record, "Items") is not IEnumerable items)
        {
            return new List<FormListItemDTO>();
        }

        return items
            .Cast<object?>()
            .Select((item, index) => new FormListItemDTO
            {
                ModKey = modKey,
                FormID = formId,
                ItemIndex = index,
                ItemFormKey = ExtractReferenceText(item) ?? throw new InvalidOperationException($"FormList {modKey}:{formId} item {index} did not expose a FormKey."),
                ImportedAtUtc = importedAtUtc
            })
            .ToList();
    }

    private static string? ExtractReferenceText(object? value)
    {
        if (value is null) return null;

        var rawValue = value.ToString();
        if (string.IsNullOrWhiteSpace(rawValue)) return null;

        var normalizedValue = FormKeyTextNormalizer.NormalizeReferenceValue(rawValue);
        return normalizedValue.Equals("Null", StringComparison.OrdinalIgnoreCase)
               || normalizedValue.Equals("NullReference", StringComparison.OrdinalIgnoreCase)
            ? null
            : normalizedValue;
    }
}
