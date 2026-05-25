using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;

namespace SFRecordCompareEngine.Core.Importers;

public class MiscItemRecordImporter(IMiscItemRepository miscItemRepository) : ITypedRecordDetailImporter
{
    public string RecordType => "MiscItem";
    public string TableName => "MiscItem";

    public void Import(IDatabase database, ModKey modKey, string formId, RecordEnumerationDTO record, string importedAtUtc)
    {
        miscItemRepository.Upsert(database, new MiscItemDTO
        {
            ModKey = modKey,
            FormID = formId,
            Name = RecordDetailValueMapper.GetTextValue(record.Record, "Name"),
            ObjectBounds = RecordDetailValueMapper.GetTextValue(record.Record, "ObjectBounds"),
            Model = RecordDetailValueMapper.GetTextValue(record.Record, "Model"),
            Destructible = RecordDetailValueMapper.GetTextValue(record.Record, "Destructible"),
            ImportedAtUtc = importedAtUtc
        });

        miscItemRepository.ReplaceKeywords(database, modKey, formId, RecordDetailValueMapper.GetKeywords(record.Record, modKey, formId, importedAtUtc));
    }
}
