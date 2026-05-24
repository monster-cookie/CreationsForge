using System.Windows.Input;
using SFRecordCompareEngine.Commands;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.Models.Configuration;

namespace SFRecordCompareEngine.ViewModels;

public class StartupConfigurationDialogViewModel : ViewModelBase
{
    private readonly IApplicationConfigurationStore ApplicationConfigurationStore;
    private readonly IGameConfigurationStore GameConfigurationStore;
    private string? _selectedGame;
    private string _statusText = string.Empty;

    public StartupConfigurationDialogViewModel(
        IApplicationConfigurationStore applicationConfigurationStore,
        IGameConfigurationStore gameConfigurationStore)
    {
        ApplicationConfigurationStore = applicationConfigurationStore;
        GameConfigurationStore = gameConfigurationStore;

        SupportedGames = gameConfigurationStore.SupportedGames;
        _selectedGame = string.IsNullOrWhiteSpace(applicationConfigurationStore.Current.SelectedGame)
            ? SupportedGames.FirstOrDefault(game => !string.Equals(game, "None", StringComparison.OrdinalIgnoreCase))
            : applicationConfigurationStore.Current.SelectedGame;
        ContinueCommand = new RelayCommand(() => { }, CanContinue);
    }

    public string[] SupportedGames { get; }

    public ICommand ContinueCommand { get; }

    public string? SelectedGame
    {
        get => _selectedGame;
        set
        {
            if (!SetProperty(ref _selectedGame, value))
            {
                return;
            }

            RaiseCommandStates();
        }
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    public bool TrySave()
    {
        if (!CanContinue())
        {
            StatusText = "Select a supported game.";
            return false;
        }

        GameConfigurationStore.SelectGame(SelectedGame);
        if (GameConfigurationStore.Game is null)
        {
            StatusText = $"{SelectedGame} is not configured yet.";
            return false;
        }

        ApplicationConfigurationStore.Save(new ApplicationConfiguration
        {
            SelectedGame = GameConfigurationStore.SelectedGame
        });
        StatusText = "Configuration saved.";
        return true;
    }

    private bool CanContinue()
    {
        return !string.IsNullOrWhiteSpace(SelectedGame)
            && !string.Equals(SelectedGame, "None", StringComparison.OrdinalIgnoreCase);
    }

    private void RaiseCommandStates()
    {
        if (ContinueCommand is RelayCommand continueCommand)
        {
            continueCommand.RaiseCanExecuteChanged();
        }
    }
}
