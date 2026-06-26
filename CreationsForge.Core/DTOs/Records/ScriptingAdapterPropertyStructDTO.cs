using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents one struct entry within a VMAD script property struct list.
/// </summary>
public class ScriptingAdapterPropertyStructDTO
{
    /// <summary>
    /// Gets or sets the game that supplied the parent record.
    /// </summary>
    public required SupportedGame Game { get; set; }

    /// <summary>
    /// Gets or sets the plugin that supplied the parent record.
    /// </summary>
    public required ModKeyDTO ModKey { get; set; }

    /// <summary>
    /// Gets or sets the Bethesda record type that owns the VMAD script.
    /// </summary>
    public required string RecordType { get; set; }

    /// <summary>
    /// Gets or sets the form key of the parent record.
    /// </summary>
    public required FormKeyDTO FormKey { get; set; }

    /// <summary>
    /// Gets or sets the name of the parent VMAD script adapter.
    /// </summary>
    public required string ScriptingAdapterName { get; set; }

    /// <summary>
    /// Gets or sets the zero-based parent script property index.
    /// </summary>
    public required int PropertyIndex { get; set; }

    /// <summary>
    /// Gets or sets the zero-based struct index within the parent property.
    /// </summary>
    public required int StructIndex { get; set; }

    /// <summary>
    /// Gets or sets when the parent plugin import produced this row.
    /// </summary>
    public required DateTime ImportedAtUTC { get; set; }

    /// <summary>
    /// Gets or sets the script property members stored inside this struct.
    /// </summary>
    public IList<ScriptingAdapterPropertyStructMemberDTO> Members { get; set; } = new List<ScriptingAdapterPropertyStructMemberDTO>();
}
