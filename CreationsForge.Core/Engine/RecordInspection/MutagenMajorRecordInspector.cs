using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace CreationsForge.Core.Engine.RecordInspection;

/// <summary>Inspects detached Mutagen records through installed field metadata and typed values.</summary>
public sealed class MutagenMajorRecordInspector : IMajorRecordInspector
{
    /// <summary>The maximum supported nested native-value depth.</summary>
    private const int MaximumDepth = 128;

    /// <summary>Cached deterministic readable-property contracts shared by inspector instances.</summary>
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<PropertyInfo>> PropertyContracts = new();

    /// <summary>The supported game's abstract mutable major-record base type.</summary>
    private readonly Type MajorRecordType;

    /// <summary>The package identity embedded into each root read view.</summary>
    private readonly string PackageIdentity;

    /// <summary>Initializes an inspector for one exact installed Mutagen game package.</summary>
    /// <param name="majorRecordType">The game's abstract mutable major-record base type.</param>
    /// <param name="packageIdentity">The stable package name and selected version.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="majorRecordType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the package identity is empty or the supplied type is not an abstract major-record base.</exception>
    public MutagenMajorRecordInspector(Type majorRecordType, string packageIdentity)
    {
        ArgumentNullException.ThrowIfNull(majorRecordType);
        ArgumentException.ThrowIfNullOrWhiteSpace(packageIdentity);
        if (!majorRecordType.IsAbstract || !typeof(IMajorRecordGetter).IsAssignableFrom(majorRecordType))
        {
            throw new ArgumentException("The Mutagen inspector requires an abstract mutable major-record base type.", nameof(majorRecordType));
        }

        MajorRecordType = majorRecordType;
        PackageIdentity = packageIdentity;
        SupportedRecordTypes = Array.AsReadOnly(majorRecordType.Assembly.GetTypes()
            .Where(type => type.IsClass
                && !type.IsAbstract
                && !type.IsGenericTypeDefinition
                && majorRecordType.IsAssignableFrom(type))
            .Select(type => type.Name)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(name => name, StringComparer.Ordinal)
            .ToArray());
        if (SupportedRecordTypes.Count == 0)
        {
            throw new ArgumentException("The Mutagen inspector could not discover any concrete major-record families.", nameof(majorRecordType));
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<string> SupportedRecordTypes { get; }

    /// <inheritdoc />
    public void WriteReadView(
        IMajorRecordGetter record,
        Utf8JsonWriter writer,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(record);
        ArgumentNullException.ThrowIfNull(writer);
        RequireSupportedRecord(record, nameof(record));
        var activeObjects = new HashSet<object>(ReferenceEqualityComparer.Instance);
        WriteObject(writer, record, "$record", 0, activeObjects, cancellationToken, isRoot: true);
    }

    /// <inheritdoc />
    public IReadOnlyList<SemanticChangeDescriptor> Compare(
        IMajorRecordGetter? before,
        IMajorRecordGetter? after,
        CancellationToken cancellationToken)
    {
        if (before is not null)
        {
            RequireSupportedRecord(before, nameof(before));
        }

        if (after is not null)
        {
            RequireSupportedRecord(after, nameof(after));
        }

        var changes = new List<SemanticChangeDescriptor>();
        if (!RecordSemanticComparer.CompareRecordPresence(before, after, changes, cancellationToken))
        {
            return Array.AsReadOnly(changes.ToArray());
        }

        CompareValue(before, after, "$record", 0, changes, cancellationToken, collectionPath: null, collectionIndex: null);
        return Array.AsReadOnly(changes.ToArray());
    }

    /// <summary>Rejects records from another game package or non-mutable detached implementations.</summary>
    /// <param name="record">The record to validate.</param>
    /// <param name="parameterName">The public parameter name used in the failure.</param>
    /// <exception cref="ArgumentException">Thrown when the record is outside this inspector's package.</exception>
    private void RequireSupportedRecord(IMajorRecordGetter record, string parameterName)
    {
        if (!MajorRecordType.IsInstanceOfType(record))
        {
            throw new ArgumentException(
                $"The record type {record.GetType().FullName} does not belong to {PackageIdentity}.",
                parameterName);
        }
    }

    /// <summary>Writes one non-null complex native value and every reachable readable field.</summary>
    /// <param name="writer">The destination JSON writer.</param>
    /// <param name="value">The non-null native value.</param>
    /// <param name="path">The deterministic field path used in diagnostics.</param>
    /// <param name="depth">The current recursive depth.</param>
    /// <param name="activeObjects">Reference objects active on the current traversal path.</param>
    /// <param name="cancellationToken">A token observed before each field read.</param>
    /// <param name="isRoot">Whether the value is the inspected major-record root.</param>
    private void WriteObject(
        Utf8JsonWriter writer,
        object value,
        string path,
        int depth,
        HashSet<object> activeObjects,
        CancellationToken cancellationToken,
        bool isRoot)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureDepth(depth, path);
        var type = value.GetType();
        var tracked = !type.IsValueType;
        if (tracked && !activeObjects.Add(value))
        {
            throw new NotSupportedException($"The native value graph contains a reference cycle at {path} ({type.FullName}).");
        }

        try
        {
            writer.WriteStartObject();
            writer.WriteString("$type", type.FullName ?? type.Name);
            if (isRoot)
            {
                writer.WriteString("$package", PackageIdentity);
            }

            var properties = GetProperties(type);
            if (properties.Count == 0)
            {
                throw new NotSupportedException($"The native value at {path} ({type.FullName}) exposes no readable fields.");
            }

            foreach (var property in properties)
            {
                cancellationToken.ThrowIfCancellationRequested();
                writer.WritePropertyName(property.Name);
                WriteValue(
                    writer,
                    ReadProperty(property, value, path),
                    property.PropertyType,
                    path + "." + property.Name,
                    depth + 1,
                    activeObjects,
                    cancellationToken);
            }

            writer.WriteEndObject();
        }
        finally
        {
            if (tracked)
            {
                activeObjects.Remove(value);
            }
        }
    }

    /// <summary>Writes one native field value without serializing through an intermediate record model.</summary>
    /// <param name="writer">The destination JSON writer.</param>
    /// <param name="value">The native value, or <see langword="null"/>.</param>
    /// <param name="declaredType">The installed native property type.</param>
    /// <param name="path">The deterministic field path used in diagnostics.</param>
    /// <param name="depth">The current recursive depth.</param>
    /// <param name="activeObjects">Reference objects active on the current traversal path.</param>
    /// <param name="cancellationToken">A token observed throughout collection traversal.</param>
    private void WriteValue(
        Utf8JsonWriter writer,
        object? value,
        Type declaredType,
        string path,
        int depth,
        HashSet<object> activeObjects,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureDepth(depth, path);
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        var type = Nullable.GetUnderlyingType(declaredType) ?? value.GetType();
        if (WriteScalar(writer, value, type, cancellationToken))
        {
            return;
        }

        if (TryGetDictionaryEntries(value, cancellationToken, out var entries))
        {
            writer.WriteStartArray();
            foreach (var entry in entries)
            {
                cancellationToken.ThrowIfCancellationRequested();
                writer.WriteStartObject();
                writer.WritePropertyName("key");
                WriteValue(writer, entry.Key, entry.Key?.GetType() ?? typeof(object), path + "[].key", depth + 1, activeObjects, cancellationToken);
                writer.WritePropertyName("value");
                WriteValue(writer, entry.Value, entry.Value?.GetType() ?? typeof(object), path + "[].value", depth + 1, activeObjects, cancellationToken);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            return;
        }

        if (value is IEnumerable enumerable)
        {
            writer.WriteStartArray();
            var index = 0;
            foreach (var item in enumerable)
            {
                cancellationToken.ThrowIfCancellationRequested();
                WriteValue(writer, item, item?.GetType() ?? GetEnumerableElementType(declaredType) ?? typeof(object), $"{path}[{index}]", depth + 1, activeObjects, cancellationToken);
                index = checked(index + 1);
            }

            writer.WriteEndArray();
            return;
        }

        WriteObject(writer, value, path, depth, activeObjects, cancellationToken, isRoot: false);
    }

    /// <summary>Writes one recognized exact scalar or Mutagen leaf value.</summary>
    /// <param name="writer">The destination JSON writer.</param>
    /// <param name="value">The non-null native value.</param>
    /// <param name="type">The effective non-null value type.</param>
    /// <param name="cancellationToken">A token observed while writing translated strings or bytes.</param>
    /// <returns><see langword="true"/> when the value was completely written as a leaf.</returns>
    private static bool WriteScalar(Utf8JsonWriter writer, object value, Type type, CancellationToken cancellationToken)
    {
        switch (value)
        {
            case string text:
                RecordJsonLeafWriter.WriteString(writer, text, null);
                return true;
            case byte[] bytes:
                RecordJsonLeafWriter.WriteBytes(writer, bytes, cancellationToken);
                return true;
            case Memory<byte> bytes:
                RecordJsonLeafWriter.WriteBytes(writer, bytes.ToArray(), cancellationToken);
                return true;
            case ReadOnlyMemory<byte> bytes:
                RecordJsonLeafWriter.WriteBytes(writer, bytes.ToArray(), cancellationToken);
                return true;
            case MemorySlice<byte> bytes:
                RecordJsonLeafWriter.WriteBytes(writer, bytes.ToArray(), cancellationToken);
                return true;
            case ReadOnlyMemorySlice<byte> bytes:
                RecordJsonLeafWriter.WriteBytes(writer, bytes.ToArray(), cancellationToken);
                return true;
            case bool boolean:
                writer.WriteBooleanValue(boolean);
                return true;
            case byte number:
                writer.WriteNumberValue(number);
                return true;
            case sbyte number:
                writer.WriteNumberValue(number);
                return true;
            case short number:
                writer.WriteNumberValue(number);
                return true;
            case ushort number:
                writer.WriteNumberValue(number);
                return true;
            case int number:
                writer.WriteNumberValue(number);
                return true;
            case uint number:
                writer.WriteNumberValue(number);
                return true;
            case long number:
                writer.WriteNumberValue(number);
                return true;
            case ulong number:
                writer.WriteNumberValue(number);
                return true;
            case decimal number:
                writer.WriteNumberValue(number);
                return true;
            case float number:
                RecordJsonLeafWriter.WriteSingle(writer, number);
                return true;
            case double number:
                RecordJsonLeafWriter.WriteDouble(writer, number);
                return true;
            case char character:
                writer.WriteStringValue(character.ToString());
                return true;
            case Guid guid:
                writer.WriteStringValue(guid.ToString("D"));
                return true;
            case DateTime dateTime:
                writer.WriteStringValue(dateTime.ToString("O", CultureInfo.InvariantCulture));
                return true;
            case DateTimeOffset dateTimeOffset:
                writer.WriteStringValue(dateTimeOffset.ToString("O", CultureInfo.InvariantCulture));
                return true;
            case DateOnly date:
                writer.WriteStringValue(date.ToString("O", CultureInfo.InvariantCulture));
                return true;
            case TimeOnly time:
                writer.WriteStringValue(time.ToString("O", CultureInfo.InvariantCulture));
                return true;
            case FormKey formKey:
                RecordJsonLeafWriter.WriteFormKey(writer, formKey);
                return true;
            case ModKey modKey:
                writer.WriteStringValue(modKey.ToString());
                return true;
            case IFormLinkGetter formLink:
                RecordJsonLeafWriter.WriteFormLink(writer, formLink);
                return true;
            case IAssetLinkGetter assetLink:
                RecordJsonLeafWriter.WriteAssetLink(writer, assetLink);
                return true;
            case ITranslatedStringGetter translatedString:
                RecordJsonLeafWriter.WriteTranslatedString(writer, translatedString, cancellationToken);
                return true;
        }

        if (!type.IsEnum)
        {
            if (type.FullName == "Noggog.Percent")
            {
                RecordJsonLeafWriter.WriteDouble(writer, ReadPercentValue(value));
                return true;
            }

            return false;
        }

        var underlying = Enum.GetUnderlyingType(type);
        if (underlying == typeof(ulong))
        {
            writer.WriteNumberValue(Convert.ToUInt64(value, CultureInfo.InvariantCulture));
        }
        else
        {
            writer.WriteNumberValue(Convert.ToInt64(value, CultureInfo.InvariantCulture));
        }

        return true;
    }

    /// <summary>Compares one pair of native values and appends deterministic semantic change descriptors.</summary>
    /// <param name="before">The prior value, or <see langword="null"/>.</param>
    /// <param name="after">The resulting value, or <see langword="null"/>.</param>
    /// <param name="path">The deterministic typed field path.</param>
    /// <param name="depth">The current recursive depth.</param>
    /// <param name="changes">The caller-owned ordered changes.</param>
    /// <param name="cancellationToken">A token observed throughout traversal.</param>
    /// <param name="collectionPath">The containing collection path for a direct item change, or <see langword="null"/>.</param>
    /// <param name="collectionIndex">The containing collection position for a direct item change, or <see langword="null"/>.</param>
    private void CompareValue(
        object? before,
        object? after,
        string path,
        int depth,
        ICollection<SemanticChangeDescriptor> changes,
        CancellationToken cancellationToken,
        string? collectionPath,
        int? collectionIndex)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureDepth(depth, path);
        if (before is null || after is null)
        {
            if (before is null != (after is null))
            {
                AddValueChange(path, changes, collectionPath, collectionIndex);
            }

            return;
        }

        var beforeType = before.GetType();
        var afterType = after.GetType();
        if (beforeType != afterType)
        {
            AddValueChange(path + ".$type", changes, collectionPath, collectionIndex);
            return;
        }

        if (TryLeafEquals(before, after, beforeType, cancellationToken, out var equal))
        {
            if (!equal)
            {
                AddValueChange(path, changes, collectionPath, collectionIndex);
            }

            return;
        }

        if (TryGetDictionaryEntries(before, cancellationToken, out var beforeEntries)
            && TryGetDictionaryEntries(after, cancellationToken, out var afterEntries))
        {
            CompareDictionaries(beforeEntries, afterEntries, path, depth, changes, cancellationToken);
            return;
        }

        if (before is IEnumerable beforeEnumerable && after is IEnumerable afterEnumerable)
        {
            CompareEnumerables(beforeEnumerable, afterEnumerable, path, depth, changes, cancellationToken);
            return;
        }

        var properties = GetProperties(beforeType);
        if (properties.Count == 0)
        {
            throw new NotSupportedException($"The native value at {path} ({beforeType.FullName}) exposes no comparable fields.");
        }

        foreach (var property in properties)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CompareValue(
                ReadProperty(property, before, path),
                ReadProperty(property, after, path),
                path + "." + property.Name,
                depth + 1,
                changes,
                cancellationToken,
                collectionPath: null,
                collectionIndex: null);
        }
    }

    /// <summary>Compares ordered enumerable values without collapsing duplicate entries.</summary>
    /// <param name="before">The prior enumerable.</param>
    /// <param name="after">The resulting enumerable.</param>
    /// <param name="path">The containing field path.</param>
    /// <param name="depth">The current recursive depth.</param>
    /// <param name="changes">The caller-owned ordered changes.</param>
    /// <param name="cancellationToken">A token observed while materializing and comparing positions.</param>
    private void CompareEnumerables(
        IEnumerable before,
        IEnumerable after,
        string path,
        int depth,
        ICollection<SemanticChangeDescriptor> changes,
        CancellationToken cancellationToken)
    {
        var beforeItems = Materialize(before, cancellationToken);
        var afterItems = Materialize(after, cancellationToken);
        var commonCount = Math.Min(beforeItems.Count, afterItems.Count);
        for (var index = 0; index < commonCount; index++)
        {
            CompareValue(beforeItems[index], afterItems[index], $"{path}[{index}]", depth + 1, changes, cancellationToken, path, index);
        }

        for (var index = commonCount; index < beforeItems.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            changes.Add(new SemanticChangeDescriptor(path, SemanticChangeKind.ItemRemoved, index, null));
        }

        for (var index = commonCount; index < afterItems.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            changes.Add(new SemanticChangeDescriptor(path, SemanticChangeKind.ItemInserted, null, index));
        }
    }

    /// <summary>Compares deterministic dictionary snapshots by native key identity and recursively typed values.</summary>
    /// <param name="before">The prior ordered key/value snapshot.</param>
    /// <param name="after">The resulting ordered key/value snapshot.</param>
    /// <param name="path">The containing field path.</param>
    /// <param name="depth">The current recursive depth.</param>
    /// <param name="changes">The caller-owned ordered changes.</param>
    /// <param name="cancellationToken">A token observed for each key.</param>
    private void CompareDictionaries(
        IReadOnlyList<DictionaryEntrySnapshot> before,
        IReadOnlyList<DictionaryEntrySnapshot> after,
        string path,
        int depth,
        ICollection<SemanticChangeDescriptor> changes,
        CancellationToken cancellationToken)
    {
        var beforeByKey = before.ToDictionary(static entry => entry.StableKey, StringComparer.Ordinal);
        var afterByKey = after.ToDictionary(static entry => entry.StableKey, StringComparer.Ordinal);
        var keys = beforeByKey.Keys.Concat(afterByKey.Keys).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();
        foreach (var key in keys)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var beforeFound = beforeByKey.TryGetValue(key, out var beforeEntry);
            var afterFound = afterByKey.TryGetValue(key, out var afterEntry);
            if (!beforeFound)
            {
                changes.Add(new SemanticChangeDescriptor(path, SemanticChangeKind.ItemInserted, null, afterEntry!.Position));
                continue;
            }

            if (!afterFound)
            {
                changes.Add(new SemanticChangeDescriptor(path, SemanticChangeKind.ItemRemoved, beforeEntry!.Position, null));
                continue;
            }

            CompareValue(
                beforeEntry!.Value,
                afterEntry!.Value,
                path + "[" + JsonSerializer.Serialize(key) + "]",
                depth + 1,
                changes,
                cancellationToken,
                collectionPath: null,
                collectionIndex: null);
        }
    }

    /// <summary>Returns exact equality for a recognized native leaf type.</summary>
    /// <param name="before">The prior non-null value.</param>
    /// <param name="after">The resulting non-null value of the same runtime type.</param>
    /// <param name="type">The common runtime type.</param>
    /// <param name="cancellationToken">A token observed while comparing translated strings.</param>
    /// <param name="equal">Receives the exact typed equality result when the type is a leaf.</param>
    /// <returns><see langword="true"/> when the type has a complete leaf equality policy.</returns>
    private static bool TryLeafEquals(
        object before,
        object after,
        Type type,
        CancellationToken cancellationToken,
        out bool equal)
    {
        switch (before)
        {
            case float beforeSingle when after is float afterSingle:
                equal = RecordSemanticComparer.BitwiseEquals(beforeSingle, afterSingle);
                return true;
            case double beforeDouble when after is double afterDouble:
                equal = RecordSemanticComparer.BitwiseEquals(beforeDouble, afterDouble);
                return true;
            case string beforeText when after is string afterText:
                equal = RecordSemanticComparer.StringEquals(beforeText, afterText);
                return true;
            case byte[] beforeBytes when after is byte[] afterBytes:
                equal = BytesEqual(beforeBytes, afterBytes, cancellationToken);
                return true;
            case Memory<byte> beforeBytes when after is Memory<byte> afterBytes:
                equal = BytesEqual(beforeBytes.Span, afterBytes.Span, cancellationToken);
                return true;
            case ReadOnlyMemory<byte> beforeBytes when after is ReadOnlyMemory<byte> afterBytes:
                equal = BytesEqual(beforeBytes.Span, afterBytes.Span, cancellationToken);
                return true;
            case MemorySlice<byte> beforeBytes when after is MemorySlice<byte> afterBytes:
                equal = BytesEqual(beforeBytes.Span, afterBytes.Span, cancellationToken);
                return true;
            case ReadOnlyMemorySlice<byte> beforeBytes when after is ReadOnlyMemorySlice<byte> afterBytes:
                equal = BytesEqual(beforeBytes.Span, afterBytes.Span, cancellationToken);
                return true;
            case FormKey beforeKey when after is FormKey afterKey:
                equal = RecordSemanticComparer.FormKeyEquals(beforeKey, afterKey);
                return true;
            case IFormLinkGetter beforeLink when after is IFormLinkGetter afterLink:
                equal = RecordSemanticComparer.FormLinkEquals(beforeLink, afterLink);
                return true;
            case IAssetLinkGetter beforeAsset when after is IAssetLinkGetter afterAsset:
                equal = RecordSemanticComparer.AssetLinkEquals(beforeAsset, afterAsset);
                return true;
            case ITranslatedStringGetter beforeText when after is ITranslatedStringGetter afterText:
                equal = RecordSemanticComparer.TranslatedStringEquals(beforeText, afterText, cancellationToken);
                return true;
        }

        if (type.IsPrimitive || type.IsEnum || type == typeof(decimal) || type == typeof(Guid)
            || type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(DateOnly)
            || type == typeof(TimeOnly) || type == typeof(ModKey))
        {
            equal = before.Equals(after);
            return true;
        }

        if (type.FullName == "Noggog.Percent")
        {
            equal = RecordSemanticComparer.BitwiseEquals(ReadPercentValue(before), ReadPercentValue(after));
            return true;
        }

        equal = false;
        return false;
    }

    /// <summary>Compares a native byte buffer as one value while observing cancellation throughout the scan.</summary>
    /// <param name="before">The prior byte buffer.</param>
    /// <param name="after">The resulting byte buffer.</param>
    /// <param name="cancellationToken">A token observed at each byte position.</param>
    /// <returns><see langword="true"/> when both buffers contain the same bytes in order.</returns>
    private static bool BytesEqual(ReadOnlySpan<byte> before, ReadOnlySpan<byte> after, CancellationToken cancellationToken)
    {
        if (before.Length != after.Length)
        {
            return false;
        }

        for (var index = 0; index < before.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (before[index] != after[index])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Reads the exact double payload of one installed Noggog percentage value.</summary>
    /// <param name="value">The boxed percentage value.</param>
    /// <returns>The exact percentage payload.</returns>
    /// <exception cref="NotSupportedException">Thrown when the installed percentage contract no longer exposes its native payload.</exception>
    private static double ReadPercentValue(object value)
    {
        var field = value.GetType().GetField("Value", BindingFlags.Instance | BindingFlags.Public);
        if (field?.GetValue(value) is not double result)
        {
            throw new NotSupportedException("The installed Noggog.Percent contract no longer exposes its exact double Value field.");
        }

        return result;
    }

    /// <summary>Adds a scalar or direct ordered-item change using the applicable semantic shape.</summary>
    /// <param name="path">The exact changed value path.</param>
    /// <param name="changes">The caller-owned change collection.</param>
    /// <param name="collectionPath">The direct containing collection path, or <see langword="null"/>.</param>
    /// <param name="collectionIndex">The direct collection position, or <see langword="null"/>.</param>
    private static void AddValueChange(
        string path,
        ICollection<SemanticChangeDescriptor> changes,
        string? collectionPath,
        int? collectionIndex)
    {
        changes.Add(collectionPath is not null && collectionIndex.HasValue
            ? new SemanticChangeDescriptor(collectionPath, SemanticChangeKind.ItemChanged, collectionIndex, collectionIndex)
            : new SemanticChangeDescriptor(path, SemanticChangeKind.ValueChanged));
    }

    /// <summary>Reads one native property and unwraps reflection failures with its exact field path.</summary>
    /// <param name="property">The readable native property.</param>
    /// <param name="instance">The owning native object.</param>
    /// <param name="ownerPath">The owning deterministic path.</param>
    /// <returns>The property's current native value.</returns>
    private static object? ReadProperty(PropertyInfo property, object instance, string ownerPath)
    {
        try
        {
            return property.GetValue(instance);
        }
        catch (TargetInvocationException exception)
        {
            throw new NotSupportedException(
                $"The native getter {ownerPath}.{property.Name} could not be read.",
                exception.InnerException ?? exception);
        }
    }

    /// <summary>Returns deterministic native fields using FieldIndex order when installed metadata supplies it.</summary>
    /// <param name="type">The concrete native runtime type.</param>
    /// <returns>Readable non-indexer properties in complete deterministic order.</returns>
    private static IReadOnlyList<PropertyInfo> GetProperties(Type type)
    {
        return PropertyContracts.GetOrAdd(type, static candidate =>
        {
            var fieldIndex = candidate.Assembly.GetType(candidate.FullName + "_FieldIndex");
            if (fieldIndex is not null && fieldIndex.IsEnum)
            {
                return Array.AsReadOnly(Enum.GetNames(fieldIndex)
                    .OrderBy(name => Convert.ToUInt64(Enum.Parse(fieldIndex, name), CultureInfo.InvariantCulture))
                    .Select(name => candidate.GetProperty(name, BindingFlags.Instance | BindingFlags.Public)
                        ?? throw new NotSupportedException($"Installed FieldIndex member {candidate.FullName}.{name} has no readable property."))
                    .Where(static property => property.CanRead && property.GetIndexParameters().Length == 0)
                    .ToArray());
            }

            return Array.AsReadOnly(candidate
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(property => property.CanRead
                    && property.GetMethod?.IsPublic == true
                    && property.GetIndexParameters().Length == 0
                    && property.PropertyType != candidate)
                .GroupBy(static property => property.Name, StringComparer.Ordinal)
                .Select(static group => group.OrderByDescending(property => GetInheritanceDepth(property.DeclaringType)).First())
                .OrderBy(static property => property.Name, StringComparer.Ordinal)
                .ToArray());
        });
    }

    /// <summary>Returns a type's base-class depth for deterministic duplicate property selection.</summary>
    /// <param name="type">The declaring type, or <see langword="null"/>.</param>
    /// <returns>The number of base-class steps before <see cref="object"/>.</returns>
    private static int GetInheritanceDepth(Type? type)
    {
        var depth = 0;
        while (type?.BaseType is not null)
        {
            depth++;
            type = type.BaseType;
        }

        return depth;
    }

    /// <summary>Attempts to create a deterministic snapshot from dictionary or key/value enumerable state.</summary>
    /// <param name="value">The native value to classify.</param>
    /// <param name="cancellationToken">A token observed while enumerating entries.</param>
    /// <param name="entries">Receives entries ordered by stable native-key representation.</param>
    /// <returns><see langword="true"/> when the native value represents a dictionary.</returns>
    private static bool TryGetDictionaryEntries(
        object value,
        CancellationToken cancellationToken,
        out IReadOnlyList<DictionaryEntrySnapshot> entries)
    {
        var snapshots = new List<DictionaryEntrySnapshot>();
        if (value is IDictionary dictionary)
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                cancellationToken.ThrowIfCancellationRequested();
                snapshots.Add(new DictionaryEntrySnapshot(GetStableKey(entry.Key), entry.Key, entry.Value));
            }
        }
        else
        {
            var dictionaryInterface = value.GetType().GetInterfaces().FirstOrDefault(static candidate =>
                candidate.IsGenericType && candidate.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>));
            if (dictionaryInterface is null || value is not IEnumerable enumerable)
            {
                entries = Array.Empty<DictionaryEntrySnapshot>();
                return false;
            }

            foreach (var entry in enumerable)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (entry is null)
                {
                    throw new NotSupportedException("A native dictionary exposed a null key/value entry.");
                }

                var entryType = entry.GetType();
                var key = entryType.GetProperty("Key")?.GetValue(entry);
                var entryValue = entryType.GetProperty("Value")?.GetValue(entry);
                snapshots.Add(new DictionaryEntrySnapshot(GetStableKey(key), key, entryValue));
            }
        }

        snapshots.Sort(static (left, right) => string.CompareOrdinal(left.StableKey, right.StableKey));
        for (var index = 0; index < snapshots.Count; index++)
        {
            snapshots[index].Position = index;
        }
        if (snapshots.Select(static entry => entry.StableKey).Distinct(StringComparer.Ordinal).Count() != snapshots.Count)
        {
            throw new NotSupportedException("Native dictionary keys do not have unique deterministic representations.");
        }

        entries = Array.AsReadOnly(snapshots.ToArray());
        return true;
    }

    /// <summary>Builds a deterministic dictionary-key identity without using it as record comparison authority.</summary>
    /// <param name="key">The native dictionary key, or <see langword="null"/>.</param>
    /// <returns>A type-qualified invariant key used only for stable entry pairing and paths.</returns>
    private static string GetStableKey(object? key)
    {
        if (key is null)
        {
            return "null";
        }

        var formatted = key switch
        {
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => key.ToString(),
        };
        return (key.GetType().FullName ?? key.GetType().Name) + ":" + formatted;
    }

    /// <summary>Materializes one ordered enumerable for bounded in-operation native comparison.</summary>
    /// <param name="values">The native enumerable.</param>
    /// <param name="cancellationToken">A token observed for every item.</param>
    /// <returns>The temporary ordered item snapshot.</returns>
    private static IReadOnlyList<object?> Materialize(IEnumerable values, CancellationToken cancellationToken)
    {
        var result = new List<object?>();
        foreach (var value in values)
        {
            cancellationToken.ThrowIfCancellationRequested();
            result.Add(value);
        }

        return result;
    }

    /// <summary>Finds the declared element type of an array or one-argument enumerable contract.</summary>
    /// <param name="type">The declared native collection type.</param>
    /// <returns>The declared element type, or <see langword="null"/> when unavailable.</returns>
    private static Type? GetEnumerableElementType(Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        var enumerable = type.GetInterfaces().Append(type).FirstOrDefault(static candidate =>
            candidate.IsGenericType && candidate.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        return enumerable?.GetGenericArguments()[0];
    }

    /// <summary>Rejects unexpectedly deep native graphs before stack exhaustion.</summary>
    /// <param name="depth">The current nested depth.</param>
    /// <param name="path">The current deterministic field path.</param>
    private static void EnsureDepth(int depth, string path)
    {
        if (depth > MaximumDepth)
        {
            throw new NotSupportedException($"The native value graph exceeded depth {MaximumDepth} at {path}.");
        }
    }

    /// <summary>Stores one temporary native dictionary entry for deterministic inspection and comparison.</summary>
    /// <param name="stableKey">The type-qualified invariant key identity.</param>
    /// <param name="key">The native key value.</param>
    /// <param name="value">The native entry value.</param>
    private sealed class DictionaryEntrySnapshot
    {
        /// <summary>Initializes one temporary native dictionary entry.</summary>
        /// <param name="stableKey">The type-qualified invariant key identity.</param>
        /// <param name="key">The native key value.</param>
        /// <param name="value">The native entry value.</param>
        internal DictionaryEntrySnapshot(string stableKey, object? key, object? value)
        {
            StableKey = stableKey;
            Key = key;
            Value = value;
        }

        /// <summary>Gets the type-qualified invariant key identity.</summary>
        internal string StableKey { get; }

        /// <summary>Gets the native key value.</summary>
        internal object? Key { get; }

        /// <summary>Gets the native entry value.</summary>
        internal object? Value { get; }

        /// <summary>Gets or sets the position in the stable-key-sorted field-tree array.</summary>
        internal int Position { get; set; }
    }
}
