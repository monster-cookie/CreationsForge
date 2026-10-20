using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Engine.Records;

/// <summary>Binds current Mutagen properties to the closed record-field contract.</summary>
internal static class MutagenMemberBinder
{
    private static readonly string[] SkippedMembers =
    [
        "EditorID",
        "FormKey",
        "IsCompressed",
        "IsDeleted",
        "MajorFlags",
        "MajorRecordFlagsRaw",
        "FormVersion",
        "Version2",
        "VersionControl",
        "TitleString",
        "RawFloat",
        "Registration",
        "StaticRegistration",
    ];

    /// <summary>Binds editable members declared for one Mutagen record.</summary>
    /// <param name="recordType">The mutable Mutagen record type.</param>
    /// <param name="getterType">The matching getter interface.</param>
    /// <param name="release">The game release passed to the record constructor.</param>
    /// <returns>The fields that can be read from a newly constructed record.</returns>
    public static IReadOnlyList<RecordField> Bind(Type recordType, Type getterType, object release)
    {
        var fields = new List<RecordField>
        {
            BindEditorId(recordType, getterType),
        };
        object? sample = null;
        foreach (var mutable in recordType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (mutable.GetIndexParameters().Length > 0 || SkippedMembers.Contains(mutable.Name, StringComparer.Ordinal) || mutable.Name.EndsWith("MajorRecordFlags", StringComparison.Ordinal))
            {
                continue;
            }

            var readable = getterType.GetProperty(mutable.Name, BindingFlags.Instance | BindingFlags.Public);
            if (readable?.GetMethod is null || mutable.GetMethod is null)
            {
                continue;
            }

            sample ??= TryConstruct(recordType, release);
            if (sample is not null && !CanRead(readable, sample))
            {
                continue;
            }

            RecordField? field = null;
            try
            {
                field = TryBindMember(recordType, getterType, mutable, readable);
            }
            catch (Exception)
            {
                field = null;
            }

            if (field is not null)
            {
                fields.Add(field);
            }
        }

        return fields;
    }

    /// <summary>Supplies values Mutagen omits unless a concrete subtype or non-default global value is present.</summary>
    /// <param name="record">The newly constructed record.</param>
    public static void PrepareRequiredMembers(IMajorRecord record)
    {
        var type = record.GetType();
        if (type.Name.StartsWith("Global", StringComparison.Ordinal))
        {
            AssignGlobalData(record, type.GetProperty("Data"));
        }

        foreach (var name in new[] { "Data", "Rules", "Options" })
        {
            var property = type.GetProperty(name);
            if (property?.GetMethod is null || property.SetMethod is null || property.GetValue(record) is not null || !property.PropertyType.IsAbstract)
            {
                continue;
            }

            var concrete = ChooseConcrete(property.PropertyType);
            if (concrete is not null)
            {
                property.SetValue(record, Activator.CreateInstance(concrete));
            }
        }
    }

    private static RecordField BindEditorId(Type recordType, Type getterType)
    {
        return InvokeFactory(
            nameof(RecordFields.String),
            [recordType, getterType],
            [
                "EditorID",
                true,
                Getter(getterType, typeof(string), typeof(IMajorRecordGetter).GetProperty(nameof(IMajorRecordGetter.EditorID))!),
                Setter(recordType, typeof(string), typeof(IMajorRecord).GetProperty(nameof(IMajorRecord.EditorID))!),
            ]);
    }

    private static RecordField? TryBindMember(Type recordType, Type getterType, PropertyInfo mutable, PropertyInfo readable)
    {
        var propertyType = Nullable.GetUnderlyingType(mutable.PropertyType) ?? mutable.PropertyType;
        var nullable = mutable.PropertyType != propertyType || !propertyType.IsValueType;
        if (propertyType == typeof(string))
        {
            return InvokeFactory(nameof(RecordFields.String), [recordType, getterType], [mutable.Name, nullable, Getter(getterType, typeof(string), readable), Setter(recordType, typeof(string), mutable)]);
        }

        if (typeof(ITranslatedStringGetter).IsAssignableFrom(propertyType))
        {
            return InvokeFactory(
                nameof(RecordFields.TranslatedString),
                [recordType, getterType],
                [mutable.Name, true, Getter(getterType, typeof(ITranslatedStringGetter), readable), Setter(recordType, typeof(TranslatedString), mutable)]);
        }

        if (propertyType == typeof(float))
        {
            return InvokeFactory(nullable ? nameof(RecordFields.NullableSingle) : nameof(RecordFields.Single), [recordType, getterType], nullable
                ? [mutable.Name, Getter(getterType, typeof(float?), readable), Setter(recordType, typeof(float?), mutable)]
                : [mutable.Name, Getter(getterType, typeof(float), readable), Setter(recordType, typeof(float), mutable)]);
        }

        if (propertyType == typeof(int))
        {
            if (nullable)
            {
                return null;
            }

            return InvokeFactory(nameof(RecordFields.Int32), [recordType, getterType], [mutable.Name, Getter(getterType, typeof(int), readable), Setter(recordType, typeof(int), mutable)]);
        }

        if (propertyType == typeof(uint))
        {
            return InvokeFactory(nullable ? nameof(RecordFields.NullableUInt32) : nameof(RecordFields.UInt32), [recordType, getterType], nullable
                ? [mutable.Name, Getter(getterType, typeof(uint?), readable), Setter(recordType, typeof(uint?), mutable)]
                : [mutable.Name, Getter(getterType, typeof(uint), readable), Setter(recordType, typeof(uint), mutable)]);
        }

        if (propertyType == typeof(byte))
        {
            if (nullable)
            {
                return null;
            }

            return InvokeFactory(nameof(RecordFields.Byte), [recordType, getterType], [mutable.Name, Getter(getterType, typeof(byte), readable), Setter(recordType, typeof(byte), mutable)]);
        }

        if (propertyType == typeof(ushort))
        {
            return InvokeFactory(nameof(RecordFields.NullableUInt16), [recordType, getterType], [mutable.Name, Getter(getterType, typeof(ushort?), readable), Setter(recordType, typeof(ushort?), mutable)]);
        }

        if (propertyType.IsEnum)
        {
            var enumNullable = typeof(Nullable<>).MakeGenericType(propertyType);
            return (RecordField)typeof(RecordFields).GetMethod(nameof(RecordFields.Enum))!.MakeGenericMethod(recordType, getterType, propertyType).Invoke(null,
            [
                mutable.Name,
                nullable,
                Getter(getterType, enumNullable, readable),
                EnumSetter(recordType, propertyType, mutable),
            ])!;
        }

        if (IsFormLink(propertyType))
        {
            var target = FormLinkTarget(propertyType);
            return InvokeFactory(nameof(RecordFields.FormLink), [recordType, getterType], [mutable.Name, new[] { target }, FormLinkReader(getterType, readable), FormLinkWriter(recordType, mutable)]);
        }

        if (TryGetFormLinkListTarget(mutable.PropertyType, out var listTarget))
        {
            return InvokeFactory(
                nameof(RecordFields.FormLinkList),
                [recordType, getterType],
                [mutable.Name, new[] { listTarget }, FormLinkListReader(getterType, readable), FormLinkListWriter(recordType, mutable, listTarget)]);
        }

        if (mutable.Name == "ObjectBounds")
        {
            return BindObjectBounds(recordType, getterType, mutable, readable);
        }

        if (mutable.Name == "Teaches" && mutable.PropertyType.IsAbstract)
        {
            return BindPolymorphic(recordType, getterType, mutable, readable);
        }

        return null;
    }

    private static RecordField BindObjectBounds(Type recordType, Type getterType, PropertyInfo mutable, PropertyInfo readable)
    {
        var component = mutable.PropertyType.GetProperty("First")?.PropertyType.GetProperty("X")?.PropertyType ?? typeof(float);
        var readType = typeof(Func<,>).MakeGenericType(getterType, typeof(RecordValue));
        var validateType = typeof(Action<>).MakeGenericType(typeof(RecordValue));
        var writeType = typeof(Action<,>).MakeGenericType(recordType, typeof(RecordValue));
        var getter = Parameter(getterType, "getter");
        var validateValue = Parameter(typeof(RecordValue), "value");
        var record = Parameter(recordType, "record");
        var writeValue = Parameter(typeof(RecordValue), "value");
        return InvokeFactory(
            nameof(RecordFields.Object),
            [recordType, getterType],
            [
                "ObjectBounds",
                false,
                new[] { "ObjectBounds" },
                Lambda(readType, Expression.Call(typeof(MutagenMemberBinder).GetMethod(nameof(ReadBounds))!, Expression.Convert(getter, typeof(object)), Expression.Constant(readable)), getter),
                Lambda(validateType, Expression.Call(typeof(MutagenMemberBinder).GetMethod(nameof(ValidateBounds))!, validateValue, Expression.Constant(component)), validateValue),
                Lambda(writeType, Expression.Call(typeof(MutagenMemberBinder).GetMethod(nameof(WriteBounds))!, Expression.Convert(record, typeof(object)), Expression.Constant(mutable), writeValue), record, writeValue),
                null,
            ]);
    }

    private static RecordField BindPolymorphic(Type recordType, Type getterType, PropertyInfo mutable, PropertyInfo readable)
    {
        var subtypes = ConcreteSubtypes(mutable.PropertyType);
        var alternatives = subtypes.Select(type => AlternativeName(recordType, type)).ToArray();
        var references = new Dictionary<string, Type>(StringComparer.Ordinal);
        foreach (var subtype in subtypes)
        {
            var link = subtype.GetProperties().FirstOrDefault(property => IsFormLink(property.PropertyType));
            if (link is not null)
            {
                references[AlternativeName(recordType, subtype)] = FormLinkTarget(link.PropertyType);
            }
        }

        var readType = typeof(Func<,>).MakeGenericType(getterType, typeof(RecordValue));
        var validateType = typeof(Action<>).MakeGenericType(typeof(RecordValue));
        var writeType = typeof(Action<,>).MakeGenericType(recordType, typeof(RecordValue));
        var getter = Parameter(getterType, "getter");
        var validateValue = Parameter(typeof(RecordValue), "value");
        var record = Parameter(recordType, "record");
        var writeValue = Parameter(typeof(RecordValue), "value");
        return InvokeFactory(
            nameof(RecordFields.Object),
            [recordType, getterType],
            [
                mutable.Name,
                false,
                alternatives,
                Lambda(readType, Expression.Call(typeof(MutagenMemberBinder).GetMethod(nameof(ReadPolymorphic))!, Expression.Convert(getter, typeof(object)), Expression.Constant(readable), Expression.Constant(recordType)), getter),
                Lambda(validateType, Expression.Call(typeof(MutagenMemberBinder).GetMethod(nameof(ValidatePolymorphic))!, validateValue, Expression.Constant(mutable.Name)), validateValue),
                Lambda(writeType, Expression.Call(typeof(MutagenMemberBinder).GetMethod(nameof(WritePolymorphic))!, Expression.Convert(record, typeof(object)), Expression.Constant(mutable), writeValue, Expression.Constant(subtypes)), record, writeValue),
                references,
            ]);
    }

    private static void AssignGlobalData(IMajorRecord record, PropertyInfo? data)
    {
        if (data?.SetMethod is null || data.GetValue(record) is not null)
        {
            return;
        }

        var type = Nullable.GetUnderlyingType(data.PropertyType) ?? data.PropertyType;
        object? value = type == typeof(float) ? 1.5f
            : type == typeof(int) ? 1
            : type == typeof(short) ? (short)1
            : type == typeof(bool) ? true
            : null;
        if (value is not null)
        {
            data.SetValue(record, value);
        }
    }

    private static Type? ChooseConcrete(Type abstractType)
    {
        var candidates = ConcreteSubtypes(abstractType);
        return candidates.FirstOrDefault(type => type.Name.Contains("Standard", StringComparison.Ordinal))
            ?? candidates.FirstOrDefault(type => type.Name.EndsWith("Voice", StringComparison.Ordinal))
            ?? candidates.FirstOrDefault(type => type.Name.EndsWith("Leaf", StringComparison.Ordinal))
            ?? candidates.FirstOrDefault(type => type.Name.Contains("Nothing", StringComparison.Ordinal))
            ?? candidates.FirstOrDefault();
    }

    private static Type[] ConcreteSubtypes(Type abstractType)
    {
        return abstractType.Assembly.GetTypes()
            .Where(type => type.IsPublic && !type.IsAbstract && abstractType.IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) is not null)
            .OrderBy(type => type.Name, StringComparer.Ordinal)
            .ToArray();
    }

    private static string AlternativeName(Type recordType, Type subtype)
    {
        if (subtype.Name.Contains("Nothing", StringComparison.Ordinal))
        {
            return "None";
        }

        return subtype.Name.StartsWith(recordType.Name, StringComparison.Ordinal)
            ? subtype.Name[recordType.Name.Length..]
            : subtype.Name;
    }

    /// <summary>Reads an object-bounds member into the closed object value.</summary>
    public static RecordValue ReadBounds(object getter, PropertyInfo property)
    {
        var bounds = property.GetValue(getter);
        if (bounds is null)
        {
            return RecordValue.Null;
        }

        var fields = new Dictionary<string, RecordValue>(StringComparer.Ordinal);
        foreach (var corner in new[] { "First", "Second" })
        {
            var point = bounds.GetType().GetProperty(corner)?.GetValue(bounds);
            if (point is null)
            {
                return RecordValue.Null;
            }

            foreach (var axis in new[] { "X", "Y", "Z" })
            {
                fields[$"{corner}.{axis}"] = NumberValue(point.GetType().GetProperty(axis)?.GetValue(point));
            }
        }

        return RecordValue.FromObject("ObjectBounds", fields);
    }

    /// <summary>Rejects an object-bounds value whose six components are missing.</summary>
    public static void ValidateBounds(RecordValue value, Type componentType)
    {
        var fields = ((RecordValue.ObjectRecordValue)value).Fields;
        var paths = new[] { "First.X", "First.Y", "First.Z", "Second.X", "Second.Y", "Second.Z" };
        if (paths.Any(path => !fields.TryGetValue(path, out var component) || !MatchesComponent(component, componentType)))
        {
            throw new RecordEditingException("ObjectBounds requires First and Second X, Y, and Z components.");
        }
    }

    /// <summary>Writes an object-bounds value back onto the Mutagen record.</summary>
    public static void WriteBounds(object record, PropertyInfo property, RecordValue value)
    {
        var fields = ((RecordValue.ObjectRecordValue)value).Fields;
        var bounds = Activator.CreateInstance(property.PropertyType)!;
        foreach (var corner in new[] { "First", "Second" })
        {
            var pointProperty = bounds.GetType().GetProperty(corner)!;
            var point = Activator.CreateInstance(pointProperty.PropertyType)!;
            foreach (var axis in new[] { "X", "Y", "Z" })
            {
                var axisProperty = point.GetType().GetProperty(axis)!;
                axisProperty.SetValue(point, ChangeType(Component(fields[$"{corner}.{axis}"]), axisProperty.PropertyType));
            }

            pointProperty.SetValue(bounds, point);
        }

        property.SetValue(record, bounds);
    }

    /// <summary>Reads a polymorphic member such as Book.Teaches.</summary>
    public static RecordValue ReadPolymorphic(object getter, PropertyInfo property, Type recordType)
    {
        var value = property.GetValue(getter);
        if (value is null)
        {
            throw new RecordEditingException($"{recordType.Name}.{property.Name} is required.");
        }

        var alternative = AlternativeName(recordType, value.GetType());
        var link = value.GetType().GetProperties().FirstOrDefault(candidate => IsFormLink(candidate.PropertyType));
        if (link is not null)
        {
            var formKey = (FormKey)link.GetValue(value)!.GetType().GetProperty(nameof(FormKey))!.GetValue(link.GetValue(value))!;
            return RecordValue.FromObject(alternative, new Dictionary<string, RecordValue> { ["FormKey"] = RecordValue.FromFormLink(formKey) });
        }

        var raw = value.GetType().GetProperty("RawContent")?.GetValue(value);
        return RecordValue.FromObject(alternative, new Dictionary<string, RecordValue>
        {
            ["RawContent"] = RecordValue.FromUnsignedInteger(Convert.ToUInt64(raw ?? 0)),
        });
    }

    /// <summary>Checks the closed shape of a polymorphic member.</summary>
    public static void ValidatePolymorphic(RecordValue value, string path)
    {
        var target = (RecordValue.ObjectRecordValue)value;
        if (target.Alternative == "None")
        {
            if (target.Fields.Count != 1 || target.Fields.GetValueOrDefault("RawContent") is not RecordValue.UnsignedIntegerRecordValue)
            {
                throw new RecordEditingException($"{path} None requires one UInt32 RawContent field.");
            }

            return;
        }

        if (target.Fields.Count != 1 || target.Fields.GetValueOrDefault("FormKey") is not RecordValue.FormLinkRecordValue)
        {
            throw new RecordEditingException($"{path} {target.Alternative} requires one FormKey field.");
        }
    }

    /// <summary>Constructs the selected polymorphic subtype and assigns it.</summary>
    public static void WritePolymorphic(object record, PropertyInfo property, RecordValue value, Type[] subtypes)
    {
        var target = (RecordValue.ObjectRecordValue)value;
        var recordType = record.GetType();
        var subtype = subtypes.Single(type => AlternativeName(recordType, type) == target.Alternative);
        var instance = Activator.CreateInstance(subtype)!;
        if (target.Alternative == "None")
        {
            subtype.GetProperty("RawContent")?.SetValue(instance, ChangeType(((RecordValue.UnsignedIntegerRecordValue)target.Fields["RawContent"]).Value, subtype.GetProperty("RawContent")!.PropertyType));
        }
        else
        {
            var link = subtype.GetProperties().First(candidate => IsFormLink(candidate.PropertyType));
            var formKey = ((RecordValue.FormLinkRecordValue)target.Fields["FormKey"]).FormKey;
            link.SetValue(instance, Activator.CreateInstance(typeof(FormLink<>).MakeGenericType(FormLinkTarget(link.PropertyType)), formKey));
        }

        property.SetValue(record, instance);
    }

    private static RecordValue NumberValue(object? value)
    {
        return value switch
        {
            float number => RecordValue.FromFloatingPoint(number),
            double number => RecordValue.FromFloatingPoint(number),
            _ => RecordValue.FromSignedInteger(Convert.ToInt64(value)),
        };
    }

    private static bool MatchesComponent(RecordValue value, Type componentType)
    {
        return componentType == typeof(float) || componentType == typeof(double)
            ? value is RecordValue.FloatingPointRecordValue
            : value is RecordValue.SignedIntegerRecordValue;
    }

    private static object Component(RecordValue value)
    {
        return value is RecordValue.FloatingPointRecordValue floating ? floating.Value : ((RecordValue.SignedIntegerRecordValue)value).Value;
    }

    private static object ChangeType(object value, Type target)
    {
        var underlying = Nullable.GetUnderlyingType(target) ?? target;
        return Convert.ChangeType(value, underlying);
    }

    private static bool IsFormLink(Type type)
    {
        return type.Name.Contains("FormLink", StringComparison.Ordinal) && FormKeyProperty(type) is not null;
    }

    private static PropertyInfo? FormKeyProperty(Type type)
    {
        return type.GetProperty(nameof(FormKey))
            ?? type.GetInterfaces().Select(implemented => implemented.GetProperty(nameof(FormKey))).FirstOrDefault(property => property is not null);
    }

    private static bool TryGetFormLinkListTarget(Type propertyType, out Type target)
    {
        target = typeof(IMajorRecordGetter);
        var element = propertyType.IsGenericType ? propertyType.GenericTypeArguments.FirstOrDefault() : null;
        if (element is null || !IsFormLink(element))
        {
            return false;
        }

        target = FormLinkTarget(element);
        return true;
    }

    private static Type FormLinkTarget(Type linkType)
    {
        return linkType.IsGenericType ? linkType.GenericTypeArguments[0] : typeof(IMajorRecordGetter);
    }

    private static Delegate FormLinkReader(Type getterType, PropertyInfo property)
    {
        return Call(typeof(Func<,>).MakeGenericType(getterType, typeof(FormKey?)), nameof(ReadFormLink), getterType, property);
    }

    private static Delegate FormLinkWriter(Type recordType, PropertyInfo property)
    {
        var value = Parameter(typeof(FormKey?), "value");
        var record = Parameter(recordType, "record");
        return Lambda(
            typeof(Action<,>).MakeGenericType(recordType, typeof(FormKey?)),
            Expression.Call(typeof(MutagenMemberBinder).GetMethod(nameof(WriteFormLink))!, Expression.Convert(record, typeof(object)), Expression.Constant(property), value),
            record,
            value);
    }

    private static Delegate FormLinkListReader(Type getterType, PropertyInfo property)
    {
        return Call(typeof(Func<,>).MakeGenericType(getterType, typeof(IReadOnlyList<FormKey>)), nameof(ReadFormLinks), getterType, property);
    }

    private static Delegate FormLinkListWriter(Type recordType, PropertyInfo property, Type target)
    {
        var values = Parameter(typeof(IReadOnlyList<FormKey>), "values");
        var record = Parameter(recordType, "record");
        return Lambda(
            typeof(Action<,>).MakeGenericType(recordType, typeof(IReadOnlyList<FormKey>)),
            Expression.Call(typeof(MutagenMemberBinder).GetMethod(nameof(ReplaceFormLinks))!, Expression.Convert(record, typeof(object)), Expression.Constant(property), values, Expression.Constant(target)),
            record,
            values);
    }

    /// <summary>Reads one form link, preserving a null link as a null form key.</summary>
    public static FormKey? ReadFormLink(object getter, PropertyInfo property)
    {
        var link = property.GetValue(getter);
        if (link is null)
        {
            return null;
        }

        return (FormKey)FormKeyProperty(link.GetType())!.GetValue(link)!;
    }

    /// <summary>Writes one form link through Mutagen's link setter.</summary>
    public static void WriteFormLink(object record, PropertyInfo property, FormKey? value)
    {
        var link = property.GetValue(record) ?? throw new RecordEditingException($"Field '{property.Name}' is not initialized on this record.");
        var method = value is null
            ? link.GetType().GetMethod("Clear") ?? link.GetType().GetMethod("SetToNull")
            : link.GetType().GetMethods().First(candidate => candidate.Name == "SetTo" && candidate.GetParameters() is [{ ParameterType: var parameter }] && parameter == typeof(FormKey));
        method?.Invoke(link, value is null ? null : [value.Value]);
    }

    /// <summary>Reads an ordered form-link list.</summary>
    public static IReadOnlyList<FormKey> ReadFormLinks(object getter, PropertyInfo property)
    {
        if (property.GetValue(getter) is not IEnumerable list)
        {
            return [];
        }

        var keys = new List<FormKey>();
        foreach (var item in list)
        {
            if (item is null)
            {
                continue;
            }

            var key = (FormKey)FormKeyProperty(item.GetType())!.GetValue(item)!;
            if (!key.IsNull)
            {
                keys.Add(key);
            }
        }

        return keys;
    }

    /// <summary>Replaces an ordered form-link list, creating it when the member is still null.</summary>
    public static void ReplaceFormLinks(object record, PropertyInfo property, IReadOnlyList<FormKey> values, Type target)
    {
        var list = property.GetValue(record);
        if (list is null)
        {
            var listType = property.PropertyType.IsAbstract || property.PropertyType.IsInterface
                ? ExtendedListType().MakeGenericType(typeof(FormLink<>).MakeGenericType(target))
                : property.PropertyType;
            list = Activator.CreateInstance(listType)!;
            property.SetValue(record, list);
        }

        list.GetType().GetMethod("Clear")?.Invoke(list, null);
        var add = list.GetType().GetMethod("Add") ?? throw new RecordEditingException($"Field '{property.Name}' cannot accept links.");
        var linkType = typeof(FormLink<>).MakeGenericType(target);
        foreach (var value in values)
        {
            add.Invoke(list, [Activator.CreateInstance(linkType, value)]);
        }
    }

    private static Type ExtendedListType()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetType("Noggog.ExtendedList`1"))
            .First(type => type is not null)!;
    }

    private static Delegate Getter(Type getterType, Type valueType, PropertyInfo property)
    {
        var getter = Parameter(getterType, "getter");
        var body = Expression.Convert(Expression.Property(Expression.Convert(getter, property.DeclaringType!), property), valueType);
        return Lambda(typeof(Func<,>).MakeGenericType(getterType, valueType), body, getter);
    }

    private static Delegate Setter(Type recordType, Type valueType, PropertyInfo property)
    {
        var record = Parameter(recordType, "record");
        var value = Parameter(valueType, "value");
        var body = Expression.Assign(Expression.Property(Expression.Convert(record, property.DeclaringType!), property), Expression.Convert(value, property.PropertyType));
        return Lambda(typeof(Action<,>).MakeGenericType(recordType, valueType), body, record, value);
    }

    private static Delegate EnumSetter(Type recordType, Type enumType, PropertyInfo property)
    {
        var nullable = typeof(Nullable<>).MakeGenericType(enumType);
        var record = Parameter(recordType, "record");
        var value = Parameter(nullable, "value");
        var method = typeof(MutagenMemberBinder).GetMethod(nameof(WriteEnumValue))!.MakeGenericMethod(enumType);
        return Lambda(typeof(Action<,>).MakeGenericType(recordType, nullable), Expression.Call(method, Expression.Convert(record, typeof(object)), Expression.Constant(property), value), record, value);
    }

    /// <summary>Assigns a nullable enum onto the Mutagen property.</summary>
    public static void WriteEnumValue<TEnum>(object record, PropertyInfo property, TEnum? value)
        where TEnum : struct, Enum
    {
        property.SetValue(record, value is null ? null : value.Value);
    }

    private static Delegate Call(Type delegateType, string methodName, Type instanceType, PropertyInfo property)
    {
        var instance = Parameter(instanceType, "instance");
        return Lambda(delegateType, Expression.Call(typeof(MutagenMemberBinder).GetMethod(methodName)!, Expression.Convert(instance, typeof(object)), Expression.Constant(property)), instance);
    }

    private static Delegate Lambda(Type delegateType, Expression body, params ParameterExpression[] parameters)
    {
        return Expression.Lambda(delegateType, body, parameters).Compile();
    }

    private static ParameterExpression Parameter(Type type, string name)
    {
        return Expression.Parameter(type, name);
    }

    private static RecordField InvokeFactory(string name, Type[] typeArguments, object?[] arguments)
    {
        var method = typeof(RecordFields).GetMethods().Single(candidate => candidate.Name == name && candidate.IsGenericMethodDefinition && candidate.GetGenericArguments().Length == typeArguments.Length);
        return (RecordField)method.MakeGenericMethod(typeArguments).Invoke(null, arguments)!;
    }

    private static object? TryConstruct(Type recordType, object release)
    {
        try
        {
            return Activator.CreateInstance(recordType, new FormKey(ModKey.FromNameAndExtension("Probe.esp"), 1), release);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static bool CanRead(PropertyInfo property, object sample)
    {
        try
        {
            property.GetValue(sample);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
