using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;

namespace SFRecordCompareEngine.Core.Importers;

public class KeywordRecordImporter(IKeywordRepository keywordRepository) : ITypedRecordDetailImporter
{
    public string RecordType => "Keyword";
    public string TableName => "Keyword";

    public void Import(IDatabase database, string modKey, string formId, RecordEnumerationDTO record, string importedAtUtc)
    {
        keywordRepository.Upsert(database, new KeywordDTO
        {
            ModKey = modKey,
            FormID = formId,
            Name = RecordDetailValueMapper.GetTextValue(record.Record, "Name"),
            Color = RecordDetailValueMapper.GetTextValue(record.Record, "Color"),
            KeywordType = RecordDetailValueMapper.GetTextValue(record.Record, "Type"),
            FNAM = RecordDetailValueMapper.GetTextValue(record.Record, "FNAM"),
            ImportedAtUtc = importedAtUtc
        });
    }
}
