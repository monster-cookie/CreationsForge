using Mutagen.Bethesda;
using Mutagen.Bethesda.Installs;

namespace CreationsForge.Engine.Installs;

/// <summary>Finds the Data directory of an installed supported game.</summary>
public static class InstalledGameData
{
    /// <summary>Returns the installed Data directory for a release name the MCP host accepts.</summary>
    /// <param name="releaseName">A release name such as <c>Starfield</c>, <c>Fallout4</c>, or <c>SkyrimSE</c>.</param>
    /// <returns>The Data directory when Mutagen can locate that install; otherwise <see langword="null"/>.</returns>
    public static string? TryGetDirectory(string releaseName)
    {
        if (!Enum.TryParse<GameRelease>(releaseName, ignoreCase: false, out var release))
        {
            return null;
        }

        try
        {
            if (!GameLocations.TryGetDataFolder(release, out var directory))
            {
                return null;
            }

            var path = directory.Path;
            return Directory.Exists(path) ? path : null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
