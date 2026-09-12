using Mutagen.Bethesda.Plugins;

namespace CreationsForge.UnitTests.Engine.ExternalSamples;

/// <summary>Verifies independent external-edit assertion boundaries without native game input.</summary>
public sealed class ExternalNativeSampleAssertionsTests
{
    /// <summary>Initializes the stateless assertion-helper test owner.</summary>
    public ExternalNativeSampleAssertionsTests()
    { }

    /// <summary>Rejects a no-op ReplaceItems intention even when EditorID would change.</summary>
    [Fact]
    public void RequireMaterialIntention_RejectsNoOpReplaceItemsWithChangedEditorId()
    {
        var modKey = ModKey.FromNameAndExtension("Source.esm");
        var formKey = new FormKey(modKey, 0x800);
        var item = new FormKey(modKey, 0x801);
        var source = new ExternalSelectedRecord
        {
            FormKey = formKey,
            Json = "{\"EditorID\":\"OriginalEditorId\"}",
            Items = [item],
        };
        var intended = new ExternalIntendedRecordEdit(source, "ChangedEditorId", [item], replaceItems: true);

        var error = Assert.Throws<InvalidDataException>(
            () => ExternalNativeSampleAssertions.RequireMaterialIntention(intended));

        Assert.Contains("no-op ReplaceItems", error.Message, StringComparison.Ordinal);
    }

    /// <summary>Rejects unchanged observed Items despite the correct EditorID and accepts the exact rotated result.</summary>
    [Fact]
    public void RequireNativeValues_RequiresObservedRotatedItemsIndependentlyOfEditorId()
    {
        var modKey = ModKey.FromNameAndExtension("Source.esm");
        var formKey = new FormKey(modKey, 0x800);
        var firstItem = new FormKey(modKey, 0x801);
        var secondItem = new FormKey(modKey, 0x802);
        var source = new ExternalSelectedRecord
        {
            FormKey = formKey,
            Json = "{\"EditorID\":\"OriginalEditorId\"}",
            Items = [firstItem, secondItem],
        };
        var intended = new ExternalIntendedRecordEdit(
            source,
            "ChangedEditorId",
            [secondItem, firstItem],
            replaceItems: true);
        ExternalNativeSampleAssertions.RequireMaterialIntention(intended);

        var error = Assert.Throws<InvalidDataException>(() => ExternalNativeSampleAssertions.RequireNativeValues(
            "ChangedEditorId",
            [firstItem, secondItem],
            intended,
            "Regression observation"));

        Assert.Contains("ordered Items", error.Message, StringComparison.Ordinal);
        ExternalNativeSampleAssertions.RequireNativeValues(
            "ChangedEditorId",
            [secondItem, firstItem],
            intended,
            "Regression observation");
    }
}
