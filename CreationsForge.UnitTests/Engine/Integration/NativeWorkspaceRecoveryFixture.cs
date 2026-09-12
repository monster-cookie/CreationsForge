using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.Engine;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Engine.NativeOutputs;
using CreationsForge.Core.Engine.Persistence;
using CreationsForge.Core.Enums;
using CreationsForge.Fallout4.Native;
using CreationsForge.Fallout4.Native.Edits;
using CreationsForge.Skyrim.Native;
using CreationsForge.Starfield.Native;
using CreationsForge.Starfield.Native.Edits;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Serilog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Integration;

/// <summary>Creates real native recovery states shared by engine and physical stdio cross-owner tests.</summary>
internal static class NativeWorkspaceRecoveryFixture
{
    /// <summary>Creates one recognized native prior-save scenario and optionally pauses before the prior save or transaction mutation.</summary>
    /// <param name="fixture">The generated native source fixture.</param>
    /// <param name="output">The absent output association to create and later recover.</param>
    /// <param name="state">The journal and physical destination state to create.</param>
    /// <param name="beforeTransaction">An optional callback invoked after terminal-state setup has committed its original output, or after mixed-state owner A has staged a new output but before its first publication.</param>
    /// <param name="cancellationToken">The token checked before and during setup operations.</param>
    /// <returns>The complete recovery identities, baselines, expectations, and snapshot support.</returns>
    /// <exception cref="ArgumentException">Thrown when mixed state lacks separate string files or a terminal state is not embedded.</exception>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="state"/> is undefined.</exception>
    internal static async Task<NativeWorkspaceRecoverySeed> CreateAsync(
        NativeWorkspaceIntegrationFixture fixture,
        OutputAssociation output,
        NativeWorkspaceRecoveryPhysicalState state,
        Func<NativeWorkspaceRecoverySetup, CancellationToken, Task>? beforeTransaction,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        ArgumentNullException.ThrowIfNull(output);
        if (!Enum.IsDefined(state))
        {
            throw new ArgumentOutOfRangeException(nameof(state));
        }

        if (state == NativeWorkspaceRecoveryPhysicalState.MutationStartedMixed)
        {
            if (output.LocalizedOutputMode != LocalizedOutputMode.SeparateStringFiles)
            {
                throw new ArgumentException("Mixed recovery requires a multi-artifact separate-strings output.", nameof(output));
            }

            return await CreateMutationStartedMixedAsync(
                fixture,
                output,
                beforeTransaction,
                cancellationToken).ConfigureAwait(false);
        }

        if (output.LocalizedOutputMode != LocalizedOutputMode.Embedded)
        {
            throw new ArgumentException("Terminal recovery fixtures require an embedded output.", nameof(output));
        }

        cancellationToken.ThrowIfCancellationRequested();
        var beforeEditorId = $"CfBefore{fixture.Game}";
        var preparedEditorId = state == NativeWorkspaceRecoveryPhysicalState.PreparingAllBefore
            ? null
            : $"CfPrepared{fixture.Game}";
        var before = await CreateCommittedOutputAsync(
            fixture,
            output,
            beforeEditorId,
            "Before localized value",
            cancellationToken).ConfigureAwait(false);
        var setup = new NativeWorkspaceRecoverySetup(
            output,
            before.FormKey,
            before.Baseline,
            beforeEditorId,
            preparedEditorId);
        if (beforeTransaction is not null)
        {
            await beforeTransaction(setup, cancellationToken).ConfigureAwait(false);
        }

        return state switch
        {
            NativeWorkspaceRecoveryPhysicalState.PreparingAllBefore => await SeedPreparingAllBeforeAsync(
                fixture,
                setup,
                cancellationToken).ConfigureAwait(false),
            NativeWorkspaceRecoveryPhysicalState.PreparedAllPrepared => await SeedPreparedAllPreparedAsync(
                fixture,
                setup,
                cancellationToken).ConfigureAwait(false),
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, "The recovery fixture requires a supported physical state."),
        };
    }

    /// <summary>Creates a canonical full-master separate-strings output association for a mixed first-publication recovery fixture.</summary>
    /// <param name="fixture">The generated native source fixture.</param>
    /// <param name="directoryName">The fixture-relative output directory name.</param>
    /// <param name="fileName">The output plugin file name.</param>
    /// <returns>The complete absent output association.</returns>
    internal static OutputAssociation CreateOutputAssociation(
        NativeWorkspaceIntegrationFixture fixture,
        string directoryName,
        string fileName)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryName);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        var directory = fixture.RootDirectory.CreateSubdirectory(directoryName);
        return new OutputAssociation(
            Path.Combine(directory.FullName, fileName),
            ModKey.FromNameAndExtension(fileName),
            LocalizedOutputMode.SeparateStringFiles,
            OutputMasterStyle.Full);
    }

    /// <summary>Creates a canonical full-master embedded output association below the fixture root.</summary>
    /// <param name="fixture">The generated native source fixture.</param>
    /// <param name="directoryName">The fixture-relative output directory name.</param>
    /// <param name="fileName">The output plugin file name.</param>
    /// <returns>The complete absent embedded output association.</returns>
    internal static OutputAssociation CreateEmbeddedOutputAssociation(
        NativeWorkspaceIntegrationFixture fixture,
        string directoryName,
        string fileName)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryName);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        var directory = fixture.RootDirectory.CreateSubdirectory(directoryName);
        return new OutputAssociation(
            Path.Combine(directory.FullName, fileName),
            ModKey.FromNameAndExtension(fileName),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
    }

    /// <summary>Creates one real native factory around a caller-supplied save coordinator.</summary>
    /// <param name="game">The supported native game.</param>
    /// <param name="saveCoordinator">The coordinator used for admission, save, recovery, and repair.</param>
    /// <returns>A factory using the production native adapter for exactly one game.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="saveCoordinator"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="game"/> is undefined.</exception>
    internal static FormListWorkspaceFactory CreateWorkspaceFactory(
        SupportedGame game,
        IWorkspaceSaveCoordinator saveCoordinator)
    {
        ArgumentNullException.ThrowIfNull(saveCoordinator);
        var inputLoader = new NativeSourceInputLoader();
        var outputInputLoader = new NativeOutputInputLoader();
        IFormListGameAdapter adapter = game switch
        {
            SupportedGame.Starfield => CreateStarfieldAdapter(inputLoader, outputInputLoader),
            SupportedGame.Fallout4 => CreateFallout4Adapter(inputLoader, outputInputLoader),
            SupportedGame.Skyrim => CreateSkyrimAdapter(inputLoader, outputInputLoader),
            _ => throw new ArgumentOutOfRangeException(nameof(game), game, "The recovery fixture requires a supported native game."),
        };
        return new FormListWorkspaceFactory(
            [adapter],
            saveCoordinator,
            new OutputDirectoryLeaseProvider(),
            Log.Logger);
    }

    /// <summary>Creates a nonterminal Preparing journal while retaining the exact original destination.</summary>
    /// <param name="fixture">The generated native source fixture.</param>
    /// <param name="setup">The committed original output state.</param>
    /// <param name="cancellationToken">The token checked during durable setup.</param>
    /// <returns>The recognized all-before recovery seed.</returns>
    private static async Task<NativeWorkspaceRecoverySeed> SeedPreparingAllBeforeAsync(
        NativeWorkspaceIntegrationFixture fixture,
        NativeWorkspaceRecoverySetup setup,
        CancellationToken cancellationToken)
    {
        var identity = await InitializeJournalAsync(fixture, setup, cancellationToken).ConfigureAwait(false);
        return CreateSeed(
            NativeWorkspaceRecoveryPhysicalState.PreparingAllBefore,
            setup,
            identity.Paths,
            identity.Journal,
            preparedBaseline: null,
            RecoverSaveStatus.NotCommitted,
            expectedRecoveryErrorCode: null,
            originalSaveResult: null,
            originalSynchronization: null,
            originalPreview: null,
            originalWorkspace: null);
    }

    /// <summary>Creates a nonterminal Prepared journal and publishes its exact complete prepared destination without terminalizing it.</summary>
    /// <param name="fixture">The generated native source fixture.</param>
    /// <param name="setup">The committed original output state.</param>
    /// <param name="cancellationToken">The token checked during durable setup.</param>
    /// <returns>The recognized all-prepared recovery seed.</returns>
    private static async Task<NativeWorkspaceRecoverySeed> SeedPreparedAllPreparedAsync(
        NativeWorkspaceIntegrationFixture fixture,
        NativeWorkspaceRecoverySetup setup,
        CancellationToken cancellationToken)
    {
        var preparedOutput = CreatePreparedAssociation(fixture, setup.Output);
        var prepared = await CreateCommittedOutputAsync(
            fixture,
            preparedOutput,
            setup.PreparedEditorId.ShouldNotBeNull(),
            "Prepared localized value",
            cancellationToken).ConfigureAwait(false);
        prepared.FormKey.ShouldBe(setup.FormKey);
        var identity = await InitializeJournalAsync(fixture, setup, cancellationToken).ConfigureAwait(false);
        var plans = await CreatePreparedPlansAsync(
            identity.Paths,
            setup.BeforeBaseline,
            prepared.Baseline,
            cancellationToken).ConfigureAwait(false);
        var journal = identity.Journal.WithPreparedArtifacts(NativeWriteDisposition.StagedChanges, plans);
        var store = new SaveTransactionStore();
        await store.WriteAsync(identity.Paths, journal, cancellationToken).ConfigureAwait(false);
        PublishPreparedDestination(plans);
        var preparedBaseline = CreatePreparedBaseline(plans);
        return CreateSeed(
            NativeWorkspaceRecoveryPhysicalState.PreparedAllPrepared,
            setup,
            identity.Paths,
            journal,
            preparedBaseline,
            RecoverSaveStatus.Committed,
            expectedRecoveryErrorCode: null,
            originalSaveResult: null,
            originalSynchronization: null,
            originalPreview: null,
            originalWorkspace: null);
    }

    /// <summary>Runs a real first-publication native save that faults after its first content-changing destination mutation.</summary>
    /// <param name="fixture">The generated native source fixture.</param>
    /// <param name="output">The absent separate-strings output selected by the original owner.</param>
    /// <param name="beforeTransaction">An optional callback invoked after owner A stages its new output while the destination remains absent.</param>
    /// <param name="cancellationToken">The token checked before the first destination mutation.</param>
    /// <returns>The recognized genuinely mixed recovery seed.</returns>
    private static async Task<NativeWorkspaceRecoverySeed> CreateMutationStartedMixedAsync(
        NativeWorkspaceIntegrationFixture fixture,
        OutputAssociation output,
        Func<NativeWorkspaceRecoverySetup, CancellationToken, Task>? beforeTransaction,
        CancellationToken cancellationToken)
    {
        const string beforeEditorId = "<absent>";
        var preparedEditorId = $"CfPrepared{fixture.Game}";
        var beforeBaseline = await NativeSaveArtifactUtilities.CaptureOutputAsync(
            fixture.CreateOpenRequest().Release,
            output,
            cancellationToken).ConfigureAwait(false);
        beforeBaseline.Artifacts.All(artifact => !artifact.Fingerprint.Exists).ShouldBeTrue();
        var workspaceId = Guid.NewGuid();
        var saveOperationId = Guid.NewGuid();
        var leaseProvider = new OutputDirectoryLeaseProvider();
        var coordinator = new WorkspaceSaveCoordinator(
            leaseProvider,
            new SaveTransactionStore(),
            new ThrowAfterFirstMutationFileOperations());
        var factory = CreateWorkspaceFactory(fixture.Game, coordinator);
        var opened = await factory.OpenAsync(
            fixture.CreateOpenRequest(workspaceId),
            cancellationToken).ConfigureAwait(false);
        opened.Succeeded.ShouldBeTrue(DescribeError(opened.Error));
        var workspace = opened.Value.ShouldNotBeNull();
        try
        {
            var selected = await SelectOutputAsync(
                workspace,
                output,
                OutputSelectionMode.CreateNew,
                cancellationToken).ConfigureAwait(false);
            selected.Baseline.BaselineId.ShouldBe(beforeBaseline.BaselineId);
            var edit = await BeginEditAsync(
                workspace,
                FormListEditRole.New,
                targetFormKey: null,
                cancellationToken).ConfigureAwait(false);
            await ApplyEditAsync(
                workspace,
                edit.EditId,
                new SetEditorIdEdit(preparedEditorId),
                cancellationToken).ConfigureAwait(false);
        var localizedEdit = CreateLocalizedNameEdit(
            fixture.Game,
            "Prepared localized value",
            output.LocalizedOutputMode);
            if (localizedEdit is not null)
            {
                await ApplyEditAsync(workspace, edit.EditId, localizedEdit, cancellationToken).ConfigureAwait(false);
            }

            var previewResult = await workspace.PreviewAsync(cancellationToken).ConfigureAwait(false);
            previewResult.Succeeded.ShouldBeTrue(DescribeError(previewResult.Error));
            var preview = previewResult.Value.ShouldNotBeNull();
            var setup = new NativeWorkspaceRecoverySetup(
                output,
                edit.FormKey,
                beforeBaseline,
                beforeEditorId,
                preparedEditorId);
            if (beforeTransaction is not null)
            {
                await beforeTransaction(setup, cancellationToken).ConfigureAwait(false);
            }

            var saveBaseRevision = workspace.Revision;
            var save = await workspace.SaveAsync(
                new SaveRequest(saveOperationId, saveBaseRevision, selected.Baseline),
                cancellationToken).ConfigureAwait(false);
            save.Status.ShouldBe(SaveCommitStatus.CommitOutcomeUnknown);
            save.WorkspaceId.ShouldBe(workspaceId);
            save.OperationId.ShouldBe(saveOperationId);
            save.BaseRevision.ShouldBe(saveBaseRevision);
            save.RecoveryEvidenceToken.ShouldNotBeNull();
            workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.RecoveryRequired);
            workspace.OutputSynchronization.PendingSave.ShouldNotBeNull().OriginalWorkspaceId.ShouldBe(workspaceId);
            workspace.OutputSynchronization.PendingSave.SaveOperationId.ShouldBe(saveOperationId);
            var synchronization = workspace.OutputSynchronization;
            var paths = new SaveTransactionPaths(
                Path.GetDirectoryName(output.PluginPath)!,
                workspaceId,
                saveOperationId);
            var journal = await new SaveTransactionStore().ReadAsync(paths, cancellationToken).ConfigureAwait(false);
            journal.ShouldNotBeNull().Phase.ShouldBe(SaveTransactionPhase.MutationStarted);
            var preparedBaseline = CreatePreparedBaseline(journal.ArtifactPlans);
            CountContentChanges(beforeBaseline, preparedBaseline).ShouldBeGreaterThanOrEqualTo(2);
            var current = await NativeSaveArtifactUtilities.CaptureOutputAsync(
                fixture.CreateOpenRequest().Release,
                output,
                cancellationToken).ConfigureAwait(false);
            NativeSaveArtifactUtilities.MatchBaseline(beforeBaseline, current).ShouldBeFalse();
            NativeSaveArtifactUtilities.MatchBaseline(preparedBaseline, current).ShouldBeFalse();
            return CreateSeed(
                NativeWorkspaceRecoveryPhysicalState.MutationStartedMixed,
                setup,
                paths,
                journal,
                preparedBaseline,
                RecoverSaveStatus.StillUnknown,
                EngineErrorCode.RepairRequired,
                save,
                synchronization,
                preview,
                workspace);
        }
        catch
        {
            await workspace.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    /// <summary>Creates and durably initializes one canonical Preparing journal.</summary>
    /// <param name="fixture">The generated native source fixture.</param>
    /// <param name="setup">The committed original output state.</param>
    /// <param name="cancellationToken">The token checked during source inspection and journal initialization.</param>
    /// <returns>The initialized canonical paths and journal.</returns>
    private static async Task<(SaveTransactionPaths Paths, SaveTransactionJournal Journal)> InitializeJournalAsync(
        NativeWorkspaceIntegrationFixture fixture,
        NativeWorkspaceRecoverySetup setup,
        CancellationToken cancellationToken)
    {
        var workspaceId = Guid.NewGuid();
        var operationId = Guid.NewGuid();
        var paths = new SaveTransactionPaths(
            Path.GetDirectoryName(setup.Output.PluginPath)!,
            workspaceId,
            operationId);
        var sourceBaseline = await CaptureSourceBaselineAsync(
            fixture.CreateOpenRequest(workspaceId),
            cancellationToken).ConfigureAwait(false);
        var journal = new SaveTransactionJournal(
            workspaceId,
            operationId,
            new string('A', 64),
            new WorkspaceRevision(Guid.NewGuid(), 17),
            fixture.Game,
            fixture.CreateOpenRequest().Release,
            sourceBaseline,
            setup.Output,
            setup.BeforeBaseline,
            NativeWriteDisposition.StagedChanges,
            SaveTransactionPhase.Preparing,
            0,
            [],
            null,
            []);
        await new SaveTransactionStore().InitializeAsync(paths, journal, cancellationToken).ConfigureAwait(false);
        return (paths, journal);
    }

    /// <summary>Creates one valid committed output through production composition.</summary>
    /// <param name="fixture">The generated native source fixture.</param>
    /// <param name="output">The absent output association to create.</param>
    /// <param name="editorId">The deterministic EditorID to write.</param>
    /// <param name="localizedValue">The localized Name value used by games whose FormLists expose it.</param>
    /// <param name="cancellationToken">The token checked throughout the operation.</param>
    /// <returns>The stable FormList identity and complete committed baseline.</returns>
    private static async Task<(FormKey FormKey, OutputArtifactSetBaseline Baseline)> CreateCommittedOutputAsync(
        NativeWorkspaceIntegrationFixture fixture,
        OutputAssociation output,
        string editorId,
        string localizedValue,
        CancellationToken cancellationToken)
    {
        await using var services = NativeEngineComposition.Create();
        var opened = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(Guid.NewGuid()),
            cancellationToken).ConfigureAwait(false);
        opened.Succeeded.ShouldBeTrue(DescribeError(opened.Error));
        await using var workspace = opened.Value.ShouldNotBeNull();
        var selected = await SelectOutputAsync(
            workspace,
            output,
            OutputSelectionMode.CreateNew,
            cancellationToken).ConfigureAwait(false);
        var edit = await BeginEditAsync(
            workspace,
            FormListEditRole.New,
            targetFormKey: null,
            cancellationToken).ConfigureAwait(false);
        await ApplyEditAsync(
            workspace,
            edit.EditId,
            new SetEditorIdEdit(editorId),
            cancellationToken).ConfigureAwait(false);
        var localizedEdit = CreateLocalizedNameEdit(
            fixture.Game,
            localizedValue,
            output.LocalizedOutputMode);
        if (localizedEdit is not null)
        {
            await ApplyEditAsync(workspace, edit.EditId, localizedEdit, cancellationToken).ConfigureAwait(false);
        }

        var save = await workspace.SaveAsync(
            new SaveRequest(Guid.NewGuid(), workspace.Revision, selected.Baseline),
            cancellationToken).ConfigureAwait(false);
        save.Status.ShouldBe(SaveCommitStatus.Committed, DescribeError(save.Error));
        return (edit.FormKey, save.CommittedBaseline.ShouldNotBeNull());
    }

    /// <summary>Creates durable staged, publication, backup, and retirement plans for a prepared native set.</summary>
    /// <param name="paths">The canonical transaction paths.</param>
    /// <param name="beforeBaseline">The complete original destination baseline.</param>
    /// <param name="preparedSourceBaseline">The complete prepared output stored under a separate directory.</param>
    /// <param name="cancellationToken">The token checked while copying and inspecting artifacts.</param>
    /// <returns>The complete destination-relative prepared plans.</returns>
    private static async Task<IReadOnlyList<SaveArtifactPlan>> CreatePreparedPlansAsync(
        SaveTransactionPaths paths,
        OutputArtifactSetBaseline beforeBaseline,
        OutputArtifactSetBaseline preparedSourceBaseline,
        CancellationToken cancellationToken)
    {
        var store = new SaveTransactionStore();
        store.CreateEmptySubdirectory(paths.StagingDirectoryPath, "recovery fixture staging directory");
        store.CreateEmptySubdirectory(paths.PublishDirectoryPath, "recovery fixture publication directory");
        store.CreateEmptySubdirectory(paths.BackupDirectoryPath, "recovery fixture backup directory");
        store.CreateEmptySubdirectory(paths.RetiredDirectoryPath, "recovery fixture retirement directory");
        var preparedByRole = preparedSourceBaseline.Artifacts.ToDictionary(
            artifact => (artifact.Role, artifact.Language),
            artifact => artifact);
        var plans = new List<SaveArtifactPlan>(beforeBaseline.Artifacts.Count);
        for (var index = 0; index < beforeBaseline.Artifacts.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var before = beforeBaseline.Artifacts[index];
            var preparedSource = preparedByRole[(before.Role, before.Language)];
            var stagedPath = Path.Combine(paths.StagingDirectoryPath, $"{index:D4}-staged.bin");
            var publishPath = Path.Combine(paths.PublishDirectoryPath, $"{index:D4}-publish.bin");
            var backupPath = Path.Combine(paths.BackupDirectoryPath, $"{index:D4}-backup.bin");
            var retiredPath = Path.Combine(paths.RetiredDirectoryPath, $"{index:D4}-retired.bin");
            NativeArtifactAssociation staged;
            NativeArtifactAssociation? publish = null;
            NativeArtifactAssociation? backup = null;
            if (preparedSource.Fingerprint.Exists)
            {
                File.Copy(preparedSource.Path, stagedPath);
                File.Copy(preparedSource.Path, publishPath);
                staged = await InspectAsync(stagedPath, before, mustExist: true, cancellationToken).ConfigureAwait(false);
                publish = await InspectAsync(publishPath, before, mustExist: true, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                staged = await InspectAsync(stagedPath, before, mustExist: false, cancellationToken).ConfigureAwait(false);
            }

            if (before.Fingerprint.Exists)
            {
                File.Copy(before.Path, backupPath);
                backup = await InspectAsync(backupPath, before, mustExist: true, cancellationToken).ConfigureAwait(false);
            }

            plans.Add(new SaveArtifactPlan(before, staged, publish, backup, retiredPath));
        }

        return plans.AsReadOnly();
    }

    /// <summary>Publishes every prepared artifact into its destination while retaining a nonterminal journal.</summary>
    /// <param name="plans">The complete prepared publication plans.</param>
    private static void PublishPreparedDestination(IReadOnlyList<SaveArtifactPlan> plans)
    {
        foreach (var plan in plans)
        {
            if (ArtifactContentMatches(plan.Before, plan.Staged))
            {
                continue;
            }

            if (plan.Staged.Fingerprint.Exists)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(plan.Before.Path)!);
                File.Move(plan.Publish!.Path, plan.Before.Path, overwrite: true);
            }
            else if (plan.Before.Fingerprint.Exists)
            {
                File.Move(plan.Before.Path, plan.RetiredPath);
            }
        }
    }

    /// <summary>Creates a destination-path prepared baseline from durable artifact plans.</summary>
    /// <param name="plans">The complete prepared artifact plans.</param>
    /// <returns>The exact complete prepared destination baseline.</returns>
    private static OutputArtifactSetBaseline CreatePreparedBaseline(IReadOnlyList<SaveArtifactPlan> plans)
    {
        return NativeSaveArtifactUtilities.CreateOutputBaseline(plans.Select(plan =>
            ArtifactContentMatches(plan.Before, plan.Staged)
                ? plan.Before
                : new NativeArtifactAssociation(
                    plan.Before.Path,
                    plan.Before.Role,
                    plan.Before.Language,
                    plan.Staged.Fingerprint,
                    plan.Publish?.FileIdentity)).ToArray());
    }

    /// <summary>Creates an immutable recovery result from the durable journal identity and supplied expected state.</summary>
    /// <param name="state">The requested journal and destination state.</param>
    /// <param name="setup">The original committed output setup.</param>
    /// <param name="paths">The canonical transaction paths.</param>
    /// <param name="journal">The durable prior-save journal.</param>
    /// <param name="preparedBaseline">The intended complete prepared baseline, when one exists.</param>
    /// <param name="expectedRecoveryStatus">The expected recovery classification.</param>
    /// <param name="expectedRecoveryErrorCode">The expected recovery error, or <see langword="null"/>.</param>
    /// <param name="originalSaveResult">The real interrupted save result, or <see langword="null"/>.</param>
    /// <param name="originalSynchronization">The interrupted workspace synchronization, or <see langword="null"/>.</param>
    /// <param name="originalPreview">The detached staged preview, or <see langword="null"/>.</param>
    /// <param name="originalWorkspace">The live original workspace, or <see langword="null"/> for direct seeding.</param>
    /// <returns>The complete recovery seed.</returns>
    private static NativeWorkspaceRecoverySeed CreateSeed(
        NativeWorkspaceRecoveryPhysicalState state,
        NativeWorkspaceRecoverySetup setup,
        SaveTransactionPaths paths,
        SaveTransactionJournal journal,
        OutputArtifactSetBaseline? preparedBaseline,
        RecoverSaveStatus expectedRecoveryStatus,
        EngineErrorCode? expectedRecoveryErrorCode,
        SaveResult? originalSaveResult,
        OutputSynchronizationState? originalSynchronization,
        WorkspacePreview? originalPreview,
        IFormListWorkspace? originalWorkspace)
    {
        return new NativeWorkspaceRecoverySeed(
            state,
            journal.WorkspaceId,
            journal.SaveOperationId,
            journal.SaveBaseRevision,
            setup.Output,
            setup.BeforeBaseline,
            preparedBaseline,
            setup.FormKey,
            setup.BeforeEditorId,
            setup.PreparedEditorId,
            expectedRecoveryStatus,
            expectedRecoveryErrorCode,
            paths,
            originalSaveResult,
            originalSynchronization,
            originalPreview,
            originalWorkspace);
    }

    /// <summary>Selects a native output through the public workspace contract.</summary>
    /// <param name="workspace">The workspace receiving the output.</param>
    /// <param name="output">The complete output association.</param>
    /// <param name="mode">Whether the output must be absent or present.</param>
    /// <param name="cancellationToken">The token checked before state publication.</param>
    /// <returns>The successful selection receipt.</returns>
    private static async Task<OutputSelectionReceipt> SelectOutputAsync(
        IFormListWorkspace workspace,
        OutputAssociation output,
        OutputSelectionMode mode,
        CancellationToken cancellationToken)
    {
        var result = await workspace.SelectOutputAsync(
            new SelectOutputRequest(Guid.NewGuid(), workspace.Revision, mode, output),
            cancellationToken).ConfigureAwait(false);
        result.Succeeded.ShouldBeTrue(DescribeError(result.Error));
        return result.Value.ShouldNotBeNull();
    }

    /// <summary>Begins a native output edit through the public workspace contract.</summary>
    /// <param name="workspace">The selected workspace.</param>
    /// <param name="role">Whether a new or existing output FormList is edited.</param>
    /// <param name="targetFormKey">The existing output identity, or <see langword="null"/> for allocation.</param>
    /// <param name="cancellationToken">The token checked before edit publication.</param>
    /// <returns>The successful edit receipt.</returns>
    private static async Task<EditReceipt> BeginEditAsync(
        IFormListWorkspace workspace,
        FormListEditRole role,
        FormKey? targetFormKey,
        CancellationToken cancellationToken)
    {
        var result = await workspace.BeginEditAsync(
            new BeginEditRequest(Guid.NewGuid(), workspace.Revision, role, targetFormKey: targetFormKey),
            cancellationToken).ConfigureAwait(false);
        result.Succeeded.ShouldBeTrue(DescribeError(result.Error));
        return result.Value.ShouldNotBeNull();
    }

    /// <summary>Applies one typed edit through the public workspace contract.</summary>
    /// <param name="workspace">The selected workspace.</param>
    /// <param name="editId">The staged edit-session identity.</param>
    /// <param name="edit">The typed immutable edit command.</param>
    /// <param name="cancellationToken">The token checked before mutation publication.</param>
    /// <returns>A task that completes after the workspace revision advances.</returns>
    private static async Task ApplyEditAsync(
        IFormListWorkspace workspace,
        Guid editId,
        FormListEdit edit,
        CancellationToken cancellationToken)
    {
        var result = await workspace.ApplyFormListEditAsync(
            new FormListEditRequest(Guid.NewGuid(), workspace.Revision, editId, edit),
            cancellationToken).ConfigureAwait(false);
        result.Succeeded.ShouldBeTrue(DescribeError(result.Error));
        result.ResultRevision.ShouldBe(workspace.Revision);
    }

    /// <summary>Creates the supported localized Name edit used to force string-sidecar differences.</summary>
    /// <param name="game">The native game whose typed edit is required.</param>
    /// <param name="value">The deterministic localized value.</param>
    /// <param name="localizedOutputMode">The output format that determines whether nondefault translations are supported.</param>
    /// <returns>The game-specific edit, or <see langword="null"/> when Skyrim exposes no FormList Name.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="game"/> is undefined.</exception>
    private static FormListEdit? CreateLocalizedNameEdit(
        SupportedGame game,
        string value,
        LocalizedOutputMode localizedOutputMode)
    {
        var name = new TranslatedString(Language.English, value);
        if (localizedOutputMode == LocalizedOutputMode.SeparateStringFiles)
        {
            name.Set(Language.French, $"{value} French");
        }

        return game switch
        {
            SupportedGame.Starfield => new StarfieldSetNameEdit(name),
            SupportedGame.Fallout4 => new Fallout4SetNameEdit(name),
            SupportedGame.Skyrim => null,
            _ => throw new ArgumentOutOfRangeException(nameof(game), game, "The recovery fixture requires a supported native game."),
        };
    }

    /// <summary>Captures a complete source baseline through the same explicit production input loader.</summary>
    /// <param name="request">The complete explicit native open request.</param>
    /// <param name="cancellationToken">The token checked throughout source inspection.</param>
    /// <returns>The exact completed source baseline.</returns>
    private static async Task<NativeSourceInputBaseline> CaptureSourceBaselineAsync(
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken)
    {
        var prepared = await new NativeSourceInputLoader().PrepareAsync(request, cancellationToken).ConfigureAwait(false);
        prepared.Succeeded.ShouldBeTrue(DescribeError(prepared.Error));
        await using var inputs = prepared.Value.ShouldNotBeNull();
        var completed = await inputs.CompleteOpenAsync(cancellationToken).ConfigureAwait(false);
        completed.Succeeded.ShouldBeTrue(DescribeError(completed.Error));
        return completed.Value.ShouldNotBeNull();
    }

    /// <summary>Creates a prepared-output association with the same plugin identity under a separate fixture directory.</summary>
    /// <param name="fixture">The generated native source fixture.</param>
    /// <param name="output">The destination association whose format and plugin identity are retained.</param>
    /// <returns>The absent prepared-output association.</returns>
    private static OutputAssociation CreatePreparedAssociation(
        NativeWorkspaceIntegrationFixture fixture,
        OutputAssociation output)
    {
        var directory = fixture.RootDirectory.CreateSubdirectory($"RecoveryPrepared{Guid.NewGuid():N}");
        return new OutputAssociation(
            Path.Combine(directory.FullName, Path.GetFileName(output.PluginPath)),
            output.ModKey,
            output.LocalizedOutputMode,
            output.MasterStyle);
    }

    /// <summary>Inspects one transaction-owned file with the role and language of its destination.</summary>
    /// <param name="path">The transaction-owned file path.</param>
    /// <param name="destination">The corresponding destination artifact.</param>
    /// <param name="mustExist">Whether the transaction-owned file must exist.</param>
    /// <param name="cancellationToken">The token checked during file inspection.</param>
    /// <returns>The exact transaction-owned artifact observation.</returns>
    private static async Task<NativeArtifactAssociation> InspectAsync(
        string path,
        NativeArtifactAssociation destination,
        bool mustExist,
        CancellationToken cancellationToken)
    {
        return await NativeFileInspector.InspectAsync(
            path,
            destination.Role,
            destination.Language,
            mustExist,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Counts output artifacts whose existence or content differs between two complete baselines.</summary>
    /// <param name="before">The original complete baseline.</param>
    /// <param name="prepared">The intended complete baseline.</param>
    /// <returns>The number of content-changing artifacts.</returns>
    private static int CountContentChanges(
        OutputArtifactSetBaseline before,
        OutputArtifactSetBaseline prepared)
    {
        var comparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        var preparedByPath = prepared.Artifacts.ToDictionary(artifact => artifact.Path, comparer);
        return before.Artifacts.Count(artifact =>
            !artifact.Fingerprint.Equals(preparedByPath[artifact.Path].Fingerprint));
    }

    /// <summary>Determines whether two artifact observations carry the same existence and content.</summary>
    /// <param name="left">The first artifact observation.</param>
    /// <param name="right">The second artifact observation.</param>
    /// <returns><see langword="true"/> when the fingerprints are content-equivalent.</returns>
    private static bool ArtifactContentMatches(
        NativeArtifactAssociation left,
        NativeArtifactAssociation right)
    {
        return left.Fingerprint.Equals(right.Fingerprint);
    }

    /// <summary>Creates the real Starfield native adapter.</summary>
    /// <param name="sourceInputLoader">The shared explicit source-input loader.</param>
    /// <param name="outputInputLoader">The shared output-input loader.</param>
    /// <returns>The complete Starfield adapter.</returns>
    private static IFormListGameAdapter CreateStarfieldAdapter(
        NativeSourceInputLoader sourceInputLoader,
        NativeOutputInputLoader outputInputLoader)
    {
        var outputService = new StarfieldNativeOutputService(outputInputLoader);
        return new StarfieldFormListGameAdapter(
            new StarfieldNativeSourceLoader(sourceInputLoader),
            outputService,
            new StarfieldNativeEditService(),
            new StarfieldNativeWriter(outputService));
    }

    /// <summary>Creates the real Fallout 4 native adapter.</summary>
    /// <param name="sourceInputLoader">The shared explicit source-input loader.</param>
    /// <param name="outputInputLoader">The shared output-input loader.</param>
    /// <returns>The complete Fallout 4 adapter.</returns>
    private static IFormListGameAdapter CreateFallout4Adapter(
        NativeSourceInputLoader sourceInputLoader,
        NativeOutputInputLoader outputInputLoader)
    {
        var outputService = new Fallout4NativeOutputService(outputInputLoader);
        return new Fallout4FormListGameAdapter(
            new Fallout4NativeSourceLoader(sourceInputLoader),
            outputService,
            new Fallout4NativeEditService(outputService.Inspector),
            new Fallout4NativeWriteService(outputService));
    }

    /// <summary>Creates the real Skyrim native adapter.</summary>
    /// <param name="sourceInputLoader">The shared explicit source-input loader.</param>
    /// <param name="outputInputLoader">The shared output-input loader.</param>
    /// <returns>The complete Skyrim adapter.</returns>
    private static IFormListGameAdapter CreateSkyrimAdapter(
        NativeSourceInputLoader sourceInputLoader,
        NativeOutputInputLoader outputInputLoader)
    {
        var outputService = new SkyrimNativeOutputService(outputInputLoader);
        return new SkyrimFormListGameAdapter(
            new SkyrimNativeSourceLoader(sourceInputLoader),
            outputService,
            new SkyrimNativeEditService(outputService.Inspector));
    }

    /// <summary>Formats an optional engine failure for assertion diagnostics.</summary>
    /// <param name="error">The engine failure, or <see langword="null"/>.</param>
    /// <returns>The complete stable failure detail.</returns>
    private static string DescribeError(EngineError? error)
    {
        return error is null ? "Engine error: <none>." : $"Engine error: {error.Code}: {error.Message}";
    }

    /// <summary>Raises one deterministic exception after the first content-changing destination mutation.</summary>
    private sealed class ThrowAfterFirstMutationFileOperations : SaveTransactionFileOperations
    {
        /// <summary>Tracks whether the one-shot interruption has fired.</summary>
        private int _remainingInterruptions = 1;

        /// <inheritdoc />
        internal override void AfterDestinationMutation(Guid saveOperationId, int artifactIndex)
        {
            if (Interlocked.Exchange(ref _remainingInterruptions, 0) == 1)
            {
                throw new InvalidOperationException("Injected interruption after the first native destination mutation.");
            }
        }
    }
}
