using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;

namespace CreationsForge.NativeEditing.Schema;

/// <summary>Describes one ordered property of a closed native wire object.</summary>
public sealed class NativeWireSchemaProperty
{
    /// <summary>Initializes one immutable object-property descriptor.</summary>
    /// <param name="name">The exact case-sensitive JSON property name.</param>
    /// <param name="order">The catalog property order.</param>
    /// <param name="isRequired">Whether the closed object requires the property.</param>
    /// <param name="isReadOnly">Whether the value is derived and cannot be changed directly.</param>
    /// <param name="descriptor">The resolved property shape.</param>
    public NativeWireSchemaProperty(
        string name,
        int order,
        bool isRequired,
        bool isReadOnly,
        NativeWireSchemaDescriptor descriptor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegative(order);
        ArgumentNullException.ThrowIfNull(descriptor);
        Name = name;
        Order = order;
        IsRequired = isRequired;
        IsReadOnly = isReadOnly;
        Descriptor = descriptor;
    }

    /// <summary>Gets the exact case-sensitive JSON property name.</summary>
    public string Name { get; }

    /// <summary>Gets the stable catalog property order.</summary>
    public int Order { get; }

    /// <summary>Gets whether the containing object requires this property.</summary>
    public bool IsRequired { get; }

    /// <summary>Gets whether the property is derived and cannot be changed directly.</summary>
    public bool IsReadOnly { get; }

    /// <summary>Gets the resolved property shape, including any local reference siblings.</summary>
    public NativeWireSchemaDescriptor Descriptor { get; }
}
