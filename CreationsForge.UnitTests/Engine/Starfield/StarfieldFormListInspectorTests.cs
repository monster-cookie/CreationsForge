using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Starfield.PluginAdapter.RecordInspection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;
using Noggog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Starfield;

/// <summary>
/// Verifies the complete Starfield FormList root view and semantic comparison contract.
/// </summary>
public sealed class StarfieldFormListInspectorTests
{
    /// <summary>Verifies every generated root field is written once in field-index order without losing translations or item order.</summary>
    [Fact]
    public void WriteReadView_WritesEveryRootFieldInFieldIndexOrder()
    {
        var formKey = new FormKey((ModKey)"Inspector.esm", 0x00000800);
        var firstItem = new FormKey((ModKey)"Inspector.esm", 0x00000801);
        var addToList = new FormKey((ModKey)"Inspector.esm", 0x00000802);
        var formList = new FormList(formKey, StarfieldRelease.Starfield)
        {
            MajorRecordFlagsRaw = 0x00040000,
            VersionControl = 0x01020304,
            EditorID = "InspectorList",
            FormVersion = 131,
            Version2 = 7,
            StarfieldMajorRecordFlags = (StarfieldMajorRecord.StarfieldMajorRecordFlag)5,
            Name = CreateName("Inspection", "Inspection française"),
        };
        formList.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(firstItem));
        formList.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(firstItem));
        formList.AddToList.SetTo(addToList);
        var inspector = new StarfieldFormListInspector();

        using var document = Write(inspector, formList, TestContext.Current.CancellationToken);
        var root = document.RootElement;

        root.EnumerateObject().Select(property => property.Name).ShouldBe(GetInstalledFormListFieldNames());
        root.GetProperty(nameof(IMajorRecordGetter.MajorRecordFlagsRaw)).GetInt32().ShouldBe(formList.MajorRecordFlagsRaw);
        root.GetProperty(nameof(IMajorRecordGetter.FormKey)).GetString().ShouldBe(formKey.ToString());
        root.GetProperty(nameof(IMajorRecordGetter.VersionControl)).GetUInt32().ShouldBe(formList.VersionControl);
        root.GetProperty(nameof(IMajorRecordGetter.EditorID)).GetString().ShouldBe(formList.EditorID);
        root.GetProperty(nameof(IStarfieldMajorRecordGetter.FormVersion)).GetUInt16().ShouldBe(formList.FormVersion);
        root.GetProperty(nameof(IStarfieldMajorRecordGetter.Version2)).GetUInt16().ShouldBe(formList.Version2);
        root.GetProperty(nameof(IStarfieldMajorRecordGetter.StarfieldMajorRecordFlags)).GetInt32()
            .ShouldBe((int)formList.StarfieldMajorRecordFlags);
        root.GetProperty(nameof(IFormListGetter.Components)).GetArrayLength().ShouldBe(0);
        root.GetProperty(nameof(IFormListGetter.ConditionalEntries)).GetArrayLength().ShouldBe(0);

        var name = root.GetProperty(nameof(IFormListGetter.Name));
        name.GetProperty("targetLanguage").GetString().ShouldBe(Language.English.ToString());
        name.GetProperty("value").GetString().ShouldBe("Inspection");
        name.GetProperty("translations").EnumerateArray()
            .Select(translation => $"{translation.GetProperty("language").GetString()}={translation.GetProperty("value").GetString()}")
            .ShouldBe(new[] { "English=Inspection", "French=Inspection française" });

        var items = root.GetProperty(nameof(IFormListGetter.Items)).EnumerateArray().ToArray();
        items.Length.ShouldBe(2);
        items[0].GetProperty("formKey").GetString().ShouldBe(firstItem.ToString());
        items[1].GetProperty("formKey").GetString().ShouldBe(firstItem.ToString());
        root.GetProperty(nameof(IFormListGetter.AddToList)).GetProperty("formKey").GetString()
            .ShouldBe(addToList.ToString());
    }

    /// <summary>Verifies root scalar, translated, ordered duplicate, and nullable-link changes retain field order and positions.</summary>
    [Fact]
    public void Compare_ReportsRootChangesInFieldIndexOrder()
    {
        var beforeKey = new FormKey((ModKey)"Before.esm", 0x00000800);
        var afterKey = new FormKey((ModKey)"After.esm", 0x00000800);
        var itemA = new FormKey((ModKey)"Before.esm", 0x00000801);
        var itemB = new FormKey((ModKey)"Before.esm", 0x00000802);
        var before = new FormList(beforeKey, StarfieldRelease.Starfield)
        {
            MajorRecordFlagsRaw = 1,
            VersionControl = 2,
            EditorID = string.Empty,
            FormVersion = 3,
            Version2 = 4,
            StarfieldMajorRecordFlags = (StarfieldMajorRecord.StarfieldMajorRecordFlag)1,
            Name = CreateName("Before", "Avant"),
        };
        before.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(itemA));
        before.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(itemA));

        var after = new FormList(afterKey, StarfieldRelease.Starfield)
        {
            MajorRecordFlagsRaw = 8,
            VersionControl = 9,
            EditorID = null,
            FormVersion = 10,
            Version2 = 11,
            StarfieldMajorRecordFlags = (StarfieldMajorRecord.StarfieldMajorRecordFlag)2,
            Name = CreateName("After", "Après"),
        };
        after.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(itemA));
        after.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(itemB));
        after.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(itemB));
        after.AddToList.SetTo(itemA);
        var inspector = new StarfieldFormListInspector();

        var changes = inspector.Compare(before, after, TestContext.Current.CancellationToken);

        changes.Select(change => change.FieldIdentifier).ShouldBe(new[]
        {
            nameof(IMajorRecordGetter.MajorRecordFlagsRaw),
            nameof(IMajorRecordGetter.FormKey),
            nameof(IMajorRecordGetter.VersionControl),
            nameof(IMajorRecordGetter.EditorID),
            nameof(IStarfieldMajorRecordGetter.FormVersion),
            nameof(IStarfieldMajorRecordGetter.Version2),
            nameof(IStarfieldMajorRecordGetter.StarfieldMajorRecordFlags),
            nameof(IFormListGetter.Name),
            nameof(IFormListGetter.Items),
            nameof(IFormListGetter.Items),
            nameof(IFormListGetter.AddToList),
        });
        changes[8].Kind.ShouldBe(SemanticChangeKind.ItemChanged);
        changes[8].BeforePosition.ShouldBe(1);
        changes[8].AfterPosition.ShouldBe(1);
        changes[9].Kind.ShouldBe(SemanticChangeKind.ItemInserted);
        changes[9].BeforePosition.ShouldBeNull();
        changes[9].AfterPosition.ShouldBe(2);
    }

    /// <summary>Verifies root traversal exposes inactive link-or-index state while semantic comparison ignores that inactive projection.</summary>
    [Fact]
    public void WriteReadViewAndCompare_ExposeNestedBitAndInactiveUnionState()
    {
        var formKey = new FormKey((ModKey)"Inspector.esm", 0x00000800);
        var actorValue = new FormKey((ModKey)"Inspector.esm", 0x00000801);
        var before = CreateNestedFormList(formKey, actorValue, -0.0f, 17);
        var after = CreateNestedFormList(formKey, actorValue, 0.0f, 18);
        var inspector = new StarfieldFormListInspector();

        using var beforeDocument = Write(inspector, before, TestContext.Current.CancellationToken);
        using var afterDocument = Write(inspector, after, TestContext.Current.CancellationToken);
        var component = beforeDocument.RootElement
            .GetProperty(nameof(IFormListGetter.Components))[0];
        component.GetProperty("$type").GetString().ShouldNotBeNull()
            .ShouldEndWith(":Mutagen.Bethesda.Starfield.PropertySheetComponent");
        component.GetProperty("Properties")[0]
            .GetProperty("Value")
            .GetProperty("bits")
            .GetString()
            .ShouldBe("0x80000000");

        var conditionalEntry = beforeDocument.RootElement
            .GetProperty(nameof(IFormListGetter.ConditionalEntries))[0];
        conditionalEntry.GetProperty(nameof(IFormListConditionalEntryGetter.Index)).ValueKind
            .ShouldBe(JsonValueKind.Null);
        var condition = conditionalEntry
            .GetProperty(nameof(IFormListConditionalEntryGetter.Conditions))[0];
        condition.GetProperty("$type").GetString().ShouldNotBeNull()
            .ShouldEndWith(":Mutagen.Bethesda.Starfield.ConditionFloat");
        var firstParameter = condition.GetProperty("Data").GetProperty("FirstParameter");
        firstParameter.GetProperty("usesAlias").GetBoolean().ShouldBeFalse();
        firstParameter.GetProperty("usesPackageData").GetBoolean().ShouldBeFalse();
        firstParameter.GetProperty("index").GetUInt32().ShouldBe(17u);
        var afterFirstParameter = afterDocument.RootElement
            .GetProperty(nameof(IFormListGetter.ConditionalEntries))[0]
            .GetProperty(nameof(IFormListConditionalEntryGetter.Conditions))[0]
            .GetProperty("Data")
            .GetProperty("FirstParameter");
        afterFirstParameter.GetProperty("index").GetUInt32().ShouldBe(18u);
        firstParameter.GetRawText().ShouldNotBe(afterFirstParameter.GetRawText());

        var changes = inspector.Compare(before, after, TestContext.Current.CancellationToken);
        changes.Select(change => change.FieldIdentifier).ShouldBe(new[]
        {
            "Components[0].Properties[0].Value",
        });
    }

    /// <summary>Verifies nullable condition collections remain distinct from present empty collections.</summary>
    [Fact]
    public void WriteReadViewAndCompare_PreserveNullAndEmptyConditionCollections()
    {
        var formKey = new FormKey((ModKey)"Inspector.esm", 0x00000800);
        var before = new FormList(formKey, StarfieldRelease.Starfield);
        before.ConditionalEntries.Add(new FormListConditionalEntry
        {
            Index = 1,
            Conditions = null,
        });
        var after = new FormList(formKey, StarfieldRelease.Starfield);
        after.ConditionalEntries.Add(new FormListConditionalEntry
        {
            Index = 1,
            Conditions = new ExtendedList<Condition>(),
        });
        var inspector = new StarfieldFormListInspector();

        using var beforeDocument = Write(inspector, before, TestContext.Current.CancellationToken);
        using var afterDocument = Write(inspector, after, TestContext.Current.CancellationToken);
        beforeDocument.RootElement
            .GetProperty(nameof(IFormListGetter.ConditionalEntries))[0]
            .GetProperty(nameof(IFormListConditionalEntryGetter.Conditions))
            .ValueKind
            .ShouldBe(JsonValueKind.Null);
        afterDocument.RootElement
            .GetProperty(nameof(IFormListGetter.ConditionalEntries))[0]
            .GetProperty(nameof(IFormListConditionalEntryGetter.Conditions))
            .GetArrayLength()
            .ShouldBe(0);

        var change = inspector.Compare(before, after, TestContext.Current.CancellationToken).ShouldHaveSingleItem();
        change.FieldIdentifier.ShouldBe("ConditionalEntries[0].Conditions");
        change.Kind.ShouldBe(SemanticChangeKind.ValueChanged);
        change.BeforePosition.ShouldBeNull();
        change.AfterPosition.ShouldBeNull();
    }

    /// <summary>Verifies record presence has one canonical change and invalid types and cancellation are rejected.</summary>
    [Fact]
    public void Compare_HandlesRecordPresenceTypeAndCancellationBoundaries()
    {
        var formList = new FormList(
            new FormKey((ModKey)"Inspector.esm", 0x00000800),
            StarfieldRelease.Starfield);
        var inspector = new StarfieldFormListInspector();

        inspector.Compare(null, null, TestContext.Current.CancellationToken).ShouldBeEmpty();
        var inserted = inspector.Compare(null, formList, TestContext.Current.CancellationToken).ShouldHaveSingleItem();
        inserted.FieldIdentifier.ShouldBe("$record");
        inserted.Kind.ShouldBe(SemanticChangeKind.ItemInserted);
        var removed = inspector.Compare(formList, null, TestContext.Current.CancellationToken).ShouldHaveSingleItem();
        removed.FieldIdentifier.ShouldBe("$record");
        removed.Kind.ShouldBe(SemanticChangeKind.ItemRemoved);

        var book = new Book(
            new FormKey((ModKey)"Inspector.esm", 0x00000801),
            StarfieldRelease.Starfield);
        Should.Throw<ArgumentException>(() => inspector.Compare(book, formList, TestContext.Current.CancellationToken));

        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();
        Should.Throw<OperationCanceledException>(() => inspector.Compare(formList, formList, cancellationSource.Token));
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);
        Should.Throw<OperationCanceledException>(() => inspector.WriteReadView(formList, writer, cancellationSource.Token));
    }

    /// <summary>Writes one detached JSON document through the public inspector contract.</summary>
    /// <param name="inspector">The Starfield inspector under test.</param>
    /// <param name="formList">The FormList getter to write.</param>
    /// <param name="cancellationToken">A token observed by the record field traversal.</param>
    /// <returns>An owned parsed document containing the complete view.</returns>
    private static JsonDocument Write(
        StarfieldFormListInspector inspector,
        IFormListGetter formList,
        CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            inspector.WriteReadView(formList, writer, cancellationToken);
        }

        return JsonDocument.Parse(stream.ToArray());
    }

    /// <summary>Creates one deterministic two-language record string for root view and comparison tests.</summary>
    /// <param name="english">The selected English value.</param>
    /// <param name="french">The preserved French translation.</param>
    /// <returns>The mutable record translated string.</returns>
    private static TranslatedString CreateName(string english, string french)
    {
        var name = new TranslatedString(Language.English, english);
        name.Set(Language.French, french);
        return name;
    }

    /// <summary>Creates a FormList with representative component and condition union state.</summary>
    /// <param name="formKey">The record identity used by both snapshots.</param>
    /// <param name="actorValue">The nested actor-value link identity.</param>
    /// <param name="propertyValue">The exact floating-point property value.</param>
    /// <param name="inactiveIndex">The retained index while link mode remains active.</param>
    /// <returns>A mutable FormList containing the requested nested state.</returns>
    private static FormList CreateNestedFormList(
        FormKey formKey,
        FormKey actorValue,
        float propertyValue,
        uint inactiveIndex)
    {
        var formList = new FormList(formKey, StarfieldRelease.Starfield);
        formList.Components.Add(new PropertySheetComponent
        {
            Properties = new ExtendedList<ObjectProperty>
            {
                new ObjectProperty
                {
                    ActorValue = new FormLink<IActorValueInformationGetter>(actorValue),
                    Value = propertyValue,
                },
            },
        });

        var conditionData = new BiomeHasKeywordConditionData();
        conditionData.FirstParameter.Index = inactiveIndex;
        formList.ConditionalEntries.Add(new FormListConditionalEntry
        {
            Index = null,
            Conditions = new ExtendedList<Condition>
            {
                new ConditionFloat
                {
                    ComparisonValue = 1.0f,
                    Data = conditionData,
                },
            },
        });
        return formList;
    }

    /// <summary>Reads the installed internal Starfield FormList field-index names for completeness coverage.</summary>
    /// <returns>Every installed root field name in record field-index order.</returns>
    private static string[] GetInstalledFormListFieldNames()
    {
        var fieldIndexType = typeof(FormList).Assembly.GetType(
            "Mutagen.Bethesda.Starfield.FormList_FieldIndex",
            throwOnError: true)!;
        return Enum.GetNames(fieldIndexType);
    }
}
