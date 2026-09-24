using CreationsForge.Engine.Interfaces;
using CreationsForge.Engine.Persistence;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Engine.Workspaces;

/// <summary>Opens plugin workspaces from explicit selections without consulting an enabled-plugin list.</summary>
public sealed class PluginWorkspaceFactory
{
    private const string OutputLockSuffix = ".creationsforge.lock";
    private readonly IReadOnlyDictionary<Mutagen.Bethesda.GameRelease, IGameIntegration> _integrations;
    private readonly IPluginPersistenceBackend _persistenceBackend;

    /// <summary>Initializes a workspace factory with one integration per supported release.</summary>
    /// <param name="integrations">The admitted game integrations.</param>
    public PluginWorkspaceFactory(IEnumerable<IGameIntegration> integrations)
        : this(integrations, new PluginPersistenceBackend())
    {
    }

    /// <summary>Initializes a workspace factory with an injectable persistence boundary.</summary>
    internal PluginWorkspaceFactory(
        IEnumerable<IGameIntegration> integrations,
        IPluginPersistenceBackend persistenceBackend)
    {
        ArgumentNullException.ThrowIfNull(integrations);
        ArgumentNullException.ThrowIfNull(persistenceBackend);
        try
        {
            _integrations = integrations.ToDictionary(integration => integration.Release);
        }
        catch (ArgumentException exception)
        {
            throw new ArgumentException("Only one game integration may be registered for each release.", nameof(integrations), exception);
        }

        _persistenceBackend = persistenceBackend;
    }

    /// <summary>Opens one complete plugin workspace and acquires ownership until it is disposed.</summary>
    /// <param name="request">The explicit game, source selection, and output definition.</param>
    /// <returns>The opened plugin workspace.</returns>
    public PluginWorkspace Open(PluginWorkspaceOpenRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!_integrations.TryGetValue(request.Release, out var integration))
        {
            throw new PluginWorkspaceException($"No Mutagen game integration is registered for '{request.Release}'.");
        }

        var dataDirectory = ValidateDataDirectory(request.DataDirectory);
        var outputPath = ValidateOutputDefinition(request.Output, integration);
        var outputInspection = request.Output.CreateNew
            ? null
            : InspectPlugin(outputPath, request.Output.ModKey, request.Release, "existing output");
        if (outputInspection is not null)
        {
            ValidateExistingOutputConfiguration(outputInspection, request.Output);
        }

        var rootModKeys = request.SelectedPlugins.ToList();
        rootModKeys.AddRange(outputInspection?.Masters ?? integration.GetNewOutputMasters());
        var sourceClosure = DiscoverSourceClosure(
            dataDirectory,
            rootModKeys,
            request.Output.ModKey,
            integration);

        var lockRequests = sourceClosure
            .Select(plugin => new WorkspaceFileLockRequest(plugin.Path, $"source plugin '{plugin.ModKey}'", isShared: true, createIfMissing: false))
            .ToList();
        lockRequests.Add(new WorkspaceFileLockRequest(outputPath + OutputLockSuffix, $"output identity '{request.Output.ModKey}'", isShared: false, createIfMissing: true));
        var outputStampBeforeOpen = request.Output.CreateNew
            ? null
            : _persistenceBackend.CaptureStamp(request.Output.ModKey, outputPath);

        WorkspaceFileLockSet? fileLocks = null;
        var openedSources = new List<IModDisposeGetter>(sourceClosure.Count);
        IMod? output = null;
        IMod? savedBaseline = null;
        ILinkCache? linkCache = null;
        try
        {
            fileLocks = WorkspaceFileLockSet.Acquire(lockRequests);
            if (!request.Output.CreateNew)
            {
                // Probe direct-file exclusivity while opening, but retain only the sidecar identity lock.
                // A lifetime destination handle would prevent replace-style publication on Windows.
                using var outputProbe = WorkspaceFileLockSet.Acquire(
                [
                    new WorkspaceFileLockRequest(
                        outputPath,
                        $"output plugin '{request.Output.ModKey}'",
                        isShared: false,
                        createIfMissing: false),
                ]);
            }

            foreach (var plugin in sourceClosure)
            {
                var knownMasters = openedSources.Cast<IModMasterStyledGetter>().ToArray();
                var source = integration.OpenSource(
                    new ModPath(plugin.ModKey, plugin.Path),
                    knownMasters,
                    request.Output.TargetLanguage);
                ValidateStableInspection(source, plugin);
                openedSources.Add(source);
            }

            output = request.Output.CreateNew
                ? CreateNewOutput(request.Output, integration)
                : integration.OpenExistingOutput(
                    new ModPath(request.Output.ModKey, outputPath),
                    openedSources.Cast<IModMasterStyledGetter>().ToArray(),
                    request.Output.TargetLanguage);
            if (!request.Output.CreateNew)
            {
                if (!Equals(outputStampBeforeOpen, _persistenceBackend.CaptureStamp(request.Output.ModKey, outputPath)))
                {
                    throw new PluginWorkspaceException($"Output plugin '{request.Output.ModKey}' changed while the workspace was opening. Retry after writes have stopped.");
                }
            }

            ValidateOpenedOutput(output, request.Output, outputInspection);
            savedBaseline = integration.CloneOutput(output);
            linkCache = integration.CreateLinkCache(openedSources, output);

            var state = new PluginWorkspaceState(
                request.Release,
                request.Output.ModKey,
                outputPath,
                request.Output.MasterStyle,
                request.Output.TextStorageMode,
                request.Output.CreateNew,
                isDirty: request.Output.CreateNew,
                revision: 0);
            var workspace = new PluginWorkspace(
                integration,
                openedSources,
                output,
                savedBaseline,
                linkCache,
                fileLocks,
                state,
                dataDirectory,
                request.Output.TargetLanguage,
                outputStampBeforeOpen,
                _persistenceBackend);
            openedSources = [];
            output = null;
            savedBaseline = null;
            linkCache = null;
            fileLocks = null;
            return workspace;
        }
        catch (PluginWorkspaceException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new PluginWorkspaceException(
                $"Could not open the {request.Release} workspace for output '{request.Output.ModKey}': {exception.Message}",
                exception);
        }
        finally
        {
            TryDisposeAfterFailedOpen(linkCache as IDisposable);
            TryDisposeAfterFailedOpen(output as IDisposable);
            TryDisposeAfterFailedOpen(savedBaseline as IDisposable);

            for (var index = openedSources.Count - 1; index >= 0; index--)
            {
                TryDisposeAfterFailedOpen(openedSources[index]);
            }

            TryDisposeAfterFailedOpen(fileLocks);
        }
    }

    private static string ValidateDataDirectory(string path)
    {
        var fullPath = Path.GetFullPath(path);
        if (!Directory.Exists(fullPath))
        {
            throw new PluginWorkspaceException($"The data directory '{fullPath}' does not exist.");
        }

        return fullPath;
    }

    private static string ValidateOutputDefinition(PluginOutputDefinition output, IGameIntegration integration)
    {
        if (!Enum.IsDefined(output.MasterStyle))
        {
            throw new PluginWorkspaceException($"Output '{output.ModKey}' specifies unknown master style value '{output.MasterStyle}'.");
        }

        if (!Enum.IsDefined(output.TextStorageMode))
        {
            throw new PluginWorkspaceException($"Output '{output.ModKey}' specifies unknown text storage value '{output.TextStorageMode}'.");
        }

        var fullPath = Path.GetFullPath(output.Path);
        var parentDirectory = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrEmpty(parentDirectory) || !Directory.Exists(parentDirectory))
        {
            throw new PluginWorkspaceException($"The output directory for '{fullPath}' does not exist.");
        }

        if (!string.Equals(Path.GetFileName(fullPath), output.ModKey.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            throw new PluginWorkspaceException($"Output path '{fullPath}' does not match the explicit plugin identity '{output.ModKey}'.");
        }

        integration.ValidateOutputStyle(output.ModKey, output.MasterStyle);
        if (output.CreateNew && File.Exists(fullPath))
        {
            throw new PluginWorkspaceException($"Cannot create new output '{output.ModKey}' because '{fullPath}' already exists.");
        }

        if (!output.CreateNew && !File.Exists(fullPath))
        {
            throw new PluginWorkspaceException($"Cannot open existing output '{output.ModKey}' because '{fullPath}' does not exist.");
        }

        return fullPath;
    }

    private static IReadOnlyList<DiscoveredPlugin> DiscoverSourceClosure(
        string dataDirectory,
        IEnumerable<ModKey> rootModKeys,
        ModKey outputModKey,
        IGameIntegration integration)
    {
        var ordered = new List<DiscoveredPlugin>();
        var states = new Dictionary<ModKey, VisitState>();

        foreach (var rootModKey in rootModKeys)
        {
            Visit(rootModKey, []);
        }

        return ordered;

        void Visit(ModKey modKey, IReadOnlyList<ModKey> ancestry)
        {
            if (modKey == outputModKey)
            {
                throw new PluginWorkspaceException($"Output '{outputModKey}' cannot also be loaded as an immutable source.");
            }

            if (states.TryGetValue(modKey, out var existingState))
            {
                if (existingState == VisitState.Visiting)
                {
                    var cycle = string.Join(" -> ", ancestry.Append(modKey));
                    throw new PluginWorkspaceException($"Plugin master cycle detected: {cycle}.");
                }

                return;
            }

            states.Add(modKey, VisitState.Visiting);
            var path = ResolveDataPluginPath(dataDirectory, modKey);
            var inspection = InspectPlugin(path, modKey, integration.Release, "source plugin");
            var nextAncestry = ancestry.Append(modKey).ToArray();
            foreach (var master in inspection.Masters)
            {
                Visit(master, nextAncestry);
            }

            states[modKey] = VisitState.Visited;
            ordered.Add(inspection);
        }
    }

    private static string ResolveDataPluginPath(string dataDirectory, ModKey modKey)
    {
        var path = Path.GetFullPath(Path.Combine(dataDirectory, modKey.ToString()));
        var directoryPrefix = Path.TrimEndingDirectorySeparator(dataDirectory) + Path.DirectorySeparatorChar;
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        if (!path.StartsWith(directoryPrefix, comparison))
        {
            throw new PluginWorkspaceException($"Source plugin '{modKey}' resolves outside data directory '{dataDirectory}'.");
        }

        if (!File.Exists(path))
        {
            throw new PluginWorkspaceException($"Required source plugin '{modKey}' is missing from '{dataDirectory}'.");
        }

        return path;
    }

    private static DiscoveredPlugin InspectPlugin(
        string path,
        ModKey expectedModKey,
        Mutagen.Bethesda.GameRelease release,
        string purpose)
    {
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            var header = ModHeaderFrame.FromStream(stream, expectedModKey, release, readSafe: true);
            var masters = new List<ModKey>();
            foreach (var subrecord in header)
            {
                if (subrecord.RecordType != new RecordType("MAST"))
                {
                    continue;
                }

                var masterName = header.Meta.Encodings.NonTranslated
                    .GetString(subrecord.Content.Span)
                    .TrimEnd('\0');
                if (string.IsNullOrWhiteSpace(masterName))
                {
                    throw new PluginWorkspaceException($"The {purpose} '{expectedModKey}' contains an empty master reference.");
                }

                masters.Add(ModKey.FromNameAndExtension(masterName));
            }

            return new DiscoveredPlugin(
                path,
                expectedModKey,
                masters,
                header.MasterStyle);
        }
        catch (PluginWorkspaceException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new PluginWorkspaceException($"Could not read {purpose} '{expectedModKey}' at '{path}': {exception.Message}", exception);
        }
    }

    private static void ValidateStableInspection(IModGetter plugin, DiscoveredPlugin inspection)
    {
        var actualMasters = plugin.MasterReferences.Select(reference => reference.Master).ToArray();
        if (plugin.ModKey != inspection.ModKey
            || !actualMasters.SequenceEqual(inspection.Masters)
            || ((IModMasterStyledGetter)plugin).MasterStyle != inspection.MasterStyle)
        {
            throw new PluginWorkspaceException($"Source plugin '{inspection.ModKey}' changed while the workspace was opening. Retry after writes have stopped.");
        }
    }

    private static IMod CreateNewOutput(PluginOutputDefinition definition, IGameIntegration integration)
    {
        var output = ModFactory.Activator(definition.ModKey, integration.Release);
        if (definition.MasterStyle == MasterStyle.Small && !output.CanBeSmallMaster)
        {
            throw new PluginWorkspaceException($"{integration.Release} cannot create '{definition.ModKey}' with the Small master style.");
        }

        if (definition.MasterStyle == MasterStyle.Medium && !output.CanBeMediumMaster)
        {
            throw new PluginWorkspaceException($"{integration.Release} cannot create '{definition.ModKey}' with the Medium master style.");
        }

        if (definition.TextStorageMode == PluginTextStorageMode.Localized && !output.CanUseLocalization)
        {
            throw new PluginWorkspaceException($"{integration.Release} cannot create '{definition.ModKey}' with localized text storage.");
        }

        output.IsMaster = definition.ModKey.Type is ModType.Master or ModType.Light;
        if (definition.MasterStyle == MasterStyle.Small)
        {
            output.IsSmallMaster = true;
        }

        if (definition.MasterStyle == MasterStyle.Medium)
        {
            output.IsMediumMaster = true;
        }

        output.UsingLocalization = definition.TextStorageMode == PluginTextStorageMode.Localized;
        foreach (var master in integration.GetNewOutputMasters())
        {
            output.MasterReferences.Add(new MasterReference { Master = master });
        }

        return output;
    }

    private static void ValidateExistingOutputConfiguration(DiscoveredPlugin output, PluginOutputDefinition definition)
    {
        if (output.MasterStyle != definition.MasterStyle)
        {
            throw new PluginWorkspaceException($"Existing output '{definition.ModKey}' uses {output.MasterStyle} master style instead of requested {definition.MasterStyle}.");
        }
    }

    private static void ValidateOpenedOutput(
        IMod output,
        PluginOutputDefinition definition,
        DiscoveredPlugin? originalInspection)
    {
        if (output.ModKey != definition.ModKey)
        {
            throw new PluginWorkspaceException($"Opened output reports identity '{output.ModKey}' instead of '{definition.ModKey}'.");
        }

        var inspection = new DiscoveredPlugin(
            definition.Path,
            output.ModKey,
            output.MasterReferences.Select(reference => reference.Master).ToArray(),
            ((IModMasterStyledGetter)output).MasterStyle);
        if (originalInspection is not null && !inspection.Masters.SequenceEqual(originalInspection.Masters))
        {
            throw new PluginWorkspaceException($"Output plugin '{definition.ModKey}' changed while the workspace was opening. Retry after writes have stopped.");
        }

        ValidateExistingOutputConfiguration(inspection, definition);
        var actualTextStorage = output.UsingLocalization ? PluginTextStorageMode.Localized : PluginTextStorageMode.Embedded;
        if (actualTextStorage != definition.TextStorageMode)
        {
            throw new PluginWorkspaceException($"Existing output '{definition.ModKey}' uses {actualTextStorage} text storage instead of requested {definition.TextStorageMode}.");
        }
    }

    private static void TryDisposeAfterFailedOpen(IDisposable? resource)
    {
        try
        {
            resource?.Dispose();
        }
        catch
        {
            // Preserve the actionable open failure while still attempting every remaining cleanup step.
        }
    }

    private enum VisitState
    {
        Visiting,
        Visited,
    }

    private sealed class DiscoveredPlugin
    {
        public DiscoveredPlugin(
            string path,
            ModKey modKey,
            IReadOnlyList<ModKey> masters,
            MasterStyle masterStyle)
        {
            Path = path;
            ModKey = modKey;
            Masters = masters;
            MasterStyle = masterStyle;
        }

        public string Path { get; }

        public ModKey ModKey { get; }

        public IReadOnlyList<ModKey> Masters { get; }

        public MasterStyle MasterStyle { get; }
    }
}
