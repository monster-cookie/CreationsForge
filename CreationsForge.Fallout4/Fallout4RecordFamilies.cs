using CreationsForge.Engine.Records;
using Mutagen.Bethesda.Fallout4;

namespace CreationsForge.Fallout4;

/// <summary>Discovers the Fallout 4 record families from the referenced Mutagen assembly.</summary>
internal static class Fallout4RecordFamilies
{
    private static readonly string[] Unwritable = ["DamageTypeIndexed"];

    /// <summary>Gets the record families Mutagen can construct and store for this package.</summary>
    public static IReadOnlyList<RecordFamily> All { get; } = MutagenRecordCatalog.Discover(typeof(IFallout4Mod), Fallout4Release.Fallout4, Unwritable);
}
