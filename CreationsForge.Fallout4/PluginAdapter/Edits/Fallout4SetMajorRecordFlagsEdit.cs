using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Fallout4;

namespace CreationsForge.Fallout4.PluginAdapter.Edits;

/// <summary>Sets the complete typed Fallout 4 major-record flag set for a FormList.</summary>
public sealed class Fallout4SetMajorRecordFlagsEdit : FormListEdit
{
    /// <summary>Initializes a typed Fallout 4 major-record flag replacement command.</summary>
    /// <param name="majorRecordFlags">The typed Mutagen flag combination to assign after supported-bit validation.</param>
    public Fallout4SetMajorRecordFlagsEdit(Fallout4MajorRecord.Fallout4MajorRecordFlag majorRecordFlags)
        : base("fallout4.form-list.set-major-record-flags")
    {
        MajorRecordFlags = majorRecordFlags;
    }

    /// <summary>Gets the typed Mutagen Fallout 4 major-record flag combination.</summary>
    public Fallout4MajorRecord.Fallout4MajorRecordFlag MajorRecordFlags { get; }
}
