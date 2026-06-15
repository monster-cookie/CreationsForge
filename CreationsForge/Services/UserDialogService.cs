using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Views;

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

    public async Task<bool> ShowOpenPluginAsync(OpenPluginDialogViewModel viewModel)
    {
        var dialog = new Window
        {
            Title = "Open Plugin",
            Width = 1040,
            Height = 720,
            MinWidth = 900,
            MinHeight = 620,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = true
        };
        dialog.Content = new OpenPluginDialogView(viewModel, result => dialog.Close(result));

        return await WindowService.ShowDialogAsync<bool>(dialog);
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

    public async Task ShowHexPayloadAsync(string title, string payloadValue)
    {
        var textBox = new TextBox
        {
            Text = FormatHexPayload(payloadValue),
            IsReadOnly = true,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.NoWrap,
            FontFamily = FontFamily.Parse("Consolas"),
            FontSize = 13,
            MinWidth = 720,
            MinHeight = 420
        };
        ScrollViewer.SetHorizontalScrollBarVisibility(textBox, ScrollBarVisibility.Auto);
        ScrollViewer.SetVerticalScrollBarVisibility(textBox, ScrollBarVisibility.Auto);

        var dialog = CreateDialog(title, textBox, "Close", null);
        dialog.Width = 820;
        dialog.Height = 560;
        dialog.CanResize = true;

        await WindowService.ShowDialogAsync<bool>(dialog);
    }

    private static string FormatHexPayload(string payloadValue)
    {
        var normalizedHex = new string(payloadValue.Where(Uri.IsHexDigit).ToArray());
        if (normalizedHex.Length == 0)
        {
            return string.Empty;
        }

        var bytes = Enumerable.Range(0, (normalizedHex.Length + 1) / 2)
            .Select(index =>
            {
                var hexIndex = index * 2;
                return hexIndex + 1 < normalizedHex.Length
                    ? normalizedHex.Substring(hexIndex, 2)
                    : "0" + normalizedHex[hexIndex];
            })
            .ToList();

        var lines = new List<string>();
        for (var offset = 0; offset < bytes.Count; offset += 16)
        {
            var lineBytes = bytes.Skip(offset).Take(16).ToList();
            var hexText = string.Join(" ", lineBytes).PadRight(47);
            var stringText = string.Concat(lineBytes.Select(FormatPayloadByteCharacter));
            lines.Add($"{offset:X8}  {hexText}  {stringText}");
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static char FormatPayloadByteCharacter(string hexValue)
    {
        var value = Convert.ToByte(hexValue, 16);
        return value is >= 32 and <= 126
            ? (char)value
            : '.';
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
