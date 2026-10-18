using CreationsForge.Engine.Installs;
using CreationsForge.UnitTests.McpHost;
using CreationsForge.Workbench;
using Mutagen.Bethesda.Strings;
using StarfieldMod = Mutagen.Bethesda.Starfield.StarfieldMod;
using StarfieldRelease = Mutagen.Bethesda.Starfield.StarfieldRelease;

namespace CreationsForge.UnitTests.Workbench;

/// <summary>Runs the guided Starfield workflow through the production MCP host.</summary>
[Collection(McpProcessCollection.Name)]
public sealed class WorkbenchStarfieldWorkflowTests
{
    /// <summary>Creates, overrides, compares, saves, and reopens a localized keyword without changing the unrelated source record.</summary>
    [Fact]
    public async Task GuidedWorkflowRoundTripsLocalizedKeywordAndLeavesUnrelatedRecord()
    {
        using var directory = new TestDirectory();
        McpPluginFixtures.WriteNamedSourceKeywords(directory.DirectoryPath);
        var outputPath = Path.Combine(directory.DirectoryPath, McpPluginFixtures.OutputPlugin);
        var viewModel = new WorkbenchViewModel(
            static async (path, token) => await StdioMcpClient.StartAsync(path, token).ConfigureAwait(false),
            settings: null,
            findMcpExecutable: null,
            findDataDirectory: null,
            connectTimeout: TimeSpan.FromSeconds(30),
            operationTimeout: TimeSpan.FromSeconds(60))
        {
            McpExecutablePath = McpHostPaths.McpAssembly(),
            DataDirectory = directory.DirectoryPath,
            SelectedPlugins = McpPluginFixtures.SourcePlugin,
            OutputPath = outputPath,
            MasterStyle = "Full",
            TextStorageMode = "Localized",
            Language = "French",
            SelectedFamily = "Keyword",
        };

        try
        {
            await viewModel.ConnectAsync();
            await viewModel.OpenWorkspaceAsync();
            await viewModel.OpenOutputAsync(createNew: true);
            Assert.Equal("Created " + outputPath + ".", viewModel.Status);
            Assert.True(File.Exists(outputPath));
            Assert.Contains(outputPath, viewModel.Observations, StringComparison.Ordinal);
            Assert.Contains("Localized", viewModel.Observations, StringComparison.Ordinal);

            var firstProcess = StdioMcpClient.LastProcessId;
            await viewModel.ReopenFreshAsync();
            Assert.NotEqual(firstProcess, StdioMcpClient.LastProcessId);
            Assert.Contains("File exists: true", viewModel.Observations, StringComparison.Ordinal);
            Assert.Equal("Reopened " + outputPath + ".", viewModel.Status);

            viewModel.EditorIdFilter = "Keyword";
            await viewModel.FindRecordsAsync();
            viewModel.SelectedHit = viewModel.SearchHits.Single(hit => hit.EditorId == McpPluginFixtures.SourceEditorId);
            await viewModel.OverrideSelectedAsync();
            var link = viewModel.Fields.Single(field => field.Path == "AttractionRule");
            link.FormKeyText = "bad";
            await viewModel.ApplyRecordAsync();
            Assert.Equal("Validation failed.", viewModel.Status);
            Assert.Equal("bad", link.FormKeyText);
            link.FormKeyText = string.Empty;

            var name = viewModel.Fields.Single(field => field.Path == "Name");
            Assert.Contains(name.Translations, row => row.Language == "English" && row.Value == "Source Name");
            name.TargetLanguage = "French";
            name.AddTranslation("French");
            name.Translations.Single(row => row.Language == "French").Value = "Nom modifié";
            await viewModel.ApplyRecordAsync();
            Assert.Equal("Applied in memory. The output file was not saved.", viewModel.Status);

            await viewModel.CompareRecordAsync();
            Assert.Contains("Compare", viewModel.Observations, StringComparison.Ordinal);
            await viewModel.PreviewChangesAsync();
            Assert.Contains("does not write", viewModel.Observations, StringComparison.OrdinalIgnoreCase);
            await viewModel.SaveChangesAsync();
            Assert.Equal("Saved " + outputPath + ".", viewModel.Status);
            Assert.True(Directory.Exists(Path.Combine(directory.DirectoryPath, "Strings")));

            await viewModel.ReopenFreshAsync();
            Assert.Contains("File exists: true", viewModel.Observations, StringComparison.Ordinal);
            Assert.Contains("Nom modifié", viewModel.Observations, StringComparison.Ordinal);
            Assert.Contains("UnrelatedKeyword is unchanged", viewModel.Observations, StringComparison.Ordinal);

            var source = ReadKeywords(directory.DirectoryPath, McpPluginFixtures.SourcePlugin, output: false, Language.English);
            Assert.Equal("Source Name", source[McpPluginFixtures.SourceEditorId]);
            Assert.Equal("Leave Alone", source["UnrelatedKeyword"]);
            var output = ReadKeywords(directory.DirectoryPath, McpPluginFixtures.OutputPlugin, output: true, Language.French);
            Assert.Equal("Nom modifié", output[McpPluginFixtures.SourceEditorId]);
            Assert.False(output.ContainsKey("UnrelatedKeyword"));
        }
        finally
        {
            await viewModel.CloseAsync();
        }
    }

    /// <summary>Saves an empty plugin from installed Starfield into a directory outside the game Data folder.</summary>
    [Fact]
    public async Task InstalledStarfieldSourceSavesOutsideTheGameDirectory()
    {
        var located = InstalledGameData.TryGetDirectory("Starfield");
        var installed = !string.IsNullOrWhiteSpace(located) && File.Exists(Path.Combine(located, "Starfield.esm"));
        Assert.SkipWhen(!installed, "Starfield is not installed.");
        var dataDirectory = located!;

        using var directory = new TestDirectory();
        var outputPath = Path.Combine(directory.DirectoryPath, "InstalledOutput.esp");
        Assert.False(outputPath.StartsWith(dataDirectory, StringComparison.OrdinalIgnoreCase));
        var viewModel = new WorkbenchViewModel(
            static async (path, token) => await StdioMcpClient.StartAsync(path, token).ConfigureAwait(false),
            settings: null,
            findMcpExecutable: null,
            findDataDirectory: null,
            connectTimeout: TimeSpan.FromSeconds(45),
            operationTimeout: TimeSpan.FromMinutes(3))
        {
            McpExecutablePath = McpHostPaths.McpAssembly(),
            DataDirectory = dataDirectory,
            SelectedPlugins = "Starfield.esm",
            OutputPath = outputPath,
            TextStorageMode = "Embedded",
            Language = "English",
        };

        try
        {
            await viewModel.ConnectAsync();
            await viewModel.OpenWorkspaceAsync();
            await viewModel.OpenOutputAsync(createNew: true);
            Assert.Equal("Created " + outputPath + ".", viewModel.Status);
            Assert.True(File.Exists(outputPath));
            Assert.False(outputPath.StartsWith(dataDirectory, StringComparison.OrdinalIgnoreCase));

            await viewModel.ReopenFreshAsync();
            Assert.Contains("File exists: true", viewModel.Observations, StringComparison.Ordinal);
            Assert.Equal("Reopened " + outputPath + ".", viewModel.Status);
        }
        finally
        {
            await viewModel.CloseAsync();
        }
    }

    private static Dictionary<string, string> ReadKeywords(string directory, string plugin, bool output, Language language)
    {
        var starfieldPath = Path.Combine(directory, McpPluginFixtures.StarfieldMaster);
        var sourcePath = Path.Combine(directory, McpPluginFixtures.SourcePlugin);
        using var starfield = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(starfieldPath)
            .WithNoLoadOrder()
            .Construct();
        using var source = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(sourcePath)
            .WithKnownMasters(starfield)
            .Construct();
        using var mod = output
            ? StarfieldMod.Create(StarfieldRelease.Starfield)
                .FromPath(Path.Combine(directory, plugin))
                .WithKnownMasters(starfield, source)
                .Construct()
            : null;
        var records = (output ? mod! : source).Keywords;
        return records
            .Where(keyword => keyword.EditorID is not null)
            .ToDictionary(
                keyword => keyword.EditorID!,
                keyword => LanguageText(keyword.Name, language),
                StringComparer.Ordinal);
    }

    private static string LanguageText(ITranslatedStringGetter? name, Language language)
    {
        if (name is null)
        {
            return string.Empty;
        }

        foreach (var pair in name.ToArray())
        {
            if (pair.Key == language)
            {
                return pair.Value ?? string.Empty;
            }
        }

        return string.Empty;
    }
}
