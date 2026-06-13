namespace CreationsForge.Core.DTOs.Records.Interfaces;

public interface IHasRawRecordPayloadsRecordDTO
{
    IList<RawRecordPayloadDTO> RawPayloads { get; set; }
}
