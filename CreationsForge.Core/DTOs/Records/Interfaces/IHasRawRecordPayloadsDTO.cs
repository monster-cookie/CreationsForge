namespace CreationsForge.Core.DTOs.Records.Interfaces;

public interface IHasRawRecordPayloadsDTO
{
    IList<RawRecordPayloadDTO> RawPayloads { get; set; }
}
