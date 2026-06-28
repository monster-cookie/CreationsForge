using Autofac;
using Avalonia.Controls;
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
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Serilog;

namespace CreationsForge.DataValidationTests.Validation.UI;

/// <summary>
/// Renders comparison UI samples headlessly and produces assertion cases for data-validation tests.
/// </summary>
public static class SpriggitComparisonUiSpecRunner
{
    /// <summary>
    /// Renders the comparison UI for one validation spec and returns the observed assertion cases.
    /// </summary>
    /// <param name="spec">The validation spec whose UI expectations should be evaluated.</param>
    /// <param name="fixture">The fixture that resolves imported records and comparison services.</param>
    /// <returns>The assertion cases that the calling test method should verify.</returns>
    public static IReadOnlyList<SpriggitComparisonUiAssertionCase> GetAssertionCases(
        ValidationSpec spec,
        SpriggitComparisonUiFixture fixture)
    {
        var cases = new List<SpriggitComparisonUiAssertionCase>();
        var sample = fixture.CreateSample(spec);
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

            AddGridAssertions(cases, mainView, viewModel, sample, spec);
            if (spec.UiComparisonExpectations.Count == 0)
            {
                AddDefaultEditorIdAssertions(cases, mainView, viewModel, sample, spec);
            }
            else
            {
                AddExpectationAssertions(cases, mainView, viewModel, sample, spec);
            }
        }
        finally
        {
            window.Close();
        }

        return cases;
    }

    /// <summary>
    /// Renders the comparison UI for one validation spec and returns the flattened comparison row paths and active
    /// column values observed in the headless main view.
    /// </summary>
    /// <param name="spec">The validation spec whose comparison rows should be inspected.</param>
    /// <param name="fixture">The fixture that resolves imported records and comparison services.</param>
    /// <returns>The rendered comparison rows, including nested child paths.</returns>
    public static IReadOnlyList<SpriggitComparisonUiRenderedRow> GetRenderedRows(
        ValidationSpec spec,
        SpriggitComparisonUiFixture fixture)
    {
        var sample = fixture.CreateSample(spec);
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

            var activeColumnIndex = GetActiveColumnIndex(viewModel);
            var rows = new List<SpriggitComparisonUiRenderedRow>();
            AddRenderedRows(rows, viewModel.RecordComparisonRows, [], activeColumnIndex);
            return rows;
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>
    /// Adds assertions that prove the comparison grid and active column rendered for the selected sample.
    /// </summary>
    /// <param name="cases">The assertion case list to append to.</param>
    /// <param name="mainView">The rendered main view.</param>
    /// <param name="viewModel">The main view model backing the rendered view.</param>
    /// <param name="sample">The sample being rendered.</param>
    /// <param name="spec">The validation spec being evaluated.</param>
    private static void AddGridAssertions(
        IList<SpriggitComparisonUiAssertionCase> cases,
        MainView mainView,
        MainViewModel viewModel,
        SpriggitComparisonUiSample sample,
        ValidationSpec spec)
    {
        var comparisonGrid = ControlFinder.FindByAutomationId<TreeDataGrid>(mainView, "RecordComparisonGrid");
        cases.Add(CreateCase(
            expected: "Present",
            actual: comparisonGrid is null ? "Missing" : "Present",
            message: "Expected the record comparison grid to render for sample '" + spec.SampleName + "'."));
        cases.Add(CreateCase(
            expected: "NonEmpty",
            actual: viewModel.RecordComparisonRows.Count == 0 ? "Empty" : "NonEmpty",
            message: "Expected comparison rows to render for sample '" + spec.SampleName + "'."));
        cases.Add(CreateCase(
            expected: "Contains " + RecordTypeCatalog.GetDisplayLabel(spec.RecordType.RecordID),
            actual: viewModel.RecordComparisonTitleText.Contains(
                RecordTypeCatalog.GetDisplayLabel(spec.RecordType.RecordID),
                StringComparison.Ordinal)
                ? "Contains " + RecordTypeCatalog.GetDisplayLabel(spec.RecordType.RecordID)
                : viewModel.RecordComparisonTitleText,
            message: "Expected the comparison title to identify the record type for sample '" + spec.SampleName + "'."));
        cases.Add(CreateCase(
            expected: sample.Plugin.ModKey.FileName,
            actual: GetActiveColumnHeader(viewModel),
            message: "Expected the active comparison column to match the selected plugin for sample '" + spec.SampleName + "'."));
    }

    /// <summary>
    /// Adds a stable default data-row assertion for specs that do not declare custom UI row expectations.
    /// </summary>
    /// <param name="cases">The assertion case list to append to.</param>
    /// <param name="mainView">The rendered main view.</param>
    /// <param name="viewModel">The main view model backing the rendered view.</param>
    /// <param name="sample">The sample being rendered.</param>
    /// <param name="spec">The validation spec being evaluated.</param>
    private static void AddDefaultEditorIdAssertions(
        IList<SpriggitComparisonUiAssertionCase> cases,
        MainView mainView,
        MainViewModel viewModel,
        SpriggitComparisonUiSample sample,
        ValidationSpec spec)
    {
        if (string.IsNullOrWhiteSpace(sample.Record.EditorID))
        {
            return;
        }

        var row = FindRow(viewModel.RecordComparisonRows, ["EditorID"]);
        cases.Add(CreateCase(
            expected: "Present",
            actual: row is null ? "Missing" : "Present",
            message: "Expected comparison row 'EditorID' to render for sample '" + spec.SampleName + "'."));

        if (row is null)
        {
            return;
        }

        var activeColumnIndex = GetActiveColumnIndex(viewModel);
        cases.Add(CreateCase(
            expected: sample.Record.EditorID,
            actual: row.GetValue(activeColumnIndex),
            message: "Expected comparison row 'EditorID' to match imported record data for sample '" +
                spec.SampleName + "'." + System.Environment.NewLine +
                "Spriggit form key: " + sample.Spriggit.FormKey));
        cases.Add(CreateCase(
            expected: "Present",
            actual: ContainsVisualText(mainView, "EditorID") ? "Present" : "Missing",
            message: "Expected rendered visual tree to contain text 'EditorID' for sample '" +
                spec.SampleName + "'."));
    }

    /// <summary>
    /// Adds assertions for each comparison row expectation declared by the validation spec.
    /// </summary>
    /// <param name="cases">The assertion case list to append to.</param>
    /// <param name="mainView">The rendered main view.</param>
    /// <param name="viewModel">The main view model backing the rendered view.</param>
    /// <param name="sample">The sample being rendered.</param>
    /// <param name="spec">The validation spec being evaluated.</param>
    private static void AddExpectationAssertions(
        IList<SpriggitComparisonUiAssertionCase> cases,
        MainView mainView,
        MainViewModel viewModel,
        SpriggitComparisonUiSample sample,
        ValidationSpec spec)
    {
        var specAssertions = ValidationSpecRunner.GetAssertionCases(spec, sample.Record);
        var activeColumnIndex = GetActiveColumnIndex(viewModel);
        foreach (var expectation in spec.UiComparisonExpectations)
        {
            var row = FindRow(viewModel.RecordComparisonRows, expectation.FieldPath);
            var formattedPath = string.Join("/", expectation.FieldPath);
            cases.Add(CreateCase(
                expected: "Present",
                actual: row is null ? "Missing" : "Present",
                message: "Expected comparison row '" + formattedPath + "' to render for sample '" + spec.SampleName + "'."));

            if (row is null)
            {
                continue;
            }

            var displayValue = row.GetValue(activeColumnIndex);
            cases.Add(CreateCase(
                expected: "NonEmpty",
                actual: string.IsNullOrWhiteSpace(displayValue) ? "Empty" : "NonEmpty",
                message: "Expected comparison row '" + formattedPath + "' to render a value for sample '" + spec.SampleName + "'."));

            if (TryResolveExpectedValue(expectation, specAssertions, out var expectedValue, out var expectedDiagnostic))
            {
                cases.Add(CreateCase(
                    expected: expectedValue,
                    actual: displayValue,
                    message: "Expected comparison row '" + formattedPath + "' to match spec value for sample '" +
                        spec.SampleName + "'." + System.Environment.NewLine +
                        "Spriggit form key: " + sample.Spriggit.FormKey));
            }
            else if (!string.IsNullOrWhiteSpace(expectation.DtoPath))
            {
                cases.Add(CreateCase(
                    expected: "Resolved",
                    actual: expectedDiagnostic,
                    message: "Expected comparison row '" + formattedPath + "' to resolve DTO path '" +
                        expectation.DtoPath + "' for sample '" + spec.SampleName + "'."));
            }

            if (!string.IsNullOrWhiteSpace(expectation.VisualText))
            {
                cases.Add(CreateCase(
                    expected: "Present",
                    actual: ContainsVisualText(mainView, expectation.VisualText) ? "Present" : "Missing",
                    message: "Expected rendered visual tree to contain text '" + expectation.VisualText + "' for sample '" +
                        spec.SampleName + "'."));
            }
        }
    }

    /// <summary>
    /// Creates a headless window containing the main view configured for a validation sample.
    /// </summary>
    /// <param name="sample">The sample to render.</param>
    /// <returns>A window containing the configured main view.</returns>
    private static Window CreateWindowWithMainView(SpriggitComparisonUiSample sample)
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

    /// <summary>
    /// Creates a root lifetime scope that satisfies view-model dependencies not used by the rendered validation path.
    /// </summary>
    /// <returns>A lifetime scope containing test doubles for root-scope services.</returns>
    private static ILifetimeScope CreateRootScope()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<FakeRecordTreeService>()
            .As<IRecordTreeService>()
            .InstancePerLifetimeScope();
        return builder.Build();
    }

    /// <summary>
    /// Creates a lifetime scope for asset preview services used by the preview pane view model.
    /// </summary>
    /// <returns>A lifetime scope containing test doubles for asset preview services.</returns>
    private static ILifetimeScope CreateAssetPreviewScope()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<FakeAssetPreviewSceneService>().As<IAssetPreviewSceneService>();
        return builder.Build();
    }

    /// <summary>
    /// Finds a nested comparison row by following the supplied field path through child rows.
    /// </summary>
    /// <param name="rows">The current comparison rows to search.</param>
    /// <param name="fieldPath">The nested field path expected by the validation spec.</param>
    /// <returns>The matching row, or <c>null</c> when any segment is absent.</returns>
    private static RecordComparisonRowViewModel? FindRow(
        IEnumerable<RecordComparisonRowViewModel> rows,
        IReadOnlyList<string> fieldPath)
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

    /// <summary>
    /// Recursively flattens rendered comparison rows into path/value snapshots for coverage auditing.
    /// </summary>
    /// <param name="rows">The destination row snapshot list.</param>
    /// <param name="sourceRows">The rendered comparison rows to flatten.</param>
    /// <param name="parentPath">The parent path segments that lead to <paramref name="sourceRows"/>.</param>
    /// <param name="activeColumnIndex">The active plugin column index whose display value should be captured.</param>
    private static void AddRenderedRows(
        IList<SpriggitComparisonUiRenderedRow> rows,
        IEnumerable<RecordComparisonRowViewModel> sourceRows,
        IReadOnlyList<string> parentPath,
        int activeColumnIndex)
    {
        foreach (var sourceRow in sourceRows)
        {
            var rowPath = parentPath.Concat([sourceRow.FieldName]).ToList();
            rows.Add(new SpriggitComparisonUiRenderedRow(rowPath, sourceRow.GetValue(activeColumnIndex)));
            AddRenderedRows(rows, sourceRow.Children, rowPath, activeColumnIndex);
        }
    }

    /// <summary>
    /// Resolves the expected rendered value from a literal expectation or DTO validation assertion.
    /// </summary>
    /// <param name="expectation">The UI expectation being evaluated.</param>
    /// <param name="assertions">The DTO validation assertions produced by the shared spec.</param>
    /// <param name="expectedValue">The expected comparison display value when resolution succeeds.</param>
    /// <param name="diagnostic">The resolution diagnostic when no expected value can be resolved.</param>
    /// <returns><c>true</c> when an expected value was resolved.</returns>
    private static bool TryResolveExpectedValue(
        ValidationUiComparisonExpectation expectation,
        IReadOnlyList<ValidationAssertionCase> assertions,
        out string expectedValue,
        out string diagnostic)
    {
        if (expectation.ExpectedDisplayValue is not null)
        {
            expectedValue = expectation.ExpectedDisplayValue;
            diagnostic = string.Empty;
            return true;
        }

        if (!string.IsNullOrWhiteSpace(expectation.DtoPath))
        {
            var assertion = assertions.FirstOrDefault(item =>
                string.Equals(item.DtoPath, expectation.DtoPath, StringComparison.OrdinalIgnoreCase));
            if (assertion is not null)
            {
                expectedValue = assertion.Expected;
                diagnostic = string.Empty;
                return true;
            }

            diagnostic = "No assertion for " + expectation.DtoPath;
            expectedValue = string.Empty;
            return false;
        }

        expectedValue = string.Empty;
        diagnostic = "No expected value source";
        return false;
    }

    /// <summary>
    /// Gets the index of the selected plugin's active comparison column.
    /// </summary>
    /// <param name="viewModel">The main view model containing comparison columns.</param>
    /// <returns>The active column index, or zero when no active column exists.</returns>
    private static int GetActiveColumnIndex(MainViewModel viewModel)
    {
        var activeColumn = viewModel.RecordComparisonColumns
            .Select((column, index) => new { column, index })
            .FirstOrDefault(item => item.column.IsActive);

        return activeColumn?.index ?? 0;
    }

    /// <summary>
    /// Gets the active comparison column header for diagnostics.
    /// </summary>
    /// <param name="viewModel">The main view model containing comparison columns.</param>
    /// <returns>The active column header, or an empty string when no comparison column rendered.</returns>
    private static string GetActiveColumnHeader(MainViewModel viewModel)
    {
        var index = GetActiveColumnIndex(viewModel);
        return index >= 0 && index < viewModel.RecordComparisonColumns.Count
            ? viewModel.RecordComparisonColumns[index].Header
            : string.Empty;
    }

    /// <summary>
    /// Determines whether the rendered visual tree contains an exact text block value.
    /// </summary>
    /// <param name="root">The root control whose visual tree should be searched.</param>
    /// <param name="text">The text expected in the visual tree.</param>
    /// <returns><c>true</c> when a text block with the expected text exists.</returns>
    private static bool ContainsVisualText(Control root, string text)
    {
        return root.GetVisualDescendants()
            .OfType<TextBlock>()
            .Any(textBlock => string.Equals(textBlock.Text, text, StringComparison.Ordinal));
    }

    /// <summary>
    /// Creates a single UI assertion case.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="actual">The actual observed value.</param>
    /// <param name="message">The message to show when the assertion fails.</param>
    /// <returns>A UI assertion case for the calling test method.</returns>
    private static SpriggitComparisonUiAssertionCase CreateCase(string expected, string actual, string message)
    {
        return new SpriggitComparisonUiAssertionCase
        {
            Expected = expected,
            Actual = actual,
            Message = message
        };
    }

    /// <summary>
    /// Supplies the selected game and theme values required by the main view model.
    /// </summary>
    private class FakeGameSelectionService : IGameSelectionService
    {
        private readonly SupportedGameDTO selectedGame;

        /// <summary>
        /// Initializes the fake game selection service with a fixed active game.
        /// </summary>
        /// <param name="selectedGame">The active game to expose to the main view model.</param>
        public FakeGameSelectionService(SupportedGameDTO selectedGame)
        {
            this.selectedGame = selectedGame;
        }

        /// <inheritdoc />
        public IReadOnlyList<SupportedGameDTO> GetSupportedGames()
        {
            return [selectedGame];
        }

        /// <inheritdoc />
        public SupportedGame? GetActiveGame()
        {
            return selectedGame.Game;
        }

        /// <inheritdoc />
        public ApplicationThemeMode GetThemeMode()
        {
            return ApplicationThemeMode.Dark;
        }

        /// <inheritdoc />
        public ApplicationThemeFamily GetThemeFamily()
        {
            return ApplicationThemeFamily.Fluent;
        }

        /// <inheritdoc />
        public void SetActiveGame(SupportedGame game)
        { }

        /// <inheritdoc />
        public void SetThemeMode(ApplicationThemeMode themeMode)
        { }

        /// <inheritdoc />
        public void SetThemeFamily(ApplicationThemeFamily themeFamily)
        { }

        /// <inheritdoc />
        public void SetActiveGameAndThemeMode(SupportedGame game, ApplicationThemeMode themeMode)
        { }

        /// <inheritdoc />
        public void SetActiveGameAndTheme(SupportedGame game, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
        { }

        /// <inheritdoc />
        public void SetTheme(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
        { }
    }

    /// <summary>
    /// Reports that imported data is available for the selected validation game.
    /// </summary>
    private class FakeGameImportReadinessService : IGameImportReadinessService
    {
        /// <inheritdoc />
        public bool HasImportedData(SupportedGame game)
        {
            return true;
        }
    }

    /// <summary>
    /// Exposes a single active plugin to the main view model.
    /// </summary>
    private class FakePluginSelectionService : IPluginSelectionService
    {
        private readonly PluginDTO plugin;

        /// <summary>
        /// Initializes the fake plugin selection service with the selected validation plugin.
        /// </summary>
        /// <param name="plugin">The selected plugin to expose.</param>
        public FakePluginSelectionService(PluginDTO plugin)
        {
            this.plugin = plugin;
        }

        /// <inheritdoc />
        public IReadOnlyList<PluginDTO> GetOpenablePlugins(SupportedGame game)
        {
            return [plugin];
        }

        /// <inheritdoc />
        public IReadOnlyList<PluginDTO> SearchOpenablePluginsByFilename(SupportedGame game, string searchFilename)
        {
            return [plugin];
        }

        /// <inheritdoc />
        public long GetImportedRecordCount(SupportedGame game)
        {
            return 1;
        }
    }

    /// <summary>
    /// Supplies an empty record tree because the test configures the rendered tree directly.
    /// </summary>
    private class FakeRecordTreeService : IRecordTreeService
    {
        /// <inheritdoc />
        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntries(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }
    }

    /// <summary>
    /// Prevents navigation side effects during headless comparison rendering.
    /// </summary>
    private class FakeApplicationNavigationService : IApplicationNavigationService
    {
        /// <inheritdoc />
        public Task ShowMainViewAsync(SupportedGameDTO? selectedGame, bool runConfiguredGameImport)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ShowMainViewAsync(
            SupportedGameDTO? selectedGame,
            bool runConfiguredGameImport,
            PluginDTO selectedPlugin,
            IList<RecordTreeItemViewModel> recordTreeItems)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ShowSettingsViewAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ShowActivePluginLoadViewAsync(SupportedGameDTO selectedGame, PluginDTO selectedPlugin)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ShowImportProgressViewAsync(SupportedGameDTO selectedGame, bool forceFullReimport)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ShowResetAndImportAllProgressViewAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void Quit()
        { }
    }

    /// <summary>
    /// Prevents modal dialog side effects during headless comparison rendering.
    /// </summary>
    private class FakeUserDialogService : IUserDialogService
    {
        /// <inheritdoc />
        public Task<SupportedGameDTO?> ShowGameSelectionAsync(IReadOnlyList<SupportedGameDTO> supportedGames, SupportedGameDTO? selectedGame)
        {
            return Task.FromResult(selectedGame);
        }

        /// <inheritdoc />
        public Task<bool> ShowOpenPluginAsync(OpenPluginDialogViewModel viewModel)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> ShowImportWarningAsync(SupportedGameDTO selectedGame, bool forceFullReimport)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task<bool> ShowResetAndImportAllWarningAsync()
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task ShowHexPayloadAsync(string title, string payloadValue)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ShowErrorAsync(string message)
        {
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Disables asset preview candidates so comparison UI validation stays focused on rendered comparison rows.
    /// </summary>
    private class FakeAssetPreviewPathResolverService : IAssetPreviewPathResolverService
    {
        /// <inheritdoc />
        public IReadOnlyList<AssetPreviewCandidateDTO> GetPreviewCandidates(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return [];
        }

        /// <inheritdoc />
        public bool CanPreviewPath(string? meshPath)
        {
            return false;
        }

        /// <inheritdoc />
        public bool CanOpenExternally(string? meshPath)
        {
            return false;
        }

        /// <inheritdoc />
        public string? ResolveExternalOpenPath(AssetPreviewCandidateDTO candidate)
        {
            return null;
        }
    }

    /// <summary>
    /// Supplies a minimal asset preview model for unreachable preview paths.
    /// </summary>
    private class FakeAssetPreviewSceneService : IAssetPreviewSceneService
    {
        /// <inheritdoc />
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

    /// <summary>
    /// Prevents external asset opening during headless validation.
    /// </summary>
    private class FakeExternalAssetOpenService : IExternalAssetOpenService
    {
        /// <inheritdoc />
        public bool OpenExternally(string assetPath)
        {
            return true;
        }
    }
}
