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

/// <summary>
/// Stores user-facing CreationsForge settings that are serialized to the local JSON configuration file.
/// </summary>
public class ApplicationConfiguration
{
    public const string DefaultRecordTextLanguage = "English";

    public string? ActiveGame { get; set; }

    public ApplicationThemeMode ThemeMode { get; set; } = ApplicationThemeMode.Dark;

    public ApplicationThemeFamily ThemeFamily { get; set; } = ApplicationThemeFamily.Semi;

    public string RecordTextLanguage { get; set; } = DefaultRecordTextLanguage;

    public string? NifSkopeExecutablePath { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether plugin selection lists hide a matching ESM when an ESP with the same
    /// base filename is also available.
    /// </summary>
    public bool PreferEspOverMatchingEsm { get; set; } = true;

    public string ApplicationDataDirectory { get; set; } = ApplicationConfigurationStore.DefaultApplicationDataDirectory;

    public string DatabaseDirectory { get; set; } = ApplicationConfigurationStore.DefaultDatabaseDirectory;

    public string LoggingDirectory { get; set; } = ApplicationConfigurationStore.DefaultLoggingDirectory;
}
