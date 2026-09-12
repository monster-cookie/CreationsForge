using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Identifies the native FormList allocated, overridden, or selected inside an unpublished output candidate.
/// </summary>
public sealed class NativeEditIdentity
{
    /// <summary>Initializes a native edit identity.</summary>
    /// <param name="editId">The non-empty staged edit identifier.</param>
    /// <param name="formKey">The allocated, overridden, or existing-output native FormKey.</param>
    /// <param name="originFormKey">The source origin FormKey for an override, or <see langword="null"/> for a new or existing-output record.</param>
    /// <param name="role">How the candidate selected or created the native record.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="editId"/> is empty.</exception>
    public NativeEditIdentity(Guid editId, FormKey formKey, FormKey? originFormKey, FormListEditRole role)
    {
        if (editId == Guid.Empty)
        {
            throw new ArgumentException("A native edit identity requires a non-empty edit identifier.", nameof(editId));
        }

        EditId = editId;
        FormKey = formKey;
        OriginFormKey = originFormKey;
        Role = role;
    }

    /// <summary>Gets the staged edit identifier.</summary>
    public Guid EditId { get; }

    /// <summary>Gets the allocated, overridden, or existing-output native FormKey.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the source origin FormKey for an override, or <see langword="null"/> for a new or existing-output record.</summary>
    public FormKey? OriginFormKey { get; }

    /// <summary>Gets how the candidate selected or created the native record.</summary>
    public FormListEditRole Role { get; }
}
