using System.Text.Json;
using System.Text.Json.Serialization;
using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.Models.Configuration;
using Serilog;

namespace CreationsForge.Core.Configuration;

public class ApplicationConfigurationStore : IApplicationConfigurationStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public ApplicationConfigurationStore() : this(Path.Combine(DefaultApplicationDataDirectory, "CreationsForge.Config.json"))
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
        var logger = Log.ForContext<ApplicationConfigurationStore>();
        string applicationDataPath;
        if (OperatingSystem.IsWindows())
        {
            applicationDataPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "CreationsForge");
            logger.Information("Windows detected so resolving application data directory; using system application data directory: {ApplicationDataPath}", applicationDataPath);
        }
        else if (OperatingSystem.IsLinux())
        {
            applicationDataPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".CreationsForge");
            logger.Information("Linux detected so resolving application data directory; using user profile directory: {ApplicationDataPath}", applicationDataPath);
        }
        else if (OperatingSystem.IsMacOS())
        {
            applicationDataPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".CreationsForge");
            logger.Information("MacOS detected so resolving application data directory; using user profile directory: {ApplicationDataPath}", applicationDataPath);
        }
        else
        {
            // Dot folder in home directories should be safe everywhere but Windows
            applicationDataPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".CreationsForge");
            logger.Warning("Unexpected operating system reached while resolving application data directory; using user profile directory: {ApplicationDataPath}", applicationDataPath);
        }

        return applicationDataPath;
    }

    public void Load()
    {
        var logger = Log.ForContext<ApplicationConfigurationStore>();
        if (!File.Exists(ConfigurationPath))
        {
            logger.Debug("No configuration file found at {ConfigurationPath} so loading default configuration.", ConfigurationPath);
            Current = new ApplicationConfiguration();
            return;
        }

        var json = File.ReadAllText(ConfigurationPath);
        if (string.IsNullOrWhiteSpace(json))
        {
            logger.Warning("Configuration file found at {ConfigurationPath} but it was null or empty so loading default configuration.", ConfigurationPath);
            Current = new ApplicationConfiguration();
            return;
        }

        try
        {
            Current = JsonSerializer.Deserialize<ApplicationConfiguration>(json, SerializerOptions) ?? new ApplicationConfiguration();
        }
        catch (JsonException)
        {
            logger.Error("Configuration file found at {ConfigurationPath} contained invalid json so loading default configuration.", ConfigurationPath);
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
