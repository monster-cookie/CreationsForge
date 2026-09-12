using System.Collections.ObjectModel;
using System.IO.Abstractions;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace CreationsForge.Core.Engine.NativeInputs;

/// <summary>
/// Prepares bounded native source inputs exclusively from caller-supplied plugin, data, and localized-string paths.
/// </summary>
public sealed class NativeSourceInputLoader
{
    /// <summary>The localized flag shared by the supported native TES4 plugin headers.</summary>
    private const uint LocalizedHeaderFlag = 0x00000080;

    /// <summary>Compares canonical paths according to the current platform's file-name semantics.</summary>
    private static readonly StringComparer PathComparer = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    /// <summary>Initializes a stateless native source-input loader.</summary>
    public NativeSourceInputLoader()
    { }

    /// <summary>Validates and prepares an independently disposable native source-input lifetime.</summary>
    /// <param name="request">The complete explicit workspace-open request.</param>
    /// <param name="cancellationToken">The token checked during path discovery, native header reads, and file hashing.</param>
    /// <returns>A prepared native input set or a stable typed failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public async Task<EngineResult<NativeSourceInputs>> PrepareAsync(
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateGameRelease(request.Game, request.Release);
            var sourcePath = CanonicalizeFile(request.SourcePluginPath, "source plugin");
            var pluginPaths = CanonicalizePluginPaths(request.LoadOrderPluginPaths);
            if (!pluginPaths.Contains(sourcePath, PathComparer))
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    "The source plugin must be an exact canonical-path member of the explicit load order.");
            }

            var dataDirectoryPath = CanonicalizeDirectory(request.DataDirectoryPath, "data directory");
            NativeFileInspector.VerifyDirectory(dataDirectoryPath, "data directory");
            var stringDirectoryPaths = CanonicalizeStringDirectories(request.StringDirectoryPaths);
            foreach (var directoryPath in stringDirectoryPaths)
            {
                cancellationToken.ThrowIfCancellationRequested();
                NativeFileInspector.VerifyDirectory(directoryPath, "localized-string directory");
            }

            var fileSystem = new FileSystem();
            var plugins = new List<NativeSourcePluginInput>(pluginPaths.Count);
            var declaredMasters = new List<IReadOnlyList<ModKey>>(pluginPaths.Count);
            var masterFlags = new Cache<IModMasterStyledGetter, ModKey>(metadata => metadata.ModKey);
            var modKeys = new HashSet<ModKey>();
            for (var index = 0; index < pluginPaths.Count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var path = pluginPaths[index];
                var modKey = ParseModKey(path);
                if (!modKeys.Add(modKey))
                {
                    throw new NativeSourceInputException(
                        EngineErrorCode.InvalidRequest,
                        $"The explicit load order contains native plugin identity '{modKey.FileName}' more than once.");
                }

                var modPath = new ModPath(modKey, path);
                var header = ModHeaderFrame.FromPath(modPath, request.Release, fileSystem);
                ValidateMasterStyle(request.Release, modKey, header.MasterStyle);
                var role = PathComparer.Equals(path, sourcePath) ? PluginRole.Source : PluginRole.LoadOrder;
                plugins.Add(new NativeSourcePluginInput(
                    path,
                    modKey,
                    index,
                    role,
                    header.MasterStyle,
                    UsesLocalization(header)));
                declaredMasters.Add(Array.AsReadOnly(MasterReferenceCollection
                    .FromModHeader(modKey, header)
                    .Masters
                    .Select(master => master.Master)
                    .ToArray()));
                masterFlags.Set(new KeyedMasterStyle(modKey, header.MasterStyle));
            }

            ValidateMasterOrder(plugins, declaredMasters);
            if (plugins.Any(plugin => plugin.UsesLocalization) && stringDirectoryPaths.Count == 0)
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    "A localized native plugin requires at least one explicit localized-string directory. Supply an explicit empty directory when only archive lookup is intended.");
            }

            var artifactCollector = new NativeSourceArtifactCollector(
                request.Release,
                plugins,
                dataDirectoryPath,
                stringDirectoryPaths,
                fileSystem);
            var initialArtifacts = await artifactCollector.CaptureAsync(cancellationToken).ConfigureAwait(false);
            ValidateDistinctPluginIdentities(initialArtifacts);
            VerifyHeaderMetadataUnchanged(request.Release, plugins, declaredMasters, fileSystem, cancellationToken);

            var lookups = CreateStringsLookups(
                request.Release,
                plugins,
                dataDirectoryPath,
                stringDirectoryPaths,
                fileSystem);
            var inputs = new NativeSourceInputs(
                request.Release,
                plugins,
                masterFlags,
                lookups,
                artifactCollector,
                initialArtifacts);
            return EngineResult<NativeSourceInputs>.Success(inputs, workspaceId: request.WorkspaceId);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (NativeSourceInputException exception)
        {
            return EngineResult<NativeSourceInputs>.Failure(
                new EngineError(exception.Code, exception.Message),
                workspaceId: request.WorkspaceId);
        }
        catch (Exception exception)
        {
            return EngineResult<NativeSourceInputs>.Failure(
                new EngineError(
                    EngineErrorCode.SourceOpenFailed,
                    $"The explicit native source inputs could not be prepared: {exception.Message}"),
                workspaceId: request.WorkspaceId);
        }
    }

    /// <summary>Reads the supported games' native localized-file selection bit from a TES4 header.</summary>
    /// <param name="header">The native plugin header frame.</param>
    /// <returns><see langword="true"/> when the header selects external strings files.</returns>
    private static bool UsesLocalization(ModHeaderFrame header)
    {
        return ((uint)header.Flags & LocalizedHeaderFlag) != 0;
    }

    /// <summary>Rejects multiple explicit plugin paths that identify the same physical file.</summary>
    /// <param name="artifacts">The complete initial artifact observations.</param>
    /// <exception cref="NativeSourceInputException">Thrown when two plugin descriptors alias the same physical file.</exception>
    private static void ValidateDistinctPluginIdentities(IReadOnlyList<NativeArtifactAssociation> artifacts)
    {
        var identities = new HashSet<NativeFileIdentity>();
        foreach (var artifact in artifacts.Where(artifact => artifact.Role == NativeArtifactRole.Plugin))
        {
            if (artifact.FileIdentity is not null && !identities.Add(artifact.FileIdentity))
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    "Two explicit native plugin paths identify the same physical file.");
            }
        }
    }

    /// <summary>Validates the exact CreationsForge game and native release pair supported by this bounded backend.</summary>
    /// <param name="game">The requested CreationsForge game.</param>
    /// <param name="release">The requested native Mutagen release.</param>
    /// <exception cref="NativeSourceInputException">Thrown when the pair is unsupported or mismatched.</exception>
    private static void ValidateGameRelease(SupportedGame game, GameRelease release)
    {
        var supported = game switch
        {
            SupportedGame.Starfield => release == GameRelease.Starfield,
            SupportedGame.Fallout4 => release == GameRelease.Fallout4,
            SupportedGame.Skyrim => release == GameRelease.SkyrimSE,
            _ => false
        };
        if (!supported)
        {
            throw new NativeSourceInputException(
                EngineErrorCode.UnsupportedGameRelease,
                $"The native source-input loader does not support {game} with release {release}.");
        }
    }

    /// <summary>Canonicalizes and de-duplicates the explicit plugin load order without discovering installed plugins.</summary>
    /// <param name="paths">The caller-supplied plugin paths.</param>
    /// <returns>The immutable canonical paths in caller order.</returns>
    /// <exception cref="NativeSourceInputException">Thrown when the load order is empty, invalid, missing, or duplicated.</exception>
    private static IReadOnlyList<string> CanonicalizePluginPaths(IReadOnlyList<string> paths)
    {
        ArgumentNullException.ThrowIfNull(paths);
        if (paths.Count == 0)
        {
            throw new NativeSourceInputException(
                EngineErrorCode.InvalidRequest,
                "At least one explicit native plugin path is required.");
        }

        var canonicalPaths = new List<string>(paths.Count);
        var seen = new HashSet<string>(PathComparer);
        foreach (var path in paths)
        {
            var canonicalPath = CanonicalizeFile(path, "load-order plugin");
            if (!seen.Add(canonicalPath))
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    $"The explicit load order contains canonical plugin path '{canonicalPath}' more than once.");
            }

            canonicalPaths.Add(canonicalPath);
        }

        return Array.AsReadOnly(canonicalPaths.ToArray());
    }

    /// <summary>Canonicalizes and de-duplicates explicit loose strings directories in caller priority order.</summary>
    /// <param name="paths">The caller-supplied strings directories.</param>
    /// <returns>The immutable canonical directories in priority order.</returns>
    /// <exception cref="NativeSourceInputException">Thrown when a directory is invalid, missing, or duplicated.</exception>
    private static IReadOnlyList<string> CanonicalizeStringDirectories(IReadOnlyList<string> paths)
    {
        ArgumentNullException.ThrowIfNull(paths);
        var canonicalPaths = new List<string>(paths.Count);
        var seen = new HashSet<string>(PathComparer);
        foreach (var path in paths)
        {
            var canonicalPath = CanonicalizeDirectory(path, "localized-string directory");
            if (!seen.Add(canonicalPath))
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    $"The localized-string directory '{canonicalPath}' is supplied more than once.");
            }

            canonicalPaths.Add(canonicalPath);
        }

        return Array.AsReadOnly(canonicalPaths.ToArray());
    }

    /// <summary>Canonicalizes and verifies one required existing file.</summary>
    /// <param name="path">The caller-supplied file path.</param>
    /// <param name="description">The file description used in diagnostics.</param>
    /// <returns>The canonical absolute file path.</returns>
    /// <exception cref="NativeSourceInputException">Thrown when the path is invalid or the file is absent.</exception>
    private static string CanonicalizeFile(string path, string description)
    {
        var canonicalPath = CanonicalizePath(path, description);
        if (!File.Exists(canonicalPath))
        {
            throw new NativeSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"The {description} does not exist: '{canonicalPath}'.");
        }

        return canonicalPath;
    }

    /// <summary>Canonicalizes and verifies one required existing directory.</summary>
    /// <param name="path">The caller-supplied directory path.</param>
    /// <param name="description">The directory description used in diagnostics.</param>
    /// <returns>The canonical absolute directory path without a trailing separator.</returns>
    /// <exception cref="NativeSourceInputException">Thrown when the path is invalid or the directory is absent.</exception>
    private static string CanonicalizeDirectory(string path, string description)
    {
        var canonicalPath = Path.TrimEndingDirectorySeparator(CanonicalizePath(path, description));
        if (!Directory.Exists(canonicalPath))
        {
            throw new NativeSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"The {description} does not exist: '{canonicalPath}'.");
        }

        return canonicalPath;
    }

    /// <summary>Converts a required path into absolute normalized form.</summary>
    /// <param name="path">The caller-supplied path.</param>
    /// <param name="description">The path description used in diagnostics.</param>
    /// <returns>The absolute normalized path.</returns>
    /// <exception cref="NativeSourceInputException">Thrown when the path is empty or invalid.</exception>
    private static string CanonicalizePath(string path, string description)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new NativeSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"A {description} path is required.");
        }

        try
        {
            return Path.GetFullPath(path);
        }
        catch (Exception exception) when (
            exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            throw new NativeSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"The {description} path is invalid: '{path}'.",
                exception);
        }
    }

    /// <summary>Parses a native plugin identity from one canonical plugin file name.</summary>
    /// <param name="path">The canonical plugin path.</param>
    /// <returns>The native plugin identity.</returns>
    /// <exception cref="NativeSourceInputException">Thrown when the file name is not a valid plugin identity.</exception>
    private static ModKey ParseModKey(string path)
    {
        var fileName = Path.GetFileName(path);
        if (ModKey.TryFromNameAndExtension(fileName, out var modKey, out var error))
        {
            return modKey;
        }

        throw new NativeSourceInputException(
            EngineErrorCode.InvalidRequest,
            $"The native plugin file name '{fileName}' is invalid: {error}");
    }

    /// <summary>Rejects a header master style unsupported by the selected native release.</summary>
    /// <param name="release">The selected native release.</param>
    /// <param name="modKey">The plugin being validated.</param>
    /// <param name="masterStyle">The style read from its native header.</param>
    /// <exception cref="NativeSourceInputException">Thrown when the release cannot decode the style.</exception>
    private static void ValidateMasterStyle(GameRelease release, ModKey modKey, MasterStyle masterStyle)
    {
        var constants = GameConstants.Get(release);
        var supported = masterStyle switch
        {
            MasterStyle.Full => true,
            MasterStyle.Small => constants.SmallMasterFlag.HasValue,
            MasterStyle.Medium => constants.MediumMasterFlag.HasValue,
            _ => false
        };
        if (!supported)
        {
            throw new NativeSourceInputException(
                EngineErrorCode.UnsupportedInput,
                $"Native plugin '{modKey.FileName}' uses {masterStyle} master style, which release {release} does not support.");
        }
    }

    /// <summary>Validates that every declared native master exists earlier in the explicit load order and preserves native declaration order.</summary>
    /// <param name="plugins">The explicit plugin descriptors.</param>
    /// <param name="declaredMasters">The native header masters corresponding to each descriptor.</param>
    /// <exception cref="NativeSourceInputException">Thrown when a master is missing, duplicated, or ordered after its dependent plugin.</exception>
    private static void ValidateMasterOrder(
        IReadOnlyList<NativeSourcePluginInput> plugins,
        IReadOnlyList<IReadOnlyList<ModKey>> declaredMasters)
    {
        var indexByModKey = plugins.ToDictionary(plugin => plugin.ModKey, plugin => plugin.LoadOrderIndex);
        for (var pluginIndex = 0; pluginIndex < plugins.Count; pluginIndex++)
        {
            var previousMasterIndex = -1;
            foreach (var master in declaredMasters[pluginIndex])
            {
                if (!indexByModKey.TryGetValue(master, out var masterIndex))
                {
                    throw new NativeSourceInputException(
                        EngineErrorCode.MissingMaster,
                        $"Native plugin '{plugins[pluginIndex].ModKey.FileName}' requires missing explicit master '{master.FileName}'.");
                }

                if (masterIndex >= pluginIndex || masterIndex <= previousMasterIndex)
                {
                    throw new NativeSourceInputException(
                        EngineErrorCode.SourceOpenFailed,
                        $"Native plugin '{plugins[pluginIndex].ModKey.FileName}' declares master '{master.FileName}' outside valid explicit load-order order.");
                }

                previousMasterIndex = masterIndex;
            }
        }
    }

    /// <summary>Confirms that header styles and master lists did not change while artifact baselines were captured.</summary>
    /// <param name="release">The selected native release.</param>
    /// <param name="plugins">The verified plugin descriptors.</param>
    /// <param name="declaredMasters">The initially observed native master lists.</param>
    /// <param name="fileSystem">The file-system adapter used for native header reads.</param>
    /// <param name="cancellationToken">The token checked between header reads.</param>
    /// <exception cref="NativeSourceInputException">Thrown when native header metadata changed.</exception>
    private static void VerifyHeaderMetadataUnchanged(
        GameRelease release,
        IReadOnlyList<NativeSourcePluginInput> plugins,
        IReadOnlyList<IReadOnlyList<ModKey>> declaredMasters,
        IFileSystem fileSystem,
        CancellationToken cancellationToken)
    {
        for (var index = 0; index < plugins.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var header = ModHeaderFrame.FromPath(plugins[index].ModPath, release, fileSystem);
            var currentMasters = MasterReferenceCollection
                .FromModHeader(plugins[index].ModKey, header)
                .Masters
                .Select(master => master.Master)
                .ToArray();
            if (header.MasterStyle != plugins[index].MasterStyle
                || UsesLocalization(header) != plugins[index].UsesLocalization
                || !currentMasters.SequenceEqual(declaredMasters[index]))
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.ExternalChangeDetected,
                    $"Native plugin '{plugins[index].ModKey.FileName}' changed its header metadata during source preparation.");
            }
        }
    }

    /// <summary>Creates one ordered native strings lookup for each explicit plugin without consulting an installed game.</summary>
    /// <param name="release">The selected native release.</param>
    /// <param name="plugins">The explicit plugin descriptors.</param>
    /// <param name="dataDirectoryPath">The explicit directory used for archive lookup.</param>
    /// <param name="stringDirectoryPaths">The explicit loose strings directories in priority order.</param>
    /// <param name="fileSystem">The file-system adapter used by Mutagen.</param>
    /// <returns>An immutable plugin-keyed dictionary of native strings lookups.</returns>
    private static IReadOnlyDictionary<ModKey, NativeStringsFolderLookup> CreateStringsLookups(
        GameRelease release,
        IReadOnlyList<NativeSourcePluginInput> plugins,
        string dataDirectoryPath,
        IReadOnlyList<string> stringDirectoryPaths,
        IFileSystem fileSystem)
    {
        var result = new Dictionary<ModKey, NativeStringsFolderLookup>();
        foreach (var plugin in plugins)
        {
            if (!plugin.UsesLocalization)
            {
                continue;
            }

            var lookups = stringDirectoryPaths
                .Select(directoryPath =>
                {
                    var parameters = new StringsReadParameters
                    {
                        StringsFolderOverride = directoryPath,
                        BsaFolderOverride = dataDirectoryPath,
                        TargetLanguage = Language.English
                    };
                    return StringsFolderLookupOverlay.TypicalFactory(
                        release,
                        plugin.ModKey,
                        directoryPath,
                        parameters,
                        fileSystem);
                })
                .ToArray();
            result.Add(plugin.ModKey, new NativeStringsFolderLookup(lookups));
        }

        return new ReadOnlyDictionary<ModKey, NativeStringsFolderLookup>(result);
    }
}
