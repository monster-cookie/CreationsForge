namespace CreationsForge.Core.DTOs.Records;

public class RecordComparisonFieldDTO
{
    public required string FieldName { get; set; }

    public bool IsComparable { get; set; } = true;

    public RecordComparisonValueState State { get; set; }

    public IReadOnlyList<RecordComparisonValueDTO> Values { get; set; } = [];

    public IReadOnlyList<RecordComparisonFieldDTO> Children { get; set; } = [];
}
