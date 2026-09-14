using Autofac;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using Shouldly;

namespace CreationsForge.PresentationTests;

/// <summary>Verifies guarded application shutdown remains retryable only before irreversible teardown.</summary>
public sealed class AppShutdownTests
{
    /// <summary>Verifies Keep Editing leaves the live container intact and a later accepted attempt can finish.</summary>
    [Fact]
    public async Task BeginShutdownAsync_AfterKeepEditing_AllowsSuccessfulRetry()
    {
        var navigation = new FakeApplicationNavigationService();
        var coordinator = new FakeWorkspaceCoordinator();
        var diagnostics = new RecordingTerminationDiagnosticsService();
        var disposeProbe = new ContainerDisposeProbe();
        var attempt = 0;
        navigation.ShutdownAction = _ => Task.FromResult(
            ++attempt == 1 ? null : ApplicationShutdownLease.CreateForSettings());
        var container = CreateContainer(navigation, coordinator, diagnostics, disposeProbe);
        var app = new App(container);

        try
        {
            var first = app.BeginShutdownAsync(desktop: null);
            await first;

            navigation.ShutdownReservationCount.ShouldBe(1);
            coordinator.DisposeCount.ShouldBe(0);
            disposeProbe.DisposeCount.ShouldBe(0);
            diagnostics.MarkCleanShutdownCount.ShouldBe(0);

            var second = app.BeginShutdownAsync(desktop: null);
            second.ShouldNotBeSameAs(first);
            await second;

            navigation.ShutdownReservationCount.ShouldBe(2);
            coordinator.DisposeCount.ShouldBe(1);
            disposeProbe.DisposeCount.ShouldBe(1);
            diagnostics.MarkCleanShutdownCount.ShouldBe(1);
        }
        finally
        {
            if (disposeProbe.DisposeCount == 0)
            {
                await container.DisposeAsync();
            }
        }
    }

    /// <summary>Verifies a teardown failure remains terminal after the container is disposed instead of requesting an impossible retry.</summary>
    [Fact]
    public async Task BeginShutdownAsync_WhenCoordinatorDisposalFails_RemainsTerminalAfterContainerDisposal()
    {
        var navigation = new FakeApplicationNavigationService();
        var coordinator = new FakeWorkspaceCoordinator
        {
            DisposeAction = () => new ValueTask(Task.FromException(
                new IOException("The test coordinator failed during accepted teardown.")))
        };
        var diagnostics = new RecordingTerminationDiagnosticsService();
        var disposeProbe = new ContainerDisposeProbe();
        var container = CreateContainer(navigation, coordinator, diagnostics, disposeProbe);
        var app = new App(container);

        var first = app.BeginShutdownAsync(desktop: null);
        await first;

        navigation.ShutdownReservationCount.ShouldBe(1);
        coordinator.DisposeCount.ShouldBe(1);
        disposeProbe.DisposeCount.ShouldBe(1);
        diagnostics.MarkCleanShutdownCount.ShouldBe(0);

        var second = app.BeginShutdownAsync(desktop: null);

        second.ShouldBeSameAs(first);
        await second;
        navigation.ShutdownReservationCount.ShouldBe(1);
        coordinator.DisposeCount.ShouldBe(1);
    }

    /// <summary>Creates the minimal application shutdown graph without configuration or profile filesystem access.</summary>
    /// <param name="navigation">The guarded shutdown admission recorder.</param>
    /// <param name="coordinator">The workspace owner.</param>
    /// <param name="diagnostics">The clean-shutdown recorder.</param>
    /// <param name="disposeProbe">The container-owned teardown probe.</param>
    /// <returns>The application container used by one shutdown test.</returns>
    private static IContainer CreateContainer(
        FakeApplicationNavigationService navigation,
        FakeWorkspaceCoordinator coordinator,
        RecordingTerminationDiagnosticsService diagnostics,
        ContainerDisposeProbe disposeProbe)
    {
        var builder = new ContainerBuilder();
        builder.RegisterInstance(navigation).As<IApplicationNavigationService>();
        builder.RegisterInstance(coordinator).As<IWorkspaceCoordinator>();
        builder.RegisterInstance(diagnostics).As<IProcessTerminationDiagnosticsService>().ExternallyOwned();
        builder.RegisterInstance(disposeProbe).AsSelf().OwnedByLifetimeScope();
        var container = builder.Build();
        container.Resolve<ContainerDisposeProbe>();
        return container;
    }

    /// <summary>Records clean-shutdown diagnostics without creating session files.</summary>
    private sealed class RecordingTerminationDiagnosticsService : IProcessTerminationDiagnosticsService
    {
        /// <summary>Gets how many clean-shutdown records were requested.</summary>
        internal int MarkCleanShutdownCount { get; private set; }

        /// <inheritdoc />
        public CancellationToken TerminationToken => CancellationToken.None;

        /// <inheritdoc />
        public void StartSession(string surfaceName, string? logPath)
        {
        }

        /// <inheritdoc />
        public void UpdateHeartbeat(string phaseName)
        {
        }

        /// <inheritdoc />
        public void MarkCleanShutdown(string reason)
        {
            MarkCleanShutdownCount++;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }

    /// <summary>Records disposal performed by the application-owned Autofac container.</summary>
    private sealed class ContainerDisposeProbe : IDisposable
    {
        /// <summary>Gets how many container-owned disposal calls completed.</summary>
        internal int DisposeCount { get; private set; }

        /// <summary>Records one container-owned disposal.</summary>
        public void Dispose()
        {
            DisposeCount++;
        }
    }
}
