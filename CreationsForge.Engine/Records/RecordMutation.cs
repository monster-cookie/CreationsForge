using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Engine.Records;

/// <summary>Describes one record create or exact-context override within an atomic change set.</summary>
public sealed class RecordMutation
{
    private RecordMutation(
        RecordMutationKind kind,
        string familyId,
        FormKey? formKey,
        ModKey? containingModKey,
        IReadOnlyList<RecordFieldChange> changes)
    {
        Kind = kind;
        FamilyId = familyId;
        FormKey = formKey;
        ContainingModKey = containingModKey;
        Changes = changes;
    }

    /// <summary>Gets whether this mutation creates or overrides a record.</summary>
    public RecordMutationKind Kind { get; }

    /// <summary>Gets the stable declared family identifier.</summary>
    public string FamilyId { get; }

    /// <summary>Gets the origin identity selected for an override.</summary>
    public FormKey? FormKey { get; }

    /// <summary>Gets the exact containing plugin selected for an override.</summary>
    public ModKey? ContainingModKey { get; }

    /// <summary>Gets the ordered field changes.</summary>
    public IReadOnlyList<RecordFieldChange> Changes { get; }

    /// <summary>Creates a mutation that allocates and publishes one new record.</summary>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="changes">Initial field changes.</param>
    /// <returns>The create mutation.</returns>
    public static RecordMutation Create(string familyId, IEnumerable<RecordFieldChange> changes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(familyId);
        ArgumentNullException.ThrowIfNull(changes);
        return new RecordMutation(RecordMutationKind.Create, familyId, null, null, changes.ToArray());
    }

    /// <summary>Creates a mutation that copies and edits one exact selected context as an override.</summary>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="formKey">The record's origin identity.</param>
    /// <param name="containingModKey">The exact plugin containing the selected version.</param>
    /// <param name="changes">Field changes applied to the copied record candidate.</param>
    /// <returns>The exact-context override mutation.</returns>
    public static RecordMutation Override(
        string familyId,
        FormKey formKey,
        ModKey containingModKey,
        IEnumerable<RecordFieldChange> changes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(familyId);
        ArgumentNullException.ThrowIfNull(changes);
        if (formKey.IsNull)
        {
            throw new ArgumentException("An override requires a non-null FormKey.", nameof(formKey));
        }

        return new RecordMutation(RecordMutationKind.Override, familyId, formKey, containingModKey, changes.ToArray());
    }
}
