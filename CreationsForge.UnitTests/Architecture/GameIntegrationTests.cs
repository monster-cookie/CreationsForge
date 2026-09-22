using System.Reflection;
using CreationsForge.Engine.Interfaces;
using CreationsForge.Engine.Workspaces;
using CreationsForge.Fallout4;
using CreationsForge.Mcp;
using CreationsForge.Skyrim;
using CreationsForge.Starfield;
using Microsoft.Extensions.DependencyInjection;
using Mutagen.Bethesda;

namespace CreationsForge.UnitTests.Architecture;

/// <summary>
/// Verifies that every approved game assembly loads through the shared engine boundary.
/// </summary>
public sealed class GameIntegrationTests
{
    /// <summary>The exact Mutagen package version approved for the rebuild.</summary>
    private const string ApprovedMutagenVersion = "0.55.0-alpha.54";

    /// <summary>Loads the Starfield, Fallout 4, and Skyrim assemblies at the approved Mutagen package version.</summary>
    [Fact]
    public void ApprovedMutagenGameAssembliesLoadTogether()
    {
        IGameIntegration[] integrations =
        [
            new StarfieldGameIntegration(),
            new Fallout4GameIntegration(),
            new SkyrimGameIntegration(),
        ];

        Assert.Equal(
            [GameRelease.Starfield, GameRelease.Fallout4, GameRelease.SkyrimSE],
            integrations.Select(integration => integration.Release));
        foreach (var integration in integrations)
        {
            Assert.Equal(integration.MutagenPackageId, integration.MutagenAssembly.GetName().Name);
            AssertApprovedPackageVersion(integration.MutagenAssembly);
        }

        // Mutagen.Bethesda.Core exposes its shared runtime types from Mutagen.Bethesda.Kernel.
        Assert.Equal("Mutagen.Bethesda.Kernel", typeof(GameRelease).Assembly.GetName().Name);
        AssertApprovedPackageVersion(typeof(GameRelease).Assembly);
    }

    /// <summary>Requires MCP host composition to expose the shared plugin workspace factory backed by all supported games.</summary>
    [Fact]
    public void McpHostRegistersPluginWorkspaceFactory()
    {
        var builder = McpHostComposition.CreateProductionBuilder("test");
        using var host = builder.Build();

        var workspaceFactory = host.Services.GetRequiredService<PluginWorkspaceFactory>();
        var integrations = host.Services.GetServices<IGameIntegration>().ToArray();

        Assert.NotNull(workspaceFactory);
        Assert.Equal(3, integrations.Length);
    }

    /// <summary>Requires a loaded Mutagen assembly to report the exact approved prerelease version.</summary>
    /// <param name="assembly">The loaded Mutagen assembly to inspect.</param>
    private static void AssertApprovedPackageVersion(Assembly assembly)
    {
        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;
        Assert.False(string.IsNullOrWhiteSpace(informationalVersion));
        Assert.True(
            informationalVersion.StartsWith(ApprovedMutagenVersion, StringComparison.Ordinal),
            $"Assembly '{assembly.GetName().Name}' reported '{informationalVersion}' instead of the approved '{ApprovedMutagenVersion}' package version.");
    }
}
