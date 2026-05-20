using SFRecordCompareEngine.Core.Models.Records;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class RecordComparisonFieldDTO
{
    public required string FieldName { get; set; }
    public RecordComparisonFieldDisplayKind DisplayKind { get; set; }
    public IDictionary<string, string?> ValuesByPlugin { get; set; } = new Dictionary<string, string?>();
    public IDictionary<string, bool?> BooleanValuesByPlugin { get; set; } = new Dictionary<string, bool?>();

    public IDictionary<string, IList<RecordComparisonFieldNodeDTO>> TreeValuesByPlugin { get; set; } =
        new Dictionary<string, IList<RecordComparisonFieldNodeDTO>>();
}