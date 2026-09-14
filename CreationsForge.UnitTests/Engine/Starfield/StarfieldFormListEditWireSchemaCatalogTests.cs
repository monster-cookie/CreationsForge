using System.Text;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Core.Enums;
using CreationsForge.Starfield.PluginAdapter.RecordInspection;
using CreationsForge.Starfield.PluginAdapter.Wire;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Starfield;

/// <summary>Verifies the content-bound Starfield command and complete generated plugin-type schema catalog.</summary>
public sealed class StarfieldFormListEditWireSchemaCatalogTests
{
    /// <summary>Verifies catalog identity and deterministic command-first node coverage include all generated types exactly once.</summary>
    [Fact]
    public void Nodes_ContainEveryCommandAndGeneratedTypeInDeterministicOrder()
    {
        var first = new StarfieldFormListEditWireSchemaCatalog();
        var second = new StarfieldFormListEditWireSchemaCatalog();
        var expectedCommands = new[]
        {
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
            "starfield.form-list.add-component",
            "starfield.form-list.clear-add-to-list",
            "starfield.form-list.clear-conditional-entries",
            "starfield.form-list.clear-name",
            "starfield.form-list.remove-component",
            "starfield.form-list.replace-component",
            "starfield.form-list.replace-components",
            "starfield.form-list.set-add-to-list",
            "starfield.form-list.set-conditional-entries",
            "starfield.form-list.set-major-flags",
            "starfield.form-list.set-name",
        };
        var expectedTypes = new[]
            {
                StarfieldFormListEditWireSchemaCatalog.FormLinkTypeName,
                StarfieldFormListEditWireSchemaCatalog.TranslatedStringTypeName,
            }
            .Concat(StarfieldGeneratedRecordFieldSchema.TypeNames)
            .ToArray();

        first.Identity.Game.ShouldBe(SupportedGame.Starfield);
        first.Identity.Release.ShouldBe(GameRelease.Starfield);
        first.Identity.SchemaVersion.ShouldBe("1.0.0");
        first.Identity.CatalogId.ShouldBe(second.Identity.CatalogId);
        first.Identity.CatalogId.Length.ShouldBe(64);
        first.Nodes.Select(node => node.CatalogId).Distinct().ShouldBe(
            new[] { first.Identity.CatalogId },
            ignoreOrder: false);
        first.Nodes.Select(node => (node.Kind, node.Name)).Distinct().Count().ShouldBe(first.Nodes.Count);
        first.Nodes
            .Where(node => node.Kind == RecordWireSchemaNodeKind.Command)
            .Select(node => node.Name)
            .ShouldBe(expectedCommands, ignoreOrder: false);
        first.Nodes
            .Where(node => node.Kind == RecordWireSchemaNodeKind.Type)
            .Select(node => node.Name)
            .ShouldBe(expectedTypes, ignoreOrder: false);
    }

    /// <summary>Verifies complex command schemas reference the exact generated unions and publish only honest context-independent defaults.</summary>
    [Fact]
    public void ReadNode_ComplexCommandsReferenceGeneratedUnionsAndHonestDefaults()
    {
        var catalog = new StarfieldFormListEditWireSchemaCatalog();
        var add = ReadNode(catalog, RecordWireSchemaNodeKind.Command, "starfield.form-list.add-component");
        var replaceAll = ReadNode(catalog, RecordWireSchemaNodeKind.Command, "starfield.form-list.replace-components");
        var conditional = ReadNode(catalog, RecordWireSchemaNodeKind.Command, "starfield.form-list.set-conditional-entries");
        var commandFormLink = ReadNode(
            catalog,
            RecordWireSchemaNodeKind.Type,
            StarfieldFormListEditWireSchemaCatalog.FormLinkTypeName);
        var expectedComponentReference = StarfieldGeneratedRecordFieldSchema.TypeUriPrefix + "record.component";
        var expectedConditionReference = StarfieldGeneratedRecordFieldSchema.TypeUriPrefix + "record.condition";

        add.Schema.GetProperty("properties").GetProperty("component").GetProperty("$ref").GetString()
            .ShouldBe(expectedComponentReference);
        add.DefaultTemplate.ShouldBeNull();
        replaceAll.Schema.GetProperty("properties").GetProperty("components").GetProperty("items")
            .GetProperty("$ref").GetString().ShouldBe(expectedComponentReference);
        replaceAll.DefaultTemplate.ShouldNotBeNull();
        replaceAll.DefaultTemplate.Value.GetProperty("components").GetArrayLength().ShouldBe(0);
        conditional.Schema.GetProperty("properties").GetProperty("conditionalEntries")
            .GetProperty("items").GetProperty("properties").GetProperty("Conditions")
            .GetProperty("oneOf")[1].GetProperty("items").GetProperty("$ref").GetString()
            .ShouldBe(expectedConditionReference);
        conditional.DefaultTemplate.ShouldNotBeNull();
        conditional.DefaultTemplate.Value.GetProperty("conditionalEntries").GetArrayLength().ShouldBe(0);
        commandFormLink.DefaultTemplate.ShouldNotBeNull();
        commandFormLink.DefaultTemplate.Value.GetProperty("isNull").GetBoolean().ShouldBeTrue();
        commandFormLink.DefaultTemplate.Value.GetProperty("formKey").GetString().ShouldBe(FormKey.Null.ToString());
    }

    /// <summary>Verifies a generated concrete default is detached, decoder-constructible, and writer-compatible byte for byte.</summary>
    [Fact]
    public void ReadNode_GeneratedConcreteDefaultRoundTripsThroughClosedReaderAndWriter()
    {
        const string typeName = "Mutagen.Bethesda.Starfield.PropertySheetComponent";
        var catalog = new StarfieldFormListEditWireSchemaCatalog();
        var node = ReadNode(catalog, RecordWireSchemaNodeKind.Type, typeName);
        node.DefaultTemplate.ShouldNotBeNull();
        var defaultTemplate = node.DefaultTemplate.Value;

        var decoded = RecordWireReadContext.Decode(
            defaultTemplate,
            RecordWireReadLimits.Default,
            TestContext.Current.CancellationToken,
            (context, value) => StarfieldNestedFieldCodec.ReadRecordType(context, value, typeName));

        decoded.Succeeded.ShouldBeTrue(decoded.Error?.Message);
        decoded.Error.ShouldBeNull();
        WriteRecordType(typeName, decoded.Value.ShouldNotBeNull())
            .ShouldBe(defaultTemplate.GetRawText());
    }

    /// <summary>Verifies stale, foreign, and unknown node identities fail without returning schema content.</summary>
    [Fact]
    public void ReadNode_RejectsForeignAndUnknownKeys()
    {
        var catalog = new StarfieldFormListEditWireSchemaCatalog();
        var foreignId = new string('0', 64);
        if (string.Equals(foreignId, catalog.Identity.CatalogId, StringComparison.Ordinal))
        {
            foreignId = new string('1', 64);
        }

        var foreign = catalog.ReadNode(new RecordWireSchemaNodeKey(
            foreignId,
            RecordWireSchemaNodeKind.Command,
            "form-list.clear-items"));
        foreign.Succeeded.ShouldBeFalse();
        foreign.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.InvalidRequest);

        var unknown = catalog.ReadNode(new RecordWireSchemaNodeKey(
            catalog.Identity.CatalogId,
            RecordWireSchemaNodeKind.Type,
            "record.unknown"));
        unknown.Succeeded.ShouldBeFalse();
        unknown.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.InvalidRequest);
    }

    /// <summary>Reads one exact schema node from the current catalog.</summary>
    /// <param name="catalog">The catalog to read.</param>
    /// <param name="kind">The required node kind.</param>
    /// <param name="name">The exact command or type name.</param>
    /// <returns>The detached successful schema node.</returns>
    private static RecordWireSchemaNode ReadNode(
        StarfieldFormListEditWireSchemaCatalog catalog,
        RecordWireSchemaNodeKind kind,
        string name)
    {
        var key = catalog.Nodes.Single(node => node.Kind == kind && node.Name == name);
        var result = catalog.ReadNode(key, TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Error.ShouldBeNull();
        return result.Value.ShouldNotBeNull();
    }

    /// <summary>Writes one decoded generated plugin type through the exact catalog-selected writer.</summary>
    /// <param name="typeName">The generated mutable plugin full name.</param>
    /// <param name="value">The matching decoded mutable record value.</param>
    /// <returns>The compact writer-compatible JSON.</returns>
    private static string WriteRecordType(string typeName, object value)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            StarfieldNestedFieldCodec.WriteRecordType(
                writer,
                typeName,
                value,
                TestContext.Current.CancellationToken);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
