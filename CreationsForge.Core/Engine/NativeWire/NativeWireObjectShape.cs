namespace CreationsForge.Core.Engine.NativeWire;

/// <summary>Defines the complete ordinal property set accepted by one generated native wire object.</summary>
public sealed class NativeWireObjectShape
{
    /// <summary>Maps every accepted property name to its stable ordinal.</summary>
    private readonly IReadOnlyDictionary<string, int> Ordinals;

    /// <summary>Initializes a closed object shape and snapshots its required and optional property names.</summary>
    /// <param name="name">The non-empty diagnostic name of the object shape.</param>
    /// <param name="requiredProperties">Every property that must occur exactly once.</param>
    /// <param name="optionalProperties">Every property that may occur at most once.</param>
    /// <exception cref="ArgumentException">Thrown when the name or a property name is empty, or property names are duplicated.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="requiredProperties"/> is <see langword="null"/>.</exception>
    public NativeWireObjectShape(
        string name,
        IReadOnlyList<string> requiredProperties,
        IReadOnlyList<string>? optionalProperties = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(requiredProperties);

        var required = requiredProperties.ToArray();
        var optional = optionalProperties?.ToArray() ?? Array.Empty<string>();
        var ordinals = new Dictionary<string, int>(required.Length + optional.Length, StringComparer.Ordinal);
        for (var index = 0; index < required.Length; index++)
        {
            ValidateAndAdd(ordinals, required[index], index, nameof(requiredProperties));
        }

        for (var index = 0; index < optional.Length; index++)
        {
            ValidateAndAdd(ordinals, optional[index], required.Length + index, nameof(optionalProperties));
        }

        Name = name;
        RequiredProperties = Array.AsReadOnly(required);
        OptionalProperties = Array.AsReadOnly(optional);
        Ordinals = ordinals;
    }

    /// <summary>Gets the diagnostic name of this object shape.</summary>
    public string Name { get; }

    /// <summary>Gets every property that must occur exactly once, in generated field order.</summary>
    public IReadOnlyList<string> RequiredProperties { get; }

    /// <summary>Gets every property that may occur at most once, in generated field order.</summary>
    public IReadOnlyList<string> OptionalProperties { get; }

    /// <summary>Gets the total number of accepted property names.</summary>
    internal int PropertyCount => RequiredProperties.Count + OptionalProperties.Count;

    /// <summary>Gets whether the property ordinal identifies a required property.</summary>
    /// <param name="ordinal">The accepted property ordinal.</param>
    /// <returns><see langword="true"/> when the property is required.</returns>
    internal bool IsRequired(int ordinal)
    {
        return ordinal >= 0 && ordinal < RequiredProperties.Count;
    }

    /// <summary>Finds the stable ordinal for one accepted property name.</summary>
    /// <param name="name">The exact ordinal property name.</param>
    /// <returns>The property ordinal, or <c>-1</c> when the property is unknown.</returns>
    internal int GetOrdinal(string name)
    {
        return Ordinals.TryGetValue(name, out var ordinal) ? ordinal : -1;
    }

    /// <summary>Validates and adds one property name to the ordinal map.</summary>
    /// <param name="ordinals">The map being constructed.</param>
    /// <param name="name">The property name to validate.</param>
    /// <param name="ordinal">The stable property ordinal.</param>
    /// <param name="parameterName">The constructor parameter reported for invalid input.</param>
    /// <exception cref="ArgumentException">Thrown when the property name is empty or duplicated.</exception>
    private static void ValidateAndAdd(
        IDictionary<string, int> ordinals,
        string name,
        int ordinal,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Native wire property names cannot be empty or whitespace.", parameterName);
        }

        if (!ordinals.TryAdd(name, ordinal))
        {
            throw new ArgumentException($"Native wire property '{name}' is duplicated.", parameterName);
        }
    }
}
