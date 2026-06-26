using Autofac;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.Headless;

[Trait("Category", "RequiresStarfield")]
public class RecordComparisonSpriggitHeadlessTests : IClassFixture<SpriggitComparisonHeadlessFixture>
{
    private readonly SpriggitComparisonHeadlessFixture fixture;

    public RecordComparisonSpriggitHeadlessTests(SpriggitComparisonHeadlessFixture fixture)
    {
        this.fixture = fixture;
    }

    [AvaloniaFact]
    public void MainView_RecordComparisonGrid_RendersSpriggitBackedComparisonValues()
    {
        var cases = new[]
        {
            new ComparisonCase(
                SupportedGame.Fallout4,
                RecordTypeCatalog.GameSetting.RecordID,
                "GameSettings",
                ["Data"],
                [new ComparisonExpectation(["Data"], "Data")]),
            new ComparisonCase(
                SupportedGame.Skyrim,
                RecordTypeCatalog.Global.RecordID,
                "Globals",
                ["Data"],
                [new ComparisonExpectation(["Data"], "Data")]),
            new ComparisonCase(
                SupportedGame.Starfield,
                RecordTypeCatalog.MiscItem.RecordID,
                "MiscItems",
                ["Model.File", "Value"],
                [
                    new ComparisonExpectation(["Value"], "Value"),
                    new ComparisonExpectation(["Model", "File"], "Model.File")
                ]),
            new ComparisonCase(
                SupportedGame.Starfield,
                RecordTypeCatalog.Perk.RecordID,
                "Perks",
                ["EditorID"],
                [
                    new ComparisonExpectation(["Ranks", "Rank [0]", "Effects", "Effect [0]", "Value"], null)
                ],
                record => ((PerkDTO)record).Ranks.Any(rank => rank.Effects.Count > 0),
                "Ranks")
        };

        foreach (var comparisonCase in cases)
        {
            AssertComparisonCase(comparisonCase);
        }
    }

    private void AssertComparisonCase(ComparisonCase comparisonCase)
    {
        var sample = fixture.CreateSample(
            comparisonCase.Game,
            comparisonCase.RecordType,
            comparisonCase.FolderName,
            comparisonCase.RequiredPaths,
            comparisonCase.RecordPredicate);
        var window = CreateWindowWithMainView(sample);

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var mainView = (MainView)window.Content!;
            var viewModel = (MainViewModel)mainView.DataContext!;
            var selectedRecord = viewModel.RecordTreeItems.Single().Children.Single();
            viewModel.SelectRecordForComparison(selectedRecord);
            Dispatcher.UIThread.RunJobs();

            var comparisonGrid = ControlFinder.FindByAutomationId<TreeDataGrid>(mainView, "RecordComparisonGrid");
            comparisonGrid.ShouldNotBeNull();
            viewModel.RecordComparisonColumns.Select(column => column.Header).ShouldBe([sample.Plugin.ModKey.FileName]);
            viewModel.RecordComparisonTitleText.ShouldContain(sample.RecordType);
            viewModel.RecordComparisonRows.ShouldNotBeEmpty();

            foreach (var expectation in comparisonCase.Expectations)
            {
                var row = FindRow(viewModel.RecordComparisonRows, expectation.FieldPath);
                row.ShouldNotBeNull($"Comparison row '{string.Join("/", expectation.FieldPath)}' should be present for sample '{sample.Spriggit.FilePath}'.");
                var displayValue = row.GetValue(0);
                displayValue.ShouldNotBeNullOrWhiteSpace($"Comparison row '{string.Join("/", expectation.FieldPath)}' should render a value for sample '{sample.Spriggit.FilePath}'.");
                if (!string.IsNullOrWhiteSpace(expectation.SpriggitPath))
                {
                    sample.Spriggit.ScalarMatchesDisplayValue(expectation.SpriggitPath, displayValue)
                        .ShouldBeTrue($"Comparison row '{string.Join("/", expectation.FieldPath)}' value '{displayValue}' should match Spriggit path '{expectation.SpriggitPath}' in '{sample.Spriggit.FilePath}'.");
                }
            }

            ShouldContainVisualText(mainView, comparisonCase.VisualText ?? comparisonCase.Expectations[0].FieldPath[^1]);
        }
        finally
        {
            window.Close();
        }
    }

    private static Window CreateWindowWithMainView(SpriggitComparisonHeadlessFixture.ComparisonSample sample)
    {
        var selectedGame = new SupportedGameDTO
        {
            Game = sample.Game,
            Name = sample.Game.ToString(),
            DisplayName = sample.Game.ToString()
        };
        var logger = new LoggerConfiguration().CreateLogger();
        var assetPreviewPaneViewModel = new AssetPreviewPaneViewModel(
            new FakeAssetPreviewPathResolverService(),
            CreateAssetPreviewScope(),
            new FakeExternalAssetOpenService(),
            logger);
        var mainViewModel = new MainViewModel(
            new FakeGameSelectionService(selectedGame),
            new FakeGameImportReadinessService(),
            new FakePluginSelectionService(sample.Plugin),
            sample.ComparisonService,
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
        var mainView = new MainView(mainViewModel, assetPreviewPaneView);
        mainView.Configure(
            selectedGame,
            runConfiguredGameImport: false,
            sample.Plugin,
            [
                new RecordTreeItemViewModel(
                    sample.RecordType,
                    string.Empty)
                {
                    Children =
                    {
                        new RecordTreeItemViewModel(
                            sample.Record.FormKey.Id.ToString("X8"),
                            sample.Record.EditorID,
                            sample.Record.FormKey,
                            sample.RecordType,
                            1)
                    }
                }
            ]);

        return new Window
        {
            Width = 1200,
            Height = 800,
            Content = mainView
        };
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

    private static RecordComparisonRowViewModel? FindRow(IEnumerable<RecordComparisonRowViewModel> rows, IReadOnlyList<string> fieldPath)
    {
        var currentRows = rows;
        RecordComparisonRowViewModel? currentRow = null;
        foreach (var fieldName in fieldPath)
        {
            currentRow = currentRows.FirstOrDefault(row => string.Equals(row.FieldName, fieldName, StringComparison.Ordinal));
            if (currentRow is null)
            {
                return null;
            }

            currentRows = currentRow.Children;
        }

        return currentRow;
    }

    private static void ShouldContainVisualText(Control root, string text)
    {
        root.GetVisualDescendants()
            .OfType<TextBlock>()
            .Any(textBlock => string.Equals(textBlock.Text, text, StringComparison.Ordinal))
            .ShouldBeTrue($"Expected visual tree to contain text '{text}'.");
    }

    private sealed record ComparisonCase(
        SupportedGame Game,
        string RecordType,
        string FolderName,
        IReadOnlyList<string> RequiredPaths,
        IReadOnlyList<ComparisonExpectation> Expectations,
        Func<RecordDTO, bool>? RecordPredicate = null,
        string? VisualText = null);

    private sealed record ComparisonExpectation(
        IReadOnlyList<string> FieldPath,
        string? SpriggitPath);

    private class FakeGameSelectionService : IGameSelectionService
    {
        private readonly SupportedGameDTO selectedGame;

        public FakeGameSelectionService(SupportedGameDTO selectedGame)
        {
            this.selectedGame = selectedGame;
        }

        public IReadOnlyList<SupportedGameDTO> GetSupportedGames()
        {
            return [selectedGame];
        }

        public SupportedGame? GetActiveGame()
        {
            return selectedGame.Game;
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
        private readonly PluginDTO plugin;

        public FakePluginSelectionService(PluginDTO plugin)
        {
            this.plugin = plugin;
        }

        public IReadOnlyList<PluginDTO> GetOpenablePlugins(SupportedGame game)
        {
            return [plugin];
        }

        public IReadOnlyList<PluginDTO> SearchOpenablePluginsByFilename(SupportedGame game, string searchFilename)
        {
            return [plugin];
        }

        public long GetImportedRecordCount(SupportedGame game)
        {
            return 1;
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
