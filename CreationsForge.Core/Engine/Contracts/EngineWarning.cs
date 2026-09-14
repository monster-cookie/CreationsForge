namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Describes a non-fatal condition observed while completing an engine operation.
/// </summary>
public sealed class EngineWarning
{
    /// <summary>
    /// Initializes a new engine warning.
    /// </summary>
    /// <param name="code">A stable, caller-visible warning identifier.</param>
    /// <param name="message">A non-empty explanation of the warning.</param>
    /// <exception cref="ArgumentException">Thrown when either value is empty or whitespace.</exception>
    public EngineWarning(string code, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        Code = code;
        Message = message;
    }

    /// <summary>Gets the stable warning identifier.</summary>
    public string Code { get; }

    /// <summary>Gets the warning description.</summary>
    public string Message { get; }
}
