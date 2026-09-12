using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Records output-owned metadata needed to reconstruct the exact before-view for a staged native edit.</summary>
public sealed class NativeEditProvenance
{
    /// <summary>Initializes immutable staged-edit provenance without retaining a native record.</summary>
    /// <param name="editId">The non-empty staged edit-session identifier.</param>
    /// <param name="targetFormKey">The non-null native FormList identity edited in the output.</param>
    /// <param name="baselineKind">Whether the before-view is absent, from the original output, or from one exact source context.</param>
    /// <param name="baselineContext">The exact resolved or deleted native context for an original-output or source baseline, otherwise <see langword="null"/>.</param>
    /// <param name="sourceBaselineId">The non-empty immutable source-set baseline identifier for a source context, otherwise <see langword="null"/>.</param>
    /// <exception cref="ArgumentException">Thrown when an identifier is empty, the target is null, or the context does not exactly identify the target and required native origin.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="baselineKind"/> is undefined.</exception>
    public NativeEditProvenance(
        Guid editId,
        FormKey targetFormKey,
        EditBaselineKind baselineKind,
        FormListContext? baselineContext = null,
        Guid? sourceBaselineId = null)
    {
        if (editId == Guid.Empty)
        {
            throw new ArgumentException("Native edit provenance requires a non-empty edit identifier.", nameof(editId));
        }

        if (targetFormKey.IsNull)
        {
            throw new ArgumentException("Native edit provenance requires a non-null target FormKey.", nameof(targetFormKey));
        }

        if (!Enum.IsDefined(baselineKind))
        {
            throw new ArgumentOutOfRangeException(nameof(baselineKind));
        }

        ValidateBaseline(targetFormKey, baselineKind, baselineContext, sourceBaselineId);
        EditId = editId;
        TargetFormKey = targetFormKey;
        BaselineKind = baselineKind;
        BaselineContext = baselineContext;
        SourceBaselineId = sourceBaselineId;
    }

    /// <summary>Gets the staged edit-session identifier.</summary>
    public Guid EditId { get; }

    /// <summary>Gets the native FormList identity edited in the output.</summary>
    public FormKey TargetFormKey { get; }

    /// <summary>Gets how the exact native before-view must be reconstructed.</summary>
    public EditBaselineKind BaselineKind { get; }

    /// <summary>Gets the exact resolved or deleted original-output or source context, or <see langword="null"/> for an absent baseline.</summary>
    public FormListContext? BaselineContext { get; }

    /// <summary>Gets the immutable source-set baseline identifier for a source context, or <see langword="null"/> otherwise.</summary>
    public Guid? SourceBaselineId { get; }

    /// <summary>Validates the complete baseline discriminator, context, and source-baseline combination.</summary>
    /// <param name="targetFormKey">The native FormList identity edited in the output.</param>
    /// <param name="baselineKind">The requested before-view source.</param>
    /// <param name="baselineContext">The optional exact native context.</param>
    /// <param name="sourceBaselineId">The optional immutable source-set baseline identifier.</param>
    /// <exception cref="ArgumentException">Thrown when the values do not form one supported exact baseline.</exception>
    private static void ValidateBaseline(
        FormKey targetFormKey,
        EditBaselineKind baselineKind,
        FormListContext? baselineContext,
        Guid? sourceBaselineId)
    {
        if (baselineKind == EditBaselineKind.Absent)
        {
            if (baselineContext is not null || sourceBaselineId is not null)
            {
                throw new ArgumentException("An absent edit baseline cannot identify a native context or source baseline.", nameof(baselineContext));
            }

            return;
        }

        if (baselineContext is null)
        {
            throw new ArgumentException("A native edit baseline requires an exact native context.", nameof(baselineContext));
        }

        if (baselineContext.Selection.FormKey != targetFormKey)
        {
            throw new ArgumentException("The native edit baseline context must identify the target FormKey.", nameof(baselineContext));
        }

        if (baselineContext.Status is not ReferenceResolutionStatus.Resolved and not ReferenceResolutionStatus.Deleted)
        {
            throw new ArgumentException("The native edit baseline context must be exactly resolved or deleted.", nameof(baselineContext));
        }

        if (!baselineContext.ContainingModKey.HasValue || baselineContext.Path is null ||
            !baselineContext.LoadOrderIndex.HasValue || !baselineContext.Role.HasValue)
        {
            throw new ArgumentException("The native edit baseline context requires complete containing-plugin provenance.", nameof(baselineContext));
        }

        if (baselineKind == EditBaselineKind.OriginalOutput)
        {
            if (baselineContext.Selection.Scope != RecordScope.StagedOutput || baselineContext.Role != PluginRole.Output)
            {
                throw new ArgumentException("An original-output edit baseline requires an exact staged-output context.", nameof(baselineContext));
            }

            if (sourceBaselineId is not null)
            {
                throw new ArgumentException("An original-output edit baseline cannot identify a source baseline.", nameof(sourceBaselineId));
            }

            return;
        }

        if (baselineContext.Selection.Scope == RecordScope.StagedOutput || baselineContext.Role == PluginRole.Output)
        {
            throw new ArgumentException("A source edit baseline cannot identify staged output state.", nameof(baselineContext));
        }

        if (!sourceBaselineId.HasValue || sourceBaselineId.Value == Guid.Empty)
        {
            throw new ArgumentException("A source edit baseline requires a non-empty source baseline identifier.", nameof(sourceBaselineId));
        }
    }
}
