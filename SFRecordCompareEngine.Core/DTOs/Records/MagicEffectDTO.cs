using System.Diagnostics.CodeAnalysis;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class MagicEffectDTO : RecordHeaderDTO
{
    public MagicEffectDTO()
    { }

    [SetsRequiredMembers]
    public MagicEffectDTO(MagicEffect model)
        : base(model)
    {
        Name = model.Name;
    }

    public string? Name { get; set; }
}
