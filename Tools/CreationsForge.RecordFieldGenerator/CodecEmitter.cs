using System.Reflection;
using System.Text;
using System.Text.Json;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.RecordFieldGenerator;

/// <summary>
/// Emits direct typed Starfield nested-field writers and comparers from installed generated metadata.
/// </summary>
internal sealed partial class CodecEmitter
{
    /// <summary>The installed package version represented by the generated source.</summary>
    private const string PackageVersion = "0.55.0-alpha.53";

    /// <summary>The maximum concrete visitor count emitted into one generated source file.</summary>
    private const int TypesPerFile = 16;

    /// <summary>All concrete installed Starfield model classes.</summary>
    private readonly Type[] ConcreteTypes;

    /// <summary>All concrete indexed types reachable from the owned Mutagen roots.</summary>
    private readonly IReadOnlyList<RecordTypeModel> Models;

    /// <summary>Mutable model types keyed by their generated getter interface.</summary>
    private readonly IReadOnlyDictionary<Type, Type> MutableByGetter;

    /// <summary>Abstract model types that require generated concrete dispatch.</summary>
    private readonly IReadOnlyList<Type> PolymorphicTypes;

    /// <summary>
    /// Initializes the deterministic emitter from the currently loaded Mutagen Starfield assembly.
    /// </summary>
    internal CodecEmitter()
    {
        var assembly = typeof(FormList).Assembly;
        ConcreteTypes = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && !type.IsGenericTypeDefinition)
            .ToArray();

        var visited = new HashSet<Type>();
        var polymorphic = new HashSet<Type>();
        foreach (var root in new[] { typeof(AComponent), typeof(Condition), typeof(ConditionData) })
        {
            Visit(root, visited, polymorphic);
        }

        Models = visited
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .Select(CreateModel)
            .ToArray();
        MutableByGetter = Models.ToDictionary(model => model.GetterType, model => model.MutableType);
        PolymorphicTypes = polymorphic.OrderBy(type => type.FullName, StringComparer.Ordinal).ToArray();

        ValidateCoverage();
    }

    /// <summary>Initializes a metadata-only visitor over one installed game's concrete record model types.</summary>
    /// <param name="majorRecordType">The game's abstract native major-record base class.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="majorRecordType"/> is <see langword="null"/>.</exception>
    internal CodecEmitter(Type majorRecordType)
    {
        ArgumentNullException.ThrowIfNull(majorRecordType);
        ConcreteTypes = majorRecordType.Assembly.GetTypes()
            .Where(static type => type.IsClass && !type.IsAbstract && !type.IsGenericTypeDefinition)
            .ToArray();
        Models = Array.Empty<RecordTypeModel>();
        MutableByGetter = new Dictionary<Type, Type>();
        PolymorphicTypes = Array.Empty<Type>();
    }

    /// <summary>
    /// Emits and validates the complete production source and coverage manifest before writing exact owned paths.
    /// </summary>
    /// <param name="repositoryRoot">The CreationsForge repository root.</param>
    internal void Generate(string repositoryRoot)
    {
        var productionRoot = Path.Combine(repositoryRoot, "CreationsForge.Starfield", "PluginAdapter", "RecordInspection", "Nested");
        var generatedRoot = Path.Combine(productionRoot, "Generated");
        var partitions = Models.Chunk(TypesPerFile).ToArray();
        var artifacts = new List<KeyValuePair<string, string>>
        {
            new(Path.Combine(productionRoot, "StarfieldNestedFieldCodec.cs"), EmitEntryPoints()),
            new(Path.Combine(productionRoot, "StarfieldNestedLeafCodec.cs"), EmitLeaves()),
            new(Path.Combine(productionRoot, "StarfieldNestedFieldReader.cs"), EmitReaderEntryPoints()),
            new(Path.Combine(productionRoot, "StarfieldNestedLeafReader.cs"), EmitReaderLeaves()),
            new(Path.Combine(generatedRoot, "StarfieldNestedDispatch.g.cs"), EmitDispatch()),
            new(Path.Combine(generatedRoot, "StarfieldNestedReaderDispatch.g.cs"), EmitReaderDispatch()),
            new(Path.Combine(generatedRoot, "StarfieldGeneratedRecordFieldSchema.g.cs"), EmitSchemaCatalog()),
        };
        for (var index = 0; index < partitions.Length; index++)
        {
            artifacts.Add(new KeyValuePair<string, string>(
                Path.Combine(generatedRoot, $"StarfieldNestedVisitors.{index:D2}.g.cs"),
                EmitVisitors(partitions[index])));
            artifacts.Add(new KeyValuePair<string, string>(
                Path.Combine(generatedRoot, $"StarfieldNestedReaders.{index:D2}.g.cs"),
                EmitReaders(partitions[index])));
        }

        artifacts.Add(new KeyValuePair<string, string>(
            Path.Combine(productionRoot, "StarfieldRecordFieldManifest.json"),
            EmitManifest()));
        ValidateGeneratedOutputs(generatedRoot, artifacts);
        foreach (var artifact in artifacts)
        {
            WriteFile(artifact.Key, artifact.Value);
        }

        Console.WriteLine($"Generated {Models.Count} concrete typed visitors across {partitions.Length} files and {PolymorphicTypes.Count} polymorphic dispatchers.");
        Console.WriteLine(
            "Construction coverage: "
            + string.Join(
                ", ",
                Models.GroupBy(static model => model.Construction.Kind)
                    .OrderBy(static group => group.Key)
                    .Select(static group => $"{group.Key}={group.Count()}")));
    }

    /// <summary>
    /// Traverses concrete Mutagen indexed fields and records abstract dispatch boundaries.
    /// </summary>
    /// <param name="type">The mutable Mutagen type to visit.</param>
    /// <param name="visited">Concrete indexed types already included.</param>
    /// <param name="polymorphic">Abstract Mutagen types requiring concrete dispatch.</param>
    private void Visit(Type type, HashSet<Type> visited, HashSet<Type> polymorphic)
    {
        if (Nullable.GetUnderlyingType(type) is { } nullable)
        {
            Visit(nullable, visited, polymorphic);
            return;
        }

        if (IsMutableLeaf(type))
        {
            if (IsCollection(type) || IsArray2d(type))
            {
                Visit(type.GetGenericArguments()[0], visited, polymorphic);
            }

            return;
        }

        if (type.IsAbstract || type.IsInterface)
        {
            polymorphic.Add(type);
            var derivedTypes = ConcreteTypes.Where(type.IsAssignableFrom).ToArray();
            if (derivedTypes.Length == 0)
            {
                throw new InvalidOperationException($"No concrete Mutagen types implement {type.FullName}.");
            }

            foreach (var derived in derivedTypes)
            {
                Visit(derived, visited, polymorphic);
            }

            return;
        }

        var fieldIndex = GetFieldIndex(type);
        if (fieldIndex is null)
        {
            throw new InvalidOperationException($"Unclassified non-indexed Mutagen type {type.FullName}.");
        }

        if (!visited.Add(type))
        {
            return;
        }

        foreach (var field in GetMutableFields(type, fieldIndex))
        {
            Visit(field.PropertyType, visited, polymorphic);
        }
    }

    /// <summary>
    /// Creates one concrete visitor model from Mutagen FieldIndex and getter metadata.
    /// </summary>
    /// <param name="mutableType">The installed concrete mutable Mutagen class.</param>
    /// <returns>A closed visitor model with fields in Mutagen index order.</returns>
    private static RecordTypeModel CreateModel(Type mutableType)
    {
        var getterType = FindGetter(mutableType)
            ?? throw new InvalidOperationException($"No generated getter interface found for {mutableType.FullName}.");
        var fieldIndex = GetFieldIndex(mutableType)
            ?? throw new InvalidOperationException($"No FieldIndex found for {mutableType.FullName}.");
        var nullability = new NullabilityInfoContext();
        var fields = Enum.GetNames(fieldIndex)
            .OrderBy(name => Convert.ToUInt16(Enum.Parse(fieldIndex, name)))
            .Select(name =>
            {
                var mutableProperty = mutableType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public)
                    ?? throw new InvalidOperationException($"No mutable property {mutableType.FullName}.{name}.");
                var getterProperty = FindInterfaceProperty(getterType, name)
                    ?? throw new InvalidOperationException($"No getter property {getterType.FullName}.{name}.");
                var fieldNullability = nullability.Create(getterProperty);
                return new RecordFieldModel(
                    name,
                    Convert.ToUInt16(Enum.Parse(fieldIndex, name)),
                    mutableProperty,
                    getterProperty,
                    fieldNullability,
                    CreateConstructionModel(
                        mutableProperty.PropertyType,
                        getterProperty.PropertyType,
                        fieldNullability,
                        $"{mutableType.FullName}.{name}"));
            })
            .ToArray();
        return new RecordTypeModel(mutableType, getterType, fields, CreateTypeConstructionModel(mutableType));
    }

    /// <summary>
    /// Rejects omitted roots, fields, getter shapes, and unclassified terminal contracts before generation.
    /// </summary>
    private void ValidateCoverage()
    {
        var components = Models.Where(model => typeof(AComponent).IsAssignableFrom(model.MutableType)).ToArray();
        var conditionData = Models.Where(model => typeof(ConditionData).IsAssignableFrom(model.MutableType)).ToArray();
        if (components.Length != 66 || conditionData.Length != 608)
        {
            throw new InvalidOperationException($"Installed root coverage changed: components={components.Length}, conditionData={conditionData.Length}.");
        }

        if (!Models.Any(model => model.MutableType == typeof(ConditionFloat))
            || !Models.Any(model => model.MutableType == typeof(ConditionGlobal)))
        {
            throw new InvalidOperationException("The two concrete Condition roots were omitted.");
        }

        foreach (var model in Models)
        {
            foreach (var field in model.Fields)
            {
                ValidateValueShape(field.MutableProperty.PropertyType, field.GetterProperty.PropertyType, $"{model.MutableType.FullName}.{field.Name}");
            }
        }
    }

    /// <summary>
    /// Validates that one concrete getter value shape has a direct generated handling policy.
    /// </summary>
    /// <param name="mutableType">The mutable indexed field type.</param>
    /// <param name="getterType">The corresponding getter field type.</param>
    /// <param name="location">The record field location used in failures.</param>
    private void ValidateValueShape(Type mutableType, Type getterType, string location)
    {
        var mutableNullable = Nullable.GetUnderlyingType(mutableType);
        var getterNullable = Nullable.GetUnderlyingType(getterType);
        if (mutableNullable is not null || getterNullable is not null)
        {
            if (mutableNullable is null || getterNullable is null)
            {
                throw new InvalidOperationException($"Nullable shape mismatch at {location}: {mutableType} / {getterType}.");
            }

            ValidateValueShape(mutableNullable, getterNullable, location);
            return;
        }

        if (IsGetterLeaf(getterType))
        {
            return;
        }

        if (IsCollection(getterType))
        {
            if (!IsCollection(mutableType))
            {
                throw new InvalidOperationException($"Collection shape mismatch at {location}: {mutableType} / {getterType}.");
            }

            ValidateValueShape(mutableType.GetGenericArguments()[0], getterType.GetGenericArguments()[0], location + "[]");
            return;
        }

        if (IsArray2d(getterType))
        {
            if (!IsArray2d(mutableType))
            {
                throw new InvalidOperationException($"Array2d shape mismatch at {location}: {mutableType} / {getterType}.");
            }

            ValidateValueShape(mutableType.GetGenericArguments()[0], getterType.GetGenericArguments()[0], location + "[,]");
            return;
        }

        if (getterType.Namespace == typeof(FormList).Namespace && getterType.IsInterface)
        {
            var resolvedMutable = ResolveMutable(getterType);
            if (resolvedMutable != mutableType)
            {
                throw new InvalidOperationException($"Plugin getter mapping mismatch at {location}: {mutableType} / {getterType} resolves {resolvedMutable}.");
            }

            return;
        }

        throw new InvalidOperationException($"Unhandled getter value shape at {location}: {mutableType} / {getterType}.");
    }

    /// <summary>
    /// Emits every root and abstract Mutagen concrete-type dispatcher.
    /// </summary>
    /// <returns>Complete generated dispatch source.</returns>
    private string EmitDispatch()
    {
        var output = GeneratedHeader();
        foreach (var polymorphic in PolymorphicTypes)
        {
            EmitPolymorphicDispatcher(output, polymorphic);
        }

        return Finish(output);
    }

    /// <summary>
    /// Emits one source partition of concrete direct typed writers and comparers.
    /// </summary>
    /// <param name="models">Concrete visitor models in deterministic order.</param>
    /// <returns>Complete generated visitor source.</returns>
    private string EmitVisitors(IEnumerable<RecordTypeModel> models)
    {
        var output = GeneratedHeader();
        foreach (var model in models)
        {
            EmitConcreteWriter(output, model);
            EmitConcreteComparer(output, model);
        }

        return Finish(output);
    }

    /// <summary>
    /// Emits ordered direct type dispatch for one abstract Mutagen model boundary.
    /// </summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="polymorphicType">The abstract mutable Mutagen model type.</param>
    private void EmitPolymorphicDispatcher(StringBuilder output, Type polymorphicType)
    {
        var getter = FindGetter(polymorphicType)
            ?? throw new InvalidOperationException($"No getter interface found for abstract type {polymorphicType.FullName}.");
        var derived = Models
            .Where(model => polymorphicType.IsAssignableFrom(model.MutableType))
            .OrderBy(model => model.MutableType.FullName, StringComparer.Ordinal)
            .ToArray();
        if (derived.Length == 0)
        {
            throw new InvalidOperationException($"No generated concrete visitor derives from {polymorphicType.FullName}.");
        }

        output.AppendLine($"    /// <summary>Writes a complete concrete {polymorphicType.Name} value through static generated type dispatch.</summary>");
        output.AppendLine("    /// <param name=\"writer\">The caller-owned JSON writer.</param>");
        output.AppendLine("    /// <param name=\"value\">The concrete record getter.</param>");
        output.AppendLine("    /// <param name=\"cancellationToken\">A token observed throughout traversal.</param>");
        output.AppendLine("    /// <param name=\"context\">The per-call write mode and validation state, or null for read-view output.</param>");
        output.AppendLine($"    private static void WritePolymorphic{polymorphicType.Name}(Utf8JsonWriter writer, {TypeName(getter)} value, CancellationToken cancellationToken, RecordJsonWriteContext? context)");
        output.AppendLine("    {");
        output.AppendLine("        cancellationToken.ThrowIfCancellationRequested();");
        for (var index = 0; index < derived.Length; index++)
        {
            var model = derived[index];
            output.AppendLine($"        if (value is {TypeName(model.GetterType)} typed{index})");
            output.AppendLine("        {");
            output.AppendLine($"            Write{model.MutableType.Name}(writer, typed{index}, cancellationToken, context);");
            output.AppendLine("            return;");
            output.AppendLine("        }");
            output.AppendLine();
        }

        output.AppendLine($"        throw new NotSupportedException(\"The installed Mutagen {polymorphicType.Name} implementation is not covered by the generated Starfield record field manifest.\");");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine($"    /// <summary>Compares two concrete {polymorphicType.Name} values after exact generated type discrimination.</summary>");
        output.AppendLine("    /// <param name=\"before\">The prior record getter, or null.</param>");
        output.AppendLine("    /// <param name=\"after\">The resulting record getter, or null.</param>");
        output.AppendLine("    /// <param name=\"path\">The stable record field path.</param>");
        output.AppendLine("    /// <param name=\"changes\">The caller-owned ordered change collection.</param>");
        output.AppendLine("    /// <param name=\"cancellationToken\">A token observed throughout traversal.</param>");
        output.AppendLine($"    private static void ComparePolymorphic{polymorphicType.Name}({TypeName(getter)}? before, {TypeName(getter)}? after, string path, ICollection<SemanticChangeDescriptor> changes, CancellationToken cancellationToken)");
        output.AppendLine("    {");
        output.AppendLine("        ArgumentException.ThrowIfNullOrWhiteSpace(path);");
        output.AppendLine("        ArgumentNullException.ThrowIfNull(changes);");
        output.AppendLine("        cancellationToken.ThrowIfCancellationRequested();");
        output.AppendLine("        if (before is null || after is null)");
        output.AppendLine("        {");
        output.AppendLine("            CompareValue(path, before is null && after is null, changes, cancellationToken);");
        output.AppendLine("            return;");
        output.AppendLine("        }");
        output.AppendLine();
        for (var index = 0; index < derived.Length; index++)
        {
            var model = derived[index];
            output.AppendLine($"        if (before is {TypeName(model.GetterType)} beforeTyped{index} && after is {TypeName(model.GetterType)} afterTyped{index})");
            output.AppendLine("        {");
            output.AppendLine($"            Compare{model.MutableType.Name}(beforeTyped{index}, afterTyped{index}, path, changes, cancellationToken);");
            output.AppendLine("            return;");
            output.AppendLine("        }");
            output.AppendLine();
        }

        output.AppendLine($"        _ = Get{polymorphicType.Name}TypeName(before);");
        output.AppendLine($"        _ = Get{polymorphicType.Name}TypeName(after);");
        output.AppendLine("        CompareValue(path + \".$type\", false, changes, cancellationToken);");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine($"    /// <summary>Returns the generated canonical concrete type name for one {polymorphicType.Name} getter.</summary>");
        output.AppendLine("    /// <param name=\"value\">The concrete record getter.</param>");
        output.AppendLine("    /// <returns>The canonical package-scoped Mutagen type name.</returns>");
        output.AppendLine($"    private static string Get{polymorphicType.Name}TypeName({TypeName(getter)} value)");
        output.AppendLine("    {");
        foreach (var model in derived)
        {
            output.AppendLine($"        if (value is {TypeName(model.GetterType)})");
            output.AppendLine("        {");
            output.AppendLine($"            return RecordTypeNamespace + \"{Escape(model.MutableType.FullName!)}\";");
            output.AppendLine("        }");
            output.AppendLine();
        }

        output.AppendLine($"        throw new NotSupportedException(\"The installed Mutagen {polymorphicType.Name} implementation is not covered by the generated Starfield record field manifest.\");");
        output.AppendLine("    }");
        output.AppendLine();
    }

    /// <summary>
    /// Emits one complete direct typed JSON writer in Mutagen FieldIndex order.
    /// </summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="model">The concrete Mutagen visitor model.</param>
    private void EmitConcreteWriter(StringBuilder output, RecordTypeModel model)
    {
        output.AppendLine($"    /// <summary>Writes every indexed field of Mutagen {model.MutableType.Name} in FieldIndex order.</summary>");
        output.AppendLine("    /// <param name=\"writer\">The caller-owned JSON writer.</param>");
        output.AppendLine("    /// <param name=\"value\">The concrete record getter.</param>");
        output.AppendLine("    /// <param name=\"cancellationToken\">A token observed throughout traversal.</param>");
        output.AppendLine("    /// <param name=\"context\">The per-call write mode and validation state, or null for read-view output.</param>");
        output.AppendLine($"    private static void Write{model.MutableType.Name}(Utf8JsonWriter writer, {TypeName(model.GetterType)} value, CancellationToken cancellationToken, RecordJsonWriteContext? context)");
        output.AppendLine("    {");
        output.AppendLine("        cancellationToken.ThrowIfCancellationRequested();");
        output.AppendLine("        writer.WriteStartObject();");
        output.AppendLine($"        WriteType(writer, \"{Escape(model.MutableType.FullName!)}\");");
        if (typeof(ConditionData).IsAssignableFrom(model.MutableType)
            && !model.Fields.Any(static field => string.Equals(field.Name, "Function", StringComparison.Ordinal)))
        {
            output.AppendLine("        writer.WritePropertyName(\"Function\");");
            output.AppendLine("        writer.WriteNumberValue((System.UInt16)((IConditionDataGetter)value).Function);");
        }

        foreach (var field in model.Fields)
        {
            output.AppendLine($"        writer.WritePropertyName(\"{Escape(field.Name)}\");");
            EmitWriteValue(output, field.MutableProperty.PropertyType, field.GetterProperty.PropertyType, $"value.{field.Name}", "        ");
        }

        output.AppendLine("        writer.WriteEndObject();");
        output.AppendLine("    }");
        output.AppendLine();
    }

    /// <summary>
    /// Emits one complete direct typed Mutagen comparer in FieldIndex order.
    /// </summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="model">The concrete Mutagen visitor model.</param>
    private void EmitConcreteComparer(StringBuilder output, RecordTypeModel model)
    {
        output.AppendLine($"    /// <summary>Compares every indexed field of Mutagen {model.MutableType.Name} without Mutagen equality or JSON authority.</summary>");
        output.AppendLine("    /// <param name=\"before\">The prior concrete record getter.</param>");
        output.AppendLine("    /// <param name=\"after\">The resulting concrete record getter.</param>");
        output.AppendLine("    /// <param name=\"path\">The stable Mutagen object path.</param>");
        output.AppendLine("    /// <param name=\"changes\">The caller-owned ordered change collection.</param>");
        output.AppendLine("    /// <param name=\"cancellationToken\">A token observed throughout traversal.</param>");
        output.AppendLine($"    private static void Compare{model.MutableType.Name}({TypeName(model.GetterType)} before, {TypeName(model.GetterType)} after, string path, ICollection<SemanticChangeDescriptor> changes, CancellationToken cancellationToken)");
        output.AppendLine("    {");
        output.AppendLine("        cancellationToken.ThrowIfCancellationRequested();");
        if (typeof(ConditionData).IsAssignableFrom(model.MutableType)
            && !model.Fields.Any(static field => string.Equals(field.Name, "Function", StringComparison.Ordinal)))
        {
            output.AppendLine("        CompareValue(path + \".Function\", ((IConditionDataGetter)before).Function == ((IConditionDataGetter)after).Function, changes, cancellationToken);");
        }

        foreach (var field in model.Fields)
        {
            EmitCompareValue(
                output,
                field.MutableProperty.PropertyType,
                field.GetterProperty.PropertyType,
                $"before.{field.Name}",
                $"after.{field.Name}",
                $"path + \".{Escape(field.Name)}\"",
                "        ",
                false,
                null,
                null);
        }

        output.AppendLine("    }");
        output.AppendLine();
    }

    /// <summary>
    /// Emits direct JSON output for one closed getter value shape.
    /// </summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="mutableType">The mutable field type used to resolve nested Mutagen models.</param>
    /// <param name="getterType">The getter field type used by production code.</param>
    /// <param name="value">The static getter expression.</param>
    /// <param name="indent">The current source indentation.</param>
    private void EmitWriteValue(StringBuilder output, Type mutableType, Type getterType, string value, string indent)
    {
        var mutableNullable = Nullable.GetUnderlyingType(mutableType);
        var getterNullable = Nullable.GetUnderlyingType(getterType);
        if (mutableNullable is not null || getterNullable is not null)
        {
            if (mutableNullable is null || getterNullable is null)
            {
                throw new InvalidOperationException($"Nullable generation mismatch for {mutableType} / {getterType}.");
            }

            output.AppendLine($"{indent}if ({value}.HasValue)");
            output.AppendLine($"{indent}{{");
            EmitWriteValue(output, mutableNullable, getterNullable, value + ".Value", indent + "    ");
            output.AppendLine($"{indent}}}");
            output.AppendLine($"{indent}else");
            output.AppendLine($"{indent}{{");
            output.AppendLine($"{indent}    writer.WriteNullValue();");
            output.AppendLine($"{indent}}}");
            return;
        }

        if (getterType == typeof(string))
        {
            output.AppendLine($"{indent}RecordJsonLeafWriter.WriteString(writer, {value}, context);");
        }
        else if (getterType == typeof(bool))
        {
            output.AppendLine($"{indent}writer.WriteBooleanValue({value});");
        }
        else if (getterType == typeof(float))
        {
            output.AppendLine($"{indent}RecordJsonLeafWriter.WriteSingle(writer, {value});");
        }
        else if (getterType == typeof(double))
        {
            output.AppendLine($"{indent}RecordJsonLeafWriter.WriteDouble(writer, {value});");
        }
        else if (getterType.IsEnum)
        {
            output.AppendLine($"{indent}writer.WriteNumberValue(({TypeName(Enum.GetUnderlyingType(getterType))}){value});");
        }
        else if (getterType.IsPrimitive || getterType == typeof(decimal))
        {
            output.AppendLine($"{indent}writer.WriteNumberValue({value});");
        }
        else if (getterType == typeof(Guid))
        {
            output.AppendLine($"{indent}writer.WriteStringValue({value}.ToString(\"D\"));");
        }
        else if (getterType == typeof(System.Drawing.Color))
        {
            output.AppendLine($"{indent}WriteColor(writer, {value}, context);");
        }
        else if (getterType.FullName == "Noggog.P2Float")
        {
            output.AppendLine($"{indent}writer.WriteStartObject();");
            output.AppendLine($"{indent}writer.WritePropertyName(\"X\");");
            output.AppendLine($"{indent}RecordJsonLeafWriter.WriteSingle(writer, {value}.X);");
            output.AppendLine($"{indent}writer.WritePropertyName(\"Y\");");
            output.AppendLine($"{indent}RecordJsonLeafWriter.WriteSingle(writer, {value}.Y);");
            output.AppendLine($"{indent}writer.WriteEndObject();");
        }
        else if (getterType.FullName == "Noggog.P2Int")
        {
            output.AppendLine($"{indent}writer.WriteStartObject();");
            output.AppendLine($"{indent}writer.WriteNumber(\"X\", {value}.X);");
            output.AppendLine($"{indent}writer.WriteNumber(\"Y\", {value}.Y);");
            output.AppendLine($"{indent}writer.WriteEndObject();");
        }
        else if (getterType.FullName == "Noggog.P3Float")
        {
            output.AppendLine($"{indent}writer.WriteStartObject();");
            output.AppendLine($"{indent}writer.WritePropertyName(\"X\");");
            output.AppendLine($"{indent}RecordJsonLeafWriter.WriteSingle(writer, {value}.X);");
            output.AppendLine($"{indent}writer.WritePropertyName(\"Y\");");
            output.AppendLine($"{indent}RecordJsonLeafWriter.WriteSingle(writer, {value}.Y);");
            output.AppendLine($"{indent}writer.WritePropertyName(\"Z\");");
            output.AppendLine($"{indent}RecordJsonLeafWriter.WriteSingle(writer, {value}.Z);");
            output.AppendLine($"{indent}writer.WriteEndObject();");
        }
        else if (getterType.FullName == "Mutagen.Bethesda.Strings.ITranslatedStringGetter")
        {
            output.AppendLine($"{indent}WriteTranslatedString(writer, {value}, cancellationToken, context);");
        }
        else if (IsMemory(getterType))
        {
            output.AppendLine($"{indent}RecordJsonLeafWriter.WriteBytes(writer, {value}.ToArray(), cancellationToken);");
        }
        else if (IsFormLink(getterType))
        {
            output.AppendLine($"{indent}WriteFormLink(writer, {value}, \"{Escape(TypeName(getterType))}\", context);");
        }
        else if (IsFormLinkOrIndex(getterType))
        {
            EmitWriteFormLinkOrIndex(output, getterType, value, indent);
        }
        else if (IsAssetLink(getterType))
        {
            output.AppendLine($"{indent}WriteAssetLink(writer, {value}, \"{Escape(TypeName(getterType))}\", context);");
        }
        else if (IsCollection(getterType))
        {
            EmitWriteCollection(output, mutableType, getterType, value, indent);
        }
        else if (IsArray2d(getterType))
        {
            EmitWriteArray2d(output, mutableType, getterType, value, indent);
        }
        else if (getterType.Namespace == typeof(FormList).Namespace && getterType.IsInterface)
        {
            EmitWriteNested(output, mutableType, getterType, value, indent);
        }
        else
        {
            throw new InvalidOperationException($"Unhandled writer shape {mutableType} / {getterType}.");
        }
    }

    /// <summary>
    /// Emits complete owner-sensitive form-link-or-index wrapper output.
    /// </summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="getterType">The closed link-or-index getter type.</param>
    /// <param name="value">The static getter expression.</param>
    /// <param name="indent">The current source indentation.</param>
    private static void EmitWriteFormLinkOrIndex(StringBuilder output, Type getterType, string value, string indent)
    {
        output.AppendLine($"{indent}if ({value} is null)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    writer.WriteNullValue();");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}else");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    writer.WriteStartObject();");
        output.AppendLine($"{indent}    WriteType(writer, \"{Escape(TypeName(getterType))}\");");
        output.AppendLine($"{indent}    writer.WriteBoolean(\"usesLink\", {value}.UsesLink());");
        output.AppendLine($"{indent}    writer.WriteBoolean(\"usesAlias\", {value}.UsesAlias());");
        output.AppendLine($"{indent}    writer.WriteBoolean(\"usesPackageData\", {value}.UsesPackageData());");
        output.AppendLine($"{indent}    if ({value}.Index.HasValue)");
        output.AppendLine($"{indent}    {{");
        output.AppendLine($"{indent}        writer.WriteNumber(\"index\", {value}.Index.Value);");
        output.AppendLine($"{indent}    }}");
        output.AppendLine($"{indent}    else");
        output.AppendLine($"{indent}    {{");
        output.AppendLine($"{indent}        writer.WriteNull(\"index\");");
        output.AppendLine($"{indent}    }}");
        output.AppendLine($"{indent}    writer.WritePropertyName(\"link\");");
        output.AppendLine($"{indent}    WriteFormLink(writer, {value}.Link, \"{Escape(TypeName(getterType.GetProperty("Link")!.PropertyType))}\", context);");
        output.AppendLine($"{indent}    writer.WriteEndObject();");
        output.AppendLine($"{indent}}}");
    }

    /// <summary>
    /// Emits ordered one-dimensional collection output without collapsing null or duplicates.
    /// </summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="mutableType">The mutable collection type used to resolve its element model.</param>
    /// <param name="getterType">The getter collection type used by production code.</param>
    /// <param name="value">The static collection getter expression.</param>
    /// <param name="indent">The current source indentation.</param>
    private void EmitWriteCollection(StringBuilder output, Type mutableType, Type getterType, string value, string indent)
    {
        var mutableElement = mutableType.GetGenericArguments()[0];
        var getterElement = getterType.GetGenericArguments()[0];
        output.AppendLine($"{indent}if ({value} is null)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    writer.WriteNullValue();");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}else");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    writer.WriteStartArray();");
        output.AppendLine($"{indent}    for (var index = 0; index < {value}.Count; index++)");
        output.AppendLine($"{indent}    {{");
        output.AppendLine($"{indent}        cancellationToken.ThrowIfCancellationRequested();");
        EmitWriteValue(output, mutableElement, getterElement, value + "[index]", indent + "        ");
        output.AppendLine($"{indent}    }}");
        output.AppendLine($"{indent}    writer.WriteEndArray();");
        output.AppendLine($"{indent}}}");
    }

    /// <summary>
    /// Emits deterministic row-major two-dimensional collection output.
    /// </summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="mutableType">The mutable array type used to resolve its element model.</param>
    /// <param name="getterType">The getter array type used by production code.</param>
    /// <param name="value">The static array getter expression.</param>
    /// <param name="indent">The current source indentation.</param>
    private void EmitWriteArray2d(StringBuilder output, Type mutableType, Type getterType, string value, string indent)
    {
        var mutableElement = mutableType.GetGenericArguments()[0];
        var getterElement = getterType.GetGenericArguments()[0];
        output.AppendLine($"{indent}if ({value} is null)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    writer.WriteNullValue();");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}else");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    writer.WriteStartObject();");
        output.AppendLine($"{indent}    writer.WriteNumber(\"width\", {value}.Width);");
        output.AppendLine($"{indent}    writer.WriteNumber(\"height\", {value}.Height);");
        output.AppendLine($"{indent}    writer.WriteStartArray(\"rows\");");
        output.AppendLine($"{indent}    for (var y = 0; y < {value}.Height; y++)");
        output.AppendLine($"{indent}    {{");
        output.AppendLine($"{indent}        cancellationToken.ThrowIfCancellationRequested();");
        output.AppendLine($"{indent}        writer.WriteStartArray();");
        output.AppendLine($"{indent}        for (var x = 0; x < {value}.Width; x++)");
        output.AppendLine($"{indent}        {{");
        EmitWriteValue(output, mutableElement, getterElement, value + "[x, y]", indent + "            ");
        output.AppendLine($"{indent}        }}");
        output.AppendLine($"{indent}        writer.WriteEndArray();");
        output.AppendLine($"{indent}    }}");
        output.AppendLine($"{indent}    writer.WriteEndArray();");
        output.AppendLine($"{indent}    writer.WriteEndObject();");
        output.AppendLine($"{indent}}}");
    }

    /// <summary>
    /// Emits null-aware direct nested model output or generated polymorphic dispatch.
    /// </summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="mutableType">The mutable nested type used to select concrete or polymorphic dispatch.</param>
    /// <param name="getterType">The getter nested type used by production code.</param>
    /// <param name="value">The static nested getter expression.</param>
    /// <param name="indent">The current source indentation.</param>
    private static void EmitWriteNested(StringBuilder output, Type mutableType, Type getterType, string value, string indent)
    {
        output.AppendLine($"{indent}if ({value} is null)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    writer.WriteNullValue();");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}else");
        output.AppendLine($"{indent}{{");
        var call = mutableType.IsAbstract || mutableType.IsInterface ? "WritePolymorphic" + mutableType.Name : "Write" + mutableType.Name;
        output.AppendLine($"{indent}    {call}(writer, {value}, cancellationToken, context);");
        output.AppendLine($"{indent}}}");
    }

    /// <summary>
    /// Emits exact typed comparison for one closed getter value shape.
    /// </summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="mutableType">The mutable field type used to resolve nested models.</param>
    /// <param name="getterType">The getter field type used by production code.</param>
    /// <param name="before">The prior static getter expression.</param>
    /// <param name="after">The resulting static getter expression.</param>
    /// <param name="path">The generated expression producing the stable record field path.</param>
    /// <param name="indent">The current source indentation.</param>
    /// <param name="collectionElement">Whether a scalar mismatch should be an ordered item change.</param>
    /// <param name="collectionPath">The parent collection path expression for item descriptors.</param>
    /// <param name="collectionIndex">The current collection index expression.</param>
    private void EmitCompareValue(
        StringBuilder output,
        Type mutableType,
        Type getterType,
        string before,
        string after,
        string path,
        string indent,
        bool collectionElement,
        string? collectionPath,
        string? collectionIndex)
    {
        var mutableNullable = Nullable.GetUnderlyingType(mutableType);
        var getterNullable = Nullable.GetUnderlyingType(getterType);
        if (mutableNullable is not null || getterNullable is not null)
        {
            if (mutableNullable is null || getterNullable is null)
            {
                throw new InvalidOperationException($"Nullable comparison mismatch for {mutableType} / {getterType}.");
            }

            output.AppendLine($"{indent}if ({before}.HasValue != {after}.HasValue)");
            output.AppendLine($"{indent}{{");
            EmitMismatch(output, path, indent + "    ", collectionElement, collectionPath, collectionIndex);
            output.AppendLine($"{indent}}}");
            output.AppendLine($"{indent}else if ({before}.HasValue && {after}.HasValue)");
            output.AppendLine($"{indent}{{");
            EmitCompareValue(output, mutableNullable, getterNullable, before + ".Value", after + ".Value", path, indent + "    ", collectionElement, collectionPath, collectionIndex);
            output.AppendLine($"{indent}}}");
            return;
        }

        if (IsCollection(getterType))
        {
            EmitCompareCollection(output, mutableType, getterType, before, after, path, indent);
            return;
        }

        if (IsArray2d(getterType))
        {
            EmitCompareArray2d(output, mutableType, getterType, before, after, path, indent);
            return;
        }

        if (getterType.Namespace == typeof(FormList).Namespace && getterType.IsInterface && !IsFormLink(getterType) && !IsFormLinkOrIndex(getterType))
        {
            EmitCompareNested(output, mutableType, getterType, before, after, path, indent);
            return;
        }

        var equality = GetLeafEquality(getterType, before, after);
        if (collectionElement)
        {
            output.AppendLine($"{indent}if (!({equality}))");
            output.AppendLine($"{indent}{{");
            EmitMismatch(output, path, indent + "    ", true, collectionPath, collectionIndex);
            output.AppendLine($"{indent}}}");
        }
        else
        {
            output.AppendLine($"{indent}CompareValue({path}, {equality}, changes, cancellationToken);");
        }
    }

    /// <summary>
    /// Emits one scalar or ordered-item mismatch descriptor.
    /// </summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="path">The generated expression producing the exact value path.</param>
    /// <param name="indent">The current source indentation.</param>
    /// <param name="collectionElement">Whether the mismatch belongs to an ordered collection item.</param>
    /// <param name="collectionPath">The parent collection path expression, or null for a scalar.</param>
    /// <param name="collectionIndex">The collection index expression, or null for a scalar.</param>
    private static void EmitMismatch(StringBuilder output, string path, string indent, bool collectionElement, string? collectionPath, string? collectionIndex)
    {
        if (collectionElement)
        {
            if (collectionPath is null || collectionIndex is null)
            {
                throw new InvalidOperationException("Collection mismatch generation requires a parent path and index.");
            }

            output.AppendLine($"{indent}AddCollectionChange({collectionPath}, SemanticChangeKind.ItemChanged, {collectionIndex}, {collectionIndex}, changes, cancellationToken);");
        }
        else
        {
            output.AppendLine($"{indent}CompareValue({path}, false, changes, cancellationToken);");
        }
    }

    /// <summary>
    /// Returns the direct exact equality expression for one non-collection Mutagen leaf.
    /// </summary>
    /// <param name="getterType">The closed getter leaf type.</param>
    /// <param name="before">The prior static getter expression.</param>
    /// <param name="after">The resulting static getter expression.</param>
    /// <returns>A generated Boolean expression that applies the leaf's Mutagen semantic equality.</returns>
    private static string GetLeafEquality(Type getterType, string before, string after)
    {
        if (getterType == typeof(float))
        {
            return $"RecordSemanticComparer.BitwiseEquals({before}, {after})";
        }

        if (getterType == typeof(double))
        {
            return $"RecordSemanticComparer.BitwiseEquals({before}, {after})";
        }

        if (getterType == typeof(string))
        {
            return $"RecordSemanticComparer.StringEquals({before}, {after})";
        }

        if (getterType.FullName == "Mutagen.Bethesda.Strings.ITranslatedStringGetter")
        {
            return $"RecordSemanticComparer.TranslatedStringEquals({before}, {after}, cancellationToken)";
        }

        if (IsMemory(getterType))
        {
            return $"RecordSemanticComparer.BytesEqual({before}.ToArray(), {after}.ToArray(), cancellationToken)";
        }

        if (IsFormLink(getterType))
        {
            return $"RecordSemanticComparer.FormLinkEquals({before}, {after})";
        }

        if (IsFormLinkOrIndex(getterType))
        {
            return $"RecordSemanticComparer.FormLinkOrIndexEquals({before}, {after})";
        }

        if (IsAssetLink(getterType))
        {
            return $"RecordSemanticComparer.AssetLinkEquals({before}, {after})";
        }

        if (getterType.FullName == "Noggog.P2Float")
        {
            return $"RecordSemanticComparer.BitwiseEquals({before}.X, {after}.X) && RecordSemanticComparer.BitwiseEquals({before}.Y, {after}.Y)";
        }

        if (getterType.FullName == "Noggog.P2Int")
        {
            return $"{before}.X == {after}.X && {before}.Y == {after}.Y";
        }

        if (getterType.FullName == "Noggog.P3Float")
        {
            return $"RecordSemanticComparer.BitwiseEquals({before}.X, {after}.X) && RecordSemanticComparer.BitwiseEquals({before}.Y, {after}.Y) && RecordSemanticComparer.BitwiseEquals({before}.Z, {after}.Z)";
        }

        if (getterType == typeof(System.Drawing.Color))
        {
            return $"{before}.ToArgb() == {after}.ToArgb() && {before}.A == {after}.A && {before}.R == {after}.R && {before}.G == {after}.G && {before}.B == {after}.B && {before}.IsEmpty == {after}.IsEmpty && {before}.IsKnownColor == {after}.IsKnownColor && {before}.IsNamedColor == {after}.IsNamedColor && {before}.IsSystemColor == {after}.IsSystemColor && RecordSemanticComparer.StringEquals({before}.Name, {after}.Name)";
        }

        if (getterType.IsPrimitive || getterType.IsEnum || getterType == typeof(decimal) || getterType == typeof(Guid))
        {
            return $"{before} == {after}";
        }

        throw new InvalidOperationException($"Unhandled leaf equality shape {getterType}.");
    }

    /// <summary>
    /// Emits comparison of an ordered collection with detailed nested field changes and positional leaf changes.
    /// </summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="mutableType">The mutable collection type used to resolve its element model.</param>
    /// <param name="getterType">The getter collection type used by production code.</param>
    /// <param name="before">The prior static collection expression.</param>
    /// <param name="after">The resulting static collection expression.</param>
    /// <param name="path">The generated expression producing the collection path.</param>
    /// <param name="indent">The current source indentation.</param>
    private void EmitCompareCollection(StringBuilder output, Type mutableType, Type getterType, string before, string after, string path, string indent)
    {
        var mutableElement = mutableType.GetGenericArguments()[0];
        var getterElement = getterType.GetGenericArguments()[0];
        output.AppendLine($"{indent}if ({before} is null || {after} is null)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    CompareValue({path}, {before} is null && {after} is null, changes, cancellationToken);");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}else");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    var commonCount = Math.Min({before}.Count, {after}.Count);");
        output.AppendLine($"{indent}    for (var index = 0; index < commonCount; index++)");
        output.AppendLine($"{indent}    {{");
        output.AppendLine($"{indent}        cancellationToken.ThrowIfCancellationRequested();");
        EmitCompareValue(output, mutableElement, getterElement, before + "[index]", after + "[index]", path + " + $\"[{index}]\"", indent + "        ", IsLeafValue(getterElement), path, "index");
        output.AppendLine($"{indent}    }}");
        output.AppendLine($"{indent}    for (var index = commonCount; index < {before}.Count; index++)");
        output.AppendLine($"{indent}    {{");
        output.AppendLine($"{indent}        AddCollectionChange({path}, SemanticChangeKind.ItemRemoved, index, null, changes, cancellationToken);");
        output.AppendLine($"{indent}    }}");
        output.AppendLine($"{indent}    for (var index = commonCount; index < {after}.Count; index++)");
        output.AppendLine($"{indent}    {{");
        output.AppendLine($"{indent}        AddCollectionChange({path}, SemanticChangeKind.ItemInserted, null, index, changes, cancellationToken);");
        output.AppendLine($"{indent}    }}");
        output.AppendLine($"{indent}}}");
    }

    /// <summary>
    /// Emits row-major two-dimensional collection comparison with exact shape and element handling.
    /// </summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="mutableType">The mutable array type used to resolve its element model.</param>
    /// <param name="getterType">The getter array type used by production code.</param>
    /// <param name="before">The prior static array expression.</param>
    /// <param name="after">The resulting static array expression.</param>
    /// <param name="path">The generated expression producing the array path.</param>
    /// <param name="indent">The current source indentation.</param>
    private void EmitCompareArray2d(StringBuilder output, Type mutableType, Type getterType, string before, string after, string path, string indent)
    {
        var mutableElement = mutableType.GetGenericArguments()[0];
        var getterElement = getterType.GetGenericArguments()[0];
        output.AppendLine($"{indent}if ({before} is null || {after} is null)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    CompareValue({path}, {before} is null && {after} is null, changes, cancellationToken);");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}else if ({before}.Width != {after}.Width || {before}.Height != {after}.Height)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    CompareValue({path} + \".Shape\", false, changes, cancellationToken);");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}else");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    for (var y = 0; y < {before}.Height; y++)");
        output.AppendLine($"{indent}    {{");
        output.AppendLine($"{indent}        for (var x = 0; x < {before}.Width; x++)");
        output.AppendLine($"{indent}        {{");
        output.AppendLine($"{indent}            cancellationToken.ThrowIfCancellationRequested();");
        EmitCompareValue(output, mutableElement, getterElement, before + "[x, y]", after + "[x, y]", path + " + $\"[{x},{y}]\"", indent + "            ", false, null, null);
        output.AppendLine($"{indent}        }}");
        output.AppendLine($"{indent}    }}");
        output.AppendLine($"{indent}}}");
    }

    /// <summary>
    /// Emits null-aware concrete or polymorphic nested Mutagen comparison.
    /// </summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="mutableType">The mutable nested type used to select concrete or polymorphic dispatch.</param>
    /// <param name="getterType">The getter nested type used by production code.</param>
    /// <param name="before">The prior static nested expression.</param>
    /// <param name="after">The resulting static nested expression.</param>
    /// <param name="path">The generated expression producing the nested path.</param>
    /// <param name="indent">The current source indentation.</param>
    private static void EmitCompareNested(StringBuilder output, Type mutableType, Type getterType, string before, string after, string path, string indent)
    {
        output.AppendLine($"{indent}if ({before} is null || {after} is null)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    CompareValue({path}, {before} is null && {after} is null, changes, cancellationToken);");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}else");
        output.AppendLine($"{indent}{{");
        var call = mutableType.IsAbstract || mutableType.IsInterface ? "ComparePolymorphic" + mutableType.Name : "Compare" + mutableType.Name;
        output.AppendLine($"{indent}    {call}({before}, {after}, {path}, changes, cancellationToken);");
        output.AppendLine($"{indent}}}");
    }

}
