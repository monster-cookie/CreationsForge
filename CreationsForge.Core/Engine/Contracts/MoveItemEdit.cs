namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Moves one FormList item to an exact final zero-based position while preserving every other item.</summary>
public sealed class MoveItemEdit : FormListEdit
{
    /// <summary>Initializes an ordered item move command.</summary>
    /// <param name="sourceIndex">The zero-based position of the item before the move.</param>
    /// <param name="destinationIndex">The zero-based position of the item after the move completes.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when either index is negative.</exception>
    public MoveItemEdit(int sourceIndex, int destinationIndex)
        : base("form-list.move-item")
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sourceIndex);
        ArgumentOutOfRangeException.ThrowIfNegative(destinationIndex);
        SourceIndex = sourceIndex;
        DestinationIndex = destinationIndex;
    }

    /// <summary>Gets the zero-based position of the item before the move.</summary>
    public int SourceIndex { get; }

    /// <summary>Gets the final zero-based position of the item after the move completes.</summary>
    public int DestinationIndex { get; }
}
