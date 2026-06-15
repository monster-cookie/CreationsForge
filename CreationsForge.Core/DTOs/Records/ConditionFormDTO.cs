using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class ConditionFormDTO : RecordDTO, IHasRawRecordPayloadsRecordDTO
{
    public int? Version2 { get; set; }

    public IList<RawRecordPayloadDTO> RawPayloads { get; set; } = new List<RawRecordPayloadDTO>();
}
