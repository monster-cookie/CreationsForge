using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Engine.Persistence;
using CreationsForge.Core.Enums;
using Moq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Persistence;

/// <summary>Exercises guarded save, recovery, repair, and historical evidence on disposable physical files.</summary>
public sealed partial class WorkspaceSaveCoordinatorTests
{
    /// <summary>Verifies existing-only recovery creates nothing and returns the stable no-evidence shape.</summary>
    [Fact]
    public async Task RecoverWithoutStableGuardReturnsNoRecoveryEvidence()
    {
        using var directory = new TestDirectory();
        var output = CreateOutput(directory.FullName);
        var provider = new TestLeaseProvider(guardExists: false);
        var coordinator = new WorkspaceSaveCoordinator(provider);

        var result = await coordinator.RecoverAsync(new RecoverSaveRequest(Guid.NewGuid(), Guid.NewGuid(), output));

        result.Status.ShouldBe(RecoverSaveStatus.StillUnknown);
        result.SaveBaseRevision.ShouldBeNull();
        result.RepairRequired.ShouldBeFalse();
        result.EvidenceToken.ShouldBeNull();
        result.ResolvedEvidence.ShouldBeNull();
        result.Error!.Code.ShouldBe(EngineErrorCode.NoRecoveryEvidence);
        provider.Modes.ShouldBe([OutputDirectoryLeaseMode.ExistingOnly]);
        Directory.Exists(Path.Combine(directory.FullName, SaveTransactionStore.MetadataDirectoryName)).ShouldBeFalse();
    }

    /// <summary>Verifies a whole-output no-op writes terminal metadata without mutating plugin bytes.</summary>
    [Fact]
    public async Task SaveNoOpCommitsEvidenceWithoutDestinationMutation()
    {
        using var directory = new TestDirectory();
        var output = CreateOutput(directory.FullName);
        var originalBytes = new byte[] { 1, 2, 3, 4 };
        await File.WriteAllBytesAsync(output.PluginPath, originalBytes);
        var baseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var source = new TestSourceSet();
        var adapter = CreateAdapter(
            (_, _) => Task.FromResult(new NativeStagedOutputSet(
                NativeWriteDisposition.Unchanged,
                null,
                Array.Empty<NativeStagedArtifactMapping>())));
        var context = CreateContext(output, baseline, source, adapter.Object);
        var operationId = Guid.NewGuid();
        var coordinator = new WorkspaceSaveCoordinator(new TestLeaseProvider());

        var result = await coordinator.SaveAsync(context, new SaveRequest(operationId, context.Revision, baseline));

        result.Status.ShouldBe(SaveCommitStatus.Committed);
        result.CommittedBaseline.ShouldNotBeNull();
        result.ResolvedEvidence.ShouldNotBeNull();
        (await File.ReadAllBytesAsync(output.PluginPath)).ShouldBe(originalBytes);
        var journalPath = Path.Combine(
            directory.FullName,
            SaveTransactionStore.MetadataDirectoryName,
            context.WorkspaceId.ToString("N"),
            operationId.ToString("N"),
            SaveTransactionStore.JournalFileName);
        File.Exists(journalPath).ShouldBeTrue();
    }

    /// <summary>Verifies an interruption between two changed artifacts is recoverable and can explicitly complete.</summary>
    [Fact]
    public async Task InterruptedMultiFileSaveCanBeRecoveredAndCompleted()
    {
        using var directory = new TestDirectory();
        var output = CreateOutput(directory.FullName, LocalizedOutputMode.SeparateStringFiles);
        await File.WriteAllBytesAsync(output.PluginPath, [1, 1, 1]);
        var emptyBaseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var strings = emptyBaseline.Artifacts.First(artifact => artifact.Role == NativeArtifactRole.Strings);
        Directory.CreateDirectory(Path.GetDirectoryName(strings.Path)!);
        await File.WriteAllBytesAsync(strings.Path, [2, 2, 2]);
        var baseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var desired = new Dictionary<string, byte[]>(NativeSaveArtifactUtilities.PathComparer)
        {
            [output.PluginPath] = [9, 9, 9],
            [strings.Path] = [8, 8, 8]
        };
        var adapter = CreateAdapter((request, token) => CreateStagedSetAsync(request, baseline, desired, token));
        var source = new TestSourceSet();
        var context = CreateContext(output, baseline, source, adapter.Object);
        var operationId = Guid.NewGuid();
        var provider = new TestLeaseProvider();
        var interruptedCoordinator = new WorkspaceSaveCoordinator(
            provider,
            new SaveTransactionStore(),
            new ThrowAfterFirstMutationFileOperations());

        var save = await interruptedCoordinator.SaveAsync(context, new SaveRequest(operationId, context.Revision, baseline));

        save.Status.ShouldBe(SaveCommitStatus.CommitOutcomeUnknown);
        var coordinator = new WorkspaceSaveCoordinator(provider);
        var recovery = await coordinator.RecoverAsync(new RecoverSaveRequest(context.WorkspaceId, operationId, output));
        recovery.Status.ShouldBe(RecoverSaveStatus.StillUnknown);
        recovery.RepairRequired.ShouldBeTrue();
        recovery.EvidenceToken.ShouldNotBeNull();

        var repair = await coordinator.RepairAsync(new RepairSaveRequest(
            context.WorkspaceId,
            operationId,
            Guid.NewGuid(),
            context.Revision,
            output,
            recovery.EvidenceToken!,
            RepairSaveDirection.CompletePrepared));

        repair.Status.ShouldBe(RepairSaveStatus.PreparedSetCompleted);
        repair.ResolvedEvidence.ShouldNotBeNull();
        (await File.ReadAllBytesAsync(output.PluginPath)).ShouldBe(desired[output.PluginPath]);
        (await File.ReadAllBytesAsync(strings.Path)).ShouldBe(desired[strings.Path]);
    }

    /// <summary>Verifies changed transaction-owned staged evidence removes repair authority.</summary>
    [Fact]
    public async Task StagedEvidenceChangeBlocksRepairWithoutOverwritingDestination()
    {
        using var directory = new TestDirectory();
        var output = CreateOutput(directory.FullName, LocalizedOutputMode.SeparateStringFiles);
        await File.WriteAllBytesAsync(output.PluginPath, [1]);
        var emptyBaseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var strings = emptyBaseline.Artifacts.First(artifact => artifact.Role == NativeArtifactRole.Strings);
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
        var coordinator = new WorkspaceSaveCoordinator(provider);
        var firstRecovery = await coordinator.RecoverAsync(new RecoverSaveRequest(context.WorkspaceId, operationId, output));
        var stageDirectory = Path.Combine(
            directory.FullName,
            SaveTransactionStore.MetadataDirectoryName,
            context.WorkspaceId.ToString("N"),
            operationId.ToString("N"),
            "stage");
        var stagedStrings = Directory.EnumerateFiles(stageDirectory, Path.GetFileName(strings.Path), SearchOption.AllDirectories).Single();
        await File.WriteAllBytesAsync(stagedStrings, [99]);

        var secondRecovery = await coordinator.RecoverAsync(new RecoverSaveRequest(context.WorkspaceId, operationId, output));
        secondRecovery.Status.ShouldBe(RecoverSaveStatus.StillUnknown);
        secondRecovery.RepairRequired.ShouldBeFalse();
        secondRecovery.EvidenceToken.ShouldBeNull();
        var destinationBeforeRepair = await File.ReadAllBytesAsync(strings.Path);
        var repair = await coordinator.RepairAsync(new RepairSaveRequest(
            context.WorkspaceId,
            operationId,
            Guid.NewGuid(),
            context.Revision,
            output,
            firstRecovery.EvidenceToken!,
            RepairSaveDirection.CompletePrepared));
        repair.Status.ShouldBe(RepairSaveStatus.BlockedByExternalChange);
        (await File.ReadAllBytesAsync(strings.Path)).ShouldBe(destinationBeforeRepair);
    }

    /// <summary>Verifies a later valid save preserves older durable commit knowledge while invalidating its adoption evidence.</summary>
    [Fact]
    public async Task LaterSaveLeavesEarlierCommittedHistoryButNotStaleAdoptionEvidence()
    {
        using var directory = new TestDirectory();
        var output = CreateOutput(directory.FullName);
        await File.WriteAllBytesAsync(output.PluginPath, [1]);
        var firstBaseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var source = new TestSourceSet();
        var firstAdapter = CreateAdapter((request, token) => CreateStagedSetAsync(
            request,
            firstBaseline,
            new Dictionary<string, byte[]>(NativeSaveArtifactUtilities.PathComparer) { [output.PluginPath] = [2] },
            token));
        var firstContext = CreateContext(output, firstBaseline, source, firstAdapter.Object);
        var provider = new TestLeaseProvider();
        var coordinator = new WorkspaceSaveCoordinator(provider);
        var firstOperation = Guid.NewGuid();
        (await coordinator.SaveAsync(firstContext, new SaveRequest(firstOperation, firstContext.Revision, firstBaseline)))
            .Status.ShouldBe(SaveCommitStatus.Committed);

        var secondBaseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var secondAdapter = CreateAdapter((request, token) => CreateStagedSetAsync(
            request,
            secondBaseline,
            new Dictionary<string, byte[]>(NativeSaveArtifactUtilities.PathComparer) { [output.PluginPath] = [3] },
            token));
        var secondContext = CreateContext(output, secondBaseline, source, secondAdapter.Object);
        var secondOperation = Guid.NewGuid();
        (await coordinator.SaveAsync(secondContext, new SaveRequest(secondOperation, secondContext.Revision, secondBaseline)))
            .Status.ShouldBe(SaveCommitStatus.Committed);

        var earlier = await coordinator.RecoverAsync(new RecoverSaveRequest(firstContext.WorkspaceId, firstOperation, output));
        earlier.Status.ShouldBe(RecoverSaveStatus.Committed);
        earlier.ResolvedEvidence.ShouldBeNull();
        earlier.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
        var latest = await coordinator.RecoverAsync(new RecoverSaveRequest(secondContext.WorkspaceId, secondOperation, output));
        latest.Status.ShouldBe(RecoverSaveStatus.Committed);
        latest.ResolvedEvidence.ShouldNotBeNull();
    }

    /// <summary>Verifies content-identical artifacts retain their original physical identity beside a changed artifact.</summary>
    [Fact]
    public async Task SaveWithChangedAndUnchangedArtifactsCommitsExactMixedIdentityPlan()
    {
        using var directory = new TestDirectory();
        var output = CreateOutput(directory.FullName, LocalizedOutputMode.SeparateStringFiles);
        await File.WriteAllBytesAsync(output.PluginPath, [1]);
        var emptyBaseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var strings = emptyBaseline.Artifacts.First(artifact => artifact.Role == NativeArtifactRole.Strings);
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
        var coordinator = new WorkspaceSaveCoordinator(new TestLeaseProvider());

        var save = await coordinator.SaveAsync(
            context,
            new SaveRequest(Guid.NewGuid(), context.Revision, baseline));

        save.Status.ShouldBe(SaveCommitStatus.Committed);
        save.ResolvedEvidence.ShouldNotBeNull();
        (await File.ReadAllBytesAsync(output.PluginPath)).ShouldBe([3]);
        (await File.ReadAllBytesAsync(strings.Path)).ShouldBe([2]);
    }

    /// <summary>Verifies a publication file changed at the final fault seam is rejected before destination bytes move.</summary>
    [Fact]
    public async Task ChangedPublicationFileIsRejectedBeforeDestinationMutation()
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
        var pluginIndex = baseline.Artifacts
            .Select((artifact, index) => (artifact, index))
            .Single(entry => entry.artifact.Role == NativeArtifactRole.Plugin)
            .index;
        var publishPath = Path.Combine(
            directory.FullName,
            SaveTransactionStore.MetadataDirectoryName,
            context.WorkspaceId.ToString("N"),
            operationId.ToString("N"),
            "publish",
            $"{pluginIndex:D4}.bin");
        var coordinator = new WorkspaceSaveCoordinator(
            new TestLeaseProvider(),
            new SaveTransactionStore(),
            new ChangeFileBeforeMutationFileOperations(publishPath));

        var save = await coordinator.SaveAsync(
            context,
            new SaveRequest(operationId, context.Revision, baseline));

        save.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        save.Error!.Code.ShouldBe(EngineErrorCode.UnexpectedFailure);
        (await File.ReadAllBytesAsync(output.PluginPath)).ShouldBe([1]);
    }

    /// <summary>Verifies a repair interrupted after one move remains recognized and a new evidence-bound repair can finish.</summary>
    [Fact]
    public async Task InterruptedRepairCanBeRecoveredAndCompletedByNewRepairOperation()
    {
        using var directory = new TestDirectory();
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
        var context = CreateContext(output, baseline, new TestSourceSet(), adapter.Object);
        var saveOperationId = Guid.NewGuid();
        var provider = new TestLeaseProvider();
        var interruptedSave = new WorkspaceSaveCoordinator(
            provider,
            new SaveTransactionStore(),
            new ThrowAfterFirstMutationFileOperations());
        (await interruptedSave.SaveAsync(context, new SaveRequest(saveOperationId, context.Revision, baseline)))
            .Status.ShouldBe(SaveCommitStatus.CommitOutcomeUnknown);
        var normal = new WorkspaceSaveCoordinator(provider);
        var initialRecovery = await normal.RecoverAsync(new RecoverSaveRequest(context.WorkspaceId, saveOperationId, output));
        initialRecovery.RepairRequired.ShouldBeTrue();
        var interruptedRepair = new WorkspaceSaveCoordinator(
            provider,
            new SaveTransactionStore(),
            new ThrowAfterFirstMutationFileOperations());
        var firstRepair = await interruptedRepair.RepairAsync(new RepairSaveRequest(
            context.WorkspaceId,
            saveOperationId,
            Guid.NewGuid(),
            context.Revision,
            output,
            initialRecovery.EvidenceToken!,
            RepairSaveDirection.CompletePrepared));

        firstRepair.Status.ShouldBe(RepairSaveStatus.StillUnknown);
        var afterInterruption = await normal.RecoverAsync(new RecoverSaveRequest(context.WorkspaceId, saveOperationId, output));
        afterInterruption.RepairRequired.ShouldBeTrue();
        afterInterruption.EvidenceToken.ShouldNotBeNull();
        var finalRepair = await normal.RepairAsync(new RepairSaveRequest(
            context.WorkspaceId,
            saveOperationId,
            Guid.NewGuid(),
            context.Revision,
            output,
            afterInterruption.EvidenceToken!,
            RepairSaveDirection.CompletePrepared));

        finalRepair.Status.ShouldBe(RepairSaveStatus.PreparedSetCompleted);
        finalRepair.ResolvedEvidence.ShouldNotBeNull();
        foreach (var artifact in desired)
        {
            (await File.ReadAllBytesAsync(artifact.Key)).ShouldBe(artifact.Value);
        }
    }

    /// <summary>Verifies cancellation at the final pre-mutation seam leaves the original destination exact.</summary>
    [Fact]
    public async Task CancellationBeforeFirstDestinationMutationReturnsNotCommitted()
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
        using var cancellationSource = new CancellationTokenSource();
        var coordinator = new WorkspaceSaveCoordinator(
            new TestLeaseProvider(),
            new SaveTransactionStore(),
            new CancelBeforeFirstMutationFileOperations(cancellationSource));

        var save = await coordinator.SaveAsync(
            context,
            new SaveRequest(Guid.NewGuid(), context.Revision, baseline),
            cancellationSource.Token);

        save.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        save.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        (await File.ReadAllBytesAsync(output.PluginPath)).ShouldBe([1]);
    }

    /// <summary>Verifies cancellation after publication begins cannot interrupt the remaining complete-set moves.</summary>
    [Fact]
    public async Task CancellationAfterFirstDestinationMutationStillCompletesPublication()
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
            [output.PluginPath] = [8],
            [strings.Path] = [9]
        };
        var adapter = CreateAdapter((request, token) => CreateStagedSetAsync(request, baseline, desired, token));
        var context = CreateContext(output, baseline, new TestSourceSet(), adapter.Object);
        using var cancellationSource = new CancellationTokenSource();
        var coordinator = new WorkspaceSaveCoordinator(
            new TestLeaseProvider(),
            new SaveTransactionStore(),
            new CancelAfterFirstMutationFileOperations(cancellationSource));

        var save = await coordinator.SaveAsync(
            context,
            new SaveRequest(Guid.NewGuid(), context.Revision, baseline),
            cancellationSource.Token);

        save.Status.ShouldBe(SaveCommitStatus.Committed);
        (await File.ReadAllBytesAsync(output.PluginPath)).ShouldBe(desired[output.PluginPath]);
        (await File.ReadAllBytesAsync(strings.Path)).ShouldBe(desired[strings.Path]);
    }

    /// <summary>Verifies same-byte replacement of one mixed destination artifact is treated as foreign identity.</summary>
    [Fact]
    public async Task ForeignDestinationIdentityBlocksRecoveryRepairAuthority()
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
        var recognized = await normal.RecoverAsync(new RecoverSaveRequest(context.WorkspaceId, operationId, output));
        recognized.RepairRequired.ShouldBeTrue();
        var preservedIdentityPath = strings.Path + ".replaced";
        File.Move(strings.Path, preservedIdentityPath);
        await File.WriteAllBytesAsync(strings.Path, [2]);

        var foreign = await normal.RecoverAsync(new RecoverSaveRequest(context.WorkspaceId, operationId, output));
        foreign.Status.ShouldBe(RecoverSaveStatus.StillUnknown);
        foreign.RepairRequired.ShouldBeFalse();
        foreign.EvidenceToken.ShouldBeNull();
        foreign.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
        var repair = await normal.RepairAsync(new RepairSaveRequest(
            context.WorkspaceId,
            operationId,
            Guid.NewGuid(),
            context.Revision,
            output,
            recognized.EvidenceToken!,
            RepairSaveDirection.CompletePrepared));
        repair.Status.ShouldBe(RepairSaveStatus.BlockedByExternalChange);
        (await File.ReadAllBytesAsync(strings.Path)).ShouldBe([2]);
    }

    /// <summary>Verifies restored bytes with a repair-owned identity become the exact baseline of a later valid save.</summary>
    [Fact]
    public async Task RestoredBaselineWithNewIdentitySupportsLaterSave()
    {
        using var directory = new TestDirectory();
        var output = CreateOutput(directory.FullName, LocalizedOutputMode.SeparateStringFiles);
        await File.WriteAllBytesAsync(output.PluginPath, [1]);
        var shape = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var strings = shape.Artifacts.First(artifact => artifact.Role == NativeArtifactRole.Strings);
        Directory.CreateDirectory(Path.GetDirectoryName(strings.Path)!);
        await File.WriteAllBytesAsync(strings.Path, [2]);
        var baseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        await using var retainedOriginalIdentity = OperatingSystem.IsWindows()
            ? null
            : new FileStream(
                output.PluginPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete);
        var firstDesired = new Dictionary<string, byte[]>(NativeSaveArtifactUtilities.PathComparer)
        {
            [output.PluginPath] = [3],
            [strings.Path] = [4]
        };
        var firstAdapter = CreateAdapter((request, token) => CreateStagedSetAsync(request, baseline, firstDesired, token));
        var firstContext = CreateContext(output, baseline, new TestSourceSet(), firstAdapter.Object);
        var operationId = Guid.NewGuid();
        var provider = new TestLeaseProvider();
        var interrupted = new WorkspaceSaveCoordinator(
            provider,
            new SaveTransactionStore(),
            new ThrowAfterFirstMutationFileOperations());
        (await interrupted.SaveAsync(firstContext, new SaveRequest(operationId, firstContext.Revision, baseline)))
            .Status.ShouldBe(SaveCommitStatus.CommitOutcomeUnknown);
        var coordinator = new WorkspaceSaveCoordinator(provider);
        var recovery = await coordinator.RecoverAsync(new RecoverSaveRequest(firstContext.WorkspaceId, operationId, output));
        var restore = await coordinator.RepairAsync(new RepairSaveRequest(
            firstContext.WorkspaceId,
            operationId,
            Guid.NewGuid(),
            firstContext.Revision,
            output,
            recovery.EvidenceToken!,
            RepairSaveDirection.RestoreBaseline));

        restore.Status.ShouldBe(RepairSaveStatus.BaselineRestored);
        restore.ResultingBaseline.ShouldNotBeNull();
        NativeSaveArtifactUtilities.MatchBaseline(baseline, restore.ResultingBaseline!).ShouldBeFalse();
        (await File.ReadAllBytesAsync(output.PluginPath)).ShouldBe([1]);
        (await File.ReadAllBytesAsync(strings.Path)).ShouldBe([2]);
        var secondBaseline = restore.ResultingBaseline!;
        var secondDesired = new Dictionary<string, byte[]>(NativeSaveArtifactUtilities.PathComparer)
        {
            [output.PluginPath] = [7],
            [strings.Path] = [6]
        };
        var secondAdapter = CreateAdapter((request, token) => CreateStagedSetAsync(request, secondBaseline, secondDesired, token));
        var secondContext = CreateContext(output, secondBaseline, new TestSourceSet(), secondAdapter.Object);
        var secondSave = await coordinator.SaveAsync(
            secondContext,
            new SaveRequest(Guid.NewGuid(), secondContext.Revision, secondBaseline));

        secondSave.Status.ShouldBe(SaveCommitStatus.Committed);
        (await File.ReadAllBytesAsync(output.PluginPath)).ShouldBe([7]);
        (await File.ReadAllBytesAsync(strings.Path)).ShouldBe([6]);
    }

    /// <summary>Verifies a successful source verification carrying a different baseline still blocks save admission.</summary>
    [Fact]
    public async Task SuccessfulSourceVerificationWithDifferentBaselineBlocksSave()
    {
        using var directory = new TestDirectory();
        var output = CreateOutput(directory.FullName);
        await File.WriteAllBytesAsync(output.PluginPath, [1]);
        var baseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(GameRelease.SkyrimSE, output, CancellationToken.None);
        var source = new TestSourceSet(reportDifferentBaseline: true);
        var adapter = CreateAdapter((request, token) => CreateStagedSetAsync(
            request,
            baseline,
            new Dictionary<string, byte[]>(NativeSaveArtifactUtilities.PathComparer) { [output.PluginPath] = [2] },
            token));
        var context = CreateContext(output, baseline, source, adapter.Object);
        var coordinator = new WorkspaceSaveCoordinator(new TestLeaseProvider());

        var save = await coordinator.SaveAsync(
            context,
            new SaveRequest(Guid.NewGuid(), context.Revision, baseline));

        save.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        save.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
        adapter.Verify(value => value.WriteAndValidateAsync(
            It.IsAny<INativeSourceSet>(),
            It.IsAny<INativeOutputState>(),
            It.IsAny<NativeWriteRequest>(),
            It.IsAny<CancellationToken>()), Times.Never);
        (await File.ReadAllBytesAsync(output.PluginPath)).ShouldBe([1]);
    }

    /// <summary>Creates one canonical test output association.</summary>
    /// <param name="directory">The canonical temporary output directory.</param>
    /// <param name="localizedMode">The localized output mode.</param>
    /// <returns>The test output association.</returns>
    private static OutputAssociation CreateOutput(
        string directory,
        LocalizedOutputMode localizedMode = LocalizedOutputMode.Embedded)
    {
        return new OutputAssociation(
            Path.GetFullPath(Path.Combine(directory, "CoordinatorTest.esp")),
            ModKey.FromNameAndExtension("CoordinatorTest.esp"),
            localizedMode,
            OutputMasterStyle.Full);
    }

    /// <summary>Creates a borrowed save context around deterministic test doubles.</summary>
    /// <param name="output">The test output association.</param>
    /// <param name="baseline">The exact current output baseline.</param>
    /// <param name="source">The source-set test double.</param>
    /// <param name="adapter">The configured game adapter.</param>
    /// <returns>The workspace save context.</returns>
    private static WorkspaceSaveContext CreateContext(
        OutputAssociation output,
        OutputArtifactSetBaseline baseline,
        TestSourceSet source,
        IFormListGameAdapter adapter)
    {
        return new WorkspaceSaveContext(
            Guid.NewGuid(),
            new WorkspaceRevision(Guid.NewGuid(), 7),
            SupportedGame.Skyrim,
            GameRelease.SkyrimSE,
            adapter,
            source,
            new TestOutputState(),
            output,
            baseline);
    }

    /// <summary>Creates a strict-enough adapter mock for native staging and final reopen behavior.</summary>
    /// <param name="stage">The staged-write implementation.</param>
    /// <returns>The configured adapter mock.</returns>
    private static Mock<IFormListGameAdapter> CreateAdapter(
        Func<NativeWriteRequest, CancellationToken, Task<NativeStagedOutputSet>> stage)
    {
        var adapter = new Mock<IFormListGameAdapter>();
        adapter.SetupGet(value => value.Game).Returns(SupportedGame.Skyrim);
        adapter.Setup(value => value.SupportsRelease(GameRelease.SkyrimSE)).Returns(true);
        adapter.Setup(value => value.WriteAndValidateAsync(
                It.IsAny<INativeSourceSet>(),
                It.IsAny<INativeOutputState>(),
                It.IsAny<NativeWriteRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns((INativeSourceSet _, INativeOutputState _, NativeWriteRequest request, CancellationToken token) =>
                new ValueTask<EngineResult<NativeStagedOutputSet>>(StageResultAsync(stage, request, token)));
        adapter.Setup(value => value.ReopenOutputAsync(
                It.IsAny<INativeSourceSet>(),
                It.IsAny<OutputAssociation>(),
                It.IsAny<OutputArtifactSetBaseline>(),
                It.IsAny<CancellationToken>()))
            .Returns((INativeSourceSet _, OutputAssociation output, OutputArtifactSetBaseline baseline, CancellationToken _) =>
                new ValueTask<EngineResult<NativeOutputOpenResult>>(EngineResult<NativeOutputOpenResult>.Success(
                    new NativeOutputOpenResult(new TestOutputState(), output, baseline))));
        return adapter;
    }

    /// <summary>Wraps an asynchronous staged test result in the engine result contract.</summary>
    /// <param name="stage">The staged-write implementation.</param>
    /// <param name="request">The adapter write request.</param>
    /// <param name="cancellationToken">The staging cancellation token.</param>
    /// <returns>The successful staged engine result.</returns>
    private static async Task<EngineResult<NativeStagedOutputSet>> StageResultAsync(
        Func<NativeWriteRequest, CancellationToken, Task<NativeStagedOutputSet>> stage,
        NativeWriteRequest request,
        CancellationToken cancellationToken)
    {
        return EngineResult<NativeStagedOutputSet>.Success(await stage(request, cancellationToken));
    }

    /// <summary>Writes a complete staged artifact set for the requested destination baseline.</summary>
    /// <param name="request">The private adapter write request.</param>
    /// <param name="baseline">The complete destination association.</param>
    /// <param name="desiredFiles">The desired present file bytes keyed by destination path.</param>
    /// <param name="cancellationToken">The staging cancellation token.</param>
    /// <returns>The complete staged set.</returns>
    private static async Task<NativeStagedOutputSet> CreateStagedSetAsync(
        NativeWriteRequest request,
        OutputArtifactSetBaseline baseline,
        IReadOnlyDictionary<string, byte[]> desiredFiles,
        CancellationToken cancellationToken)
    {
        var outputDirectory = Path.GetDirectoryName(request.Output.PluginPath)!;
        var mappings = new List<NativeStagedArtifactMapping>();
        foreach (var destination in baseline.Artifacts)
        {
            var relative = Path.GetRelativePath(outputDirectory, destination.Path);
            var stagedPath = Path.GetFullPath(Path.Combine(request.StagingDirectoryPath, relative));
            if (desiredFiles.TryGetValue(destination.Path, out var bytes))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(stagedPath)!);
                await File.WriteAllBytesAsync(stagedPath, bytes, cancellationToken);
            }

            var staged = await NativeFileInspector.InspectAsync(
                stagedPath,
                destination.Role,
                destination.Language,
                mustExist: false,
                cancellationToken);
            mappings.Add(new NativeStagedArtifactMapping(staged, destination.Path));
        }

        return new NativeStagedOutputSet(
            NativeWriteDisposition.StagedChanges,
            new TestStagedOutputSet(),
            mappings);
    }

    /// <summary>Owns an immutable empty source baseline for coordinator tests.</summary>
    private sealed class TestSourceSet : INativeSourceSet
    {
        /// <summary>Whether verification should deliberately return another successful baseline.</summary>
        private readonly bool ReportDifferentBaseline;

        /// <summary>Initializes an empty deterministic source baseline.</summary>
        /// <param name="reportDifferentBaseline">Whether verification reports success with a different baseline identity.</param>
        internal TestSourceSet(bool reportDifferentBaseline = false)
        {
            Baseline = new NativeSourceInputBaseline(Guid.NewGuid(), Array.Empty<NativeArtifactAssociation>());
            ReportDifferentBaseline = reportDifferentBaseline;
        }

        /// <summary>Initializes a source set with one exact physical test baseline.</summary>
        /// <param name="baseline">The exact source baseline returned by normal verification.</param>
        internal TestSourceSet(NativeSourceInputBaseline baseline)
        {
            Baseline = baseline;
        }

        /// <inheritdoc />
        public NativeSourceInputBaseline Baseline { get; }

        /// <inheritdoc />
        public Task<EngineResult<NativeSourceInputBaseline>> VerifyUnchangedAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var verified = ReportDifferentBaseline
                ? new NativeSourceInputBaseline(Guid.NewGuid(), Array.Empty<NativeArtifactAssociation>())
                : Baseline;
            return Task.FromResult(EngineResult<NativeSourceInputBaseline>.Success(verified));
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>Provides an opaque disposable output state for adapter mocks.</summary>
    private sealed class TestOutputState : INativeOutputState
    {
        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>Provides an opaque disposable natively reopened staged state for adapter mocks.</summary>
    private sealed class TestStagedOutputSet : INativeStagedOutputSet
    {
        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>Provides deterministic acquired or missing output-directory leases.</summary>
    private sealed class TestLeaseProvider : IOutputDirectoryLeaseProvider
    {
        /// <summary>Whether existing-only acquisition recognizes the stable guard.</summary>
        private bool GuardExists;

        /// <summary>Initializes the test lease provider.</summary>
        /// <param name="guardExists">Whether a stable guard already exists.</param>
        internal TestLeaseProvider(bool guardExists = true)
        {
            GuardExists = guardExists;
        }

        /// <summary>Gets observed acquisition modes in call order.</summary>
        internal List<OutputDirectoryLeaseMode> Modes { get; } = new();

        /// <inheritdoc />
        public ValueTask<EngineResult<OutputDirectoryLeaseAcquisition>> AcquireAsync(
            string outputDirectoryPath,
            OutputDirectoryLeaseMode mode,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Modes.Add(mode);
            if (mode == OutputDirectoryLeaseMode.ExistingOnly && !GuardExists)
            {
                return ValueTask.FromResult(EngineResult<OutputDirectoryLeaseAcquisition>.Success(
                    new OutputDirectoryLeaseAcquisition(OutputDirectoryLeaseAcquisitionStatus.GuardNotFound, null)));
            }

            GuardExists = true;
            return ValueTask.FromResult(EngineResult<OutputDirectoryLeaseAcquisition>.Success(
                new OutputDirectoryLeaseAcquisition(
                    OutputDirectoryLeaseAcquisitionStatus.Acquired,
                    new TestLease(Path.GetFullPath(outputDirectoryPath)))));
        }
    }

    /// <summary>Represents a no-op exclusive test lease for one canonical directory.</summary>
    private sealed class TestLease : IOutputDirectoryLease
    {
        /// <summary>Initializes a test lease.</summary>
        /// <param name="outputDirectoryPath">The canonical protected directory.</param>
        internal TestLease(string outputDirectoryPath)
        {
            OutputDirectoryPath = outputDirectoryPath;
        }

        /// <inheritdoc />
        public string OutputDirectoryPath { get; }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>Injects a process interruption immediately after the first successful destination mutation.</summary>
    private sealed class ThrowAfterFirstMutationFileOperations : SaveTransactionFileOperations
    {
        /// <summary>Whether the interruption has already been injected.</summary>
        private bool HasThrown;

        /// <inheritdoc />
        internal override void AfterDestinationMutation(Guid saveOperationId, int artifactIndex)
        {
            if (!HasThrown)
            {
                HasThrown = true;
                throw new IOException("Injected interruption after the first destination mutation.");
            }
        }
    }

    /// <summary>Changes one transaction-owned publication file at the final pre-mutation fault seam.</summary>
    private sealed class ChangeFileBeforeMutationFileOperations : SaveTransactionFileOperations
    {
        /// <summary>The exact publication path changed once before its destination move.</summary>
        private readonly string PathToChange;

        /// <summary>Whether the configured change was already injected.</summary>
        private bool HasChanged;

        /// <summary>Initializes the deterministic publication-file fault.</summary>
        /// <param name="pathToChange">The exact publication path to overwrite.</param>
        internal ChangeFileBeforeMutationFileOperations(string pathToChange)
        {
            PathToChange = pathToChange;
        }

        /// <inheritdoc />
        internal override void BeforeDestinationMutation(Guid saveOperationId, int artifactIndex)
        {
            if (!HasChanged)
            {
                HasChanged = true;
                File.WriteAllBytes(PathToChange, [99]);
            }
        }
    }

    /// <summary>Cancels the caller token at the final seam before the first destination mutation.</summary>
    private sealed class CancelBeforeFirstMutationFileOperations : SaveTransactionFileOperations
    {
        /// <summary>The caller-owned cancellation source.</summary>
        private readonly CancellationTokenSource CancellationSource;

        /// <summary>Initializes the deterministic pre-mutation cancellation.</summary>
        /// <param name="cancellationSource">The source canceled from the fault seam.</param>
        internal CancelBeforeFirstMutationFileOperations(CancellationTokenSource cancellationSource)
        {
            CancellationSource = cancellationSource;
        }

        /// <inheritdoc />
        internal override void BeforeDestinationMutation(Guid saveOperationId, int artifactIndex)
        {
            CancellationSource.Cancel();
        }
    }

    /// <summary>Cancels the caller token after the first successful destination mutation.</summary>
    private sealed class CancelAfterFirstMutationFileOperations : SaveTransactionFileOperations
    {
        /// <summary>The caller-owned cancellation source.</summary>
        private readonly CancellationTokenSource CancellationSource;

        /// <summary>Whether the configured cancellation was already injected.</summary>
        private bool HasCanceled;

        /// <summary>Initializes the deterministic post-mutation cancellation.</summary>
        /// <param name="cancellationSource">The source canceled from the fault seam.</param>
        internal CancelAfterFirstMutationFileOperations(CancellationTokenSource cancellationSource)
        {
            CancellationSource = cancellationSource;
        }

        /// <inheritdoc />
        internal override void AfterDestinationMutation(Guid saveOperationId, int artifactIndex)
        {
            if (!HasCanceled)
            {
                HasCanceled = true;
                CancellationSource.Cancel();
            }
        }
    }

    /// <summary>Owns one disposable physical temporary directory for save-coordinator tests.</summary>
    private sealed class TestDirectory : IDisposable
    {
        /// <summary>The owned temporary directory.</summary>
        private readonly DirectoryInfo Directory;

        /// <summary>Initializes a new empty temporary directory.</summary>
        internal TestDirectory()
        {
            Directory = System.IO.Directory.CreateTempSubdirectory();
        }

        /// <summary>Gets the absolute owned directory path.</summary>
        internal string FullName => Directory.FullName;

        /// <summary>Recursively removes the owned temporary directory.</summary>
        public void Dispose()
        {
            if (Directory.Exists)
            {
                Directory.Delete(recursive: true);
            }
        }
    }
}
