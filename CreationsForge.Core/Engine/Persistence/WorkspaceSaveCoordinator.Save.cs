using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;

namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Implements guarded staging and destination publication for workspace saves.</summary>
public sealed partial class WorkspaceSaveCoordinator
{
    /// <inheritdoc />
    public async ValueTask<SaveResult> SaveAsync(
        WorkspaceSaveContext context,
        SaveRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(request);
        if (context.Revision != request.ExpectedRevision)
        {
            return SaveFailure(context, request, EngineErrorCode.RevisionConflict, "The workspace revision changed before guarded save coordination.");
        }

        if (!NativeSaveArtifactUtilities.MatchBaseline(context.OutputBaseline, request.ExpectedOutputBaseline))
        {
            return SaveFailure(context, request, EngineErrorCode.ExternalChangeDetected, "The save request output baseline differs from the borrowed workspace baseline.");
        }

        string outputDirectory;
        try
        {
            outputDirectory = GetOutputDirectory(context.OutputAssociation);
        }
        catch (Exception exception)
        {
            return SaveFailure(context, request, EngineErrorCode.InvalidRequest, exception.Message);
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
            return SaveFailure(context, request, EngineErrorCode.ValidationFailed, "The save was canceled before destination mutation.");
        }
        catch (Exception exception)
        {
            return SaveFailure(context, request, EngineErrorCode.OutputDirectoryBusy, exception.Message);
        }

        if (!acquisition.Succeeded
            || acquisition.Value?.Status != OutputDirectoryLeaseAcquisitionStatus.Acquired
            || acquisition.Value.Lease is null)
        {
            return SaveFailure(
                context,
                request,
                acquisition.Error?.Code ?? EngineErrorCode.OutputDirectoryBusy,
                acquisition.Error?.Message ?? "The exclusive output-directory lease could not be acquired.");
        }

        await using var lease = acquisition.Value.Lease;
        SaveTransactionJournal? journal = null;
        var destinationMutationCount = 0;
        var warnings = new List<EngineWarning>();
        try
        {
            ValidateLease(lease, context.OutputAssociation);
            var paths = new SaveTransactionPaths(outputDirectory, context.WorkspaceId, request.OperationId);
            var fingerprint = CreateSaveFingerprint(context, request);
            var existing = await TransactionStore.ReadAsync(paths, cancellationToken).ConfigureAwait(false);
            if (existing is not null)
            {
                if (!string.Equals(existing.RequestFingerprint, fingerprint, StringComparison.Ordinal))
                {
                    return SaveFailure(context, request, EngineErrorCode.OperationIdReuse, "The save operation identifier was reused with a different canonical payload.");
                }
            }

            var inventory = await TransactionStore.ReadAllAsync(outputDirectory, cancellationToken).ConfigureAwait(false);
            if (existing is not null)
            {
                return await ReplaySaveAsync(existing, cancellationToken).ConfigureAwait(false);
            }

            if (!TransactionStore.HasCapacityForNewTransaction(inventory, context.WorkspaceId))
            {
                return SaveFailure(
                    context,
                    request,
                    EngineErrorCode.ValidationFailed,
                    "The bounded save transaction metadata capacity is exhausted.");
            }

            var admissionFailure = await ResolvePriorTransactionsBeforeSaveAsync(
                inventory,
                context.OutputAssociation,
                request.OperationId,
                cancellationToken).ConfigureAwait(false);
            if (admissionFailure is not null)
            {
                return SaveFailure(context, request, admissionFailure.Code, admissionFailure.Message);
            }

            var sourceBefore = await context.Sources.VerifyUnchangedAsync(cancellationToken).ConfigureAwait(false);
            if (!sourceBefore.Succeeded
                || sourceBefore.Value is null
                || !NativeSaveArtifactUtilities.MatchSourceBaseline(context.Sources.Baseline, sourceBefore.Value))
            {
                return SaveFailure(
                    context,
                    request,
                    sourceBefore.Error?.Code ?? EngineErrorCode.ExternalChangeDetected,
                    sourceBefore.Error?.Message ?? "The native source baseline changed before staging.");
            }

            var outputBefore = await NativeSaveArtifactUtilities.CaptureOutputAsync(
                context.Release,
                context.OutputAssociation,
                cancellationToken).ConfigureAwait(false);
            if (!NativeSaveArtifactUtilities.MatchBaseline(context.OutputBaseline, outputBefore))
            {
                return SaveFailure(context, request, EngineErrorCode.ExternalChangeDetected, "The complete output artifact set changed before staging.");
            }

            var initialJournal = new SaveTransactionJournal(
                context.WorkspaceId,
                request.OperationId,
                fingerprint,
                context.Revision,
                context.Game,
                context.Release,
                context.Sources.Baseline,
                context.OutputAssociation,
                outputBefore,
                NativeWriteDisposition.StagedChanges,
                SaveTransactionPhase.Preparing,
                0,
                Array.Empty<SaveArtifactPlan>(),
                null,
                Array.Empty<SaveRepairAttempt>());
            await TransactionStore.InitializeAsync(paths, initialJournal, cancellationToken).ConfigureAwait(false);
            journal = initialJournal;
            TransactionStore.CreateEmptySubdirectory(paths.StagingDirectoryPath, "private native save staging directory");

            var stagedResult = await context.Adapter.WriteAndValidateAsync(
                context.Sources,
                context.Output,
                new NativeWriteRequest(paths.StagingDirectoryPath, context.OutputAssociation, context.OutputBaseline),
                cancellationToken).ConfigureAwait(false);
            warnings.AddRange(stagedResult.Warnings);
            if (!stagedResult.Succeeded || stagedResult.Value is null)
            {
                journal = await FinalizeNotCommittedAsync(paths, journal, context.Release, CancellationToken.None).ConfigureAwait(false);
                return SaveFailure(
                    context,
                    request,
                    stagedResult.Error?.Code ?? EngineErrorCode.ValidationFailed,
                    stagedResult.Error?.Message ?? "Native staging and reopen validation failed.",
                    warnings);
            }

            var staged = stagedResult.Value;
            await using var stagedLifetime = staged.StagedOutput;
            if (staged.Disposition == NativeWriteDisposition.Unchanged)
            {
                if (!journal.BeforeBaseline.Artifacts.Single(artifact => artifact.Role == NativeArtifactRole.Plugin).Fingerprint.Exists)
                {
                    journal = await FinalizeNotCommittedAsync(paths, journal, context.Release, CancellationToken.None).ConfigureAwait(false);
                    return SaveFailure(context, request, EngineErrorCode.ValidationFailed, "A native no-op save requires an existing complete output.", warnings);
                }

                var sourceNow = await context.Sources.VerifyUnchangedAsync(cancellationToken).ConfigureAwait(false);
                var outputNow = await NativeSaveArtifactUtilities.CaptureOutputAsync(
                    context.Release,
                    context.OutputAssociation,
                    cancellationToken).ConfigureAwait(false);
                if (!sourceNow.Succeeded
                    || sourceNow.Value is null
                    || !NativeSaveArtifactUtilities.MatchSourceBaseline(context.Sources.Baseline, sourceNow.Value)
                    || !NativeSaveArtifactUtilities.MatchBaseline(journal.BeforeBaseline, outputNow))
                {
                    journal = await FinalizeNotCommittedAsync(paths, journal, context.Release, CancellationToken.None).ConfigureAwait(false);
                    return SaveFailure(
                        context,
                        request,
                        sourceNow.Error?.Code ?? EngineErrorCode.ExternalChangeDetected,
                        sourceNow.Error?.Message ?? "The native source or complete output changed while validating a no-op save.",
                        warnings);
                }

                journal = journal.WithPreparedArtifacts(NativeWriteDisposition.Unchanged, Array.Empty<SaveArtifactPlan>());
                await TransactionStore.WriteAsync(paths, journal, CancellationToken.None).ConfigureAwait(false);
                journal = journal.WithProgress(SaveTransactionPhase.Committed, 0, outputNow);
                await TransactionStore.WriteAsync(paths, journal, CancellationToken.None).ConfigureAwait(false);
                return await CreateCommittedSaveResultAsync(context, request, journal, outputNow, warnings, cancellationToken).ConfigureAwait(false);
            }

            var plans = await PrepareArtifactPlansAsync(
                paths,
                journal.BeforeBaseline,
                staged.ArtifactMappings,
                context.Sources.Baseline,
                cancellationToken).ConfigureAwait(false);
            journal = journal.WithPreparedArtifacts(NativeWriteDisposition.StagedChanges, plans);
            await TransactionStore.WriteAsync(paths, journal, cancellationToken).ConfigureAwait(false);

            var sourceImmediatelyBefore = await context.Sources.VerifyUnchangedAsync(cancellationToken).ConfigureAwait(false);
            var outputImmediatelyBefore = await NativeSaveArtifactUtilities.CaptureOutputAsync(
                context.Release,
                context.OutputAssociation,
                cancellationToken).ConfigureAwait(false);
            if (!sourceImmediatelyBefore.Succeeded
                || sourceImmediatelyBefore.Value is null
                || !NativeSaveArtifactUtilities.MatchSourceBaseline(context.Sources.Baseline, sourceImmediatelyBefore.Value)
                || !NativeSaveArtifactUtilities.MatchBaseline(journal.BeforeBaseline, outputImmediatelyBefore))
            {
                journal = await FinalizeNotCommittedAsync(paths, journal, context.Release, CancellationToken.None).ConfigureAwait(false);
                return SaveFailure(
                    context,
                    request,
                    sourceImmediatelyBefore.Error?.Code ?? EngineErrorCode.ExternalChangeDetected,
                    sourceImmediatelyBefore.Error?.Message ?? "The native source or complete output changed immediately before publication.",
                    warnings);
            }

            cancellationToken.ThrowIfCancellationRequested();
            journal = journal.WithProgress(SaveTransactionPhase.MutationStarted, 0);
            await TransactionStore.WriteAsync(paths, journal, CancellationToken.None).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            for (var index = 0; index < journal.ArtifactPlans.Count; index++)
            {
                var plan = journal.ArtifactPlans[index];
                await VerifyExpectedProgressAsync(journal, index, CancellationToken.None).ConfigureAwait(false);
                if (!ArtifactContentMatches(plan.Before, plan.Staged))
                {
                    FileOperations.BeforeDestinationMutation(journal.SaveOperationId, index);
                    if (destinationMutationCount == 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    await VerifyExpectedProgressAsync(journal, index, CancellationToken.None).ConfigureAwait(false);
                    await VerifyPublicationSourceAsync(plan, CancellationToken.None).ConfigureAwait(false);

                    if (plan.Staged.Fingerprint.Exists)
                    {
                        EnsureDestinationParent(plan.Before.Path, outputDirectory);
                        FileOperations.Publish(plan.Publish!.Path, plan.Before.Path);
                    }
                    else if (plan.Before.Fingerprint.Exists)
                    {
                        FileOperations.Retire(plan.Before.Path, plan.RetiredPath);
                    }

                    destinationMutationCount++;
                    FileOperations.AfterDestinationMutation(journal.SaveOperationId, index);
                }

                journal = journal.WithProgress(SaveTransactionPhase.MutationStarted, index + 1);
                await TransactionStore.WriteAsync(paths, journal, CancellationToken.None).ConfigureAwait(false);
            }

            var committedBaseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(
                context.Release,
                context.OutputAssociation,
                CancellationToken.None).ConfigureAwait(false);
            var preparedBaseline = CreatePreparedBaseline(journal);
            if (!NativeSaveArtifactUtilities.MatchBaseline(preparedBaseline, committedBaseline))
            {
                return await CreateUnknownSaveResultAsync(context, request, journal, warnings).ConfigureAwait(false);
            }

            journal = journal.WithProgress(
                SaveTransactionPhase.Committed,
                journal.ArtifactPlans.Count,
                committedBaseline);
            await TransactionStore.WriteAsync(paths, journal, CancellationToken.None).ConfigureAwait(false);
            return await CreateCommittedSaveResultAsync(
                context,
                request,
                journal,
                committedBaseline,
                warnings,
                CancellationToken.None).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            if (journal is not null && destinationMutationCount == 0)
            {
                try
                {
                    var paths = new SaveTransactionPaths(outputDirectory, context.WorkspaceId, request.OperationId);
                    journal = await FinalizeNotCommittedAsync(paths, journal, context.Release, CancellationToken.None).ConfigureAwait(false);
                }
                catch
                {
                    return await CreateUnknownSaveResultAsync(context, request, journal, warnings).ConfigureAwait(false);
                }

                return SaveFailure(context, request, EngineErrorCode.ValidationFailed, "The save was canceled before destination mutation.", warnings);
            }

            return journal is null
                ? SaveFailure(context, request, EngineErrorCode.ValidationFailed, "The save was canceled before destination mutation.", warnings)
                : await CreateUnknownSaveResultAsync(context, request, journal, warnings).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            if (journal is not null && destinationMutationCount > 0)
            {
                return await CreateUnknownSaveResultAsync(context, request, journal, warnings, exception).ConfigureAwait(false);
            }

            if (journal is not null)
            {
                try
                {
                    var paths = new SaveTransactionPaths(outputDirectory, context.WorkspaceId, request.OperationId);
                    await FinalizeNotCommittedAsync(paths, journal, context.Release, CancellationToken.None).ConfigureAwait(false);
                }
                catch
                {
                    return await CreateUnknownSaveResultAsync(context, request, journal, warnings, exception).ConfigureAwait(false);
                }
            }

            return SaveFailure(context, request, EngineErrorCode.UnexpectedFailure, exception.Message, warnings);
        }
    }

    /// <summary>Prepares complete flushed publication and backup files for every validated staged mapping.</summary>
    /// <param name="paths">The exact transaction paths.</param>
    /// <param name="beforeBaseline">The fresh complete destination baseline.</param>
    /// <param name="mappings">The adapter's complete staged mappings.</param>
    /// <param name="sourceBaseline">The immutable source baseline used for alias checks.</param>
    /// <param name="cancellationToken">The token honored before destination mutation.</param>
    /// <returns>The complete immutable artifact plan.</returns>
    private async Task<IReadOnlyList<SaveArtifactPlan>> PrepareArtifactPlansAsync(
        SaveTransactionPaths paths,
        OutputArtifactSetBaseline beforeBaseline,
        IReadOnlyList<NativeStagedArtifactMapping> mappings,
        NativeSourceInputBaseline sourceBaseline,
        CancellationToken cancellationToken)
    {
        if (mappings.Count != beforeBaseline.Artifacts.Count)
        {
            throw new InvalidDataException("The native staged mapping does not cover the complete output artifact association.");
        }

        var mappingByDestination = mappings.ToDictionary(
            mapping => Path.GetFullPath(mapping.DestinationPath),
            NativeSaveArtifactUtilities.PathComparer);
        if (mappingByDestination.Count != mappings.Count)
        {
            throw new InvalidDataException("The native staged mapping contains duplicate destination paths.");
        }

        TransactionStore.CreateEmptySubdirectory(paths.PublishDirectoryPath, "save publication directory");
        TransactionStore.CreateEmptySubdirectory(paths.BackupDirectoryPath, "save backup directory");
        TransactionStore.CreateEmptySubdirectory(paths.RetiredDirectoryPath, "save retirement directory");
        var plans = new List<SaveArtifactPlan>(beforeBaseline.Artifacts.Count);
        var anyContentChange = false;
        for (var index = 0; index < beforeBaseline.Artifacts.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var before = beforeBaseline.Artifacts[index];
            NativeSaveArtifactUtilities.RequireDescendant(before.Path, paths.OutputDirectoryPath, "output artifact");
            if (!mappingByDestination.TryGetValue(before.Path, out var mapping))
            {
                throw new InvalidDataException($"The native staged set omitted output artifact '{before.Path}'.");
            }

            NativeSaveArtifactUtilities.RequireDescendant(mapping.StagedArtifact.Path, paths.StagingDirectoryPath, "staged artifact");
            if (mapping.StagedArtifact.Role != before.Role
                || !string.Equals(mapping.StagedArtifact.Language, before.Language, StringComparison.Ordinal))
            {
                throw new InvalidDataException("A native staged artifact role or language differs from its complete destination association.");
            }

            var staged = await NativeFileInspector.InspectAsync(
                mapping.StagedArtifact.Path,
                mapping.StagedArtifact.Role,
                mapping.StagedArtifact.Language,
                mustExist: false,
                cancellationToken).ConfigureAwait(false);
            if (!NativeSaveArtifactUtilities.MatchExact([mapping.StagedArtifact], [staged]))
            {
                throw new InvalidDataException($"The native staged artifact changed after adapter validation: '{mapping.StagedArtifact.Path}'.");
            }

            NativeArtifactAssociation? publish = null;
            if (staged.Fingerprint.Exists)
            {
                var publishPath = Path.Combine(paths.PublishDirectoryPath, $"{index:D4}.bin");
                await NativeSaveArtifactUtilities.CopyAndFlushAsync(staged.Path, publishPath, cancellationToken).ConfigureAwait(false);
                publish = await NativeFileInspector.InspectAsync(
                    publishPath,
                    staged.Role,
                    staged.Language,
                    mustExist: true,
                    cancellationToken).ConfigureAwait(false);
                if (!ArtifactContentMatches(staged, publish))
                {
                    throw new InvalidDataException("A flushed publication copy differs from its natively validated staged artifact.");
                }
            }

            NativeArtifactAssociation? backup = null;
            if (before.Fingerprint.Exists)
            {
                var backupPath = Path.Combine(paths.BackupDirectoryPath, $"{index:D4}.bin");
                await NativeSaveArtifactUtilities.CopyAndFlushAsync(before.Path, backupPath, cancellationToken).ConfigureAwait(false);
                backup = await NativeFileInspector.InspectAsync(
                    backupPath,
                    before.Role,
                    before.Language,
                    mustExist: true,
                    cancellationToken).ConfigureAwait(false);
                if (!ArtifactContentMatches(before, backup))
                {
                    throw new InvalidDataException("A flushed backup copy differs from its exact pre-save destination artifact.");
                }
            }

            var retiredPath = Path.Combine(paths.RetiredDirectoryPath, $"{index:D4}.bin");
            if (File.Exists(retiredPath) || Directory.Exists(retiredPath))
            {
                throw new InvalidDataException($"The transaction retirement path is not empty: '{retiredPath}'.");
            }

            anyContentChange |= !ArtifactContentMatches(before, staged);
            plans.Add(new SaveArtifactPlan(before, staged, publish, backup, retiredPath));
        }

        if (!anyContentChange)
        {
            throw new InvalidDataException("The adapter reported staged changes although every output artifact is content-identical.");
        }

        ValidateOwnedIdentities(plans, sourceBaseline);
        return plans.AsReadOnly();
    }

    /// <summary>Verifies the exact expected old/new state and owned files immediately before an artifact step.</summary>
    /// <param name="journal">The complete prepared journal.</param>
    /// <param name="nextIndex">The next artifact-plan index.</param>
    /// <param name="cancellationToken">The token used for post-boundary bookkeeping inspection.</param>
    private static async Task VerifyExpectedProgressAsync(
        SaveTransactionJournal journal,
        int nextIndex,
        CancellationToken cancellationToken)
    {
        var output = await NativeSaveArtifactUtilities.CaptureOutputAsync(
            journal.Release,
            journal.Output,
            cancellationToken).ConfigureAwait(false);
        if (output.Artifacts.Count != journal.ArtifactPlans.Count)
        {
            throw new InvalidDataException("The complete output association changed during guarded publication.");
        }

        var actual = output.Artifacts.ToDictionary(artifact => artifact.Path, NativeSaveArtifactUtilities.PathComparer);
        for (var index = 0; index < journal.ArtifactPlans.Count; index++)
        {
            var plan = journal.ArtifactPlans[index];
            var expected = index < nextIndex ? CreatePreparedDestination(plan) : plan.Before;
            if (!actual.TryGetValue(plan.Before.Path, out var observed)
                || !NativeSaveArtifactUtilities.MatchExact([expected], [observed]))
            {
                throw new InvalidDataException($"Output artifact '{plan.Before.Path}' changed outside the guarded publication plan.");
            }
        }
    }

    /// <summary>Verifies the exact publication or retirement source immediately before its destination move.</summary>
    /// <param name="plan">The artifact plan about to mutate its destination.</param>
    /// <param name="cancellationToken">The post-boundary bookkeeping token.</param>
    /// <returns>A task that completes when the source path still matches its durable identity.</returns>
    private static async Task VerifyPublicationSourceAsync(
        SaveArtifactPlan plan,
        CancellationToken cancellationToken)
    {
        if (plan.Publish is not null)
        {
            var publish = await NativeFileInspector.InspectAsync(
                plan.Publish.Path,
                plan.Publish.Role,
                plan.Publish.Language,
                mustExist: true,
                cancellationToken).ConfigureAwait(false);
            if (!NativeSaveArtifactUtilities.MatchExact([plan.Publish], [publish]))
            {
                throw new InvalidDataException($"Publication artifact '{plan.Publish.Path}' changed before its destination move.");
            }

            return;
        }

        if (File.Exists(plan.RetiredPath) || Directory.Exists(plan.RetiredPath))
        {
            throw new InvalidDataException($"Retirement artifact path '{plan.RetiredPath}' became occupied before its destination move.");
        }
    }

    /// <summary>Finalizes a physically unchanged pre-boundary transaction as not committed.</summary>
    /// <param name="paths">The transaction paths.</param>
    /// <param name="journal">The current complete journal.</param>
    /// <param name="release">The exact native release.</param>
    /// <param name="cancellationToken">The bookkeeping token.</param>
    /// <returns>The terminal not-committed journal.</returns>
    private async Task<SaveTransactionJournal> FinalizeNotCommittedAsync(
        SaveTransactionPaths paths,
        SaveTransactionJournal journal,
        Mutagen.Bethesda.GameRelease release,
        CancellationToken cancellationToken)
    {
        var output = await NativeSaveArtifactUtilities.CaptureOutputAsync(
            release,
            journal.Output,
            cancellationToken).ConfigureAwait(false);
        if (!NativeSaveArtifactUtilities.MatchBaseline(journal.BeforeBaseline, output))
        {
            throw new InvalidDataException("A pre-boundary save failure could not prove that the complete output remained unchanged.");
        }

        var terminal = journal.WithProgress(SaveTransactionPhase.NotCommitted, journal.MutationProgress, output);
        await TransactionStore.WriteAsync(paths, terminal, CancellationToken.None).ConfigureAwait(false);
        return terminal;
    }

    /// <summary>Resolves physically terminal older transactions before a new save can change their evidence.</summary>
    /// <param name="inventory">The bounded transaction inventory captured while the output lease is held.</param>
    /// <param name="output">The output association about to be saved.</param>
    /// <param name="currentOperationId">The new save operation identifier.</param>
    /// <param name="cancellationToken">The token honored before destination mutation.</param>
    /// <returns>A blocking error, or <see langword="null"/> when admission is ready.</returns>
    private async Task<EngineError?> ResolvePriorTransactionsBeforeSaveAsync(
        SaveTransactionInventory inventory,
        OutputAssociation output,
        Guid currentOperationId,
        CancellationToken cancellationToken)
    {
        foreach (var entry in inventory.Journals)
        {
            var journal = entry.Journal;
            if (journal.SaveOperationId == currentOperationId
                || !NativeSaveArtifactUtilities.MatchOutput(journal.Output, output)
                || journal.Phase is SaveTransactionPhase.Committed or SaveTransactionPhase.NotCommitted)
            {
                continue;
            }

            var current = await NativeSaveArtifactUtilities.CaptureOutputAsync(
                journal.Release,
                journal.Output,
                cancellationToken).ConfigureAwait(false);
            var physical = ClassifyPhysicalState(journal, current);
            if (physical == SavePhysicalState.AllBefore)
            {
                var terminal = ProjectTerminalJournal(journal, RecoverSaveStatus.NotCommitted, current);
                await TransactionStore.WriteAsync(entry.Paths, terminal, CancellationToken.None).ConfigureAwait(false);
                continue;
            }

            if (physical == SavePhysicalState.AllPrepared)
            {
                var terminal = ProjectTerminalJournal(journal, RecoverSaveStatus.Committed, current);
                await TransactionStore.WriteAsync(entry.Paths, terminal, CancellationToken.None).ConfigureAwait(false);
                continue;
            }

            return new EngineError(
                EngineErrorCode.RepairRequired,
                $"Output admission is blocked by unresolved save '{journal.SaveOperationId:D}' from workspace '{journal.WorkspaceId:D}'.");
        }

        return null;
    }

    /// <summary>Creates a committed result after terminal evidence and a temporary native destination reopen.</summary>
    /// <param name="context">The borrowed workspace context.</param>
    /// <param name="request">The original save request.</param>
    /// <param name="journal">The terminal committed journal.</param>
    /// <param name="baseline">The complete committed output baseline.</param>
    /// <param name="warnings">The accumulated staging warnings.</param>
    /// <param name="cancellationToken">The token used only when the caller has not crossed mutation.</param>
    /// <returns>The committed or committed-but-reopen-failed result.</returns>
    private async Task<SaveResult> CreateCommittedSaveResultAsync(
        WorkspaceSaveContext context,
        SaveRequest request,
        SaveTransactionJournal journal,
        OutputArtifactSetBaseline baseline,
        List<EngineWarning> warnings,
        CancellationToken cancellationToken)
    {
        var recovery = await InspectRecoveryAsync(journal, cancellationToken).ConfigureAwait(false);
        var evidence = recovery.ResolvedEvidence;
        if (evidence is null || recovery.EvidenceToken is null)
        {
            return new SaveResult(
                context.WorkspaceId,
                request.OperationId,
                context.Revision,
                context.Revision,
                SaveCommitStatus.CommitOutcomeUnknown,
                null,
                recovery.EvidenceToken,
                null,
                recovery.Error ?? new EngineError(EngineErrorCode.CommitOutcomeUnknown, "Committed output evidence could not be reconstructed."),
                warnings);
        }

        var reopen = await context.Adapter.ReopenOutputAsync(
            context.Sources,
            context.OutputAssociation,
            baseline,
            cancellationToken).ConfigureAwait(false);
        warnings.AddRange(reopen.Warnings);
        if (!reopen.Succeeded || reopen.Value is null)
        {
            return new SaveResult(
                context.WorkspaceId,
                request.OperationId,
                context.Revision,
                context.Revision,
                SaveCommitStatus.CommittedButReopenFailed,
                baseline,
                recovery.EvidenceToken,
                evidence,
                reopen.Error ?? new EngineError(EngineErrorCode.OutputOpenFailed, "The committed native output could not be reopened."),
                warnings);
        }

        await reopen.Value.Output.DisposeAsync().ConfigureAwait(false);
        return new SaveResult(
            context.WorkspaceId,
            request.OperationId,
            context.Revision,
            context.Revision,
            SaveCommitStatus.Committed,
            baseline,
            recovery.EvidenceToken,
            evidence,
            null,
            warnings);
    }

    /// <summary>Reconstructs an exact idempotent result for a previously recognized save operation.</summary>
    /// <param name="journal">The recognized matching journal.</param>
    /// <param name="cancellationToken">The read-only inspection token.</param>
    /// <returns>The replayed save result.</returns>
    private static async Task<SaveResult> ReplaySaveAsync(
        SaveTransactionJournal journal,
        CancellationToken cancellationToken)
    {
        var recovery = await InspectRecoveryAsync(journal, cancellationToken).ConfigureAwait(false);
        return recovery.Status switch
        {
            RecoverSaveStatus.Committed => new SaveResult(
                journal.WorkspaceId,
                journal.SaveOperationId,
                journal.SaveBaseRevision,
                journal.SaveBaseRevision,
                SaveCommitStatus.Committed,
                journal.TerminalBaseline ?? recovery.ResolvedEvidence?.ResolvedOutputBaseline,
                recovery.EvidenceToken,
                recovery.ResolvedEvidence,
                recovery.Error,
                Array.Empty<EngineWarning>()),
            RecoverSaveStatus.NotCommitted => new SaveResult(
                journal.WorkspaceId,
                journal.SaveOperationId,
                journal.SaveBaseRevision,
                journal.SaveBaseRevision,
                SaveCommitStatus.NotCommitted,
                null,
                recovery.EvidenceToken,
                null,
                recovery.Error,
                Array.Empty<EngineWarning>()),
            _ => new SaveResult(
                journal.WorkspaceId,
                journal.SaveOperationId,
                journal.SaveBaseRevision,
                journal.SaveBaseRevision,
                SaveCommitStatus.CommitOutcomeUnknown,
                null,
                recovery.EvidenceToken,
                null,
                recovery.Error ?? new EngineError(EngineErrorCode.CommitOutcomeUnknown, "The original save outcome remains unknown."),
                Array.Empty<EngineWarning>())
        };
    }

    /// <summary>Creates an unknown save result from the freshest available recovery evidence.</summary>
    /// <param name="context">The borrowed save context.</param>
    /// <param name="request">The original request.</param>
    /// <param name="journal">The latest durable journal.</param>
    /// <param name="warnings">The accumulated warnings.</param>
    /// <param name="exception">The optional triggering exception.</param>
    /// <returns>The exact physically classified result when possible, otherwise unknown.</returns>
    private static async Task<SaveResult> CreateUnknownSaveResultAsync(
        WorkspaceSaveContext context,
        SaveRequest request,
        SaveTransactionJournal journal,
        IReadOnlyList<EngineWarning> warnings,
        Exception? exception = null)
    {
        var recovery = await InspectRecoveryAsync(journal, CancellationToken.None).ConfigureAwait(false);
        if (recovery.Status == RecoverSaveStatus.Committed && recovery.ResolvedEvidence is not null)
        {
            return new SaveResult(
                context.WorkspaceId,
                request.OperationId,
                context.Revision,
                context.Revision,
                SaveCommitStatus.CommittedButReopenFailed,
                recovery.ResolvedEvidence.ResolvedOutputBaseline,
                recovery.EvidenceToken,
                recovery.ResolvedEvidence,
                new EngineError(EngineErrorCode.OutputOpenFailed, exception?.Message ?? "The destination committed, but final native reopen did not complete."),
                warnings);
        }

        if (recovery.Status == RecoverSaveStatus.NotCommitted)
        {
            return new SaveResult(
                context.WorkspaceId,
                request.OperationId,
                context.Revision,
                context.Revision,
                SaveCommitStatus.NotCommitted,
                null,
                recovery.EvidenceToken,
                null,
                exception is null ? recovery.Error : Unexpected("Guarded save", exception),
                warnings);
        }

        return new SaveResult(
            context.WorkspaceId,
            request.OperationId,
            context.Revision,
            context.Revision,
            SaveCommitStatus.CommitOutcomeUnknown,
            null,
            recovery.EvidenceToken,
            null,
            recovery.Error ?? new EngineError(EngineErrorCode.CommitOutcomeUnknown, exception?.Message ?? "The save crossed its destination mutation boundary and remains unresolved."),
            warnings);
    }

    /// <summary>Creates a pre-boundary save failure with an unchanged revision.</summary>
    /// <param name="context">The borrowed save context.</param>
    /// <param name="request">The guarded save request.</param>
    /// <param name="code">The stable failure category.</param>
    /// <param name="message">The diagnostic failure message.</param>
    /// <param name="warnings">Optional accumulated warnings.</param>
    /// <returns>A not-committed save result.</returns>
    private static SaveResult SaveFailure(
        WorkspaceSaveContext context,
        SaveRequest request,
        EngineErrorCode code,
        string message,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return new SaveResult(
            context.WorkspaceId,
            request.OperationId,
            context.Revision,
            context.Revision,
            SaveCommitStatus.NotCommitted,
            null,
            null,
            null,
            new EngineError(code, message),
            warnings ?? Array.Empty<EngineWarning>());
    }

    /// <summary>Determines whether two artifacts have equal role, language, existence, length, and content digest.</summary>
    /// <param name="left">The first artifact.</param>
    /// <param name="right">The second artifact.</param>
    /// <returns><see langword="true"/> when content semantics match independently of path and file identity.</returns>
    private static bool ArtifactContentMatches(NativeArtifactAssociation left, NativeArtifactAssociation right)
    {
        return left.Role == right.Role
            && string.Equals(left.Language, right.Language, StringComparison.Ordinal)
            && left.Fingerprint.Equals(right.Fingerprint);
    }

    /// <summary>Rejects untrusted or multiply linked transaction-owned files and aliases with source or destination artifacts.</summary>
    /// <param name="plans">The complete prepared plans.</param>
    /// <param name="sourceBaseline">The complete source baseline.</param>
    private static void ValidateOwnedIdentities(
        IReadOnlyList<SaveArtifactPlan> plans,
        NativeSourceInputBaseline sourceBaseline)
    {
        var identities = new HashSet<NativeFileIdentity>();
        foreach (var source in sourceBaseline.Artifacts.Where(artifact => artifact.FileIdentity is not null))
        {
            identities.Add(source.FileIdentity!);
        }

        foreach (var plan in plans)
        {
            foreach (var artifact in new[] { plan.Before, plan.Staged, plan.Publish, plan.Backup }.Where(artifact => artifact?.FileIdentity is not null))
            {
                var identity = artifact!.FileIdentity!;
                if (identity.LinkCount is null or > 1 || !identities.Add(identity))
                {
                    throw new InvalidDataException($"Save artifact '{artifact.Path}' has an untrusted, multiply linked, or aliased physical identity.");
                }
            }
        }
    }

    /// <summary>Creates a bounded destination parent directory when a mapped loose sidecar requires it.</summary>
    /// <param name="destinationPath">The mapped destination artifact.</param>
    /// <param name="outputDirectory">The leased canonical output directory.</param>
    private static void EnsureDestinationParent(string destinationPath, string outputDirectory)
    {
        var parent = Path.GetDirectoryName(destinationPath)!;
        if (NativeSaveArtifactUtilities.PathComparer.Equals(parent, outputDirectory))
        {
            return;
        }

        NativeSaveArtifactUtilities.RequireDescendant(parent, outputDirectory, "output artifact directory");
        Directory.CreateDirectory(parent);
        NativeFileInspector.VerifyDirectory(parent, "output artifact directory");
    }
}
