namespace SFRecordCompareEngine.Core.DTOs.Records;

public class RecordComparisonFieldNodeDTO
{
    public required string Name { get; set; }
    public string? Value { get; set; }
    public IList<RecordComparisonFieldNodeDTO> Children { get; set; } = new List<RecordComparisonFieldNodeDTO>();
}