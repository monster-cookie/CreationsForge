using CreationsForge.Engine.Records;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim;

/// <summary>Discovers the Skyrim record families from the referenced Mutagen assembly.</summary>
internal static class SkyrimRecordFamilies
{
    /// <summary>Gets the record families Mutagen can construct and store for this package.</summary>
    public static IReadOnlyList<RecordFamily> All { get; } = MutagenRecordCatalog.Discover(typeof(ISkyrimMod), SkyrimRelease.SkyrimSE, []);
}
