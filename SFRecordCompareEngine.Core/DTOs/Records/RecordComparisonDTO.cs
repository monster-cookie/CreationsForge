namespace SFRecordCompareEngine.Core.DTOs.Records;

public class RecordComparisonDTO
{
    public IList<RecordComparisonPluginDTO> Plugins { get; set; } = new List<RecordComparisonPluginDTO>();
    public IList<RecordComparisonFieldDTO> Fields { get; set; } = new List<RecordComparisonFieldDTO>();
}
