using System.Diagnostics.CodeAnalysis;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class ActorValueInformationDTO : RecordHeaderDTO
{
    public ActorValueInformationDTO()
    { }

    [SetsRequiredMembers]
    public ActorValueInformationDTO(ActorValueInformation model)
        : base(model)
    {
        Name = model.Name;
    }

    public string? Name { get; set; }
}
