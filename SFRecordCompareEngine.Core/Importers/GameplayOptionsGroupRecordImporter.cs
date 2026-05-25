using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;

namespace SFRecordCompareEngine.Core.Importers;

public class GameplayOptionsGroupRecordImporter(IGameplayOptionsGroupRepository gameplayOptionsGroupRepository) : ITypedRecordDetailImporter
{
    public string RecordType => "GameplayOptionsGroup";
    public string TableName => "GameplayOptionsGroup";

    public void Import(IDatabase database, ModKey modKey, string formId, RecordEnumerationDTO record, string importedAtUtc)
    {
        gameplayOptionsGroupRepository.Upsert(database, new GameplayOptionsGroupDTO
        {
            ModKey = modKey,
            FormID = formId,
            Name = RecordDetailValueMapper.GetTextValue(record.Record, "Name"),
            ImportedAtUtc = importedAtUtc
        });
    }
}
