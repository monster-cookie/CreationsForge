using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.ViewModels;

namespace CreationsForge.Services.Interfaces;

public interface IApplicationNavigationService
{
    Task ShowMainViewAsync(SupportedGameDTO? selectedGame, bool runConfiguredGameImport);

    Task ShowMainViewAsync(SupportedGameDTO? selectedGame, bool runConfiguredGameImport, PluginDTO selectedPlugin, IList<RecordTreeItemViewModel> recordTreeItems);

    Task ShowSettingsViewAsync();

    Task ShowActivePluginLoadViewAsync(SupportedGameDTO selectedGame, PluginDTO selectedPlugin);

    Task ShowImportProgressViewAsync(SupportedGameDTO selectedGame, bool forceFullReimport);

    Task ShowResetAndImportAllProgressViewAsync();

    void Quit();
}
