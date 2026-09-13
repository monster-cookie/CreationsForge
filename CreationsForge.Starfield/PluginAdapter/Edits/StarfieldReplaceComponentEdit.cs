using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.PluginAdapter.Edits;

/// <summary>Replaces one existing component with a complete concrete Mutagen payload.</summary>
public sealed class StarfieldReplaceComponentEdit : FormListEdit
{
    /// <summary>Initializes a typed component replacement.</summary>
    /// <param name="index">The zero-based existing component position.</param>
    /// <param name="component">The concrete Mutagen component copied during preparation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="component"/> is <see langword="null"/>.</exception>
    public StarfieldReplaceComponentEdit(int index, IAComponentGetter component)
        : base("starfield.form-list.replace-component")
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentNullException.ThrowIfNull(component);
        Index = index;
        Component = component;
    }

    /// <summary>Gets the exact zero-based existing component position.</summary>
    public int Index { get; }

    /// <summary>Gets the caller-owned component that preparation must deeply copy before publication.</summary>
    public IAComponentGetter Component { get; }
}
