using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;

namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Implements explicit evidence-bound repair of recognized incomplete save transactions.</summary>
public sealed partial class WorkspaceSaveCoordinator
{
    /// <inheritdoc />
    public async ValueTask<RepairSaveResult> RepairAsync(
        RepairSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        string outputDirectory;
        try
        {
            outputDirectory = GetOutputDirectory(request.Output);
        }
        catch (Exception exception)
        {
            return RepairFailure(request, RepairSaveStatus.NotStarted, EngineErrorCode.InvalidRequest, exception.Message);
        }

        EngineResult<OutputDirectoryLeaseAcquisition> acquisition;
        try
        {
            acquisition = await LeaseProvider.AcquireAsync(
                outputDirectory,
                OutputDirectoryLeaseMode.CreateOrOpen,
                cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return RepairFailure(request, RepairSaveStatus.NotStarted, EngineErrorCode.ValidationFailed, "The repair was canceled before destination mutation.");
        }
        catch (Exception exception)
        {
            return RepairFailure(request, RepairSaveStatus.NotStarted, EngineErrorCode.OutputDirectoryBusy, exception.Message);
        }

        if (!acquisition.Succeeded
            || acquisition.Value?.Status != OutputDirectoryLeaseAcquisitionStatus.Acquired
            || acquisition.Value.Lease is null)
        {
            return RepairFailure(
                request,
                RepairSaveStatus.NotStarted,
                acquisition.Error?.Code ?? EngineErrorCode.OutputDirectoryBusy,
                acquisition.Error?.Message ?? "The exclusive output-directory lease could not be acquired for repair.");
        }

        await using var lease = acquisition.Value.Lease;
        SaveTransactionJournal? journal = null;
        SaveTransactionPaths? paths = null;
        var destinationMutationCount = 0;
        try
        {
            ValidateLease(lease, request.Output);
            paths = new SaveTransactionPaths(outputDirectory, request.WorkspaceId, request.SaveOperationId);
            journal = await TransactionStore.ReadAsync(paths, cancellationToken).ConfigureAwait(false);
            if (journal is null
                || journal.WorkspaceId != request.WorkspaceId
                || journal.SaveOperationId != request.SaveOperationId
                || journal.SaveBaseRevision != request.ExpectedSaveRevision
                || !PluginSaveArtifactUtilities.MatchOutput(journal.Output, request.Output))
            {
                return RepairFailure(request, RepairSaveStatus.NotStarted, EngineErrorCode.NoRecoveryEvidence, "No recognized save journal matches the exact repair request.");
            }

            var repairFingerprint = CreateRepairFingerprint(request);
            var replay = journal.RepairAttempts.SingleOrDefault(attempt => attempt.OperationId == request.RepairOperationId);
            SaveRepairAttempt attempt;
            if (replay is not null)
            {
                if (!string.Equals(replay.RequestFingerprint, repairFingerprint, StringComparison.Ordinal))
                {
                    return RepairFailure(request, RepairSaveStatus.NotStarted, EngineErrorCode.OperationIdReuse, "The repair operation identifier was reused with a different canonical payload.");
                }

                if (replay.Status is not null)
                {
                    return await ReplayRepairAsync(request, journal, replay, cancellationToken).ConfigureAwait(false);
                }

                attempt = replay;
            }
            else
            {
                if (journal.RepairAttempts.Count >= 64)
                {
                    return RepairFailure(request, RepairSaveStatus.NotStarted, EngineErrorCode.ValidationFailed, "The bounded repair-attempt history is full.");
                }

                var recovery = await InspectRecoveryAsync(journal, cancellationToken).ConfigureAwait(false);
                if (!recovery.RepairRequired
                    || recovery.EvidenceToken is null
                    || !recovery.EvidenceToken.Equals(request.EvidenceToken))
                {
                    return RepairFailure(
                        request,
                        RepairSaveStatus.BlockedByExternalChange,
                        recovery.Error?.Code ?? EngineErrorCode.ExternalChangeDetected,
                        recovery.Error?.Message ?? "The reviewed recovery evidence is stale or the transaction no longer requires repair.",
                        recovery.EvidenceToken);
                }

                attempt = await PrepareRepairAttemptAsync(
                    request,
                    journal,
                    paths,
                    repairFingerprint,
                    cancellationToken).ConfigureAwait(false);
                journal = journal.WithRepairAttempts(journal.RepairAttempts.Concat([attempt]).ToArray());
                await TransactionStore.WriteAsync(paths, journal, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            var targetStatus = request.Direction == RepairSaveDirection.CompletePrepared
                ? RecoverSaveStatus.Committed
                : RecoverSaveStatus.NotCommitted;
            for (var index = 0; index < journal.ArtifactPlans.Count; index++)
            {
                var plan = journal.ArtifactPlans[index];
                var repairPlan = attempt.ArtifactPlans[index];
                var output = await PluginSaveArtifactUtilities.CaptureOutputAsync(
                    journal.Release,
                    journal.Output,
                    CancellationToken.None).ConfigureAwait(false);
                var currentByPath = output.Artifacts.ToDictionary(artifact => artifact.Path, PluginSaveArtifactUtilities.PathComparer);
                if (!currentByPath.TryGetValue(plan.Before.Path, out var current))
                {
                    throw new InvalidDataException("The complete output association changed during explicit repair.");
                }

                if (PluginSaveArtifactUtilities.MatchExact([repairPlan.Target], [current]))
                {
                    continue;
                }

                if (!PluginSaveArtifactUtilities.MatchExact([repairPlan.Current], [current]))
                {
                    return RepairFailure(
                        request,
                        RepairSaveStatus.BlockedByExternalChange,
                        EngineErrorCode.ExternalChangeDetected,
                        $"Output artifact '{plan.Before.Path}' changed outside the recognized repair transaction.");
                }

                if (repairPlan.Publish is not null)
                {
                    var publishNow = await PluginFileInspector.InspectAsync(
                        repairPlan.Publish.Path,
                        repairPlan.Publish.Role,
                        repairPlan.Publish.Language,
                        mustExist: true,
                        CancellationToken.None).ConfigureAwait(false);
                    if (!PluginSaveArtifactUtilities.MatchExact([repairPlan.Publish], [publishNow]))
                    {
                        return RepairFailure(
                            request,
                            RepairSaveStatus.BlockedByExternalChange,
                            EngineErrorCode.ExternalChangeDetected,
                            $"Transaction-owned repair publication '{repairPlan.Publish.Path}' changed.");
                    }
                }

                var recheck = await PluginFileInspector.InspectAsync(
                    plan.Before.Path,
                    plan.Before.Role,
                    plan.Before.Language,
                    mustExist: false,
                    CancellationToken.None).ConfigureAwait(false);
                if (!PluginSaveArtifactUtilities.MatchExact([repairPlan.Current], [recheck]))
                {
                    return RepairFailure(
                        request,
                        RepairSaveStatus.BlockedByExternalChange,
                        EngineErrorCode.ExternalChangeDetected,
                        $"Output artifact '{plan.Before.Path}' changed immediately before repair mutation.");
                }

                FileOperations.BeforeDestinationMutation(journal.SaveOperationId, index);
                if (destinationMutationCount == 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                await VerifyRepairMutationInputsAsync(repairPlan, CancellationToken.None).ConfigureAwait(false);

                if (repairPlan.Publish is not null)
                {
                    EnsureDestinationParent(plan.Before.Path, outputDirectory);
                    FileOperations.Publish(repairPlan.Publish.Path, plan.Before.Path);
                }
                else if (repairPlan.Retired is not null)
                {
                    if (File.Exists(repairPlan.Retired.Path) || Directory.Exists(repairPlan.Retired.Path))
                    {
                        return RepairFailure(
                            request,
                            RepairSaveStatus.BlockedByExternalChange,
                            EngineErrorCode.ExternalChangeDetected,
                            $"The repair retirement path is unexpectedly occupied: '{repairPlan.Retired.Path}'.");
                    }

                    FileOperations.Retire(plan.Before.Path, repairPlan.Retired.Path);
                }

                destinationMutationCount++;
                FileOperations.AfterDestinationMutation(journal.SaveOperationId, index);
            }

            var finalOutput = await PluginSaveArtifactUtilities.CaptureOutputAsync(
                journal.Release,
                journal.Output,
                CancellationToken.None).ConfigureAwait(false);
            var exactTarget = PluginSaveArtifactUtilities.CreateOutputBaseline(
                attempt.ArtifactPlans.Select(plan => plan.Target).ToArray());
            if (!PluginSaveArtifactUtilities.MatchBaseline(exactTarget, finalOutput))
            {
                return await RepairUnknownAsync(request, journal, destinationMutationCount).ConfigureAwait(false);
            }

            var terminalPhase = targetStatus == RecoverSaveStatus.Committed
                ? SaveTransactionPhase.Committed
                : SaveTransactionPhase.NotCommitted;
            var successfulStatus = targetStatus == RecoverSaveStatus.Committed
                ? RepairSaveStatus.PreparedSetCompleted
                : RepairSaveStatus.BaselineRestored;
            var completedAttempts = journal.RepairAttempts
                .Select(attempt => attempt.OperationId == request.RepairOperationId
                    ? new SaveRepairAttempt(
                        attempt.OperationId,
                        attempt.RequestFingerprint,
                        attempt.Direction,
                        successfulStatus,
                        attempt.ArtifactPlans)
                    : attempt)
                .ToArray();
            journal = journal.WithRepairAttempts(completedAttempts)
                .WithProgress(terminalPhase, journal.ArtifactPlans.Count, finalOutput);
            await TransactionStore.WriteAsync(paths, journal, CancellationToken.None).ConfigureAwait(false);

            var terminalRecovery = await InspectRecoveryAsync(journal, CancellationToken.None).ConfigureAwait(false);
            if (terminalRecovery.ResolvedEvidence is null || terminalRecovery.EvidenceToken is null)
            {
                return new RepairSaveResult(
                    request.WorkspaceId,
                    request.SaveOperationId,
                    request.RepairOperationId,
                    RepairSaveStatus.StillUnknown,
                    null,
                    terminalRecovery.EvidenceToken,
                    null,
                    terminalRecovery.Error ?? new EngineError(EngineErrorCode.ExternalChangeDetected, "The repair completed physically, but current evidence is not adoptable."));
            }

            return new RepairSaveResult(
                request.WorkspaceId,
                request.SaveOperationId,
                request.RepairOperationId,
                successfulStatus,
                finalOutput,
                terminalRecovery.EvidenceToken,
                terminalRecovery.ResolvedEvidence,
                null);
        }
        catch (OperationCanceledException)
        {
            if (journal is null || destinationMutationCount == 0)
            {
                return RepairFailure(request, RepairSaveStatus.NotStarted, EngineErrorCode.ValidationFailed, "The repair was canceled before destination mutation.");
            }

            return await RepairUnknownAsync(request, journal, destinationMutationCount).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            if (journal is null || destinationMutationCount == 0)
            {
                return RepairFailure(request, RepairSaveStatus.NotStarted, EngineErrorCode.UnexpectedFailure, exception.Message);
            }

            return await RepairUnknownAsync(request, journal, destinationMutationCount, exception).ConfigureAwait(false);
        }
    }

    /// <summary>Prepares and observes every repair-owned file before any destination mutation.</summary>
    /// <param name="request">The explicit repair request.</param>
    /// <param name="journal">The recognized incomplete save journal.</param>
    /// <param name="paths">The stable transaction paths.</param>
    /// <param name="requestFingerprint">The canonical repair request digest.</param>
    /// <param name="cancellationToken">The token honored while repair preparation remains reversible.</param>
    /// <returns>The complete exact repair plan ready for durable journaling.</returns>
    private static async Task<SaveRepairAttempt> PrepareRepairAttemptAsync(
        RepairSaveRequest request,
        SaveTransactionJournal journal,
        SaveTransactionPaths paths,
        string requestFingerprint,
        CancellationToken cancellationToken)
    {
        var output = await PluginSaveArtifactUtilities.CaptureOutputAsync(
            journal.Release,
            journal.Output,
            cancellationToken).ConfigureAwait(false);
        if (output.Artifacts.Count != journal.ArtifactPlans.Count)
        {
            throw new InvalidDataException("The complete output association changed during repair preparation.");
        }

        var outputByPath = output.Artifacts.ToDictionary(
            artifact => artifact.Path,
            PluginSaveArtifactUtilities.PathComparer);
        var preparationId = Guid.NewGuid();
        var repairPlans = new List<SaveRepairArtifactPlan>(journal.ArtifactPlans.Count);
        for (var index = 0; index < journal.ArtifactPlans.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var savePlan = journal.ArtifactPlans[index];
            if (!outputByPath.TryGetValue(savePlan.Before.Path, out var current)
                || !IsRecognizedDestinationArtifact(journal, index, current))
            {
                throw new InvalidDataException($"Output artifact '{savePlan.Before.Path}' changed before repair preparation.");
            }

            var desiredContent = request.Direction == RepairSaveDirection.CompletePrepared
                ? savePlan.Staged
                : savePlan.Before;
            if (ArtifactContentMatches(desiredContent, current))
            {
                repairPlans.Add(new SaveRepairArtifactPlan(current, current, null, null));
                continue;
            }

            if (desiredContent.Fingerprint.Exists)
            {
                var ownedSource = request.Direction == RepairSaveDirection.CompletePrepared
                    ? savePlan.Staged
                    : savePlan.Backup!;
                var sourceNow = await PluginFileInspector.InspectAsync(
                    ownedSource.Path,
                    ownedSource.Role,
                    ownedSource.Language,
                    mustExist: true,
                    cancellationToken).ConfigureAwait(false);
                if (!PluginSaveArtifactUtilities.MatchExact([ownedSource], [sourceNow]))
                {
                    throw new InvalidDataException($"Transaction-owned repair source '{ownedSource.Path}' changed.");
                }

                var publishPath = Path.Combine(
                    paths.PublishDirectoryPath,
                    $"repair-{request.RepairOperationId:N}-{preparationId:N}-{index:D4}.bin");
                await PluginSaveArtifactUtilities.CopyAndFlushAsync(
                    ownedSource.Path,
                    publishPath,
                    cancellationToken).ConfigureAwait(false);
                var publish = await PluginFileInspector.InspectAsync(
                    publishPath,
                    savePlan.Before.Role,
                    savePlan.Before.Language,
                    mustExist: true,
                    cancellationToken).ConfigureAwait(false);
                if (!ArtifactContentMatches(desiredContent, publish))
                {
                    throw new InvalidDataException($"Repair publication '{publishPath}' differs from its validated source.");
                }

                var target = new PluginArtifactAssociation(
                    savePlan.Before.Path,
                    savePlan.Before.Role,
                    savePlan.Before.Language,
                    publish.Fingerprint,
                    publish.FileIdentity);
                repairPlans.Add(new SaveRepairArtifactPlan(current, target, publish, null));
                continue;
            }

            var retiredPath = Path.Combine(
                paths.RetiredDirectoryPath,
                $"repair-{request.RepairOperationId:N}-{preparationId:N}-{index:D4}.bin");
            if (File.Exists(retiredPath) || Directory.Exists(retiredPath))
            {
                throw new InvalidDataException($"The repair retirement path is unexpectedly occupied: '{retiredPath}'.");
            }

            var targetAbsent = new PluginArtifactAssociation(
                savePlan.Before.Path,
                savePlan.Before.Role,
                savePlan.Before.Language,
                desiredContent.Fingerprint);
            var retired = new PluginArtifactAssociation(
                retiredPath,
                current.Role,
                current.Language,
                current.Fingerprint,
                current.FileIdentity);
            repairPlans.Add(new SaveRepairArtifactPlan(current, targetAbsent, null, retired));
        }

        return new SaveRepairAttempt(
            request.RepairOperationId,
            requestFingerprint,
            request.Direction,
            null,
            repairPlans);
    }

    /// <summary>Rechecks exact repair source, destination, and retirement paths after the final fault seam.</summary>
    /// <param name="plan">The durable repair artifact plan about to move.</param>
    /// <param name="cancellationToken">The post-boundary bookkeeping token.</param>
    /// <returns>A task that completes when every mutation input remains exact.</returns>
    private static async Task VerifyRepairMutationInputsAsync(
        SaveRepairArtifactPlan plan,
        CancellationToken cancellationToken)
    {
        var destination = await PluginFileInspector.InspectAsync(
            plan.Current.Path,
            plan.Current.Role,
            plan.Current.Language,
            mustExist: plan.Current.Fingerprint.Exists,
            cancellationToken).ConfigureAwait(false);
        if (!PluginSaveArtifactUtilities.MatchExact([plan.Current], [destination]))
        {
            throw new InvalidDataException($"Repair destination '{plan.Current.Path}' changed at the final mutation boundary.");
        }

        if (plan.Publish is not null)
        {
            var publish = await PluginFileInspector.InspectAsync(
                plan.Publish.Path,
                plan.Publish.Role,
                plan.Publish.Language,
                mustExist: true,
                cancellationToken).ConfigureAwait(false);
            if (!PluginSaveArtifactUtilities.MatchExact([plan.Publish], [publish]))
            {
                throw new InvalidDataException($"Repair publication '{plan.Publish.Path}' changed at the final mutation boundary.");
            }
        }

        if (plan.Retired is not null
            && (File.Exists(plan.Retired.Path) || Directory.Exists(plan.Retired.Path)))
        {
            throw new InvalidDataException($"Repair retirement path '{plan.Retired.Path}' became occupied at the final mutation boundary.");
        }
    }

    /// <summary>Replays one exact repair operation from its durable journal state.</summary>
    /// <param name="request">The exact matching repair request.</param>
    /// <param name="journal">The recognized save journal.</param>
    /// <param name="attempt">The matching repair attempt.</param>
    /// <param name="cancellationToken">The read-only inspection token.</param>
    /// <returns>The reconstructed repair result.</returns>
    private static async Task<RepairSaveResult> ReplayRepairAsync(
        RepairSaveRequest request,
        SaveTransactionJournal journal,
        SaveRepairAttempt attempt,
        CancellationToken cancellationToken)
    {
        var recovery = await InspectRecoveryAsync(journal, cancellationToken).ConfigureAwait(false);
        if (attempt.Status is RepairSaveStatus.PreparedSetCompleted or RepairSaveStatus.BaselineRestored
            && recovery.ResolvedEvidence is not null)
        {
            return new RepairSaveResult(
                request.WorkspaceId,
                request.SaveOperationId,
                request.RepairOperationId,
                attempt.Status.Value,
                recovery.ResolvedEvidence.ResolvedOutputBaseline,
                recovery.EvidenceToken,
                recovery.ResolvedEvidence,
                recovery.Error);
        }

        return new RepairSaveResult(
            request.WorkspaceId,
            request.SaveOperationId,
            request.RepairOperationId,
            attempt.Status ?? RepairSaveStatus.StillUnknown,
            null,
            recovery.EvidenceToken,
            null,
            recovery.Error ?? new EngineError(EngineErrorCode.CommitOutcomeUnknown, "The idempotent repair replay remains unresolved."));
    }

    /// <summary>Returns an exact repair outcome after a post-boundary exception or incomplete final classification.</summary>
    /// <param name="request">The explicit repair request.</param>
    /// <param name="journal">The latest durable journal.</param>
    /// <param name="mutationCount">The known successful destination mutation count.</param>
    /// <param name="exception">The optional triggering exception.</param>
    /// <returns>A terminal repair result when physically known, otherwise still unknown.</returns>
    private static async Task<RepairSaveResult> RepairUnknownAsync(
        RepairSaveRequest request,
        SaveTransactionJournal journal,
        int mutationCount,
        Exception? exception = null)
    {
        var recovery = await InspectRecoveryAsync(journal, CancellationToken.None).ConfigureAwait(false);
        if (recovery.ResolvedEvidence is not null)
        {
            var status = recovery.Status == RecoverSaveStatus.Committed
                ? RepairSaveStatus.PreparedSetCompleted
                : RepairSaveStatus.BaselineRestored;
            return new RepairSaveResult(
                request.WorkspaceId,
                request.SaveOperationId,
                request.RepairOperationId,
                status,
                recovery.ResolvedEvidence.ResolvedOutputBaseline,
                recovery.EvidenceToken,
                recovery.ResolvedEvidence,
                exception is null ? recovery.Error : Unexpected("Explicit save repair", exception));
        }

        return new RepairSaveResult(
            request.WorkspaceId,
            request.SaveOperationId,
            request.RepairOperationId,
            mutationCount == 0 ? RepairSaveStatus.NotStarted : RepairSaveStatus.StillUnknown,
            null,
            recovery.EvidenceToken,
            null,
            recovery.Error ?? new EngineError(EngineErrorCode.CommitOutcomeUnknown, exception?.Message ?? "The explicit repair outcome remains unknown."));
    }

    /// <summary>Creates a non-mutating or external-change repair failure.</summary>
    /// <param name="request">The repair request.</param>
    /// <param name="status">The non-success repair status.</param>
    /// <param name="code">The stable error category.</param>
    /// <param name="message">The diagnostic message.</param>
    /// <param name="evidenceToken">Optional updated evidence when still recognized.</param>
    /// <returns>The failed repair result.</returns>
    private static RepairSaveResult RepairFailure(
        RepairSaveRequest request,
        RepairSaveStatus status,
        EngineErrorCode code,
        string message,
        RecoveryEvidenceToken? evidenceToken = null)
    {
        return new RepairSaveResult(
            request.WorkspaceId,
            request.SaveOperationId,
            request.RepairOperationId,
            status,
            null,
            evidenceToken,
            null,
            new EngineError(code, message));
    }
}
