using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs;

public class ValidationSpec
{
    public required SupportedGame Game { get; init; }

    public required RecordTypeData RecordType { get; init; }

    public required string SampleName { get; init; }

    public required string FormKey { get; init; }

    public IList<ValidationFieldRule> Rules { get; } = new List<ValidationFieldRule>();
}
