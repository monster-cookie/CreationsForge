using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents one Starfield NPC tint row.
/// </summary>
public class NPCTintDTO
{
    /// <summary>
    /// Gets or sets the game that owns the row.
    /// </summary>
    public SupportedGame Game { get; set; }

    /// <summary>
    /// Gets or sets the plugin that supplied the parent NPC.
    /// </summary>
    public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

    /// <summary>
    /// Gets or sets the parent NPC form key.
    /// </summary>
    public FormKeyDTO FormKey { get; set; } = new() { ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 };

    /// <summary>
    /// Gets or sets the zero-based tint row index.
    /// </summary>
    public int TintIndex { get; set; }

    /// <summary>
    /// Gets or sets the tint type.
    /// </summary>
    public string? TintType { get; set; }

    /// <summary>
    /// Gets or sets the tint group name.
    /// </summary>
    public string? TintGroup { get; set; }

    /// <summary>
    /// Gets or sets the tint name.
    /// </summary>
    public string? TintName { get; set; }

    /// <summary>
    /// Gets or sets the tint texture path.
    /// </summary>
    public string? TintTexture { get; set; }

    /// <summary>
    /// Gets or sets the tint color.
    /// </summary>
    public string? TintColor { get; set; }

    /// <summary>
    /// Gets or sets the tint intensity.
    /// </summary>
    public double? TintIntensity { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp for the import that produced this row.
    /// </summary>
    public DateTime ImportedAtUTC { get; set; }
}
