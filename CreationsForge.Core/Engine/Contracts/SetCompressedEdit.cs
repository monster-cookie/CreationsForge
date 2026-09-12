namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Sets whether the native FormList record is compressed through its typed Boolean field.</summary>
public sealed class SetCompressedEdit : FormListEdit
{
    /// <summary>Initializes a native compression command.</summary>
    /// <param name="isCompressed">Whether the record must use native compression.</param>
    public SetCompressedEdit(bool isCompressed)
        : base("form-list.set-compressed")
    {
        IsCompressed = isCompressed;
    }

    /// <summary>Gets whether the record must use native compression.</summary>
    public bool IsCompressed { get; }
}
