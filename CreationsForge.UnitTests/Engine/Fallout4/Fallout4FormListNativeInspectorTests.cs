using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Fallout4.Native.NativeInspection;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Fallout4;

/// <summary>Verifies complete typed Fallout 4 FormList response writing and semantic comparison.</summary>
public sealed class Fallout4FormListNativeInspectorTests
{
    /// <summary>Verifies the response view writes every installed native field in exact generated index order.</summary>
    [Fact]
    public void WriteReadView_WritesEveryFieldIndexAndPreservesTypedLeaves()
    {
        var formList = CreateCompleteFormList();
        var inspector = new Fallout4FormListNativeInspector();
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            inspector.WriteReadView(formList, writer, TestContext.Current.CancellationToken);
        }

        using var document = JsonDocument.Parse(stream.ToArray());
        var root = document.RootElement;

        root.EnumerateObject().Select(property => property.Name)
            .ShouldBe(GetInstalledFieldIdentifiers());
        root.GetProperty("FormKey").GetString().ShouldBe(formList.FormKey.ToString());
        root.GetProperty("MajorRecordFlagsRaw").GetInt32().ShouldBe(formList.MajorRecordFlagsRaw);
        root.GetProperty("VersionControl").GetUInt32().ShouldBe(formList.VersionControl);
        root.GetProperty("EditorID").GetString().ShouldBe(formList.EditorID);
        root.GetProperty("FormVersion").GetUInt16().ShouldBe(formList.FormVersion);
        root.GetProperty("Version2").GetUInt16().ShouldBe(formList.Version2);
        root.GetProperty("Fallout4MajorRecordFlags").GetInt32()
            .ShouldBe((int)formList.Fallout4MajorRecordFlags);

        var name = root.GetProperty("Name");
        name.GetProperty("targetLanguage").GetString().ShouldBe(Language.English.ToString());
        name.GetProperty("value").GetString().ShouldBe("English name");
        name.GetProperty("translations").EnumerateArray()
            .Select(translation => translation.GetProperty("value").GetString())
            .ShouldBe(["English name", "Nom français"]);

        var itemKeys = root.GetProperty("Items")
            .EnumerateArray()
            .Select(item => item.GetProperty("formKey").GetString());
        itemKeys.ShouldBe(formList.Items.Select(item => item.FormKey.ToString()));
    }

    /// <summary>Verifies present records compare every indexed field with positional ordered-item changes.</summary>
    [Fact]
    public void Compare_WhenAllFieldsDiffer_ReportsExactFieldOrderAndItemPositions()
    {
        var before = CreateCompleteFormList();
        var after = CreateCompleteFormList(new FormKey((ModKey)"Other.esm", before.FormKey.ID));
        after.MajorRecordFlagsRaw ^= 0x40;
        after.VersionControl++;
        after.EditorID = null;
        after.FormVersion++;
        after.Version2++;
        after.Fallout4MajorRecordFlags ^= Fallout4MajorRecord.Fallout4MajorRecordFlag.NotPlayable;
        after.Name = CreateName("Changed English", "Nom changé");
        after.Items[0] = new FormLink<IFallout4MajorRecordGetter>(new FormKey(before.FormKey.ModKey, 0x900));
        after.Items.Add(new FormLink<IFallout4MajorRecordGetter>(new FormKey(before.FormKey.ModKey, 0x901)));

        var changes = new Fallout4FormListNativeInspector().Compare(
            before,
            after,
            TestContext.Current.CancellationToken);

        changes.Select(change => change.FieldIdentifier).ShouldBe(
        [
            "MajorRecordFlagsRaw",
            "FormKey",
            "VersionControl",
            "EditorID",
            "FormVersion",
            "Version2",
            "Fallout4MajorRecordFlags",
            "Name",
            "Items",
            "Items",
        ]);
        changes.Take(8).ShouldAllBe(change => change.Kind == SemanticChangeKind.ValueChanged);
        changes[8].Kind.ShouldBe(SemanticChangeKind.ItemChanged);
        changes[8].BeforePosition.ShouldBe(0);
        changes[8].AfterPosition.ShouldBe(0);
        changes[9].Kind.ShouldBe(SemanticChangeKind.ItemInserted);
        changes[9].BeforePosition.ShouldBeNull();
        changes[9].AfterPosition.ShouldBe(2);
    }

    /// <summary>Verifies record creation, removal, and mutual absence use the shared record-level convention.</summary>
    [Fact]
    public void Compare_WhenRecordPresenceChanges_ReportsOnlyRecordLevelChange()
    {
        var formList = CreateCompleteFormList();
        var inspector = new Fallout4FormListNativeInspector();

        inspector.Compare(null, null, TestContext.Current.CancellationToken).ShouldBeEmpty();
        var inserted = inspector.Compare(null, formList, TestContext.Current.CancellationToken).ShouldHaveSingleItem();
        var removed = inspector.Compare(formList, null, TestContext.Current.CancellationToken).ShouldHaveSingleItem();

        inserted.FieldIdentifier.ShouldBe("$record");
        inserted.Kind.ShouldBe(SemanticChangeKind.ItemInserted);
        removed.FieldIdentifier.ShouldBe("$record");
        removed.Kind.ShouldBe(SemanticChangeKind.ItemRemoved);
    }

    /// <summary>Verifies equal records, unsupported families, and cancellation do not yield lossy fallback behavior.</summary>
    [Fact]
    public void Inspector_UsesTypedEqualityAndObservesCancellation()
    {
        var formList = CreateCompleteFormList();
        var inspector = new Fallout4FormListNativeInspector();
        inspector.Compare(formList, formList.DeepCopy(), TestContext.Current.CancellationToken).ShouldBeEmpty();

        var mod = new Fallout4Mod("NativeInspector.esm", Fallout4Release.Fallout4);
        var book = new Book(mod, "NotAFormList");
        Should.Throw<ArgumentException>(() => inspector.Compare(formList, book, CancellationToken.None));

        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        Should.Throw<OperationCanceledException>(() => inspector.Compare(formList, formList, cancellation.Token));
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);
        Should.Throw<OperationCanceledException>(() => inspector.WriteReadView(formList, writer, cancellation.Token));
    }

    /// <summary>Creates a FormList with non-default values for every indexed Fallout 4 field family.</summary>
    /// <param name="formKey">The native identity to assign during construction, or <see langword="null"/> for the standard test identity.</param>
    /// <returns>A detached native FormList containing localized text and ordered duplicate-capable links.</returns>
    private static FormList CreateCompleteFormList(FormKey? formKey = null)
    {
        var modKey = (ModKey)"NativeInspector.esm";
        var formList = new FormList(formKey ?? new FormKey(modKey, 0x800), Fallout4Release.Fallout4)
        {
            MajorRecordFlagsRaw = 0x20,
            VersionControl = 0x12345678,
            EditorID = "CompleteList",
            FormVersion = 44,
            Version2 = 7,
            Fallout4MajorRecordFlags = Fallout4MajorRecord.Fallout4MajorRecordFlag.Deleted,
            Name = CreateName("English name", "Nom français"),
        };
        formList.Items.Add(new FormLink<IFallout4MajorRecordGetter>(new FormKey(modKey, 0x801)));
        formList.Items.Add(new FormLink<IFallout4MajorRecordGetter>(new FormKey(modKey, 0x801)));
        return formList;
    }

    /// <summary>Creates a two-language native translated string for response and comparison coverage.</summary>
    /// <param name="english">The selected English value.</param>
    /// <param name="french">The retained French translation.</param>
    /// <returns>A native translated string retaining both language entries.</returns>
    private static TranslatedString CreateName(string english, string french)
    {
        var name = new TranslatedString(Language.English, english);
        name.Set(Language.French, french);
        return name;
    }

    /// <summary>Reads the non-public generated field-index enum for test-only completeness verification.</summary>
    /// <returns>The installed Fallout 4 FormList field names ordered by their generated numeric indices.</returns>
    private static IReadOnlyList<string> GetInstalledFieldIdentifiers()
    {
        var fieldIndexType = typeof(FormList).Assembly.GetType(
            "Mutagen.Bethesda.Fallout4.FormList_FieldIndex",
            throwOnError: true)!;
        return Array.AsReadOnly(Enum.GetNames(fieldIndexType)
            .OrderBy(name => Convert.ToUInt16(Enum.Parse(fieldIndexType, name)))
            .ToArray());
    }
}
