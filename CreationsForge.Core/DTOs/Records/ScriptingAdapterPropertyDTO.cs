using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class ScriptingAdapterPropertyDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required string RecordType { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required string ScriptingAdapterName { get; set; }

    public required int PropertyIndex { get; set; }

    public required string Name { get; set; }

    public required string MutagenObjectType { get; set; }

    public bool? DataBool { get; set; }

    public int? DataInt { get; set; }

    public double? DataFloat { get; set; }

    public string? DataString { get; set; }

    public FormKeyDTO? ObjectFormKey { get; set; }

    public short? ObjectAlias { get; set; }

    public ushort? ObjectUnused { get; set; }

    public required DateTime ImportedAtUTC { get; set; }

    public IList<ScriptingAdapterPropertyListItemDTO> ListItems { get; set; } = new List<ScriptingAdapterPropertyListItemDTO>();

    /// <summary>
    /// Gets or sets struct-list entries owned by this VMAD script property.
    /// </summary>
    public IList<ScriptingAdapterPropertyStructDTO> Structs { get; set; } = new List<ScriptingAdapterPropertyStructDTO>();
}
