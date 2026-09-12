namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Requests one named typed mutation against an existing staged FormList edit.</summary>
public sealed class FormListEditRequest
{
    /// <summary>Initializes a typed FormList mutation request.</summary>
    /// <param name="operationId">The non-empty idempotency identifier.</param>
    /// <param name="expectedRevision">The exact expected workspace revision.</param>
    /// <param name="editId">The non-empty staged edit identifier.</param>
    /// <param name="edit">The named typed edit payload.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="operationId"/> or <paramref name="editId"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="edit"/> is <see langword="null"/>.</exception>
    public FormListEditRequest(Guid operationId, WorkspaceRevision expectedRevision, Guid editId, FormListEdit edit)
    {
        if (operationId == Guid.Empty)
        {
            throw new ArgumentException("A FormList edit operation requires a non-empty identifier.", nameof(operationId));
        }

        if (editId == Guid.Empty)
        {
            throw new ArgumentException("A FormList edit operation requires a non-empty edit identifier.", nameof(editId));
        }

        ArgumentNullException.ThrowIfNull(edit);
        OperationId = operationId;
        ExpectedRevision = expectedRevision;
        EditId = editId;
        Edit = edit;
    }

    /// <summary>Gets the idempotency identifier.</summary>
    public Guid OperationId { get; }

    /// <summary>Gets the exact expected workspace revision.</summary>
    public WorkspaceRevision ExpectedRevision { get; }

    /// <summary>Gets the staged edit identifier.</summary>
    public Guid EditId { get; }

    /// <summary>Gets the named typed mutation payload.</summary>
    public FormListEdit Edit { get; }
}
