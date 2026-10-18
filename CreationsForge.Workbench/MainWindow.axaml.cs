using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Serilog;

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
        catch (Exception exception)
        {
            Log.Error(exception, "Workbench window close failed. The window will still close.");
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

    /// <summary>Cancels the active startup or tool call. A connected host keeps running.</summary>
    /// <param name="sender">The cancel button.</param>
    /// <param name="e">The click arguments.</param>
    private async void CancelClick(object? sender, RoutedEventArgs e) => await _viewModel.CancelAsync();

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

    /// <summary>Searches the open output for records in the selected family.</summary>
    /// <param name="sender">The find button.</param>
    /// <param name="e">The click arguments.</param>
    private async void FindRecordsClick(object? sender, RoutedEventArgs e) => await _viewModel.FindRecordsAsync();

    /// <summary>Starts an empty draft for a new record.</summary>
    /// <param name="sender">The new button.</param>
    /// <param name="e">The click arguments.</param>
    private async void NewRecordClick(object? sender, RoutedEventArgs e) => await _viewModel.NewRecordAsync();

    /// <summary>Reads the selected search hit into an override draft.</summary>
    /// <param name="sender">The override button.</param>
    /// <param name="e">The click arguments.</param>
    private async void OverrideRecordClick(object? sender, RoutedEventArgs e) => await _viewModel.OverrideSelectedAsync();

    /// <summary>Sends the current draft. A failure keeps the form and does not report a save.</summary>
    /// <param name="sender">The apply button.</param>
    /// <param name="e">The click arguments.</param>
    private async void ApplyRecordClick(object? sender, RoutedEventArgs e) => await _viewModel.ApplyRecordAsync();

    /// <summary>Compares the exact source version with the output version.</summary>
    /// <param name="sender">The compare button.</param>
    /// <param name="e">The click arguments.</param>
    private async void CompareRecordClick(object? sender, RoutedEventArgs e) => await _viewModel.CompareRecordAsync();

    /// <summary>Shows unsaved records without writing the output file.</summary>
    /// <param name="sender">The preview button.</param>
    /// <param name="e">The click arguments.</param>
    private async void PreviewChangesClick(object? sender, RoutedEventArgs e) => await _viewModel.PreviewChangesAsync();

    /// <summary>Saves the workspace. A failure does not report a successful save.</summary>
    /// <param name="sender">The save button.</param>
    /// <param name="e">The click arguments.</param>
    private async void SaveChangesClick(object? sender, RoutedEventArgs e) => await _viewModel.SaveChangesAsync();

    /// <summary>Discards unsaved edits without writing the output file.</summary>
    /// <param name="sender">The discard button.</param>
    /// <param name="e">The click arguments.</param>
    private async void DiscardChangesClick(object? sender, RoutedEventArgs e) => await _viewModel.DiscardChangesAsync();

    /// <summary>Opens the recorded output in a new MCP process.</summary>
    /// <param name="sender">The reopen button.</param>
    /// <param name="e">The click arguments.</param>
    private async void ReopenFreshClick(object? sender, RoutedEventArgs e) => await _viewModel.ReopenFreshAsync();

    /// <summary>Adds a language row to the translated field that owns the button.</summary>
    /// <param name="sender">The add button.</param>
    /// <param name="e">The click arguments.</param>
    private void AddTranslationClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: WorkbenchFieldDraft field })
        {
            field.AddTranslation(field.TargetLanguage);
        }
    }

    /// <summary>Removes the language row that owns the button.</summary>
    /// <param name="sender">The remove button.</param>
    /// <param name="e">The click arguments.</param>
    private void RemoveTranslationClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: WorkbenchTranslationRow row })
        {
            row.Remove();
        }
    }
}
