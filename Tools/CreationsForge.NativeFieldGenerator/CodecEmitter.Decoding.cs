using System.Reflection;
using System.Text;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.NativeFieldGenerator;

/// <content>Emits complete static Starfield native readers and their exact type dispatch.</content>
internal sealed partial class CodecEmitter
{
    /// <summary>Emits the public-to-project reader entry points and generated all-type test/catalog hooks.</summary>
    /// <returns>Complete reader entry-point source.</returns>
    private string EmitReaderEntryPoints()
    {
        var output = SourceHeader();
        output.AppendLine("using System.Text.Json;");
        output.AppendLine("using CreationsForge.Core.Engine.NativeWire;");
        output.AppendLine("using Mutagen.Bethesda.Starfield;");
        output.AppendLine();
        output.AppendLine("namespace CreationsForge.Starfield.Native.NativeInspection;");
        output.AppendLine();
        output.AppendLine("/// <content>Reads complete Starfield component and condition graphs through generated static native construction.</content>");
        output.AppendLine("internal static partial class StarfieldNestedFieldCodec");
        output.AppendLine("{");
        output.AppendLine("    /// <summary>Reads one complete concrete Starfield component from its closed native wire object.</summary>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned component value.</param>");
        output.AppendLine("    /// <returns>The fully validated concrete mutable component.</returns>");
        output.AppendLine("    internal static AComponent ReadComponent(NativeWireReadContext context, NativeWireValue value)");
        output.AppendLine("    {");
        output.AppendLine("        ArgumentNullException.ThrowIfNull(context);");
        output.AppendLine("        return ReadPolymorphicAComponent(context, value);");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Reads one complete concrete Starfield condition and its concrete condition-data graph.</summary>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned condition value.</param>");
        output.AppendLine("    /// <returns>The fully validated concrete mutable condition.</returns>");
        output.AppendLine("    internal static Condition ReadCondition(NativeWireReadContext context, NativeWireValue value)");
        output.AppendLine("    {");
        output.AppendLine("        ArgumentNullException.ThrowIfNull(context);");
        output.AppendLine("        return ReadPolymorphicCondition(context, value);");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Reads any generated concrete native type by its exact catalog node name.</summary>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned native object value.</param>");
        output.AppendLine("    /// <param name=\"nativeTypeName\">The exact mutable native full name.</param>");
        output.AppendLine("    /// <returns>The fully validated concrete mutable native object.</returns>");
        output.AppendLine("    /// <exception cref=\"ArgumentException\">Thrown when the type name is empty.</exception>");
        output.AppendLine("    /// <exception cref=\"NotSupportedException\">Thrown when the type name is outside generated coverage.</exception>");
        output.AppendLine("    internal static object ReadNativeType(NativeWireReadContext context, NativeWireValue value, string nativeTypeName)");
        output.AppendLine("    {");
        output.AppendLine("        ArgumentNullException.ThrowIfNull(context);");
        output.AppendLine("        ArgumentException.ThrowIfNullOrWhiteSpace(nativeTypeName);");
        output.AppendLine("        return nativeTypeName switch");
        output.AppendLine("        {");
        foreach (var model in Models)
        {
            output.AppendLine($"            \"{Escape(model.MutableType.FullName!)}\" => Read{model.MutableType.Name}(context, value),");
        }

        output.AppendLine("            _ => throw new NotSupportedException($\"Native type '{nativeTypeName}' is outside generated Starfield coverage.\"),");
        output.AppendLine("        };");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Writes any generated concrete native type selected by its exact catalog node name.</summary>");
        output.AppendLine("    /// <param name=\"writer\">The caller-owned JSON writer.</param>");
        output.AppendLine("    /// <param name=\"nativeTypeName\">The exact mutable native full name.</param>");
        output.AppendLine("    /// <param name=\"value\">The matching concrete native getter.</param>");
        output.AppendLine("    /// <param name=\"cancellationToken\">A token observed throughout traversal.</param>");
        output.AppendLine("    /// <exception cref=\"ArgumentException\">Thrown when the type name is empty.</exception>");
        output.AppendLine("    /// <exception cref=\"ArgumentNullException\">Thrown when the writer or native value is null.</exception>");
        output.AppendLine("    /// <exception cref=\"InvalidOperationException\">Thrown when the value does not implement the selected getter type.</exception>");
        output.AppendLine("    /// <exception cref=\"NotSupportedException\">Thrown when the type name is outside generated coverage.</exception>");
        output.AppendLine("    internal static void WriteNativeType(Utf8JsonWriter writer, string nativeTypeName, object value, CancellationToken cancellationToken)");
        output.AppendLine("    {");
        output.AppendLine("        ArgumentNullException.ThrowIfNull(writer);");
        output.AppendLine("        ArgumentException.ThrowIfNullOrWhiteSpace(nativeTypeName);");
        output.AppendLine("        ArgumentNullException.ThrowIfNull(value);");
        output.AppendLine("        switch (nativeTypeName)");
        output.AppendLine("        {");
        foreach (var model in Models)
        {
            var getterName = TypeName(model.GetterType);
            output.AppendLine($"            case \"{Escape(model.MutableType.FullName!)}\" when value is {getterName} typedValue:");
            output.AppendLine($"                Write{model.MutableType.Name}(writer, typedValue, cancellationToken, null);");
            output.AppendLine("                return;");
            output.AppendLine($"            case \"{Escape(model.MutableType.FullName!)}\":");
            output.AppendLine($"                throw new InvalidOperationException(\"Native value does not implement {Escape(getterName)}.\");");
        }

        output.AppendLine("            default:");
        output.AppendLine("                throw new NotSupportedException($\"Native type '{nativeTypeName}' is outside generated Starfield coverage.\");");
        output.AppendLine("        }");
        output.AppendLine("    }");
        output.AppendLine();
        output.AppendLine("    /// <summary>Constructs and writes the actual installed default for any generated concrete native type.</summary>");
        output.AppendLine("    /// <param name=\"writer\">The caller-owned JSON writer.</param>");
        output.AppendLine("    /// <param name=\"nativeTypeName\">The exact mutable native full name.</param>");
        output.AppendLine("    /// <param name=\"cancellationToken\">A token observed throughout traversal.</param>");
        output.AppendLine("    /// <exception cref=\"ArgumentException\">Thrown when the type name is empty.</exception>");
        output.AppendLine("    /// <exception cref=\"ArgumentNullException\">Thrown when the writer is null.</exception>");
        output.AppendLine("    /// <exception cref=\"NotSupportedException\">Thrown when the type name is outside generated coverage.</exception>");
        output.AppendLine("    internal static void WriteDefaultNativeType(Utf8JsonWriter writer, string nativeTypeName, CancellationToken cancellationToken)");
        output.AppendLine("    {");
        output.AppendLine("        ArgumentNullException.ThrowIfNull(writer);");
        output.AppendLine("        ArgumentException.ThrowIfNullOrWhiteSpace(nativeTypeName);");
        output.AppendLine("        switch (nativeTypeName)");
        output.AppendLine("        {");
        foreach (var model in Models)
        {
            output.AppendLine($"            case \"{Escape(model.MutableType.FullName!)}\":");
            output.AppendLine($"                Write{model.MutableType.Name}(writer, new {TypeName(model.MutableType)}(), cancellationToken, null);");
            output.AppendLine("                return;");
        }

        output.AppendLine("            default:");
        output.AppendLine("                throw new NotSupportedException($\"Native type '{nativeTypeName}' is outside generated Starfield coverage.\");");
        output.AppendLine("        }");
        output.AppendLine("    }");
        return Finish(output);
    }

    /// <summary>Emits every generated polymorphic reader dispatcher.</summary>
    /// <returns>Complete reader dispatch source.</returns>
    private string EmitReaderDispatch()
    {
        var output = ReaderGeneratedHeader();
        foreach (var polymorphic in PolymorphicTypes)
        {
            EmitPolymorphicReaderDispatcher(output, polymorphic);
        }

        return Finish(output);
    }

    /// <summary>Emits one partition of concrete native reader methods and closed object shapes.</summary>
    /// <param name="models">The concrete native models assigned to this partition.</param>
    /// <returns>Complete generated reader source.</returns>
    private string EmitReaders(IEnumerable<NativeTypeModel> models)
    {
        var output = ReaderGeneratedHeader();
        foreach (var model in models)
        {
            EmitConcreteReadShape(output, model);
            EmitConcreteReader(output, model);
        }

        return Finish(output);
    }

    /// <summary>Emits one discriminator-only union shape and its exact concrete reader switch.</summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="polymorphicType">The abstract mutable type being dispatched.</param>
    private void EmitPolymorphicReaderDispatcher(StringBuilder output, Type polymorphicType)
    {
        var getter = FindGetter(polymorphicType)
            ?? throw new InvalidOperationException($"No getter interface found for polymorphic type {polymorphicType.FullName}.");
        var derived = Models
            .Where(model => polymorphicType.IsAssignableFrom(model.MutableType))
            .OrderBy(model => model.MutableType.FullName, StringComparer.Ordinal)
            .ToArray();
        var optionalNames = derived
            .SelectMany(static model => model.Fields.Select(static field => field.Name))
            .Concat(typeof(ConditionData).IsAssignableFrom(polymorphicType) ? new[] { "Function" } : Array.Empty<string>())
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();
        output.AppendLine($"    /// <summary>Defines the discriminator projection accepted before selecting a concrete {polymorphicType.Name} shape.</summary>");
        output.AppendLine($"    private static readonly NativeWireObjectShape Polymorphic{polymorphicType.Name}ReadShape = new(");
        output.AppendLine($"        \"{Escape(polymorphicType.FullName!)} discriminator\",");
        output.AppendLine("        new[] { \"$type\" },");
        output.AppendLine("        new[]");
        output.AppendLine("        {");
        foreach (var name in optionalNames)
        {
            output.AppendLine($"            \"{Escape(name)}\",");
        }

        output.AppendLine("        });");
        output.AppendLine();
        output.AppendLine($"    /// <summary>Reads one concrete native {polymorphicType.Name} selected by its exact package-scoped discriminator.</summary>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned native object value.</param>");
        output.AppendLine("    /// <returns>The fully validated concrete mutable native value.</returns>");
        output.AppendLine($"    private static {TypeName(polymorphicType)} ReadPolymorphic{polymorphicType.Name}(NativeWireReadContext context, NativeWireValue value)");
        output.AppendLine("    {");
        output.AppendLine($"        var objectValue = context.ReadObject(value, Polymorphic{polymorphicType.Name}ReadShape);");
        output.AppendLine("        var typeValue = objectValue.GetRequiredProperty(\"$type\");");
        output.AppendLine("        var typeName = context.ReadString(typeValue);");
        output.AppendLine("        return typeName switch");
        output.AppendLine("        {");
        foreach (var model in derived)
        {
            output.AppendLine($"            NativeTypeNamespace + \"{Escape(model.MutableType.FullName!)}\" => Read{model.MutableType.Name}(context, value),");
        }

        output.AppendLine($"            _ => RejectValue<{TypeName(polymorphicType)}>(context, typeValue, $\"Native type '{{typeName}}' is not a generated {Escape(getter.FullName!)} implementation.\"),");
        output.AppendLine("        };");
        output.AppendLine("    }");
        output.AppendLine();
    }

    /// <summary>Emits the exact closed object shape used by one concrete native reader.</summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="model">The concrete native model.</param>
    private static void EmitConcreteReadShape(StringBuilder output, NativeTypeModel model)
    {
        var hasOptionalDerivedFunction = typeof(ConditionData).IsAssignableFrom(model.MutableType)
            && !model.Fields.Any(static field => string.Equals(field.Name, "Function", StringComparison.Ordinal));
        output.AppendLine($"    /// <summary>Defines the complete required and optional wire property shape for native {model.MutableType.Name}.</summary>");
        output.AppendLine($"    private static readonly NativeWireObjectShape {model.MutableType.Name}ReadShape = new(");
        output.AppendLine($"        \"{Escape(model.MutableType.FullName!)}\",");
        output.AppendLine("        new[]");
        output.AppendLine("        {");
        output.AppendLine("            \"$type\",");
        foreach (var field in model.Fields)
        {
            output.AppendLine($"            \"{Escape(field.Name)}\",");
        }

        output.Append("        }");
        if (hasOptionalDerivedFunction)
        {
            output.AppendLine(",");
            output.AppendLine("        new[] { \"Function\" });");
        }
        else
        {
            output.AppendLine(");");
        }

        output.AppendLine();
    }

    /// <summary>Emits a complete concrete native reader with owner-linked construction deferred until its owner exists.</summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="model">The concrete native model.</param>
    private void EmitConcreteReader(StringBuilder output, NativeTypeModel model)
    {
        if (model.Construction.Kind != NativeTypeConstructionKind.PublicParameterlessConstructor
            || model.Construction.RequiredMemberNames.Count != 0)
        {
            throw new InvalidOperationException($"Native type {model.MutableType.FullName} is not directly default constructible.");
        }

        var ownerLinkedFields = model.Fields
            .Where(static field => field.Construction.Kind == NativeValueConstructionKind.FormLinkOrIndex)
            .ToArray();
        var initOnlyFields = model.Fields
            .Where(static field => IsInitOnly(field.MutableProperty))
            .ToArray();
        if (ownerLinkedFields.Length > 0
            && !model.MutableType.GetInterfaces().Any(static type => type.FullName == "Mutagen.Bethesda.Plugins.IFormLinkOrIndexFlagGetter"))
        {
            throw new InvalidOperationException($"Native owner {model.MutableType.FullName} does not implement IFormLinkOrIndexFlagGetter.");
        }

        if (ownerLinkedFields.Any(static field => IsInitOnly(field.MutableProperty)))
        {
            throw new InvalidOperationException($"Native owner {model.MutableType.FullName} exposes an init-only owner-linked field that cannot be bound after construction.");
        }

        foreach (var ownerLinkedField in ownerLinkedFields)
        {
            foreach (var prerequisiteName in new[] { "UseAliases", "UsePackageData" })
            {
                var prerequisite = model.Fields.SingleOrDefault(field => string.Equals(field.Name, prerequisiteName, StringComparison.Ordinal));
                if (prerequisite is null || prerequisite.Ordinal >= ownerLinkedField.Ordinal)
                {
                    throw new InvalidOperationException(
                        $"Native owner {model.MutableType.FullName} must expose {prerequisiteName} before owner-linked field {ownerLinkedField.Name}.");
                }
            }
        }

        output.AppendLine($"    /// <summary>Reads every indexed field of native {model.MutableType.Name} in FieldIndex order.</summary>");
        output.AppendLine("    /// <param name=\"context\">The request-local bounded read context.</param>");
        output.AppendLine("    /// <param name=\"value\">The context-owned native object value.</param>");
        output.AppendLine("    /// <returns>The fully validated concrete mutable native value.</returns>");
        output.AppendLine($"    private static {TypeName(model.MutableType)} Read{model.MutableType.Name}(NativeWireReadContext context, NativeWireValue value)");
        output.AppendLine("    {");
        output.AppendLine($"        var objectValue = context.ReadObject(value, {model.MutableType.Name}ReadShape);");
        output.AppendLine("        var typeValue = objectValue.GetRequiredProperty(\"$type\");");
        output.AppendLine($"        ReadAndValidateType(context, typeValue, \"{Escape(model.MutableType.FullName!)}\");");
        var validatesDerivedFunction = typeof(ConditionData).IsAssignableFrom(model.MutableType)
            && !model.Fields.Any(static field => string.Equals(field.Name, "Function", StringComparison.Ordinal));
        if (validatesDerivedFunction)
        {
            output.AppendLine("        var hasFunction = objectValue.TryGetOptionalProperty(\"Function\", out var functionValue);");
            output.AppendLine("        var function = hasFunction ? context.ReadUInt16(functionValue) : (ushort?)null;");
        }

        for (var index = 0; index < model.Fields.Count; index++)
        {
            var field = model.Fields[index];
            output.AppendLine($"        var field{index}Value = objectValue.GetRequiredProperty(\"{Escape(field.Name)}\");");
            EmitReadValue(output, field.Construction, $"field{index}Value", $"field{index}", "        ");
        }

        if (initOnlyFields.Length == 0)
        {
            output.AppendLine($"        var result = new {TypeName(model.MutableType)}();");
        }
        else
        {
            output.AppendLine($"        var result = new {TypeName(model.MutableType)}");
            output.AppendLine("        {");
            for (var index = 0; index < model.Fields.Count; index++)
            {
                var field = model.Fields[index];
                if (IsInitOnly(field.MutableProperty))
                {
                    output.AppendLine($"            {field.Name} = field{index},");
                }
            }

            output.AppendLine("        };");
        }

        for (var index = 0; index < model.Fields.Count; index++)
        {
            var field = model.Fields[index];
            if (field.Construction.Kind == NativeValueConstructionKind.FormLinkOrIndex)
            {
                EmitOwnerLinkedAssignment(output, model, field, index, "        ");
            }
            else if (!IsInitOnly(field.MutableProperty))
            {
                output.AppendLine($"        result.{field.Name} = field{index};");
            }
        }

        if (validatesDerivedFunction)
        {
            output.AppendLine("        if (function.HasValue && (ushort)((IConditionDataGetter)result).Function != function.Value)");
            output.AppendLine("        {");
            output.AppendLine("            context.Reject(functionValue, $\"Derived Function {function.Value} does not match concrete native type value {(ushort)((IConditionDataGetter)result).Function}.\");");
            output.AppendLine("        }");
        }

        output.AppendLine("        return result;");
        output.AppendLine("    }");
        output.AppendLine();
    }

    /// <summary>Determines whether a reflected public property setter is restricted to object initialization.</summary>
    /// <param name="property">The mutable native property.</param>
    /// <returns><see langword="true"/> when the setter carries the required <c>IsExternalInit</c> modifier.</returns>
    private static bool IsInitOnly(PropertyInfo property)
    {
        return property.SetMethod?.ReturnParameter.GetRequiredCustomModifiers()
            .Contains(typeof(System.Runtime.CompilerServices.IsExternalInit)) == true;
    }

    /// <summary>Emits assignment and derived owner-flag validation for one link-or-index field.</summary>
    /// <param name="output">The generated source builder.</param>
    /// <param name="model">The concrete owner model.</param>
    /// <param name="field">The owner-linked field.</param>
    /// <param name="index">The stable field-local index.</param>
    /// <param name="indent">The current source indentation.</param>
    private static void EmitOwnerLinkedAssignment(
        StringBuilder output,
        NativeTypeModel model,
        NativeFieldModel field,
        int index,
        string indent)
    {
        var targetType = field.Construction.MutableType.GetGenericArguments()[0];
        var nullable = AllowsNull(field.Construction);
        if (nullable)
        {
            output.AppendLine($"{indent}if (field{index}.HasValue)");
            output.AppendLine($"{indent}{{");
            indent += "    ";
        }

        var valueExpression = nullable ? $"field{index}.Value" : $"field{index}";
        output.AppendLine($"{indent}var field{index}Linked = new FormLinkOrIndex<{TypeName(targetType)}>(result);");
        output.AppendLine($"{indent}field{index}Linked.Index = {valueExpression}.Index;");
        output.AppendLine($"{indent}field{index}Linked.Link.FormKeyNullable = {valueExpression}.Link;");
        output.AppendLine($"{indent}result.{field.Name} = field{index}Linked;");
        output.AppendLine($"{indent}if ({valueExpression}.UsesLink.HasValue && field{index}Linked.UsesLink() != {valueExpression}.UsesLink.Value)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    context.Reject({valueExpression}.UsesLinkValue!.Value, \"Link-or-index usesLink does not match the decoded native owner state.\");");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}if ({valueExpression}.UsesAlias.HasValue && field{index}Linked.UsesAlias() != {valueExpression}.UsesAlias.Value)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    context.Reject({valueExpression}.UsesAliasValue!.Value, \"Link-or-index usesAlias does not match the decoded native owner state.\");");
        output.AppendLine($"{indent}}}");
        output.AppendLine($"{indent}if ({valueExpression}.UsesPackageData.HasValue && field{index}Linked.UsesPackageData() != {valueExpression}.UsesPackageData.Value)");
        output.AppendLine($"{indent}{{");
        output.AppendLine($"{indent}    context.Reject({valueExpression}.UsesPackageDataValue!.Value, \"Link-or-index usesPackageData does not match the decoded native owner state.\");");
        output.AppendLine($"{indent}}}");
        if (nullable)
        {
            indent = indent[..^4];
            output.AppendLine($"{indent}}}");
            output.AppendLine($"{indent}else");
            output.AppendLine($"{indent}{{");
            output.AppendLine($"{indent}    result.{field.Name} = null!;");
            output.AppendLine($"{indent}}}");
        }
    }
}
