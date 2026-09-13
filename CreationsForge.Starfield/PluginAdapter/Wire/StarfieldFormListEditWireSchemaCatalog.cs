using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Core.Enums;
using CreationsForge.Starfield.PluginAdapter.RecordInspection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Starfield.PluginAdapter.Wire;

/// <summary>Publishes the immutable content-bound Starfield FormList command and complete generated record schemas.</summary>
public sealed class StarfieldFormListEditWireSchemaCatalog : IFormListEditWireSchemaCatalog
{
    /// <summary>The first stable Starfield FormList wire schema version.</summary>
    private const string SchemaVersion = "1.0.0";

    /// <summary>The stable inspector-compatible Mutagen form-link type key.</summary>
    internal const string FormLinkTypeName = "command.form-link";

    /// <summary>The stable inspector-compatible translated-string type key.</summary>
    internal const string TranslatedStringTypeName = "command.translated-string";

    /// <summary>The closed schema shared by every no-argument command.</summary>
    private const string EmptyArgumentsSchema = "{\"type\":\"object\",\"description\":\"This command has no arguments.\",\"properties\":{},\"required\":[],\"additionalProperties\":false}";

    /// <summary>The closed schema for one exact inspector-compatible plugin form link.</summary>
    private const string FormLinkSchema = "{\"type\":\"object\",\"description\":\"One FormList link. Plugin null links require isNull true and either JSON null or the canonical FormKey.Null string; non-null links require isNull false and a canonical non-null Mutagen FormKey string.\",\"properties\":{\"isNull\":{\"type\":\"boolean\"},\"formKey\":{\"type\":[\"string\",\"null\"],\"maxLength\":1048576}},\"required\":[\"isNull\",\"formKey\"],\"additionalProperties\":false}";

    /// <summary>The closed schema for a non-null inspector-compatible plugin form link.</summary>
    private const string NonNullFormLinkSchema = "{\"type\":\"object\",\"description\":\"One non-null FormList link.\",\"properties\":{\"isNull\":{\"const\":false},\"formKey\":{\"type\":\"string\",\"maxLength\":1048576}},\"required\":[\"isNull\",\"formKey\"],\"additionalProperties\":false}";

    /// <summary>The closed schema for one exact inspector-compatible translated string.</summary>
    private const string TranslatedStringSchema = "{\"type\":\"object\",\"description\":\"One record translated string. The value must exactly match the target-language entry, or be null when that entry is absent. Languages use exact installed Mutagen names and may occur only once.\",\"properties\":{\"targetLanguage\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1048576},\"value\":{\"type\":[\"string\",\"null\"],\"maxLength\":1048576},\"translations\":{\"type\":\"array\",\"maxItems\":65536,\"x-unique-property\":\"language\",\"items\":{\"type\":\"object\",\"properties\":{\"language\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1048576},\"value\":{\"type\":\"string\",\"maxLength\":1048576}},\"required\":[\"language\",\"value\"],\"additionalProperties\":false}}},\"required\":[\"targetLanguage\",\"value\",\"translations\"],\"additionalProperties\":false}";

    /// <summary>The closed EditorID setter schema.</summary>
    private const string SetEditorIdSchema = "{\"type\":\"object\",\"properties\":{\"editorId\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1048576,\"description\":\"The exact non-whitespace EditorID; the codec does not trim or normalize it.\"}},\"required\":[\"editorId\"],\"additionalProperties\":false}";

    /// <summary>The closed version-control setter schema.</summary>
    private const string SetVersionControlSchema = "{\"type\":\"object\",\"properties\":{\"versionControl\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":4294967295}},\"required\":[\"versionControl\"],\"additionalProperties\":false}";

    /// <summary>The closed form-version setter schema.</summary>
    private const string SetFormVersionSchema = "{\"type\":\"object\",\"properties\":{\"formVersion\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":65535}},\"required\":[\"formVersion\"],\"additionalProperties\":false}";

    /// <summary>The closed secondary-version setter schema.</summary>
    private const string SetVersion2Schema = "{\"type\":\"object\",\"properties\":{\"version2\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":65535}},\"required\":[\"version2\"],\"additionalProperties\":false}";

    /// <summary>The closed compression setter schema.</summary>
    private const string SetCompressedSchema = "{\"type\":\"object\",\"properties\":{\"isCompressed\":{\"type\":\"boolean\"}},\"required\":[\"isCompressed\"],\"additionalProperties\":false}";

    /// <summary>The closed deletion setter schema.</summary>
    private const string SetDeletedSchema = "{\"type\":\"object\",\"properties\":{\"isDeleted\":{\"type\":\"boolean\"}},\"required\":[\"isDeleted\"],\"additionalProperties\":false}";

    /// <summary>The closed complete item replacement schema with an inline record link definition.</summary>
    private static readonly string ReplaceItemsSchema = "{\"type\":\"object\",\"properties\":{\"items\":{\"type\":\"array\",\"maxItems\":65536,\"items\":" + FormLinkSchema + "}},\"required\":[\"items\"],\"additionalProperties\":false}";

    /// <summary>The closed single-item insertion schema with an inline record link definition.</summary>
    private static readonly string InsertItemSchema = "{\"type\":\"object\",\"properties\":{\"index\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":2147483647},\"item\":" + FormLinkSchema + "},\"required\":[\"index\",\"item\"],\"additionalProperties\":false}";

    /// <summary>The closed single-item removal schema.</summary>
    private const string RemoveItemSchema = "{\"type\":\"object\",\"properties\":{\"index\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":2147483647}},\"required\":[\"index\"],\"additionalProperties\":false}";

    /// <summary>The closed ordered item move schema.</summary>
    private const string MoveItemSchema = "{\"type\":\"object\",\"properties\":{\"sourceIndex\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":2147483647},\"destinationIndex\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":2147483647}},\"required\":[\"sourceIndex\",\"destinationIndex\"],\"additionalProperties\":false}";

    /// <summary>The closed translated-name setter schema with an inline translated-string definition.</summary>
    private static readonly string SetNameSchema = "{\"type\":\"object\",\"properties\":{\"name\":" + TranslatedStringSchema + "},\"required\":[\"name\"],\"additionalProperties\":false}";

    /// <summary>The closed AddToList setter schema with an inline non-null record link definition.</summary>
    private static readonly string SetAddToListSchema = "{\"type\":\"object\",\"properties\":{\"formList\":" + NonNullFormLinkSchema + "},\"required\":[\"formList\"],\"additionalProperties\":false}";

    /// <summary>The closed typed Starfield major-record flag setter schema.</summary>
    private static readonly string SetMajorFlagsSchema = "{\"type\":\"object\",\"properties\":{\"majorRecordFlags\":{\"type\":\"integer\",\"minimum\":-2147483648,\"maximum\":2147483647,\"description\":\"The signed Int32 representation of the installed Starfield typed major-record flags. Only installed enum bits are accepted; unrelated raw record bits remain unchanged.\",\"x-record-type\":\"Mutagen.Bethesda.Starfield.StarfieldMajorRecord.StarfieldMajorRecordFlag\",\"x-supported-bit-mask\":\"0x" + unchecked((uint)StarfieldFormListEditWireCodec.SupportedMajorRecordFlagMask).ToString("X8", System.Globalization.CultureInfo.InvariantCulture) + "\"}},\"required\":[\"majorRecordFlags\"],\"additionalProperties\":false}";

    /// <summary>The stable generated component-union schema reference.</summary>
    private static readonly string ComponentSchemaReference = "{\"$ref\":\"" + StarfieldGeneratedRecordFieldSchema.TypeUriPrefix + "record.component\"}";

    /// <summary>The stable generated condition-union schema reference.</summary>
    private static readonly string ConditionSchemaReference = "{\"$ref\":\"" + StarfieldGeneratedRecordFieldSchema.TypeUriPrefix + "record.condition\"}";

    /// <summary>The closed indexed component insertion schema.</summary>
    private static readonly string AddComponentSchema = "{\"type\":\"object\",\"properties\":{\"index\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":2147483647,\"description\":\"The exact insertion position; the current component count performs an append.\"},\"component\":" + ComponentSchemaReference + "},\"required\":[\"index\",\"component\"],\"additionalProperties\":false}";

    /// <summary>The closed indexed component replacement schema.</summary>
    private static readonly string ReplaceComponentSchema = "{\"type\":\"object\",\"properties\":{\"index\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":2147483647},\"component\":" + ComponentSchemaReference + "},\"required\":[\"index\",\"component\"],\"additionalProperties\":false}";

    /// <summary>The closed complete component replacement schema.</summary>
    private static readonly string ReplaceComponentsSchema = "{\"type\":\"object\",\"properties\":{\"components\":{\"type\":\"array\",\"maxItems\":65536,\"items\":" + ComponentSchemaReference + "}},\"required\":[\"components\"],\"additionalProperties\":false}";

    /// <summary>The closed complete conditional-entry replacement schema.</summary>
    private static readonly string SetConditionalEntriesSchema = "{\"type\":\"object\",\"properties\":{\"conditionalEntries\":{\"type\":\"array\",\"maxItems\":65536,\"items\":{\"type\":\"object\",\"properties\":{\"Index\":{\"type\":[\"integer\",\"null\"],\"minimum\":0,\"maximum\":4294967295},\"Conditions\":{\"oneOf\":[{\"type\":\"null\"},{\"type\":\"array\",\"maxItems\":65536,\"items\":" + ConditionSchemaReference + "}]}},\"required\":[\"Index\",\"Conditions\"],\"additionalProperties\":false}}},\"required\":[\"conditionalEntries\"],\"additionalProperties\":false}";

    /// <summary>The Mutagen default for an explicit null form link.</summary>
    private static readonly string NullFormLinkDefault = "{\"isNull\":true,\"formKey\":" + JsonSerializer.Serialize(FormKey.Null.ToString()) + "}";

    /// <summary>The Mutagen default for an empty English-targeted translated string.</summary>
    private const string EmptyTranslatedStringDefault = "{\"targetLanguage\":\"English\",\"value\":null,\"translations\":[]}";

    /// <summary>Every command node in ordinal name order followed by command-specific helper type nodes.</summary>
    private static readonly IReadOnlyList<(
        RecordWireSchemaNodeKind Kind,
        string Name,
        string SchemaJson,
        string? DefaultJson)> LocalDefinitions = Array.AsReadOnly(new[]
    {
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.ClearEditorIdCommand, EmptyArgumentsSchema, "{}"),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.ClearItemsCommand, EmptyArgumentsSchema, "{}"),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.InsertItemCommand, InsertItemSchema, (string?)null),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.MoveItemCommand, MoveItemSchema, (string?)null),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.RemoveItemCommand, RemoveItemSchema, (string?)null),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.ReplaceItemsCommand, ReplaceItemsSchema, "{\"items\":[]}"),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.SetCompressedCommand, SetCompressedSchema, "{\"isCompressed\":false}"),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.SetDeletedCommand, SetDeletedSchema, "{\"isDeleted\":false}"),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.SetEditorIdCommand, SetEditorIdSchema, (string?)null),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.SetFormVersionCommand, SetFormVersionSchema, "{\"formVersion\":131}"),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.SetVersion2Command, SetVersion2Schema, "{\"version2\":0}"),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.SetVersionControlCommand, SetVersionControlSchema, "{\"versionControl\":0}"),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.AddComponentCommand, AddComponentSchema, (string?)null),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.ClearAddToListCommand, EmptyArgumentsSchema, "{}"),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.ClearConditionalEntriesCommand, EmptyArgumentsSchema, "{}"),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.ClearNameCommand, EmptyArgumentsSchema, "{}"),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.RemoveComponentCommand, RemoveItemSchema, (string?)null),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.ReplaceComponentCommand, ReplaceComponentSchema, (string?)null),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.ReplaceComponentsCommand, ReplaceComponentsSchema, "{\"components\":[]}"),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.SetAddToListCommand, SetAddToListSchema, (string?)null),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.SetConditionalEntriesCommand, SetConditionalEntriesSchema, "{\"conditionalEntries\":[]}"),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.SetMajorFlagsCommand, SetMajorFlagsSchema, "{\"majorRecordFlags\":0}"),
        (RecordWireSchemaNodeKind.Command, StarfieldFormListEditWireCodec.SetNameCommand, SetNameSchema, (string?)null),
        (RecordWireSchemaNodeKind.Type, FormLinkTypeName, FormLinkSchema, NullFormLinkDefault),
        (RecordWireSchemaNodeKind.Type, TranslatedStringTypeName, TranslatedStringSchema, EmptyTranslatedStringDefault),
    });

    /// <summary>Every command node followed by every generated Mutagen type node in deterministic order.</summary>
    private static readonly IReadOnlyList<(
        RecordWireSchemaNodeKind Kind,
        string Name,
        string SchemaJson,
        string? DefaultJson)> Definitions = CreateDefinitions();

    /// <summary>The canonical SHA-256 identity of the versioned ordered catalog content.</summary>
    private static readonly string CatalogId = CreateCatalogId();

    /// <summary>The immutable exact game, release, version, and catalog content identity.</summary>
    private static readonly RecordWireSchemaCatalogIdentity CatalogIdentity = new(
        SupportedGame.Starfield,
        GameRelease.Starfield,
        SchemaVersion,
        CatalogId);

    /// <summary>The immutable content-bound node keys in deterministic command-first order.</summary>
    private static readonly IReadOnlyList<RecordWireSchemaNodeKey> CatalogNodes = Array.AsReadOnly(
        Definitions.Select(definition => new RecordWireSchemaNodeKey(
            CatalogId,
            definition.Kind,
            definition.Name)).ToArray());

    /// <summary>Gets the immutable Starfield schema catalog identity.</summary>
    public RecordWireSchemaCatalogIdentity Identity => CatalogIdentity;

    /// <summary>Gets every content-bound key in ordinal command-first then type order.</summary>
    public IReadOnlyList<RecordWireSchemaNodeKey> Nodes => CatalogNodes;

    /// <summary>Reads a detached command or Mutagen-type schema from this exact catalog identity.</summary>
    /// <param name="key">The content-bound node key to resolve.</param>
    /// <param name="cancellationToken">A token checked before and after detached JSON materialization.</param>
    /// <returns>The detached node, or an invalid-request failure for a stale, foreign, or unknown key.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<RecordWireSchemaNode> ReadNode(
        RecordWireSchemaNodeKey key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        if (!string.Equals(key.CatalogId, CatalogId, StringComparison.Ordinal))
        {
            return EngineResult<RecordWireSchemaNode>.Failure(new EngineError(
                EngineErrorCode.InvalidRequest,
                "The Starfield Mutagen wire schema key belongs to a stale or foreign catalog."));
        }

        foreach (var definition in Definitions)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (definition.Kind != key.Kind
                || !string.Equals(definition.Name, key.Name, StringComparison.Ordinal))
            {
                continue;
            }

            var schema = ParseDetached(definition.SchemaJson);
            JsonElement? defaultTemplate = definition.DefaultJson is null
                ? null
                : ParseDetached(definition.DefaultJson);
            cancellationToken.ThrowIfCancellationRequested();
            return EngineResult<RecordWireSchemaNode>.Success(new RecordWireSchemaNode(
                key,
                schema,
                defaultTemplate));
        }

        return EngineResult<RecordWireSchemaNode>.Failure(new EngineError(
            EngineErrorCode.InvalidRequest,
            $"The Starfield Mutagen wire schema node '{key.Name}' is not present in this catalog."));
    }

    /// <summary>Combines closed command schemas with every generated Mutagen type schema before content hashing.</summary>
    /// <returns>All immutable catalog definitions in deterministic command-first order.</returns>
    private static IReadOnlyList<(
        RecordWireSchemaNodeKind Kind,
        string Name,
        string SchemaJson,
        string? DefaultJson)> CreateDefinitions()
    {
        var definitions = new List<(
            RecordWireSchemaNodeKind Kind,
            string Name,
            string SchemaJson,
            string? DefaultJson)>(
                LocalDefinitions.Count + StarfieldGeneratedRecordFieldSchema.TypeNames.Count);
        definitions.AddRange(LocalDefinitions);
        foreach (var recordTypeName in StarfieldGeneratedRecordFieldSchema.TypeNames)
        {
            definitions.Add((
                RecordWireSchemaNodeKind.Type,
                recordTypeName,
                StarfieldGeneratedRecordFieldSchema.GetSchemaJson(recordTypeName),
                StarfieldGeneratedRecordFieldSchema.CreateDefaultJson(recordTypeName)));
        }

        return Array.AsReadOnly(definitions.ToArray());
    }

    /// <summary>Computes one unambiguous content identity over the exact game, release, version, node order, schemas, and defaults.</summary>
    /// <returns>The uppercase SHA-256 hexadecimal identity.</returns>
    private static string CreateCatalogId()
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            writer.Write((int)SupportedGame.Starfield);
            writer.Write((int)GameRelease.Starfield);
            writer.Write(SchemaVersion);
            writer.Write(Definitions.Count);
            foreach (var definition in Definitions)
            {
                writer.Write((int)definition.Kind);
                writer.Write(definition.Name);
                writer.Write(definition.SchemaJson);
                writer.Write(definition.DefaultJson is not null);
                if (definition.DefaultJson is not null)
                {
                    writer.Write(definition.DefaultJson);
                }
            }

            writer.Flush();
        }

        return Convert.ToHexString(SHA256.HashData(stream.GetBuffer().AsSpan(0, checked((int)stream.Length))));
    }

    /// <summary>Parses one static trusted catalog JSON value into a detached request-independent element.</summary>
    /// <param name="json">The complete static JSON representation.</param>
    /// <returns>A detached JSON value.</returns>
    private static JsonElement ParseDetached(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}
