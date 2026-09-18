namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Replaces the independent native fields of one staged GameSettingFloat as a single guarded operation.</summary>
public sealed class GameSettingFloatEditRequest
{
    /// <summary>Initializes a complete immutable float game-setting edit.</summary>
    /// <param name="operationId">The non-empty idempotency identifier.</param>
    /// <param name="expectedRevision">The exact workspace revision observed by the editor.</param>
    /// <param name="editId">The staged native record edit identifier.</param>
    /// <param name="editorId">The non-empty float setting identifier.</param>
    /// <param name="data">The optional native float value.</param>
    /// <param name="majorRecordFlagsRaw">The complete native major-record flags, including compressed and deleted bits.</param>
    /// <param name="formVersion">The native form version.</param>
    /// <param name="version2">The secondary native version.</param>
    /// <param name="versionControl">The native version-control value.</param>
    /// <param name="xalg">The optional Starfield XALG value; other games require <see langword="null"/>.</param>
    /// <exception cref="ArgumentException">Thrown for an empty operation or edit identifier or a blank EditorID.</exception>
    public GameSettingFloatEditRequest(
        Guid operationId,
        WorkspaceRevision expectedRevision,
        Guid editId,
        string editorId,
        float? data,
        int majorRecordFlagsRaw,
        ushort formVersion,
        ushort version2,
        uint versionControl,
        ulong? xalg = null)
    {
        if (operationId == Guid.Empty)
        {
            throw new ArgumentException("A GameSettingFloat operation requires a non-empty identifier.", nameof(operationId));
        }

        if (editId == Guid.Empty)
        {
            throw new ArgumentException("A GameSettingFloat edit requires a non-empty staged edit identifier.", nameof(editId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(editorId);
        OperationId = operationId;
        ExpectedRevision = expectedRevision;
        EditId = editId;
        EditorId = editorId;
        Data = data;
        MajorRecordFlagsRaw = majorRecordFlagsRaw;
        FormVersion = formVersion;
        Version2 = version2;
        VersionControl = versionControl;
        Xalg = xalg;
    }

    /// <summary>Gets the idempotency identifier.</summary>
    public Guid OperationId { get; }

    /// <summary>Gets the exact expected workspace revision.</summary>
    public WorkspaceRevision ExpectedRevision { get; }

    /// <summary>Gets the staged native record edit identifier.</summary>
    public Guid EditId { get; }

    /// <summary>Gets the requested float setting EditorID.</summary>
    public string EditorId { get; }

    /// <summary>Gets the optional native float payload.</summary>
    public float? Data { get; }

    /// <summary>Gets all native major-record flag bits.</summary>
    public int MajorRecordFlagsRaw { get; }

    /// <summary>Gets the native form version.</summary>
    public ushort FormVersion { get; }

    /// <summary>Gets the secondary native version.</summary>
    public ushort Version2 { get; }

    /// <summary>Gets the native version-control value.</summary>
    public uint VersionControl { get; }

    /// <summary>Gets the Starfield-only XALG value, or <see langword="null"/> when absent.</summary>
    public ulong? Xalg { get; }
}
