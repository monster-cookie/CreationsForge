using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.Services.Interfaces;

public interface IRawRecordPayloadImportService
{
    void ReplaceRawRecordPayloads(IHasRawRecordPayloadsRecordDTO record, string recordType);
}
