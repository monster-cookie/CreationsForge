namespace CreationsForge.Engine.Workspaces;

/// <summary>Represents an actionable plugin workspace open or ownership failure.</summary>
public class PluginWorkspaceException : InvalidOperationException
{
    /// <summary>Initializes a workspace exception.</summary>
    /// <param name="message">The actionable failure description.</param>
    public PluginWorkspaceException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a workspace exception with an underlying cause.</summary>
    /// <param name="message">The actionable failure description.</param>
    /// <param name="innerException">The underlying failure.</param>
    public PluginWorkspaceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>Represents a source or output ownership conflict.</summary>
public sealed class PluginWorkspaceLockException : PluginWorkspaceException
{
    /// <summary>Initializes an ownership-conflict exception.</summary>
    /// <param name="message">The actionable failure description.</param>
    /// <param name="innerException">The underlying operating-system failure.</param>
    public PluginWorkspaceLockException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
