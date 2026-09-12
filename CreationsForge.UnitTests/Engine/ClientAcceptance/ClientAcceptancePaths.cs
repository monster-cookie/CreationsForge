namespace CreationsForge.UnitTests.Engine.ClientAcceptance;

/// <summary>Validates the explicit process-local retained root without consulting installed-game state.</summary>
internal static class ClientAcceptancePaths
{
    /// <summary>Gets and validates a fresh export root, dynamically skipping only when the opt-in variable is absent.</summary>
    /// <returns>The canonical absent or empty export root beneath the repository's <c>.work</c> directory.</returns>
    /// <exception cref="InvalidDataException">Thrown when an explicit root is unsafe, malformed, or non-empty.</exception>
    internal static string GetExportRootOrSkip()
    {
        return PrepareExplicitExportRoot(GetConfiguredRootOrSkip());
    }

    /// <summary>Validates and creates one explicit fresh export root without following reparse-point ancestry.</summary>
    /// <param name="configuredRoot">The fully qualified process-local export root.</param>
    /// <returns>The canonical newly created or previously empty root.</returns>
    /// <exception cref="InvalidDataException">Thrown before creation when the root is unsafe, malformed, aliased, or non-empty.</exception>
    internal static string PrepareExplicitExportRoot(string configuredRoot)
    {
        var root = ValidateConfiguredRoot(configuredRoot);
        RejectReparseAncestry(root, "client-acceptance export root");
        if (Directory.Exists(root) && Directory.EnumerateFileSystemEntries(root).Any())
        {
            throw new InvalidDataException($"The retained client-acceptance export root must be absent or empty: '{root}'.");
        }

        if (File.Exists(root))
        {
            throw new InvalidDataException($"The retained client-acceptance export root is an existing file: '{root}'.");
        }

        Directory.CreateDirectory(root);
        RequireExistingDirectory(root, "client-acceptance export root");
        return root;
    }

    /// <summary>Gets and validates an existing verification root, dynamically skipping only when the opt-in variable is absent.</summary>
    /// <returns>The canonical retained root containing a manifest and authored outputs.</returns>
    /// <exception cref="InvalidDataException">Thrown when an explicit root or manifest is missing.</exception>
    internal static string GetVerificationRootOrSkip()
    {
        var root = ValidateConfiguredRoot(GetConfiguredRootOrSkip());
        RequireExistingDirectory(root, "client-acceptance verification root");

        var manifestPath = Path.Combine(root, ClientAcceptanceFixtureExportTests.ManifestFileName);
        RequireExistingRegularFile(manifestPath, "client-acceptance manifest");

        return root;
    }

    /// <summary>Requires an existing regular file whose complete existing ancestry contains no reparse point.</summary>
    /// <param name="path">The fully qualified file path.</param>
    /// <param name="description">The path role used in diagnostics.</param>
    /// <exception cref="InvalidDataException">Thrown when the path is absent, aliased, or not a regular file.</exception>
    internal static void RequireExistingRegularFile(string path, string description)
    {
        RejectReparseAncestry(path, description);
        if (!File.Exists(path))
        {
            throw new InvalidDataException($"{description} does not identify an existing regular file: '{path}'.");
        }

        var attributes = File.GetAttributes(path);
        if ((attributes & (FileAttributes.Directory | FileAttributes.ReparsePoint)) != 0)
        {
            throw new InvalidDataException($"{description} does not identify a non-reparse regular file: '{path}'.");
        }
    }

    /// <summary>Requires an existing physical directory whose complete existing ancestry contains no reparse point.</summary>
    /// <param name="path">The fully qualified directory path.</param>
    /// <param name="description">The path role used in diagnostics.</param>
    /// <exception cref="InvalidDataException">Thrown when the path is absent, aliased, or not a directory.</exception>
    internal static void RequireExistingDirectory(string path, string description)
    {
        RejectReparseAncestry(path, description);
        if (!Directory.Exists(path))
        {
            throw new InvalidDataException($"{description} does not identify an existing directory: '{path}'.");
        }

        var attributes = File.GetAttributes(path);
        if ((attributes & FileAttributes.Directory) == 0 || (attributes & FileAttributes.ReparsePoint) != 0)
        {
            throw new InvalidDataException($"{description} does not identify a non-reparse directory: '{path}'.");
        }
    }

    /// <summary>Rejects an existing reparse point at any segment of an input or output path.</summary>
    /// <param name="path">The fully qualified path whose existing ancestry is inspected.</param>
    /// <param name="description">The path role used in diagnostics.</param>
    /// <exception cref="InvalidDataException">Thrown when an existing segment is a reparse point.</exception>
    internal static void RejectReparseAncestry(string path, string description)
    {
        var cursor = Path.GetFullPath(path);
        while (!string.IsNullOrEmpty(cursor))
        {
            if ((File.Exists(cursor) || Directory.Exists(cursor))
                && (File.GetAttributes(cursor) & FileAttributes.ReparsePoint) != 0)
            {
                throw new InvalidDataException($"{description} ancestry contains reparse point '{cursor}'.");
            }

            var parent = Path.GetDirectoryName(cursor);
            if (string.Equals(parent, cursor, PathComparison))
            {
                break;
            }

            cursor = parent ?? string.Empty;
        }
    }

    /// <summary>Finds the repository root from the current directory or built test location.</summary>
    /// <returns>The canonical directory containing <c>CreationsForge.sln</c>.</returns>
    /// <exception cref="InvalidDataException">Thrown when the repository root cannot be located.</exception>
    internal static string FindRepositoryRoot()
    {
        foreach (var startingPath in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            var current = new DirectoryInfo(Path.GetFullPath(startingPath));
            while (current is not null)
            {
                if (File.Exists(Path.Combine(current.FullName, "CreationsForge.sln")))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }
        }

        throw new InvalidDataException("The CreationsForge repository root could not be located from the current process.");
    }

    /// <summary>Reads and bounds the explicit environment-selected retained root.</summary>
    /// <returns>The canonical root beneath repository <c>.work</c>.</returns>
    /// <exception cref="InvalidDataException">Thrown when a supplied value is not an absolute strict descendant of <c>.work</c>.</exception>
    private static string GetConfiguredRootOrSkip()
    {
        var configured = Environment.GetEnvironmentVariable(ClientAcceptanceFixtureExportTests.RootEnvironmentVariable);
        if (string.IsNullOrWhiteSpace(configured))
        {
            Assert.Skip($"Set {ClientAcceptanceFixtureExportTests.RootEnvironmentVariable} to an absolute process-local root beneath repository .work to opt in.");
        }

        return configured;
    }

    /// <summary>Canonicalizes and lexically bounds one explicit export or verification root beneath repository <c>.work</c>.</summary>
    /// <param name="configured">The non-empty process-local root value.</param>
    /// <returns>The canonical strict descendant of repository <c>.work</c>.</returns>
    /// <exception cref="InvalidDataException">Thrown when the supplied value is relative, malformed, or outside <c>.work</c>.</exception>
    private static string ValidateConfiguredRoot(string configured)
    {
        if (string.IsNullOrWhiteSpace(configured) || !Path.IsPathFullyQualified(configured))
        {
            throw new InvalidDataException($"{ClientAcceptanceFixtureExportTests.RootEnvironmentVariable} must be an absolute path.");
        }

        string root;
        try
        {
            root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(configured));
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            throw new InvalidDataException($"{ClientAcceptanceFixtureExportTests.RootEnvironmentVariable} is invalid: {exception.Message}", exception);
        }

        var workRoot = Path.TrimEndingDirectorySeparator(Path.GetFullPath(Path.Combine(FindRepositoryRoot(), ".work")));
        var relative = Path.GetRelativePath(workRoot, root);
        if (string.Equals(relative, ".", StringComparison.Ordinal)
            || Path.IsPathRooted(relative)
            || string.Equals(relative, "..", StringComparison.Ordinal)
            || relative.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
        {
            throw new InvalidDataException($"{ClientAcceptanceFixtureExportTests.RootEnvironmentVariable} must be a strict descendant of '{workRoot}'.");
        }

        return root;
    }

    /// <summary>Gets the platform-appropriate path comparison.</summary>
    private static StringComparison PathComparison => OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
}
