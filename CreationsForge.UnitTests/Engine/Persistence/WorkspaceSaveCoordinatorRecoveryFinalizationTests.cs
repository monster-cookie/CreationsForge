using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Engine.Persistence;
using CreationsForge.Core.Enums;
using Moq;
using Mutagen.Bethesda;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Persistence;

/// <summary>Exercises transaction-capacity admission and explicit terminal recovery-evidence finalization.</summary>
public sealed partial class WorkspaceSaveCoordinatorTests
{
    /// <summary>Contains focused tests for capacity admission and persistent terminal evidence validation.</summary>
    public sealed class WorkspaceSaveCoordinatorRecoveryFinalizationBoundaryTests
    {
        /// <summary>Verifies exact replay remains available at capacity while conflicting reuse retains its typed rejection.</summary>
        [Fact]
        public async Task ExactReplayAtCapacitySucceedsAndConflictingPayloadIsRejected()
        {
            using var directory = new TestDirectory();
            var output = CreateOutput(directory.FullName);
            await File.WriteAllBytesAsync(output.PluginPath, [1]);
            var baseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(
                GameRelease.SkyrimSE,
                output,
                CancellationToken.None);
            var adapter = CreateAdapter((request, token) => CreateStagedSetAsync(
                request,
                baseline,
                new Dictionary<string, byte[]>(NativeSaveArtifactUtilities.PathComparer)
                {
                    [output.PluginPath] = [2]
                },
                token));
            var context = CreateContext(output, baseline, new TestSourceSet(), adapter.Object);
            var request = new SaveRequest(Guid.NewGuid(), context.Revision, baseline);
            var coordinator = new WorkspaceSaveCoordinator(
                new TestLeaseProvider(),
                new SaveTransactionStore(1),
                new SaveTransactionFileOperations());
    
            var first = await coordinator.SaveAsync(context, request);
            var replay = await coordinator.SaveAsync(context, request);
            var sourcePath = Path.Combine(directory.FullName, "DifferentSource.esm");
            await File.WriteAllBytesAsync(sourcePath, [7]);
            var sourceArtifact = await NativeFileInspector.InspectAsync(
                sourcePath,
                NativeArtifactRole.Plugin,
                language: null,
                mustExist: true,
                CancellationToken.None);
            var differentSource = new TestSourceSet(new NativeSourceInputBaseline(Guid.NewGuid(), [sourceArtifact]));
            var conflictingContext = new WorkspaceSaveContext(
                context.WorkspaceId,
                context.Revision,
                context.Game,
                context.Release,
                adapter.Object,
                differentSource,
                new TestOutputState(),
                output,
                baseline);
            var conflict = await coordinator.SaveAsync(conflictingContext, request);
    
            first.Status.ShouldBe(SaveCommitStatus.Committed, first.Error?.Message);
            replay.Status.ShouldBe(SaveCommitStatus.Committed, replay.Error?.Message);
            conflict.Status.ShouldBe(SaveCommitStatus.NotCommitted);
            conflict.Error!.Code.ShouldBe(EngineErrorCode.OperationIdReuse);
            adapter.Verify(value => value.WriteAndValidateAsync(
                It.IsAny<INativeSourceSet>(),
                It.IsAny<INativeOutputState>(),
                It.IsAny<NativeWriteRequest>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    
        /// <summary>Verifies an unpublished remnant consumes capacity before any adapter staging call or canonical publication.</summary>
        [Fact]
        public async Task ExhaustedTransactionCapacityRejectsSaveBeforeStaging()
        {
            using var directory = new TestDirectory();
            var output = CreateOutput(directory.FullName);
            await File.WriteAllBytesAsync(output.PluginPath, [1]);
            var baseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(
                GameRelease.SkyrimSE,
                output,
                CancellationToken.None);
            var adapter = CreateAdapter((request, token) => CreateStagedSetAsync(
                request,
                baseline,
                new Dictionary<string, byte[]>(NativeSaveArtifactUtilities.PathComparer)
                {
                    [output.PluginPath] = [2]
                },
                token));
            var context = CreateContext(output, baseline, new TestSourceSet(), adapter.Object);
            CreateInitializationRemnant(
                directory.FullName,
                context.WorkspaceId,
                Guid.NewGuid(),
                journalBytes: null);
            var operationId = Guid.NewGuid();
            var coordinator = new WorkspaceSaveCoordinator(
                new TestLeaseProvider(),
                new SaveTransactionStore(1),
                new SaveTransactionFileOperations());
    
            var result = await coordinator.SaveAsync(
                context,
                new SaveRequest(operationId, context.Revision, baseline));
    
            result.Status.ShouldBe(SaveCommitStatus.NotCommitted);
            result.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
            Directory.Exists(new SaveTransactionPaths(directory.FullName, context.WorkspaceId, operationId).TransactionDirectoryPath)
                .ShouldBeFalse();
            adapter.Verify(value => value.WriteAndValidateAsync(
                It.IsAny<INativeSourceSet>(),
                It.IsAny<INativeOutputState>(),
                It.IsAny<NativeWriteRequest>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }
    
        /// <summary>Verifies preparing evidence remains read-only until validation persists the matching terminal projection without changing its token.</summary>
        [Fact]
        public async Task PreparingEvidenceTokenRemainsStableAfterTerminalValidation()
        {
            using var directory = new TestDirectory();
            var setup = await CreatePreparingJournalAsync(directory.FullName);
            var store = new SaveTransactionStore();
            await store.WriteAsync(setup.Paths, setup.Journal, CancellationToken.None);
            var provider = new TestLeaseProvider();
            var coordinator = new WorkspaceSaveCoordinator(provider, store, new SaveTransactionFileOperations());
            var journalBeforeRecovery = await File.ReadAllBytesAsync(setup.Paths.JournalPath);
    
            var recovery = await coordinator.RecoverAsync(new RecoverSaveRequest(
                setup.Journal.WorkspaceId,
                setup.Journal.SaveOperationId,
                setup.Journal.Output));
    
            recovery.Status.ShouldBe(RecoverSaveStatus.NotCommitted);
            recovery.ResolvedEvidence.ShouldNotBeNull();
            (await File.ReadAllBytesAsync(setup.Paths.JournalPath)).ShouldBe(journalBeforeRecovery);
            var validated = await coordinator.ValidateResolvedEvidenceAsync(
                new TestLease(directory.FullName),
                recovery.ResolvedEvidence!,
                CancellationToken.None);
            validated.Succeeded.ShouldBeTrue(validated.Error?.Message);
            validated.Value!.EvidenceToken.ShouldBe(recovery.EvidenceToken);
            var terminal = await store.ReadAsync(setup.Paths, CancellationToken.None);
            terminal!.Phase.ShouldBe(SaveTransactionPhase.NotCommitted);
            NativeSaveArtifactUtilities.MatchBaseline(
                terminal.TerminalBaseline!,
                recovery.ResolvedEvidence!.ResolvedOutputBaseline).ShouldBeTrue();
            var afterFinalization = await coordinator.RecoverAsync(new RecoverSaveRequest(
                setup.Journal.WorkspaceId,
                setup.Journal.SaveOperationId,
                setup.Journal.Output));
            afterFinalization.EvidenceToken.ShouldBe(recovery.EvidenceToken);
            var admission = await coordinator.InspectOutputAdmissionAsync(
                new TestLease(directory.FullName),
                new OutputAdmissionRequest(
                    Guid.NewGuid(),
                    setup.Journal.Game,
                    setup.Journal.Release,
                    setup.Journal.SourceBaseline,
                    setup.Journal.Output),
                CancellationToken.None);
            admission.Succeeded.ShouldBeTrue(admission.Error?.Message);
            admission.Value!.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        }
    
        /// <summary>Verifies an all-prepared changed output uses the same committed token before and after terminal journal persistence.</summary>
        [Fact]
        public async Task PreparedAllPreparedEvidenceTokenRemainsStableAfterTerminalValidation()
        {
            using var directory = new TestDirectory();
            var setup = await CreatePhysicallyPreparedJournalAsync(directory.FullName);
            var coordinator = new WorkspaceSaveCoordinator(
                new TestLeaseProvider(),
                setup.Store,
                new SaveTransactionFileOperations());
    
            var recovery = await coordinator.RecoverAsync(new RecoverSaveRequest(
                setup.Journal.WorkspaceId,
                setup.Journal.SaveOperationId,
                setup.Journal.Output));
            recovery.Status.ShouldBe(RecoverSaveStatus.Committed);
            recovery.ResolvedEvidence.ShouldNotBeNull();
    
            var validated = await coordinator.ValidateResolvedEvidenceAsync(
                new TestLease(directory.FullName),
                recovery.ResolvedEvidence!,
                CancellationToken.None);
    
            validated.Succeeded.ShouldBeTrue(validated.Error?.Message);
            validated.Value!.EvidenceToken.ShouldBe(recovery.EvidenceToken);
            var terminal = await setup.Store.ReadAsync(setup.Paths, CancellationToken.None);
            terminal!.Phase.ShouldBe(SaveTransactionPhase.Committed);
            terminal.MutationProgress.ShouldBe(setup.Journal.ArtifactPlans.Count);
            var reopened = await coordinator.RecoverAsync(new RecoverSaveRequest(
                setup.Journal.WorkspaceId,
                setup.Journal.SaveOperationId,
                setup.Journal.Output));
            reopened.Status.ShouldBe(RecoverSaveStatus.Committed);
            reopened.EvidenceToken.ShouldBe(recovery.EvidenceToken);
        }
    
        /// <summary>Verifies changed output after review rejects validation without rewriting the nonterminal journal.</summary>
        [Fact]
        public async Task ExternalChangeDuringEvidenceValidationDoesNotFinalizeJournal()
        {
            using var directory = new TestDirectory();
            var setup = await CreatePreparingJournalAsync(directory.FullName);
            var store = new SaveTransactionStore();
            await store.WriteAsync(setup.Paths, setup.Journal, CancellationToken.None);
            var coordinator = new WorkspaceSaveCoordinator(
                new TestLeaseProvider(),
                store,
                new SaveTransactionFileOperations());
            var recovery = await coordinator.RecoverAsync(new RecoverSaveRequest(
                setup.Journal.WorkspaceId,
                setup.Journal.SaveOperationId,
                setup.Journal.Output));
            var journalBeforeValidation = await File.ReadAllBytesAsync(setup.Paths.JournalPath);
            await File.WriteAllBytesAsync(setup.Journal.Output.PluginPath, [9]);
    
            var result = await coordinator.ValidateResolvedEvidenceAsync(
                new TestLease(directory.FullName),
                recovery.ResolvedEvidence!,
                CancellationToken.None);
    
            result.Succeeded.ShouldBeFalse();
            result.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
            (await File.ReadAllBytesAsync(setup.Paths.JournalPath)).ShouldBe(journalBeforeValidation);
            (await store.ReadAsync(setup.Paths, CancellationToken.None))!.Phase.ShouldBe(SaveTransactionPhase.Preparing);
        }
    
        /// <summary>Creates a prepared journal whose changed publication file has already reached the destination with exact physical identity.</summary>
        /// <param name="directoryPath">The canonical temporary output directory.</param>
        /// <returns>The prepared journal, paths, and store.</returns>
        private static async Task<(SaveTransactionStore Store, SaveTransactionPaths Paths, SaveTransactionJournal Journal)> CreatePhysicallyPreparedJournalAsync(
            string directoryPath)
        {
            var output = CreateOutput(directoryPath);
            await File.WriteAllBytesAsync(output.PluginPath, [1]);
            var before = await NativeSaveArtifactUtilities.CaptureOutputAsync(
                GameRelease.SkyrimSE,
                output,
                CancellationToken.None);
            var workspaceId = Guid.NewGuid();
            var operationId = Guid.NewGuid();
            var paths = new SaveTransactionPaths(directoryPath, workspaceId, operationId);
            var store = new SaveTransactionStore();
            store.EnsureTransactionDirectory(paths);
            store.CreateEmptySubdirectory(paths.StagingDirectoryPath, "test staging directory");
            store.CreateEmptySubdirectory(paths.PublishDirectoryPath, "test publication directory");
            store.CreateEmptySubdirectory(paths.BackupDirectoryPath, "test backup directory");
            store.CreateEmptySubdirectory(paths.RetiredDirectoryPath, "test retirement directory");
            var pluginIndex = before.Artifacts
                .Select((artifact, index) => (Artifact: artifact, Index: index))
                .Single(candidate => candidate.Artifact.Role == NativeArtifactRole.Plugin)
                .Index;
            var stagedPath = Path.Combine(paths.StagingDirectoryPath, $"{pluginIndex:D4}.bin");
            await File.WriteAllBytesAsync(stagedPath, [2]);
            var staged = await NativeFileInspector.InspectAsync(
                stagedPath,
                NativeArtifactRole.Plugin,
                language: null,
                mustExist: true,
                CancellationToken.None);
            var publishPath = Path.Combine(paths.PublishDirectoryPath, $"{pluginIndex:D4}.bin");
            await NativeSaveArtifactUtilities.CopyAndFlushAsync(stagedPath, publishPath, CancellationToken.None);
            var publish = await NativeFileInspector.InspectAsync(
                publishPath,
                NativeArtifactRole.Plugin,
                language: null,
                mustExist: true,
                CancellationToken.None);
            var beforePlugin = before.Artifacts[pluginIndex];
            var backupPath = Path.Combine(paths.BackupDirectoryPath, $"{pluginIndex:D4}.bin");
            await NativeSaveArtifactUtilities.CopyAndFlushAsync(output.PluginPath, backupPath, CancellationToken.None);
            var backup = await NativeFileInspector.InspectAsync(
                backupPath,
                NativeArtifactRole.Plugin,
                language: null,
                mustExist: true,
                CancellationToken.None);
            var plans = new List<SaveArtifactPlan>(before.Artifacts.Count);
            foreach (var indexed in before.Artifacts.Select((artifact, index) => (Artifact: artifact, Index: index)))
            {
                var retiredPath = Path.Combine(paths.RetiredDirectoryPath, $"{indexed.Index:D4}.bin");
                if (indexed.Index == pluginIndex)
                {
                    plans.Add(new SaveArtifactPlan(beforePlugin, staged, publish, backup, retiredPath));
                    continue;
                }

                var unchangedStagedPath = Path.Combine(paths.StagingDirectoryPath, $"{indexed.Index:D4}.bin");
                var unchangedStaged = await NativeFileInspector.InspectAsync(
                    unchangedStagedPath,
                    indexed.Artifact.Role,
                    indexed.Artifact.Language,
                    mustExist: false,
                    CancellationToken.None);
                plans.Add(new SaveArtifactPlan(
                    indexed.Artifact,
                    unchangedStaged,
                    publish: null,
                    backup: null,
                    retiredPath));
            }
            var source = new TestSourceSet();
            var journal = new SaveTransactionJournal(
                workspaceId,
                operationId,
                new string('B', 64),
                new WorkspaceRevision(Guid.NewGuid(), 3),
                SupportedGame.Skyrim,
                GameRelease.SkyrimSE,
                source.Baseline,
                output,
                before,
                NativeWriteDisposition.StagedChanges,
                SaveTransactionPhase.Prepared,
                0,
                plans,
                null,
                Array.Empty<SaveRepairAttempt>());
            await store.WriteAsync(paths, journal, CancellationToken.None);
            File.Move(publishPath, output.PluginPath, overwrite: true);
            return (store, paths, journal);
        }

    }
}
