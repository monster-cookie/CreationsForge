using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Services;

/// <summary>Returns either an explicit null or one resolved reference with exact search provenance.</summary>
public sealed class ReferencePickerSelection
{
    /// <summary>Initializes an immutable reference-picker selection.</summary>
    /// <param name="workspaceId">The exact workspace from which the selection was resolved.</param>
    /// <param name="revision">The unchanged exact revision at resolution time.</param>
    /// <param name="isNull">Whether this is an explicit null selection.</param>
    /// <param name="match">The selected plugin match, or <see langword="null"/> for an explicit null.</param>
    /// <exception cref="ArgumentException">Thrown when workspace identity, revision identity, or null-selection invariants are invalid.</exception>
    public ReferencePickerSelection(
        Guid workspaceId,
        WorkspaceRevision revision,
        bool isNull,
        ReferenceSearchMatch? match)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("A reference-picker selection requires a non-empty workspace identity.", nameof(workspaceId));
        }

        if (revision.BaselineId == Guid.Empty)
        {
            throw new ArgumentException("A reference-picker selection requires a non-empty revision baseline.", nameof(revision));
        }

        if (isNull == (match is not null))
        {
            throw new ArgumentException(
                "An explicit null selection cannot carry a match, and a plugin selection requires one.",
                nameof(match));
        }

        WorkspaceId = workspaceId;
        Revision = revision;
        IsNull = isNull;
        Match = match;
    }

    /// <summary>Gets the exact workspace from which the selection was resolved.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the unchanged exact workspace revision at resolution time.</summary>
    public WorkspaceRevision Revision { get; }

    /// <summary>Gets whether the user explicitly selected a null reference.</summary>
    public bool IsNull { get; }

    /// <summary>Gets the resolved plugin search match, or <see langword="null"/> for an explicit null.</summary>
    public ReferenceSearchMatch? Match { get; }
}
