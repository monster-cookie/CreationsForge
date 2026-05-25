using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Importers.Interfaces;

public interface ITypedRecordDetailImporter
{
    string RecordType { get; }
    string TableName { get; }
    void Import(IDatabase database, ModKey modKey, string formId, RecordEnumerationDTO record, string importedAtUtc);
}
