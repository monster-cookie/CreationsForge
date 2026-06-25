namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents Skyrim player-skill values exported for an NPC.
/// </summary>
public class NPCPlayerSkillsDTO
{
    /// <summary>
    /// Gets or sets the skill value rows.
    /// </summary>
    public IList<NPCPlayerSkillValueDTO> SkillValues { get; set; } = new List<NPCPlayerSkillValueDTO>();

    /// <summary>
    /// Gets or sets the skill offset rows.
    /// </summary>
    public IList<NPCPlayerSkillValueDTO> SkillOffsets { get; set; } = new List<NPCPlayerSkillValueDTO>();

    /// <summary>
    /// Gets or sets the health value.
    /// </summary>
    public int? Health { get; set; }

    /// <summary>
    /// Gets or sets the magicka value.
    /// </summary>
    public int? Magicka { get; set; }

    /// <summary>
    /// Gets or sets the stamina value.
    /// </summary>
    public int? Stamina { get; set; }

    /// <summary>
    /// Gets or sets the geared-up weapons count.
    /// </summary>
    public int? GearedUpWeapons { get; set; }
}
