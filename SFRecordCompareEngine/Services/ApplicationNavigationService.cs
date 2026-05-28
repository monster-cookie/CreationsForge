using Autofac;
using SFRecordCompareEngine.Pages;
using SFRecordCompareEngine.Services.Interfaces;

namespace SFRecordCompareEngine.Services;

public class ApplicationNavigationService : IApplicationNavigationService
{
    private readonly ILifetimeScope LifetimeScope;
    private readonly IApplicationWindowService ApplicationWindowService;

    public ApplicationNavigationService(ILifetimeScope lifetimeScope, IApplicationWindowService applicationWindowService)
    {
        LifetimeScope = lifetimeScope;
        ApplicationWindowService = applicationWindowService;
    }

    public Task ShowMainPageAsync()
    {
        var window = Application.Current?.Windows.FirstOrDefault();
        if (window == null) return Task.CompletedTask;

        window.Page = LifetimeScope.Resolve<MainPage>();
        ApplicationWindowService.MaximizeMainWindow();
        return Task.CompletedTask;
    }

    public async Task ShowOpenDialogAsync()
    {
        var page = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (page == null) return;

        await page.Navigation.PushModalAsync(LifetimeScope.Resolve<OpenPluginDialogPage>());
    }

    public async Task CloseOpenDialogAsync()
    {
        var page = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (page == null) return;

        await page.Navigation.PopModalAsync();
    }

    public void Quit()
    {
        Application.Current?.Quit();
    }
}
