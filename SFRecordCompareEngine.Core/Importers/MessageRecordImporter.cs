using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class MessageRecordImporter(IMessageRepository messageRepository) : ITypedRecordDetailImporter
{
    public string RecordType => "Message";
    public string TableName => "Message";

    public void Import(IDatabase database, string modKey, string formId, RecordEnumerationDTO record, string importedAtUtc)
    {
        messageRepository.Upsert(database, new MessageDTO
        {
            ModKey = modKey,
            FormID = formId,
            Name = RecordDetailValueMapper.GetTextValue(record.Record, "Name"),
            ImportedAtUtc = importedAtUtc
        });
    }
}
