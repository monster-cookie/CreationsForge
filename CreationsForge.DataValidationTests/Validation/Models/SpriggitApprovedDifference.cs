namespace CreationsForge.DataValidationTests.Validation.Models;

public class SpriggitApprovedDifference
{
    public required string Game { get; set; }

    public required string RecordType { get; set; }

    public string? FormKey { get; set; }

    public required string FieldPath { get; set; }

    public required string Category { get; set; }

    public required string Reason { get; set; }

    public required string DateAdded { get; set; }

    public string? Notes { get; set; }
}
