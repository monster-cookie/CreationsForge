using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine;

public class RecordTypeTreeNode
{
    public required string Name { get; init; }
    public required IList<RecordSummaryDTO> Records { get; init; }
}
