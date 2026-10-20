using CreationsForge.Engine.Records;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield;

/// <summary>Discovers the Starfield record families from the referenced Mutagen assembly.</summary>
internal static class StarfieldRecordFamilies
{
    private static readonly string[] Unwritable = ["ContainerModification", "UnknownObjectModification"];

    /// <summary>Gets the record families Mutagen can construct and store for this package.</summary>
    public static IReadOnlyList<RecordFamily> All { get; } = MutagenRecordCatalog.Discover(typeof(IStarfieldMod), StarfieldRelease.Starfield, Unwritable);
}
