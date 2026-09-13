using System.Text.Json;

namespace CreationsForge.Core.Engine.RecordWire;

/// <summary>Provides one detached schema node and its optional record-constructible default template.</summary>
public sealed class RecordWireSchemaNode
{
    /// <summary>Initializes one immutable detached schema node.</summary>
    /// <param name="key">The content-bound node identity.</param>
    /// <param name="schema">The complete command or type metadata object.</param>
    /// <param name="defaultTemplate">A syntax-valid record-constructible template, or <see langword="null"/> when no honest default exists.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the schema is not an object or a supplied template is undefined.</exception>
    public RecordWireSchemaNode(
        RecordWireSchemaNodeKey key,
        JsonElement schema,
        JsonElement? defaultTemplate)
    {
        ArgumentNullException.ThrowIfNull(key);
        if (schema.ValueKind != JsonValueKind.Object)
        {
            throw new ArgumentException("A record wire schema node must contain a JSON object.", nameof(schema));
        }

        if (defaultTemplate.HasValue && defaultTemplate.Value.ValueKind == JsonValueKind.Undefined)
        {
            throw new ArgumentException("A supplied record wire default template cannot be undefined.", nameof(defaultTemplate));
        }

        Key = key;
        Schema = schema.Clone();
        DefaultTemplate = defaultTemplate?.Clone();
    }

    /// <summary>Gets the content-bound node identity.</summary>
    public RecordWireSchemaNodeKey Key { get; }

    /// <summary>Gets detached complete command or type metadata.</summary>
    public JsonElement Schema { get; }

    /// <summary>Gets a detached syntax-valid record-constructible template, or <see langword="null"/> when no honest default exists.</summary>
    public JsonElement? DefaultTemplate { get; }
}
