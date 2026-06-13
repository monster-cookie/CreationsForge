using Autofac;
using CreationsForge.Bootstrap.Composition;
using CreationsForge.Bootstrap.Logging;
using CreationsForge.Console.CommandLine;
using CreationsForge.Core.Configuration;
using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.Database.Interfaces;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Services.Interfaces;
using Serilog;

var bootstrapConfigurationStore = new ApplicationConfigurationStore();
SerilogConfigurator.Configure(bootstrapConfigurationStore, writeToConsole: true);

try
{
    using (var container = AutofacConfigurator.Configure(RegisterConsoleServices))
    {
        var parser = container.Resolve<GameArgumentParser>();
        var parseResult = parser.Parse(args);
        if (!parseResult.IsSuccess || (!parseResult.ResetAll && parseResult.Game is null))
        {
            System.Console.Error.WriteLine(parseResult.ErrorMessage);
            return 2;
        }

        var configurationStore = container.Resolve<IApplicationConfigurationStore>();
        SerilogConfigurator.Configure(configurationStore, writeToConsole: true);

        if (parseResult.ResetAll)
        {
            Log.Information("Starting CreationsForge Reset & Import All from CLI");
            var allGamesResult = await container.Resolve<IAllGamesImportWorkflowService>().ImportAllAsync(resetDatabase: true);
            Log.Information(
                "CreationsForge Reset & Import All completed; games imported: {GamesImported}; plugins imported: {PluginsImported}",
                allGamesResult.ImportResults.Count,
                allGamesResult.ImportResults.Sum(result => result.PluginsImported));
            return 0;
        }

        if (parseResult.Game is null)
        {
            System.Console.Error.WriteLine(parseResult.ErrorMessage ?? "A game is required.");
            return 2;
        }

        var game = parseResult.Game.Value;
        Log.Information("Starting CreationsForge import for {Game}; force full reimport: {ForceFullReimport}", game, parseResult.ForceFullReimport);
        var migrationsApplied = container.Resolve<IDatabaseSchemaInitializer>().Initialize();
        var result = container.Resolve<GameImportDispatcher>().Import(game, parseResult.ForceFullReimport || migrationsApplied);
        Log.Information("CreationsForge import completed for {Game}; plugins imported: {PluginsImported}", result.Game, result.PluginsImported);
        return 0;
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "CreationsForge import failed");
    System.Console.Error.WriteLine(ex.Message);
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

static void RegisterConsoleServices(ContainerBuilder builder)
{
    builder.RegisterType<GameArgumentParser>().SingleInstance();
}
