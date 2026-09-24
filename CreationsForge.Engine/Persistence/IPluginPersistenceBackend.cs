using CreationsForge.Engine.Interfaces;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Engine.Persistence;

/// <summary>Provides the narrow native IO and filesystem boundary needed by plugin persistence.</summary>
internal interface IPluginPersistenceBackend
{
    /// <summary>Exports a native output with explicit data-directory and load-order context.</summary>
    Task ExportAsync(
        IModGetter output,
        string path,
        string dataDirectory,
        IReadOnlyList<IModMasterStyledGetter> loadOrder,
        Language targetLanguage);

    /// <summary>Opens a complete mutable output through the selected game integration.</summary>
    IMod OpenOutput(
        IGameIntegration integration,
        ModPath path,
        IReadOnlyList<IModMasterStyledGetter> knownMasters,
        Language targetLanguage);

    /// <summary>Captures the existing plugin and raw-string file-set identity, or <see langword="null"/> when none exists.</summary>
    PluginDestinationStamp? CaptureStamp(ModKey modKey, string pluginPath);

    /// <summary>Gets existing plugin and raw-string paths associated with an output identity.</summary>
    IReadOnlyList<string> GetAssociatedFiles(ModKey modKey, string pluginPath);

    /// <summary>Creates a directory and any missing parents.</summary>
    void CreateDirectory(string path);

    /// <summary>Copies one file with explicit overwrite behavior.</summary>
    void CopyFile(string sourcePath, string destinationPath, bool overwrite);

    /// <summary>Moves one file with explicit overwrite behavior.</summary>
    void MoveFile(string sourcePath, string destinationPath, bool overwrite);

    /// <summary>Deletes one file when it exists.</summary>
    void DeleteFile(string path);

    /// <summary>Deletes one directory with explicit recursive behavior.</summary>
    void DeleteDirectory(string path, bool recursive);

    /// <summary>Gets whether a file currently exists.</summary>
    bool FileExists(string path);

    /// <summary>Gets whether a directory currently exists.</summary>
    bool DirectoryExists(string path);
}
