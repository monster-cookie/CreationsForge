namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Sets whether the FormList record is deleted through its typed Boolean field.</summary>
public sealed class SetDeletedEdit : FormListEdit
{
    /// <summary>Initializes a plugin deletion-state command.</summary>
    /// <param name="isDeleted">Whether the record must carry plugin deletion state.</param>
    public SetDeletedEdit(bool isDeleted)
        : base("form-list.set-deleted")
    {
        IsDeleted = isDeleted;
    }

    /// <summary>Gets whether the record must carry plugin deletion state.</summary>
    public bool IsDeleted { get; }
}
