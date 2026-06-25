namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents the Spriggit NPC level object for fixed-level and player-level-mult records.
/// </summary>
public class NPCLevelDTO
{
    /// <summary>
    /// Gets or sets the concrete Mutagen level object type name exported by Spriggit.
    /// </summary>
    public string? MutagenObjectType { get; set; }

    /// <summary>
    /// Gets or sets the fixed level value for <c>NpcLevel</c> records.
    /// </summary>
    public int? Level { get; set; }

    /// <summary>
    /// Gets or sets the player-level multiplier for <c>PcLevelMult</c> records.
    /// </summary>
    public double? LevelMult { get; set; }
}
