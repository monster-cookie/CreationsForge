using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs;

/// <summary>
/// Builds immutable-enough validation specs while keeping record-specific spec declarations readable.
/// </summary>
public class ValidationSpecBuilder
{
    private readonly SupportedGame game;
    private readonly RecordTypeData recordType;
    private readonly IList<ValidationFieldRule> rules = new List<ValidationFieldRule>();
    private readonly IList<ValidationUiComparisonExpectation> uiComparisonExpectations =
        new List<ValidationUiComparisonExpectation>();
    private string sampleName = string.Empty;
    private string formKey = string.Empty;

    /// <summary>
    /// Initializes a builder for the supplied game and record type.
    /// </summary>
    /// <param name="game">The game whose Spriggit and imported DTO data should be validated.</param>
    /// <param name="recordType">The CreationsForge record type for the validation sample.</param>
    private ValidationSpecBuilder(SupportedGame game, RecordTypeData recordType)
    {
        this.game = game;
        this.recordType = recordType;
    }

    /// <summary>
    /// Creates a builder for one game and record type pair.
    /// </summary>
    /// <param name="game">The game whose imported records and Spriggit extraction should be used.</param>
    /// <param name="recordType">The CreationsForge record type under validation.</param>
    /// <returns>A builder that can be populated with sample, form key, field rules, and UI expectations.</returns>
    public static ValidationSpecBuilder ForRecord(SupportedGame game, RecordTypeData recordType)
    {
        return new ValidationSpecBuilder(game, recordType);
    }

    /// <summary>
    /// Sets the Spriggit sample name resolved by the validation harness.
    /// </summary>
    /// <param name="value">The Spriggit YAML filename stem or configured sample name.</param>
    /// <returns>The current builder for chained spec setup.</returns>
    public ValidationSpecBuilder Sample(string value)
    {
        sampleName = value;
        return this;
    }

    /// <summary>
    /// Sets the form key expected for the Spriggit sample and imported DTO.
    /// </summary>
    /// <param name="value">The form key text in Spriggit form, such as <c>000800:Starfield.esm</c>.</param>
    /// <returns>The current builder for chained spec setup.</returns>
    public ValidationSpecBuilder FormKey(string value)
    {
        formKey = value;
        return this;
    }

    /// <summary>
    /// Adds one Spriggit-to-DTO validation rule.
    /// </summary>
    /// <param name="rule">The rule to evaluate for DTO readback validation.</param>
    /// <returns>The current builder for chained spec setup.</returns>
    public ValidationSpecBuilder AddRule(ValidationFieldRule rule)
    {
        rules.Add(rule);
        return this;
    }

    /// <summary>
    /// Adds multiple Spriggit-to-DTO validation rules in declaration order.
    /// </summary>
    /// <param name="values">The validation rules to append.</param>
    /// <returns>The current builder for chained spec setup.</returns>
    public ValidationSpecBuilder AddRules(IEnumerable<ValidationFieldRule> values)
    {
        foreach (var rule in values)
        {
            rules.Add(rule);
        }

        return this;
    }

    /// <summary>
    /// Adds one comparison UI expectation that should be rendered from the same validation sample.
    /// </summary>
    /// <param name="expectation">The row path and expected value source for the comparison UI.</param>
    /// <returns>The current builder for chained spec setup.</returns>
    public ValidationSpecBuilder AddUiComparisonExpectation(ValidationUiComparisonExpectation expectation)
    {
        uiComparisonExpectations.Add(expectation);
        return this;
    }

    /// <summary>
    /// Adds multiple comparison UI expectations in declaration order.
    /// </summary>
    /// <param name="values">The comparison UI expectations to append.</param>
    /// <returns>The current builder for chained spec setup.</returns>
    public ValidationSpecBuilder AddUiComparisonExpectations(IEnumerable<ValidationUiComparisonExpectation> values)
    {
        foreach (var expectation in values)
        {
            uiComparisonExpectations.Add(expectation);
        }

        return this;
    }

    /// <summary>
    /// Creates the validation spec from the current builder state.
    /// </summary>
    /// <returns>A validation spec containing all configured DTO rules and comparison UI expectations.</returns>
    public ValidationSpec Build()
    {
        var spec = new ValidationSpec
        {
            Game = game,
            RecordType = recordType,
            SampleName = sampleName,
            FormKey = formKey
        };

        foreach (var rule in rules)
        {
            spec.Rules.Add(rule);
        }

        foreach (var expectation in uiComparisonExpectations)
        {
            spec.UiComparisonExpectations.Add(expectation);
        }

        return spec;
    }
}
