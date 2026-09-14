using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.Engine;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Engine.PluginOutputs;
using CreationsForge.Core.Engine.Persistence;
using CreationsForge.Core.Enums;
using CreationsForge.Skyrim.PluginAdapter;
using Mutagen.Bethesda.Plugins;
using Serilog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Integration;

/// <summary>Verifies physically terminal plugin save recovery remains usable through the complete workspace lifecycle.</summary>
public sealed class WorkspaceRecoveryLifecycleTests
{
    /// <summary>Identifies the nonterminal journal and matching physical state seeded by a recovery test.</summary>
    public enum RecoveryPhysicalState
    {
        /// <summary>The Preparing journal retains the exact original destination.</summary>
        PreparingAllBefore,

        /// <summary>The Prepared journal is paired with the complete prepared destination.</summary>
        PreparedAllPrepared,
    }

    /// <summary>Verifies an unpublished partial initialization directory cannot block a valid plugin output in the same directory.</summary>
    /// <param name="game">The plugin game exercised through production composition.</param>
    /// <returns>A task that completes after a fresh workspace selects and reads the unaffected output.</returns>
    [Theory]
    [InlineData(SupportedGame.Starfield)]
    [InlineData(SupportedGame.Fallout4)]
    [InlineData(SupportedGame.Skyrim)]
    public async Task PartialInitializationDirectory_DoesNotBlockFreshPluginOutputSelection(SupportedGame game)
    {
        using var fixture = WorkspaceIntegrationFixture.Create(game);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        await using var services = EngineComposition.Create();
        var association = CreateAssociation(fixture, "InitializationInterruption", "UnaffectedOutput.esm");
        var committed = await CreateCommittedOutputAsync(
            services,
            fixture,
            association,
            "UnaffectedByInitialization");
        var expectedOutputBytes = File.ReadAllBytes(association.PluginPath);

        var interruptedOperationId = Guid.NewGuid();
        var paths = new SaveTransactionPaths(
            Path.GetDirectoryName(association.PluginPath)!,
            Guid.NewGuid(),
            interruptedOperationId);
        Directory.CreateDirectory(paths.WorkspaceDirectoryPath);
        var initializationDirectory = Path.Combine(
            paths.WorkspaceDirectoryPath,
            $"{SaveTransactionStore.InitializationDirectoryPrefix}{interruptedOperationId:N}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(initializationDirectory);
        await File.WriteAllBytesAsync(
            Path.Combine(initializationDirectory, SaveTransactionStore.JournalFileName),
            [1, 2, 3],
            TestContext.Current.CancellationToken);

        var opened = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        opened.Succeeded.ShouldBeTrue(DescribeError(opened.Error));
        await using (var workspace = opened.Value!)
        {
            var selected = await workspace.SelectOutputAsync(
                new SelectOutputRequest(
                    Guid.NewGuid(),
                    workspace.Revision,
                    OutputSelectionMode.OpenExisting,
                    association),
                TestContext.Current.CancellationToken);
            selected.Succeeded.ShouldBeTrue(DescribeError(selected.Error));
            workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
            await AssertEditorIdAsync(
                workspace,
                association,
                committed.FormKey,
                "UnaffectedByInitialization");
        }

        Directory.Exists(initializationDirectory).ShouldBeTrue();
        File.ReadAllBytes(association.PluginPath).ShouldBe(expectedOutputBytes);
        AssertArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());
    }

    /// <summary>Verifies resolving a physically terminal journal permits discard, reopen, and fresh-workspace selection.</summary>
    /// <param name="game">The plugin game exercised through production composition.</param>
    /// <param name="physicalState">The exact nonterminal journal and physical destination combination.</param>
    /// <returns>A task that completes after current and fresh workspace lifecycle operations remain usable.</returns>
    [Theory]
    [InlineData(SupportedGame.Starfield, RecoveryPhysicalState.PreparingAllBefore)]
    [InlineData(SupportedGame.Starfield, RecoveryPhysicalState.PreparedAllPrepared)]
    [InlineData(SupportedGame.Fallout4, RecoveryPhysicalState.PreparingAllBefore)]
    [InlineData(SupportedGame.Fallout4, RecoveryPhysicalState.PreparedAllPrepared)]
    [InlineData(SupportedGame.Skyrim, RecoveryPhysicalState.PreparingAllBefore)]
    [InlineData(SupportedGame.Skyrim, RecoveryPhysicalState.PreparedAllPrepared)]
    public async Task ResolveOutputRecovery_PhysicallyTerminalJournal_RemainsUsableAcrossLifecycle(
        SupportedGame game,
        RecoveryPhysicalState physicalState)
    {
        using var fixture = WorkspaceIntegrationFixture.Create(game);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        await using var services = EngineComposition.Create();
        var association = CreateAssociation(fixture, "RecoveryDestination", "RecoveryLifecycle.esm");
        var before = await CreateCommittedOutputAsync(
            services,
            fixture,
            association,
            "BeforeRecovery");
        var expectedEditorId = "BeforeRecovery";
        string? preparedPluginPath = null;
        if (physicalState == RecoveryPhysicalState.PreparedAllPrepared)
        {
            var preparedAssociation = CreateAssociation(fixture, "RecoveryPrepared", "RecoveryLifecycle.esm");
            var prepared = await CreateCommittedOutputAsync(
                services,
                fixture,
                preparedAssociation,
                "PreparedRecovery");
            prepared.FormKey.ShouldBe(before.FormKey);
            preparedPluginPath = preparedAssociation.PluginPath;
            expectedEditorId = "PreparedRecovery";
        }

        var sourceBaseline = await CaptureSourceBaselineAsync(fixture.CreateOpenRequest());
        var seeded = await SeedRecoveryJournalAsync(
            fixture,
            association,
            before.Baseline,
            sourceBaseline,
            physicalState,
            preparedPluginPath);
        var expectedOutputBytes = File.ReadAllBytes(association.PluginPath);

        var opened = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        opened.Succeeded.ShouldBeTrue(DescribeError(opened.Error));
        await using (var workspace = opened.Value!)
        {
            var blockedSelection = await workspace.SelectOutputAsync(
                new SelectOutputRequest(
                    Guid.NewGuid(),
                    workspace.Revision,
                    OutputSelectionMode.OpenExisting,
                    association),
                TestContext.Current.CancellationToken);
            blockedSelection.Succeeded.ShouldBeFalse();
            blockedSelection.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.RepairRequired);
            workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.RecoveryRequired);
            workspace.OutputSynchronization.PendingSave.ShouldNotBeNull().OriginalWorkspaceId
                .ShouldBe(seeded.WorkspaceId);
            workspace.OutputSynchronization.PendingSave.SaveOperationId.ShouldBe(seeded.SaveOperationId);

            var recovery = await services.SaveCoordinator.RecoverAsync(
                new RecoverSaveRequest(seeded.WorkspaceId, seeded.SaveOperationId, association),
                TestContext.Current.CancellationToken);
            recovery.Error.ShouldBeNull(DescribeError(recovery.Error));
            recovery.Status.ShouldBe(seeded.ExpectedStatus);
            var evidence = recovery.ResolvedEvidence.ShouldNotBeNull();
            recovery.EvidenceToken.ShouldNotBeNull();

            var resolved = await workspace.ResolveOutputRecoveryAsync(
                new ResolveOutputRecoveryRequest(
                    Guid.NewGuid(),
                    workspace.Revision,
                    OutputRecoveryAdoptionMode.ReopenResolvedOutput,
                    evidence),
                TestContext.Current.CancellationToken);
            resolved.Succeeded.ShouldBeTrue(DescribeError(resolved.Error));
            workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
            await AssertEditorIdAsync(workspace, association, before.FormKey, expectedEditorId);

            var terminalRecovery = await services.SaveCoordinator.RecoverAsync(
                new RecoverSaveRequest(seeded.WorkspaceId, seeded.SaveOperationId, association),
                TestContext.Current.CancellationToken);
            terminalRecovery.Error.ShouldBeNull(DescribeError(terminalRecovery.Error));
            terminalRecovery.Status.ShouldBe(seeded.ExpectedStatus);
            terminalRecovery.EvidenceToken.ShouldBe(recovery.EvidenceToken);
            terminalRecovery.ResolvedEvidence.ShouldNotBeNull();

            var discard = await workspace.DiscardChangesAsync(
                new DiscardChangesRequest(Guid.NewGuid(), workspace.Revision, resolved.Value!.Baseline),
                TestContext.Current.CancellationToken);
            discard.Succeeded.ShouldBeTrue(DescribeError(discard.Error));
            workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
            await AssertEditorIdAsync(workspace, association, before.FormKey, expectedEditorId);

            var reopened = await workspace.ReopenOutputAsync(
                new ReopenOutputRequest(Guid.NewGuid(), workspace.Revision, resolved.Value.Baseline),
                TestContext.Current.CancellationToken);
            reopened.Succeeded.ShouldBeTrue(DescribeError(reopened.Error));
            workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
            await AssertEditorIdAsync(workspace, association, before.FormKey, expectedEditorId);
        }

        var freshOpen = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        freshOpen.Succeeded.ShouldBeTrue(DescribeError(freshOpen.Error));
        await using (var workspace = freshOpen.Value!)
        {
            var selected = await workspace.SelectOutputAsync(
                new SelectOutputRequest(
                    Guid.NewGuid(),
                    workspace.Revision,
                    OutputSelectionMode.OpenExisting,
                    association),
                TestContext.Current.CancellationToken);
            selected.Succeeded.ShouldBeTrue(DescribeError(selected.Error));
            workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
            await AssertEditorIdAsync(workspace, association, before.FormKey, expectedEditorId);
        }

        File.ReadAllBytes(association.PluginPath).ShouldBe(expectedOutputBytes);
        AssertArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());
    }

    /// <summary>Verifies cancellation after durable terminalization leaves the original evidence valid for a later adoption retry.</summary>
    /// <returns>A task that completes after the canceled adoption is retried successfully.</returns>
    [Fact]
    public async Task ResolveOutputRecovery_CanceledAfterTerminalPublication_RetriesWithStableEvidence()
    {
        using var fixture = WorkspaceIntegrationFixture.Create(SupportedGame.Skyrim);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        await using var services = EngineComposition.Create();
        var association = CreateAssociation(fixture, "RecoveryCancellation", "CanceledRecovery.esm");
        var before = await CreateCommittedOutputAsync(
            services,
            fixture,
            association,
            "CancellationRecovery");
        var sourceBaseline = await CaptureSourceBaselineAsync(fixture.CreateOpenRequest());
        var seeded = await SeedRecoveryJournalAsync(
            fixture,
            association,
            before.Baseline,
            sourceBaseline,
            RecoveryPhysicalState.PreparingAllBefore,
            preparedPluginPath: null);
        var expectedOutputBytes = File.ReadAllBytes(association.PluginPath);

        using var cancelAfterValidation = new CancellationTokenSource();
        var cancelingCoordinator = new CancelAfterValidationSaveCoordinator(
            services.SaveCoordinator,
            cancelAfterValidation);
        var cancellationFactory = CreateSkyrimWorkspaceFactory(cancelingCoordinator);
        var opened = await cancellationFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        opened.Succeeded.ShouldBeTrue(DescribeError(opened.Error));
        await using var workspace = opened.Value!;
        var blockedSelection = await workspace.SelectOutputAsync(
            new SelectOutputRequest(
                Guid.NewGuid(),
                workspace.Revision,
                OutputSelectionMode.OpenExisting,
                association),
            TestContext.Current.CancellationToken);
        blockedSelection.Succeeded.ShouldBeFalse();
        workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.RecoveryRequired);

        var recoveryRequest = new RecoverSaveRequest(seeded.WorkspaceId, seeded.SaveOperationId, association);
        var recovery = await services.SaveCoordinator.RecoverAsync(
            recoveryRequest,
            TestContext.Current.CancellationToken);
        recovery.Error.ShouldBeNull(DescribeError(recovery.Error));
        var evidence = recovery.ResolvedEvidence.ShouldNotBeNull();

        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await workspace.ResolveOutputRecoveryAsync(
                new ResolveOutputRecoveryRequest(
                    Guid.NewGuid(),
                    workspace.Revision,
                    OutputRecoveryAdoptionMode.ReopenResolvedOutput,
                    evidence),
                cancelAfterValidation.Token));
        cancelAfterValidation.IsCancellationRequested.ShouldBeTrue();
        workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.RecoveryRequired);

        var terminalRecovery = await services.SaveCoordinator.RecoverAsync(
            recoveryRequest,
            TestContext.Current.CancellationToken);
        terminalRecovery.Error.ShouldBeNull(DescribeError(terminalRecovery.Error));
        terminalRecovery.Status.ShouldBe(RecoverSaveStatus.NotCommitted);
        terminalRecovery.EvidenceToken.ShouldBe(evidence.EvidenceToken);
        terminalRecovery.ResolvedEvidence.ShouldNotBeNull();

        var retry = await workspace.ResolveOutputRecoveryAsync(
            new ResolveOutputRecoveryRequest(
                Guid.NewGuid(),
                workspace.Revision,
                OutputRecoveryAdoptionMode.ReopenResolvedOutput,
                evidence),
            TestContext.Current.CancellationToken);
        retry.Succeeded.ShouldBeTrue(DescribeError(retry.Error));
        workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        await AssertEditorIdAsync(workspace, association, before.FormKey, "CancellationRecovery");
        File.ReadAllBytes(association.PluginPath).ShouldBe(expectedOutputBytes);
        AssertArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());
    }

    /// <summary>Creates a production Skyrim workspace factory around a test-controlled recovery coordinator.</summary>
    /// <param name="saveCoordinator">The coordinator used for admission, save, recovery, repair, and adoption validation.</param>
    /// <returns>A workspace factory using the real Skyrim source, output, edit, and lease services.</returns>
    private static FormListWorkspaceFactory CreateSkyrimWorkspaceFactory(IWorkspaceSaveCoordinator saveCoordinator)
    {
        var outputService = new SkyrimPluginOutputService(new PluginOutputInputLoader());
        var adapter = new SkyrimFormListGameAdapter(
            new SkyrimPluginSourceLoader(new PluginSourceInputLoader()),
            outputService,
            new SkyrimRecordEditService(outputService.Inspector));
        return new FormListWorkspaceFactory(
            [adapter],
            saveCoordinator,
            new OutputDirectoryLeaseProvider(),
            Log.Logger);
    }

    /// <summary>Creates a full-master embedded output association in one fixture-owned directory.</summary>
    /// <param name="fixture">The generated plugin fixture.</param>
    /// <param name="directoryName">The fixture-relative output directory name.</param>
    /// <param name="fileName">The output plugin file name.</param>
    /// <returns>The complete output association.</returns>
    private static OutputAssociation CreateAssociation(
        WorkspaceIntegrationFixture fixture,
        string directoryName,
        string fileName)
    {
        var directory = fixture.RootDirectory.CreateSubdirectory(directoryName);
        var modKey = ModKey.FromNameAndExtension(fileName);
        return new OutputAssociation(
            Path.Combine(directory.FullName, fileName),
            modKey,
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
    }

    /// <summary>Creates and commits one valid plugin output through the production workspace composition.</summary>
    /// <param name="services">The production plugin service lifetime.</param>
    /// <param name="fixture">The generated plugin fixture.</param>
    /// <param name="association">The absent output to create.</param>
    /// <param name="editorId">The exact EditorID stored in the output record.</param>
    /// <returns>The committed FormList identity and output baseline.</returns>
    private static async Task<(FormKey FormKey, OutputArtifactSetBaseline Baseline)> CreateCommittedOutputAsync(
        EngineServices services,
        WorkspaceIntegrationFixture fixture,
        OutputAssociation association,
        string editorId)
    {
        var opened = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        opened.Succeeded.ShouldBeTrue(DescribeError(opened.Error));
        await using var workspace = opened.Value!;
        var selected = await workspace.SelectOutputAsync(
            new SelectOutputRequest(
                Guid.NewGuid(),
                workspace.Revision,
                OutputSelectionMode.CreateNew,
                association),
            TestContext.Current.CancellationToken);
        selected.Succeeded.ShouldBeTrue(DescribeError(selected.Error));
        var edit = await workspace.BeginEditAsync(
            new BeginEditRequest(Guid.NewGuid(), workspace.Revision, FormListEditRole.New),
            TestContext.Current.CancellationToken);
        edit.Succeeded.ShouldBeTrue(DescribeError(edit.Error));
        var applied = await workspace.ApplyFormListEditAsync(
            new FormListEditRequest(
                Guid.NewGuid(),
                workspace.Revision,
                edit.Value!.EditId,
                new SetEditorIdEdit(editorId)),
            TestContext.Current.CancellationToken);
        applied.Succeeded.ShouldBeTrue(DescribeError(applied.Error));
        var saved = await workspace.SaveAsync(
            new SaveRequest(Guid.NewGuid(), workspace.Revision, selected.Value!.Baseline),
            TestContext.Current.CancellationToken);
        saved.Status.ShouldBe(SaveCommitStatus.Committed, DescribeError(saved.Error));
        saved.CommittedBaseline.ShouldNotBeNull();
        return (edit.Value.FormKey, saved.CommittedBaseline);
    }

    /// <summary>Captures the deterministic source baseline produced by the same explicit plugin input loader used by composition.</summary>
    /// <param name="request">The generated fixture's complete explicit open request.</param>
    /// <returns>The exact completed source baseline.</returns>
    private static async Task<PluginSourceInputBaseline> CaptureSourceBaselineAsync(WorkspaceOpenRequest request)
    {
        var prepared = await new PluginSourceInputLoader().PrepareAsync(
            request,
            TestContext.Current.CancellationToken);
        prepared.Succeeded.ShouldBeTrue(DescribeError(prepared.Error));
        await using var inputs = prepared.Value!;
        var completed = await inputs.CompleteOpenAsync(TestContext.Current.CancellationToken);
        completed.Succeeded.ShouldBeTrue(DescribeError(completed.Error));
        return completed.Value.ShouldNotBeNull();
    }

    /// <summary>Seeds one canonical nonterminal journal with an exact all-before or all-prepared plugin destination.</summary>
    /// <param name="fixture">The generated plugin fixture that owns all disposable paths.</param>
    /// <param name="association">The committed output being recovered.</param>
    /// <param name="beforeBaseline">The exact original output baseline.</param>
    /// <param name="sourceBaseline">The exact unchanged source baseline.</param>
    /// <param name="physicalState">The journal phase and physical destination state to create.</param>
    /// <param name="preparedPluginPath">The separately generated valid prepared plugin, when required.</param>
    /// <returns>The seeded save identity and expected terminal status.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="physicalState"/> is undefined.</exception>
    private static async Task<(Guid WorkspaceId, Guid SaveOperationId, RecoverSaveStatus ExpectedStatus)> SeedRecoveryJournalAsync(
        WorkspaceIntegrationFixture fixture,
        OutputAssociation association,
        OutputArtifactSetBaseline beforeBaseline,
        PluginSourceInputBaseline sourceBaseline,
        RecoveryPhysicalState physicalState,
        string? preparedPluginPath)
    {
        var workspaceId = Guid.NewGuid();
        var operationId = Guid.NewGuid();
        var outputDirectory = Path.GetDirectoryName(association.PluginPath)!;
        var paths = new SaveTransactionPaths(outputDirectory, workspaceId, operationId);
        var store = new SaveTransactionStore();
        var saveRevision = new WorkspaceRevision(Guid.NewGuid(), 17);
        var journal = new SaveTransactionJournal(
            workspaceId,
            operationId,
            new string('A', 64),
            saveRevision,
            fixture.Game,
            fixture.CreateOpenRequest().Release,
            sourceBaseline,
            association,
            beforeBaseline,
            PluginWriteDisposition.StagedChanges,
            SaveTransactionPhase.Preparing,
            0,
            [],
            null,
            []);
        await store.InitializeAsync(paths, journal, TestContext.Current.CancellationToken);

        switch (physicalState)
        {
            case RecoveryPhysicalState.PreparingAllBefore:
                return (workspaceId, operationId, RecoverSaveStatus.NotCommitted);
            case RecoveryPhysicalState.PreparedAllPrepared:
                ArgumentException.ThrowIfNullOrWhiteSpace(preparedPluginPath);
                store.CreateEmptySubdirectory(paths.StagingDirectoryPath, "test recovery staging directory");
                store.CreateEmptySubdirectory(paths.PublishDirectoryPath, "test recovery publication directory");
                store.CreateEmptySubdirectory(paths.BackupDirectoryPath, "test recovery backup directory");
                store.CreateEmptySubdirectory(paths.RetiredDirectoryPath, "test recovery retired directory");
                var plans = new List<SaveArtifactPlan>(beforeBaseline.Artifacts.Count);
                string? pluginPublishPath = null;
                for (var index = 0; index < beforeBaseline.Artifacts.Count; index++)
                {
                    var beforeArtifact = beforeBaseline.Artifacts[index];
                    var stagedPath = Path.Combine(paths.StagingDirectoryPath, $"{index:D4}-staged.bin");
                    var retiredPath = Path.Combine(paths.RetiredDirectoryPath, $"{index:D4}-retired.bin");
                    if (beforeArtifact.Role != PluginArtifactRole.Plugin)
                    {
                        beforeArtifact.Fingerprint.Exists.ShouldBeFalse(
                            "The embedded recovery fixture expects every loose localization sidecar to be absent.");
                        var absentStaged = await PluginFileInspector.InspectAsync(
                            stagedPath,
                            beforeArtifact.Role,
                            beforeArtifact.Language,
                            mustExist: false,
                            TestContext.Current.CancellationToken);
                        plans.Add(new SaveArtifactPlan(
                            beforeArtifact,
                            absentStaged,
                            publish: null,
                            backup: null,
                            retiredPath: retiredPath));
                        continue;
                    }

                    pluginPublishPath.ShouldBeNull(
                        "The complete output baseline must contain exactly one plugin artifact.");
                    var publishPath = Path.Combine(paths.PublishDirectoryPath, $"{index:D4}-publish.bin");
                    var backupPath = Path.Combine(paths.BackupDirectoryPath, $"{index:D4}-backup.bin");
                    File.Copy(preparedPluginPath, stagedPath);
                    File.Copy(preparedPluginPath, publishPath);
                    File.Copy(association.PluginPath, backupPath);
                    var staged = await PluginFileInspector.InspectAsync(
                        stagedPath,
                        beforeArtifact.Role,
                        beforeArtifact.Language,
                        mustExist: true,
                        TestContext.Current.CancellationToken);
                    var publish = await PluginFileInspector.InspectAsync(
                        publishPath,
                        beforeArtifact.Role,
                        beforeArtifact.Language,
                        mustExist: true,
                        TestContext.Current.CancellationToken);
                    var backup = await PluginFileInspector.InspectAsync(
                        backupPath,
                        beforeArtifact.Role,
                        beforeArtifact.Language,
                        mustExist: true,
                        TestContext.Current.CancellationToken);
                    plans.Add(new SaveArtifactPlan(beforeArtifact, staged, publish, backup, retiredPath));
                    pluginPublishPath = publishPath;
                }

                journal = journal.WithPreparedArtifacts(PluginWriteDisposition.StagedChanges, plans);
                await store.WriteAsync(paths, journal, TestContext.Current.CancellationToken);
                File.Move(
                    pluginPublishPath.ShouldNotBeNull(
                        "The complete output baseline must contain one plugin artifact."),
                    association.PluginPath,
                    overwrite: true);
                return (workspaceId, operationId, RecoverSaveStatus.Committed);
            default:
                throw new ArgumentOutOfRangeException(nameof(physicalState), physicalState, "The recovery integration requires a supported physical state.");
        }
    }

    /// <summary>Asserts one committed output FormList retains the expected exact EditorID.</summary>
    /// <param name="workspace">The selected production workspace.</param>
    /// <param name="association">The selected output association.</param>
    /// <param name="formKey">The exact output-owned FormList identity.</param>
    /// <param name="expectedEditorId">The EditorID expected after plugin reopen.</param>
    /// <returns>A task that completes after detached plugin inspection.</returns>
    private static async Task AssertEditorIdAsync(
        IFormListWorkspace workspace,
        OutputAssociation association,
        FormKey formKey,
        string expectedEditorId)
    {
        var read = await workspace.ReadFormListViewAsync(
            new ReferenceRequest(formKey, RecordScope.StagedOutput, association.ModKey),
            TestContext.Current.CancellationToken);
        read.Succeeded.ShouldBeTrue(DescribeError(read.Error));
        var record = read.Value.ShouldNotBeNull().Record;
        record.ShouldNotBeNull();
        record.Value.GetProperty("EditorID").GetString().ShouldBe(expectedEditorId);
    }

    /// <summary>Asserts every source artifact path and byte sequence remains unchanged.</summary>
    /// <param name="expected">The source snapshot captured before recovery setup.</param>
    /// <param name="actual">The source snapshot captured after fresh reopen.</param>
    private static void AssertArtifactsUnchanged(
        IReadOnlyDictionary<string, byte[]> expected,
        IReadOnlyDictionary<string, byte[]> actual)
    {
        actual.Keys.ShouldBe(expected.Keys, ignoreOrder: false);
        foreach (var artifact in expected)
        {
            actual[artifact.Key].ShouldBe(artifact.Value);
        }
    }

    /// <summary>Formats an optional engine failure for assertion diagnostics.</summary>
    /// <param name="error">The engine failure, or <see langword="null"/>.</param>
    /// <returns>The complete stable failure detail.</returns>
    private static string DescribeError(EngineError? error)
    {
        return error is null ? "Engine error: <none>." : $"Engine error: {error.Code}: {error.Message}";
    }

    /// <summary>Cancels one workspace adoption immediately after its terminal recovery evidence becomes durable.</summary>
    private sealed class CancelAfterValidationSaveCoordinator : IWorkspaceSaveCoordinator
    {
        /// <summary>The production coordinator that owns all save and recovery behavior.</summary>
        private readonly IWorkspaceSaveCoordinator Inner;

        /// <summary>The operation token canceled after the first successful evidence validation.</summary>
        private readonly CancellationTokenSource CancellationSource;

        /// <summary>Tracks whether the single deterministic cancellation remains armed.</summary>
        private int RemainingCancellations = 1;

        /// <summary>Initializes a delegating coordinator with one deterministic post-validation cancellation.</summary>
        /// <param name="inner">The production coordinator receiving every operation.</param>
        /// <param name="cancellationSource">The source whose token guards the first recovery adoption attempt.</param>
        /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
        public CancelAfterValidationSaveCoordinator(
            IWorkspaceSaveCoordinator inner,
            CancellationTokenSource cancellationSource)
        {
            ArgumentNullException.ThrowIfNull(inner);
            ArgumentNullException.ThrowIfNull(cancellationSource);
            Inner = inner;
            CancellationSource = cancellationSource;
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<OutputAdmissionResult>> InspectOutputAdmissionAsync(
            IOutputDirectoryLease lease,
            OutputAdmissionRequest request,
            CancellationToken cancellationToken = default)
        {
            return Inner.InspectOutputAdmissionAsync(lease, request, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<SaveResult> SaveAsync(
            WorkspaceSaveContext context,
            SaveRequest request,
            CancellationToken cancellationToken = default)
        {
            return Inner.SaveAsync(context, request, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<RecoverSaveResult> RecoverAsync(
            RecoverSaveRequest request,
            CancellationToken cancellationToken = default)
        {
            return Inner.RecoverAsync(request, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<RepairSaveResult> RepairAsync(
            RepairSaveRequest request,
            CancellationToken cancellationToken = default)
        {
            return Inner.RepairAsync(request, cancellationToken);
        }

        /// <inheritdoc />
        public async ValueTask<EngineResult<ResolvedOutputEvidence>> ValidateResolvedEvidenceAsync(
            IOutputDirectoryLease lease,
            ResolvedOutputEvidence evidence,
            CancellationToken cancellationToken = default)
        {
            var result = await Inner.ValidateResolvedEvidenceAsync(
                lease,
                evidence,
                cancellationToken);
            if (result.Succeeded
                && result.Value is not null
                && Interlocked.Exchange(ref RemainingCancellations, 0) == 1)
            {
                CancellationSource.Cancel();
            }

            return result;
        }
    }
}
