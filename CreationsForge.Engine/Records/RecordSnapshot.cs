using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Engine.Records;

/// <summary>Captures registered values from one exact record without becoming record authority.</summary>
public sealed class RecordSnapshot
{
    /// <summary>Initializes a transient record snapshot.</summary>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="formKey">The record's origin identity.</param>
    /// <param name="containingModKey">The plugin containing this exact version.</param>
    /// <param name="values">Registered field values keyed by discovered path.</param>
    public RecordSnapshot(
        string familyId,
        FormKey formKey,
        ModKey containingModKey,
        IReadOnlyDictionary<string, RecordValue> values)
    {
        FamilyId = familyId;
        FormKey = formKey;
        ContainingModKey = containingModKey;
        Values = values;
    }

    /// <summary>Gets the declared family identifier.</summary>
    public string FamilyId { get; }

    /// <summary>Gets the record's origin identity.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the plugin containing this exact version.</summary>
    public ModKey ContainingModKey { get; }

    /// <summary>Gets registered field values keyed by discovered path.</summary>
    public IReadOnlyDictionary<string, RecordValue> Values { get; }
}
