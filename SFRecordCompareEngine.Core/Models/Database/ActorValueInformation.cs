using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("ActorValueInformation")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_Id", AutoIncrement = false)]
public class ActorValueInformation : RecordHeader
{
    public ActorValueInformation()
    { }

    public ActorValueInformation(ActorValueInformationDTO dto)
    {
        MapHeader(dto);
        Name = dto.Name;
    }

    [Column("Name")] public string? Name { get; set; }
}
