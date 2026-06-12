using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

public class MainViewModelTests
{
    [Fact]
    public async Task ChoosePluginSuggestion_WhenLargePluginIsAlreadyLoading_DoesNotStartSecondLoad()
    {
        var navigationService = new FakeApplicationNavigationService();
        var viewModel = CreateViewModel(navigationService: navigationService);

        viewModel.ChoosePluginSuggestion("Large.esm");
        viewModel.ChoosePluginSuggestion("Large.esm");
        await WaitUntil(() => navigationService.ActivePluginLoadCount == 1);
        await Task.Delay(20);

        navigationService.ActivePluginLoadCount.ShouldBe(1);
    }

    [Fact]
    public async Task PluginSelectionAndSubmit_WhenLargePluginIsAlreadyLoading_DoesNotStartSecondLoad()
    {
        var navigationService = new FakeApplicationNavigationService();
        var viewModel = CreateViewModel(navigationService: navigationService);

        viewModel.SelectedPluginFileName = "Large.esm";
        viewModel.SubmitPluginQuery("Large.esm");
        await WaitUntil(() => navigationService.ActivePluginLoadCount == 1);
        await Task.Delay(20);

        navigationService.ActivePluginLoadCount.ShouldBe(1);
    }

    [Fact]
    public void SelectRecordForComparison_AfterPreloadedPlugin_DoesNotStartActivePluginLoad()
    {
        var navigationService = new FakeApplicationNavigationService();
        var plugin = CreatePlugin("Large.esm", 5000);
        var recordItem = new RecordTreeItemViewModel("0000000A", "PreviewRecord", CreateFormKey(), "STAT", 1);
        var viewModel = CreateViewModel(navigationService: navigationService);
        viewModel.Configure(CreateGame(), runConfiguredGameImport: false, plugin, [new RecordTreeItemViewModel("STAT", string.Empty) { Children = { recordItem } }]);

        viewModel.SelectRecordForComparison(recordItem);

        navigationService.ActivePluginLoadCount.ShouldBe(0);
    }

    private static MainViewModel CreateViewModel(FakeApplicationNavigationService? navigationService = null)
    {
        return new MainViewModel(
            new FakeGameSelectionService(),
            new FakeGameImportReadinessService(),
            new FakePluginSelectionService(),
            new FakeRecordComparisonService(),
            new FakeRecordTreeService(),
            CreateAssetPreviewPaneViewModel(),
            navigationService ?? new FakeApplicationNavigationService(),
            new FakeUserDialogService(),
            new LoggerConfiguration().CreateLogger());
    }

    private static AssetPreviewPaneViewModel CreateAssetPreviewPaneViewModel()
    {
        return new AssetPreviewPaneViewModel(
            new FakeAssetPreviewPathResolverService(),
            new FakeAssetPreviewSceneService(),
            new FakeExternalAssetOpenService(),
            new LoggerConfiguration().CreateLogger());
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

    private static PluginDTO CreatePlugin(string fileName, int recordCount)
    {
        return new PluginDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = new ModKeyDTO
            {
                Name = Path.GetFileNameWithoutExtension(fileName),
                Type = 0,
                FileName = fileName
            },
            LoadOrderIndex = 0,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current,
            HeaderFlags = 0,
            FormVersion = 0,
            RecordCount = recordCount,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow
        };
    }

    private static FormKeyDTO CreateFormKey()
    {
        return new FormKeyDTO
        {
            ModKey = new ModKeyDTO
            {
                Name = "Large",
                Type = 0,
                FileName = "Large.esm"
            },
            Id = 0x0000000A
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
            return SupportedGame.Starfield;
        }

        public ApplicationThemeMode GetThemeMode()
        {
            return ApplicationThemeMode.Dark;
        }

        public ApplicationThemeFamily GetThemeFamily()
        {
            return ApplicationThemeFamily.Semi;
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
        private readonly IReadOnlyList<PluginDTO> Plugins = [CreatePlugin("Large.esm", 5000)];

        public IReadOnlyList<PluginDTO> GetOpenablePlugins(SupportedGame game)
        {
            return Plugins;
        }

        public IReadOnlyList<PluginDTO> SearchOpenablePluginsByFilename(SupportedGame game, string searchFilename)
        {
            return Plugins
                .Where(plugin => plugin.ModKey.FileName.Contains(searchFilename, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public long GetImportedRecordCount(SupportedGame game)
        {
            return 5000;
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
            return
            [
                new RecordTreeEntryDTO
                {
                    Game = game,
                    ModKey = modKey,
                    FormKey = CreateFormKey(),
                    RecordType = "STAT",
                    EditorID = "PreviewRecord",
                    PluginCount = 1
                }
            ];
        }
    }

    private class FakeApplicationNavigationService : IApplicationNavigationService
    {
        private string? CurrentActivePluginLoadKey;

        public int ActivePluginLoadCount { get; private set; }

        public Task ShowMainViewAsync(SupportedGameDTO? selectedGame, bool runConfiguredGameImport)
        {
            CurrentActivePluginLoadKey = null;
            return Task.CompletedTask;
        }

        public Task ShowMainViewAsync(SupportedGameDTO? selectedGame, bool runConfiguredGameImport, PluginDTO selectedPlugin, IList<RecordTreeItemViewModel> recordTreeItems)
        {
            CurrentActivePluginLoadKey = null;
            return Task.CompletedTask;
        }

        public Task ShowSettingsViewAsync()
        {
            CurrentActivePluginLoadKey = null;
            return Task.CompletedTask;
        }

        public Task ShowActivePluginLoadViewAsync(SupportedGameDTO selectedGame, PluginDTO selectedPlugin)
        {
            var activePluginLoadKey = GetActivePluginLoadKey(selectedGame, selectedPlugin);
            if (string.Equals(CurrentActivePluginLoadKey, activePluginLoadKey, StringComparison.Ordinal))
            {
                return Task.CompletedTask;
            }

            CurrentActivePluginLoadKey = activePluginLoadKey;
            ActivePluginLoadCount++;
            return Task.CompletedTask;
        }

        public Task ShowImportProgressViewAsync(SupportedGameDTO selectedGame, bool forceFullReimport)
        {
            CurrentActivePluginLoadKey = null;
            return Task.CompletedTask;
        }

        public Task ShowResetAndImportAllProgressViewAsync()
        {
            CurrentActivePluginLoadKey = null;
            return Task.CompletedTask;
        }

        public void Quit()
        { }

        private static string GetActivePluginLoadKey(SupportedGameDTO selectedGame, PluginDTO selectedPlugin)
        {
            return $"{selectedGame.Game}:{selectedPlugin.ModKey.FileName}".ToUpperInvariant();
        }
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

    private static async Task WaitUntil(Func<bool> condition)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            if (condition())
            {
                return;
            }

            await Task.Delay(20);
        }

        condition().ShouldBeTrue();
    }
}
