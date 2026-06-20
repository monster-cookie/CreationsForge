using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class FactionRankDTO
{
    public SupportedGame Game { get; set; }

    public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

    public FormKeyDTO FormKey { get; set; } = new()
    {
        ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
        Id = 0
    };

    public int RankIndex { get; set; }

    public int? RankNumber { get; set; }

    public TranslatedStringDTO? MaleTitle { get; set; }

    public TranslatedStringDTO? FemaleTitle { get; set; }

    public DateTime ImportedAtUTC { get; set; }
}
