using System.Reflection;
using System.Text;
using System.Text.Json;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.NativeFieldGenerator;

/// <content>Emits the coverage manifest and provides deterministic native metadata and source-file helpers.</content>
internal sealed partial class CodecEmitter
{
    /// <summary>
    /// Emits the deterministic offline field manifest used to detect package drift and support later decoder generation.
    /// </summary>
    /// <returns>The complete generated manifest JSON.</returns>
    private string EmitManifest()
    {
        var manifest = new
        {
            format = "creationsforge-starfield-native-field-manifest-v1",
            package = "Mutagen.Bethesda.Starfield",
            packageVersion = PackageVersion,
            indexedTypeCount = Models.Count,
            indexedFieldCount = Models.Sum(model => model.Fields.Count),
            componentTypeCount = Models.Count(model => typeof(AComponent).IsAssignableFrom(model.MutableType)),
            conditionDataTypeCount = Models.Count(model => typeof(ConditionData).IsAssignableFrom(model.MutableType)),
            conditionTypeCount = Models.Count(model => model.MutableType == typeof(ConditionFloat) || model.MutableType == typeof(ConditionGlobal)),
            types = Models.Select(model => new
            {
                nativeType = model.MutableType.FullName,
                getterType = TypeName(model.GetterType),
                role = typeof(AComponent).IsAssignableFrom(model.MutableType)
                    ? "component"
                    : typeof(ConditionData).IsAssignableFrom(model.MutableType)
                        ? "condition-data"
                        : model.MutableType == typeof(ConditionFloat) || model.MutableType == typeof(ConditionGlobal)
                            ? "condition"
                            : "nested",
                fields = model.Fields.Select(field => new
                {
                    field.Ordinal,
                    field.Name,
                    mutableType = TypeName(field.MutableProperty.PropertyType),
                    getterType = TypeName(field.GetterProperty.PropertyType),
                    nullability = DescribeNullability(field.Nullability),
                    constructionPolicy = field.MutableProperty.SetMethod?.IsPublic == true ? "ordinary-setter" : "derived-or-writer-owned",
                }),
                explicitContractFields = typeof(ConditionData).IsAssignableFrom(model.MutableType)
                    && !model.Fields.Any(static field => string.Equals(field.Name, "Function", StringComparison.Ordinal))
                    ? new[]
                    {
                        new
                        {
                            name = "Function",
                            getterType = TypeName(typeof(Condition.Function)),
                            comparison = "concrete-kind-derived",
                            futureConstructionPolicy = "optional-input-validate-when-supplied-against-concrete-kind",
                        },
                    }
                    : Array.Empty<object>(),
            }),
        };
        return JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Validates the complete in-memory output set and reports obsolete owned generated files without deleting them.
    /// </summary>
    /// <param name="generatedRoot">The directory containing generated visitor source files.</param>
    /// <param name="artifacts">Every exact path and complete content prepared for publication.</param>
    private static void ValidateGeneratedOutputs(
        string generatedRoot,
        IReadOnlyList<KeyValuePair<string, string>> artifacts)
    {
        var normalizedPaths = artifacts
            .Select(artifact => Path.GetFullPath(artifact.Key))
            .ToArray();
        if (normalizedPaths.Distinct(StringComparer.OrdinalIgnoreCase).Count() != normalizedPaths.Length)
        {
            throw new InvalidOperationException("Generated output paths must be unique.");
        }

        var emptyArtifact = artifacts.FirstOrDefault(artifact => string.IsNullOrWhiteSpace(artifact.Value));
        if (!string.IsNullOrEmpty(emptyArtifact.Key))
        {
            throw new InvalidOperationException($"Generated artifact {emptyArtifact.Key} had no content.");
        }

        var expectedOwnedFiles = normalizedPaths
            .Where(path => string.Equals(Path.GetDirectoryName(path), Path.GetFullPath(generatedRoot), StringComparison.OrdinalIgnoreCase))
            .Select(Path.GetFileName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!Directory.Exists(generatedRoot))
        {
            return;
        }

        var obsoleteOwnedFiles = Directory.EnumerateFiles(
                generatedRoot,
                "StarfieldNested*.g.cs",
                SearchOption.TopDirectoryOnly)
            .Where(path => !expectedOwnedFiles.Contains(Path.GetFileName(path)))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (obsoleteOwnedFiles.Length > 0)
        {
            throw new InvalidOperationException(
                "Obsolete generated Starfield nested artifacts require explicit review: "
                + string.Join(", ", obsoleteOwnedFiles.Select(Path.GetFileName)));
        }
    }

    /// <summary>
    /// Formats recursive reflected getter nullability for the offline manifest.
    /// </summary>
    /// <param name="nullability">The reflected getter nullability graph.</param>
    /// <returns>A compact recursive nullability description.</returns>
    private static string DescribeNullability(NullabilityInfo nullability)
    {
        var nested = nullability.GenericTypeArguments.Length == 0
            ? string.Empty
            : "<" + string.Join(",", nullability.GenericTypeArguments.Select(DescribeNullability)) + ">";
        return nullability.ReadState + nested;
    }

    /// <summary>Classifies the statically callable construction surface of one concrete native model.</summary>
    /// <param name="mutableType">The concrete installed mutable type.</param>
    /// <returns>The constructor accessibility and required-member contract used by reader generation.</returns>
    private static NativeTypeConstructionModel CreateTypeConstructionModel(Type mutableType)
    {
        const BindingFlags constructorFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var parameterlessConstructor = mutableType.GetConstructor(
            constructorFlags,
            binder: null,
            Type.EmptyTypes,
            modifiers: null);
        var kind = parameterlessConstructor switch
        {
            { IsPublic: true } => NativeTypeConstructionKind.PublicParameterlessConstructor,
            not null => NativeTypeConstructionKind.NonPublicParameterlessConstructor,
            _ => NativeTypeConstructionKind.ParameterizedConstructorOnly,
        };
        var requiredMemberNames = mutableType
            .GetMembers(BindingFlags.Instance | BindingFlags.Public)
            .Where(static member => member.CustomAttributes.Any(static attribute =>
                attribute.AttributeType.FullName == "System.Runtime.CompilerServices.RequiredMemberAttribute"))
            .Select(static member => member.Name)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();
        return new NativeTypeConstructionModel(kind, parameterlessConstructor, requiredMemberNames);
    }

    /// <summary>Creates the recursive construction category for one mutable/getter value pair.</summary>
    /// <param name="mutableType">The mutable native API type.</param>
    /// <param name="getterType">The corresponding getter API type.</param>
    /// <param name="nullability">The reflected getter nullability graph.</param>
    /// <param name="location">The native field path used in generation failures.</param>
    /// <returns>The complete recursive value construction model.</returns>
    private static NativeValueConstructionModel CreateConstructionModel(
        Type mutableType,
        Type getterType,
        NullabilityInfo nullability,
        string location)
    {
        var mutableNullable = Nullable.GetUnderlyingType(mutableType);
        var getterNullable = Nullable.GetUnderlyingType(getterType);
        if (mutableNullable is not null || getterNullable is not null)
        {
            if (mutableNullable is null || getterNullable is null)
            {
                throw new InvalidOperationException($"Nullable construction mismatch at {location}: {mutableType} / {getterType}.");
            }

            return new NativeValueConstructionModel(
                NativeValueConstructionKind.Nullable,
                mutableType,
                getterType,
                nullability,
                CreateConstructionModel(
                    mutableNullable,
                    getterNullable,
                    GetSingleNullabilityArgument(nullability, getterType, location),
                    location));
        }

        if (IsCollection(getterType) || IsArray2d(getterType))
        {
            var kind = IsCollection(getterType)
                ? NativeValueConstructionKind.Collection
                : NativeValueConstructionKind.Array2d;
            return new NativeValueConstructionModel(
                kind,
                mutableType,
                getterType,
                nullability,
                CreateConstructionModel(
                    mutableType.GetGenericArguments()[0],
                    getterType.GetGenericArguments()[0],
                    GetSingleNullabilityArgument(nullability, getterType, location),
                    location + "[]"));
        }

        var constructionKind = GetTerminalConstructionKind(mutableType, getterType, location);
        return new NativeValueConstructionModel(constructionKind, mutableType, getterType, nullability);
    }

    /// <summary>Returns the only recursive nullability argument required by a nullable or collection construction.</summary>
    /// <param name="nullability">The parent reflected nullability description.</param>
    /// <param name="containingType">The reflected generic type whose single argument is being classified.</param>
    /// <param name="location">The native field path used in generation failures.</param>
    /// <returns>The single wrapped or element nullability description.</returns>
    private static NullabilityInfo GetSingleNullabilityArgument(
        NullabilityInfo nullability,
        Type containingType,
        string location)
    {
        ArgumentNullException.ThrowIfNull(containingType);
        if (nullability.GenericTypeArguments.Length != 1)
        {
            if (Nullable.GetUnderlyingType(containingType) is not null
                && nullability.GenericTypeArguments.Length == 0)
            {
                return nullability;
            }

            throw new InvalidOperationException(
                $"Construction metadata at {location} expected one nullability argument but found {nullability.GenericTypeArguments.Length}.");
        }

        return nullability.GenericTypeArguments[0];
    }

    /// <summary>Classifies one non-nullable, non-collection native construction terminal.</summary>
    /// <param name="mutableType">The mutable native API type.</param>
    /// <param name="getterType">The corresponding getter API type.</param>
    /// <param name="location">The native field path used in generation failures.</param>
    /// <returns>The exact terminal construction category.</returns>
    private static NativeValueConstructionKind GetTerminalConstructionKind(
        Type mutableType,
        Type getterType,
        string location)
    {
        if (getterType == typeof(string))
        {
            return NativeValueConstructionKind.String;
        }

        if (getterType == typeof(bool))
        {
            return NativeValueConstructionKind.Boolean;
        }

        if (getterType == typeof(float))
        {
            return NativeValueConstructionKind.Single;
        }

        if (getterType == typeof(double))
        {
            return NativeValueConstructionKind.Double;
        }

        if (getterType.IsEnum)
        {
            return NativeValueConstructionKind.Enum;
        }

        if (getterType.IsPrimitive || getterType == typeof(decimal))
        {
            return NativeValueConstructionKind.Numeric;
        }

        if (getterType == typeof(Guid))
        {
            return NativeValueConstructionKind.Guid;
        }

        if (getterType == typeof(System.Drawing.Color))
        {
            return NativeValueConstructionKind.Color;
        }

        if (getterType.FullName == "Noggog.P2Float")
        {
            return NativeValueConstructionKind.P2Float;
        }

        if (getterType.FullName == "Noggog.P2Int")
        {
            return NativeValueConstructionKind.P2Int;
        }

        if (getterType.FullName == "Noggog.P3Float")
        {
            return NativeValueConstructionKind.P3Float;
        }

        if (getterType.FullName == "Mutagen.Bethesda.Strings.ITranslatedStringGetter")
        {
            return NativeValueConstructionKind.TranslatedString;
        }

        if (IsMemory(getterType))
        {
            return NativeValueConstructionKind.Memory;
        }

        if (IsFormLink(getterType))
        {
            return NativeValueConstructionKind.FormLink;
        }

        if (IsFormLinkOrIndex(getterType))
        {
            return NativeValueConstructionKind.FormLinkOrIndex;
        }

        if (IsAssetLink(getterType))
        {
            return NativeValueConstructionKind.AssetLink;
        }

        if (getterType.Namespace == typeof(FormList).Namespace && getterType.IsInterface)
        {
            return mutableType.IsAbstract || mutableType.IsInterface
                ? NativeValueConstructionKind.NestedPolymorphic
                : NativeValueConstructionKind.NestedConcrete;
        }

        throw new InvalidOperationException(
            $"Unhandled construction shape at {location}: {mutableType} / {getterType}.");
    }

    /// <summary>
    /// Creates the common generated source header and partial class opening.
    /// </summary>
    /// <returns>A source builder containing the shared generated header and imports.</returns>
    private static StringBuilder GeneratedHeader()
    {
        var output = SourceHeader();
        output.AppendLine("using System.Text.Json;");
        output.AppendLine("using CreationsForge.Core.Engine.Contracts;");
        output.AppendLine("using CreationsForge.Core.Engine.NativeInspection;");
        output.AppendLine("using Mutagen.Bethesda.Plugins;");
        output.AppendLine("using Mutagen.Bethesda.Plugins.Assets;");
        output.AppendLine("using Mutagen.Bethesda.Starfield;");
        output.AppendLine("using Mutagen.Bethesda.Strings;");
        output.AppendLine("using Noggog;");
        output.AppendLine();
        output.AppendLine("namespace CreationsForge.Starfield.Native.NativeInspection;");
        output.AppendLine();
        output.AppendLine("/// <content>Contains deterministic visitors generated from the installed Mutagen Starfield FieldIndex contracts.</content>");
        output.AppendLine("internal static partial class StarfieldNestedFieldCodec");
        output.AppendLine("{");
        return output;
    }

    /// <summary>
    /// Closes one generated partial class source file.
    /// </summary>
    /// <param name="output">The generated source builder to close.</param>
    /// <returns>The complete generated source.</returns>
    private static string Finish(StringBuilder output)
    {
        output.AppendLine("}");
        return output.ToString();
    }

    /// <summary>
    /// Creates the common generated-file warning and nullable directive.
    /// </summary>
    /// <returns>A source builder containing the deterministic generated-file warning.</returns>
    private static StringBuilder SourceHeader()
    {
        var output = new StringBuilder();
        output.AppendLine("// <auto-generated>");
        output.AppendLine("// Generated from Mutagen.Bethesda.Starfield 0.55.0-alpha.48 FieldIndex and getter metadata.");
        output.AppendLine("// Run: dotnet run --project Tools/CreationsForge.NativeFieldGenerator/CreationsForge.NativeFieldGenerator.csproj -- generate .");
        output.AppendLine("// Do not hand-edit generated visitor files.");
        output.AppendLine("// </auto-generated>");
        output.AppendLine("#nullable enable");
        return output;
    }

    /// <summary>
    /// Writes one generated artifact with repository CRLF line endings and UTF-8 without a BOM.
    /// </summary>
    /// <param name="path">The generated artifact path.</param>
    /// <param name="content">The complete generated artifact content.</param>
    private static void WriteFile(string path, string content)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var normalized = content.Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\n", "\r\n", StringComparison.Ordinal);
        File.WriteAllText(path, normalized, new UTF8Encoding(false));
    }

    /// <summary>
    /// Resolves one generated Starfield getter interface back to its mutable indexed model class.
    /// </summary>
    /// <param name="getterType">The generated getter interface to resolve.</param>
    /// <returns>The matching mutable native model class.</returns>
    private Type ResolveMutable(Type getterType)
    {
        if (MutableByGetter.TryGetValue(getterType, out var mutable))
        {
            return mutable;
        }

        var expectedName = getterType.Name;
        if (expectedName.StartsWith('I') && expectedName.EndsWith("Getter", StringComparison.Ordinal))
        {
            var mutableName = expectedName[1..^"Getter".Length];
            var candidate = typeof(FormList).Assembly.GetType(typeof(FormList).Namespace + "." + mutableName);
            if (candidate is not null && FindGetter(candidate) == getterType)
            {
                return candidate;
            }
        }

        throw new InvalidOperationException($"No mutable native model maps to getter {getterType.FullName}.");
    }

    /// <summary>
    /// Returns native fields in exact generated FieldIndex order.
    /// </summary>
    /// <param name="type">The mutable native model class.</param>
    /// <param name="fieldIndex">The generated FieldIndex enum for that model.</param>
    /// <returns>The mutable properties ordered by their installed FieldIndex ordinals.</returns>
    private static IEnumerable<PropertyInfo> GetMutableFields(Type type, Type fieldIndex)
    {
        return Enum.GetNames(fieldIndex)
            .OrderBy(name => Convert.ToUInt16(Enum.Parse(fieldIndex, name)))
            .Select(name => type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public)
                ?? throw new InvalidOperationException($"No mutable property {type.FullName}.{name}."));
    }

    /// <summary>
    /// Finds the generated getter interface matching one mutable model class.
    /// </summary>
    /// <param name="type">The mutable native model class.</param>
    /// <returns>The generated getter interface, or null when none exists.</returns>
    private static Type? FindGetter(Type type)
    {
        var expected = "I" + type.Name + "Getter";
        return type.GetInterfaces().FirstOrDefault(candidate => candidate.Namespace == type.Namespace && candidate.Name == expected);
    }

    /// <summary>
    /// Finds an indexed property across a generated getter interface inheritance graph.
    /// </summary>
    /// <param name="type">The getter interface to search.</param>
    /// <param name="name">The indexed native field name.</param>
    /// <returns>The inherited property contract, or null when it is absent.</returns>
    private static PropertyInfo? FindInterfaceProperty(Type type, string name)
    {
        return type.GetProperty(name) ?? type.GetInterfaces().Select(candidate => FindInterfaceProperty(candidate, name)).FirstOrDefault(property => property is not null);
    }

    /// <summary>
    /// Finds a generated FieldIndex enum for one mutable native model class.
    /// </summary>
    /// <param name="type">The mutable native model class.</param>
    /// <returns>The generated FieldIndex enum, or null when the model is not indexed.</returns>
    private static Type? GetFieldIndex(Type type)
    {
        return type.Assembly.GetType(type.FullName + "_FieldIndex");
    }

    /// <summary>
    /// Formats a closed C# type name without assembly-qualified runtime lookup.
    /// </summary>
    /// <param name="type">The native or framework type to format.</param>
    /// <returns>A fully qualified C# type expression.</returns>
    private static string TypeName(Type type)
    {
        if (type.IsArray)
        {
            return TypeName(type.GetElementType()!) + "[]";
        }

        if (!type.IsGenericType)
        {
            return (type.FullName ?? type.Name).Replace('+', '.');
        }

        var definition = type.GetGenericTypeDefinition().FullName!.Split('`')[0].Replace('+', '.');
        return definition + "<" + string.Join(", ", type.GetGenericArguments().Select(TypeName)) + ">";
    }

    /// <summary>
    /// Escapes one generated C# string literal value.
    /// </summary>
    /// <param name="value">The unescaped literal content.</param>
    /// <returns>The content escaped for a generated C# string literal.</returns>
    private static string Escape(string value)
    {
        return value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal);
    }

    /// <summary>
    /// Identifies mutable terminal types that do not contain FieldIndex descendants.
    /// </summary>
    /// <param name="type">The mutable field type to classify.</param>
    /// <returns>True when traversal stops at the mutable type.</returns>
    private static bool IsMutableLeaf(Type type)
    {
        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(Guid)
            || type == typeof(System.Drawing.Color)
            || type.FullName == "Mutagen.Bethesda.Strings.TranslatedString"
            || type.FullName is "Noggog.P2Float" or "Noggog.P2Int" or "Noggog.P3Float"
            || IsMemory(type)
            || IsFormLink(type)
            || IsFormLinkOrIndex(type)
            || IsAssetLink(type)
            || IsCollection(type)
            || IsArray2d(type);
    }

    /// <summary>
    /// Identifies getter leaf types handled without nested indexed visitor dispatch.
    /// </summary>
    /// <param name="type">The getter field type to classify.</param>
    /// <returns>True when a direct generated leaf policy handles the getter type.</returns>
    private static bool IsGetterLeaf(Type type)
    {
        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(Guid)
            || type == typeof(System.Drawing.Color)
            || type.FullName == "Mutagen.Bethesda.Strings.ITranslatedStringGetter"
            || type.FullName is "Noggog.P2Float" or "Noggog.P2Int" or "Noggog.P3Float"
            || IsMemory(type)
            || IsFormLink(type)
            || IsFormLinkOrIndex(type)
            || IsAssetLink(type);
    }

    /// <summary>
    /// Identifies scalar or wrapper values whose ordered collection mismatch is represented as ItemChanged.
    /// </summary>
    /// <param name="type">The getter collection element type to classify.</param>
    /// <returns>True when an element mismatch should produce one positional item change.</returns>
    private static bool IsLeafValue(Type type)
    {
        return Nullable.GetUnderlyingType(type) is { } nullable ? IsLeafValue(nullable) : IsGetterLeaf(type);
    }

    /// <summary>Identifies exact byte-slice getter and mutable shapes.</summary>
    /// <param name="type">The field type to classify.</param>
    /// <returns>True for supported memory-slice shapes.</returns>
    private static bool IsMemory(Type type) => type.IsGenericType && type.Name is "MemorySlice`1" or "ReadOnlyMemorySlice`1";

    /// <summary>Identifies ordinary and nullable native form-link getter shapes.</summary>
    /// <param name="type">The field type to classify.</param>
    /// <returns>True for supported native form-link wrappers.</returns>
    private static bool IsFormLink(Type type) => type.IsGenericType && type.Name is "IFormLink`1" or "IFormLinkGetter`1" or "IFormLinkNullable`1" or "IFormLinkNullableGetter`1";

    /// <summary>Identifies owner-sensitive native link-or-index getter shapes.</summary>
    /// <param name="type">The field type to classify.</param>
    /// <returns>True for supported link-or-index wrappers.</returns>
    private static bool IsFormLinkOrIndex(Type type) => type.IsGenericType && type.Name is "IFormLinkOrIndex`1" or "IFormLinkOrIndexGetter`1";

    /// <summary>Identifies native asset-link getter and mutable shapes.</summary>
    /// <param name="type">The field type to classify.</param>
    /// <returns>True for supported native asset-link wrappers.</returns>
    private static bool IsAssetLink(Type type) => type.IsGenericType && type.Name is "AssetLink`1" or "AssetLinkGetter`1" or "IAssetLinkGetter`1";

    /// <summary>Identifies ordered one-dimensional mutable and getter collection shapes.</summary>
    /// <param name="type">The field type to classify.</param>
    /// <returns>True for supported one-dimensional collection contracts.</returns>
    private static bool IsCollection(Type type) => type.IsGenericType && type.Name is "ExtendedList`1" or "IReadOnlyList`1" or "IList`1";

    /// <summary>Identifies ordered two-dimensional mutable and getter collection shapes.</summary>
    /// <param name="type">The field type to classify.</param>
    /// <returns>True for supported two-dimensional collection contracts.</returns>
    private static bool IsArray2d(Type type) => type.IsGenericType && type.Name is "IArray2d`1" or "IReadOnlyArray2d`1";
}
