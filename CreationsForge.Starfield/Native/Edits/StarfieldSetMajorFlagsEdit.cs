using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.Native.Edits;

/// <summary>Sets the complete known Starfield major-record flag set for a FormList.</summary>
public sealed class StarfieldSetMajorFlagsEdit : FormListEdit
{
    /// <summary>Initializes a typed Starfield major-record flag replacement.</summary>
    /// <param name="flags">The requested typed Starfield flag combination. Preparation rejects unknown bits.</param>
    public StarfieldSetMajorFlagsEdit(StarfieldMajorRecord.StarfieldMajorRecordFlag flags)
        : base("starfield.form-list.set-major-flags")
    {
        Flags = flags;
    }

    /// <summary>Gets the complete requested Starfield major-record flag combination.</summary>
    public StarfieldMajorRecord.StarfieldMajorRecordFlag Flags { get; }
}
