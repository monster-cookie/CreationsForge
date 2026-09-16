using Autofac;
using Avalonia.Controls;
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
/// Verifies the independent plugin desktop composition root resolves its startup and retained presentation graph.
/// </summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class DesktopCompositionTests
{
    /// <summary>
    /// Verifies plugin startup navigation and retained settings, diagnostics, and asset services resolve without legacy modules.
    /// </summary>
    [AvaloniaFact]
    public async Task Create_WhenResolved_ProvidesDesktopGraph()
    {
        var testDirectory = Directory.CreateTempSubdirectory("CreationsForge-DesktopComposition-");
        try
        {
            var configurationStore = CreateConfigurationStore(testDirectory.FullName);
            await using var container = DesktopComposition.Create(
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
            container.Resolve<IWorkspaceCoordinator>().ShouldNotBeNull();
            container.Resolve<IReferencePickerService>().ShouldNotBeNull();
            container.Resolve<RecordJsonTreeProjectionService>().ShouldNotBeNull();

            var mainWindow = container.Resolve<MainWindow>();
            container.Resolve<IApplicationNavigationService>().ShowWorkspaceShell();
            mainWindow.Show();
            Dispatcher.UIThread.RunJobs();

            mainWindow.Content.ShouldBeOfType<WorkspaceShellView>();
            var shell = (WorkspaceShellView)mainWindow.Content;
            CreationsForge.PresentationTests.Headless.ControlFinder
                .FindByAutomationId<MajorRecordBrowserView>(shell, "MajorRecordBrowserView")
                .ShouldNotBeNull();
            var tabs = CreationsForge.PresentationTests.Headless.ControlFinder
                .FindByAutomationId<TabControl>(shell, "WorkspaceBrowserTabs")
                .ShouldNotBeNull();
            tabs.SelectedIndex = 1;
            Dispatcher.UIThread.RunJobs();
            CreationsForge.PresentationTests.Headless.ControlFinder
                .FindByAutomationId<FormListBrowserView>(shell, "FormListBrowserView")
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
        var testDirectory = Directory.CreateTempSubdirectory("CreationsForge-DesktopComposition-");
        try
        {
            await using var container = DesktopComposition.Create(
                CreateConfigurationStore(testDirectory.FullName),
                new LoggerConfiguration().CreateLogger());
            await using var firstScope = container.BeginLifetimeScope();
            await using var secondScope = container.BeginLifetimeScope();
            var firstArbiter = firstScope.Resolve<IWorkspacePresentationOperationArbiter>();
            var secondArbiter = secondScope.Resolve<IWorkspacePresentationOperationArbiter>();
            var firstBrowser = firstScope.Resolve<FormListBrowserViewModel>();
            var secondBrowser = secondScope.Resolve<FormListBrowserViewModel>();

            firstArbiter.ShouldBeSameAs(firstScope.Resolve<IWorkspacePresentationOperationArbiter>());
            firstArbiter.ShouldNotBeSameAs(secondArbiter);
            firstScope.Resolve<IWorkspaceEditParticipant>().ShouldBeSameAs(firstBrowser);
            secondScope.Resolve<IWorkspaceEditParticipant>().ShouldBeSameAs(secondBrowser);
            firstBrowser.ShouldNotBeSameAs(secondBrowser);
            firstScope.Resolve<IFormListEditorViewModelFactory>()
                .ShouldNotBeSameAs(secondScope.Resolve<IFormListEditorViewModelFactory>());
            firstScope.Resolve<IWorkspaceCoordinator>()
                .ShouldBeSameAs(secondScope.Resolve<IWorkspaceCoordinator>());
            firstScope.Resolve<IWorkspaceCoordinator>()
                .ShouldBeSameAs(container.Resolve<IWorkspaceCoordinator>());
            var firstChanges = firstScope.Resolve<WorkspaceChangesViewModel>();
            var firstShell = firstScope.Resolve<WorkspaceShellViewModel>();
            firstScope.Resolve<IWorkspaceLeaveGuard>().ShouldBeSameAs(firstShell);
            firstShell.ReviewChangesCommand.ShouldBeSameAs(firstChanges.ReviewChangesCommand);
            firstShell.SaveChangesCommand.ShouldBeSameAs(firstChanges.ShowSaveChangesDialogCommand);
            firstShell.DiscardChangesCommand.ShouldBeSameAs(firstChanges.ShowDiscardChangesDialogCommand);
            using (var reservation = await firstShell.ReserveLeaveAsync(
                WorkspaceLeaveReason.OpenWorkspace, TestContext.Current.CancellationToken))
            {
                reservation.ShouldNotBeNull().Disposition.ShouldBe(WorkspaceLeaveDisposition.NoWorkspace);
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
