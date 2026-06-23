namespace CreationsForge.Core.DTOs.Records;

public class MiscItemDestructibleDTO
{
    public MiscItemDestructibleDataDTO? Data { get; set; }

    public IList<MiscItemDestructibleStageDTO> Stages { get; set; } = new List<MiscItemDestructibleStageDTO>();
}
