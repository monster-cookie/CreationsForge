using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("Global")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_Id", AutoIncrement = false)]
public class Global : RecordHeader
{
    public Global()
    { }

    public Global(GlobalDTO dto)
    {
        MapHeader(dto);
        Data = dto.Data;
    }

    [Column("Data")] public string? Data { get; set; }
}
