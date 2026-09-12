namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Describes a stable engine failure and its human-readable context.
/// </summary>
public sealed class EngineError
{
    /// <summary>
    /// Initializes a new engine error.
    /// </summary>
    /// <param name="code">The stable failure category.</param>
    /// <param name="message">A non-empty description suitable for diagnostics and user-facing projection.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="message"/> is empty or whitespace.</exception>
    public EngineError(EngineErrorCode code, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        Code = code;
        Message = message;
    }

    /// <summary>Gets the stable failure category.</summary>
    public EngineErrorCode Code { get; }

    /// <summary>Gets the diagnostic description of the failure.</summary>
    public string Message { get; }
}
