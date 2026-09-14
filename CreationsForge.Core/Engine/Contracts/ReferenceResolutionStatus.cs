namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Describes the result of resolving a plugin FormKey.
/// </summary>
public enum ReferenceResolutionStatus
{
    /// <summary>The reference resolved to a supported detached record.</summary>
    Resolved,

    /// <summary>No record matched the requested identity and scope.</summary>
    Unresolved,

    /// <summary>The reference exists but its record family is unsupported for this operation.</summary>
    Unsupported,

    /// <summary>The selected record context is a deletion and no older context is substituted.</summary>
    Deleted,

    /// <summary>More than one context matched a single-record request that did not select a containing plugin.</summary>
    Ambiguous,

    /// <summary>The record did not expose a stable registered record family.</summary>
    UnknownFamily
}
