using System.Text.Json;
using System.Text.Json.Serialization;
using CreationsForge.DataValidationTests.Validation.Models;

namespace CreationsForge.DataValidationTests.Validation.Configuration;

public class SpriggitValidationConfigurationLoader
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public SpriggitValidationManifest LoadManifest()
    {
        return LoadJson<SpriggitValidationManifest>("SpriggitValidationSamples.json");
    }

    public SpriggitApprovedDifferenceSet LoadApprovedDifferences()
    {
        return LoadJson<SpriggitApprovedDifferenceSet>("SpriggitApprovedDifferences.json");
    }

    private static T LoadJson<T>(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Configuration", fileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Unable to find validation configuration file '{path}'.", path);
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, SerializerOptions) ??
               throw new InvalidOperationException($"Validation configuration file '{path}' did not contain a valid {typeof(T).Name} payload.");
    }
}
