using Autofac;
using Avalonia.Controls;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Views;

namespace CreationsForge.Services;

public class ApplicationNavigationService : IApplicationNavigationService, IDisposable
{
    private readonly ILifetimeScope RootScope;
    private readonly IApplicationWindowService WindowService;
    private ILifetimeScope? CurrentViewScope;

    public ApplicationNavigationService(
        ILifetimeScope rootScope,
        IApplicationWindowService windowService)
    {
        RootScope = rootScope;
        WindowService = windowService;
    }

    public Task ShowMainViewAsync(SupportedGameDTO? selectedGame, bool runConfiguredGameImport)
    {
        var mainView = CreateView<MainView>(out var viewScope);
        mainView.Configure(selectedGame, runConfiguredGameImport);
        SetCurrentView(mainView, viewScope);
        return Task.CompletedTask;
    }

    public Task ShowMainViewAsync(SupportedGameDTO? selectedGame, bool runConfiguredGameImport, PluginDTO selectedPlugin, IList<RecordTreeItemViewModel> recordTreeItems)
    {
        var mainView = CreateView<MainView>(out var viewScope);
        mainView.Configure(selectedGame, runConfiguredGameImport, selectedPlugin, recordTreeItems);
        SetCurrentView(mainView, viewScope);
        return Task.CompletedTask;
    }

    public Task ShowSettingsViewAsync()
    {
        SetCurrentView(CreateView<SettingsView>(out var viewScope), viewScope);
        return Task.CompletedTask;
    }

    public Task ShowActivePluginLoadViewAsync(SupportedGameDTO selectedGame, PluginDTO selectedPlugin)
    {
        var activePluginLoadView = CreateView<ActivePluginLoadView>(out var viewScope);
        activePluginLoadView.Configure(selectedGame, selectedPlugin);
        SetCurrentView(activePluginLoadView, viewScope);
        return Task.CompletedTask;
    }

    public Task ShowImportProgressViewAsync(SupportedGameDTO selectedGame, bool forceFullReimport)
    {
        var importProgressView = CreateView<ImportProgressView>(out var viewScope);
        importProgressView.Configure(selectedGame, forceFullReimport);
        SetCurrentView(importProgressView, viewScope);
        return Task.CompletedTask;
    }

    public Task ShowResetAndImportAllProgressViewAsync()
    {
        var importProgressView = CreateView<ImportProgressView>(out var viewScope);
        importProgressView.ConfigureResetAndImportAll();
        SetCurrentView(importProgressView, viewScope);
        return Task.CompletedTask;
    }

    public void Quit()
    {
        WindowService.Quit();
    }

    public void Dispose()
    {
        CurrentViewScope?.Dispose();
    }

    private TView CreateView<TView>(out ILifetimeScope viewScope)
        where TView : Control
    {
        viewScope = RootScope.BeginLifetimeScope();
        return viewScope.Resolve<TView>();
    }

    private void SetCurrentView(Control view, ILifetimeScope viewScope)
    {
        CurrentViewScope?.Dispose();
        CurrentViewScope = viewScope;
        WindowService.SetContent(view);
    }
}
