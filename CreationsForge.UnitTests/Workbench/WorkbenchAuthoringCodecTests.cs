using System.Text.Json.Nodes;
using CreationsForge.Workbench;

namespace CreationsForge.UnitTests.Workbench;

/// <summary>Verifies the guided editor's schema parsing and change payloads.</summary>
public sealed class WorkbenchAuthoringCodecTests
{
    private static readonly string[] Languages = ["English", "French", "German"];

    /// <summary>Builds a translated-string change with the selected target language and keeps languages ordered.</summary>
    [Fact]
    public void TranslatedChangeUsesTargetLanguageAndSortedValues()
    {
        var field = TranslatedField();
        field.TargetLanguage = "French";
        field.AddTranslation("French");
        field.AddTranslation("English");
        field.Translations.Single(row => row.Language == "French").Value = "Nom";
        field.Translations.Single(row => row.Language == "English").Value = "Name";
        field.OriginalCanonical = "{\"kind\":\"Null\"}";

        var error = WorkbenchAuthoringCodec.TryBuildChanges([field], out var changes);

        Assert.Null(error);
        var change = Assert.IsType<JsonObject>(Assert.Single(changes));
        Assert.Equal("Name", change["path"]?.GetValue<string>());
        Assert.Equal("Set", change["operation"]?.GetValue<string>());
        var value = Assert.IsType<JsonObject>(change["value"]);
        Assert.Equal("TranslatedString", value["kind"]?.GetValue<string>());
        Assert.Equal("French", value["targetLanguage"]?.GetValue<string>());
        var translations = Assert.IsType<JsonObject>(value["translations"]);
        Assert.Equal(["English", "French"], translations.Select(pair => pair.Key).ToArray());
        Assert.Equal("Nom", translations["French"]?.GetValue<string>());
    }

    /// <summary>Rejects an enum name that the schema did not list and sends no changes.</summary>
    [Fact]
    public void InvalidEnumIsRejectedWithoutChanges()
    {
        var field = new WorkbenchFieldDraft(
            "Type",
            "Enum",
            isNullable: true,
            isFlags: false,
            isSupported: true,
            ["Location"],
            Languages,
            string.Empty,
            string.Empty)
        {
            EnumValue = "NotAType",
            OriginalCanonical = "{\"kind\":\"Null\"}",
        };

        var error = WorkbenchAuthoringCodec.TryBuildChanges([field], out var changes);

        Assert.Contains("NotAType", error, StringComparison.Ordinal);
        Assert.Empty(changes);
    }

    /// <summary>Rejects a FormKey that is only free text.</summary>
    [Fact]
    public void InvalidFormKeyIsRejected()
    {
        var field = new WorkbenchFieldDraft(
            "AttractionRule",
            "FormLink",
            isNullable: true,
            isFlags: false,
            isSupported: true,
            [],
            Languages,
            "Link targets: AttractionRule",
            string.Empty)
        {
            FormKeyText = "bad",
            OriginalCanonical = "{\"kind\":\"Null\"}",
        };

        var error = WorkbenchAuthoringCodec.TryBuildChanges([field], out var changes);

        Assert.Contains("not a FormKey", error, StringComparison.Ordinal);
        Assert.Empty(changes);
    }

    /// <summary>Accepts a FormKey whose plugin name contains a space, and leaves Clear checked when the same text is written again.</summary>
    [Fact]
    public void SpacedPluginFormKeyIsAcceptedWithoutReleasingClear()
    {
        var field = new WorkbenchFieldDraft(
            "AttractionRule",
            "FormLink",
            isNullable: true,
            isFlags: false,
            isSupported: true,
            [],
            Languages,
            string.Empty,
            string.Empty)
        {
            FormKeyText = "00000001:My Mod.esp",
            OriginalCanonical = "{\"kind\":\"Null\"}",
        };
        field.ClearValue = true;
        field.FormKeyText = "00000001:My Mod.esp";

        Assert.True(field.ClearValue);
        field.ClearValue = false;
        var error = WorkbenchAuthoringCodec.TryBuildChanges([field], out var changes);

        Assert.Null(error);
        var change = Assert.IsType<JsonObject>(Assert.Single(changes));
        Assert.Equal("00000001:My Mod.esp", change["value"]?["formKey"]?.GetValue<string>());
    }

    /// <summary>Accepts only a save whose status is Succeeded and whose path is present.</summary>
    [Fact]
    public void SaveWithoutSucceededStatusIsNotAPublishedPath()
    {
        var failed = new JsonObject
        {
            ["structuredContent"] = new JsonObject
            {
                ["status"] = "Failed",
                ["destinationPath"] = "C:\\mods\\Output.esp",
            },
        };
        var succeeded = new JsonObject
        {
            ["structuredContent"] = new JsonObject
            {
                ["status"] = "Succeeded",
                ["destinationPath"] = "C:\\mods\\Output.esp",
            },
        };

        Assert.False(WorkbenchAuthoringCodec.TryReadSuccessfulSave(failed, out _));
        Assert.True(WorkbenchAuthoringCodec.TryReadSuccessfulSave(succeeded, out var path));
        Assert.Equal("C:\\mods\\Output.esp", path);
    }

    /// <summary>Shows list and object fields as unsupported instead of editing them.</summary>
    [Fact]
    public void UnsupportedShapesStayVisibleAndUnchanged()
    {
        var schema = new JsonObject
        {
            ["structuredContent"] = new JsonObject
            {
                ["fields"] = new JsonArray
                {
                    Field("Items", "List"),
                    Field("ObjectBounds", "Object"),
                    Field("EditorID", "String"),
                },
            },
        };

        var fields = WorkbenchAuthoringCodec.ReadSchemaFields(schema, Languages);

        Assert.Equal(3, fields.Count);
        Assert.False(fields[0].IsSupported);
        Assert.Contains("List", fields[0].UnsupportedReason, StringComparison.Ordinal);
        Assert.False(fields[1].IsSupported);
        Assert.True(fields[2].IsString);
        WorkbenchAuthoringCodec.Snapshot(fields);
        fields[2].StringValue = "Created";
        var error = WorkbenchAuthoringCodec.TryBuildChanges(fields, out var changes);
        Assert.Null(error);
        var change = Assert.Single(changes);
        Assert.NotNull(change);
        Assert.Equal("EditorID", change["path"]?.GetValue<string>());
    }

    /// <summary>Reports an unrelated record as unchanged when every field matches.</summary>
    [Fact]
    public void UnrelatedSnapshotMatchSaysUnchanged()
    {
        var before = new Dictionary<string, string> { ["EditorID"] = "{\"kind\":\"String\",\"string\":\"Leave\"}" };
        var after = new Dictionary<string, string> { ["EditorID"] = "{\"kind\":\"String\",\"string\":\"Leave\"}" };

        var text = WorkbenchAuthoringCodec.CompareMaps("UnrelatedKeyword", before, after);

        Assert.Contains("UnrelatedKeyword is unchanged", text, StringComparison.Ordinal);
    }

    private static WorkbenchFieldDraft TranslatedField()
    {
        return new WorkbenchFieldDraft(
            "Name",
            "TranslatedString",
            isNullable: false,
            isFlags: false,
            isSupported: true,
            [],
            Languages,
            string.Empty,
            string.Empty);
    }

    private static JsonObject Field(string path, string kind)
    {
        return new JsonObject
        {
            ["path"] = path,
            ["kind"] = kind,
            ["isNullable"] = true,
            ["isFlags"] = false,
            ["choices"] = new JsonArray(),
            ["referenceTargets"] = new JsonArray(),
        };
    }
}
