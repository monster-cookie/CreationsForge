using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Fallout4.Native.Wire;

/// <summary>Publishes the immutable content-bound Fallout 4 FormList command and native leaf schemas.</summary>
public sealed class Fallout4FormListEditWireSchemaCatalog : IFormListEditWireSchemaCatalog
{
    /// <summary>The first stable Fallout 4 FormList wire schema version.</summary>
    private const string SchemaVersion = "1.0.0";

    /// <summary>The stable inspector-compatible native form-link type key.</summary>
    internal const string FormLinkTypeName = "native.form-link";

    /// <summary>The stable inspector-compatible translated-string type key.</summary>
    internal const string TranslatedStringTypeName = "native.translated-string";

    /// <summary>The closed schema shared by every no-argument command.</summary>
    private const string EmptyArgumentsSchema = "{\"type\":\"object\",\"description\":\"This command has no arguments.\",\"properties\":{},\"required\":[],\"additionalProperties\":false}";

    /// <summary>The closed schema for one exact inspector-compatible native form link.</summary>
    private const string FormLinkSchema = "{\"type\":\"object\",\"description\":\"One present native FormList link. Native null links require isNull true and either JSON null or canonical FormKey.Null; non-null links require isNull false and a canonical non-null Mutagen FormKey string. Whole JSON null represents an absent nullable link rather than a native null link.\",\"properties\":{\"isNull\":{\"type\":\"boolean\"},\"formKey\":{\"type\":[\"string\",\"null\"],\"maxLength\":1048576}},\"required\":[\"isNull\",\"formKey\"],\"additionalProperties\":false}";

    /// <summary>The closed schema for one exact inspector-compatible translated string.</summary>
    private const string TranslatedStringSchema = "{\"type\":\"object\",\"description\":\"One native translated string. The value must exactly match the target-language entry, or be null when that entry is absent. Languages use exact installed Mutagen names and may occur only once.\",\"properties\":{\"targetLanguage\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1048576},\"value\":{\"type\":[\"string\",\"null\"],\"maxLength\":1048576},\"translations\":{\"type\":\"array\",\"maxItems\":65536,\"x-unique-property\":\"language\",\"items\":{\"type\":\"object\",\"properties\":{\"language\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1048576},\"value\":{\"type\":\"string\",\"maxLength\":1048576}},\"required\":[\"language\",\"value\"],\"additionalProperties\":false}}},\"required\":[\"targetLanguage\",\"value\",\"translations\"],\"additionalProperties\":false}";

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

    /// <summary>The closed complete item replacement schema with an inline native link definition.</summary>
    private static readonly string ReplaceItemsSchema = "{\"type\":\"object\",\"properties\":{\"items\":{\"type\":\"array\",\"maxItems\":65536,\"items\":" + FormLinkSchema + "}},\"required\":[\"items\"],\"additionalProperties\":false}";

    /// <summary>The closed single-item insertion schema with an inline native link definition.</summary>
    private static readonly string InsertItemSchema = "{\"type\":\"object\",\"properties\":{\"index\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":2147483647},\"item\":" + FormLinkSchema + "},\"required\":[\"index\",\"item\"],\"additionalProperties\":false}";

    /// <summary>The closed single-item removal schema.</summary>
    private const string RemoveItemSchema = "{\"type\":\"object\",\"properties\":{\"index\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":2147483647}},\"required\":[\"index\"],\"additionalProperties\":false}";

    /// <summary>The closed ordered item move schema.</summary>
    private const string MoveItemSchema = "{\"type\":\"object\",\"properties\":{\"sourceIndex\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":2147483647},\"destinationIndex\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":2147483647}},\"required\":[\"sourceIndex\",\"destinationIndex\"],\"additionalProperties\":false}";

    /// <summary>The closed translated-name setter schema with an inline translated-string definition.</summary>
    private static readonly string SetNameSchema = "{\"type\":\"object\",\"properties\":{\"name\":" + TranslatedStringSchema + "},\"required\":[\"name\"],\"additionalProperties\":false}";

    /// <summary>The closed typed Fallout 4 major-record flag setter schema.</summary>
    private const string SetMajorRecordFlagsSchema = "{\"type\":\"object\",\"properties\":{\"majorRecordFlags\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":2147483647,\"description\":\"The signed Int32 representation of the installed Fallout 4 typed major-record flags. Only bits in 0x000E9825 are accepted; unrelated raw record bits remain unchanged.\",\"x-native-type\":\"Mutagen.Bethesda.Fallout4.Fallout4MajorRecord.Fallout4MajorRecordFlag\",\"x-supported-bit-mask\":\"0x000E9825\"}},\"required\":[\"majorRecordFlags\"],\"additionalProperties\":false}";

    /// <summary>The native default for an explicit null form link.</summary>
    private const string NullFormLinkDefault = "{\"isNull\":true,\"formKey\":\"Null\"}";

    /// <summary>The native default for an empty English-targeted translated string.</summary>
    private const string EmptyTranslatedStringDefault = "{\"targetLanguage\":\"English\",\"value\":null,\"translations\":[]}";

    /// <summary>Every command node followed by every native type node in ordinal name order.</summary>
    private static readonly IReadOnlyList<(
        NativeWireSchemaNodeKind Kind,
        string Name,
        string SchemaJson,
        string? DefaultJson)> Definitions = Array.AsReadOnly(new[]
    {
        (NativeWireSchemaNodeKind.Command, Fallout4FormListEditWireCodec.ClearNameCommand, EmptyArgumentsSchema, "{}"),
        (NativeWireSchemaNodeKind.Command, Fallout4FormListEditWireCodec.SetMajorRecordFlagsCommand, SetMajorRecordFlagsSchema, "{\"majorRecordFlags\":0}"),
        (NativeWireSchemaNodeKind.Command, Fallout4FormListEditWireCodec.SetNameCommand, SetNameSchema, (string?)null),
        (NativeWireSchemaNodeKind.Command, Fallout4FormListEditWireCodec.ClearEditorIdCommand, EmptyArgumentsSchema, "{}"),
        (NativeWireSchemaNodeKind.Command, Fallout4FormListEditWireCodec.ClearItemsCommand, EmptyArgumentsSchema, "{}"),
        (NativeWireSchemaNodeKind.Command, Fallout4FormListEditWireCodec.InsertItemCommand, InsertItemSchema, (string?)null),
        (NativeWireSchemaNodeKind.Command, Fallout4FormListEditWireCodec.MoveItemCommand, MoveItemSchema, (string?)null),
        (NativeWireSchemaNodeKind.Command, Fallout4FormListEditWireCodec.RemoveItemCommand, RemoveItemSchema, (string?)null),
        (NativeWireSchemaNodeKind.Command, Fallout4FormListEditWireCodec.ReplaceItemsCommand, ReplaceItemsSchema, "{\"items\":[]}"),
        (NativeWireSchemaNodeKind.Command, Fallout4FormListEditWireCodec.SetCompressedCommand, SetCompressedSchema, "{\"isCompressed\":false}"),
        (NativeWireSchemaNodeKind.Command, Fallout4FormListEditWireCodec.SetDeletedCommand, SetDeletedSchema, "{\"isDeleted\":false}"),
        (NativeWireSchemaNodeKind.Command, Fallout4FormListEditWireCodec.SetEditorIdCommand, SetEditorIdSchema, (string?)null),
        (NativeWireSchemaNodeKind.Command, Fallout4FormListEditWireCodec.SetFormVersionCommand, SetFormVersionSchema, "{\"formVersion\":131}"),
        (NativeWireSchemaNodeKind.Command, Fallout4FormListEditWireCodec.SetVersion2Command, SetVersion2Schema, "{\"version2\":0}"),
        (NativeWireSchemaNodeKind.Command, Fallout4FormListEditWireCodec.SetVersionControlCommand, SetVersionControlSchema, "{\"versionControl\":0}"),
        (NativeWireSchemaNodeKind.Type, FormLinkTypeName, FormLinkSchema, NullFormLinkDefault),
        (NativeWireSchemaNodeKind.Type, TranslatedStringTypeName, TranslatedStringSchema, EmptyTranslatedStringDefault),
    });

    /// <summary>The canonical SHA-256 identity of the versioned ordered catalog content.</summary>
    private static readonly string CatalogId = CreateCatalogId();

    /// <summary>The immutable exact game, release, version, and catalog content identity.</summary>
    private static readonly NativeWireSchemaCatalogIdentity CatalogIdentity = new(
        SupportedGame.Fallout4,
        GameRelease.Fallout4,
        SchemaVersion,
        CatalogId);

    /// <summary>The immutable content-bound node keys in deterministic command-first order.</summary>
    private static readonly IReadOnlyList<NativeWireSchemaNodeKey> CatalogNodes = Array.AsReadOnly(
        Definitions.Select(definition => new NativeWireSchemaNodeKey(
            CatalogId,
            definition.Kind,
            definition.Name)).ToArray());

    /// <summary>Gets the immutable Fallout 4 schema catalog identity.</summary>
    public NativeWireSchemaCatalogIdentity Identity => CatalogIdentity;

    /// <summary>Gets every content-bound key in ordinal command-first then type order.</summary>
    public IReadOnlyList<NativeWireSchemaNodeKey> Nodes => CatalogNodes;

    /// <summary>Reads a detached command or native-type schema from this exact catalog identity.</summary>
    /// <param name="key">The content-bound node key to resolve.</param>
    /// <param name="cancellationToken">A token checked before and after detached JSON materialization.</param>
    /// <returns>The detached node, or an invalid-request failure for a stale, foreign, or unknown key.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<NativeWireSchemaNode> ReadNode(
        NativeWireSchemaNodeKey key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        if (!string.Equals(key.CatalogId, CatalogId, StringComparison.Ordinal))
        {
            return EngineResult<NativeWireSchemaNode>.Failure(new EngineError(
                EngineErrorCode.InvalidRequest,
                "The Fallout 4 native wire schema key belongs to a stale or foreign catalog."));
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
            return EngineResult<NativeWireSchemaNode>.Success(new NativeWireSchemaNode(
                key,
                schema,
                defaultTemplate));
        }

        return EngineResult<NativeWireSchemaNode>.Failure(new EngineError(
            EngineErrorCode.InvalidRequest,
            $"The Fallout 4 native wire schema node '{key.Name}' is not present in this catalog."));
    }

    /// <summary>Computes one unambiguous content identity over the exact game, release, version, node order, schemas, and defaults.</summary>
    /// <returns>The uppercase SHA-256 hexadecimal identity.</returns>
    private static string CreateCatalogId()
    {
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            writer.Write((int)SupportedGame.Fallout4);
            writer.Write((int)GameRelease.Fallout4);
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
