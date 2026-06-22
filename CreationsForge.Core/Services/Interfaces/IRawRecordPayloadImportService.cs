using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.Services.Interfaces;

public interface IRawRecordPayloadImportService
{
    void ReplaceRawRecordPayloads(IHasRawRecordPayloadsDTO record, string recordType);
}
