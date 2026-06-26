using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

public class ImportProgressViewModelTests
{
    [Fact]
    public async Task StartImportAsync_WithSelectedGame_ShowsConfiguredGameForAllProgress()
    {
        var workflowService = new FakeGameImportWorkflowService();
        var viewModel = CreateViewModel(gameImportWorkflowService: workflowService);
        viewModel.Configure(CreateGame(SupportedGame.Starfield, "Starfield"), forceFullReimport: false);

        await viewModel.StartImportAsync();
        await WaitUntil(() => viewModel.CurrentGameText == "Current game: Starfield");

        viewModel.CurrentGameText.ShouldBe("Current game: Starfield");
        workflowService.ImportedGame.ShouldBe(SupportedGame.Starfield);
    }

    [Fact]
    public async Task StartImportAsync_WhenResetAndImportAllProgressReportsGame_ShowsCurrentGame()
    {
        var allGamesWorkflowService = new FakeAllGamesImportWorkflowService(SupportedGame.Skyrim);
        var viewModel = CreateViewModel(allGamesImportWorkflowService: allGamesWorkflowService);
        viewModel.ConfigureResetAndImportAll();

        await viewModel.StartImportAsync();
        await WaitUntil(() => viewModel.CurrentGameText == "Current game: Skyrim");

        viewModel.CurrentGameText.ShouldBe("Current game: Skyrim");
    }

    private static ImportProgressViewModel CreateViewModel(
        IGameImportWorkflowService? gameImportWorkflowService = null,
        IAllGamesImportWorkflowService? allGamesImportWorkflowService = null)
    {
        return new ImportProgressViewModel(
            gameImportWorkflowService ?? new FakeGameImportWorkflowService(),
            allGamesImportWorkflowService ?? new FakeAllGamesImportWorkflowService(SupportedGame.Starfield),
            new FakeApplicationNavigationService(),
            new FakeUserDialogService(),
            new FakeProcessTerminationDiagnosticsService(),
            Log.Logger);
    }

    private static SupportedGameDTO CreateGame(SupportedGame game, string displayName)
    {
        return new SupportedGameDTO
        {
            Game = game,
            Name = game.ToString(),
            DisplayName = displayName
        };
    }

    private static async Task WaitUntil(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(2);
        while (!condition() && DateTime.UtcNow < deadline)
        {
            await Task.Delay(10);
        }
    }

    private sealed class FakeGameImportWorkflowService : IGameImportWorkflowService
    {
        public SupportedGame? ImportedGame { get; private set; }

        public Task<GameImportWorkflowResultDTO> ImportAsync(
            SupportedGame game,
            bool forceFullReimport = false,
            IProgress<GameImportProgressDTO>? progress = null,
            CancellationToken cancellationToken = default)
        {
            ImportedGame = game;
            progress?.Report(new GameImportProgressDTO
            {
                Game = game,
                StatusText = "Importing records",
                DetailText = "Testing progress.",
                IsIndeterminate = true
            });
            return Task.FromResult(new GameImportWorkflowResultDTO
            {
                ImportResult = new GameImportResultDTO
                {
                    Game = game
                }
            });
        }
    }

    private sealed class FakeAllGamesImportWorkflowService : IAllGamesImportWorkflowService
    {
        private readonly SupportedGame ProgressGame;

        public FakeAllGamesImportWorkflowService(SupportedGame progressGame)
        {
            ProgressGame = progressGame;
        }

        public Task<AllGamesImportWorkflowResultDTO> ImportAllAsync(
            bool resetDatabase,
            IProgress<GameImportProgressDTO>? progress = null,
            CancellationToken cancellationToken = default)
        {
            progress?.Report(new GameImportProgressDTO
            {
                Game = ProgressGame,
                StatusText = "Importing records",
                DetailText = "Testing all-games progress.",
                IsIndeterminate = true
            });
            return Task.FromResult(new AllGamesImportWorkflowResultDTO
            {
                DatabaseReset = resetDatabase,
                ImportResults =
                [
                    new GameImportResultDTO
                    {
                        Game = ProgressGame
                    }
                ]
            });
        }
    }

    private sealed class FakeApplicationNavigationService : IApplicationNavigationService
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

    private sealed class FakeUserDialogService : IUserDialogService
    {
        public Task<SupportedGameDTO?> ShowGameSelectionAsync(IReadOnlyList<SupportedGameDTO> supportedGames, SupportedGameDTO? selectedGame)
        {
            return Task.FromResult(selectedGame);
        }

        public Task<bool> ShowOpenPluginAsync(OpenPluginDialogViewModel viewModel)
        {
            return Task.FromResult(false);
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

    private sealed class FakeProcessTerminationDiagnosticsService : IProcessTerminationDiagnosticsService
    {
        public CancellationToken TerminationToken => CancellationToken.None;

        public void StartSession(string surfaceName, string? logPath)
        { }

        public void UpdateHeartbeat(string phaseName, GameImportProgressDTO? progress = null)
        { }

        public void MarkCleanShutdown(string reason)
        { }

        public void Dispose()
        { }
    }
}
