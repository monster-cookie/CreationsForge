using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.DTOs.Records;

public class PerkBackgroundSkillDTO
{
    public required FormKeyDTO FormKey { get; set; }

    public required FormKeyDTO SkillFormKey { get; set; }

    public int SkillIndex { get; set; }

    public DateTime ImportedAtUTC { get; set; }
}
