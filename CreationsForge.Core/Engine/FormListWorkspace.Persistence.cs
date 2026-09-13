using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;

namespace CreationsForge.Core.Engine;

/// <summary>
/// Owns output-directory admission and synchronization helpers for one isolated workspace.
/// </summary>
public sealed partial class FormListWorkspace
{
    /// <inheritdoc />
    public async ValueTask<SaveResult> SaveAsync(SaveRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var fingerprint = FingerprintFactory.Create(WorkspaceId, request);
        await OperationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (TryReplay(request.OperationId, fingerprint, out SaveResult? replay, out var conflict, out var expired))
            {
                return replay!;
            }

            if (expired)
            {
                return CreateSaveFailure(request, EngineErrorCode.OperationReplayExpired, "The exact save result has expired from the bounded workspace replay window. Use a new operation identifier after refreshing workspace state.");
            }

            if (conflict)
            {
                return CreateSaveFailure(request, EngineErrorCode.OperationIdReuse, "The operation identifier was already used with a different canonical request payload.");
            }

            if (!CanStoreFinalizationOperation(request.OperationId))
            {
                return CreateSaveFailure(request, EngineErrorCode.OperationCapacityExceeded, "The workspace operation replay capacity has been reached. Close and reopen the workspace before issuing another mutation.");
            }

            var guardFailure = ValidateMutation(request.OperationId, request.ExpectedRevision);
            if (guardFailure is not null)
            {
                return StoreFinalization(request.OperationId, fingerprint, CreateSaveFailure(request, guardFailure.Code, guardFailure.Message));
            }

            if (Output is null || SelectedOutput is null || SelectedOutputBaseline is null)
            {
                return StoreFinalization(request.OperationId, fingerprint, CreateSaveFailure(request, EngineErrorCode.OutputNotSelected, "Select an output before saving staged changes."));
            }

            if (!BaselinesMatch(request.ExpectedOutputBaseline, SelectedOutputBaseline))
            {
                return StoreFinalization(request.OperationId, fingerprint, CreateSaveFailure(request, EngineErrorCode.ExternalChangeDetected, "The selected output baseline differs from the caller's expected baseline."));
            }

            var context = new WorkspaceSaveContext(
                WorkspaceId,
                CurrentRevision,
                Request.Game,
                Request.Release,
                Adapter,
                Sources!,
                Output,
                SelectedOutput,
                SelectedOutputBaseline);
            var pendingSave = new PendingSaveIdentity(
                WorkspaceId,
                request.OperationId,
                CurrentRevision,
                Request.Game,
                Request.Release,
                SelectedOutput);

            SaveResult coordinatorResult;
            try
            {
                coordinatorResult = await Task.Run(
                    async () => await SaveCoordinator.SaveAsync(context, request, cancellationToken).ConfigureAwait(false))
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Save coordinator failed without a definitive result for workspace {WorkspaceId} and operation {OperationId}", WorkspaceId, request.OperationId);
                return StoreFinalization(request.OperationId, fingerprint, CreateUnknownSaveResult(
                    request,
                    pendingSave,
                    "The save coordinator failed without establishing the destination outcome.",
                    null,
                    Array.Empty<EngineWarning>()));
            }

            if (!IsCoordinatorSaveResultConsistent(coordinatorResult, request, pendingSave))
            {
                return StoreFinalization(request.OperationId, fingerprint, CreateUnknownSaveResult(
                    request,
                    pendingSave,
                    "The save coordinator returned an inconsistent status, identity, revision, baseline, or evidence envelope.",
                    coordinatorResult.RecoveryEvidenceToken,
                    coordinatorResult.Warnings));
            }

            if (coordinatorResult.Status == SaveCommitStatus.NotCommitted)
            {
                return StoreFinalization(request.OperationId, fingerprint, coordinatorResult);
            }

            if (coordinatorResult.Status == SaveCommitStatus.CommitOutcomeUnknown)
            {
                return StoreFinalization(request.OperationId, fingerprint, CreateUnknownSaveResult(
                    request,
                    pendingSave,
                    coordinatorResult.Error?.Message ?? "The destination save outcome remains unknown.",
                    coordinatorResult.RecoveryEvidenceToken,
                    coordinatorResult.Warnings));
            }

            var result = await AdoptCommittedSaveAsync(
                request,
                pendingSave,
                coordinatorResult).ConfigureAwait(false);
            return StoreFinalization(request.OperationId, fingerprint, result);
        }
        finally
        {
            OperationGate.Release();
        }
    }

    /// <summary>Acquires the exclusive lease required before opening or publishing an output.</summary>
    /// <param name="output">The canonical output whose containing directory must be protected.</param>
    /// <param name="cancellationToken">A token that cancels lease acquisition.</param>
    /// <returns>The owned lease or a typed acquisition failure.</returns>
    private async ValueTask<EngineResult<IOutputDirectoryLease>> AcquireOutputDirectoryLeaseAsync(
        OutputAssociation output,
        CancellationToken cancellationToken)
    {
        var outputDirectoryPath = Path.GetDirectoryName(output.PluginPath);
        if (string.IsNullOrWhiteSpace(outputDirectoryPath))
        {
            return EngineResult<IOutputDirectoryLease>.Failure(new EngineError(
                EngineErrorCode.InvalidRequest,
                "The output plugin path does not identify a containing directory."));
        }

        try
        {
            var result = await OutputDirectoryLeaseProvider.AcquireAsync(
                outputDirectoryPath,
                OutputDirectoryLeaseMode.CreateOrOpen,
                cancellationToken).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return EngineResult<IOutputDirectoryLease>.Failure(
                    result.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The output-directory lease provider returned an invalid failed result."),
                    warnings: result.Warnings);
            }

            if (result.Value is null
                || result.Value.Status != OutputDirectoryLeaseAcquisitionStatus.Acquired
                || result.Value.Lease is null)
            {
                return EngineResult<IOutputDirectoryLease>.Failure(
                    new EngineError(EngineErrorCode.UnexpectedFailure, "Create-or-open lease acquisition returned no owned lease."),
                    warnings: result.Warnings);
            }

            return EngineResult<IOutputDirectoryLease>.Success(result.Value.Lease, warnings: result.Warnings);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Logger.Error(
                exception,
                "Failed to acquire the output-directory lease for workspace {WorkspaceId} and output {OutputPath}",
                WorkspaceId,
                output.PluginPath);
            return EngineResult<IOutputDirectoryLease>.Failure(new EngineError(
                EngineErrorCode.UnexpectedFailure,
                "The output-directory lease could not be acquired."));
        }
    }

    /// <summary>Inspects durable save metadata while the caller holds the output-directory lease.</summary>
    /// <param name="lease">The caller-owned output-directory lease.</param>
    /// <param name="output">The exact output seeking engine-open admission.</param>
    /// <param name="cancellationToken">A token that cancels metadata inspection.</param>
    /// <returns>Ready admission, unresolved-save admission, or a typed inspection failure.</returns>
    private async ValueTask<EngineResult<OutputAdmissionResult>> InspectOutputAdmissionAsync(
        IOutputDirectoryLease lease,
        OutputAssociation output,
        CancellationToken cancellationToken)
    {
        try
        {
            return await SaveCoordinator.InspectOutputAdmissionAsync(
                lease,
                new OutputAdmissionRequest(
                    WorkspaceId,
                    Request.Game,
                    Request.Release,
                    Sources!.Baseline,
                    output),
                cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Logger.Error(
                exception,
                "Failed to inspect output admission for workspace {WorkspaceId} and output {OutputPath}",
                WorkspaceId,
                output.PluginPath);
            return EngineResult<OutputAdmissionResult>.Failure(new EngineError(
                EngineErrorCode.UnexpectedFailure,
                "Output admission could not be inspected."));
        }
    }

    /// <summary>Recaptures the complete source baseline and converts unexpected verification failure to a typed result.</summary>
    /// <param name="cancellationToken">A token that cancels source recapture and hashing.</param>
    /// <returns>The recaptured unchanged baseline or a typed verification failure.</returns>
    private async ValueTask<EngineResult<PluginSourceInputBaseline>> VerifySourcesUnchangedAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            return await Sources!.VerifyUnchangedAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Failed to verify plugin sources for workspace {WorkspaceId}", WorkspaceId);
            return EngineResult<PluginSourceInputBaseline>.Failure(new EngineError(
                EngineErrorCode.UnexpectedFailure,
                "The plugin source baseline could not be verified."));
        }
    }

    /// <summary>Validates the coordinator-owned envelope before the workspace trusts destination knowledge.</summary>
    /// <param name="result">The coordinator result to inspect.</param>
    /// <param name="request">The exact save request executed by the workspace.</param>
    /// <param name="pendingSave">The immutable original save identity captured before coordination.</param>
    /// <returns><see langword="true"/> when every status-dependent identity and evidence field is consistent.</returns>
    private bool IsCoordinatorSaveResultConsistent(
        SaveResult result,
        SaveRequest request,
        PendingSaveIdentity pendingSave)
    {
        if (result.WorkspaceId != WorkspaceId
            || result.OperationId != request.OperationId
            || result.BaseRevision != CurrentRevision
            || result.ResultRevision != CurrentRevision)
        {
            return false;
        }

        if (result.Status is SaveCommitStatus.NotCommitted or SaveCommitStatus.CommitOutcomeUnknown)
        {
            return result.CommittedBaseline is null && result.ResolvedEvidence is null;
        }

        return result.CommittedBaseline is not null
            && result.RecoveryEvidenceToken is not null
            && result.ResolvedEvidence is not null
            && ResolvedEvidenceMatches(
                result.ResolvedEvidence,
                pendingSave,
                Sources!.Baseline,
                result.CommittedBaseline,
                result.RecoveryEvidenceToken,
                RecoverSaveStatus.Committed);
    }

    /// <summary>Creates and latches an unknown save outcome while retaining the staged candidate.</summary>
    /// <param name="request">The save request whose destination knowledge became unavailable.</param>
    /// <param name="pendingSave">The original save identity retained for recovery.</param>
    /// <param name="message">The safe caller-facing reason for the unknown outcome.</param>
    /// <param name="evidenceToken">Optional coordinator-issued inspection or repair evidence.</param>
    /// <param name="warnings">Warnings already reported by the coordinator.</param>
    /// <returns>An unknown save result describing the still-live pre-save workspace revision.</returns>
    private SaveResult CreateUnknownSaveResult(
        SaveRequest request,
        PendingSaveIdentity pendingSave,
        string message,
        RecoveryEvidenceToken? evidenceToken,
        IReadOnlyList<EngineWarning> warnings)
    {
        SetOutputSynchronization(OutputSynchronizationStatus.RecoveryRequired, pendingSave);
        return new SaveResult(
            WorkspaceId,
            request.OperationId,
            CurrentRevision,
            CurrentRevision,
            SaveCommitStatus.CommitOutcomeUnknown,
            null,
            evidenceToken,
            null,
            new EngineError(EngineErrorCode.CommitOutcomeUnknown, message),
            warnings);
    }

    /// <summary>Creates a committed-but-not-reopened result and latches explicit reopen adoption.</summary>
    /// <param name="request">The original save request.</param>
    /// <param name="pendingSave">The immutable original save identity retained for adoption.</param>
    /// <param name="coordinatorResult">The coordinator result carrying exact committed evidence.</param>
    /// <param name="error">The engine reopen or validation failure.</param>
    /// <param name="warnings">Combined coordinator, lease, admission, reopen, and cleanup warnings.</param>
    /// <returns>A committed result that leaves the staged candidate and live revision unchanged.</returns>
    private SaveResult CreateCommittedButReopenFailedResult(
        SaveRequest request,
        PendingSaveIdentity pendingSave,
        SaveResult coordinatorResult,
        EngineError error,
        IReadOnlyList<EngineWarning> warnings)
    {
        SetOutputSynchronization(OutputSynchronizationStatus.ReopenRequired, pendingSave);
        return new SaveResult(
            WorkspaceId,
            request.OperationId,
            CurrentRevision,
            CurrentRevision,
            SaveCommitStatus.CommittedButReopenFailed,
            coordinatorResult.CommittedBaseline,
            coordinatorResult.RecoveryEvidenceToken,
            coordinatorResult.ResolvedEvidence,
            error,
            warnings);
    }

    /// <summary>Reopens a known committed destination and publishes only exact fully validated engine state.</summary>
    /// <param name="request">The original guarded save request.</param>
    /// <param name="pendingSave">The immutable original save identity.</param>
    /// <param name="coordinatorResult">The consistent coordinator result carrying committed evidence.</param>
    /// <returns>The final published save result, or a reopen-required result retaining the candidate.</returns>
    private async ValueTask<SaveResult> AdoptCommittedSaveAsync(
        SaveRequest request,
        PendingSaveIdentity pendingSave,
        SaveResult coordinatorResult)
    {
        if (coordinatorResult.Status == SaveCommitStatus.CommittedButReopenFailed)
        {
            return CreateCommittedButReopenFailedResult(
                request,
                pendingSave,
                coordinatorResult,
                coordinatorResult.Error ?? new EngineError(EngineErrorCode.OutputOpenFailed, "The committed output could not be reopened."),
                coordinatorResult.Warnings);
        }

        var leaseResult = await AcquireOutputDirectoryLeaseAsync(
            pendingSave.Output,
            CancellationToken.None).ConfigureAwait(false);
        if (!leaseResult.Succeeded || leaseResult.Value is null)
        {
            return CreateCommittedButReopenFailedResult(
                request,
                pendingSave,
                coordinatorResult,
                leaseResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The output-directory lease provider returned no lease after commit."),
                CombineWarnings(coordinatorResult.Warnings, leaseResult.Warnings));
        }

        await using var outputLease = leaseResult.Value;
        var admissionResult = await InspectOutputAdmissionAsync(
            outputLease,
            pendingSave.Output,
            CancellationToken.None).ConfigureAwait(false);
        var warnings = CombineWarnings(
            coordinatorResult.Warnings,
            CombineWarnings(leaseResult.Warnings, admissionResult.Warnings));
        if (!admissionResult.Succeeded || admissionResult.Value is null)
        {
            return CreateCommittedButReopenFailedResult(
                request,
                pendingSave,
                coordinatorResult,
                admissionResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "Output admission returned no result after commit."),
                warnings);
        }

        if (admissionResult.Value.Status != OutputSynchronizationStatus.Ready)
        {
            return CreateCommittedButReopenFailedResult(
                request,
                pendingSave,
                coordinatorResult,
                new EngineError(EngineErrorCode.RepairRequired, "The committed output still has unresolved save metadata."),
                warnings);
        }

        var sourceVerification = await VerifySourcesUnchangedAsync(CancellationToken.None).ConfigureAwait(false);
        warnings = CombineWarnings(warnings, sourceVerification.Warnings);
        if (!sourceVerification.Succeeded
            || sourceVerification.Value is null
            || !SourceBaselinesMatch(Sources!.Baseline, sourceVerification.Value))
        {
            return CreateCommittedButReopenFailedResult(
                request,
                pendingSave,
                coordinatorResult,
                sourceVerification.Error ?? new EngineError(EngineErrorCode.ExternalChangeDetected, "The plugin source baseline changed before the committed output could be reopened."),
                warnings);
        }

        EngineResult<PluginOutputOpenResult> openResult;
        try
        {
            openResult = await Task.Run(
                async () => await Adapter.ReopenOutputAsync(
                    Sources!,
                    pendingSave.Output,
                    coordinatorResult.CommittedBaseline!,
                    CancellationToken.None).ConfigureAwait(false)).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Logger.Error(
                exception,
                "Failed to reopen committed plugin output in workspace {WorkspaceId} for save {OperationId}",
                WorkspaceId,
                request.OperationId);
            return CreateCommittedButReopenFailedResult(
                request,
                pendingSave,
                coordinatorResult,
                new EngineError(EngineErrorCode.OutputOpenFailed, "The committed plugin output could not be reopened."),
                warnings);
        }

        warnings = CombineWarnings(warnings, openResult.Warnings);
        if (!openResult.Succeeded || openResult.Value is null)
        {
            return CreateCommittedButReopenFailedResult(
                request,
                pendingSave,
                coordinatorResult,
                openResult.Error ?? new EngineError(EngineErrorCode.OutputOpenFailed, "The committed plugin output could not be reopened."),
                warnings);
        }

        if (!OutputAssociationsMatch(pendingSave.Output, openResult.Value.Association)
            || !BaselinesMatch(coordinatorResult.CommittedBaseline!, openResult.Value.Baseline))
        {
            await DisposeRejectedOpenedOutputAsync(
                openResult.Value,
                "committed output identity validation").ConfigureAwait(false);
            return CreateCommittedButReopenFailedResult(
                request,
                pendingSave,
                coordinatorResult,
                new EngineError(EngineErrorCode.ExternalChangeDetected, "The reopened plugin output did not match the exact committed association and baseline."),
                warnings);
        }

        var baseRevision = CurrentRevision;
        var resultRevision = BaselinesMatch(SelectedOutputBaseline!, openResult.Value.Baseline)
            ? baseRevision
            : CreateOutputRevision(openResult.Value.Association, openResult.Value.Baseline);
        var disposalWarnings = await PublishOutputAsync(
            openResult.Value,
            true,
            resultRevision).ConfigureAwait(false);
        SetOutputSynchronization(OutputSynchronizationStatus.Ready, null);
        return new SaveResult(
            WorkspaceId,
            request.OperationId,
            baseRevision,
            resultRevision,
            SaveCommitStatus.Committed,
            SelectedOutputBaseline,
            null,
            null,
            null,
            CombineWarnings(warnings, disposalWarnings));
    }

    /// <summary>Disposes a successfully opened state that failed workspace-level identity validation.</summary>
    /// <param name="openResult">The rejected independently owned plugin output.</param>
    /// <param name="reason">The validation stage used for structured diagnostics.</param>
    /// <returns>A task that completes after cleanup succeeds or its failure is logged.</returns>
    private async ValueTask DisposeRejectedOpenedOutputAsync(
        PluginOutputOpenResult openResult,
        string reason)
    {
        try
        {
            await openResult.Output.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Logger.Error(
                exception,
                "Failed to dispose rejected reopened plugin output in workspace {WorkspaceId} after {Reason}",
                WorkspaceId,
                reason);
        }
    }

    /// <summary>Gets the current typed failure that blocks ordinary output operations.</summary>
    /// <returns>A recovery-required failure while synchronization is latched; otherwise <see langword="null"/>.</returns>
    private EngineError? GetOutputSynchronizationFailure()
    {
        return CurrentOutputSynchronization.Status switch
        {
            OutputSynchronizationStatus.Ready => null,
            OutputSynchronizationStatus.RecoveryRequired => new EngineError(
                EngineErrorCode.RepairRequired,
                "The workspace is waiting for explicit recovery of an unresolved save."),
            OutputSynchronizationStatus.ReopenRequired => new EngineError(
                EngineErrorCode.RepairRequired,
                "The workspace is waiting for explicit adoption of a terminal recovered output."),
            _ => new EngineError(
                EngineErrorCode.UnexpectedFailure,
                "The workspace has an invalid output synchronization state.")
        };
    }

    /// <summary>Publishes an atomic output synchronization snapshot under the shared state lock.</summary>
    /// <param name="status">The ready or blocked synchronization state.</param>
    /// <param name="pendingSave">The original pending save for a blocked state.</param>
    private void SetOutputSynchronization(
        OutputSynchronizationStatus status,
        PendingSaveIdentity? pendingSave)
    {
        var synchronization = new OutputSynchronizationState(status, pendingSave);
        lock (RevisionSync)
        {
            CurrentOutputSynchronization = synchronization;
        }
    }

    /// <summary>Determines whether two output associations identify the same path, plugin, and engine modes.</summary>
    /// <param name="expected">The expected output association.</param>
    /// <param name="actual">The actual output association.</param>
    /// <returns><see langword="true"/> when every canonical identity field matches.</returns>
    private static bool OutputAssociationsMatch(OutputAssociation expected, OutputAssociation actual)
    {
        var pathComparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        return string.Equals(expected.PluginPath, actual.PluginPath, pathComparison)
            && expected.ModKey == actual.ModKey
            && expected.LocalizedOutputMode == actual.LocalizedOutputMode
            && expected.MasterStyle == actual.MasterStyle;
    }

    /// <summary>Determines whether two source baselines carry the same complete physical artifact observations.</summary>
    /// <param name="expected">The expected source baseline.</param>
    /// <param name="actual">The actual source baseline.</param>
    /// <returns><see langword="true"/> when the deterministic identity and every artifact field match.</returns>
    private static bool SourceBaselinesMatch(
        PluginSourceInputBaseline expected,
        PluginSourceInputBaseline actual)
    {
        return expected.BaselineId == actual.BaselineId
            && ArtifactCollectionsMatch(expected.Artifacts, actual.Artifacts);
    }

    /// <summary>Determines whether two ordered plugin artifact collections match exactly.</summary>
    /// <param name="expected">The expected artifact collection.</param>
    /// <param name="actual">The actual artifact collection.</param>
    /// <returns><see langword="true"/> when every physical and logical identity field matches.</returns>
    private static bool ArtifactCollectionsMatch(
        IReadOnlyList<PluginArtifactAssociation> expected,
        IReadOnlyList<PluginArtifactAssociation> actual)
    {
        if (expected.Count != actual.Count)
        {
            return false;
        }

        var pathComparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        for (var index = 0; index < expected.Count; index++)
        {
            var expectedArtifact = expected[index];
            var actualArtifact = actual[index];
            if (!string.Equals(expectedArtifact.Path, actualArtifact.Path, pathComparison)
                || expectedArtifact.Role != actualArtifact.Role
                || !string.Equals(expectedArtifact.Language, actualArtifact.Language, StringComparison.OrdinalIgnoreCase)
                || !expectedArtifact.Fingerprint.Equals(actualArtifact.Fingerprint)
                || !Equals(expectedArtifact.FileIdentity, actualArtifact.FileIdentity))
            {
                return false;
            }
        }

        return true;
    }
}
