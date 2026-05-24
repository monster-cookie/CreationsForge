namespace SFRecordCompareEngine.Core.DTOs.Records;

public class RecordEnumerationDTO
{
    public required string RecordType { get; set; }
    public required object Record { get; set; }
    public IList<CellGroupLocationDTO> CellGroupLocations { get; set; } = new List<CellGroupLocationDTO>();
}
