using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public abstract class RecordDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required string EditorID { get; set; }

    public required int FormVersion { get; set; }

    public required int MajorRecordFlags { get; set; }

    public required DateTime ImportedAtUTC { get; set; }
}
