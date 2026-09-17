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

/// <summary>Verifies guarded plugin root-view replacement and scoped presentation cleanup.</summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class ApplicationNavigationServiceTests
{
    /// <summary>Verifies Settings publication drains the old shell scope and a fresh shell receives a new shared arbiter.</summary>
    [AvaloniaFact]
    public async Task TryShowSettingsAsync_WhenNoWorkspace_DrainsShellBeforeFreshReturn()
    {
        var coordinator = new FakeWorkspaceCoordinator();
        var windowService = new FakeApplicationWindowService();
        var arbiters = new List<IWorkspacePresentationOperationArbiter>();
        await using (var container = CreateContainer(coordinator, windowService, arbiters))
        {
            var navigation = container.Resolve<IApplicationNavigationService>();

            navigation.ShowWorkspaceShell();

            var firstView = windowService.Content.ShouldBeOfType<WorkspaceShellView>();
            coordinator.SubscriberCount.ShouldBe(5);
            arbiters.ShouldHaveSingleItem();

            (await navigation.TryShowSettingsAsync()).ShouldBeTrue();

            windowService.Content.ShouldBeOfType<SettingsView>();
            coordinator.SubscriberCount.ShouldBe(0);
            arbiters[0].IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();

            navigation.ShowWorkspaceShell();

            windowService.Content.ShouldBeOfType<WorkspaceShellView>().ShouldNotBeSameAs(firstView);
            coordinator.SubscriberCount.ShouldBe(5);
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
        var coordinator = new FakeWorkspaceCoordinator();
        var windowService = new FakeApplicationWindowService();
        var arbiters = new List<IWorkspacePresentationOperationArbiter>();
        await using var container = CreateContainer(coordinator, windowService, arbiters);
        var navigation = container.Resolve<IApplicationNavigationService>();
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

    /// <summary>Creates the plugin navigation graph with test-owned presentation services and scoped admission.</summary>
    /// <param name="coordinator">The controllable root workspace coordinator.</param>
    /// <param name="windowService">The root-window recorder.</param>
    /// <param name="arbiters">The recorder for each navigation-scope operation arbiter.</param>
    /// <returns>The owned Autofac test container.</returns>
    private static IContainer CreateContainer(
        FakeWorkspaceCoordinator coordinator,
        FakeApplicationWindowService windowService,
        IList<IWorkspacePresentationOperationArbiter> arbiters)
    {
        var builder = new ContainerBuilder();
        var dispatcher = new InlineUiDispatcher();
        var picker = new RecordingReferencePickerService();
        builder.RegisterInstance(coordinator).As<IWorkspaceCoordinator>();
        builder.RegisterInstance(windowService).As<IApplicationWindowService>();
        builder.RegisterInstance(new TestApplicationConfigurationStore()).As<IApplicationConfigurationStore>();
        builder.RegisterType<ApplicationSettingsService>().As<IApplicationSettingsService>();
        builder.RegisterInstance(new FakeWorkspaceSelectionDialogService())
            .As<IWorkspaceSelectionDialogService>();
        builder.RegisterInstance(new RecordingWorkspaceChangesDialogService())
            .As<IWorkspaceChangesDialogService>();
        builder.RegisterInstance(new RecordingWorkspaceSaveCoordinator()).As<IWorkspaceSaveCoordinator>();
        builder.RegisterInstance(new FakeWorkspacePathPicker()).As<IWorkspacePathPicker>();
        builder.RegisterInstance(new FakeGameSelectionService()).As<IGameSelectionService>();
        builder.RegisterInstance(dispatcher).As<IUiDispatcher>();
        builder.RegisterInstance(new Serilog.LoggerConfiguration().CreateLogger()).As<Serilog.ILogger>();
        builder.RegisterType<SettingsViewModel>().InstancePerLifetimeScope();
        builder.RegisterType<SettingsView>().InstancePerLifetimeScope();
        builder.RegisterType<WorkspaceSelectionViewModel>();
        builder.RegisterType<WorkspaceShellViewModel>()
            .AsSelf()
            .As<IWorkspaceLeaveGuard>()
            .InstancePerLifetimeScope();
        builder.RegisterType<WorkspaceShellView>().InstancePerLifetimeScope();
        builder.RegisterType<RecordJsonTreeProjectionService>().SingleInstance();
        builder.RegisterInstance(picker).As<IReferencePickerService>();
        builder.RegisterType<WorkspacePresentationOperationArbiter>()
            .As<IWorkspacePresentationOperationArbiter>()
            .InstancePerLifetimeScope()
            .OnActivated(eventArgs => arbiters.Add(eventArgs.Instance));
        builder.Register(context => HeadlessRecordEditorFactory.Create(
                coordinator,
                context.Resolve<IWorkspacePresentationOperationArbiter>(),
                picker,
                dispatcher))
            .As<IFormListEditorViewModelFactory>()
            .InstancePerLifetimeScope();
        builder.RegisterType<FormListBrowserViewModel>()
            .AsSelf()
            .As<IWorkspaceEditParticipant>()
            .InstancePerLifetimeScope();
        builder.RegisterType<FormListBrowserView>().InstancePerLifetimeScope();
        builder.RegisterType<MajorRecordBrowserViewModel>().InstancePerLifetimeScope();
        builder.RegisterType<MajorRecordBrowserView>().InstancePerLifetimeScope();
        builder.RegisterType<WorkspaceChangesViewModel>().InstancePerLifetimeScope();
        builder.RegisterType<ApplicationNavigationService>()
            .As<IApplicationNavigationService>()
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
