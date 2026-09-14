using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Core.Enums;
using CreationsForge.Fallout4.PluginAdapter.Edits;
using CreationsForge.Fallout4.PluginAdapter.Wire;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Fallout4;

/// <summary>Verifies the deterministic content-bound Fallout 4 FormList wire schema catalog.</summary>
public sealed class Fallout4FormListEditWireSchemaCatalogTests
{
    /// <summary>The catalog exposes every approved command followed by the shared plugin leaf types in ordinal order.</summary>
    [Fact]
    public void IdentityAndNodes_AreCompleteDeterministicAndContentBound()
    {
        var catalog = new Fallout4FormListEditWireSchemaCatalog();
        var second = new Fallout4FormListEditWireSchemaCatalog();
        var expectedCommands = new[]
        {
            "fallout4.form-list.clear-name",
            "fallout4.form-list.set-major-record-flags",
            "fallout4.form-list.set-name",
            "form-list.clear-editor-id",
            "form-list.clear-items",
            "form-list.insert-item",
            "form-list.move-item",
            "form-list.remove-item",
            "form-list.replace-items",
            "form-list.set-compressed",
            "form-list.set-deleted",
            "form-list.set-editor-id",
            "form-list.set-form-version",
            "form-list.set-version-2",
            "form-list.set-version-control",
        };
        var expectedTypes = new[]
        {
            "record.form-link",
            "record.translated-string",
        };

        catalog.Identity.Game.ShouldBe(SupportedGame.Fallout4);
        catalog.Identity.Release.ShouldBe(GameRelease.Fallout4);
        catalog.Identity.SchemaVersion.ShouldBe("1.0.0");
        catalog.Identity.CatalogId.Length.ShouldBe(64);
        catalog.Identity.CatalogId.ShouldMatch("^[0-9A-F]{64}$");
        second.Identity.CatalogId.ShouldBe(catalog.Identity.CatalogId);
        catalog.Nodes.Count.ShouldBe(17);
        catalog.Nodes.Take(expectedCommands.Length).ShouldAllBe(
            node => node.Kind == RecordWireSchemaNodeKind.Command);
        catalog.Nodes.Take(expectedCommands.Length).Select(node => node.Name)
            .ShouldBe(expectedCommands, ignoreOrder: false);
        catalog.Nodes.Skip(expectedCommands.Length).ShouldAllBe(
            node => node.Kind == RecordWireSchemaNodeKind.Type);
        catalog.Nodes.Skip(expectedCommands.Length).Select(node => node.Name)
            .ShouldBe(expectedTypes, ignoreOrder: false);
        catalog.Nodes.ShouldAllBe(node => node.CatalogId == catalog.Identity.CatalogId);
        catalog.Nodes.Select(node => node.Name).Distinct(StringComparer.Ordinal).Count()
            .ShouldBe(catalog.Nodes.Count);
    }

    /// <summary>Every catalog node contains only closed object schemas, including all nested inline plugin leaf objects.</summary>
    [Fact]
    public void ReadNode_AllSchemasAreDetachedClosedObjects()
    {
        var catalog = new Fallout4FormListEditWireSchemaCatalog();

        foreach (var key in catalog.Nodes)
        {
            var result = catalog.ReadNode(key, TestContext.Current.CancellationToken);
            result.Succeeded.ShouldBeTrue(result.Error?.Message);
            result.Value.ShouldNotBeNull();
            result.Value!.Key.ShouldBeSameAs(key);
            result.Value.Schema.ValueKind.ShouldBe(JsonValueKind.Object);
            AllObjectSchemasAreClosed(result.Value.Schema).ShouldBeTrue(key.Name);
        }

        var firstRead = catalog.ReadNode(catalog.Nodes[0]).Value!;
        var secondRead = catalog.ReadNode(catalog.Nodes[0]).Value!;
        firstRead.Schema.GetRawText().ShouldBe(secondRead.Schema.GetRawText());
        firstRead.Schema.GetProperty("type").GetString().ShouldBe("object");
    }

    /// <summary>Every advertised command default is valid for its codec, while target-dependent commands explicitly omit defaults.</summary>
    [Fact]
    public void CommandDefaults_AreHonestPluginConstructibleTemplates()
    {
        var catalog = new Fallout4FormListEditWireSchemaCatalog();
        var codec = new Fallout4FormListEditWireCodec();
        var unavailableDefaults = new HashSet<string>(StringComparer.Ordinal)
        {
            "fallout4.form-list.set-name",
            "form-list.insert-item",
            "form-list.move-item",
            "form-list.remove-item",
            "form-list.set-editor-id",
        };

        foreach (var key in catalog.Nodes.Where(node => node.Kind == RecordWireSchemaNodeKind.Command))
        {
            var node = catalog.ReadNode(key).Value!;
            if (unavailableDefaults.Contains(key.Name))
            {
                node.DefaultTemplate.ShouldBeNull();
                continue;
            }

            node.DefaultTemplate.ShouldNotBeNull();
            var decoded = codec.Decode(
                key.Name,
                node.DefaultTemplate!.Value,
                RecordWireReadLimits.Default);
            decoded.Succeeded.ShouldBeTrue(decoded.Error?.Message);
            decoded.Value.ShouldNotBeNull();
            decoded.Value!.CommandName.ShouldBe(key.Name);
        }

        var formVersionKey = catalog.Nodes.Single(node =>
            node.Kind == RecordWireSchemaNodeKind.Command
            && node.Name == "form-list.set-form-version");
        catalog.ReadNode(formVersionKey).Value!.DefaultTemplate!.Value
            .GetProperty("formVersion").GetUInt16().ShouldBe((ushort)131);
    }

    /// <summary>The shared type defaults preserve a plugin null link and a valid empty translated string.</summary>
    [Fact]
    public void TypeDefaults_AreValidInspectorCompatiblePluginLeaves()
    {
        var catalog = new Fallout4FormListEditWireSchemaCatalog();
        var codec = new Fallout4FormListEditWireCodec();
        var linkKey = catalog.Nodes.Single(node => node.Name == "record.form-link");
        var translatedStringKey = catalog.Nodes.Single(node => node.Name == "record.translated-string");
        var linkDefault = catalog.ReadNode(linkKey).Value!.DefaultTemplate!.Value;
        var translatedStringDefault = catalog.ReadNode(translatedStringKey).Value!.DefaultTemplate!.Value;

        linkDefault.GetProperty("isNull").GetBoolean().ShouldBeTrue();
        linkDefault.GetProperty("formKey").GetString().ShouldBe(FormKey.Null.ToString());

        var insertArguments = Parse("{\"index\":0,\"item\":" + linkDefault.GetRawText() + "}");
        var insert = codec.Decode(
            "form-list.insert-item",
            insertArguments,
            RecordWireReadLimits.Default);
        insert.Succeeded.ShouldBeTrue(insert.Error?.Message);
        insert.Value.ShouldBeOfType<InsertItemEdit>().Item.ShouldBe(FormKey.Null);

        var nameArguments = Parse("{\"name\":" + translatedStringDefault.GetRawText() + "}");
        var setName = codec.Decode(
            "fallout4.form-list.set-name",
            nameArguments,
            RecordWireReadLimits.Default);
        setName.Succeeded.ShouldBeTrue(setName.Error?.Message);
        var name = setName.Value.ShouldBeOfType<Fallout4SetNameEdit>().Name;
        name.TargetLanguage.ToString().ShouldBe("English");
        name.String.ShouldBeNull();
        name.ShouldBeEmpty();
    }

    /// <summary>Command schemas publish the exact agreed argument names and signed typed-flag metadata.</summary>
    [Fact]
    public void CommandSchemas_UseStableArgumentNamesAndTypedFlagMetadata()
    {
        var catalog = new Fallout4FormListEditWireSchemaCatalog();
        var expectedArguments = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["fallout4.form-list.clear-name"] = [],
            ["fallout4.form-list.set-major-record-flags"] = ["majorRecordFlags"],
            ["fallout4.form-list.set-name"] = ["name"],
            ["form-list.clear-editor-id"] = [],
            ["form-list.clear-items"] = [],
            ["form-list.insert-item"] = ["index", "item"],
            ["form-list.move-item"] = ["sourceIndex", "destinationIndex"],
            ["form-list.remove-item"] = ["index"],
            ["form-list.replace-items"] = ["items"],
            ["form-list.set-compressed"] = ["isCompressed"],
            ["form-list.set-deleted"] = ["isDeleted"],
            ["form-list.set-editor-id"] = ["editorId"],
            ["form-list.set-form-version"] = ["formVersion"],
            ["form-list.set-version-2"] = ["version2"],
            ["form-list.set-version-control"] = ["versionControl"],
        };

        foreach (var command in expectedArguments)
        {
            var key = catalog.Nodes.Single(node =>
                node.Kind == RecordWireSchemaNodeKind.Command
                && node.Name == command.Key);
            var schema = catalog.ReadNode(key).Value!.Schema;
            schema.GetProperty("properties").EnumerateObject().Select(property => property.Name)
                .ShouldBe(command.Value, ignoreOrder: false);
            schema.GetProperty("required").EnumerateArray().Select(value => value.GetString())
                .ShouldBe(command.Value, ignoreOrder: false);
        }

        var flagsKey = catalog.Nodes.Single(node => node.Name == "fallout4.form-list.set-major-record-flags");
        var flagsSchema = catalog.ReadNode(flagsKey).Value!.Schema
            .GetProperty("properties")
            .GetProperty("majorRecordFlags");
        flagsSchema.GetProperty("type").GetString().ShouldBe("integer");
        flagsSchema.GetProperty("minimum").GetInt32().ShouldBe(0);
        flagsSchema.GetProperty("maximum").GetInt32().ShouldBe(int.MaxValue);
        flagsSchema.GetProperty("x-supported-bit-mask").GetString().ShouldBe("0x000E9825");
    }

    /// <summary>Stale, foreign, and unknown content-bound keys fail without substituting a current node.</summary>
    [Fact]
    public void ReadNode_RejectsStaleForeignAndUnknownKeysAndHonorsCancellation()
    {
        var catalog = new Fallout4FormListEditWireSchemaCatalog();
        var stale = new RecordWireSchemaNodeKey(
            new string('0', 64),
            RecordWireSchemaNodeKind.Command,
            "form-list.clear-items");
        var unknown = new RecordWireSchemaNodeKey(
            catalog.Identity.CatalogId,
            RecordWireSchemaNodeKind.Command,
            "form-list.unknown");
        var wrongKind = new RecordWireSchemaNodeKey(
            catalog.Identity.CatalogId,
            RecordWireSchemaNodeKind.Type,
            "form-list.clear-items");

        catalog.ReadNode(stale).Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        catalog.ReadNode(unknown).Error!.Message.ShouldContain("not present");
        catalog.ReadNode(wrongKind).Error!.Message.ShouldContain("not present");

        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        Should.Throw<OperationCanceledException>(() => catalog.ReadNode(
            catalog.Nodes[0],
            cancellation.Token));
    }

    /// <summary>Recursively confirms that every object schema rejects undeclared properties.</summary>
    /// <param name="schema">The schema node or nested schema to inspect.</param>
    /// <returns><see langword="true"/> when every reachable object schema is closed.</returns>
    private static bool AllObjectSchemasAreClosed(JsonElement schema)
    {
        if (schema.ValueKind != JsonValueKind.Object)
        {
            return true;
        }

        if (schema.TryGetProperty("type", out var type)
            && type.ValueKind == JsonValueKind.String
            && type.GetString() == "object"
            && (!schema.TryGetProperty("additionalProperties", out var additionalProperties)
                || additionalProperties.ValueKind != JsonValueKind.False))
        {
            return false;
        }

        if (schema.TryGetProperty("properties", out var properties)
            && properties.EnumerateObject().Any(property => !AllObjectSchemasAreClosed(property.Value)))
        {
            return false;
        }

        if (schema.TryGetProperty("items", out var items)
            && !AllObjectSchemasAreClosed(items))
        {
            return false;
        }

        return true;
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
