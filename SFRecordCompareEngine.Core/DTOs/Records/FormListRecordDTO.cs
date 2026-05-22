namespace SFRecordCompareEngine.Core.DTOs.Records;

public class FormListRecordDTO
{
    public required RecordHeaderDTO Header { get; set; }
    public required FormListDTO FormList { get; set; }
    public IList<FormListItemDTO> Items { get; set; } = new List<FormListItemDTO>();
    public int? EffectiveLoadOrderIndex { get; set; }
}
