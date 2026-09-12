using System.Reflection;
using CreationsForge.Bootstrap.Composition;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace CreationsForge.Mcp;

/// <summary>
/// Runs the bounded CreationsForge MCP server on standard input and standard output.
/// </summary>
public static class McpHostRunner
{
    /// <summary>Runs the stdio MCP host until input closes, host shutdown is requested, or cancellation occurs.</summary>
    /// <param name="arguments">Arguments passed directly to the MCP executable.</param>
    /// <param name="cancellationToken">A token that requests graceful host shutdown.</param>
    /// <returns>Zero after clean shutdown, two for unsupported command arguments, or 130 for token cancellation.</returns>
    public static async Task<int> RunAsync(
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        if (arguments.Count != 0)
        {
            System.Console.Error.WriteLine("The CreationsForge MCP server does not accept command-line arguments.");
            return 2;
        }

        var serverVersion = GetServerVersion();
        using var logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console(standardErrorFromLevel: LogEventLevel.Verbose)
            .CreateLogger();
        await using var nativeServices = NativeEngineComposition.Create(logger);
        using var metadataStore = new McpMetadataStore();
        await using var workspaceRegistry = new McpWorkspaceRegistry();
        var builder = McpHostComposition.CreateProductionBuilder(
            workspaceRegistry,
            serverVersion,
            nativeServices.WorkspaceFactory,
            nativeServices.SaveCoordinator,
            nativeServices.FormListEditWireCodecs,
            nativeServices.FormListEditWireSchemaCatalogs,
            metadataStore);

        using var host = builder.Build();
        try
        {
            await host.RunAsync(cancellationToken).ConfigureAwait(false);
            return 0;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return 130;
        }
    }

    /// <summary>Reads the assembly informational version without adding machine-specific build state.</summary>
    /// <returns>The non-empty informational or assembly version for this executable.</returns>
    private static string GetServerVersion()
    {
        var assembly = typeof(McpHostRunner).Assembly;
        return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? assembly.GetName().Version?.ToString()
            ?? "1.0.0";
    }
}
