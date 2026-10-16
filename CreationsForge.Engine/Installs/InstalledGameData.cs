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
        if (!TryGetSupportedRelease(releaseName, out var release))
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

    /// <summary>Accepts only the release names the production MCP host advertises.</summary>
    /// <param name="releaseName">The candidate release name.</param>
    /// <param name="release">The matching Mutagen release when the name is supported.</param>
    /// <returns><see langword="true"/> for Starfield, Fallout 4, or Skyrim Special Edition.</returns>
    private static bool TryGetSupportedRelease(string releaseName, out GameRelease release)
    {
        switch (releaseName)
        {
            case nameof(GameRelease.Starfield):
                release = GameRelease.Starfield;
                return true;
            case nameof(GameRelease.Fallout4):
                release = GameRelease.Fallout4;
                return true;
            case nameof(GameRelease.SkyrimSE):
                release = GameRelease.SkyrimSE;
                return true;
            default:
                release = default;
                return false;
        }
    }
}
