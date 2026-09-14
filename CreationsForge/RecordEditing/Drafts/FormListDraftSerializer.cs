using CreationsForge.Core.Engine.RecordWire;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Validates and serializes typed FormList command drafts to detached transient arguments.</summary>
public sealed class FormListDraftSerializer : IFormListDraftSerializer
{
    /// <summary>The validator used before every serialization.</summary>
    private readonly IFormListDraftValidator Validator;

    /// <summary>Initializes a serializer with the authoritative presentation validator.</summary>
    /// <param name="validator">The typed draft validator.</param>
    public FormListDraftSerializer(IFormListDraftValidator validator)
    {
        ArgumentNullException.ThrowIfNull(validator);
        Validator = validator;
    }

    /// <inheritdoc />
    public FormListDraftSerializationResult Serialize(FormListDraft draft, RecordWireReadLimits limits, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(limits);
        cancellationToken.ThrowIfCancellationRequested();
        var validation = Validator.Validate(draft, limits, cancellationToken);
        if (!validation.IsValid)
        {
            return new FormListDraftSerializationResult(null, validation.Issues);
        }

        var arguments = RecordWireDraftJsonWriter.WriteDetached(draft.Root, cancellationToken);
        return new FormListDraftSerializationResult(arguments, Array.Empty<RecordWireDraftIssue>());
    }
}
