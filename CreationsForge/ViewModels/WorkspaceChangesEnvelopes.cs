using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.ViewModels;

/// <summary>Identifies the first incomplete presentation step after a known successful Core transition.</summary>
internal enum WorkspaceRefreshStep
{
    /// <summary>The exact resulting workspace state still needs to be read and validated.</summary>
    ReadState,

    /// <summary>The exact resulting state is accepted and the browser participant still needs to publish it.</summary>
    RefreshParticipant,
}

/// <summary>Identifies the known Core transition whose presentation refresh remains incomplete.</summary>
internal enum WorkspaceRefreshKind
{
    /// <summary>A committed save reopened the output successfully.</summary>
    Save,

    /// <summary>An explicit discard reopened the selected output.</summary>
    Discard,

    /// <summary>Not-committed evidence was adopted while retaining the original staged candidate.</summary>
    RecoveryResume,

    /// <summary>Terminal recovery evidence was adopted by reopening the resolved output.</summary>
    RecoveryReopen,
}

/// <summary>Retains the exact fresh state and preview captured within one coordinator borrow.</summary>
internal sealed class WorkspaceReviewCapture
{
    /// <summary>Initializes one detached review capture.</summary>
    /// <param name="workspaceId">The borrowed workspace identity.</param>
    /// <param name="state">The accepted atomic workspace state.</param>
    /// <param name="preview">The accepted preview at the same revision.</param>
    /// <param name="warnings">All state and preview warnings.</param>
    internal WorkspaceReviewCapture(
        Guid workspaceId,
        WorkspaceState state,
        WorkspacePreview preview,
        IReadOnlyList<EngineWarning> warnings)
    {
        WorkspaceId = workspaceId;
        State = state;
        Preview = preview;
        Warnings = Array.AsReadOnly(warnings.ToArray());
    }

    /// <summary>Gets the borrowed workspace identity.</summary>
    internal Guid WorkspaceId { get; }

    /// <summary>Gets the accepted atomic workspace state.</summary>
    internal WorkspaceState State { get; }

    /// <summary>Gets the accepted preview at the same revision.</summary>
    internal WorkspacePreview Preview { get; }

    /// <summary>Gets all state and preview warnings.</summary>
    internal IReadOnlyList<EngineWarning> Warnings { get; }
}

/// <summary>Retains the complete immutable context used to construct one logical save request.</summary>
internal sealed class WorkspaceSaveEnvelope
{
    /// <summary>Initializes one immutable save request envelope.</summary>
    /// <param name="workspaceId">The exact borrowed workspace identity.</param>
    /// <param name="game">The exact supported game.</param>
    /// <param name="release">The exact plugin release.</param>
    /// <param name="output">The exact selected output.</param>
    /// <param name="preview">The fresh preview used to admit the save.</param>
    /// <param name="request">The exact guarded save request.</param>
    internal WorkspaceSaveEnvelope(
        Guid workspaceId,
        SupportedGame game,
        GameRelease release,
        OutputAssociation output,
        WorkspacePreview preview,
        SaveRequest request)
    {
        WorkspaceId = workspaceId;
        Game = game;
        Release = release;
        Output = output;
        Preview = preview;
        Request = request;
    }

    /// <summary>Gets the exact borrowed workspace identity.</summary>
    internal Guid WorkspaceId { get; }

    /// <summary>Gets the exact supported game.</summary>
    internal SupportedGame Game { get; }

    /// <summary>Gets the exact plugin release.</summary>
    internal GameRelease Release { get; }

    /// <summary>Gets the exact selected output.</summary>
    internal OutputAssociation Output { get; }

    /// <summary>Gets the fresh authoritative preview used to admit the save.</summary>
    internal WorkspacePreview Preview { get; }

    /// <summary>Gets the exact guarded save request.</summary>
    internal SaveRequest Request { get; }
}

/// <summary>Retains a returned save result separately from its best-effort post-result state read.</summary>
internal sealed class WorkspaceSaveBorrowOutcome
{
    /// <summary>Initializes one save borrow outcome.</summary>
    /// <param name="envelope">The exact request envelope.</param>
    /// <param name="result">The exact Core save result.</param>
    /// <param name="postSaveState">The post-result state read or its typed detached failure.</param>
    internal WorkspaceSaveBorrowOutcome(
        WorkspaceSaveEnvelope envelope,
        SaveResult result,
        EngineResult<WorkspaceState> postSaveState)
    {
        Envelope = envelope;
        Result = result;
        PostSaveState = postSaveState;
    }

    /// <summary>Gets the exact request envelope.</summary>
    internal WorkspaceSaveEnvelope Envelope { get; }

    /// <summary>Gets the exact Core save result.</summary>
    internal SaveResult Result { get; }

    /// <summary>Gets the post-result state read or its typed detached failure.</summary>
    internal EngineResult<WorkspaceState> PostSaveState { get; }
}

/// <summary>Retains the complete immutable context used to construct one logical discard request.</summary>
internal sealed class WorkspaceDiscardEnvelope
{
    /// <summary>Initializes one immutable discard request envelope.</summary>
    /// <param name="workspaceId">The exact borrowed workspace identity.</param>
    /// <param name="output">The exact selected output.</param>
    /// <param name="preview">The fresh preview used to admit the discard.</param>
    /// <param name="request">The exact guarded discard request.</param>
    internal WorkspaceDiscardEnvelope(
        Guid workspaceId,
        OutputAssociation output,
        WorkspacePreview preview,
        DiscardChangesRequest request)
    {
        WorkspaceId = workspaceId;
        Output = output;
        Preview = preview;
        Request = request;
    }

    /// <summary>Gets the exact borrowed workspace identity.</summary>
    internal Guid WorkspaceId { get; }

    /// <summary>Gets the exact selected output.</summary>
    internal OutputAssociation Output { get; }

    /// <summary>Gets the fresh authoritative preview used to admit the discard.</summary>
    internal WorkspacePreview Preview { get; }

    /// <summary>Gets the exact guarded discard request.</summary>
    internal DiscardChangesRequest Request { get; }
}

/// <summary>Retains one accepted discard request and its exact Core result.</summary>
internal sealed class WorkspaceDiscardBorrowOutcome
{
    /// <summary>Initializes one discard borrow outcome.</summary>
    /// <param name="capture">The exact fresh state and preview used to decide whether Core discard was needed.</param>
    /// <param name="envelope">The exact request envelope, or <see langword="null"/> when the fresh preview was empty.</param>
    /// <param name="result">The exact Core discard result, or <see langword="null"/> when Core was not called.</param>
    internal WorkspaceDiscardBorrowOutcome(
        WorkspaceReviewCapture capture,
        WorkspaceDiscardEnvelope? envelope,
        EngineResult<OperationReceipt>? result)
    {
        if ((envelope is null) != (result is null))
        {
            throw new ArgumentException("Discard request and result must be present or absent together.", nameof(result));
        }

        Capture = capture;
        Envelope = envelope;
        Result = result;
    }

    /// <summary>Gets the exact fresh state and preview used to decide whether Core discard was needed.</summary>
    internal WorkspaceReviewCapture Capture { get; }

    /// <summary>Gets the exact request envelope, or <see langword="null"/> when Core was not called.</summary>
    internal WorkspaceDiscardEnvelope? Envelope { get; }

    /// <summary>Gets the exact Core discard result, or <see langword="null"/> when Core was not called.</summary>
    internal EngineResult<OperationReceipt>? Result { get; }
}

/// <summary>Retains the exact blocked workspace state and latest recovery evidence for one original save.</summary>
internal sealed class WorkspaceRecoveryEnvelope
{
    /// <summary>Initializes one immutable recovery presentation envelope.</summary>
    /// <param name="liveWorkspaceId">The current live workspace identity.</param>
    /// <param name="state">The exact blocked workspace state used for recovery inspection.</param>
    /// <param name="result">The exact recovery result.</param>
    /// <param name="repairResult">The most recent exact repair result, when repair was attempted.</param>
    /// <param name="canResumeStaged">Whether every presentation-side original-workspace resume precondition was proved.</param>
    internal WorkspaceRecoveryEnvelope(
        Guid liveWorkspaceId,
        WorkspaceState state,
        RecoverSaveResult result,
        RepairSaveResult? repairResult,
        bool canResumeStaged)
    {
        LiveWorkspaceId = liveWorkspaceId;
        State = state;
        Result = result;
        RepairResult = repairResult;
        CanResumeStaged = canResumeStaged;
    }

    /// <summary>Gets the current live workspace identity.</summary>
    internal Guid LiveWorkspaceId { get; }

    /// <summary>Gets the exact blocked workspace state used for inspection.</summary>
    internal WorkspaceState State { get; }

    /// <summary>Gets the exact recovery result.</summary>
    internal RecoverSaveResult Result { get; }

    /// <summary>Gets the most recent exact repair result, when available.</summary>
    internal RepairSaveResult? RepairResult { get; }

    /// <summary>Gets whether every presentation-side original-workspace resume precondition was proved.</summary>
    internal bool CanResumeStaged { get; }
}

/// <summary>Retains an exact repair request whose response was not received.</summary>
internal sealed class WorkspaceRepairEnvelope
{
    /// <summary>Initializes one immutable repair replay envelope.</summary>
    /// <param name="request">The exact repair request.</param>
    /// <param name="sourceRecovery">The reviewed recovery state that authorized the request.</param>
    internal WorkspaceRepairEnvelope(
        RepairSaveRequest request,
        WorkspaceRecoveryEnvelope sourceRecovery)
    {
        Request = request;
        SourceRecovery = sourceRecovery;
    }

    /// <summary>Gets the exact repair request that must be replayed unchanged after response loss.</summary>
    internal RepairSaveRequest Request { get; }

    /// <summary>Gets the reviewed recovery state that authorized this request.</summary>
    internal WorkspaceRecoveryEnvelope SourceRecovery { get; }
}

/// <summary>Retains an exact recovery-adoption request until its definitive result is mapped.</summary>
internal sealed class WorkspaceAdoptionEnvelope
{
    /// <summary>Initializes one immutable adoption request envelope.</summary>
    /// <param name="workspaceId">The live workspace identity.</param>
    /// <param name="request">The exact recovery-adoption request.</param>
    internal WorkspaceAdoptionEnvelope(Guid workspaceId, ResolveOutputRecoveryRequest request)
    {
        WorkspaceId = workspaceId;
        Request = request;
    }

    /// <summary>Gets the exact live workspace identity.</summary>
    internal Guid WorkspaceId { get; }

    /// <summary>Gets the exact recovery-adoption request.</summary>
    internal ResolveOutputRecoveryRequest Request { get; }
}

/// <summary>Retains one exact recovery adoption result separately from its post-result state read.</summary>
internal sealed class WorkspaceAdoptionBorrowOutcome
{
    /// <summary>Initializes one immutable recovery-adoption outcome.</summary>
    /// <param name="envelope">The exact adoption request envelope.</param>
    /// <param name="result">The exact Core adoption result.</param>
    /// <param name="postAdoptionState">The post-result state read or its detached typed failure.</param>
    internal WorkspaceAdoptionBorrowOutcome(
        WorkspaceAdoptionEnvelope envelope,
        EngineResult<OutputSelectionReceipt> result,
        EngineResult<WorkspaceState> postAdoptionState)
    {
        Envelope = envelope;
        Result = result;
        PostAdoptionState = postAdoptionState;
    }

    /// <summary>Gets the exact adoption request envelope.</summary>
    internal WorkspaceAdoptionEnvelope Envelope { get; }

    /// <summary>Gets the exact Core adoption result.</summary>
    internal EngineResult<OutputSelectionReceipt> Result { get; }

    /// <summary>Gets the post-result state read or its detached typed failure.</summary>
    internal EngineResult<WorkspaceState> PostAdoptionState { get; }
}

/// <summary>Retains a known successful Core transition while its presentation refresh remains incomplete.</summary>
internal sealed class WorkspaceRefreshEnvelope
{
    /// <summary>Initializes one immutable refresh-resume envelope.</summary>
    /// <param name="kind">The known successful transition.</param>
    /// <param name="workspaceId">The exact resulting workspace identity.</param>
    /// <param name="revision">The exact resulting revision.</param>
    /// <param name="baseline">The exact resulting output baseline.</param>
    /// <param name="step">The first incomplete refresh step.</param>
    /// <param name="acceptedState">The accepted resulting state when only participant refresh remains.</param>
    /// <param name="saveResult">The exact committed save result when the transition was a save.</param>
    internal WorkspaceRefreshEnvelope(
        WorkspaceRefreshKind kind,
        Guid workspaceId,
        WorkspaceRevision revision,
        OutputArtifactSetBaseline baseline,
        WorkspaceRefreshStep step,
        WorkspaceState? acceptedState,
        SaveResult? saveResult)
    {
        Kind = kind;
        WorkspaceId = workspaceId;
        Revision = revision;
        Baseline = baseline;
        Step = step;
        AcceptedState = acceptedState;
        SaveResult = saveResult;
    }

    /// <summary>Gets the known successful transition.</summary>
    internal WorkspaceRefreshKind Kind { get; }

    /// <summary>Gets the exact resulting workspace identity.</summary>
    internal Guid WorkspaceId { get; }

    /// <summary>Gets the exact resulting revision.</summary>
    internal WorkspaceRevision Revision { get; }

    /// <summary>Gets the exact resulting output baseline.</summary>
    internal OutputArtifactSetBaseline Baseline { get; }

    /// <summary>Gets the first incomplete refresh step.</summary>
    internal WorkspaceRefreshStep Step { get; }

    /// <summary>Gets the accepted resulting state when only participant refresh remains.</summary>
    internal WorkspaceState? AcceptedState { get; }

    /// <summary>Gets the exact committed save result retained across presentation failure.</summary>
    internal SaveResult? SaveResult { get; }

    /// <summary>Creates an envelope that resumes at participant refresh without changing the persistence identity.</summary>
    /// <param name="state">The newly accepted exact resulting workspace state.</param>
    /// <returns>A new immutable envelope retaining the original transition identity.</returns>
    internal WorkspaceRefreshEnvelope WithAcceptedState(WorkspaceState state)
    {
        return new WorkspaceRefreshEnvelope(
            Kind,
            WorkspaceId,
            Revision,
            Baseline,
            WorkspaceRefreshStep.RefreshParticipant,
            state,
            SaveResult);
    }
}

/// <summary>Retains the exact external-conflict identity reviewed for an Open-only abandonment choice.</summary>
internal sealed class WorkspaceAbandonmentEnvelope
{
    /// <summary>Initializes one immutable external-conflict abandonment envelope.</summary>
    /// <param name="workspaceId">The exact live workspace identity.</param>
    /// <param name="output">The exact selected output named by confirmation.</param>
    internal WorkspaceAbandonmentEnvelope(Guid workspaceId, OutputAssociation output)
    {
        WorkspaceId = workspaceId;
        Output = output;
    }

    /// <summary>Gets the exact live workspace identity.</summary>
    internal Guid WorkspaceId { get; }

    /// <summary>Gets the exact selected output named by confirmation.</summary>
    internal OutputAssociation Output { get; }
}
