namespace CreationsForge.ViewModels;

/// <summary>Describes the presentation comparison state of one detached JSON field.</summary>
internal enum ComparisonFieldState
{
    /// <summary>The field has not been paired with another projected value.</summary>
    Neutral,

    /// <summary>The paired field and its complete subtree have the same detached JSON value.</summary>
    Identical,

    /// <summary>The field differs from or is absent in the compared projection.</summary>
    Conflict,

    /// <summary>The field differs on the explicitly selected winning-override projection.</summary>
    WinningOverride
}
