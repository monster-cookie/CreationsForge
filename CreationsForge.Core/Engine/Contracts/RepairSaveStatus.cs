namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Describes the result of an explicit mutating incomplete-save repair.
/// </summary>
public enum RepairSaveStatus
{
    /// <summary>The already prepared output set was completed and verified.</summary>
    PreparedSetCompleted,

    /// <summary>Every destination entry was restored to its verified pre-save baseline.</summary>
    BaselineRestored,

    /// <summary>An unrecognized external change blocked repair without overwriting it.</summary>
    BlockedByExternalChange,

    /// <summary>The repair did not mutate a destination but could not establish a final state.</summary>
    NotStarted,

    /// <summary>The repair crossed a destination mutation and its final outcome is still unknown.</summary>
    StillUnknown
}
