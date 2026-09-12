namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Identifies a specific reviewed save journal and its observed recovery state without exposing mutable journal data.
/// </summary>
public sealed class RecoveryEvidenceToken : IEquatable<RecoveryEvidenceToken>
{
    /// <summary>Initializes a recovery evidence token.</summary>
    /// <param name="value">The non-empty opaque token issued by the save coordinator.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is empty or whitespace.</exception>
    public RecoveryEvidenceToken(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    /// <summary>Gets the opaque evidence token.</summary>
    public string Value { get; }

    /// <inheritdoc />
    public bool Equals(RecoveryEvidenceToken? other)
    {
        return other is not null && string.Equals(Value, other.Value, StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return Equals(obj as RecoveryEvidenceToken);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return StringComparer.Ordinal.GetHashCode(Value);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value;
    }
}
