using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Carries reference-resolution status and an optional detached record getter.
/// </summary>
public sealed class ReferenceResolution
{
    /// <summary>Initializes a reference resolution.</summary>
    /// <param name="status">The observed resolution status.</param>
    /// <param name="formKey">The requested record identity.</param>
    /// <param name="recordType">The record-type identifier, when known.</param>
    /// <param name="record">The detached record getter for a resolved record, otherwise <see langword="null"/>.</param>
    /// <param name="containingModKey">The plugin containing the selected record context, when singular.</param>
    /// <param name="sourcePath">The canonical path of the selected containing plugin, when singular.</param>
    /// <param name="loadOrderIndex">The explicit load-order position of the selected containing plugin, when singular.</param>
    /// <param name="role">The workspace role of the selected containing plugin, when singular.</param>
    /// <exception cref="ArgumentException">Thrown when the status and detached record presence disagree.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="status"/> or <paramref name="role"/> is undefined, or <paramref name="loadOrderIndex"/> is negative.</exception>
    public ReferenceResolution(
        ReferenceResolutionStatus status,
        FormKey formKey,
        string? recordType,
        IMajorRecordGetter? record,
        ModKey? containingModKey = null,
        string? sourcePath = null,
        int? loadOrderIndex = null,
        PluginRole? role = null)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status));
        }

        if (status == ReferenceResolutionStatus.Resolved && record is null)
        {
            throw new ArgumentException("A resolved reference requires a detached record.", nameof(record));
        }

        if (status != ReferenceResolutionStatus.Resolved && record is not null)
        {
            throw new ArgumentException("Only a resolved reference may include a detached record.", nameof(record));
        }

        if (loadOrderIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(loadOrderIndex));
        }
        if (role.HasValue && !Enum.IsDefined(role.Value))
        {
            throw new ArgumentOutOfRangeException(nameof(role));
        }

        Status = status;
        FormKey = formKey;
        RecordType = recordType;
        Record = record;
        ContainingModKey = containingModKey;
        SourcePath = sourcePath;
        LoadOrderIndex = loadOrderIndex;
        Role = role;
    }

    /// <summary>Gets the resolution status.</summary>
    public ReferenceResolutionStatus Status { get; }

    /// <summary>Gets the requested record identity.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the record-type identifier, when known.</summary>
    public string? RecordType { get; }

    /// <summary>
    /// Gets a detached semantic record getter that cannot mutate workspace state, or <see langword="null"/>.
    /// Mutable plugin copies retain record values but may regenerate binary storage metadata such as localized string-table keys when written.
    /// </summary>
    public IMajorRecordGetter? Record { get; }

    /// <summary>Gets the containing plugin, distinct from the record's origin <see cref="FormKey"/>, when singular.</summary>
    public ModKey? ContainingModKey { get; }

    /// <summary>Gets the canonical path of the selected containing plugin, or <see langword="null"/>.</summary>
    public string? SourcePath { get; }

    /// <summary>Gets the containing plugin's explicit load-order position, or <see langword="null"/>.</summary>
    public int? LoadOrderIndex { get; }

    /// <summary>Gets the containing plugin's workspace role, or <see langword="null"/>.</summary>
    public PluginRole? Role { get; }
}
