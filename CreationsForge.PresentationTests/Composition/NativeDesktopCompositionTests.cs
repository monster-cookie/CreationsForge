using Autofac;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CreationsForge.Bethesda.Assets.Archives;
using CreationsForge.Bethesda.Assets.Nif;
using CreationsForge.Bethesda.Assets.Resources;
using CreationsForge.Composition;
using CreationsForge.Core.Configuration;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.PresentationTests.Headless;
using CreationsForge.Services.Interfaces;
using CreationsForge.Services;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.Composition;

/// <summary>
/// Verifies the independent native desktop composition root resolves its startup and retained presentation graph.
/// </summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class NativeDesktopCompositionTests
{
    /// <summary>
    /// Verifies native startup navigation and retained settings, diagnostics, and asset services resolve without legacy modules.
    /// </summary>
    [AvaloniaFact]
    public async Task Create_WhenResolved_ProvidesNativeDesktopGraph()
    {
        var testDirectory = Directory.CreateTempSubdirectory("CreationsForge-NativeDesktopComposition-");
        try
        {
            var configurationStore = CreateConfigurationStore(testDirectory.FullName);
            await using var container = NativeDesktopComposition.Create(
                configurationStore,
                new LoggerConfiguration().CreateLogger());

            container.Resolve<IApplicationSettingsService>().ShouldNotBeNull();
            container.Resolve<IGameSelectionService>().GetSupportedGames().Count.ShouldBe(3);
            var diagnostics = container.Resolve<IProcessTerminationDiagnosticsService>();
            diagnostics.StartSession("Presentation test", logPath: null);
            container.Resolve<IEnumerable<IAssetArchiveReader>>().Count().ShouldBe(2);
            container.Resolve<IBethesdaAssetProvider>().ShouldNotBeNull();
            container.Resolve<INifPreviewModelReader>().ShouldNotBeNull();
            container.Resolve<IAssetPreviewRenderMeshFactory>().ShouldNotBeNull();
            container.Resolve<IAssetPreviewSceneService>().ShouldNotBeNull();
            container.Resolve<IExternalAssetOpenService>().ShouldNotBeNull();
            container.Resolve<IFormListWorkspaceFactory>().ShouldNotBeNull();
            container.Resolve<INativeWorkspaceCoordinator>().ShouldNotBeNull();
            container.Resolve<INativeReferencePickerService>().ShouldNotBeNull();
            container.Resolve<NativeJsonTreeProjectionService>().ShouldNotBeNull();

            var mainWindow = container.Resolve<MainWindow>();
            container.Resolve<INativeApplicationNavigationService>().ShowWorkspaceShell();
            mainWindow.Show();
            Dispatcher.UIThread.RunJobs();

            mainWindow.Content.ShouldBeOfType<NativeWorkspaceShellView>();
            var shell = (NativeWorkspaceShellView)mainWindow.Content;
            CreationsForge.PresentationTests.Headless.ControlFinder
                .FindByAutomationId<NativeFormListBrowserView>(shell, "NativeFormListBrowserView")
                .ShouldNotBeNull();
            diagnostics.MarkCleanShutdown("Presentation composition verified");
            mainWindow.Close();
        }
        finally
        {
            testDirectory.Delete(recursive: true);
        }
    }

    /// <summary>Verifies each view scope shares one editor admission boundary while both scopes use the application-owned workspace.</summary>
    /// <returns>A task that completes after both view scopes and their application container are disposed.</returns>
    [AvaloniaFact]
    public async Task Create_WhenMultipleViewScopes_UsesScopedAdmissionAndRootWorkspace()
    {
        var testDirectory = Directory.CreateTempSubdirectory("CreationsForge-NativeDesktopComposition-");
        try
        {
            await using var container = NativeDesktopComposition.Create(
                CreateConfigurationStore(testDirectory.FullName),
                new LoggerConfiguration().CreateLogger());
            await using var firstScope = container.BeginLifetimeScope();
            await using var secondScope = container.BeginLifetimeScope();
            var firstArbiter = firstScope.Resolve<INativeWorkspacePresentationOperationArbiter>();
            var secondArbiter = secondScope.Resolve<INativeWorkspacePresentationOperationArbiter>();
            var firstBrowser = firstScope.Resolve<NativeFormListBrowserViewModel>();
            var secondBrowser = secondScope.Resolve<NativeFormListBrowserViewModel>();

            firstArbiter.ShouldBeSameAs(firstScope.Resolve<INativeWorkspacePresentationOperationArbiter>());
            firstArbiter.ShouldNotBeSameAs(secondArbiter);
            firstScope.Resolve<INativeWorkspaceEditParticipant>().ShouldBeSameAs(firstBrowser);
            secondScope.Resolve<INativeWorkspaceEditParticipant>().ShouldBeSameAs(secondBrowser);
            firstBrowser.ShouldNotBeSameAs(secondBrowser);
            firstScope.Resolve<INativeFormListEditorViewModelFactory>()
                .ShouldNotBeSameAs(secondScope.Resolve<INativeFormListEditorViewModelFactory>());
            firstScope.Resolve<INativeWorkspaceCoordinator>()
                .ShouldBeSameAs(secondScope.Resolve<INativeWorkspaceCoordinator>());
            firstScope.Resolve<INativeWorkspaceCoordinator>()
                .ShouldBeSameAs(container.Resolve<INativeWorkspaceCoordinator>());
            var firstChanges = firstScope.Resolve<NativeWorkspaceChangesViewModel>();
            var firstShell = firstScope.Resolve<NativeWorkspaceShellViewModel>();
            firstScope.Resolve<INativeWorkspaceLeaveGuard>().ShouldBeSameAs(firstShell);
            firstShell.ReviewChangesCommand.ShouldBeSameAs(firstChanges.ReviewChangesCommand);
            firstShell.SaveChangesCommand.ShouldBeSameAs(firstChanges.ShowSaveChangesDialogCommand);
            firstShell.DiscardChangesCommand.ShouldBeSameAs(firstChanges.ShowDiscardChangesDialogCommand);
            using (var reservation = await firstShell.ReserveLeaveAsync(
                NativeWorkspaceLeaveReason.OpenWorkspace, TestContext.Current.CancellationToken))
            {
                reservation.ShouldNotBeNull().Disposition.ShouldBe(NativeWorkspaceLeaveDisposition.NoWorkspace);
                firstArbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
                secondArbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
                firstArbiter.TryBeginEditorOperation().ShouldBeNull();
                using var independentEditor = secondArbiter.TryBeginEditorOperation();
                independentEditor.ShouldNotBeNull();
            }

            firstArbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
        }
        finally
        {
            testDirectory.Delete(recursive: true);
        }
    }

    /// <summary>Creates a persisted configuration store whose diagnostics and temporary data remain inside the test directory.</summary>
    /// <param name="testDirectory">The test-owned directory for configuration and application data.</param>
    /// <returns>The configured production store.</returns>
    private static ApplicationConfigurationStore CreateConfigurationStore(string testDirectory)
    {
        var configurationStore = new ApplicationConfigurationStore(Path.Combine(testDirectory, "CreationsForge.Config.json"));
        configurationStore.Save(new ApplicationConfiguration
        {
            ApplicationDataDirectory = testDirectory,
            LoggingDirectory = testDirectory
        });
        return configurationStore;
    }
}
