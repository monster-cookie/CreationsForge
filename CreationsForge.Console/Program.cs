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
using System.Diagnostics;
using System.Runtime.InteropServices;

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
        var terminationDiagnosticsService = container.Resolve<IProcessTerminationDiagnosticsService>();
        terminationDiagnosticsService.StartSession("CLI", SerilogConfigurator.CurrentLogPath);
        var sqliteConnectionFactory = container.Resolve<ISqliteConnectionFactory>();
        LogCliStartupDiagnostics(configurationStore, sqliteConnectionFactory);

        if (parseResult.ResetAll)
        {
            Log.Information("Starting CreationsForge Reset & Import All from CLI");
            var allGamesResult = await container.Resolve<IAllGamesImportWorkflowService>().ImportAllAsync(
                resetDatabase: true,
                cancellationToken: terminationDiagnosticsService.TerminationToken);
            Log.Information(
                "CreationsForge Reset & Import All completed; games imported: {GamesImported}; plugins imported: {PluginsImported}",
                allGamesResult.ImportResults.Count,
                allGamesResult.ImportResults.Sum(result => result.PluginsImported));
            Log.Information("Marking CLI reset-all session as clean shutdown");
            terminationDiagnosticsService.MarkCleanShutdown("CLI reset-all completed");
            Log.Information("CLI reset-all completed; returning success exit code");
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
        var result = container.Resolve<GameImportDispatcher>().Import(
            game,
            parseResult.ForceFullReimport || migrationsApplied,
            cancellationToken: terminationDiagnosticsService.TerminationToken);
        Log.Information("CreationsForge import completed for {Game}; plugins imported: {PluginsImported}", result.Game, result.PluginsImported);
        terminationDiagnosticsService.MarkCleanShutdown($"CLI {game} import completed");
        return 0;
    }
}
catch (OperationCanceledException)
{
    Log.Warning("CreationsForge import canceled by process termination request");
    return 130;
}
catch (Exception ex)
{
    Log.Fatal(ex, "CreationsForge import failed");
    System.Console.Error.WriteLine(ex.Message);
    return 1;
}
finally
{
    Log.Information("Closing and flushing CreationsForge CLI log");
    await Log.CloseAndFlushAsync();
}

static void RegisterConsoleServices(ContainerBuilder builder)
{
    builder.RegisterType<GameArgumentParser>().SingleInstance();
}

static void LogCliStartupDiagnostics(
    IApplicationConfigurationStore configurationStore,
    ISqliteConnectionFactory sqliteConnectionFactory)
{
    using var process = Process.GetCurrentProcess();
    Log.Information(
        "CreationsForge CLI diagnostics; process id: {ProcessId}; process name: {ProcessName}; command line: {CommandLine}; current directory: {CurrentDirectory}; runtime: {RuntimeDescription}; OS: {OSDescription}; 64-bit process: {Is64BitProcess}; output redirected: {IsOutputRedirected}; error redirected: {IsErrorRedirected}; input redirected: {IsInputRedirected}; log path: {LogPath}; database path: {DatabasePath}; application data directory: {ApplicationDataDirectory}; logging directory: {LoggingDirectory}; managed bytes: {ManagedBytes}; working set bytes: {WorkingSetBytes}; private bytes: {PrivateBytes}; handle count: {HandleCount}; thread count: {ThreadCount}",
        Environment.ProcessId,
        process.ProcessName,
        Environment.CommandLine,
        Environment.CurrentDirectory,
        RuntimeInformation.FrameworkDescription,
        RuntimeInformation.OSDescription,
        Environment.Is64BitProcess,
        System.Console.IsOutputRedirected,
        System.Console.IsErrorRedirected,
        System.Console.IsInputRedirected,
        SerilogConfigurator.CurrentLogPath,
        sqliteConnectionFactory.DatabasePath,
        configurationStore.Current.ApplicationDataDirectory,
        configurationStore.Current.LoggingDirectory,
        GC.GetTotalMemory(forceFullCollection: false),
        process.WorkingSet64,
        process.PrivateMemorySize64,
        process.HandleCount,
        process.Threads.Count);
}
