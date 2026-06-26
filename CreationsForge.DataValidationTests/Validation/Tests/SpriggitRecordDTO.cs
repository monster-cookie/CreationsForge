namespace CreationsForge.DataValidationTests.Validation.Tests;

public class SpriggitRecordDTO
{
    public required string FormKey { get; set; }

    public IReadOnlyDictionary<string, string> Fields { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}
