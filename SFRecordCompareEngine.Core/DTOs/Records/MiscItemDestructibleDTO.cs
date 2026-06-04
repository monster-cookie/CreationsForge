namespace SFRecordCompareEngine.Core.DTOs.Records;

public class MiscItemDestructibleDTO
{
    public int? Health { get; set; }
    public byte? Count { get; set; }
    public string? Flags { get; set; }
    public IList<MiscItemDestructibleResistanceDTO> Resistances { get; set; } = new List<MiscItemDestructibleResistanceDTO>();
    public IList<MiscItemDestructionStageDTO> Stages { get; set; } = new List<MiscItemDestructionStageDTO>();
}
