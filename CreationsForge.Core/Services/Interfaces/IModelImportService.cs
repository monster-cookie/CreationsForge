using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.Services.Interfaces;

public interface IModelImportService
{
    void ReplaceRecordModels(IHasModelsRecordDTO record, string recordType);
}
