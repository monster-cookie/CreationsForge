namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Requests read-only recovery inspection after the original workspace or response is gone.
/// </summary>
public sealed class RecoverSaveRequest
{
    /// <summary>Initializes a read-only save-recovery request.</summary>
    /// <param name="workspaceId">The original caller workspace identifier.</param>
    /// <param name="saveOperationId">The original save operation identifier.</param>
    /// <param name="output">The exact output association used by the save.</param>
    /// <exception cref="ArgumentException">Thrown when either identifier is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <see langword="null"/>.</exception>
    public RecoverSaveRequest(Guid workspaceId, Guid saveOperationId, OutputAssociation output)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("Save recovery requires a non-empty workspace identifier.", nameof(workspaceId));
        }

        if (saveOperationId == Guid.Empty)
        {
            throw new ArgumentException("Save recovery requires a non-empty save operation identifier.", nameof(saveOperationId));
        }

        ArgumentNullException.ThrowIfNull(output);
        WorkspaceId = workspaceId;
        SaveOperationId = saveOperationId;
        Output = output;
    }

    /// <summary>Gets the original caller workspace identifier.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the original save operation identifier.</summary>
    public Guid SaveOperationId { get; }

    /// <summary>Gets the exact output association used by the save.</summary>
    public OutputAssociation Output { get; }
}
