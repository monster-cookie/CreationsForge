using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class ActivatorRecordImporter(IActivatorRepository activatorRepository) : ITypedRecordDetailImporter
{
    public string RecordType => "Activator";
    public string TableName => "Activator";

    public void Import(IDatabase database, string modKey, string formId, RecordEnumerationDTO record, string importedAtUtc)
    {
        activatorRepository.Upsert(database, new ActivatorDTO
        {
            ModKey = modKey,
            FormID = formId,
            Name = RecordDetailValueMapper.GetTextValue(record.Record, "Name"),
            ObjectBounds = RecordDetailValueMapper.GetTextValue(record.Record, "ObjectBounds"),
            Model = RecordDetailValueMapper.GetTextValue(record.Record, "Model"),
            Destructible = RecordDetailValueMapper.GetTextValue(record.Record, "Destructible"),
            ImportedAtUtc = importedAtUtc
        });

        activatorRepository.ReplaceKeywords(database, modKey, formId, RecordDetailValueMapper.GetKeywords(record.Record, modKey, formId, importedAtUtc));
    }
}
