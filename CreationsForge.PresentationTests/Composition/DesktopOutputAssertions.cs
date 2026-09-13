using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.Fallout4.PluginAdapter.RecordInspection;
using CreationsForge.Skyrim.PluginAdapter.RecordInspection;
using CreationsForge.Starfield.PluginAdapter.RecordInspection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Shouldly;
using Fallout4Mod = Mutagen.Bethesda.Fallout4.Fallout4Mod;
using Fallout4Release = Mutagen.Bethesda.Fallout4.Fallout4Release;
using SkyrimMod = Mutagen.Bethesda.Skyrim.SkyrimMod;
using SkyrimRelease = Mutagen.Bethesda.Skyrim.SkyrimRelease;
using StarfieldMod = Mutagen.Bethesda.Starfield.StarfieldMod;
using StarfieldRelease = Mutagen.Bethesda.Starfield.StarfieldRelease;

namespace CreationsForge.PresentationTests.Composition;

/// <summary>Reads complete fixture output binaries directly through plugin parsers independently of the desktop coordinator.</summary>
internal static class DesktopOutputAssertions
{
    /// <summary>Requires the entire output to contain the exact FormList count and ordered masters before returning complete record views.</summary>
    /// <param name="fixture">The known full-master source and output fixture.</param>
    /// <param name="expectedRecordCount">The exact total major-record count; every record must be a FormList.</param>
    /// <param name="expectedMasters">The exact expected output master order.</param>
    /// <returns>Detached complete plugin inspector values keyed by their real FormKeys.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the fixture selects an unsupported game.</exception>
    public static IReadOnlyDictionary<FormKey, JsonElement> ReadAllFormLists(
        DesktopWorkspaceFixture fixture,
        int expectedRecordCount,
        IReadOnlyList<ModKey> expectedMasters)
    {
        var masterFlags = new Cache<IModMasterStyledGetter, ModKey>(metadata => metadata.ModKey);
        masterFlags.Set(new KeyedMasterStyle(fixture.SourceFormKey.ModKey, MasterStyle.Full));
        masterFlags.Set(new KeyedMasterStyle(fixture.ExistingOutput.ModKey, MasterStyle.Full));
        var metadata = ParsingMeta.Factory(
            new BinaryReadParameters { MasterFlagsLookup = masterFlags, ThrowOnUnknownSubrecord = true },
            fixture.Release,
            new ModPath(fixture.ExistingOutput.ModKey, fixture.ExistingOutput.PluginPath));
        using var stream = new FileStream(fixture.ExistingOutput.PluginPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var binaryStream = new MutagenBinaryReadStream(stream, metadata, bufferSize: 4096, dispose: true, offsetReference: 0);
        var frame = new MutagenFrame(binaryStream);
        switch (fixture.Game)
        {
            case SupportedGame.Starfield:
            {
                var mod = StarfieldMod.CreateFromBinary(frame, StarfieldRelease.Starfield,
                    new Mutagen.Bethesda.Starfield.GroupMask(true));
                mod.IsMaster.ShouldBeTrue();
                ((Mutagen.Bethesda.Starfield.IStarfieldModGetter)mod).EnumerateMajorRecords().Count().ShouldBe(expectedRecordCount);
                mod.FormLists.Count.ShouldBe(expectedRecordCount);
                mod.ModHeader.MasterReferences.Select(reference => reference.Master).ShouldBe(expectedMasters);
                var inspector = new StarfieldFormListInspector();
                return mod.FormLists.ToDictionary(record => record.FormKey, record => WriteRecord(inspector, record));
            }
            case SupportedGame.Fallout4:
            {
                var mod = Fallout4Mod.CreateFromBinary(frame, Fallout4Release.Fallout4,
                    new Mutagen.Bethesda.Fallout4.GroupMask(true));
                mod.IsMaster.ShouldBeTrue();
                ((Mutagen.Bethesda.Fallout4.IFallout4ModGetter)mod).EnumerateMajorRecords().Count().ShouldBe(expectedRecordCount);
                mod.FormLists.Count.ShouldBe(expectedRecordCount);
                mod.ModHeader.MasterReferences.Select(reference => reference.Master).ShouldBe(expectedMasters);
                var inspector = new Fallout4FormListInspector();
                return mod.FormLists.ToDictionary(record => record.FormKey, record => WriteRecord(inspector, record));
            }
            case SupportedGame.Skyrim:
            {
                var mod = SkyrimMod.CreateFromBinary(frame, SkyrimRelease.SkyrimSE,
                    new Mutagen.Bethesda.Skyrim.GroupMask(true));
                mod.IsMaster.ShouldBeTrue();
                ((Mutagen.Bethesda.Skyrim.ISkyrimModGetter)mod).EnumerateMajorRecords().Count().ShouldBe(expectedRecordCount);
                mod.FormLists.Count.ShouldBe(expectedRecordCount);
                mod.ModHeader.MasterReferences.Select(reference => reference.Master).ShouldBe(expectedMasters);
                var inspector = new SkyrimFormListInspector();
                return mod.FormLists.ToDictionary(record => record.FormKey, record => WriteRecord(inspector, record));
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(fixture), fixture.Game, "A plugin desktop output requires a supported game.");
        }
    }

    /// <summary>Serializes a complete directly parsed record to a detached inspection value.</summary>
    /// <param name="inspector">The stateless inspector for the fixture game.</param>
    /// <param name="record">The directly parsed FormList.</param>
    /// <returns>The complete detached JSON record.</returns>
    private static JsonElement WriteRecord(IFormListInspector inspector, IMajorRecordGetter record)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            inspector.WriteReadView(record, writer, TestContext.Current.CancellationToken);
        }

        using var document = JsonDocument.Parse(stream.ToArray());
        return document.RootElement.Clone();
    }
}
