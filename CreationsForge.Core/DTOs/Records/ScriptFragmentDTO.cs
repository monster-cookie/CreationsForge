using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class ScriptFragmentDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required string RecordType { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required string FragmentSlot { get; set; }

    public required int FragmentIndex { get; set; }

    public string? MutagenObjectType { get; set; }

    public string? ScriptName { get; set; }

    public string? FragmentName { get; set; }

    public int? Unknown2 { get; set; }

    public int? ExtraBindDataVersion { get; set; }

    public required DateTime ImportedAtUTC { get; set; }
}
