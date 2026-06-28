using CreationsForge.Specification.Records;

namespace CreationsForge.Specification.Validation;

/// <summary>
/// Describes one Spriggit sample and the DTO/UI validation rules that should be evaluated for that sample.
/// </summary>
public class ValidationSpec
{
    /// <summary>
    /// Gets the game whose imported data and Spriggit extraction should be used for the validation sample.
    /// </summary>
    public required SpecificationGame Game { get; init; }

    /// <summary>
    /// Gets the CreationsForge record type represented by the Spriggit sample.
    /// </summary>
    public required RecordSpecification RecordType { get; init; }

    /// <summary>
    /// Gets the Spriggit YAML sample name, without requiring callers to know the full local extraction path.
    /// </summary>
    public required string SampleName { get; init; }

    /// <summary>
    /// Gets the expected form key for the imported DTO and Spriggit sample.
    /// </summary>
    public required string FormKey { get; init; }

    /// <summary>
    /// Gets the Spriggit-to-DTO validation rules for import and repository readback validation.
    /// </summary>
    public IList<ValidationFieldRule> Rules { get; } = new List<ValidationFieldRule>();

    /// <summary>
    /// Gets optional comparison UI expectations that reuse this spec's sample, DTO, and Spriggit mappings.
    /// </summary>
    public IList<ValidationUiComparisonExpectation> UiComparisonExpectations { get; } =
        new List<ValidationUiComparisonExpectation>();
}
