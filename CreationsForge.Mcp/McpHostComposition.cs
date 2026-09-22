using CreationsForge.Engine;
using CreationsForge.Fallout4;
using CreationsForge.Skyrim;
using CreationsForge.Starfield;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using Mutagen.Bethesda;

namespace CreationsForge.Mcp;

/// <summary>
/// Builds the production MCP process around the UI-neutral engine and game-integration boundaries.
/// </summary>
public static class McpHostComposition
{
    /// <summary>Creates the production stdio host with all supported Mutagen game packages loaded and validated.</summary>
    /// <param name="serverVersion">The non-empty version advertised during MCP initialization.</param>
    /// <returns>A host builder configured to reserve standard output for MCP protocol frames.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="serverVersion"/> is empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the expected game integration set is incomplete or inconsistent.</exception>
    public static HostApplicationBuilder CreateProductionBuilder(string serverVersion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serverVersion);

        var integrations = CreateGameIntegrations();
        ValidateGameIntegrations(integrations);

        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Args = [],
            ApplicationName = typeof(McpHostComposition).Assembly.GetName().Name,
        });
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);

        foreach (var integration in integrations)
        {
            builder.Services.AddSingleton<IGameIntegration>(integration);
        }

        builder.Services.AddSingleton(serviceProvider =>
            new NativeWorkspaceFactory(serviceProvider.GetServices<IGameIntegration>()));

        builder.Services
            .AddMcpServer(options =>
            {
                options.ServerInfo = new Implementation
                {
                    Name = "CreationsForge",
                    Version = serverVersion,
                };
                options.ServerInstructions = "CreationsForge provides its native Mutagen workspace backend. Authoring and save MCP tools are not available yet.";
            })
            .WithStdioServerTransport();

        return builder;
    }

    /// <summary>Creates the exact three game integrations admitted by the production host.</summary>
    /// <returns>Starfield, Fallout 4, and Skyrim Special Edition integrations in stable order.</returns>
    private static IReadOnlyList<IGameIntegration> CreateGameIntegrations()
    {
        return
        [
            new StarfieldGameIntegration(),
            new Fallout4GameIntegration(),
            new SkyrimGameIntegration(),
        ];
    }

    /// <summary>Requires one loadable Mutagen assembly for every supported game release.</summary>
    /// <param name="integrations">The integrations selected for production composition.</param>
    /// <exception cref="InvalidOperationException">Thrown when a release is missing, duplicated, or backed by an unexpected assembly.</exception>
    private static void ValidateGameIntegrations(IReadOnlyList<IGameIntegration> integrations)
    {
        var expectedReleases = new HashSet<GameRelease>
        {
            GameRelease.Starfield,
            GameRelease.Fallout4,
            GameRelease.SkyrimSE,
        };
        var actualReleases = integrations.Select(integration => integration.Release).ToHashSet();
        if (!actualReleases.SetEquals(expectedReleases) || integrations.Count != expectedReleases.Count)
        {
            throw new InvalidOperationException("The production MCP host requires exactly one integration for Starfield, Fallout 4, and Skyrim Special Edition.");
        }

        foreach (var integration in integrations)
        {
            var assemblyName = integration.MutagenAssembly.GetName().Name;
            if (!string.Equals(assemblyName, integration.MutagenPackageId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"The {integration.Release} integration loaded '{assemblyName}' instead of '{integration.MutagenPackageId}'.");
            }
        }
    }
}
