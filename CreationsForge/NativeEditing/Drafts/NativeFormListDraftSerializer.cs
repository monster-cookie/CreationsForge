using CreationsForge.Core.Engine.NativeWire;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Validates and serializes typed native FormList command drafts to detached transient arguments.</summary>
public sealed class NativeFormListDraftSerializer : INativeFormListDraftSerializer
{
    /// <summary>The validator used before every serialization.</summary>
    private readonly INativeFormListDraftValidator Validator;

    /// <summary>Initializes a serializer with the authoritative presentation validator.</summary>
    /// <param name="validator">The typed draft validator.</param>
    public NativeFormListDraftSerializer(INativeFormListDraftValidator validator)
    {
        ArgumentNullException.ThrowIfNull(validator);
        Validator = validator;
    }

    /// <inheritdoc />
    public NativeFormListDraftSerializationResult Serialize(NativeFormListDraft draft, NativeWireReadLimits limits, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(limits);
        cancellationToken.ThrowIfCancellationRequested();
        var validation = Validator.Validate(draft, limits, cancellationToken);
        if (!validation.IsValid)
        {
            return new NativeFormListDraftSerializationResult(null, validation.Issues);
        }

        var arguments = NativeWireDraftJsonWriter.WriteDetached(draft.Root, cancellationToken);
        return new NativeFormListDraftSerializationResult(arguments, Array.Empty<NativeWireDraftIssue>());
    }
}
