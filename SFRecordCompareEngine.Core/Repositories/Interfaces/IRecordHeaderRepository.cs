using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IRecordHeaderRepository
{
    RecordHeaderDTO? GetCurrentByFormKey(IDatabase database, string formKey);
    IList<RecordHeaderDTO> GetByHierarchy(IDatabase database, string selectedModKey, string? formId, string? editorId, string? recordType);
    RecordHeaderDTO? GetWinningOverride(IDatabase database, string selectedModKey, string? formId, string? editorId, string? recordType);
    void Upsert(IDatabase database, RecordHeaderDTO recordHeader);
    void DeleteByModKey(IDatabase database, string modKey);
    void DeleteByModKeyAndRecordType(IDatabase database, string modKey, string recordType);
}
