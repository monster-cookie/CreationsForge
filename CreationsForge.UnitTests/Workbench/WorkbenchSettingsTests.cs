using System.Text.Json.Nodes;
using CreationsForge.Workbench;

namespace CreationsForge.UnitTests.Workbench;

/// <summary>Verifies that Workbench selections round-trip through the historical configuration file.</summary>
public sealed class WorkbenchSettingsTests
{
    /// <summary>Leaves a missing configuration file uncreated until a save.</summary>
    [Fact]
    public void MissingFileStaysMissingUntilSave()
    {
        var directory = Path.Combine(Path.GetTempPath(), "creationsforge-settings-" + Guid.NewGuid().ToString("N"));
        var path = Path.Combine(directory, "CreationsForge.Config.json");
        try
        {
            var store = new WorkbenchSettingsStore(path);
            var state = store.Load();

            Assert.Equal("Starfield", state.Release);
            Assert.Equal("English", state.Language);
            Assert.False(File.Exists(path));

            store.Save(state);
            var json = File.ReadAllText(path);
            Assert.Contains("\"ThemeMode\": \"Dark\"", json, StringComparison.Ordinal);
            Assert.Contains("\"ThemeFamily\": \"Semi\"", json, StringComparison.Ordinal);
            Assert.Contains("\"PreferEspOverMatchingEsm\": true", json, StringComparison.Ordinal);
            Assert.Contains("\"NifSkopeExecutablePath\": null", json, StringComparison.Ordinal);
            Assert.Contains("\"Workbench\"", json, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    /// <summary>Updates Workbench fields and keeps theme and other unknown properties.</summary>
    [Fact]
    public void SavePreservesUnknownPropertiesAndExistingNames()
    {
        var directory = Path.Combine(Path.GetTempPath(), "creationsforge-settings-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "CreationsForge.Config.json");
        File.WriteAllText(
            path,
            """
            {
              "activeGame": "Fallout4",
              "themeMode": "Light",
              "recordTextLanguage": "French",
              "customFutureField": { "keep": true },
              "Workbench": {
                "mcpExecutablePath": "C:\\old\\CreationsForge.Mcp.dll",
                "Releases": {
                  "Fallout4": {
                    "dataDirectory": "C:\\Games\\Fallout4\\Data",
                    "notes": "keep me"
                  }
                }
              }
            }
            """);
        try
        {
            var store = new WorkbenchSettingsStore(path);
            var loaded = store.Load();
            Assert.Equal("Fallout4", loaded.Release);
            Assert.Equal("French", loaded.Language);
            Assert.Equal("C:\\old\\CreationsForge.Mcp.dll", loaded.McpExecutablePath);
            Assert.Equal("C:\\Games\\Fallout4\\Data", loaded.Releases["Fallout4"].DataDirectory);

            loaded.Release = "SkyrimSE";
            loaded.Language = "German";
            loaded.McpExecutablePath = "C:\\new\\CreationsForge.Mcp.dll";
            loaded.Releases["Fallout4"].DataDirectory = "D:\\Fallout4\\Data";
            loaded.Releases["Fallout4"].SelectedPlugins = "Update.esm";
            loaded.Releases["Fallout4"].OutputPath = "D:\\Fallout4\\Data\\Out.esp";
            loaded.Releases["Fallout4"].MasterStyle = "Medium";
            loaded.Releases["Fallout4"].TextStorageMode = "Localized";
            loaded.Releases["SkyrimSE"] = new WorkbenchReleaseSettings
            {
                DataDirectory = "D:\\Skyrim\\Data",
                OutputPath = "D:\\Skyrim\\Data\\Out.esp",
            };
            store.Save(loaded);

            var json = File.ReadAllText(path);
            Assert.Contains("\"activeGame\": \"SkyrimSE\"", json, StringComparison.Ordinal);
            Assert.DoesNotContain("\"ActiveGame\"", json, StringComparison.Ordinal);
            Assert.Contains("\"themeMode\": \"Light\"", json, StringComparison.Ordinal);
            Assert.Contains("\"customFutureField\"", json, StringComparison.Ordinal);
            Assert.Contains("\"keep\": true", json, StringComparison.Ordinal);
            Assert.Contains("\"mcpExecutablePath\": \"C:\\\\new\\\\CreationsForge.Mcp.dll\"", json, StringComparison.Ordinal);
            Assert.Contains("\"notes\": \"keep me\"", json, StringComparison.Ordinal);
            Assert.Contains("\"dataDirectory\": \"D:\\\\Fallout4\\\\Data\"", json, StringComparison.Ordinal);
            Assert.Contains("\"SkyrimSE\"", json, StringComparison.Ordinal);

            var reread = new WorkbenchSettingsStore(path).Load();
            Assert.Equal("SkyrimSE", reread.Release);
            Assert.Equal("German", reread.Language);
            Assert.Equal("Update.esm", reread.Releases["Fallout4"].SelectedPlugins);
            Assert.Equal("Medium", reread.Releases["Fallout4"].MasterStyle);
            Assert.Equal("D:\\Skyrim\\Data", reread.Releases["SkyrimSE"].DataDirectory);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    /// <summary>Uses defaults when the configuration file is not JSON.</summary>
    [Fact]
    public void InvalidJsonLoadsDefaultsWithoutThrowing()
    {
        var directory = Path.Combine(Path.GetTempPath(), "creationsforge-settings-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "CreationsForge.Config.json");
        File.WriteAllText(path, "{");
        try
        {
            var state = new WorkbenchSettingsStore(path).Load();
            Assert.Equal("Starfield", state.Release);
            Assert.Equal("English", state.Language);
            Assert.Empty(state.McpExecutablePath);
            Assert.Equal("{", File.ReadAllText(path));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    /// <summary>Prefers the installed sibling executable, then the development host for the running configuration.</summary>
    [Fact]
    public void LocatorFindsInstalledExecutableBeforeDevelopmentHost()
    {
        var root = Path.Combine(Path.GetTempPath(), "creationsforge-locator-" + Guid.NewGuid().ToString("N"));
        var desktop = Path.Combine(root, "app", "Desktop");
        var mcpDirectory = Path.Combine(root, "app", "Mcp");
        Directory.CreateDirectory(desktop);
        Directory.CreateDirectory(mcpDirectory);
        var executable = Path.Combine(mcpDirectory, "CreationsForge.Mcp.exe");
        var library = Path.Combine(mcpDirectory, "CreationsForge.Mcp.dll");
        File.WriteAllText(executable, "exe");
        File.WriteAllText(library, "dll");
        try
        {
            Assert.Equal(executable, WorkbenchExecutableLocator.Find(desktop));
            File.Delete(executable);
            Assert.Equal(library, WorkbenchExecutableLocator.Find(desktop));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    /// <summary>Finds the Debug development host when the Workbench itself is a Debug build.</summary>
    [Fact]
    public void LocatorFindsTheDevelopmentHostForTheRunningConfiguration()
    {
        var root = Path.Combine(Path.GetTempPath(), "creationsforge-locator-" + Guid.NewGuid().ToString("N"));
        var workbench = Path.Combine(root, "CreationsForge.Workbench", "bin", "Debug", "net10.0");
        var debugHost = Path.Combine(root, "CreationsForge.Mcp", "bin", "Debug", "net10.0", "CreationsForge.Mcp.dll");
        var releaseHost = Path.Combine(root, "CreationsForge.Mcp", "bin", "Release", "net10.0", "CreationsForge.Mcp.dll");
        Directory.CreateDirectory(workbench);
        Directory.CreateDirectory(Path.GetDirectoryName(debugHost)!);
        Directory.CreateDirectory(Path.GetDirectoryName(releaseHost)!);
        File.WriteAllText(debugHost, "debug");
        File.WriteAllText(releaseHost, "release");
        try
        {
            Assert.Equal(debugHost, WorkbenchExecutableLocator.Find(workbench));
            File.Delete(debugHost);
            Assert.Equal(releaseHost, WorkbenchExecutableLocator.Find(workbench));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    /// <summary>Fills a missing MCP executable and Data directory, then restores each release on its own.</summary>
    [Fact]
    public void ViewModelRemembersSelectionsPerRelease()
    {
        var directory = Path.Combine(Path.GetTempPath(), "creationsforge-settings-" + Guid.NewGuid().ToString("N"));
        var starfieldData = Path.Combine(directory, "Starfield", "Data");
        var falloutData = Path.Combine(directory, "Fallout4", "Data");
        var skyrimData = Path.Combine(directory, "Skyrim", "Data");
        Directory.CreateDirectory(starfieldData);
        Directory.CreateDirectory(falloutData);
        Directory.CreateDirectory(skyrimData);
        var mcp = Path.Combine(directory, "CreationsForge.Mcp.dll");
        File.WriteAllText(mcp, "dll");
        var savedMcp = Path.Combine(directory, "saved.dll");
        var lookups = new List<string>();
        var locatorCalls = 0;
        var store = new MemorySettingsStore(new WorkbenchSettingsState
        {
            Release = "Oblivion",
            Language = "French",
            McpExecutablePath = string.Empty,
            Releases =
            {
                ["Fallout4"] = new WorkbenchReleaseSettings
                {
                    DataDirectory = falloutData,
                    SelectedPlugins = "Fallout4.esm",
                    OutputPath = Path.Combine(falloutData, "Out.esp"),
                    MasterStyle = "Huge",
                    TextStorageMode = "Localized",
                },
            },
        });

        try
        {
            var viewModel = new WorkbenchViewModel(
                (_, _) => Task.FromResult<IWorkbenchMcpSession>(new WorkbenchSessionDouble()),
                store,
                () =>
                {
                    locatorCalls++;
                    return mcp;
                },
                release =>
                {
                    lookups.Add(release);
                    return release switch
                    {
                        "Starfield" => starfieldData,
                        "SkyrimSE" => skyrimData,
                        _ => null,
                    };
                })
            {
                OutputPath = Path.Combine(starfieldData, "Created.esp"),
                SelectedPlugins = "Starfield.esm",
                MasterStyle = "Small",
            };

            Assert.Equal("Starfield", viewModel.Release);
            Assert.Equal("French", viewModel.Language);
            Assert.Equal(mcp, viewModel.McpExecutablePath);
            Assert.Equal(1, locatorCalls);
            Assert.Equal(starfieldData, viewModel.DataDirectory);
            Assert.Equal(["Starfield"], lookups);
            Assert.Equal("Small", viewModel.MasterStyle);

            viewModel.Release = "Fallout4";

            Assert.Equal(falloutData, viewModel.DataDirectory);
            Assert.Equal("Fallout4.esm", viewModel.SelectedPlugins);
            Assert.Equal(Path.Combine(falloutData, "Out.esp"), viewModel.OutputPath);
            Assert.Equal("Full", viewModel.MasterStyle);
            Assert.Equal("Localized", viewModel.TextStorageMode);
            Assert.Equal("French", viewModel.Language);
            Assert.Equal(mcp, viewModel.McpExecutablePath);
            Assert.Equal(["Starfield"], lookups);

            viewModel.Release = "Starfield";

            Assert.Equal(starfieldData, viewModel.DataDirectory);
            Assert.Equal("Starfield.esm", viewModel.SelectedPlugins);
            Assert.Equal(Path.Combine(starfieldData, "Created.esp"), viewModel.OutputPath);
            Assert.Equal("Small", viewModel.MasterStyle);
            Assert.Equal("Embedded", viewModel.TextStorageMode);

            viewModel.Language = "German";
            viewModel.Release = "SkyrimSE";
            Assert.Equal(skyrimData, viewModel.DataDirectory);
            Assert.Equal(string.Empty, viewModel.SelectedPlugins);
            Assert.Equal("German", viewModel.Language);
            Assert.Equal(["Starfield", "SkyrimSE"], lookups);

            Assert.Equal("SkyrimSE", store.Current.Release);
            Assert.Equal("German", store.Current.Language);
            Assert.Equal(mcp, store.Current.McpExecutablePath);
            Assert.Equal("Starfield.esm", store.Current.Releases["Starfield"].SelectedPlugins);
            Assert.Equal("Fallout4.esm", store.Current.Releases["Fallout4"].SelectedPlugins);
            Assert.Equal("Full", store.Current.Releases["Fallout4"].MasterStyle);

            locatorCalls = 0;
            File.WriteAllText(savedMcp, "saved");
            var kept = new WorkbenchViewModel(
                (_, _) => Task.FromResult<IWorkbenchMcpSession>(new WorkbenchSessionDouble()),
                new MemorySettingsStore(new WorkbenchSettingsState
                {
                    Release = "Starfield",
                    McpExecutablePath = savedMcp,
                    Releases =
                    {
                        ["Starfield"] = new WorkbenchReleaseSettings { DataDirectory = starfieldData },
                    },
                }),
                () =>
                {
                    locatorCalls++;
                    return mcp;
                },
                _ => throw new InvalidOperationException("The saved Data directory still exists."));
            Assert.Equal(savedMcp, kept.McpExecutablePath);
            Assert.Equal(0, locatorCalls);
            Assert.Equal(starfieldData, kept.DataDirectory);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    /// <summary>Keeps a copy of the settings document so a later edit does not change the last save.</summary>
    private sealed class MemorySettingsStore : IWorkbenchSettingsStore
    {
        internal MemorySettingsStore(WorkbenchSettingsState initial)
        {
            Current = Clone(initial);
        }

        internal WorkbenchSettingsState Current { get; private set; }

        public WorkbenchSettingsState Load() => Clone(Current);

        public void Save(WorkbenchSettingsState state) => Current = Clone(state);

        private static WorkbenchSettingsState Clone(WorkbenchSettingsState state)
        {
            var clone = new WorkbenchSettingsState
            {
                Release = state.Release,
                Language = state.Language,
                McpExecutablePath = state.McpExecutablePath,
            };
            foreach (var pair in state.Releases)
            {
                clone.Releases[pair.Key] = new WorkbenchReleaseSettings
                {
                    DataDirectory = pair.Value.DataDirectory,
                    SelectedPlugins = pair.Value.SelectedPlugins,
                    OutputPath = pair.Value.OutputPath,
                    MasterStyle = pair.Value.MasterStyle,
                    TextStorageMode = pair.Value.TextStorageMode,
                };
            }

            return clone;
        }
    }
}
