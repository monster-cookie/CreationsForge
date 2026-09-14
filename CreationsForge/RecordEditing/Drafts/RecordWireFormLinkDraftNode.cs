using System.ComponentModel;
using System.Globalization;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Represents one canonical FormLink value while preserving JSON null and canonical <c>Null</c> identities.</summary>
public sealed class RecordWireFormLinkDraftNode : RecordWireDraftNode
{
    /// <summary>The exact schema-bound record wrapper discriminator, when this is a generated FormLink.</summary>
    private readonly string? RecordTypeDiscriminator;

    /// <summary>Whether the link is explicitly null.</summary>
    private bool CurrentIsNull;

    /// <summary>The exact canonical FormKey string or JSON-null identity.</summary>
    private string? CurrentFormKey;

    /// <summary>Initializes one FormLink draft.</summary>
    /// <param name="path">The exact JSON path.</param>
    /// <param name="displayName">The user-facing field name.</param>
    /// <param name="descriptor">The resolved FormLink schema.</param>
    /// <param name="isRequired">Whether the containing object requires the link.</param>
    /// <param name="isReadOnly">Whether the link is derived.</param>
    /// <param name="valueState">How the link was initialized.</param>
    /// <param name="isNull">Whether the link has record null identity.</param>
    /// <param name="formKey">The exact canonical FormKey string or JSON-null identity.</param>
    /// <param name="typeDiscriminator">The exact supplied generated-wrapper discriminator, or <see langword="null"/> for compact command links.</param>
    internal RecordWireFormLinkDraftNode(string path, string displayName, RecordWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, RecordWireDraftValueState valueState, bool isNull, string? formKey, string? typeDiscriminator = null)
        : base(RecordWireDraftNodeKind.FormLink, path, displayName, descriptor, isRequired, isReadOnly, valueState)
    {
        CurrentIsNull = isNull;
        CurrentFormKey = formKey;
        RecordTypeDiscriminator = typeDiscriminator;
    }

    /// <summary>Gets or sets whether the record link is explicitly null.</summary>
    public bool IsNull
    {
        get => CurrentIsNull;
        set
        {
            if (IsReadOnly || CurrentIsNull == value)
            {
                return;
            }

            CurrentIsNull = value;
            MarkChanged();
        }
    }

    /// <summary>Gets or sets the exact canonical FormKey string; <see langword="null"/> preserves a JSON-null link identity.</summary>
    public string? FormKey
    {
        get => CurrentFormKey;
        set
        {
            if (IsReadOnly || string.Equals(CurrentFormKey, value, StringComparison.Ordinal))
            {
                return;
            }

            CurrentFormKey = value;
            MarkChanged();
        }
    }

    /// <summary>Gets the catalog-declared reference target, when available.</summary>
    public string? ReferenceTarget => Descriptor.Annotations.GetValueOrDefault("x-record-reference-target");

    /// <summary>Gets the exact supplied generated-wrapper discriminator, or <see langword="null"/> for compact command links.</summary>
    public string? TypeDiscriminator => RecordTypeDiscriminator;

    /// <inheritdoc />
    public override IReadOnlyList<RecordWireDraftNode> Children => Array.Empty<RecordWireDraftNode>();
}
