using NPoco;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface ITypedRecordDetailImporter
{
    string RecordType { get; }
    string TableName { get; }
    void Import(IDatabase database, string modKey, string formId, object record, string importedAtUtc);
}
