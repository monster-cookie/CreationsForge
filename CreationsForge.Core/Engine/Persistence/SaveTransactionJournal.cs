using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Describes the durable phase of one guarded plugin save transaction.</summary>
internal enum SaveTransactionPhase
{
    /// <summary>The request and original baselines are durable, but staging is not yet complete.</summary>
    Preparing,

    /// <summary>The complete staged set, publish files, backups, and mutation plan are durable.</summary>
    Prepared,

    /// <summary>At least one destination mutation may have occurred.</summary>
    MutationStarted,

    /// <summary>The prepared output set was completely published and verified.</summary>
    Committed,

    /// <summary>The original output set is known to remain or has been explicitly restored.</summary>
    NotCommitted
}

/// <summary>Records one immutable repair-operation decision and its last durable outcome.</summary>
internal sealed class SaveRepairAttempt
{
    /// <summary>Initializes an immutable repair attempt.</summary>
    /// <param name="operationId">The repair idempotency identifier.</param>
    /// <param name="requestFingerprint">The canonical repair request digest.</param>
    /// <param name="direction">The caller's explicit repair direction.</param>
    /// <param name="status">The last durable repair status, or <see langword="null"/> while the attempt may be in progress.</param>
    /// <param name="artifactPlans">The exact durable per-artifact repair plan.</param>
    internal SaveRepairAttempt(
        Guid operationId,
        string requestFingerprint,
        RepairSaveDirection direction,
        RepairSaveStatus? status,
        IReadOnlyList<SaveRepairArtifactPlan> artifactPlans)
    {
        OperationId = operationId;
        RequestFingerprint = requestFingerprint;
        Direction = direction;
        Status = status;
        ArtifactPlans = Array.AsReadOnly(artifactPlans.ToArray());
    }

    /// <summary>Gets the repair idempotency identifier.</summary>
    internal Guid OperationId { get; }

    /// <summary>Gets the canonical repair request digest.</summary>
    internal string RequestFingerprint { get; }

    /// <summary>Gets the caller's explicit repair direction.</summary>
    internal RepairSaveDirection Direction { get; }

    /// <summary>Gets the last durable repair status, or <see langword="null"/> while the attempt may be in progress.</summary>
    internal RepairSaveStatus? Status { get; }

    /// <summary>Gets the exact durable per-artifact repair plan.</summary>
    internal IReadOnlyList<SaveRepairArtifactPlan> ArtifactPlans { get; }
}

/// <summary>Records one destination artifact's exact state transition for an explicit repair.</summary>
internal sealed class SaveRepairArtifactPlan
{
    /// <summary>Initializes an immutable repair artifact plan.</summary>
    /// <param name="current">The exact recognized destination state before this repair mutation.</param>
    /// <param name="target">The exact destination state after this repair mutation.</param>
    /// <param name="publish">The flushed repair publication file, or <see langword="null"/> when no present file is published.</param>
    /// <param name="retired">The exact transaction-owned retirement result, or <see langword="null"/> when no present file is retired.</param>
    internal SaveRepairArtifactPlan(
        PluginArtifactAssociation current,
        PluginArtifactAssociation target,
        PluginArtifactAssociation? publish,
        PluginArtifactAssociation? retired)
    {
        Current = current;
        Target = target;
        Publish = publish;
        Retired = retired;
    }

    /// <summary>Gets the exact recognized destination state before this repair mutation.</summary>
    internal PluginArtifactAssociation Current { get; }

    /// <summary>Gets the exact destination state after this repair mutation.</summary>
    internal PluginArtifactAssociation Target { get; }

    /// <summary>Gets the flushed repair publication file, or <see langword="null"/> when no present file is published.</summary>
    internal PluginArtifactAssociation? Publish { get; }

    /// <summary>Gets the exact transaction-owned retirement result, or <see langword="null"/> when no present file is retired.</summary>
    internal PluginArtifactAssociation? Retired { get; }
}

/// <summary>Records one staged-to-destination publication plan with independently observed owned files.</summary>
internal sealed class SaveArtifactPlan
{
    /// <summary>Initializes an immutable artifact publication plan.</summary>
    /// <param name="before">The exact destination observation before the save.</param>
    /// <param name="staged">The exact adapter-staged observation, including intended absence.</param>
    /// <param name="publish">The flushed same-volume publication file, or <see langword="null"/> for an intended deletion.</param>
    /// <param name="backup">The flushed transaction-owned copy of prior bytes, or <see langword="null"/> when the destination was absent.</param>
    /// <param name="retiredPath">The transaction-owned path used for an atomic removal from the destination namespace.</param>
    internal SaveArtifactPlan(
        PluginArtifactAssociation before,
        PluginArtifactAssociation staged,
        PluginArtifactAssociation? publish,
        PluginArtifactAssociation? backup,
        string retiredPath)
    {
        Before = before;
        Staged = staged;
        Publish = publish;
        Backup = backup;
        RetiredPath = retiredPath;
    }

    /// <summary>Gets the exact destination observation before the save.</summary>
    internal PluginArtifactAssociation Before { get; }

    /// <summary>Gets the exact adapter-staged observation, including intended absence.</summary>
    internal PluginArtifactAssociation Staged { get; }

    /// <summary>Gets the flushed same-volume publication file, or <see langword="null"/> for an intended deletion.</summary>
    internal PluginArtifactAssociation? Publish { get; }

    /// <summary>Gets the flushed transaction-owned prior-byte copy, or <see langword="null"/> when originally absent.</summary>
    internal PluginArtifactAssociation? Backup { get; }

    /// <summary>Gets the transaction-owned path used for an atomic removal from the destination namespace.</summary>
    internal string RetiredPath { get; }
}

/// <summary>Contains one complete immutable snapshot of a guarded plugin save transaction.</summary>
internal sealed class SaveTransactionJournal
{
    /// <summary>The supported on-disk journal version.</summary>
    internal const int CurrentVersion = 2;

    /// <summary>Initializes a complete immutable journal snapshot.</summary>
    /// <param name="workspaceId">The original workspace identifier.</param>
    /// <param name="saveOperationId">The original save operation identifier.</param>
    /// <param name="requestFingerprint">The canonical original save request digest.</param>
    /// <param name="saveBaseRevision">The exact original workspace revision.</param>
    /// <param name="game">The exact supported game.</param>
    /// <param name="release">The exact plugin release.</param>
    /// <param name="sourceBaseline">The complete immutable source baseline.</param>
    /// <param name="output">The full output association.</param>
    /// <param name="beforeBaseline">The complete output set before the save.</param>
    /// <param name="disposition">Whether the adapter proved a whole-output no-op or staged changes.</param>
    /// <param name="phase">The last durable transaction phase.</param>
    /// <param name="mutationProgress">The number of destination plans durably recorded as processed.</param>
    /// <param name="artifactPlans">The complete prepared publication plan, empty only while preparing or for a no-op.</param>
    /// <param name="terminalBaseline">The freshly observed terminal baseline, when known.</param>
    /// <param name="repairAttempts">The bounded immutable repair-attempt history.</param>
    internal SaveTransactionJournal(
        Guid workspaceId,
        Guid saveOperationId,
        string requestFingerprint,
        WorkspaceRevision saveBaseRevision,
        SupportedGame game,
        GameRelease release,
        PluginSourceInputBaseline sourceBaseline,
        OutputAssociation output,
        OutputArtifactSetBaseline beforeBaseline,
        PluginWriteDisposition disposition,
        SaveTransactionPhase phase,
        int mutationProgress,
        IReadOnlyList<SaveArtifactPlan> artifactPlans,
        OutputArtifactSetBaseline? terminalBaseline,
        IReadOnlyList<SaveRepairAttempt> repairAttempts)
    {
        WorkspaceId = workspaceId;
        SaveOperationId = saveOperationId;
        RequestFingerprint = requestFingerprint;
        SaveBaseRevision = saveBaseRevision;
        Game = game;
        Release = release;
        SourceBaseline = sourceBaseline;
        Output = output;
        BeforeBaseline = beforeBaseline;
        Disposition = disposition;
        Phase = phase;
        MutationProgress = mutationProgress;
        ArtifactPlans = Array.AsReadOnly(artifactPlans.ToArray());
        TerminalBaseline = terminalBaseline;
        RepairAttempts = Array.AsReadOnly(repairAttempts.ToArray());
    }

    /// <summary>Gets the original workspace identifier.</summary>
    internal Guid WorkspaceId { get; }

    /// <summary>Gets the original save operation identifier.</summary>
    internal Guid SaveOperationId { get; }

    /// <summary>Gets the canonical original save request digest.</summary>
    internal string RequestFingerprint { get; }

    /// <summary>Gets the exact original workspace revision.</summary>
    internal WorkspaceRevision SaveBaseRevision { get; }

    /// <summary>Gets the exact supported game.</summary>
    internal SupportedGame Game { get; }

    /// <summary>Gets the exact plugin release.</summary>
    internal GameRelease Release { get; }

    /// <summary>Gets the complete immutable source baseline.</summary>
    internal PluginSourceInputBaseline SourceBaseline { get; }

    /// <summary>Gets the full output association.</summary>
    internal OutputAssociation Output { get; }

    /// <summary>Gets the complete output set before the save.</summary>
    internal OutputArtifactSetBaseline BeforeBaseline { get; }

    /// <summary>Gets whether the adapter proved a whole-output no-op or staged changes.</summary>
    internal PluginWriteDisposition Disposition { get; }

    /// <summary>Gets the last durable transaction phase.</summary>
    internal SaveTransactionPhase Phase { get; }

    /// <summary>Gets the number of destination plans durably recorded as processed.</summary>
    internal int MutationProgress { get; }

    /// <summary>Gets the complete prepared publication plan.</summary>
    internal IReadOnlyList<SaveArtifactPlan> ArtifactPlans { get; }

    /// <summary>Gets the freshly observed terminal baseline, when known.</summary>
    internal OutputArtifactSetBaseline? TerminalBaseline { get; }

    /// <summary>Gets the bounded immutable repair-attempt history.</summary>
    internal IReadOnlyList<SaveRepairAttempt> RepairAttempts { get; }

    /// <summary>Creates a new complete snapshot with updated transaction progress.</summary>
    /// <param name="phase">The new durable phase.</param>
    /// <param name="mutationProgress">The number of processed artifact plans.</param>
    /// <param name="terminalBaseline">The terminal baseline, when the new phase is terminal.</param>
    /// <returns>A complete updated immutable snapshot.</returns>
    internal SaveTransactionJournal WithProgress(
        SaveTransactionPhase phase,
        int mutationProgress,
        OutputArtifactSetBaseline? terminalBaseline = null)
    {
        return new SaveTransactionJournal(
            WorkspaceId,
            SaveOperationId,
            RequestFingerprint,
            SaveBaseRevision,
            Game,
            Release,
            SourceBaseline,
            Output,
            BeforeBaseline,
            Disposition,
            phase,
            mutationProgress,
            ArtifactPlans,
            terminalBaseline,
            RepairAttempts);
    }

    /// <summary>Creates a new complete snapshot after staging and owned-file preparation.</summary>
    /// <param name="disposition">The adapter's complete-output disposition.</param>
    /// <param name="plans">The complete validated artifact plans.</param>
    /// <returns>A prepared immutable snapshot.</returns>
    internal SaveTransactionJournal WithPreparedArtifacts(
        PluginWriteDisposition disposition,
        IReadOnlyList<SaveArtifactPlan> plans)
    {
        return new SaveTransactionJournal(
            WorkspaceId,
            SaveOperationId,
            RequestFingerprint,
            SaveBaseRevision,
            Game,
            Release,
            SourceBaseline,
            Output,
            BeforeBaseline,
            disposition,
            SaveTransactionPhase.Prepared,
            0,
            plans,
            null,
            RepairAttempts);
    }

    /// <summary>Creates a new complete snapshot with a replacement repair-attempt history.</summary>
    /// <param name="repairAttempts">The updated bounded repair attempts.</param>
    /// <returns>A complete updated immutable snapshot.</returns>
    internal SaveTransactionJournal WithRepairAttempts(IReadOnlyList<SaveRepairAttempt> repairAttempts)
    {
        return new SaveTransactionJournal(
            WorkspaceId,
            SaveOperationId,
            RequestFingerprint,
            SaveBaseRevision,
            Game,
            Release,
            SourceBaseline,
            Output,
            BeforeBaseline,
            Disposition,
            Phase,
            MutationProgress,
            ArtifactPlans,
            TerminalBaseline,
            repairAttempts);
    }
}
