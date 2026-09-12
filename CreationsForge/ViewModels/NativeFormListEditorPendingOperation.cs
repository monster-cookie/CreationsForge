using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;

namespace CreationsForge.ViewModels;

/// <summary>Retains the exact immutable request state needed to replay one uncertain editor mutation.</summary>
internal abstract class NativeFormListEditorPendingOperation
{
    /// <summary>Initializes common immutable pending-operation identity.</summary>
    /// <param name="operationId">The non-empty idempotency identity.</param>
    /// <param name="workspaceId">The non-empty owning workspace identity.</param>
    /// <param name="expectedRevision">The exact originally captured revision.</param>
    /// <param name="actionName">The user-facing action name used in unresolved-outcome status.</param>
    protected NativeFormListEditorPendingOperation(
        Guid operationId,
        Guid workspaceId,
        WorkspaceRevision expectedRevision,
        string actionName)
    {
        if (operationId == Guid.Empty)
        {
            throw new ArgumentException("A pending editor operation requires a non-empty operation identity.", nameof(operationId));
        }

        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("A pending editor operation requires a non-empty workspace identity.", nameof(workspaceId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(actionName);
        OperationId = operationId;
        WorkspaceId = workspaceId;
        ExpectedRevision = expectedRevision;
        ActionName = actionName;
    }

    /// <summary>Gets the exact idempotency identity used by the original mutation.</summary>
    public Guid OperationId { get; }

    /// <summary>Gets the workspace that owned the original mutation.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the exact revision used by the original mutation.</summary>
    public WorkspaceRevision ExpectedRevision { get; }

    /// <summary>Gets the presentation action name associated with the uncertain outcome.</summary>
    public string ActionName { get; }
}

/// <summary>Retains one exact immutable Begin request for uncertain-outcome replay.</summary>
internal sealed class NativeFormListEditorPendingBegin : NativeFormListEditorPendingOperation
{
    /// <summary>Initializes an exact pending Begin envelope.</summary>
    /// <param name="workspaceId">The owning workspace identity.</param>
    /// <param name="request">The exact immutable Core request.</param>
    /// <param name="actionName">The user-facing begin action name.</param>
    public NativeFormListEditorPendingBegin(Guid workspaceId, BeginEditRequest request, string actionName)
        : base(request?.OperationId ?? throw new ArgumentNullException(nameof(request)), workspaceId, request.ExpectedRevision, actionName)
    {
        Request = request;
    }

    /// <summary>Gets the exact immutable Begin request used by the original call.</summary>
    public BeginEditRequest Request { get; }
}

/// <summary>Retains detached serialized Apply input so replay can resolve and decode the exact codec again.</summary>
internal sealed class NativeFormListEditorPendingApply : NativeFormListEditorPendingOperation
{
    /// <summary>The cloned detached serialized command arguments.</summary>
    private readonly JsonElement ArgumentsValue;

    /// <summary>Initializes an exact pending Apply envelope.</summary>
    /// <param name="workspaceId">The owning workspace identity.</param>
    /// <param name="expectedRevision">The session revision captured before the original call.</param>
    /// <param name="operationId">The exact idempotency identity.</param>
    /// <param name="editId">The staged edit identity.</param>
    /// <param name="catalogIdentity">The exact catalog identity.</param>
    /// <param name="commandName">The exact codec command discriminator.</param>
    /// <param name="arguments">The detached serialized command arguments.</param>
    /// <param name="limits">The exact immutable read limits used for decode.</param>
    public NativeFormListEditorPendingApply(
        Guid workspaceId,
        WorkspaceRevision expectedRevision,
        Guid operationId,
        Guid editId,
        NativeWireSchemaCatalogIdentity catalogIdentity,
        string commandName,
        JsonElement arguments,
        NativeWireReadLimits limits)
        : base(operationId, workspaceId, expectedRevision, "Apply")
    {
        if (editId == Guid.Empty)
        {
            throw new ArgumentException("A pending Apply operation requires a non-empty edit identity.", nameof(editId));
        }

        ArgumentNullException.ThrowIfNull(catalogIdentity);
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);
        ArgumentNullException.ThrowIfNull(limits);
        if (arguments.ValueKind == JsonValueKind.Undefined)
        {
            throw new ArgumentException("Pending Apply arguments cannot be undefined.", nameof(arguments));
        }

        EditId = editId;
        CatalogIdentity = catalogIdentity;
        CommandName = commandName;
        ArgumentsValue = arguments.Clone();
        Limits = limits;
    }

    /// <summary>Gets the staged edit identity used by the original Apply.</summary>
    public Guid EditId { get; }

    /// <summary>Gets the exact schema and codec catalog identity used by the original Apply.</summary>
    public NativeWireSchemaCatalogIdentity CatalogIdentity { get; }

    /// <summary>Gets the exact codec command discriminator used by the original Apply.</summary>
    public string CommandName { get; }

    /// <summary>Gets a detached clone of the original serialized command arguments.</summary>
    public JsonElement Arguments => ArgumentsValue.Clone();

    /// <summary>Gets the exact immutable resource limits used by the original decode.</summary>
    public NativeWireReadLimits Limits { get; }
}
