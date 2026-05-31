using System.Diagnostics.CodeAnalysis;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class PerkDTO : RecordHeaderDTO
{
    public PerkDTO()
    { }

    [SetsRequiredMembers]
    public PerkDTO(Perk model)
        : base(model)
    {
        Name = model.Name;
    }

    public string? Name { get; set; }
}
