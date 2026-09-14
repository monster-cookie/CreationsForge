using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Starfield.PluginAdapter.Edits;

/// <summary>Removes the component at one exact zero-based Mutagen position.</summary>
public sealed class StarfieldRemoveComponentEdit : FormListEdit
{
    /// <summary>Initializes an exact component removal.</summary>
    /// <param name="index">The zero-based existing component position.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative.</exception>
    public StarfieldRemoveComponentEdit(int index)
        : base("starfield.form-list.remove-component")
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        Index = index;
    }

    /// <summary>Gets the exact zero-based existing component position.</summary>
    public int Index { get; }
}
