using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Replaces all FormList items while preserving caller-supplied order, duplicates, and null FormKeys.
/// </summary>
public sealed class ReplaceItemsEdit : FormListEdit
{
    /// <summary>Initializes an ordered item replacement command and snapshots the caller-owned collection.</summary>
    /// <param name="items">The complete ordered item list. Duplicate entries and <see cref="FormKey.Null"/> are preserved.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <see langword="null"/>.</exception>
    public ReplaceItemsEdit(IReadOnlyList<FormKey> items)
        : base("form-list.replace-items")
    {
        ArgumentNullException.ThrowIfNull(items);
        Items = Array.AsReadOnly(items.ToArray());
    }

    /// <summary>Gets an immutable snapshot of the complete ordered item list.</summary>
    public IReadOnlyList<FormKey> Items { get; }
}
