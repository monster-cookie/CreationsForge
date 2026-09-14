using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;

namespace CreationsForge.RecordEditing.Schema;

/// <summary>Describes one known symbolic integer value without excluding unknown numeric values.</summary>
public sealed class RecordWireSchemaEnumValue
{
    /// <summary>Initializes one symbolic enum value.</summary>
    /// <param name="name">The display symbol emitted by the catalog.</param>
    /// <param name="value">The canonical signed or unsigned decimal integer text.</param>
    public RecordWireSchemaEnumValue(string name, string value)
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
