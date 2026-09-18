using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Confirms that a native major-record edit session became part of workspace state.
/// </summary>
public sealed class EditReceipt
{
    /// <summary>Initializes a staged edit receipt.</summary>
    /// <param name="editId">The non-empty edit identifier.</param>
    /// <param name="formKey">The allocated, overridden, or existing-output plugin FormKey.</param>
    /// <param name="originFormKey">The source origin FormKey for an override, or <see langword="null"/> for a new or existing-output record.</param>
    /// <param name="role">How the edit selected or created the record.</param>
    /// <param name="revision">The resulting workspace revision.</param>
    /// <param name="recordType">The exact native record family; omitted legacy receipts select FormList.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="editId"/> is empty.</exception>
    public EditReceipt(
        Guid editId,
        FormKey formKey,
        FormKey? originFormKey,
        FormListEditRole role,
        WorkspaceRevision revision,
        string recordType = "FormList")
    {
        if (editId == Guid.Empty)
        {
            throw new ArgumentException("An edit receipt requires a non-empty edit identifier.", nameof(editId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(recordType);

        EditId = editId;
        FormKey = formKey;
        OriginFormKey = originFormKey;
        Role = role;
        Revision = revision;
        RecordType = recordType;
    }

    /// <summary>Gets the staged edit identifier.</summary>
    public Guid EditId { get; }

    /// <summary>Gets the allocated, overridden, or existing-output plugin FormKey.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the source origin FormKey for an override, or <see langword="null"/> for a new or existing-output record.</summary>
    public FormKey? OriginFormKey { get; }

    /// <summary>Gets how the edit selected or created the record.</summary>
    public FormListEditRole Role { get; }

    /// <summary>Gets the resulting workspace revision.</summary>
    public WorkspaceRevision Revision { get; }

    /// <summary>Gets the exact native major-record family associated with this session.</summary>
    public string RecordType { get; }
}
