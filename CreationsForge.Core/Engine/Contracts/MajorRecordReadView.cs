using System.Text.Json;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Carries one complete detached native major-record field tree with its exact plugin context.</summary>
public sealed class MajorRecordReadView
{
    /// <summary>Initializes an immutable major-record read response.</summary>
    /// <param name="context">The exact request, selection outcome, and containing-plugin provenance.</param>
    /// <param name="recordType">The registered record family, when known.</param>
    /// <param name="record">The detached typed field tree for a resolved or deleted context, otherwise <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when field-tree presence disagrees with context status or the JSON value is undefined.</exception>
    public MajorRecordReadView(FormListContext context, string? recordType, JsonElement? record)
    {
        ArgumentNullException.ThrowIfNull(context);
        var inspectable = context.Status is ReferenceResolutionStatus.Resolved or ReferenceResolutionStatus.Deleted;
        if (inspectable != record.HasValue)
        {
            throw new ArgumentException("Only resolved and deleted major-record contexts may carry a typed field tree.", nameof(record));
        }

        if (record is { ValueKind: JsonValueKind.Undefined })
        {
            throw new ArgumentException("A major-record read view cannot retain an undefined JSON value.", nameof(record));
        }

        Context = context;
        RecordType = recordType;
        Record = record?.Clone();
    }

    /// <summary>Gets the exact request, selection outcome, and containing-plugin provenance.</summary>
    public FormListContext Context { get; }

    /// <summary>Gets the registered record family, or <see langword="null"/> when unavailable.</summary>
    public string? RecordType { get; }

    /// <summary>Gets the complete detached typed field tree, or <see langword="null"/> for an unavailable context.</summary>
    public JsonElement? Record { get; }
}
