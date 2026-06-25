using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents role-specific template actor references exported under Spriggit's <c>TemplateActors</c> object.
/// </summary>
public class NPCTemplateActorsDTO
{
    /// <summary>
    /// Gets or sets the traits template actor reference.
    /// </summary>
    public FormKeyDTO? TraitTemplate { get; set; }

    /// <summary>
    /// Gets or sets the stats template actor reference.
    /// </summary>
    public FormKeyDTO? StatsTemplate { get; set; }

    /// <summary>
    /// Gets or sets the factions template actor reference.
    /// </summary>
    public FormKeyDTO? FactionsTemplate { get; set; }

    /// <summary>
    /// Gets or sets the spell list template actor reference.
    /// </summary>
    public FormKeyDTO? SpellListTemplate { get; set; }

    /// <summary>
    /// Gets or sets the AI packages template actor reference.
    /// </summary>
    public FormKeyDTO? AiPackagesTemplate { get; set; }

    /// <summary>
    /// Gets or sets the AI data template actor reference.
    /// </summary>
    public FormKeyDTO? AiDataTemplate { get; set; }

    /// <summary>
    /// Gets or sets the base data template actor reference.
    /// </summary>
    public FormKeyDTO? BaseDataTemplate { get; set; }

    /// <summary>
    /// Gets or sets the inventory template actor reference.
    /// </summary>
    public FormKeyDTO? InventoryTemplate { get; set; }

    /// <summary>
    /// Gets or sets the script template actor reference.
    /// </summary>
    public FormKeyDTO? ScriptTemplate { get; set; }

    /// <summary>
    /// Gets or sets the default package list template actor reference.
    /// </summary>
    public FormKeyDTO? DefPackListTemplate { get; set; }

    /// <summary>
    /// Gets or sets the attack data template actor reference.
    /// </summary>
    public FormKeyDTO? AttackDataTemplate { get; set; }

    /// <summary>
    /// Gets or sets the keywords template actor reference.
    /// </summary>
    public FormKeyDTO? KeywordsTemplate { get; set; }

    /// <summary>
    /// Gets or sets the first Starfield unknown template actor reference.
    /// </summary>
    public FormKeyDTO? Unknown1 { get; set; }

    /// <summary>
    /// Gets or sets the Starfield unknown template actor reference.
    /// </summary>
    public FormKeyDTO? Unknown2 { get; set; }
}
