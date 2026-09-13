using System.IO.Abstractions;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Installs;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Order;

namespace CreationsForge.Core.Services;

/// <summary>Uses Mutagen's installed-game and load-order readers to build a detached plugin catalog.</summary>
public sealed class NativePluginDiscoveryService : INativePluginDiscoveryService
{
    /// <summary>The native TES4 header flag indicating separate localized string files.</summary>
    private const uint LocalizedHeaderFlag = 0x00000080;

    /// <inheritdoc />
    public ValueTask<EngineResult<NativePluginCatalog>> DiscoverAsync(
        SupportedGame game,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var release = GetRelease(game);
            if (!GameLocations.TryGetDataFolder(release, out var dataDirectory))
            {
                return ValueTask.FromResult(EngineResult<NativePluginCatalog>.Failure(new EngineError(
                    EngineErrorCode.SourceOpenFailed,
                    $"Mutagen could not locate the installed {GetDisplayName(game)} data directory.")));
            }

            var dataDirectoryPath = Path.GetFullPath(dataDirectory.ToString());
            var loadOrderListings = LoadOrder
                .GetLoadOrderListings(release, dataDirectory, throwOnMissingMods: false)
                .ToArray();
            var implicitPluginKeys = Implicits.Get(release).Listings.ToHashSet();
            var listings = new List<(ModKey ModKey, string FileName, bool Enabled)>();
            var observedModKeys = new HashSet<ModKey>();
            foreach (var listing in loadOrderListings)
            {
                if (observedModKeys.Add(listing.ModKey))
                {
                    listings.Add((listing.ModKey, listing.FileName, listing.Enabled));
                }
            }

            foreach (var pluginPath in Directory
                         .EnumerateFiles(dataDirectoryPath, "*", SearchOption.TopDirectoryOnly)
                         .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var extension = Path.GetExtension(pluginPath);
                if (!extension.Equals(".esm", StringComparison.OrdinalIgnoreCase)
                    && !extension.Equals(".esp", StringComparison.OrdinalIgnoreCase)
                    && !extension.Equals(".esl", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (ModKey.TryFromNameAndExtension(Path.GetFileName(pluginPath), out var modKey)
                    && observedModKeys.Add(modKey))
                {
                    listings.Add((modKey, Path.GetFileName(pluginPath), false));
                }
            }

            var fileSystem = new FileSystem();
            var pluginPaths = new Dictionary<ModKey, string>();
            var masterKeys = new Dictionary<ModKey, IReadOnlyList<ModKey>>();
            var styles = new Dictionary<ModKey, OutputMasterStyle>();
            var localizationModes = new Dictionary<ModKey, LocalizedOutputMode>();
            var unavailableReasons = new Dictionary<ModKey, string>();
            foreach (var listing in listings)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var pluginPath = Path.GetFullPath(Path.Combine(dataDirectoryPath, listing.FileName));
                if (!File.Exists(pluginPath))
                {
                    continue;
                }

                pluginPaths[listing.ModKey] = pluginPath;
                try
                {
                    var header = ModHeaderFrame.FromPath(
                        new ModPath(listing.ModKey, pluginPath),
                        release,
                        fileSystem);
                    styles[listing.ModKey] = header.MasterStyle switch
                    {
                        MasterStyle.Full => OutputMasterStyle.Full,
                        MasterStyle.Small => OutputMasterStyle.Small,
                        MasterStyle.Medium => OutputMasterStyle.Medium,
                        _ => throw new InvalidDataException($"Plugin '{listing.ModKey.FileName}' uses an unsupported native master style.")
                    };
                    localizationModes[listing.ModKey] = ((uint)header.Flags & LocalizedHeaderFlag) == 0
                        ? LocalizedOutputMode.Embedded
                        : LocalizedOutputMode.SeparateStringFiles;
                    masterKeys[listing.ModKey] = Array.AsReadOnly(MasterReferenceCollection
                        .FromModHeader(listing.ModKey, header)
                        .Masters
                        .Select(master => master.Master)
                        .ToArray());
                }
                catch (Exception exception)
                {
                    unavailableReasons[listing.ModKey] = $"The plugin header could not be read: {exception.Message}";
                }

                if ((File.GetAttributes(pluginPath) & FileAttributes.ReadOnly) != 0)
                {
                    unavailableReasons[listing.ModKey] = "The plugin file is read-only.";
                }
            }

            var entries = new List<NativePluginCatalogEntry>(pluginPaths.Count);
            for (var index = 0; index < listings.Count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var listing = listings[index];
                if (!pluginPaths.TryGetValue(listing.ModKey, out var pluginPath))
                {
                    continue;
                }

                var requiredMasters = GetRequiredMasters(listing.ModKey, masterKeys);
                var missingMasters = requiredMasters
                    .Where(master => !pluginPaths.ContainsKey(master))
                    .Select(master => master.FileName.String)
                    .ToArray();
                var dependencySnapshot = listings
                    .Where(candidate => requiredMasters.Contains(candidate.ModKey))
                    .Select(candidate => pluginPaths.TryGetValue(candidate.ModKey, out var path) ? path : null)
                    .Where(path => path is not null)
                    .Cast<string>()
                    .ToArray();
                unavailableReasons.TryGetValue(listing.ModKey, out var unavailableReason);
                if (implicitPluginKeys.Contains(listing.ModKey))
                {
                    unavailableReason = "Bethesda-supplied game plugins are read-only.";
                }
                else if (unavailableReason is null && missingMasters.Length > 0)
                {
                    unavailableReason = $"Required master files are unavailable: {string.Join(", ", missingMasters)}.";
                }
                else if (unavailableReason is null && dependencySnapshot.Length == 0)
                {
                    unavailableReason = "The plugin declares no masters and is treated as read-only.";
                }

                var canEdit = unavailableReason is null;
                entries.Add(new NativePluginCatalogEntry(
                    listing.ModKey,
                    pluginPath,
                    index,
                    listing.Enabled,
                    dependencySnapshot,
                    localizationModes.GetValueOrDefault(listing.ModKey, LocalizedOutputMode.Embedded),
                    styles.GetValueOrDefault(listing.ModKey, OutputMasterStyle.Full),
                    canEdit,
                    unavailableReason));
            }

            return ValueTask.FromResult(EngineResult<NativePluginCatalog>.Success(
                new NativePluginCatalog(game, release, dataDirectoryPath, entries)));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return ValueTask.FromResult(EngineResult<NativePluginCatalog>.Failure(new EngineError(
                EngineErrorCode.SourceOpenFailed,
                $"The installed plugin list could not be read: {exception.Message}")));
        }
    }

    /// <summary>Computes the transitive declared-master closure for one installed plugin.</summary>
    /// <param name="modKey">The plugin whose dependency closure is requested.</param>
    /// <param name="masterKeys">Declared master keys indexed by installed plugin identity.</param>
    /// <returns>Every reachable declared master identity.</returns>
    private static IReadOnlySet<ModKey> GetRequiredMasters(
        ModKey modKey,
        IReadOnlyDictionary<ModKey, IReadOnlyList<ModKey>> masterKeys)
    {
        var required = new HashSet<ModKey>();
        var pending = new Stack<ModKey>();
        if (masterKeys.TryGetValue(modKey, out var directMasters))
        {
            foreach (var master in directMasters)
            {
                pending.Push(master);
            }
        }

        while (pending.TryPop(out var current))
        {
            if (!required.Add(current) || !masterKeys.TryGetValue(current, out var parents))
            {
                continue;
            }

            foreach (var parent in parents)
            {
                pending.Push(parent);
            }
        }

        return required;
    }

    /// <summary>Maps a supported UI game to its exact native release.</summary>
    /// <param name="game">The supported UI game.</param>
    /// <returns>The exact Mutagen release.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="game"/> is undefined.</exception>
    private static GameRelease GetRelease(SupportedGame game)
    {
        return game switch
        {
            SupportedGame.Starfield => GameRelease.Starfield,
            SupportedGame.Fallout4 => GameRelease.Fallout4,
            SupportedGame.Skyrim => GameRelease.SkyrimSE,
            _ => throw new ArgumentOutOfRangeException(nameof(game), game, "The game is not supported.")
        };
    }

    /// <summary>Gets a readable game name for discovery failures.</summary>
    /// <param name="game">The supported UI game.</param>
    /// <returns>The display name.</returns>
    private static string GetDisplayName(SupportedGame game)
    {
        return game switch
        {
            SupportedGame.Starfield => "Starfield",
            SupportedGame.Fallout4 => "Fallout 4",
            SupportedGame.Skyrim => "Skyrim Special Edition",
            _ => game.ToString()
        };
    }
}
