using CreationsForge.Services;
using Shouldly;

namespace CreationsForge.PresentationTests.Services;

/// <summary>Verifies transferred leave and shutdown leases retain exact admission and reject broader permissions.</summary>
public sealed class WorkspaceLeaveReservationTests
{
    /// <summary>Verifies the reservation alone owns the supplied lease until its idempotent disposal.</summary>
    [Fact]
    public async Task Dispose_WhenReservationOwnsLease_ReleasesExactAdmissionOnce()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        var lease = await arbiter.ReserveWorkspaceTransitionAsync(WorkspaceTransitionDrainMode.WaitForCurrentOperation);
        var reservation = new WorkspaceLeaveReservation(
            lease,
            WorkspaceLeaveReason.OpenWorkspace,
            WorkspaceLeaveDisposition.NoWorkspace,
            expectedWorkspaceId: null);

        reservation.IsActive.ShouldBeTrue();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();

        reservation.Dispose();
        reservation.Dispose();

        reservation.IsActive.ShouldBeFalse();
        lease.IsActive.ShouldBeFalse();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies reason, disposition, and workspace identity combinations cannot broaden the established proof.</summary>
    /// <param name="reason">The requested final action.</param>
    /// <param name="disposition">The supplied proof.</param>
    /// <param name="hasWorkspaceId">Whether the constructor receives a live workspace identity.</param>
    [Theory]
    [InlineData(WorkspaceLeaveReason.OpenWorkspace, WorkspaceLeaveDisposition.NoWorkspace, true)]
    [InlineData(WorkspaceLeaveReason.CloseWorkspace, WorkspaceLeaveDisposition.ReadyAndClean, false)]
    [InlineData(WorkspaceLeaveReason.CloseWorkspace, WorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen, true)]
    [InlineData(WorkspaceLeaveReason.ShowSettings, WorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen, true)]
    [InlineData(WorkspaceLeaveReason.ExitApplication, WorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen, true)]
    public async Task Constructor_WithIncompatibleProof_RejectsReservation(
        WorkspaceLeaveReason reason,
        WorkspaceLeaveDisposition disposition,
        bool hasWorkspaceId)
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        using var lease = await arbiter.ReserveWorkspaceTransitionAsync(WorkspaceTransitionDrainMode.WaitForCurrentOperation);

        Should.Throw<ArgumentException>(() => new WorkspaceLeaveReservation(
            lease,
            reason,
            disposition,
            hasWorkspaceId ? Guid.NewGuid() : null));
    }

    /// <summary>Verifies a previously released transition cannot be revived as a leave reservation.</summary>
    [Fact]
    public async Task Constructor_WithDisposedLease_RejectsReservation()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        var lease = await arbiter.ReserveWorkspaceTransitionAsync(WorkspaceTransitionDrainMode.WaitForCurrentOperation);
        lease.Dispose();

        Should.Throw<ArgumentException>(() => new WorkspaceLeaveReservation(
            lease,
            WorkspaceLeaveReason.OpenWorkspace,
            WorkspaceLeaveDisposition.NoWorkspace,
            expectedWorkspaceId: null));
    }

    /// <summary>Verifies a shutdown lease keeps the accepted exit reservation active until application cleanup finishes.</summary>
    [Fact]
    public async Task ShutdownLease_WithExitReservation_ReleasesAdmissionAfterCleanup()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        var transitionLease = await arbiter.ReserveWorkspaceTransitionAsync(WorkspaceTransitionDrainMode.WaitForCurrentOperation);
        var reservation = new WorkspaceLeaveReservation(
            transitionLease,
            WorkspaceLeaveReason.ExitApplication,
            WorkspaceLeaveDisposition.NoWorkspace,
            expectedWorkspaceId: null);
        var shutdownLease = new ApplicationShutdownLease(reservation);

        shutdownLease.IsActive.ShouldBeTrue();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();

        shutdownLease.Dispose();
        shutdownLease.Dispose();

        shutdownLease.IsActive.ShouldBeFalse();
        reservation.IsActive.ShouldBeFalse();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies application shutdown cannot consume an Open-only reservation.</summary>
    [Fact]
    public async Task ShutdownLease_WithOpenReservation_RejectsShutdown()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        using var transitionLease = await arbiter.ReserveWorkspaceTransitionAsync(WorkspaceTransitionDrainMode.WaitForCurrentOperation);
        using var reservation = new WorkspaceLeaveReservation(
            transitionLease,
            WorkspaceLeaveReason.OpenWorkspace,
            WorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen,
            Guid.NewGuid());

        Should.Throw<ArgumentException>(() => new ApplicationShutdownLease(reservation));
        reservation.IsActive.ShouldBeTrue();
    }
}
