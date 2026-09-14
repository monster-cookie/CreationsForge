using System.Buffers;
using System.Text.Json;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Writes typed draft nodes to detached command JSON without record construction.</summary>
internal static class RecordWireDraftJsonWriter
{
    /// <summary>Serializes one root node to a detached JSON value.</summary>
    internal static JsonElement WriteDetached(RecordWireDraftNode root, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(root);
        cancellationToken.ThrowIfCancellationRequested();
        var buffer = new ArrayBufferWriter<byte>();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            WriteNode(writer, root, cancellationToken);
        }

        using var document = JsonDocument.Parse(buffer.WrittenMemory);
        return document.RootElement.Clone();
    }

    /// <summary>Writes one typed node recursively.</summary>
    internal static void WriteNode(Utf8JsonWriter writer, RecordWireDraftNode node, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        switch (node)
        {
            case RecordWireNullableDraftNode nullable:
                if (nullable.IsNull)
                {
                    writer.WriteNullValue();
                }
                else if (nullable.Value is not null)
                {
                    WriteNode(writer, nullable.Value, cancellationToken);
                }
                else
                {
                    throw new InvalidOperationException($"{nullable.Path}: present nullable draft has no typed value.");
                }

                break;
            case RecordWireUnionDraftNode union when union.SelectedValue is not null:
                WriteNode(writer, union.SelectedValue, cancellationToken);
                break;
            case RecordWireUnionDraftNode union:
                throw new InvalidOperationException($"{union.Path}: union has no selected value.");
            case RecordWireFormLinkOrIndexDraftNode formLinkOrIndex:
                WriteFormLinkOrIndex(writer, formLinkOrIndex, cancellationToken);
                break;
            case RecordWireFormLinkDraftNode formLink:
                WriteFormLink(writer, formLink);
                break;
            case RecordWireObjectDraftNode objectNode:
                writer.WriteStartObject();
                foreach (var property in objectNode.Properties)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (property.Node.ValueState == RecordWireDraftValueState.RequiredUnset && !property.Node.IsRequired)
                    {
                        continue;
                    }

                    writer.WritePropertyName(property.Name);
                    WriteNode(writer, property.Node, cancellationToken);
                }

                writer.WriteEndObject();
                break;
            case RecordWireArrayDraftNode array:
                writer.WriteStartArray();
                foreach (var item in array.Items)
                {
                    WriteNode(writer, item, cancellationToken);
                }

                writer.WriteEndArray();
                break;
            case RecordWireBooleanDraftNode boolean:
                writer.WriteBooleanValue(boolean.Value);
                break;
            case RecordWireIntegerDraftNode integer:
                writer.WriteRawValue(integer.Text, skipInputValidation: false);
                break;
            case RecordWireStringDraftNode text when text.Descriptor.Annotations.ContainsKey("x-presentation-json-number"):
                writer.WriteRawValue(text.Value, skipInputValidation: false);
                break;
            case RecordWireStringDraftNode text:
                writer.WriteStringValue(text.Value);
                break;
            default:
                throw new InvalidOperationException($"{node.Path}: unsupported typed draft node {node.GetType().Name}.");
        }
    }

    /// <summary>Writes one FormLink object in schema order, including exact constants when required.</summary>
    private static void WriteFormLink(Utf8JsonWriter writer, RecordWireFormLinkDraftNode node)
    {
        writer.WriteStartObject();
        foreach (var property in node.Descriptor.Properties)
        {
            switch (property.Name)
            {
                case "$type" when property.Descriptor.Constant.HasValue:
                    writer.WritePropertyName(property.Name);
                    property.Descriptor.Constant.Value.WriteTo(writer);
                    break;
                case "$type" when node.TypeDiscriminator is not null:
                    writer.WriteString(property.Name, node.TypeDiscriminator);
                    break;
                case "isNull":
                    writer.WriteBoolean(property.Name, node.IsNull);
                    break;
                case "formKey":
                    writer.WritePropertyName(property.Name);
                    if (node.FormKey is null)
                    {
                        writer.WriteNullValue();
                    }
                    else
                    {
                        writer.WriteStringValue(node.FormKey);
                    }

                    break;
            }
        }

        writer.WriteEndObject();
    }

    /// <summary>Writes one FormLink-or-index with both exact projections and derived owner mode.</summary>
    private static void WriteFormLinkOrIndex(Utf8JsonWriter writer, RecordWireFormLinkOrIndexDraftNode node, CancellationToken cancellationToken)
    {
        writer.WriteStartObject();
        foreach (var property in node.Descriptor.Properties)
        {
            switch (property.Name)
            {
                case "$type" when property.Descriptor.Constant.HasValue:
                    writer.WritePropertyName(property.Name);
                    property.Descriptor.Constant.Value.WriteTo(writer);
                    break;
                case "index":
                    writer.WritePropertyName(property.Name);
                    if (node.IsIndexNull)
                    {
                        writer.WriteNullValue();
                    }
                    else
                    {
                        writer.WriteRawValue(node.IndexText, skipInputValidation: false);
                    }

                    break;
                case "link":
                    writer.WritePropertyName(property.Name);
                    WriteNode(writer, node.Link, cancellationToken);
                    break;
                case "usesLink" when property.IsRequired:
                    writer.WriteBoolean(property.Name, node.UsesLink);
                    break;
                case "usesAlias" when property.IsRequired:
                    writer.WriteBoolean(property.Name, node.UsesAlias);
                    break;
                case "usesPackageData" when property.IsRequired:
                    writer.WriteBoolean(property.Name, node.UsesPackageData);
                    break;
            }
        }

        writer.WriteEndObject();
    }
}
