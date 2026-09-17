using CreationsForge.Core.Engine.RecordInspection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.UnitTests.Engine.RecordInspection;

/// <summary>Checks complete native output comparison across major-record families.</summary>
public sealed class MajorRecordSetComparerTests
{
    /// <summary>Detects an edited non-FormList field after a plugin copy while accepting an unchanged copy.</summary>
    [Fact]
    public void DetectsChangedBookField()
    {
        var original = new StarfieldMod("RecordComparison.esm", StarfieldRelease.Starfield);
        original.Books.Add(new Book(original, "ComparisonBook"));
        original.FormLists.Add(new FormList(original, "ComparisonList"));
        var reopened = (StarfieldMod)((IModGetter)original).DeepCopy();
        var inspector = new MutagenMajorRecordInspector(
            typeof(StarfieldMajorRecord),
            "Mutagen.Bethesda.Starfield/0.55.0-alpha.53");

        Assert.True(MajorRecordSetComparer.AreEqual(
            original.EnumerateMajorRecords(),
            reopened.EnumerateMajorRecords(),
            inspector,
            CancellationToken.None));

        reopened.Books.Single().EditorID = "ChangedBook";

        Assert.False(MajorRecordSetComparer.AreEqual(
            original.EnumerateMajorRecords(),
            reopened.EnumerateMajorRecords(),
            inspector,
            CancellationToken.None));
    }
}
