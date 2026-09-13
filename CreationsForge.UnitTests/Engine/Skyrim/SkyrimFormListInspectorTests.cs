using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Skyrim.PluginAdapter.RecordInspection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Skyrim;

/// <summary>
/// Verifies complete typed Skyrim FormList JSON projection and field-by-field semantic comparison.
/// </summary>
public sealed class SkyrimFormListInspectorTests
{
    /// <summary>
    /// Verifies the response projection covers the installed Mutagen field index and preserves record link order, duplicates, and null identities.
    /// </summary>
    [Fact]
    public void WriteReadView_CoversInstalledFieldIndexAndPreservesPluginValues()
    {
        var formKey = new FormKey("Inspection.esm", 0x0800);
        var firstItem = new FormKey("Master.esm", 0x0123);
        var secondItem = new FormKey("Inspection.esm", 0x0801);
        var formList = new FormList(formKey, SkyrimRelease.SkyrimSE)
        {
            EditorID = "InspectionList",
            FormVersion = 44,
            Version2 = 7,
            VersionControl = 0x01020304,
            MajorRecordFlagsRaw = 0x00080020,
        };
        formList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(firstItem));
        formList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(secondItem));
        formList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(firstItem));
        formList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(FormKey.Null));

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            new SkyrimFormListInspector().WriteReadView(
                formList,
                writer,
                TestContext.Current.CancellationToken);
        }

        using var document = JsonDocument.Parse(stream.ToArray());
        var root = document.RootElement;
        var fieldIndexType = typeof(FormList).Assembly.GetType(
            "Mutagen.Bethesda.Skyrim.FormList_FieldIndex",
            throwOnError: true)!;
        root.EnumerateObject().Select(property => property.Name)
            .ShouldBe(Enum.GetNames(fieldIndexType));
        root.GetProperty(nameof(IMajorRecordGetter.MajorRecordFlagsRaw)).GetInt32()
            .ShouldBe(formList.MajorRecordFlagsRaw);
        root.GetProperty(nameof(IMajorRecordGetter.FormKey)).GetString()
            .ShouldBe(formKey.ToString());
        root.GetProperty(nameof(IMajorRecordGetter.VersionControl)).GetUInt32()
            .ShouldBe(formList.VersionControl);
        root.GetProperty(nameof(IMajorRecordGetter.EditorID)).GetString()
            .ShouldBe(formList.EditorID);
        root.GetProperty(nameof(ISkyrimMajorRecordGetter.FormVersion)).GetUInt16()
            .ShouldBe(formList.FormVersion);
        root.GetProperty(nameof(ISkyrimMajorRecordGetter.Version2)).GetUInt16()
            .ShouldBe(formList.Version2);
        root.GetProperty(nameof(ISkyrimMajorRecordGetter.SkyrimMajorRecordFlags)).GetInt32()
            .ShouldBe((int)formList.SkyrimMajorRecordFlags);
        var items = root.GetProperty(nameof(IFormListGetter.Items)).EnumerateArray().ToArray();
        items.Select(item => item.GetProperty("isNull").GetBoolean()).ShouldBe([false, false, false, true]);
        items.Select(item => item.GetProperty("formKey").ValueKind == JsonValueKind.Null
                ? null
                : item.GetProperty("formKey").GetString())
            .ShouldBe([firstItem.ToString(), secondItem.ToString(), firstItem.ToString(), FormKey.Null.ToString()]);
    }

    /// <summary>
    /// Verifies scalar changes follow installed field order and ordered item changes retain exact positions.
    /// </summary>
    [Fact]
    public void Compare_ReportsEveryChangedFieldAndPreciseItemPositions()
    {
        var sharedItem = new FormKey("Master.esm", 0x0100);
        var beforeChangedItem = new FormKey("Master.esm", 0x0101);
        var afterChangedItem = new FormKey("Master.esm", 0x0102);
        var insertedItem = new FormKey("After.esm", 0x0801);
        var before = CreateFormList(
            new FormKey("Before.esm", 0x0800),
            null,
            1,
            40,
            2,
            0,
            [sharedItem, beforeChangedItem, sharedItem, FormKey.Null]);
        var after = CreateFormList(
            new FormKey("After.esm", 0x0800),
            "AfterList",
            2,
            44,
            7,
            0x00080020,
            [sharedItem, afterChangedItem, sharedItem, FormKey.Null, insertedItem]);

        var changes = new SkyrimFormListInspector().Compare(
            before,
            after,
            TestContext.Current.CancellationToken);

        changes.Select(ChangeIdentity).ShouldBe(
        [
            "MajorRecordFlagsRaw:ValueChanged:-:-",
            "FormKey:ValueChanged:-:-",
            "VersionControl:ValueChanged:-:-",
            "EditorID:ValueChanged:-:-",
            "FormVersion:ValueChanged:-:-",
            "Version2:ValueChanged:-:-",
            "SkyrimMajorRecordFlags:ValueChanged:-:-",
            "Items:ItemChanged:1:1",
            "Items:ItemInserted:-:4",
        ]);

        var removal = new SkyrimFormListInspector().Compare(
            after,
            before,
            TestContext.Current.CancellationToken);
        removal[^1].FieldIdentifier.ShouldBe(nameof(IFormListGetter.Items));
        removal[^1].Kind.ShouldBe(SemanticChangeKind.ItemRemoved);
        removal[^1].BeforePosition.ShouldBe(4);
        removal[^1].AfterPosition.ShouldBeNull();
    }

    /// <summary>
    /// Verifies whole-record insertions use one structural descriptor and cancellation stops traversal before output is produced.
    /// </summary>
    [Fact]
    public void Compare_HandlesAbsentRecordsAndPropagatesCancellation()
    {
        var after = CreateFormList(
            new FormKey("After.esm", 0x0800),
            "AfterList",
            2,
            44,
            7,
            0x00080020,
            [new FormKey("Master.esm", 0x0100), FormKey.Null]);
        var inspector = new SkyrimFormListInspector();

        var changes = inspector.Compare(null, after, TestContext.Current.CancellationToken);

        changes.Count.ShouldBe(1);
        changes[0].FieldIdentifier.ShouldBe("$record");
        changes[0].Kind.ShouldBe(SemanticChangeKind.ItemInserted);
        changes[0].BeforePosition.ShouldBeNull();
        changes[0].AfterPosition.ShouldBeNull();
        inspector.Compare(null, null, TestContext.Current.CancellationToken).ShouldBeEmpty();

        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();
        Should.Throw<OperationCanceledException>(() => inspector.Compare(after, after, cancellationSource.Token));
    }

    /// <summary>
    /// Creates one mutable Skyrim FormList with explicit complete header and ordered item state.
    /// </summary>
    /// <param name="formKey">The record identity.</param>
    /// <param name="editorId">The nullable plugin EditorID.</param>
    /// <param name="versionControl">The plugin version-control value.</param>
    /// <param name="formVersion">The plugin form version.</param>
    /// <param name="version2">The second plugin version field.</param>
    /// <param name="rawFlags">The complete plugin major-record flags.</param>
    /// <param name="items">The ordered plugin FormKeys, including duplicates and null identities.</param>
    /// <returns>A mutable FormList used only by the current test.</returns>
    private static FormList CreateFormList(
        FormKey formKey,
        string? editorId,
        uint versionControl,
        ushort formVersion,
        ushort version2,
        int rawFlags,
        IReadOnlyList<FormKey> items)
    {
        var formList = new FormList(formKey, SkyrimRelease.SkyrimSE)
        {
            EditorID = editorId,
            VersionControl = versionControl,
            FormVersion = formVersion,
            Version2 = version2,
            MajorRecordFlagsRaw = rawFlags,
        };
        foreach (var item in items)
        {
            formList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(item));
        }

        return formList;
    }

    /// <summary>
    /// Formats one semantic change for concise deterministic sequence assertions.
    /// </summary>
    /// <param name="change">The semantic change descriptor.</param>
    /// <returns>The field, kind, and optional prior and resulting positions.</returns>
    private static string ChangeIdentity(SemanticChangeDescriptor change)
    {
        return $"{change.FieldIdentifier}:{change.Kind}:{change.BeforePosition?.ToString() ?? "-"}:{change.AfterPosition?.ToString() ?? "-"}";
    }
}
