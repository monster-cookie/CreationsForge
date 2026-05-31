using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("Perk")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_Id", AutoIncrement = false)]
public class Perk : RecordHeader
{
    public Perk()
    { }

    public Perk(PerkDTO dto)
    {
        MapHeader(dto);
        Name = dto.Name;
    }

    [Column("Name")] public string? Name { get; set; }
}
