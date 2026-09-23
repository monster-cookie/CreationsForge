using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Engine.Records;

/// <summary>Owns bounded typed record operations for one declared concrete major-record family.</summary>
public abstract class RecordFamily
{
    /// <summary>Initializes a record family registration.</summary>
    /// <param name="descriptor">The public family descriptor.</param>
    /// <param name="fields">The registered field implementations.</param>
    protected RecordFamily(RecordFamilyDescriptor descriptor, IReadOnlyList<RecordField> fields)
    {
        Descriptor = descriptor;
        Fields = fields;
    }

    /// <summary>Gets the published family descriptor.</summary>
    public RecordFamilyDescriptor Descriptor { get; }

    /// <summary>Gets the exact Mutagen getter type used for context resolution.</summary>
    public Type GetterType => Descriptor.GetterType;

    internal IReadOnlyList<RecordField> Fields { get; }

    internal abstract IMajorRecord CreateCandidate(IMod output);

    internal abstract IMajorRecord CopyCandidate(IMajorRecordGetter source);

    internal abstract IMajorRecordGetter? FindOutputRecord(IMod output, FormKey formKey);

    internal abstract void Publish(IMod output, IMajorRecord record);

    internal abstract bool Remove(IMod output, FormKey formKey);

    internal void ValidateChanges(IEnumerable<RecordFieldChange> changes)
    {
        foreach (var change in changes)
        {
            GetField(change.Path).Validate(change);
        }
    }

    internal void ApplyChanges(IMajorRecord record, IEnumerable<RecordFieldChange> changes)
    {
        foreach (var change in changes)
        {
            GetField(change.Path).Apply(record, change);
        }
    }

    internal IReadOnlyList<RecordFormLinkReference> GetReferences(IEnumerable<RecordFieldChange> changes)
    {
        return changes.SelectMany(change => GetField(change.Path).GetReferences(change)).ToArray();
    }

    internal IReadOnlyDictionary<string, RecordValue> ReadFields(IMajorRecordGetter record)
    {
        return Fields.ToDictionary(field => field.Descriptor.Path, field => field.Read(record), StringComparer.Ordinal);
    }

    private RecordField GetField(string path)
    {
        return Fields.FirstOrDefault(field => string.Equals(field.Descriptor.Path, path, StringComparison.Ordinal))
            ?? throw new RecordEditingException($"Field path '{path}' is not editable for declared family '{Descriptor.FamilyId}'.");
    }
}

/// <summary>Implements one record family through exact typed constructors, deep copies, and top-level Mutagen groups.</summary>
/// <typeparam name="TMod">The exact mutable game mod interface.</typeparam>
/// <typeparam name="TRecord">The exact mutable record type.</typeparam>
/// <typeparam name="TGetter">The exact Mutagen getter interface.</typeparam>
public sealed class RecordFamily<TMod, TRecord, TGetter> : RecordFamily
    where TMod : class, IMod
    where TRecord : class, IMajorRecord
    where TGetter : class, IMajorRecordGetter
{
    private readonly Func<TMod, FormKey, TRecord> _create;
    private readonly Func<TGetter, TRecord> _copy;
    private readonly Func<TMod, IGroup<TRecord>> _group;

    /// <summary>Initializes a top-level record family registration.</summary>
    /// <param name="familyId">The stable family identifier.</param>
    /// <param name="create">The exact typed detached-record constructor.</param>
    /// <param name="copy">The exact typed Mutagen deep-copy operation.</param>
    /// <param name="group">The exact typed mutable output group accessor.</param>
    /// <param name="fields">Family-specific registered field implementations.</param>
    public RecordFamily(
        string familyId,
        Func<TMod, FormKey, TRecord> create,
        Func<TGetter, TRecord> copy,
        Func<TMod, IGroup<TRecord>> group,
        IEnumerable<RecordField<TRecord, TGetter>> fields)
        : base(CreateDescriptor(familyId, fields, out var registeredFields), registeredFields)
    {
        _create = create;
        _copy = copy;
        _group = group;
    }

    internal override IMajorRecord CreateCandidate(IMod output)
    {
        var mod = RequireMod(output);
        return _create(mod, mod.GetNextFormKey());
    }

    internal override IMajorRecord CopyCandidate(IMajorRecordGetter source)
    {
        if (source is not TGetter getter)
        {
            throw new RecordEditingException($"Record '{source.FormKey}' is not declared family '{Descriptor.FamilyId}'.");
        }

        return _copy(getter);
    }

    internal override IMajorRecordGetter? FindOutputRecord(IMod output, FormKey formKey)
    {
        return _group(RequireMod(output)).Records.FirstOrDefault(record => record.FormKey == formKey);
    }

    internal override void Publish(IMod output, IMajorRecord record)
    {
        if (record is not TRecord typedRecord)
        {
            throw new RecordEditingException($"Record '{record.FormKey}' is not mutable family '{Descriptor.FamilyId}'.");
        }

        _group(RequireMod(output)).Set(typedRecord);
    }

    internal override bool Remove(IMod output, FormKey formKey)
    {
        return _group(RequireMod(output)).Remove(formKey);
    }

    private static RecordFamilyDescriptor CreateDescriptor(
        string familyId,
        IEnumerable<RecordField<TRecord, TGetter>> fields,
        out IReadOnlyList<RecordField> registeredFields)
    {
        ArgumentNullException.ThrowIfNull(fields);
        var allFields = new List<RecordField<TRecord, TGetter>>
        {
            RecordFields.String<TRecord, TGetter>(
                "EditorID",
                isNullable: true,
                getter => getter.EditorID,
                (record, value) => record.EditorID = value),
        };
        allFields.AddRange(fields);
        registeredFields = allFields;
        return new RecordFamilyDescriptor(familyId, typeof(TGetter), allFields.Select(field => field.Descriptor));
    }

    private static TMod RequireMod(IMod output)
    {
        return output as TMod
            ?? throw new RecordEditingException($"Output '{output.ModKey}' is not expected Mutagen mod type '{typeof(TMod).FullName}'.");
    }
}
