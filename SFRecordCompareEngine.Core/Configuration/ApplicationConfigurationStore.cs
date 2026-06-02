using System.Text.Json;
using System.Text.Json.Serialization;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.Models.Configuration;

namespace SFRecordCompareEngine.Core.Configuration;

public class ApplicationConfigurationStore : IApplicationConfigurationStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public ApplicationConfigurationStore() : this(Path.Combine(DefaultApplicationDataDirectory, "SFRecordCompareEngine.Config.json"))
    { }

    public ApplicationConfigurationStore(string configurationPath)
    {
        ConfigurationPath = configurationPath;
        Load();
    }

    public static string DefaultApplicationDataDirectory { get; } = GetDefaultApplicationDataDirectory();

    public static string DefaultDatabaseDirectory { get; } = DefaultApplicationDataDirectory;

    public static string DefaultLoggingDirectory { get; } = Path.Combine(DefaultApplicationDataDirectory, "Logs");

    public string ConfigurationPath { get; }

    public ApplicationConfiguration Current { get; private set; } = new();

    private static string GetDefaultApplicationDataDirectory()
    {
        var applicationDataRoot = OperatingSystem.IsLinux()
            ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            : Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        var applicationDirectoryName = OperatingSystem.IsLinux()
            ? ".SFRecordCompareEngine"
            : "SFRecordCompareEngine";

        return Path.Combine(applicationDataRoot, applicationDirectoryName);
    }

    public void Load()
    {
        if (!File.Exists(ConfigurationPath))
        {
            Current = new ApplicationConfiguration();
            return;
        }

        var json = File.ReadAllText(ConfigurationPath);
        if (string.IsNullOrWhiteSpace(json))
        {
            Current = new ApplicationConfiguration();
            Current.ApplicationDataDirectory = DefaultApplicationDataDirectory;
            return;
        }

        try
        {
            Current = JsonSerializer.Deserialize<ApplicationConfiguration>(json, SerializerOptions) ?? new ApplicationConfiguration();
        }
        catch (JsonException)
        {
            Current = new ApplicationConfiguration();
        }
    }

    public void Save(ApplicationConfiguration configuration)
    {
        var directory = Path.GetDirectoryName(ConfigurationPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(ConfigurationPath, JsonSerializer.Serialize(configuration, SerializerOptions));
        Current = configuration;
    }
}
