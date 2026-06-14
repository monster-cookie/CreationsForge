using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class RawRecordPayloadImportService : IRawRecordPayloadImportService
{
    private readonly IRawRecordPayloadRepository RawRecordPayloadRepository;

    public RawRecordPayloadImportService(IRawRecordPayloadRepository rawRecordPayloadRepository)
    {
        RawRecordPayloadRepository = rawRecordPayloadRepository;
    }

    public void ReplaceRawRecordPayloads(IHasRawRecordPayloadsRecordDTO record, string recordType)
    {
        if (record is not RecordDTO recordDTO)
        {
            throw new ArgumentException($"Expected {nameof(RecordDTO)}.", nameof(record));
        }

        RawRecordPayloadRepository.DeleteByRecord(recordDTO.Game, recordDTO.ModKey, recordType, recordDTO.FormKey);

        foreach (var payload in record.RawPayloads)
        {
            payload.Game = recordDTO.Game;
            payload.ModKey = recordDTO.ModKey;
            payload.RecordType = recordType;
            payload.FormKey = recordDTO.FormKey;
            payload.ImportedAtUTC = recordDTO.ImportedAtUTC;
            RawRecordPayloadRepository.Save(payload);
        }
    }
}
