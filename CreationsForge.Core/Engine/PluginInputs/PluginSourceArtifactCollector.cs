using System.IO.Abstractions;
using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace CreationsForge.Core.Engine.PluginInputs;

/// <summary>Captures the complete deterministic physical artifact set considered by one explicit plugin source request.</summary>
internal sealed class PluginSourceArtifactCollector
{
    /// <summary>The selected plugin game release.</summary>
    private readonly GameRelease Release;

    /// <summary>The explicit plugin descriptors in load-order order.</summary>
    private readonly IReadOnlyList<PluginSourcePluginInput> Plugins;

    /// <summary>The explicit directory used for applicable plugin archive discovery.</summary>
    private readonly string DataDirectoryPath;

    /// <summary>The explicit loose strings directories in lookup-priority order.</summary>
    private readonly IReadOnlyList<string> StringDirectoryPaths;

    /// <summary>The file-system adapter supplied to supported Mutagen discovery APIs.</summary>
    private readonly IFileSystem FileSystem;

    /// <summary>Initializes a deterministic source artifact collector.</summary>
    /// <param name="release">The selected plugin game release.</param>
    /// <param name="plugins">The explicit plugins in load-order order.</param>
    /// <param name="dataDirectoryPath">The explicit archive discovery directory.</param>
    /// <param name="stringDirectoryPaths">The explicit loose strings directories in priority order.</param>
    /// <param name="fileSystem">The file-system adapter used by Mutagen archive discovery.</param>
    internal PluginSourceArtifactCollector(
        GameRelease release,
        IReadOnlyList<PluginSourcePluginInput> plugins,
        string dataDirectoryPath,
        IReadOnlyList<string> stringDirectoryPaths,
        IFileSystem fileSystem)
    {
        Release = release;
        Plugins = plugins;
        DataDirectoryPath = dataDirectoryPath;
        StringDirectoryPaths = stringDirectoryPaths;
        FileSystem = fileSystem;
    }

    /// <summary>Captures plugin content, every possible release language sidecar state, and each currently applicable archive.</summary>
    /// <param name="cancellationToken">The token checked during discovery and hashing.</param>
    /// <returns>The complete artifact set in stable plugin, directory, sidecar, language, and archive order.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    /// <exception cref="PluginSourceInputException">Thrown when an artifact cannot be observed safely.</exception>
    internal async Task<IReadOnlyList<PluginArtifactAssociation>> CaptureAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PluginFileInspector.VerifyDirectory(DataDirectoryPath, "data directory");
        foreach (var directoryPath in StringDirectoryPaths)
        {
            cancellationToken.ThrowIfCancellationRequested();
            PluginFileInspector.VerifyDirectory(directoryPath, "localized-string directory");
        }

        var artifacts = new List<PluginArtifactAssociation>();
        foreach (var plugin in Plugins)
        {
            cancellationToken.ThrowIfCancellationRequested();
            artifacts.Add(await PluginFileInspector.InspectAsync(
                plugin.Path,
                PluginArtifactRole.Plugin,
                null,
                mustExist: true,
                cancellationToken).ConfigureAwait(false));
        }

        var constants = GameConstants.Get(Release);
        var languageFormat = constants.StringsLanguageFormat
            ?? throw new PluginSourceInputException(
                EngineErrorCode.UnsupportedInput,
                $"Plugin localized-string discovery is unavailable for release {Release}.");
        foreach (var plugin in Plugins)
        {
            if (!plugin.UsesLocalization)
            {
                continue;
            }

            foreach (var directoryPath in StringDirectoryPaths)
            {
                foreach (var source in OrderedStringsSources)
                {
                    foreach (var language in constants.Languages.OrderBy(language => language))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var fileName = StringsUtility.GetFileName(languageFormat, plugin.ModKey, language, source);
                        var path = Path.GetFullPath(Path.Combine(directoryPath, fileName));
                        artifacts.Add(await PluginFileInspector.InspectAsync(
                            path,
                            MapRole(source),
                            language.ToString(),
                            mustExist: false,
                            cancellationToken).ConfigureAwait(false));
                    }
                }
            }
        }

        var archiveTargets = new Dictionary<string, HashSet<string>>(PathComparer);
        foreach (var plugin in Plugins)
        {
            if (!plugin.UsesLocalization)
            {
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();
            var applicablePaths = FileSystem.Directory
                .EnumerateFiles(DataDirectoryPath)
                .Where(path => Archive.IsApplicable(
                    Release,
                    plugin.ModKey,
                    new FileName(Path.GetFileName(path))))
                .Select(path => Path.GetFullPath(path.ToString()))
                .OrderBy(path => path, PathComparer)
                .ToArray();
            foreach (var path in applicablePaths)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!archiveTargets.TryGetValue(path, out var targetFileNames))
                {
                    targetFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    archiveTargets.Add(path, targetFileNames);
                }

                foreach (var source in OrderedStringsSources)
                {
                    foreach (var language in constants.Languages.OrderBy(language => language))
                    {
                        targetFileNames.Add(StringsUtility
                            .GetFileName(languageFormat, plugin.ModKey, language, source)
                            .ToString());
                    }
                }
            }
        }

        foreach (var archiveTarget in archiveTargets.OrderBy(pair => pair.Key, PathComparer))
        {
            artifacts.Add(await PluginFileInspector.InspectArchiveStringsAsync(
                archiveTarget.Key,
                Release,
                archiveTarget.Value,
                FileSystem,
                cancellationToken).ConfigureAwait(false));
        }

        return Array.AsReadOnly(artifacts.ToArray());
    }

    /// <summary>Maps a record strings source to its physical sidecar role.</summary>
    /// <param name="source">The record strings source.</param>
    /// <returns>The corresponding source artifact role.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown for an unknown record strings source.</exception>
    private static PluginArtifactRole MapRole(StringsSource source)
    {
        return source switch
        {
            StringsSource.Normal => PluginArtifactRole.Strings,
            StringsSource.DL => PluginArtifactRole.DlStrings,
            StringsSource.IL => PluginArtifactRole.IlStrings,
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }

    /// <summary>Gets the record strings sources in stable artifact order.</summary>
    private static IReadOnlyList<StringsSource> OrderedStringsSources { get; } = Array.AsReadOnly(
        new[] { StringsSource.Normal, StringsSource.DL, StringsSource.IL });

    /// <summary>Compares canonical artifact paths according to the host file system.</summary>
    private static StringComparer PathComparer { get; } = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;
}
