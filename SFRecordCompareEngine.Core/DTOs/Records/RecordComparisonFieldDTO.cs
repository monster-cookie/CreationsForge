namespace SFRecordCompareEngine.Core.DTOs.Records;

public class RecordComparisonFieldDTO
{
    public required string FieldName { get; set; }
    public IDictionary<string, string?> ValuesByPlugin { get; set; } = new Dictionary<string, string?>();
}
