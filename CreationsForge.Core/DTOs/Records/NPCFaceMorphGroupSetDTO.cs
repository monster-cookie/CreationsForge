using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents one Starfield face morph entry with nested morph-group blend rows.
/// </summary>
public class NPCFaceMorphGroupSetDTO
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
    /// Gets or sets the zero-based face morph entry index.
    /// </summary>
    public int FaceMorphIndex { get; set; }

    /// <summary>
    /// Gets or sets the source morph index.
    /// </summary>
    public int? Index { get; set; }

    /// <summary>
    /// Gets or sets the nested morph group rows.
    /// </summary>
    public IList<NPCFaceMorphGroupDTO> MorphGroups { get; set; } = new List<NPCFaceMorphGroupDTO>();

    /// <summary>
    /// Gets or sets the UTC timestamp for the import that produced this row.
    /// </summary>
    public DateTime ImportedAtUTC { get; set; }
}
