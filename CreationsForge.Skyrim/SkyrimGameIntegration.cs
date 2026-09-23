using System.Reflection;
using CreationsForge.Engine.Interfaces;
using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim;

/// <summary>
/// Supplies the production Skyrim Special Edition Mutagen workspace boundary.
/// </summary>
public sealed class SkyrimGameIntegration : IGameIntegration
{
    /// <inheritdoc />
    public IReadOnlyList<RecordFamily> RecordFamilies => SkyrimRecordFamilies.All;

    /// <inheritdoc />
    public GameRelease Release => GameRelease.SkyrimSE;

    /// <inheritdoc />
    public string MutagenPackageId => "Mutagen.Bethesda.Skyrim";

    /// <inheritdoc />
    public Assembly MutagenAssembly => typeof(SkyrimMod).Assembly;

    /// <inheritdoc />
    public void ValidateOutputStyle(ModKey modKey, MasterStyle masterStyle)
    {
        if (masterStyle == MasterStyle.Medium)
        {
            throw new PluginWorkspaceException("Skyrim Special Edition does not support the Medium master style.");
        }

        if (modKey.Type == ModType.Light && masterStyle != MasterStyle.Small)
        {
            throw new PluginWorkspaceException($"The .esl output '{modKey}' must use the Small master style.");
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<ModKey> GetNewOutputMasters()
    {
        return [];
    }

    /// <inheritdoc />
    public IModDisposeGetter OpenSource(ModPath path, IReadOnlyList<IModMasterStyledGetter> knownMasters)
    {
        return ModFactory.ImportGetter(path, Release, new BinaryReadParameters());
    }

    /// <inheritdoc />
    public IMod OpenExistingOutput(ModPath path, IReadOnlyList<IModMasterStyledGetter> knownMasters)
    {
        return ModFactory.ImportSetter(path, Release, new BinaryReadParameters());
    }

    /// <inheritdoc />
    public ILinkCache CreateLinkCache(IReadOnlyList<IModGetter> sources, IMod output)
    {
        return sources
            .Cast<ISkyrimModGetter>()
            .ToMutableLinkCache<ISkyrimMod, ISkyrimModGetter>((ISkyrimMod)output);
    }

    /// <inheritdoc />
    public IModContext ResolveContextFromMod(ILinkCache cache, FormKey formKey, Type recordType, ModKey containingModKey)
    {
        return ((ILinkCache<ISkyrimMod, ISkyrimModGetter>)cache)
            .ResolveContextFromMod(formKey, recordType, containingModKey);
    }

    /// <inheritdoc />
    public IModContext ResolveWinningContext(ILinkCache cache, FormKey formKey, Type recordType)
    {
        return ((ILinkCache<ISkyrimMod, ISkyrimModGetter>)cache)
            .ResolveContext(formKey, recordType, ResolveTarget.Winner);
    }

    /// <inheritdoc />
    public IEnumerable<IModContext> EnumerateWinningContexts(ILinkCache cache, Type recordType)
    {
        return cache.PriorityOrder
            .Cast<ISkyrimModGetter>()
            .WinningContextOverrides<ISkyrimMod, ISkyrimModGetter>(cache, recordType)
            .Cast<IModContext>();
    }
}
