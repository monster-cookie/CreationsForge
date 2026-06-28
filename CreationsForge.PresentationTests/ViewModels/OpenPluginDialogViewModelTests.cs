using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.ViewModels;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <summary>
/// Tests Open Plugin dialog state that should remain independent of pointer hover behavior.
/// </summary>
public class OpenPluginDialogViewModelTests
{
    /// <summary>
    /// Verifies the dialog starts without an implicit plugin selection so opening requires an explicit row selection.
    /// </summary>
    [Fact]
    public void Constructor_WithOpenablePlugins_DoesNotSelectFirstPlugin()
    {
        var service = new TestPluginSelectionService
        {
            Plugins = [CreatePlugin("First.esm"), CreatePlugin("Second.esm")]
        };

        var viewModel = new OpenPluginDialogViewModel(CreateGames(), CreateGames()[0], service);

        viewModel.SelectedPluginRow.ShouldBeNull();
        viewModel.CanOpenSelectedPlugin.ShouldBeFalse();
        viewModel.CanRunPrimaryAction.ShouldBeFalse();
    }

    /// <summary>
    /// Verifies filtering keeps a selected row only when the selected plugin remains visible.
    /// </summary>
    [Fact]
    public void SearchText_WhenSelectedPluginRemainsVisible_PreservesSelection()
    {
        var service = new TestPluginSelectionService
        {
            Plugins = [CreatePlugin("First.esm"), CreatePlugin("Second.esm")]
        };
        var viewModel = new OpenPluginDialogViewModel(CreateGames(), CreateGames()[0], service);
        viewModel.SelectedPluginRow = viewModel.PluginRows.First(row => row.FileName == "Second.esm");

        viewModel.SearchText = "Second";

        viewModel.SelectedPluginRow.ShouldNotBeNull();
        viewModel.SelectedPluginRow.FileName.ShouldBe("Second.esm");
    }

    /// <summary>
    /// Verifies filtering clears a selected row when that plugin is no longer visible.
    /// </summary>
    [Fact]
    public void SearchText_WhenSelectedPluginIsFilteredOut_ClearsSelection()
    {
        var service = new TestPluginSelectionService
        {
            Plugins = [CreatePlugin("First.esm"), CreatePlugin("Second.esm")]
        };
        var viewModel = new OpenPluginDialogViewModel(CreateGames(), CreateGames()[0], service);
        viewModel.SelectedPluginRow = viewModel.PluginRows.First(row => row.FileName == "Second.esm");

        viewModel.SearchText = "First";

        viewModel.SelectedPluginRow.ShouldBeNull();
    }

    /// <summary>
    /// Creates supported game options for dialog construction.
    /// </summary>
    /// <returns>The supported game options used by the test dialog.</returns>
    private static IReadOnlyList<SupportedGameDTO> CreateGames()
    {
        return
        [
            new SupportedGameDTO
            {
                Game = SupportedGame.Starfield,
                Name = nameof(SupportedGame.Starfield),
                DisplayName = "Starfield"
            }
        ];
    }

    /// <summary>
    /// Creates a current imported plugin row for dialog selection tests.
    /// </summary>
    /// <param name="fileName">The plugin filename to place on the row.</param>
    /// <returns>A plugin DTO with enough data for dialog display.</returns>
    private static PluginDTO CreatePlugin(string fileName)
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
            RecordCount = 1,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Supplies deterministic plugin rows to the Open Plugin dialog view model.
    /// </summary>
    private sealed class TestPluginSelectionService : IPluginSelectionService
    {
        /// <summary>
        /// Gets or sets the plugin rows returned from openable and search queries.
        /// </summary>
        public IReadOnlyList<PluginDTO> Plugins { get; set; } = [];

        /// <inheritdoc />
        public IReadOnlyList<PluginDTO> GetOpenablePlugins(SupportedGame game)
        {
            return Plugins;
        }

        /// <inheritdoc />
        public IReadOnlyList<PluginDTO> SearchOpenablePluginsByFilename(SupportedGame game, string searchFilename)
        {
            return Plugins
                .Where(plugin => plugin.ModKey.FileName.Contains(searchFilename, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <inheritdoc />
        public long GetImportedRecordCount(SupportedGame game)
        {
            return Plugins.Sum(plugin => plugin.RecordCount);
        }
    }
}
