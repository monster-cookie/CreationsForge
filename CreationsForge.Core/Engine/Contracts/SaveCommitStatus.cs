namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Describes what is known about destination files after a save attempt.
/// </summary>
public enum SaveCommitStatus
{
    /// <summary>The complete intended plugin-and-strings set was committed and reopened successfully.</summary>
    Committed,

    /// <summary>The save failed before the first destination mutation and no destination file changed.</summary>
    NotCommitted,

    /// <summary>The save may have partially changed destination files and requires recovery inspection.</summary>
    CommitOutcomeUnknown,

    /// <summary>The complete intended file set is known to be committed, but its final plugin reopen failed.</summary>
    CommittedButReopenFailed
}
