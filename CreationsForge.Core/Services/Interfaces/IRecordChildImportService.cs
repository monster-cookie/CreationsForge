using CreationsForge.Core.DTOs.Records;

namespace CreationsForge.Core.Services.Interfaces;

public interface IRecordChildImportService
{
    void ReplaceRecordChildren(RecordDTO record, string recordType);
}
