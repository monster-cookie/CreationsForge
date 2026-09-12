using System.Security.Cryptography;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Skyrim.Native.Wire;

/// <summary>Exposes the immutable closed command and native-type schemas for Skyrim Special Edition FormList authoring.</summary>
public sealed class SkyrimFormListEditWireSchemaCatalog : IFormListEditWireSchemaCatalog
{
    /// <summary>The semantic version of the first closed Skyrim FormList wire schema.</summary>
    private const string SchemaVersion = "1.0.0";

    /// <summary>The command-first, type-second source definitions used to construct the immutable catalog.</summary>
    private static readonly IReadOnlyList<CatalogDefinition> Definitions = CreateDefinitions();

    /// <summary>The exact game, release, version, and canonical content hash.</summary>
    private static readonly NativeWireSchemaCatalogIdentity CatalogIdentity = CreateIdentity();

    /// <summary>The immutable materialized catalog nodes.</summary>
    private static readonly IReadOnlyList<NativeWireSchemaNode> CatalogNodes = CreateNodes();

    /// <summary>The immutable ordered catalog key projection.</summary>
    private static readonly IReadOnlyList<NativeWireSchemaNodeKey> CatalogKeys = Array.AsReadOnly(
        CatalogNodes.Select(node => node.Key).ToArray());

    /// <summary>Initializes a stateless view over the immutable compiled Skyrim FormList wire catalog.</summary>
    public SkyrimFormListEditWireSchemaCatalog()
    { }

    /// <summary>Gets the exact Skyrim game, release, schema version, and content identity.</summary>
    public NativeWireSchemaCatalogIdentity Identity => CatalogIdentity;

    /// <summary>Gets every schema key in deterministic command-first, type-second order.</summary>
    public IReadOnlyList<NativeWireSchemaNodeKey> Nodes => CatalogKeys;

    /// <summary>Reads one detached node from this exact immutable catalog.</summary>
    /// <param name="key">The content-bound command or type key obtained from <see cref="Nodes"/>.</param>
    /// <param name="cancellationToken">A token checked before and after detached JSON materialization.</param>
    /// <returns>The detached node, or a typed invalid-request failure for a stale, foreign, or unknown key.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public EngineResult<NativeWireSchemaNode> ReadNode(
        NativeWireSchemaNodeKey key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        cancellationToken.ThrowIfCancellationRequested();
        if (!string.Equals(key.CatalogId, Identity.CatalogId, StringComparison.Ordinal))
        {
            return Failure("The Skyrim FormList wire schema key belongs to a stale or foreign catalog.");
        }

        var node = CatalogNodes.FirstOrDefault(candidate =>
            candidate.Key.Kind == key.Kind &&
            string.Equals(candidate.Key.Name, key.Name, StringComparison.Ordinal));
        if (node is null)
        {
            return Failure($"The Skyrim FormList wire schema node '{key.Kind}/{key.Name}' is unknown.");
        }

        var detached = new NativeWireSchemaNode(
            new NativeWireSchemaNodeKey(Identity.CatalogId, node.Key.Kind, node.Key.Name),
            node.Schema,
            node.DefaultTemplate);
        cancellationToken.ThrowIfCancellationRequested();
        return EngineResult<NativeWireSchemaNode>.Success(detached);
    }

    /// <summary>Builds the deterministic command-first, type-second schema source definitions.</summary>
    /// <returns>The immutable parsed definitions in ordinal command and then ordinal type order.</returns>
    private static IReadOnlyList<CatalogDefinition> CreateDefinitions()
    {
        return Array.AsReadOnly<CatalogDefinition>(
        [
            Command(
                SkyrimFormListEditWireCodec.ClearEditorIdCommand,
                EmptyObjectSchema,
                "{}"),
            Command(
                SkyrimFormListEditWireCodec.ClearItemsCommand,
                EmptyObjectSchema,
                "{}"),
            Command(
                SkyrimFormListEditWireCodec.InsertItemCommand,
                """
                {"$schema":"https://json-schema.org/draft/2020-12/schema","type":"object","additionalProperties":false,"required":["index","item"],"properties":{"index":{"type":"integer","minimum":0,"maximum":2147483647},"item":{"type":"object","additionalProperties":false,"required":["isNull","formKey"],"properties":{"isNull":{"type":"boolean"},"formKey":{"type":["string","null"],"maxLength":1048576,"description":"A canonical Mutagen FormKey string; explicit null links accept JSON null or the canonical FormKey.Null identity."}},"oneOf":[{"properties":{"isNull":{"const":true},"formKey":{"oneOf":[{"type":"null"},{"const":"Null"}]}}},{"properties":{"isNull":{"const":false},"formKey":{"type":"string","minLength":1}}}]}}}
                """),
            Command(
                SkyrimFormListEditWireCodec.MoveItemCommand,
                """
                {"$schema":"https://json-schema.org/draft/2020-12/schema","type":"object","additionalProperties":false,"required":["sourceIndex","destinationIndex"],"properties":{"sourceIndex":{"type":"integer","minimum":0,"maximum":2147483647},"destinationIndex":{"type":"integer","minimum":0,"maximum":2147483647}}}
                """),
            Command(
                SkyrimFormListEditWireCodec.RemoveItemCommand,
                IndexObjectSchema),
            Command(
                SkyrimFormListEditWireCodec.ReplaceItemsCommand,
                """
                {"$schema":"https://json-schema.org/draft/2020-12/schema","type":"object","additionalProperties":false,"required":["items"],"properties":{"items":{"type":"array","maxItems":65536,"items":{"type":"object","additionalProperties":false,"required":["isNull","formKey"],"properties":{"isNull":{"type":"boolean"},"formKey":{"type":["string","null"],"maxLength":1048576,"description":"A canonical Mutagen FormKey string; explicit null links accept JSON null or the canonical FormKey.Null identity."}},"oneOf":[{"properties":{"isNull":{"const":true},"formKey":{"oneOf":[{"type":"null"},{"const":"Null"}]}}},{"properties":{"isNull":{"const":false},"formKey":{"type":"string","minLength":1}}}]}}}}
                """,
                "{\"items\":[]}"),
            Command(
                SkyrimFormListEditWireCodec.SetCompressedCommand,
                """
                {"$schema":"https://json-schema.org/draft/2020-12/schema","type":"object","additionalProperties":false,"required":["isCompressed"],"properties":{"isCompressed":{"type":"boolean"}}}
                """,
                "{\"isCompressed\":false}"),
            Command(
                SkyrimFormListEditWireCodec.SetDeletedCommand,
                """
                {"$schema":"https://json-schema.org/draft/2020-12/schema","type":"object","additionalProperties":false,"required":["isDeleted"],"properties":{"isDeleted":{"type":"boolean"}}}
                """,
                "{\"isDeleted\":false}"),
            Command(
                SkyrimFormListEditWireCodec.SetEditorIdCommand,
                """
                {"$schema":"https://json-schema.org/draft/2020-12/schema","type":"object","additionalProperties":false,"required":["editorId"],"properties":{"editorId":{"type":"string","minLength":1,"maxLength":1048576,"pattern":"\\S","description":"The non-empty, non-whitespace Skyrim EditorID to assign without normalization."}}}
                """),
            Command(
                SkyrimFormListEditWireCodec.SetFormVersionCommand,
                """
                {"$schema":"https://json-schema.org/draft/2020-12/schema","type":"object","additionalProperties":false,"required":["formVersion"],"properties":{"formVersion":{"type":"integer","minimum":0,"maximum":65535}}}
                """,
                "{\"formVersion\":44}"),
            Command(
                SkyrimFormListEditWireCodec.SetVersion2Command,
                """
                {"$schema":"https://json-schema.org/draft/2020-12/schema","type":"object","additionalProperties":false,"required":["version2"],"properties":{"version2":{"type":"integer","minimum":0,"maximum":65535}}}
                """,
                "{\"version2\":0}"),
            Command(
                SkyrimFormListEditWireCodec.SetVersionControlCommand,
                """
                {"$schema":"https://json-schema.org/draft/2020-12/schema","type":"object","additionalProperties":false,"required":["versionControl"],"properties":{"versionControl":{"type":"integer","minimum":0,"maximum":4294967295}}}
                """,
                "{\"versionControl\":0}"),
            Command(
                SkyrimFormListEditWireCodec.SetMajorRecordFlagsCommand,
                """
                {"$schema":"https://json-schema.org/draft/2020-12/schema","type":"object","additionalProperties":false,"required":["majorRecordFlags"],"properties":{"majorRecordFlags":{"type":"integer","minimum":-2147483648,"maximum":2147483647,"description":"The signed Int32 representation of the installed Skyrim typed major-record flags. Only bits in 0x000E9825 are accepted; unrelated raw record bits remain unchanged.","x-native-type":"Mutagen.Bethesda.Skyrim.SkyrimMajorRecord.SkyrimMajorRecordFlag","x-supported-bit-mask":"0x000E9825"}}}
                """,
                "{\"majorRecordFlags\":0}"),
            Type(
                "native.form-link",
                FormLinkSchema,
                "{\"isNull\":true,\"formKey\":\"Null\"}"),
            Type(
                "skyrim.form-list",
                """
                {"$schema":"https://json-schema.org/draft/2020-12/schema","type":"object","additionalProperties":false,"required":["MajorRecordFlagsRaw","FormKey","VersionControl","EditorID","FormVersion","Version2","SkyrimMajorRecordFlags","Items"],"properties":{"MajorRecordFlagsRaw":{"type":"integer","minimum":-2147483648,"maximum":2147483647,"readOnly":true,"description":"The exact raw native header bits; typed edit commands preserve unrepresented bits."},"FormKey":{"type":"string","minLength":1,"maxLength":1048576,"readOnly":true,"description":"The immutable canonical Mutagen record identity."},"VersionControl":{"type":"integer","minimum":0,"maximum":4294967295},"EditorID":{"type":["string","null"],"maxLength":1048576},"FormVersion":{"type":"integer","minimum":0,"maximum":65535},"Version2":{"type":"integer","minimum":0,"maximum":65535},"SkyrimMajorRecordFlags":{"type":"integer","minimum":-2147483648,"maximum":2147483647,"description":"The installed signed SkyrimMajorRecordFlag enum value."},"Items":{"type":"array","maxItems":65536,"items":{"type":"object","additionalProperties":false,"required":["isNull","formKey"],"properties":{"isNull":{"type":"boolean"},"formKey":{"type":["string","null"],"maxLength":1048576,"description":"A canonical Mutagen FormKey string; explicit null links accept JSON null or the canonical FormKey.Null identity."}},"oneOf":[{"properties":{"isNull":{"const":true},"formKey":{"oneOf":[{"type":"null"},{"const":"Null"}]}}},{"properties":{"isNull":{"const":false},"formKey":{"type":"string","minLength":1}}}]}}}}
                """)
        ]);
    }

    /// <summary>The closed JSON Schema for a command whose arguments must be an empty object.</summary>
    private const string EmptyObjectSchema =
        "{\"$schema\":\"https://json-schema.org/draft/2020-12/schema\",\"type\":\"object\",\"additionalProperties\":false,\"properties\":{}}";

    /// <summary>The closed JSON Schema for one non-negative item index.</summary>
    private const string IndexObjectSchema =
        "{\"$schema\":\"https://json-schema.org/draft/2020-12/schema\",\"type\":\"object\",\"additionalProperties\":false,\"required\":[\"index\"],\"properties\":{\"index\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":2147483647}}}";

    /// <summary>The reusable closed JSON Schema for the readable native FormLink representation.</summary>
    private const string FormLinkSchema =
        "{\"$schema\":\"https://json-schema.org/draft/2020-12/schema\",\"type\":\"object\",\"additionalProperties\":false,\"required\":[\"isNull\",\"formKey\"],\"properties\":{\"isNull\":{\"type\":\"boolean\"},\"formKey\":{\"type\":[\"string\",\"null\"],\"maxLength\":1048576,\"description\":\"A canonical Mutagen FormKey string; explicit null links accept JSON null or the canonical FormKey.Null identity.\"}},\"oneOf\":[{\"properties\":{\"isNull\":{\"const\":true},\"formKey\":{\"oneOf\":[{\"type\":\"null\"},{\"const\":\"Null\"}]}}},{\"properties\":{\"isNull\":{\"const\":false},\"formKey\":{\"type\":\"string\",\"minLength\":1}}}]}";

    /// <summary>Creates one command definition from its exact closed argument schema and optional default.</summary>
    /// <param name="name">The exact stable command discriminator.</param>
    /// <param name="schema">The complete closed JSON Schema object.</param>
    /// <param name="defaultTemplate">The optional syntax-valid command argument default.</param>
    /// <returns>The immutable parsed command definition.</returns>
    private static CatalogDefinition Command(
        string name,
        string schema,
        string? defaultTemplate = null)
    {
        return new CatalogDefinition(NativeWireSchemaNodeKind.Command, name, schema, defaultTemplate);
    }

    /// <summary>Creates one concrete native-type definition from its exact schema and optional constructible default.</summary>
    /// <param name="name">The stable concrete native type identity.</param>
    /// <param name="schema">The complete closed JSON Schema object.</param>
    /// <param name="defaultTemplate">The optional syntax-valid native-constructible default.</param>
    /// <returns>The immutable parsed native-type definition.</returns>
    private static CatalogDefinition Type(
        string name,
        string schema,
        string? defaultTemplate = null)
    {
        return new CatalogDefinition(NativeWireSchemaNodeKind.Type, name, schema, defaultTemplate);
    }

    /// <summary>Computes the catalog identity from deterministic canonical JSON over game, release, version, and ordered definitions.</summary>
    /// <returns>The immutable exact identity containing the uppercase SHA-256 catalog hash.</returns>
    private static NativeWireSchemaCatalogIdentity CreateIdentity()
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteString("game", SupportedGame.Skyrim.ToString());
            writer.WriteString("release", GameRelease.SkyrimSE.ToString());
            writer.WriteString("schemaVersion", SchemaVersion);
            writer.WriteStartArray("nodes");
            foreach (var definition in Definitions)
            {
                writer.WriteStartObject();
                writer.WriteString("kind", definition.Kind.ToString());
                writer.WriteString("name", definition.Name);
                writer.WritePropertyName("schema");
                definition.Schema.WriteTo(writer);
                writer.WritePropertyName("defaultTemplate");
                if (definition.DefaultTemplate.HasValue)
                {
                    definition.DefaultTemplate.Value.WriteTo(writer);
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

        var catalogId = Convert.ToHexString(SHA256.HashData(stream.ToArray()));
        return new NativeWireSchemaCatalogIdentity(
            SupportedGame.Skyrim,
            GameRelease.SkyrimSE,
            SchemaVersion,
            catalogId);
    }

    /// <summary>Materializes immutable nodes whose keys are bound to the computed catalog identity.</summary>
    /// <returns>The immutable ordered materialized schema nodes.</returns>
    private static IReadOnlyList<NativeWireSchemaNode> CreateNodes()
    {
        return Array.AsReadOnly(Definitions.Select(definition =>
            new NativeWireSchemaNode(
                new NativeWireSchemaNodeKey(CatalogIdentity.CatalogId, definition.Kind, definition.Name),
                definition.Schema,
                definition.DefaultTemplate)).ToArray());
    }

    /// <summary>Creates a typed invalid-request result for stale, foreign, or unknown schema keys.</summary>
    /// <param name="message">The non-empty catalog lookup diagnostic.</param>
    /// <returns>A failed result without a schema node.</returns>
    private static EngineResult<NativeWireSchemaNode> Failure(string message)
    {
        return EngineResult<NativeWireSchemaNode>.Failure(
            new EngineError(EngineErrorCode.InvalidRequest, message));
    }

    /// <summary>Stores one parsed immutable catalog definition before its key is bound to the complete catalog hash.</summary>
    private sealed class CatalogDefinition
    {
        /// <summary>Initializes one parsed catalog definition and detaches its JSON values.</summary>
        /// <param name="kind">The command or concrete-type role.</param>
        /// <param name="name">The stable command discriminator or native type name.</param>
        /// <param name="schema">The exact closed JSON Schema object.</param>
        /// <param name="defaultTemplate">The optional syntax-valid native-constructible default JSON.</param>
        internal CatalogDefinition(
            NativeWireSchemaNodeKind kind,
            string name,
            string schema,
            string? defaultTemplate)
        {
            Kind = kind;
            Name = name;
            Schema = Parse(schema, JsonValueKind.Object, nameof(schema));
            DefaultTemplate = defaultTemplate is null
                ? null
                : Parse(defaultTemplate, expectedKind: null, nameof(defaultTemplate));
        }

        /// <summary>Gets whether the definition describes a command or concrete native type.</summary>
        internal NativeWireSchemaNodeKind Kind { get; }

        /// <summary>Gets the stable command discriminator or native type name.</summary>
        internal string Name { get; }

        /// <summary>Gets the detached complete JSON Schema object.</summary>
        internal JsonElement Schema { get; }

        /// <summary>Gets the detached syntax-valid default JSON, or <see langword="null"/> when unavailable.</summary>
        internal JsonElement? DefaultTemplate { get; }

        /// <summary>Parses and detaches one catalog JSON value while optionally enforcing its root kind.</summary>
        private static JsonElement Parse(
            string json,
            JsonValueKind? expectedKind,
            string parameterName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(json);
            using var document = JsonDocument.Parse(json);
            if (expectedKind.HasValue && document.RootElement.ValueKind != expectedKind.Value)
            {
                throw new ArgumentException(
                    $"Catalog JSON must contain a {expectedKind.Value} root.",
                    parameterName);
            }

            return document.RootElement.Clone();
        }
    }
}
