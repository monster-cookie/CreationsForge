using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Records output-owned metadata needed to reconstruct the exact before-view for a staged record edit.</summary>
public sealed class RecordEditProvenance
{
    /// <summary>Initializes immutable staged-edit provenance without retaining a record.</summary>
    /// <param name="editId">The non-empty staged edit-session identifier.</param>
    /// <param name="targetFormKey">The non-null major-record identity edited in the output.</param>
    /// <param name="baselineKind">Whether the before-view is absent, from the original output, or from one exact source context.</param>
    /// <param name="baselineContext">The exact resolved or deleted record context for an original-output or source baseline, otherwise <see langword="null"/>.</param>
    /// <param name="sourceBaselineId">The non-empty immutable source-set baseline identifier for a source context, otherwise <see langword="null"/>.</param>
    /// <param name="recordType">The exact native record family; omitted legacy entries select FormList.</param>
    /// <exception cref="ArgumentException">Thrown when an identifier is empty, the target is null, or the context does not exactly identify the target and required plugin origin.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="baselineKind"/> is undefined.</exception>
    public RecordEditProvenance(
        Guid editId,
        FormKey targetFormKey,
        EditBaselineKind baselineKind,
        FormListContext? baselineContext = null,
        Guid? sourceBaselineId = null,
        string recordType = "FormList")
    {
        if (editId == Guid.Empty)
        {
            throw new ArgumentException("Record edit provenance requires a non-empty edit identifier.", nameof(editId));
        }

        if (targetFormKey.IsNull)
        {
            throw new ArgumentException("Record edit provenance requires a non-null target FormKey.", nameof(targetFormKey));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(recordType);

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
        RecordType = recordType;
    }

    /// <summary>Gets the staged edit-session identifier.</summary>
    public Guid EditId { get; }

    /// <summary>Gets the major-record identity edited in the output.</summary>
    public FormKey TargetFormKey { get; }

    /// <summary>Gets how the exact plugin before-view must be reconstructed.</summary>
    public EditBaselineKind BaselineKind { get; }

    /// <summary>Gets the exact resolved or deleted original-output or source context, or <see langword="null"/> for an absent baseline.</summary>
    public FormListContext? BaselineContext { get; }

    /// <summary>Gets the immutable source-set baseline identifier for a source context, or <see langword="null"/> otherwise.</summary>
    public Guid? SourceBaselineId { get; }

    /// <summary>Gets the exact native major-record family represented by this baseline.</summary>
    public string RecordType { get; }

    /// <summary>Validates the complete baseline discriminator, context, and source-baseline combination.</summary>
    /// <param name="targetFormKey">The major-record identity edited in the output.</param>
    /// <param name="baselineKind">The requested before-view source.</param>
    /// <param name="baselineContext">The optional exact record context.</param>
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
                throw new ArgumentException("An absent edit baseline cannot identify a record context or source baseline.", nameof(baselineContext));
            }

            return;
        }

        if (baselineContext is null)
        {
            throw new ArgumentException("A record edit baseline requires an exact record context.", nameof(baselineContext));
        }

        if (baselineContext.Selection.FormKey != targetFormKey)
        {
            throw new ArgumentException("The record edit baseline context must identify the target FormKey.", nameof(baselineContext));
        }

        if (baselineContext.Status is not ReferenceResolutionStatus.Resolved and not ReferenceResolutionStatus.Deleted)
        {
            throw new ArgumentException("The record edit baseline context must be exactly resolved or deleted.", nameof(baselineContext));
        }

        if (!baselineContext.ContainingModKey.HasValue || baselineContext.Path is null ||
            !baselineContext.LoadOrderIndex.HasValue || !baselineContext.Role.HasValue)
        {
            throw new ArgumentException("The record edit baseline context requires complete containing-plugin provenance.", nameof(baselineContext));
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
