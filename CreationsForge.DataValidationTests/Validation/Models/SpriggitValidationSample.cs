namespace CreationsForge.DataValidationTests.Validation.Models;

public class SpriggitValidationSample
{
    public required string Game { get; set; }

    public required string RecordType { get; set; }

    public required string Plugin { get; set; }

    public required string FormKey { get; set; }

    public string? EditorId { get; set; }

    public required string SpriggitFile { get; set; }

    public IList<string> CoverageHints { get; set; } = new List<string>();

    public string? Notes { get; set; }
}
