namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Controls how a workspace explicitly adopts terminal save-recovery evidence.</summary>
public enum OutputRecoveryAdoptionMode
{
    /// <summary>Keep the original workspace's pending staged candidate after proving that no destination change committed.</summary>
    ResumeStagedAfterNotCommitted,

    /// <summary>Discard any selected staged state and natively reopen the terminal destination baseline.</summary>
    ReopenResolvedOutput
}
