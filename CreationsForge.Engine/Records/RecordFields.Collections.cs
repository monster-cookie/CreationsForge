using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Engine.Records;

/// <content>Provides bounded collection-field registration and mutation helpers.</content>
public static partial class RecordFields
{
    /// <summary>Creates an ordered FormKey list that preserves duplicates and supports bounded collection operations.</summary>
    /// <typeparam name="TRecord">The exact mutable record type.</typeparam>
    /// <typeparam name="TGetter">The exact Mutagen getter interface.</typeparam>
    /// <param name="path">The stable field path.</param>
    /// <param name="referenceTargets">The permitted Mutagen getter interfaces.</param>
    /// <param name="read">The typed Mutagen read operation.</param>
    /// <param name="replace">The typed Mutagen whole-list replacement used after an operation is applied.</param>
    /// <returns>The bounded field registration.</returns>
    public static RecordField<TRecord, TGetter> FormLinkList<TRecord, TGetter>(
        string path,
        IEnumerable<Type> referenceTargets,
        Func<TGetter, IReadOnlyList<FormKey>> read,
        Action<TRecord, IReadOnlyList<FormKey>> replace)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        var targetTypes = referenceTargets.ToArray();
        var targets = targetTypes.Select(type => type.FullName ?? type.Name).ToArray();
        var descriptor = new RecordFieldDescriptor(path, RecordValueKind.List, false, false, null, targets, ListOperations);
        return new RecordField<TRecord, TGetter>(
            descriptor,
            getter => RecordValue.FromList(read(getter).Select(RecordValue.FromFormLink)),
            change => ValidateFormLinkListChange(change, path),
            (record, change) => replace(record, ApplyFormLinkListChange(read((TGetter)(object)record), change, path)),
            change => ChangedFormLinks(change).Select(formKey => new RecordFormLinkReference(formKey, targetTypes)).ToArray());
    }

    private static void ValidateFormLinkListChange(RecordFieldChange change, string path)
    {
        switch (change.Operation)
        {
            case RecordCollectionOperation.Set:
                if (change.Index is not null || change.Value is not RecordValue.ListRecordValue list || list.Values.Any(value => value is not RecordValue.FormLinkRecordValue))
                {
                    throw new RecordEditingException($"Field '{path}' Set requires an ordered list of non-null FormLink values and no index.");
                }

                break;
            case RecordCollectionOperation.Append:
                if (change.Index is not null || change.Value is not RecordValue.FormLinkRecordValue)
                {
                    throw new RecordEditingException($"Field '{path}' Append requires one non-null FormLink value and no index.");
                }

                break;
            case RecordCollectionOperation.Insert:
                if (change.Index is null || change.Value is not RecordValue.FormLinkRecordValue)
                {
                    throw new RecordEditingException($"Field '{path}' Insert requires one non-null FormLink value and an index.");
                }

                break;
            case RecordCollectionOperation.RemoveAt:
                if (change.Index is null || change.Value is not null)
                {
                    throw new RecordEditingException($"Field '{path}' RemoveAt requires an index and no value.");
                }

                break;
            case RecordCollectionOperation.Clear:
                if (change.Index is not null || change.Value is not null)
                {
                    throw new RecordEditingException($"Field '{path}' Clear accepts neither a value nor an index.");
                }

                break;
            default:
                throw new RecordEditingException($"Operation '{change.Operation}' is not legal for field '{path}'.");
        }
    }

    private static IReadOnlyList<FormKey> ApplyFormLinkListChange(IReadOnlyList<FormKey> current, RecordFieldChange change, string path)
    {
        var values = current.ToList();
        switch (change.Operation)
        {
            case RecordCollectionOperation.Set:
                return ((RecordValue.ListRecordValue)change.Value!).Values.Cast<RecordValue.FormLinkRecordValue>().Select(value => value.FormKey).ToArray();
            case RecordCollectionOperation.Append:
                values.Add(((RecordValue.FormLinkRecordValue)change.Value!).FormKey);
                break;
            case RecordCollectionOperation.Insert:
                if (change.Index < 0 || change.Index > values.Count)
                {
                    throw new RecordEditingException($"Field '{path}' insert index '{change.Index}' is outside 0..{values.Count}.");
                }

                values.Insert(change.Index.GetValueOrDefault(), ((RecordValue.FormLinkRecordValue)change.Value!).FormKey);
                break;
            case RecordCollectionOperation.RemoveAt:
                if (change.Index < 0 || change.Index >= values.Count)
                {
                    throw new RecordEditingException($"Field '{path}' removal index '{change.Index}' is outside 0..{Math.Max(0, values.Count - 1)}.");
                }

                values.RemoveAt(change.Index.GetValueOrDefault());
                break;
            case RecordCollectionOperation.Clear:
                values.Clear();
                break;
        }

        return values;
    }

    private static IReadOnlyList<FormKey> ChangedFormLinks(RecordFieldChange change)
    {
        return change.Value switch
        {
            RecordValue.FormLinkRecordValue link => [link.FormKey],
            RecordValue.ListRecordValue list => list.Values.Cast<RecordValue.FormLinkRecordValue>().Select(value => value.FormKey).ToArray(),
            _ => [],
        };
    }

    private static IReadOnlyList<RecordFormLinkReference> GetObjectReferences(
        RecordFieldChange change,
        IReadOnlyDictionary<string, Type>? referenceAlternatives)
    {
        if (referenceAlternatives is null
            || change.Value is not RecordValue.ObjectRecordValue objectValue
            || !referenceAlternatives.TryGetValue(objectValue.Alternative, out var targetType)
            || !objectValue.Fields.TryGetValue("FormKey", out var field)
            || field is not RecordValue.FormLinkRecordValue link)
        {
            return [];
        }

        return [new RecordFormLinkReference(link.FormKey, [targetType])];
    }
}
