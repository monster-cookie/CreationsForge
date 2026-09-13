using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.RecordInspection;

/// <summary>Carries one record JSON write mode and its first deterministic validation failure.</summary>
public sealed class RecordJsonWriteContext
{
    /// <summary>Initializes an isolated record JSON write context.</summary>
    /// <param name="mode">The representation required for this write.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="mode"/> is undefined.</exception>
    public RecordJsonWriteContext(RecordJsonWriteMode mode)
    {
        if (!Enum.IsDefined(mode))
        {
            throw new ArgumentOutOfRangeException(nameof(mode));
        }

        Mode = mode;
    }

    /// <summary>Gets the representation required for this write.</summary>
    public RecordJsonWriteMode Mode { get; }

    /// <summary>Gets the first deterministic validation failure recorded while writing, or <see langword="null"/> when every value was valid.</summary>
    public EngineError? ValidationError { get; private set; }

    /// <summary>Records malformed UTF-16 while allowing the complete payload to continue into its unique fingerprint.</summary>
    internal void RecordMalformedUtf16()
    {
        ValidationError ??= new EngineError(
            EngineErrorCode.ValidationFailed,
            "The record edit payload contains malformed UTF-16 and cannot be applied.");
    }
}
