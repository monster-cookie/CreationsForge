using System.Text.Json;
using System.Text.Json.Nodes;
using Serilog;

namespace CreationsForge.Workbench;

/// <summary>Loads and saves the Workbench section of <c>CreationsForge.Config.json</c>.</summary>
internal interface IWorkbenchSettingsStore
{
    /// <summary>Reads the current settings.</summary>
    /// <returns>The release, language, MCP executable, and per-release profiles.</returns>
    WorkbenchSettingsState Load();

    /// <summary>Merges <paramref name="state"/> into the configuration file.</summary>
    /// <param name="state">The settings to store. Unknown JSON properties already in the file are left in place.</param>
    void Save(WorkbenchSettingsState state);
}

/// <summary>The Workbench selections stored in the shared Creations Forge configuration file.</summary>
internal sealed class WorkbenchSettingsState
{
    /// <summary>Gets or sets the active game release stored as <c>ActiveGame</c>.</summary>
    public string Release { get; set; } = "Starfield";

    /// <summary>Gets or sets the translated-string language stored as <c>RecordTextLanguage</c>.</summary>
    public string Language { get; set; } = "English";

    /// <summary>Gets or sets the production MCP executable or <c>.dll</c> path.</summary>
    public string McpExecutablePath { get; set; } = string.Empty;

    /// <summary>Gets the selections remembered separately for each release.</summary>
    public Dictionary<string, WorkbenchReleaseSettings> Releases { get; } = new(StringComparer.Ordinal);
}

/// <summary>Selections that belong to one game release.</summary>
internal sealed class WorkbenchReleaseSettings
{
    /// <summary>Gets or sets the game Data directory.</summary>
    public string DataDirectory { get; set; } = string.Empty;

    /// <summary>Gets or sets the comma-separated source plugin names.</summary>
    public string SelectedPlugins { get; set; } = string.Empty;

    /// <summary>Gets or sets the output plugin path.</summary>
    public string OutputPath { get; set; } = string.Empty;

    /// <summary>Gets or sets the output master style.</summary>
    public string MasterStyle { get; set; } = "Full";

    /// <summary>Gets or sets the output text storage mode.</summary>
    public string TextStorageMode { get; set; } = "Embedded";
}

/// <summary>Persists Workbench selections in the historical <c>CreationsForge.Config.json</c> file.</summary>
internal sealed class WorkbenchSettingsStore : IWorkbenchSettingsStore
{
    private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = true };

    private readonly string _path;
    private readonly JsonObject _root;

    /// <summary>Opens the configuration file in the standard Creations Forge application directory.</summary>
    internal WorkbenchSettingsStore()
        : this(ResolveConfigurationPath())
    {
    }

    /// <summary>Opens an explicit configuration file.</summary>
    /// <param name="configurationPath">The <c>CreationsForge.Config.json</c> path. A missing file stays missing until <see cref="Save"/>.</param>
    internal WorkbenchSettingsStore(string configurationPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(configurationPath);
        _path = configurationPath;
        _root = LoadRoot();
        Log.ForContext<WorkbenchSettingsStore>().Information("Workbench settings file: {ConfigurationPath}", _path);
    }

    /// <inheritdoc />
    public WorkbenchSettingsState Load() => ReadState(_root);

    /// <inheritdoc />
    public void Save(WorkbenchSettingsState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        ApplyState(_root, state);
        var json = _root.ToJsonString(WriteOptions) + Environment.NewLine;
        if (File.Exists(_path))
        {
            try
            {
                if (string.Equals(File.ReadAllText(_path), json, StringComparison.Ordinal))
                {
                    return;
                }
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                // The write below reports the failure if the file still cannot be replaced.
            }
        }

        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var temporary = _path + ".tmp";
        try
        {
            File.WriteAllText(temporary, json);
            File.Move(temporary, _path, overwrite: true);
        }
        catch
        {
            try
            {
                File.Delete(temporary);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                // The original save exception is the one the caller handles.
            }

            throw;
        }
    }

    /// <summary>Selects the first writable directory, preferring the historical Creations Forge location.</summary>
    /// <returns>The configuration file path. The file itself is not created here.</returns>
    private static string ResolveConfigurationPath()
    {
        foreach (var directory in CandidateDirectories())
        {
            if (CanWrite(directory))
            {
                return Path.Combine(directory, "CreationsForge.Config.json");
            }
        }

        return Path.Combine(Path.GetTempPath(), "CreationsForge", "CreationsForge.Config.json");
    }

    /// <summary>Lists application-data directories in preference order.</summary>
    /// <returns>The historical directory, then the user directory, then the temporary directory.</returns>
    private static IEnumerable<string> CandidateDirectories()
    {
        if (OperatingSystem.IsWindows())
        {
            var common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            if (!string.IsNullOrWhiteSpace(common))
            {
                yield return Path.Combine(common, "CreationsForge");
            }
        }
        else
        {
            var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!string.IsNullOrWhiteSpace(profile))
            {
                yield return Path.Combine(profile, ".CreationsForge");
            }
        }

        var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (!string.IsNullOrWhiteSpace(local))
        {
            yield return Path.Combine(local, "CreationsForge");
        }

        yield return Path.Combine(Path.GetTempPath(), "CreationsForge");
    }

    /// <summary>Creates a directory and checks that a file can be written there.</summary>
    /// <param name="directory">The candidate application-data directory.</param>
    /// <returns><see langword="true"/> when a probe file can be created and deleted.</returns>
    private static bool CanWrite(string directory)
    {
        try
        {
            Directory.CreateDirectory(directory);
            var probe = Path.Combine(directory, ".config-write-probe");
            File.WriteAllText(probe, "ok");
            File.Delete(probe);
            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    /// <summary>Reads the configuration object, or builds the historical defaults when the file cannot be used.</summary>
    /// <returns>The JSON object that later saves must preserve.</returns>
    private JsonObject LoadRoot()
    {
        if (!File.Exists(_path))
        {
            Log.ForContext<WorkbenchSettingsStore>().Debug("No configuration file at {ConfigurationPath}. Using defaults.", _path);
            return CreateDefaultRoot();
        }

        string json;
        try
        {
            json = File.ReadAllText(_path);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            Log.ForContext<WorkbenchSettingsStore>().Warning(exception, "Could not read {ConfigurationPath}. Using defaults.", _path);
            return CreateDefaultRoot();
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            Log.ForContext<WorkbenchSettingsStore>().Warning("Configuration file {ConfigurationPath} was empty. Using defaults.", _path);
            return CreateDefaultRoot();
        }

        try
        {
            if (JsonNode.Parse(json) is JsonObject root)
            {
                return root;
            }

            Log.ForContext<WorkbenchSettingsStore>().Warning("Configuration file {ConfigurationPath} was not a JSON object. Using defaults.", _path);
        }
        catch (JsonException exception)
        {
            Log.ForContext<WorkbenchSettingsStore>().Warning(exception, "Configuration file {ConfigurationPath} contained invalid JSON. Using defaults.", _path);
        }

        return CreateDefaultRoot();
    }

    /// <summary>Builds a new document with the historical Creations Forge defaults.</summary>
    /// <returns>A JSON object that still has room for the Workbench section.</returns>
    private JsonObject CreateDefaultRoot()
    {
        var directory = Path.GetDirectoryName(_path);
        var root = new JsonObject
        {
            ["ActiveGame"] = "Starfield",
            ["ThemeMode"] = "Dark",
            ["ThemeFamily"] = "Semi",
            ["RecordTextLanguage"] = "English",
            ["NifSkopeExecutablePath"] = null,
            ["PreferEspOverMatchingEsm"] = true,
        };
        if (!string.IsNullOrWhiteSpace(directory))
        {
            root["ApplicationDataDirectory"] = directory;
            root["LoggingDirectory"] = Path.Combine(directory, "Logs");
        }

        root["Workbench"] = new JsonObject
        {
            ["McpExecutablePath"] = string.Empty,
            ["Releases"] = new JsonObject(),
        };
        return root;
    }

    /// <summary>Copies the Workbench fields out of a configuration object.</summary>
    /// <param name="root">The configuration document.</param>
    /// <returns>The selections the Workbench understands.</returns>
    private static WorkbenchSettingsState ReadState(JsonObject root)
    {
        var state = new WorkbenchSettingsState
        {
            Release = ReadString(root, "ActiveGame") ?? "Starfield",
            Language = ReadString(root, "RecordTextLanguage") ?? "English",
        };
        if (FindObject(root, "Workbench") is not JsonObject workbench)
        {
            return state;
        }

        state.McpExecutablePath = ReadString(workbench, "McpExecutablePath") ?? string.Empty;
        if (FindObject(workbench, "Releases") is not JsonObject releases)
        {
            return state;
        }

        foreach (var pair in releases)
        {
            if (pair.Value is not JsonObject release)
            {
                continue;
            }

            state.Releases[pair.Key] = new WorkbenchReleaseSettings
            {
                DataDirectory = ReadString(release, "DataDirectory") ?? string.Empty,
                SelectedPlugins = ReadString(release, "SelectedPlugins") ?? string.Empty,
                OutputPath = ReadString(release, "OutputPath") ?? string.Empty,
                MasterStyle = ReadString(release, "MasterStyle") ?? "Full",
                TextStorageMode = ReadString(release, "TextStorageMode") ?? "Embedded",
            };
        }

        return state;
    }

    /// <summary>Writes Workbench fields onto an existing document without dropping other properties.</summary>
    /// <param name="root">The configuration document loaded from disk.</param>
    /// <param name="state">The selections to store.</param>
    private static void ApplyState(JsonObject root, WorkbenchSettingsState state)
    {
        SetString(root, "ActiveGame", state.Release);
        SetString(root, "RecordTextLanguage", state.Language);
        var workbench = FindOrCreateObject(root, "Workbench");
        SetString(workbench, "McpExecutablePath", state.McpExecutablePath ?? string.Empty);
        var releases = FindOrCreateObject(workbench, "Releases");
        foreach (var pair in state.Releases)
        {
            var release = FindOrCreateObject(releases, pair.Key);
            SetString(release, "DataDirectory", pair.Value.DataDirectory ?? string.Empty);
            SetString(release, "SelectedPlugins", pair.Value.SelectedPlugins ?? string.Empty);
            SetString(release, "OutputPath", pair.Value.OutputPath ?? string.Empty);
            SetString(release, "MasterStyle", pair.Value.MasterStyle ?? "Full");
            SetString(release, "TextStorageMode", pair.Value.TextStorageMode ?? "Embedded");
        }
    }

    /// <summary>Finds a child object without regard to property-name casing.</summary>
    /// <param name="parent">The object to search.</param>
    /// <param name="name">The property name.</param>
    /// <returns>The child object, or <see langword="null"/> when the property is absent or not an object.</returns>
    private static JsonObject? FindObject(JsonObject parent, string name)
    {
        foreach (var pair in parent)
        {
            if (string.Equals(pair.Key, name, StringComparison.OrdinalIgnoreCase) && pair.Value is JsonObject child)
            {
                return child;
            }
        }

        return null;
    }

    /// <summary>Returns a child object, creating or replacing the property when needed.</summary>
    /// <param name="parent">The object to update.</param>
    /// <param name="name">The property name. An existing name keeps its current casing.</param>
    /// <returns>The child object that the caller can update.</returns>
    private static JsonObject FindOrCreateObject(JsonObject parent, string name)
    {
        string? existingKey = null;
        JsonNode? existing = null;
        foreach (var pair in parent)
        {
            if (!string.Equals(pair.Key, name, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            existingKey = pair.Key;
            existing = pair.Value;
            break;
        }

        if (existingKey is not null)
        {
            if (existing is JsonObject child)
            {
                return child;
            }

            var replacement = new JsonObject();
            parent[existingKey] = replacement;
            return replacement;
        }

        var created = new JsonObject();
        parent[name] = created;
        return created;
    }

    /// <summary>Reads a string property without regard to property-name casing.</summary>
    /// <param name="parent">The object to search.</param>
    /// <param name="name">The property name.</param>
    /// <returns>The string value, or <see langword="null"/> when the property is absent or not a string.</returns>
    private static string? ReadString(JsonObject parent, string name)
    {
        foreach (var pair in parent)
        {
            if (!string.Equals(pair.Key, name, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            return pair.Value is JsonValue value && value.TryGetValue<string>(out var text) ? text : null;
        }

        return null;
    }

    /// <summary>Writes a string property, reusing an existing property name when only the casing differs.</summary>
    /// <param name="parent">The object to update.</param>
    /// <param name="name">The property name used when the property is new.</param>
    /// <param name="value">The value to store.</param>
    private static void SetString(JsonObject parent, string name, string value)
    {
        foreach (var key in parent.Select(pair => pair.Key).ToArray())
        {
            if (!string.Equals(key, name, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            parent[key] = value;
            return;
        }

        parent[name] = value;
    }
}
