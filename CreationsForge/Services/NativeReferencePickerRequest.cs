using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Services;

/// <summary>Defines one bounded native reference-selection interaction against an exact workspace revision.</summary>
public sealed class NativeReferencePickerRequest
{
    /// <summary>The largest accepted presentation-purpose length after trimming.</summary>
    public const int MaximumPurposeLength = 256;

    /// <summary>Initializes an immutable native reference-picker request.</summary>
    /// <param name="workspaceId">The non-empty active workspace identity.</param>
    /// <param name="expectedRevision">The exact workspace revision against which search and resolution must run.</param>
    /// <param name="currentFormKey">The FormList currently being edited, when the caller has one.</param>
    /// <param name="recordScope">The native contexts that search and resolution may consider.</param>
    /// <param name="containingModKey">An optional containing-plugin filter for a non-winning scope.</param>
    /// <param name="allowNull">Whether the user may explicitly select a null reference.</param>
    /// <param name="purpose">A short user-facing description of where the reference will be used.</param>
    /// <exception cref="ArgumentException">Thrown when the workspace identity or revision baseline is empty, the purpose is blank, or scope and containing-plugin selection conflict.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the purpose is too long or <paramref name="recordScope"/> is undefined.</exception>
    public NativeReferencePickerRequest(
        Guid workspaceId,
        WorkspaceRevision expectedRevision,
        FormKey? currentFormKey,
        RecordScope recordScope,
        ModKey? containingModKey,
        bool allowNull,
        string purpose)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("A native reference picker requires a non-empty workspace identity.", nameof(workspaceId));
        }

        if (expectedRevision.BaselineId == Guid.Empty)
        {
            throw new ArgumentException("A native reference picker requires a non-empty revision baseline.", nameof(expectedRevision));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(purpose);
        var normalizedPurpose = purpose.Trim();
        if (normalizedPurpose.Length > MaximumPurposeLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(purpose),
                $"A native reference-picker purpose cannot exceed {MaximumPurposeLength} characters.");
        }

        if (!Enum.IsDefined(recordScope))
        {
            throw new ArgumentOutOfRangeException(nameof(recordScope));
        }

        if (recordScope == RecordScope.WinningOverrides && containingModKey.HasValue)
        {
            throw new ArgumentException(
                "A containing plugin cannot be combined with the winning-override reference picker.",
                nameof(containingModKey));
        }

        WorkspaceId = workspaceId;
        ExpectedRevision = expectedRevision;
        CurrentFormKey = currentFormKey;
        RecordScope = recordScope;
        ContainingModKey = containingModKey;
        AllowNull = allowNull;
        Purpose = normalizedPurpose;
    }

    /// <summary>Gets the exact active workspace identity.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the exact workspace revision required for every picker operation.</summary>
    public WorkspaceRevision ExpectedRevision { get; }

    /// <summary>Gets the FormList currently being edited, or <see langword="null"/> when unavailable.</summary>
    public FormKey? CurrentFormKey { get; }

    /// <summary>Gets the native contexts considered by search and resolution.</summary>
    public RecordScope RecordScope { get; }

    /// <summary>Gets the optional containing-plugin filter for a non-winning scope.</summary>
    public ModKey? ContainingModKey { get; }

    /// <summary>Gets whether the user may explicitly return a null reference.</summary>
    public bool AllowNull { get; }

    /// <summary>Gets the normalized user-facing purpose of the selected reference.</summary>
    public string Purpose { get; }
}
