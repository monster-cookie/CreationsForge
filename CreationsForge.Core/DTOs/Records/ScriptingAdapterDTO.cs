using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class ScriptingAdapterDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required string RecordType { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required string Name { get; set; }

    public required int ScriptIndex { get; set; }

    public required DateTime ImportedAtUTC { get; set; }

    public IList<ScriptingAdapterPropertyDTO> Properties { get; set; } = new List<ScriptingAdapterPropertyDTO>();
}
