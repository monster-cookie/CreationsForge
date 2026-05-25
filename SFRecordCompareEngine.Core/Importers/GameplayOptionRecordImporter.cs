using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;

namespace SFRecordCompareEngine.Core.Importers;

public class GameplayOptionRecordImporter(IGameplayOptionRepository gameplayOptionRepository) : ITypedRecordDetailImporter
{
    public string RecordType => "GameplayOption";
    public string TableName => "GameplayOption";

    public void Import(IDatabase database, ModKey modKey, string formId, RecordEnumerationDTO record, string importedAtUtc)
    {
        gameplayOptionRepository.Upsert(database, new GameplayOptionDTO
        {
            ModKey = modKey,
            FormID = formId,
            Name = RecordDetailValueMapper.GetTextValue(record.Record, "Name"),
            ImportedAtUtc = importedAtUtc
        });

        gameplayOptionRepository.ReplaceKeywords(database, modKey, formId, RecordDetailValueMapper.GetKeywords(record.Record, modKey, formId, importedAtUtc));
    }
}
