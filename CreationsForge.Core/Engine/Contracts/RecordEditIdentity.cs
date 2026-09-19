using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Identifies the major record allocated, overridden, or selected inside an unpublished output candidate.
/// </summary>
public sealed class RecordEditIdentity
{
    /// <summary>Initializes a record edit identity.</summary>
    /// <param name="editId">The non-empty staged edit identifier.</param>
    /// <param name="formKey">The allocated, overridden, or existing-output plugin FormKey.</param>
    /// <param name="originFormKey">The source origin FormKey for an override, or <see langword="null"/> for a new or existing-output record.</param>
    /// <param name="role">How the candidate selected or created the record.</param>
    /// <param name="recordType">The exact native record family; omitted legacy identities select FormList.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="editId"/> is empty.</exception>
    public RecordEditIdentity(Guid editId, FormKey formKey, FormKey? originFormKey, FormListEditRole role, string recordType = "FormList")
    {
        if (editId == Guid.Empty)
        {
            throw new ArgumentException("A record edit identity requires a non-empty edit identifier.", nameof(editId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(recordType);

        EditId = editId;
        FormKey = formKey;
        OriginFormKey = originFormKey;
        Role = role;
        RecordType = recordType;
    }

    /// <summary>Gets the staged edit identifier.</summary>
    public Guid EditId { get; }

    /// <summary>Gets the allocated, overridden, or existing-output plugin FormKey.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the source origin FormKey for an override, or <see langword="null"/> for a new or existing-output record.</summary>
    public FormKey? OriginFormKey { get; }

    /// <summary>Gets how the candidate selected or created the record.</summary>
    public FormListEditRole Role { get; }

    /// <summary>Gets the exact native major-record family owned by this edit identity.</summary>
    public string RecordType { get; }
}
