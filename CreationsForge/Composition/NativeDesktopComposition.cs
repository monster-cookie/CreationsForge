using Autofac;
using CreationsForge.Bethesda.Assets.Archives;
using CreationsForge.Bethesda.Assets.Archives.Ba2;
using CreationsForge.Bethesda.Assets.Archives.Bsa;
using CreationsForge.Bethesda.Assets.Nif;
using CreationsForge.Bethesda.Assets.Resources;
using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.Services;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.NativeEditing.Drafts;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Serilog;

namespace CreationsForge.Composition;

/// <summary>
/// Builds the desktop application graph from the native engine and the presentation services that remain live during legacy retirement.
/// </summary>
internal static class NativeDesktopComposition
{
    /// <summary>
    /// Creates the application-owned container without registering legacy database, importer, or game presentation modules.
    /// </summary>
    /// <param name="configurationStore">The configuration store shared by startup, settings, and diagnostics.</param>
    /// <param name="logger">The application logger supplied to native and presentation services.</param>
    /// <returns>A container that owns the native desktop service graph.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configurationStore"/> or <paramref name="logger"/> is <see langword="null"/>.</exception>
    internal static IContainer Create(IApplicationConfigurationStore configurationStore, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(configurationStore);
        ArgumentNullException.ThrowIfNull(logger);

        var builder = new ContainerBuilder();
        builder.RegisterInstance(configurationStore).As<IApplicationConfigurationStore>().SingleInstance();
        builder.RegisterInstance(logger).As<ILogger>().SingleInstance();
        builder.RegisterModule<NativeEngineModule>();

        builder.RegisterType<ApplicationSettingsService>().As<IApplicationSettingsService>().InstancePerLifetimeScope();
        builder.RegisterType<GameSelectionService>().As<IGameSelectionService>().InstancePerLifetimeScope();
        builder.RegisterType<ProcessTerminationDiagnosticsService>().As<IProcessTerminationDiagnosticsService>().SingleInstance();

        builder.RegisterType<Ba2ArchiveReader>().As<IAssetArchiveReader>().InstancePerLifetimeScope();
        builder.RegisterType<BsaArchiveReader>().As<IAssetArchiveReader>().InstancePerLifetimeScope();
        builder.RegisterType<BethesdaAssetProvider>().As<IBethesdaAssetProvider>().InstancePerLifetimeScope();
        builder.RegisterType<NifPreviewModelReader>().As<INifPreviewModelReader>().InstancePerLifetimeScope();
        builder.RegisterType<AssetPreviewRenderMeshFactory>().As<IAssetPreviewRenderMeshFactory>().SingleInstance();
        builder.RegisterType<AssetPreviewSceneService>().As<IAssetPreviewSceneService>().InstancePerLifetimeScope();
        builder.RegisterType<ExternalAssetOpenService>().As<IExternalAssetOpenService>().SingleInstance();

        builder.RegisterType<MainWindow>().SingleInstance();
        builder.RegisterType<SettingsView>().InstancePerLifetimeScope();
        builder.RegisterType<SettingsViewModel>().InstancePerLifetimeScope();
        builder.RegisterType<NativeWorkspaceShellView>().InstancePerLifetimeScope();
        builder.RegisterType<NativeWorkspaceShellViewModel>().AsSelf().As<INativeWorkspaceLeaveGuard>().InstancePerLifetimeScope();
        builder.RegisterType<NativeFormListBrowserView>().InstancePerLifetimeScope();
        builder.RegisterType<NativeFormListBrowserViewModel>().AsSelf().As<INativeWorkspaceEditParticipant>().InstancePerLifetimeScope();
        builder.RegisterType<NativeFormListEditorViewModelFactory>().As<INativeFormListEditorViewModelFactory>().InstancePerLifetimeScope();
        builder.RegisterType<NativeWorkspacePresentationOperationArbiter>().As<INativeWorkspacePresentationOperationArbiter>().InstancePerLifetimeScope();
        builder.RegisterType<NativeWorkspaceChangesViewModel>().InstancePerLifetimeScope();
        builder.RegisterType<NativeFormListWireCatalogResolver>().As<INativeFormListWireCatalogResolver>().SingleInstance();
        builder.RegisterType<NativeFormListDraftFactory>().As<INativeFormListDraftFactory>().SingleInstance();
        builder.RegisterType<NativeFormListDraftValidator>().As<INativeFormListDraftValidator>().SingleInstance();
        builder.RegisterType<NativeFormListDraftSerializer>().As<INativeFormListDraftSerializer>().SingleInstance();
        builder.RegisterType<NativeJsonTreeProjectionService>().SingleInstance();
        builder.RegisterType<NativeWorkspaceSelectionViewModel>();
        builder.RegisterType<ApplicationWindowService>().As<IApplicationWindowService>().SingleInstance();
        builder.RegisterType<NativeApplicationNavigationService>().As<INativeApplicationNavigationService>().SingleInstance();
        builder.RegisterType<AvaloniaUiDispatcher>().As<IUiDispatcher>().SingleInstance();
        builder.RegisterType<NativeWorkspaceCoordinator>().As<INativeWorkspaceCoordinator>().SingleInstance();
        builder.RegisterType<NativeReferencePickerService>().As<INativeReferencePickerService>().SingleInstance();
        builder.RegisterType<NativeWorkspacePathPicker>().As<INativeWorkspacePathPicker>().SingleInstance();
        builder.RegisterType<NativeWorkspaceSelectionDialogService>().As<INativeWorkspaceSelectionDialogService>().SingleInstance();
        builder.RegisterType<NativeWorkspaceChangesDialogService>().As<INativeWorkspaceChangesDialogService>().SingleInstance();

        return builder.Build();
    }
}
