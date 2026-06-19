namespace CreationsForge.DataValidationTests.Validation.Tests;

public class GlobalSpriggitDTO
{
    public required string FormKey { get; set; }

    public int? MajorRecordFlagsRaw { get; set; }

    public int? FormVersion { get; set; }

    public double? Data { get; set; }

    public IReadOnlyDictionary<string, string> Fields { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}
