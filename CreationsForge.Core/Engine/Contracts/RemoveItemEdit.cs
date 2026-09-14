namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Removes the FormList item at an exact zero-based position.
/// </summary>
public sealed class RemoveItemEdit : FormListEdit
{
    /// <summary>Initializes an ordered item removal command.</summary>
    /// <param name="index">The zero-based position to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative.</exception>
    public RemoveItemEdit(int index)
        : base("form-list.remove-item")
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        Index = index;
    }

    /// <summary>Gets the zero-based position to remove.</summary>
    public int Index { get; }
}
