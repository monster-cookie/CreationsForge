using System.Diagnostics.CodeAnalysis;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class GlobalDTO : RecordHeaderDTO
{
    public GlobalDTO()
    { }

    [SetsRequiredMembers]
    public GlobalDTO(Global model)
        : base(model)
    {
        Data = model.Data;
    }

    public string? Data { get; set; }
}
