using Avalonia.Controls;
using Avalonia.Layout;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.Services;

public class UserDialogService : IUserDialogService
{
    private readonly IApplicationWindowService WindowService;

    public UserDialogService(IApplicationWindowService windowService)
    {
        WindowService = windowService;
    }

    public async Task<SupportedGameDTO?> ShowGameSelectionAsync(IReadOnlyList<SupportedGameDTO> supportedGames, SupportedGameDTO? selectedGame)
    {
        var gameSelector = new ComboBox
        {
            ItemsSource = supportedGames,
            SelectedItem = selectedGame ?? supportedGames.FirstOrDefault(),
            MinWidth = 320
        };

        var dialog = CreateDialog("Select Game", gameSelector, "Continue", "Exit");
        var result = await WindowService.ShowDialogAsync<bool>(dialog);

        return result
            ? gameSelector.SelectedItem as SupportedGameDTO
            : null;
    }

    public async Task<bool> ShowImportWarningAsync(SupportedGameDTO selectedGame, bool forceFullReimport)
    {
        var message = forceFullReimport
            ? $"A full {selectedGame.DisplayName} import can take 5-15 minutes depending on load order size."
            : $"{selectedGame.DisplayName} has not been imported yet. The first import can take 5-15 minutes depending on load order size.";

        var dialog = CreateDialog("Run Import", new TextBlock
        {
            Text = message,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            MaxWidth = 440
        }, "Import", "Cancel");

        return await WindowService.ShowDialogAsync<bool>(dialog);
    }

    public async Task<bool> ShowResetAndImportAllWarningAsync()
    {
        var dialog = CreateDialog("Reset & Import All", new TextBlock
        {
            Text = "This will delete the current CreationsForge database and run a full import for every supported game. This can take a while.",
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            MaxWidth = 440
        }, "Reset & Import All", "Cancel");

        return await WindowService.ShowDialogAsync<bool>(dialog);
    }

    public async Task ShowErrorAsync(string message)
    {
        var dialog = CreateDialog("CreationsForge", new TextBlock
        {
            Text = message,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            MaxWidth = 440
        }, "Close", null);

        await WindowService.ShowDialogAsync<bool>(dialog);
    }

    private static Window CreateDialog(string title, Control content, string primaryText, string? closeText)
    {
        var dialog = new Window
        {
            Title = title,
            Width = 520,
            Height = 220,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8
        };

        var primaryButton = new Button
        {
            Content = primaryText,
            MinWidth = 96
        };
        primaryButton.Click += (_, _) => dialog.Close(true);
        buttons.Children.Add(primaryButton);

        if (closeText is not null)
        {
            var closeButton = new Button
            {
                Content = closeText,
                MinWidth = 96
            };
            closeButton.Click += (_, _) => dialog.Close(false);
            buttons.Children.Add(closeButton);
        }

        dialog.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Spacing = 16,
            Children =
            {
                new TextBlock
                {
                    Text = title,
                    FontSize = 20,
                    FontWeight = Avalonia.Media.FontWeight.SemiBold
                },
                content,
                buttons
            }
        };

        return dialog;
    }
}
