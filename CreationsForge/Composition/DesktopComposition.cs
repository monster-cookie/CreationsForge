using Autofac;
using CreationsForge.Bethesda.Assets.Archives;
using CreationsForge.Bethesda.Assets.Archives.Ba2;
using CreationsForge.Bethesda.Assets.Archives.Bsa;
using CreationsForge.Bethesda.Assets.Nif;
using CreationsForge.Bethesda.Assets.Resources;
using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Services;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.RecordEditing.Drafts;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Serilog;

namespace CreationsForge.Composition;

/// <summary>
/// Builds the desktop application graph from the engine and the presentation services that remain live during legacy retirement.
/// </summary>
internal static class DesktopComposition
{
    /// <summary>
    /// Creates the application-owned container without registering legacy database, importer, or game presentation modules.
    /// </summary>
    /// <param name="configurationStore">The configuration store shared by startup, settings, and diagnostics.</param>
    /// <param name="logger">The application logger supplied to plugin and presentation services.</param>
    /// <returns>A container that owns the plugin desktop service graph.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configurationStore"/> or <paramref name="logger"/> is <see langword="null"/>.</exception>
    internal static IContainer Create(IApplicationConfigurationStore configurationStore, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(configurationStore);
        ArgumentNullException.ThrowIfNull(logger);

        var builder = new ContainerBuilder();
        builder.RegisterInstance(configurationStore).As<IApplicationConfigurationStore>().SingleInstance();
        builder.RegisterInstance(logger).As<ILogger>().SingleInstance();
        builder.RegisterModule<FormListEngineModule>();

        builder.RegisterType<ApplicationSettingsService>().As<IApplicationSettingsService>().InstancePerLifetimeScope();
        builder.RegisterType<GameSelectionService>().As<IGameSelectionService>().InstancePerLifetimeScope();
        builder.RegisterType<PluginDiscoveryService>().As<IPluginDiscoveryService>().SingleInstance();
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
        builder.RegisterType<WorkspaceShellView>().InstancePerLifetimeScope();
        builder.RegisterType<WorkspaceShellViewModel>().AsSelf().As<IWorkspaceLeaveGuard>().InstancePerLifetimeScope();
        builder.RegisterType<FormListBrowserView>().InstancePerLifetimeScope();
        builder.RegisterType<FormListBrowserViewModel>().AsSelf().As<IWorkspaceEditParticipant>().InstancePerLifetimeScope();
        builder.RegisterType<MajorRecordBrowserView>().InstancePerLifetimeScope();
        builder.RegisterType<MajorRecordBrowserViewModel>().InstancePerLifetimeScope();
        builder.RegisterType<FormListEditorViewModelFactory>().As<IFormListEditorViewModelFactory>().InstancePerLifetimeScope();
        builder.RegisterType<WorkspacePresentationOperationArbiter>().As<IWorkspacePresentationOperationArbiter>().InstancePerLifetimeScope();
        builder.RegisterType<WorkspaceChangesViewModel>().InstancePerLifetimeScope();
        builder.RegisterType<FormListWireCatalogResolver>().As<IFormListWireCatalogResolver>().SingleInstance();
        builder.RegisterType<FormListDraftFactory>().As<IFormListDraftFactory>().SingleInstance();
        builder.RegisterType<FormListDraftValidator>().As<IFormListDraftValidator>().SingleInstance();
        builder.RegisterType<FormListDraftSerializer>().As<IFormListDraftSerializer>().SingleInstance();
        builder.RegisterType<RecordJsonTreeProjectionService>().SingleInstance();
        builder.RegisterType<WorkspaceSelectionViewModel>();
        builder.RegisterType<ApplicationWindowService>().As<IApplicationWindowService>().SingleInstance();
        builder.RegisterType<ApplicationNavigationService>().As<IApplicationNavigationService>().SingleInstance();
        builder.RegisterType<AvaloniaUiDispatcher>().As<IUiDispatcher>().SingleInstance();
        builder.RegisterType<WorkspaceCoordinator>().As<IWorkspaceCoordinator>().SingleInstance();
        builder.RegisterType<ReferencePickerService>().As<IReferencePickerService>().SingleInstance();
        builder.RegisterType<WorkspacePathPicker>().As<IWorkspacePathPicker>().SingleInstance();
        builder.RegisterType<WorkspaceSelectionDialogService>().As<IWorkspaceSelectionDialogService>().SingleInstance();
        builder.RegisterType<WorkspaceChangesDialogService>().As<IWorkspaceChangesDialogService>().SingleInstance();

        return builder.Build();
    }
}
