namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Requests an explicit mutating repair of a recognized incomplete save transaction.
/// </summary>
public sealed class RepairSaveRequest
{
    /// <summary>Initializes an explicit save-repair request.</summary>
    /// <param name="workspaceId">The original caller workspace identifier.</param>
    /// <param name="saveOperationId">The original incomplete save operation identifier.</param>
    /// <param name="repairOperationId">A new non-empty idempotency identifier for this repair decision.</param>
    /// <param name="expectedSaveRevision">The exact original save revision returned by recognized recovery inspection.</param>
    /// <param name="output">The exact output association used by the save.</param>
    /// <param name="evidenceToken">The recovery evidence token reviewed by the caller.</param>
    /// <param name="direction">Whether to complete the prepared set or restore its baseline.</param>
    /// <exception cref="ArgumentException">Thrown when an identifier is empty or the repair reuses the save identifier.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> or <paramref name="evidenceToken"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="direction"/> is undefined.</exception>
    public RepairSaveRequest(
        Guid workspaceId,
        Guid saveOperationId,
        Guid repairOperationId,
        WorkspaceRevision expectedSaveRevision,
        OutputAssociation output,
        RecoveryEvidenceToken evidenceToken,
        RepairSaveDirection direction)
    {
        if (workspaceId == Guid.Empty || saveOperationId == Guid.Empty || repairOperationId == Guid.Empty)
        {
            throw new ArgumentException("Save repair requires non-empty workspace, save, and repair operation identifiers.");
        }

        if (saveOperationId == repairOperationId)
        {
            throw new ArgumentException("A repair operation must use a new identifier distinct from the original save.", nameof(repairOperationId));
        }

        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(evidenceToken);
        if (!Enum.IsDefined(direction))
        {
            throw new ArgumentOutOfRangeException(nameof(direction));
        }

        WorkspaceId = workspaceId;
        SaveOperationId = saveOperationId;
        RepairOperationId = repairOperationId;
        ExpectedSaveRevision = expectedSaveRevision;
        Output = output;
        EvidenceToken = evidenceToken;
        Direction = direction;
    }

    /// <summary>Gets the original caller workspace identifier.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the original incomplete save operation identifier.</summary>
    public Guid SaveOperationId { get; }

    /// <summary>Gets the new idempotency identifier for this repair decision.</summary>
    public Guid RepairOperationId { get; }

    /// <summary>Gets the exact original save revision that the recognized journal must still contain.</summary>
    public WorkspaceRevision ExpectedSaveRevision { get; }

    /// <summary>Gets the exact output association used by the save.</summary>
    public OutputAssociation Output { get; }

    /// <summary>Gets the recovery evidence token reviewed by the caller.</summary>
    public RecoveryEvidenceToken EvidenceToken { get; }

    /// <summary>Gets whether to complete the prepared set or restore its baseline.</summary>
    public RepairSaveDirection Direction { get; }
}
