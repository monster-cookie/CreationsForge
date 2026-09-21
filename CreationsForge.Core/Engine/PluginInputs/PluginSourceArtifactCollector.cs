using System.IO.Abstractions;
using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace CreationsForge.Core.Engine.PluginInputs;

/// <summary>Captures the deterministic physical inventory and read locks for one explicit plugin source request.</summary>
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

    /// <summary>The lifetime owner for every existing source artifact.</summary>
    private readonly PluginSourceFileLockSet SourceLocks;

    /// <summary>Initializes a deterministic source artifact collector.</summary>
    /// <param name="release">The selected plugin game release.</param>
    /// <param name="plugins">The explicit plugins in load-order order.</param>
    /// <param name="dataDirectoryPath">The explicit archive discovery directory.</param>
    /// <param name="stringDirectoryPaths">The explicit loose strings directories in priority order.</param>
    /// <param name="fileSystem">The file-system adapter used by Mutagen archive discovery.</param>
    /// <param name="sourceLocks">The lifetime owner receiving each existing source handle.</param>
    internal PluginSourceArtifactCollector(
        GameRelease release,
        IReadOnlyList<PluginSourcePluginInput> plugins,
        string dataDirectoryPath,
        IReadOnlyList<string> stringDirectoryPaths,
        IFileSystem fileSystem,
        PluginSourceFileLockSet sourceLocks)
    {
        Release = release;
        Plugins = plugins;
        DataDirectoryPath = dataDirectoryPath;
        StringDirectoryPaths = stringDirectoryPaths;
        FileSystem = fileSystem;
        SourceLocks = sourceLocks;
    }

    /// <summary>Captures plugin metadata, every possible release-language sidecar state, and each currently applicable archive while retaining read locks.</summary>
    /// <param name="cancellationToken">The token checked during discovery and lock acquisition.</param>
    /// <returns>The complete artifact inventory in stable plugin, directory, sidecar, language, and archive order.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    /// <exception cref="PluginSourceInputException">Thrown when an artifact cannot be locked or observed safely.</exception>
    internal IReadOnlyList<PluginArtifactAssociation> Capture(CancellationToken cancellationToken)
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
            artifacts.Add(SourceLocks.ObserveAndLock(
                plugin.Path,
                PluginArtifactRole.Plugin,
                null,
                mustExist: true,
                cancellationToken));
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
                        artifacts.Add(SourceLocks.ObserveAndLock(
                            path,
                            MapRole(source),
                            language.ToString(),
                            mustExist: false,
                            cancellationToken));
                    }
                }
            }
        }

        var archivePaths = new HashSet<string>(PathComparer);
        foreach (var plugin in Plugins)
        {
            if (!plugin.UsesLocalization)
            {
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();
            foreach (var path in FileSystem.Directory
                         .EnumerateFiles(DataDirectoryPath)
                         .Where(path => Archive.IsApplicable(
                             Release,
                             plugin.ModKey,
                             new FileName(Path.GetFileName(path))))
                         .Select(path => Path.GetFullPath(path.ToString())))
            {
                archivePaths.Add(path);
            }
        }

        foreach (var archivePath in archivePaths.OrderBy(path => path, PathComparer))
        {
            cancellationToken.ThrowIfCancellationRequested();
            artifacts.Add(SourceLocks.ObserveAndLock(
                archivePath,
                PluginArtifactRole.StringsArchive,
                null,
                mustExist: true,
                cancellationToken));
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
