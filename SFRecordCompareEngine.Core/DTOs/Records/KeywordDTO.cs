using System.Diagnostics.CodeAnalysis;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class KeywordDTO : RecordHeaderDTO
{
    public KeywordDTO()
    { }

    [SetsRequiredMembers]
    public KeywordDTO(Keyword model)
        : base(model)
    {
        Name = model.Name;
    }

    public string? Name { get; set; }
}
