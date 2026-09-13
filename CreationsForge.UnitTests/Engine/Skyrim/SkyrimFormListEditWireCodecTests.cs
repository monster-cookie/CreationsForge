using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Core.Enums;
using CreationsForge.Skyrim.PluginAdapter;
using CreationsForge.Skyrim.PluginAdapter.RecordInspection;
using CreationsForge.Skyrim.PluginAdapter.Wire;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Skyrim;

/// <summary>Verifies strict bounded decoding of every Skyrim Special Edition FormList wire command.</summary>
public sealed class SkyrimFormListEditWireCodecTests
{
    /// <summary>Verifies the codec advertises the one exact game and plugin release it constructs.</summary>
    [Fact]
    public void Identity_UsesSkyrimSpecialEdition()
    {
        var codec = new SkyrimFormListEditWireCodec();

        codec.Game.ShouldBe(SupportedGame.Skyrim);
        codec.Release.ShouldBe(GameRelease.SkyrimSE);
    }

    /// <summary>Verifies every approved command decodes to its existing typed edit with exact scalar, order, duplicate, and null-link state.</summary>
    [Fact]
    public void Decode_CoversEveryApprovedTypedEdit()
    {
        var codec = new SkyrimFormListEditWireCodec();
        var first = new FormKey("Master.esm", 0x0123);
        var second = new FormKey("Output.esp", 0x0800);

        Decode<SetEditorIdEdit>(codec, "form-list.set-editor-id", "{\"editorId\":\"WireList\"}")
            .EditorId.ShouldBe("WireList");
        Decode<ClearEditorIdEdit>(codec, "form-list.clear-editor-id", "{}");

        var replacement = Decode<ReplaceItemsEdit>(
            codec,
            "form-list.replace-items",
            $"{{\"items\":[{Link(first)},{Link(first)},{{\"isNull\":true,\"formKey\":null}},{Link(second)}]}}");
        replacement.Items.ShouldBe([first, first, FormKey.Null, second]);

        var insertion = Decode<InsertItemEdit>(
            codec,
            "form-list.insert-item",
            "{\"index\":2147483647,\"item\":{\"isNull\":true,\"formKey\":null}}");
        insertion.Index.ShouldBe(int.MaxValue);
        insertion.Item.ShouldBe(FormKey.Null);

        Decode<RemoveItemEdit>(codec, "form-list.remove-item", "{\"index\":0}")
            .Index.ShouldBe(0);
        Decode<ClearItemsEdit>(codec, "form-list.clear-items", "{}");

        var move = Decode<MoveItemEdit>(
            codec,
            "form-list.move-item",
            "{\"sourceIndex\":2,\"destinationIndex\":0}");
        move.SourceIndex.ShouldBe(2);
        move.DestinationIndex.ShouldBe(0);

        Decode<SetVersionControlEdit>(
                codec,
                "form-list.set-version-control",
                "{\"versionControl\":4294967295}")
            .VersionControl.ShouldBe(uint.MaxValue);
        Decode<SetFormVersionEdit>(codec, "form-list.set-form-version", "{\"formVersion\":65535}")
            .FormVersion.ShouldBe(ushort.MaxValue);
        Decode<SetVersion2Edit>(codec, "form-list.set-version-2", "{\"version2\":65535}")
            .Version2.ShouldBe(ushort.MaxValue);
        Decode<SetCompressedEdit>(codec, "form-list.set-compressed", "{\"isCompressed\":true}")
            .IsCompressed.ShouldBeTrue();
        Decode<SetDeletedEdit>(codec, "form-list.set-deleted", "{\"isDeleted\":true}")
            .IsDeleted.ShouldBeTrue();

        var expectedFlags = SkyrimMajorRecord.SkyrimMajorRecordFlag.NotPlayable
            | SkyrimMajorRecord.SkyrimMajorRecordFlag.CantWait;
        Decode<SkyrimSetMajorRecordFlagsEdit>(
                codec,
                "skyrim.form-list.set-major-record-flags",
                $"{{\"majorRecordFlags\":{(int)expectedFlags}}}")
            .MajorRecordFlags.ShouldBe(expectedFlags);
    }

    /// <summary>Verifies empty ordered item replacement is distinct from missing arguments and constructs an empty immutable edit payload.</summary>
    [Fact]
    public void Decode_ReplaceItemsAcceptsAnExplicitEmptyOrderedList()
    {
        var edit = Decode<ReplaceItemsEdit>(
            new SkyrimFormListEditWireCodec(),
            "form-list.replace-items",
            "{\"items\":[]}");

        edit.Items.ShouldBeEmpty();
    }

    /// <summary>Verifies an explicit null link accepts the canonical FormKey.Null string emitted by the plugin inspector.</summary>
    [Fact]
    public void Decode_NullLinkAcceptsCanonicalFormKeyNullString()
    {
        var edit = Decode<InsertItemEdit>(
            new SkyrimFormListEditWireCodec(),
            "form-list.insert-item",
            $"{{\"index\":0,\"item\":{{\"isNull\":true,\"formKey\":\"{FormKey.Null}\"}}}}");

        edit.Item.ShouldBe(FormKey.Null);
    }

    /// <summary>Verifies the readable inspector FormLink representation round-trips into a typed replacement without losing duplicates or null links.</summary>
    [Fact]
    public void Decode_FormLinksRoundTripFromPluginReadJson()
    {
        var first = new FormKey("Master.esm", 0x0123);
        var formList = new FormList(new FormKey("Output.esp", 0x0800), SkyrimRelease.SkyrimSE);
        formList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(first));
        formList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(first));
        formList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(FormKey.Null));
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            new SkyrimFormListInspector().WriteReadView(
                formList,
                writer,
                TestContext.Current.CancellationToken);
        }

        using var readDocument = JsonDocument.Parse(stream.ToArray());
        var itemsJson = readDocument.RootElement.GetProperty(nameof(IFormListGetter.Items)).GetRawText();
        var edit = Decode<ReplaceItemsEdit>(
            new SkyrimFormListEditWireCodec(),
            "form-list.replace-items",
            $"{{\"items\":{itemsJson}}}");

        edit.Items.ShouldBe([first, first, FormKey.Null]);
    }

    /// <summary>Verifies unknown commands and malformed closed objects fail as invalid requests without publishing a typed edit.</summary>
    /// <param name="commandName">The absent or supported command discriminator under test.</param>
    /// <param name="argumentsJson">The malformed or command-incompatible argument JSON.</param>
    /// <param name="expectedMessage">The stable diagnostic fragment expected from the boundary.</param>
    [Theory]
    [InlineData("unknown", "{}", "does not support")]
    [InlineData("form-list.clear-items", "{\"unexpected\":true}", "not accepted")]
    [InlineData("form-list.set-editor-id", "{}", "Required property 'editorId'")]
    [InlineData("form-list.set-editor-id", "{\"editorId\":\"   \"}", "non-whitespace")]
    [InlineData("form-list.replace-items", "{}", "Required property 'items'")]
    [InlineData("form-list.remove-item", "{\"index\":-1}", "non-negative")]
    [InlineData("form-list.remove-item", "{\"index\":0,\"index\":1}", "more than once")]
    [InlineData("form-list.set-version-control", "{\"versionControl\":4294967296}", "unsigned 32-bit")]
    [InlineData("form-list.set-form-version", "{\"formVersion\":65536}", "unsigned 16-bit")]
    [InlineData("form-list.set-compressed", "{\"isCompressed\":1}", "Boolean")]
    [InlineData("skyrim.form-list.set-major-record-flags", "{\"majorRecordFlags\":1073741824}", "unsupported bits 0x40000000")]
    [InlineData("skyrim.form-list.set-major-record-flags", "{\"majorRecordFlags\":-2147483648}", "unsupported bits 0x80000000")]
    public void Decode_RejectsInvalidCommandsBeforeTypedConstruction(
        string commandName,
        string argumentsJson,
        string expectedMessage)
    {
        using var document = JsonDocument.Parse(argumentsJson);

        var result = new SkyrimFormListEditWireCodec().Decode(
            commandName,
            document.RootElement,
            RecordWireReadLimits.Default,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        result.Error.Message.ShouldContain(expectedMessage);
    }

    /// <summary>Verifies explicit null links, non-null links, and canonical FormKey text cannot contradict each other.</summary>
    /// <param name="linkJson">The malformed explicit FormLink JSON.</param>
    /// <param name="expectedMessage">The expected path-specific diagnostic fragment.</param>
    [Theory]
    [InlineData("{\"isNull\":true,\"formKey\":\"000123:Master.esm\"}", "requires JSON null or the canonical FormKey.Null identity")]
    [InlineData("{\"isNull\":false,\"formKey\":null}", "requires a canonical formKey")]
    [InlineData("{\"isNull\":false}", "Required property 'formKey'")]
    [InlineData("{\"isNull\":false,\"formKey\":\"not-a-form-key\"}", "canonical Mutagen FormKey")]
    public void Decode_RejectsMalformedOrContradictoryFormLinks(string linkJson, string expectedMessage)
    {
        using var document = JsonDocument.Parse($"{{\"items\":[{linkJson}]}}");

        var result = new SkyrimFormListEditWireCodec().Decode(
            "form-list.replace-items",
            document.RootElement,
            RecordWireReadLimits.Default,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        result.Error.Message.ShouldContain(expectedMessage);
    }

    /// <summary>Verifies per-array bounds are enforced before the decoder allocates the typed replacement collection.</summary>
    [Fact]
    public void Decode_RejectsItemsBeyondTheConfiguredArrayLimit()
    {
        var item = Link(new FormKey("Master.esm", 0x0123));
        using var document = JsonDocument.Parse($"{{\"items\":[{item},{item}]}}");
        var limits = new RecordWireReadLimits(
            RecordWireReadLimits.DefaultMaximumDepth,
            RecordWireReadLimits.DefaultMaximumNodes,
            maximumArrayElements: 1,
            RecordWireReadLimits.DefaultMaximumStringLength,
            RecordWireReadLimits.DefaultMaximumDecodedByteLength);

        var result = new SkyrimFormListEditWireCodec().Decode(
            "form-list.replace-items",
            document.RootElement,
            limits,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        result.Error.Message.ShouldContain("Array length 2 exceeds the per-array limit of 1");
    }

    /// <summary>Verifies cancellation is observed before command dispatch or typed construction.</summary>
    [Fact]
    public void Decode_ObservesCancellationBeforeConstruction()
    {
        using var document = JsonDocument.Parse("{}");
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        Should.Throw<OperationCanceledException>(() =>
            new SkyrimFormListEditWireCodec().Decode(
                "form-list.clear-items",
                document.RootElement,
                RecordWireReadLimits.Default,
                cancellationSource.Token));
    }

    /// <summary>Decodes one command and requires the successful result to have the expected typed command.</summary>
    /// <typeparam name="TEdit">The exact existing typed edit class expected from the command.</typeparam>
    /// <param name="codec">The stateless Skyrim wire codec.</param>
    /// <param name="commandName">The exact supported command discriminator.</param>
    /// <param name="argumentsJson">The complete valid closed arguments JSON.</param>
    /// <returns>The successfully decoded typed edit.</returns>
    private static TEdit Decode<TEdit>(
        SkyrimFormListEditWireCodec codec,
        string commandName,
        string argumentsJson)
        where TEdit : FormListEdit
    {
        using var document = JsonDocument.Parse(argumentsJson);
        var result = codec.Decode(
            commandName,
            document.RootElement,
            RecordWireReadLimits.Default,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value.ShouldBeOfType<TEdit>();
    }

    /// <summary>Creates the exact readable non-null FormLink JSON for one canonical FormKey.</summary>
    /// <param name="formKey">The non-null record identity to represent.</param>
    /// <returns>The closed explicit non-null FormLink JSON.</returns>
    private static string Link(FormKey formKey)
    {
        return $"{{\"isNull\":false,\"formKey\":\"{formKey}\"}}";
    }
}
