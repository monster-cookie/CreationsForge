using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Records;

namespace SFRecordCompareEngine.ViewModels;

public class RecordComparisonCellViewModel
{
    public RecordComparisonFieldDisplayKind DisplayKind { get; set; }
    public string TextValue { get; set; } = string.Empty;
    public bool? BooleanValue { get; set; }
    public IList<RecordComparisonFieldNodeDTO> TreeNodes { get; set; } = new List<RecordComparisonFieldNodeDTO>();
}
