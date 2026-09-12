using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.NativeFieldGenerator;

/// <content>Emits the deterministic paged Starfield native-field schema source and constructible defaults.</content>
internal sealed partial class CodecEmitter
{
    /// <summary>The stable generated typed-leaf node names in ordinal order.</summary>
    private static readonly IReadOnlyList<string> SchemaLeafTypeNames = Array.AsReadOnly(new[]
    {
        "native.array2d",
        "native.asset-link",
        "native.byte-sequence",
        "native.color",
        "native.double",
        "native.form-link",
        "native.form-link-or-index",
        "native.p2-float",
        "native.p2-int",
        "native.p3-float",
        "native.single",
        "native.translated-string",
    });

    /// <summary>The stable generated polymorphic union node names in ownership order.</summary>
    private static readonly IReadOnlyList<KeyValuePair<string, Type>> SchemaUnionTypes = Array.AsReadOnly(new[]
    {
        new KeyValuePair<string, Type>("native.component", typeof(AComponent)),
        new KeyValuePair<string, Type>("native.condition", typeof(Condition)),
        new KeyValuePair<string, Type>("native.condition-data", typeof(ConditionData)),
    });

    /// <summary>The stable absolute JSON Schema URI prefix for Starfield v1 type nodes.</summary>
    private const string SchemaTypeUriPrefix = "urn:creationsforge:native-wire:starfield:starfield:v1:type:";

    /// <summary>The default Core per-array limit represented by generated catalog schemas.</summary>
    private const int SchemaMaximumArrayElements = 65536;

    /// <summary>Explains why installed condition-root constructor defaults cannot satisfy their non-null wire contract.</summary>
    private const string ConditionRootDefaultUnavailableReason = "The installed public constructor leaves required Data null; supply a concrete ConditionData payload.";

    /// <summary>Explains why the installed volumes-item constructor default cannot satisfy its non-null wire contract.</summary>
    private const string VolumesItemDefaultUnavailableReason = "The installed public constructor leaves required Ender null; supply a concrete AVolumesUnknownEnder payload.";

    /// <summary>Emits the generated schema lookup and actual installed default materializer.</summary>
    /// <returns>Complete generated schema helper source.</returns>
    private string EmitSchemaCatalog()
    {
        var schemas = SchemaUnionTypes
            .Select(pair => new KeyValuePair<string, string>(pair.Key, EmitUnionSchema(pair.Key, pair.Value)))
            .Concat(SchemaLeafTypeNames
            .Select(name => new KeyValuePair<string, string>(name, EmitLeafSchema(name)))
            )
            .Concat(Models.Select(model => new KeyValuePair<string, string>(
                model.MutableType.FullName!,
                EmitTypeSchema(model))))
            .ToArray();
        var componentTypeNames = Models
            .Where(model => typeof(AComponent).IsAssignableFrom(model.MutableType))
            .Select(model => model.MutableType.FullName!)
            .ToArray();
        var conditionTypeNames = Models
            .Where(model => model.MutableType == typeof(ConditionFloat) || model.MutableType == typeof(ConditionGlobal))
            .Select(model => model.MutableType.FullName!)
            .ToArray();
        var conditionDataTypeNames = Models
            .Where(model => typeof(ConditionData).IsAssignableFrom(model.MutableType))
            .Select(model => model.MutableType.FullName!)
            .ToArray();
        var output = SourceHeader();
        output.AppendLine("using System.Buffers;");
        output.AppendLine("using System.Text;");
        output.AppendLine("using System.Text.Json;");
        output.AppendLine();
        output.AppendLine("namespace CreationsForge.Starfield.Native.NativeInspection;");
        output.AppendLine();
        output.AppendLine("/// <summary>Exposes generated Starfield native-field schemas and actual installed constructor defaults to the game catalog.</summary>");
        output.AppendLine("internal static class StarfieldGeneratedNativeFieldSchema");
        output.AppendLine("{");
        output.AppendLine("    /// <summary>The stable absolute JSON Schema URI prefix for Starfield v1 type nodes.</summary>");
        output.AppendLine($"    internal const string TypeUriPrefix = \"{SchemaTypeUriPrefix}\";");
        output.AppendLine();
        output.AppendLine("    /// <summary>Every generated typed-leaf and concrete native node name in deterministic ordinal order.</summary>");
        output.AppendLine("    private static readonly IReadOnlyList<string> OrderedTypeNames = Array.AsReadOnly(new[]");
        output.AppendLine("    {");
        foreach (var schema in schemas)
        {
            output.AppendLine($"        \"{Escape(schema.Key)}\",");
        }

        output.AppendLine("    });");
        output.AppendLine();
        EmitGeneratedTypeNameList(output, "OrderedComponentTypeNames", componentTypeNames);
        EmitGeneratedTypeNameList(output, "OrderedConditionTypeNames", conditionTypeNames);
        EmitGeneratedTypeNameList(output, "OrderedConditionDataTypeNames", conditionDataTypeNames);
        output.AppendLine("    /// <summary>Gets every generated union, typed-leaf, and concrete native node name in deterministic ordinal order.</summary>");
        output.AppendLine("    internal static IReadOnlyList<string> TypeNames => OrderedTypeNames;");
        output.AppendLine();
        output.AppendLine("    /// <summary>Gets every concrete generated component type name in deterministic ordinal order.</summary>");
        output.AppendLine("    internal static IReadOnlyList<string> ComponentTypeNames => OrderedComponentTypeNames;");
        output.AppendLine();
        output.AppendLine("    /// <summary>Gets both concrete generated condition root type names in deterministic ordinal order.</summary>");
        output.AppendLine("    internal static IReadOnlyList<string> ConditionTypeNames => OrderedConditionTypeNames;");
        output.AppendLine();
        output.AppendLine("    /// <summary>Gets every concrete generated condition-data type name in deterministic ordinal order.</summary>");
        output.AppendLine("    internal static IReadOnlyList<string> ConditionDataTypeNames => OrderedConditionDataTypeNames;");
        output.AppendLine();
        output.AppendLine("    /// <summary>Returns the complete JSON Schema object for one generated type node.</summary>");
        output.AppendLine("    /// <param name=\"nativeTypeName\">The exact generated leaf or mutable native type name.</param>");
        output.AppendLine("    /// <returns>The deterministic compact JSON Schema object.</returns>");
        output.AppendLine("    /// <exception cref=\"ArgumentException\">Thrown when the type name is empty.</exception>");
        output.AppendLine("    /// <exception cref=\"NotSupportedException\">Thrown when the type name is outside generated coverage.</exception>");
        output.AppendLine("    internal static string GetSchemaJson(string nativeTypeName)");
        output.AppendLine("    {");
        output.AppendLine("        ArgumentException.ThrowIfNullOrWhiteSpace(nativeTypeName);");
        output.AppendLine("        return nativeTypeName switch");
        output.AppendLine("        {");
        foreach (var schema in schemas)
        {
            output.AppendLine($"            \"{Escape(schema.Key)}\" => \"{Escape(schema.Value)}\",");
        }

        output.AppendLine("            _ => throw new NotSupportedException($\"Native type '{nativeTypeName}' is outside generated Starfield schema coverage.\"),");
        output.AppendLine("        };");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Creates the exact writer-compatible default JSON for one constructible node.</summary>");
        output.AppendLine("    /// <param name=\"nativeTypeName\">The exact generated leaf or mutable native type name.</param>");
        output.AppendLine("    /// <param name=\"cancellationToken\">A token observed during default construction and traversal.</param>");
        output.AppendLine("    /// <returns>The compact actual default JSON, or null when the schema records why no honest generic default exists.</returns>");
        output.AppendLine("    /// <exception cref=\"ArgumentException\">Thrown when the type name is empty.</exception>");
        output.AppendLine("    /// <exception cref=\"NotSupportedException\">Thrown when the type name is outside generated coverage.</exception>");
        output.AppendLine("    internal static string? CreateDefaultJson(string nativeTypeName, CancellationToken cancellationToken = default)");
        output.AppendLine("    {");
        output.AppendLine("        _ = GetSchemaJson(nativeTypeName);");
        output.AppendLine("        cancellationToken.ThrowIfCancellationRequested();");
        output.AppendLine("        switch (nativeTypeName)");
        output.AppendLine("        {");
        output.AppendLine("            case \"native.component\":");
        output.AppendLine("            case \"native.condition\":");
        output.AppendLine("            case \"native.condition-data\":");
        output.AppendLine("            case \"native.array2d\":");
        output.AppendLine("            case \"native.asset-link\":");
        output.AppendLine("            case \"native.form-link\":");
        output.AppendLine("            case \"native.form-link-or-index\":");
        output.AppendLine("            case \"Mutagen.Bethesda.Starfield.ConditionFloat\":");
        output.AppendLine("            case \"Mutagen.Bethesda.Starfield.ConditionGlobal\":");
        output.AppendLine("            case \"Mutagen.Bethesda.Starfield.VolumesComponentItem\":");
        output.AppendLine("                return null;");
        output.AppendLine("            case \"native.byte-sequence\":");
        output.AppendLine("                return \"{\\\"length\\\":0,\\\"base64\\\":\\\"\\\"}\";");
        output.AppendLine("            case \"native.color\":");
        output.AppendLine($"                return \"{Escape(EmitColorDefault())}\";");
        output.AppendLine("            case \"native.double\":");
        output.AppendLine("                return \"{\\\"text\\\":\\\"0\\\",\\\"bits\\\":\\\"0x0000000000000000\\\",\\\"number\\\":0}\";");
        output.AppendLine("            case \"native.p2-float\":");
        output.AppendLine("                return \"{\\\"X\\\":{\\\"text\\\":\\\"0\\\",\\\"bits\\\":\\\"0x00000000\\\",\\\"number\\\":0},\\\"Y\\\":{\\\"text\\\":\\\"0\\\",\\\"bits\\\":\\\"0x00000000\\\",\\\"number\\\":0}}\";");
        output.AppendLine("            case \"native.p2-int\":");
        output.AppendLine("                return \"{\\\"X\\\":0,\\\"Y\\\":0}\";");
        output.AppendLine("            case \"native.p3-float\":");
        output.AppendLine("                return \"{\\\"X\\\":{\\\"text\\\":\\\"0\\\",\\\"bits\\\":\\\"0x00000000\\\",\\\"number\\\":0},\\\"Y\\\":{\\\"text\\\":\\\"0\\\",\\\"bits\\\":\\\"0x00000000\\\",\\\"number\\\":0},\\\"Z\\\":{\\\"text\\\":\\\"0\\\",\\\"bits\\\":\\\"0x00000000\\\",\\\"number\\\":0}}\";");
        output.AppendLine("            case \"native.single\":");
        output.AppendLine("                return \"{\\\"text\\\":\\\"0\\\",\\\"bits\\\":\\\"0x00000000\\\",\\\"number\\\":0}\";");
        output.AppendLine("            case \"native.translated-string\":");
        output.AppendLine($"                return \"{Escape(EmitTranslatedStringDefault())}\";");
        output.AppendLine("        }");
        output.AppendLine();
        output.AppendLine("        var buffer = new ArrayBufferWriter<byte>();");
        output.AppendLine("        using (var writer = new Utf8JsonWriter(buffer))");
        output.AppendLine("        {");
        output.AppendLine("            StarfieldNestedFieldCodec.WriteDefaultNativeType(writer, nativeTypeName, cancellationToken);");
        output.AppendLine("        }");
        output.AppendLine("        cancellationToken.ThrowIfCancellationRequested();");
        output.AppendLine("        return Encoding.UTF8.GetString(buffer.WrittenSpan);");
        output.AppendLine("    }");
        output.AppendLine("}");
        return output.ToString();
    }

    /// <summary>Emits one generated deterministic string list field.</summary>
    /// <param name="output">The generated source receiving the field.</param>
    /// <param name="fieldName">The private generated field name.</param>
    /// <param name="typeNames">The ordered exact native type names.</param>
    private static void EmitGeneratedTypeNameList(StringBuilder output, string fieldName, IEnumerable<string> typeNames)
    {
        output.AppendLine($"    private static readonly IReadOnlyList<string> {fieldName} = Array.AsReadOnly(new[]");
        output.AppendLine("    {");
        foreach (var typeName in typeNames)
        {
            output.AppendLine($"        \"{Escape(typeName)}\",");
        }

        output.AppendLine("    });");
        output.AppendLine();
    }

    /// <summary>Emits one complete polymorphic union schema from installed generated coverage.</summary>
    /// <param name="nodeName">The stable generated union node name.</param>
    /// <param name="polymorphicType">The native root type represented by the union.</param>
    /// <returns>Compact deterministic JSON Schema.</returns>
    private string EmitUnionSchema(string nodeName, Type polymorphicType)
    {
        var alternatives = new JsonArray();
        foreach (var model in Models.Where(model => polymorphicType.IsAssignableFrom(model.MutableType)))
        {
            alternatives.Add(ReferenceSchema(model.MutableType.FullName!));
        }

        if (alternatives.Count == 0)
        {
            throw new InvalidOperationException($"Polymorphic schema {polymorphicType} has no generated concrete alternatives.");
        }

        var schema = SchemaObject(nodeName, TypeName(polymorphicType));
        schema["title"] = nodeName;
        schema["description"] = $"Generated exact concrete union for installed native type {TypeName(polymorphicType)}.";
        schema["oneOf"] = alternatives;
        schema["x-native-polymorphic-type"] = TypeName(polymorphicType);
        schema["x-default-unavailable-reason"] = "A polymorphic union requires selection of one concrete native type.";
        return schema.ToJsonString();
    }

    /// <summary>Emits one complete concrete indexed type schema from installed metadata.</summary>
    /// <param name="model">The concrete generated native model.</param>
    /// <returns>Compact deterministic JSON Schema.</returns>
    private string EmitTypeSchema(NativeTypeModel model)
    {
        var typeName = model.MutableType.FullName!;
        var properties = new JsonObject
        {
            ["$type"] = new JsonObject
            {
                ["const"] = NativeTypeDiscriminator(typeName),
                ["description"] = "The exact package-scoped native concrete type discriminator.",
            },
        };
        var required = new JsonArray("$type");
        if (typeof(ConditionData).IsAssignableFrom(model.MutableType)
            && !model.Fields.Any(static field => string.Equals(field.Name, "Function", StringComparison.Ordinal)))
        {
            var instance = System.Activator.CreateInstance(model.MutableType)
                ?? throw new InvalidOperationException($"Could not construct default native type {typeName} for derived Function metadata.");
            var functionProperty = model.GetterType.GetProperty("Function")
                ?? model.GetterType.GetInterfaces().Select(type => type.GetProperty("Function")).FirstOrDefault(property => property is not null)
                ?? throw new InvalidOperationException($"No Function getter found for condition data {typeName}.");
            var function = Convert.ToUInt16(functionProperty.GetValue(instance));
            properties["Function"] = new JsonObject
            {
                ["type"] = "integer",
                ["const"] = function,
                ["readOnly"] = true,
                ["description"] = "Derived from the concrete installed ConditionData type and validated during decoding.",
                ["x-native-type"] = TypeName(typeof(Condition.Function)),
                ["x-native-derived"] = true,
            };
        }

        foreach (var field in model.Fields)
        {
            var fieldSchema = CreateValueSchema(field.Construction);
            fieldSchema["description"] = $"Native FieldIndex ordinal {field.Ordinal} for {typeName}.{field.Name}.";
            fieldSchema["x-native-field-ordinal"] = field.Ordinal;
            fieldSchema["x-native-mutable-type"] = TypeName(field.MutableProperty.PropertyType);
            fieldSchema["x-native-getter-type"] = TypeName(field.GetterProperty.PropertyType);
            fieldSchema["x-native-nullability"] = DescribeNullability(field.Nullability);
            properties[field.Name] = fieldSchema;
            required.Add(field.Name);
        }

        var schema = SchemaObject(typeName, typeName);
        schema["title"] = model.MutableType.Name;
        schema["description"] = $"Complete generated construction schema for installed native type {typeName}. JSON is request-local wire data and is not persisted record authority.";
        schema["type"] = "object";
        schema["properties"] = properties;
        schema["required"] = required;
        schema["additionalProperties"] = false;
        schema["x-native-mutable-type"] = typeName;
        schema["x-native-getter-type"] = TypeName(model.GetterType);
        schema["x-native-field-count"] = model.Fields.Count;
        schema["x-native-construction"] = "public-parameterless-constructor-then-indexed-setters";
        if (model.MutableType == typeof(ConditionFloat) || model.MutableType == typeof(ConditionGlobal))
        {
            schema["x-default-unavailable-reason"] = ConditionRootDefaultUnavailableReason;
        }
        else if (model.MutableType == typeof(VolumesComponentItem))
        {
            schema["x-default-unavailable-reason"] = VolumesItemDefaultUnavailableReason;
        }

        return schema.ToJsonString();
    }

    /// <summary>Creates one recursive JSON Schema value for a generated native field.</summary>
    /// <param name="construction">The recursive construction description.</param>
    /// <returns>The complete field schema without its owning ordinal metadata.</returns>
    private JsonObject CreateValueSchema(NativeValueConstructionModel construction)
    {
        if (construction.Kind == NativeValueConstructionKind.Nullable)
        {
            return NullableSchema(CreateValueSchema(construction.Element
                ?? throw new InvalidOperationException($"Nullable construction {construction.GetterType} has no element.")));
        }

        JsonObject schema = construction.Kind switch
        {
            NativeValueConstructionKind.String => new JsonObject
            {
                ["type"] = "string",
                ["maxLength"] = 1048576,
            },
            NativeValueConstructionKind.Boolean => new JsonObject { ["type"] = "boolean" },
            NativeValueConstructionKind.Single => ReferenceSchema("native.single"),
            NativeValueConstructionKind.Double => ReferenceSchema("native.double"),
            NativeValueConstructionKind.Enum => CreateEnumSchema(construction.GetterType),
            NativeValueConstructionKind.Numeric => CreateNumericSchema(construction.GetterType),
            NativeValueConstructionKind.Guid => new JsonObject
            {
                ["type"] = "string",
                ["format"] = "uuid",
                ["pattern"] = "^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$",
            },
            NativeValueConstructionKind.Color => ReferenceSchema("native.color"),
            NativeValueConstructionKind.P2Float => ReferenceSchema("native.p2-float"),
            NativeValueConstructionKind.P2Int => ReferenceSchema("native.p2-int"),
            NativeValueConstructionKind.P3Float => ReferenceSchema("native.p3-float"),
            NativeValueConstructionKind.TranslatedString => ReferenceSchema("native.translated-string"),
            NativeValueConstructionKind.Memory => ReferenceSchema("native.byte-sequence"),
            NativeValueConstructionKind.FormLink => CreateFormLinkSchema(construction),
            NativeValueConstructionKind.FormLinkOrIndex => CreateFormLinkOrIndexSchema(construction),
            NativeValueConstructionKind.AssetLink => CreateAssetLinkSchema(construction),
            NativeValueConstructionKind.Collection => CreateCollectionSchema(construction),
            NativeValueConstructionKind.Array2d => CreateArray2dSchema(construction),
            NativeValueConstructionKind.NestedConcrete => ReferenceSchema(construction.MutableType.FullName!),
            NativeValueConstructionKind.NestedPolymorphic => CreatePolymorphicSchema(construction.MutableType),
            _ => throw new InvalidOperationException($"Unhandled schema construction kind {construction.Kind} for {construction.GetterType}."),
        };
        return AllowsNull(construction) ? NullableSchema(schema) : schema;
    }

    /// <summary>Creates a reference to one stable generated schema node URI.</summary>
    /// <param name="nodeName">The exact target node name.</param>
    /// <returns>A JSON Schema reference object.</returns>
    private static JsonObject ReferenceSchema(string nodeName)
    {
        return new JsonObject { ["$ref"] = SchemaTypeUri(nodeName) };
    }

    /// <summary>Wraps one non-null schema with explicit JSON null support.</summary>
    /// <param name="schema">The non-null schema to wrap.</param>
    /// <returns>An anyOf schema preserving the original value constraints.</returns>
    private static JsonObject NullableSchema(JsonObject schema)
    {
        return new JsonObject
        {
            ["anyOf"] = new JsonArray(schema, new JsonObject { ["type"] = "null" }),
        };
    }

    /// <summary>Creates one complete underlying-domain enum schema with installed symbolic metadata.</summary>
    /// <param name="enumType">The installed native enum type.</param>
    /// <returns>The integral schema and exact installed names and values.</returns>
    private static JsonObject CreateEnumSchema(Type enumType)
    {
        var schema = CreateNumericSchema(Enum.GetUnderlyingType(enumType));
        schema["x-native-type"] = TypeName(enumType);
        var values = new JsonArray();
        foreach (var name in Enum.GetNames(enumType))
        {
            var value = Enum.Parse(enumType, name);
            values.Add(new JsonObject
            {
                ["name"] = name,
                ["value"] = EnumValueText(value, enumType),
            });
        }

        schema["x-native-enum-values"] = values;
        schema["description"] = "The decoder accepts the complete underlying integral domain; installed symbolic names are provided for UI controls.";
        return schema;
    }

    /// <summary>Returns one enum value as exact invariant decimal text without signedness loss.</summary>
    /// <param name="value">The boxed enum value.</param>
    /// <param name="enumType">The enum type defining signedness.</param>
    /// <returns>Canonical invariant decimal text.</returns>
    private static string EnumValueText(object value, Type enumType)
    {
        var underlying = Enum.GetUnderlyingType(enumType);
        return underlying == typeof(byte) || underlying == typeof(ushort) || underlying == typeof(uint) || underlying == typeof(ulong)
            ? Convert.ToUInt64(value).ToString(System.Globalization.CultureInfo.InvariantCulture)
            : Convert.ToInt64(value).ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>Creates one exact bounded integral schema, including canonical-string support for 64-bit values.</summary>
    /// <param name="numericType">The exact supported integral type.</param>
    /// <returns>The complete scalar schema.</returns>
    private static JsonObject CreateNumericSchema(Type numericType)
    {
        if (numericType == typeof(long))
        {
            return new JsonObject
            {
                ["anyOf"] = new JsonArray(
                    new JsonObject { ["type"] = "string", ["pattern"] = "^-?(0|[1-9][0-9]*)$", ["x-canonical-invariant-int64"] = true },
                    new JsonObject { ["type"] = "integer", ["minimum"] = long.MinValue, ["maximum"] = long.MaxValue }),
            };
        }

        if (numericType == typeof(ulong))
        {
            return new JsonObject
            {
                ["anyOf"] = new JsonArray(
                    new JsonObject { ["type"] = "string", ["pattern"] = "^(0|[1-9][0-9]*)$", ["x-canonical-invariant-uint64"] = true },
                    new JsonObject { ["type"] = "integer", ["minimum"] = 0, ["maximum"] = ulong.MaxValue }),
                ["description"] = "Canonical decimal strings are preferred for the complete unsigned 64-bit domain; exact JSON integer tokens are also accepted.",
            };
        }

        (long Minimum, ulong Maximum, bool Unsigned) range = numericType == typeof(sbyte) ? (sbyte.MinValue, (ulong)sbyte.MaxValue, false)
            : numericType == typeof(byte) ? (byte.MinValue, byte.MaxValue, true)
            : numericType == typeof(short) ? (short.MinValue, (ulong)short.MaxValue, false)
            : numericType == typeof(ushort) ? (ushort.MinValue, ushort.MaxValue, true)
            : numericType == typeof(int) ? (int.MinValue, (ulong)int.MaxValue, false)
            : numericType == typeof(uint) ? (uint.MinValue, uint.MaxValue, true)
            : throw new InvalidOperationException($"Numeric type {numericType} has no JSON Schema range.");
        return new JsonObject
        {
            ["type"] = "integer",
            ["minimum"] = range.Minimum,
            ["maximum"] = range.Unsigned ? JsonValue.Create(range.Maximum) : JsonValue.Create((long)range.Maximum),
        };
    }

    /// <summary>Creates one specialized typed FormKey link schema.</summary>
    /// <param name="construction">The closed link construction description.</param>
    /// <returns>The common link reference with exact wrapper and target metadata.</returns>
    private static JsonObject CreateFormLinkSchema(NativeValueConstructionModel construction)
    {
        return new JsonObject
        {
            ["$ref"] = SchemaTypeUri("native.form-link"),
            ["properties"] = new JsonObject
            {
                ["$type"] = new JsonObject { ["const"] = NativeTypeDiscriminator(TypeName(construction.GetterType)) },
            },
            ["x-native-wrapper-type"] = TypeName(construction.GetterType),
            ["x-native-reference-target"] = TypeName(construction.GetterType.GetGenericArguments()[0]),
        };
    }

    /// <summary>Creates one specialized owner-sensitive link-or-index schema.</summary>
    /// <param name="construction">The closed owner-linked construction description.</param>
    /// <returns>The common wrapper reference with exact target metadata.</returns>
    private static JsonObject CreateFormLinkOrIndexSchema(NativeValueConstructionModel construction)
    {
        return new JsonObject
        {
            ["$ref"] = SchemaTypeUri("native.form-link-or-index"),
            ["properties"] = new JsonObject
            {
                ["$type"] = new JsonObject { ["const"] = NativeTypeDiscriminator(TypeName(construction.GetterType)) },
            },
            ["x-native-wrapper-type"] = TypeName(construction.GetterType),
            ["x-native-reference-target"] = TypeName(construction.GetterType.GetGenericArguments()[0]),
            ["x-native-owner-derived-mode"] = true,
        };
    }

    /// <summary>Creates one specialized typed asset-link schema.</summary>
    /// <param name="construction">The closed asset-link construction description.</param>
    /// <returns>The common asset reference with exact asset-type metadata.</returns>
    private static JsonObject CreateAssetLinkSchema(NativeValueConstructionModel construction)
    {
        return new JsonObject
        {
            ["$ref"] = SchemaTypeUri("native.asset-link"),
            ["properties"] = new JsonObject
            {
                ["$type"] = new JsonObject { ["const"] = NativeTypeDiscriminator(TypeName(construction.GetterType)) },
            },
            ["x-native-wrapper-type"] = TypeName(construction.GetterType),
            ["x-native-asset-type"] = TypeName(construction.GetterType.GetGenericArguments()[0]),
        };
    }

    /// <summary>Creates one bounded ordered collection schema.</summary>
    /// <param name="construction">The closed collection construction description.</param>
    /// <returns>The array schema with its recursive element contract.</returns>
    private JsonObject CreateCollectionSchema(NativeValueConstructionModel construction)
    {
        return new JsonObject
        {
            ["type"] = "array",
            ["maxItems"] = 65536,
            ["items"] = CreateValueSchema(construction.Element
                ?? throw new InvalidOperationException($"Collection construction {construction.GetterType} has no element.")),
            ["x-native-collection-type"] = TypeName(construction.MutableType),
        };
    }

    /// <summary>Creates one closed row-major two-dimensional collection schema.</summary>
    /// <param name="construction">The closed Array2d construction description.</param>
    /// <returns>The dimensioned rows schema with recursive element contract.</returns>
    private JsonObject CreateArray2dSchema(NativeValueConstructionModel construction)
    {
        var element = construction.Element
            ?? throw new InvalidOperationException($"Array2d construction {construction.GetterType} has no element.");
        return new JsonObject
        {
            ["type"] = "object",
            ["properties"] = new JsonObject
            {
                ["width"] = new JsonObject { ["type"] = "integer", ["minimum"] = 0, ["maximum"] = SchemaMaximumArrayElements },
                ["height"] = new JsonObject { ["type"] = "integer", ["minimum"] = 0, ["maximum"] = SchemaMaximumArrayElements },
                ["rows"] = new JsonObject
                {
                    ["type"] = "array",
                    ["maxItems"] = 65536,
                    ["items"] = new JsonObject
                    {
                        ["type"] = "array",
                        ["maxItems"] = 65536,
                        ["items"] = CreateValueSchema(element),
                    },
                },
            },
            ["required"] = new JsonArray("width", "height", "rows"),
            ["additionalProperties"] = false,
            ["x-native-array-type"] = TypeName(construction.MutableType),
            ["x-row-major"] = true,
        };
    }

    /// <summary>Creates one anyOf reference set for an abstract installed native union.</summary>
    /// <param name="polymorphicType">The abstract mutable native type.</param>
    /// <returns>The exact generated concrete alternatives in ordinal type-name order.</returns>
    private JsonObject CreatePolymorphicSchema(Type polymorphicType)
    {
        var alternatives = new JsonArray();
        foreach (var model in Models.Where(model => polymorphicType.IsAssignableFrom(model.MutableType)))
        {
            alternatives.Add(ReferenceSchema(model.MutableType.FullName!));
        }

        if (alternatives.Count == 0)
        {
            throw new InvalidOperationException($"Polymorphic schema {polymorphicType} has no generated concrete alternatives.");
        }

        return new JsonObject
        {
            ["oneOf"] = alternatives,
            ["x-native-polymorphic-type"] = TypeName(polymorphicType),
        };
    }

    /// <summary>Emits one common typed-leaf JSON Schema.</summary>
    /// <param name="name">The stable typed-leaf node name.</param>
    /// <returns>Compact deterministic JSON Schema.</returns>
    private static string EmitLeafSchema(string name)
    {
        var schema = SchemaObject(name, name);
        schema["title"] = name;
        schema["description"] = "Generated native wire leaf used by Starfield indexed-field construction.";
        switch (name)
        {
            case "native.single":
                AddFloatingSchema(schema, 8, "single");
                break;
            case "native.double":
                AddFloatingSchema(schema, 16, "double");
                break;
            case "native.byte-sequence":
                AddClosedObject(schema, new JsonObject
                {
                    ["length"] = new JsonObject { ["type"] = "integer", ["minimum"] = 0, ["maximum"] = 8388608 },
                    ["base64"] = new JsonObject { ["type"] = "string", ["contentEncoding"] = "base64" },
                }, "length", "base64");
                break;
            case "native.form-link":
                AddClosedObject(schema, new JsonObject
                {
                    ["$type"] = new JsonObject { ["type"] = "string", ["minLength"] = 1 },
                    ["isNull"] = new JsonObject { ["type"] = "boolean" },
                    ["formKey"] = NullableSchema(new JsonObject { ["type"] = "string", ["maxLength"] = 1048576 }),
                }, "$type", "isNull", "formKey");
                var nullFormKey = Mutagen.Bethesda.Plugins.FormKey.Null.ToString();
                schema["oneOf"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["properties"] = new JsonObject
                        {
                            ["isNull"] = new JsonObject { ["const"] = true },
                            ["formKey"] = new JsonObject
                            {
                                ["oneOf"] = new JsonArray
                                {
                                    new JsonObject { ["type"] = "null" },
                                    new JsonObject { ["const"] = nullFormKey },
                                },
                            },
                        },
                    },
                    new JsonObject
                    {
                        ["properties"] = new JsonObject
                        {
                            ["isNull"] = new JsonObject { ["const"] = false },
                            ["formKey"] = new JsonObject
                            {
                                ["type"] = "string",
                                ["maxLength"] = 1048576,
                                ["not"] = new JsonObject { ["const"] = nullFormKey },
                            },
                        },
                    },
                };
                schema["x-default-unavailable-reason"] = "A generic typed FormLink default requires a concrete native reference target.";
                break;
            case "native.form-link-or-index":
                AddClosedObject(schema, new JsonObject
                {
                    ["$type"] = new JsonObject { ["type"] = "string", ["minLength"] = 1 },
                    ["usesLink"] = new JsonObject { ["type"] = "boolean", ["readOnly"] = true },
                    ["usesAlias"] = new JsonObject { ["type"] = "boolean", ["readOnly"] = true },
                    ["usesPackageData"] = new JsonObject { ["type"] = "boolean", ["readOnly"] = true },
                    ["index"] = NullableSchema(new JsonObject { ["type"] = "integer", ["minimum"] = 0, ["maximum"] = uint.MaxValue }),
                    ["link"] = ReferenceSchema("native.form-link"),
                }, "$type", "index", "link");
                schema["x-default-unavailable-reason"] = "A link-or-index default requires a concrete native reference target and owner flags.";
                break;
            case "native.translated-string":
                AddClosedObject(schema, new JsonObject
                {
                    ["$type"] = new JsonObject { ["const"] = NativeTypeDiscriminator("Mutagen.Bethesda.Strings.ITranslatedStringGetter") },
                    ["value"] = CreateTranslatedStringValueSchema(),
                }, "$type", "value");
                break;
            case "native.asset-link":
                AddClosedObject(schema, new JsonObject
                {
                    ["$type"] = new JsonObject { ["type"] = "string", ["minLength"] = 1 },
                    ["value"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JsonObject
                        {
                            ["isNull"] = new JsonObject { ["type"] = "boolean" },
                            ["givenPath"] = new JsonObject { ["type"] = "string", ["maxLength"] = 1048576 },
                            ["dataRelativePath"] = new JsonObject { ["type"] = "string", ["maxLength"] = 1048576, ["readOnly"] = true },
                            ["extension"] = new JsonObject { ["type"] = "string", ["maxLength"] = 1048576, ["readOnly"] = true },
                        },
                        ["required"] = new JsonArray("isNull", "givenPath"),
                        ["additionalProperties"] = false,
                        ["oneOf"] = new JsonArray
                        {
                            new JsonObject
                            {
                                ["properties"] = new JsonObject
                                {
                                    ["isNull"] = new JsonObject { ["const"] = true },
                                    ["givenPath"] = new JsonObject { ["const"] = string.Empty },
                                },
                            },
                            new JsonObject
                            {
                                ["properties"] = new JsonObject
                                {
                                    ["isNull"] = new JsonObject { ["const"] = false },
                                    ["givenPath"] = new JsonObject { ["type"] = "string", ["minLength"] = 1, ["maxLength"] = 1048576 },
                                },
                            },
                        },
                    },
                }, "$type", "value");
                schema["x-default-unavailable-reason"] = "A generic typed asset-link default requires a concrete asset target type.";
                break;
            case "native.color":
                AddClosedObject(schema, CreateColorProperties(), "argb", "a", "r", "g", "b", "isEmpty", "isKnownColor", "isNamedColor", "isSystemColor", "name");
                break;
            case "native.p2-float":
                AddClosedObject(schema, FloatVectorProperties("X", "Y"), "X", "Y");
                break;
            case "native.p2-int":
                AddClosedObject(schema, new JsonObject
                {
                    ["X"] = CreateNumericSchema(typeof(int)),
                    ["Y"] = CreateNumericSchema(typeof(int)),
                }, "X", "Y");
                break;
            case "native.p3-float":
                AddClosedObject(schema, FloatVectorProperties("X", "Y", "Z"), "X", "Y", "Z");
                break;
            case "native.array2d":
                schema["type"] = "object";
                schema["description"] = "Generic row-major Array2d shape. Concrete field schemas supply the recursive element contract.";
                schema["x-default-unavailable-reason"] = "A generic Array2d default requires a concrete native element type.";
                break;
            default:
                throw new InvalidOperationException($"Unknown generated typed-leaf schema {name}.");
        }

        return schema.ToJsonString();
    }

    /// <summary>Creates the common root fields for one JSON Schema type node.</summary>
    /// <param name="nodeName">The exact schema node name.</param>
    /// <param name="nativeTypeName">The installed native or stable leaf type name.</param>
    /// <returns>The ordered schema root.</returns>
    private static JsonObject SchemaObject(string nodeName, string nativeTypeName)
    {
        return new JsonObject
        {
            ["$schema"] = "https://json-schema.org/draft/2020-12/schema",
            ["$id"] = SchemaTypeUri(nodeName),
            ["x-native-type"] = nativeTypeName,
        };
    }

    /// <summary>Returns the stable absolute schema URI for one generated node name.</summary>
    /// <param name="nodeName">The exact schema node name.</param>
    /// <returns>The catalog-content-independent absolute URI.</returns>
    private static string SchemaTypeUri(string nodeName)
    {
        return SchemaTypeUriPrefix + Uri.EscapeDataString(nodeName);
    }

    /// <summary>Returns the package-scoped wire discriminator for one native type identity.</summary>
    /// <param name="nativeTypeName">The native full type identity.</param>
    /// <returns>The exact writer-compatible discriminator.</returns>
    private static string NativeTypeDiscriminator(string nativeTypeName)
    {
        return $"Mutagen.Bethesda.Starfield/{PackageVersion}:{nativeTypeName}";
    }

    /// <summary>Adds a closed object contract to one schema root.</summary>
    /// <param name="schema">The schema root to complete.</param>
    /// <param name="properties">The exact accepted properties.</param>
    /// <param name="required">Every required property in wire order.</param>
    private static void AddClosedObject(JsonObject schema, JsonObject properties, params string[] required)
    {
        var requiredProperties = new JsonArray();
        foreach (var requiredName in required)
        {
            requiredProperties.Add(requiredName);
        }

        schema["type"] = "object";
        schema["properties"] = properties;
        schema["required"] = requiredProperties;
        schema["additionalProperties"] = false;
    }

    /// <summary>Adds the exact bits-authoritative floating-point object contract.</summary>
    /// <param name="schema">The schema root to complete.</param>
    /// <param name="hexDigits">The exact hexadecimal digit count.</param>
    /// <param name="description">The native floating-point kind.</param>
    private static void AddFloatingSchema(JsonObject schema, int hexDigits, string description)
    {
        AddClosedObject(schema, new JsonObject
        {
            ["text"] = new JsonObject { ["type"] = "string" },
            ["bits"] = new JsonObject { ["type"] = "string", ["pattern"] = $"^0x[0-9A-F]{{{hexDigits}}}$" },
            ["number"] = new JsonObject { ["type"] = "number" },
        }, "text", "bits");
        schema["description"] = $"Bits-authoritative exact {description} representation. Finite values require a matching number; non-finite values omit it.";
    }

    /// <summary>Creates a repeated exact float-vector property map.</summary>
    /// <param name="names">The ordered vector component names.</param>
    /// <returns>The component schema map.</returns>
    private static JsonObject FloatVectorProperties(params string[] names)
    {
        var properties = new JsonObject();
        foreach (var name in names)
        {
            properties[name] = ReferenceSchema("native.single");
        }

        return properties;
    }

    /// <summary>Creates the complete translated-string inner value schema.</summary>
    /// <returns>The closed language-map schema.</returns>
    private static JsonObject CreateTranslatedStringValueSchema()
    {
        return new JsonObject
        {
            ["type"] = "object",
            ["properties"] = new JsonObject
            {
                ["targetLanguage"] = new JsonObject { ["type"] = "string", ["minLength"] = 1, ["maxLength"] = 1048576 },
                ["value"] = NullableSchema(new JsonObject { ["type"] = "string", ["maxLength"] = 1048576 }),
                ["translations"] = new JsonObject
                {
                    ["type"] = "array",
                    ["maxItems"] = 65536,
                    ["x-unique-property"] = "language",
                    ["items"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JsonObject
                        {
                            ["language"] = new JsonObject { ["type"] = "string", ["minLength"] = 1, ["maxLength"] = 1048576 },
                            ["value"] = new JsonObject { ["type"] = "string", ["maxLength"] = 1048576 },
                        },
                        ["required"] = new JsonArray("language", "value"),
                        ["additionalProperties"] = false,
                    },
                },
            },
            ["required"] = new JsonArray("targetLanguage", "value", "translations"),
            ["additionalProperties"] = false,
        };
    }

    /// <summary>Creates the exact redundant System.Drawing.Color property map.</summary>
    /// <returns>The complete color property schema.</returns>
    private static JsonObject CreateColorProperties()
    {
        return new JsonObject
        {
            ["argb"] = CreateNumericSchema(typeof(int)),
            ["a"] = CreateNumericSchema(typeof(byte)),
            ["r"] = CreateNumericSchema(typeof(byte)),
            ["g"] = CreateNumericSchema(typeof(byte)),
            ["b"] = CreateNumericSchema(typeof(byte)),
            ["isEmpty"] = new JsonObject { ["type"] = "boolean" },
            ["isKnownColor"] = new JsonObject { ["type"] = "boolean" },
            ["isNamedColor"] = new JsonObject { ["type"] = "boolean" },
            ["isSystemColor"] = new JsonObject { ["type"] = "boolean" },
            ["name"] = new JsonObject { ["type"] = "string", ["maxLength"] = 1048576 },
        };
    }

    /// <summary>Emits the exact generated-reader default for an empty translated string.</summary>
    /// <returns>Compact wrapper JSON.</returns>
    private static string EmitTranslatedStringDefault()
    {
        return JsonSerializer.Serialize(new
        {
            type = NativeTypeDiscriminator("Mutagen.Bethesda.Strings.ITranslatedStringGetter"),
            value = new
            {
                targetLanguage = "English",
                value = (string?)null,
                translations = Array.Empty<object>(),
            },
        }).Replace("\"type\"", "\"$type\"", StringComparison.Ordinal);
    }

    /// <summary>Emits the exact System.Drawing.Color.Empty writer representation.</summary>
    /// <returns>Compact color JSON.</returns>
    private static string EmitColorDefault()
    {
        var value = System.Drawing.Color.Empty;
        return JsonSerializer.Serialize(new
        {
            argb = value.ToArgb(),
            a = value.A,
            r = value.R,
            g = value.G,
            b = value.B,
            isEmpty = value.IsEmpty,
            isKnownColor = value.IsKnownColor,
            isNamedColor = value.IsNamedColor,
            isSystemColor = value.IsSystemColor,
            name = value.Name,
        });
    }
}
