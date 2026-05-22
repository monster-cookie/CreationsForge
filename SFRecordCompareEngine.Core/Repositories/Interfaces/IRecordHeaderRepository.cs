using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IRecordHeaderRepository
{
    RecordHeaderDTO? GetByFormKey(IDatabase database, string formKey);
    void Upsert(IDatabase database, RecordHeaderDTO recordHeader);
    void DeleteByModKeyAndRecordType(IDatabase database, string modKey, string recordType);
}
