using System.IO.Abstractions;
using System.Text.Json;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace CreationsForge.UnitTests.Engine.ExternalSamples;

/// <summary>Inspects only native headers and filesystem metadata before any full plugin parse or large-file hash.</summary>
internal static class ExternalNativeSamplePreflight
{
    /// <summary>The localized bit shared by supported native TES4 headers.</summary>
    private const uint LocalizedHeaderFlag = 0x00000080;

    /// <summary>Builds a bounded metadata-only report and validates exact master closure and order.</summary>
    /// <param name="manifest">The validated explicit native sample manifest.</param>
    /// <returns>A complete preflight report without parsed major records or content hashes.</returns>
    /// <exception cref="InvalidDataException">Thrown when native identities, master closure, localization inputs, or artifact metadata are invalid.</exception>
    internal static ExternalNativeSamplePreflightReport Inspect(ExternalNativeSampleManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        var fileSystem = new FileSystem();
        var plugins = new List<ExternalNativePluginMetadata>(manifest.CanonicalLoadOrderPluginPaths.Count);
        var knownPaths = new Dictionary<ModKey, int>();
        for (var index = 0; index < manifest.CanonicalLoadOrderPluginPaths.Count; index++)
        {
            var path = manifest.CanonicalLoadOrderPluginPaths[index];
            var modKey = ParseModKey(path);
            if (!knownPaths.TryAdd(modKey, index))
            {
                throw new InvalidDataException($"Explicit load order repeats native identity '{modKey.FileName}'.");
            }

            var header = ModHeaderFrame.FromPath(new ModPath(modKey, path), manifest.GameRelease, fileSystem);
            var masters = MasterReferenceCollection.FromModHeader(modKey, header)
                .Masters
                .Select(master => master.Master)
                .ToArray();
            plugins.Add(new ExternalNativePluginMetadata
            {
                Path = path,
                ModKey = modKey.ToString(),
                LoadOrderIndex = index,
                IsSource = PathComparer.Equals(path, manifest.CanonicalSourcePluginPath),
                MasterStyle = header.MasterStyle.ToString(),
                UsesLocalization = ((uint)header.Flags & LocalizedHeaderFlag) != 0,
                DeclaredMasters = masters.Select(master => master.ToString()).ToArray(),
                File = InspectFile(path),
            });
        }

        ValidateMasterClosure(plugins, knownPaths);
        if (plugins.Any(plugin => plugin.UsesLocalization) && manifest.CanonicalStringDirectoryPaths.Count == 0)
        {
            throw new InvalidDataException("At least one explicit stringDirectoryPaths entry is required because the load order contains a localized plugin.");
        }

        var directories = new[] { manifest.CanonicalDataDirectoryPath }
            .Concat(manifest.CanonicalStringDirectoryPaths)
            .Distinct(PathComparer)
            .Select(InspectDirectory)
            .ToArray();
        var looseSidecars = InspectLooseStringSidecars(manifest, plugins);
        var applicableArchives = InspectApplicableArchives(manifest, plugins, fileSystem);
        var anticipatedBytes = checked(
            plugins.Sum(plugin => plugin.File.Length)
            + looseSidecars.Where(file => file.Exists).Sum(file => file.Length)
            + applicableArchives.Sum(file => file.Length));

        return new ExternalNativeSamplePreflightReport
        {
            SchemaVersion = 1,
            CapturedAtUtc = DateTimeOffset.UtcNow,
            Game = manifest.SupportedGame.ToString(),
            Release = manifest.GameRelease.ToString(),
            SourcePluginPath = manifest.CanonicalSourcePluginPath,
            TaskOutputRoot = manifest.CanonicalTaskOutputRoot,
            OutputPluginFileName = manifest.OutputModKey.FileName.String,
            OutputModes = manifest.LocalizedOutputModes.Select(mode => mode.ToString()).ToArray(),
            SelectedFormKeys = manifest.FormKeys.Select(formKey => formKey.ToString()).ToArray(),
            Plugins = plugins.ToArray(),
            Directories = directories,
            LooseStringSidecars = looseSidecars,
            ApplicableArchives = applicableArchives,
            AnticipatedArtifactBytes = anticipatedBytes,
            NativeRecordParsingPerformed = false,
            ContentHashingPerformed = false,
        };
    }

    /// <summary>Creates a fresh retained run directory beneath the manifest-owned task root.</summary>
    /// <param name="manifest">The validated manifest whose root owns every write.</param>
    /// <param name="purpose">The short run purpose included in the retained directory name.</param>
    /// <returns>The newly created canonical run directory.</returns>
    /// <exception cref="InvalidDataException">Thrown when the output root changes into a reparse point or a fresh directory cannot be created.</exception>
    internal static string CreateRunDirectory(ExternalNativeSampleManifest manifest, string purpose)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentException.ThrowIfNullOrWhiteSpace(purpose);
        try
        {
            Directory.CreateDirectory(manifest.CanonicalTaskOutputRoot);
            RejectReparsePoint(manifest.CanonicalTaskOutputRoot);
            var name = $"{DateTimeOffset.UtcNow:yyyyMMddTHHmmssfffffffZ}-{manifest.SupportedGame}-{purpose}-{Guid.NewGuid():N}";
            var path = Path.GetFullPath(Path.Combine(manifest.CanonicalTaskOutputRoot, name));
            Directory.CreateDirectory(path);
            RejectReparsePoint(path);
            return path;
        }
        catch (Exception exception) when (exception is not InvalidDataException)
        {
            throw new InvalidDataException($"A fresh task-owned run directory could not be created: {exception.Message}", exception);
        }
    }

    /// <summary>Writes one compact structured report atomically inside a fresh retained run directory.</summary>
    /// <typeparam name="T">The report payload type.</typeparam>
    /// <param name="runDirectory">The already validated task-owned run directory.</param>
    /// <param name="fileName">The safe report file name.</param>
    /// <param name="report">The report payload.</param>
    /// <param name="cancellationToken">A token that bounds serialization and durable flush.</param>
    /// <returns>A task that completes after the report replaces its private temporary file.</returns>
    /// <exception cref="ArgumentException">Thrown when the run directory or file name is empty or the file name contains directory components.</exception>
    /// <exception cref="OperationCanceledException">Thrown when serialization or flush is cancelled.</exception>
    internal static async Task WriteReportAsync<T>(
        string runDirectory,
        string fileName,
        T report,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(runDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        if (!string.Equals(Path.GetFileName(fileName), fileName, StringComparison.Ordinal))
        {
            throw new ArgumentException("A report name cannot contain directory components.", nameof(fileName));
        }

        var path = Path.Combine(runDirectory, fileName);
        var temporaryPath = path + ".tmp-" + Guid.NewGuid().ToString("N");
        try
        {
            await using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await JsonSerializer.SerializeAsync(stream, report, ReportOptions, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }

            File.Move(temporaryPath, path, overwrite: false);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    /// <summary>Validates that every declared master precedes its dependent plugin in the explicit load order.</summary>
    /// <param name="plugins">Header metadata in explicit order.</param>
    /// <param name="knownPaths">The identity-to-load-order map.</param>
    /// <exception cref="InvalidDataException">Thrown when a master is omitted, repeated, or ordered after its dependent.</exception>
    private static void ValidateMasterClosure(
        IReadOnlyList<ExternalNativePluginMetadata> plugins,
        IReadOnlyDictionary<ModKey, int> knownPaths)
    {
        foreach (var plugin in plugins)
        {
            var seenMasters = new HashSet<ModKey>();
            foreach (var text in plugin.DeclaredMasters)
            {
                var master = ModKey.FromNameAndExtension(text);
                if (!seenMasters.Add(master))
                {
                    throw new InvalidDataException($"Plugin '{plugin.ModKey}' declares master '{master.FileName}' more than once.");
                }

                if (!knownPaths.TryGetValue(master, out var masterIndex))
                {
                    throw new InvalidDataException($"Plugin '{plugin.ModKey}' requires omitted master '{master.FileName}'.");
                }

                if (masterIndex >= plugin.LoadOrderIndex)
                {
                    throw new InvalidDataException($"Master '{master.FileName}' must precede dependent plugin '{plugin.ModKey}' in loadOrderPluginPaths.");
                }
            }
        }
    }

    /// <summary>Enumerates every possible explicit loose strings sidecar as present-or-absent metadata.</summary>
    /// <param name="manifest">The validated explicit manifest.</param>
    /// <param name="plugins">Native header metadata.</param>
    /// <returns>Sidecar metadata in plugin, directory, strings-source, and language order.</returns>
    private static ExternalNativeFileMetadata[] InspectLooseStringSidecars(
        ExternalNativeSampleManifest manifest,
        IReadOnlyList<ExternalNativePluginMetadata> plugins)
    {
        var format = GameConstants.Get(manifest.GameRelease).StringsLanguageFormat;
        if (!format.HasValue)
        {
            return [];
        }

        var files = new List<ExternalNativeFileMetadata>();
        foreach (var plugin in plugins.Where(plugin => plugin.UsesLocalization))
        {
            var modKey = ModKey.FromNameAndExtension(plugin.ModKey);
            foreach (var directory in manifest.CanonicalStringDirectoryPaths)
            {
                foreach (var source in OrderedStringsSources)
                {
                    foreach (var language in GameConstants.Get(manifest.GameRelease).Languages.OrderBy(language => language))
                    {
                        var fileName = StringsUtility.GetFileName(format.Value, modKey, language, source);
                        files.Add(InspectFile(Path.GetFullPath(Path.Combine(directory, fileName))));
                    }
                }
            }
        }

        return files.ToArray();
    }

    /// <summary>Enumerates the exact currently applicable archive set and captures metadata without opening archive payloads.</summary>
    /// <param name="manifest">The validated explicit manifest.</param>
    /// <param name="plugins">Native header metadata.</param>
    /// <param name="fileSystem">The filesystem adapter used by Mutagen archive discovery.</param>
    /// <returns>Unique applicable archive metadata in canonical path order.</returns>
    private static ExternalNativeFileMetadata[] InspectApplicableArchives(
        ExternalNativeSampleManifest manifest,
        IReadOnlyList<ExternalNativePluginMetadata> plugins,
        IFileSystem fileSystem)
    {
        return plugins
            .Where(plugin => plugin.UsesLocalization)
            .SelectMany(plugin => Archive.GetApplicableArchivePaths(
                manifest.GameRelease,
                new DirectoryPath(manifest.CanonicalDataDirectoryPath),
                ModKey.FromNameAndExtension(plugin.ModKey),
                fileSystem))
            .Select(path => Path.GetFullPath(path.ToString()))
            .Distinct(PathComparer)
            .OrderBy(path => path, PathComparer)
            .Select(path =>
            {
                var metadata = InspectFile(path);
                return metadata.Exists
                    ? metadata
                    : throw new InvalidDataException($"Mutagen reported applicable archive '{path}', but it is absent.");
            })
            .ToArray();
    }

    /// <summary>Parses one plugin identity from its exact file name.</summary>
    /// <param name="path">The canonical plugin path.</param>
    /// <returns>The native plugin identity.</returns>
    /// <exception cref="InvalidDataException">Thrown when the file name is not a valid native identity.</exception>
    private static ModKey ParseModKey(string path)
    {
        if (!ModKey.TryFromNameAndExtension(Path.GetFileName(path), out var modKey, out var error))
        {
            throw new InvalidDataException($"Plugin path '{path}' has an invalid native file name: {error}");
        }

        return modKey;
    }

    /// <summary>Captures regular-file metadata or an explicit absence state without reading contents.</summary>
    /// <param name="path">The canonical file path.</param>
    /// <returns>Stable metadata captured at one instant.</returns>
    private static ExternalNativeFileMetadata InspectFile(string path)
    {
        var info = new FileInfo(path);
        info.Refresh();
        return new ExternalNativeFileMetadata
        {
            Path = Path.GetFullPath(path),
            Exists = info.Exists,
            Length = info.Exists ? info.Length : 0,
            LastWriteTimeUtc = info.Exists ? info.LastWriteTimeUtc : null,
            IsReparsePoint = info.Exists && (info.Attributes & FileAttributes.ReparsePoint) != 0,
        };
    }

    /// <summary>Captures directory metadata without enumerating unrelated contents.</summary>
    /// <param name="path">The canonical directory path.</param>
    /// <returns>Directory existence, timestamp, and reparse metadata.</returns>
    private static ExternalNativeDirectoryMetadata InspectDirectory(string path)
    {
        var info = new DirectoryInfo(path);
        info.Refresh();
        return new ExternalNativeDirectoryMetadata
        {
            Path = Path.GetFullPath(path),
            Exists = info.Exists,
            LastWriteTimeUtc = info.Exists ? info.LastWriteTimeUtc : null,
            IsReparsePoint = info.Exists && (info.Attributes & FileAttributes.ReparsePoint) != 0,
        };
    }

    /// <summary>Rejects a directory that became a reparse point after manifest validation.</summary>
    /// <param name="path">The existing directory to inspect.</param>
    /// <exception cref="InvalidDataException">Thrown when the path is a reparse point.</exception>
    private static void RejectReparsePoint(string path)
    {
        if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
        {
            throw new InvalidDataException($"Task output directory '{path}' is a reparse point.");
        }
    }

    /// <summary>Gets compact deterministic report serializer settings.</summary>
    private static JsonSerializerOptions ReportOptions { get; } = new()
    {
        WriteIndented = true,
    };

    /// <summary>Gets native strings sources in stable artifact order.</summary>
    private static IReadOnlyList<StringsSource> OrderedStringsSources { get; } =
        Array.AsReadOnly(new[] { StringsSource.Normal, StringsSource.DL, StringsSource.IL });

    /// <summary>Gets platform path comparison semantics.</summary>
    private static StringComparer PathComparer { get; } = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;
}

/// <summary>Describes the complete bounded metadata-only preflight result.</summary>
internal sealed class ExternalNativeSamplePreflightReport
{
    /// <summary>Initializes an empty serializable preflight report.</summary>
    internal ExternalNativeSamplePreflightReport()
    { }

    /// <summary>Gets or initializes the report schema version.</summary>
    public int SchemaVersion { get; init; }

    /// <summary>Gets or initializes the UTC capture timestamp.</summary>
    public DateTimeOffset CapturedAtUtc { get; init; }

    /// <summary>Gets or initializes the supported game name.</summary>
    public string Game { get; init; } = string.Empty;

    /// <summary>Gets or initializes the native release name.</summary>
    public string Release { get; init; } = string.Empty;

    /// <summary>Gets or initializes the exact source plugin path.</summary>
    public string SourcePluginPath { get; init; } = string.Empty;

    /// <summary>Gets or initializes the safe task output root.</summary>
    public string TaskOutputRoot { get; init; } = string.Empty;

    /// <summary>Gets or initializes the future output plugin file name.</summary>
    public string OutputPluginFileName { get; init; } = string.Empty;

    /// <summary>Gets or initializes the selected output representations.</summary>
    public string[] OutputModes { get; init; } = [];

    /// <summary>Gets or initializes explicitly selected FormKeys, if any.</summary>
    public string[] SelectedFormKeys { get; init; } = [];

    /// <summary>Gets or initializes ordered plugin header metadata.</summary>
    public ExternalNativePluginMetadata[] Plugins { get; init; } = [];

    /// <summary>Gets or initializes explicit directory metadata.</summary>
    public ExternalNativeDirectoryMetadata[] Directories { get; init; } = [];

    /// <summary>Gets or initializes possible loose strings sidecar presence metadata.</summary>
    public ExternalNativeFileMetadata[] LooseStringSidecars { get; init; } = [];

    /// <summary>Gets or initializes exact applicable archive metadata.</summary>
    public ExternalNativeFileMetadata[] ApplicableArchives { get; init; } = [];

    /// <summary>Gets or initializes the anticipated bytes across plugins, present sidecars, and applicable archives.</summary>
    public long AnticipatedArtifactBytes { get; init; }

    /// <summary>Gets or initializes whether any full native record parser ran during preflight.</summary>
    public bool NativeRecordParsingPerformed { get; init; }

    /// <summary>Gets or initializes whether any content hash ran during preflight.</summary>
    public bool ContentHashingPerformed { get; init; }
}

/// <summary>Describes one explicitly ordered plugin using native-header metadata only.</summary>
internal sealed class ExternalNativePluginMetadata
{
    /// <summary>Initializes an empty serializable native plugin metadata entry.</summary>
    internal ExternalNativePluginMetadata()
    { }

    /// <summary>Gets or initializes the canonical plugin path.</summary>
    public string Path { get; init; } = string.Empty;

    /// <summary>Gets or initializes the canonical native identity.</summary>
    public string ModKey { get; init; } = string.Empty;

    /// <summary>Gets or initializes the zero-based explicit load-order index.</summary>
    public int LoadOrderIndex { get; init; }

    /// <summary>Gets or initializes whether this path is the selected source plugin.</summary>
    public bool IsSource { get; init; }

    /// <summary>Gets or initializes the native master style.</summary>
    public string MasterStyle { get; init; } = string.Empty;

    /// <summary>Gets or initializes whether the native localized-header bit is set.</summary>
    public bool UsesLocalization { get; init; }

    /// <summary>Gets or initializes declared masters in exact header order.</summary>
    public string[] DeclaredMasters { get; init; } = [];

    /// <summary>Gets or initializes regular-file metadata for the plugin.</summary>
    public ExternalNativeFileMetadata File { get; init; } = new();
}

/// <summary>Describes one file without reading or hashing its contents.</summary>
internal sealed class ExternalNativeFileMetadata
{
    /// <summary>Initializes an empty serializable file metadata entry.</summary>
    internal ExternalNativeFileMetadata()
    { }

    /// <summary>Gets or initializes the canonical path.</summary>
    public string Path { get; init; } = string.Empty;

    /// <summary>Gets or initializes whether the file existed.</summary>
    public bool Exists { get; init; }

    /// <summary>Gets or initializes the observed byte length, or zero when absent.</summary>
    public long Length { get; init; }

    /// <summary>Gets or initializes the last-write timestamp, or null when absent.</summary>
    public DateTime? LastWriteTimeUtc { get; init; }

    /// <summary>Gets or initializes whether the file itself is a reparse point.</summary>
    public bool IsReparsePoint { get; init; }
}

/// <summary>Describes one explicit input directory without enumerating unrelated files.</summary>
internal sealed class ExternalNativeDirectoryMetadata
{
    /// <summary>Initializes an empty serializable directory metadata entry.</summary>
    internal ExternalNativeDirectoryMetadata()
    { }

    /// <summary>Gets or initializes the canonical path.</summary>
    public string Path { get; init; } = string.Empty;

    /// <summary>Gets or initializes whether the directory existed.</summary>
    public bool Exists { get; init; }

    /// <summary>Gets or initializes the last-write timestamp, or null when absent.</summary>
    public DateTime? LastWriteTimeUtc { get; init; }

    /// <summary>Gets or initializes whether the directory itself is a reparse point.</summary>
    public bool IsReparsePoint { get; init; }
}
