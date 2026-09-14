using System.Text;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordInspection;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Core.Enums;
using CreationsForge.Starfield.PluginAdapter.Edits;
using CreationsForge.Starfield.PluginAdapter.RecordInspection;
using CreationsForge.Starfield.PluginAdapter.Wire;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;
using Noggog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Starfield;

/// <summary>Verifies bounded closed Starfield FormList wire decoding into every existing typed edit.</summary>
public sealed class StarfieldFormListEditWireCodecTests
{
    /// <summary>Verifies the codec advertises the exact game and release used by its plugin constructors.</summary>
    [Fact]
    public void Identity_UsesExactStarfieldRelease()
    {
        var codec = new StarfieldFormListEditWireCodec();

        codec.Game.ShouldBe(SupportedGame.Starfield);
        codec.Release.ShouldBe(GameRelease.Starfield);
    }

    /// <summary>Verifies every scalar and no-payload command constructs the exact existing typed edit without coercion.</summary>
    [Fact]
    public void Decode_ScalarAndEmptyCommands_ReturnExactTypedEdits()
    {
        var codec = new StarfieldFormListEditWireCodec();

        Decode<SetEditorIdEdit>(codec, "form-list.set-editor-id", "{\"editorId\":\" WireExact \"}")
            .EditorId.ShouldBe(" WireExact ");
        Decode<ClearEditorIdEdit>(codec, "form-list.clear-editor-id", "{}");
        Decode<SetVersionControlEdit>(codec, "form-list.set-version-control", "{\"versionControl\":4294967295}")
            .VersionControl.ShouldBe(uint.MaxValue);
        Decode<SetFormVersionEdit>(codec, "form-list.set-form-version", "{\"formVersion\":65535}")
            .FormVersion.ShouldBe(ushort.MaxValue);
        Decode<SetVersion2Edit>(codec, "form-list.set-version-2", "{\"version2\":65535}")
            .Version2.ShouldBe(ushort.MaxValue);
        Decode<SetCompressedEdit>(codec, "form-list.set-compressed", "{\"isCompressed\":true}")
            .IsCompressed.ShouldBeTrue();
        Decode<SetDeletedEdit>(codec, "form-list.set-deleted", "{\"isDeleted\":false}")
            .IsDeleted.ShouldBeFalse();
        Decode<ClearItemsEdit>(codec, "form-list.clear-items", "{}");
        Decode<StarfieldClearNameEdit>(codec, "starfield.form-list.clear-name", "{}");
        Decode<StarfieldClearAddToListEdit>(codec, "starfield.form-list.clear-add-to-list", "{}");
        Decode<StarfieldClearConditionalEntriesEdit>(
            codec,
            "starfield.form-list.clear-conditional-entries",
            "{}");

        var supportedMask = StarfieldFormListEditWireCodec.SupportedMajorRecordFlagMask;
        Decode<StarfieldSetMajorFlagsEdit>(
            codec,
            "starfield.form-list.set-major-flags",
            $"{{\"majorRecordFlags\":{supportedMask}}}")
            .Flags.ShouldBe((StarfieldMajorRecord.StarfieldMajorRecordFlag)supportedMask);
    }

    /// <summary>Verifies item and AddToList commands preserve order, duplicates, explicit null links, and non-null link requirements.</summary>
    [Fact]
    public void Decode_LinkCommands_PreserveExactInspectorLinkState()
    {
        var codec = new StarfieldFormListEditWireCodec();
        var modKey = ModKey.FromNameAndExtension("WireItems.esm");
        var first = new FormKey(modKey, 0x123);
        var second = new FormKey(modKey, 0x456);
        var replacementJson = "{\"items\":["
            + LinkJson(first)
            + ","
            + LinkJson(FormKey.Null)
            + ","
            + LinkJson(first)
            + ","
            + LinkJson(second)
            + "]}";

        Decode<ReplaceItemsEdit>(codec, "form-list.replace-items", replacementJson)
            .Items.ShouldBe([first, FormKey.Null, first, second], ignoreOrder: false);
        Decode<ReplaceItemsEdit>(codec, "form-list.replace-items", "{\"items\":[]}")
            .Items.ShouldBeEmpty();

        var inserted = Decode<InsertItemEdit>(
            codec,
            "form-list.insert-item",
            "{\"index\":2,\"item\":{\"isNull\":true,\"formKey\":null}}");
        inserted.Index.ShouldBe(2);
        inserted.Item.ShouldBe(FormKey.Null);
        Decode<RemoveItemEdit>(codec, "form-list.remove-item", "{\"index\":3}")
            .Index.ShouldBe(3);
        var moved = Decode<MoveItemEdit>(
            codec,
            "form-list.move-item",
            "{\"sourceIndex\":4,\"destinationIndex\":1}");
        moved.SourceIndex.ShouldBe(4);
        moved.DestinationIndex.ShouldBe(1);

        Decode<StarfieldSetAddToListEdit>(
            codec,
            "starfield.form-list.set-add-to-list",
            "{\"formList\":" + LinkJson(second) + "}")
            .FormList.ShouldBe(second);
        DecodeFailure(
            codec,
            "starfield.form-list.set-add-to-list",
            "{\"formList\":{\"isNull\":false,\"formKey\":"
                + JsonSerializer.Serialize(FormKey.Null.ToString())
                + "}}")
            .Message.ShouldContain("cannot contain FormKey.Null");
    }

    /// <summary>Verifies a real inspector-emitted plugin null link uses and decodes the canonical FormKey.Null string.</summary>
    [Fact]
    public void Decode_InspectorNullLink_RoundTripsCanonicalFormKeyNullString()
    {
        var formList = new FormList(new FormKey("WireNull.esm", 0x800), StarfieldRelease.Starfield);
        formList.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(FormKey.Null));
        var emittedLink = WriteReadView(formList).GetProperty(nameof(IFormListGetter.Items))[0];

        emittedLink.GetProperty("isNull").GetBoolean().ShouldBeTrue();
        emittedLink.GetProperty("formKey").GetString().ShouldBe(FormKey.Null.ToString());
        Decode<ReplaceItemsEdit>(
            new StarfieldFormListEditWireCodec(),
            "form-list.replace-items",
            $"{{\"items\":[{emittedLink.GetRawText()}]}}")
            .Items.ShouldBe([FormKey.Null], ignoreOrder: false);
    }

    /// <summary>Verifies translated-name decoding retains complete, sparse, and explicitly empty language maps.</summary>
    [Fact]
    public void Decode_SetName_PreservesCompleteSparseAndEmptyTranslatedStrings()
    {
        var codec = new StarfieldFormListEditWireCodec();
        var complete = new TranslatedString(
            Language.English,
            new KeyValuePair<Language, string>(Language.French, "Nom francais"),
            new KeyValuePair<Language, string>(Language.English, "English name"));
        var sparse = new TranslatedString(
            Language.English,
            new KeyValuePair<Language, string>(Language.French, "Seulement francais"));
        var empty = new TranslatedString(Language.English, Array.Empty<KeyValuePair<Language, string>>());

        AssertTranslatedStringEqual(
            complete,
            Decode<StarfieldSetNameEdit>(codec, "starfield.form-list.set-name", NameArgumentsJson(complete)).Name);
        AssertTranslatedStringEqual(
            sparse,
            Decode<StarfieldSetNameEdit>(codec, "starfield.form-list.set-name", NameArgumentsJson(sparse)).Name);
        AssertTranslatedStringEqual(
            empty,
            Decode<StarfieldSetNameEdit>(codec, "starfield.form-list.set-name", NameArgumentsJson(empty)).Name);
    }

    /// <summary>Verifies all component commands round-trip complete inspector-emitted polymorphic graphs through generated readers.</summary>
    [Fact]
    public void Decode_ComponentCommands_RoundTripCompleteInspectorGraphs()
    {
        var source = CreateNestedFormList();
        var readView = WriteReadView(source);
        var componentJson = readView.GetProperty(nameof(IFormListGetter.Components))[0].GetRawText();
        var codec = new StarfieldFormListEditWireCodec();

        var added = Decode<StarfieldAddComponentEdit>(
            codec,
            "starfield.form-list.add-component",
            $"{{\"index\":0,\"component\":{componentJson}}}");
        added.Index.ShouldBe(0);
        WriteComponent(added.Component).ShouldBe(componentJson);

        var replaced = Decode<StarfieldReplaceComponentEdit>(
            codec,
            "starfield.form-list.replace-component",
            $"{{\"index\":2147483647,\"component\":{componentJson}}}");
        replaced.Index.ShouldBe(int.MaxValue);
        WriteComponent(replaced.Component).ShouldBe(componentJson);

        Decode<StarfieldRemoveComponentEdit>(codec, "starfield.form-list.remove-component", "{\"index\":2}")
            .Index.ShouldBe(2);
        var all = Decode<StarfieldReplaceComponentsEdit>(
            codec,
            "starfield.form-list.replace-components",
            $"{{\"components\":[{componentJson},{componentJson}]}}");
        all.Components.Count.ShouldBe(2);
        WriteComponent(all.Components[0]).ShouldBe(componentJson);
        WriteComponent(all.Components[1]).ShouldBe(componentJson);
        Decode<StarfieldReplaceComponentsEdit>(codec, "starfield.form-list.replace-components", "{\"components\":[]}")
            .Components.ShouldBeEmpty();
    }

    /// <summary>Verifies conditional-entry decoding preserves entry order, nullable indices, null versus empty conditions, and complete condition graphs.</summary>
    [Fact]
    public void Decode_ConditionalEntries_PreserveNullEmptyAndCompleteConditionGraphs()
    {
        var source = CreateNestedFormList();
        var conditionJson = WriteReadView(source)
            .GetProperty(nameof(IFormListGetter.ConditionalEntries))[0]
            .GetProperty(nameof(IFormListConditionalEntryGetter.Conditions))[0]
            .GetRawText();
        var arguments = "{\"conditionalEntries\":["
            + "{\"Index\":null,\"Conditions\":null},"
            + "{\"Index\":0,\"Conditions\":[]},"
            + $"{{\"Index\":4294967295,\"Conditions\":[{conditionJson}]}}"
            + "]}";

        var edit = Decode<StarfieldSetConditionalEntriesEdit>(
            new StarfieldFormListEditWireCodec(),
            "starfield.form-list.set-conditional-entries",
            arguments);

        edit.Entries.Count.ShouldBe(3);
        edit.Entries[0].Index.ShouldBeNull();
        edit.Entries[0].Conditions.ShouldBeNull();
        edit.Entries[1].Index.ShouldBe(0U);
        edit.Entries[1].Conditions.ShouldNotBeNull().ShouldBeEmpty();
        edit.Entries[2].Index.ShouldBe(uint.MaxValue);
        var condition = edit.Entries[2].Conditions.ShouldNotBeNull().ShouldHaveSingleItem();
        WriteCondition(condition).ShouldBe(conditionJson);
    }

    /// <summary>Verifies closed roots, exact discriminators, link contradictions, numeric ranges, and generated union boundaries fail before edit publication.</summary>
    [Theory]
    [InlineData("unknown.command", "{}", "does not support")]
    [InlineData("form-list.set-editor-id", "{}", "Required property 'editorId'")]
    [InlineData("form-list.set-editor-id", "{\"editorId\":\"A\",\"extra\":true}", "is not accepted")]
    [InlineData("form-list.remove-item", "{\"index\":-1}", "cannot be negative")]
    [InlineData("form-list.set-form-version", "{\"formVersion\":65536}", "unsigned 16-bit")]
    [InlineData("starfield.form-list.set-add-to-list", "{\"formList\":{\"isNull\":true,\"formKey\":null}}", "requires a non-null")]
    [InlineData("starfield.form-list.set-add-to-list", "{\"formList\":{\"isNull\":true,\"formKey\":\"000123:Master.esm\"}}", "requires JSON null or the canonical FormKey.Null")]
    [InlineData("starfield.form-list.set-add-to-list", "{\"formList\":{\"isNull\":false,\"formKey\":null}}", "requires a canonical FormKey")]
    [InlineData("starfield.form-list.add-component", "{\"index\":0,\"component\":{\"$type\":\"unknown\"}}", "is not a generated")]
    [InlineData("starfield.form-list.set-conditional-entries", "{\"conditionalEntries\":[{\"Index\":0}]}", "Required property 'Conditions'")]
    public void Decode_RejectsInvalidClosedCommandPayloads(
        string commandName,
        string argumentsJson,
        string expectedMessage)
    {
        var error = DecodeFailure(new StarfieldFormListEditWireCodec(), commandName, argumentsJson);

        error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        error.Message.ShouldStartWith("$");
        error.Message.ShouldContain(expectedMessage);
    }

    /// <summary>Verifies installed typed flags reject the first raw bit absent from the complete plugin enum mask.</summary>
    [Fact]
    public void Decode_SetMajorFlags_RejectsBitsOutsideInstalledTypedMask()
    {
        var supportedMask = unchecked((uint)StarfieldFormListEditWireCodec.SupportedMajorRecordFlagMask);
        var unsupportedBit = Enumerable.Range(0, 32)
            .Select(static bit => 1U << bit)
            .First(bit => (supportedMask & bit) == 0);

        var error = DecodeFailure(
            new StarfieldFormListEditWireCodec(),
            "starfield.form-list.set-major-flags",
            $"{{\"majorRecordFlags\":{unchecked((int)unsupportedBit)}}}");

        error.Message.ShouldContain($"unsupported bits 0x{unsupportedBit:X8}");
    }

    /// <summary>Verifies caller-selected array and cancellation limits stop traversal before publishing a typed edit.</summary>
    [Fact]
    public void Decode_EnforcesResourceLimitsAndCancellation()
    {
        var codec = new StarfieldFormListEditWireCodec();
        var shortArrays = new RecordWireReadLimits(64, 100, 1, 100, 32);
        DecodeFailure(
            codec,
            "starfield.form-list.replace-components",
            "{\"components\":[{},{ }]}",
            shortArrays).Message.ShouldContain("per-array limit of 1");

        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        Should.Throw<OperationCanceledException>(() => codec.Decode(
            "form-list.clear-items",
            Parse("{}"),
            RecordWireReadLimits.Default,
            cancellation.Token));
    }

    /// <summary>Decodes one successful edit and asserts its exact runtime type.</summary>
    /// <typeparam name="TEdit">The required existing typed edit type.</typeparam>
    /// <param name="codec">The Starfield wire codec.</param>
    /// <param name="commandName">The exact command discriminator.</param>
    /// <param name="json">The complete argument JSON.</param>
    /// <returns>The decoded typed edit.</returns>
    private static TEdit Decode<TEdit>(
        StarfieldFormListEditWireCodec codec,
        string commandName,
        string json)
        where TEdit : FormListEdit
    {
        var result = codec.Decode(commandName, Parse(json), RecordWireReadLimits.Default);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Error.ShouldBeNull();
        return result.Value.ShouldBeOfType<TEdit>();
    }

    /// <summary>Decodes one expected invalid request under the default limits.</summary>
    /// <param name="codec">The Starfield wire codec.</param>
    /// <param name="commandName">The exact command discriminator.</param>
    /// <param name="json">The complete invalid argument JSON.</param>
    /// <returns>The path-specific invalid-request error.</returns>
    private static EngineError DecodeFailure(
        StarfieldFormListEditWireCodec codec,
        string commandName,
        string json)
    {
        return DecodeFailure(codec, commandName, json, RecordWireReadLimits.Default);
    }

    /// <summary>Decodes one expected invalid request under caller-selected resource limits.</summary>
    /// <param name="codec">The Starfield wire codec.</param>
    /// <param name="commandName">The exact command discriminator.</param>
    /// <param name="json">The complete invalid argument JSON.</param>
    /// <param name="limits">The exact resource limits to exercise.</param>
    /// <returns>The path-specific invalid-request error.</returns>
    private static EngineError DecodeFailure(
        StarfieldFormListEditWireCodec codec,
        string commandName,
        string json,
        RecordWireReadLimits limits)
    {
        var result = codec.Decode(commandName, Parse(json), limits);
        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
        return result.Error.ShouldNotBeNull();
    }

    /// <summary>Creates the exact inspector-compatible JSON for one record link.</summary>
    /// <param name="formKey">The non-null key or explicit <see cref="FormKey.Null"/>.</param>
    /// <returns>The closed plugin form-link JSON object.</returns>
    private static string LinkJson(FormKey formKey)
    {
        return formKey.IsNull
            ? $"{{\"isNull\":true,\"formKey\":{JsonSerializer.Serialize(FormKey.Null.ToString())}}}"
            : $"{{\"isNull\":false,\"formKey\":{JsonSerializer.Serialize(formKey.ToString())}}}";
    }

    /// <summary>Writes one translated string through the production plugin leaf writer and wraps it as set-name arguments.</summary>
    /// <param name="name">The exact record translated string.</param>
    /// <returns>Closed set-name arguments using the inspector read shape.</returns>
    private static string NameArgumentsJson(ITranslatedStringGetter name)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WritePropertyName("name");
            RecordJsonLeafWriter.WriteTranslatedString(writer, name, CancellationToken.None);
            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>Creates representative generated component and condition graphs with exact signed-zero and link-or-index state.</summary>
    /// <returns>A mutable Starfield FormList containing one component and one conditional entry.</returns>
    private static FormList CreateNestedFormList()
    {
        var formList = new FormList(new FormKey("WireGraphs.esm", 0x800), StarfieldRelease.Starfield);
        formList.Components.Add(new PropertySheetComponent
        {
            Properties = new ExtendedList<ObjectProperty>
            {
                new ObjectProperty
                {
                    ActorValue = new FormLink<IActorValueInformationGetter>(
                        new FormKey("WireGraphs.esm", 0x123)),
                    Value = -0.0f,
                },
            },
        });
        var data = new BiomeHasKeywordConditionData();
        data.FirstParameter.Index = 17;
        formList.ConditionalEntries.Add(new FormListConditionalEntry
        {
            Index = null,
            Conditions = new ExtendedList<Condition>
            {
                new ConditionFloat
                {
                    ComparisonValue = 1.0f,
                    Data = data,
                },
            },
        });
        return formList;
    }

    /// <summary>Writes one complete Starfield FormList through the production typed inspector.</summary>
    /// <param name="formList">The complete detached record.</param>
    /// <returns>A detached typed JSON read view.</returns>
    private static JsonElement WriteReadView(FormList formList)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            new StarfieldFormListInspector().WriteReadView(formList, writer, CancellationToken.None);
        }

        return Parse(Encoding.UTF8.GetString(stream.ToArray()));
    }

    /// <summary>Writes one decoded component through the generated inspector entry point.</summary>
    /// <param name="component">The decoded complete component.</param>
    /// <returns>The exact detached component JSON.</returns>
    private static string WriteComponent(IAComponentGetter component)
    {
        return WriteNested(writer => StarfieldNestedFieldCodec.WriteComponent(
            writer,
            component,
            CancellationToken.None));
    }

    /// <summary>Writes one decoded condition through the generated inspector entry point.</summary>
    /// <param name="condition">The decoded complete condition.</param>
    /// <returns>The exact detached condition JSON.</returns>
    private static string WriteCondition(IConditionGetter condition)
    {
        return WriteNested(writer => StarfieldNestedFieldCodec.WriteCondition(
            writer,
            condition,
            CancellationToken.None));
    }

    /// <summary>Writes exactly one nested record JSON value into an owned UTF-8 buffer.</summary>
    /// <param name="write">The generated writer invocation.</param>
    /// <returns>The exact JSON text.</returns>
    private static string WriteNested(Action<Utf8JsonWriter> write)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            write(writer);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>Asserts complete target-language and translation-map equality.</summary>
    /// <param name="expected">The expected record translated string.</param>
    /// <param name="actual">The decoded record translated string.</param>
    private static void AssertTranslatedStringEqual(
        ITranslatedStringGetter expected,
        ITranslatedStringGetter actual)
    {
        actual.TargetLanguage.ShouldBe(expected.TargetLanguage);
        actual.String.ShouldBe(expected.String);
        actual.OrderBy(entry => (int)entry.Key).ShouldBe(
            expected.OrderBy(entry => (int)entry.Key),
            ignoreOrder: false);
    }

    /// <summary>Parses one JSON value and returns a detached element after disposing the document.</summary>
    /// <param name="json">The complete JSON value.</param>
    /// <returns>A detached request-local element.</returns>
    private static JsonElement Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}
