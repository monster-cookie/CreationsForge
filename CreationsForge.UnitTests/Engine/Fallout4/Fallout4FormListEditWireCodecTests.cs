using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInspection;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Core.Enums;
using CreationsForge.Fallout4.Native.Edits;
using CreationsForge.Fallout4.Native.NativeInspection;
using CreationsForge.Fallout4.Native.Wire;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Fallout4;

/// <summary>Verifies bounded closed Fallout 4 FormList wire decoding into the existing typed edit domain.</summary>
public sealed class Fallout4FormListEditWireCodecTests
{
    /// <summary>The codec advertises the exact game and release used by its compiled Mutagen constructors.</summary>
    [Fact]
    public void Identity_UsesExactFallout4Release()
    {
        var codec = new Fallout4FormListEditWireCodec();

        codec.Game.ShouldBe(SupportedGame.Fallout4);
        codec.Release.ShouldBe(GameRelease.Fallout4);
    }

    /// <summary>Every scalar and no-payload command produces the exact existing typed edit without coercion.</summary>
    [Fact]
    public void Decode_ScalarAndEmptyCommands_ReturnExactTypedEdits()
    {
        var codec = new Fallout4FormListEditWireCodec();

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
        Decode<Fallout4ClearNameEdit>(codec, "fallout4.form-list.clear-name", "{}");
        Decode<Fallout4SetMajorRecordFlagsEdit>(
            codec,
            "fallout4.form-list.set-major-record-flags",
            "{\"majorRecordFlags\":262148}")
            .MajorRecordFlags.ShouldBe(
                Fallout4MajorRecord.Fallout4MajorRecordFlag.Compressed
                | Fallout4MajorRecord.Fallout4MajorRecordFlag.NotPlayable);
    }

    /// <summary>Ordered item commands retain duplicates and distinguish native null links from non-null FormKeys.</summary>
    [Fact]
    public void Decode_ItemCommands_PreserveOrderDuplicatesEmptyAndNativeNullLinks()
    {
        var codec = new Fallout4FormListEditWireCodec();
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

        var replacement = Decode<ReplaceItemsEdit>(codec, "form-list.replace-items", replacementJson);
        replacement.Items.ShouldBe(
            [first, FormKey.Null, first, second],
            ignoreOrder: false);
        Decode<ReplaceItemsEdit>(codec, "form-list.replace-items", "{\"items\":[]}")
            .Items.ShouldBeEmpty();

        var inserted = Decode<InsertItemEdit>(
            codec,
            "form-list.insert-item",
            "{\"index\":2,\"item\":" + LinkJson(FormKey.Null) + "}");
        inserted.Index.ShouldBe(2);
        inserted.Item.ShouldBe(FormKey.Null);
        Decode<InsertItemEdit>(
            codec,
            "form-list.insert-item",
            "{\"index\":2,\"item\":{\"isNull\":true,\"formKey\":null}}")
            .Item.ShouldBe(FormKey.Null);

        Decode<RemoveItemEdit>(codec, "form-list.remove-item", "{\"index\":3}")
            .Index.ShouldBe(3);
        var moved = Decode<MoveItemEdit>(
            codec,
            "form-list.move-item",
            "{\"sourceIndex\":4,\"destinationIndex\":1}");
        moved.SourceIndex.ShouldBe(4);
        moved.DestinationIndex.ShouldBe(1);
    }

    /// <summary>Translated-name decoding retains full, sparse, and empty native language maps exactly.</summary>
    [Fact]
    public void Decode_SetName_PreservesCompleteSparseAndEmptyTranslatedStrings()
    {
        var codec = new Fallout4FormListEditWireCodec();
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
            Decode<Fallout4SetNameEdit>(codec, "fallout4.form-list.set-name", NameArgumentsJson(complete)).Name);
        AssertTranslatedStringEqual(
            sparse,
            Decode<Fallout4SetNameEdit>(codec, "fallout4.form-list.set-name", NameArgumentsJson(sparse)).Name);
        AssertTranslatedStringEqual(
            empty,
            Decode<Fallout4SetNameEdit>(codec, "fallout4.form-list.set-name", NameArgumentsJson(empty)).Name);
    }

    /// <summary>The typed flag command accepts every installed bit combination and rejects raw or unknown bits at decode time.</summary>
    [Fact]
    public void Decode_SetMajorRecordFlags_RejectsBitsOutsideInstalledTypedMask()
    {
        var codec = new Fallout4FormListEditWireCodec();
        const int supportedMask = 0x000E9825;

        Decode<Fallout4SetMajorRecordFlagsEdit>(
            codec,
            "fallout4.form-list.set-major-record-flags",
            $"{{\"majorRecordFlags\":{supportedMask}}}")
            .MajorRecordFlags.ShouldBe((Fallout4MajorRecord.Fallout4MajorRecordFlag)supportedMask);

        var rawBit = DecodeFailure(
            codec,
            "fallout4.form-list.set-major-record-flags",
            "{\"majorRecordFlags\":1073741824}");
        rawBit.Message.ShouldContain("unsupported bits 0x40000000");

        var negative = DecodeFailure(
            codec,
            "fallout4.form-list.set-major-record-flags",
            "{\"majorRecordFlags\":-2147483648}");
        negative.Message.ShouldContain("unsupported bits 0x80000000");
    }

    /// <summary>Closed argument shapes reject unknown commands and missing, duplicate, additional, or wrongly typed properties.</summary>
    [Theory]
    [InlineData("unknown.command", "{}", "does not support")]
    [InlineData("form-list.set-editor-id", "{}", "Required property 'editorId' is missing")]
    [InlineData("form-list.set-editor-id", "{\"editorId\":\"A\",\"editorId\":\"B\"}", "occurs more than once")]
    [InlineData("form-list.set-editor-id", "{\"editorId\":\"A\",\"extra\":true}", "is not accepted")]
    [InlineData("form-list.set-editor-id", "{\"editorId\":5}", "Expected a string")]
    [InlineData("form-list.clear-items", "null", "Expected a")]
    [InlineData("form-list.clear-items", "[]", "Expected a")]
    public void Decode_ClosedArguments_RejectInvalidObjectShapes(
        string commandName,
        string json,
        string expectedMessage)
    {
        var error = DecodeFailure(new Fallout4FormListEditWireCodec(), commandName, json);

        error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        error.Message.ShouldStartWith("$");
        error.Message.ShouldContain(expectedMessage);
    }

    /// <summary>Numeric leaves reject negative, fractional, and overflowing values before a typed edit is constructed.</summary>
    [Theory]
    [InlineData("form-list.insert-item", "{\"index\":-1,\"item\":{\"isNull\":true,\"formKey\":\"Null\"}}", "cannot be negative")]
    [InlineData("form-list.remove-item", "{\"index\":2147483648}", "signed 32-bit")]
    [InlineData("form-list.move-item", "{\"sourceIndex\":0.5,\"destinationIndex\":0}", "signed 32-bit")]
    [InlineData("form-list.set-version-control", "{\"versionControl\":4294967296}", "unsigned 32-bit")]
    [InlineData("form-list.set-form-version", "{\"formVersion\":65536}", "unsigned 16-bit")]
    [InlineData("form-list.set-version-2", "{\"version2\":-1}", "unsigned 32-bit")]
    public void Decode_NumericPayloads_RejectOutOfRangeValues(
        string commandName,
        string json,
        string expectedMessage)
    {
        DecodeFailure(new Fallout4FormListEditWireCodec(), commandName, json)
            .Message.ShouldContain(expectedMessage);
    }

    /// <summary>Form links reject JSON null containers, contradictory null markers, and noncanonical FormKeys.</summary>
    [Theory]
    [InlineData("null", "Expected a native form link object")]
    [InlineData("{\"isNull\":true,\"formKey\":\"000001:Wire.esm\"}", "requires JSON null or canonical FormKey.Null")]
    [InlineData("{\"isNull\":false,\"formKey\":null}", "requires a canonical FormKey")]
    [InlineData("{\"isNull\":false,\"formKey\":\"Null\"}", "cannot contain FormKey.Null")]
    [InlineData("{\"isNull\":false,\"formKey\":\"not-a-form-key\"}", "canonical Mutagen FormKey")]
    public void Decode_FormLinks_RejectInvalidNullAndIdentityShapes(string linkJson, string expectedMessage)
    {
        var json = "{\"index\":0,\"item\":" + linkJson + "}";

        DecodeFailure(new Fallout4FormListEditWireCodec(), "form-list.insert-item", json)
            .Message.ShouldContain(expectedMessage);
    }

    /// <summary>Translated strings reject unknown or duplicate languages and inconsistent target-language values.</summary>
    [Theory]
    [InlineData("{\"name\":{\"targetLanguage\":\"english\",\"value\":null,\"translations\":[]}}", "exact installed Mutagen Language")]
    [InlineData("{\"name\":{\"targetLanguage\":\"English\",\"value\":\"A\",\"translations\":[{\"language\":\"English\",\"value\":\"A\"},{\"language\":\"English\",\"value\":\"B\"}]}}", "occurs more than once")]
    [InlineData("{\"name\":{\"targetLanguage\":\"English\",\"value\":\"A\",\"translations\":[{\"language\":\"English\",\"value\":\"B\"}]}}", "does not match")]
    [InlineData("{\"name\":null}", "Expected a translated string object")]
    public void Decode_TranslatedStrings_RejectInconsistentNativeShapes(string json, string expectedMessage)
    {
        DecodeFailure(new Fallout4FormListEditWireCodec(), "fallout4.form-list.set-name", json)
            .Message.ShouldContain(expectedMessage);
    }

    /// <summary>Caller-selected resource limits and cancellation stop traversal before publishing a typed edit.</summary>
    [Fact]
    public void Decode_EnforcesResourceLimitsAndCancellation()
    {
        var codec = new Fallout4FormListEditWireCodec();
        var shortStrings = new NativeWireReadLimits(64, 100, 10, 8, 32);
        var shortArrays = new NativeWireReadLimits(64, 100, 1, 100, 32);
        var shallow = new NativeWireReadLimits(1, 100, 10, 100, 32);
        var fewNodes = new NativeWireReadLimits(64, 1, 10, 100, 32);

        DecodeFailure(codec, "form-list.set-editor-id", "{\"editorId\":\"123456789\"}", shortStrings)
            .Message.ShouldContain("exceeds the limit of 8");
        DecodeFailure(
            codec,
            "form-list.replace-items",
            "{\"items\":[{\"isNull\":true,\"formKey\":\"Null\"},{\"isNull\":true,\"formKey\":\"Null\"}]}",
            shortArrays).Message.ShouldContain("per-array limit of 1");
        DecodeFailure(codec, "form-list.set-editor-id", "{\"editorId\":\"A\"}", shallow)
            .Message.ShouldContain("Nesting depth 2 exceeds the limit of 1");
        DecodeFailure(codec, "form-list.set-editor-id", "{\"editorId\":\"A\"}", fewNodes)
            .Message.ShouldContain("remaining node limit");

        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        Should.Throw<OperationCanceledException>(() => codec.Decode(
            "form-list.clear-items",
            Parse("{}"),
            NativeWireReadLimits.Default,
            cancellation.Token));
    }

    /// <summary>Inspector-emitted native leaves feed the matching edit commands without an alternate wire representation.</summary>
    [Fact]
    public void Decode_InspectorReadLeaves_RoundTripIntoTypedEdits()
    {
        var modKey = ModKey.FromNameAndExtension("WireRoundTrip.esm");
        var linkedKey = new FormKey(modKey, 0x801);
        var formList = new FormList(new FormKey(modKey, 0x800), Fallout4Release.Fallout4)
        {
            EditorID = "RoundTripList",
            VersionControl = uint.MaxValue,
            FormVersion = ushort.MaxValue,
            Version2 = 7,
            Fallout4MajorRecordFlags = Fallout4MajorRecord.Fallout4MajorRecordFlag.NotPlayable,
            Name = new TranslatedString(
                Language.English,
                new KeyValuePair<Language, string>(Language.French, "Nom"),
                new KeyValuePair<Language, string>(Language.English, "Name")),
        };
        formList.Items.Add(new FormLink<IFallout4MajorRecordGetter>(linkedKey));
        formList.Items.Add(new FormLink<IFallout4MajorRecordGetter>(FormKey.Null));
        var readView = WriteReadView(formList);
        var codec = new Fallout4FormListEditWireCodec();

        Decode<SetEditorIdEdit>(
            codec,
            "form-list.set-editor-id",
            WrapProperty("editorId", readView.GetProperty("EditorID"))).EditorId.ShouldBe(formList.EditorID);
        Decode<SetVersionControlEdit>(
            codec,
            "form-list.set-version-control",
            WrapProperty("versionControl", readView.GetProperty("VersionControl"))).VersionControl.ShouldBe(formList.VersionControl);
        Decode<SetFormVersionEdit>(
            codec,
            "form-list.set-form-version",
            WrapProperty("formVersion", readView.GetProperty("FormVersion"))).FormVersion.ShouldBe(formList.FormVersion);
        Decode<SetVersion2Edit>(
            codec,
            "form-list.set-version-2",
            WrapProperty("version2", readView.GetProperty("Version2"))).Version2.ShouldBe(formList.Version2);
        Decode<Fallout4SetMajorRecordFlagsEdit>(
            codec,
            "fallout4.form-list.set-major-record-flags",
            WrapProperty("majorRecordFlags", readView.GetProperty("Fallout4MajorRecordFlags")))
            .MajorRecordFlags.ShouldBe(formList.Fallout4MajorRecordFlags);
        AssertTranslatedStringEqual(
            formList.Name!,
            Decode<Fallout4SetNameEdit>(
                codec,
                "fallout4.form-list.set-name",
                WrapProperty("name", readView.GetProperty("Name"))).Name);
        Decode<ReplaceItemsEdit>(
            codec,
            "form-list.replace-items",
            WrapProperty("items", readView.GetProperty("Items"))).Items.ShouldBe(
                [linkedKey, FormKey.Null],
                ignoreOrder: false);
    }

    /// <summary>Decodes one successful edit and asserts its exact runtime type.</summary>
    /// <typeparam name="TEdit">The required existing typed edit type.</typeparam>
    /// <param name="codec">The Fallout 4 wire codec.</param>
    /// <param name="commandName">The exact command discriminator.</param>
    /// <param name="json">The complete argument JSON.</param>
    /// <returns>The decoded typed edit.</returns>
    private static TEdit Decode<TEdit>(
        Fallout4FormListEditWireCodec codec,
        string commandName,
        string json)
        where TEdit : FormListEdit
    {
        var result = codec.Decode(commandName, Parse(json), NativeWireReadLimits.Default);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Error.ShouldBeNull();
        return result.Value.ShouldBeOfType<TEdit>();
    }

    /// <summary>Decodes one expected invalid request under the default limits.</summary>
    /// <param name="codec">The Fallout 4 wire codec.</param>
    /// <param name="commandName">The exact command discriminator.</param>
    /// <param name="json">The complete invalid argument JSON.</param>
    /// <returns>The path-specific invalid-request error.</returns>
    private static EngineError DecodeFailure(
        Fallout4FormListEditWireCodec codec,
        string commandName,
        string json)
    {
        return DecodeFailure(codec, commandName, json, NativeWireReadLimits.Default);
    }

    /// <summary>Decodes one expected invalid request under caller-selected resource limits.</summary>
    /// <param name="codec">The Fallout 4 wire codec.</param>
    /// <param name="commandName">The exact command discriminator.</param>
    /// <param name="json">The complete invalid argument JSON.</param>
    /// <param name="limits">The exact resource limits to exercise.</param>
    /// <returns>The path-specific invalid-request error.</returns>
    private static EngineError DecodeFailure(
        Fallout4FormListEditWireCodec codec,
        string commandName,
        string json,
        NativeWireReadLimits limits)
    {
        var result = codec.Decode(commandName, Parse(json), limits);
        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
        return result.Error.ShouldNotBeNull();
    }

    /// <summary>Creates the exact inspector-compatible JSON for one native link.</summary>
    /// <param name="formKey">The non-null key or explicit <see cref="FormKey.Null"/>.</param>
    /// <returns>The closed native form-link object JSON.</returns>
    private static string LinkJson(FormKey formKey)
    {
        return formKey.IsNull
            ? $"{{\"isNull\":true,\"formKey\":{JsonSerializer.Serialize(formKey.ToString())}}}"
            : $"{{\"isNull\":false,\"formKey\":{JsonSerializer.Serialize(formKey.ToString())}}}";
    }

    /// <summary>Writes one translated string through the production native leaf writer and wraps it as set-name arguments.</summary>
    /// <param name="name">The exact native translated string.</param>
    /// <returns>Closed set-name argument JSON using the read-view shape.</returns>
    private static string NameArgumentsJson(ITranslatedStringGetter name)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WritePropertyName("name");
            NativeJsonLeafWriter.WriteTranslatedString(writer, name, CancellationToken.None);
            writer.WriteEndObject();
        }

        return System.Text.Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>Writes one complete Fallout 4 FormList through the production typed inspector.</summary>
    /// <param name="formList">The complete detached native record.</param>
    /// <returns>A detached typed JSON read view.</returns>
    private static JsonElement WriteReadView(FormList formList)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            new Fallout4FormListNativeInspector().WriteReadView(
                formList,
                writer,
                CancellationToken.None);
        }

        return Parse(System.Text.Encoding.UTF8.GetString(stream.ToArray()));
    }

    /// <summary>Wraps one detached read-view leaf under a command argument property.</summary>
    /// <param name="propertyName">The exact command argument name.</param>
    /// <param name="value">The inspector-emitted detached JSON leaf.</param>
    /// <returns>A complete closed command argument object JSON.</returns>
    private static string WrapProperty(string propertyName, JsonElement value)
    {
        return $"{{{JsonSerializer.Serialize(propertyName)}:{value.GetRawText()}}}";
    }

    /// <summary>Asserts complete target-language and translation-map equality.</summary>
    /// <param name="expected">The expected native translated string.</param>
    /// <param name="actual">The decoded native translated string.</param>
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

    /// <summary>Parses and detaches one test JSON value from its temporary document.</summary>
    /// <param name="json">The complete JSON text.</param>
    /// <returns>A detached JSON value.</returns>
    private static JsonElement Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}
