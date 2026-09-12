namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Holds an adapter-owned defensively copied native command payload and its complete canonical identity.
/// </summary>
public abstract class PreparedFormListEdit
{
    /// <summary>Initializes a prepared native command payload.</summary>
    /// <param name="fingerprint">The complete typed canonical SHA-256 identity.</param>
    /// <param name="error">A deterministic validation failure, or <see langword="null"/> when the payload is valid.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="fingerprint"/> is <see langword="null"/>.</exception>
    protected PreparedFormListEdit(OperationFingerprint fingerprint, EngineError? error = null)
    {
        ArgumentNullException.ThrowIfNull(fingerprint);
        Fingerprint = fingerprint;
        Error = error;
    }

    /// <summary>Gets the complete typed canonical identity of the copied payload.</summary>
    public OperationFingerprint Fingerprint { get; }

    /// <summary>Gets a deterministic preparation failure, or <see langword="null"/> when the payload is valid.</summary>
    public EngineError? Error { get; }

    /// <summary>Gets whether the adapter prepared a valid payload that may be applied.</summary>
    public bool IsValid => Error is null;
}
