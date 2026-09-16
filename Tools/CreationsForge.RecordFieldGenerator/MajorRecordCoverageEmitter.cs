using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.RecordFieldGenerator;

/// <summary>Emits deterministic per-game manifests for every installed concrete major-record family and indexed field.</summary>
internal sealed class MajorRecordCoverageEmitter
{
    /// <summary>The exact mutually compatible Mutagen package version represented by emitted manifests.</summary>
    private const string PackageVersion = "0.55.0-alpha.53";

    /// <summary>Generates all three read-only major-record coverage manifests.</summary>
    /// <param name="repositoryRoot">The CreationsForge repository root.</param>
    internal void Generate(string repositoryRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(repositoryRoot);
        var games = new[]
        {
            new GameCoverageConfiguration(
                "Starfield",
                "Mutagen.Bethesda.Starfield",
                typeof(Mutagen.Bethesda.Starfield.StarfieldMajorRecord),
                Path.Combine(repositoryRoot, "CreationsForge.Starfield", "PluginAdapter", "RecordInspection", "MajorRecords", "StarfieldMajorRecordCoverage.json")),
            new GameCoverageConfiguration(
                "Fallout4",
                "Mutagen.Bethesda.Fallout4",
                typeof(Mutagen.Bethesda.Fallout4.Fallout4MajorRecord),
                Path.Combine(repositoryRoot, "CreationsForge.Fallout4", "PluginAdapter", "RecordInspection", "MajorRecords", "Fallout4MajorRecordCoverage.json")),
            new GameCoverageConfiguration(
                "Skyrim",
                "Mutagen.Bethesda.Skyrim",
                typeof(Mutagen.Bethesda.Skyrim.SkyrimMajorRecord),
                Path.Combine(repositoryRoot, "CreationsForge.Skyrim", "PluginAdapter", "RecordInspection", "MajorRecords", "SkyrimMajorRecordCoverage.json")),
        };

        foreach (var game in games)
        {
            var manifest = CreateManifest(game);
            if (manifest.UnsupportedShapes.Count != 0)
            {
                throw new InvalidOperationException(
                    $"{game.Name} major-record coverage contains {manifest.UnsupportedShapes.Count} unclassified native shapes: "
                    + string.Join("; ", manifest.UnsupportedShapes.Take(10)));
            }

            var json = JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
            Directory.CreateDirectory(Path.GetDirectoryName(game.OutputPath)!);
            var normalizedJson = json
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Replace('\r', '\n')
                .Replace("\n", "\r\n", StringComparison.Ordinal);
            File.WriteAllText(game.OutputPath, normalizedJson + "\r\n", new System.Text.UTF8Encoding(false));
            Console.WriteLine($"Generated {game.Name}: {manifest.ConcreteFamilyCount} concrete families, {manifest.IndexedTypeCount} indexed contracts, {manifest.IndexedFieldCount} indexed fields, {manifest.AdditionalContractCount} additional contracts, {manifest.UnsupportedShapes.Count} unsupported shapes.");
        }
    }

    /// <summary>Builds one deterministic coverage manifest from installed package metadata.</summary>
    /// <param name="game">The package, root type, and output mapping.</param>
    /// <returns>The complete manifest model.</returns>
    private static CoverageManifest CreateManifest(GameCoverageConfiguration game)
    {
        var assembly = game.MajorRecordType.Assembly;
        var assemblyTypes = assembly.GetTypes();
        var concreteTypes = assemblyTypes
            .Where(static type => type.IsClass && !type.IsAbstract && !type.IsGenericTypeDefinition)
            .OrderBy(static type => type.FullName, StringComparer.Ordinal)
            .ToArray();
        var families = concreteTypes
            .Where(game.MajorRecordType.IsAssignableFrom)
            .Select(type => new FamilyCoverage(
                type.FullName ?? type.Name,
                FindGetter(type)?.FullName,
                GetFieldIndex(type) is not null))
            .ToArray();

        var indexedContracts = new List<TypeCoverage>();
        var additionalContracts = new SortedDictionary<string, TypeCoverage>(StringComparer.Ordinal);
        var unsupported = new SortedSet<string>(StringComparer.Ordinal);
        var queuedAdditionalTypes = new Queue<Type>();
        var indexedTypes = concreteTypes
            .Select(type => (Type: type, FieldIndex: GetFieldIndex(type)))
            .Where(static item => item.FieldIndex is not null)
            .OrderBy(static item => item.Type.FullName, StringComparer.Ordinal)
            .ToArray();
        foreach (var indexed in indexedTypes)
        {
            var fields = new List<FieldCoverage>();
            foreach (var name in Enum.GetNames(indexed.FieldIndex!)
                         .OrderBy(name => Convert.ToUInt64(Enum.Parse(indexed.FieldIndex!, name), CultureInfo.InvariantCulture)))
            {
                var property = indexed.Type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
                if (property is null || !property.CanRead || property.GetIndexParameters().Length != 0)
                {
                    unsupported.Add($"{indexed.Type.FullName}.{name}: indexed field has no readable property");
                    fields.Add(new FieldCoverage(name, null, "unsupported"));
                    continue;
                }

                var shape = ClassifyShape(property.PropertyType);
                fields.Add(new FieldCoverage(name, FormatType(property.PropertyType), shape));
                QueueNestedContracts(property.PropertyType, queuedAdditionalTypes);
            }

            indexedContracts.Add(new TypeCoverage(
                indexed.Type.FullName ?? indexed.Type.Name,
                FindGetter(indexed.Type)?.FullName,
                "field_index",
                fields));
        }

        var visitedAdditional = new HashSet<string>(StringComparer.Ordinal);
        while (queuedAdditionalTypes.TryDequeue(out var queued))
        {
            var type = Nullable.GetUnderlyingType(queued) ?? queued;
            var typeName = FormatType(type);
            if (!visitedAdditional.Add(typeName) || GetFieldIndex(type) is not null || IsLeaf(type) || IsContainer(type))
            {
                continue;
            }

            var properties = GetFallbackProperties(type);
            var fields = new List<FieldCoverage>();
            foreach (var property in properties)
            {
                fields.Add(new FieldCoverage(property.Name, FormatType(property.PropertyType), ClassifyShape(property.PropertyType)));
                QueueNestedContracts(property.PropertyType, queuedAdditionalTypes);
            }

            var publicFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                .OrderBy(static field => field.Name, StringComparer.Ordinal)
                .ToArray();
            foreach (var field in publicFields)
            {
                fields.Add(new FieldCoverage(field.Name, FormatType(field.FieldType), ClassifyShape(field.FieldType)));
                QueueNestedContracts(field.FieldType, queuedAdditionalTypes);
            }

            if (fields.Count == 0)
            {
                var assignable = concreteTypes.Where(type.IsAssignableFrom).ToArray();
                if (assignable.Length == 0)
                {
                    unsupported.Add($"{typeName}: no indexed, scalar, container, readable-property, public-field, or concrete-union contract");
                }
                else
                {
                    foreach (var concrete in assignable)
                    {
                        queuedAdditionalTypes.Enqueue(concrete);
                    }
                }
            }

            additionalContracts[typeName] = new TypeCoverage(
                typeName,
                type.IsInterface ? type.FullName : FindGetter(type)?.FullName,
                type.IsInterface || type.IsAbstract ? "polymorphic_contract" : "public_members",
                fields);
        }

        var indexedFieldCount = indexedContracts.Sum(static contract => contract.Fields.Count);
        return new CoverageManifest(
            schemaVersion: 1,
            game.Name,
            game.PackageId,
            PackageVersion,
            game.MajorRecordType.FullName ?? game.MajorRecordType.Name,
            families.Length,
            indexedContracts.Count,
            indexedFieldCount,
            additionalContracts.Count,
            families,
            indexedContracts,
            additionalContracts.Values.ToArray(),
            unsupported.ToArray());
    }

    /// <summary>Queues recursive value contracts that are not closed native leaves.</summary>
    /// <param name="type">The declared native field type.</param>
    /// <param name="queue">The pending contract queue.</param>
    private static void QueueNestedContracts(Type type, Queue<Type> queue)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        if (type.IsArray)
        {
            queue.Enqueue(type.GetElementType()!);
            return;
        }

        if (type.IsGenericType && IsContainer(type))
        {
            foreach (var argument in type.GetGenericArguments())
            {
                queue.Enqueue(argument);
            }
            return;
        }

        if (!IsLeaf(type))
        {
            queue.Enqueue(type);
        }
    }

    /// <summary>Classifies one declared native field shape for coverage review.</summary>
    /// <param name="type">The declared native type.</param>
    /// <returns>A stable manifest shape identifier.</returns>
    private static string ClassifyShape(Type type)
    {
        if (Nullable.GetUnderlyingType(type) is not null)
        {
            return "nullable";
        }
        if (IsLeaf(type))
        {
            return "leaf";
        }
        if (type.IsArray)
        {
            return "ordered_array";
        }
        if (IsDictionary(type))
        {
            return "dictionary";
        }
        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            return "ordered_collection";
        }
        if (GetFieldIndex(type) is not null)
        {
            return "indexed_object";
        }
        if (type.IsInterface || type.IsAbstract)
        {
            return "polymorphic_object";
        }
        return "public_object";
    }

    /// <summary>Identifies native scalar and wrapper values handled atomically by production inspection.</summary>
    /// <param name="type">The non-null declared type.</param>
    /// <returns><see langword="true"/> for a complete leaf policy.</returns>
    private static bool IsLeaf(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal)
            || type == typeof(Guid) || type == typeof(DateTime) || type == typeof(DateTimeOffset)
            || type == typeof(DateOnly) || type == typeof(TimeOnly) || type == typeof(FormKey)
            || type == typeof(ModKey) || type.FullName == "Noggog.Percent"
            || typeof(IFormLinkGetter).IsAssignableFrom(type)
            || typeof(IAssetLinkGetter).IsAssignableFrom(type)
            || typeof(ITranslatedStringGetter).IsAssignableFrom(type);
    }

    /// <summary>Identifies ordered, array, or dictionary value containers.</summary>
    /// <param name="type">The declared native type.</param>
    /// <returns><see langword="true"/> when recursive element contracts apply.</returns>
    private static bool IsContainer(Type type)
    {
        return type.IsArray || (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type));
    }

    /// <summary>Identifies dictionary contracts distinct from ordered collections.</summary>
    /// <param name="type">The declared native type.</param>
    /// <returns><see langword="true"/> when key/value semantics apply.</returns>
    private static bool IsDictionary(Type type)
    {
        return typeof(IDictionary).IsAssignableFrom(type)
            || type.GetInterfaces().Append(type).Any(static candidate =>
                candidate.IsGenericType && candidate.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>));
    }

    /// <summary>Returns deterministic readable fallback properties while excluding self-returning computed projections.</summary>
    /// <param name="type">The native contract type.</param>
    /// <returns>Public readable properties in ordinal name order.</returns>
    private static IReadOnlyList<PropertyInfo> GetFallbackProperties(Type type)
    {
        return Array.AsReadOnly(type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.CanRead && property.GetMethod?.IsPublic == true
                && property.GetIndexParameters().Length == 0 && property.PropertyType != type)
            .GroupBy(static property => property.Name, StringComparer.Ordinal)
            .Select(static group => group.First())
            .OrderBy(static property => property.Name, StringComparer.Ordinal)
            .ToArray());
    }

    /// <summary>Finds the generated getter interface for one mutable package model.</summary>
    /// <param name="type">The mutable native type.</param>
    /// <returns>The getter interface, or <see langword="null"/> when the type has no conventional getter.</returns>
    private static Type? FindGetter(Type type)
    {
        var expectedName = "I" + type.Name + "Getter";
        return type.GetInterfaces().FirstOrDefault(candidate =>
            string.Equals(candidate.Namespace, type.Namespace, StringComparison.Ordinal)
            && string.Equals(candidate.Name, expectedName, StringComparison.Ordinal));
    }

    /// <summary>Finds the installed generated FieldIndex for one mutable model.</summary>
    /// <param name="type">The mutable native type.</param>
    /// <returns>The FieldIndex enum, or <see langword="null"/>.</returns>
    private static Type? GetFieldIndex(Type type)
    {
        return type.FullName is null ? null : type.Assembly.GetType(type.FullName + "_FieldIndex");
    }

    /// <summary>Formats one closed native type without assembly-version noise.</summary>
    /// <param name="type">The native type.</param>
    /// <returns>A deterministic closed type identity.</returns>
    private static string FormatType(Type type)
    {
        if (type.IsArray)
        {
            return FormatType(type.GetElementType()!) + "[]";
        }
        if (!type.IsGenericType)
        {
            return type.FullName ?? type.Name;
        }
        var definition = type.GetGenericTypeDefinition().FullName!.Split('`')[0].Replace('+', '.');
        return definition + "<" + string.Join(",", type.GetGenericArguments().Select(FormatType)) + ">";
    }

    /// <summary>Maps one game package to its major-record root and manifest path.</summary>
    private sealed class GameCoverageConfiguration
    {
        /// <summary>Initializes one game coverage mapping.</summary>
        /// <param name="name">The stable CreationsForge game name.</param>
        /// <param name="packageId">The Mutagen package identifier.</param>
        /// <param name="majorRecordType">The abstract mutable major-record base.</param>
        /// <param name="outputPath">The exact manifest output path.</param>
        internal GameCoverageConfiguration(string name, string packageId, Type majorRecordType, string outputPath)
        {
            Name = name;
            PackageId = packageId;
            MajorRecordType = majorRecordType;
            OutputPath = outputPath;
        }

        /// <summary>Gets the stable game name.</summary>
        internal string Name { get; }

        /// <summary>Gets the Mutagen package identifier.</summary>
        internal string PackageId { get; }

        /// <summary>Gets the abstract mutable major-record base.</summary>
        internal Type MajorRecordType { get; }

        /// <summary>Gets the exact manifest output path.</summary>
        internal string OutputPath { get; }
    }

    /// <summary>Defines one serialized per-game coverage manifest.</summary>
    private sealed class CoverageManifest
    {
        /// <summary>Initializes a complete coverage manifest.</summary>
        internal CoverageManifest(int schemaVersion, string game, string packageId, string packageVersion, string majorRecordBase, int concreteFamilyCount, int indexedTypeCount, int indexedFieldCount, int additionalContractCount, IReadOnlyList<FamilyCoverage> concreteFamilies, IReadOnlyList<TypeCoverage> indexedContracts, IReadOnlyList<TypeCoverage> additionalContracts, IReadOnlyList<string> unsupportedShapes)
        {
            SchemaVersion = schemaVersion;
            Game = game;
            PackageId = packageId;
            PackageVersion = packageVersion;
            MajorRecordBase = majorRecordBase;
            ConcreteFamilyCount = concreteFamilyCount;
            IndexedTypeCount = indexedTypeCount;
            IndexedFieldCount = indexedFieldCount;
            AdditionalContractCount = additionalContractCount;
            ConcreteFamilies = concreteFamilies;
            IndexedContracts = indexedContracts;
            AdditionalContracts = additionalContracts;
            UnsupportedShapes = unsupportedShapes;
        }

        /// <summary>Gets the manifest schema version.</summary>
        public int SchemaVersion { get; }
        /// <summary>Gets the game name.</summary>
        public string Game { get; }
        /// <summary>Gets the exact package identifier.</summary>
        public string PackageId { get; }
        /// <summary>Gets the exact package version.</summary>
        public string PackageVersion { get; }
        /// <summary>Gets the installed abstract mutable major-record base.</summary>
        public string MajorRecordBase { get; }
        /// <summary>Gets the number of concrete major-record families.</summary>
        public int ConcreteFamilyCount { get; }
        /// <summary>Gets the number of indexed native type contracts.</summary>
        public int IndexedTypeCount { get; }
        /// <summary>Gets the number of indexed fields.</summary>
        public int IndexedFieldCount { get; }
        /// <summary>Gets the number of additional public wrapper contracts.</summary>
        public int AdditionalContractCount { get; }
        /// <summary>Gets every concrete major-record family.</summary>
        public IReadOnlyList<FamilyCoverage> ConcreteFamilies { get; }
        /// <summary>Gets every installed indexed native type and field.</summary>
        public IReadOnlyList<TypeCoverage> IndexedContracts { get; }
        /// <summary>Gets non-indexed wrapper and polymorphic contracts reachable from indexed fields.</summary>
        public IReadOnlyList<TypeCoverage> AdditionalContracts { get; }
        /// <summary>Gets explicitly accounted shapes that production inspection cannot traverse.</summary>
        public IReadOnlyList<string> UnsupportedShapes { get; }
    }

    /// <summary>Defines one concrete major-record family manifest entry.</summary>
    private sealed class FamilyCoverage
    {
        /// <summary>Initializes one family entry.</summary>
        internal FamilyCoverage(string mutableType, string? getterType, bool hasFieldIndex)
        {
            MutableType = mutableType;
            GetterType = getterType;
            HasFieldIndex = hasFieldIndex;
        }

        /// <summary>Gets the concrete mutable family type.</summary>
        public string MutableType { get; }
        /// <summary>Gets the generated getter family type, when conventional.</summary>
        public string? GetterType { get; }
        /// <summary>Gets whether the family supplies deterministic FieldIndex metadata.</summary>
        public bool HasFieldIndex { get; }
    }

    /// <summary>Defines one indexed or additional native type contract.</summary>
    private sealed class TypeCoverage
    {
        /// <summary>Initializes one type contract entry.</summary>
        internal TypeCoverage(string mutableType, string? getterType, string ordering, IReadOnlyList<FieldCoverage> fields)
        {
            MutableType = mutableType;
            GetterType = getterType;
            Ordering = ordering;
            Fields = fields;
        }

        /// <summary>Gets the mutable or public contract type.</summary>
        public string MutableType { get; }
        /// <summary>Gets the corresponding getter contract, when known.</summary>
        public string? GetterType { get; }
        /// <summary>Gets the deterministic field-order policy.</summary>
        public string Ordering { get; }
        /// <summary>Gets every readable field in deterministic order.</summary>
        public IReadOnlyList<FieldCoverage> Fields { get; }
    }

    /// <summary>Defines one accounted native field.</summary>
    private sealed class FieldCoverage
    {
        /// <summary>Initializes one field entry.</summary>
        internal FieldCoverage(string name, string? declaredType, string shape)
        {
            Name = name;
            DeclaredType = declaredType;
            Shape = shape;
        }

        /// <summary>Gets the exact native field name.</summary>
        public string Name { get; }
        /// <summary>Gets the closed declared type, or <see langword="null"/> for a missing indexed property.</summary>
        public string? DeclaredType { get; }
        /// <summary>Gets the production inspection shape policy.</summary>
        public string Shape { get; }
    }
}
