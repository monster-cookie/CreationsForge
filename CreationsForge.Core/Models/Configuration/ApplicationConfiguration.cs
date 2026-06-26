using CreationsForge.Core.Configuration;

namespace CreationsForge.Core.Models.Configuration;

public enum ApplicationThemeMode
{
    Dark,
    Light
}

public enum ApplicationThemeFamily
{
    Semi,
    Fluent
}

public class ApplicationConfiguration
{
    public const string DefaultRecordTextLanguage = "English";

    public string? ActiveGame { get; set; }

    public ApplicationThemeMode ThemeMode { get; set; } = ApplicationThemeMode.Dark;

    public ApplicationThemeFamily ThemeFamily { get; set; } = ApplicationThemeFamily.Semi;

    public string RecordTextLanguage { get; set; } = DefaultRecordTextLanguage;

    public string? NifSkopeExecutablePath { get; set; }

    public string ApplicationDataDirectory { get; set; } = ApplicationConfigurationStore.DefaultApplicationDataDirectory;

    public string DatabaseDirectory { get; set; } = ApplicationConfigurationStore.DefaultDatabaseDirectory;

    public string LoggingDirectory { get; set; } = ApplicationConfigurationStore.DefaultLoggingDirectory;
}
