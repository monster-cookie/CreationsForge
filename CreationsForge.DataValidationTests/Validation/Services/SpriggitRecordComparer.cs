using CreationsForge.DataValidationTests.Validation.Models;
using CreationsForge.DataValidationTests.Validation.Parsing;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Services;

public class SpriggitRecordComparer
{
    private readonly DtoFlattener dtoFlattener = new();
    private readonly SpriggitFieldPathMapper pathMapper = new();
    private readonly SpriggitValueNormalizer normalizer = new();
    private readonly SpriggitApprovedDifferenceMatcher approvedDifferenceMatcher;

    public SpriggitRecordComparer(SpriggitApprovedDifferenceMatcher approvedDifferenceMatcher)
    {
        this.approvedDifferenceMatcher = approvedDifferenceMatcher;
    }

    public SpriggitSampleValidationResult Compare(
        SupportedGame game,
        SpriggitValidationSample sample,
        SpriggitYamlDocument spriggit,
        RecordDTO record)
    {
        var result = new SpriggitSampleValidationResult
        {
            Sample = sample
        };
        var spriggitValues = spriggit.FlattenScalars();
        var dtoValues = dtoFlattener.Flatten(record);

        foreach (var spriggitPair in spriggitValues.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            var spriggitPath = spriggitPair.Key;
            var dtoPath = pathMapper.Map(spriggitPath);
            var approvedDifference = approvedDifferenceMatcher.Find(game, sample.RecordType, sample.FormKey, spriggitPath);
            if (!dtoValues.TryGetValue(dtoPath, out var dtoValue))
            {
                result.Comparisons.Add(new SpriggitFieldComparison
                {
                    FieldPath = spriggitPath,
                    Category = approvedDifference is null
                        ? SpriggitValidationCategory.MissingInCreationsForge
                        : SpriggitValidationCategory.ApprovedDifference,
                    SpriggitValue = spriggitPair.Value,
                    Notes = approvedDifference?.Reason ?? $"Expected DTO path '{dtoPath}' was not populated."
                });
                continue;
            }

            var normalizedSpriggitValue = normalizer.Normalize(spriggitPath, spriggitPair.Value);
            var normalizedDtoValue = normalizer.Normalize(dtoPath, dtoValue);
            var category = string.Equals(spriggitPair.Value, dtoValue, StringComparison.Ordinal)
                ? SpriggitValidationCategory.Match
                : string.Equals(normalizedSpriggitValue, normalizedDtoValue, StringComparison.Ordinal)
                    ? SpriggitValidationCategory.EquivalentAfterNormalization
                    : SpriggitValidationCategory.ValueMismatch;

            if (approvedDifference is not null && category is not SpriggitValidationCategory.Match)
            {
                category = SpriggitValidationCategory.ApprovedDifference;
            }

            result.Comparisons.Add(new SpriggitFieldComparison
            {
                FieldPath = spriggitPath,
                Category = category,
                SpriggitValue = spriggitPair.Value,
                CreationsForgeValue = dtoValue,
                NormalizedSpriggitValue = normalizedSpriggitValue,
                NormalizedCreationsForgeValue = normalizedDtoValue,
                Notes = approvedDifference?.Reason
            });
        }

        foreach (var dtoPair in dtoValues.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (spriggitValues.ContainsKey(dtoPair.Key))
            {
                continue;
            }

            var matchingSpriggitPath = spriggitValues.Keys.FirstOrDefault(path =>
                string.Equals(pathMapper.Map(path), dtoPair.Key, StringComparison.OrdinalIgnoreCase));
            if (matchingSpriggitPath is not null)
            {
                continue;
            }

            result.Comparisons.Add(new SpriggitFieldComparison
            {
                FieldPath = dtoPair.Key,
                Category = SpriggitValidationCategory.MissingInSpriggit,
                CreationsForgeValue = dtoPair.Value,
                Notes = "Creations Forge exposes this DTO field, but the selected Spriggit sample does not contain a comparable path."
            });
        }

        return result;
    }
}
