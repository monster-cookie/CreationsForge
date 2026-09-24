using CreationsForge.Engine.Interfaces;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.IO;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Engine.Persistence;

/// <summary>Uses Mutagen's native export and associated-file facilities with the operating-system filesystem.</summary>
internal sealed class PluginPersistenceBackend : IPluginPersistenceBackend
{
    private const AssociatedModFileCategory PersistedCategories =
        AssociatedModFileCategory.Plugin | AssociatedModFileCategory.RawStrings;

    /// <inheritdoc />
    public Task ExportAsync(
        IModGetter output,
        string path,
        string dataDirectory,
        IReadOnlyList<IModMasterStyledGetter> loadOrder,
        Language targetLanguage)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(dataDirectory);
        ArgumentNullException.ThrowIfNull(loadOrder);

        return output.BeginWrite
            .ToPath(path)
            .WithLoadOrder(loadOrder)
            .WithDataFolder(dataDirectory)
            .WithTargetLanguage(targetLanguage)
            .WithExtraIncludedMasters(output.MasterReferences.Select(reference => reference.Master))
            .WriteAsync();
    }

    /// <inheritdoc />
    public IMod OpenOutput(
        IGameIntegration integration,
        ModPath path,
        IReadOnlyList<IModMasterStyledGetter> knownMasters,
        Language targetLanguage)
    {
        ArgumentNullException.ThrowIfNull(integration);
        ArgumentNullException.ThrowIfNull(knownMasters);
        return integration.OpenExistingOutput(path, knownMasters, targetLanguage);
    }

    /// <inheritdoc />
    public PluginDestinationStamp? CaptureStamp(ModKey modKey, string pluginPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginPath);
        var root = Path.GetDirectoryName(Path.GetFullPath(pluginPath))
            ?? throw new InvalidOperationException($"Plugin path '{pluginPath}' does not have a parent directory.");
        var entries = GetAssociatedFiles(modKey, pluginPath)
            .Where(File.Exists)
            .Select(path =>
            {
                var file = new FileInfo(path);
                file.Refresh();
                return new PluginFileStamp(
                    Path.GetRelativePath(root, file.FullName),
                    file.Length,
                    file.LastWriteTimeUtc);
            })
            .ToArray();
        if (entries.Length == 0)
        {
            return null;
        }

        return new PluginDestinationStamp(entries);
    }

    /// <inheritdoc />
    public IReadOnlyList<string> GetAssociatedFiles(ModKey modKey, string pluginPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginPath);
        return PluginUtilityIO
            .GetAssociatedFiles(new ModPath(modKey, pluginPath), PersistedCategories)
            .Select(path => Path.GetFullPath(path.Path))
            .ToArray();
    }

    /// <inheritdoc />
    public void CreateDirectory(string path) => Directory.CreateDirectory(path);

    /// <inheritdoc />
    public void CopyFile(string sourcePath, string destinationPath, bool overwrite) =>
        File.Copy(sourcePath, destinationPath, overwrite);

    /// <inheritdoc />
    public void MoveFile(string sourcePath, string destinationPath, bool overwrite) =>
        File.Move(sourcePath, destinationPath, overwrite);

    /// <inheritdoc />
    public void DeleteFile(string path) => File.Delete(path);

    /// <inheritdoc />
    public void DeleteDirectory(string path, bool recursive) => Directory.Delete(path, recursive);

    /// <inheritdoc />
    public bool FileExists(string path) => File.Exists(path);

    /// <inheritdoc />
    public bool DirectoryExists(string path) => Directory.Exists(path);
}
