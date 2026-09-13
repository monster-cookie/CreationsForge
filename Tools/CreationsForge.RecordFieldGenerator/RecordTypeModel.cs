using System.Reflection;

namespace CreationsForge.RecordFieldGenerator;

/// <summary>
/// Describes one concrete Mutagen model and its exact indexed getter fields for deterministic source generation.
/// </summary>
internal sealed class RecordTypeModel
{
    /// <summary>
    /// Initializes a complete Mutagen model description.
    /// </summary>
    /// <param name="mutableType">The concrete mutable Mutagen model class.</param>
    /// <param name="getterType">The matching generated getter interface.</param>
    /// <param name="fields">The record fields in exact FieldIndex order.</param>
    /// <param name="construction">The compile-time construction surface exposed by the installed Mutagen type.</param>
    internal RecordTypeModel(
        Type mutableType,
        Type getterType,
        IReadOnlyList<RecordFieldModel> fields,
        RecordTypeConstructionModel construction)
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

    /// <summary>Gets the record fields in exact FieldIndex order.</summary>
    internal IReadOnlyList<RecordFieldModel> Fields { get; }

    /// <summary>Gets the compile-time construction surface exposed by the installed Mutagen type.</summary>
    internal RecordTypeConstructionModel Construction { get; }
}
