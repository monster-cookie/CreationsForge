using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("Keyword")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_Id", AutoIncrement = false)]
public class Keyword : RecordHeader
{
    public Keyword()
    { }

    public Keyword(KeywordDTO dto)
    {
        MapHeader(dto);
        Name = dto.Name;
    }

    [Column("Name")] public string? Name { get; set; }
}
