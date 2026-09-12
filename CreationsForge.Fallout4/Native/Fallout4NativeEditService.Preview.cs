using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInspection;
using CreationsForge.Fallout4.Native.Edits;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Fallout4.Native;

/// <summary>Contains exact baseline reconstruction, semantic comparison, and unresolved-reference reporting for Fallout 4 previews.</summary>
public sealed partial class Fallout4NativeEditService
{
    /// <summary>Builds exact detached before-and-after comparisons for every staged Fallout 4 FormList without writing files.</summary>
    /// <param name="sources">The immutable source lifetime that owns exact captured source baselines.</param>
    /// <param name="output">The complete staged output and its ordered edit provenance.</param>
    /// <returns>All staged comparisons and current unresolved direct-reference warnings, or a typed provenance failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    public EngineResult<WorkspacePreview> Preview(
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputState output)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(output);
        if (sources.Baseline.BaselineId == Guid.Empty)
        {
            return PreviewFailure(sources, "The Fallout 4 source baseline identity is empty.");
        }

        Fallout4Mod current;
        IFallout4ModGetter original;
        IReadOnlyList<NativeEditProvenance> provenance;
        try
        {
            current = output.BorrowMod();
            original = output.BorrowOriginalMod();
            provenance = output.GetEditProvenance();
        }
        catch (ObjectDisposedException exception)
        {
            return EngineResult<WorkspacePreview>.Failure(
                new EngineError(EngineErrorCode.WorkspaceDisposed, exception.Message),
                workspaceId: sources.WorkspaceId,
                resultRevision: sources.Revision);
        }

        var comparisons = new List<FormListComparison>(provenance.Count);
        var allWarnings = new List<EngineWarning>();
        var unresolvedReferenceCount = 0;
        foreach (var entry in provenance)
        {
            var beforeResult = ReadBefore(sources, original, entry);
            if (!beforeResult.Succeeded)
            {
                return EngineResult<WorkspacePreview>.Failure(
                    beforeResult.Error!,
                    workspaceId: sources.WorkspaceId,
                    resultRevision: sources.Revision,
                    warnings: allWarnings.Concat(beforeResult.Warnings).ToArray());
            }

            var afterMatches = FindRecords(current, entry.TargetFormKey);
            if (afterMatches.Count != 1 || afterMatches[0] is not IFormListGetter afterBorrowed)
            {
                return PreviewFailure(
                    sources,
                    $"Staged Fallout 4 FormList '{entry.TargetFormKey}' is missing, duplicated, or belongs to another native record family.",
                    allWarnings);
            }

            var after = (IFormListGetter)afterBorrowed.DeepCopy();
            var afterContext = CreateOutputContext(sources, output, after);
            var comparisonWarningsResult = CreateMissingReferenceWarnings(sources, current, after);
            if (!comparisonWarningsResult.Succeeded)
            {
                return EngineResult<WorkspacePreview>.Failure(
                    comparisonWarningsResult.Error!,
                    workspaceId: sources.WorkspaceId,
                    resultRevision: sources.Revision,
                    warnings: allWarnings);
            }

            var comparisonWarnings = comparisonWarningsResult.Value!;
            unresolvedReferenceCount += comparisonWarnings.Count;
            allWarnings.AddRange(comparisonWarnings);
            var beforeRead = beforeResult.Value!;
            comparisons.Add(new FormListComparison(
                beforeRead.Context,
                afterContext,
                WriteView(beforeRead.Record),
                WriteView(after),
                _inspector.Compare(beforeRead.Record, after, CancellationToken.None),
                comparisonWarnings));
        }

        return EngineResult<WorkspacePreview>.Success(
            new WorkspacePreview(comparisons, unresolvedReferenceCount, allWarnings),
            workspaceId: sources.WorkspaceId,
            resultRevision: sources.Revision,
            warnings: allWarnings);
    }

    /// <summary>Reads the immutable exact baseline identified by one provenance entry.</summary>
    /// <param name="sources">The immutable native source lifetime.</param>
    /// <param name="original">The complete original output snapshot.</param>
    /// <param name="provenance">The exact first-edit baseline metadata.</param>
    /// <returns>A detached optional before record and its exact context.</returns>
    private static EngineResult<PreviewRecord> ReadBefore(
        Fallout4NativeSourceSet sources,
        IFallout4ModGetter original,
        NativeEditProvenance provenance)
    {
        if (provenance.BaselineKind == EditBaselineKind.Absent)
        {
            return EngineResult<PreviewRecord>.Success(new PreviewRecord(
                new FormListContext(
                    new ReferenceRequest(provenance.TargetFormKey, RecordScope.StagedOutput),
                    ReferenceResolutionStatus.Unresolved,
                    containingModKey: null,
                    path: null,
                    loadOrderIndex: null,
                    role: null),
                record: null));
        }

        if (provenance.BaselineKind == EditBaselineKind.OriginalOutput)
        {
            var matches = original.EnumerateMajorRecords()
                .Where(record => record.FormKey == provenance.TargetFormKey)
                .ToArray();
            if (matches.Length != 1 || matches[0] is not IFormListGetter formList)
            {
                return EngineResult<PreviewRecord>.Failure(new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Original Fallout 4 output baseline for '{provenance.TargetFormKey}' is missing, duplicated, or belongs to another native family."));
            }

            return EngineResult<PreviewRecord>.Success(new PreviewRecord(
                provenance.BaselineContext!,
                (IFormListGetter)formList.DeepCopy()));
        }

        if (provenance.SourceBaselineId != sources.Baseline.BaselineId)
        {
            return EngineResult<PreviewRecord>.Failure(new EngineError(
                EngineErrorCode.ValidationFailed,
                $"Fallout 4 source baseline for staged edit '{provenance.EditId}' no longer matches the open source lifetime."));
        }

        var read = sources.ReadFormListContext(provenance.BaselineContext!.Selection);
        if (!read.Succeeded)
        {
            return EngineResult<PreviewRecord>.Failure(read.Error!, warnings: read.Warnings);
        }

        if (read.Value!.Record is not IFormListGetter sourceFormList
            || !ContextsMatch(provenance.BaselineContext, read.Value.Context))
        {
            return EngineResult<PreviewRecord>.Failure(new EngineError(
                EngineErrorCode.ValidationFailed,
                $"Exact Fallout 4 source baseline for '{provenance.TargetFormKey}' could not be reconstructed."),
                warnings: read.Warnings);
        }

        return EngineResult<PreviewRecord>.Success(
            new PreviewRecord(provenance.BaselineContext, (IFormListGetter)sourceFormList.DeepCopy()),
            warnings: read.Warnings);
    }

    /// <summary>Creates the exact current staged-output context for a detached record.</summary>
    /// <param name="sources">The source lifetime used to determine output load-order position.</param>
    /// <param name="output">The staged output identity owner.</param>
    /// <param name="record">The current detached output FormList.</param>
    /// <returns>A complete resolved or deleted output context.</returns>
    private static FormListContext CreateOutputContext(
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputState output,
        IFormListGetter record)
    {
        return new FormListContext(
            new ReferenceRequest(record.FormKey, RecordScope.StagedOutput, output.Association.ModKey),
            record.IsDeleted ? ReferenceResolutionStatus.Deleted : ReferenceResolutionStatus.Resolved,
            output.Association.ModKey,
            output.Association.PluginPath,
            sources.GetNativeMods().Count,
            PluginRole.Output);
    }

    /// <summary>Collects one warning for each current direct item that lacks a live staged-output or source target.</summary>
    /// <param name="sources">The immutable native source lifetime.</param>
    /// <param name="mod">The complete staged output.</param>
    /// <param name="formList">The detached current FormList.</param>
    /// <returns>Warnings in exact item order, or a typed source-resolution failure.</returns>
    private static EngineResult<IReadOnlyList<EngineWarning>> CreateMissingReferenceWarnings(
        Fallout4NativeSourceSet sources,
        Fallout4Mod mod,
        IFormListGetter formList)
    {
        var warnings = new List<EngineWarning>();
        for (var index = 0; index < formList.Items.Count; index++)
        {
            var formKey = formList.Items[index].FormKey;
            if (formKey.IsNull)
            {
                continue;
            }

            var outputMatches = FindRecords(mod, formKey);
            var isLive = outputMatches.Count == 1 && !outputMatches[0].IsDeleted;
            if (outputMatches.Count == 0)
            {
                var resolution = sources.Resolve(new ReferenceRequest(formKey, RecordScope.WinningOverrides));
                if (!resolution.Succeeded)
                {
                    return EngineResult<IReadOnlyList<EngineWarning>>.Failure(resolution.Error!);
                }

                isLive = resolution.Value!.Status == ReferenceResolutionStatus.Resolved;
            }

            if (!isLive)
            {
                warnings.Add(new EngineWarning(
                    "missing-native-reference",
                    $"Native reference {formKey} at Items[{index}] does not resolve to a live record in the current workspace view."));
            }
        }

        return EngineResult<IReadOnlyList<EngineWarning>>.Success(Array.AsReadOnly(warnings.ToArray()));
    }

    /// <summary>Writes one optional detached record into an independently owned JSON value.</summary>
    /// <param name="record">The detached record, or <see langword="null"/> for an absent baseline.</param>
    /// <returns>A detached JSON object, or <see langword="null"/>.</returns>
    private JsonElement? WriteView(IFormListGetter? record)
    {
        if (record is null)
        {
            return null;
        }

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            _inspector.WriteReadView(record, writer, CancellationToken.None);
        }

        using var document = JsonDocument.Parse(stream.ToArray());
        return document.RootElement.Clone();
    }

    /// <summary>Compares every exact provenance property needed to prevent winner re-resolution.</summary>
    /// <param name="expected">The captured exact context.</param>
    /// <param name="actual">The freshly reconstructed exact context.</param>
    /// <returns><see langword="true"/> when every selection and containing-plugin value matches.</returns>
    private static bool ContextsMatch(FormListContext expected, FormListContext actual)
    {
        var pathComparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        return expected.Selection.FormKey == actual.Selection.FormKey
            && expected.Selection.Scope == actual.Selection.Scope
            && expected.Selection.ContainingModKey == actual.Selection.ContainingModKey
            && expected.Status == actual.Status
            && expected.ContainingModKey == actual.ContainingModKey
            && string.Equals(expected.Path, actual.Path, pathComparison)
            && expected.LoadOrderIndex == actual.LoadOrderIndex
            && expected.Role == actual.Role;
    }

    /// <summary>Creates a typed preview failure with optional warnings already collected.</summary>
    /// <param name="sources">The immutable source lifetime.</param>
    /// <param name="message">The actionable provenance or staged-state failure.</param>
    /// <param name="warnings">Optional warnings collected before the failure.</param>
    /// <returns>A failed workspace preview result.</returns>
    private static EngineResult<WorkspacePreview> PreviewFailure(
        Fallout4NativeSourceSet sources,
        string message,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return EngineResult<WorkspacePreview>.Failure(
            new EngineError(EngineErrorCode.ValidationFailed, message),
            workspaceId: sources.WorkspaceId,
            resultRevision: sources.Revision,
            warnings: warnings);
    }

    /// <summary>Pairs an exact immutable preview context with an optional detached native FormList.</summary>
    private sealed class PreviewRecord
    {
        /// <summary>Initializes one detached preview record.</summary>
        /// <param name="context">The exact native context.</param>
        /// <param name="record">The detached FormList, or <see langword="null"/> for an absent baseline.</param>
        internal PreviewRecord(FormListContext context, IFormListGetter? record)
        {
            Context = context;
            Record = record;
        }

        /// <summary>Gets the exact native context.</summary>
        internal FormListContext Context { get; }

        /// <summary>Gets the detached FormList, or <see langword="null"/>.</summary>
        internal IFormListGetter? Record { get; }
    }
}
