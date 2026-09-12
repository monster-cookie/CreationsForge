using System.Security.Cryptography;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Core.Enums;
using CreationsForge.Skyrim.Native.Wire;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Skyrim;

/// <summary>Verifies the immutable content-bound Skyrim FormList wire schema and native defaults catalog.</summary>
public sealed class SkyrimFormListEditWireSchemaCatalogTests
{
    /// <summary>The exact command-first, type-second order exposed by the first Skyrim catalog version.</summary>
    private static readonly IReadOnlyList<(NativeWireSchemaNodeKind Kind, string Name)> ExpectedNodes =
        Array.AsReadOnly<(NativeWireSchemaNodeKind Kind, string Name)>(
        [
            (NativeWireSchemaNodeKind.Command, "form-list.clear-editor-id"),
            (NativeWireSchemaNodeKind.Command, "form-list.clear-items"),
            (NativeWireSchemaNodeKind.Command, "form-list.insert-item"),
            (NativeWireSchemaNodeKind.Command, "form-list.move-item"),
            (NativeWireSchemaNodeKind.Command, "form-list.remove-item"),
            (NativeWireSchemaNodeKind.Command, "form-list.replace-items"),
            (NativeWireSchemaNodeKind.Command, "form-list.set-compressed"),
            (NativeWireSchemaNodeKind.Command, "form-list.set-deleted"),
            (NativeWireSchemaNodeKind.Command, "form-list.set-editor-id"),
            (NativeWireSchemaNodeKind.Command, "form-list.set-form-version"),
            (NativeWireSchemaNodeKind.Command, "form-list.set-version-2"),
            (NativeWireSchemaNodeKind.Command, "form-list.set-version-control"),
            (NativeWireSchemaNodeKind.Command, "skyrim.form-list.set-major-record-flags"),
            (NativeWireSchemaNodeKind.Type, "native.form-link"),
            (NativeWireSchemaNodeKind.Type, "skyrim.form-list")
        ]);

    /// <summary>Verifies identity, ordering, closed schemas, and read-only native fields are stable and complete.</summary>
    [Fact]
    public void Catalog_ExposesDeterministicClosedCommandAndTypeNodes()
    {
        var catalog = new SkyrimFormListEditWireSchemaCatalog();

        catalog.Identity.Game.ShouldBe(SupportedGame.Skyrim);
        catalog.Identity.Release.ShouldBe(GameRelease.SkyrimSE);
        catalog.Identity.SchemaVersion.ShouldBe("1.0.0");
        catalog.Identity.CatalogId.Length.ShouldBe(64);
        catalog.Nodes.Select(node => (node.Kind, node.Name)).ShouldBe(ExpectedNodes);
        catalog.Nodes.ShouldAllBe(node => node.CatalogId == catalog.Identity.CatalogId);

        foreach (var key in catalog.Nodes)
        {
            var read = catalog.ReadNode(key, TestContext.Current.CancellationToken);
            read.Succeeded.ShouldBeTrue(read.Error?.Message);
            var node = read.Value.ShouldNotBeNull();
            node.Schema.GetProperty("$schema").GetString()
                .ShouldBe("https://json-schema.org/draft/2020-12/schema");
            node.Schema.GetProperty("type").GetString().ShouldBe("object");
            node.Schema.GetProperty("additionalProperties").GetBoolean().ShouldBeFalse();
        }

        var formList = Read(catalog, NativeWireSchemaNodeKind.Type, "skyrim.form-list");
        formList.DefaultTemplate.ShouldBeNull();
        var properties = formList.Schema.GetProperty("properties");
        properties.EnumerateObject().Select(property => property.Name).ShouldBe(
        [
            "MajorRecordFlagsRaw",
            "FormKey",
            "VersionControl",
            "EditorID",
            "FormVersion",
            "Version2",
            "SkyrimMajorRecordFlags",
            "Items"
        ]);
        properties.GetProperty("MajorRecordFlagsRaw").GetProperty("readOnly").GetBoolean().ShouldBeTrue();
        properties.GetProperty("FormKey").GetProperty("readOnly").GetBoolean().ShouldBeTrue();
        properties.GetProperty("Items").GetProperty("items").TryGetProperty("$ref", out _)
            .ShouldBeFalse();
        var supportedFlags = SkyrimMajorRecord.SkyrimMajorRecordFlag.ESM
            | SkyrimMajorRecord.SkyrimMajorRecordFlag.NotPlayable
            | SkyrimMajorRecord.SkyrimMajorRecordFlag.Deleted
            | SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled
            | SkyrimMajorRecord.SkyrimMajorRecordFlag.Ignored
            | SkyrimMajorRecord.SkyrimMajorRecordFlag.VisibleWhenDistant
            | SkyrimMajorRecord.SkyrimMajorRecordFlag.Dangerous_OffLimits_InteriorCell
            | SkyrimMajorRecord.SkyrimMajorRecordFlag.Compressed
            | SkyrimMajorRecord.SkyrimMajorRecordFlag.CantWait;
        Read(catalog, NativeWireSchemaNodeKind.Command, "skyrim.form-list.set-major-record-flags")
            .Schema.GetProperty("properties").GetProperty("majorRecordFlags")
            .GetProperty("x-supported-bit-mask").GetString()
            .ShouldBe($"0x{unchecked((uint)(int)supportedFlags):X8}");
    }

    /// <summary>Verifies every supplied command default is syntax-valid for the same strict codec and matches native FormList defaults.</summary>
    [Fact]
    public void Catalog_DefaultTemplatesDecodeThroughTheTypedCodec()
    {
        var catalog = new SkyrimFormListEditWireSchemaCatalog();
        var codec = new SkyrimFormListEditWireCodec();
        var expectedDefaults = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["form-list.clear-editor-id"] = "{}",
            ["form-list.replace-items"] = "{\"items\":[]}",
            ["form-list.clear-items"] = "{}",
            ["form-list.set-version-control"] = "{\"versionControl\":0}",
            ["form-list.set-form-version"] = "{\"formVersion\":44}",
            ["form-list.set-version-2"] = "{\"version2\":0}",
            ["form-list.set-compressed"] = "{\"isCompressed\":false}",
            ["form-list.set-deleted"] = "{\"isDeleted\":false}",
            ["skyrim.form-list.set-major-record-flags"] = "{\"majorRecordFlags\":0}"
        };

        foreach (var key in catalog.Nodes.Where(key => key.Kind == NativeWireSchemaNodeKind.Command))
        {
            var node = catalog.ReadNode(key, TestContext.Current.CancellationToken).Value.ShouldNotBeNull();
            if (!expectedDefaults.TryGetValue(key.Name, out var expectedDefault))
            {
                node.DefaultTemplate.ShouldBeNull();
                continue;
            }

            node.DefaultTemplate.ShouldNotBeNull();
            node.DefaultTemplate.Value.GetRawText().ShouldBe(expectedDefault);
            var decoded = codec.Decode(
                key.Name,
                node.DefaultTemplate.Value,
                NativeWireReadLimits.Default,
                TestContext.Current.CancellationToken);
            decoded.Succeeded.ShouldBeTrue(decoded.Error?.Message);
            decoded.Value.ShouldNotBeNull();
            decoded.Value.CommandName.ShouldBe(key.Name);
        }

        Read(catalog, NativeWireSchemaNodeKind.Type, "native.form-link")
            .DefaultTemplate!.Value.GetRawText()
            .ShouldBe($"{{\"isNull\":true,\"formKey\":\"{FormKey.Null}\"}}");
    }

    /// <summary>Verifies the published identity is the SHA-256 of exact canonical metadata and ordered exposed node content.</summary>
    [Fact]
    public void CatalogId_HashesCanonicalOrderedExposedContent()
    {
        var catalog = new SkyrimFormListEditWireSchemaCatalog();
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteString("game", catalog.Identity.Game.ToString());
            writer.WriteString("release", catalog.Identity.Release.ToString());
            writer.WriteString("schemaVersion", catalog.Identity.SchemaVersion);
            writer.WriteStartArray("nodes");
            foreach (var key in catalog.Nodes)
            {
                var node = catalog.ReadNode(key, TestContext.Current.CancellationToken).Value.ShouldNotBeNull();
                writer.WriteStartObject();
                writer.WriteString("kind", key.Kind.ToString());
                writer.WriteString("name", key.Name);
                writer.WritePropertyName("schema");
                node.Schema.WriteTo(writer);
                writer.WritePropertyName("defaultTemplate");
                if (node.DefaultTemplate.HasValue)
                {
                    node.DefaultTemplate.Value.WriteTo(writer);
                }
                else
                {
                    writer.WriteNullValue();
                }

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        Convert.ToHexString(SHA256.HashData(stream.ToArray())).ShouldBe(catalog.Identity.CatalogId);
        new SkyrimFormListEditWireSchemaCatalog().Identity.CatalogId.ShouldBe(catalog.Identity.CatalogId);
    }

    /// <summary>Verifies stale, foreign, and unknown content-bound keys fail without returning a schema node.</summary>
    [Fact]
    public void ReadNode_RejectsStaleForeignAndUnknownKeys()
    {
        var catalog = new SkyrimFormListEditWireSchemaCatalog();
        var foreignCatalogId = new string('0', 64);
        var foreign = catalog.ReadNode(
            new NativeWireSchemaNodeKey(
                foreignCatalogId,
                NativeWireSchemaNodeKind.Command,
                catalog.Nodes[0].Name),
            TestContext.Current.CancellationToken);
        var unknown = catalog.ReadNode(
            new NativeWireSchemaNodeKey(
                catalog.Identity.CatalogId,
                NativeWireSchemaNodeKind.Command,
                "form-list.unknown"),
            TestContext.Current.CancellationToken);

        foreign.Succeeded.ShouldBeFalse();
        foreign.Error.ShouldNotBeNull();
        foreign.Error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        foreign.Error.Message.ShouldContain("stale or foreign");
        unknown.Succeeded.ShouldBeFalse();
        unknown.Error.ShouldNotBeNull();
        unknown.Error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        unknown.Error.Message.ShouldContain("unknown");
    }

    /// <summary>Verifies each schema read returns a detached node with content equal to the immutable catalog source.</summary>
    [Fact]
    public void ReadNode_ReturnsDetachedEquivalentNodes()
    {
        var catalog = new SkyrimFormListEditWireSchemaCatalog();
        var first = catalog.ReadNode(catalog.Nodes[0], TestContext.Current.CancellationToken).Value.ShouldNotBeNull();
        var second = catalog.ReadNode(catalog.Nodes[0], TestContext.Current.CancellationToken).Value.ShouldNotBeNull();

        ReferenceEquals(first, second).ShouldBeFalse();
        ReferenceEquals(first.Key, second.Key).ShouldBeFalse();
        first.Schema.GetRawText().ShouldBe(second.Schema.GetRawText());
    }

    /// <summary>Verifies cancellation is observed before a catalog lookup or detached node materialization.</summary>
    [Fact]
    public void ReadNode_ObservesCancellation()
    {
        var catalog = new SkyrimFormListEditWireSchemaCatalog();
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        Should.Throw<OperationCanceledException>(() =>
            catalog.ReadNode(catalog.Nodes[0], cancellationSource.Token));
    }

    /// <summary>Reads one expected node by exact kind and stable name.</summary>
    /// <param name="catalog">The immutable Skyrim schema catalog.</param>
    /// <param name="kind">The expected command or type node role.</param>
    /// <param name="name">The exact stable node name.</param>
    /// <returns>The successfully detached schema node.</returns>
    private static NativeWireSchemaNode Read(
        SkyrimFormListEditWireSchemaCatalog catalog,
        NativeWireSchemaNodeKind kind,
        string name)
    {
        var key = catalog.Nodes.Single(candidate =>
            candidate.Kind == kind && string.Equals(candidate.Name, name, StringComparison.Ordinal));
        var result = catalog.ReadNode(key, TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value.ShouldNotBeNull();
    }
}
