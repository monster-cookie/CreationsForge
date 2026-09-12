using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Engine.Persistence;
using CreationsForge.Core.Enums;
using Moq;
using Mutagen.Bethesda;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Persistence;

/// <summary>Exercises durable journal recovery boundaries and faulted repair publication.</summary>
public sealed partial class WorkspaceSaveCoordinatorTests
{
    /// <summary>Verifies a durable preparing journal with exact original output resolves as not committed and does not block the next save.</summary>
    [Fact]
    public async Task PreparingJournalWithExactBaselineAllowsSubsequentSave()
    {
        using var directory = new TestDirectory();
        var output = CreateOutput(directory.FullName);
        await File.WriteAllBytesAsync(output.PluginPath, [1]);
        var baseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var source = new TestSourceSet();
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 3);
        var interruptedOperationId = Guid.NewGuid();
        var store = new SaveTransactionStore();
        var paths = new SaveTransactionPaths(directory.FullName, workspaceId, interruptedOperationId);
        var journal = CreateJournal(
            workspaceId,
            interruptedOperationId,
            revision,
            source.Baseline,
            output,
            baseline,
            NativeWriteDisposition.StagedChanges,
            SaveTransactionPhase.Preparing);
        await store.WriteAsync(paths, journal, CancellationToken.None);
        var provider = new TestLeaseProvider();
        var coordinator = new WorkspaceSaveCoordinator(provider);

        var recovery = await coordinator.RecoverAsync(new RecoverSaveRequest(workspaceId, interruptedOperationId, output));

        recovery.Status.ShouldBe(RecoverSaveStatus.NotCommitted);
        recovery.ResolvedEvidence.ShouldNotBeNull();
        var adapter = CreateAdapter((request, token) => CreateStagedSetAsync(
            request,
            baseline,
            new Dictionary<string, byte[]>(NativeSaveArtifactUtilities.PathComparer) { [output.PluginPath] = [2] },
            token));
        var nextContext = new WorkspaceSaveContext(
            Guid.NewGuid(),
            new WorkspaceRevision(Guid.NewGuid(), 1),
            SupportedGame.Skyrim,
            GameRelease.SkyrimSE,
            adapter.Object,
            new TestSourceSet(),
            new TestOutputState(),
            output,
            baseline);
        var nextSave = await coordinator.SaveAsync(
            nextContext,
            new SaveRequest(Guid.NewGuid(), nextContext.Revision, baseline));
        nextSave.Status.ShouldBe(SaveCommitStatus.Committed);
    }

    /// <summary>Verifies a durable no-op prepared marker resolves as committed before its terminal journal write.</summary>
    [Fact]
    public async Task PreparedNoOpJournalRecoversCommitted()
    {
        using var directory = new TestDirectory();
        var output = CreateOutput(directory.FullName);
        await File.WriteAllBytesAsync(output.PluginPath, [1]);
        var baseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var source = new TestSourceSet();
        var workspaceId = Guid.NewGuid();
        var operationId = Guid.NewGuid();
        var journal = CreateJournal(
            workspaceId,
            operationId,
            new WorkspaceRevision(Guid.NewGuid(), 4),
            source.Baseline,
            output,
            baseline,
            NativeWriteDisposition.Unchanged,
            SaveTransactionPhase.Prepared);
        var paths = new SaveTransactionPaths(directory.FullName, workspaceId, operationId);
        await new SaveTransactionStore().WriteAsync(paths, journal, CancellationToken.None);
        var coordinator = new WorkspaceSaveCoordinator(new TestLeaseProvider());

        var recovery = await coordinator.RecoverAsync(new RecoverSaveRequest(workspaceId, operationId, output));

        recovery.Status.ShouldBe(RecoverSaveStatus.Committed);
        recovery.ResolvedEvidence.ShouldNotBeNull();
    }

    /// <summary>Verifies a staging failure leaves a readable terminal journal that permits a later save.</summary>
    [Fact]
    public async Task StagingFailureJournalRemainsReadableAndAllowsSubsequentSave()
    {
        using var directory = new TestDirectory();
        var output = CreateOutput(directory.FullName);
        await File.WriteAllBytesAsync(output.PluginPath, [1]);
        var baseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var failingAdapter = CreateAdapter((_, _) => throw new InvalidOperationException("The staged delegate must not run."));
        failingAdapter.Setup(value => value.WriteAndValidateAsync(
                It.IsAny<INativeSourceSet>(),
                It.IsAny<INativeOutputState>(),
                It.IsAny<NativeWriteRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<EngineResult<NativeStagedOutputSet>>(
                EngineResult<NativeStagedOutputSet>.Failure(new EngineError(
                    EngineErrorCode.ValidationFailed,
                    "Injected adapter validation failure."))));
        var failedContext = CreateContext(output, baseline, new TestSourceSet(), failingAdapter.Object);
        var provider = new TestLeaseProvider();
        var coordinator = new WorkspaceSaveCoordinator(provider);

        var failed = await coordinator.SaveAsync(
            failedContext,
            new SaveRequest(Guid.NewGuid(), failedContext.Revision, baseline));

        failed.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        var succeedingAdapter = CreateAdapter((request, token) => CreateStagedSetAsync(
            request,
            baseline,
            new Dictionary<string, byte[]>(NativeSaveArtifactUtilities.PathComparer) { [output.PluginPath] = [2] },
            token));
        var succeedingContext = CreateContext(output, baseline, new TestSourceSet(), succeedingAdapter.Object);
        var succeeded = await coordinator.SaveAsync(
            succeedingContext,
            new SaveRequest(Guid.NewGuid(), succeedingContext.Revision, baseline));
        succeeded.Status.ShouldBe(SaveCommitStatus.Committed);
    }

    /// <summary>Verifies unchanged artifact slots remain neutral when all materially changed slots reached their prepared identities.</summary>
    [Fact]
    public async Task CompletedChangedSlotsWithUnchangedSlotRecoverAsCommitted()
    {
        using var directory = new TestDirectory();
        var output = CreateOutput(directory.FullName, LocalizedOutputMode.SeparateStringFiles);
        await File.WriteAllBytesAsync(output.PluginPath, [1]);
        var shape = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var strings = shape.Artifacts.First(artifact => artifact.Role == NativeArtifactRole.Strings);
        Directory.CreateDirectory(Path.GetDirectoryName(strings.Path)!);
        await File.WriteAllBytesAsync(strings.Path, [2]);
        var baseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var desired = new Dictionary<string, byte[]>(NativeSaveArtifactUtilities.PathComparer)
        {
            [output.PluginPath] = [3],
            [strings.Path] = [2]
        };
        var adapter = CreateAdapter((request, token) => CreateStagedSetAsync(request, baseline, desired, token));
        var context = CreateContext(output, baseline, new TestSourceSet(), adapter.Object);
        var operationId = Guid.NewGuid();
        var provider = new TestLeaseProvider();
        var interrupted = new WorkspaceSaveCoordinator(
            provider,
            new SaveTransactionStore(),
            new ThrowAfterFirstMutationFileOperations());

        var save = await interrupted.SaveAsync(context, new SaveRequest(operationId, context.Revision, baseline));
        var recovery = await new WorkspaceSaveCoordinator(provider).RecoverAsync(
            new RecoverSaveRequest(context.WorkspaceId, operationId, output));

        save.Status.ShouldBe(SaveCommitStatus.CommittedButReopenFailed);
        recovery.Status.ShouldBe(RecoverSaveStatus.Committed);
        recovery.RepairRequired.ShouldBeFalse();
        recovery.ResolvedEvidence.ShouldNotBeNull();
    }

    /// <summary>Verifies terminal historical status survives a temporary exclusive lock that prevents current evidence inspection.</summary>
    [Fact]
    public async Task LockedCommittedOutputPreservesHistoricalCommittedStatus()
    {
        using var directory = new TestDirectory();
        var output = CreateOutput(directory.FullName);
        await File.WriteAllBytesAsync(output.PluginPath, [1]);
        var baseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var adapter = CreateAdapter((request, token) => CreateStagedSetAsync(
            request,
            baseline,
            new Dictionary<string, byte[]>(NativeSaveArtifactUtilities.PathComparer) { [output.PluginPath] = [2] },
            token));
        var context = CreateContext(output, baseline, new TestSourceSet(), adapter.Object);
        var operationId = Guid.NewGuid();
        var coordinator = new WorkspaceSaveCoordinator(new TestLeaseProvider());
        (await coordinator.SaveAsync(context, new SaveRequest(operationId, context.Revision, baseline)))
            .Status.ShouldBe(SaveCommitStatus.Committed);
        await using var lockStream = new FileStream(
            output.PluginPath,
            FileMode.Open,
            FileAccess.ReadWrite,
            FileShare.None);

        var recovery = await coordinator.RecoverAsync(new RecoverSaveRequest(context.WorkspaceId, operationId, output));

        recovery.Status.ShouldBe(RecoverSaveStatus.Committed);
        recovery.SaveBaseRevision.ShouldBe(context.Revision);
        recovery.ResolvedEvidence.ShouldBeNull();
        recovery.EvidenceToken.ShouldBeNull();
        recovery.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
    }

    /// <summary>Verifies repair publication drift at the final fault seam is rejected before destination mutation.</summary>
    [Fact]
    public async Task ChangedRepairPublicationIsRejectedBeforeDestinationMutation()
    {
        using var directory = new TestDirectory();
        var output = CreateOutput(directory.FullName, LocalizedOutputMode.SeparateStringFiles);
        await File.WriteAllBytesAsync(output.PluginPath, [1]);
        var shape = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var strings = shape.Artifacts.First(artifact => artifact.Role == NativeArtifactRole.Strings);
        Directory.CreateDirectory(Path.GetDirectoryName(strings.Path)!);
        await File.WriteAllBytesAsync(strings.Path, [2]);
        var baseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var desired = new Dictionary<string, byte[]>(NativeSaveArtifactUtilities.PathComparer)
        {
            [output.PluginPath] = [3],
            [strings.Path] = [4]
        };
        var adapter = CreateAdapter((request, token) => CreateStagedSetAsync(request, baseline, desired, token));
        var context = CreateContext(output, baseline, new TestSourceSet(), adapter.Object);
        var operationId = Guid.NewGuid();
        var provider = new TestLeaseProvider();
        var interrupted = new WorkspaceSaveCoordinator(
            provider,
            new SaveTransactionStore(),
            new ThrowAfterFirstMutationFileOperations());
        (await interrupted.SaveAsync(context, new SaveRequest(operationId, context.Revision, baseline)))
            .Status.ShouldBe(SaveCommitStatus.CommitOutcomeUnknown);
        var normal = new WorkspaceSaveCoordinator(provider);
        var recovery = await normal.RecoverAsync(new RecoverSaveRequest(context.WorkspaceId, operationId, output));
        var publishDirectory = Path.Combine(
            directory.FullName,
            SaveTransactionStore.MetadataDirectoryName,
            context.WorkspaceId.ToString("N"),
            operationId.ToString("N"),
            "publish");
        var faulted = new WorkspaceSaveCoordinator(
            provider,
            new SaveTransactionStore(),
            new ChangeRepairPublicationBeforeMutationFileOperations(publishDirectory));
        var destinationBeforeRepair = await File.ReadAllBytesAsync(strings.Path);

        var repair = await faulted.RepairAsync(new RepairSaveRequest(
            context.WorkspaceId,
            operationId,
            Guid.NewGuid(),
            context.Revision,
            output,
            recovery.EvidenceToken!,
            RepairSaveDirection.CompletePrepared));

        repair.Status.ShouldBe(RepairSaveStatus.NotStarted);
        repair.Error!.Code.ShouldBe(EngineErrorCode.UnexpectedFailure);
        (await File.ReadAllBytesAsync(strings.Path)).ShouldBe(destinationBeforeRepair);
    }

    /// <summary>Verifies an incomplete repair replay cannot mutate more output after its native source baseline changes.</summary>
    [Fact]
    public async Task InterruptedRepairReplayWithChangedSourceDoesNotMutateDestination()
    {
        using var directory = new TestDirectory();
        var sourcePath = Path.Combine(directory.FullName, "Source.esm");
        await File.WriteAllBytesAsync(sourcePath, [5]);
        var sourceArtifact = await NativeFileInspector.InspectAsync(
            sourcePath,
            NativeArtifactRole.Plugin,
            language: null,
            mustExist: true,
            CancellationToken.None);
        var source = new TestSourceSet(new NativeSourceInputBaseline(Guid.NewGuid(), [sourceArtifact]));
        var output = CreateOutput(directory.FullName, LocalizedOutputMode.SeparateStringFiles);
        await File.WriteAllBytesAsync(output.PluginPath, [1]);
        var shape = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        foreach (var artifact in shape.Artifacts.Where(artifact => artifact.Role == NativeArtifactRole.Strings))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(artifact.Path)!);
            await File.WriteAllBytesAsync(artifact.Path, [2]);
        }

        var baseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var desired = baseline.Artifacts.ToDictionary(
            artifact => artifact.Path,
            artifact => artifact.Role == NativeArtifactRole.Plugin ? new byte[] { 9 } : new byte[] { 8 },
            NativeSaveArtifactUtilities.PathComparer);
        var adapter = CreateAdapter((request, token) => CreateStagedSetAsync(request, baseline, desired, token));
        var context = CreateContext(output, baseline, source, adapter.Object);
        var operationId = Guid.NewGuid();
        var provider = new TestLeaseProvider();
        var interruptedSave = new WorkspaceSaveCoordinator(
            provider,
            new SaveTransactionStore(),
            new ThrowAfterFirstMutationFileOperations());
        (await interruptedSave.SaveAsync(context, new SaveRequest(operationId, context.Revision, baseline)))
            .Status.ShouldBe(SaveCommitStatus.CommitOutcomeUnknown);
        var normal = new WorkspaceSaveCoordinator(provider);
        var recovery = await normal.RecoverAsync(new RecoverSaveRequest(context.WorkspaceId, operationId, output));
        var repairRequest = new RepairSaveRequest(
            context.WorkspaceId,
            operationId,
            Guid.NewGuid(),
            context.Revision,
            output,
            recovery.EvidenceToken!,
            RepairSaveDirection.CompletePrepared);
        var interruptedRepair = new WorkspaceSaveCoordinator(
            provider,
            new SaveTransactionStore(),
            new ThrowAfterFirstMutationFileOperations());
        (await interruptedRepair.RepairAsync(repairRequest)).Status.ShouldBe(RepairSaveStatus.StillUnknown);
        var beforeReplay = await NativeSaveArtifactUtilities.CaptureOutputAsync(
            GameRelease.SkyrimSE,
            output,
            CancellationToken.None);
        await File.WriteAllBytesAsync(sourcePath, [6]);

        var replay = await normal.RepairAsync(repairRequest);

        replay.Status.ShouldBe(RepairSaveStatus.BlockedByExternalChange);
        replay.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
        var afterReplay = await NativeSaveArtifactUtilities.CaptureOutputAsync(
            GameRelease.SkyrimSE,
            output,
            CancellationToken.None);
        NativeSaveArtifactUtilities.MatchBaseline(beforeReplay, afterReplay).ShouldBeTrue();
    }

    /// <summary>Verifies destination replacement at the final save seam is detected before transaction bytes publish.</summary>
    [Fact]
    public async Task ReplacedDestinationAtFinalSaveSeamRemainsUntouched()
    {
        using var directory = new TestDirectory();
        var output = CreateOutput(directory.FullName);
        await File.WriteAllBytesAsync(output.PluginPath, [1]);
        var baseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var adapter = CreateAdapter((request, token) => CreateStagedSetAsync(
            request,
            baseline,
            new Dictionary<string, byte[]>(NativeSaveArtifactUtilities.PathComparer) { [output.PluginPath] = [2] },
            token));
        var context = CreateContext(output, baseline, new TestSourceSet(), adapter.Object);
        var preservedOriginal = output.PluginPath + ".original";
        var coordinator = new WorkspaceSaveCoordinator(
            new TestLeaseProvider(),
            new SaveTransactionStore(),
            new ReplaceDestinationBeforeMutationFileOperations(output.PluginPath, preservedOriginal));

        var save = await coordinator.SaveAsync(
            context,
            new SaveRequest(Guid.NewGuid(), context.Revision, baseline));

        save.Status.ShouldBe(SaveCommitStatus.CommitOutcomeUnknown);
        save.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
        (await File.ReadAllBytesAsync(output.PluginPath)).ShouldBe([99]);
        (await File.ReadAllBytesAsync(preservedOriginal)).ShouldBe([1]);
    }

    /// <summary>Creates one direct journal snapshot for phase-boundary recovery tests.</summary>
    /// <param name="workspaceId">The journal workspace identity.</param>
    /// <param name="operationId">The journal save identity.</param>
    /// <param name="revision">The save base revision.</param>
    /// <param name="sourceBaseline">The exact source baseline.</param>
    /// <param name="output">The exact output association.</param>
    /// <param name="beforeBaseline">The exact original output baseline.</param>
    /// <param name="disposition">The durable write disposition.</param>
    /// <param name="phase">The durable journal phase.</param>
    /// <returns>The complete journal snapshot.</returns>
    private static SaveTransactionJournal CreateJournal(
        Guid workspaceId,
        Guid operationId,
        WorkspaceRevision revision,
        NativeSourceInputBaseline sourceBaseline,
        OutputAssociation output,
        OutputArtifactSetBaseline beforeBaseline,
        NativeWriteDisposition disposition,
        SaveTransactionPhase phase)
    {
        return new SaveTransactionJournal(
            workspaceId,
            operationId,
            new string('A', 64),
            revision,
            SupportedGame.Skyrim,
            GameRelease.SkyrimSE,
            sourceBaseline,
            output,
            beforeBaseline,
            disposition,
            phase,
            0,
            Array.Empty<SaveArtifactPlan>(),
            null,
            Array.Empty<SaveRepairAttempt>());
    }

    /// <summary>Changes the single prepared repair publication at the final pre-mutation seam.</summary>
    private sealed class ChangeRepairPublicationBeforeMutationFileOperations : SaveTransactionFileOperations
    {
        /// <summary>The transaction publication directory containing the prepared repair file.</summary>
        private readonly string PublishDirectory;

        /// <summary>Whether the configured change was already injected.</summary>
        private bool HasChanged;

        /// <summary>Initializes the deterministic repair-publication fault.</summary>
        /// <param name="publishDirectory">The exact transaction publication directory.</param>
        internal ChangeRepairPublicationBeforeMutationFileOperations(string publishDirectory)
        {
            PublishDirectory = publishDirectory;
        }

        /// <inheritdoc />
        internal override void BeforeDestinationMutation(Guid saveOperationId, int artifactIndex)
        {
            if (!HasChanged)
            {
                HasChanged = true;
                var repairPublication = Directory.EnumerateFiles(PublishDirectory, "repair-*.bin").Single();
                File.WriteAllBytes(repairPublication, [99]);
            }
        }
    }

    /// <summary>Replaces the destination with foreign bytes at the final normal-save mutation seam.</summary>
    private sealed class ReplaceDestinationBeforeMutationFileOperations : SaveTransactionFileOperations
    {
        /// <summary>The exact destination path replaced by the fault.</summary>
        private readonly string DestinationPath;

        /// <summary>The path retaining the original destination identity during the test.</summary>
        private readonly string PreservedOriginalPath;

        /// <summary>Whether the configured replacement was already injected.</summary>
        private bool HasReplaced;

        /// <summary>Initializes the deterministic destination-replacement fault.</summary>
        /// <param name="destinationPath">The destination path to replace.</param>
        /// <param name="preservedOriginalPath">The path that retains the original file identity.</param>
        internal ReplaceDestinationBeforeMutationFileOperations(
            string destinationPath,
            string preservedOriginalPath)
        {
            DestinationPath = destinationPath;
            PreservedOriginalPath = preservedOriginalPath;
        }

        /// <inheritdoc />
        internal override void BeforeDestinationMutation(Guid saveOperationId, int artifactIndex)
        {
            if (!HasReplaced)
            {
                HasReplaced = true;
                File.Move(DestinationPath, PreservedOriginalPath);
                File.WriteAllBytes(DestinationPath, [99]);
            }
        }
    }
}
