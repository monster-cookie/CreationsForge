using System.Reflection;

namespace CreationsForge.NativeFieldGenerator;

/// <summary>Identifies the compile-time construction route available for one concrete native model.</summary>
internal enum NativeTypeConstructionKind
{
    /// <summary>The generated reader can call an accessible parameterless constructor directly.</summary>
    PublicParameterlessConstructor,

    /// <summary>A parameterless constructor exists but cannot be called by generated production source.</summary>
    NonPublicParameterlessConstructor,

    /// <summary>No parameterless constructor exists and an explicit typed factory must be supplied.</summary>
    ParameterizedConstructorOnly
}

/// <summary>Describes the statically callable construction surface of one concrete native model.</summary>
internal sealed class NativeTypeConstructionModel
{
    /// <summary>Initializes one concrete native type construction description.</summary>
    /// <param name="kind">The available parameterless-construction category.</param>
    /// <param name="parameterlessConstructor">The discovered parameterless constructor, or <see langword="null"/> when none exists.</param>
    /// <param name="requiredMemberNames">Public required members that a generated constructor expression must satisfy.</param>
    internal NativeTypeConstructionModel(
        NativeTypeConstructionKind kind,
        ConstructorInfo? parameterlessConstructor,
        IReadOnlyList<string> requiredMemberNames)
    {
        ArgumentNullException.ThrowIfNull(requiredMemberNames);
        Kind = kind;
        ParameterlessConstructor = parameterlessConstructor;
        RequiredMemberNames = requiredMemberNames;
    }

    /// <summary>Gets the available parameterless-construction category.</summary>
    internal NativeTypeConstructionKind Kind { get; }

    /// <summary>Gets the discovered parameterless constructor, or <see langword="null"/> when none exists.</summary>
    internal ConstructorInfo? ParameterlessConstructor { get; }

    /// <summary>Gets the public required members a generated constructor expression must satisfy.</summary>
    internal IReadOnlyList<string> RequiredMemberNames { get; }
}

/// <summary>Identifies the recursive wire and native construction shape of one value.</summary>
internal enum NativeValueConstructionKind
{
    /// <summary>A nullable value-type wrapper around another classified value.</summary>
    Nullable,

    /// <summary>An ordinary UTF-16 string.</summary>
    String,

    /// <summary>A Boolean scalar.</summary>
    Boolean,

    /// <summary>A single-precision IEEE-754 scalar.</summary>
    Single,

    /// <summary>A double-precision IEEE-754 scalar.</summary>
    Double,

    /// <summary>An enum encoded through its complete underlying integral domain.</summary>
    Enum,

    /// <summary>An integral or decimal scalar other than an enum.</summary>
    Numeric,

    /// <summary>A canonical GUID.</summary>
    Guid,

    /// <summary>A complete <see cref="System.Drawing.Color"/> value.</summary>
    Color,

    /// <summary>A two-dimensional floating-point vector.</summary>
    P2Float,

    /// <summary>A two-dimensional integer vector.</summary>
    P2Int,

    /// <summary>A three-dimensional floating-point vector.</summary>
    P3Float,

    /// <summary>A localized translated-string value.</summary>
    TranslatedString,

    /// <summary>An exact byte-memory slice.</summary>
    Memory,

    /// <summary>A typed ordinary or nullable FormKey link.</summary>
    FormLink,

    /// <summary>An owner-sensitive link-or-index wrapper.</summary>
    FormLinkOrIndex,

    /// <summary>A typed game-asset path link.</summary>
    AssetLink,

    /// <summary>An ordered one-dimensional collection.</summary>
    Collection,

    /// <summary>A row-major two-dimensional collection.</summary>
    Array2d,

    /// <summary>A concrete nested indexed native model.</summary>
    NestedConcrete,

    /// <summary>An abstract native union selected by an exact generated discriminator.</summary>
    NestedPolymorphic
}

/// <summary>Describes one recursively classified mutable/getter value pair for generated construction.</summary>
internal sealed class NativeValueConstructionModel
{
    /// <summary>Initializes one complete recursive construction description.</summary>
    /// <param name="kind">The closed native construction category.</param>
    /// <param name="mutableType">The mutable native API type.</param>
    /// <param name="getterType">The corresponding getter API type.</param>
    /// <param name="nullability">The reflected getter nullability contract.</param>
    /// <param name="element">The wrapped or collection element construction model, or <see langword="null"/> for a terminal value.</param>
    internal NativeValueConstructionModel(
        NativeValueConstructionKind kind,
        Type mutableType,
        Type getterType,
        NullabilityInfo nullability,
        NativeValueConstructionModel? element = null)
    {
        ArgumentNullException.ThrowIfNull(mutableType);
        ArgumentNullException.ThrowIfNull(getterType);
        ArgumentNullException.ThrowIfNull(nullability);
        Kind = kind;
        MutableType = mutableType;
        GetterType = getterType;
        Nullability = nullability;
        Element = element;
    }

    /// <summary>Gets the closed native construction category.</summary>
    internal NativeValueConstructionKind Kind { get; }

    /// <summary>Gets the mutable native API type.</summary>
    internal Type MutableType { get; }

    /// <summary>Gets the corresponding getter API type.</summary>
    internal Type GetterType { get; }

    /// <summary>Gets the recursive getter nullability contract.</summary>
    internal NullabilityInfo Nullability { get; }

    /// <summary>Gets the wrapped or collection element model, or <see langword="null"/> for a terminal value.</summary>
    internal NativeValueConstructionModel? Element { get; }
}
