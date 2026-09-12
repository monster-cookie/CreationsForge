using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;

namespace CreationsForge.NativeEditing.Schema;

/// <summary>Parses the current closed native wire schema vocabulary into typed presentation descriptors.</summary>
internal static class NativeWireSchemaParser
{
    /// <summary>Resolves one schema node into a presentation descriptor.</summary>
    internal static EngineResult<NativeWireSchemaDescriptor> Resolve(IFormListEditWireSchemaCatalog catalog, NativeWireSchemaNodeKey key, NativeWireReadLimits limits, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(limits);
        cancellationToken.ThrowIfCancellationRequested();
        if (!string.Equals(key.CatalogId, catalog.Identity.CatalogId, StringComparison.Ordinal))
        {
            return Failure($"Schema node '{key.Name}' belongs to catalog {key.CatalogId}, not active catalog {catalog.Identity.CatalogId}.");
        }

        var node = catalog.ReadNode(key, cancellationToken);
        if (!node.Succeeded || node.Value is null)
        {
            return EngineResult<NativeWireSchemaDescriptor>.Failure(node.Error ?? new EngineError(EngineErrorCode.ValidationFailed, $"Schema node '{key.Name}' could not be read."));
        }

        try
        {
            var state = new ParseState(catalog, limits, cancellationToken);
            var descriptor = state.Parse(node.Value.Schema, key.Name, "$schema", key, node.Value.DefaultTemplate, new HashSet<string>(StringComparer.Ordinal) { key.Name });
            cancellationToken.ThrowIfCancellationRequested();
            return EngineResult<NativeWireSchemaDescriptor>.Success(descriptor);
        }
        catch (SchemaParseException exception)
        {
            return Failure(exception.Message);
        }
    }

    /// <summary>Creates one typed schema parsing failure.</summary>
    private static EngineResult<NativeWireSchemaDescriptor> Failure(string message)
    {
        return EngineResult<NativeWireSchemaDescriptor>.Failure(new EngineError(EngineErrorCode.ValidationFailed, message));
    }

    /// <summary>Tracks bounded schema traversal for one descriptor resolution.</summary>
    private sealed class ParseState
    {
        /// <summary>The exact source catalog.</summary>
        private readonly IFormListEditWireSchemaCatalog Catalog;

        /// <summary>The operation limits.</summary>
        private readonly NativeWireReadLimits Limits;

        /// <summary>The operation cancellation token.</summary>
        private readonly CancellationToken CancellationToken;

        /// <summary>The number of schema values visited.</summary>
        private int VisitedNodes;

        /// <summary>Initializes one bounded parser state.</summary>
        internal ParseState(IFormListEditWireSchemaCatalog catalog, NativeWireReadLimits limits, CancellationToken cancellationToken)
        {
            Catalog = catalog;
            Limits = limits;
            CancellationToken = cancellationToken;
        }

        /// <summary>Parses one schema fragment into its closed presentation descriptor.</summary>
        internal NativeWireSchemaDescriptor Parse(JsonElement schema, string fallbackTitle, string path, NativeWireSchemaNodeKey? sourceKey, JsonElement? defaultTemplate, HashSet<string> referenceStack, int depth = 1)
        {
            Visit(path, depth);
            if (schema.ValueKind != JsonValueKind.Object)
            {
                throw new SchemaParseException($"{path}: native wire schema fragments must be JSON objects.");
            }

            if (schema.TryGetProperty("$ref", out var referenceElement))
            {
                if (referenceElement.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(referenceElement.GetString()))
                {
                    throw new SchemaParseException($"{path}.$ref: schema reference must be a non-empty string.");
                }

                var reference = referenceElement.GetString()!;
                var targetKey = ResolveReferenceKey(reference, path);
                if (!referenceStack.Add(targetKey.Name))
                {
                    throw new SchemaParseException($"{path}.$ref: schema reference cycle detected at '{targetKey.Name}'.");
                }

                var target = Catalog.ReadNode(targetKey, CancellationToken);
                if (!target.Succeeded || target.Value is null)
                {
                    throw new SchemaParseException(target.Error?.Message ?? $"{path}.$ref: schema node '{targetKey.Name}' could not be read.");
                }

                var merged = MergeReferencedSchema(target.Value.Schema, schema);
                var parsed = Parse(merged, ReadTitle(schema, targetKey.Name), path, targetKey, target.Value.DefaultTemplate, referenceStack, depth + 1);
                referenceStack.Remove(targetKey.Name);
                return parsed;
            }

            var title = ReadTitle(schema, fallbackTitle);
            var description = ReadOptionalString(schema, "description", path);
            var annotations = ReadAnnotations(schema);
            var constant = schema.TryGetProperty("const", out var constantElement) ? constantElement.Clone() : (JsonElement?)null;
            var defaultValue = schema.TryGetProperty("default", out var defaultElement) ? defaultElement.Clone() : (JsonElement?)null;

            var hasPrimaryShape = schema.TryGetProperty("type", out _) || schema.TryGetProperty("properties", out _);
            if (!hasPrimaryShape && (TryReadUnion(schema, "anyOf", out var anyOf) || TryReadUnion(schema, "oneOf", out anyOf)))
            {
                var options = anyOf.EnumerateArray().Select(option => option.Clone()).ToArray();
                var nullOptions = options.Where(IsNullSchema).ToArray();
                if (nullOptions.Length == 1 && options.Length == 2)
                {
                    var nonNull = options.Single(option => !IsNullSchema(option));
                    var nonNullDescriptor = Parse(nonNull, title, path, sourceKey, defaultTemplate, new HashSet<string>(referenceStack, StringComparer.Ordinal), depth + 1);
                    return new NativeWireSchemaDescriptor(
                        NativeWireSchemaValueKind.Nullable,
                        title,
                        description,
                        sourceKey,
                        defaultTemplate,
                        nonNullDescriptor: nonNullDescriptor,
                        constant: constant,
                        defaultValue: defaultValue,
                        annotations: annotations,
                        allowsNull: true);
                }

                var indexedOptions = options.Select((option, index) => CreateUnionOption(option, index, title, path, referenceStack, depth)).ToArray();
                return new NativeWireSchemaDescriptor(
                    NativeWireSchemaValueKind.Union,
                    title,
                    description,
                    sourceKey,
                    defaultTemplate,
                    unionOptions: Array.AsReadOnly(indexedOptions),
                    constant: constant,
                    defaultValue: defaultValue,
                    annotations: annotations,
                    allowsNull: nullOptions.Length != 0);
            }

            var types = ReadTypes(schema, path);
            if (types.Contains("null", StringComparer.Ordinal) && types.Count > 1)
            {
                var nonNullSchema = RemoveNullType(schema, types);
                var nonNullDescriptor = Parse(nonNullSchema, title, path, sourceKey, defaultTemplate, new HashSet<string>(referenceStack, StringComparer.Ordinal), depth + 1);
                return new NativeWireSchemaDescriptor(
                    NativeWireSchemaValueKind.Nullable,
                    title,
                    description,
                    sourceKey,
                    defaultTemplate,
                    nonNullDescriptor: nonNullDescriptor,
                    constant: constant,
                    defaultValue: defaultValue,
                    annotations: annotations,
                    allowsNull: true);
            }

            var type = types.FirstOrDefault() ?? InferType(schema, constant, path);
            return type switch
            {
                "object" => ParseObject(schema, title, description, sourceKey, defaultTemplate, constant, defaultValue, annotations, referenceStack, path, depth),
                "array" => ParseArray(schema, title, description, sourceKey, defaultTemplate, constant, defaultValue, annotations, referenceStack, path, depth),
                "integer" => ParseInteger(schema, title, description, sourceKey, defaultTemplate, constant, defaultValue, annotations, path),
                "string" => ParseString(schema, title, description, sourceKey, defaultTemplate, constant, defaultValue, annotations, path),
                "number" => ParseNumber(schema, title, description, sourceKey, defaultTemplate, constant, defaultValue, annotations),
                "boolean" => new NativeWireSchemaDescriptor(NativeWireSchemaValueKind.Boolean, title, description, sourceKey, defaultTemplate, constant: constant, defaultValue: defaultValue, annotations: annotations),
                "null" => throw new SchemaParseException($"{path}: a standalone null schema is only valid inside a nullable union."),
                _ => throw new SchemaParseException($"{path}: unsupported native wire schema type '{type}'."),
            };
        }

        /// <summary>Parses a closed object and its ordered properties.</summary>
        private NativeWireSchemaDescriptor ParseObject(JsonElement schema, string title, string? description, NativeWireSchemaNodeKey? sourceKey, JsonElement? defaultTemplate, JsonElement? constant, JsonElement? defaultValue, IReadOnlyDictionary<string, string?> annotations, HashSet<string> referenceStack, string path, int depth)
        {
            var required = schema.TryGetProperty("required", out var requiredElement) && requiredElement.ValueKind == JsonValueKind.Array
                ? requiredElement.EnumerateArray().Select(item => item.GetString() ?? string.Empty).ToHashSet(StringComparer.Ordinal)
                : new HashSet<string>(StringComparer.Ordinal);
            var properties = new List<NativeWireSchemaProperty>();
            if (schema.TryGetProperty("properties", out var propertiesElement))
            {
                if (propertiesElement.ValueKind != JsonValueKind.Object)
                {
                    throw new SchemaParseException($"{path}.properties: expected an object.");
                }

                var order = 0;
                foreach (var property in propertiesElement.EnumerateObject())
                {
                    var propertyPath = $"{path}.properties.{property.Name}";
                    var descriptor = Parse(property.Value, property.Name, propertyPath, sourceKey, null, new HashSet<string>(referenceStack, StringComparer.Ordinal), depth + 1);
                    var readOnly = property.Value.TryGetProperty("readOnly", out var readOnlyElement) && readOnlyElement.ValueKind == JsonValueKind.True;
                    var optionalDerived = readOnly && !property.Value.TryGetProperty("const", out _) &&
                        (property.Value.TryGetProperty("x-native-derived", out _) || property.Value.TryGetProperty("x-native-derived-value", out _) || property.Name.StartsWith("uses", StringComparison.Ordinal));
                    properties.Add(new NativeWireSchemaProperty(property.Name, order++, required.Contains(property.Name) && !optionalDerived, readOnly, descriptor));
                }
            }

            var kind = ClassifyObject(schema, properties, annotations);
            return new NativeWireSchemaDescriptor(kind, title, description, sourceKey, defaultTemplate, Array.AsReadOnly(properties.ToArray()), constant: constant, defaultValue: defaultValue, annotations: annotations);
        }

        /// <summary>Parses one ordered array descriptor.</summary>
        private NativeWireSchemaDescriptor ParseArray(JsonElement schema, string title, string? description, NativeWireSchemaNodeKey? sourceKey, JsonElement? defaultTemplate, JsonElement? constant, JsonElement? defaultValue, IReadOnlyDictionary<string, string?> annotations, HashSet<string> referenceStack, string path, int depth)
        {
            if (!schema.TryGetProperty("items", out var itemsElement) || itemsElement.ValueKind != JsonValueKind.Object)
            {
                throw new SchemaParseException($"{path}.items: native wire arrays require one closed item schema.");
            }

            var item = Parse(itemsElement, $"{title} item", $"{path}.items", sourceKey, null, new HashSet<string>(referenceStack, StringComparer.Ordinal), depth + 1);
            var maximumItems = ReadOptionalNonNegativeInt32(schema, "maxItems", path);
            return new NativeWireSchemaDescriptor(NativeWireSchemaValueKind.Array, title, description, sourceKey, defaultTemplate, itemDescriptor: item, constant: constant, defaultValue: defaultValue, annotations: annotations, maximumItems: maximumItems);
        }

        /// <summary>Parses one integer or enum descriptor.</summary>
        private NativeWireSchemaDescriptor ParseInteger(JsonElement schema, string title, string? description, NativeWireSchemaNodeKey? sourceKey, JsonElement? defaultTemplate, JsonElement? constant, JsonElement? defaultValue, IReadOnlyDictionary<string, string?> annotations, string path)
        {
            var enumValues = new List<NativeWireSchemaEnumValue>();
            if (schema.TryGetProperty("x-native-enum-values", out var valuesElement))
            {
                if (valuesElement.ValueKind != JsonValueKind.Array)
                {
                    throw new SchemaParseException($"{path}.x-native-enum-values: expected an array.");
                }

                foreach (var value in valuesElement.EnumerateArray())
                {
                    if (value.ValueKind != JsonValueKind.Object || !value.TryGetProperty("name", out var nameElement) || !value.TryGetProperty("value", out var numberElement) || nameElement.ValueKind != JsonValueKind.String || numberElement.ValueKind != JsonValueKind.String)
                    {
                        throw new SchemaParseException($"{path}.x-native-enum-values: each value requires string name and value properties.");
                    }

                    enumValues.Add(new NativeWireSchemaEnumValue(nameElement.GetString()!, numberElement.GetString()!));
                }
            }

            return new NativeWireSchemaDescriptor(
                enumValues.Count == 0 ? NativeWireSchemaValueKind.Integer : NativeWireSchemaValueKind.Enum,
                title,
                description,
                sourceKey,
                defaultTemplate,
                constant: constant,
                defaultValue: defaultValue,
                enumValues: Array.AsReadOnly(enumValues.ToArray()),
                annotations: annotations,
                minimum: ReadOptionalNumberText(schema, "minimum", path),
                maximum: ReadOptionalNumberText(schema, "maximum", path));
        }

        /// <summary>Parses one string or byte-array descriptor.</summary>
        private NativeWireSchemaDescriptor ParseString(JsonElement schema, string title, string? description, NativeWireSchemaNodeKey? sourceKey, JsonElement? defaultTemplate, JsonElement? constant, JsonElement? defaultValue, IReadOnlyDictionary<string, string?> annotations, string path)
        {
            var format = ReadOptionalString(schema, "format", path) ?? ReadOptionalString(schema, "contentEncoding", path);
            return new NativeWireSchemaDescriptor(
                NativeWireSchemaValueKind.String,
                title,
                description,
                sourceKey,
                defaultTemplate,
                constant: constant,
                defaultValue: defaultValue,
                annotations: annotations,
                minimumLength: ReadOptionalNonNegativeInt32(schema, "minLength", path),
                maximumLength: ReadOptionalNonNegativeInt32(schema, "maxLength", path),
                pattern: ReadOptionalString(schema, "pattern", path),
                format: format);
        }

        /// <summary>Maps an exact JSON number projection to a string-backed typed editor without floating-point coercion.</summary>
        private static NativeWireSchemaDescriptor ParseNumber(JsonElement schema, string title, string? description, NativeWireSchemaNodeKey? sourceKey, JsonElement? defaultTemplate, JsonElement? constant, JsonElement? defaultValue, IReadOnlyDictionary<string, string?> annotations)
        {
            var marked = annotations.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
            marked["x-presentation-json-number"] = "true";
            return new NativeWireSchemaDescriptor(
                NativeWireSchemaValueKind.String,
                title,
                description,
                sourceKey,
                defaultTemplate,
                constant: constant,
                defaultValue: defaultValue,
                annotations: new System.Collections.ObjectModel.ReadOnlyDictionary<string, string?>(marked));
        }

        /// <summary>Creates one lightweight lazy union option.</summary>
        private NativeWireSchemaUnionOption CreateUnionOption(JsonElement option, int index, string parentTitle, string path, HashSet<string> referenceStack, int depth)
        {
            CancellationToken.ThrowIfCancellationRequested();
            if (option.ValueKind != JsonValueKind.Object)
            {
                throw new SchemaParseException($"{path}: union option {index} must be an object schema.");
            }

            if (option.TryGetProperty("$ref", out var referenceElement) && referenceElement.ValueKind == JsonValueKind.String)
            {
                var targetKey = ResolveReferenceKey(referenceElement.GetString()!, path);
                var display = targetKey.Name.Contains('.') ? targetKey.Name[(targetKey.Name.LastIndexOf('.') + 1)..] : targetKey.Name;
                return new NativeWireSchemaUnionOption(
                    targetKey.Name,
                    display,
                    targetKey.Name,
                    (limits, token) => NativeWireSchemaParser.Resolve(Catalog, targetKey, limits, token));
            }

            var optionCopy = option.Clone();
            var key = $"{parentTitle}:{index.ToString(CultureInfo.InvariantCulture)}";
            var displayName = ReadTitle(optionCopy, key);
            var discriminator = TryReadDiscriminator(optionCopy);
            return new NativeWireSchemaUnionOption(
                key,
                displayName,
                discriminator,
                (limits, token) =>
                {
                    try
                    {
                        var state = new ParseState(Catalog, limits, token);
                        return EngineResult<NativeWireSchemaDescriptor>.Success(state.Parse(optionCopy, displayName, path, null, null, new HashSet<string>(referenceStack, StringComparer.Ordinal), depth + 1));
                    }
                    catch (SchemaParseException exception)
                    {
                        return Failure(exception.Message);
                    }
                });
        }

        /// <summary>Resolves one type reference URI to an exact catalog key without materializing sibling types.</summary>
        private NativeWireSchemaNodeKey ResolveReferenceKey(string reference, string path)
        {
            const string marker = ":type:";
            var markerIndex = reference.IndexOf(marker, StringComparison.Ordinal);
            var name = markerIndex >= 0 ? reference[(markerIndex + marker.Length)..] : reference;
            var matches = Catalog.Nodes.Where(candidate => candidate.Kind == NativeWireSchemaNodeKind.Type && string.Equals(candidate.Name, name, StringComparison.Ordinal)).ToArray();
            if (matches.Length != 1)
            {
                throw new SchemaParseException($"{path}.$ref: reference '{reference}' resolved to {matches.Length} catalog nodes.");
            }

            return matches[0];
        }

        /// <summary>Applies a reference fragment's sibling constraints over the referenced schema.</summary>
        private static JsonElement MergeReferencedSchema(JsonElement target, JsonElement referenceFragment)
        {
            var merged = JsonNode.Parse(target.GetRawText())?.AsObject() ?? throw new SchemaParseException("Referenced schema could not be cloned.");
            foreach (var sibling in referenceFragment.EnumerateObject())
            {
                if (sibling.NameEquals("$ref"))
                {
                    continue;
                }

                if (sibling.NameEquals("properties") && sibling.Value.ValueKind == JsonValueKind.Object && merged["properties"] is JsonObject mergedProperties)
                {
                    foreach (var property in sibling.Value.EnumerateObject())
                    {
                        mergedProperties[property.Name] = JsonNode.Parse(property.Value.GetRawText());
                    }
                }
                else
                {
                    merged[sibling.Name] = JsonNode.Parse(sibling.Value.GetRawText());
                }
            }

            using var document = JsonDocument.Parse(merged.ToJsonString());
            return document.RootElement.Clone();
        }

        /// <summary>Removes null from a multi-type schema for nullable wrapper parsing.</summary>
        private static JsonElement RemoveNullType(JsonElement schema, IReadOnlyList<string> types)
        {
            var clone = JsonNode.Parse(schema.GetRawText())?.AsObject() ?? throw new SchemaParseException("Nullable schema could not be cloned.");
            var remaining = types.Where(type => !string.Equals(type, "null", StringComparison.Ordinal)).ToArray();
            clone["type"] = remaining.Length == 1
                ? JsonValue.Create(remaining[0])
                : new JsonArray(remaining.Select(value => (JsonNode?)JsonValue.Create(value)).ToArray());
            using var document = JsonDocument.Parse(clone.ToJsonString());
            return document.RootElement.Clone();
        }

        /// <summary>Classifies recognized native wrapper objects before falling back to an ordinary object.</summary>
        private static NativeWireSchemaValueKind ClassifyObject(JsonElement schema, IReadOnlyList<NativeWireSchemaProperty> properties, IReadOnlyDictionary<string, string?> annotations)
        {
            var names = properties.Select(property => property.Name).ToHashSet(StringComparer.Ordinal);
            var nativeType = string.Join(' ', annotations.Values.Where(value => value is not null));
            var normalizedNativeType = new string(nativeType.Where(char.IsLetterOrDigit).ToArray());
            if (names.SetEquals(new[] { "isNull", "formKey" }) || nativeType.Contains("FormLink", StringComparison.Ordinal) && !nativeType.Contains("FormLinkOrIndex", StringComparison.Ordinal) && names.Contains("formKey"))
            {
                return NativeWireSchemaValueKind.FormLink;
            }

            if (nativeType.Contains("FormLinkOrIndex", StringComparison.Ordinal) || names.Contains("index") && names.Contains("link") && names.Contains("usesLink"))
            {
                return NativeWireSchemaValueKind.FormLinkOrIndex;
            }

            if (nativeType.Contains("Array2d", StringComparison.OrdinalIgnoreCase) || annotations.ContainsKey("x-native-array2d"))
            {
                return NativeWireSchemaValueKind.Array2D;
            }

            if (normalizedNativeType.Contains("TranslatedString", StringComparison.OrdinalIgnoreCase) || names.Contains("translations") && names.Contains("targetLanguage"))
            {
                return NativeWireSchemaValueKind.TranslatedString;
            }

            if (normalizedNativeType.Contains("AssetLink", StringComparison.OrdinalIgnoreCase) || names.Contains("givenPath") && names.Contains("isNull"))
            {
                return NativeWireSchemaValueKind.Asset;
            }

            if (nativeType.Contains("Color", StringComparison.OrdinalIgnoreCase) && (names.Contains("red") || names.Contains("r") || names.Contains("raw")))
            {
                return NativeWireSchemaValueKind.Color;
            }

            if (names.Contains("bits") && (names.Contains("value") || names.Contains("text") || nativeType.Contains("Single", StringComparison.Ordinal) || nativeType.Contains("Double", StringComparison.Ordinal)))
            {
                return NativeWireSchemaValueKind.FloatBits;
            }

            if (names.Contains("length") && names.Contains("base64"))
            {
                return NativeWireSchemaValueKind.ByteArray;
            }

            return NativeWireSchemaValueKind.Object;
        }

        /// <summary>Reads a schema's explicit type strings.</summary>
        private static IReadOnlyList<string> ReadTypes(JsonElement schema, string path)
        {
            if (!schema.TryGetProperty("type", out var typeElement))
            {
                return Array.Empty<string>();
            }

            if (typeElement.ValueKind == JsonValueKind.String)
            {
                return new[] { typeElement.GetString()! };
            }

            if (typeElement.ValueKind == JsonValueKind.Array && typeElement.EnumerateArray().All(item => item.ValueKind == JsonValueKind.String))
            {
                return typeElement.EnumerateArray().Select(item => item.GetString()!).ToArray();
            }

            throw new SchemaParseException($"{path}.type: expected one string or an array of strings.");
        }

        /// <summary>Infers the primitive type for schema constants or property-only object schemas.</summary>
        private static string InferType(JsonElement schema, JsonElement? constant, string path)
        {
            if (schema.TryGetProperty("properties", out _))
            {
                return "object";
            }

            if (constant.HasValue)
            {
                return constant.Value.ValueKind switch
                {
                    JsonValueKind.Object => "object",
                    JsonValueKind.Array => "array",
                    JsonValueKind.String => "string",
                    JsonValueKind.Number => "integer",
                    JsonValueKind.True or JsonValueKind.False => "boolean",
                    JsonValueKind.Null => "null",
                    _ => throw new SchemaParseException($"{path}.const: unsupported constant kind {constant.Value.ValueKind}."),
                };
            }

            throw new SchemaParseException($"{path}: schema requires a supported type, union, reference, object properties, or constant.");
        }

        /// <summary>Reads one union array by name.</summary>
        private static bool TryReadUnion(JsonElement schema, string propertyName, out JsonElement union)
        {
            if (schema.TryGetProperty(propertyName, out union))
            {
                if (union.ValueKind != JsonValueKind.Array || union.GetArrayLength() == 0)
                {
                    throw new SchemaParseException($"$schema.{propertyName}: expected a non-empty array.");
                }

                return true;
            }

            return false;
        }

        /// <summary>Determines whether an alternative accepts only JSON null.</summary>
        private static bool IsNullSchema(JsonElement schema)
        {
            return schema.ValueKind == JsonValueKind.Object && schema.TryGetProperty("type", out var type) && type.ValueKind == JsonValueKind.String && string.Equals(type.GetString(), "null", StringComparison.Ordinal);
        }

        /// <summary>Reads the catalog title or a stable fallback.</summary>
        private static string ReadTitle(JsonElement schema, string fallback)
        {
            return schema.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(title.GetString()) ? title.GetString()! : fallback;
        }

        /// <summary>Reads one optional string schema member.</summary>
        private static string? ReadOptionalString(JsonElement schema, string name, string path)
        {
            if (!schema.TryGetProperty(name, out var value))
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.String)
            {
                throw new SchemaParseException($"{path}.{name}: expected a string.");
            }

            return value.GetString();
        }

        /// <summary>Reads one optional non-negative Int32 schema member.</summary>
        private static int? ReadOptionalNonNegativeInt32(JsonElement schema, string name, string path)
        {
            if (!schema.TryGetProperty(name, out var value))
            {
                return null;
            }

            if (!value.TryGetInt32(out var parsed) || parsed < 0)
            {
                throw new SchemaParseException($"{path}.{name}: expected a non-negative Int32.");
            }

            return parsed;
        }

        /// <summary>Reads one optional exact numeric token as raw text.</summary>
        private static string? ReadOptionalNumberText(JsonElement schema, string name, string path)
        {
            if (!schema.TryGetProperty(name, out var value))
            {
                return null;
            }

            if (value.ValueKind != JsonValueKind.Number)
            {
                throw new SchemaParseException($"{path}.{name}: expected a JSON number.");
            }

            return value.GetRawText();
        }

        /// <summary>Preserves all native annotation values as exact raw JSON or string content.</summary>
        private static IReadOnlyDictionary<string, string?> ReadAnnotations(JsonElement schema)
        {
            var annotations = new Dictionary<string, string?>(StringComparer.Ordinal);
            foreach (var property in schema.EnumerateObject().Where(property => property.Name.StartsWith("x-", StringComparison.Ordinal)))
            {
                annotations[property.Name] = property.Value.ValueKind == JsonValueKind.String ? property.Value.GetString() : property.Value.GetRawText();
            }

            return new System.Collections.ObjectModel.ReadOnlyDictionary<string, string?>(annotations);
        }

        /// <summary>Reads an inline object's exact discriminator constant when present.</summary>
        private static string? TryReadDiscriminator(JsonElement schema)
        {
            return schema.TryGetProperty("properties", out var properties) && properties.ValueKind == JsonValueKind.Object &&
                properties.TryGetProperty("$type", out var type) && type.ValueKind == JsonValueKind.Object &&
                type.TryGetProperty("const", out var constant) && constant.ValueKind == JsonValueKind.String
                ? constant.GetString()
                : null;
        }

        /// <summary>Counts one schema value and enforces operation depth and node limits.</summary>
        private void Visit(string path, int depth)
        {
            CancellationToken.ThrowIfCancellationRequested();
            if (depth > Limits.MaximumDepth)
            {
                throw new SchemaParseException($"{path}: schema traversal exceeds maximum depth {Limits.MaximumDepth}.");
            }

            if (++VisitedNodes > Limits.MaximumNodes)
            {
                throw new SchemaParseException($"{path}: schema traversal exceeds maximum node count {Limits.MaximumNodes}.");
            }
        }
    }

    /// <summary>Represents one path-specific schema parsing failure.</summary>
    private sealed class SchemaParseException : Exception
    {
        /// <summary>Initializes one schema failure.</summary>
        internal SchemaParseException(string message)
            : base(message)
        {
        }
    }
}
