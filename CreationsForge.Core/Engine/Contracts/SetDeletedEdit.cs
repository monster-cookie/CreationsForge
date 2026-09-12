namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Sets whether the native FormList record is deleted through its typed Boolean field.</summary>
public sealed class SetDeletedEdit : FormListEdit
{
    /// <summary>Initializes a native deletion-state command.</summary>
    /// <param name="isDeleted">Whether the record must carry native deletion state.</param>
    public SetDeletedEdit(bool isDeleted)
        : base("form-list.set-deleted")
    {
        IsDeleted = isDeleted;
    }

    /// <summary>Gets whether the record must carry native deletion state.</summary>
    public bool IsDeleted { get; }
}
