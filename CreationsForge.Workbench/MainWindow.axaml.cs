using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace CreationsForge.Workbench;

/// <summary>Hosts the explicit MCP connection and workspace controls.</summary>
public sealed partial class MainWindow : Window
{
    private readonly WorkbenchViewModel _viewModel;
    private bool _allowClose;
    private Task? _windowClose;

    /// <summary>Initializes the Workbench window and its UI-neutral state owner.</summary>
    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new WorkbenchViewModel();
        DataContext = _viewModel;
    }

    /// <inheritdoc />
    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        if (_allowClose)
        {
            return;
        }

        e.Cancel = true;
        var ownsClose = false;
        if (_windowClose is null)
        {
            _windowClose = _viewModel.CloseAsync();
            ownsClose = true;
        }

        try
        {
            await _windowClose.ConfigureAwait(true);
        }
        catch (Exception)
        {
            // Closing still has to finish. The view model records cleanup failures in diagnostics.
        }

        if (!ownsClose)
        {
            return;
        }

        _allowClose = true;
        Close();
    }

    /// <summary>Browses for a production MCP executable, including an extensionless apphost.</summary>
    /// <param name="sender">The browse button.</param>
    /// <param name="e">The click arguments.</param>
    private async void BrowseExecutableClick(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select the CreationsForge MCP executable",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("MCP executable") { Patterns = ["*.dll", "*.exe"] },
                new FilePickerFileType("All files") { Patterns = ["*"] },
            ],
        });
        if (files.Count > 0)
        {
            _viewModel.McpExecutablePath = files[0].Path.LocalPath;
        }
    }

    /// <summary>Browses for the game Data directory.</summary>
    /// <param name="sender">The browse button.</param>
    /// <param name="e">The click arguments.</param>
    private async void BrowseDataDirectoryClick(object? sender, RoutedEventArgs e)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select the game Data directory",
            AllowMultiple = false,
        });
        if (folders.Count > 0)
        {
            _viewModel.DataDirectory = folders[0].Path.LocalPath;
        }
    }

    /// <summary>Browses for the explicit output plugin path.</summary>
    /// <param name="sender">The browse button.</param>
    /// <param name="e">The click arguments.</param>
    private async void BrowseOutputClick(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Select the output plugin",
            SuggestedFileName = "CreationsForgeOutput.esp",
            FileTypeChoices = [new FilePickerFileType("Bethesda plugin") { Patterns = ["*.esp", "*.esm", "*.esl"] }],
        });
        if (files is not null)
        {
            _viewModel.OutputPath = files.Path.LocalPath;
        }
    }

    /// <summary>Starts the selected MCP executable.</summary>
    /// <param name="sender">The connect button.</param>
    /// <param name="e">The click arguments.</param>
    private async void ConnectClick(object? sender, RoutedEventArgs e) => await _viewModel.ConnectAsync();

    /// <summary>Cancels startup or stops the current MCP child.</summary>
    /// <param name="sender">The disconnect button.</param>
    /// <param name="e">The click arguments.</param>
    private async void DisconnectClick(object? sender, RoutedEventArgs e) => await _viewModel.CloseAsync();

    /// <summary>Opens a workspace with the explicit source selections.</summary>
    /// <param name="sender">The open workspace button.</param>
    /// <param name="e">The click arguments.</param>
    private async void OpenWorkspaceClick(object? sender, RoutedEventArgs e) => await _viewModel.OpenWorkspaceAsync();

    /// <summary>Creates a new output plugin on the open workspace.</summary>
    /// <param name="sender">The create output button.</param>
    /// <param name="e">The click arguments.</param>
    private async void CreateOutputClick(object? sender, RoutedEventArgs e) => await _viewModel.OpenOutputAsync(createNew: true);

    /// <summary>Opens an existing output plugin on the open workspace.</summary>
    /// <param name="sender">The open output button.</param>
    /// <param name="e">The click arguments.</param>
    private async void OpenOutputClick(object? sender, RoutedEventArgs e) => await _viewModel.OpenOutputAsync(createNew: false);
}
