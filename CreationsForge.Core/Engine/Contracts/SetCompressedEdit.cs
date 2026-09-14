namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Sets whether the FormList record is compressed through its typed Boolean field.</summary>
public sealed class SetCompressedEdit : FormListEdit
{
    /// <summary>Initializes a plugin compression command.</summary>
    /// <param name="isCompressed">Whether the record must use plugin compression.</param>
    public SetCompressedEdit(bool isCompressed)
        : base("form-list.set-compressed")
    {
        IsCompressed = isCompressed;
    }

    /// <summary>Gets whether the record must use plugin compression.</summary>
    public bool IsCompressed { get; }
}
