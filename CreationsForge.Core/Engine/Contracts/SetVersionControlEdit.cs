namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Sets the FormList version-control value through its typed unsigned field.</summary>
public sealed class SetVersionControlEdit : FormListEdit
{
    /// <summary>Initializes a plugin version-control command.</summary>
    /// <param name="versionControl">The exact unsigned version-control value to assign.</param>
    public SetVersionControlEdit(uint versionControl)
        : base("form-list.set-version-control")
    {
        VersionControl = versionControl;
    }

    /// <summary>Gets the exact unsigned version-control value to assign.</summary>
    public uint VersionControl { get; }
}
