using System.Collections.Concurrent;
using System.Reflection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Engine.Records;

/// <summary>Builds record families from the Mutagen game assembly currently referenced by the process.</summary>
public static class MutagenRecordCatalog
{
    /// <summary>Discovers every public concrete major record that can be constructed and stored.</summary>
    /// <param name="modType">The game mod interface, such as <c>IStarfieldMod</c>.</param>
    /// <param name="release">The Mutagen release value passed to record constructors.</param>
    /// <param name="excludedTypeNames">Record types Mutagen still cannot round-trip.</param>
    /// <returns>The discovered families, ordered by type name.</returns>
    public static IReadOnlyList<RecordFamily> Discover(Type modType, object release, IEnumerable<string> excludedTypeNames)
    {
        ArgumentNullException.ThrowIfNull(modType);
        ArgumentNullException.ThrowIfNull(release);
        var excluded = excludedTypeNames.ToHashSet(StringComparer.Ordinal);
        var groups = GroupProperties(modType);
        var families = new List<RecordFamily>();
        foreach (var recordType in MajorRecordTypes(modType.Assembly, modType.Namespace!, release.GetType(), excluded))
        {
            var getterType = modType.Assembly.GetType($"{recordType.Namespace}.I{recordType.Name}Getter");
            if (getterType is null)
            {
                continue;
            }

            var fields = MutagenMemberBinder.Bind(recordType, getterType, release);
            Func<IMod, IMajorRecord> create = mod =>
            {
                var record = (IMajorRecord)Activator.CreateInstance(recordType, mod.GetNextFormKey(), release)!;
                MutagenMemberBinder.PrepareRequiredMembers(record);
                return record;
            };
            var copy = Copy;
            var stored = StoredType(recordType, groups);
            if (groups.TryGetValue(stored, out var group))
            {
                var capturedGroup = group;
                families.Add(new DiscoveredRecordFamily(
                    recordType.Name,
                    getterType,
                    fields,
                    create,
                    copy,
                    (mod, key) => FindInGroup(capturedGroup.GetValue(mod)!, key, recordType),
                    (mod, record) => SetInGroup(capturedGroup.GetValue(mod)!, record),
                    (mod, key) => RemoveFromGroup(capturedGroup.GetValue(mod)!, key, recordType),
                    PlaceGroupedOverride(recordType, capturedGroup)));
                continue;
            }

            if (!MutagenRecordPlacement.TryBind(modType, recordType, release, out var find, out var publish, out var remove, out var placeOverride))
            {
                continue;
            }

            families.Add(new DiscoveredRecordFamily(recordType.Name, getterType, fields, create, copy, find, publish, remove, placeOverride));
        }

        return families.OrderBy(family => family.Descriptor.FamilyId, StringComparer.Ordinal).ToArray();
    }

    private static readonly ConcurrentDictionary<Assembly, Dictionary<Type, MethodInfo>> DeepCopies = new();

    /// <summary>Copies a container record with its child groups left behind.</summary>
    /// <param name="getter">The source record.</param>
    /// <param name="recordName">The mutable record type name.</param>
    /// <returns>The copy.</returns>
    internal static IMajorRecord CopyWithoutChildren(IMajorRecordGetter getter, string recordName)
    {
        var assembly = getter.GetType().Assembly;
        var mutableType = assembly.GetType($"{getter.GetType().Namespace}.{recordName}");
        var maskType = mutableType?.GetNestedType("TranslationMask");
        var getterInterface = assembly.GetType($"{getter.GetType().Namespace}.I{recordName}Getter");
        if (maskType is null || getterInterface is null)
        {
            return Copy(getter);
        }

        var constructor = maskType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .OrderBy(candidate => candidate.GetParameters().Length)
            .FirstOrDefault(candidate => candidate.GetParameters().All(parameter => parameter.ParameterType == typeof(bool) || parameter.HasDefaultValue));
        if (constructor is null)
        {
            var signatures = string.Join("; ", maskType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Select(candidate => candidate.ToString()));
            throw new RecordEditingException($"Mutagen copy mask '{maskType.FullName}' constructors: {signatures}.");
        }

        var maskArguments = constructor.GetParameters().Select(parameter => parameter.ParameterType == typeof(bool) ? (object)true : parameter.DefaultValue).ToArray();
        var mask = constructor.Invoke(maskArguments);
        foreach (var fieldName in ChildMaskFields(recordName))
        {
            SetMaskOff(mask, fieldName);
        }

        var indexed = DeepCopies.GetOrAdd(assembly, IndexDeepCopies);
        if (!indexed.TryGetValue(getterInterface, out var method))
        {
            return Copy(getter);
        }

        var arguments = new object?[method.GetParameters().Length];
        arguments[0] = getter;
        arguments[1] = mask;
        return (IMajorRecord)method.Invoke(null, arguments)!;
    }

    internal static IMajorRecord CopyRecord(IMajorRecordGetter getter)
    {
        return Copy(getter);
    }

    private static Func<IMod, IModContext, IMajorRecord>? PlaceGroupedOverride(Type recordType, PropertyInfo group)
    {
        if (recordType.Name is "Worldspace" or "Quest" or "DialogTopic")
        {
            return (mod, context) => PlaceMaskedGroupRecord(mod, context, recordType, group);
        }

        return null;
    }

    private static IMajorRecord PlaceMaskedGroupRecord(IMod mod, IModContext context, Type recordType, PropertyInfo group)
    {
        var source = context.Record as IMajorRecordGetter
            ?? throw new RecordEditingException($"Mutagen context for '{recordType.Name}' did not contain a major record.");
        var groupObject = group.GetValue(mod)!;
        if (FindAny(groupObject, source.FormKey) is IMajorRecordGetter occupant && occupant.GetType() != recordType)
        {
            throw new RecordEditingException($"Output already contains '{occupant.GetType().Name}' at '{source.FormKey}', which is not '{recordType.Name}'.");
        }

        if (FindInGroup(groupObject, source.FormKey, recordType) is IMajorRecord existing)
        {
            return existing;
        }

        var shell = CopyWithoutChildren(source, recordType.Name);
        SetInGroup(groupObject, shell);
        return shell;
    }

    private static void SetMaskOff(object mask, string fieldName)
    {
        var field = mask.GetType().GetField(fieldName);
        if (field is null)
        {
            return;
        }

        if (field.FieldType == typeof(bool))
        {
            field.SetValue(mask, false);
            return;
        }

        var convert = field.FieldType.GetMethod("op_Implicit", BindingFlags.Public | BindingFlags.Static, [typeof(bool)]);
        if (convert is not null)
        {
            field.SetValue(mask, convert.Invoke(null, [false]));
        }
    }

    private static string[] ChildMaskFields(string recordName)
    {
        return recordName switch
        {
            "Cell" => ["Persistent", "Temporary", "NavigationMeshes", "Landscape"],
            "Worldspace" => ["SubCells", "TopCell"],
            "Quest" => ["DialogTopics", "DialogBranches", "Scenes"],
            "DialogTopic" => ["Responses"],
            _ => [],
        };
    }

    private static IMajorRecord Copy(IMajorRecordGetter getter)
    {
        var instance = getter.GetType().GetMethod("DeepCopy", Type.EmptyTypes);
        if (instance is not null)
        {
            return (IMajorRecord)instance.Invoke(getter, null)!;
        }

        var indexed = DeepCopies.GetOrAdd(getter.GetType().Assembly, IndexDeepCopies);
        var match = getter.GetType().GetInterfaces()
            .Where(type => indexed.ContainsKey(type))
            .OrderByDescending(type => type.Name.Length)
            .FirstOrDefault()
            ?? throw new RecordEditingException($"Mutagen record '{getter.GetType().Name}' does not expose DeepCopy.");
        var method = indexed[match];
        var arguments = new object?[method.GetParameters().Length];
        arguments[0] = getter;
        return (IMajorRecord)method.Invoke(null, arguments)!;
    }

    private static Dictionary<Type, MethodInfo> IndexDeepCopies(Assembly assembly)
    {
        var map = new Dictionary<Type, MethodInfo>();
        foreach (var type in assembly.GetTypes())
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (method.Name != "DeepCopy")
                {
                    continue;
                }

                var parameters = method.GetParameters();
                if (parameters.Length == 0 || parameters.Any(parameter => parameter.ParameterType.IsByRef))
                {
                    continue;
                }

                var getter = parameters[0].ParameterType;
                if (!map.TryGetValue(getter, out var existing) || parameters.Length < existing.GetParameters().Length)
                {
                    map[getter] = method;
                }
            }
        }

        return map;
    }

    private static IEnumerable<Type> MajorRecordTypes(Assembly assembly, string gameNamespace, Type releaseType, HashSet<string> excluded)
    {
        return assembly.GetTypes()
            .Where(type => type.IsPublic && type.IsClass && !type.IsAbstract && type.Namespace == gameNamespace)
            .Where(type => !excluded.Contains(type.Name))
            .Where(type => type.GetConstructor([typeof(FormKey), releaseType]) is not null)
            .OrderBy(type => type.Name, StringComparer.Ordinal);
    }

    private static Dictionary<Type, PropertyInfo> GroupProperties(Type modType)
    {
        var groups = new Dictionary<Type, PropertyInfo>();
        foreach (var property in modType.GetProperties())
        {
            var group = property.PropertyType.GetInterfaces()
                .Append(property.PropertyType)
                .FirstOrDefault(type => type.IsGenericType && type.GetGenericTypeDefinition().Name == "IGroup`1");
            var stored = group?.GenericTypeArguments[0];
            if (stored is not null && !groups.ContainsKey(stored))
            {
                groups.Add(stored, property);
            }
        }

        return groups;
    }

    private static Type StoredType(Type recordType, Dictionary<Type, PropertyInfo> groups)
    {
        for (var type = recordType; type is not null; type = type.BaseType)
        {
            if (groups.ContainsKey(type))
            {
                return type;
            }
        }

        return recordType;
    }

    private static IMajorRecordGetter? FindInGroup(object group, FormKey formKey, Type recordType)
    {
        var records = group.GetType().GetProperty("Records")?.GetValue(group) ?? group;
        if (records is not System.Collections.IEnumerable list)
        {
            return null;
        }

        foreach (var item in list)
        {
            if (item is IMajorRecordGetter record && record.FormKey == formKey && record.GetType() == recordType)
            {
                return record;
            }
        }

        return null;
    }

    private static void SetInGroup(object group, IMajorRecord record)
    {
        if (FindAny(group, record.FormKey) is IMajorRecordGetter occupant && occupant.GetType() != record.GetType())
        {
            throw new RecordEditingException($"Output already contains '{occupant.GetType().Name}' at '{record.FormKey}', which is not '{record.GetType().Name}'.");
        }

        var set = group.GetType().GetMethods().First(method => method.Name == "Set" && method.GetParameters().Length == 1);
        set.Invoke(group, [record]);
    }

    private static bool RemoveFromGroup(object group, FormKey formKey, Type recordType)
    {
        if (FindAny(group, formKey) is not IMajorRecordGetter occupant)
        {
            return false;
        }

        if (occupant.GetType() != recordType)
        {
            throw new RecordEditingException($"Output contains '{occupant.GetType().Name}' at '{formKey}', which is not '{recordType.Name}'.");
        }

        var remove = group.GetType().GetMethods().First(method => method.Name == "Remove" && method.GetParameters() is [{ ParameterType: var parameter }] && parameter == typeof(FormKey));
        return remove.Invoke(group, [formKey]) is not false;
    }

    private static IMajorRecordGetter? FindAny(object group, FormKey formKey)
    {
        var records = group.GetType().GetProperty("Records")?.GetValue(group) ?? group;
        if (records is not System.Collections.IEnumerable list)
        {
            return null;
        }

        foreach (var item in list)
        {
            if (item is IMajorRecordGetter record && record.FormKey == formKey)
            {
                return record;
            }
        }

        return null;
    }
}
