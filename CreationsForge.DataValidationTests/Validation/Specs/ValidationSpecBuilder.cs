using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs;

public class ValidationSpecBuilder
{
    private readonly SupportedGame game;
    private readonly RecordTypeData recordType;
    private readonly IList<ValidationFieldRule> rules = new List<ValidationFieldRule>();
    private string sampleName = string.Empty;
    private string formKey = string.Empty;

    private ValidationSpecBuilder(SupportedGame game, RecordTypeData recordType)
    {
        this.game = game;
        this.recordType = recordType;
    }

    public static ValidationSpecBuilder ForRecord(SupportedGame game, RecordTypeData recordType)
    {
        return new ValidationSpecBuilder(game, recordType);
    }

    public ValidationSpecBuilder Sample(string value)
    {
        sampleName = value;
        return this;
    }

    public ValidationSpecBuilder FormKey(string value)
    {
        formKey = value;
        return this;
    }

    public ValidationSpecBuilder AddRule(ValidationFieldRule rule)
    {
        rules.Add(rule);
        return this;
    }

    public ValidationSpecBuilder AddRules(IEnumerable<ValidationFieldRule> values)
    {
        foreach (var rule in values)
        {
            rules.Add(rule);
        }

        return this;
    }

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

        return spec;
    }
}
