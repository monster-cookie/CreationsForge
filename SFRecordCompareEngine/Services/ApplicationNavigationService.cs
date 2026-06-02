using Autofac;
using SFRecordCompareEngine.Services.Interfaces;
using SFRecordCompareEngine.ViewModels;
using SFRecordCompareEngine.Views;

namespace SFRecordCompareEngine.Services;

public class ApplicationNavigationService : IApplicationNavigationService
{
    private readonly IApplicationWindowService ApplicationWindowService;
    private readonly ILifetimeScope LifetimeScope;

    public ApplicationNavigationService(ILifetimeScope lifetimeScope, IApplicationWindowService applicationWindowService)
    {
        LifetimeScope = lifetimeScope;
        ApplicationWindowService = applicationWindowService;
    }

    public Task ShowMainPageAsync()
    {
        var viewModel = LifetimeScope.Resolve<MainPageViewModel>();
        var view = new MainView(viewModel);

        ApplicationWindowService.ShowMainCommandSurface(viewModel);
        ApplicationWindowService.SetContent(view);
        ApplicationWindowService.MaximizeMainWindow();
        return Task.CompletedTask;
    }

    public Task ShowStartupImportAsync(bool forceFullReimport)
    {
        var viewModel = LifetimeScope.Resolve<StartupImportViewModel>();
        viewModel.ForceFullReimport = forceFullReimport;
        var view = new StartupImportView(viewModel);

        ApplicationWindowService.HideMainCommandSurface();
        ApplicationWindowService.SetContent(view);
        ApplicationWindowService.MaximizeMainWindow();
        return Task.CompletedTask;
    }

    public async Task ShowOpenDialogAsync()
    {
        await ApplicationWindowService.ShowDialogAsync(LifetimeScope.Resolve<OpenPluginDialog>());
    }

    public async Task ShowSettingsDialogAsync()
    {
        await ApplicationWindowService.ShowDialogAsync(LifetimeScope.Resolve<SettingsDialog>());
    }

    public Task CloseOpenDialogAsync()
    {
        ApplicationWindowService.CloseDialog();
        return Task.CompletedTask;
    }

    public void Quit()
    {
        ApplicationWindowService.Quit();
    }
}
