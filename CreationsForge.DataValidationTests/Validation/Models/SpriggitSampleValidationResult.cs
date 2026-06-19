namespace CreationsForge.DataValidationTests.Validation.Models;

public class SpriggitSampleValidationResult
{
    public required SpriggitValidationSample Sample { get; set; }

    public IList<SpriggitFieldComparison> Comparisons { get; set; } = new List<SpriggitFieldComparison>();

    public IList<string> Errors { get; set; } = new List<string>();

    public bool HasFailingFindings
    {
        get
        {
            return Errors.Count > 0 ||
                   Comparisons.Any(comparison =>
                       comparison.Category is SpriggitValidationCategory.MissingInCreationsForge
                           or SpriggitValidationCategory.ValueMismatch
                           or SpriggitValidationCategory.TypeMismatch
                           or SpriggitValidationCategory.CollectionCountMismatch
                           or SpriggitValidationCategory.CollectionOrderMismatch
                           or SpriggitValidationCategory.Error);
        }
    }
}
