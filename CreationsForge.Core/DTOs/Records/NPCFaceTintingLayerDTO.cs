using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents one Fallout 4 NPC face tinting layer row and its Spriggit-visible state flags.
/// </summary>
public class NPCFaceTintingLayerDTO
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
    /// Gets or sets the zero-based row index in the imported face tinting layer collection.
    /// </summary>
    public int FaceTintingLayerIndex { get; set; }

    /// <summary>
    /// Gets or sets the layer data type name exported by Spriggit, such as <c>Value</c> or <c>ValueAndColor</c>.
    /// </summary>
    public string? DataType { get; set; }

    /// <summary>
    /// Gets or sets the source tint index used by the Fallout 4 face tinting layer.
    /// </summary>
    public int? Index { get; set; }

    /// <summary>
    /// Gets or sets the layer strength value when Spriggit exports one.
    /// </summary>
    public double? Value { get; set; }

    /// <summary>
    /// Gets or sets the Spriggit color text when the layer carries a color value.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the template color index, including negative sentinel values exported by Spriggit.
    /// </summary>
    public int? TemplateColorIndex { get; set; }

    /// <summary>
    /// Gets or sets the TEND data type state flags in the order Spriggit exports them.
    /// </summary>
    public IList<string> TENDDataTypeState { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the UTC timestamp for the import that produced this row.
    /// </summary>
    public DateTime ImportedAtUTC { get; set; }
}
