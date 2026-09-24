using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Engine.Persistence;

/// <summary>Publishes one staged plugin/string set with per-file backups and best-effort restoration.</summary>
internal sealed class PluginFilePublisher
{
    private readonly IPluginPersistenceBackend _backend;

    /// <summary>Initializes publication over the supplied filesystem boundary.</summary>
    public PluginFilePublisher(IPluginPersistenceBackend backend)
    {
        ArgumentNullException.ThrowIfNull(backend);
        _backend = backend;
    }

    /// <summary>Publishes one staged plugin/string set and restores the prior set on publication failure when possible.</summary>
    public PluginPublicationResult Publish(
        ModKey modKey,
        string stagingRoot,
        string stagedPluginPath,
        string destinationPluginPath,
        string backupRoot,
        Action<PluginSavePhase, string> report)
    {
        var destinationRoot = Path.GetDirectoryName(destinationPluginPath)
            ?? throw new InvalidOperationException($"Destination '{destinationPluginPath}' does not have a parent directory.");
        var stagedFiles = CreateRelativeMap(
            _backend.GetAssociatedFiles(modKey, stagedPluginPath),
            stagingRoot,
            "staged");
        var pluginRelativePath = Path.GetRelativePath(stagingRoot, stagedPluginPath);
        if (!stagedFiles.ContainsKey(pluginRelativePath))
        {
            throw new InvalidOperationException($"Mutagen export did not produce staged plugin '{stagedPluginPath}'.");
        }

        var desiredDestinations = stagedFiles.ToDictionary(
            pair => pair.Key,
            pair => Path.GetFullPath(Path.Combine(destinationRoot, pair.Key)),
            RelativePathComparer);
        var existingFiles = _backend.GetAssociatedFiles(modKey, destinationPluginPath);
        var existingByRelativePath = CreateRelativeMap(existingFiles, destinationRoot, "destination");
        var backupPaths = new List<string>(existingByRelativePath.Count);

        report(PluginSavePhase.Backup, $"Backing up {existingByRelativePath.Count} existing destination file(s).");
        try
        {
            foreach (var pair in existingByRelativePath.OrderBy(pair => pair.Key, RelativePathComparer))
            {
                var backupPath = Path.GetFullPath(Path.Combine(backupRoot, pair.Key));
                EnsureContainedPath(backupRoot, backupPath, "backup");
                EnsureParentDirectory(backupPath);
                _backend.CopyFile(pair.Value, backupPath, overwrite: false);
                backupPaths.Add(backupPath);
            }
        }
        catch (Exception exception)
        {
            return PluginPublicationResult.Failure(
                PluginPublicationState.Unchanged,
                [],
                ExistingPaths(backupPaths),
                $"Destination backup failed before publication: {exception.Message}");
        }

        var mutations = new List<PublicationMutation>();
        var publishedPaths = new List<string>(desiredDestinations.Count);
        try
        {
            report(PluginSavePhase.Publication, $"Publishing {desiredDestinations.Count} staged file(s) with the plugin last.");
            foreach (var pair in stagedFiles
                         .Where(pair => !RelativePathComparer.Equals(pair.Key, pluginRelativePath))
                         .OrderBy(pair => pair.Key, RelativePathComparer))
            {
                PublishFile(pair.Key, pair.Value, desiredDestinations[pair.Key], existingByRelativePath, mutations);
                publishedPaths.Add(desiredDestinations[pair.Key]);
            }

            foreach (var pair in existingByRelativePath
                         .Where(pair => !desiredDestinations.ContainsKey(pair.Key)
                             && !RelativePathComparer.Equals(pair.Key, pluginRelativePath))
                         .OrderBy(pair => pair.Key, RelativePathComparer))
            {
                mutations.Add(new PublicationMutation(pair.Key, pair.Value, ExistedBefore: true));
                _backend.DeleteFile(pair.Value);
            }

            PublishFile(
                pluginRelativePath,
                stagedFiles[pluginRelativePath],
                destinationPluginPath,
                existingByRelativePath,
                mutations);
            publishedPaths.Add(destinationPluginPath);

            return PluginPublicationResult.Success(publishedPaths, ExistingPaths(backupPaths));
        }
        catch (Exception publicationException)
        {
            report(PluginSavePhase.Restoration, "Publication failed; restoring destination files in reverse order.");
            var restorationFailures = RestoreMutations(mutations, backupRoot);
            var state = restorationFailures.Count == 0
                ? PluginPublicationState.Restored
                : PluginPublicationState.PartiallyPublished;
            var diagnostic = restorationFailures.Count == 0
                ? $"Publication failed and the prior destination file set was restored: {publicationException.Message}"
                : $"Publication failed and restoration also failed for {restorationFailures.Count} file(s): {publicationException.Message}";
            return PluginPublicationResult.Failure(
                state,
                ExistingPaths(publishedPaths),
                ExistingPaths(backupPaths),
                diagnostic);
        }
    }

    private void PublishFile(
        string relativePath,
        string stagedPath,
        string destinationPath,
        IReadOnlyDictionary<string, string> existingByRelativePath,
        ICollection<PublicationMutation> mutations)
    {
        EnsureParentDirectory(destinationPath);
        mutations.Add(new PublicationMutation(
            relativePath,
            destinationPath,
            existingByRelativePath.ContainsKey(relativePath)));
        _backend.MoveFile(stagedPath, destinationPath, overwrite: true);
    }

    private IReadOnlyList<string> RestoreMutations(
        IReadOnlyList<PublicationMutation> mutations,
        string backupRoot)
    {
        var failures = new List<string>();
        for (var index = mutations.Count - 1; index >= 0; index--)
        {
            var mutation = mutations[index];
            try
            {
                if (mutation.ExistedBefore)
                {
                    var backupPath = Path.GetFullPath(Path.Combine(backupRoot, mutation.RelativePath));
                    EnsureContainedPath(backupRoot, backupPath, "backup");
                    EnsureParentDirectory(mutation.DestinationPath);
                    _backend.CopyFile(backupPath, mutation.DestinationPath, overwrite: true);
                }
                else if (_backend.FileExists(mutation.DestinationPath))
                {
                    _backend.DeleteFile(mutation.DestinationPath);
                }
            }
            catch
            {
                failures.Add(mutation.DestinationPath);
            }
        }

        return failures;
    }

    private Dictionary<string, string> CreateRelativeMap(
        IEnumerable<string> paths,
        string root,
        string purpose)
    {
        var result = new Dictionary<string, string>(RelativePathComparer);
        foreach (var path in paths)
        {
            var fullPath = Path.GetFullPath(path);
            EnsureContainedPath(root, fullPath, purpose);
            var relativePath = Path.GetRelativePath(root, fullPath);
            if (!result.TryAdd(relativePath, fullPath))
            {
                throw new InvalidOperationException($"The {purpose} file set contains duplicate relative path '{relativePath}'.");
            }
        }

        return result;
    }

    private static void EnsureContainedPath(string root, string path, string purpose)
    {
        var fullRoot = Path.GetFullPath(root);
        var fullPath = Path.GetFullPath(path);
        var relativePath = Path.GetRelativePath(fullRoot, fullPath);
        if (relativePath == ".."
            || relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
            || Path.IsPathRooted(relativePath))
        {
            throw new InvalidOperationException($"The {purpose} path '{fullPath}' resolves outside '{fullRoot}'.");
        }
    }

    private void EnsureParentDirectory(string path)
    {
        var parent = Path.GetDirectoryName(path)
            ?? throw new InvalidOperationException($"File path '{path}' does not have a parent directory.");
        if (!_backend.DirectoryExists(parent))
        {
            _backend.CreateDirectory(parent);
        }
    }

    private IReadOnlyList<string> ExistingPaths(IEnumerable<string> paths)
    {
        return paths.Where(_backend.FileExists).ToArray();
    }

    private static StringComparer RelativePathComparer => OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    private sealed record PublicationMutation(string RelativePath, string DestinationPath, bool ExistedBefore);
}
