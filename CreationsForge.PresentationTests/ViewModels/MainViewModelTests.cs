using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using Autofac;
using Avalonia.Media;
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
        viewModel.RecordComparisonTitleText.ShouldBe("Static (STAT) PreviewRecord (0000000A)");
    }

    [Fact]
    public void BuildRecordTree_SortsRecordTypeGroupsAlphabetically()
    {
        var modKey = new ModKeyDTO
        {
            Name = "Starfield",
            Type = 0,
            FileName = "Starfield.esm"
        };
        var entries = new[]
        {
            CreateRecordTreeEntry(modKey, "PERK", "PerkRecord", 0x30),
            CreateRecordTreeEntry(modKey, "BOOK", "BookRecord", 0x10),
            CreateRecordTreeEntry(modKey, "CONT", "ContainerRecord", 0x20)
        };

        var tree = MainViewModel.BuildRecordTree(entries);

        tree.Select(item => item.FormIDText).ShouldBe(["Book (BOOK)", "Container (CONT)", "Perk (PERK)"]);
    }

    [Fact]
    public void BuildRecordTree_FormatsRecordTypeGroupWithFriendlyNameSignatureAndCount()
    {
        var modKey = new ModKeyDTO
        {
            Name = "Starfield",
            Type = 0,
            FileName = "Starfield.esm"
        };
        var entries = new[]
        {
            CreateRecordTreeEntry(modKey, "KYWD", "FirstKeyword", 0x10),
            CreateRecordTreeEntry(modKey, "KYWD", "SecondKeyword", 0x20),
            CreateRecordTreeEntry(modKey, "KYWD", "ThirdKeyword", 0x30)
        };

        var tree = MainViewModel.BuildRecordTree(entries);

        tree.Single().FormIDText.ShouldBe("Keyword (KYWD)");
        tree.Single().DisplayFormIDText.ShouldBe("Keyword (KYWD) (3)");
    }

    [Fact]
    public void PluginSuggestions_ExposeStatusColors()
    {
        var changed = CreatePlugin("Changed.esm", 1);
        changed.ImportState = PluginImportState.Changed;
        var partiallyImported = CreatePlugin("Partial.esm", 1);
        partiallyImported.ImportState = PluginImportState.PartiallyImported;
        var missing = CreatePlugin("Missing.esm", 1);
        missing.ImportState = PluginImportState.Missing;
        missing.ExistsOnDisk = false;
        var failed = CreatePlugin("Failed.esm", 1);
        failed.ImportState = PluginImportState.Failed;
        var unsupported = CreatePlugin("Unsupported.esm", 1);
        unsupported.ImportState = PluginImportState.Unsupported;
        var viewModel = CreateViewModel(pluginSelectionService: new FakePluginSelectionService([changed, partiallyImported, missing, failed, unsupported]));

        viewModel.PluginSuggestions.Select(plugin => plugin.ImportState).ShouldBe(
        [
            PluginImportState.Changed,
            PluginImportState.PartiallyImported,
            PluginImportState.Missing,
            PluginImportState.Failed,
            PluginImportState.Unsupported
        ]);
        GetColor(viewModel.PluginSuggestions[0]).ShouldBe(Color.FromRgb(255, 168, 74));
        GetColor(viewModel.PluginSuggestions[1]).ShouldBe(Color.FromRgb(238, 190, 82));
        GetColor(viewModel.PluginSuggestions[2]).ShouldBe(Color.FromRgb(178, 144, 255));
        GetColor(viewModel.PluginSuggestions[3]).ShouldBe(Color.FromRgb(255, 112, 112));
        GetColor(viewModel.PluginSuggestions[4]).ShouldBe(Color.FromRgb(178, 186, 196));
    }

    [Fact]
    public async Task OpenPluginCommand_WhenDialogSelectsPlugin_LoadsSelectedPlugin()
    {
        var plugin = CreatePlugin("Small.esm", 10);
        var dialogService = new FakeUserDialogService
        {
            OpenPluginDialogAccepted = true
        };
        var viewModel = CreateViewModel(
            pluginSelectionService: new FakePluginSelectionService([plugin]),
            userDialogService: dialogService);

        viewModel.OpenPluginCommand.Execute(null);
        await WaitUntil(() => viewModel.StatusText.Contains("Small.esm", StringComparison.OrdinalIgnoreCase));

        viewModel.StatusText.ShouldContain("Small.esm");
    }

    [Fact]
    public async Task OpenPluginCommand_WhenDialogSelectsEmptyGame_StartsImport()
    {
        var navigationService = new FakeApplicationNavigationService();
        var dialogService = new FakeUserDialogService
        {
            OpenPluginDialogAccepted = true
        };
        var viewModel = CreateViewModel(
            navigationService,
            new FakePluginSelectionService([]),
            dialogService);

        viewModel.OpenPluginCommand.Execute(null);
        await WaitUntil(() => navigationService.ImportProgressCount == 1);

        navigationService.ImportProgressCount.ShouldBe(1);
    }

    [Fact]
    public async Task ChoosePluginSuggestion_WithMissingPlugin_DoesNotLoadRecords()
    {
        var navigationService = new FakeApplicationNavigationService();
        var missing = CreatePlugin("Missing.esm", 100);
        missing.ImportState = PluginImportState.Missing;
        missing.ExistsOnDisk = false;
        var viewModel = CreateViewModel(
            navigationService,
            new FakePluginSelectionService([missing]));

        viewModel.ChoosePluginSuggestion("Missing.esm");
        await Task.Delay(20);

        navigationService.ActivePluginLoadCount.ShouldBe(0);
        viewModel.StatusText.ShouldContain("cannot be opened");
    }

    private static MainViewModel CreateViewModel(
        FakeApplicationNavigationService? navigationService = null,
        IPluginSelectionService? pluginSelectionService = null,
        IUserDialogService? userDialogService = null)
    {
        return new MainViewModel(
            new FakeGameSelectionService(),
            new FakeGameImportReadinessService(),
            pluginSelectionService ?? new FakePluginSelectionService(),
            new FakeRecordComparisonService(),
            new FakeRecordTreeService(),
            CreateRootScope(),
            CreateAssetPreviewPaneViewModel(),
            navigationService ?? new FakeApplicationNavigationService(),
            userDialogService ?? new FakeUserDialogService(),
            new LoggerConfiguration().CreateLogger());
    }

    private static AssetPreviewPaneViewModel CreateAssetPreviewPaneViewModel()
    {
        return new AssetPreviewPaneViewModel(
            new FakeAssetPreviewPathResolverService(),
            CreateAssetPreviewScope(),
            new FakeExternalAssetOpenService(),
            new LoggerConfiguration().CreateLogger());
    }

    private static ILifetimeScope CreateAssetPreviewScope()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<FakeAssetPreviewSceneService>().As<IAssetPreviewSceneService>();
        return builder.Build();
    }

    private static Color GetColor(PluginSuggestionViewModel plugin)
    {
        return plugin.StatusBrush.ShouldBeOfType<SolidColorBrush>().Color;
    }

    private static ILifetimeScope CreateRootScope()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<FakeRecordTreeService>()
            .As<IRecordTreeService>()
            .InstancePerLifetimeScope();
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

    private static RecordTreeEntryDTO CreateRecordTreeEntry(ModKeyDTO modKey, string recordType, string editorId, uint formId)
    {
        return new RecordTreeEntryDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = modKey,
            FormKey = new FormKeyDTO
            {
                ModKey = modKey,
                Id = formId
            },
            RecordType = recordType,
            EditorID = editorId,
            PluginCount = 1
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

        public void SetActiveGame(SupportedGame game)
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
        private readonly IReadOnlyList<PluginDTO> Plugins;

        public FakePluginSelectionService(IReadOnlyList<PluginDTO>? plugins = null)
        {
            Plugins = plugins ?? [CreatePlugin("Large.esm", 5000)];
        }

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

        public int ImportProgressCount { get; private set; }

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
            ImportProgressCount++;
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
        public bool OpenPluginDialogAccepted { get; set; }

        public Task<SupportedGameDTO?> ShowGameSelectionAsync(IReadOnlyList<SupportedGameDTO> supportedGames, SupportedGameDTO? selectedGame)
        {
            return Task.FromResult(selectedGame);
        }

        public Task<bool> ShowOpenPluginAsync(OpenPluginDialogViewModel viewModel)
        {
            viewModel.SelectedPluginRow = viewModel.PluginRows.FirstOrDefault();
            return Task.FromResult(OpenPluginDialogAccepted);
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
