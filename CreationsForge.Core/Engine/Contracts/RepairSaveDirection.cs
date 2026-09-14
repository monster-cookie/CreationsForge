namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Selects how an explicitly reviewed incomplete save transaction is repaired.
/// </summary>
public enum RepairSaveDirection
{
    /// <summary>Complete the already prepared output set.</summary>
    CompletePrepared,

    /// <summary>Restore every destination entry to its recorded pre-save baseline.</summary>
    RestoreBaseline
}
