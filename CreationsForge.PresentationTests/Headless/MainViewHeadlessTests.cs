using Autofac;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.Headless;

public class MainViewHeadlessTests
{
    [AvaloniaFact]
    public void MainView_ShowHeadlessly_CreatesMainContent()
    {
        var window = CreateWindowWithMainView();

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            window.Content.ShouldBeOfType<MainView>();
            var mainView = ControlFinder.FindByAutomationId<MainView>((Control)window.Content!, "MainView");
            mainView.ShouldNotBeNull();
            mainView.Content.ShouldNotBeNull();
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void MainView_ShowHeadlessly_ExposesMainToolbarCommands()
    {
        var window = CreateWindowWithMainView();

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var mainView = (MainView)window.Content!;
            ControlFinder.FindByAutomationId<StackPanel>(mainView, "MainToolbar").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(mainView, "ReimportButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(mainView, "ResetAndImportAllButton").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<Button>(mainView, "SettingsButton").ShouldNotBeNull();
        }
        finally
        {
            window.Close();
        }
    }

    private static Window CreateWindowWithMainView()
    {
        var mainView = CreateMainView();
        mainView.Configure(selectedGame: null, runConfiguredGameImport: false);
        return new Window
        {
            Width = 1200,
            Height = 800,
            Content = mainView
        };
    }

    private static MainView CreateMainView()
    {
        var logger = new LoggerConfiguration().CreateLogger();
        var assetPreviewPaneViewModel = CreateAssetPreviewPaneViewModel(logger);
        var mainViewModel = new MainViewModel(
            new FakeGameSelectionService(),
            new FakeGameImportReadinessService(),
            new FakePluginSelectionService(),
            new FakeRecordComparisonService(),
            new FakeRecordTreeService(),
            CreateRootScope(),
            assetPreviewPaneViewModel,
            new FakeApplicationNavigationService(),
            new FakeUserDialogService(),
            logger);
        var assetPreviewPaneView = new AssetPreviewPaneView(
            assetPreviewPaneViewModel,
            new AssetPreviewRenderMeshFactory(logger),
            logger);
        return new MainView(mainViewModel, assetPreviewPaneView);
    }

    private static AssetPreviewPaneViewModel CreateAssetPreviewPaneViewModel(ILogger logger)
    {
        return new AssetPreviewPaneViewModel(
            new FakeAssetPreviewPathResolverService(),
            CreateAssetPreviewScope(),
            new FakeExternalAssetOpenService(),
            logger);
    }

    private static ILifetimeScope CreateRootScope()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<FakeRecordTreeService>()
            .As<IRecordTreeService>()
            .InstancePerLifetimeScope();
        return builder.Build();
    }

    private static ILifetimeScope CreateAssetPreviewScope()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<FakeAssetPreviewSceneService>().As<IAssetPreviewSceneService>();
        return builder.Build();
    }

    private static SupportedGameDTO CreateGame()
    {
        return new SupportedGameDTO
        {
            Game = SupportedGame.Starfield,
            Name = nameof(SupportedGame.Starfield),
            DisplayName = "Starfield"
        };
    }

    private static PluginDTO CreatePlugin()
    {
        return new PluginDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = new ModKeyDTO
            {
                Name = "Starfield",
                Type = 0,
                FileName = "Starfield.esm"
            },
            LoadOrderIndex = 0,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current,
            HeaderFlags = 0,
            FormVersion = 0,
            RecordCount = 1,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow
        };
    }

    private class FakeGameSelectionService : IGameSelectionService
    {
        public IReadOnlyList<SupportedGameDTO> GetSupportedGames()
        {
            return [CreateGame()];
        }

        public SupportedGame? GetActiveGame()
        {
            return null;
        }

        public ApplicationThemeMode GetThemeMode()
        {
            return ApplicationThemeMode.Dark;
        }

        public ApplicationThemeFamily GetThemeFamily()
        {
            return ApplicationThemeFamily.Fluent;
        }

        public void SetActiveGame(SupportedGame game)
        { }

        public void SetThemeMode(ApplicationThemeMode themeMode)
        { }

        public void SetThemeFamily(ApplicationThemeFamily themeFamily)
        { }

        public void SetActiveGameAndThemeMode(SupportedGame game, ApplicationThemeMode themeMode)
        { }

        public void SetActiveGameAndTheme(SupportedGame game, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
        { }

        public void SetTheme(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
        { }
    }

    private class FakeGameImportReadinessService : IGameImportReadinessService
    {
        public bool HasImportedData(SupportedGame game)
        {
            return true;
        }
    }

    private class FakePluginSelectionService : IPluginSelectionService
    {
        public IReadOnlyList<PluginDTO> GetOpenablePlugins(SupportedGame game)
        {
            return [CreatePlugin()];
        }

        public IReadOnlyList<PluginDTO> SearchOpenablePluginsByFilename(SupportedGame game, string searchFilename)
        {
            return [CreatePlugin()];
        }

        public long GetImportedRecordCount(SupportedGame game)
        {
            return 1;
        }
    }

    private class FakeRecordComparisonService : IRecordComparisonService
    {
        public RecordComparisonDTO GetRecordComparison(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return new RecordComparisonDTO
            {
                RecordType = recordType,
                FormKey = formKey,
                EditorID = "PreviewRecord"
            };
        }
    }

    private class FakeRecordTreeService : IRecordTreeService
    {
        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntries(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }
    }

    private class FakeApplicationNavigationService : IApplicationNavigationService
    {
        public Task ShowMainViewAsync(SupportedGameDTO? selectedGame, bool runConfiguredGameImport)
        {
            return Task.CompletedTask;
        }

        public Task ShowMainViewAsync(SupportedGameDTO? selectedGame, bool runConfiguredGameImport, PluginDTO selectedPlugin, IList<RecordTreeItemViewModel> recordTreeItems)
        {
            return Task.CompletedTask;
        }

        public Task ShowSettingsViewAsync()
        {
            return Task.CompletedTask;
        }

        public Task ShowActivePluginLoadViewAsync(SupportedGameDTO selectedGame, PluginDTO selectedPlugin)
        {
            return Task.CompletedTask;
        }

        public Task ShowImportProgressViewAsync(SupportedGameDTO selectedGame, bool forceFullReimport)
        {
            return Task.CompletedTask;
        }

        public Task ShowResetAndImportAllProgressViewAsync()
        {
            return Task.CompletedTask;
        }

        public void Quit()
        { }
    }

    private class FakeUserDialogService : IUserDialogService
    {
        public Task<SupportedGameDTO?> ShowGameSelectionAsync(IReadOnlyList<SupportedGameDTO> supportedGames, SupportedGameDTO? selectedGame)
        {
            return Task.FromResult(selectedGame);
        }

        public Task<bool> ShowImportWarningAsync(SupportedGameDTO selectedGame, bool forceFullReimport)
        {
            return Task.FromResult(true);
        }

        public Task<bool> ShowResetAndImportAllWarningAsync()
        {
            return Task.FromResult(true);
        }

        public Task ShowHexPayloadAsync(string title, string payloadValue)
        {
            return Task.CompletedTask;
        }

        public Task ShowErrorAsync(string message)
        {
            return Task.CompletedTask;
        }
    }

    private class FakeAssetPreviewPathResolverService : IAssetPreviewPathResolverService
    {
        public IReadOnlyList<AssetPreviewCandidateDTO> GetPreviewCandidates(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return [];
        }

        public bool CanPreviewPath(string? meshPath)
        {
            return false;
        }

        public bool CanOpenExternally(string? meshPath)
        {
            return false;
        }

        public string? ResolveExternalOpenPath(AssetPreviewCandidateDTO candidate)
        {
            return null;
        }
    }

    private class FakeAssetPreviewSceneService : IAssetPreviewSceneService
    {
        public AssetPreviewModelDTO CreatePreview(AssetPreviewCandidateDTO candidate, out string statusMessage)
        {
            statusMessage = "Loaded preview.";
            return new AssetPreviewModelDTO
            {
                DisplayName = "Preview",
                SourcePath = candidate.MeshPath
            };
        }
    }

    private class FakeExternalAssetOpenService : IExternalAssetOpenService
    {
        public bool OpenExternally(string assetPath)
        {
            return true;
        }
    }
}
