using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface ITypedRecordDetailImporter
{
    string RecordType { get; }
    string TableName { get; }
    void Import(IDatabase database, string modKey, string formId, RecordEnumerationDTO record, string importedAtUtc);
}
