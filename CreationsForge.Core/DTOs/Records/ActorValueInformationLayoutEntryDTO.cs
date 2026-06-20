using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class ActorValueInformationLayoutEntryDTO
{
    public SupportedGame Game { get; set; }

    public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

    public FormKeyDTO FormKey { get; set; } = new()
    {
        ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
        Id = 0
    };

    public int LayoutIndex { get; set; }

    public FormKeyDTO? AssociatedSkillFormKey { get; set; }

    public string? Fnam { get; set; }

    public double? HorizontalPosition { get; set; }

    public int? Index { get; set; }

    public int? PerkGridX { get; set; }

    public int? PerkGridY { get; set; }

    public double? VerticalPosition { get; set; }

    public DateTime ImportedAtUTC { get; set; }
}
