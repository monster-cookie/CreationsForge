using System.Reflection;

namespace CreationsForge.NativeFieldGenerator;

/// <summary>
/// Describes one concrete native model and its exact indexed getter fields for deterministic source generation.
/// </summary>
internal sealed class NativeTypeModel
{
    /// <summary>
    /// Initializes a complete native model description.
    /// </summary>
    /// <param name="mutableType">The concrete mutable Mutagen model class.</param>
    /// <param name="getterType">The matching generated getter interface.</param>
    /// <param name="fields">The native fields in exact FieldIndex order.</param>
    /// <param name="construction">The compile-time construction surface exposed by the installed native type.</param>
    internal NativeTypeModel(
        Type mutableType,
        Type getterType,
        IReadOnlyList<NativeFieldModel> fields,
        NativeTypeConstructionModel construction)
    {
        ArgumentNullException.ThrowIfNull(mutableType);
        ArgumentNullException.ThrowIfNull(getterType);
        ArgumentNullException.ThrowIfNull(fields);
        ArgumentNullException.ThrowIfNull(construction);
        MutableType = mutableType;
        GetterType = getterType;
        Fields = fields;
        Construction = construction;
    }

    /// <summary>Gets the concrete mutable Mutagen model class.</summary>
    internal Type MutableType { get; }

    /// <summary>Gets the matching generated getter interface.</summary>
    internal Type GetterType { get; }

    /// <summary>Gets the native fields in exact FieldIndex order.</summary>
    internal IReadOnlyList<NativeFieldModel> Fields { get; }

    /// <summary>Gets the compile-time construction surface exposed by the installed native type.</summary>
    internal NativeTypeConstructionModel Construction { get; }
}
