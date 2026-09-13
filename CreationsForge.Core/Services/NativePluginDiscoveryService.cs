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
    public async ValueTask<EngineResult<NativePluginCatalog>> DiscoverAsync(
        SupportedGame game,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(
            () => Discover(game, cancellationToken),
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Builds one detached installed-plugin catalog on a background worker.</summary>
    /// <param name="game">The supported game whose installed plugins are discovered.</param>
    /// <param name="cancellationToken">A token observed between filesystem and header reads.</param>
    /// <returns>The detached catalog or a typed discovery failure.</returns>
    private static EngineResult<NativePluginCatalog> Discover(
        SupportedGame game,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var release = GetRelease(game);
            if (!GameLocations.TryGetDataFolder(release, out var dataDirectory))
            {
                return EngineResult<NativePluginCatalog>.Failure(new EngineError(
                    EngineErrorCode.SourceOpenFailed,
                    $"Mutagen could not locate the installed {GetDisplayName(game)} data directory."));
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
            var authors = new Dictionary<ModKey, string?>();
            var descriptions = new Dictionary<ModKey, string?>();
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
                    var textMetadata = ReadHeaderTextMetadata(header);
                    authors[listing.ModKey] = textMetadata.Author;
                    descriptions[listing.ModKey] = textMetadata.Description;
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

                var hasValidMasterGraph = TryGetRequiredMastersInLoadOrder(
                    listing.ModKey,
                    masterKeys,
                    out var requiredMasters);
                var missingMasters = requiredMasters
                    .Where(master => !pluginPaths.ContainsKey(master))
                    .Select(master => master.FileName.String)
                    .ToArray();
                var dependencySnapshot = requiredMasters
                    .Select(master => pluginPaths.TryGetValue(master, out var path) ? path : null)
                    .Where(path => path is not null)
                    .Cast<string>()
                    .ToArray();
                unavailableReasons.TryGetValue(listing.ModKey, out var unavailableReason);
                if (implicitPluginKeys.Contains(listing.ModKey))
                {
                    unavailableReason = "Bethesda-supplied game plugins are read-only.";
                }
                else if (unavailableReason is null && !hasValidMasterGraph)
                {
                    unavailableReason = "The plugin's declared master graph contains a dependency cycle.";
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
                    unavailableReason,
                    masterKeys.GetValueOrDefault(listing.ModKey, Array.Empty<ModKey>()),
                    authors.GetValueOrDefault(listing.ModKey),
                    descriptions.GetValueOrDefault(listing.ModKey)));
            }

            return EngineResult<NativePluginCatalog>.Success(
                new NativePluginCatalog(game, release, dataDirectoryPath, entries));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return EngineResult<NativePluginCatalog>.Failure(new EngineError(
                EngineErrorCode.SourceOpenFailed,
                $"The installed plugin list could not be read: {exception.Message}"));
        }
    }

    /// <summary>Orders the transitive declared-master closure with every master before its dependent plugin.</summary>
    /// <param name="modKey">The plugin whose dependency closure is requested.</param>
    /// <param name="masterKeys">Declared master keys indexed by installed plugin identity.</param>
    /// <param name="requiredMasters">Receives every reachable master in valid native order, or an empty list for a cycle.</param>
    /// <returns><see langword="true"/> when the declared master graph is acyclic; otherwise <see langword="false"/>.</returns>
    internal static bool TryGetRequiredMastersInLoadOrder(
        ModKey modKey,
        IReadOnlyDictionary<ModKey, IReadOnlyList<ModKey>> masterKeys,
        out IReadOnlyList<ModKey> requiredMasters)
    {
        var ordered = new List<ModKey>();
        var visited = new HashSet<ModKey>();
        var visiting = new HashSet<ModKey>();

        bool Visit(ModKey current)
        {
            if (visited.Contains(current))
            {
                return true;
            }

            if (!visiting.Add(current))
            {
                return false;
            }

            if (masterKeys.TryGetValue(current, out var declaredMasters))
            {
                foreach (var master in declaredMasters)
                {
                    if (!Visit(master))
                    {
                        return false;
                    }
                }
            }

            visiting.Remove(current);
            visited.Add(current);
            if (current != modKey)
            {
                ordered.Add(current);
            }

            return true;
        }

        if (!Visit(modKey))
        {
            requiredMasters = Array.Empty<ModKey>();
            return false;
        }

        requiredMasters = Array.AsReadOnly(ordered.ToArray());
        return true;
    }

    /// <summary>Reads bounded author and description text directly from a native plugin header.</summary>
    /// <param name="header">The already loaded native plugin header.</param>
    /// <returns>The optional author and description fields.</returns>
    private static (string? Author, string? Description) ReadHeaderTextMetadata(ModHeaderFrame header)
    {
        string? author = null;
        string? description = null;
        var remaining = header.Content;
        while (remaining.Length > 0)
        {
            if (remaining.Length < header.Meta.SubConstants.HeaderLength)
            {
                throw new InvalidDataException("The plugin header ends with an incomplete subrecord header.");
            }

            var subrecord = new SubrecordFrame(header.Meta, remaining);
            if (subrecord.TotalLength <= 0 || subrecord.TotalLength > remaining.Length)
            {
                throw new InvalidDataException("The plugin header contains an invalid subrecord length.");
            }

            var value = subrecord.RecordType.ToString() switch
            {
                "CNAM" => NormalizeHeaderText(subrecord.AsString(header.Meta.Encodings.NonTranslated)),
                "SNAM" => NormalizeHeaderText(subrecord.AsString(header.Meta.Encodings.NonTranslated)),
                _ => null
            };
            if (subrecord.RecordType.ToString() == "CNAM")
            {
                author = value;
            }
            else if (subrecord.RecordType.ToString() == "SNAM")
            {
                description = value;
            }

            remaining = remaining.Slice(subrecord.TotalLength);
        }

        return (author, description);
    }

    /// <summary>Normalizes optional native header text for product display.</summary>
    /// <param name="value">The decoded native text.</param>
    /// <returns>Trimmed text, or <see langword="null"/> when no visible text exists.</returns>
    private static string? NormalizeHeaderText(string? value)
    {
        var normalized = value?.Trim().TrimEnd('\0').Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
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
