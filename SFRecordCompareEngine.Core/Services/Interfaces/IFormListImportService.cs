using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IFormListImportService
{
    Task<FormListImportResultDTO> ImportForPluginHierarchyAsync(string selectedModKey, CancellationToken cancellationToken);
    IList<FormListRecordDTO> GetFormListsForPlugin(string modKey);
    FormListRecordDTO? GetFormList(string modKey, string formId);
    IList<FormListRecordDTO> GetFormListsForHierarchy(string selectedModKey);
    IList<FormListRecordDTO> GetMatchingFormListsForHierarchy(string selectedModKey, string formId);
    IList<FormListRecordDTO> SearchFormListsByEditorId(string selectedModKey, string searchText);
}
