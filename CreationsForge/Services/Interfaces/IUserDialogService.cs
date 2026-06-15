using CreationsForge.Core.DTOs.Games;
using CreationsForge.ViewModels;

namespace CreationsForge.Services.Interfaces;

public interface IUserDialogService
{
    Task<SupportedGameDTO?> ShowGameSelectionAsync(IReadOnlyList<SupportedGameDTO> supportedGames, SupportedGameDTO? selectedGame);

    Task<bool> ShowOpenPluginAsync(OpenPluginDialogViewModel viewModel);

    Task<bool> ShowImportWarningAsync(SupportedGameDTO selectedGame, bool forceFullReimport);

    Task<bool> ShowResetAndImportAllWarningAsync();

    Task ShowHexPayloadAsync(string title, string payloadValue);

    Task ShowErrorAsync(string message);
}
