using System.Globalization;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Mcp;

/// <summary>Projects exact retained Core metadata to transient stable MCP JSON views.</summary>
internal static class McpMetadataProjection
{
    /// <summary>Projects one metadata reference without exposing or copying its retained value.</summary>
    /// <param name="reference">The opaque host-local descriptor.</param>
    /// <returns>A closed small reference object.</returns>
    internal static object Reference(McpMetadataReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        return new
        {
            handle = reference.Handle,
            kind = Kind(reference.Kind),
            baselineId = reference.BaselineId?.ToString("D"),
        };
    }

    /// <summary>Projects the complete retained object for transient bounded paging.</summary>
    /// <param name="value">The exact retained Core metadata object.</param>
    /// <returns>A detached JSON view preserving order, nullability, and lossless integer strings.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown for an unsupported retained metadata type.</exception>
    internal static JsonElement Value(object value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var projected = value switch
        {
            OutputAssociation output => Output(output),
            OutputArtifactSetBaseline baseline => OutputBaseline(baseline),
            ResolvedOutputEvidence evidence => ResolvedEvidence(evidence),
            SaveResult save => Save(save),
            RecoverSaveResult recovery => Recovery(recovery),
            RepairSaveResult repair => Repair(repair),
            _ => throw new ArgumentOutOfRangeException(nameof(value), "The retained metadata type cannot be projected."),
        };
        return JsonSerializer.SerializeToElement(projected);
    }

    /// <summary>Returns the stable wire name for a metadata kind.</summary>
    /// <param name="kind">The closed metadata kind.</param>
    /// <returns>The lower-snake-case wire name.</returns>
    internal static string Kind(McpMetadataKind kind)
    {
        return kind switch
        {
            McpMetadataKind.OutputAssociation => "output_association",
            McpMetadataKind.OutputBaseline => "output_baseline",
            McpMetadataKind.ResolvedOutputEvidence => "resolved_output_evidence",
            McpMetadataKind.SaveResult => "save_result",
            McpMetadataKind.RecoverSaveResult => "recover_save_result",
            McpMetadataKind.RepairSaveResult => "repair_save_result",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }

    /// <summary>Projects an exact output association.</summary>
    /// <param name="output">The immutable association.</param>
    /// <returns>A closed output object.</returns>
    internal static object Output(OutputAssociation output)
    {
        ArgumentNullException.ThrowIfNull(output);
        return new
        {
            pluginPath = output.PluginPath,
            modKey = output.ModKey.ToString(),
            localizedOutputMode = LocalizedOutputMode(output.LocalizedOutputMode),
            masterStyle = MasterStyle(output.MasterStyle),
        };
    }

    /// <summary>Projects a complete output artifact baseline without omitting absent sidecars.</summary>
    /// <param name="baseline">The immutable ordered baseline.</param>
    /// <returns>A closed baseline object.</returns>
    internal static object OutputBaseline(OutputArtifactSetBaseline baseline)
    {
        ArgumentNullException.ThrowIfNull(baseline);
        return new
        {
            baselineId = baseline.BaselineId.ToString("D"),
            artifacts = baseline.Artifacts.Select(Artifact).ToArray(),
        };
    }

    /// <summary>Projects terminal recovery evidence with its complete source and resolved output baselines.</summary>
    /// <param name="evidence">The immutable evidence.</param>
    /// <returns>A closed evidence object.</returns>
    private static object ResolvedEvidence(ResolvedOutputEvidence evidence)
    {
        return new
        {
            evidenceToken = evidence.EvidenceToken.Value,
            game = Game(evidence.Game),
            release = Release(evidence.Release),
            originalWorkspaceId = evidence.OriginalWorkspaceId.ToString("D"),
            saveOperationId = evidence.SaveOperationId.ToString("D"),
            saveBaseRevision = McpProjection.Revision(evidence.SaveBaseRevision),
            sourceBaseline = SourceBaseline(evidence.SourceBaseline),
            output = Output(evidence.Output),
            resolvedOutputBaseline = OutputBaseline(evidence.ResolvedOutputBaseline),
            status = RecoverStatus(evidence.Status),
        };
    }

    /// <summary>Projects complete save details.</summary>
    /// <param name="save">The immutable save outcome.</param>
    /// <returns>A closed save-result object.</returns>
    private static object Save(SaveResult save)
    {
        return new
        {
            workspaceId = save.WorkspaceId.ToString("D"),
            operationId = save.OperationId.ToString("D"),
            baseRevision = McpProjection.Revision(save.BaseRevision),
            resultRevision = McpProjection.Revision(save.ResultRevision),
            status = SaveStatus(save.Status),
            committedBaseline = save.CommittedBaseline is null ? null : OutputBaseline(save.CommittedBaseline),
            recoveryEvidenceToken = save.RecoveryEvidenceToken?.Value,
            resolvedEvidence = save.ResolvedEvidence is null ? null : ResolvedEvidence(save.ResolvedEvidence),
            error = Error(save.Error),
            warnings = save.Warnings.Select(McpProjection.Warning).ToArray(),
        };
    }

    /// <summary>Projects complete recovery details.</summary>
    /// <param name="recovery">The immutable recovery observation.</param>
    /// <returns>A closed recovery-result object.</returns>
    private static object Recovery(RecoverSaveResult recovery)
    {
        return new
        {
            workspaceId = recovery.WorkspaceId.ToString("D"),
            saveOperationId = recovery.SaveOperationId.ToString("D"),
            status = RecoverStatus(recovery.Status),
            saveBaseRevision = recovery.SaveBaseRevision.HasValue
                ? McpProjection.Revision(recovery.SaveBaseRevision.Value)
                : null,
            repairRequired = recovery.RepairRequired,
            evidenceToken = recovery.EvidenceToken?.Value,
            resolvedEvidence = recovery.ResolvedEvidence is null ? null : ResolvedEvidence(recovery.ResolvedEvidence),
            error = Error(recovery.Error),
        };
    }

    /// <summary>Projects complete repair details.</summary>
    /// <param name="repair">The immutable repair outcome.</param>
    /// <returns>A closed repair-result object.</returns>
    private static object Repair(RepairSaveResult repair)
    {
        return new
        {
            workspaceId = repair.WorkspaceId.ToString("D"),
            saveOperationId = repair.SaveOperationId.ToString("D"),
            repairOperationId = repair.RepairOperationId.ToString("D"),
            status = RepairStatus(repair.Status),
            resultingBaseline = repair.ResultingBaseline is null ? null : OutputBaseline(repair.ResultingBaseline),
            evidenceToken = repair.EvidenceToken?.Value,
            resolvedEvidence = repair.ResolvedEvidence is null ? null : ResolvedEvidence(repair.ResolvedEvidence),
            error = Error(repair.Error),
        };
    }

    /// <summary>Projects a complete source baseline in its existing engine order.</summary>
    /// <param name="baseline">The immutable source baseline.</param>
    /// <returns>A closed source-baseline object.</returns>
    private static object SourceBaseline(PluginSourceInputBaseline baseline)
    {
        return new
        {
            baselineId = baseline.BaselineId.ToString("D"),
            artifacts = baseline.Artifacts.Select(Artifact).ToArray(),
        };
    }

    /// <summary>Projects one exact artifact, preserving nullable identity fields and 64-bit values as decimal strings.</summary>
    /// <param name="artifact">The artifact association.</param>
    /// <returns>A closed artifact object.</returns>
    private static object Artifact(PluginArtifactAssociation artifact)
    {
        return new
        {
            path = artifact.Path,
            role = ArtifactRole(artifact.Role),
            language = artifact.Language,
            fingerprint = new
            {
                exists = artifact.Fingerprint.Exists,
                length = artifact.Fingerprint.Length.ToString(CultureInfo.InvariantCulture),
                sha256 = artifact.Fingerprint.Sha256,
            },
            fileIdentity = artifact.FileIdentity is null
                ? null
                : new
                {
                    provider = artifact.FileIdentity.Provider,
                    volumeId = artifact.FileIdentity.VolumeId,
                    fileId = artifact.FileIdentity.FileId,
                    linkCount = artifact.FileIdentity.LinkCount?.ToString(CultureInfo.InvariantCulture),
                },
        };
    }

    /// <summary>Projects an optional engine error without truncating its retained detail.</summary>
    /// <param name="error">The error, or <see langword="null"/>.</param>
    /// <returns>A closed error object or <see langword="null"/>.</returns>
    private static object? Error(EngineError? error)
    {
        return error is null
            ? null
            : new
            {
                code = McpProjection.ErrorCode(error.Code),
                message = error.Message,
            };
    }

    /// <summary>Returns the stable game name.</summary>
    /// <param name="game">The supported CreationsForge game.</param>
    /// <returns>The lower-snake-case wire name.</returns>
    internal static string Game(SupportedGame game)
    {
        return game switch
        {
            SupportedGame.Starfield => "starfield",
            SupportedGame.Fallout4 => "fallout4",
            SupportedGame.Skyrim => "skyrim",
            _ => throw new ArgumentOutOfRangeException(nameof(game)),
        };
    }

    /// <summary>Returns the stable engine release name.</summary>
    /// <param name="release">The exact engine release.</param>
    /// <returns>The lower-snake-case wire name.</returns>
    internal static string Release(GameRelease release)
    {
        return release switch
        {
            GameRelease.Starfield => "starfield",
            GameRelease.Fallout4 => "fallout4",
            GameRelease.SkyrimSE => "skyrim_se",
            _ => throw new ArgumentOutOfRangeException(nameof(release)),
        };
    }

    /// <summary>Returns the stable localized-output mode.</summary>
    /// <param name="mode">The engine localized-output mode.</param>
    /// <returns>The lower-snake-case wire name.</returns>
    private static string LocalizedOutputMode(CreationsForge.Core.Engine.Contracts.LocalizedOutputMode mode)
    {
        return mode switch
        {
            CreationsForge.Core.Engine.Contracts.LocalizedOutputMode.Embedded => "embedded",
            CreationsForge.Core.Engine.Contracts.LocalizedOutputMode.SeparateStringFiles => "separate_string_files",
            _ => throw new ArgumentOutOfRangeException(nameof(mode)),
        };
    }

    /// <summary>Returns the stable output master style.</summary>
    /// <param name="style">The plugin output master style.</param>
    /// <returns>The lower-snake-case wire name.</returns>
    private static string MasterStyle(OutputMasterStyle style)
    {
        return style switch
        {
            OutputMasterStyle.Full => "full",
            OutputMasterStyle.Small => "small",
            OutputMasterStyle.Medium => "medium",
            _ => throw new ArgumentOutOfRangeException(nameof(style)),
        };
    }

    /// <summary>Returns the stable artifact role.</summary>
    /// <param name="role">The plugin artifact role.</param>
    /// <returns>The lower-snake-case wire name.</returns>
    private static string ArtifactRole(PluginArtifactRole role)
    {
        return role switch
        {
            PluginArtifactRole.Plugin => "plugin",
            PluginArtifactRole.Strings => "strings",
            PluginArtifactRole.DlStrings => "dlstrings",
            PluginArtifactRole.IlStrings => "ilstrings",
            PluginArtifactRole.StringsArchive => "strings_archive",
            _ => throw new ArgumentOutOfRangeException(nameof(role)),
        };
    }

    /// <summary>Returns the stable save commitment status.</summary>
    /// <param name="status">The Core save status.</param>
    /// <returns>The lower-snake-case wire name.</returns>
    internal static string SaveStatus(SaveCommitStatus status)
    {
        return status switch
        {
            SaveCommitStatus.Committed => "committed",
            SaveCommitStatus.NotCommitted => "not_committed",
            SaveCommitStatus.CommitOutcomeUnknown => "commit_outcome_unknown",
            SaveCommitStatus.CommittedButReopenFailed => "committed_but_reopen_failed",
            _ => throw new ArgumentOutOfRangeException(nameof(status)),
        };
    }

    /// <summary>Returns the stable recovery status.</summary>
    /// <param name="status">The Core recovery status.</param>
    /// <returns>The lower-snake-case wire name.</returns>
    internal static string RecoverStatus(RecoverSaveStatus status)
    {
        return status switch
        {
            RecoverSaveStatus.Committed => "committed",
            RecoverSaveStatus.NotCommitted => "not_committed",
            RecoverSaveStatus.StillUnknown => "still_unknown",
            _ => throw new ArgumentOutOfRangeException(nameof(status)),
        };
    }

    /// <summary>Returns the stable repair status.</summary>
    /// <param name="status">The Core repair status.</param>
    /// <returns>The lower-snake-case wire name.</returns>
    internal static string RepairStatus(RepairSaveStatus status)
    {
        return status switch
        {
            RepairSaveStatus.PreparedSetCompleted => "prepared_set_completed",
            RepairSaveStatus.BaselineRestored => "baseline_restored",
            RepairSaveStatus.BlockedByExternalChange => "blocked_by_external_change",
            RepairSaveStatus.NotStarted => "not_started",
            RepairSaveStatus.StillUnknown => "still_unknown",
            _ => throw new ArgumentOutOfRangeException(nameof(status)),
        };
    }

    /// <summary>Returns the stable synchronization status.</summary>
    /// <param name="status">The Core synchronization status.</param>
    /// <returns>The lower-snake-case wire name.</returns>
    internal static string SynchronizationStatus(OutputSynchronizationStatus status)
    {
        return status switch
        {
            OutputSynchronizationStatus.Ready => "ready",
            OutputSynchronizationStatus.RecoveryRequired => "recovery_required",
            OutputSynchronizationStatus.ReopenRequired => "reopen_required",
            _ => throw new ArgumentOutOfRangeException(nameof(status)),
        };
    }

    /// <summary>Returns the stable recovery-adoption mode.</summary>
    /// <param name="mode">The Core recovery-adoption mode.</param>
    /// <returns>The lower-snake-case wire name.</returns>
    internal static string AdoptionMode(OutputRecoveryAdoptionMode mode)
    {
        return mode switch
        {
            OutputRecoveryAdoptionMode.ResumeStagedAfterNotCommitted => "resume_staged_after_not_committed",
            OutputRecoveryAdoptionMode.ReopenResolvedOutput => "reopen_resolved_output",
            _ => throw new ArgumentOutOfRangeException(nameof(mode)),
        };
    }
}
