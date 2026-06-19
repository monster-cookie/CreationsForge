namespace CreationsForge.DataValidationTests.Validation.Models;

public class SpriggitApprovedDifferenceSet
{
    public int SchemaVersion { get; set; } = 1;

    public IList<SpriggitApprovedDifference> ApprovedDifferences { get; set; } = new List<SpriggitApprovedDifference>();
}
