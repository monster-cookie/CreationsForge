using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.Native.Edits;

/// <summary>Replaces every ordered FormList conditional entry with complete native payloads.</summary>
public sealed class StarfieldSetConditionalEntriesEdit : FormListEdit
{
    /// <summary>Initializes an ordered conditional-entry replacement.</summary>
    /// <param name="entries">The entries whose indices, concrete conditions, order, and nullable condition collections are copied during preparation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the collection or one of its entries is <see langword="null"/>.</exception>
    public StarfieldSetConditionalEntriesEdit(IReadOnlyList<IFormListConditionalEntryGetter> entries)
        : base("starfield.form-list.set-conditional-entries")
    {
        ArgumentNullException.ThrowIfNull(entries);
        var snapshot = entries.ToArray();
        if (snapshot.Any(static entry => entry is null))
        {
            throw new ArgumentNullException(nameof(entries), "Conditional-entry collections cannot contain null entries.");
        }

        Entries = Array.AsReadOnly(snapshot);
    }

    /// <summary>Gets the caller-owned entry references that preparation must deeply copy before publication.</summary>
    public IReadOnlyList<IFormListConditionalEntryGetter> Entries { get; }
}
