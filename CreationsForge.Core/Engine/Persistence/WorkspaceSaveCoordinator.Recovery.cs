using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;

namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Implements read-only recovery and admission plus explicit terminal-evidence finalization for guarded saves.</summary>
public sealed partial class WorkspaceSaveCoordinator
{
    /// <inheritdoc />
    public async ValueTask<RecoverSaveResult> RecoverAsync(
        RecoverSaveRequest request,
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
            return NoRecovery(request, new EngineError(EngineErrorCode.NoRecoveryEvidence, exception.Message));
        }

        EngineResult<OutputDirectoryLeaseAcquisition> acquisition;
        try
        {
            acquisition = await LeaseProvider.AcquireAsync(
                outputDirectory,
                OutputDirectoryLeaseMode.ExistingOnly,
                cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return NoRecovery(request, Unexpected("Save recovery lease acquisition", exception));
        }

        if (!acquisition.Succeeded || acquisition.Value is null)
        {
            return NoRecovery(request, acquisition.Error ?? new EngineError(
                EngineErrorCode.OutputDirectoryBusy,
                "The output-directory lease could not be acquired for recovery inspection."));
        }

        if (acquisition.Value.Status == OutputDirectoryLeaseAcquisitionStatus.GuardNotFound)
        {
            return NoRecovery(request, new EngineError(
                EngineErrorCode.NoRecoveryEvidence,
                "No stable output guard exists, so no CreationsForge recovery evidence is available."));
        }

        await using var lease = acquisition.Value.Lease!;
        try
        {
            ValidateLease(lease, request.Output);
            var paths = new SaveTransactionPaths(outputDirectory, request.WorkspaceId, request.SaveOperationId);
            var journal = await TransactionStore.ReadAsync(paths, cancellationToken).ConfigureAwait(false);
            if (journal is null || !PluginSaveArtifactUtilities.MatchOutput(journal.Output, request.Output))
            {
                return NoRecovery(request, new EngineError(
                    EngineErrorCode.NoRecoveryEvidence,
                    "No recognized save journal matches the requested workspace, operation, and output association."));
            }

            return await InspectRecoveryAsync(journal, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return NoRecovery(request, Unexpected("Read-only save recovery inspection", exception));
        }
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<OutputAdmissionResult>> InspectOutputAdmissionAsync(
        IOutputDirectoryLease lease,
        OutputAdmissionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lease);
        ArgumentNullException.ThrowIfNull(request);
        try
        {
            var outputDirectory = ValidateLease(lease, request.Output);
            var inventory = await TransactionStore.ReadAllAsync(outputDirectory, cancellationToken).ConfigureAwait(false);
            foreach (var entry in inventory.Journals)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var journal = entry.Journal;
                if (!PluginSaveArtifactUtilities.MatchOutput(journal.Output, request.Output)
                    || journal.Phase is SaveTransactionPhase.Committed or SaveTransactionPhase.NotCommitted)
                {
                    continue;
                }

                return EngineResult<OutputAdmissionResult>.Success(new OutputAdmissionResult(
                    OutputSynchronizationStatus.RecoveryRequired,
                    CreatePendingSave(journal)));
            }

            return EngineResult<OutputAdmissionResult>.Success(new OutputAdmissionResult(
                OutputSynchronizationStatus.Ready,
                null));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return EngineResult<OutputAdmissionResult>.Failure(Unexpected("Output save admission inspection", exception));
        }
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<ResolvedOutputEvidence>> ValidateResolvedEvidenceAsync(
        IOutputDirectoryLease lease,
        ResolvedOutputEvidence evidence,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lease);
        ArgumentNullException.ThrowIfNull(evidence);
        try
        {
            var outputDirectory = ValidateLease(lease, evidence.Output);
            var paths = new SaveTransactionPaths(
                outputDirectory,
                evidence.OriginalWorkspaceId,
                evidence.SaveOperationId);
            var journal = await TransactionStore.ReadAsync(paths, cancellationToken).ConfigureAwait(false);
            if (journal is null || !EvidenceClaimsMatchJournal(evidence, journal))
            {
                return EngineResult<ResolvedOutputEvidence>.Failure(new EngineError(
                    EngineErrorCode.NoRecoveryEvidence,
                    "The supplied recovery evidence does not match a recognized canonical save journal."));
            }

            var recovery = await InspectRecoveryAsync(journal, cancellationToken).ConfigureAwait(false);
            var canonical = recovery.ResolvedEvidence;
            if (canonical is null
                || recovery.Status != evidence.Status
                || !canonical.EvidenceToken.Equals(evidence.EvidenceToken)
                || !PluginSaveArtifactUtilities.MatchSourceBaseline(canonical.SourceBaseline, evidence.SourceBaseline)
                || !PluginSaveArtifactUtilities.MatchOutput(canonical.Output, evidence.Output)
                || !PluginSaveArtifactUtilities.MatchBaseline(canonical.ResolvedOutputBaseline, evidence.ResolvedOutputBaseline))
            {
                return EngineResult<ResolvedOutputEvidence>.Failure(recovery.Error ?? new EngineError(
                    EngineErrorCode.ExternalChangeDetected,
                    "The reviewed recovery evidence is stale or its current source or output observations changed."));
            }

            if (journal.Phase is not SaveTransactionPhase.Committed and not SaveTransactionPhase.NotCommitted)
            {
                var terminal = ProjectTerminalJournal(journal, canonical.Status, canonical.ResolvedOutputBaseline);
                await TransactionStore.WriteAsync(paths, terminal, cancellationToken).ConfigureAwait(false);
            }

            return EngineResult<ResolvedOutputEvidence>.Success(canonical);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return EngineResult<ResolvedOutputEvidence>.Failure(Unexpected("Resolved output evidence validation", exception));
        }
    }

    /// <summary>Inspects one recognized journal and current physical evidence without mutating persistent state.</summary>
    /// <param name="journal">The recognized immutable journal.</param>
    /// <param name="cancellationToken">The token checked during read-only artifact inspection.</param>
    /// <returns>The durable or physically established recovery result.</returns>
    private static async Task<RecoverSaveResult> InspectRecoveryAsync(
        SaveTransactionJournal journal,
        CancellationToken cancellationToken)
    {
        try
        {
            var output = await PluginSaveArtifactUtilities.CaptureOutputAsync(
                journal.Release,
                journal.Output,
                cancellationToken).ConfigureAwait(false);
            var sourceArtifacts = await PluginSaveArtifactUtilities.RecaptureSourceAsync(
                journal.Release,
                journal.SourceBaseline.Artifacts,
                cancellationToken).ConfigureAwait(false);
            var sourceMatches = PluginSaveArtifactUtilities.MatchExact(
                journal.SourceBaseline.Artifacts,
                sourceArtifacts);
            var ownedArtifacts = await ObserveOwnedArtifactsAsync(journal, cancellationToken).ConfigureAwait(false);
            var tokenObservations = sourceArtifacts.Concat(output.Artifacts).Concat(ownedArtifacts).ToArray();

            if (journal.Phase is SaveTransactionPhase.Committed or SaveTransactionPhase.NotCommitted)
            {
                var token = PluginSaveArtifactUtilities.CreateEvidenceToken(journal, tokenObservations);
                var status = journal.Phase == SaveTransactionPhase.Committed
                    ? RecoverSaveStatus.Committed
                    : RecoverSaveStatus.NotCommitted;
                var terminal = journal.TerminalBaseline!;
                var outputMatches = PluginSaveArtifactUtilities.MatchBaseline(terminal, output);
                if (!sourceMatches || !outputMatches)
                {
                    var changedEvidence = !sourceMatches && !outputMatches
                        ? "source and output baselines"
                        : !sourceMatches
                            ? "source baseline"
                            : "output baseline";
                    return new RecoverSaveResult(
                        journal.WorkspaceId,
                        journal.SaveOperationId,
                        status,
                        journal.SaveBaseRevision,
                        repairRequired: false,
                        token,
                        null,
                        new EngineError(
                            EngineErrorCode.ExternalChangeDetected,
                            $"The original save outcome is known, but its {changedEvidence} is no longer currently adoptable."));
                }

                var evidence = CreateResolvedEvidence(journal, status, output, token);
                return new RecoverSaveResult(
                    journal.WorkspaceId,
                    journal.SaveOperationId,
                    status,
                    journal.SaveBaseRevision,
                    repairRequired: false,
                    token,
                    evidence,
                    null);
            }

            if (journal.Phase == SaveTransactionPhase.Preparing)
            {
                if (!PluginSaveArtifactUtilities.MatchBaseline(journal.BeforeBaseline, output))
                {
                    return UnknownExternalChange(journal);
                }

                var token = CreateProjectedTerminalEvidenceToken(
                    journal,
                    RecoverSaveStatus.NotCommitted,
                    output,
                    tokenObservations);
                if (!sourceMatches)
                {
                    return TerminalConflict(journal, RecoverSaveStatus.NotCommitted, token);
                }

                var evidence = CreateResolvedEvidence(journal, RecoverSaveStatus.NotCommitted, output, token);
                return new RecoverSaveResult(
                    journal.WorkspaceId,
                    journal.SaveOperationId,
                    RecoverSaveStatus.NotCommitted,
                    journal.SaveBaseRevision,
                    repairRequired: false,
                    token,
                    evidence,
                    null);
            }

            if (journal.Disposition == PluginWriteDisposition.Unchanged
                && journal.Phase == SaveTransactionPhase.Prepared)
            {
                if (!PluginSaveArtifactUtilities.MatchBaseline(journal.BeforeBaseline, output))
                {
                    return UnknownExternalChange(journal);
                }

                var token = CreateProjectedTerminalEvidenceToken(
                    journal,
                    RecoverSaveStatus.Committed,
                    output,
                    tokenObservations);
                if (!sourceMatches)
                {
                    return TerminalConflict(journal, RecoverSaveStatus.Committed, token);
                }

                var evidence = CreateResolvedEvidence(journal, RecoverSaveStatus.Committed, output, token);
                return new RecoverSaveResult(
                    journal.WorkspaceId,
                    journal.SaveOperationId,
                    RecoverSaveStatus.Committed,
                    journal.SaveBaseRevision,
                    repairRequired: false,
                    token,
                    evidence,
                    null);
            }

            var classification = ClassifyPhysicalState(journal, output);
            if (classification == SavePhysicalState.AllBefore)
            {
                var token = CreateProjectedTerminalEvidenceToken(
                    journal,
                    RecoverSaveStatus.NotCommitted,
                    output,
                    tokenObservations);
                if (!sourceMatches)
                {
                    return TerminalConflict(journal, RecoverSaveStatus.NotCommitted, token);
                }

                var evidence = CreateResolvedEvidence(journal, RecoverSaveStatus.NotCommitted, output, token);
                return new RecoverSaveResult(
                    journal.WorkspaceId,
                    journal.SaveOperationId,
                    RecoverSaveStatus.NotCommitted,
                    journal.SaveBaseRevision,
                    repairRequired: false,
                    token,
                    evidence,
                    null);
            }

            if (classification == SavePhysicalState.AllPrepared)
            {
                var token = CreateProjectedTerminalEvidenceToken(
                    journal,
                    RecoverSaveStatus.Committed,
                    output,
                    tokenObservations);
                if (!sourceMatches)
                {
                    return TerminalConflict(journal, RecoverSaveStatus.Committed, token);
                }

                var evidence = CreateResolvedEvidence(journal, RecoverSaveStatus.Committed, output, token);
                return new RecoverSaveResult(
                    journal.WorkspaceId,
                    journal.SaveOperationId,
                    RecoverSaveStatus.Committed,
                    journal.SaveBaseRevision,
                    repairRequired: false,
                    token,
                    evidence,
                    null);
            }

            if (classification == SavePhysicalState.OwnedMixed && OwnedRepairFilesMatch(journal, ownedArtifacts))
            {
                var token = PluginSaveArtifactUtilities.CreateEvidenceToken(journal, tokenObservations);
                return new RecoverSaveResult(
                    journal.WorkspaceId,
                    journal.SaveOperationId,
                    RecoverSaveStatus.StillUnknown,
                    journal.SaveBaseRevision,
                    repairRequired: true,
                    token,
                    null,
                    new EngineError(
                        EngineErrorCode.RepairRequired,
                        "The recognized save contains a mixed destination set and requires an explicit complete-or-restore repair decision."));
            }

            return UnknownExternalChange(journal);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            if (journal.Phase is SaveTransactionPhase.Committed or SaveTransactionPhase.NotCommitted)
            {
                var status = journal.Phase == SaveTransactionPhase.Committed
                    ? RecoverSaveStatus.Committed
                    : RecoverSaveStatus.NotCommitted;
                return new RecoverSaveResult(
                    journal.WorkspaceId,
                    journal.SaveOperationId,
                    status,
                    journal.SaveBaseRevision,
                    repairRequired: false,
                    null,
                    null,
                    new EngineError(
                        EngineErrorCode.ExternalChangeDetected,
                        $"The original save outcome is known, but its current source or output evidence could not be verified: {exception.Message}"));
            }

            return new RecoverSaveResult(
                journal.WorkspaceId,
                journal.SaveOperationId,
                RecoverSaveStatus.StillUnknown,
                journal.SaveBaseRevision,
                repairRequired: false,
                null,
                null,
                Unexpected("Recovery artifact inspection", exception));
        }
    }

    /// <summary>Classifies the current complete output set against exact old and prepared identities.</summary>
    /// <param name="journal">The recognized save journal.</param>
    /// <param name="actual">The freshly captured complete output baseline.</param>
    /// <returns>The exact all-old, all-new, owned-mixed, or foreign state.</returns>
    private static SavePhysicalState ClassifyPhysicalState(
        SaveTransactionJournal journal,
        OutputArtifactSetBaseline actual)
    {
        if (journal.Phase == SaveTransactionPhase.Preparing)
        {
            return PluginSaveArtifactUtilities.MatchBaseline(journal.BeforeBaseline, actual)
                ? SavePhysicalState.AllBefore
                : SavePhysicalState.ForeignOrUnknown;
        }

        if (journal.Disposition == PluginWriteDisposition.Unchanged
            && journal.Phase == SaveTransactionPhase.Prepared)
        {
            return PluginSaveArtifactUtilities.MatchBaseline(journal.BeforeBaseline, actual)
                ? SavePhysicalState.AllPrepared
                : SavePhysicalState.ForeignOrUnknown;
        }

        if (journal.ArtifactPlans.Count == 0)
        {
            return SavePhysicalState.ForeignOrUnknown;
        }

        if (actual.Artifacts.Count != journal.ArtifactPlans.Count)
        {
            return SavePhysicalState.ForeignOrUnknown;
        }

        var actualByPath = actual.Artifacts.ToDictionary(artifact => artifact.Path, PluginSaveArtifactUtilities.PathComparer);
        var sawBefore = false;
        var sawPrepared = false;
        for (var index = 0; index < journal.ArtifactPlans.Count; index++)
        {
            var plan = journal.ArtifactPlans[index];
            if (!actualByPath.TryGetValue(plan.Before.Path, out var observed))
            {
                return SavePhysicalState.ForeignOrUnknown;
            }

            if (!IsRecognizedDestinationArtifact(journal, index, observed))
            {
                return SavePhysicalState.ForeignOrUnknown;
            }

            if (ArtifactContentMatches(plan.Before, plan.Staged))
            {
                continue;
            }

            if (ArtifactContentMatches(plan.Before, observed))
            {
                sawBefore = true;
            }
            else if (ArtifactContentMatches(plan.Staged, observed))
            {
                sawPrepared = true;
            }
            else
            {
                return SavePhysicalState.ForeignOrUnknown;
            }
        }

        if (sawBefore && sawPrepared)
        {
            return SavePhysicalState.OwnedMixed;
        }

        return sawPrepared ? SavePhysicalState.AllPrepared : SavePhysicalState.AllBefore;
    }

    /// <summary>Creates the stable unknown result for destination or owned-evidence conflict.</summary>
    /// <param name="journal">The recognized save journal.</param>
    /// <returns>An unknown result without repair authority or evidence token.</returns>
    private static RecoverSaveResult UnknownExternalChange(SaveTransactionJournal journal)
    {
        return new RecoverSaveResult(
            journal.WorkspaceId,
            journal.SaveOperationId,
            RecoverSaveStatus.StillUnknown,
            journal.SaveBaseRevision,
            repairRequired: false,
            null,
            null,
            new EngineError(
                EngineErrorCode.ExternalChangeDetected,
                "The destination or transaction-owned evidence is missing, inaccessible, or differs from the recognized save transaction."));
    }

    /// <summary>Determines whether one destination observation has an exact identity durably owned by the save or a repair attempt.</summary>
    /// <param name="journal">The recognized save journal.</param>
    /// <param name="artifactIndex">The stable artifact-plan index.</param>
    /// <param name="observed">The fresh destination observation.</param>
    /// <returns><see langword="true"/> when the observation exactly matches a journaled destination state.</returns>
    private static bool IsRecognizedDestinationArtifact(
        SaveTransactionJournal journal,
        int artifactIndex,
        PluginArtifactAssociation observed)
    {
        var savePlan = journal.ArtifactPlans[artifactIndex];
        if (PluginSaveArtifactUtilities.MatchExact([savePlan.Before], [observed])
            || PluginSaveArtifactUtilities.MatchExact([CreatePreparedDestination(savePlan)], [observed]))
        {
            return true;
        }

        foreach (var attempt in journal.RepairAttempts)
        {
            if (attempt.ArtifactPlans.Count == journal.ArtifactPlans.Count)
            {
                var repairPlan = attempt.ArtifactPlans[artifactIndex];
                if (PluginSaveArtifactUtilities.MatchExact([repairPlan.Current], [observed])
                    || PluginSaveArtifactUtilities.MatchExact([repairPlan.Target], [observed]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>Creates the expected destination association after one prepared plan is published.</summary>
    /// <param name="plan">The artifact plan.</param>
    /// <returns>The expected destination observation.</returns>
    private static PluginArtifactAssociation CreatePreparedDestination(SaveArtifactPlan plan)
    {
        if (ArtifactContentMatches(plan.Before, plan.Staged))
        {
            return plan.Before;
        }

        return new PluginArtifactAssociation(
            plan.Before.Path,
            plan.Before.Role,
            plan.Before.Language,
            plan.Staged.Fingerprint,
            plan.Publish?.FileIdentity);
    }

    /// <summary>Creates the complete expected destination baseline for a prepared set.</summary>
    /// <param name="journal">The recognized save journal.</param>
    /// <returns>The expected prepared baseline.</returns>
    private static OutputArtifactSetBaseline CreatePreparedBaseline(SaveTransactionJournal journal)
    {
        return PluginSaveArtifactUtilities.CreateOutputBaseline(
            journal.ArtifactPlans.Select(CreatePreparedDestination).ToArray());
    }

    /// <summary>Creates the exact terminal journal represented by physically resolved output evidence.</summary>
    /// <param name="journal">The recognized current journal.</param>
    /// <param name="status">The physically established terminal status.</param>
    /// <param name="terminalBaseline">The fresh exact terminal output baseline.</param>
    /// <returns>The existing terminal journal or a deterministic terminal projection of the nonterminal journal.</returns>
    private static SaveTransactionJournal ProjectTerminalJournal(
        SaveTransactionJournal journal,
        RecoverSaveStatus status,
        OutputArtifactSetBaseline terminalBaseline)
    {
        if (journal.Phase is SaveTransactionPhase.Committed or SaveTransactionPhase.NotCommitted)
        {
            return journal;
        }

        var phase = status switch
        {
            RecoverSaveStatus.Committed => SaveTransactionPhase.Committed,
            RecoverSaveStatus.NotCommitted => SaveTransactionPhase.NotCommitted,
            _ => throw new InvalidDataException("Only physically terminal recovery evidence can project a terminal save journal.")
        };
        return journal.WithProgress(phase, journal.ArtifactPlans.Count, terminalBaseline);
    }

    /// <summary>Creates a stable token from the exact terminal journal that explicit evidence validation will persist.</summary>
    /// <param name="journal">The recognized current journal.</param>
    /// <param name="status">The physically established terminal status.</param>
    /// <param name="terminalBaseline">The fresh exact terminal output baseline.</param>
    /// <param name="observations">The complete current source, output, and transaction-owned artifact observations.</param>
    /// <returns>The evidence token bound to the deterministic terminal projection and current observations.</returns>
    private static RecoveryEvidenceToken CreateProjectedTerminalEvidenceToken(
        SaveTransactionJournal journal,
        RecoverSaveStatus status,
        OutputArtifactSetBaseline terminalBaseline,
        IReadOnlyList<PluginArtifactAssociation> observations)
    {
        return PluginSaveArtifactUtilities.CreateEvidenceToken(
            ProjectTerminalJournal(journal, status, terminalBaseline),
            observations);
    }

    /// <summary>Recaptures every staged, publication, backup, and retirement path recorded by the transaction.</summary>
    /// <param name="journal">The recognized save journal.</param>
    /// <param name="cancellationToken">The token checked during hashing.</param>
    /// <returns>The current owned-file observations in stable plan order.</returns>
    private static async Task<IReadOnlyList<PluginArtifactAssociation>> ObserveOwnedArtifactsAsync(
        SaveTransactionJournal journal,
        CancellationToken cancellationToken)
    {
        var expected = new List<PluginArtifactAssociation>();
        foreach (var plan in journal.ArtifactPlans)
        {
            expected.Add(plan.Staged);
            if (plan.Publish is not null)
            {
                expected.Add(plan.Publish);
            }

            if (plan.Backup is not null)
            {
                expected.Add(plan.Backup);
            }

            expected.Add(new PluginArtifactAssociation(
                plan.RetiredPath,
                plan.Before.Role,
                plan.Before.Language,
                plan.Before.Fingerprint,
                plan.Before.FileIdentity));
        }

        foreach (var attempt in journal.RepairAttempts)
        {
            foreach (var plan in attempt.ArtifactPlans)
            {
                if (plan.Publish is not null)
                {
                    expected.Add(plan.Publish);
                }

                if (plan.Retired is not null)
                {
                    expected.Add(plan.Retired);
                }
            }
        }

        return await PluginSaveArtifactUtilities.RecaptureAsync(expected, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Checks that every required staged or backup source remains exact for explicit repair.</summary>
    /// <param name="journal">The recognized save journal.</param>
    /// <param name="observed">All owned observations returned in stable plan order.</param>
    /// <returns><see langword="true"/> when each required source remains exact or has already moved to its recognized destination.</returns>
    private static bool OwnedRepairFilesMatch(
        SaveTransactionJournal journal,
        IReadOnlyList<PluginArtifactAssociation> observed)
    {
        var observedByPath = observed.ToDictionary(artifact => artifact.Path, PluginSaveArtifactUtilities.PathComparer);
        foreach (var plan in journal.ArtifactPlans)
        {
            if (!observedByPath.TryGetValue(plan.Staged.Path, out var staged)
                || !PluginSaveArtifactUtilities.MatchExact([plan.Staged], [staged]))
            {
                return false;
            }

            if (plan.Backup is not null
                && (!observedByPath.TryGetValue(plan.Backup.Path, out var backup)
                    || !PluginSaveArtifactUtilities.MatchExact([plan.Backup], [backup])))
            {
                return false;
            }

            if (plan.Publish is not null
                && observedByPath.TryGetValue(plan.Publish.Path, out var publish)
                && publish.Fingerprint.Exists
                && !PluginSaveArtifactUtilities.MatchExact([plan.Publish], [publish]))
            {
                return false;
            }

            var retired = new PluginArtifactAssociation(
                plan.RetiredPath,
                plan.Before.Role,
                plan.Before.Language,
                plan.Before.Fingerprint,
                plan.Before.FileIdentity);
            if (observedByPath.TryGetValue(retired.Path, out var retiredNow)
                && retiredNow.Fingerprint.Exists
                && !PluginSaveArtifactUtilities.MatchExact([retired], [retiredNow]))
            {
                return false;
            }
        }

        foreach (var attempt in journal.RepairAttempts)
        {
            foreach (var plan in attempt.ArtifactPlans)
            {
                if (plan.Publish is not null
                    && observedByPath.TryGetValue(plan.Publish.Path, out var publish)
                    && publish.Fingerprint.Exists
                    && !PluginSaveArtifactUtilities.MatchExact([plan.Publish], [publish]))
                {
                    return false;
                }

                if (plan.Retired is not null
                    && observedByPath.TryGetValue(plan.Retired.Path, out var retired)
                    && retired.Fingerprint.Exists
                    && !PluginSaveArtifactUtilities.MatchExact([plan.Retired], [retired]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>Creates terminal resolved evidence from canonical journal and fresh current observations.</summary>
    /// <param name="journal">The recognized save journal.</param>
    /// <param name="status">The terminal physical status.</param>
    /// <param name="output">The fresh complete output baseline.</param>
    /// <param name="token">The token bound to current evidence.</param>
    /// <returns>The canonical resolved evidence.</returns>
    private static ResolvedOutputEvidence CreateResolvedEvidence(
        SaveTransactionJournal journal,
        RecoverSaveStatus status,
        OutputArtifactSetBaseline output,
        RecoveryEvidenceToken token)
    {
        return new ResolvedOutputEvidence(
            token,
            journal.Game,
            journal.Release,
            journal.WorkspaceId,
            journal.SaveOperationId,
            journal.SaveBaseRevision,
            journal.SourceBaseline,
            journal.Output,
            output,
            status);
    }

    /// <summary>Creates a terminal historical result whose current sources prevent adoption.</summary>
    /// <param name="journal">The recognized save journal.</param>
    /// <param name="status">The physically established terminal status.</param>
    /// <param name="token">The reviewed evidence token.</param>
    /// <returns>The terminal result with a typed current conflict.</returns>
    private static RecoverSaveResult TerminalConflict(
        SaveTransactionJournal journal,
        RecoverSaveStatus status,
        RecoveryEvidenceToken token)
    {
        return new RecoverSaveResult(
            journal.WorkspaceId,
            journal.SaveOperationId,
            status,
            journal.SaveBaseRevision,
            repairRequired: false,
            token,
            null,
            new EngineError(
                EngineErrorCode.ExternalChangeDetected,
                "The destination outcome is physically established, but the original source baseline is no longer currently adoptable."));
    }

    /// <summary>Checks every caller-supplied evidence claim against the recognized journal.</summary>
    /// <param name="evidence">The untrusted supplied evidence.</param>
    /// <param name="journal">The canonical recognized journal.</param>
    /// <returns><see langword="true"/> when every durable claim matches.</returns>
    private static bool EvidenceClaimsMatchJournal(
        ResolvedOutputEvidence evidence,
        SaveTransactionJournal journal)
    {
        return evidence.OriginalWorkspaceId == journal.WorkspaceId
            && evidence.SaveOperationId == journal.SaveOperationId
            && evidence.SaveBaseRevision == journal.SaveBaseRevision
            && evidence.Game == journal.Game
            && evidence.Release == journal.Release
            && PluginSaveArtifactUtilities.MatchSourceBaseline(evidence.SourceBaseline, journal.SourceBaseline)
            && PluginSaveArtifactUtilities.MatchOutput(evidence.Output, journal.Output);
    }

    /// <summary>Creates a pending-save contract from one recognized nonterminal journal.</summary>
    /// <param name="journal">The recognized nonterminal journal.</param>
    /// <returns>The immutable pending-save identity.</returns>
    private static PendingSaveIdentity CreatePendingSave(SaveTransactionJournal journal)
    {
        return new PendingSaveIdentity(
            journal.WorkspaceId,
            journal.SaveOperationId,
            journal.SaveBaseRevision,
            journal.Game,
            journal.Release,
            journal.Output);
    }

    /// <summary>Creates the required missing-evidence recovery result.</summary>
    /// <param name="request">The original recovery request.</param>
    /// <param name="error">The typed absence or inspection failure.</param>
    /// <returns>A stable unknown result without revision, repair, token, or evidence.</returns>
    private static RecoverSaveResult NoRecovery(RecoverSaveRequest request, EngineError error)
    {
        return new RecoverSaveResult(
            request.WorkspaceId,
            request.SaveOperationId,
            RecoverSaveStatus.StillUnknown,
            null,
            repairRequired: false,
            null,
            null,
            error);
    }

    /// <summary>Describes exact physical destination classification for a recognized journal.</summary>
    private enum SavePhysicalState
    {
        /// <summary>Every destination artifact matches the original baseline.</summary>
        AllBefore,

        /// <summary>Every destination artifact matches the prepared publication set.</summary>
        AllPrepared,

        /// <summary>Every artifact is recognized as old or prepared, with both states present.</summary>
        OwnedMixed,

        /// <summary>At least one artifact is missing from the complete shape, foreign, or inaccessible.</summary>
        ForeignOrUnknown
    }
}
