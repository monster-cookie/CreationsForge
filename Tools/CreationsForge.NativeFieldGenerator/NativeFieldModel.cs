using System.Reflection;

namespace CreationsForge.NativeFieldGenerator;

/// <summary>
/// Describes one native indexed field, its getter contract, and its future construction metadata.
/// </summary>
internal sealed class NativeFieldModel
{
    /// <summary>
    /// Initializes one complete indexed field description.
    /// </summary>
    /// <param name="name">The exact native FieldIndex name.</param>
    /// <param name="ordinal">The native FieldIndex ordinal.</param>
    /// <param name="mutableProperty">The mutable property used by later decoder generation.</param>
    /// <param name="getterProperty">The getter property used by the current read visitors.</param>
    /// <param name="nullability">The reflected getter nullability graph.</param>
    /// <param name="construction">The recursive wire-to-native construction model.</param>
    internal NativeFieldModel(
        string name,
        ushort ordinal,
        PropertyInfo mutableProperty,
        PropertyInfo getterProperty,
        NullabilityInfo nullability,
        NativeValueConstructionModel construction)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(mutableProperty);
        ArgumentNullException.ThrowIfNull(getterProperty);
        ArgumentNullException.ThrowIfNull(nullability);
        ArgumentNullException.ThrowIfNull(construction);
        Name = name;
        Ordinal = ordinal;
        MutableProperty = mutableProperty;
        GetterProperty = getterProperty;
        Nullability = nullability;
        Construction = construction;
    }

    /// <summary>Gets the exact native FieldIndex name.</summary>
    internal string Name { get; }

    /// <summary>Gets the native FieldIndex ordinal.</summary>
    internal ushort Ordinal { get; }

    /// <summary>Gets the mutable property used by later decoder generation.</summary>
    internal PropertyInfo MutableProperty { get; }

    /// <summary>Gets the getter property used by the current read visitors.</summary>
    internal PropertyInfo GetterProperty { get; }

    /// <summary>Gets the reflected getter nullability graph.</summary>
    internal NullabilityInfo Nullability { get; }

    /// <summary>Gets the recursively classified wire-to-native construction model.</summary>
    internal NativeValueConstructionModel Construction { get; }
}
