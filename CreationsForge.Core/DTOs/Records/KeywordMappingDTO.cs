using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class KeywordMappingDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required string RecordType { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required FormKeyDTO Keyword { get; set; }

    public required int KeywordIndex { get; set; }

    public required DateTime ImportedAtUTC { get; set; }
}
