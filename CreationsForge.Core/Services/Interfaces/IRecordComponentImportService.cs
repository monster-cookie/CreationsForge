using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.Services.Interfaces;

public interface IRecordComponentImportService
{
    void ReplaceRecordComponents(IHasComponentsRecordDTO record, string recordType);
}
