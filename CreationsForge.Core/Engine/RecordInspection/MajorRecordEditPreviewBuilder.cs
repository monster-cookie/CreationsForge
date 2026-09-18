using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Core.Engine.RecordInspection;

/// <summary>Reconstructs exact native before-and-after previews for staged non-FormList edits.</summary>
public static class MajorRecordEditPreviewBuilder
{
    /// <summary>Builds complete comparisons from retained first-edit provenance and native records.</summary>
    /// <param name="entries">The ordered staged edit provenance.</param>
    /// <param name="sourceBaselineId">The immutable source set identity.</param>
    /// <param name="association">The selected output identity and path.</param>
    /// <param name="outputIndex">The output position following all immutable source plugins.</param>
    /// <param name="inspector">The game's complete native record inspector.</param>
    /// <param name="readOriginal">Selects one record from the selection-time output baseline.</param>
    /// <param name="readCurrent">Selects one record from the staged candidate.</param>
    /// <param name="readSource">Reads one exact source plugin context.</param>
    /// <param name="cancellationToken">A token observed during record selection and traversal.</param>
    /// <returns>One complete comparison per non-FormList edit or a typed exact-context failure.</returns>
    public static EngineResult<IReadOnlyList<MajorRecordComparison>> Build(
        IReadOnlyList<RecordEditProvenance> entries,
        Guid sourceBaselineId,
        OutputAssociation association,
        int outputIndex,
        IMajorRecordInspector inspector,
        Func<FormKey, IMajorRecordGetter?> readOriginal,
        Func<FormKey, IMajorRecordGetter?> readCurrent,
        Func<ReferenceRequest, EngineResult<RecordRead>> readSource,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(association);
        ArgumentNullException.ThrowIfNull(inspector);
        ArgumentNullException.ThrowIfNull(readOriginal);
        ArgumentNullException.ThrowIfNull(readCurrent);
        ArgumentNullException.ThrowIfNull(readSource);
        var comparisons = new List<MajorRecordComparison>();
        foreach (var entry in entries)
        {
            if (entry.RecordType == "FormList")
            {
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (entry.RecordType != "GameSettingFloat")
            {
                return Failure($"The staged record family '{entry.RecordType}' has no native preview support.");
            }

            var after = readCurrent(entry.TargetFormKey);
            if (after is null || after.GetType().Name != entry.RecordType)
            {
                return Failure($"The staged {entry.RecordType} '{entry.TargetFormKey}' is missing or belongs to another native family.");
            }

            IMajorRecordGetter? before = null;
            FormListContext beforeContext;
            if (entry.BaselineKind == EditBaselineKind.Absent)
            {
                beforeContext = new FormListContext(
                    new ReferenceRequest(entry.TargetFormKey, RecordScope.StagedOutput, association.ModKey),
                    ReferenceResolutionStatus.Unresolved,
                    null, null, null, null);
            }
            else if (entry.BaselineKind == EditBaselineKind.OriginalOutput)
            {
                before = readOriginal(entry.TargetFormKey);
                beforeContext = entry.BaselineContext!;
                if (before is null || before.GetType().Name != entry.RecordType ||
                    !MatchesOutputContext(beforeContext, before, association, outputIndex))
                {
                    return Failure($"The original output context for {entry.RecordType} '{entry.TargetFormKey}' no longer matches its first-edit baseline.");
                }
            }
            else if (entry.BaselineKind == EditBaselineKind.SourceContext)
            {
                if (entry.SourceBaselineId != sourceBaselineId)
                {
                    return Failure($"The source baseline for {entry.RecordType} '{entry.TargetFormKey}' changed.", EngineErrorCode.ExternalChangeDetected);
                }

                beforeContext = entry.BaselineContext!;
                var sourceResult = readSource(beforeContext.Selection);
                if (!sourceResult.Succeeded || sourceResult.Value is null ||
                    !MatchesContext(beforeContext, sourceResult.Value.Context) ||
                    sourceResult.Value.RecordType != entry.RecordType ||
                    sourceResult.Value.Record is null)
                {
                    return Failure($"The exact source context for {entry.RecordType} '{entry.TargetFormKey}' is no longer available.", EngineErrorCode.ExternalChangeDetected);
                }

                before = sourceResult.Value.Record;
            }
            else
            {
                return Failure($"The edit baseline kind '{entry.BaselineKind}' is unsupported.");
            }

            var afterContext = new FormListContext(
                new ReferenceRequest(entry.TargetFormKey, RecordScope.StagedOutput, association.ModKey),
                after.IsDeleted ? ReferenceResolutionStatus.Deleted : ReferenceResolutionStatus.Resolved,
                association.ModKey,
                association.PluginPath,
                outputIndex,
                PluginRole.Output);
            try
            {
                comparisons.Add(new MajorRecordComparison(
                    beforeContext,
                    afterContext,
                    entry.RecordType,
                    before is null ? null : WriteView(before, inspector, cancellationToken),
                    WriteView(after, inspector, cancellationToken),
                    inspector.Compare(before, after, cancellationToken),
                    Array.Empty<EngineWarning>()));
            }
            catch (NotSupportedException exception)
            {
                return Failure(exception.Message, EngineErrorCode.UnsupportedOperation);
            }
        }

        return EngineResult<IReadOnlyList<MajorRecordComparison>>.Success(comparisons);
    }

    private static EngineResult<IReadOnlyList<MajorRecordComparison>> Failure(string message, EngineErrorCode code = EngineErrorCode.ValidationFailed) =>
        EngineResult<IReadOnlyList<MajorRecordComparison>>.Failure(new EngineError(code, message));

    private static JsonElement WriteView(IMajorRecordGetter record, IMajorRecordInspector inspector, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            inspector.WriteReadView(record, writer, cancellationToken);
            writer.Flush();
        }

        using var document = JsonDocument.Parse(stream.GetBuffer().AsMemory(0, checked((int)stream.Length)));
        return document.RootElement.Clone();
    }

    private static bool MatchesOutputContext(FormListContext expected, IMajorRecordGetter record, OutputAssociation association, int index) =>
        expected.Selection.FormKey == record.FormKey &&
        expected.Status == (record.IsDeleted ? ReferenceResolutionStatus.Deleted : ReferenceResolutionStatus.Resolved) &&
        expected.ContainingModKey == association.ModKey &&
        expected.LoadOrderIndex == index &&
        expected.Role == PluginRole.Output &&
        PathsEqual(expected.Path, association.PluginPath);

    private static bool MatchesContext(FormListContext expected, FormListContext actual) =>
        expected.Selection.FormKey == actual.Selection.FormKey &&
        expected.Selection.Scope == actual.Selection.Scope &&
        expected.Selection.ContainingModKey == actual.Selection.ContainingModKey &&
        expected.Status == actual.Status &&
        expected.ContainingModKey == actual.ContainingModKey &&
        expected.LoadOrderIndex == actual.LoadOrderIndex &&
        expected.Role == actual.Role &&
        PathsEqual(expected.Path, actual.Path);

    private static bool PathsEqual(string? left, string? right) => string.Equals(
        left,
        right,
        OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
}
