namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Describes where a semantic change occurred while values remain authoritative on detached native getters.
/// </summary>
public sealed class SemanticChangeDescriptor
{
    /// <summary>Initializes a semantic change descriptor.</summary>
    /// <param name="fieldIdentifier">The stable typed field identifier.</param>
    /// <param name="kind">The semantic change shape.</param>
    /// <param name="beforePosition">The prior collection position, when applicable.</param>
    /// <param name="afterPosition">The resulting collection position, when applicable.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldIdentifier"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="kind"/> is undefined or a supplied collection position is negative.</exception>
    public SemanticChangeDescriptor(
        string fieldIdentifier,
        SemanticChangeKind kind,
        int? beforePosition = null,
        int? afterPosition = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldIdentifier);

        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(nameof(kind));
        }

        if (beforePosition < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(beforePosition));
        }

        if (afterPosition < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(afterPosition));
        }

        FieldIdentifier = fieldIdentifier;
        Kind = kind;
        BeforePosition = beforePosition;
        AfterPosition = afterPosition;
    }

    /// <summary>Gets the stable typed field identifier.</summary>
    public string FieldIdentifier { get; }

    /// <summary>Gets the semantic change shape.</summary>
    public SemanticChangeKind Kind { get; }

    /// <summary>Gets the prior zero-based collection position, when applicable.</summary>
    public int? BeforePosition { get; }

    /// <summary>Gets the resulting zero-based collection position, when applicable.</summary>
    public int? AfterPosition { get; }
}
