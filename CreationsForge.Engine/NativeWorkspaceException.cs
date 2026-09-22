namespace CreationsForge.Engine;

/// <summary>Represents an actionable native workspace open or ownership failure.</summary>
public class NativeWorkspaceException : InvalidOperationException
{
    /// <summary>Initializes a workspace exception.</summary>
    /// <param name="message">The actionable failure description.</param>
    public NativeWorkspaceException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a workspace exception with an underlying cause.</summary>
    /// <param name="message">The actionable failure description.</param>
    /// <param name="innerException">The underlying failure.</param>
    public NativeWorkspaceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>Represents a source or output ownership conflict.</summary>
public sealed class NativeWorkspaceLockException : NativeWorkspaceException
{
    /// <summary>Initializes an ownership-conflict exception.</summary>
    /// <param name="message">The actionable failure description.</param>
    /// <param name="innerException">The underlying operating-system failure.</param>
    public NativeWorkspaceLockException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
