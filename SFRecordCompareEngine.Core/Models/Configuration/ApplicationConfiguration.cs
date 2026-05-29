namespace SFRecordCompareEngine.Core.Models.Configuration;

public class ApplicationConfiguration
{
    public string? SelectedGame { get; set; }
    public ApplicationThemeMode Theme { get; set; } = ApplicationThemeMode.Dark;
}
