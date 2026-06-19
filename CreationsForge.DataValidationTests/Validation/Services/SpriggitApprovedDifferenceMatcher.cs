using CreationsForge.DataValidationTests.Validation.Models;
using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Services;

public class SpriggitApprovedDifferenceMatcher
{
    private readonly IReadOnlyList<SpriggitApprovedDifference> approvedDifferences;

    public SpriggitApprovedDifferenceMatcher(IReadOnlyList<SpriggitApprovedDifference> approvedDifferences)
    {
        this.approvedDifferences = approvedDifferences;
    }

    public SpriggitApprovedDifference? Find(SupportedGame game, string recordType, string formKey, string fieldPath)
    {
        return approvedDifferences.FirstOrDefault(difference =>
            string.Equals(difference.Game, game.ToString(), StringComparison.OrdinalIgnoreCase) &&
            string.Equals(difference.RecordType, recordType, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(difference.FieldPath, fieldPath, StringComparison.OrdinalIgnoreCase) &&
            (string.IsNullOrWhiteSpace(difference.FormKey) ||
             string.Equals(difference.FormKey, formKey, StringComparison.OrdinalIgnoreCase)));
    }
}
