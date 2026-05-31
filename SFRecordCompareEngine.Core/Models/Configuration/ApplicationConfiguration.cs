using SFRecordCompareEngine.Core.Configuration;

namespace SFRecordCompareEngine.Core.Models.Configuration;

public class ApplicationConfiguration
{
    public ApplicationThemeMode Theme { get; set; } = ApplicationThemeMode.Dark;

    public string ApplicationDataDirectory { get; set; } = ApplicationConfigurationStore.DefaultApplicationDataDirectory;

    public string DatabaseDirectory { get; set; } = ApplicationConfigurationStore.DefaultDatabaseDirectory;

    public string LoggingDirectory { get; set; } = ApplicationConfigurationStore.DefaultLoggingDirectory;
}