using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Engine.Records;

/// <summary>Adapts one Mutagen major record discovered from the referenced game assembly.</summary>
internal sealed class DiscoveredRecordFamily : RecordFamily
{
    private readonly Func<IMod, IMajorRecord> _create;
    private readonly Func<IMajorRecordGetter, IMajorRecord> _copy;
    private readonly Func<IMod, FormKey, IMajorRecordGetter?> _find;
    private readonly Action<IMod, IMajorRecord> _publish;
    private readonly Func<IMod, FormKey, bool> _remove;
    private readonly Func<IMod, IModContext, IMajorRecord>? _placeOverride;

    /// <summary>Initializes a discovered family.</summary>
    /// <param name="familyId">The Mutagen record type name.</param>
    /// <param name="getterType">The Mutagen getter interface.</param>
    /// <param name="fields">The fields bound from the current Mutagen members.</param>
    /// <param name="create">Constructs a detached record for the next form key.</param>
    /// <param name="copy">Deep-copies a Mutagen getter into a mutable record.</param>
    /// <param name="find">Locates the output record.</param>
    /// <param name="publish">Inserts or replaces the output record.</param>
    /// <param name="remove">Removes the output record.</param>
    /// <param name="placeOverride">Places an override through its Mutagen context, when the record is nested or must omit child groups.</param>
    public DiscoveredRecordFamily(
        string familyId,
        Type getterType,
        IReadOnlyList<RecordField> fields,
        Func<IMod, IMajorRecord> create,
        Func<IMajorRecordGetter, IMajorRecord> copy,
        Func<IMod, FormKey, IMajorRecordGetter?> find,
        Action<IMod, IMajorRecord> publish,
        Func<IMod, FormKey, bool> remove,
        Func<IMod, IModContext, IMajorRecord>? placeOverride = null)
        : base(new RecordFamilyDescriptor(familyId, getterType, fields.Select(field => field.Descriptor)), fields)
    {
        _create = create;
        _copy = copy;
        _find = find;
        _publish = publish;
        _remove = remove;
        _placeOverride = placeOverride;
    }

    internal override IMajorRecord CreateCandidate(IMod output)
    {
        return _create(output);
    }

    internal override IMajorRecord CopyCandidate(IMajorRecordGetter source)
    {
        if (!Descriptor.GetterType.IsInstanceOfType(source))
        {
            throw new RecordEditingException($"Record '{source.FormKey}' is not declared family '{Descriptor.FamilyId}'.");
        }

        return _copy(source);
    }

    internal override IMajorRecordGetter? FindOutputRecord(IMod output, FormKey formKey)
    {
        return _find(output, formKey);
    }

    internal override void Publish(IMod output, IMajorRecord record)
    {
        if (record.GetType().Name != Descriptor.FamilyId)
        {
            throw new RecordEditingException($"Record '{record.FormKey}' is not mutable family '{Descriptor.FamilyId}'.");
        }

        _publish(output, record);
    }

    internal override bool Remove(IMod output, FormKey formKey)
    {
        return _remove(output, formKey);
    }

    internal override bool TryPlaceOverride(IMod output, IModContext context, out IMajorRecord record)
    {
        if (_placeOverride is null)
        {
            record = null!;
            return false;
        }

        record = _placeOverride(output, context);
        return true;
    }
}
