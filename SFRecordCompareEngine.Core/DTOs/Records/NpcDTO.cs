using System.Diagnostics.CodeAnalysis;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class NpcDTO : RecordHeaderDTO
{
    public NpcDTO()
    { }

    [SetsRequiredMembers]
    public NpcDTO(Npc model)
        : base(model)
    {
        Name = model.Name;
    }

    public string? Name { get; set; }
}
