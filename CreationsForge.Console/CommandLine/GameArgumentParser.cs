using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;

namespace CreationsForge.Console.CommandLine;

public class GameArgumentParser
{
    private readonly IApplicationConfigurationStore ConfigurationStore;

    public GameArgumentParser(IApplicationConfigurationStore configurationStore)
    {
        ConfigurationStore = configurationStore;
    }

    public GameArgumentParseResult Parse(string[] args)
    {
        if (HasResetAllArgument(args))
        {
            return GameArgumentParseResult.ResetAllSuccess();
        }

        var gameValue = GetGameArgument(args);
        var forceFullReimport = HasForceFullReimportArgument(args);
        if (string.IsNullOrWhiteSpace(gameValue))
        {
            gameValue = ConfigurationStore.Current.ActiveGame;
        }

        if (string.IsNullOrWhiteSpace(gameValue))
        {
            return GameArgumentParseResult.Failure("A game is required. Use --game Starfield, --game Fallout4, or --game Skyrim.");
        }

        if (!Enum.TryParse<SupportedGame>(gameValue, true, out var game))
        {
            return GameArgumentParseResult.Failure($"Unsupported game '{gameValue}'. Supported values are Starfield, Fallout4, and Skyrim.");
        }

        var configuration = new ApplicationConfiguration
        {
            ActiveGame = game.ToString(),
            ApplicationDataDirectory = ConfigurationStore.Current.ApplicationDataDirectory,
            DatabaseDirectory = ConfigurationStore.Current.DatabaseDirectory,
            LoggingDirectory = ConfigurationStore.Current.LoggingDirectory
        };
        ConfigurationStore.Save(configuration);

        return GameArgumentParseResult.Success(game, forceFullReimport);
    }

    private static string? GetGameArgument(string[] args)
    {
        for (var index = 0; index < args.Length; index++)
        {
            if (!IsGameSwitch(args[index]))
            {
                continue;
            }

            if (index + 1 >= args.Length)
            {
                return string.Empty;
            }

            return args[index + 1];
        }

        return null;
    }

    private static bool IsGameSwitch(string value)
    {
        return string.Equals(value, "--game", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "-g", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "-game", StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasForceFullReimportArgument(IEnumerable<string> args)
    {
        return args.Any(arg =>
            string.Equals(arg, "--force", StringComparison.OrdinalIgnoreCase)
            || string.Equals(arg, "--full", StringComparison.OrdinalIgnoreCase));
    }

    private static bool HasResetAllArgument(IEnumerable<string> args)
    {
        return args.Any(arg => string.Equals(arg, "--reset-all", StringComparison.OrdinalIgnoreCase));
    }
}
