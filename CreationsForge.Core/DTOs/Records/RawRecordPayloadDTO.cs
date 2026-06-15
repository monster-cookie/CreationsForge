using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class RawRecordPayloadDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required string RecordType { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required string PayloadSlot { get; set; }

    public int PayloadIndex { get; set; }

    public required string PayloadType { get; set; }

    public string? SourcePath { get; set; }

    public string? PayloadValue { get; set; }

    public required DateTime ImportedAtUTC { get; set; }
}
