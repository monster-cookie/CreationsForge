namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents one Skyrim player-skill key/value entry.
/// </summary>
public class NPCPlayerSkillValueDTO
{
    /// <summary>
    /// Gets or sets the zero-based row index in the source list.
    /// </summary>
    public int SkillIndex { get; set; }

    /// <summary>
    /// Gets or sets the skill key.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the skill value.
    /// </summary>
    public int? Value { get; set; }
}
