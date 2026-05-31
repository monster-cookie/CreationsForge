using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("Npc")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_Id", AutoIncrement = false)]
public class Npc : RecordHeader
{
    public Npc()
    { }

    public Npc(NpcDTO dto)
    {
        MapHeader(dto);
        Name = dto.Name;
    }

    [Column("Name")] public string? Name { get; set; }
}
