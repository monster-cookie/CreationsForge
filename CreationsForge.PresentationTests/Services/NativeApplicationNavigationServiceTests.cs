using Autofac;
using Avalonia.Headless.XUnit;
using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.PresentationTests.Headless;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Shouldly;

namespace CreationsForge.PresentationTests.Services;

/// <summary>Verifies guarded native root-view replacement and scoped presentation cleanup.</summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class NativeApplicationNavigationServiceTests
{
    /// <summary>Verifies Settings publication drains the old shell scope and a fresh shell receives a new shared arbiter.</summary>
    [AvaloniaFact]
    public async Task TryShowSettingsAsync_WhenNoWorkspace_DrainsShellBeforeFreshReturn()
    {
        var coordinator = new FakeNativeWorkspaceCoordinator();
        var windowService = new FakeApplicationWindowService();
        var arbiters = new List<INativeWorkspacePresentationOperationArbiter>();
        await using (var container = CreateContainer(coordinator, windowService, arbiters))
        {
            var navigation = container.Resolve<INativeApplicationNavigationService>();

            navigation.ShowWorkspaceShell();

            var firstView = windowService.Content.ShouldBeOfType<NativeWorkspaceShellView>();
            coordinator.SubscriberCount.ShouldBe(4);
            arbiters.ShouldHaveSingleItem();

            (await navigation.TryShowSettingsAsync()).ShouldBeTrue();

            windowService.Content.ShouldBeOfType<SettingsView>();
            coordinator.SubscriberCount.ShouldBe(0);
            arbiters[0].IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();

            navigation.ShowWorkspaceShell();

            windowService.Content.ShouldBeOfType<NativeWorkspaceShellView>().ShouldNotBeSameAs(firstView);
            coordinator.SubscriberCount.ShouldBe(4);
            arbiters.Count.ShouldBe(2);
            arbiters[1].ShouldNotBeSameAs(arbiters[0]);
        }

        coordinator.SubscriberCount.ShouldBe(0);
        windowService.Content.ShouldBeNull();
    }

    /// <summary>Verifies shutdown owns shell transition admission until the caller releases the returned lease.</summary>
    [AvaloniaFact]
    public async Task ReserveShutdownAsync_WhenShellIsCurrent_HoldsAdmissionUntilLeaseDisposal()
    {
        var coordinator = new FakeNativeWorkspaceCoordinator();
        var windowService = new FakeApplicationWindowService();
        var arbiters = new List<INativeWorkspacePresentationOperationArbiter>();
        await using var container = CreateContainer(coordinator, windowService, arbiters);
        var navigation = container.Resolve<INativeApplicationNavigationService>();
        navigation.ShowWorkspaceShell();
        var arbiter = arbiters.ShouldHaveSingleItem();

        var lease = await navigation.ReserveShutdownAsync();

        lease.ShouldNotBeNull().IsActive.ShouldBeTrue();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
        arbiter.TryBeginEditorOperation().ShouldBeNull();

        lease.Dispose();

        lease.IsActive.ShouldBeFalse();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Creates the native navigation graph with test-owned presentation services and scoped admission.</summary>
    /// <param name="coordinator">The controllable root workspace coordinator.</param>
    /// <param name="windowService">The root-window recorder.</param>
    /// <param name="arbiters">The recorder for each navigation-scope operation arbiter.</param>
    /// <returns>The owned Autofac test container.</returns>
    private static IContainer CreateContainer(
        FakeNativeWorkspaceCoordinator coordinator,
        FakeApplicationWindowService windowService,
        IList<INativeWorkspacePresentationOperationArbiter> arbiters)
    {
        var builder = new ContainerBuilder();
        var dispatcher = new InlineUiDispatcher();
        var picker = new RecordingNativeReferencePickerService();
        builder.RegisterInstance(coordinator).As<INativeWorkspaceCoordinator>();
        builder.RegisterInstance(windowService).As<IApplicationWindowService>();
        builder.RegisterInstance(new TestApplicationConfigurationStore()).As<IApplicationConfigurationStore>();
        builder.RegisterType<ApplicationSettingsService>().As<IApplicationSettingsService>();
        builder.RegisterInstance(new FakeNativeWorkspaceSelectionDialogService())
            .As<INativeWorkspaceSelectionDialogService>();
        builder.RegisterInstance(new RecordingWorkspaceChangesDialogService())
            .As<INativeWorkspaceChangesDialogService>();
        builder.RegisterInstance(new RecordingWorkspaceSaveCoordinator()).As<IWorkspaceSaveCoordinator>();
        builder.RegisterInstance(new FakeNativeWorkspacePathPicker()).As<INativeWorkspacePathPicker>();
        builder.RegisterInstance(new FakeGameSelectionService()).As<IGameSelectionService>();
        builder.RegisterInstance(dispatcher).As<IUiDispatcher>();
        builder.RegisterInstance(new Serilog.LoggerConfiguration().CreateLogger()).As<Serilog.ILogger>();
        builder.RegisterType<SettingsViewModel>().InstancePerLifetimeScope();
        builder.RegisterType<SettingsView>().InstancePerLifetimeScope();
        builder.RegisterType<NativeWorkspaceSelectionViewModel>();
        builder.RegisterType<NativeWorkspaceShellViewModel>()
            .AsSelf()
            .As<INativeWorkspaceLeaveGuard>()
            .InstancePerLifetimeScope();
        builder.RegisterType<NativeWorkspaceShellView>().InstancePerLifetimeScope();
        builder.RegisterType<NativeJsonTreeProjectionService>().SingleInstance();
        builder.RegisterInstance(picker).As<INativeReferencePickerService>();
        builder.RegisterType<NativeWorkspacePresentationOperationArbiter>()
            .As<INativeWorkspacePresentationOperationArbiter>()
            .InstancePerLifetimeScope()
            .OnActivated(eventArgs => arbiters.Add(eventArgs.Instance));
        builder.Register(context => HeadlessNativeEditorFactory.Create(
                coordinator,
                context.Resolve<INativeWorkspacePresentationOperationArbiter>(),
                picker,
                dispatcher))
            .As<INativeFormListEditorViewModelFactory>()
            .InstancePerLifetimeScope();
        builder.RegisterType<NativeFormListBrowserViewModel>()
            .AsSelf()
            .As<INativeWorkspaceEditParticipant>()
            .InstancePerLifetimeScope();
        builder.RegisterType<NativeFormListBrowserView>().InstancePerLifetimeScope();
        builder.RegisterType<NativeWorkspaceChangesViewModel>().InstancePerLifetimeScope();
        builder.RegisterType<NativeApplicationNavigationService>()
            .As<INativeApplicationNavigationService>()
            .SingleInstance();
        return builder.Build();
    }

    /// <summary>Stores an in-memory default application configuration for constructing the retained Settings view.</summary>
    private sealed class TestApplicationConfigurationStore : IApplicationConfigurationStore
    {
        /// <inheritdoc />
        public string ConfigurationPath => string.Empty;

        /// <inheritdoc />
        public ApplicationConfiguration Current { get; private set; } = new();

        /// <inheritdoc />
        public void Load()
        {
        }

        /// <inheritdoc />
        public void Save(ApplicationConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            Current = configuration;
        }
    }
}
