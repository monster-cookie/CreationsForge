namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Describes the result of read-only recovery inspection.
/// </summary>
public enum RecoverSaveStatus
{
    /// <summary>The complete intended output set is committed.</summary>
    Committed,

    /// <summary>No destination entry changed.</summary>
    NotCommitted,

    /// <summary>The destination state is partial, ambiguous, or otherwise still unknown.</summary>
    StillUnknown
}
