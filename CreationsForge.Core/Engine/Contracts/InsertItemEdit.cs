using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Inserts one FormKey at an exact zero-based position without deduplicating the list.
/// </summary>
public sealed class InsertItemEdit : FormListEdit
{
    /// <summary>Initializes an ordered item insertion command.</summary>
    /// <param name="index">The zero-based insertion position.</param>
    /// <param name="item">The plugin FormKey to insert, including <see cref="FormKey.Null"/> when intentional.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative.</exception>
    public InsertItemEdit(int index, FormKey item)
        : base("form-list.insert-item")
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        Index = index;
        Item = item;
    }

    /// <summary>Gets the zero-based insertion position.</summary>
    public int Index { get; }

    /// <summary>Gets the plugin FormKey to insert.</summary>
    public FormKey Item { get; }
}
