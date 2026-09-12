using System.Reflection;
using System.Text;

namespace CreationsForge.NativeFieldGenerator;

/// <content>Emits recursive wire-to-native value construction and shared Starfield reader leaves.</content>
internal sealed partial class CodecEmitter
{
    /// <summary>Creates the generated reader header without changing established writer artifact imports.</summary>
    /// <returns>A source builder containing reader-specific imports and the partial class opening.</returns>
    private static StringBuilder ReaderGeneratedHeader()
    {
        var output = SourceHeader();
        output.AppendLine("using System.Text.Json;");
        output.AppendLine("using CreationsForge.Core.Engine.NativeWire;");
        output.AppendLine("using Mutagen.Bethesda.Plugins;");
        output.AppendLine("using Mutagen.Bethesda.Plugins.Assets;");
        output.AppendLine("using Mutagen.Bethesda.Starfield;");
        output.AppendLine("using Mutagen.Bethesda.Strings;");
        output.AppendLine("using Noggog;");
        output.AppendLine();
        output.AppendLine("namespace CreationsForge.Starfield.Native.NativeInspection;");
        output.AppendLine();
        output.AppendLine("/// <content>Contains deterministic readers generated from the installed Mutagen Starfield FieldIndex contracts.</content>");
        output.AppendLine("internal static partial class StarfieldNestedFieldCodec");
        output.AppendLine("{");
        return output;
    }

    /// <summary>Returns whether the reflected getter contract permits a null reference value.</summary>
    /// <param name="construction">The recursive construction description.</param>
    /// <returns><see langword="true"/> only for nullable reference annotations.</returns>
    private static bool AllowsNull(NativeValueConstructionModel construction)
    {
        return !construction.GetterType.IsValueType
            && construction.Nullability.ReadState == NullabilityState.Nullable;
    }

    /// <summary>Emits one complete recursive value decode into a new local variable.</summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="construction">The recursive mutable/getter construction description.</param>
    /// <param name="wireValue">The generated context-owned wire-value expression.</param>
    /// <param name="variableName">The generated local variable name.</param>
    /// <param name="indent">The current source indentation.</param>
    private void EmitReadValue(
        StringBuilder output,
        NativeValueConstructionModel construction,
        string wireValue,
        string variableName,
        string indent)
    {
        if (construction.Kind == NativeValueConstructionKind.Nullable)
        {
            var element = construction.Element
                ?? throw new InvalidOperationException($"Nullable construction {construction.GetterType} has no element.");
            output.AppendLine($"{indent}{TypeName(construction.MutableType)} {variableName};");
            output.AppendLine($"{indent}if (context.IsNull({wireValue}))");
            output.AppendLine($"{indent}{{");
            output.AppendLine($"{indent}    {variableName} = null;");
            output.AppendLine($"{indent}}}");
            output.AppendLine($"{indent}else");
            output.AppendLine($"{indent}{{");
            EmitReadValue(output, element, wireValue, variableName + "Inner", indent + "    ");
            output.AppendLine($"{indent}    {variableName} = {variableName}Inner;");
            output.AppendLine($"{indent}}}");
            return;
        }

        if (AllowsNull(construction))
        {
            output.AppendLine($"{indent}{TypeName(construction.MutableType)}? {variableName};");
            output.AppendLine($"{indent}if (context.IsNull({wireValue}))");
            output.AppendLine($"{indent}{{");
            output.AppendLine($"{indent}    {variableName} = null;");
            output.AppendLine($"{indent}}}");
            output.AppendLine($"{indent}else");
            output.AppendLine($"{indent}{{");
            EmitReadNonNullValue(output, construction, wireValue, variableName + "Inner", indent + "    ");
            output.AppendLine($"{indent}    {variableName} = {variableName}Inner;");
            output.AppendLine($"{indent}}}");
            return;
        }

        EmitReadNonNullValue(output, construction, wireValue, variableName, indent);
    }

    /// <summary>Emits one non-null wire value decode into a new local variable.</summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="construction">The non-null construction description.</param>
    /// <param name="wireValue">The generated context-owned wire-value expression.</param>
    /// <param name="variableName">The generated local variable name.</param>
    /// <param name="indent">The current source indentation.</param>
    private void EmitReadNonNullValue(
        StringBuilder output,
        NativeValueConstructionModel construction,
        string wireValue,
        string variableName,
        string indent)
    {
        switch (construction.Kind)
        {
            case NativeValueConstructionKind.String:
                output.AppendLine($"{indent}var {variableName} = context.ReadString({wireValue});");
                return;
            case NativeValueConstructionKind.Boolean:
                output.AppendLine($"{indent}var {variableName} = context.ReadBoolean({wireValue});");
                return;
            case NativeValueConstructionKind.Single:
                output.AppendLine($"{indent}var {variableName} = context.ReadSingle({wireValue});");
                return;
            case NativeValueConstructionKind.Double:
                output.AppendLine($"{indent}var {variableName} = context.ReadDouble({wireValue});");
                return;
            case NativeValueConstructionKind.Enum:
                EmitReadEnum(output, construction.GetterType, wireValue, variableName, indent);
                return;
            case NativeValueConstructionKind.Numeric:
                EmitReadNumeric(output, construction.GetterType, wireValue, variableName, indent);
                return;
            case NativeValueConstructionKind.Guid:
                output.AppendLine($"{indent}var {variableName} = context.ReadGuid({wireValue});");
                return;
            case NativeValueConstructionKind.Color:
                output.AppendLine($"{indent}var {variableName} = ReadColor(context, {wireValue});");
                return;
            case NativeValueConstructionKind.P2Float:
                output.AppendLine($"{indent}var {variableName} = ReadP2Float(context, {wireValue});");
                return;
            case NativeValueConstructionKind.P2Int:
                output.AppendLine($"{indent}var {variableName} = ReadP2Int(context, {wireValue});");
                return;
            case NativeValueConstructionKind.P3Float:
                output.AppendLine($"{indent}var {variableName} = ReadP3Float(context, {wireValue});");
                return;
            case NativeValueConstructionKind.TranslatedString:
                output.AppendLine($"{indent}var {variableName} = ReadTranslatedString(context, {wireValue});");
                return;
            case NativeValueConstructionKind.Memory:
                output.AppendLine($"{indent}var {variableName} = new Noggog.MemorySlice<System.Byte>(context.ReadBytes({wireValue}));");
                return;
            case NativeValueConstructionKind.FormLink:
                EmitReadFormLink(output, construction, wireValue, variableName, indent);
                return;
            case NativeValueConstructionKind.FormLinkOrIndex:
                output.AppendLine($"{indent}var {variableName} = ReadFormLinkOrIndex(context, {wireValue}, \"{Escape(TypeName(construction.GetterType))}\", \"{Escape(TypeName(construction.GetterType.GetProperty("Link")!.PropertyType))}\");");
                return;
            case NativeValueConstructionKind.AssetLink:
                EmitReadAssetLink(output, construction, wireValue, variableName, indent);
                return;
            case NativeValueConstructionKind.Collection:
                EmitReadCollection(output, construction, wireValue, variableName, indent);
                return;
            case NativeValueConstructionKind.Array2d:
                EmitReadArray2d(output, construction, wireValue, variableName, indent);
                return;
            case NativeValueConstructionKind.NestedConcrete:
                output.AppendLine($"{indent}var {variableName} = Read{construction.MutableType.Name}(context, {wireValue});");
                return;
            case NativeValueConstructionKind.NestedPolymorphic:
                output.AppendLine($"{indent}var {variableName} = ReadPolymorphic{construction.MutableType.Name}(context, {wireValue});");
                return;
            default:
                throw new InvalidOperationException($"Unhandled reader construction kind {construction.Kind} for {construction.GetterType}.");
        }
    }

    /// <summary>Emits one enum read across its complete underlying integral domain.</summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="enumType">The concrete native enum type.</param>
    /// <param name="wireValue">The context-owned numeric value expression.</param>
    /// <param name="variableName">The generated local variable name.</param>
    /// <param name="indent">The current source indentation.</param>
    private static void EmitReadEnum(StringBuilder output, Type enumType, string wireValue, string variableName, string indent)
    {
        var underlyingType = Enum.GetUnderlyingType(enumType);
        var reader = GetNumericReader(underlyingType);
        output.AppendLine($"{indent}var {variableName} = ({TypeName(enumType)})context.{reader}({wireValue});");
    }

    /// <summary>Emits one supported non-enum numeric scalar read.</summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="numericType">The exact numeric type.</param>
    /// <param name="wireValue">The context-owned numeric value expression.</param>
    /// <param name="variableName">The generated local variable name.</param>
    /// <param name="indent">The current source indentation.</param>
    private static void EmitReadNumeric(StringBuilder output, Type numericType, string wireValue, string variableName, string indent)
    {
        var reader = GetNumericReader(numericType);
        output.AppendLine($"{indent}var {variableName} = context.{reader}({wireValue});");
    }

    /// <summary>Maps an exact supported numeric type to its bounded Core reader.</summary>
    /// <param name="numericType">The integral numeric type.</param>
    /// <returns>The exact Core reader method name.</returns>
    private static string GetNumericReader(Type numericType)
    {
        if (numericType == typeof(sbyte)) return "ReadSByte";
        if (numericType == typeof(byte)) return "ReadByte";
        if (numericType == typeof(short)) return "ReadInt16";
        if (numericType == typeof(ushort)) return "ReadUInt16";
        if (numericType == typeof(int)) return "ReadInt32";
        if (numericType == typeof(uint)) return "ReadUInt32";
        if (numericType == typeof(long)) return "ReadInt64";
        if (numericType == typeof(ulong)) return "ReadUInt64";
        throw new InvalidOperationException($"Numeric type {numericType} has no bounded native wire reader.");
    }

    /// <summary>Emits one typed ordinary or nullable FormKey link construction.</summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="construction">The closed link construction description.</param>
    /// <param name="wireValue">The context-owned link object expression.</param>
    /// <param name="variableName">The generated local variable name.</param>
    /// <param name="indent">The current source indentation.</param>
    private static void EmitReadFormLink(
        StringBuilder output,
        NativeValueConstructionModel construction,
        string wireValue,
        string variableName,
        string indent)
    {
        var targetType = construction.GetterType.GetGenericArguments()[0];
        output.AppendLine($"{indent}var {variableName}FormKey = ReadFormLinkKey(context, {wireValue}, \"{Escape(TypeName(construction.GetterType))}\");");
        var nullableWrapper = construction.MutableType.Name.Contains("Nullable", StringComparison.Ordinal)
            || construction.GetterType.Name.Contains("Nullable", StringComparison.Ordinal);
        var wrapperName = nullableWrapper ? "FormLinkNullable" : "FormLink";
        if (nullableWrapper)
        {
            output.AppendLine($"{indent}var {variableName} = new Mutagen.Bethesda.Plugins.{wrapperName}<{TypeName(targetType)}>({variableName}FormKey);");
        }
        else
        {
            output.AppendLine($"{indent}var {variableName} = {variableName}FormKey.HasValue");
            output.AppendLine($"{indent}    ? new Mutagen.Bethesda.Plugins.{wrapperName}<{TypeName(targetType)}>({variableName}FormKey.Value)");
            output.AppendLine($"{indent}    : new Mutagen.Bethesda.Plugins.{wrapperName}<{TypeName(targetType)}>();");
        }
    }

    /// <summary>Emits one typed asset-link construction with redundant path validation.</summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="construction">The closed asset-link construction description.</param>
    /// <param name="wireValue">The context-owned asset-link object expression.</param>
    /// <param name="variableName">The generated local variable name.</param>
    /// <param name="indent">The current source indentation.</param>
    private static void EmitReadAssetLink(
        StringBuilder output,
        NativeValueConstructionModel construction,
        string wireValue,
        string variableName,
        string indent)
    {
        var assetType = construction.MutableType.GetGenericArguments()[0];
        output.AppendLine($"{indent}var {variableName}Data = ReadAssetLink(context, {wireValue}, \"{Escape(TypeName(construction.GetterType))}\");");
        output.AppendLine($"{indent}Mutagen.Bethesda.Plugins.Assets.AssetLink<{TypeName(assetType)}> {variableName};");
        output.AppendLine($"{indent}try");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    {variableName} = new Mutagen.Bethesda.Plugins.Assets.AssetLink<{TypeName(assetType)}>({variableName}Data.GivenPath);");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}catch (Mutagen.Bethesda.Plugins.Exceptions.AssetPathMisalignedException exception)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    context.Reject({variableName}Data.GivenPathValue, $\"Invalid native asset path: {{exception.Message}}\");");
        output.AppendLine($"{indent}    throw new InvalidOperationException(\"NativeWireReadContext.Reject unexpectedly returned.\", exception);");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}if ({variableName}Data.IsNull)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    {variableName}.SetToNull();");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}if ({variableName}.IsNull != {variableName}Data.IsNull)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    context.Reject({variableName}Data.IsNullValue, \"Asset-link null state does not match the supplied native path.\");");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}if (!string.Equals({variableName}.GivenPath, {variableName}Data.GivenPath, StringComparison.Ordinal))");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    context.Reject({variableName}Data.GivenPathValue, \"Asset-link native path does not round-trip exactly.\");");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}if ({variableName}Data.DataRelativePathValue.HasValue");
        output.AppendLine($"{indent}    && !string.Equals({variableName}.DataRelativePath.ToString(), {variableName}Data.DataRelativePath, StringComparison.Ordinal))");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    context.Reject({variableName}Data.DataRelativePathValue.Value, \"Asset-link data-relative path does not match the supplied native path.\");");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}if ({variableName}Data.ExtensionValue.HasValue");
        output.AppendLine($"{indent}    && !string.Equals({variableName}.Extension, {variableName}Data.Extension, StringComparison.Ordinal))");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    context.Reject({variableName}Data.ExtensionValue.Value, \"Asset-link extension does not match the supplied native path.\");");
        output.AppendLine($"{indent}}}");
    }

    /// <summary>Emits one bounded ordered collection construction.</summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="construction">The closed collection construction description.</param>
    /// <param name="wireValue">The context-owned array expression.</param>
    /// <param name="variableName">The generated local variable name.</param>
    /// <param name="indent">The current source indentation.</param>
    private void EmitReadCollection(
        StringBuilder output,
        NativeValueConstructionModel construction,
        string wireValue,
        string variableName,
        string indent)
    {
        var element = construction.Element
            ?? throw new InvalidOperationException($"Collection construction {construction.GetterType} has no element.");
        var elementType = construction.MutableType.GetGenericArguments()[0];
        if (construction.MutableType.GetGenericTypeDefinition().Name != "ExtendedList`1")
        {
            throw new InvalidOperationException($"Collection {construction.MutableType} is not a constructible ExtendedList.");
        }

        output.AppendLine($"{indent}var {variableName}Array = context.ReadArray({wireValue});");
        output.AppendLine($"{indent}var {variableName} = new Noggog.ExtendedList<{TypeName(elementType)}>();");
        output.AppendLine($"{indent}for (var {variableName}Index = 0; {variableName}Index < {variableName}Array.Count; {variableName}Index++)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    var {variableName}ElementValue = {variableName}Array.GetElement({variableName}Index);");
        EmitReadValue(output, element, variableName + "ElementValue", variableName + "Element", indent + "    ");
        output.AppendLine($"{indent}    {variableName}.Add({variableName}Element);");
        output.AppendLine($"{indent}}}");
    }

    /// <summary>Emits one bounded row-major two-dimensional native collection construction.</summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="construction">The closed two-dimensional collection description.</param>
    /// <param name="wireValue">The context-owned array object expression.</param>
    /// <param name="variableName">The generated local variable name.</param>
    /// <param name="indent">The current source indentation.</param>
    private void EmitReadArray2d(
        StringBuilder output,
        NativeValueConstructionModel construction,
        string wireValue,
        string variableName,
        string indent)
    {
        var element = construction.Element
            ?? throw new InvalidOperationException($"Array2d construction {construction.GetterType} has no element.");
        var elementType = construction.MutableType.GetGenericArguments()[0];
        output.AppendLine($"{indent}var {variableName}Object = context.ReadObject({wireValue}, Array2dReadShape);");
        output.AppendLine($"{indent}var {variableName}WidthValue = {variableName}Object.GetRequiredProperty(\"width\");");
        output.AppendLine($"{indent}var {variableName}HeightValue = {variableName}Object.GetRequiredProperty(\"height\");");
        output.AppendLine($"{indent}var {variableName}RowsValue = {variableName}Object.GetRequiredProperty(\"rows\");");
        output.AppendLine($"{indent}var {variableName}Width = context.ReadInt32({variableName}WidthValue);");
        output.AppendLine($"{indent}var {variableName}Height = context.ReadInt32({variableName}HeightValue);");
        output.AppendLine($"{indent}context.ValidateArrayLength({variableName}WidthValue, {variableName}Width);");
        output.AppendLine($"{indent}context.ValidateArrayLength({variableName}HeightValue, {variableName}Height);");
        output.AppendLine($"{indent}var {variableName}Rows = context.ReadArray({variableName}RowsValue);");
        output.AppendLine($"{indent}if ({variableName}Rows.Count != {variableName}Height)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    context.Reject({variableName}RowsValue, \"Array2d row count does not match height.\");");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}var {variableName}DecodedRows = new {TypeName(elementType)}[{variableName}Height][];");
        output.AppendLine($"{indent}for (var {variableName}Y = 0; {variableName}Y < {variableName}Height; {variableName}Y++)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    var {variableName}RowValue = {variableName}Rows.GetElement({variableName}Y);");
        output.AppendLine($"{indent}    var {variableName}Row = context.ReadArray({variableName}RowValue);");
        output.AppendLine($"{indent}    if ({variableName}Row.Count != {variableName}Width)");
        output.AppendLine($"{indent}    {{");
        output.AppendLine($"{indent}        context.Reject({variableName}RowValue, \"Array2d column count does not match width.\");");
        output.AppendLine($"{indent}    }}");
        output.AppendLine($"{indent}    var {variableName}DecodedRow = new {TypeName(elementType)}[{variableName}Width];");
        output.AppendLine($"{indent}    for (var {variableName}X = 0; {variableName}X < {variableName}Width; {variableName}X++)");
        output.AppendLine($"{indent}    {{");
        output.AppendLine($"{indent}        var {variableName}ElementValue = {variableName}Row.GetElement({variableName}X);");
        EmitReadValue(output, element, variableName + "ElementValue", variableName + "Element", indent + "        ");
        output.AppendLine($"{indent}        {variableName}DecodedRow[{variableName}X] = {variableName}Element;");
        output.AppendLine($"{indent}    }}");
        output.AppendLine($"{indent}    {variableName}DecodedRows[{variableName}Y] = {variableName}DecodedRow;");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}var {variableName}Cells = new {TypeName(elementType)}[{variableName}Width, {variableName}Height];");
        output.AppendLine($"{indent}for (var {variableName}Y = 0; {variableName}Y < {variableName}Height; {variableName}Y++)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    for (var {variableName}X = 0; {variableName}X < {variableName}Width; {variableName}X++)");
        output.AppendLine($"{indent}    {{");
        output.AppendLine($"{indent}        {variableName}Cells[{variableName}X, {variableName}Y] = {variableName}DecodedRows[{variableName}Y][{variableName}X];");
        output.AppendLine($"{indent}    }}");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}var {variableName} = new Noggog.Array2d<{TypeName(elementType)}>({variableName}Cells);");
    }

    /// <summary>Emits shared closed-object leaf readers used by generated native construction.</summary>
    /// <returns>Complete generated leaf-reader source.</returns>
    private static string EmitReaderLeaves()
    {
        var output = SourceHeader();
        output.AppendLine("using CreationsForge.Core.Engine.NativeWire;");
        output.AppendLine("using Mutagen.Bethesda.Plugins;");
        output.AppendLine("using Mutagen.Bethesda.Strings;");
        output.AppendLine("using Noggog;");
        output.AppendLine();
        output.AppendLine("namespace CreationsForge.Starfield.Native.NativeInspection;");
        output.AppendLine();
        output.AppendLine("/// <content>Contains closed Starfield reader leaf shapes and derived-value validation.</content>");
        output.AppendLine("internal static partial class StarfieldNestedFieldCodec");
        output.AppendLine("{");
        EmitReaderLeafSource(output);
        output.AppendLine("}");
        return output.ToString();
    }

    /// <summary>Appends the generated closed leaf shapes and helper method implementations.</summary>
    /// <param name="output">The generated source builder.</param>
    private static void EmitReaderLeafSource(StringBuilder output)
    {
        output.AppendLine("    /// <summary>Defines a closed two-dimensional native array value.</summary>");
        output.AppendLine("    private static readonly NativeWireObjectShape Array2dReadShape = new(\"Array2d\", new[] { \"width\", \"height\", \"rows\" });");
        output.AppendLine();
        output.AppendLine("    /// <summary>Defines a closed two-dimensional float vector.</summary>");
        output.AppendLine("    private static readonly NativeWireObjectShape P2FloatReadShape = new(\"P2Float\", new[] { \"X\", \"Y\" });");
        output.AppendLine();
        output.AppendLine("    /// <summary>Defines a closed two-dimensional integer vector.</summary>");
        output.AppendLine("    private static readonly NativeWireObjectShape P2IntReadShape = new(\"P2Int\", new[] { \"X\", \"Y\" });");
        output.AppendLine();
        output.AppendLine("    /// <summary>Defines a closed three-dimensional float vector.</summary>");
        output.AppendLine("    private static readonly NativeWireObjectShape P3FloatReadShape = new(\"P3Float\", new[] { \"X\", \"Y\", \"Z\" });");
        output.AppendLine();
        output.AppendLine("    /// <summary>Defines the complete redundant native color representation.</summary>");
        output.AppendLine("    private static readonly NativeWireObjectShape ColorReadShape = new(\"Color\", new[] { \"argb\", \"a\", \"r\", \"g\", \"b\", \"isEmpty\", \"isKnownColor\", \"isNamedColor\", \"isSystemColor\", \"name\" });");
        output.AppendLine();
        output.AppendLine("    /// <summary>Defines a typed ordinary or nullable FormKey link wrapper.</summary>");
        output.AppendLine("    private static readonly NativeWireObjectShape FormLinkReadShape = new(\"FormLink\", new[] { \"$type\", \"isNull\", \"formKey\" });");
        output.AppendLine();
        output.AppendLine("    /// <summary>Defines an owner-sensitive link-or-index wrapper.</summary>");
        output.AppendLine("    private static readonly NativeWireObjectShape FormLinkOrIndexReadShape = new(\"FormLinkOrIndex\", new[] { \"$type\", \"index\", \"link\" }, new[] { \"usesLink\", \"usesAlias\", \"usesPackageData\" });");
        output.AppendLine();
        output.AppendLine("    /// <summary>Defines a typed asset-link wrapper.</summary>");
        output.AppendLine("    private static readonly NativeWireObjectShape AssetLinkReadShape = new(\"AssetLink\", new[] { \"$type\", \"value\" });");
        output.AppendLine();
        output.AppendLine("    /// <summary>Defines the redundant asset-link path payload.</summary>");
        output.AppendLine("    private static readonly NativeWireObjectShape AssetLinkValueReadShape = new(\"asset-link value\", new[] { \"isNull\", \"givenPath\" }, new[] { \"dataRelativePath\", \"extension\" });");
        output.AppendLine();
        output.AppendLine("    /// <summary>Defines a translated-string wrapper.</summary>");
        output.AppendLine("    private static readonly NativeWireObjectShape TranslatedStringReadShape = new(\"TranslatedString\", new[] { \"$type\", \"value\" });");
        output.AppendLine();
        output.AppendLine("    /// <summary>Defines a complete translated-string payload.</summary>");
        output.AppendLine("    private static readonly NativeWireObjectShape TranslatedStringValueReadShape = new(\"translated-string value\", new[] { \"targetLanguage\", \"value\", \"translations\" });");
        output.AppendLine();
        output.AppendLine("    /// <summary>Defines one translated-string language entry.</summary>");
        output.AppendLine("    private static readonly NativeWireObjectShape TranslationEntryReadShape = new(\"translation entry\", new[] { \"language\", \"value\" });");
        output.AppendLine();
        output.AppendLine("    /// <summary>Validates one package-scoped wire discriminator against the statically selected native type.</summary>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned discriminator value.</param>");
        output.AppendLine("    /// <param name=\"nativeTypeName\">The expected native full name without package scope.</param>");
        output.AppendLine("    private static void ReadAndValidateType(NativeWireReadContext context, NativeWireValue value, string nativeTypeName)");
        output.AppendLine("    {");
        output.AppendLine("        var actual = context.ReadString(value);");
        output.AppendLine("        var expected = NativeTypeNamespace + nativeTypeName;");
        output.AppendLine("        if (!string.Equals(actual, expected, StringComparison.Ordinal))");
        output.AppendLine("        {");
        output.AppendLine("            context.Reject(value, $\"Expected native type discriminator '{expected}'.\");");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Rejects one invalid generated value while satisfying an expression return contract.</summary>");
        output.AppendLine("    /// <typeparam name=\"T\">The expression result type that is never produced.</typeparam>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned invalid value.</param>");
        output.AppendLine("    /// <param name=\"message\">The invariant failure description.</param>");
        output.AppendLine("    /// <returns>This method never returns.</returns>");
        output.AppendLine("    private static T RejectValue<T>(NativeWireReadContext context, NativeWireValue value, string message)");
        output.AppendLine("    {");
        output.AppendLine("        context.Reject(value, message);");
        output.AppendLine("        throw new InvalidOperationException(\"NativeWireReadContext.Reject unexpectedly returned.\");");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Reads a nullable native string from an explicitly present required property.</summary>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned string or null value.</param>");
        output.AppendLine("    /// <returns>The decoded string or null.</returns>");
        output.AppendLine("    private static string? ReadNullableString(NativeWireReadContext context, NativeWireValue value)");
        output.AppendLine("    {");
        output.AppendLine("        return context.IsNull(value) ? null : context.ReadString(value);");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Reads a nullable FormKey while validating the complete typed wrapper state.</summary>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned link wrapper.</param>");
        output.AppendLine("    /// <param name=\"expectedWrapperType\">The exact generated getter wrapper identity.</param>");
        output.AppendLine("    /// <returns>The decoded FormKey, or null for the native null-link state.</returns>");
        output.AppendLine("    private static FormKey? ReadFormLinkKey(NativeWireReadContext context, NativeWireValue value, string expectedWrapperType)");
        output.AppendLine("    {");
        output.AppendLine("        var container = context.ReadObject(value, FormLinkReadShape);");
        output.AppendLine("        ReadAndValidateType(context, container.GetRequiredProperty(\"$type\"), expectedWrapperType);");
        output.AppendLine("        var isNullValue = container.GetRequiredProperty(\"isNull\");");
        output.AppendLine("        var isNull = context.ReadBoolean(isNullValue);");
        output.AppendLine("        var formKeyValue = container.GetRequiredProperty(\"formKey\");");
        output.AppendLine("        var formKey = context.IsNull(formKeyValue) ? (FormKey?)null : context.ReadFormKey(formKeyValue);");
        output.AppendLine("        if (isNull != (!formKey.HasValue || formKey.Value.IsNull))");
        output.AppendLine("        {");
        output.AppendLine("            context.Reject(isNullValue, \"Form-link null state does not match its FormKey.\");");
        output.AppendLine("        }");
        output.AppendLine("        return formKey;");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Reads the owner-independent data of one link-or-index wrapper for later owner-bound construction.</summary>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned link-or-index wrapper.</param>");
        output.AppendLine("    /// <param name=\"expectedWrapperType\">The exact generated link-or-index getter identity.</param>");
        output.AppendLine("    /// <param name=\"expectedLinkType\">The exact generated nested link getter identity.</param>");
        output.AppendLine("    /// <returns>The complete owner-independent mode, index, and link data.</returns>");
        output.AppendLine("    private static (bool? UsesLink, NativeWireValue? UsesLinkValue, bool? UsesAlias, NativeWireValue? UsesAliasValue, bool? UsesPackageData, NativeWireValue? UsesPackageDataValue, uint? Index, FormKey? Link) ReadFormLinkOrIndex(NativeWireReadContext context, NativeWireValue value, string expectedWrapperType, string expectedLinkType)");
        output.AppendLine("    {");
        output.AppendLine("        var container = context.ReadObject(value, FormLinkOrIndexReadShape);");
        output.AppendLine("        ReadAndValidateType(context, container.GetRequiredProperty(\"$type\"), expectedWrapperType);");
        output.AppendLine("        var hasUsesLink = container.TryGetOptionalProperty(\"usesLink\", out var usesLinkValue);");
        output.AppendLine("        var usesLink = hasUsesLink ? context.ReadBoolean(usesLinkValue) : (bool?)null;");
        output.AppendLine("        var hasUsesAlias = container.TryGetOptionalProperty(\"usesAlias\", out var usesAliasValue);");
        output.AppendLine("        var usesAlias = hasUsesAlias ? context.ReadBoolean(usesAliasValue) : (bool?)null;");
        output.AppendLine("        var hasUsesPackageData = container.TryGetOptionalProperty(\"usesPackageData\", out var usesPackageDataValue);");
        output.AppendLine("        var usesPackageData = hasUsesPackageData ? context.ReadBoolean(usesPackageDataValue) : (bool?)null;");
        output.AppendLine("        var indexValue = container.GetRequiredProperty(\"index\");");
        output.AppendLine("        var index = context.IsNull(indexValue) ? (uint?)null : context.ReadUInt32(indexValue);");
        output.AppendLine("        var link = ReadFormLinkKey(context, container.GetRequiredProperty(\"link\"), expectedLinkType);");
        output.AppendLine("        return (usesLink, hasUsesLink ? usesLinkValue : null, usesAlias, hasUsesAlias ? usesAliasValue : null, usesPackageData, hasUsesPackageData ? usesPackageDataValue : null, index, link);");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Reads one complete typed asset-link wrapper without treating normalized paths as construction authority.</summary>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned asset-link wrapper.</param>");
        output.AppendLine("    /// <param name=\"expectedWrapperType\">The exact generated asset-link getter identity.</param>");
        output.AppendLine("    /// <returns>The supplied path and redundant derived path state.</returns>");
        output.AppendLine("    private static (bool IsNull, NativeWireValue IsNullValue, string GivenPath, NativeWireValue GivenPathValue, string? DataRelativePath, NativeWireValue? DataRelativePathValue, string? Extension, NativeWireValue? ExtensionValue) ReadAssetLink(NativeWireReadContext context, NativeWireValue value, string expectedWrapperType)");
        output.AppendLine("    {");
        output.AppendLine("        var container = context.ReadObject(value, AssetLinkReadShape);");
        output.AppendLine("        ReadAndValidateType(context, container.GetRequiredProperty(\"$type\"), expectedWrapperType);");
        output.AppendLine("        var payload = context.ReadObject(container.GetRequiredProperty(\"value\"), AssetLinkValueReadShape);");
        output.AppendLine("        var isNullValue = payload.GetRequiredProperty(\"isNull\");");
        output.AppendLine("        var isNull = context.ReadBoolean(isNullValue);");
        output.AppendLine("        var givenPathValue = payload.GetRequiredProperty(\"givenPath\");");
        output.AppendLine("        var givenPath = context.ReadString(givenPathValue);");
        output.AppendLine("        var hasDataRelativePath = payload.TryGetOptionalProperty(\"dataRelativePath\", out var dataRelativePathValue);");
        output.AppendLine("        var dataRelativePath = hasDataRelativePath ? context.ReadString(dataRelativePathValue) : null;");
        output.AppendLine("        var hasExtension = payload.TryGetOptionalProperty(\"extension\", out var extensionValue);");
        output.AppendLine("        var extension = hasExtension ? context.ReadString(extensionValue) : null;");
        output.AppendLine("        return (isNull, isNullValue, givenPath, givenPathValue, dataRelativePath, hasDataRelativePath ? dataRelativePathValue : null, extension, hasExtension ? extensionValue : null);");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Reads a translated string and verifies its redundant target-language value.</summary>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned translated-string wrapper.</param>");
        output.AppendLine("    /// <returns>The fully populated mutable translated string.</returns>");
        output.AppendLine("    private static TranslatedString ReadTranslatedString(NativeWireReadContext context, NativeWireValue value)");
        output.AppendLine("    {");
        output.AppendLine("        var container = context.ReadObject(value, TranslatedStringReadShape);");
        output.AppendLine("        ReadAndValidateType(context, container.GetRequiredProperty(\"$type\"), \"Mutagen.Bethesda.Strings.ITranslatedStringGetter\");");
        output.AppendLine("        var payload = context.ReadObject(container.GetRequiredProperty(\"value\"), TranslatedStringValueReadShape);");
        output.AppendLine("        var targetLanguageValue = payload.GetRequiredProperty(\"targetLanguage\");");
        output.AppendLine("        var targetLanguage = ReadNamedEnum<Language>(context, targetLanguageValue);");
        output.AppendLine("        var expectedValue = ReadNullableString(context, payload.GetRequiredProperty(\"value\"));");
        output.AppendLine("        var translationsValue = payload.GetRequiredProperty(\"translations\");");
        output.AppendLine("        var translations = context.ReadArray(translationsValue);");
        output.AppendLine("        var result = new TranslatedString(targetLanguage);");
        output.AppendLine("        var seen = new HashSet<Language>();");
        output.AppendLine("        for (var index = 0; index < translations.Count; index++)");
        output.AppendLine("        {");
        output.AppendLine("            var entryValue = translations.GetElement(index);");
        output.AppendLine("            var entry = context.ReadObject(entryValue, TranslationEntryReadShape);");
        output.AppendLine("            var languageValue = entry.GetRequiredProperty(\"language\");");
        output.AppendLine("            var language = ReadNamedEnum<Language>(context, languageValue);");
        output.AppendLine("            if (!seen.Add(language))");
        output.AppendLine("            {");
        output.AppendLine("                context.Reject(languageValue, \"Translation language occurs more than once.\");");
        output.AppendLine("            }");
        output.AppendLine("            result.Set(language, context.ReadString(entry.GetRequiredProperty(\"value\")));");
        output.AppendLine("        }");
        output.AppendLine("        if (!string.Equals(result.String, expectedValue, StringComparison.Ordinal))");
        output.AppendLine("        {");
        output.AppendLine("            context.Reject(value, \"Translated-string target value does not match its language entries.\");");
        output.AppendLine("        }");
        output.AppendLine("        return result;");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Reads one exact symbolic enum name and rejects aliases or numeric spellings.</summary>");
        output.AppendLine("    /// <typeparam name=\"TEnum\">The enum selected by generated code.</typeparam>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned enum-name string.</param>");
        output.AppendLine("    /// <returns>The exact defined enum value.</returns>");
        output.AppendLine("    private static TEnum ReadNamedEnum<TEnum>(NativeWireReadContext context, NativeWireValue value) where TEnum : struct, Enum");
        output.AppendLine("    {");
        output.AppendLine("        var text = context.ReadString(value);");
        output.AppendLine("        if (!Enum.TryParse<TEnum>(text, false, out var result) || !Enum.IsDefined(result) || !string.Equals(result.ToString(), text, StringComparison.Ordinal))");
        output.AppendLine("        {");
        output.AppendLine("            context.Reject(value, $\"Expected a canonical {typeof(TEnum).Name} name.\");");
        output.AppendLine("        }");
        output.AppendLine("        return result;");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Reads a two-dimensional floating-point vector.</summary>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned vector object.</param>");
        output.AppendLine("    /// <returns>The exact native vector.</returns>");
        output.AppendLine("    private static P2Float ReadP2Float(NativeWireReadContext context, NativeWireValue value)");
        output.AppendLine("    {");
        output.AppendLine("        var container = context.ReadObject(value, P2FloatReadShape);");
        output.AppendLine("        return new P2Float(context.ReadSingle(container.GetRequiredProperty(\"X\")), context.ReadSingle(container.GetRequiredProperty(\"Y\")));");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Reads a two-dimensional integer vector.</summary>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned vector object.</param>");
        output.AppendLine("    /// <returns>The exact native vector.</returns>");
        output.AppendLine("    private static P2Int ReadP2Int(NativeWireReadContext context, NativeWireValue value)");
        output.AppendLine("    {");
        output.AppendLine("        var container = context.ReadObject(value, P2IntReadShape);");
        output.AppendLine("        return new P2Int(context.ReadInt32(container.GetRequiredProperty(\"X\")), context.ReadInt32(container.GetRequiredProperty(\"Y\")));");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Reads a three-dimensional floating-point vector.</summary>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned vector object.</param>");
        output.AppendLine("    /// <returns>The exact native vector.</returns>");
        output.AppendLine("    private static P3Float ReadP3Float(NativeWireReadContext context, NativeWireValue value)");
        output.AppendLine("    {");
        output.AppendLine("        var container = context.ReadObject(value, P3FloatReadShape);");
        output.AppendLine("        return new P3Float(context.ReadSingle(container.GetRequiredProperty(\"X\")), context.ReadSingle(container.GetRequiredProperty(\"Y\")), context.ReadSingle(container.GetRequiredProperty(\"Z\")));");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Reads one complete native color and validates every redundant representation field.</summary>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned color object.</param>");
        output.AppendLine("    /// <returns>The exact reconstructible native color.</returns>");
        output.AppendLine("    private static System.Drawing.Color ReadColor(NativeWireReadContext context, NativeWireValue value)");
        output.AppendLine("    {");
        output.AppendLine("        var container = context.ReadObject(value, ColorReadShape);");
        output.AppendLine("        var argb = context.ReadInt32(container.GetRequiredProperty(\"argb\"));");
        output.AppendLine("        var a = context.ReadByte(container.GetRequiredProperty(\"a\"));");
        output.AppendLine("        var r = context.ReadByte(container.GetRequiredProperty(\"r\"));");
        output.AppendLine("        var g = context.ReadByte(container.GetRequiredProperty(\"g\"));");
        output.AppendLine("        var b = context.ReadByte(container.GetRequiredProperty(\"b\"));");
        output.AppendLine("        var isEmpty = context.ReadBoolean(container.GetRequiredProperty(\"isEmpty\"));");
        output.AppendLine("        var isKnownColor = context.ReadBoolean(container.GetRequiredProperty(\"isKnownColor\"));");
        output.AppendLine("        var isNamedColor = context.ReadBoolean(container.GetRequiredProperty(\"isNamedColor\"));");
        output.AppendLine("        var isSystemColor = context.ReadBoolean(container.GetRequiredProperty(\"isSystemColor\"));");
        output.AppendLine("        var nameValue = container.GetRequiredProperty(\"name\");");
        output.AppendLine("        var name = context.ReadString(nameValue);");
        output.AppendLine("        System.Drawing.Color result;");
        output.AppendLine("        if (isEmpty)");
        output.AppendLine("        {");
        output.AppendLine("            result = System.Drawing.Color.Empty;");
        output.AppendLine("        }");
        output.AppendLine("        else if (isKnownColor || isSystemColor)");
        output.AppendLine("        {");
        output.AppendLine("            var knownColor = ReadNamedEnum<System.Drawing.KnownColor>(context, nameValue);");
        output.AppendLine("            result = System.Drawing.Color.FromKnownColor(knownColor);");
        output.AppendLine("        }");
        output.AppendLine("        else if (isNamedColor)");
        output.AppendLine("        {");
        output.AppendLine("            result = System.Drawing.Color.FromName(name);");
        output.AppendLine("        }");
        output.AppendLine("        else");
        output.AppendLine("        {");
        output.AppendLine("            result = System.Drawing.Color.FromArgb(argb);");
        output.AppendLine("        }");
        output.AppendLine("        if (result.ToArgb() != argb || result.A != a || result.R != r || result.G != g || result.B != b");
        output.AppendLine("            || result.IsEmpty != isEmpty || result.IsKnownColor != isKnownColor || result.IsNamedColor != isNamedColor");
        output.AppendLine("            || result.IsSystemColor != isSystemColor || !string.Equals(result.Name, name, StringComparison.Ordinal))");
        output.AppendLine("        {");
        output.AppendLine("            context.Reject(value, \"Color metadata is not an exact reconstructible native color.\");");
        output.AppendLine("        }");
        output.AppendLine("        return result;");
        output.AppendLine("    }");
    }
}
