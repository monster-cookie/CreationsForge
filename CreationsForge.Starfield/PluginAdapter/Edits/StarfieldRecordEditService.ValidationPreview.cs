using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.PluginAdapter.Edits;

/// <summary>Contains Starfield target validation, output-first link resolution, and exact-baseline preview construction.</summary>
public sealed partial class StarfieldRecordEditService
{
    /// <summary>Builds detached before-and-after comparisons for every first-baseline provenance entry.</summary>
    /// <param name="sources">The borrowed immutable Starfield source set.</param>
    /// <param name="output">The borrowed complete staged output and original baseline.</param>
    /// <returns>An immutable preview, including unresolved-reference warnings, without writing files.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after a borrowed plugin lifetime is disposed.</exception>
    public EngineResult<WorkspacePreview> Preview(
        StarfieldPluginSourceSet sources,
        StarfieldPluginOutputState output)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(output);
        var provenanceEntries = output.BorrowProvenance().ToArray();
        var comparisons = new List<FormListComparison>(provenanceEntries.Length);
        var workspaceWarnings = new List<EngineWarning>();
        var unresolvedReferenceCount = 0;

        foreach (var provenance in provenanceEntries)
        {
            if (provenance.RecordType != "FormList")
            {
                continue;
            }

            var beforeResult = GetBeforeRecord(sources, output, provenance);
            if (!beforeResult.Succeeded || beforeResult.Value is null)
            {
                return EngineResult<WorkspacePreview>.Failure(
                    beforeResult.Error ?? new EngineError(
                        EngineErrorCode.UnexpectedFailure,
                        $"Starfield preview could not reconstruct the baseline for {provenance.TargetFormKey}."),
                    warnings: workspaceWarnings);
            }

            var afterResult = GetAfterRecord(sources, output, provenance.TargetFormKey);
            if (!afterResult.Succeeded || afterResult.Value is null)
            {
                return EngineResult<WorkspacePreview>.Failure(
                    afterResult.Error ?? new EngineError(
                        EngineErrorCode.UnexpectedFailure,
                        $"Starfield preview could not read staged target {provenance.TargetFormKey}."),
                    warnings: workspaceWarnings);
            }

            var before = beforeResult.Value;
            var after = afterResult.Value;
            var referenceWarnings = CreateReferenceWarnings(
                sources,
                output.BorrowMod(),
                after.Record!,
                out var recordUnresolvedCount);
            unresolvedReferenceCount = checked(unresolvedReferenceCount + recordUnresolvedCount);
            workspaceWarnings.AddRange(referenceWarnings);
            var changes = Inspector.Compare(before.Record, after.Record, CancellationToken.None);
            JsonElement? beforeView = before.Record is null ? null : WriteReadView(before.Record);
            var afterView = WriteReadView(after.Record!);
            comparisons.Add(new FormListComparison(
                before.Context,
                after.Context,
                beforeView,
                afterView,
                changes,
                referenceWarnings));
        }

        return EngineResult<WorkspacePreview>.Success(new WorkspacePreview(
            comparisons,
            unresolvedReferenceCount,
            workspaceWarnings));
    }

    /// <summary>Finds one unambiguous mutable FormList target without accepting another record family.</summary>
    /// <param name="mod">The complete unpublished output candidate.</param>
    /// <param name="target">The exact requested record identity.</param>
    /// <returns>The mutable FormList or a typed missing, ambiguous, or wrong-family failure.</returns>
    private static EngineResult<FormList> FindMutableTarget(StarfieldMod mod, FormKey target)
    {
        FormList? selected = null;
        var wrongFamily = false;
        foreach (var record in mod.EnumerateMajorRecords())
        {
            if (record.FormKey != target)
            {
                continue;
            }

            if (record is FormList formList)
            {
                if (selected is not null)
                {
                    return EngineResult<FormList>.Failure(new EngineError(
                        EngineErrorCode.ValidationFailed,
                        $"Starfield output contains more than one FormList for {target}."));
                }

                selected = formList;
            }
            else
            {
                wrongFamily = true;
            }
        }

        if (selected is null)
        {
            return EngineResult<FormList>.Failure(new EngineError(
                wrongFamily ? EngineErrorCode.ValidationFailed : EngineErrorCode.RecordNotFound,
                wrongFamily
                    ? $"Starfield record {target} belongs to another record family."
                    : $"Starfield FormList {target} is not contained in the unpublished output candidate."));
        }

        if (wrongFamily)
        {
            return EngineResult<FormList>.Failure(new EngineError(
                EngineErrorCode.ValidationFailed,
                $"Starfield output contains ambiguous record families for {target}."));
        }

        return EngineResult<FormList>.Success(selected);
    }

    /// <summary>Validates every newly introduced non-null link against staged output first and then the source winner.</summary>
    /// <param name="sources">The borrowed immutable source set.</param>
    /// <param name="output">The complete unpublished output candidate.</param>
    /// <param name="references">The copied links introduced by the command.</param>
    /// <returns>The first typed failure, or <see langword="null"/> when every link is valid.</returns>
    private static EngineError? ValidateIntroducedReferences(
        StarfieldPluginSourceSet sources,
        IStarfieldModGetter output,
        IReadOnlyList<PreparedReference> references)
    {
        foreach (var reference in references)
        {
            var resolution = ResolveLiveReference(sources, output, reference.FormKey);
            if (!resolution.Succeeded || resolution.Value is null)
            {
                return resolution.Error ?? new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Starfield reference {reference.FormKey} at {reference.FieldPath} did not resolve to a live record.");
            }

            if (!reference.ExpectedType.IsInstanceOfType(resolution.Value))
            {
                return new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Starfield reference {reference.FormKey} at {reference.FieldPath} must resolve to Mutagen target family {reference.ExpectedType.Name}.");
            }
        }

        return null;
    }

    /// <summary>Resolves a live record with staged output taking precedence over the immutable source winner.</summary>
    /// <param name="sources">The borrowed Starfield source set.</param>
    /// <param name="output">The complete staged output candidate.</param>
    /// <param name="formKey">The non-null record identity to resolve.</param>
    /// <returns>The selected live record getter, or a typed validation failure.</returns>
    private static EngineResult<IMajorRecordGetter> ResolveLiveReference(
        StarfieldPluginSourceSet sources,
        IStarfieldModGetter output,
        FormKey formKey)
    {
        IMajorRecordGetter? outputRecord = null;
        foreach (var record in output.EnumerateMajorRecords())
        {
            if (record.FormKey != formKey)
            {
                continue;
            }

            if (outputRecord is not null)
            {
                return EngineResult<IMajorRecordGetter>.Failure(new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Starfield output contains ambiguous record families for reference {formKey}."));
            }

            outputRecord = record;
        }

        if (outputRecord is not null)
        {
            return outputRecord.IsDeleted
                ? EngineResult<IMajorRecordGetter>.Failure(new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Starfield reference {formKey} resolves to a deleted staged-output record."))
                : EngineResult<IMajorRecordGetter>.Success(outputRecord);
        }

        var sourceResult = sources.Resolve(new ReferenceRequest(formKey, RecordScope.WinningOverrides));
        if (!sourceResult.Succeeded || sourceResult.Value is null)
        {
            return EngineResult<IMajorRecordGetter>.Failure(
                sourceResult.Error ?? new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Starfield reference {formKey} could not be resolved against the immutable sources."));
        }

        var sourceResolution = sourceResult.Value;
        return sourceResolution.Status == ReferenceResolutionStatus.Resolved && sourceResolution.Record is not null
            ? EngineResult<IMajorRecordGetter>.Success(sourceResolution.Record)
            : EngineResult<IMajorRecordGetter>.Failure(new EngineError(
                EngineErrorCode.ValidationFailed,
                $"Starfield reference {formKey} has non-live source status {sourceResolution.Status}."));
    }

    /// <summary>Reconstructs one exact original-output, source-context, or absent before-view.</summary>
    /// <param name="sources">The borrowed immutable source set.</param>
    /// <param name="output">The staged output and independent original baseline.</param>
    /// <param name="provenance">The first-baseline metadata for the target.</param>
    /// <returns>The exact detached prior Mutagen side or a typed baseline failure.</returns>
    private static EngineResult<PreviewRecord> GetBeforeRecord(
        StarfieldPluginSourceSet sources,
        StarfieldPluginOutputState output,
        RecordEditProvenance provenance)
    {
        if (provenance.BaselineKind == EditBaselineKind.Absent)
        {
            var context = new FormListContext(
                new ReferenceRequest(provenance.TargetFormKey, RecordScope.StagedOutput),
                ReferenceResolutionStatus.Unresolved,
                null,
                null,
                null,
                null);
            return EngineResult<PreviewRecord>.Success(new PreviewRecord(context, null));
        }

        if (provenance.BaselineKind == EditBaselineKind.OriginalOutput)
        {
            var record = FindFormList(output.BorrowOriginalMod(), provenance.TargetFormKey);
            if (record is null || provenance.BaselineContext is null)
            {
                return EngineResult<PreviewRecord>.Failure(new EngineError(
                    EngineErrorCode.UnexpectedFailure,
                    $"The original Starfield output baseline no longer contains {provenance.TargetFormKey}."));
            }

            var copy = record.DeepCopy();
            return EngineResult<PreviewRecord>.Success(new PreviewRecord(provenance.BaselineContext, copy));
        }

        if (provenance.SourceBaselineId != sources.Baseline.BaselineId || provenance.BaselineContext is null)
        {
            return EngineResult<PreviewRecord>.Failure(new EngineError(
                EngineErrorCode.ExternalChangeDetected,
                $"The Starfield source baseline for {provenance.TargetFormKey} is no longer current."));
        }

        var readResult = sources.ReadFormListContext(provenance.BaselineContext.Selection);
        if (!readResult.Succeeded || readResult.Value is null)
        {
            return EngineResult<PreviewRecord>.Failure(
                readResult.Error ?? new EngineError(
                    EngineErrorCode.UnexpectedFailure,
                    $"The exact Starfield source baseline for {provenance.TargetFormKey} could not be read."));
        }

        var read = readResult.Value;
        if (!ContextsMatch(provenance.BaselineContext, read.Context) || read.Record is not IFormListGetter formList)
        {
            return EngineResult<PreviewRecord>.Failure(new EngineError(
                EngineErrorCode.ExternalChangeDetected,
                $"The exact Starfield source context for {provenance.TargetFormKey} no longer matches its captured baseline."));
        }

        return EngineResult<PreviewRecord>.Success(new PreviewRecord(provenance.BaselineContext, formList));
    }

    /// <summary>Reads one exact current staged-output FormList and constructs its complete context.</summary>
    /// <param name="sources">The borrowed source set whose plugin count positions the output last.</param>
    /// <param name="output">The complete staged output state.</param>
    /// <param name="formKey">The exact target identity.</param>
    /// <returns>The detached current Mutagen side or a typed missing-state failure.</returns>
    private static EngineResult<PreviewRecord> GetAfterRecord(
        StarfieldPluginSourceSet sources,
        StarfieldPluginOutputState output,
        FormKey formKey)
    {
        var record = FindFormList(output.BorrowMod(), formKey);
        if (record is null)
        {
            return EngineResult<PreviewRecord>.Failure(new EngineError(
                EngineErrorCode.RecordNotFound,
                $"The staged Starfield output no longer contains edited FormList {formKey}."));
        }

        var context = new FormListContext(
            new ReferenceRequest(formKey, RecordScope.StagedOutput),
            record.IsDeleted ? ReferenceResolutionStatus.Deleted : ReferenceResolutionStatus.Resolved,
            output.Association.ModKey,
            output.Association.PluginPath,
            sources.GetMutagenMods().Count,
            PluginRole.Output);
        return EngineResult<PreviewRecord>.Success(new PreviewRecord(context, record.DeepCopy()));
    }

    /// <summary>Finds one FormList getter in an exact complete plugin.</summary>
    /// <param name="mod">The complete Starfield plugin to search.</param>
    /// <param name="formKey">The exact record identity.</param>
    /// <returns>The matching FormList getter, or <see langword="null"/> when absent.</returns>
    private static IFormListGetter? FindFormList(IStarfieldModGetter mod, FormKey formKey)
    {
        foreach (var formList in mod.FormLists)
        {
            if (formList.FormKey == formKey)
            {
                return formList;
            }
        }

        return null;
    }

    /// <summary>Collects non-fatal warnings for unresolved links already present in an edited resulting record.</summary>
    /// <param name="sources">The borrowed immutable source set.</param>
    /// <param name="output">The current complete staged output.</param>
    /// <param name="record">The edited resulting FormList.</param>
    /// <param name="unresolvedCount">Receives the number of links without a live output-first target.</param>
    /// <returns>Warnings in record link traversal order.</returns>
    private static IReadOnlyList<EngineWarning> CreateReferenceWarnings(
        StarfieldPluginSourceSet sources,
        IStarfieldModGetter output,
        IFormListGetter record,
        out int unresolvedCount)
    {
        var warnings = new List<EngineWarning>();
        unresolvedCount = 0;
        var index = 0;
        foreach (var link in record.EnumerateFormLinks(false))
        {
            if (link.FormKeyNullable is not { } formKey || formKey.IsNull)
            {
                index++;
                continue;
            }

            var resolution = ResolveLiveReference(sources, output, formKey);
            if (!resolution.Succeeded || resolution.Value is null)
            {
                unresolvedCount = checked(unresolvedCount + 1);
                warnings.Add(new EngineWarning(
                    "missing-plugin-reference",
                    $"Starfield reference {formKey} at FormLinks[{index}] does not resolve to a live output or source record."));
            }
            else if (!link.Type.IsInstanceOfType(resolution.Value))
            {
                warnings.Add(new EngineWarning(
                    "invalid-plugin-reference-family",
                    $"Starfield reference {formKey} at FormLinks[{index}] does not resolve to declared Mutagen target family {link.Type.Name}."));
            }

            index++;
        }

        return Array.AsReadOnly(warnings.ToArray());
    }

    /// <summary>Writes one complete detached read view through the public inspector representation.</summary>
    /// <param name="record">The detached Starfield FormList getter.</param>
    /// <returns>An independently owned JSON value.</returns>
    private JsonElement WriteReadView(IFormListGetter record)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            Inspector.WriteReadView(record, writer, CancellationToken.None);
            writer.Flush();
        }

        using var document = JsonDocument.Parse(stream.GetBuffer().AsMemory(0, checked((int)stream.Length)));
        return document.RootElement.Clone();
    }

    /// <summary>Compares two complete captured record contexts without substituting current winners.</summary>
    /// <param name="expected">The exact captured baseline context.</param>
    /// <param name="actual">The exact context returned by current immutable-source lookup.</param>
    /// <returns><see langword="true"/> when every selection, status, and containing-plugin property matches.</returns>
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

    /// <summary>Pairs one exact record context with its detached FormList, including a confirmed absent side.</summary>
    private sealed class PreviewRecord
    {
        /// <summary>Initializes one detached preview side.</summary>
        /// <param name="context">The exact context and resolution status.</param>
        /// <param name="record">The detached record, or <see langword="null"/> only for a confirmed absent context.</param>
        internal PreviewRecord(FormListContext context, IFormListGetter? record)
        {
            ArgumentNullException.ThrowIfNull(context);
            Context = context;
            Record = record;
        }

        /// <summary>Gets the exact Mutagen selection context.</summary>
        internal FormListContext Context { get; }

        /// <summary>Gets the detached FormList, or <see langword="null"/> for confirmed absence.</summary>
        internal IFormListGetter? Record { get; }
    }
}
