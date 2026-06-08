using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class RecordSoundDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required string RecordType { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required string SoundSlot { get; set; }

    public required int SoundIndex { get; set; }

    public string? Start { get; set; }

    public string? Versioning { get; set; }

    public string? Unknown { get; set; }

    public required DateTime ImportedAtUTC { get; set; }
}
