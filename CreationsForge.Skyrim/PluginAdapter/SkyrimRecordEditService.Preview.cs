using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim.PluginAdapter;

/// <summary>Builds exact detached previews for staged Skyrim FormList edits.</summary>
public sealed partial class SkyrimRecordEditService
{
    /// <summary>Builds detached Mutagen before-and-after comparisons for every staged Skyrim FormList target.</summary>
    /// <param name="sources">The borrowed immutable source set used for exact source baselines and link resolution.</param>
    /// <param name="output">The borrowed complete staged output and its original Mutagen baseline.</param>
    /// <returns>An immutable preview with ordered comparisons and one warning for each unresolved link occurrence.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    public EngineResult<WorkspacePreview> Preview(
        SkyrimPluginSourceSet sources,
        SkyrimPluginOutputState output)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(output);

        try
        {
            var current = output.GetMutableMod();
            var original = output.GetOriginalMod();
            var comparisons = new List<FormListComparison>();
            var warnings = new List<EngineWarning>();
            var unresolvedReferenceCount = 0;
            foreach (var provenance in output.GetEditProvenance())
            {
                if (provenance.RecordType != "FormList")
                {
                    continue;
                }

                var afterResult = FindRecord(current, provenance.TargetFormKey);
                if (!afterResult.Succeeded)
                {
                    return PreviewFailure(afterResult.Error!, warnings);
                }

                if (afterResult.Value is not IFormListGetter afterRecord)
                {
                    return PreviewFailure(
                        new EngineError(
                            EngineErrorCode.ValidationFailed,
                            $"Staged Skyrim identity '{provenance.TargetFormKey}' is not a FormList."),
                        warnings);
                }

                IFormListGetter? beforeRecord = null;
                FormListContext beforeContext;
                if (provenance.BaselineKind == EditBaselineKind.Absent)
                {
                    beforeContext = new FormListContext(
                        new ReferenceRequest(
                            provenance.TargetFormKey,
                            RecordScope.StagedOutput,
                            output.Association.ModKey),
                        ReferenceResolutionStatus.Unresolved,
                        containingModKey: null,
                        path: null,
                        loadOrderIndex: null,
                        role: null);
                }
                else if (provenance.BaselineKind == EditBaselineKind.OriginalOutput)
                {
                    var beforeResult = FindRecord(original, provenance.TargetFormKey);
                    if (!beforeResult.Succeeded || beforeResult.Value is not IFormListGetter originalRecord)
                    {
                        return PreviewFailure(
                            beforeResult.Error
                                ?? new EngineError(
                                    EngineErrorCode.ValidationFailed,
                                    $"Original Skyrim output baseline '{provenance.TargetFormKey}' is not a FormList."),
                            warnings);
                    }

                    beforeContext = provenance.BaselineContext!;
                    beforeRecord = originalRecord.DeepCopy();
                }
                else if (provenance.BaselineKind == EditBaselineKind.SourceContext)
                {
                    if (provenance.SourceBaselineId != sources.Baseline.BaselineId)
                    {
                        return PreviewFailure(
                            new EngineError(
                                EngineErrorCode.ExternalChangeDetected,
                                $"Skyrim source baseline changed after FormList '{provenance.TargetFormKey}' began editing."),
                            warnings);
                    }

                    var expectedContext = provenance.BaselineContext!;
                    var sourceResult = sources.ReadFormListContext(expectedContext.Selection);
                    if (!sourceResult.Succeeded
                        || sourceResult.Value?.Context.Status != ReferenceResolutionStatus.Resolved
                        || sourceResult.Value.Record is not IFormListGetter sourceRecord)
                    {
                        return PreviewFailure(
                            sourceResult.Error
                                ?? new EngineError(
                                    EngineErrorCode.ValidationFailed,
                                    $"Exact Skyrim source baseline '{provenance.TargetFormKey}' is no longer resolved."),
                            warnings);
                    }

                    if (!ContextsMatch(expectedContext, sourceResult.Value.Context))
                    {
                        return PreviewFailure(
                            new EngineError(
                                EngineErrorCode.ExternalChangeDetected,
                                $"Exact Skyrim source context changed after FormList '{provenance.TargetFormKey}' began editing."),
                            warnings);
                    }

                    beforeContext = expectedContext;
                    beforeRecord = sourceRecord.DeepCopy();
                }
                else
                {
                    return PreviewFailure(
                        new EngineError(
                            EngineErrorCode.ValidationFailed,
                            $"Skyrim edit baseline kind '{provenance.BaselineKind}' is undefined."),
                        warnings);
                }

                var afterContext = CreateOutputContext(sources, output, afterRecord);
                var afterDetached = afterRecord.DeepCopy();
                var recordWarnings = new List<EngineWarning>();
                for (var index = 0; index < afterDetached.Items.Count; index++)
                {
                    var item = GetFormKey(afterDetached.Items[index]);
                    if (item.IsNull)
                    {
                        continue;
                    }

                    var resolution = ResolveLink(sources, current, item);
                    if (!resolution.Succeeded)
                    {
                        return PreviewFailure(resolution.Error!, warnings);
                    }

                    if (resolution.Value == ReferenceResolutionStatus.Resolved)
                    {
                        continue;
                    }

                    unresolvedReferenceCount++;
                    var warning = new EngineWarning(
                        "missing-plugin-reference",
                        $"Reference {item} at {nameof(IFormListGetter.Items)}[{index}] does not resolve to a live record in the current workspace view.");
                    recordWarnings.Add(warning);
                    warnings.Add(warning);
                }

                var changes = _inspector.Compare(beforeRecord, afterDetached, CancellationToken.None);
                comparisons.Add(new FormListComparison(
                    beforeContext,
                    afterContext,
                    beforeRecord is null ? null : CreateJsonView(beforeRecord),
                    CreateJsonView(afterDetached),
                    changes,
                    recordWarnings));
            }

            return EngineResult<WorkspacePreview>.Success(new WorkspacePreview(
                comparisons,
                unresolvedReferenceCount,
                warnings));
        }
        catch (ObjectDisposedException exception)
        {
            return PreviewFailure(
                new EngineError(EngineErrorCode.WorkspaceDisposed, exception.Message),
                Array.Empty<EngineWarning>());
        }
        catch (Exception exception)
        {
            return PreviewFailure(
                new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Skyrim rejected the FormList preview: {exception.Message}"),
                Array.Empty<EngineWarning>());
        }
    }

    /// <summary>Creates the exact staged-output context for one current FormList.</summary>
    /// <param name="sources">The source set establishing output load-order position.</param>
    /// <param name="output">The selected complete plugin output state.</param>
    /// <param name="record">The current staged FormList record.</param>
    /// <returns>The resolved or deleted exact output context.</returns>
    private static FormListContext CreateOutputContext(
        SkyrimPluginSourceSet sources,
        SkyrimPluginOutputState output,
        IFormListGetter record)
    {
        return new FormListContext(
            new ReferenceRequest(record.FormKey, RecordScope.StagedOutput, output.Association.ModKey),
            record.IsDeleted ? ReferenceResolutionStatus.Deleted : ReferenceResolutionStatus.Resolved,
            output.Association.ModKey,
            output.Association.PluginPath,
            sources.GetMutagenMods().Count,
            PluginRole.Output);
    }

    /// <summary>Compares every field of two exact immutable record contexts.</summary>
    /// <param name="expected">The context captured when editing began.</param>
    /// <param name="actual">The context selected from the same immutable source baseline.</param>
    /// <returns><see langword="true"/> when selection, status, and containing-plugin provenance remain identical.</returns>
    private static bool ContextsMatch(FormListContext expected, FormListContext actual)
    {
        return expected.Selection.FormKey == actual.Selection.FormKey
            && expected.Selection.Scope == actual.Selection.Scope
            && expected.Selection.ContainingModKey == actual.Selection.ContainingModKey
            && expected.Status == actual.Status
            && expected.ContainingModKey == actual.ContainingModKey
            && string.Equals(expected.Path, actual.Path, StringComparison.Ordinal)
            && expected.LoadOrderIndex == actual.LoadOrderIndex
            && expected.Role == actual.Role;
    }

    /// <summary>Writes one detached complete Skyrim FormList to an independently owned JSON value.</summary>
    /// <param name="record">The detached FormList getter.</param>
    /// <returns>The cloned complete typed JSON record view.</returns>
    private JsonElement CreateJsonView(IFormListGetter record)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            _inspector.WriteFields(record, writer, CancellationToken.None, context: null);
            writer.Flush();
        }

        using var document = JsonDocument.Parse(stream.GetBuffer().AsMemory(0, checked((int)stream.Length)));
        return document.RootElement.Clone();
    }

    /// <summary>Creates a failed preview while preserving already observed warnings.</summary>
    /// <param name="error">The typed preview failure.</param>
    /// <param name="warnings">Warnings observed before the failure.</param>
    /// <returns>The failed immutable preview result.</returns>
    private static EngineResult<WorkspacePreview> PreviewFailure(
        EngineError error,
        IReadOnlyList<EngineWarning> warnings)
    {
        return EngineResult<WorkspacePreview>.Failure(error, warnings: warnings);
    }
}
