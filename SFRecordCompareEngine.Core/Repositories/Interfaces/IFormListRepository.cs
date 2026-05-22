using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IFormListRepository
{
    void UpsertFormList(IDatabase database, FormListDTO formList);
    void ReplaceItems(IDatabase database, string modKey, string formId, IList<FormListItemDTO> items);
    IList<FormListRecordDTO> GetByModKey(IDatabase database, string modKey);
    FormListRecordDTO? GetByModKeyAndFormId(IDatabase database, string modKey, string formId);
    IList<FormListRecordDTO> GetByHierarchy(IDatabase database, string selectedModKey);
    IList<FormListRecordDTO> GetByHierarchyAndFormId(IDatabase database, string selectedModKey, string formId);
    IList<FormListRecordDTO> SearchByEditorId(IDatabase database, string selectedModKey, string searchText);
}
