using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace CreationsForge.Workbench;

/// <summary>Hosts the explicit MCP connection and workspace controls.</summary>
public sealed partial class MainWindow : Window
{
    private readonly WorkbenchViewModel _viewModel;
    private bool _allowClose;

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
        await _viewModel.CloseAsync().ConfigureAwait(true);
        _allowClose = true;
        e.Cancel = false;
        Close();
    }

    private async void BrowseExecutableClick(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select the CreationsForge MCP executable",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("MCP executable") { Patterns = ["*.dll", "*.exe"] }],
        });
        if (files.Count > 0)
        {
            _viewModel.McpExecutablePath = files[0].Path.LocalPath;
        }
    }

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

    private async void ConnectClick(object? sender, RoutedEventArgs e) => await _viewModel.ConnectAsync();

    private async void DisconnectClick(object? sender, RoutedEventArgs e) => await _viewModel.CloseAsync();

    private async void OpenWorkspaceClick(object? sender, RoutedEventArgs e) => await _viewModel.OpenWorkspaceAsync();

    private async void CreateOutputClick(object? sender, RoutedEventArgs e) => await _viewModel.OpenOutputAsync(createNew: true);

    private async void OpenOutputClick(object? sender, RoutedEventArgs e) => await _viewModel.OpenOutputAsync(createNew: false);
}
