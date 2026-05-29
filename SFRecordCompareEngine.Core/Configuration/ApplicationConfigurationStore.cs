using System.IO;
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

    public static string DefaultApplicationDataDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SFRecordCompareEngine");

    public ApplicationConfigurationStore() 
        : this(Path.Combine(DefaultApplicationDataDirectory, "SFRecordCompareEngine.config.json"))
    {
    }

    public ApplicationConfigurationStore(string configurationPath)
    {
        ConfigurationPath = configurationPath;
        Load();
    }

    public string ConfigurationPath { get; }

    public ApplicationConfiguration Current { get; private set; } = new();

    public bool IsConfigurationRequired => string.IsNullOrWhiteSpace(Current.SelectedGame);

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
