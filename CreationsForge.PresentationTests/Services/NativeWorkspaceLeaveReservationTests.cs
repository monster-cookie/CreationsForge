using CreationsForge.Services;
using Shouldly;

namespace CreationsForge.PresentationTests.Services;

/// <summary>Verifies transferred leave and shutdown leases retain exact admission and reject broader permissions.</summary>
public sealed class NativeWorkspaceLeaveReservationTests
{
    /// <summary>Verifies the reservation alone owns the supplied lease until its idempotent disposal.</summary>
    [Fact]
    public async Task Dispose_WhenReservationOwnsLease_ReleasesExactAdmissionOnce()
    {
        var arbiter = new NativeWorkspacePresentationOperationArbiter();
        var lease = await arbiter.ReserveWorkspaceTransitionAsync(NativeWorkspaceTransitionDrainMode.WaitForCurrentOperation);
        var reservation = new NativeWorkspaceLeaveReservation(
            lease,
            NativeWorkspaceLeaveReason.OpenWorkspace,
            NativeWorkspaceLeaveDisposition.NoWorkspace,
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
    [InlineData(NativeWorkspaceLeaveReason.OpenWorkspace, NativeWorkspaceLeaveDisposition.NoWorkspace, true)]
    [InlineData(NativeWorkspaceLeaveReason.CloseWorkspace, NativeWorkspaceLeaveDisposition.ReadyAndClean, false)]
    [InlineData(NativeWorkspaceLeaveReason.CloseWorkspace, NativeWorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen, true)]
    [InlineData(NativeWorkspaceLeaveReason.ShowSettings, NativeWorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen, true)]
    [InlineData(NativeWorkspaceLeaveReason.ExitApplication, NativeWorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen, true)]
    public async Task Constructor_WithIncompatibleProof_RejectsReservation(
        NativeWorkspaceLeaveReason reason,
        NativeWorkspaceLeaveDisposition disposition,
        bool hasWorkspaceId)
    {
        var arbiter = new NativeWorkspacePresentationOperationArbiter();
        using var lease = await arbiter.ReserveWorkspaceTransitionAsync(NativeWorkspaceTransitionDrainMode.WaitForCurrentOperation);

        Should.Throw<ArgumentException>(() => new NativeWorkspaceLeaveReservation(
            lease,
            reason,
            disposition,
            hasWorkspaceId ? Guid.NewGuid() : null));
    }

    /// <summary>Verifies a previously released transition cannot be revived as a leave reservation.</summary>
    [Fact]
    public async Task Constructor_WithDisposedLease_RejectsReservation()
    {
        var arbiter = new NativeWorkspacePresentationOperationArbiter();
        var lease = await arbiter.ReserveWorkspaceTransitionAsync(NativeWorkspaceTransitionDrainMode.WaitForCurrentOperation);
        lease.Dispose();

        Should.Throw<ArgumentException>(() => new NativeWorkspaceLeaveReservation(
            lease,
            NativeWorkspaceLeaveReason.OpenWorkspace,
            NativeWorkspaceLeaveDisposition.NoWorkspace,
            expectedWorkspaceId: null));
    }

    /// <summary>Verifies a shutdown lease keeps the accepted exit reservation active until application cleanup finishes.</summary>
    [Fact]
    public async Task ShutdownLease_WithExitReservation_ReleasesAdmissionAfterCleanup()
    {
        var arbiter = new NativeWorkspacePresentationOperationArbiter();
        var transitionLease = await arbiter.ReserveWorkspaceTransitionAsync(NativeWorkspaceTransitionDrainMode.WaitForCurrentOperation);
        var reservation = new NativeWorkspaceLeaveReservation(
            transitionLease,
            NativeWorkspaceLeaveReason.ExitApplication,
            NativeWorkspaceLeaveDisposition.NoWorkspace,
            expectedWorkspaceId: null);
        var shutdownLease = new NativeApplicationShutdownLease(reservation);

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
        var arbiter = new NativeWorkspacePresentationOperationArbiter();
        using var transitionLease = await arbiter.ReserveWorkspaceTransitionAsync(NativeWorkspaceTransitionDrainMode.WaitForCurrentOperation);
        using var reservation = new NativeWorkspaceLeaveReservation(
            transitionLease,
            NativeWorkspaceLeaveReason.OpenWorkspace,
            NativeWorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen,
            Guid.NewGuid());

        Should.Throw<ArgumentException>(() => new NativeApplicationShutdownLease(reservation));
        reservation.IsActive.ShouldBeTrue();
    }
}
