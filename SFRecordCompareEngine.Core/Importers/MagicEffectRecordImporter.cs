using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;

namespace SFRecordCompareEngine.Core.Importers;

public class MagicEffectRecordImporter(IMagicEffectRepository magicEffectRepository) : ITypedRecordDetailImporter
{
    public string RecordType => "MagicEffect";
    public string TableName => "MagicEffect";

    public void Import(IDatabase database, ModKey modKey, string formId, RecordEnumerationDTO record, string importedAtUtc)
    {
        magicEffectRepository.Upsert(database, new MagicEffectDTO
        {
            ModKey = modKey,
            FormID = formId,
            Name = RecordDetailValueMapper.GetTextValue(record.Record, "Name"),
            ImportedAtUtc = importedAtUtc
        });

        magicEffectRepository.ReplaceKeywords(database, modKey, formId, RecordDetailValueMapper.GetKeywords(record.Record, modKey, formId, importedAtUtc));
    }
}
