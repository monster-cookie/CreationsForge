namespace CreationsForge.DataValidationTests.Validation.Specs;

public class ValidationAssertionCase
{
    public required string SpriggitPath { get; init; }

    public required string DtoPath { get; init; }

    public required string Expected { get; init; }

    public required string Actual { get; init; }

    public required string Message { get; init; }
}
