using CreationsForge.Core.Engine.Contracts;
using System.Text.Json;

namespace CreationsForge.Core.Engine;

/// <summary>Exposes family-neutral contextual reads through the workspace's serialized plugin lifetime.</summary>
public sealed partial class PluginWorkspace
{
    /// <inheritdoc />
    public ValueTask<EngineResult<RecordRead>> ReadRecordContextAsync(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ExecuteReadAsync(
            () => Adapter.ReadRecordContext(Sources!, Output, request, cancellationToken),
            cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<MajorRecordReadView>> ReadMajorRecordViewAsync(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ExecuteReadAsync(
            () => CreateMajorRecordReadView(request, cancellationToken),
            cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<MajorRecordComparison>> CompareMajorRecordAsync(
        CompareMajorRecordRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ExecuteReadAsync(
            () => CreateMajorRecordComparison(request, cancellationToken),
            cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<MajorRecordListPage>> ListMajorRecordsAsync(
        MajorRecordListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ExecuteReadAsync(
            () =>
            {
                var result = Adapter.SearchReferences(
                    Sources!,
                    Output,
                    request.ToSearchRequest(),
                    WorkspaceId,
                    CurrentRevision,
                    cancellationToken);
                return result.Succeeded && result.Value is not null
                    ? EngineResult<MajorRecordListPage>.Success(
                        new MajorRecordListPage(result.Value.Matches, result.Value.ContinuationToken),
                        warnings: result.Warnings)
                    : EngineResult<MajorRecordListPage>.Failure(
                        result.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The adapter returned an invalid major-record list page."),
                        warnings: result.Warnings);
            },
            cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<IReadOnlyList<string>>> ListMajorRecordTypesAsync(
        CancellationToken cancellationToken = default)
    {
        return ExecuteReadAsync(
            () => EngineResult<IReadOnlyList<string>>.Success(Adapter.MajorRecordInspector.SupportedRecordTypes),
            cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<int>> VisitWinningRecordSummariesAsync(
        Action<ReferenceSearchMatch> onRecord,
        Action<Mutagen.Bethesda.Plugins.ModKey, int>? onProgress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(onRecord);
        return ExecuteReadAsync(
            () => Adapter.VisitWinningRecordSummaries(Sources!, Output, onRecord, onProgress, cancellationToken),
            cancellationToken);
    }

    /// <summary>Creates one complete detached native field tree inside the workspace operation gate.</summary>
    /// <param name="request">The exact record context selection.</param>
    /// <param name="cancellationToken">A token observed during selection and native traversal.</param>
    /// <returns>The complete typed read view or a precise adapter or unsupported-shape failure.</returns>
    private EngineResult<MajorRecordReadView> CreateMajorRecordReadView(
        ReferenceRequest request,
        CancellationToken cancellationToken)
    {
        var readResult = Adapter.ReadRecordContext(Sources!, Output, request, cancellationToken);
        if (!readResult.Succeeded || readResult.Value is null)
        {
            return EngineResult<MajorRecordReadView>.Failure(
                readResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The adapter returned an invalid major-record context read."),
                warnings: readResult.Warnings);
        }

        var read = readResult.Value;
        try
        {
            JsonElement? record = read.Record is null ? null : WriteMajorRecordView(read.Record, cancellationToken);
            return EngineResult<MajorRecordReadView>.Success(
                new MajorRecordReadView(read.Context, read.RecordType, record),
                warnings: readResult.Warnings);
        }
        catch (NotSupportedException exception)
        {
            return EngineResult<MajorRecordReadView>.Failure(
                new EngineError(EngineErrorCode.UnsupportedOperation, exception.Message),
                warnings: readResult.Warnings);
        }
    }

    /// <summary>Reads and compares two exact contexts through native values inside the operation gate.</summary>
    /// <param name="request">The prior and resulting selections for one FormKey.</param>
    /// <param name="cancellationToken">A token observed during both reads and all field traversal.</param>
    /// <returns>The comparison or a precise contextual-read or unsupported-shape failure.</returns>
    private EngineResult<MajorRecordComparison> CreateMajorRecordComparison(
        CompareMajorRecordRequest request,
        CancellationToken cancellationToken)
    {
        var beforeResult = Adapter.ReadRecordContext(Sources!, Output, request.Before, cancellationToken);
        if (!beforeResult.Succeeded || beforeResult.Value is null)
        {
            return EngineResult<MajorRecordComparison>.Failure(
                beforeResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The adapter returned an invalid prior major-record context."),
                warnings: beforeResult.Warnings);
        }

        var beforeError = CreateMajorRecordContextError(beforeResult.Value.Context, "prior");
        if (beforeError is not null)
        {
            return EngineResult<MajorRecordComparison>.Failure(beforeError, warnings: beforeResult.Warnings);
        }

        var afterResult = Adapter.ReadRecordContext(Sources!, Output, request.After, cancellationToken);
        var warnings = CombineWarnings(beforeResult.Warnings, afterResult.Warnings);
        if (!afterResult.Succeeded || afterResult.Value is null)
        {
            return EngineResult<MajorRecordComparison>.Failure(
                afterResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The adapter returned an invalid resulting major-record context."),
                warnings: warnings);
        }

        var afterError = CreateMajorRecordContextError(afterResult.Value.Context, "resulting");
        if (afterError is not null)
        {
            return EngineResult<MajorRecordComparison>.Failure(afterError, warnings: warnings);
        }

        var before = beforeResult.Value;
        var after = afterResult.Value;
        if (before.RecordType is not null && after.RecordType is not null
            && !string.Equals(before.RecordType, after.RecordType, StringComparison.Ordinal))
        {
            return EngineResult<MajorRecordComparison>.Failure(
                new EngineError(EngineErrorCode.InvalidRequest, "The selected contexts resolve to different major-record families."),
                warnings: warnings);
        }

        try
        {
            var changes = Adapter.MajorRecordInspector.Compare(before.Record, after.Record, cancellationToken);
            var comparison = new MajorRecordComparison(
                before.Context,
                after.Context,
                before.RecordType ?? after.RecordType,
                before.Record is null ? null : WriteMajorRecordView(before.Record, cancellationToken),
                after.Record is null ? null : WriteMajorRecordView(after.Record, cancellationToken),
                changes,
                warnings);
            return EngineResult<MajorRecordComparison>.Success(comparison, warnings: warnings);
        }
        catch (NotSupportedException exception)
        {
            return EngineResult<MajorRecordComparison>.Failure(
                new EngineError(EngineErrorCode.UnsupportedOperation, exception.Message),
                warnings: warnings);
        }
    }

    /// <summary>Writes one complete native major-record field tree into an independently owned JSON element.</summary>
    /// <param name="record">The detached native record.</param>
    /// <param name="cancellationToken">A token observed throughout field traversal and parsing.</param>
    /// <returns>The independently owned complete field tree.</returns>
    private JsonElement WriteMajorRecordView(
        Mutagen.Bethesda.Plugins.Records.IMajorRecordGetter record,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            Adapter.MajorRecordInspector.WriteReadView(record, writer, cancellationToken);
            writer.Flush();
        }

        cancellationToken.ThrowIfCancellationRequested();
        using var document = JsonDocument.Parse(stream.GetBuffer().AsMemory(0, checked((int)stream.Length)));
        return document.RootElement.Clone();
    }

    /// <summary>Rejects ambiguous or unsupported major-record contexts while allowing confirmed absence.</summary>
    /// <param name="context">The exact selected context and outcome.</param>
    /// <param name="sideName">The comparison side used in diagnostics.</param>
    /// <returns>A typed failure, or <see langword="null"/> for resolved, deleted, or unresolved contexts.</returns>
    private static EngineError? CreateMajorRecordContextError(FormListContext context, string sideName)
    {
        return context.Status switch
        {
            ReferenceResolutionStatus.Resolved or ReferenceResolutionStatus.Deleted or ReferenceResolutionStatus.Unresolved => null,
            ReferenceResolutionStatus.Ambiguous => new EngineError(
                EngineErrorCode.InvalidRequest,
                $"The {sideName} major-record context is ambiguous; specify an exact containing plugin."),
            ReferenceResolutionStatus.Unsupported or ReferenceResolutionStatus.UnknownFamily => new EngineError(
                EngineErrorCode.UnsupportedOperation,
                $"The {sideName} major-record context has status {context.Status} and cannot be inspected."),
            _ => new EngineError(
                EngineErrorCode.UnexpectedFailure,
                $"The {sideName} major-record context has unsupported status {context.Status}.")
        };
    }
}
