namespace CreationsForge.DataValidationTests.Validation.Models;

public class SpriggitFieldComparison
{
    public required string FieldPath { get; set; }

    public required SpriggitValidationCategory Category { get; set; }

    public string? SpriggitValue { get; set; }

    public string? CreationsForgeValue { get; set; }

    public string? NormalizedSpriggitValue { get; set; }

    public string? NormalizedCreationsForgeValue { get; set; }

    public string? Notes { get; set; }
}
