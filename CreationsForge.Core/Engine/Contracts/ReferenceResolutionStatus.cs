namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Describes the result of resolving a native FormKey.
/// </summary>
public enum ReferenceResolutionStatus
{
    /// <summary>The reference resolved to a supported detached native record.</summary>
    Resolved,

    /// <summary>No native record matched the requested identity and scope.</summary>
    Unresolved,

    /// <summary>The reference exists but its native record family is unsupported for this operation.</summary>
    Unsupported,

    /// <summary>The selected native context is a deletion and no older context is substituted.</summary>
    Deleted,

    /// <summary>More than one context matched a single-record request that did not select a containing plugin.</summary>
    Ambiguous,

    /// <summary>The native record did not expose a stable registered record family.</summary>
    UnknownFamily
}
