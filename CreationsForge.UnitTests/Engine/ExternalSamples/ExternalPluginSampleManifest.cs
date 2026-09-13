using System.Text.Json;
using System.Text.Json.Serialization;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.UnitTests.Engine.ExternalSamples;

/// <summary>Loads and validates the process-local manifest for one opt-in external plugin sample.</summary>
internal sealed class ExternalPluginSampleManifest
{
    /// <summary>The only environment variable from which an external sample manifest may be selected.</summary>
    internal const string EnvironmentVariableName = "CREATIONSFORGE_EXTERNAL_SAMPLE_MANIFEST";

    /// <summary>The closed manifest schema version implemented by this harness.</summary>
    private const int CurrentSchemaVersion = 1;

    /// <summary>The maximum number of explicitly selected FormLists admitted by one bounded execution.</summary>
    private const int MaximumSelectedFormKeys = 8;

    /// <summary>Initializes an empty serializer target.</summary>
    public ExternalPluginSampleManifest()
    { }

    /// <summary>Gets or initializes the closed manifest schema version.</summary>
    public int SchemaVersion { get; init; }

    /// <summary>Gets or initializes the CreationsForge game name.</summary>
    public string Game { get; init; } = string.Empty;

    /// <summary>Gets or initializes the exact Mutagen game release name.</summary>
    public string Release { get; init; } = string.Empty;

    /// <summary>Gets or initializes the canonical source plugin path supplied by the operator.</summary>
    public string SourcePluginPath { get; init; } = string.Empty;

    /// <summary>Gets or initializes every plugin path in complete explicit load-order order.</summary>
    public string[] LoadOrderPluginPaths { get; init; } = [];

    /// <summary>Gets or initializes the real game Data directory used for applicable archive discovery.</summary>
    public string DataDirectoryPath { get; init; } = string.Empty;

    /// <summary>Gets or initializes explicit loose-string lookup directories in lookup-priority order.</summary>
    public string[]? StringDirectoryPaths { get; init; }

    /// <summary>Gets or initializes optional exact FormKeys; an empty array requests deterministic bounded discovery.</summary>
    public string[] SelectedFormKeys { get; init; } = [];

    /// <summary>Gets or initializes the task-owned directory beneath which fresh retained run roots are created.</summary>
    public string TaskOutputRoot { get; init; } = string.Empty;

    /// <summary>Gets or initializes the output plugin file name, which must not collide with any admitted source identity.</summary>
    public string OutputPluginFileName { get; init; } = string.Empty;

    /// <summary>Gets or initializes the output representations exercised by separate bounded cases.</summary>
    public string[] OutputModes { get; init; } = [];

    /// <summary>Gets the validated supported game.</summary>
    [JsonIgnore]
    public SupportedGame SupportedGame { get; private set; }

    /// <summary>Gets the validated plugin release.</summary>
    [JsonIgnore]
    public GameRelease GameRelease { get; private set; }

    /// <summary>Gets the validated canonical source plugin path.</summary>
    [JsonIgnore]
    public string CanonicalSourcePluginPath { get; private set; } = string.Empty;

    /// <summary>Gets validated canonical plugin paths in exact load-order order.</summary>
    [JsonIgnore]
    public IReadOnlyList<string> CanonicalLoadOrderPluginPaths { get; private set; } = [];

    /// <summary>Gets the validated canonical real Data directory.</summary>
    [JsonIgnore]
    public string CanonicalDataDirectoryPath { get; private set; } = string.Empty;

    /// <summary>Gets validated canonical loose-string lookup directories.</summary>
    [JsonIgnore]
    public IReadOnlyList<string> CanonicalStringDirectoryPaths { get; private set; } = [];

    /// <summary>Gets validated selected FormKeys in manifest order.</summary>
    [JsonIgnore]
    public IReadOnlyList<FormKey> FormKeys { get; private set; } = [];

    /// <summary>Gets the validated canonical task output root.</summary>
    [JsonIgnore]
    public string CanonicalTaskOutputRoot { get; private set; } = string.Empty;

    /// <summary>Gets the validated output plugin identity.</summary>
    [JsonIgnore]
    public ModKey OutputModKey { get; private set; }

    /// <summary>Gets validated distinct output representations in manifest order.</summary>
    [JsonIgnore]
    public IReadOnlyList<LocalizedOutputMode> LocalizedOutputModes { get; private set; } = [];

    /// <summary>Loads the environment-selected manifest, returning null only when the opt-in variable is absent.</summary>
    /// <returns>The validated manifest, or <see langword="null"/> when the environment variable is absent.</returns>
    /// <exception cref="InvalidDataException">Thrown when a supplied manifest path or document is invalid.</exception>
    internal static ExternalPluginSampleManifest? LoadFromEnvironment()
    {
        var manifestPath = Environment.GetEnvironmentVariable(EnvironmentVariableName);
        if (string.IsNullOrWhiteSpace(manifestPath))
        {
            return null;
        }

        return LoadFromPath(manifestPath);
    }

    /// <summary>Loads and validates one explicitly supplied manifest path without consulting process environment.</summary>
    /// <param name="manifestPath">The fully qualified manifest path.</param>
    /// <returns>The validated manifest.</returns>
    /// <exception cref="InvalidDataException">Thrown when the supplied path or document is invalid.</exception>
    internal static ExternalPluginSampleManifest LoadFromPath(string manifestPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(manifestPath);

        string canonicalManifestPath;
        try
        {
            canonicalManifestPath = Path.GetFullPath(manifestPath);
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            throw new InvalidDataException($"The supplied external plugin sample manifest path is invalid: {exception.Message}", exception);
        }

        if (!File.Exists(canonicalManifestPath))
        {
            throw new InvalidDataException($"The supplied external plugin sample manifest does not exist: '{canonicalManifestPath}'.");
        }

        try
        {
            using var stream = new FileStream(canonicalManifestPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var manifest = JsonSerializer.Deserialize<ExternalPluginSampleManifest>(stream, SerializerOptions)
                ?? throw new InvalidDataException("The supplied external plugin sample manifest contained JSON null.");
            manifest.Validate();
            return manifest;
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException($"The supplied external plugin sample manifest is not valid closed-schema JSON: {exception.Message}", exception);
        }
    }

    /// <summary>Creates an explicit production workspace-open request without consulting ambient game state.</summary>
    /// <param name="workspaceId">The fresh workspace identity.</param>
    /// <returns>A request containing only validated manifest inputs.</returns>
    internal WorkspaceOpenRequest CreateWorkspaceOpenRequest(Guid workspaceId)
    {
        return new WorkspaceOpenRequest(
            workspaceId,
            SupportedGame,
            GameRelease,
            CanonicalSourcePluginPath,
            CanonicalLoadOrderPluginPaths,
            CanonicalDataDirectoryPath,
            CanonicalStringDirectoryPaths);
    }

    /// <summary>Validates all scalar values, paths, identities, and bounded collections.</summary>
    /// <exception cref="InvalidDataException">Thrown when the manifest is incomplete, ambiguous, unsafe, or unsupported.</exception>
    private void Validate()
    {
        if (SchemaVersion != CurrentSchemaVersion)
        {
            throw new InvalidDataException($"External plugin sample schemaVersion must be {CurrentSchemaVersion}.");
        }

        SupportedGame = ParseEnum<SupportedGame>(Game, "game");
        GameRelease = ParseEnum<GameRelease>(Release, "release");
        var expectedRelease = SupportedGame switch
        {
            SupportedGame.Starfield => GameRelease.Starfield,
            SupportedGame.Fallout4 => GameRelease.Fallout4,
            SupportedGame.Skyrim => GameRelease.SkyrimSE,
            _ => throw new InvalidDataException($"Game '{SupportedGame}' is unsupported."),
        };
        if (GameRelease != expectedRelease)
        {
            throw new InvalidDataException($"Game '{SupportedGame}' requires release '{expectedRelease}', not '{GameRelease}'.");
        }

        CanonicalSourcePluginPath = RequireExistingFile(SourcePluginPath, "sourcePluginPath");
        if (LoadOrderPluginPaths is null || LoadOrderPluginPaths.Length == 0)
        {
            throw new InvalidDataException("loadOrderPluginPaths must contain the complete explicit plugin load order.");
        }

        var pluginPaths = LoadOrderPluginPaths.Select(path => RequireExistingFile(path, "loadOrderPluginPaths entry")).ToArray();
        EnsureDistinct(pluginPaths, "loadOrderPluginPaths");
        if (!pluginPaths.Contains(CanonicalSourcePluginPath, PathComparer))
        {
            throw new InvalidDataException("sourcePluginPath must be an exact canonical member of loadOrderPluginPaths.");
        }

        CanonicalLoadOrderPluginPaths = Array.AsReadOnly(pluginPaths);
        CanonicalDataDirectoryPath = RequireExistingDirectory(DataDirectoryPath, "dataDirectoryPath");
        if (StringDirectoryPaths is null)
        {
            throw new InvalidDataException("stringDirectoryPaths must be supplied explicitly, using an empty array only when no localized input is admitted.");
        }

        var stringDirectories = StringDirectoryPaths
            .Select(path => RequireExistingDirectory(path, "stringDirectoryPaths entry"))
            .ToArray();
        EnsureDistinct(stringDirectories, "stringDirectoryPaths");
        CanonicalStringDirectoryPaths = Array.AsReadOnly(stringDirectories);
        foreach (var sourcePath in pluginPaths
            .Append(CanonicalDataDirectoryPath)
            .Concat(stringDirectories))
        {
            RejectReparseAncestry(sourcePath, "plugin source input");
        }

        if (SelectedFormKeys is null || SelectedFormKeys.Length > MaximumSelectedFormKeys)
        {
            throw new InvalidDataException($"selectedFormKeys must contain at most {MaximumSelectedFormKeys} entries.");
        }

        var formKeys = SelectedFormKeys.Select(ParseFormKey).ToArray();
        if (formKeys.Distinct().Count() != formKeys.Length)
        {
            throw new InvalidDataException("selectedFormKeys cannot contain duplicate plugin identities.");
        }

        FormKeys = Array.AsReadOnly(formKeys);
        CanonicalTaskOutputRoot = RequireAbsolutePath(TaskOutputRoot, "taskOutputRoot");
        ValidateOutputRootSafety(CanonicalTaskOutputRoot, pluginPaths, CanonicalDataDirectoryPath, stringDirectories);
        if (string.IsNullOrWhiteSpace(OutputPluginFileName)
            || !string.Equals(Path.GetFileName(OutputPluginFileName), OutputPluginFileName, StringComparison.Ordinal))
        {
            throw new InvalidDataException("outputPluginFileName must be one plugin file name without directory components.");
        }

        if (!ModKey.TryFromNameAndExtension(OutputPluginFileName, out var outputModKey, out var modKeyError))
        {
            throw new InvalidDataException($"outputPluginFileName is not a valid record identity: {modKeyError}");
        }

        var sourceModKeys = pluginPaths.Select(path => ModKey.FromNameAndExtension(Path.GetFileName(path))).ToArray();
        if (sourceModKeys.Contains(outputModKey))
        {
            throw new InvalidDataException($"Output identity '{outputModKey.FileName}' collides with the explicit source load order.");
        }

        OutputModKey = outputModKey;
        if (OutputModes is null || OutputModes.Length == 0)
        {
            throw new InvalidDataException("outputModes must explicitly select Embedded, SeparateStringFiles, or both.");
        }

        var modes = OutputModes.Select(value => ParseEnum<LocalizedOutputMode>(value, "outputModes entry")).ToArray();
        if (modes.Distinct().Count() != modes.Length)
        {
            throw new InvalidDataException("outputModes cannot contain duplicate values.");
        }

        LocalizedOutputModes = Array.AsReadOnly(modes);
    }

    /// <summary>Parses one exact non-null plugin FormKey and rejects sentinel identities.</summary>
    /// <param name="value">The manifest FormKey text.</param>
    /// <returns>The parsed real plugin FormKey.</returns>
    /// <exception cref="InvalidDataException">Thrown when the value is invalid or null.</exception>
    private static FormKey ParseFormKey(string value)
    {
        try
        {
            var formKey = FormKey.Factory(value);
            return formKey.IsNull
                ? throw new InvalidDataException("selectedFormKeys cannot contain FormKey.Null.")
                : formKey;
        }
        catch (Exception exception) when (exception is not InvalidDataException)
        {
            throw new InvalidDataException($"selectedFormKeys contains invalid record identity '{value}': {exception.Message}", exception);
        }
    }

    /// <summary>Parses one case-sensitive enum name and rejects numeric or undefined values.</summary>
    /// <typeparam name="TEnum">The closed enum type.</typeparam>
    /// <param name="value">The exact enum name.</param>
    /// <param name="propertyName">The manifest property used in diagnostics.</param>
    /// <returns>The defined enum value.</returns>
    /// <exception cref="InvalidDataException">Thrown when the value is absent, numeric, misspelled, or undefined.</exception>
    private static TEnum ParseEnum<TEnum>(string value, string propertyName)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value)
            || !Enum.TryParse<TEnum>(value, ignoreCase: false, out var parsed)
            || !Enum.IsDefined(parsed)
            || int.TryParse(value, out _))
        {
            throw new InvalidDataException($"{propertyName} must be an exact supported name for {typeof(TEnum).Name}.");
        }

        return parsed;
    }

    /// <summary>Canonicalizes and verifies one existing regular file.</summary>
    /// <param name="path">The supplied path.</param>
    /// <param name="propertyName">The manifest property used in diagnostics.</param>
    /// <returns>The canonical absolute file path.</returns>
    /// <exception cref="InvalidDataException">Thrown when the path is absent, invalid, or not a regular file.</exception>
    private static string RequireExistingFile(string path, string propertyName)
    {
        var canonicalPath = RequireAbsolutePath(path, propertyName);
        if (!File.Exists(canonicalPath))
        {
            throw new InvalidDataException($"{propertyName} does not identify an existing file: '{canonicalPath}'.");
        }

        return canonicalPath;
    }

    /// <summary>Canonicalizes and verifies one existing directory.</summary>
    /// <param name="path">The supplied path.</param>
    /// <param name="propertyName">The manifest property used in diagnostics.</param>
    /// <returns>The canonical absolute directory path without a trailing separator.</returns>
    /// <exception cref="InvalidDataException">Thrown when the path is absent, invalid, or not a directory.</exception>
    private static string RequireExistingDirectory(string path, string propertyName)
    {
        var canonicalPath = RequireAbsolutePath(path, propertyName);
        if (!Directory.Exists(canonicalPath))
        {
            throw new InvalidDataException($"{propertyName} does not identify an existing directory: '{canonicalPath}'.");
        }

        return canonicalPath;
    }

    /// <summary>Canonicalizes one required fully qualified path.</summary>
    /// <param name="path">The supplied path.</param>
    /// <param name="propertyName">The manifest property used in diagnostics.</param>
    /// <returns>The canonical absolute path without a trailing separator.</returns>
    /// <exception cref="InvalidDataException">Thrown when the path is absent, relative, or invalid.</exception>
    private static string RequireAbsolutePath(string path, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(path) || !Path.IsPathFullyQualified(path))
        {
            throw new InvalidDataException($"{propertyName} must be a fully qualified path.");
        }

        try
        {
            return Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            throw new InvalidDataException($"{propertyName} is invalid: {exception.Message}", exception);
        }
    }

    /// <summary>Rejects path duplicates according to current platform semantics.</summary>
    /// <param name="paths">Canonical paths to compare.</param>
    /// <param name="propertyName">The manifest collection used in diagnostics.</param>
    /// <exception cref="InvalidDataException">Thrown when a canonical path is repeated.</exception>
    private static void EnsureDistinct(IReadOnlyList<string> paths, string propertyName)
    {
        if (paths.Distinct(PathComparer).Count() != paths.Count)
        {
            throw new InvalidDataException($"{propertyName} cannot contain duplicate canonical paths.");
        }
    }

    /// <summary>Rejects lexical overlap and existing reparse-point ancestry for task-owned output.</summary>
    /// <param name="outputRoot">The canonical task output root.</param>
    /// <param name="pluginPaths">Canonical plugin paths.</param>
    /// <param name="dataDirectoryPath">The canonical Data directory.</param>
    /// <param name="stringDirectoryPaths">Canonical loose-string directories.</param>
    /// <exception cref="InvalidDataException">Thrown when output and source locations overlap or output ancestry contains a reparse point.</exception>
    private static void ValidateOutputRootSafety(
        string outputRoot,
        IReadOnlyList<string> pluginPaths,
        string dataDirectoryPath,
        IReadOnlyList<string> stringDirectoryPaths)
    {
        var sourceLocations = pluginPaths
            .Select(Path.GetDirectoryName)
            .Where(path => path is not null)
            .Cast<string>()
            .Append(dataDirectoryPath)
            .Distinct(PathComparer);
        foreach (var sourceLocation in sourceLocations)
        {
            if (PathsOverlap(outputRoot, sourceLocation))
            {
                throw new InvalidDataException($"taskOutputRoot overlaps plugin source directory '{sourceLocation}'.");
            }
        }

        foreach (var stringDirectoryPath in stringDirectoryPaths)
        {
            if (!PathsOverlap(outputRoot, stringDirectoryPath))
            {
                continue;
            }

            var isDedicatedEmptyInput = IsUnder(stringDirectoryPath, outputRoot)
                && !Directory.EnumerateFileSystemEntries(stringDirectoryPath).Any();
            if (!isDedicatedEmptyInput)
            {
                throw new InvalidDataException($"taskOutputRoot overlaps nonempty or non-dedicated record strings directory '{stringDirectoryPath}'.");
            }
        }

        RejectReparseAncestry(outputRoot, "taskOutputRoot");
    }

    /// <summary>Rejects an existing reparse point at any segment of a source or output path.</summary>
    /// <param name="path">The canonical path whose existing ancestry is inspected.</param>
    /// <param name="description">The path role used in diagnostics.</param>
    /// <exception cref="InvalidDataException">Thrown when an existing segment is a reparse point.</exception>
    private static void RejectReparseAncestry(string path, string description)
    {
        var cursor = path;
        while (!string.IsNullOrEmpty(cursor))
        {
            if ((File.Exists(cursor) || Directory.Exists(cursor))
                && (File.GetAttributes(cursor) & FileAttributes.ReparsePoint) != 0)
            {
                throw new InvalidDataException($"{description} ancestry contains reparse point '{cursor}'.");
            }

            var parent = Path.GetDirectoryName(cursor);
            if (string.Equals(parent, cursor, PathComparison))
            {
                break;
            }

            cursor = parent ?? string.Empty;
        }
    }

    /// <summary>Determines whether either canonical path is equal to or nested beneath the other.</summary>
    /// <param name="left">The first canonical path.</param>
    /// <param name="right">The second canonical path.</param>
    /// <returns><see langword="true"/> when the paths overlap by containment.</returns>
    private static bool PathsOverlap(string left, string right)
    {
        return PathComparer.Equals(left, right)
            || IsUnder(left, right)
            || IsUnder(right, left);
    }

    /// <summary>Determines whether one canonical path is strictly beneath a canonical directory.</summary>
    /// <param name="path">The possible descendant.</param>
    /// <param name="directory">The possible ancestor.</param>
    /// <returns><see langword="true"/> when <paramref name="path"/> is beneath <paramref name="directory"/>.</returns>
    private static bool IsUnder(string path, string directory)
    {
        var prefix = Path.TrimEndingDirectorySeparator(directory) + Path.DirectorySeparatorChar;
        return path.StartsWith(prefix, PathComparison);
    }

    /// <summary>Gets strict manifest JSON settings with no comments, trailing commas, or unknown properties.</summary>
    private static JsonSerializerOptions SerializerOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    };

    /// <summary>Gets platform path equality semantics.</summary>
    private static StringComparer PathComparer { get; } = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    /// <summary>Gets platform path prefix-comparison semantics.</summary>
    private static StringComparison PathComparison { get; } = OperatingSystem.IsWindows()
        ? StringComparison.OrdinalIgnoreCase
        : StringComparison.Ordinal;
}
