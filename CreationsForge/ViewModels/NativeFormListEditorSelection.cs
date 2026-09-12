using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

/// <summary>Captures one atomic workspace, revision, FormList, and exact-context selection for editor actions.</summary>
public sealed class NativeFormListEditorSelection
{
    /// <summary>Initializes one revision-bound editor selection.</summary>
    /// <param name="workspaceId">The non-empty active workspace identity.</param>
    /// <param name="revision">The browser revision that produced the selection.</param>
    /// <param name="formKey">The selected native FormList identity.</param>
    /// <param name="exactReferenceRequest">The exact non-output context, or <see langword="null"/> for staged output.</param>
    /// <param name="isStagedOutput">Whether the selected context belongs to the staged output.</param>
    /// <exception cref="ArgumentException">Thrown when an identity is empty, the exact request targets another record, or staged-output state disagrees with the request.</exception>
    public NativeFormListEditorSelection(
        Guid workspaceId,
        WorkspaceRevision revision,
        FormKey formKey,
        ReferenceRequest? exactReferenceRequest,
        bool isStagedOutput)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("An editor selection requires a non-empty workspace identity.", nameof(workspaceId));
        }

        if (formKey == FormKey.Null)
        {
            throw new ArgumentException("An editor selection requires a real FormList identity.", nameof(formKey));
        }

        if (exactReferenceRequest is not null && exactReferenceRequest.FormKey != formKey)
        {
            throw new ArgumentException("An editor selection's exact request must identify the same FormList.", nameof(exactReferenceRequest));
        }

        if (isStagedOutput == (exactReferenceRequest is not null))
        {
            throw new ArgumentException(
                "A staged-output selection omits a source request, while a non-output selection requires one.",
                nameof(exactReferenceRequest));
        }

        if (exactReferenceRequest?.Scope == RecordScope.StagedOutput)
        {
            throw new ArgumentException("A non-output editor selection cannot carry a staged-output request.", nameof(exactReferenceRequest));
        }

        WorkspaceId = workspaceId;
        Revision = revision;
        FormKey = formKey;
        ExactReferenceRequest = exactReferenceRequest;
        IsStagedOutput = isStagedOutput;
    }

    /// <summary>Gets the active workspace identity captured with the selection.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the browser revision that produced every value in this selection.</summary>
    public WorkspaceRevision Revision { get; }

    /// <summary>Gets the selected native FormList identity.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the exact selected source or load-order context, or <see langword="null"/> for staged output.</summary>
    public ReferenceRequest? ExactReferenceRequest { get; }

    /// <summary>Gets whether this selection identifies an exact record already contained in staged output.</summary>
    public bool IsStagedOutput { get; }
}
