using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("MiscItem")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_Id", AutoIncrement = false)]
public class MiscItem : RecordHeader
{
    public MiscItem()
    { }

    public MiscItem(MiscItemDTO dto)
    {
        MapHeader(dto);
        Name = dto.Name;
    }

    [Column("Name")] public string? Name { get; set; }
}
