namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Clears all FormList items while retaining the record itself.
/// </summary>
public sealed class ClearItemsEdit : FormListEdit
{
    /// <summary>Initializes an ordered item clear command.</summary>
    public ClearItemsEdit()
        : base("form-list.clear-items")
    { }
}
