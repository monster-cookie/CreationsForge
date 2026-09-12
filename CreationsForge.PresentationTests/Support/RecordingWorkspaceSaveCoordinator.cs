using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.PresentationTests.Support;

/// <summary>Records recovery and repair requests made by the presentation change lifecycle.</summary>
internal sealed class RecordingWorkspaceSaveCoordinator : IWorkspaceSaveCoordinator
{
    /// <summary>Initializes a save coordinator whose operations require explicit test handlers.</summary>
    internal RecordingWorkspaceSaveCoordinator()
    {
    }

    /// <summary>Gets or sets a custom recovery implementation.</summary>
    internal Func<RecoverSaveRequest, CancellationToken, ValueTask<RecoverSaveResult>>? OnRecoverAsync { get; set; }

    /// <summary>Gets or sets a custom repair implementation.</summary>
    internal Func<RepairSaveRequest, CancellationToken, ValueTask<RepairSaveResult>>? OnRepairAsync { get; set; }

    /// <summary>Gets the exact recovery requests in invocation order.</summary>
    internal List<RecoverSaveRequest> RecoverRequests { get; } = [];

    /// <summary>Gets the exact repair requests in invocation order.</summary>
    internal List<RepairSaveRequest> RepairRequests { get; } = [];

    /// <inheritdoc />
    public ValueTask<RecoverSaveResult> RecoverAsync(
        RecoverSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        RecoverRequests.Add(request);
        cancellationToken.ThrowIfCancellationRequested();
        return OnRecoverAsync?.Invoke(request, cancellationToken)
            ?? throw new NotSupportedException("The recording recovery operation was not configured.");
    }

    /// <inheritdoc />
    public ValueTask<RepairSaveResult> RepairAsync(
        RepairSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        RepairRequests.Add(request);
        cancellationToken.ThrowIfCancellationRequested();
        return OnRepairAsync?.Invoke(request, cancellationToken)
            ?? throw new NotSupportedException("The recording repair operation was not configured.");
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OutputAdmissionResult>> InspectOutputAdmissionAsync(
        IOutputDirectoryLease lease,
        OutputAdmissionRequest request,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException("The presentation recording coordinator does not inspect output admission.");

    /// <inheritdoc />
    public ValueTask<SaveResult> SaveAsync(
        WorkspaceSaveContext context,
        SaveRequest request,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException("The presentation recording coordinator does not persist native files.");

    /// <inheritdoc />
    public ValueTask<EngineResult<ResolvedOutputEvidence>> ValidateResolvedEvidenceAsync(
        IOutputDirectoryLease lease,
        ResolvedOutputEvidence evidence,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException("The presentation recording coordinator does not validate output evidence.");
}
