using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class StaticCollectionRecordImporter(IStaticCollectionRepository staticCollectionRepository) : ITypedRecordDetailImporter
{
    public string RecordType => "StaticCollection";
    public string TableName => "StaticCollection";

    public void Import(IDatabase database, string modKey, string formId, RecordEnumerationDTO record, string importedAtUtc)
    {
        staticCollectionRepository.Upsert(database, new StaticCollectionDTO
        {
            ModKey = modKey,
            FormID = formId,
            Name = RecordDetailValueMapper.GetTextValue(record.Record, "Name"),
            ObjectBounds = RecordDetailValueMapper.GetTextValue(record.Record, "ObjectBounds"),
            Model = RecordDetailValueMapper.GetTextValue(record.Record, "Model"),
            ImportedAtUtc = importedAtUtc
        });
    }
}
