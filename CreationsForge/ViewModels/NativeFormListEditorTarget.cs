using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

/// <summary>Captures the exact immutable identity and revision used to begin one native FormList edit.</summary>
public sealed class NativeFormListEditorTarget
{
    /// <summary>Initializes one validated editor target.</summary>
    /// <param name="workspaceId">The non-empty active workspace identity.</param>
    /// <param name="expectedRevision">The exact revision that must reach Core unchanged.</param>
    /// <param name="role">How the native edit is acquired.</param>
    /// <param name="originFormKey">The source FormList for an override.</param>
    /// <param name="originSelection">The exact non-output source context for an override.</param>
    /// <param name="targetFormKey">The staged-output FormList selected for an existing-output edit.</param>
    /// <exception cref="ArgumentException">Thrown when the workspace or role-specific identities are invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="role"/> is undefined.</exception>
    internal NativeFormListEditorTarget(
        Guid workspaceId,
        WorkspaceRevision expectedRevision,
        FormListEditRole role,
        FormKey? originFormKey,
        ReferenceRequest? originSelection,
        FormKey? targetFormKey)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("An editor target requires a non-empty workspace identity.", nameof(workspaceId));
        }

        if (!Enum.IsDefined(role))
        {
            throw new ArgumentOutOfRangeException(nameof(role));
        }

        if (role == FormListEditRole.New &&
            (originFormKey is not null || originSelection is not null || targetFormKey is not null))
        {
            throw new ArgumentException("A new editor target cannot identify an origin or staged target.", nameof(originFormKey));
        }

        if (role == FormListEditRole.Override &&
            (originFormKey is null || originSelection is null || targetFormKey is not null))
        {
            throw new ArgumentException("An override editor target requires an exact source origin and no staged target.", nameof(originFormKey));
        }

        if (originSelection is not null &&
            (originSelection.FormKey != originFormKey || originSelection.Scope == RecordScope.StagedOutput))
        {
            throw new ArgumentException("An override editor target requires a matching exact non-output context.", nameof(originSelection));
        }

        if (role == FormListEditRole.ExistingOutput &&
            (targetFormKey is null || originFormKey is not null || originSelection is not null))
        {
            throw new ArgumentException("An existing-output editor target requires only one staged target identity.", nameof(targetFormKey));
        }
        WorkspaceId = workspaceId;
        ExpectedRevision = expectedRevision;
        Role = role;
        OriginFormKey = originFormKey;
        OriginSelection = originSelection;
        TargetFormKey = targetFormKey;
    }

    /// <summary>Gets the workspace identity captured for this action.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the exact revision that must reach Core unchanged.</summary>
    public WorkspaceRevision ExpectedRevision { get; }

    /// <summary>Gets how the edit selects or creates its staged FormList.</summary>
    public FormListEditRole Role { get; }

    /// <summary>Gets the source FormList identity for an override.</summary>
    public FormKey? OriginFormKey { get; }

    /// <summary>Gets the exact source context for an override.</summary>
    public ReferenceRequest? OriginSelection { get; }

    /// <summary>Gets the staged-output target for an existing-output edit.</summary>
    public FormKey? TargetFormKey { get; }

    /// <summary>Creates the exact immutable Core begin request with the supplied operation identity.</summary>
    /// <param name="operationId">The non-empty idempotency identity.</param>
    /// <returns>A Core request containing this target's unchanged identities and revision.</returns>
    internal BeginEditRequest CreateRequest(Guid operationId)
    {
        return new BeginEditRequest(
            operationId,
            ExpectedRevision,
            Role,
            OriginFormKey,
            OriginSelection,
            TargetFormKey);
    }
}
