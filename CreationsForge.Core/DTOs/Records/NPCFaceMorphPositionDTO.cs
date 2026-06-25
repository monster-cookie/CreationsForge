using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents one simple face morph position row.
/// </summary>
public class NPCFaceMorphPositionDTO
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
    /// Gets or sets the zero-based row index.
    /// </summary>
    public int FaceMorphIndex { get; set; }

    /// <summary>
    /// Gets or sets the source morph index.
    /// </summary>
    public int? Index { get; set; }

    /// <summary>
    /// Gets or sets the position value, including vector text when Spriggit exports a vector.
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Gets or sets the optional rotation vector text exported for Fallout 4 face morph positions.
    /// </summary>
    public string? Rotation { get; set; }

    /// <summary>
    /// Gets or sets the optional scale value.
    /// </summary>
    public double? Scale { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp for the import that produced this row.
    /// </summary>
    public DateTime ImportedAtUTC { get; set; }
}
