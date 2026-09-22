using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace CreationsForge.Mcp;

/// <summary>
/// Runs the production CreationsForge MCP server over standard input and standard output.
/// </summary>
public static class McpHostRunner
{
    /// <summary>Runs the stdio MCP host until input closes, shutdown is requested, or cancellation occurs.</summary>
    /// <param name="arguments">Command-line arguments supplied to the MCP executable.</param>
    /// <param name="cancellationToken">A token that requests graceful host shutdown.</param>
    /// <returns>Zero after clean shutdown, two for unsupported arguments, or 130 after token cancellation.</returns>
    public static async Task<int> RunAsync(
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        if (arguments.Count != 0)
        {
            Console.Error.WriteLine("The CreationsForge MCP server does not accept command-line arguments.");
            return 2;
        }

        var builder = McpHostComposition.CreateProductionBuilder(GetServerVersion());
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

    /// <summary>Reads a stable non-empty version for MCP initialization metadata.</summary>
    /// <returns>The informational version, assembly version, or a deterministic fallback.</returns>
    private static string GetServerVersion()
    {
        var assembly = typeof(McpHostRunner).Assembly;
        return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? assembly.GetName().Version?.ToString()
            ?? "1.0.0";
    }
}
