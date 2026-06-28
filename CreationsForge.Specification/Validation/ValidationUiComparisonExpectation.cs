namespace CreationsForge.Specification.Validation;

/// <summary>
/// Describes a comparison UI row that should be rendered for a Spriggit validation spec sample.
/// </summary>
public class ValidationUiComparisonExpectation
{
    /// <summary>
    /// Initializes a comparison UI expectation for one row path.
    /// </summary>
    /// <param name="fieldPath">The nested comparison row labels to follow in the rendered UI tree.</param>
    /// <param name="dtoPath">The DTO assertion path whose normalized expected value should match the rendered row.</param>
    /// <param name="expectedDisplayValue">An optional literal display value when no DTO assertion path is available.</param>
    /// <param name="visualText">Optional visual text that should appear in the rendered Avalonia tree.</param>
    public ValidationUiComparisonExpectation(
        IReadOnlyList<string> fieldPath,
        string? dtoPath = null,
        string? expectedDisplayValue = null,
        string? visualText = null)
    {
        FieldPath = fieldPath;
        DtoPath = dtoPath;
        ExpectedDisplayValue = expectedDisplayValue;
        VisualText = visualText;
    }

    /// <summary>
    /// Gets the nested comparison row labels to follow in the rendered UI tree.
    /// </summary>
    public IReadOnlyList<string> FieldPath { get; }

    /// <summary>
    /// Gets the DTO validation assertion path used as the expected display value source when available.
    /// </summary>
    public string? DtoPath { get; }

    /// <summary>
    /// Gets the literal display value expected for the row when no DTO assertion path is available.
    /// </summary>
    public string? ExpectedDisplayValue { get; }

    /// <summary>
    /// Gets optional visual text that should be present in the rendered Avalonia tree.
    /// </summary>
    public string? VisualText { get; }

    /// <summary>
    /// Creates an expectation whose expected display value comes from a DTO validation assertion path.
    /// </summary>
    /// <param name="fieldPath">The nested comparison row labels to follow in the rendered UI tree.</param>
    /// <param name="dtoPath">The DTO assertion path whose expected value should match the rendered row.</param>
    /// <param name="visualText">Optional visual text that should appear in the rendered Avalonia tree.</param>
    /// <returns>A comparison UI expectation bound to the supplied DTO path.</returns>
    public static ValidationUiComparisonExpectation DtoField(
        IReadOnlyList<string> fieldPath,
        string dtoPath,
        string? visualText = null)
    {
        return new ValidationUiComparisonExpectation(fieldPath, dtoPath: dtoPath, visualText: visualText);
    }

    /// <summary>
    /// Creates an expectation whose expected display value is declared directly by the spec.
    /// </summary>
    /// <param name="fieldPath">The nested comparison row labels to follow in the rendered UI tree.</param>
    /// <param name="expectedDisplayValue">The display value expected in the rendered comparison row.</param>
    /// <param name="visualText">Optional visual text that should appear in the rendered Avalonia tree.</param>
    /// <returns>A comparison UI expectation with a literal expected display value.</returns>
    public static ValidationUiComparisonExpectation Literal(
        IReadOnlyList<string> fieldPath,
        string expectedDisplayValue,
        string? visualText = null)
    {
        return new ValidationUiComparisonExpectation(
            fieldPath,
            expectedDisplayValue: expectedDisplayValue,
            visualText: visualText);
    }
}
