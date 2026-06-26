using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents one member value inside a VMAD script property struct entry.
/// </summary>
public class ScriptingAdapterPropertyStructMemberDTO
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
    /// Gets or sets the zero-based member index within the parent struct.
    /// </summary>
    public required int MemberIndex { get; set; }

    /// <summary>
    /// Gets or sets the Spriggit/Mutagen member name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the Mutagen script property type name for this member.
    /// </summary>
    public required string MutagenObjectType { get; set; }

    /// <summary>
    /// Gets or sets the Boolean member value when this member stores a Boolean.
    /// </summary>
    public bool? DataBool { get; set; }

    /// <summary>
    /// Gets or sets the integer member value when this member stores an integer.
    /// </summary>
    public int? DataInt { get; set; }

    /// <summary>
    /// Gets or sets the floating-point member value when this member stores a floating-point value.
    /// </summary>
    public double? DataFloat { get; set; }

    /// <summary>
    /// Gets or sets the string member value when this member stores text.
    /// </summary>
    public string? DataString { get; set; }

    /// <summary>
    /// Gets or sets the referenced object form key when this member stores an object reference.
    /// </summary>
    public FormKeyDTO? ObjectFormKey { get; set; }

    /// <summary>
    /// Gets or sets the VMAD alias index when this member stores an alias reference.
    /// </summary>
    public short? ObjectAlias { get; set; }

    /// <summary>
    /// Gets or sets the unused object-reference payload value preserved by Mutagen.
    /// </summary>
    public ushort? ObjectUnused { get; set; }

    /// <summary>
    /// Gets or sets when the parent plugin import produced this row.
    /// </summary>
    public required DateTime ImportedAtUTC { get; set; }
}
