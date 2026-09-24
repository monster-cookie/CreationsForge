namespace CreationsForge.Engine.Persistence;

/// <summary>Describes the terminal outcome of one save request.</summary>
public enum PluginSaveStatus
{
    /// <summary>The destination was published, reopened, and adopted as the saved baseline.</summary>
    Succeeded,

    /// <summary>The workspace was already clean and the destination still matched its saved baseline.</summary>
    NoChanges,

    /// <summary>The save failed without establishing a confirmed published output.</summary>
    Failed,

    /// <summary>The save was canceled before destination publication began.</summary>
    Canceled,

    /// <summary>The files were published, but the published output could not be reopened and adopted.</summary>
    PublishedButReopenFailed,
}

/// <summary>Describes the observed destination state after a save request.</summary>
public enum PluginPublicationState
{
    /// <summary>No destination file was intentionally changed.</summary>
    Unchanged,

    /// <summary>A failed publication was restored to its prior file set.</summary>
    Restored,

    /// <summary>The complete staged file set was published.</summary>
    Published,

    /// <summary>Publication and restoration did not establish one complete known file set.</summary>
    PartiallyPublished,
}

/// <summary>Identifies a persistence phase for progress and failure diagnostics.</summary>
public enum PluginSavePhase
{
    /// <summary>The request has not entered a persistence phase.</summary>
    None,

    /// <summary>The workspace and destination are being checked before export.</summary>
    Preflight,

    /// <summary>Mutagen is exporting the native output into a same-filesystem staging directory.</summary>
    Staging,

    /// <summary>The staged plugin is being reopened and verified.</summary>
    StagedReopen,

    /// <summary>Existing destination members are being backed up.</summary>
    Backup,

    /// <summary>Staged plugin and string files are being published.</summary>
    Publication,

    /// <summary>A failed publication is being restored.</summary>
    Restoration,

    /// <summary>The published destination is being reopened and verified.</summary>
    PublishedReopen,

    /// <summary>The save reached its terminal result.</summary>
    Complete,
}
