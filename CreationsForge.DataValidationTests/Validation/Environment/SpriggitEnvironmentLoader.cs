using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Environment;

public class SpriggitEnvironmentLoader
{
    private const string StarfieldVariableName = "SPRIGGIT_STARFIELD_EXTRACTIONS";
    private const string Fallout4VariableName = "SPRIGGIT_FALLOUT_EXTRACTIONS";
    private const string SkyrimVariableName = "SPRIGGIT_SKYRIM_EXTRACTIONS";

    public SpriggitEnvironmentConfiguration Load()
    {
        var repositoryRoot = FindRepositoryRoot();
        var dotEnvValues = LoadDotEnvValues(Path.Combine(repositoryRoot, ".env"));
        var configuration = new SpriggitEnvironmentConfiguration
        {
            StarfieldExtractionRoot = ResolvePath(StarfieldVariableName, dotEnvValues),
            Fallout4ExtractionRoot = ResolvePath(Fallout4VariableName, dotEnvValues),
            SkyrimExtractionRoot = ResolvePath(SkyrimVariableName, dotEnvValues)
        };

        ValidateDirectory(StarfieldVariableName, configuration.StarfieldExtractionRoot);
        ValidateDirectory(Fallout4VariableName, configuration.Fallout4ExtractionRoot);
        ValidateDirectory(SkyrimVariableName, configuration.SkyrimExtractionRoot);
        return configuration;
    }

    public string GetExtractionRoot(SupportedGame game, SpriggitEnvironmentConfiguration configuration)
    {
        return game switch
        {
            SupportedGame.Starfield => configuration.StarfieldExtractionRoot,
            SupportedGame.Fallout4 => configuration.Fallout4ExtractionRoot,
            SupportedGame.Skyrim => configuration.SkyrimExtractionRoot,
            _ => throw new InvalidOperationException($"Unsupported game '{game}'.")
        };
    }

    private static string ResolvePath(string variableName, IReadOnlyDictionary<string, string> dotEnvValues)
    {
        var environmentValue = System.Environment.GetEnvironmentVariable(variableName);
        if (!string.IsNullOrWhiteSpace(environmentValue))
        {
            return environmentValue;
        }

        if (dotEnvValues.TryGetValue(variableName, out var dotEnvValue) && !string.IsNullOrWhiteSpace(dotEnvValue))
        {
            return dotEnvValue;
        }

        throw new InvalidOperationException($"Missing required Spriggit extraction path '{variableName}'. Set it in the environment or repo-root .env.");
    }

    private static IReadOnlyDictionary<string, string> LoadDotEnvValues(string dotEnvPath)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!File.Exists(dotEnvPath))
        {
            return values;
        }

        foreach (var rawLine in File.ReadAllLines(dotEnvPath))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
            {
                continue;
            }

            var separatorIndex = line.IndexOf('=', StringComparison.Ordinal);
            if (separatorIndex <= 0)
            {
                continue;
            }

            var name = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim().Trim('"');
            values[name] = value;
        }

        return values;
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "CreationsForge.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate repository root from test output directory.");
    }

    private static void ValidateDirectory(string variableName, string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Configured Spriggit extraction path '{variableName}' does not exist: {path}");
        }
    }
}
