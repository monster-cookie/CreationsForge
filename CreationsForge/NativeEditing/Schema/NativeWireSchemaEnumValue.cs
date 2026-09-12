using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;

namespace CreationsForge.NativeEditing.Schema;

/// <summary>Describes one known symbolic integer value without excluding unknown numeric values.</summary>
public sealed class NativeWireSchemaEnumValue
{
    /// <summary>Initializes one symbolic enum value.</summary>
    /// <param name="name">The display symbol emitted by the catalog.</param>
    /// <param name="value">The canonical signed or unsigned decimal integer text.</param>
    public NativeWireSchemaEnumValue(string name, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Name = name;
        Value = value;
    }

    /// <summary>Gets the catalog-provided symbolic name.</summary>
    public string Name { get; }

    /// <summary>Gets the exact canonical integer text.</summary>
    public string Value { get; }
}
