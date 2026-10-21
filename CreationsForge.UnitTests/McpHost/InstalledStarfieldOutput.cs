using CreationsForge.Engine.Installs;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>
/// Owns one disposable output plugin inside the installed Starfield Data directory and removes every file it may produce.
/// </summary>
/// <remarks>
/// Mutagen resolves masters from the game Data directory, so the output must live there rather than in a temporary directory.
/// Only files named for this instance's unique plugin, its lock sidecar, and save-operation directories that appeared after
/// construction are deleted; installed plugins are snapshotted so a test can prove they were not modified.
/// </remarks>
internal sealed class InstalledStarfieldOutput : IDisposable
{
    private const string LockSuffix = ".creationsforge.lock";
    private const string OperationDirectoryPattern = ".CreationsForge-save-*";
    private static readonly string[] StringsExtensions = [".STRINGS", ".DLSTRINGS", ".ILSTRINGS"];
    private static readonly string[] PluginExtensions = [".esm", ".esp", ".esl"];
    private readonly IReadOnlyDictionary<string, InstalledFileStamp> _installedPlugins;
    private readonly HashSet<string> _existingOperationDirectories;

    /// <summary>Chooses a unique plugin name and snapshots installed plugins and existing save-operation directories.</summary>
    /// <param name="dataDirectory">The installed Starfield Data directory.</param>
    /// <exception cref="InvalidOperationException">Thrown when the generated plugin or its lock already exists.</exception>
    private InstalledStarfieldOutput(string dataDirectory)
    {
        DataDirectory = Path.GetFullPath(dataDirectory);
        var token = Guid.NewGuid().ToString("N")[..12];
        EditorIdPrefix = "CFAcceptance" + token;
        PluginName = EditorIdPrefix + ".esp";
        PluginPath = Path.Combine(DataDirectory, PluginName);
        LockPath = PluginPath + LockSuffix;
        if (OutputFiles().Any(File.Exists) || File.Exists(LockPath))
        {
            throw new InvalidOperationException($"Disposable output '{PluginName}' already exists in '{DataDirectory}'.");
        }

        _installedPlugins = CaptureInstalledPlugins();
        _existingOperationDirectories = OperationDirectories().ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Gets the installed Starfield Data directory that Mutagen reported.</summary>
    public string DataDirectory { get; }

    /// <summary>Gets the unique EditorID prefix used for every record this output creates.</summary>
    public string EditorIdPrefix { get; }

    /// <summary>Gets the disposable plugin file name.</summary>
    public string PluginName { get; }

    /// <summary>Gets the disposable plugin path inside <see cref="DataDirectory"/>.</summary>
    public string PluginPath { get; }

    /// <summary>Gets the output identity lock sidecar path the engine creates while a workspace owns the output.</summary>
    public string LockPath { get; }

    /// <summary>Resolves the installed Starfield Data directory through the production lookup.</summary>
    /// <returns>A new disposable output, or <see langword="null"/> when Starfield or <c>Starfield.esm</c> is not installed.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the generated plugin name unexpectedly already exists.</exception>
    public static InstalledStarfieldOutput? TryCreate()
    {
        var directory = InstalledGameData.TryGetDirectory("Starfield");
        if (directory is null || !File.Exists(Path.Combine(directory, "Starfield.esm")))
        {
            return null;
        }

        return new InstalledStarfieldOutput(directory);
    }

    /// <summary>Lists the localized string files currently written for the disposable plugin.</summary>
    /// <returns>Full paths under the Data <c>Strings</c> directory; empty when none exist.</returns>
    public IReadOnlyList<string> StringFiles()
    {
        var stringsDirectory = Path.Combine(DataDirectory, "Strings");
        if (!Directory.Exists(stringsDirectory))
        {
            return [];
        }

        var prefix = Path.GetFileNameWithoutExtension(PluginName) + "_";
        return Directory
            .EnumerateFiles(stringsDirectory)
            .Where(path => Path.GetFileName(path).StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Where(path => StringsExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
            .Select(Path.GetFullPath)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    /// <summary>Reads the physical plugin and string-file bytes.</summary>
    /// <returns>File bytes keyed by path relative to <see cref="DataDirectory"/>.</returns>
    public IReadOnlyDictionary<string, byte[]> ReadFileSet()
    {
        return OutputFiles()
            .Where(File.Exists)
            .ToDictionary(
                path => Path.GetRelativePath(DataDirectory, path),
                File.ReadAllBytes,
                StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Asserts that the physical plugin and string files still hold exactly the expected bytes.</summary>
    /// <param name="expected">A file set previously captured with <see cref="ReadFileSet"/>.</param>
    public void AssertFileSetUnchanged(IReadOnlyDictionary<string, byte[]> expected)
    {
        var actual = ReadFileSet();
        Assert.Equal(
            expected.Keys.Order(StringComparer.OrdinalIgnoreCase),
            actual.Keys.Order(StringComparer.OrdinalIgnoreCase));
        foreach (var pair in expected)
        {
            Assert.True(pair.Value.AsSpan().SequenceEqual(actual[pair.Key]), $"Output file '{pair.Key}' changed.");
        }
    }

    /// <summary>Asserts that no installed plugin file changed size or last-write time since construction.</summary>
    public void AssertInstalledPluginsUnchanged()
    {
        var current = CaptureInstalledPlugins();
        Assert.Equal(
            _installedPlugins.Keys.Order(StringComparer.OrdinalIgnoreCase),
            current.Keys.Order(StringComparer.OrdinalIgnoreCase));
        foreach (var pair in _installedPlugins)
        {
            Assert.True(pair.Value == current[pair.Key], $"Installed plugin '{pair.Key}' changed.");
        }
    }

    /// <summary>Deletes the disposable plugin and its string files, leaving the output missing.</summary>
    /// <returns>Paths that could not be deleted.</returns>
    public IReadOnlyList<string> DeleteOutputFiles()
    {
        return OutputFiles().Where(path => !TryDeleteFile(path)).ToArray();
    }

    /// <summary>Lists engine save-operation directories that appeared in the Data directory after construction.</summary>
    /// <returns>Full paths; empty when every save removed its staging and backup directory.</returns>
    public IReadOnlyList<string> NewOperationDirectories()
    {
        return OperationDirectories().Where(path => !_existingOperationDirectories.Contains(path)).ToArray();
    }

    /// <summary>
    /// Removes the plugin, string files, lock sidecar, and new save-operation directories that hold only this plugin's files.
    /// </summary>
    /// <returns>Paths that still exist after cleanup, including operation directories left for another plugin.</returns>
    public IReadOnlyList<string> Cleanup()
    {
        var remaining = DeleteOutputFiles().ToList();
        if (!TryDeleteFile(LockPath))
        {
            remaining.Add(LockPath);
        }

        foreach (var directory in NewOperationDirectories())
        {
            if (!HoldsOnlyOwnFiles(directory))
            {
                remaining.Add(directory);
                continue;
            }

            try
            {
                Directory.Delete(directory, recursive: true);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                remaining.Add(directory);
            }
        }

        return remaining;
    }

    /// <summary>
    /// Performs <see cref="Cleanup"/>, retrying briefly while an exiting host releases its handles, so a failing test still
    /// removes its disposable files. Paths that remain are reported as a diagnostic instead of throwing over the test failure.
    /// </summary>
    public void Dispose()
    {
        var remaining = Cleanup();
        for (var attempt = 0; attempt < 10 && remaining.Count > 0; attempt++)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(250));
            remaining = Cleanup();
        }

        if (remaining.Count > 0)
        {
            TestContext.Current.SendDiagnosticMessage(
                "Disposable Starfield output files remain in '{0}': {1}",
                DataDirectory,
                string.Join(", ", remaining));
        }
    }

    /// <summary>Enumerates the plugin path and any string files that belong to it.</summary>
    /// <returns>The plugin path followed by its string files; the plugin path is returned even when it does not exist.</returns>
    private IEnumerable<string> OutputFiles()
    {
        return StringFiles().Prepend(PluginPath);
    }

    /// <summary>Enumerates engine save-operation directories directly under the Data directory.</summary>
    /// <returns>Full directory paths.</returns>
    private IEnumerable<string> OperationDirectories()
    {
        return Directory.EnumerateDirectories(DataDirectory, OperationDirectoryPattern).Select(Path.GetFullPath);
    }

    /// <summary>Returns whether every file in an operation directory is named for this disposable plugin.</summary>
    /// <param name="directory">A save-operation directory created after construction.</param>
    /// <returns><see langword="true"/> when the directory can be deleted without touching another plugin's staging or backup.</returns>
    private bool HoldsOnlyOwnFiles(string directory)
    {
        var stem = Path.GetFileNameWithoutExtension(PluginName);
        try
        {
            return Directory
                .EnumerateFiles(directory, "*", SearchOption.AllDirectories)
                .All(path => Path.GetFileName(path).StartsWith(stem, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    /// <summary>Captures the size and last-write time of every top-level installed plugin except the disposable output.</summary>
    /// <returns>Stamps keyed by plugin file name.</returns>
    private Dictionary<string, InstalledFileStamp> CaptureInstalledPlugins()
    {
        return Directory
            .EnumerateFiles(DataDirectory)
            .Where(path => PluginExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
            .Where(path => !string.Equals(Path.GetFileName(path), PluginName, StringComparison.OrdinalIgnoreCase))
            .Select(path => new FileInfo(path))
            .ToDictionary(
                file => file.Name,
                file => new InstalledFileStamp(file.Length, file.LastWriteTimeUtc),
                StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Deletes one file when it exists.</summary>
    /// <param name="path">The file to delete.</param>
    /// <returns><see langword="true"/> when the file is absent afterwards.</returns>
    private static bool TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    /// <summary>Identifies an installed plugin's on-disk version without hashing multi-gigabyte masters.</summary>
    /// <param name="Length">The file size in bytes.</param>
    /// <param name="LastWriteTimeUtc">The last write time in UTC.</param>
    private readonly record struct InstalledFileStamp(long Length, DateTime LastWriteTimeUtc);
}
