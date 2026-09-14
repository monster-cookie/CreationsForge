using System.Text.Json;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Carries one detached typed JSON FormList view with its exact record context.</summary>
public sealed class FormListReadView
{
    /// <summary>Initializes an immutable detached FormList read response.</summary>
    /// <param name="context">The exact request, selection outcome, and containing-plugin provenance.</param>
    /// <param name="record">The detached typed JSON record for a resolved or deleted context, otherwise <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when JSON presence disagrees with the context status or is undefined.</exception>
    public FormListReadView(FormListContext context, JsonElement? record)
    {
        ArgumentNullException.ThrowIfNull(context);
        var hasInspectableContext = context.Status is ReferenceResolutionStatus.Resolved or ReferenceResolutionStatus.Deleted;
        if (hasInspectableContext != record.HasValue)
        {
            throw new ArgumentException("Only resolved and deleted FormList contexts may carry a typed JSON record view.", nameof(record));
        }

        if (record is { ValueKind: JsonValueKind.Undefined })
        {
            throw new ArgumentException("A FormList read view cannot retain an undefined JSON value.", nameof(record));
        }

        Context = context;
        Record = record?.Clone();
    }

    /// <summary>Gets the exact request, selection outcome, and containing-plugin provenance.</summary>
    public FormListContext Context { get; }

    /// <summary>Gets the detached typed JSON record for a resolved or deleted context, otherwise <see langword="null"/>.</summary>
    public JsonElement? Record { get; }
}
