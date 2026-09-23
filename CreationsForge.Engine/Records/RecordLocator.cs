using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Engine.Records;

/// <summary>Identifies one exact record version by family, origin identity, and containing plugin.</summary>
public sealed class RecordLocator
{
    /// <summary>Initializes an exact record locator.</summary>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="formKey">The record's origin identity.</param>
    /// <param name="containingModKey">The plugin containing the exact version.</param>
    public RecordLocator(string familyId, FormKey formKey, ModKey containingModKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(familyId);
        if (formKey.IsNull)
        {
            throw new ArgumentException("A record locator requires a non-null FormKey.", nameof(formKey));
        }

        FamilyId = familyId;
        FormKey = formKey;
        ContainingModKey = containingModKey;
    }

    /// <summary>Gets the declared family identifier.</summary>
    public string FamilyId { get; }

    /// <summary>Gets the record's origin identity.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the plugin containing the exact version.</summary>
    public ModKey ContainingModKey { get; }
}
