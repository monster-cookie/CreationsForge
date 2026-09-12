using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.Native.Edits;

/// <summary>Replaces the complete ordered native component collection without deduplication.</summary>
public sealed class StarfieldReplaceComponentsEdit : FormListEdit
{
    /// <summary>Initializes a complete ordered component replacement.</summary>
    /// <param name="components">The concrete native component payloads copied during preparation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the collection or one of its components is <see langword="null"/>.</exception>
    public StarfieldReplaceComponentsEdit(IReadOnlyList<IAComponentGetter> components)
        : base("starfield.form-list.replace-components")
    {
        ArgumentNullException.ThrowIfNull(components);
        var snapshot = components.ToArray();
        if (snapshot.Any(static component => component is null))
        {
            throw new ArgumentNullException(nameof(components), "Component collections cannot contain null values.");
        }

        Components = Array.AsReadOnly(snapshot);
    }

    /// <summary>Gets the caller-owned component references that preparation must deeply copy before publication.</summary>
    public IReadOnlyList<IAComponentGetter> Components { get; }
}
