using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class ModelRecordDTO : RecordDTO, IHasModelsDTO
{
    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();
}
