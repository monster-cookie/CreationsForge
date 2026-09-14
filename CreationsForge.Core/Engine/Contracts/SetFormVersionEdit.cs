namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Sets the FormList form-version value through its typed unsigned field.</summary>
public sealed class SetFormVersionEdit : FormListEdit
{
    /// <summary>Initializes a plugin form-version command.</summary>
    /// <param name="formVersion">The exact unsigned form-version value to assign.</param>
    public SetFormVersionEdit(ushort formVersion)
        : base("form-list.set-form-version")
    {
        FormVersion = formVersion;
    }

    /// <summary>Gets the exact unsigned form-version value to assign.</summary>
    public ushort FormVersion { get; }
}
