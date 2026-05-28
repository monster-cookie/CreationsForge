using SFRecordCompareEngine.Services.Interfaces;

namespace SFRecordCompareEngine.Services;

public class UserDialogService : IUserDialogService
{
    public async Task ShowErrorAsync(string message)
    {
        var page = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (page == null) return;

        await page.DisplayAlertAsync("Starfield Record Compare Engine", message, "OK");
    }
}
