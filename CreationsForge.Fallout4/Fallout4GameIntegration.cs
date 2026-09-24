using System.Reflection;
using CreationsForge.Engine.Interfaces;
using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Fallout4;

/// <summary>
/// Supplies the production Fallout 4 Mutagen workspace boundary.
/// </summary>
public sealed class Fallout4GameIntegration : IGameIntegration
{
    /// <inheritdoc />
    public IReadOnlyList<RecordFamily> RecordFamilies => Fallout4RecordFamilies.All;

    /// <inheritdoc />
    public GameRelease Release => GameRelease.Fallout4;

    /// <inheritdoc />
    public string MutagenPackageId => "Mutagen.Bethesda.Fallout4";

    /// <inheritdoc />
    public Assembly MutagenAssembly => typeof(Fallout4Mod).Assembly;

    /// <inheritdoc />
    public void ValidateOutputStyle(ModKey modKey, MasterStyle masterStyle)
    {
        if (masterStyle == MasterStyle.Medium)
        {
            throw new PluginWorkspaceException("Fallout 4 does not support the Medium master style.");
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
    public IModDisposeGetter OpenSource(
        ModPath path,
        IReadOnlyList<IModMasterStyledGetter> knownMasters,
        Language targetLanguage)
    {
        return Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(path)
            .WithTargetLanguage(targetLanguage)
            .Construct();
    }

    /// <inheritdoc />
    public IMod OpenExistingOutput(
        ModPath path,
        IReadOnlyList<IModMasterStyledGetter> knownMasters,
        Language targetLanguage)
    {
        return Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(path)
            .WithTargetLanguage(targetLanguage)
            .Mutable()
            .Construct();
    }

    /// <inheritdoc />
    public IMod CloneOutput(IModGetter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        return Fallout4ModMixIn.DeepCopy((IFallout4ModGetter)output);
    }

    /// <inheritdoc />
    public ILinkCache CreateLinkCache(IReadOnlyList<IModGetter> sources, IMod output)
    {
        return sources
            .Cast<IFallout4ModGetter>()
            .ToMutableLinkCache<IFallout4Mod, IFallout4ModGetter>((IFallout4Mod)output);
    }

    /// <inheritdoc />
    public IModContext ResolveContextFromMod(ILinkCache cache, FormKey formKey, Type recordType, ModKey containingModKey)
    {
        return ((ILinkCache<IFallout4Mod, IFallout4ModGetter>)cache)
            .ResolveContextFromMod(formKey, recordType, containingModKey);
    }

    /// <inheritdoc />
    public IModContext ResolveWinningContext(ILinkCache cache, FormKey formKey, Type recordType)
    {
        return ((ILinkCache<IFallout4Mod, IFallout4ModGetter>)cache)
            .ResolveContext(formKey, recordType, ResolveTarget.Winner);
    }

    /// <inheritdoc />
    public IEnumerable<IModContext> EnumerateWinningContexts(ILinkCache cache, Type recordType)
    {
        return cache.PriorityOrder
            .Cast<IFallout4ModGetter>()
            .WinningContextOverrides<IFallout4Mod, IFallout4ModGetter>(cache, recordType)
            .Cast<IModContext>();
    }
}
