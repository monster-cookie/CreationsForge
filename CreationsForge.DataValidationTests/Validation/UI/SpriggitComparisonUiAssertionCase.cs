namespace CreationsForge.DataValidationTests.Validation.UI;

/// <summary>
/// Represents one assertion produced by the headless comparison UI validation runner.
/// </summary>
public class SpriggitComparisonUiAssertionCase
{
    /// <summary>
    /// Gets the expected assertion value.
    /// </summary>
    public required string Expected { get; init; }

    /// <summary>
    /// Gets the actual assertion value observed from the rendered UI or supporting validation state.
    /// </summary>
    public required string Actual { get; init; }

    /// <summary>
    /// Gets the failure message that explains the rendered sample and row under validation.
    /// </summary>
    public required string Message { get; init; }
}
