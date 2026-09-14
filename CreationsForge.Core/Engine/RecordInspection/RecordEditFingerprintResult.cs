using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.RecordInspection;

/// <summary>Returns a complete canonical record-edit identity together with any deterministic payload validation failure.</summary>
public sealed class RecordEditFingerprintResult
{
    /// <summary>Initializes the result of one complete canonical record-edit fingerprint write.</summary>
    /// <param name="fingerprint">The SHA-256 identity of the complete payload, including invalid code units when present.</param>
    /// <param name="validationError">The deterministic validation failure, or <see langword="null"/> when the payload may be applied.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fingerprint"/> is <see langword="null"/>.</exception>
    public RecordEditFingerprintResult(OperationFingerprint fingerprint, EngineError? validationError)
    {
        ArgumentNullException.ThrowIfNull(fingerprint);
        Fingerprint = fingerprint;
        ValidationError = validationError;
    }

    /// <summary>Gets the SHA-256 identity of the complete canonical payload.</summary>
    public OperationFingerprint Fingerprint { get; }

    /// <summary>Gets the deterministic validation failure, or <see langword="null"/> when the payload may be applied.</summary>
    public EngineError? ValidationError { get; }

    /// <summary>Gets whether the complete payload was valid while its canonical fingerprint was written.</summary>
    public bool IsValid => ValidationError is null;
}
