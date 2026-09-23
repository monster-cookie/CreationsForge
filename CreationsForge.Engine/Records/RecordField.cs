using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Engine.Records;

/// <summary>Connects one published descriptor to bounded typed Mutagen read, validation, and mutation delegates.</summary>
public abstract class RecordField
{
    /// <summary>Initializes a registered field.</summary>
    /// <param name="descriptor">The public field descriptor.</param>
    protected RecordField(RecordFieldDescriptor descriptor)
    {
        Descriptor = descriptor;
    }

    /// <summary>Gets the public field descriptor.</summary>
    public RecordFieldDescriptor Descriptor { get; }

    internal abstract RecordValue Read(IMajorRecordGetter record);

    internal abstract void Validate(RecordFieldChange change);

    internal abstract void Apply(IMajorRecord record, RecordFieldChange change);

    internal virtual IReadOnlyList<RecordFormLinkReference> GetReferences(RecordFieldChange change) => [];
}

/// <summary>Implements a registered field with delegates compiled against exact Mutagen setter and getter types.</summary>
/// <typeparam name="TRecord">The exact mutable record type.</typeparam>
/// <typeparam name="TGetter">The exact Mutagen getter interface.</typeparam>
public sealed class RecordField<TRecord, TGetter> : RecordField
    where TRecord : class, IMajorRecord
    where TGetter : class, IMajorRecordGetter
{
    private readonly Func<TGetter, RecordValue> _read;
    private readonly Action<RecordFieldChange> _validate;
    private readonly Action<TRecord, RecordFieldChange> _apply;
    private readonly Func<RecordFieldChange, IReadOnlyList<RecordFormLinkReference>> _references;

    /// <summary>Initializes a field backed only by explicitly supplied typed Mutagen delegates.</summary>
    /// <param name="descriptor">The public discovered field descriptor.</param>
    /// <param name="read">The typed Mutagen read projection.</param>
    /// <param name="validate">The closed value and operation validator.</param>
    /// <param name="apply">The typed Mutagen mutation.</param>
    /// <param name="references">Optional FormKey references carried by a change for link-cache validation.</param>
    internal RecordField(
        RecordFieldDescriptor descriptor,
        Func<TGetter, RecordValue> read,
        Action<RecordFieldChange> validate,
        Action<TRecord, RecordFieldChange> apply,
        Func<RecordFieldChange, IReadOnlyList<RecordFormLinkReference>>? references = null)
        : base(descriptor)
    {
        _read = read;
        _validate = validate;
        _apply = apply;
        _references = references ?? (_ => []);
    }

    internal override RecordValue Read(IMajorRecordGetter record)
    {
        if (record is not TGetter getter)
        {
            throw new RecordEditingException($"Record '{record.GetType().FullName}' does not implement expected getter '{typeof(TGetter).FullName}'.");
        }

        return _read(getter);
    }

    internal override void Validate(RecordFieldChange change)
    {
        if (!string.Equals(change.Path, Descriptor.Path, StringComparison.Ordinal))
        {
            throw new RecordEditingException($"Field change path '{change.Path}' does not match registered field '{Descriptor.Path}'.");
        }

        if (!Descriptor.Operations.Contains(change.Operation))
        {
            throw new RecordEditingException($"Operation '{change.Operation}' is not legal for field '{Descriptor.Path}'.");
        }

        _validate(change);
    }

    internal override void Apply(IMajorRecord record, RecordFieldChange change)
    {
        if (record is not TRecord setter)
        {
            throw new RecordEditingException($"Record '{record.GetType().FullName}' is not expected setter '{typeof(TRecord).FullName}'.");
        }

        _apply(setter, change);
    }

    internal override IReadOnlyList<RecordFormLinkReference> GetReferences(RecordFieldChange change) => _references(change);
}

/// <summary>Captures one changed FormKey and its permitted getter interfaces for link-cache validation.</summary>
internal sealed class RecordFormLinkReference
{
    /// <summary>Initializes a reference validation request.</summary>
    /// <param name="formKey">The referenced record identity.</param>
    /// <param name="targetTypes">The permitted Mutagen getter interfaces.</param>
    public RecordFormLinkReference(FormKey formKey, IReadOnlyList<Type> targetTypes)
    {
        FormKey = formKey;
        TargetTypes = targetTypes;
    }

    /// <summary>Gets the referenced record identity.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the permitted Mutagen getter interfaces.</summary>
    public IReadOnlyList<Type> TargetTypes { get; }
}
