using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordInspection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.RecordInspection;

/// <summary>Verifies compact native byte leaves and dictionary positions in detached major-record views.</summary>
public sealed class MutagenMajorRecordInspectorTests
{
    /// <summary>Verifies a native memory slice remains one lossless leaf and compares as one changed field.</summary>
    [Fact]
    public void ByteSlice_WritesCompactLeafAndOneSemanticChange()
    {
        var formKey = new FormKey("Inspection.esm", 0x0800);
        var before = new ActorValueInformation(formKey, SkyrimRelease.SkyrimSE)
        {
            CNAM = new MemorySlice<byte>([1, 2, 3]),
        };
        var after = new ActorValueInformation(formKey, SkyrimRelease.SkyrimSE)
        {
            CNAM = new MemorySlice<byte>([1, 4, 3]),
        };
        var inspector = new MutagenMajorRecordInspector(typeof(SkyrimMajorRecord), "Mutagen.Bethesda.Skyrim/0.55.0-alpha.53");

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            inspector.WriteReadView(after, writer, TestContext.Current.CancellationToken);
        }

        using var document = JsonDocument.Parse(stream.ToArray());
        var bytes = document.RootElement.GetProperty("CNAM");
        bytes.ValueKind.ShouldBe(JsonValueKind.Object);
        bytes.GetProperty("length").GetInt32().ShouldBe(3);
        bytes.GetProperty("base64").GetString().ShouldBe(Convert.ToBase64String([1, 4, 3]));

        var changes = inspector.Compare(before, after, TestContext.Current.CancellationToken);
        changes.Select(change => change.FieldIdentifier).ShouldContain("$record.CNAM");
        changes.Count(change => change.FieldIdentifier.StartsWith("$record.CNAM", StringComparison.Ordinal)).ShouldBe(1);
    }

    /// <summary>Verifies dictionary insertion positions index the sorted field-tree array rather than native enumeration order.</summary>
    [Fact]
    public void DictionaryInsertion_ReportsSortedFieldTreePosition()
    {
        var formKey = new FormKey("Inspection.esm", 0x0801);
        var before = new Class(formKey, SkyrimRelease.SkyrimSE);
        var after = new Class(formKey, SkyrimRelease.SkyrimSE);
        var keys = Enum.GetValues<Skill>()
            .OrderBy(key => key.ToString(), StringComparer.Ordinal)
            .ToArray();
        var first = keys[0];
        var last = keys[^1];
        before.SkillWeights[last] = 2;
        after.SkillWeights[last] = 2;
        after.SkillWeights[first] = 1;
        var inspector = new MutagenMajorRecordInspector(typeof(SkyrimMajorRecord), "Mutagen.Bethesda.Skyrim/0.55.0-alpha.53");

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            inspector.WriteReadView(after, writer, TestContext.Current.CancellationToken);
        }

        using var document = JsonDocument.Parse(stream.ToArray());
        var entries = document.RootElement.GetProperty("SkillWeights").EnumerateArray().ToArray();
        entries.Length.ShouldBe(2);
        entries[0].GetProperty("key").GetInt32().ShouldBe((int)first);

        var insertion = inspector.Compare(before, after, TestContext.Current.CancellationToken)
            .Single(change => change.FieldIdentifier == "$record.SkillWeights" && change.Kind == SemanticChangeKind.ItemInserted);
        insertion.BeforePosition.ShouldBeNull();
        insertion.AfterPosition.ShouldBe(0);
    }
}
