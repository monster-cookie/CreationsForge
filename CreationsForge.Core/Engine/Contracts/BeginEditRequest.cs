using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Requests allocation, override, or selection of one native major record in a transactional output candidate.</summary>
public sealed class BeginEditRequest
{
    /// <summary>Initializes a begin-edit request.</summary>
    /// <param name="operationId">The non-empty idempotency identifier.</param>
    /// <param name="expectedRevision">The exact expected workspace revision.</param>
    /// <param name="role">Whether to allocate a new record, override a source record, or select a record already contained in the output.</param>
    /// <param name="originFormKey">The existing FormKey for an override, or <see langword="null"/> for a new or existing-output record.</param>
    /// <param name="originSelection">An optional exact source or load-order context for an override. When omitted, the adapter resolves the winning context across only the explicit source and load-order inputs.</param>
    /// <param name="targetFormKey">The FormKey already contained in the selected output for an existing-output edit, otherwise <see langword="null"/>.</param>
    /// <param name="recordType">The exact registered major-record family; omitted legacy requests select FormList.</param>
    /// <exception cref="ArgumentException">Thrown when the operation identifier is empty, the role and identity values disagree, the selector identifies a different FormKey, or a staged-output origin is requested.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="role"/> is undefined.</exception>
    public BeginEditRequest(
        Guid operationId,
        WorkspaceRevision expectedRevision,
        FormListEditRole role,
        FormKey? originFormKey = null,
        ReferenceRequest? originSelection = null,
        FormKey? targetFormKey = null,
        string recordType = "FormList")
    {
        if (operationId == Guid.Empty)
        {
            throw new ArgumentException("A begin-edit operation requires a non-empty identifier.", nameof(operationId));
        }

        if (!Enum.IsDefined(role))
        {
            throw new ArgumentOutOfRangeException(nameof(role));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(recordType);

        if (role == FormListEditRole.New &&
            (originFormKey is not null || originSelection is not null || targetFormKey is not null))
        {
            throw new ArgumentException($"A new {recordType} edit cannot identify an origin or existing-output target.", nameof(originFormKey));
        }

        if (role == FormListEditRole.Override && originFormKey is null)
        {
            throw new ArgumentException($"A {recordType} override requires an origin FormKey.", nameof(originFormKey));
        }

        if (role == FormListEditRole.Override && targetFormKey is not null)
        {
            throw new ArgumentException($"A {recordType} override cannot identify an existing-output target.", nameof(targetFormKey));
        }

        if (originSelection is not null && originSelection.FormKey != originFormKey)
        {
            throw new ArgumentException($"A {recordType} override context must identify the same FormKey as its origin.", nameof(originSelection));
        }

        if (originSelection?.Scope == RecordScope.StagedOutput)
        {
            throw new ArgumentException($"A {recordType} override origin cannot be selected from staged output state.", nameof(originSelection));
        }

        if (role == FormListEditRole.ExistingOutput && targetFormKey is null)
        {
            throw new ArgumentException($"An existing-output {recordType} edit requires a target FormKey.", nameof(targetFormKey));
        }

        if (role == FormListEditRole.ExistingOutput && (originFormKey is not null || originSelection is not null))
        {
            throw new ArgumentException($"An existing-output {recordType} edit cannot identify a source origin or context.", nameof(originFormKey));
        }

        OperationId = operationId;
        ExpectedRevision = expectedRevision;
        Role = role;
        OriginFormKey = originFormKey;
        OriginSelection = originSelection;
        TargetFormKey = targetFormKey;
        RecordType = recordType;
    }

    /// <summary>Gets the idempotency identifier.</summary>
    public Guid OperationId { get; }

    /// <summary>Gets the exact expected workspace revision.</summary>
    public WorkspaceRevision ExpectedRevision { get; }

    /// <summary>Gets whether the edit allocates, overrides, or selects an existing-output record.</summary>
    public FormListEditRole Role { get; }

    /// <summary>Gets the existing FormKey for an override, or <see langword="null"/> for a new or existing-output record.</summary>
    public FormKey? OriginFormKey { get; }

    /// <summary>Gets the exact source or load-order override context, or <see langword="null"/> to resolve the winning explicit input context.</summary>
    public ReferenceRequest? OriginSelection { get; }

    /// <summary>Gets the FormKey already contained in selected output state, or <see langword="null"/> for new and override edits.</summary>
    public FormKey? TargetFormKey { get; }

    /// <summary>Gets the exact registered major-record family selected for this edit.</summary>
    public string RecordType { get; }
}
