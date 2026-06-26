namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents Skyrim NPC configuration values exported under Spriggit's <c>Configuration</c> object.
/// </summary>
public class NPCConfigurationDTO
{
    /// <summary>
    /// Gets or sets the configuration flag names in Spriggit order.
    /// </summary>
    public IList<string> Flags { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the configuration level object.
    /// </summary>
    public NPCLevelDTO? Level { get; set; }

    /// <summary>
    /// Gets or sets the calculated minimum level.
    /// </summary>
    public int? CalcMinLevel { get; set; }

    /// <summary>
    /// Gets or sets the calculated maximum level.
    /// </summary>
    public int? CalcMaxLevel { get; set; }

    /// <summary>
    /// Gets or sets the health offset used by Skyrim configuration data.
    /// </summary>
    public int? HealthOffset { get; set; }

    /// <summary>
    /// Gets or sets the configured movement speed multiplier.
    /// </summary>
    public int? SpeedMultiplier { get; set; }

    /// <summary>
    /// Gets or sets the template flag names in Spriggit order.
    /// </summary>
    public IList<string> TemplateFlags { get; set; } = new List<string>();
}
