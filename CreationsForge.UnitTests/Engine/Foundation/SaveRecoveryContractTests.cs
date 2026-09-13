using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Enums;
using Moq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Foundation;

/// <summary>Verifies that save and recovery envelopes cannot contradict their nested evidence or artifact disposition.</summary>
public sealed class SaveRecoveryContractTests
{
    /// <summary>Verifies a result cannot report uncommitted or unknown files while attaching committed adoption evidence.</summary>
    /// <param name="status">The save outcome that contradicts committed evidence.</param>
    [Theory]
    [InlineData(SaveCommitStatus.NotCommitted)]
    [InlineData(SaveCommitStatus.CommitOutcomeUnknown)]
    public void SaveResult_WithCommittedEvidenceAndContradictoryStatus_RejectsEnvelope(SaveCommitStatus status)
    {
        var evidence = CreateEvidence(RecoverSaveStatus.Committed);

        Should.Throw<ArgumentException>(() => new SaveResult(
            evidence.OriginalWorkspaceId,
            evidence.SaveOperationId,
            evidence.SaveBaseRevision,
            evidence.SaveBaseRevision,
            status,
            evidence.ResolvedOutputBaseline,
            evidence.EvidenceToken,
            evidence,
            null,
            []));
    }

    /// <summary>Verifies known historical commitment remains expressible when a later file change prevents current adoption.</summary>
    /// <param name="status">The proven original save outcome.</param>
    [Theory]
    [InlineData(RecoverSaveStatus.Committed)]
    [InlineData(RecoverSaveStatus.NotCommitted)]
    public void RecoverSaveResult_WithHistoricalOutcomeAndCurrentConflict_PreservesKnowledge(RecoverSaveStatus status)
    {
        var workspaceId = Guid.NewGuid();
        var saveId = Guid.NewGuid();
        var originalRevision = new WorkspaceRevision(Guid.NewGuid(), 7);
        var conflict = new EngineError(
            EngineErrorCode.ExternalChangeDetected,
            "A later save changed the output after the original transaction completed.");

        var result = new RecoverSaveResult(
            workspaceId, saveId, status, originalRevision, false, null, null, conflict);

        result.Status.ShouldBe(status);
        result.SaveBaseRevision.ShouldBe(originalRevision);
        result.RepairRequired.ShouldBeFalse();
        result.ResolvedEvidence.ShouldBeNull();
        result.Error.ShouldBeSameAs(conflict);
        Should.Throw<ArgumentException>(() => new RecoverSaveResult(
            workspaceId, saveId, status, originalRevision, false, null, null, null));
    }

    /// <summary>Verifies unknown recovery cannot masquerade as terminal evidence or lose the original revision required for repair.</summary>
    [Fact]
    public void RecoverSaveResult_WithUnknownOutcome_SeparatesMissingEvidenceFromRepair()
    {
        var evidence = CreateEvidence(RecoverSaveStatus.Committed);
        var missing = new RecoverSaveResult(
            evidence.OriginalWorkspaceId,
            evidence.SaveOperationId,
            RecoverSaveStatus.StillUnknown,
            null,
            false,
            null,
            null,
            new EngineError(EngineErrorCode.NoRecoveryEvidence, "No recognized guard or transaction exists."));

        missing.SaveBaseRevision.ShouldBeNull();
        missing.EvidenceToken.ShouldBeNull();
        missing.RepairRequired.ShouldBeFalse();
        Should.Throw<ArgumentException>(() => new RecoverSaveResult(
            evidence.OriginalWorkspaceId,
            evidence.SaveOperationId,
            RecoverSaveStatus.StillUnknown,
            evidence.SaveBaseRevision,
            false,
            evidence.EvidenceToken,
            evidence,
            null));
        Should.Throw<ArgumentException>(() => new RecoverSaveResult(
            evidence.OriginalWorkspaceId,
            evidence.SaveOperationId,
            RecoverSaveStatus.StillUnknown,
            null,
            true,
            evidence.EvidenceToken,
            null,
            new EngineError(EngineErrorCode.RepairRequired, "The transaction is incomplete.")));
    }

    /// <summary>Verifies callers cannot combine one save's evidence with another save identity, revision, or token.</summary>
    [Fact]
    public void RecoverSaveResult_WithMismatchedNestedEvidence_RejectsEnvelope()
    {
        var evidence = CreateEvidence(RecoverSaveStatus.Committed);

        Should.Throw<ArgumentException>(() => CreateRecoveryResult(
            evidence, workspaceId: Guid.NewGuid()));
        Should.Throw<ArgumentException>(() => CreateRecoveryResult(
            evidence, saveId: Guid.NewGuid()));
        Should.Throw<ArgumentException>(() => CreateRecoveryResult(
            evidence, revision: new WorkspaceRevision(evidence.SaveBaseRevision.BaselineId, 8)));
        Should.Throw<ArgumentException>(() => CreateRecoveryResult(
            evidence, token: new RecoveryEvidenceToken("another-observation")));
    }

    /// <summary>Verifies a completed restore cannot carry evidence that the prepared new output committed.</summary>
    [Fact]
    public void RepairSaveResult_WithOppositeTerminalEvidence_RejectsEnvelope()
    {
        var evidence = CreateEvidence(RecoverSaveStatus.Committed);

        Should.Throw<ArgumentException>(() => new RepairSaveResult(
            evidence.OriginalWorkspaceId,
            evidence.SaveOperationId,
            Guid.NewGuid(),
            RepairSaveStatus.BaselineRestored,
            evidence.ResolvedOutputBaseline,
            evidence.EvidenceToken,
            evidence,
            null));
    }

    /// <summary>Verifies unchanged plugin validation cannot smuggle destination mutations into a no-op save.</summary>
    [Fact]
    public void StagedPluginOutputSet_WithUnchangedDisposition_RequiresNoMutationPayload()
    {
        var stagePath = Path.GetFullPath(Path.Combine("stage", "Output.esm"));
        var destinationPath = Path.GetFullPath("Output.esm");
        var stage = new PluginArtifactAssociation(
            stagePath,
            PluginArtifactRole.Plugin,
            null,
            new PluginArtifactFingerprint(true, 1, new string('A', 64)));
        var mapping = new StagedPluginArtifactMapping(stage, destinationPath);
        var stagedLifetime = Mock.Of<IStagedPluginOutputSet>();

        var unchanged = new StagedPluginOutputSet(PluginWriteDisposition.Unchanged, null, []);

        unchanged.StagedOutput.ShouldBeNull();
        unchanged.ArtifactMappings.ShouldBeEmpty();
        Should.Throw<ArgumentException>(() => new StagedPluginOutputSet(
            PluginWriteDisposition.Unchanged, null, [mapping]));
        Should.Throw<ArgumentException>(() => new StagedPluginOutputSet(
            PluginWriteDisposition.Unchanged, stagedLifetime, []));
        Should.Throw<ArgumentException>(() => new StagedPluginOutputSet(
            PluginWriteDisposition.StagedChanges, stagedLifetime, []));
    }

    /// <summary>Creates a terminal test envelope with optional conflicting outer identity fields.</summary>
    /// <param name="evidence">The nested terminal evidence.</param>
    /// <param name="workspaceId">An optional replacement original workspace identity.</param>
    /// <param name="saveId">An optional replacement original save operation identity.</param>
    /// <param name="revision">An optional replacement original save revision.</param>
    /// <param name="token">An optional replacement observation token.</param>
    /// <returns>The envelope when its outer and nested evidence agree.</returns>
    private static RecoverSaveResult CreateRecoveryResult(
        ResolvedOutputEvidence evidence,
        Guid? workspaceId = null,
        Guid? saveId = null,
        WorkspaceRevision? revision = null,
        RecoveryEvidenceToken? token = null)
    {
        return new RecoverSaveResult(
            workspaceId ?? evidence.OriginalWorkspaceId,
            saveId ?? evidence.SaveOperationId,
            evidence.Status,
            revision ?? evidence.SaveBaseRevision,
            false,
            token ?? evidence.EvidenceToken,
            evidence,
            null);
    }

    /// <summary>Creates immutable metadata for a hypothetical terminal save without touching the filesystem.</summary>
    /// <param name="status">The terminal commitment state to model.</param>
    /// <returns>Complete plugin-record-free metadata for testing envelope consistency only.</returns>
    private static ResolvedOutputEvidence CreateEvidence(RecoverSaveStatus status)
    {
        var output = new OutputAssociation(
            Path.GetFullPath("Output.esm"),
            ModKey.FromNameAndExtension("Output.esm"),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
        var outputBaseline = new OutputArtifactSetBaseline(
            Guid.NewGuid(),
            [new PluginArtifactAssociation(
                output.PluginPath,
                PluginArtifactRole.Plugin,
                null,
                new PluginArtifactFingerprint(true, 1, new string('A', 64)))]);

        return new ResolvedOutputEvidence(
            new RecoveryEvidenceToken("verified-test-observation"),
            SupportedGame.Fallout4,
            GameRelease.Fallout4,
            Guid.NewGuid(),
            Guid.NewGuid(),
            new WorkspaceRevision(Guid.NewGuid(), 7),
            new PluginSourceInputBaseline(Guid.NewGuid(), []),
            output,
            outputBaseline,
            status);
    }
}
