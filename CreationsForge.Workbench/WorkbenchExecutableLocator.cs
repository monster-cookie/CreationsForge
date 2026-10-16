namespace CreationsForge.Workbench;

/// <summary>Finds the production MCP host next to an installed Workbench or in the development tree.</summary>
internal static class WorkbenchExecutableLocator
{
    /// <summary>Returns the MCP executable or <c>.dll</c> for the running Workbench, when one can be found.</summary>
    /// <returns>The host path, or <see langword="null"/> when neither the install layout nor the development tree has one.</returns>
    internal static string? Find() => Find(AppContext.BaseDirectory);

    /// <summary>Returns the MCP host relative to a Workbench output directory.</summary>
    /// <param name="baseDirectory">The directory that contains the running Workbench.</param>
    /// <returns>An installed <c>CreationsForge.Mcp.exe</c>, or the matching development <c>CreationsForge.Mcp.dll</c>.</returns>
    internal static string? Find(string baseDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDirectory);
        var fullBase = Path.GetFullPath(baseDirectory);
        return FindInstalledSibling(fullBase) ?? FindDevelopmentHost(fullBase);
    }

    /// <summary>Looks for the MCP host in the sibling <c>Mcp</c> folder used by the installer.</summary>
    /// <param name="baseDirectory">The installed Desktop directory.</param>
    /// <returns>The executable when it exists, otherwise the sibling <c>.dll</c>.</returns>
    private static string? FindInstalledSibling(string baseDirectory)
    {
        var mcpDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "..", "Mcp"));
        var executable = Path.Combine(mcpDirectory, "CreationsForge.Mcp.exe");
        if (File.Exists(executable))
        {
            return executable;
        }

        var library = Path.Combine(mcpDirectory, "CreationsForge.Mcp.dll");
        return File.Exists(library) ? library : null;
    }

    /// <summary>Walks parent directories for <c>CreationsForge.Mcp/bin/&lt;configuration&gt;/net10.0</c>.</summary>
    /// <param name="baseDirectory">The running Workbench output directory.</param>
    /// <returns>The development <c>.dll</c> for this configuration, or the other configuration when that is the one on disk.</returns>
    private static string? FindDevelopmentHost(string baseDirectory)
    {
        var debug = IsDebugBuild(baseDirectory);
        var configurations = debug ? new[] { "Debug", "Release" } : new[] { "Release", "Debug" };
        for (var current = new DirectoryInfo(baseDirectory); current is not null; current = current.Parent)
        {
            foreach (var configuration in configurations)
            {
                var candidate = Path.Combine(
                    current.FullName,
                    "CreationsForge.Mcp",
                    "bin",
                    configuration,
                    "net10.0",
                    "CreationsForge.Mcp.dll");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        return null;
    }

    /// <summary>Reports whether <paramref name="path"/> is a Debug build output.</summary>
    /// <param name="path">The running Workbench directory.</param>
    /// <returns><see langword="true"/> when the path contains a Debug directory segment.</returns>
    private static bool IsDebugBuild(string path)
    {
        return path.Contains($"{Path.DirectorySeparatorChar}Debug{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
            || path.Contains($"{Path.AltDirectorySeparatorChar}Debug{Path.AltDirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase);
    }
}
