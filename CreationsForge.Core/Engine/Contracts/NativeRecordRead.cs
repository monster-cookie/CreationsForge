using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Carries one detached native record together with its exact selected FormList context.</summary>
public sealed class NativeRecordRead
{
    /// <summary>Initializes a contextual native record read.</summary>
    /// <param name="context">The exact request, selection outcome, and containing-plugin provenance.</param>
    /// <param name="recordType">The stable registered native record family, when known.</param>
    /// <param name="record">The detached native getter for resolved or deleted contexts, otherwise <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when record presence disagrees with the context status or native identity.</exception>
    public NativeRecordRead(
        FormListContext context,
        string? recordType,
        IMajorRecordGetter? record)
    {
        ArgumentNullException.ThrowIfNull(context);
        var requiresRecord = context.Status is ReferenceResolutionStatus.Resolved or ReferenceResolutionStatus.Deleted;
        if (requiresRecord != (record is not null))
        {
            throw new ArgumentException("Resolved and deleted native context reads require a detached record, while other outcomes cannot carry one.", nameof(record));
        }

        if (record is not null && record.FormKey != context.Selection.FormKey)
        {
            throw new ArgumentException("The detached native record must match the requested FormKey.", nameof(record));
        }

        Context = context;
        RecordType = recordType;
        Record = record;
    }

    /// <summary>Gets the exact request, selection outcome, and containing-plugin provenance.</summary>
    public FormListContext Context { get; }

    /// <summary>Gets the stable registered native record family, or <see langword="null"/> when unavailable.</summary>
    public string? RecordType { get; }

    /// <summary>Gets the detached native getter for a resolved or deleted context, otherwise <see langword="null"/>.</summary>
    public IMajorRecordGetter? Record { get; }
}
