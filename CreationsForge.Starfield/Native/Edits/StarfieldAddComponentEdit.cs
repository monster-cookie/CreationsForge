using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.Native.Edits;

/// <summary>Inserts one complete concrete native component at an exact position.</summary>
public sealed class StarfieldAddComponentEdit : FormListEdit
{
    /// <summary>Initializes a typed component insertion.</summary>
    /// <param name="index">The zero-based insertion position, including the current count for an append.</param>
    /// <param name="component">The concrete native component copied during preparation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="component"/> is <see langword="null"/>.</exception>
    public StarfieldAddComponentEdit(int index, IAComponentGetter component)
        : base("starfield.form-list.add-component")
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentNullException.ThrowIfNull(component);
        Index = index;
        Component = component;
    }

    /// <summary>Gets the exact zero-based insertion position.</summary>
    public int Index { get; }

    /// <summary>Gets the caller-owned component that preparation must deeply copy before publication.</summary>
    public IAComponentGetter Component { get; }
}
