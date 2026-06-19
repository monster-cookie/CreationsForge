using CreationsForge.Core.DTOs.Records;

namespace CreationsForge.Core.DTOs.Records.Interfaces;

public interface IHasComponentsRecordDTO
{
    IList<RecordComponentDTO> Components { get; set; }
}
