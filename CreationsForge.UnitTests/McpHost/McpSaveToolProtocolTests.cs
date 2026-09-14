using System.IO.Pipelines;
using System.Text.Json;
using CreationsForge.Mcp;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Moq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>Verifies exact save, recovery, repair, state, and historical metadata contracts through a real MCP SDK stream.</summary>
public sealed class McpSaveToolProtocolTests
{
    /// <summary>Verifies every save commitment status remains a successful outcome receipt even when the result carries an error.</summary>
    /// <returns>A task that completes after all four status receipts are observed.</returns>
    [Fact(Timeout = 30_000)]
    public async Task Save_ThroughSdkProtocol_PreservesEveryCommitStatusAndError()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 4);
        var resultRevision = new WorkspaceRevision(revision.BaselineId, 5);
        var output = CreateOutput("StatusOutput.esm");
        var baseline = CreateBaseline(output, includeStrings: true);
        var token = new RecoveryEvidenceToken(new string('A', 64));
        var operationIds = Enumerable.Range(0, 4).Select(_ => Guid.NewGuid()).ToArray();
        var evidence = CreateEvidence(workspaceId, operationIds[3], revision, output, baseline, token, RecoverSaveStatus.Committed);
        var results = new Dictionary<Guid, SaveResult>
        {
            [operationIds[0]] = new SaveResult(workspaceId, operationIds[0], revision, resultRevision, SaveCommitStatus.Committed, baseline, null, null, null, []),
            [operationIds[1]] = new SaveResult(workspaceId, operationIds[1], revision, revision, SaveCommitStatus.NotCommitted, null, null, null, new EngineError(EngineErrorCode.ValidationFailed, "Rejected before destination mutation."), []),
            [operationIds[2]] = new SaveResult(workspaceId, operationIds[2], revision, revision, SaveCommitStatus.CommitOutcomeUnknown, null, token, null, new EngineError(EngineErrorCode.CommitOutcomeUnknown, "Destination commitment requires recovery."), []),
            [operationIds[3]] = new SaveResult(workspaceId, operationIds[3], revision, revision, SaveCommitStatus.CommittedButReopenFailed, baseline, token, evidence, new EngineError(EngineErrorCode.OutputOpenFailed, "Committed output could not be reopened."), []),
        };
        var workspace = CreateWorkspace(workspaceId, revision);
        workspace.Setup(candidate => candidate.SaveAsync(It.IsAny<SaveRequest>(), It.IsAny<CancellationToken>()))
            .Returns((SaveRequest request, CancellationToken _) => ValueTask.FromResult(results[request.OperationId]));
        await using var registry = new McpWorkspaceRegistry();
        await OpenRegistryAsync(registry, workspace.Object, workspaceId);
        using var store = new McpMetadataStore();
        store.TryPublishWorkspaceMetadata(workspaceId, baseline, out var baselineReference).ShouldBeTrue();
        await using var harness = await ProtocolHarness.CreateAsync([new SaveTool(registry, store)]);
        var expected = new[]
        {
            (Status: "committed", Error: (string?)null, HasBaseline: true, HasEvidence: false, Token: (string?)null),
            (Status: "not_committed", Error: "validation_failed", HasBaseline: false, HasEvidence: false, Token: (string?)null),
            (Status: "commit_outcome_unknown", Error: "commit_outcome_unknown", HasBaseline: false, HasEvidence: false, Token: token.Value),
            (Status: "committed_but_reopen_failed", Error: "output_open_failed", HasBaseline: true, HasEvidence: true, Token: token.Value),
        };

        for (var index = 0; index < expected.Length; index++)
        {
            var receipt = GetResult(await harness.Client.CallToolAsync(
                "creationsforge_save",
                SaveArguments(workspaceId, operationIds[index], revision, baselineReference.Handle),
                cancellationToken: TestContext.Current.CancellationToken));

            receipt.GetProperty("workspaceId").GetString().ShouldBe(workspaceId.ToString("D"));
            receipt.GetProperty("operationId").GetString().ShouldBe(operationIds[index].ToString("D"));
            receipt.GetProperty("baseRevision").GetProperty("baselineId").GetString().ShouldBe(revision.BaselineId.ToString("D"));
            receipt.GetProperty("baseRevision").GetProperty("sequence").GetString().ShouldBe("4");
            receipt.GetProperty("resultRevision").GetProperty("baselineId").GetString().ShouldBe(resultRevision.BaselineId.ToString("D"));
            receipt.GetProperty("resultRevision").GetProperty("sequence").GetString().ShouldBe(index == 0 ? "5" : "4");
            receipt.GetProperty("status").GetString().ShouldBe(expected[index].Status);
            receipt.GetProperty("errorCode").GetString().ShouldBe(expected[index].Error);
            receipt.GetProperty("recoveryEvidenceToken").GetString().ShouldBe(expected[index].Token);
            receipt.GetProperty("warningCount").GetInt32().ShouldBe(0);
            receipt.GetProperty("detailsUnavailable").GetBoolean().ShouldBeFalse();
            receipt.GetProperty("details").GetProperty("kind").GetString().ShouldBe("save_result");
            receipt.GetProperty("output").ValueKind.ShouldBe(expected[index].HasEvidence ? JsonValueKind.Object : JsonValueKind.Null);
            (receipt.GetProperty("committedBaseline").ValueKind != JsonValueKind.Null).ShouldBe(expected[index].HasBaseline);
            if (expected[index].HasBaseline)
            {
                receipt.GetProperty("committedBaseline").GetProperty("kind").GetString().ShouldBe("output_baseline");
                receipt.GetProperty("committedBaseline").GetProperty("baselineId").GetString().ShouldBe(baseline.BaselineId.ToString("D"));
            }

            (receipt.GetProperty("resolvedEvidence").ValueKind != JsonValueKind.Null).ShouldBe(expected[index].HasEvidence);
            if (expected[index].HasEvidence)
            {
                receipt.GetProperty("output").GetProperty("kind").GetString().ShouldBe("output_association");
                receipt.GetProperty("resolvedEvidence").GetProperty("kind").GetString().ShouldBe("resolved_output_evidence");
            }
        }

        workspace.Verify(candidate => candidate.SaveAsync(
            It.Is<SaveRequest>(request => ReferenceEquals(request.ExpectedOutputBaseline, baseline)),
            It.IsAny<CancellationToken>()), Times.Exactly(4));
    }

    /// <summary>Verifies full metadata capacity cannot block exact repeated saves or hide a newly committed outcome.</summary>
    /// <returns>A task that completes after repeated and committed saves run at global capacity.</returns>
    [Fact(Timeout = 30_000)]
    public async Task Save_ThroughSdkProtocol_ReplaysExactHistoricalBaselineAtGlobalCapacity()
    {
        var workspaceId = Guid.NewGuid();
        var operationId = Guid.NewGuid();
        var committedOperationId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 8);
        var output = CreateOutput("ReplayOutput.esm");
        var baseline = CreateBaseline(output, includeStrings: false);
        var rejectedResult = new SaveResult(
            workspaceId,
            operationId,
            revision,
            new WorkspaceRevision(revision.BaselineId, 9),
            SaveCommitStatus.NotCommitted,
            null,
            null,
            null,
            new EngineError(EngineErrorCode.ValidationFailed, "Replayable rejection."),
            []);
        var committedBaseline = CreateBaseline(output, includeStrings: true);
        var committedResult = new SaveResult(
            workspaceId,
            committedOperationId,
            revision,
            new WorkspaceRevision(revision.BaselineId, 10),
            SaveCommitStatus.Committed,
            committedBaseline,
            null,
            null,
            null,
            []);
        var workspace = CreateWorkspace(workspaceId, new WorkspaceRevision(revision.BaselineId, 9));
        workspace.Setup(candidate => candidate.SaveAsync(It.IsAny<SaveRequest>(), It.IsAny<CancellationToken>()))
            .Returns((SaveRequest request, CancellationToken _) =>
            {
                request.ExpectedRevision.ShouldBe(revision);
                request.ExpectedOutputBaseline.ShouldBeSameAs(baseline);
                return ValueTask.FromResult(request.OperationId == operationId ? rejectedResult : committedResult);
            });
        await using var registry = new McpWorkspaceRegistry();
        await OpenRegistryAsync(registry, workspace.Object, workspaceId);
        using var store = new McpMetadataStore(maximumHandles: 1, operationReservationSize: 1);
        store.TryPublishWorkspaceMetadata(workspaceId, baseline, out var baselineReference).ShouldBeTrue();
        await using var harness = await ProtocolHarness.CreateAsync([new SaveTool(registry, store)]);
        var arguments = SaveArguments(workspaceId, operationId, revision, baselineReference.Handle);

        var first = GetResult(await harness.Client.CallToolAsync("creationsforge_save", arguments, cancellationToken: TestContext.Current.CancellationToken));
        first.GetProperty("status").GetString().ShouldBe("not_committed");
        first.GetProperty("details").ValueKind.ShouldBe(JsonValueKind.Null);
        first.GetProperty("detailsUnavailable").GetBoolean().ShouldBeTrue();
        store.AllocatedSlots.ShouldBe(1);
        var second = GetResult(await harness.Client.CallToolAsync("creationsforge_save", arguments, cancellationToken: TestContext.Current.CancellationToken));
        second.GetProperty("status").GetString().ShouldBe("not_committed");
        second.GetProperty("detailsUnavailable").GetBoolean().ShouldBeTrue();
        var committed = GetResult(await harness.Client.CallToolAsync(
            "creationsforge_save",
            SaveArguments(workspaceId, committedOperationId, revision, baselineReference.Handle),
            cancellationToken: TestContext.Current.CancellationToken));
        committed.GetProperty("status").GetString().ShouldBe("committed");
        committed.GetProperty("committedBaseline").ValueKind.ShouldBe(JsonValueKind.Null);
        committed.GetProperty("details").ValueKind.ShouldBe(JsonValueKind.Null);
        committed.GetProperty("detailsUnavailable").GetBoolean().ShouldBeTrue();
        workspace.Verify(candidate => candidate.SaveAsync(It.IsAny<SaveRequest>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    /// <summary>Verifies recovery inspection and explicit repair operate from persisted identities without an active workspace registry.</summary>
    /// <returns>A task that completes after both coordinator requests are observed through the transport.</returns>
    [Fact(Timeout = 30_000)]
    public async Task RecoverAndRepair_ThroughSdkProtocol_DoNotRequireLiveWorkspace()
    {
        var workspaceId = Guid.NewGuid();
        var saveOperationId = Guid.NewGuid();
        var repairOperationId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 6);
        var output = CreateOutput("RestartOutput.esm");
        var baseline = CreateBaseline(output, includeStrings: false);
        var token = new RecoveryEvidenceToken(new string('B', 64));
        var evidence = CreateEvidence(workspaceId, saveOperationId, revision, output, baseline, token, RecoverSaveStatus.Committed);
        var coordinator = new Mock<IWorkspaceSaveCoordinator>(MockBehavior.Strict);
        coordinator.Setup(candidate => candidate.RecoverAsync(It.IsAny<RecoverSaveRequest>(), It.IsAny<CancellationToken>()))
            .Returns((RecoverSaveRequest request, CancellationToken _) =>
            {
                request.WorkspaceId.ShouldBe(workspaceId);
                request.SaveOperationId.ShouldBe(saveOperationId);
                AssertOutput(request.Output, output);
                return ValueTask.FromResult(new RecoverSaveResult(
                    workspaceId,
                    saveOperationId,
                    RecoverSaveStatus.StillUnknown,
                    revision,
                    true,
                    token,
                    null,
                    new EngineError(EngineErrorCode.RepairRequired, "Reviewed repair is required.")));
            });
        coordinator.Setup(candidate => candidate.RepairAsync(It.IsAny<RepairSaveRequest>(), It.IsAny<CancellationToken>()))
            .Returns((RepairSaveRequest request, CancellationToken _) =>
            {
                request.WorkspaceId.ShouldBe(workspaceId);
                request.SaveOperationId.ShouldBe(saveOperationId);
                request.RepairOperationId.ShouldBe(repairOperationId);
                request.ExpectedSaveRevision.ShouldBe(revision);
                request.EvidenceToken.Value.ShouldBe(token.Value);
                request.Direction.ShouldBe(RepairSaveDirection.CompletePrepared);
                AssertOutput(request.Output, output);
                return ValueTask.FromResult(new RepairSaveResult(
                    workspaceId,
                    saveOperationId,
                    repairOperationId,
                    RepairSaveStatus.PreparedSetCompleted,
                    baseline,
                    token,
                    evidence,
                    null));
            });
        using var store = new McpMetadataStore();
        await using var harness = await ProtocolHarness.CreateAsync(
            [new SaveRecoverTool(coordinator.Object, store), new SaveRepairTool(coordinator.Object, store)]);

        var recovery = GetResult(await harness.Client.CallToolAsync(
            "creationsforge_save_recover",
            RecoverArguments(workspaceId, saveOperationId, output),
            cancellationToken: TestContext.Current.CancellationToken));
        recovery.GetProperty("originalWorkspaceId").GetString().ShouldBe(workspaceId.ToString("D"));
        recovery.GetProperty("saveOperationId").GetString().ShouldBe(saveOperationId.ToString("D"));
        recovery.GetProperty("status").GetString().ShouldBe("still_unknown");
        recovery.GetProperty("saveBaseRevision").GetProperty("baselineId").GetString().ShouldBe(revision.BaselineId.ToString("D"));
        recovery.GetProperty("saveBaseRevision").GetProperty("sequence").GetString().ShouldBe("6");
        recovery.GetProperty("repairRequired").GetBoolean().ShouldBeTrue();
        recovery.GetProperty("evidenceToken").GetString().ShouldBe(token.Value);
        recovery.GetProperty("output").ValueKind.ShouldBe(JsonValueKind.Null);
        recovery.GetProperty("resolvedBaseline").ValueKind.ShouldBe(JsonValueKind.Null);
        recovery.GetProperty("resolvedEvidence").ValueKind.ShouldBe(JsonValueKind.Null);
        recovery.GetProperty("errorCode").GetString().ShouldBe("repair_required");
        recovery.GetProperty("details").GetProperty("kind").GetString().ShouldBe("recover_save_result");
        recovery.GetProperty("detailsUnavailable").GetBoolean().ShouldBeFalse();

        var repair = GetResult(await harness.Client.CallToolAsync(
            "creationsforge_save_repair",
            RepairArguments(workspaceId, saveOperationId, repairOperationId, revision, output, token.Value),
            cancellationToken: TestContext.Current.CancellationToken));
        repair.GetProperty("originalWorkspaceId").GetString().ShouldBe(workspaceId.ToString("D"));
        repair.GetProperty("saveOperationId").GetString().ShouldBe(saveOperationId.ToString("D"));
        repair.GetProperty("repairOperationId").GetString().ShouldBe(repairOperationId.ToString("D"));
        repair.GetProperty("status").GetString().ShouldBe("prepared_set_completed");
        repair.GetProperty("resultingBaseline").GetProperty("kind").GetString().ShouldBe("output_baseline");
        repair.GetProperty("resultingBaseline").GetProperty("baselineId").GetString().ShouldBe(baseline.BaselineId.ToString("D"));
        repair.GetProperty("output").GetProperty("kind").GetString().ShouldBe("output_association");
        repair.GetProperty("resolvedEvidence").GetProperty("kind").GetString().ShouldBe("resolved_output_evidence");
        repair.GetProperty("evidenceToken").GetString().ShouldBe(token.Value);
        repair.GetProperty("errorCode").ValueKind.ShouldBe(JsonValueKind.Null);
        repair.GetProperty("details").GetProperty("kind").GetString().ShouldBe("repair_save_result");
        repair.GetProperty("detailsUnavailable").GetBoolean().ShouldBeFalse();
        coordinator.VerifyAll();
    }

    /// <summary>Verifies terminal evidence crosses workspace origins exactly and historical metadata cursors survive a later live mutation.</summary>
    /// <returns>A task that completes after recovery, adoption, and both metadata pages are observed.</returns>
    [Fact(Timeout = 30_000)]
    public async Task EvidenceAndMetadata_ThroughSdkProtocol_PreserveIdentityAcrossLiveMutation()
    {
        var originalWorkspaceId = Guid.NewGuid();
        var liveWorkspaceId = Guid.NewGuid();
        var saveOperationId = Guid.NewGuid();
        var adoptionOperationId = Guid.NewGuid();
        var saveRevision = new WorkspaceRevision(Guid.NewGuid(), 2);
        var liveRevision = new WorkspaceRevision(Guid.NewGuid(), 10);
        var adoptedRevision = new WorkspaceRevision(liveRevision.BaselineId, 11);
        var output = CreateOutput("EvidenceOutput.esm");
        var baseline = CreateBaseline(output, includeStrings: true);
        var token = new RecoveryEvidenceToken(new string('C', 64));
        var evidence = CreateEvidence(originalWorkspaceId, saveOperationId, saveRevision, output, baseline, token, RecoverSaveStatus.Committed);
        var coordinator = new Mock<IWorkspaceSaveCoordinator>(MockBehavior.Strict);
        coordinator.Setup(candidate => candidate.RecoverAsync(It.IsAny<RecoverSaveRequest>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(new RecoverSaveResult(
                originalWorkspaceId,
                saveOperationId,
                RecoverSaveStatus.Committed,
                saveRevision,
                false,
                token,
                evidence,
                null)));
        var workspace = CreateWorkspace(liveWorkspaceId, liveRevision);
        workspace.Setup(candidate => candidate.ResolveOutputRecoveryAsync(It.IsAny<ResolveOutputRecoveryRequest>(), It.IsAny<CancellationToken>()))
            .Returns((ResolveOutputRecoveryRequest request, CancellationToken _) =>
            {
                request.OperationId.ShouldBe(adoptionOperationId);
                request.ExpectedRevision.ShouldBe(liveRevision);
                request.Mode.ShouldBe(OutputRecoveryAdoptionMode.ReopenResolvedOutput);
                request.Evidence.ShouldBeSameAs(evidence);
                return ValueTask.FromResult(EngineResult<OutputSelectionReceipt>.Success(
                    new OutputSelectionReceipt(output, baseline, adoptedRevision),
                    workspaceId: liveWorkspaceId,
                    operationId: adoptionOperationId,
                    baseRevision: liveRevision,
                    resultRevision: adoptedRevision));
            });
        await using var registry = new McpWorkspaceRegistry();
        await OpenRegistryAsync(registry, workspace.Object, liveWorkspaceId);
        using var store = new McpMetadataStore();
        await using var harness = await ProtocolHarness.CreateAsync(
            [new SaveRecoverTool(coordinator.Object, store), new MetadataReadTool(store), new OutputRecoveryResolveTool(registry, store)]);
        var recovery = GetResult(await harness.Client.CallToolAsync(
            "creationsforge_save_recover",
            RecoverArguments(originalWorkspaceId, saveOperationId, output),
            cancellationToken: TestContext.Current.CancellationToken));
        recovery.GetProperty("originalWorkspaceId").GetString().ShouldBe(originalWorkspaceId.ToString("D"));
        recovery.GetProperty("saveOperationId").GetString().ShouldBe(saveOperationId.ToString("D"));
        recovery.GetProperty("status").GetString().ShouldBe("committed");
        recovery.GetProperty("saveBaseRevision").GetProperty("baselineId").GetString().ShouldBe(saveRevision.BaselineId.ToString("D"));
        recovery.GetProperty("saveBaseRevision").GetProperty("sequence").GetString().ShouldBe("2");
        recovery.GetProperty("repairRequired").GetBoolean().ShouldBeFalse();
        recovery.GetProperty("evidenceToken").GetString().ShouldBe(token.Value);
        recovery.GetProperty("output").GetProperty("kind").GetString().ShouldBe("output_association");
        recovery.GetProperty("resolvedBaseline").GetProperty("kind").GetString().ShouldBe("output_baseline");
        recovery.GetProperty("resolvedBaseline").GetProperty("baselineId").GetString().ShouldBe(baseline.BaselineId.ToString("D"));
        recovery.GetProperty("resolvedEvidence").GetProperty("kind").GetString().ShouldBe("resolved_output_evidence");
        recovery.GetProperty("errorCode").ValueKind.ShouldBe(JsonValueKind.Null);
        recovery.GetProperty("details").GetProperty("kind").GetString().ShouldBe("recover_save_result");
        recovery.GetProperty("detailsUnavailable").GetBoolean().ShouldBeFalse();
        var evidenceHandle = recovery.GetProperty("resolvedEvidence").GetProperty("handle").GetString().ShouldNotBeNull();
        var firstPage = GetResult(await harness.Client.CallToolAsync(
            "creationsforge_metadata_read",
            MetadataArguments(evidenceHandle, "/resolvedOutputBaseline/artifacts", 1),
            cancellationToken: TestContext.Current.CancellationToken)).GetProperty("page");
        firstPage.GetProperty("count").GetInt32().ShouldBe(1);
        firstPage.GetProperty("totalCount").GetInt32().ShouldBe(2);
        var cursor = firstPage.GetProperty("cursor").GetString().ShouldNotBeNull();

        var adoption = GetResult(await harness.Client.CallToolAsync(
            "creationsforge_output_recovery_resolve",
            ResolveArguments(liveWorkspaceId, adoptionOperationId, liveRevision, evidenceHandle),
            cancellationToken: TestContext.Current.CancellationToken));
        adoption.GetProperty("workspaceId").GetString().ShouldBe(liveWorkspaceId.ToString("D"));
        adoption.GetProperty("operationId").GetString().ShouldBe(adoptionOperationId.ToString("D"));
        adoption.GetProperty("mode").GetString().ShouldBe("reopen_resolved_output");
        adoption.GetProperty("revision").GetProperty("baselineId").GetString().ShouldBe(adoptedRevision.BaselineId.ToString("D"));
        adoption.GetProperty("revision").GetProperty("sequence").GetString().ShouldBe("11");
        adoption.GetProperty("output").GetProperty("kind").GetString().ShouldBe("output_association");
        adoption.GetProperty("outputBaseline").GetProperty("kind").GetString().ShouldBe("output_baseline");
        adoption.GetProperty("outputBaseline").GetProperty("baselineId").GetString().ShouldBe(baseline.BaselineId.ToString("D"));
        adoption.GetProperty("warningCount").GetInt32().ShouldBe(0);
        adoption.GetProperty("detailsUnavailable").GetBoolean().ShouldBeFalse();

        var continuationArguments = MetadataArguments(evidenceHandle, "/resolvedOutputBaseline/artifacts", 1);
        continuationArguments["cursor"] = cursor;
        var secondPage = GetResult(await harness.Client.CallToolAsync(
            "creationsforge_metadata_read",
            continuationArguments,
            cancellationToken: TestContext.Current.CancellationToken)).GetProperty("page");
        secondPage.GetProperty("count").GetInt32().ShouldBe(1);
        secondPage.GetProperty("cursor").ValueKind.ShouldBe(JsonValueKind.Null);
        secondPage.GetProperty("children")[0].GetProperty("path").GetString()
            .ShouldNotBe(firstPage.GetProperty("children")[0].GetProperty("path").GetString());
    }

    /// <summary>Verifies exhausted per-key recovery and repair capacity rejects before either coordinator method is invoked.</summary>
    /// <returns>A task that completes after both pre-invocation capacity failures are observed.</returns>
    [Fact(Timeout = 30_000)]
    public async Task RecoverAndRepair_ThroughSdkProtocol_RejectCapacityBeforeCoordinatorInvocation()
    {
        var workspaceId = Guid.NewGuid();
        var saveOperationId = Guid.NewGuid();
        var repairOperationId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 1);
        var output = CreateOutput("CapacityOutput.esm");
        using var store = new McpMetadataStore(maximumHandles: 32, operationReservationSize: 16);
        await FillReservationAsync(store, Guid.NewGuid(), Guid.NewGuid(), McpMetadataOperationKind.WorkspaceMutation, 16, 100);
        await FillReservationAsync(store, Guid.NewGuid(), Guid.NewGuid(), McpMetadataOperationKind.WorkspaceMutation, 16, 200);
        var coordinator = new Mock<IWorkspaceSaveCoordinator>(MockBehavior.Strict);
        await using var harness = await ProtocolHarness.CreateAsync(
            [new SaveRecoverTool(coordinator.Object, store), new SaveRepairTool(coordinator.Object, store)]);

        var recovery = await harness.Client.CallToolAsync(
            "creationsforge_save_recover",
            RecoverArguments(workspaceId, saveOperationId, output),
            cancellationToken: TestContext.Current.CancellationToken);
        GetErrorCode(recovery).ShouldBe("metadata_capacity_exceeded");
        var repair = await harness.Client.CallToolAsync(
            "creationsforge_save_repair",
            RepairArguments(workspaceId, saveOperationId, repairOperationId, revision, output, new string('D', 64)),
            cancellationToken: TestContext.Current.CancellationToken);
        GetErrorCode(repair).ShouldBe("metadata_capacity_exceeded");
        coordinator.Verify(candidate => candidate.RecoverAsync(It.IsAny<RecoverSaveRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        coordinator.Verify(candidate => candidate.RepairAsync(It.IsAny<RepairSaveRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>Verifies workspace state exposes the exact pending journal output when no output or baseline is selected.</summary>
    /// <returns>A task that completes after the pending association is read through its issued metadata handle.</returns>
    [Fact(Timeout = 30_000)]
    public async Task WorkspaceState_ThroughSdkProtocol_ProjectsPendingOutputWithoutSelectedOutput()
    {
        var liveWorkspaceId = Guid.NewGuid();
        var originalWorkspaceId = Guid.NewGuid();
        var saveOperationId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 0);
        var pendingOutput = CreateOutput("PendingOutput.esm");
        var pending = new PendingSaveIdentity(
            originalWorkspaceId,
            saveOperationId,
            revision,
            SupportedGame.Starfield,
            GameRelease.Starfield,
            pendingOutput);
        var state = new WorkspaceState(
            SupportedGame.Starfield,
            GameRelease.Starfield,
            null,
            null,
            new OutputSynchronizationState(OutputSynchronizationStatus.RecoveryRequired, pending),
            revision);
        var workspace = CreateWorkspace(liveWorkspaceId, revision);
        workspace.Setup(candidate => candidate.ReadStateAsync(It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<WorkspaceState>.Success(
                state,
                workspaceId: liveWorkspaceId,
                resultRevision: revision)));
        await using var registry = new McpWorkspaceRegistry();
        await OpenRegistryAsync(registry, workspace.Object, liveWorkspaceId);
        using var store = new McpMetadataStore();
        await using var harness = await ProtocolHarness.CreateAsync(
            [new WorkspaceStateTool(registry, store), new MetadataReadTool(store)]);

        var receipt = GetResult(await harness.Client.CallToolAsync(
            "creationsforge_workspace_state",
            new Dictionary<string, object?> { ["workspaceId"] = liveWorkspaceId.ToString("D") },
            cancellationToken: TestContext.Current.CancellationToken));
        receipt.GetProperty("output").ValueKind.ShouldBe(JsonValueKind.Null);
        receipt.GetProperty("outputBaseline").ValueKind.ShouldBe(JsonValueKind.Null);
        var pendingReceipt = receipt.GetProperty("outputSynchronization").GetProperty("pendingSave");
        pendingReceipt.GetProperty("originalWorkspaceId").GetString().ShouldBe(originalWorkspaceId.ToString("D"));
        var pendingHandle = pendingReceipt.GetProperty("output").GetProperty("handle").GetString().ShouldNotBeNull();
        var metadata = GetResult(await harness.Client.CallToolAsync(
            "creationsforge_metadata_read",
            MetadataArguments(pendingHandle, "/pluginPath", McpInput.MaximumResults),
            cancellationToken: TestContext.Current.CancellationToken));
        metadata.GetProperty("page").GetProperty("value").GetString().ShouldBe(pendingOutput.PluginPath);
    }

    /// <summary>Verifies the host input parser rejects malformed nested output and revision keys and values.</summary>
    [Fact]
    public void McpSaveInput_RejectsMalformedNestedUnicode()
    {
        using var invalidValueDocument = JsonDocument.Parse(
            "{\"pluginPath\":\"\\uD800\",\"modKey\":\"Output.esm\",\"localizedOutputMode\":\"embedded\",\"masterStyle\":\"full\"}");
        using var invalidNameDocument = JsonDocument.Parse(
            "{\"\\uD800\":\"ignored\",\"pluginPath\":\"C:\\\\Output.esm\",\"modKey\":\"Output.esm\",\"localizedOutputMode\":\"embedded\",\"masterStyle\":\"full\"}");

        foreach (var invalidOutput in new[] { invalidValueDocument.RootElement.Clone(), invalidNameDocument.RootElement.Clone() })
        {
            var arguments = new Dictionary<string, JsonElement>
            {
                ["output"] = invalidOutput,
            };

            McpSaveInput.TryGetOutputAssociation(arguments, "output", out _, out var error).ShouldBeFalse();
            error.ShouldNotBeNullOrWhiteSpace();
        }

        using var invalidRevisionValueDocument = JsonDocument.Parse(
            "{\"baselineId\":\"\\uD800\",\"sequence\":\"1\"}");
        using var invalidRevisionNameDocument = JsonDocument.Parse(
            $"{{\"\\uD800\":\"ignored\",\"baselineId\":\"{Guid.NewGuid():D}\",\"sequence\":\"1\"}}");
        foreach (var invalidRevision in new[] { invalidRevisionValueDocument.RootElement.Clone(), invalidRevisionNameDocument.RootElement.Clone() })
        {
            var arguments = new Dictionary<string, JsonElement>
            {
                ["expectedSaveRevision"] = invalidRevision,
            };

            McpSaveInput.TryGetRequiredRevision(arguments, "expectedSaveRevision", out _, out var error).ShouldBeFalse();
            error.ShouldNotBeNullOrWhiteSpace();
        }
    }

    /// <summary>Fills one operation's requested publication capacity with retained metadata handles.</summary>
    /// <param name="store">The host metadata store.</param>
    /// <param name="workspaceId">The operation workspace identity.</param>
    /// <param name="operationId">The operation identity.</param>
    /// <param name="kind">The owning workflow kind.</param>
    /// <param name="count">The number of distinct references to publish.</param>
    /// <param name="indexOffset">The first unique output suffix.</param>
    /// <returns>A task that completes after the lease is released.</returns>
    private static async Task FillReservationAsync(
        McpMetadataStore store,
        Guid workspaceId,
        Guid operationId,
        McpMetadataOperationKind kind,
        int count,
        int indexOffset)
    {
        await using var lease = await store.AcquireOperationAsync(workspaceId, operationId, kind, count);
        lease.ShouldNotBeNull();
        for (var index = 0; index < count; index++)
        {
            lease.Publish(CreateOutput($"Capacity{indexOffset + index}.esm")).ShouldNotBeNull();
        }
    }

    /// <summary>Creates canonical save-tool arguments.</summary>
    /// <param name="workspaceId">The live workspace identifier.</param>
    /// <param name="operationId">The save operation identifier.</param>
    /// <param name="revision">The exact expected revision.</param>
    /// <param name="baselineHandle">The exact retained baseline handle.</param>
    /// <returns>A closed MCP argument dictionary.</returns>
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
            ["expectedRevision"] = Revision(revision),
            ["expectedOutputBaselineHandle"] = baselineHandle,
        };
    }

    /// <summary>Creates canonical restart-safe recovery arguments.</summary>
    /// <param name="workspaceId">The original workspace identifier.</param>
    /// <param name="saveOperationId">The original save operation identifier.</param>
    /// <param name="output">The exact persisted output association.</param>
    /// <returns>A closed MCP argument dictionary.</returns>
    private static Dictionary<string, object?> RecoverArguments(Guid workspaceId, Guid saveOperationId, OutputAssociation output)
    {
        return new Dictionary<string, object?>
        {
            ["originalWorkspaceId"] = workspaceId.ToString("D"),
            ["saveOperationId"] = saveOperationId.ToString("D"),
            ["output"] = OutputArgument(output),
        };
    }

    /// <summary>Creates canonical explicit repair arguments.</summary>
    /// <param name="workspaceId">The original workspace identifier.</param>
    /// <param name="saveOperationId">The original save operation identifier.</param>
    /// <param name="repairOperationId">The new repair operation identifier.</param>
    /// <param name="revision">The expected original save revision.</param>
    /// <param name="output">The exact persisted output association.</param>
    /// <param name="evidenceToken">The caller-reviewed evidence token.</param>
    /// <returns>A closed MCP argument dictionary.</returns>
    private static Dictionary<string, object?> RepairArguments(
        Guid workspaceId,
        Guid saveOperationId,
        Guid repairOperationId,
        WorkspaceRevision revision,
        OutputAssociation output,
        string evidenceToken)
    {
        return new Dictionary<string, object?>
        {
            ["originalWorkspaceId"] = workspaceId.ToString("D"),
            ["saveOperationId"] = saveOperationId.ToString("D"),
            ["repairOperationId"] = repairOperationId.ToString("D"),
            ["expectedSaveRevision"] = Revision(revision),
            ["output"] = OutputArgument(output),
            ["evidenceToken"] = evidenceToken,
            ["direction"] = "complete_prepared",
        };
    }

    /// <summary>Creates canonical recovery-adoption arguments.</summary>
    /// <param name="workspaceId">The current live workspace identifier.</param>
    /// <param name="operationId">The adoption operation identifier.</param>
    /// <param name="revision">The current expected revision.</param>
    /// <param name="evidenceHandle">The exact terminal evidence handle.</param>
    /// <returns>A closed MCP argument dictionary.</returns>
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
            ["expectedRevision"] = Revision(revision),
            ["mode"] = "reopen_resolved_output",
            ["resolvedEvidenceHandle"] = evidenceHandle,
        };
    }

    /// <summary>Creates bounded historical metadata-read arguments.</summary>
    /// <param name="handle">The opaque metadata handle.</param>
    /// <param name="path">The exact JSON Pointer.</param>
    /// <param name="maximumResults">The requested page count.</param>
    /// <returns>A closed MCP argument dictionary.</returns>
    private static Dictionary<string, object?> MetadataArguments(string handle, string path, int maximumResults)
    {
        return new Dictionary<string, object?>
        {
            ["metadataHandle"] = handle,
            ["path"] = path,
            ["maxResults"] = maximumResults,
        };
    }

    /// <summary>Projects an exact revision into canonical transport values.</summary>
    /// <param name="revision">The exact Core revision.</param>
    /// <returns>A closed revision object.</returns>
    private static Dictionary<string, object?> Revision(WorkspaceRevision revision)
    {
        return new Dictionary<string, object?>
        {
            ["baselineId"] = revision.BaselineId.ToString("D"),
            ["sequence"] = revision.Sequence.ToString(System.Globalization.CultureInfo.InvariantCulture),
        };
    }

    /// <summary>Projects an output association into the restart-safe inline transport shape.</summary>
    /// <param name="output">The exact output association.</param>
    /// <returns>A closed output association object.</returns>
    private static Dictionary<string, object?> OutputArgument(OutputAssociation output)
    {
        return new Dictionary<string, object?>
        {
            ["pluginPath"] = output.PluginPath,
            ["modKey"] = output.ModKey.ToString(),
            ["localizedOutputMode"] = "embedded",
            ["masterStyle"] = "full",
        };
    }

    /// <summary>Creates a deterministic plugin output association.</summary>
    /// <param name="fileName">The unique output plugin file name.</param>
    /// <returns>An immutable output association.</returns>
    private static OutputAssociation CreateOutput(string fileName)
    {
        return new OutputAssociation(
            Path.Combine(Path.GetTempPath(), fileName),
            ModKey.FromNameAndExtension(fileName),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
    }

    /// <summary>Creates a complete absent output baseline with an optional localized sidecar.</summary>
    /// <param name="output">The exact output association.</param>
    /// <param name="includeStrings">Whether to include one absent strings artifact.</param>
    /// <returns>An immutable complete output baseline.</returns>
    private static OutputArtifactSetBaseline CreateBaseline(OutputAssociation output, bool includeStrings)
    {
        var artifacts = new List<PluginArtifactAssociation>
        {
            new(output.PluginPath, PluginArtifactRole.Plugin, null, new PluginArtifactFingerprint(false, 0, null)),
        };
        if (includeStrings)
        {
            artifacts.Add(new PluginArtifactAssociation(
                Path.Combine(Path.GetDirectoryName(output.PluginPath)!, $"{Path.GetFileNameWithoutExtension(output.PluginPath)}_en.strings"),
                PluginArtifactRole.Strings,
                "en",
                new PluginArtifactFingerprint(false, 0, null)));
        }

        return new OutputArtifactSetBaseline(Guid.NewGuid(), artifacts);
    }

    /// <summary>Creates complete terminal recovery evidence for protocol tests.</summary>
    /// <param name="workspaceId">The original save workspace identifier.</param>
    /// <param name="saveOperationId">The original save operation identifier.</param>
    /// <param name="revision">The original save base revision.</param>
    /// <param name="output">The exact saved output association.</param>
    /// <param name="baseline">The terminal output baseline.</param>
    /// <param name="token">The exact evidence token.</param>
    /// <param name="status">The terminal recovery status.</param>
    /// <returns>Immutable terminal recovery evidence.</returns>
    private static ResolvedOutputEvidence CreateEvidence(
        Guid workspaceId,
        Guid saveOperationId,
        WorkspaceRevision revision,
        OutputAssociation output,
        OutputArtifactSetBaseline baseline,
        RecoveryEvidenceToken token,
        RecoverSaveStatus status)
    {
        var sourcePath = Path.Combine(Path.GetTempPath(), "Source.esm");
        var sourceBaseline = new PluginSourceInputBaseline(
            revision.BaselineId,
            [new PluginArtifactAssociation(sourcePath, PluginArtifactRole.Plugin, null, new PluginArtifactFingerprint(false, 0, null))]);
        return new ResolvedOutputEvidence(
            token,
            SupportedGame.Starfield,
            GameRelease.Starfield,
            workspaceId,
            saveOperationId,
            revision,
            sourceBaseline,
            output,
            baseline,
            status);
    }

    /// <summary>Creates a workspace mock with deterministic identity and disposal behavior.</summary>
    /// <param name="workspaceId">The live workspace identifier.</param>
    /// <param name="revision">The revision exposed by the workspace.</param>
    /// <returns>A configured workspace mock.</returns>
    private static Mock<IFormListWorkspace> CreateWorkspace(Guid workspaceId, WorkspaceRevision revision)
    {
        var workspace = new Mock<IFormListWorkspace>();
        workspace.SetupGet(candidate => candidate.WorkspaceId).Returns(workspaceId);
        workspace.SetupGet(candidate => candidate.Revision).Returns(revision);
        workspace.Setup(candidate => candidate.DisposeAsync()).Returns(ValueTask.CompletedTask);
        return workspace;
    }

    /// <summary>Publishes a mock workspace into the registry.</summary>
    /// <param name="registry">The registry that takes ownership.</param>
    /// <param name="workspace">The workspace to publish.</param>
    /// <param name="workspaceId">The exact workspace identifier.</param>
    /// <returns>A task that completes after registry publication.</returns>
    private static async Task OpenRegistryAsync(McpWorkspaceRegistry registry, IFormListWorkspace workspace, Guid workspaceId)
    {
        var factory = new Mock<IFormListWorkspaceFactory>();
        factory.Setup(candidate => candidate.OpenAsync(It.IsAny<WorkspaceOpenRequest>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<IFormListWorkspace>.Success(workspace)));
        var request = new WorkspaceOpenRequest(
            workspaceId,
            SupportedGame.Starfield,
            GameRelease.Starfield,
            Path.Combine(Path.GetTempPath(), "Source.esm"),
            [],
            Path.GetTempPath(),
            []);
        (await registry.OpenAsync(factory.Object, request, TestContext.Current.CancellationToken)).Succeeded.ShouldBeTrue();
    }

    /// <summary>Verifies two output associations preserve every exact field relevant to recovery.</summary>
    /// <param name="actual">The reconstructed association received by Core.</param>
    /// <param name="expected">The persisted caller association.</param>
    private static void AssertOutput(OutputAssociation actual, OutputAssociation expected)
    {
        actual.PluginPath.ShouldBe(expected.PluginPath);
        actual.ModKey.ShouldBe(expected.ModKey);
        actual.LocalizedOutputMode.ShouldBe(expected.LocalizedOutputMode);
        actual.MasterStyle.ShouldBe(expected.MasterStyle);
    }

    /// <summary>Gets a successful tool-specific result.</summary>
    /// <param name="result">The SDK tool result.</param>
    /// <returns>The nested successful result object.</returns>
    private static JsonElement GetResult(CallToolResult result)
    {
        result.IsError.ShouldNotBe(true);
        return result.StructuredContent.ShouldNotBeNull().GetProperty("result");
    }

    /// <summary>Gets the stable code from a failed tool result.</summary>
    /// <param name="result">The SDK tool result.</param>
    /// <returns>The stable error code.</returns>
    private static string GetErrorCode(CallToolResult result)
    {
        result.IsError.ShouldBe(true);
        return result.StructuredContent.ShouldNotBeNull()
            .GetProperty("error")
            .GetProperty("code")
            .GetString()
            .ShouldNotBeNull();
    }

    /// <summary>Owns a real in-process MCP SDK stream server and client.</summary>
    private sealed class ProtocolHarness : IAsyncDisposable
    {
        /// <summary>The running host.</summary>
        private readonly IHost Host;

        /// <summary>Initializes the protocol harness.</summary>
        /// <param name="host">The running server host.</param>
        /// <param name="client">The connected SDK client.</param>
        private ProtocolHarness(IHost host, McpClient client)
        {
            Host = host;
            Client = client;
        }

        /// <summary>Gets the connected client.</summary>
        internal McpClient Client { get; }

        /// <summary>Starts the in-memory SDK server and client.</summary>
        /// <param name="tools">The exact tools to register.</param>
        /// <returns>The connected owned harness.</returns>
        internal static async Task<ProtocolHarness> CreateAsync(IReadOnlyList<McpServerTool> tools)
        {
            var clientToServer = new Pipe();
            var serverToClient = new Pipe();
            var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();
            builder.Logging.ClearProviders();
            builder.Services.AddMcpServer()
                .WithStreamServerTransport(clientToServer.Reader.AsStream(), serverToClient.Writer.AsStream())
                .WithTools(tools);
            var host = builder.Build();
            try
            {
                await host.StartAsync(TestContext.Current.CancellationToken);
                var client = await McpClient.CreateAsync(
                    new StreamClientTransport(clientToServer.Writer.AsStream(), serverToClient.Reader.AsStream()),
                    cancellationToken: TestContext.Current.CancellationToken);
                return new ProtocolHarness(host, client);
            }
            catch
            {
                host.Dispose();
                throw;
            }
        }

        /// <summary>Disposes the client before stopping the host.</summary>
        /// <returns>A task that completes after both endpoints stop.</returns>
        public async ValueTask DisposeAsync()
        {
            await Client.DisposeAsync();
            await Host.StopAsync(TestContext.Current.CancellationToken);
            Host.Dispose();
        }
    }
}
