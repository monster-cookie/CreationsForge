using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents one nested Starfield face morph group blend row.
/// </summary>
public class NPCFaceMorphGroupDTO
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
    /// Gets or sets the parent face morph entry index.
    /// </summary>
    public int FaceMorphIndex { get; set; }

    /// <summary>
    /// Gets or sets the zero-based morph group index.
    /// </summary>
    public int MorphGroupIndex { get; set; }

    /// <summary>
    /// Gets or sets the morph group name.
    /// </summary>
    public string? MorphGroup { get; set; }

    /// <summary>
    /// Gets or sets the blend intensity.
    /// </summary>
    public double? BlendIntensity { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp for the import that produced this row.
    /// </summary>
    public DateTime ImportedAtUTC { get; set; }
}
