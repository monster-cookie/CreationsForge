namespace CreationsForge.DataValidationTests.Validation.UI;

/// <summary>
/// Describes one comparison UI coverage audit finding for a Spriggit validation spec.
/// </summary>
public class SpriggitComparisonUiCoverageDiagnostic
{
    /// <summary>
    /// Gets or sets the validation game name.
    /// </summary>
    public required string Game { get; init; }

    /// <summary>
    /// Gets or sets the record type identifier.
    /// </summary>
    public required string RecordType { get; init; }

    /// <summary>
    /// Gets or sets the validation sample name.
    /// </summary>
    public required string SampleName { get; init; }

    /// <summary>
    /// Gets or sets the audit category.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Gets or sets the DTO assertion path related to the finding, when one applies.
    /// </summary>
    public string? DtoPath { get; init; }

    /// <summary>
    /// Gets or sets whether the finding is high-confidence enough to fail the audit test.
    /// </summary>
    public bool IsBlocking { get; init; }

    /// <summary>
    /// Gets or sets the human-readable diagnostic text.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Formats the diagnostic for grouped test failure output.
    /// </summary>
    /// <returns>A single-line diagnostic containing sample identity and message text.</returns>
    public string Format()
    {
        var dtoPathText = string.IsNullOrWhiteSpace(DtoPath)
            ? string.Empty
            : " DTO=" + DtoPath;
        return Game + " " + RecordType + " " + SampleName + " [" + Category + "]" + dtoPathText + ": " + Message;
    }
}
