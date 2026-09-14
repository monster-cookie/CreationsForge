namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Describes whether ordinary workspace operations may use the selected output state.</summary>
public enum OutputSynchronizationStatus
{
    /// <summary>The in-memory output and destination state are synchronized for ordinary operations.</summary>
    Ready,

    /// <summary>A pending save has no terminal outcome and requires recovery inspection or repair.</summary>
    RecoveryRequired,

    /// <summary>A terminal save outcome is known, but the workspace must explicitly reopen the resolved output.</summary>
    ReopenRequired
}
