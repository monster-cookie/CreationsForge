namespace CreationsForge.DataValidationTests.Validation.UI;

/// <summary>
/// Captures one rendered comparison row path and the active plugin column value observed by headless UI validation.
/// </summary>
public class SpriggitComparisonUiRenderedRow
{
    /// <summary>
    /// Initializes a rendered comparison row snapshot.
    /// </summary>
    /// <param name="path">The nested comparison row labels from root to leaf.</param>
    /// <param name="displayValue">The active plugin column display value rendered for the row.</param>
    public SpriggitComparisonUiRenderedRow(IReadOnlyList<string> path, string displayValue)
    {
        Path = path;
        DisplayValue = displayValue;
    }

    /// <summary>
    /// Gets the nested comparison row labels from root to leaf.
    /// </summary>
    public IReadOnlyList<string> Path { get; }

    /// <summary>
    /// Gets the active plugin column display value rendered for the row.
    /// </summary>
    public string DisplayValue { get; }

    /// <summary>
    /// Gets the path formatted for diagnostics.
    /// </summary>
    public string FormattedPath => string.Join("/", Path);
}
