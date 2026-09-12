using System.Security.Cryptography;
using System.Text.Json;
using CreationsForge.Bootstrap.Composition;
using CreationsForge.Console.Mcp;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.Persistence;
using CreationsForge.Core.Enums;
using CreationsForge.TestSupport;
using CreationsForge.UnitTests.Engine.Integration;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Shouldly;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>Verifies physical stdio recovery, repair, adoption, and stale-evidence behavior against real native artifacts.</summary>
public sealed class McpNativeCrossOwnerStdioTests
{
    /// <summary>Verifies an all-before Preparing journal remains byte-exact through recovery and can be adopted by a fresh physical host workspace.</summary>
    /// <returns>A task that completes after terminal recovery, adoption, native inspection, and normal child exit.</returns>
    [Fact(Timeout = 90_000)]
    public async Task ProductionStdioRecovery_PreparingAllBefore_AdoptsOriginalOutput()
    {
        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        timeoutSource.CancelAfter(TimeSpan.FromSeconds(75));
        var cancellationToken = timeoutSource.Token;
        using var fixture = NativeWorkspaceIntegrationFixture.Create(SupportedGame.Starfield);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var output = NativeWorkspaceRecoveryFixture.CreateEmbeddedOutputAssociation(
            fixture,
            "McpRecoveryAllBefore",
            "McpRecoveryAllBefore.esm");
        await using var seed = await NativeWorkspaceRecoveryFixture.CreateAsync(
            fixture,
            output,
            NativeWorkspaceRecoveryPhysicalState.PreparingAllBefore,
            beforeTransaction: null,
            cancellationToken);
        var physicalBeforeRecovery = seed.SnapshotPhysicalArtifacts();

        await using var process = await McpStdioProcessFixture.StartAsync(
            typeof(McpHostRunner).Assembly.Location,
            cancellationToken);
        var live = await OpenRecoveryRequiredWorkspaceAsync(process.Client, fixture, seed, cancellationToken);
        var recovery = await RecoverAsync(process.Client, seed, cancellationToken);
        AssertTerminalRecovery(seed, recovery, "not_committed", seed.BeforeBaseline.BaselineId);
        AssertPhysicalArtifactsEqual(physicalBeforeRecovery, seed.SnapshotPhysicalArtifacts());

        var adoptionOperationId = Guid.NewGuid();
        var adoption = await ResolveRecoveryAsync(
            process.Client,
            live.WorkspaceId,
            live.Revision,
            adoptionOperationId,
            recovery.GetProperty("resolvedEvidence").GetProperty("handle").GetString().ShouldNotBeNull(),
            cancellationToken);
        AssertAdoption(adoption, live.WorkspaceId, adoptionOperationId, seed.BeforeBaseline.BaselineId);
        await AssertEditorIdAsync(
            process.Client,
            live.WorkspaceId,
            seed.Output,
            seed.FormKey,
            seed.BeforeEditorId,
            cancellationToken);

        var terminalRecovery = await RecoverAsync(process.Client, seed, cancellationToken);
        AssertTerminalRecovery(seed, terminalRecovery, "not_committed", seed.BeforeBaseline.BaselineId);
        await CloseWorkspaceAsync(process.Client, live.WorkspaceId, cancellationToken);
        AssertSourceArtifactsEqual(sourceArtifacts, fixture.SnapshotArtifacts());
        await AssertNormalExitAsync(process, cancellationToken);
    }

    /// <summary>Verifies another real engine owner can invalidate previously observed committed evidence without allowing stale stdio adoption to overwrite its save.</summary>
    /// <returns>A task that completes after stale evidence is rejected and the independent owner's native output is reopened.</returns>
    [Fact(Timeout = 90_000)]
    public async Task ProductionStdioRecovery_PreparedAllPrepared_RejectsEvidenceAfterForeignSave()
    {
        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        timeoutSource.CancelAfter(TimeSpan.FromSeconds(75));
        var cancellationToken = timeoutSource.Token;
        using var fixture = NativeWorkspaceIntegrationFixture.Create(SupportedGame.Starfield);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var output = NativeWorkspaceRecoveryFixture.CreateEmbeddedOutputAssociation(
            fixture,
            "McpRecoveryAllPrepared",
            "McpRecoveryAllPrepared.esm");
        await using var seed = await NativeWorkspaceRecoveryFixture.CreateAsync(
            fixture,
            output,
            NativeWorkspaceRecoveryPhysicalState.PreparedAllPrepared,
            beforeTransaction: null,
            cancellationToken);
        var physicalBeforeRecovery = seed.SnapshotPhysicalArtifacts();

        await using var process = await McpStdioProcessFixture.StartAsync(
            typeof(McpHostRunner).Assembly.Location,
            cancellationToken);
        var stale = await OpenRecoveryRequiredWorkspaceAsync(process.Client, fixture, seed, cancellationToken);
        var recovery = await RecoverAsync(process.Client, seed, cancellationToken);
        AssertTerminalRecovery(
            seed,
            recovery,
            "committed",
            seed.PreparedBaseline.ShouldNotBeNull().BaselineId);
        AssertPhysicalArtifactsEqual(physicalBeforeRecovery, seed.SnapshotPhysicalArtifacts());
        var evidenceHandle = recovery.GetProperty("resolvedEvidence").GetProperty("handle").GetString().ShouldNotBeNull();

        const string foreignEditorId = "CfForeignAfterEvidence";
        await CommitForeignChangeAsync(fixture, seed, foreignEditorId, cancellationToken);
        var physicalAfterForeignSave = seed.SnapshotPhysicalArtifacts();
        var staleAdoption = await process.Client.CallToolAsync(
            "creationsforge_output_recovery_resolve",
            ResolveArguments(stale.WorkspaceId, Guid.NewGuid(), stale.Revision, evidenceHandle),
            cancellationToken: cancellationToken);
        GetErrorCode(staleAdoption).ShouldBe("external_change_detected");
        AssertPhysicalArtifactsEqual(physicalAfterForeignSave, seed.SnapshotPhysicalArtifacts());
        await CloseWorkspaceAsync(process.Client, stale.WorkspaceId, cancellationToken);

        var fresh = await OpenReadyWorkspaceAsync(process.Client, fixture, seed.Output, cancellationToken);
        await AssertEditorIdAsync(
            process.Client,
            fresh.WorkspaceId,
            seed.Output,
            seed.FormKey,
            foreignEditorId,
            cancellationToken);
        await CloseWorkspaceAsync(process.Client, fresh.WorkspaceId, cancellationToken);
        AssertSourceArtifactsEqual(sourceArtifacts, fixture.SnapshotArtifacts());
        await AssertNormalExitAsync(process, cancellationToken);
    }

    /// <summary>Verifies a genuinely mixed native transaction remains unchanged through physical recovery, then supports explicit repair, reinspection, and adoption.</summary>
    /// <returns>A task that completes after repair publishes the prepared set and the fresh host inspects it natively.</returns>
    [Fact(Timeout = 90_000)]
    public async Task ProductionStdioRecovery_MutationStartedMixed_RepairsAndAdoptsPreparedOutput()
    {
        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        timeoutSource.CancelAfter(TimeSpan.FromSeconds(75));
        var cancellationToken = timeoutSource.Token;
        using var fixture = NativeWorkspaceIntegrationFixture.Create(SupportedGame.Starfield);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var output = NativeWorkspaceRecoveryFixture.CreateOutputAssociation(
            fixture,
            "McpRecoveryMixed",
            "McpRecoveryMixed.esm");
        await using var process = await McpStdioProcessFixture.StartAsync(
            typeof(McpHostRunner).Assembly.Location,
            cancellationToken);
        var readyWorkspaceId = Guid.Empty;
        var readyRevision = default(WorkspaceRevision);
        string? readyBaselineHandle = null;
        const string readyEditorId = "CfMixedOwnerB";
        await using var seed = await NativeWorkspaceRecoveryFixture.CreateAsync(
            fixture,
            output,
            NativeWorkspaceRecoveryPhysicalState.MutationStartedMixed,
            async (setup, token) =>
            {
                var ready = await OpenStagedNewOutputOwnerAsync(
                    process.Client,
                    fixture,
                    setup,
                    readyEditorId,
                    token);
                readyWorkspaceId = ready.WorkspaceId;
                readyRevision = ready.Revision;
                readyBaselineHandle = ready.BaselineHandle;
            },
            cancellationToken);
        readyWorkspaceId.ShouldNotBe(Guid.Empty);
        readyRevision.BaselineId.ShouldNotBe(Guid.Empty);
        readyBaselineHandle.ShouldNotBeNullOrWhiteSpace();
        var originalWorkspace = seed.OriginalWorkspace.ShouldNotBeNull();
        var originalSave = seed.OriginalSaveResult.ShouldNotBeNull();
        originalSave.Status.ShouldBe(SaveCommitStatus.CommitOutcomeUnknown);
        originalSave.WorkspaceId.ShouldBe(seed.OriginalWorkspaceId);
        originalSave.OperationId.ShouldBe(seed.SaveOperationId);
        originalSave.BaseRevision.ShouldBe(seed.SaveBaseRevision);
        var originalSynchronization = seed.OriginalSynchronization.ShouldNotBeNull();
        originalSynchronization.Status.ShouldBe(OutputSynchronizationStatus.RecoveryRequired);
        originalSynchronization.PendingSave.ShouldNotBeNull().OriginalWorkspaceId.ShouldBe(seed.OriginalWorkspaceId);
        originalSynchronization.PendingSave.SaveOperationId.ShouldBe(seed.SaveOperationId);
        originalWorkspace.WorkspaceId.ShouldBe(seed.OriginalWorkspaceId);
        originalWorkspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.RecoveryRequired);
        originalWorkspace.OutputSynchronization.PendingSave.ShouldNotBeNull().SaveOperationId.ShouldBe(seed.SaveOperationId);
        AssertPreviewEditorId(
            seed.OriginalPreview.ShouldNotBeNull(),
            seed.FormKey,
            seed.PreparedEditorId.ShouldNotBeNull());
        var physicalBeforeRecovery = seed.SnapshotPhysicalArtifacts();

        var recovery = await RecoverAsync(process.Client, seed, cancellationToken);
        AssertUnknownRecovery(seed, recovery);
        AssertPhysicalArtifactsEqual(physicalBeforeRecovery, seed.SnapshotPhysicalArtifacts());
        var evidenceToken = recovery.GetProperty("evidenceToken").GetString().ShouldNotBeNull();

        var blockedSaveOperationId = Guid.NewGuid();
        var blockedSave = GetResult(await process.Client.CallToolAsync(
            "creationsforge_save",
            SaveArguments(
                readyWorkspaceId,
                blockedSaveOperationId,
                readyRevision,
                readyBaselineHandle.ShouldNotBeNull()),
            cancellationToken: cancellationToken));
        AssertRepairRequiredSave(blockedSave, readyWorkspaceId, blockedSaveOperationId, readyRevision);
        await AssertReadyStagedOwnerAsync(
            process.Client,
            readyWorkspaceId,
            readyRevision,
            seed.BeforeBaseline,
            seed.Output,
            seed.FormKey,
            readyEditorId,
            cancellationToken);
        AssertPhysicalArtifactsEqual(physicalBeforeRecovery, seed.SnapshotPhysicalArtifacts());
        Directory.Exists(new SaveTransactionPaths(
            Path.GetDirectoryName(seed.Output.PluginPath)!,
            readyWorkspaceId,
            blockedSaveOperationId).TransactionDirectoryPath).ShouldBeFalse();
        originalWorkspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.RecoveryRequired);
        originalWorkspace.OutputSynchronization.PendingSave.ShouldNotBeNull().SaveOperationId.ShouldBe(seed.SaveOperationId);
        AssertPreviewEditorId(
            seed.OriginalPreview.ShouldNotBeNull(),
            seed.FormKey,
            seed.PreparedEditorId.ShouldNotBeNull());

        var recoveryAfterBlockedSave = await RecoverAsync(process.Client, seed, cancellationToken);
        AssertUnknownRecovery(seed, recoveryAfterBlockedSave);
        recoveryAfterBlockedSave.GetProperty("evidenceToken").GetString().ShouldBe(evidenceToken);
        await CloseWorkspaceAsync(process.Client, readyWorkspaceId, cancellationToken);
        var live = await OpenRecoveryRequiredWorkspaceAsync(process.Client, fixture, seed, cancellationToken);

        var repairOperationId = Guid.NewGuid();
        var repair = GetResult(await process.Client.CallToolAsync(
            "creationsforge_save_repair",
            RepairArguments(
                seed,
                repairOperationId,
                recoveryAfterBlockedSave.GetProperty("evidenceToken").GetString().ShouldNotBeNull()),
            cancellationToken: cancellationToken));
        var repairedBaselineId = AssertPreparedRepair(seed, repair, repairOperationId);
        AssertPreparedArtifactContent(seed);

        var terminalRecovery = await RecoverAsync(process.Client, seed, cancellationToken);
        AssertTerminalRecovery(
            seed,
            terminalRecovery,
            "committed",
            repairedBaselineId);
        var adoptionOperationId = Guid.NewGuid();
        var adoption = await ResolveRecoveryAsync(
            process.Client,
            live.WorkspaceId,
            live.Revision,
            adoptionOperationId,
            terminalRecovery.GetProperty("resolvedEvidence").GetProperty("handle").GetString().ShouldNotBeNull(),
            cancellationToken);
        AssertAdoption(adoption, live.WorkspaceId, adoptionOperationId, repairedBaselineId);
        await AssertEditorIdAsync(
            process.Client,
            live.WorkspaceId,
            seed.Output,
            seed.FormKey,
            seed.PreparedEditorId.ShouldNotBeNull(),
            cancellationToken);
        AssertPreparedArtifactContent(seed);
        await CloseWorkspaceAsync(process.Client, live.WorkspaceId, cancellationToken);
        AssertSourceArtifactsEqual(sourceArtifacts, fixture.SnapshotArtifacts());
        await AssertNormalExitAsync(process, cancellationToken);
    }

    /// <summary>Opens the physical second owner before the first publication and stages its independent new FormList.</summary>
    /// <param name="client">The initialized physical stdio client.</param>
    /// <param name="fixture">The generated native source fixture.</param>
    /// <param name="setup">The original owner's staged state immediately before its first save.</param>
    /// <param name="editorId">The independent second owner's staged EditorID.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>The second owner's identity, final staged revision, and exact baseline handle.</returns>
    private static async Task<(Guid WorkspaceId, WorkspaceRevision Revision, string BaselineHandle)> OpenStagedNewOutputOwnerAsync(
        McpClient client,
        NativeWorkspaceIntegrationFixture fixture,
        NativeWorkspaceRecoverySetup setup,
        string editorId,
        CancellationToken cancellationToken)
    {
        var workspaceId = Guid.NewGuid();
        var open = GetResult(await client.CallToolAsync(
            "creationsforge_workspace_open",
            OpenArguments(fixture.CreateOpenRequest(workspaceId)),
            cancellationToken: cancellationToken));
        var revision = ReadRevision(open.GetProperty("revision"));
        await AssertRegistryAsync(client, workspaceId, cancellationToken);

        var selectionOperationId = Guid.NewGuid();
        var selection = GetResult(await client.CallToolAsync(
            "creationsforge_output_select",
            SelectOutputArguments(workspaceId, selectionOperationId, revision, "create_new", setup.Output),
            cancellationToken: cancellationToken));
        selection.GetProperty("workspaceId").GetString().ShouldBe(workspaceId.ToString("D"));
        selection.GetProperty("operationId").GetString().ShouldBe(selectionOperationId.ToString("D"));
        selection.GetProperty("outputReference").GetProperty("kind").GetString().ShouldBe("output_association");
        var baselineReference = selection.GetProperty("baselineReference");
        baselineReference.GetProperty("kind").GetString().ShouldBe("output_baseline");
        var baselineHandle = baselineReference.GetProperty("handle").GetString().ShouldNotBeNull();
        selection.GetProperty("detailsUnavailable").GetBoolean().ShouldBeFalse();
        selection.GetProperty("warnings").GetArrayLength().ShouldBe(0);
        revision = ReadRevision(selection.GetProperty("revision"));
        (await ReadMetadataStringAsync(client, baselineHandle, "/baselineId", cancellationToken))
            .ShouldBe(setup.BeforeBaseline.BaselineId.ToString("D"));

        var beginOperationId = Guid.NewGuid();
        var begin = GetResult(await client.CallToolAsync(
            "creationsforge_formlist_begin_edit",
            BeginNewEditArguments(workspaceId, beginOperationId, revision),
            cancellationToken: cancellationToken));
        begin.GetProperty("workspaceId").GetString().ShouldBe(workspaceId.ToString("D"));
        begin.GetProperty("operationId").GetString().ShouldBe(beginOperationId.ToString("D"));
        begin.GetProperty("formKey").GetString().ShouldBe(setup.FormKey.ToString());
        begin.GetProperty("originFormKey").ValueKind.ShouldBe(JsonValueKind.Null);
        begin.GetProperty("role").GetString().ShouldBe("new");
        begin.GetProperty("warnings").GetArrayLength().ShouldBe(0);
        var editId = Guid.Parse(begin.GetProperty("editId").GetString().ShouldNotBeNull());
        revision = ReadRevision(begin.GetProperty("revision"));

        var applyOperationId = Guid.NewGuid();
        var applied = GetResult(await client.CallToolAsync(
            "creationsforge_formlist_apply_edit",
            ApplyEditorIdArguments(workspaceId, applyOperationId, revision, editId, editorId),
            cancellationToken: cancellationToken));
        applied.GetProperty("workspaceId").GetString().ShouldBe(workspaceId.ToString("D"));
        applied.GetProperty("operationId").GetString().ShouldBe(applyOperationId.ToString("D"));
        applied.GetProperty("editId").GetString().ShouldBe(editId.ToString("D"));
        applied.GetProperty("commandName").GetString().ShouldBe("form-list.set-editor-id");
        applied.GetProperty("warnings").GetArrayLength().ShouldBe(0);
        revision = ReadRevision(applied.GetProperty("revision"));

        await AssertReadyStagedOwnerAsync(
            client,
            workspaceId,
            revision,
            setup.BeforeBaseline,
            setup.Output,
            setup.FormKey,
            editorId,
            cancellationToken);
        return (workspaceId, revision, baselineHandle);
    }

    /// <summary>Requires an independently staged physical owner to retain its exact ready state.</summary>
    /// <param name="client">The initialized physical stdio client.</param>
    /// <param name="workspaceId">The second owner's workspace identity.</param>
    /// <param name="revision">The exact staged revision.</param>
    /// <param name="baseline">The absent destination baseline selected before first publication.</param>
    /// <param name="output">The selected output association.</param>
    /// <param name="formKey">The deterministic new FormList identity.</param>
    /// <param name="editorId">The second owner's staged EditorID.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>A task that completes after state, baseline metadata, and staged native inspection are checked.</returns>
    private static async Task AssertReadyStagedOwnerAsync(
        McpClient client,
        Guid workspaceId,
        WorkspaceRevision revision,
        OutputArtifactSetBaseline baseline,
        OutputAssociation output,
        Mutagen.Bethesda.Plugins.FormKey formKey,
        string editorId,
        CancellationToken cancellationToken)
    {
        var state = GetResult(await client.CallToolAsync(
            "creationsforge_workspace_state",
            WorkspaceArguments(workspaceId),
            cancellationToken: cancellationToken));
        ReadRevision(state.GetProperty("revision")).ShouldBe(revision);
        state.GetProperty("output").GetProperty("kind").GetString().ShouldBe("output_association");
        var baselineReference = state.GetProperty("outputBaseline");
        baselineReference.GetProperty("kind").GetString().ShouldBe("output_baseline");
        var baselineHandle = baselineReference.GetProperty("handle").GetString().ShouldNotBeNull();
        (await ReadMetadataStringAsync(client, baselineHandle, "/baselineId", cancellationToken))
            .ShouldBe(baseline.BaselineId.ToString("D"));
        var synchronization = state.GetProperty("outputSynchronization");
        synchronization.GetProperty("status").GetString().ShouldBe("ready");
        synchronization.GetProperty("pendingSave").ValueKind.ShouldBe(JsonValueKind.Null);
        await AssertEditorIdAsync(client, workspaceId, output, formKey, editorId, cancellationToken);
    }

    /// <summary>Checks the exact definitive repair-required save receipt for the already-open second owner.</summary>
    /// <param name="save">The structured save receipt.</param>
    /// <param name="workspaceId">The second owner's workspace identity.</param>
    /// <param name="operationId">The rejected save operation identity.</param>
    /// <param name="revision">The exact unchanged staged revision.</param>
    private static void AssertRepairRequiredSave(
        JsonElement save,
        Guid workspaceId,
        Guid operationId,
        WorkspaceRevision revision)
    {
        save.GetProperty("workspaceId").GetString().ShouldBe(workspaceId.ToString("D"));
        save.GetProperty("operationId").GetString().ShouldBe(operationId.ToString("D"));
        ReadRevision(save.GetProperty("baseRevision")).ShouldBe(revision);
        ReadRevision(save.GetProperty("resultRevision")).ShouldBe(revision);
        save.GetProperty("status").GetString().ShouldBe("not_committed");
        save.GetProperty("committedBaseline").ValueKind.ShouldBe(JsonValueKind.Null);
        save.GetProperty("recoveryEvidenceToken").ValueKind.ShouldBe(JsonValueKind.Null);
        save.GetProperty("output").ValueKind.ShouldBe(JsonValueKind.Null);
        save.GetProperty("resolvedEvidence").ValueKind.ShouldBe(JsonValueKind.Null);
        save.GetProperty("errorCode").GetString().ShouldBe("repair_required");
        save.GetProperty("warningCount").GetInt32().ShouldBe(0);
        save.GetProperty("details").GetProperty("kind").GetString().ShouldBe("save_result");
        save.GetProperty("detailsUnavailable").GetBoolean().ShouldBeFalse();
    }

    /// <summary>Checks a detached direct-engine preview retains the exact staged EditorID.</summary>
    /// <param name="preview">The original owner's detached preview captured before its interrupted save.</param>
    /// <param name="formKey">The original owner's new FormList identity.</param>
    /// <param name="editorId">The expected staged EditorID.</param>
    private static void AssertPreviewEditorId(
        WorkspacePreview preview,
        Mutagen.Bethesda.Plugins.FormKey formKey,
        string editorId)
    {
        var comparison = preview.Comparisons.Single(candidate => candidate.FormKey == formKey);
        comparison.After.ShouldNotBeNull().GetProperty("EditorID").GetString().ShouldBe(editorId);
    }

    /// <summary>Opens a physical-host workspace and latches the seeded prior transaction through failed output selection.</summary>
    /// <param name="client">The initialized physical stdio client.</param>
    /// <param name="fixture">The generated native source fixture.</param>
    /// <param name="seed">The seeded recognized transaction.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>The live workspace identity and unchanged open revision.</returns>
    private static async Task<(Guid WorkspaceId, WorkspaceRevision Revision)> OpenRecoveryRequiredWorkspaceAsync(
        McpClient client,
        NativeWorkspaceIntegrationFixture fixture,
        NativeWorkspaceRecoverySeed seed,
        CancellationToken cancellationToken)
    {
        var workspaceId = Guid.NewGuid();
        var open = GetResult(await client.CallToolAsync(
            "creationsforge_workspace_open",
            OpenArguments(fixture.CreateOpenRequest(workspaceId)),
            cancellationToken: cancellationToken));
        var revision = ReadRevision(open.GetProperty("revision"));
        await AssertRegistryAsync(client, workspaceId, cancellationToken);
        var selection = await client.CallToolAsync(
            "creationsforge_output_select",
            SelectOutputArguments(workspaceId, Guid.NewGuid(), revision, "open_existing", seed.Output),
            cancellationToken: cancellationToken);
        GetErrorCode(selection).ShouldBe("repair_required");

        var state = GetResult(await client.CallToolAsync(
            "creationsforge_workspace_state",
            WorkspaceArguments(workspaceId),
            cancellationToken: cancellationToken));
        ReadRevision(state.GetProperty("revision")).ShouldBe(revision);
        var synchronization = state.GetProperty("outputSynchronization");
        synchronization.GetProperty("status").GetString().ShouldBe("recovery_required");
        var pending = synchronization.GetProperty("pendingSave");
        pending.GetProperty("originalWorkspaceId").GetString().ShouldBe(seed.OriginalWorkspaceId.ToString("D"));
        pending.GetProperty("saveOperationId").GetString().ShouldBe(seed.SaveOperationId.ToString("D"));
        ReadRevision(pending.GetProperty("saveBaseRevision")).ShouldBe(seed.SaveBaseRevision);
        pending.GetProperty("output").GetProperty("kind").GetString().ShouldBe("output_association");
        return (workspaceId, revision);
    }

    /// <summary>Opens and selects an existing output after all prior recovery work is terminal.</summary>
    /// <param name="client">The initialized physical stdio client.</param>
    /// <param name="fixture">The generated native source fixture.</param>
    /// <param name="output">The existing output association.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>The selected live workspace identity and revision.</returns>
    private static async Task<(Guid WorkspaceId, WorkspaceRevision Revision)> OpenReadyWorkspaceAsync(
        McpClient client,
        NativeWorkspaceIntegrationFixture fixture,
        OutputAssociation output,
        CancellationToken cancellationToken)
    {
        var workspaceId = Guid.NewGuid();
        var open = GetResult(await client.CallToolAsync(
            "creationsforge_workspace_open",
            OpenArguments(fixture.CreateOpenRequest(workspaceId)),
            cancellationToken: cancellationToken));
        var revision = ReadRevision(open.GetProperty("revision"));
        await AssertRegistryAsync(client, workspaceId, cancellationToken);
        var selection = GetResult(await client.CallToolAsync(
            "creationsforge_output_select",
            SelectOutputArguments(workspaceId, Guid.NewGuid(), revision, "open_existing", output),
            cancellationToken: cancellationToken));
        return (workspaceId, ReadRevision(selection.GetProperty("revision")));
    }

    /// <summary>Calls read-only physical recovery with the exact persisted identities and inline output.</summary>
    /// <param name="client">The initialized physical stdio client.</param>
    /// <param name="seed">The seeded recognized transaction.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>The successful structured recovery receipt.</returns>
    private static async Task<JsonElement> RecoverAsync(
        McpClient client,
        NativeWorkspaceRecoverySeed seed,
        CancellationToken cancellationToken)
    {
        return GetResult(await client.CallToolAsync(
            "creationsforge_save_recover",
            RecoverArguments(seed),
            cancellationToken: cancellationToken));
    }

    /// <summary>Adopts one terminal recovery observation into the blocked physical-host workspace.</summary>
    /// <param name="client">The initialized physical stdio client.</param>
    /// <param name="workspaceId">The live workspace identity.</param>
    /// <param name="revision">The exact live revision.</param>
    /// <param name="operationId">The exact new adoption identity.</param>
    /// <param name="evidenceHandle">The host-local terminal evidence handle.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>The successful structured adoption receipt.</returns>
    private static async Task<JsonElement> ResolveRecoveryAsync(
        McpClient client,
        Guid workspaceId,
        WorkspaceRevision revision,
        Guid operationId,
        string evidenceHandle,
        CancellationToken cancellationToken)
    {
        return GetResult(await client.CallToolAsync(
            "creationsforge_output_recovery_resolve",
            ResolveArguments(workspaceId, operationId, revision, evidenceHandle),
            cancellationToken: cancellationToken));
    }

    /// <summary>Checks the exact structured fields for terminal recovery.</summary>
    /// <param name="seed">The seeded recognized transaction.</param>
    /// <param name="recovery">The structured recovery receipt.</param>
    /// <param name="expectedStatus">The expected terminal wire status.</param>
    /// <param name="expectedBaselineId">The terminal physical output-baseline identity.</param>
    private static void AssertTerminalRecovery(
        NativeWorkspaceRecoverySeed seed,
        JsonElement recovery,
        string expectedStatus,
        Guid expectedBaselineId)
    {
        recovery.GetProperty("originalWorkspaceId").GetString().ShouldBe(seed.OriginalWorkspaceId.ToString("D"));
        recovery.GetProperty("saveOperationId").GetString().ShouldBe(seed.SaveOperationId.ToString("D"));
        recovery.GetProperty("status").GetString().ShouldBe(expectedStatus);
        ReadRevision(recovery.GetProperty("saveBaseRevision")).ShouldBe(seed.SaveBaseRevision);
        recovery.GetProperty("repairRequired").GetBoolean().ShouldBeFalse();
        recovery.GetProperty("evidenceToken").GetString().ShouldNotBeNullOrWhiteSpace();
        recovery.GetProperty("output").GetProperty("kind").GetString().ShouldBe("output_association");
        recovery.GetProperty("resolvedBaseline").GetProperty("kind").GetString().ShouldBe("output_baseline");
        recovery.GetProperty("resolvedBaseline").GetProperty("baselineId").GetString().ShouldBe(expectedBaselineId.ToString("D"));
        recovery.GetProperty("resolvedEvidence").GetProperty("kind").GetString().ShouldBe("resolved_output_evidence");
        recovery.GetProperty("errorCode").ValueKind.ShouldBe(JsonValueKind.Null);
        recovery.GetProperty("details").GetProperty("kind").GetString().ShouldBe("recover_save_result");
        recovery.GetProperty("detailsUnavailable").GetBoolean().ShouldBeFalse();
    }

    /// <summary>Checks the exact structured fields for unresolved mixed recovery.</summary>
    /// <param name="seed">The seeded genuinely mixed transaction.</param>
    /// <param name="recovery">The structured recovery receipt.</param>
    private static void AssertUnknownRecovery(NativeWorkspaceRecoverySeed seed, JsonElement recovery)
    {
        recovery.GetProperty("originalWorkspaceId").GetString().ShouldBe(seed.OriginalWorkspaceId.ToString("D"));
        recovery.GetProperty("saveOperationId").GetString().ShouldBe(seed.SaveOperationId.ToString("D"));
        recovery.GetProperty("status").GetString().ShouldBe("still_unknown");
        ReadRevision(recovery.GetProperty("saveBaseRevision")).ShouldBe(seed.SaveBaseRevision);
        recovery.GetProperty("repairRequired").GetBoolean().ShouldBeTrue();
        recovery.GetProperty("evidenceToken").GetString().ShouldNotBeNullOrWhiteSpace();
        recovery.GetProperty("output").ValueKind.ShouldBe(JsonValueKind.Null);
        recovery.GetProperty("resolvedBaseline").ValueKind.ShouldBe(JsonValueKind.Null);
        recovery.GetProperty("resolvedEvidence").ValueKind.ShouldBe(JsonValueKind.Null);
        recovery.GetProperty("errorCode").GetString().ShouldBe("repair_required");
        recovery.GetProperty("details").GetProperty("kind").GetString().ShouldBe("recover_save_result");
        recovery.GetProperty("detailsUnavailable").GetBoolean().ShouldBeFalse();
    }

    /// <summary>Checks the exact structured fields for a completed prepared-set repair.</summary>
    /// <param name="seed">The seeded genuinely mixed transaction.</param>
    /// <param name="repair">The structured repair receipt.</param>
    /// <param name="repairOperationId">The exact repair idempotency identity.</param>
    /// <returns>The freshly captured physical output-baseline identity returned by repair.</returns>
    private static Guid AssertPreparedRepair(
        NativeWorkspaceRecoverySeed seed,
        JsonElement repair,
        Guid repairOperationId)
    {
        repair.GetProperty("originalWorkspaceId").GetString().ShouldBe(seed.OriginalWorkspaceId.ToString("D"));
        repair.GetProperty("saveOperationId").GetString().ShouldBe(seed.SaveOperationId.ToString("D"));
        repair.GetProperty("repairOperationId").GetString().ShouldBe(repairOperationId.ToString("D"));
        repair.GetProperty("status").GetString().ShouldBe("prepared_set_completed");
        var resultingBaseline = repair.GetProperty("resultingBaseline");
        resultingBaseline.GetProperty("kind").GetString().ShouldBe("output_baseline");
        var resultingBaselineId = Guid.Parse(resultingBaseline.GetProperty("baselineId").GetString().ShouldNotBeNull());
        resultingBaselineId.ShouldNotBe(Guid.Empty);
        repair.GetProperty("output").GetProperty("kind").GetString().ShouldBe("output_association");
        repair.GetProperty("resolvedEvidence").GetProperty("kind").GetString().ShouldBe("resolved_output_evidence");
        repair.GetProperty("evidenceToken").GetString().ShouldNotBeNullOrWhiteSpace();
        repair.GetProperty("errorCode").ValueKind.ShouldBe(JsonValueKind.Null);
        repair.GetProperty("details").GetProperty("kind").GetString().ShouldBe("repair_save_result");
        repair.GetProperty("detailsUnavailable").GetBoolean().ShouldBeFalse();
        return resultingBaselineId;
    }

    /// <summary>Checks a successful recovery-adoption receipt and its exact selected baseline.</summary>
    /// <param name="adoption">The structured adoption receipt.</param>
    /// <param name="workspaceId">The live workspace identity.</param>
    /// <param name="operationId">The exact adoption identity.</param>
    /// <param name="expectedBaselineId">The physical output-baseline identity adopted by the workspace.</param>
    private static void AssertAdoption(
        JsonElement adoption,
        Guid workspaceId,
        Guid operationId,
        Guid expectedBaselineId)
    {
        adoption.GetProperty("workspaceId").GetString().ShouldBe(workspaceId.ToString("D"));
        adoption.GetProperty("operationId").GetString().ShouldBe(operationId.ToString("D"));
        adoption.GetProperty("mode").GetString().ShouldBe("reopen_resolved_output");
        ReadRevision(adoption.GetProperty("revision")).Sequence.ShouldBeGreaterThan(0UL);
        adoption.GetProperty("output").GetProperty("kind").GetString().ShouldBe("output_association");
        adoption.GetProperty("outputBaseline").GetProperty("kind").GetString().ShouldBe("output_baseline");
        adoption.GetProperty("outputBaseline").GetProperty("baselineId").GetString().ShouldBe(expectedBaselineId.ToString("D"));
        adoption.GetProperty("warningCount").GetInt32().ShouldBe(0);
        adoption.GetProperty("detailsUnavailable").GetBoolean().ShouldBeFalse();
    }

    /// <summary>Checks the repaired destination paths retain the exact prepared existence, length, and content hashes.</summary>
    /// <param name="seed">The seeded genuinely mixed transaction and its projected prepared fingerprints.</param>
    private static void AssertPreparedArtifactContent(NativeWorkspaceRecoverySeed seed)
    {
        foreach (var artifact in seed.PreparedBaseline.ShouldNotBeNull().Artifacts)
        {
            File.Exists(artifact.Path).ShouldBe(artifact.Fingerprint.Exists);
            if (!artifact.Fingerprint.Exists)
            {
                continue;
            }

            var bytes = File.ReadAllBytes(artifact.Path);
            bytes.LongLength.ShouldBe(artifact.Fingerprint.Length);
            Convert.ToHexString(SHA256.HashData(bytes)).ShouldBe(artifact.Fingerprint.Sha256);
        }
    }

    /// <summary>Uses an independent production engine lifetime to adopt terminal evidence and commit a later output change.</summary>
    /// <param name="fixture">The generated native source fixture.</param>
    /// <param name="seed">The seeded committed physical state.</param>
    /// <param name="editorId">The later foreign EditorID.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>A task that completes after the independent owner commits and reopens the later output.</returns>
    private static async Task CommitForeignChangeAsync(
        NativeWorkspaceIntegrationFixture fixture,
        NativeWorkspaceRecoverySeed seed,
        string editorId,
        CancellationToken cancellationToken)
    {
        await using var services = NativeEngineComposition.Create();
        var open = await services.WorkspaceFactory.OpenAsync(fixture.CreateOpenRequest(), cancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var workspace = open.Value.ShouldNotBeNull();
        var selection = await workspace.SelectOutputAsync(
            new SelectOutputRequest(Guid.NewGuid(), workspace.Revision, OutputSelectionMode.OpenExisting, seed.Output),
            cancellationToken);
        selection.Succeeded.ShouldBeFalse();
        selection.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.RepairRequired);

        var recovery = await services.SaveCoordinator.RecoverAsync(
            new RecoverSaveRequest(seed.OriginalWorkspaceId, seed.SaveOperationId, seed.Output),
            cancellationToken);
        recovery.Status.ShouldBe(RecoverSaveStatus.Committed);
        var resolved = await workspace.ResolveOutputRecoveryAsync(
            new ResolveOutputRecoveryRequest(
                Guid.NewGuid(),
                workspace.Revision,
                OutputRecoveryAdoptionMode.ReopenResolvedOutput,
                recovery.ResolvedEvidence.ShouldNotBeNull()),
            cancellationToken);
        resolved.Succeeded.ShouldBeTrue(resolved.Error?.Message);

        var edit = await workspace.BeginEditAsync(
            new BeginEditRequest(
                Guid.NewGuid(),
                workspace.Revision,
                FormListEditRole.ExistingOutput,
                targetFormKey: seed.FormKey),
            cancellationToken);
        edit.Succeeded.ShouldBeTrue(edit.Error?.Message);
        var applied = await workspace.ApplyFormListEditAsync(
            new FormListEditRequest(
                Guid.NewGuid(),
                workspace.Revision,
                edit.Value.ShouldNotBeNull().EditId,
                new SetEditorIdEdit(editorId)),
            cancellationToken);
        applied.Succeeded.ShouldBeTrue(applied.Error?.Message);
        var save = await workspace.SaveAsync(
            new SaveRequest(Guid.NewGuid(), workspace.Revision, resolved.Value.ShouldNotBeNull().Baseline),
            cancellationToken);
        save.Status.ShouldBe(SaveCommitStatus.Committed, save.Error?.Message);
        save.CommittedBaseline.ShouldNotBeNull();
    }

    /// <summary>Reads one scalar string from host-retained immutable metadata.</summary>
    /// <param name="client">The initialized physical stdio client.</param>
    /// <param name="handle">The host-local immutable metadata handle.</param>
    /// <param name="path">The exact JSON Pointer to a string value.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>The exact projected string.</returns>
    private static async Task<string> ReadMetadataStringAsync(
        McpClient client,
        string handle,
        string path,
        CancellationToken cancellationToken)
    {
        var metadata = GetResult(await client.CallToolAsync(
            "creationsforge_metadata_read",
            MetadataArguments(handle, path),
            cancellationToken: cancellationToken));
        metadata.GetProperty("metadata").GetProperty("handle").GetString().ShouldBe(handle);
        var page = metadata.GetProperty("page");
        page.GetProperty("kind").GetString().ShouldBe("string");
        page.GetProperty("cursor").ValueKind.ShouldBe(JsonValueKind.Null);
        return page.GetProperty("value").GetString().ShouldNotBeNull();
    }

    /// <summary>Reads the staged-output FormList EditorID through the physical protocol.</summary>
    /// <param name="client">The initialized physical stdio client.</param>
    /// <param name="workspaceId">The selected live workspace identity.</param>
    /// <param name="output">The selected output containing the staged FormList.</param>
    /// <param name="formKey">The exact staged FormList identity.</param>
    /// <param name="expectedEditorId">The expected native EditorID.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>A task that completes after the scalar JSON page is checked.</returns>
    private static async Task AssertEditorIdAsync(
        McpClient client,
        Guid workspaceId,
        OutputAssociation output,
        Mutagen.Bethesda.Plugins.FormKey formKey,
        string expectedEditorId,
        CancellationToken cancellationToken)
    {
        var inspection = GetResult(await client.CallToolAsync(
            "creationsforge_formlist_inspect",
            new Dictionary<string, object?>
            {
                ["workspaceId"] = workspaceId.ToString("D"),
                ["selection"] = new Dictionary<string, object?>
                {
                    ["formKey"] = formKey.ToString(),
                    ["scope"] = "staged_output",
                    ["containingModKey"] = output.ModKey.ToString(),
                },
                ["path"] = "/EditorID",
                ["maxResults"] = 250,
            },
            cancellationToken: cancellationToken));
        var page = inspection.GetProperty("page");
        page.GetProperty("kind").GetString().ShouldBe("string");
        page.GetProperty("value").GetString().ShouldBe(expectedEditorId);
        page.GetProperty("cursor").ValueKind.ShouldBe(JsonValueKind.Null);
    }

    /// <summary>Closes one physical-host workspace and requires positive disposal acknowledgement.</summary>
    /// <param name="client">The initialized physical stdio client.</param>
    /// <param name="workspaceId">The live workspace identity.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>A task that completes after native disposal.</returns>
    private static async Task CloseWorkspaceAsync(
        McpClient client,
        Guid workspaceId,
        CancellationToken cancellationToken)
    {
        var close = GetResult(await client.CallToolAsync(
            "creationsforge_workspace_close",
            WorkspaceArguments(workspaceId),
            cancellationToken: cancellationToken));
        close.GetProperty("workspaceId").GetString().ShouldBe(workspaceId.ToString("D"));
        close.GetProperty("closed").GetBoolean().ShouldBeTrue();
        await AssertRegistryAsync(client, expectedWorkspaceId: null, cancellationToken);
    }

    /// <summary>Checks the physical host registry contains only the expected workspace, or is empty after closure.</summary>
    /// <param name="client">The initialized physical stdio client.</param>
    /// <param name="expectedWorkspaceId">The sole expected workspace identity, or <see langword="null"/> for an empty registry.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>A task that completes after exact registry inspection.</returns>
    private static async Task AssertRegistryAsync(
        McpClient client,
        Guid? expectedWorkspaceId,
        CancellationToken cancellationToken)
    {
        var serverInfo = GetResult(await client.CallToolAsync(
            "creationsforge_server_info",
            new Dictionary<string, object?>(),
            cancellationToken: cancellationToken));
        var lifecycle = serverInfo.GetProperty("lifecycle");
        var workspaceIds = lifecycle.GetProperty("activeWorkspaceIds");
        lifecycle.GetProperty("activeWorkspaceCount").GetInt32().ShouldBe(expectedWorkspaceId.HasValue ? 1 : 0);
        workspaceIds.GetArrayLength().ShouldBe(expectedWorkspaceId.HasValue ? 1 : 0);
        if (expectedWorkspaceId.HasValue)
        {
            workspaceIds[0].GetString().ShouldBe(expectedWorkspaceId.Value.ToString("D"));
        }
    }

    /// <summary>Requires the physical child to exit normally and preserves standard error as failure context.</summary>
    /// <param name="process">The owned physical stdio fixture.</param>
    /// <param name="cancellationToken">The bounded test token.</param>
    /// <returns>A task that completes after normal process exit.</returns>
    private static async Task AssertNormalExitAsync(
        McpStdioProcessFixture process,
        CancellationToken cancellationToken)
    {
        var completion = await process.CompleteAsync(cancellationToken);
        completion.ExitCode.ShouldBe(0, completion.StandardError);
    }

    /// <summary>Creates exact MCP workspace-open arguments from an engine request.</summary>
    /// <param name="request">The explicit generated native request.</param>
    /// <returns>A closed protocol argument dictionary.</returns>
    private static Dictionary<string, object?> OpenArguments(WorkspaceOpenRequest request)
    {
        return new Dictionary<string, object?>
        {
            ["workspaceId"] = request.WorkspaceId.ToString("D"),
            ["game"] = "starfield",
            ["release"] = "starfield",
            ["sourcePluginPath"] = request.SourcePluginPath,
            ["loadOrderPluginPaths"] = request.LoadOrderPluginPaths,
            ["dataDirectoryPath"] = request.DataDirectoryPath,
            ["stringDirectoryPaths"] = request.StringDirectoryPaths,
        };
    }

    /// <summary>Creates a closed request for allocating one new FormList edit.</summary>
    /// <param name="workspaceId">The live workspace identity.</param>
    /// <param name="operationId">The begin-edit idempotency identity.</param>
    /// <param name="revision">The exact live revision.</param>
    /// <returns>A closed protocol argument dictionary.</returns>
    private static Dictionary<string, object?> BeginNewEditArguments(
        Guid workspaceId,
        Guid operationId,
        WorkspaceRevision revision)
    {
        return new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"),
            ["operationId"] = operationId.ToString("D"),
            ["expectedRevision"] = RevisionArgument(revision),
            ["role"] = "new",
        };
    }

    /// <summary>Creates a closed typed-edit request for one deterministic EditorID.</summary>
    /// <param name="workspaceId">The live workspace identity.</param>
    /// <param name="operationId">The apply-edit idempotency identity.</param>
    /// <param name="revision">The exact live revision.</param>
    /// <param name="editId">The active edit-session identity.</param>
    /// <param name="editorId">The EditorID to stage.</param>
    /// <returns>A closed protocol argument dictionary.</returns>
    private static Dictionary<string, object?> ApplyEditorIdArguments(
        Guid workspaceId,
        Guid operationId,
        WorkspaceRevision revision,
        Guid editId,
        string editorId)
    {
        return new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"),
            ["operationId"] = operationId.ToString("D"),
            ["expectedRevision"] = RevisionArgument(revision),
            ["editId"] = editId.ToString("D"),
            ["commandName"] = "form-list.set-editor-id",
            ["argumentsJson"] = JsonSerializer.Serialize(new { editorId }),
        };
    }

    /// <summary>Creates exact arguments for saving against one retained output baseline.</summary>
    /// <param name="workspaceId">The live workspace identity.</param>
    /// <param name="operationId">The save operation identity.</param>
    /// <param name="revision">The exact live revision.</param>
    /// <param name="baselineHandle">The host-local selected baseline handle.</param>
    /// <returns>A closed protocol argument dictionary.</returns>
    private static Dictionary<string, object?> SaveArguments(
        Guid workspaceId,
        Guid operationId,
        WorkspaceRevision revision,
        string baselineHandle)
    {
        return new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"),
            ["operationId"] = operationId.ToString("D"),
            ["expectedRevision"] = RevisionArgument(revision),
            ["expectedOutputBaselineHandle"] = baselineHandle,
        };
    }

    /// <summary>Creates a closed output-selection request.</summary>
    /// <param name="workspaceId">The live workspace identity.</param>
    /// <param name="operationId">The selection idempotency identity.</param>
    /// <param name="revision">The exact live revision.</param>
    /// <param name="mode">The exact create-new or open-existing protocol mode.</param>
    /// <param name="output">The exact output association.</param>
    /// <returns>A closed protocol argument dictionary.</returns>
    private static Dictionary<string, object?> SelectOutputArguments(
        Guid workspaceId,
        Guid operationId,
        WorkspaceRevision revision,
        string mode,
        OutputAssociation output)
    {
        return new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"),
            ["operationId"] = operationId.ToString("D"),
            ["expectedRevision"] = RevisionArgument(revision),
            ["mode"] = mode,
            ["output"] = OutputArgument(output),
        };
    }

    /// <summary>Creates a bounded immutable metadata-read request.</summary>
    /// <param name="handle">The host-local immutable metadata handle.</param>
    /// <param name="path">The exact JSON Pointer to read.</param>
    /// <returns>A closed protocol argument dictionary.</returns>
    private static Dictionary<string, object?> MetadataArguments(string handle, string path)
    {
        return new Dictionary<string, object?>
        {
            ["metadataHandle"] = handle,
            ["path"] = path,
            ["maxResults"] = 250,
        };
    }

    /// <summary>Creates exact restart-safe recovery arguments.</summary>
    /// <param name="seed">The seeded recognized transaction.</param>
    /// <returns>A closed protocol argument dictionary.</returns>
    private static Dictionary<string, object?> RecoverArguments(NativeWorkspaceRecoverySeed seed)
    {
        return new Dictionary<string, object?>
        {
            ["originalWorkspaceId"] = seed.OriginalWorkspaceId.ToString("D"),
            ["saveOperationId"] = seed.SaveOperationId.ToString("D"),
            ["output"] = OutputArgument(seed.Output),
        };
    }

    /// <summary>Creates exact complete-prepared repair arguments.</summary>
    /// <param name="seed">The seeded genuinely mixed transaction.</param>
    /// <param name="repairOperationId">The new repair identity.</param>
    /// <param name="evidenceToken">The reviewed recovery token.</param>
    /// <returns>A closed protocol argument dictionary.</returns>
    private static Dictionary<string, object?> RepairArguments(
        NativeWorkspaceRecoverySeed seed,
        Guid repairOperationId,
        string evidenceToken)
    {
        return new Dictionary<string, object?>
        {
            ["originalWorkspaceId"] = seed.OriginalWorkspaceId.ToString("D"),
            ["saveOperationId"] = seed.SaveOperationId.ToString("D"),
            ["repairOperationId"] = repairOperationId.ToString("D"),
            ["expectedSaveRevision"] = RevisionArgument(seed.SaveBaseRevision),
            ["output"] = OutputArgument(seed.Output),
            ["evidenceToken"] = evidenceToken,
            ["direction"] = "complete_prepared",
        };
    }

    /// <summary>Creates exact recovery-adoption arguments.</summary>
    /// <param name="workspaceId">The live workspace identity.</param>
    /// <param name="operationId">The new adoption identity.</param>
    /// <param name="revision">The exact live revision.</param>
    /// <param name="evidenceHandle">The terminal evidence handle.</param>
    /// <returns>A closed protocol argument dictionary.</returns>
    private static Dictionary<string, object?> ResolveArguments(
        Guid workspaceId,
        Guid operationId,
        WorkspaceRevision revision,
        string evidenceHandle)
    {
        return new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"),
            ["operationId"] = operationId.ToString("D"),
            ["expectedRevision"] = RevisionArgument(revision),
            ["mode"] = "reopen_resolved_output",
            ["resolvedEvidenceHandle"] = evidenceHandle,
        };
    }

    /// <summary>Creates one closed workspace-identity argument object.</summary>
    /// <param name="workspaceId">The live workspace identity.</param>
    /// <returns>A closed protocol argument dictionary.</returns>
    private static Dictionary<string, object?> WorkspaceArguments(Guid workspaceId)
    {
        return new Dictionary<string, object?>
        {
            ["workspaceId"] = workspaceId.ToString("D"),
        };
    }

    /// <summary>Projects an exact workspace revision to transport values.</summary>
    /// <param name="revision">The exact revision.</param>
    /// <returns>A closed revision object.</returns>
    private static Dictionary<string, object?> RevisionArgument(WorkspaceRevision revision)
    {
        return new Dictionary<string, object?>
        {
            ["baselineId"] = revision.BaselineId.ToString("D"),
            ["sequence"] = revision.Sequence.ToString(System.Globalization.CultureInfo.InvariantCulture),
        };
    }

    /// <summary>Projects an exact output association to restart-safe transport values.</summary>
    /// <param name="output">The exact native output association.</param>
    /// <returns>A closed output object.</returns>
    private static Dictionary<string, object?> OutputArgument(OutputAssociation output)
    {
        return new Dictionary<string, object?>
        {
            ["pluginPath"] = output.PluginPath,
            ["modKey"] = output.ModKey.ToString(),
            ["localizedOutputMode"] = output.LocalizedOutputMode switch
            {
                LocalizedOutputMode.Embedded => "embedded",
                LocalizedOutputMode.SeparateStringFiles => "separate_string_files",
                _ => throw new ArgumentOutOfRangeException(nameof(output), output.LocalizedOutputMode, "The output uses an unsupported localized-output mode."),
            },
            ["masterStyle"] = "full",
        };
    }

    /// <summary>Reads one canonical transport revision.</summary>
    /// <param name="value">The structured revision object.</param>
    /// <returns>The exact engine revision.</returns>
    private static WorkspaceRevision ReadRevision(JsonElement value)
    {
        return new WorkspaceRevision(
            Guid.Parse(value.GetProperty("baselineId").GetString().ShouldNotBeNull()),
            ulong.Parse(
                value.GetProperty("sequence").GetString().ShouldNotBeNull(),
                System.Globalization.CultureInfo.InvariantCulture));
    }

    /// <summary>Checks two physical snapshots for exact path, absence, and byte equality.</summary>
    /// <param name="expected">The expected stable physical snapshot.</param>
    /// <param name="actual">The later physical snapshot.</param>
    private static void AssertPhysicalArtifactsEqual(
        IReadOnlyDictionary<string, byte[]?> expected,
        IReadOnlyDictionary<string, byte[]?> actual)
    {
        actual.Keys.ShouldBe(expected.Keys, ignoreOrder: false);
        foreach (var path in expected.Keys)
        {
            if (expected[path] is null)
            {
                actual[path].ShouldBeNull();
            }
            else
            {
                actual[path].ShouldNotBeNull().ShouldBe(expected[path]);
            }
        }
    }

    /// <summary>Checks every generated source artifact remains byte-exact.</summary>
    /// <param name="expected">The original source artifact snapshot.</param>
    /// <param name="actual">The final source artifact snapshot.</param>
    private static void AssertSourceArtifactsEqual(
        IReadOnlyDictionary<string, byte[]> expected,
        IReadOnlyDictionary<string, byte[]> actual)
    {
        actual.Keys.ShouldBe(expected.Keys, ignoreOrder: false);
        foreach (var path in expected.Keys)
        {
            actual[path].ShouldBe(expected[path]);
        }
    }

    /// <summary>Extracts a successful tool-specific structured result.</summary>
    /// <param name="result">The SDK call result.</param>
    /// <returns>The nested result object.</returns>
    private static JsonElement GetResult(CallToolResult result)
    {
        result.IsError.ShouldNotBe(true);
        result.StructuredContent.ShouldNotBeNull();
        var structured = result.StructuredContent.Value;
        structured.GetProperty("ok").GetBoolean().ShouldBeTrue();
        return structured.GetProperty("result");
    }

    /// <summary>Extracts a failed tool's stable protocol error code.</summary>
    /// <param name="result">The SDK call result.</param>
    /// <returns>The stable protocol error code.</returns>
    private static string GetErrorCode(CallToolResult result)
    {
        result.IsError.ShouldBe(true);
        result.StructuredContent.ShouldNotBeNull();
        var structured = result.StructuredContent.Value;
        structured.GetProperty("ok").GetBoolean().ShouldBeFalse();
        return structured.GetProperty("error").GetProperty("code").GetString().ShouldNotBeNull();
    }
}
