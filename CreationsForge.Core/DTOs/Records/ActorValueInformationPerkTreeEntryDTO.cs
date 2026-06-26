using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.DTOs.Records.Metadata;

namespace CreationsForge.Core.DTOs.Records;

public class ActorValueInformationPerkTreeEntryDTO
{
    public SupportedGame Game { get; set; }

    public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

    public FormKeyDTO FormKey { get; set; } = new()
    {
        ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
        Id = 0
    };

    public int PerkTreeIndex { get; set; }

    [FormKeyColumnPrefix("AssociatedSkill")]
    public FormKeyDTO? AssociatedSkill { get; set; }

    public string? FNAM { get; set; }

    public double? HorizontalPosition { get; set; }

    public int? Index { get; set; }

    public int? PerkGridX { get; set; }

    public int? PerkGridY { get; set; }

    public double? VerticalPosition { get; set; }

    [FormKeyColumnPrefix("Perk")]
    public FormKeyDTO? Perk { get; set; }

    public IList<ActorValueInformationConnectionLineIndexDTO> ConnectionLineToIndices { get; set; } = new List<ActorValueInformationConnectionLineIndexDTO>();

    public DateTime ImportedAtUTC { get; set; }
}
