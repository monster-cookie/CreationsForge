using System.Diagnostics.CodeAnalysis;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class MiscItemDTO : RecordHeaderDTO
{
    public MiscItemDTO()
    { }

    [SetsRequiredMembers]
    public MiscItemDTO(MiscItem model)
        : base(model)
    {
        Name = model.Name;
    }

    public string? Name { get; set; }
}
