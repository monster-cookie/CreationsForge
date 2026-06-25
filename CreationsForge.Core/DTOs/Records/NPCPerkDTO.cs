using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents one perk row on an NPC.
/// </summary>
public class NPCPerkDTO
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
    /// Gets or sets the zero-based perk row index.
    /// </summary>
    public int PerkIndex { get; set; }

    /// <summary>
    /// Gets or sets the perk reference.
    /// </summary>
    public FormKeyDTO? Perk { get; set; }

    /// <summary>
    /// Gets or sets the perk rank.
    /// </summary>
    public int? Rank { get; set; }

    /// <summary>
    /// Gets or sets the perk fluff value using Spriggit-compatible formatting.
    /// </summary>
    public string? Fluff { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp for the import that produced this row.
    /// </summary>
    public DateTime ImportedAtUTC { get; set; }
}
