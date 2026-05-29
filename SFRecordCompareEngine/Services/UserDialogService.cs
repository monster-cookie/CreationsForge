using Microsoft.UI.Xaml.Controls;
using SFRecordCompareEngine.Services.Interfaces;

namespace SFRecordCompareEngine.Services;

public class UserDialogService : IUserDialogService
{
    private readonly IApplicationWindowService ApplicationWindowService;

    public UserDialogService(IApplicationWindowService applicationWindowService)
    {
        ApplicationWindowService = applicationWindowService;
    }

    public async Task ShowErrorAsync(string message)
    {
        var dialog = new ContentDialog
        {
            Title = "Starfield Record Compare Engine",
            Content = message,
            CloseButtonText = "OK"
        };

        await ApplicationWindowService.ShowDialogAsync(dialog);
    }
}
