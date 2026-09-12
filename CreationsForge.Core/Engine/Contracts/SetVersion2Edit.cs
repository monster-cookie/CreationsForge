namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Sets the native FormList secondary-version value through its typed unsigned field.</summary>
public sealed class SetVersion2Edit : FormListEdit
{
    /// <summary>Initializes a native secondary-version command.</summary>
    /// <param name="version2">The exact unsigned secondary-version value to assign.</param>
    public SetVersion2Edit(ushort version2)
        : base("form-list.set-version-2")
    {
        Version2 = version2;
    }

    /// <summary>Gets the exact unsigned secondary-version value to assign.</summary>
    public ushort Version2 { get; }
}
