using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents one Skyrim NPC tint layer row.
/// </summary>
public class NPCTintLayerDTO
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
    /// Gets or sets the zero-based tint layer row index.
    /// </summary>
    public int TintLayerIndex { get; set; }

    /// <summary>
    /// Gets or sets the source tint index.
    /// </summary>
    public int? Index { get; set; }

    /// <summary>
    /// Gets or sets the tint color.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the interpolation value.
    /// </summary>
    public double? InterpolationValue { get; set; }

    /// <summary>
    /// Gets or sets the preset value.
    /// </summary>
    public int? Preset { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp for the import that produced this row.
    /// </summary>
    public DateTime ImportedAtUTC { get; set; }
}
