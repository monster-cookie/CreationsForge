using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim.PluginAdapter;

/// <summary>Sets the Skyrim-specific typed major-record flags supported by the installed Mutagen API.</summary>
public sealed class SkyrimSetMajorRecordFlagsEdit : FormListEdit
{
    /// <summary>Initializes a Skyrim typed major-record flag command.</summary>
    /// <param name="majorRecordFlags">The exact supported Skyrim flag combination to assign while retaining unrelated raw header bits.</param>
    public SkyrimSetMajorRecordFlagsEdit(
        SkyrimMajorRecord.SkyrimMajorRecordFlag majorRecordFlags)
        : base("skyrim.form-list.set-major-record-flags")
    {
        MajorRecordFlags = majorRecordFlags;
    }

    /// <summary>Gets the exact Skyrim typed major-record flag combination to assign.</summary>
    public SkyrimMajorRecord.SkyrimMajorRecordFlag MajorRecordFlags { get; }
}
