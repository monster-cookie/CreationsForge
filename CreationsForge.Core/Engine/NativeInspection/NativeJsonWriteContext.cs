using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.NativeInspection;

/// <summary>Carries one native JSON write mode and its first deterministic validation failure.</summary>
public sealed class NativeJsonWriteContext
{
    /// <summary>Initializes an isolated native JSON write context.</summary>
    /// <param name="mode">The representation required for this write.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="mode"/> is undefined.</exception>
    public NativeJsonWriteContext(NativeJsonWriteMode mode)
    {
        if (!Enum.IsDefined(mode))
        {
            throw new ArgumentOutOfRangeException(nameof(mode));
        }

        Mode = mode;
    }

    /// <summary>Gets the representation required for this write.</summary>
    public NativeJsonWriteMode Mode { get; }

    /// <summary>Gets the first deterministic validation failure recorded while writing, or <see langword="null"/> when every value was valid.</summary>
    public EngineError? ValidationError { get; private set; }

    /// <summary>Records malformed UTF-16 while allowing the complete payload to continue into its unique fingerprint.</summary>
    internal void RecordMalformedUtf16()
    {
        ValidationError ??= new EngineError(
            EngineErrorCode.ValidationFailed,
            "The native edit payload contains malformed UTF-16 and cannot be applied.");
    }
}
