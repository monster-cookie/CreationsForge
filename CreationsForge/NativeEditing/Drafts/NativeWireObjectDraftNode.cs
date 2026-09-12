using System.Collections.ObjectModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.NativeEditing.Schema;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Represents one closed ordered object draft.</summary>
public class NativeWireObjectDraftNode : NativeWireDraftNode
{
    /// <summary>The immutable ordered properties.</summary>
    private readonly IReadOnlyList<NativeWireObjectDraftProperty> OrderedProperties;

    /// <summary>The immutable direct child projection.</summary>
    private readonly IReadOnlyList<NativeWireDraftNode> DirectChildren;

    /// <summary>Initializes one closed object draft.</summary>
    /// <param name="kind">The specialized or ordinary object node kind.</param>
    /// <param name="path">The exact JSON path.</param>
    /// <param name="displayName">The user-facing field name.</param>
    /// <param name="descriptor">The resolved object schema.</param>
    /// <param name="isRequired">Whether the containing object requires the value.</param>
    /// <param name="isReadOnly">Whether the value is derived.</param>
    /// <param name="valueState">How the object was initialized.</param>
    /// <param name="properties">The ordered typed properties.</param>
    internal NativeWireObjectDraftNode(
        NativeWireDraftNodeKind kind,
        string path,
        string displayName,
        NativeWireSchemaDescriptor descriptor,
        bool isRequired,
        bool isReadOnly,
        NativeWireDraftValueState valueState,
        IEnumerable<NativeWireObjectDraftProperty> properties)
        : base(kind, path, displayName, descriptor, isRequired, isReadOnly, valueState)
    {
        ArgumentNullException.ThrowIfNull(properties);
        var copy = properties.ToArray();
        OrderedProperties = Array.AsReadOnly(copy);
        DirectChildren = Array.AsReadOnly(copy.Select(property => property.Node).ToArray());
        foreach (var child in DirectChildren)
        {
            ObserveChild(child);
        }
    }

    /// <summary>Gets the exact ordered closed object properties.</summary>
    public IReadOnlyList<NativeWireObjectDraftProperty> Properties => OrderedProperties;

    /// <inheritdoc />
    public override IReadOnlyList<NativeWireDraftNode> Children => DirectChildren;

    /// <summary>Finds one direct property by its exact case-sensitive JSON name.</summary>
    /// <param name="name">The exact property name.</param>
    /// <returns>The property node, or <see langword="null"/> when the closed object has no such property.</returns>
    public NativeWireDraftNode? FindProperty(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        return OrderedProperties.FirstOrDefault(property => string.Equals(property.Name, name, StringComparison.Ordinal))?.Node;
    }

    /// <inheritdoc />
    protected override void RebaseChildren()
    {
        foreach (var property in OrderedProperties)
        {
            property.Node.RebasePath(AppendProperty(Path, property.Name));
        }
    }

    /// <summary>Appends one closed object property to a structural path.</summary>
    private static string AppendProperty(string path, string property)
    {
        return property.All(character => char.IsLetterOrDigit(character) || character is '_' or '$')
            ? $"{path}.{property}"
            : $"{path}['{property.Replace("'", "\\'", StringComparison.Ordinal)}']";
    }
}
