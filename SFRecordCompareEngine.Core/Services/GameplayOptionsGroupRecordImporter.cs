using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class GameplayOptionsGroupRecordImporter(IGameplayOptionsGroupRepository gameplayOptionsGroupRepository) : ITypedRecordDetailImporter
{
    public string RecordType => "GameplayOptionsGroup";
    public string TableName => "GameplayOptionsGroup";

    public void Import(IDatabase database, string modKey, string formId, RecordEnumerationDTO record, string importedAtUtc)
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
