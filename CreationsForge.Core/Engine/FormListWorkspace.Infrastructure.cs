using CreationsForge.Core.Engine.Contracts;
using System.Text.Json;

namespace CreationsForge.Core.Engine;

/// <summary>
/// Contains serialized reads, output publication, replay support, and mutation guards for the workspace.
/// </summary>
public sealed partial class FormListWorkspace
{
    /// <summary>Adapts a contextual engine read to the legacy detached-getter workspace response.</summary>
    /// <param name="request">The exact FormList identity and scope to read.</param>
    /// <param name="cancellationToken">A token observed during engine selection and copying.</param>
    /// <returns>The detached resolved getter or a typed non-resolved failure.</returns>
    private EngineResult<Mutagen.Bethesda.Plugins.Records.IMajorRecordGetter> ReadLegacyFormList(
        ReferenceRequest request,
        CancellationToken cancellationToken)
    {
        var readResult = Adapter.ReadFormListContext(Sources!, Output, request, cancellationToken);
        if (!readResult.Succeeded || readResult.Value is null)
        {
            return EngineResult<Mutagen.Bethesda.Plugins.Records.IMajorRecordGetter>.Failure(
                readResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The engine adapter returned an invalid contextual FormList read."),
                warnings: readResult.Warnings);
        }

        var read = readResult.Value;
        if (read.Context.Status == ReferenceResolutionStatus.Resolved && read.Record is not null)
        {
            return EngineResult<Mutagen.Bethesda.Plugins.Records.IMajorRecordGetter>.Success(
                read.Record,
                warnings: readResult.Warnings);
        }

        return EngineResult<Mutagen.Bethesda.Plugins.Records.IMajorRecordGetter>.Failure(
            CreateReadStatusError(read.Context.Status),
            warnings: readResult.Warnings);
    }

    /// <summary>Creates a detached typed JSON response from one resolved or deleted contextual FormList read.</summary>
    /// <param name="request">The exact FormList identity, scope, and optional containing-plugin selection.</param>
    /// <param name="cancellationToken">A token observed during engine selection, copying, and typed JSON traversal.</param>
    /// <returns>The contextual detached read view or a typed adapter failure.</returns>
    private EngineResult<FormListReadView> CreateReadView(
        ReferenceRequest request,
        CancellationToken cancellationToken)
    {
        var readResult = Adapter.ReadFormListContext(Sources!, Output, request, cancellationToken);
        if (!readResult.Succeeded || readResult.Value is null)
        {
            return EngineResult<FormListReadView>.Failure(
                readResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The engine adapter returned an invalid contextual FormList read."),
                warnings: readResult.Warnings);
        }

        var read = readResult.Value;
        JsonElement? record = read.Context.Status is ReferenceResolutionStatus.Resolved or ReferenceResolutionStatus.Deleted
            ? WriteReadView(read.Record!, cancellationToken)
            : null;
        return EngineResult<FormListReadView>.Success(
            new FormListReadView(read.Context, record),
            warnings: readResult.Warnings);
    }

    /// <summary>Reads, inspects, and compares two explicit record contexts inside the current workspace operation gate.</summary>
    /// <param name="request">The explicit prior and resulting selections for one FormList.</param>
    /// <param name="cancellationToken">A token observed throughout both reads, JSON traversal, and typed comparison.</param>
    /// <returns>The detached comparison or a typed contextual-read failure.</returns>
    private EngineResult<FormListComparison> CreateComparison(
        CompareFormListRequest request,
        CancellationToken cancellationToken)
    {
        var beforeResult = Adapter.ReadFormListContext(Sources!, Output, request.Before, cancellationToken);
        if (!beforeResult.Succeeded || beforeResult.Value is null)
        {
            return EngineResult<FormListComparison>.Failure(
                beforeResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The engine adapter returned an invalid prior FormList context."),
                warnings: beforeResult.Warnings);
        }

        var beforeContextError = CreateComparisonContextError(beforeResult.Value.Context, "prior");
        if (beforeContextError is not null)
        {
            return EngineResult<FormListComparison>.Failure(
                beforeContextError,
                warnings: beforeResult.Warnings);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var afterResult = Adapter.ReadFormListContext(Sources!, Output, request.After, cancellationToken);
        var warnings = CombineWarnings(beforeResult.Warnings, afterResult.Warnings);
        if (!afterResult.Succeeded || afterResult.Value is null)
        {
            return EngineResult<FormListComparison>.Failure(
                afterResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The engine adapter returned an invalid resulting FormList context."),
                warnings: warnings);
        }

        var afterContextError = CreateComparisonContextError(afterResult.Value.Context, "resulting");
        if (afterContextError is not null)
        {
            return EngineResult<FormListComparison>.Failure(
                afterContextError,
                warnings: warnings);
        }

        var before = beforeResult.Value;
        var after = afterResult.Value;
        var changes = Adapter.Inspector.Compare(before.Record, after.Record, cancellationToken);
        ArgumentNullException.ThrowIfNull(changes);
        JsonElement? beforeView = before.Record is null ? null : WriteReadView(before.Record, cancellationToken);
        JsonElement? afterView = after.Record is null ? null : WriteReadView(after.Record, cancellationToken);
        var comparison = new FormListComparison(
            before.Context,
            after.Context,
            beforeView,
            afterView,
            changes,
            warnings);
        return EngineResult<FormListComparison>.Success(comparison, warnings: warnings);
    }

    /// <summary>Rejects uncertain or uninspectable comparison contexts without treating them as record absence.</summary>
    /// <param name="context">The explicit record context and selection outcome to validate.</param>
    /// <param name="sideName">The human-readable comparison side used in diagnostics.</param>
    /// <returns>A typed failure for an invalid semantic side, or <see langword="null"/> for resolved, deleted, or confirmed unresolved contexts.</returns>
    private static EngineError? CreateComparisonContextError(FormListContext context, string sideName)
    {
        return context.Status switch
        {
            ReferenceResolutionStatus.Resolved or
            ReferenceResolutionStatus.Deleted or
            ReferenceResolutionStatus.Unresolved => null,
            ReferenceResolutionStatus.Ambiguous => new EngineError(
                EngineErrorCode.InvalidRequest,
                $"The {sideName} FormList comparison context for {DescribeSelection(context.Selection)} is ambiguous; specify an exact containing plugin."),
            ReferenceResolutionStatus.Unsupported or ReferenceResolutionStatus.UnknownFamily => new EngineError(
                EngineErrorCode.UnsupportedOperation,
                $"The {sideName} FormList comparison context for {DescribeSelection(context.Selection)} has status {context.Status} and cannot be inspected as a FormList."),
            _ => new EngineError(
                EngineErrorCode.UnexpectedFailure,
                $"The {sideName} FormList comparison context for {DescribeSelection(context.Selection)} has unsupported status {context.Status}.")
        };
    }

    /// <summary>Formats the exact comparison selection without discarding scope or containing-plugin context.</summary>
    /// <param name="selection">The record identity and context selection.</param>
    /// <returns>A stable diagnostic containing the FormKey, scope, and optional containing plugin.</returns>
    private static string DescribeSelection(ReferenceRequest selection)
    {
        var containingPlugin = selection.ContainingModKey.HasValue
            ? $", containing plugin {selection.ContainingModKey.Value}"
            : string.Empty;
        return $"FormKey {selection.FormKey}, scope {selection.Scope}{containingPlugin}";
    }

    /// <summary>Writes one complete typed record into a detached JSON element.</summary>
    /// <param name="record">The detached FormList getter to inspect.</param>
    /// <param name="cancellationToken">A token observed while writing and parsing the transient response.</param>
    /// <returns>An independently owned clone of the complete typed JSON value.</returns>
    private JsonElement WriteReadView(
        Mutagen.Bethesda.Plugins.Records.IMajorRecordGetter record,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            Adapter.Inspector.WriteReadView(record, writer, cancellationToken);
            writer.Flush();
        }

        cancellationToken.ThrowIfCancellationRequested();
        using var document = JsonDocument.Parse(stream.GetBuffer().AsMemory(0, checked((int)stream.Length)));
        return document.RootElement.Clone();
    }

    /// <summary>Maps a non-resolved contextual read outcome to the legacy typed failure contract.</summary>
    /// <param name="status">The contextual engine read outcome.</param>
    /// <returns>The stable legacy read failure.</returns>
    private static EngineError CreateReadStatusError(ReferenceResolutionStatus status)
    {
        return status switch
        {
            ReferenceResolutionStatus.Unresolved or ReferenceResolutionStatus.Deleted =>
                new EngineError(EngineErrorCode.RecordNotFound, "The selected FormList context is absent or deleted."),
            ReferenceResolutionStatus.Ambiguous =>
                new EngineError(EngineErrorCode.InvalidRequest, "The FormList selection is ambiguous; specify a containing plugin."),
            ReferenceResolutionStatus.Unsupported or ReferenceResolutionStatus.UnknownFamily =>
                new EngineError(EngineErrorCode.UnsupportedOperation, "The selected record family cannot be read as a FormList."),
            _ => new EngineError(EngineErrorCode.UnexpectedFailure, "The contextual engine read did not provide a resolved FormList record.")
        };
    }

    /// <summary>Executes one serialized read and attaches live workspace context to its result.</summary>
    private async ValueTask<EngineResult<T>> ExecuteReadAsync<T>(
        Func<EngineResult<T>> operation,
        CancellationToken cancellationToken)
    {
        await OperationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (Disposed)
            {
                return EngineResult<T>.Failure(
                    new EngineError(EngineErrorCode.WorkspaceDisposed, "The workspace has already been disposed."),
                    WorkspaceId,
                    resultRevision: CurrentRevision);
            }

            var synchronizationFailure = GetOutputSynchronizationFailure();
            if (synchronizationFailure is not null)
            {
                return EngineResult<T>.Failure(
                    synchronizationFailure,
                    WorkspaceId,
                    baseRevision: CurrentRevision,
                    resultRevision: CurrentRevision);
            }

            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var result = await Task.Run(operation).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                return Contextualize(result);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Plugin read failed in workspace {WorkspaceId}", WorkspaceId);
                return EngineResult<T>.Failure(
                    new EngineError(EngineErrorCode.UnexpectedFailure, "The engine read operation failed."),
                    WorkspaceId,
                    baseRevision: CurrentRevision,
                    resultRevision: CurrentRevision);
            }
        }
        finally
        {
            OperationGate.Release();
        }
    }

    /// <summary>Reopens selected output state and publishes it only after full engine success.</summary>
    /// <param name="request">The idempotent reopen request guarded by the current revision and baseline.</param>
    /// <param name="cancellationToken">A token that cancels before reopened state is published.</param>
    /// <returns>The reopened output receipt, or a typed failure that preserves current state.</returns>
    private async ValueTask<EngineResult<OutputSelectionReceipt>> ReplaceWithReopenedOutputAsync(
        ReopenOutputRequest request,
        CancellationToken cancellationToken)
    {
        var fingerprint = FingerprintFactory.Create(WorkspaceId, request);
        await OperationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (TryReplay(request.OperationId, fingerprint, out EngineResult<OutputSelectionReceipt>? replay, out var conflict, out var expired))
            {
                return replay!;
            }

            if (expired)
            {
                return CreateOperationReplayExpiredFailure<OutputSelectionReceipt>(request.OperationId, request.ExpectedRevision);
            }

            if (conflict)
            {
                return CreateReuseFailure<OutputSelectionReceipt>(request.OperationId, request.ExpectedRevision);
            }

            if (!CanStoreFinalizationOperation(request.OperationId))
            {
                return CreateOperationCapacityFailure<OutputSelectionReceipt>(request.OperationId, request.ExpectedRevision);
            }

            var guardFailure = ValidateOutputMutation(request.OperationId, request.ExpectedRevision, request.ExpectedBaseline);
            if (guardFailure is not null)
            {
                return StoreFinalization(request.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Failure(
                    guardFailure,
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision));
            }

            var leaseResult = await AcquireOutputDirectoryLeaseAsync(
                SelectedOutput!,
                cancellationToken).ConfigureAwait(false);
            if (!leaseResult.Succeeded || leaseResult.Value is null)
            {
                return StoreFinalization(request.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Failure(
                    leaseResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The output-directory lease provider returned no lease."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision,
                    leaseResult.Warnings));
            }

            await using var outputLease = leaseResult.Value;
            var admissionResult = await InspectOutputAdmissionAsync(
                outputLease,
                SelectedOutput!,
                cancellationToken).ConfigureAwait(false);
            var admissionWarnings = CombineWarnings(leaseResult.Warnings, admissionResult.Warnings);
            if (!admissionResult.Succeeded || admissionResult.Value is null)
            {
                return StoreFinalization(request.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Failure(
                    admissionResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "Output admission returned no result."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision,
                    admissionWarnings));
            }

            if (admissionResult.Value.Status == OutputSynchronizationStatus.RecoveryRequired)
            {
                SetOutputSynchronization(OutputSynchronizationStatus.RecoveryRequired, admissionResult.Value.UnresolvedSave!);
                return StoreFinalization(request.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Failure(
                    new EngineError(EngineErrorCode.RepairRequired, "The selected output has an unresolved save journal that must be recovered before reopening."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision,
                    admissionWarnings));
            }

            var openResult = await Task.Run(
                async () => await Adapter.ReopenOutputAsync(
                    Sources!,
                    SelectedOutput!,
                    SelectedOutputBaseline!,
                    cancellationToken).ConfigureAwait(false))
                .ConfigureAwait(false);
            if (!openResult.Succeeded || openResult.Value is null)
            {
                return StoreFinalization(request.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Failure(
                    openResult.Error ?? new EngineError(EngineErrorCode.OutputOpenFailed, "The selected plugin output could not be reopened."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision,
                    CombineWarnings(admissionWarnings, openResult.Warnings)));
            }

            await DisposeOpenedOutputIfCanceledAsync(openResult.Value, cancellationToken).ConfigureAwait(false);
            var baseRevision = CurrentRevision;
            var resultRevision = CreateOutputRevision(openResult.Value.Association, openResult.Value.Baseline);
            var disposalWarnings = await PublishOutputAsync(openResult.Value, true, resultRevision).ConfigureAwait(false);
            var receipt = new OutputSelectionReceipt(SelectedOutput!, SelectedOutputBaseline!, resultRevision);
            return StoreFinalization(request.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Success(
                receipt,
                WorkspaceId,
                request.OperationId,
                baseRevision,
                resultRevision,
                CombineWarnings(
                    CombineWarnings(admissionWarnings, openResult.Warnings),
                    disposalWarnings)));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Failed to reopen plugin output in workspace {WorkspaceId} for operation {OperationId}", WorkspaceId, request.OperationId);
            return StoreFinalization(request.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Failure(
                new EngineError(EngineErrorCode.UnexpectedFailure, "The selected plugin output could not be reopened."),
                WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                CurrentRevision));
        }
        finally
        {
            OperationGate.Release();
        }
    }

    /// <summary>Validates common mutation state.</summary>
    /// <param name="operationId">The required idempotency identifier.</param>
    /// <param name="expectedRevision">The exact caller-observed revision.</param>
    /// <returns>A typed guard failure, or <see langword="null"/> when mutation may proceed.</returns>
    private EngineError? ValidateMutation(Guid operationId, WorkspaceRevision expectedRevision)
    {
        if (Disposed)
        {
            return new EngineError(EngineErrorCode.WorkspaceDisposed, "The workspace has already been disposed.");
        }

        var synchronizationFailure = GetOutputSynchronizationFailure();
        if (synchronizationFailure is not null)
        {
            return synchronizationFailure;
        }

        if (operationId == Guid.Empty)
        {
            return new EngineError(EngineErrorCode.InvalidRequest, "A mutation requires a non-empty operation identifier.");
        }

        if (expectedRevision != CurrentRevision)
        {
            return new EngineError(EngineErrorCode.RevisionConflict, "The workspace revision differs from the caller's expected revision.");
        }

        return null;
    }

    /// <summary>Validates common mutation state and the selected output baseline.</summary>
    /// <param name="operationId">The required idempotency identifier.</param>
    /// <param name="expectedRevision">The exact caller-observed revision.</param>
    /// <param name="expectedBaseline">The exact caller-observed output artifact baseline.</param>
    /// <returns>A typed guard failure, or <see langword="null"/> when output mutation may proceed.</returns>
    private EngineError? ValidateOutputMutation(
        Guid operationId,
        WorkspaceRevision expectedRevision,
        OutputArtifactSetBaseline expectedBaseline)
    {
        var mutationFailure = ValidateMutation(operationId, expectedRevision);
        if (mutationFailure is not null)
        {
            return mutationFailure;
        }

        if (Output is null || SelectedOutput is null || SelectedOutputBaseline is null)
        {
            return new EngineError(EngineErrorCode.OutputNotSelected, "Select an output before running this operation.");
        }

        if (!BaselinesMatch(expectedBaseline, SelectedOutputBaseline))
        {
            return new EngineError(EngineErrorCode.ExternalChangeDetected, "The selected output baseline differs from the caller's expected baseline.");
        }

        return null;
    }

    /// <summary>Publishes a newly opened output and disposes the superseded plugin output.</summary>
    /// <param name="openResult">The complete newly opened engine state, association, and baseline.</param>
    /// <param name="clearEdits">Whether staged edit identities must be invalidated.</param>
    /// <param name="revision">The revision to publish atomically with the new state.</param>
    /// <returns>Warnings reported while disposing superseded engine state.</returns>
    private async ValueTask<IReadOnlyList<EngineWarning>> PublishOutputAsync(
        PluginOutputOpenResult openResult,
        bool clearEdits,
        WorkspaceRevision revision)
    {
        var oldOutput = Output;
        Output = openResult.Output;
        SelectedOutput = openResult.Association;
        SelectedOutputBaseline = openResult.Baseline;
        if (clearEdits)
        {
            Edits.Clear();
        }

        SetRevision(revision);
        return await DisposeSupersededOutputAsync(oldOutput).ConfigureAwait(false);
    }

    /// <summary>Publishes a mutated candidate while preserving output association and baseline identity.</summary>
    /// <param name="candidate">The complete unpublished candidate that becomes live state.</param>
    /// <param name="revision">The revision to publish atomically with the candidate.</param>
    /// <returns>Warnings reported while disposing superseded engine state.</returns>
    private async ValueTask<IReadOnlyList<EngineWarning>> PublishCandidateAsync(
        IPluginOutputState candidate,
        WorkspaceRevision revision)
    {
        var oldOutput = Output;
        Output = candidate;
        SetRevision(revision);
        return await DisposeSupersededOutputAsync(oldOutput).ConfigureAwait(false);
    }

    /// <summary>Disposes superseded plugin output state and converts disposal failure into a visible warning.</summary>
    /// <param name="oldOutput">The previously published plugin output, or <see langword="null"/>.</param>
    /// <returns>An immutable warning collection describing any disposal failure.</returns>
    private async ValueTask<IReadOnlyList<EngineWarning>> DisposeSupersededOutputAsync(IPluginOutputState? oldOutput)
    {
        if (oldOutput is null)
        {
            return Array.Empty<EngineWarning>();
        }

        try
        {
            await oldOutput.DisposeAsync().ConfigureAwait(false);
            return Array.Empty<EngineWarning>();
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Failed to dispose superseded plugin output in workspace {WorkspaceId}", WorkspaceId);
            return Array.AsReadOnly(new[]
            {
                new EngineWarning("engine-output-disposal-failed", "The new state was published, but disposal of superseded plugin output reported a failure.")
            });
        }
    }

    /// <summary>Attempts candidate cleanup without replacing the primary operation failure.</summary>
    /// <param name="candidate">The unpublished engine candidate to dispose.</param>
    /// <param name="primaryFailure">The failure whose outcome cleanup must not replace.</param>
    /// <returns>A task that completes after cleanup succeeds or its failure is logged.</returns>
    private async ValueTask DisposeCandidateAfterFailureAsync(IPluginOutputState candidate, Exception primaryFailure)
    {
        try
        {
            await candidate.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception disposalFailure)
        {
            Logger.Error(disposalFailure, "Failed to dispose rejected plugin output candidate in workspace {WorkspaceId} after {FailureType}", WorkspaceId, primaryFailure.GetType().Name);
        }
    }

    /// <summary>Disposes a newly acquired output before propagating cancellation observed after engine open.</summary>
    /// <param name="openResult">The newly acquired unpublished plugin output.</param>
    /// <param name="cancellationToken">The token checked after acquisition.</param>
    /// <returns>A task that completes immediately when active, or after canceled output cleanup is attempted.</returns>
    /// <exception cref="OperationCanceledException">Thrown after unpublished output cleanup is attempted when cancellation was requested.</exception>
    private async ValueTask DisposeOpenedOutputIfCanceledAsync(
        PluginOutputOpenResult openResult,
        CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            return;
        }

        try
        {
            await openResult.Output.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Failed to dispose canceled unpublished plugin output in workspace {WorkspaceId}", WorkspaceId);
        }

        cancellationToken.ThrowIfCancellationRequested();
    }

    /// <summary>Attaches the live workspace and unchanged revision to an adapter read result.</summary>
    private EngineResult<T> Contextualize<T>(EngineResult<T> result)
    {
        if (result.Succeeded && result.Value is not null)
        {
            return EngineResult<T>.Success(
                result.Value,
                WorkspaceId,
                baseRevision: CurrentRevision,
                resultRevision: CurrentRevision,
                warnings: result.Warnings);
        }

        return EngineResult<T>.Failure(
            result.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The engine adapter returned an invalid failed result."),
            WorkspaceId,
            baseRevision: CurrentRevision,
            resultRevision: CurrentRevision,
            warnings: result.Warnings);
    }

    /// <summary>Gets an exact replay or conflicting reuse state.</summary>
    /// <typeparam name="T">The immutable result type expected by the operation.</typeparam>
    /// <param name="operationId">The operation identifier.</param>
    /// <param name="fingerprint">The complete canonical request fingerprint.</param>
    /// <param name="result">The exact retained result when available.</param>
    /// <param name="conflict">Whether the identifier belongs to another payload or result type.</param>
    /// <param name="expired">Whether the exact finalization replay left the bounded result window.</param>
    /// <returns><see langword="true"/> when an exact retained result is available.</returns>
    private bool TryReplay<T>(
        Guid operationId,
        OperationFingerprint fingerprint,
        out T? result,
        out bool conflict,
        out bool expired)
        where T : class
    {
        return ReplayStore.TryGet(operationId, fingerprint, out result, out conflict, out expired);
    }

    /// <summary>Stores the first result for an operation and returns it.</summary>
    private T Store<T>(Guid operationId, OperationFingerprint fingerprint, T result)
        where T : class
    {
        ReplayStore.Store(operationId, fingerprint, result);
        return result;
    }

    /// <summary>Creates a stable failure before a fresh operation would exceed retained replay capacity.</summary>
    /// <typeparam name="T">The result value type requested by the operation.</typeparam>
    /// <param name="operationId">The fresh operation identifier.</param>
    /// <param name="expectedRevision">The caller's expected workspace revision.</param>
    /// <returns>A typed capacity failure with the unchanged workspace revision.</returns>
    private EngineResult<T> CreateOperationCapacityFailure<T>(Guid operationId, WorkspaceRevision expectedRevision)
    {
        return EngineResult<T>.Failure(
            new EngineError(EngineErrorCode.OperationCapacityExceeded, "The workspace operation replay capacity has been reached. Close and reopen the workspace before issuing another mutation."),
            WorkspaceId,
            operationId,
            expectedRevision,
            CurrentRevision);
    }

    /// <summary>Creates a stable failure when an exact finalization result has left the bounded replay window.</summary>
    /// <typeparam name="T">The result value type requested by the operation.</typeparam>
    /// <param name="operationId">The expired operation identifier.</param>
    /// <param name="expectedRevision">The caller's expected workspace revision.</param>
    /// <returns>A typed replay-expiration failure with the unchanged workspace revision.</returns>
    private EngineResult<T> CreateOperationReplayExpiredFailure<T>(Guid operationId, WorkspaceRevision expectedRevision)
    {
        return EngineResult<T>.Failure(
            new EngineError(EngineErrorCode.OperationReplayExpired, "The exact finalization result has expired from the bounded workspace replay window. Use a new operation identifier after refreshing workspace state."),
            WorkspaceId,
            operationId,
            expectedRevision,
            CurrentRevision);
    }

    /// <summary>Determines whether a fresh operation can reserve replay capacity before engine side effects.</summary>
    /// <param name="operationId">The fresh operation identifier.</param>
    /// <returns><see langword="true"/> when the result can be retained.</returns>
    private bool CanStoreOperation(Guid operationId)
    {
        return ReplayStore.CanStore(operationId, reservedEntryCount: 2);
    }

    /// <summary>Determines whether a save, reset, or recovery result can use bounded evictable finalization capacity.</summary>
    /// <param name="operationId">The finalization operation identifier.</param>
    /// <returns><see langword="true"/> when finalization capacity remains available.</returns>
    private bool CanStoreFinalizationOperation(Guid operationId)
    {
        return ReplayStore.CanStoreFinalization(operationId);
    }

    /// <summary>Retains a finalization result while permitting older finalization results to yield bounded retry capacity.</summary>
    /// <typeparam name="T">The immutable finalization result type.</typeparam>
    /// <param name="operationId">The finalization operation identifier.</param>
    /// <param name="fingerprint">The complete canonical request fingerprint.</param>
    /// <param name="result">The result to replay while retained.</param>
    /// <returns>The supplied immutable result.</returns>
    private T StoreFinalization<T>(Guid operationId, OperationFingerprint fingerprint, T result)
        where T : class
    {
        ReplayStore.StoreFinalization(operationId, fingerprint, result);
        return result;
    }

    /// <summary>Creates a stable conflicting-operation-identifier failure.</summary>
    private EngineResult<T> CreateReuseFailure<T>(Guid operationId, WorkspaceRevision expectedRevision)
    {
        return EngineResult<T>.Failure(
            new EngineError(EngineErrorCode.OperationIdReuse, "The operation identifier was already used with a different canonical request payload."),
            WorkspaceId,
            operationId,
            expectedRevision,
            CurrentRevision);
    }

    /// <summary>Creates a save failure known to occur before a destination mutation.</summary>
    /// <param name="request">The guarded save request that failed.</param>
    /// <param name="code">The typed pre-commit failure code.</param>
    /// <param name="message">The safe caller-facing failure detail.</param>
    /// <returns>A not-committed result with the unchanged workspace revision.</returns>
    private SaveResult CreateSaveFailure(SaveRequest request, EngineErrorCode code, string message)
    {
        return new SaveResult(
            WorkspaceId,
            request.OperationId,
            request.ExpectedRevision,
            CurrentRevision,
            SaveCommitStatus.NotCommitted,
            null,
            null,
            null,
            new EngineError(code, message),
            Array.Empty<EngineWarning>());
    }

    /// <summary>Sets the current revision under the synchronous revision lock.</summary>
    /// <param name="revision">The exact baseline and mutation sequence to publish.</param>
    private void SetRevision(WorkspaceRevision revision)
    {
        lock (RevisionSync)
        {
            CurrentRevision = revision;
        }
    }

    /// <summary>Creates the next revision from the immutable source baseline and a complete output observation.</summary>
    /// <param name="association">The selected canonical output identity and mode.</param>
    /// <param name="baseline">The complete selected output artifact-set observation.</param>
    /// <returns>The composed baseline identity with the next monotonic workspace sequence.</returns>
    /// <exception cref="OverflowException">Thrown when the workspace sequence has reached <see cref="ulong.MaxValue"/>.</exception>
    private WorkspaceRevision CreateOutputRevision(
        OutputAssociation association,
        OutputArtifactSetBaseline baseline)
    {
        var baselineId = FingerprintFactory.CreateRevisionBaseline(SourceBaselineId, association, baseline);
        return new WorkspaceRevision(baselineId, checked(CurrentRevision.Sequence + 1));
    }

    /// <summary>Determines whether a canonical path aliases any explicit source or load-order plugin path.</summary>
    /// <param name="canonicalPath">The canonical output path to compare.</param>
    /// <returns><see langword="true"/> when the path matches an explicit source or load-order input under platform path rules.</returns>
    private bool IsSourcePath(string canonicalPath)
    {
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        return Request.LoadOrderPluginPaths.Any(path => string.Equals(path, canonicalPath, comparison));
    }

    /// <summary>Compares every immutable field of two ordered complete artifact baselines.</summary>
    /// <param name="expected">The caller-observed baseline.</param>
    /// <param name="actual">The workspace-owned baseline.</param>
    /// <returns><see langword="true"/> when both values describe the same exact engine file-set observation.</returns>
    private static bool BaselinesMatch(OutputArtifactSetBaseline expected, OutputArtifactSetBaseline actual)
    {
        return expected.BaselineId == actual.BaselineId
            && ArtifactCollectionsMatch(expected.Artifacts, actual.Artifacts);
    }

    /// <summary>Combines immutable warning collections.</summary>
    /// <param name="first">The first ordered warning collection.</param>
    /// <param name="second">The second ordered warning collection.</param>
    /// <returns>An immutable collection preserving the input order.</returns>
    private static IReadOnlyList<EngineWarning> CombineWarnings(
        IReadOnlyList<EngineWarning> first,
        IReadOnlyList<EngineWarning> second)
    {
        if (first.Count == 0 && second.Count == 0)
        {
            return Array.Empty<EngineWarning>();
        }

        return Array.AsReadOnly(first.Concat(second).ToArray());
    }
}
