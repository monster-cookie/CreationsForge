using CreationsForge.Core.DTOs.Records;

namespace CreationsForge.Core.DTOs.Records.Interfaces;

public interface IHasComponentsDTO
{
    IList<RecordComponentDTO> Components { get; set; }
}
