using System.Text.Json;
using CreationsForge.Core.Engine.NativeWire;

namespace CreationsForge.NativeEditing.Schema;

/// <summary>Contains the resolved closed presentation shape and native annotations for one schema value.</summary>
public sealed class NativeWireSchemaDescriptor
{
    /// <summary>Initializes one immutable schema descriptor.</summary>
    /// <param name="kind">The closed presentation value kind.</param>
    /// <param name="title">The catalog title or stable fallback display name.</param>
    /// <param name="description">The optional catalog description.</param>
    /// <param name="sourceKey">The exact catalog node that owns this graph.</param>
    /// <param name="defaultTemplate">The optional complete catalog node default.</param>
    /// <param name="properties">The ordered closed object properties.</param>
    /// <param name="unionOptions">The lightweight lazy union alternatives.</param>
    /// <param name="itemDescriptor">The array item descriptor, when applicable.</param>
    /// <param name="nonNullDescriptor">The non-null shape of a nullable value, when applicable.</param>
    /// <param name="constant">The explicit schema constant, when present.</param>
    /// <param name="defaultValue">The explicit schema default, when present.</param>
    /// <param name="enumValues">The known symbolic enum values.</param>
    /// <param name="annotations">The immutable native annotation map.</param>
    /// <param name="minimum">The inclusive integer minimum as exact text.</param>
    /// <param name="maximum">The inclusive integer maximum as exact text.</param>
    /// <param name="minimumLength">The inclusive minimum string length.</param>
    /// <param name="maximumLength">The inclusive maximum string length.</param>
    /// <param name="maximumItems">The inclusive array element limit.</param>
    /// <param name="pattern">The optional regular-expression pattern.</param>
    /// <param name="format">The optional string format.</param>
    /// <param name="allowsNull">Whether the schema permits a whole JSON null value.</param>
    internal NativeWireSchemaDescriptor(
        NativeWireSchemaValueKind kind,
        string title,
        string? description,
        NativeWireSchemaNodeKey? sourceKey = null,
        JsonElement? defaultTemplate = null,
        IReadOnlyList<NativeWireSchemaProperty>? properties = null,
        IReadOnlyList<NativeWireSchemaUnionOption>? unionOptions = null,
        NativeWireSchemaDescriptor? itemDescriptor = null,
        NativeWireSchemaDescriptor? nonNullDescriptor = null,
        JsonElement? constant = null,
        JsonElement? defaultValue = null,
        IReadOnlyList<NativeWireSchemaEnumValue>? enumValues = null,
        IReadOnlyDictionary<string, string?>? annotations = null,
        string? minimum = null,
        string? maximum = null,
        int? minimumLength = null,
        int? maximumLength = null,
        int? maximumItems = null,
        string? pattern = null,
        string? format = null,
        bool allowsNull = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        Kind = kind;
        Title = title;
        Description = description;
        SourceKey = sourceKey;
        DefaultTemplate = defaultTemplate?.Clone();
        Properties = Array.AsReadOnly(properties?.ToArray() ?? Array.Empty<NativeWireSchemaProperty>());
        UnionOptions = Array.AsReadOnly(unionOptions?.ToArray() ?? Array.Empty<NativeWireSchemaUnionOption>());
        ItemDescriptor = itemDescriptor;
        NonNullDescriptor = nonNullDescriptor;
        Constant = constant?.Clone();
        DefaultValue = defaultValue?.Clone();
        EnumValues = Array.AsReadOnly(enumValues?.ToArray() ?? Array.Empty<NativeWireSchemaEnumValue>());
        Annotations = new System.Collections.ObjectModel.ReadOnlyDictionary<string, string?>(
            (annotations ?? new Dictionary<string, string?>()).ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal));
        Minimum = minimum;
        Maximum = maximum;
        MinimumLength = minimumLength;
        MaximumLength = maximumLength;
        MaximumItems = maximumItems;
        Pattern = pattern;
        Format = format;
        AllowsNull = allowsNull;
    }

    /// <summary>Gets the closed presentation value kind.</summary>
    public NativeWireSchemaValueKind Kind { get; }

    /// <summary>Gets the catalog title or stable fallback display name.</summary>
    public string Title { get; }

    /// <summary>Gets the optional catalog description.</summary>
    public string? Description { get; }

    /// <summary>Gets the exact catalog node key that owns this resolved graph, when available.</summary>
    public NativeWireSchemaNodeKey? SourceKey { get; }

    /// <summary>Gets the detached complete type or command default template, when supplied.</summary>
    public JsonElement? DefaultTemplate { get; }

    /// <summary>Gets the ordered closed object properties.</summary>
    public IReadOnlyList<NativeWireSchemaProperty> Properties { get; }

    /// <summary>Gets the lightweight union alternatives.</summary>
    public IReadOnlyList<NativeWireSchemaUnionOption> UnionOptions { get; }

    /// <summary>Gets the array item descriptor, or <see langword="null"/> for non-array values.</summary>
    public NativeWireSchemaDescriptor? ItemDescriptor { get; }

    /// <summary>Gets the non-null shape of a nullable value, or <see langword="null"/> otherwise.</summary>
    public NativeWireSchemaDescriptor? NonNullDescriptor { get; }

    /// <summary>Gets a detached explicit schema constant, when present.</summary>
    public JsonElement? Constant { get; }

    /// <summary>Gets a detached explicit property default, when present.</summary>
    public JsonElement? DefaultValue { get; }

    /// <summary>Gets known symbolic values for an enum while unknown numeric values remain permitted.</summary>
    public IReadOnlyList<NativeWireSchemaEnumValue> EnumValues { get; }

    /// <summary>Gets the preserved native <c>x-*</c> annotations.</summary>
    public IReadOnlyDictionary<string, string?> Annotations { get; }

    /// <summary>Gets the inclusive integer minimum as exact decimal text.</summary>
    public string? Minimum { get; }

    /// <summary>Gets the inclusive integer maximum as exact decimal text.</summary>
    public string? Maximum { get; }

    /// <summary>Gets the inclusive minimum string length.</summary>
    public int? MinimumLength { get; }

    /// <summary>Gets the inclusive maximum string length.</summary>
    public int? MaximumLength { get; }

    /// <summary>Gets the maximum array element count.</summary>
    public int? MaximumItems { get; }

    /// <summary>Gets the optional regular-expression pattern.</summary>
    public string? Pattern { get; }

    /// <summary>Gets the optional string format or content encoding.</summary>
    public string? Format { get; }

    /// <summary>Gets whether the whole value may be JSON null.</summary>
    public bool AllowsNull { get; }
}
