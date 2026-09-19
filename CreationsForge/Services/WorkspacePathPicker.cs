using Avalonia.Platform.Storage;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.Services;

/// <summary>
/// Uses the main window's Avalonia storage provider to acquire explicit workspace paths.
/// </summary>
public sealed class WorkspacePathPicker : IWorkspacePathPicker
{
    /// <summary>Plugin file types supported by the workspace.</summary>
    private static readonly IReadOnlyList<FilePickerFileType> PluginFileTypes =
    [
        new FilePickerFileType("Bethesda plugins")
        {
            Patterns = ["*.esm", "*.esp", "*.esl"]
        },
        FilePickerFileTypes.All
    ];

    /// <summary>The window that owns platform storage pickers.</summary>
    private readonly MainWindow MainWindow;

    /// <summary>Initializes the path picker with the root application window.</summary>
    /// <param name="mainWindow">The window whose storage provider owns each picker.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="mainWindow"/> is <see langword="null"/>.</exception>
    public WorkspacePathPicker(MainWindow mainWindow)
    {
        ArgumentNullException.ThrowIfNull(mainWindow);
        MainWindow = mainWindow;
    }

    /// <inheritdoc />
    public async Task<string?> PickSourcePluginAsync(CancellationToken cancellationToken = default)
    {
        var paths = await PickPluginFilesAsync("Select read-only source plugin", allowMultiple: false, cancellationToken);
        return paths.FirstOrDefault();
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<string>> PickLoadOrderPluginsAsync(CancellationToken cancellationToken = default)
    {
        return PickPluginFilesAsync("Select plugins in load-order order", allowMultiple: true, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string?> PickDataDirectoryAsync(CancellationToken cancellationToken = default)
    {
        var paths = await PickFoldersAsync("Select game data directory", allowMultiple: false, cancellationToken);
        return paths.FirstOrDefault();
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<string>> PickStringDirectoriesAsync(CancellationToken cancellationToken = default)
    {
        return PickFoldersAsync("Select localized-string directories", allowMultiple: true, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string?> PickOutputPluginAsync(
        OutputSelectionMode mode,
        string? suggestedDirectoryPath = null,
        string? preferredExtension = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (mode == OutputSelectionMode.OpenExisting)
        {
            var paths = await PickPluginFilesAsync("Select existing output plugin", allowMultiple: false, cancellationToken);
            return paths.FirstOrDefault();
        }

        if (mode != OutputSelectionMode.CreateNew)
        {
            throw new ArgumentOutOfRangeException(nameof(mode));
        }

        var extension = preferredExtension?.TrimStart('.') ?? "esp";
        var selectedFileType = new FilePickerFileType($"{extension.ToUpperInvariant()} plugin")
        {
            Patterns = [$"*.{extension}"]
        };
        var startLocation = !string.IsNullOrWhiteSpace(suggestedDirectoryPath) && Directory.Exists(suggestedDirectoryPath)
            ? await MainWindow.StorageProvider.TryGetFolderFromPathAsync(suggestedDirectoryPath)
            : null;
        cancellationToken.ThrowIfCancellationRequested();
        var file = await MainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Select new output plugin",
            DefaultExtension = extension,
            SuggestedFileType = selectedFileType,
            FileTypeChoices = [selectedFileType],
            SuggestedStartLocation = startLocation,
            ShowOverwritePrompt = false
        });
        cancellationToken.ThrowIfCancellationRequested();
        return file?.TryGetLocalPath();
    }

    /// <summary>Opens a plugin picker and projects only local file-system paths.</summary>
    /// <param name="title">The platform picker title.</param>
    /// <param name="allowMultiple">Whether multiple plugins may be selected.</param>
    /// <param name="cancellationToken">A token checked before and after the platform picker.</param>
    /// <returns>The selected local paths in provider order.</returns>
    private async Task<IReadOnlyList<string>> PickPluginFilesAsync(
        string title,
        bool allowMultiple,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var files = await MainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = allowMultiple,
            FileTypeFilter = PluginFileTypes
        });
        cancellationToken.ThrowIfCancellationRequested();
        return files
            .Select(file => file.TryGetLocalPath())
            .Where(path => path is not null)
            .Cast<string>()
            .ToArray();
    }

    /// <summary>Opens a folder picker and projects only local file-system paths.</summary>
    /// <param name="title">The platform picker title.</param>
    /// <param name="allowMultiple">Whether multiple folders may be selected.</param>
    /// <param name="cancellationToken">A token checked before and after the platform picker.</param>
    /// <returns>The selected local paths in provider order.</returns>
    private async Task<IReadOnlyList<string>> PickFoldersAsync(
        string title,
        bool allowMultiple,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var folders = await MainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = allowMultiple
        });
        cancellationToken.ThrowIfCancellationRequested();
        return folders
            .Select(folder => folder.TryGetLocalPath())
            .Where(path => path is not null)
            .Cast<string>()
            .ToArray();
    }
}
