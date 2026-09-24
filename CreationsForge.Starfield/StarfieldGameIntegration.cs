using System.Reflection;
using CreationsForge.Engine.Interfaces;
using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Starfield;

/// <summary>
/// Supplies the production Starfield Mutagen workspace boundary.
/// </summary>
public sealed class StarfieldGameIntegration : IGameIntegration
{
    /// <inheritdoc />
    public IReadOnlyList<RecordFamily> RecordFamilies => StarfieldRecordFamilies.All;

    /// <inheritdoc />
    public GameRelease Release => GameRelease.Starfield;

    /// <inheritdoc />
    public string MutagenPackageId => "Mutagen.Bethesda.Starfield";

    /// <inheritdoc />
    public Assembly MutagenAssembly => typeof(StarfieldMod).Assembly;

    /// <inheritdoc />
    public void ValidateOutputStyle(ModKey modKey, MasterStyle masterStyle)
    {
        if (modKey.Type == ModType.Light && masterStyle != MasterStyle.Small)
        {
            throw new PluginWorkspaceException($"The .esl output '{modKey}' must use the Small master style.");
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<ModKey> GetNewOutputMasters()
    {
        return [ModKey.FromNameAndExtension("Starfield.esm")];
    }

    /// <inheritdoc />
    public IModDisposeGetter OpenSource(
        ModPath path,
        IReadOnlyList<IModMasterStyledGetter> knownMasters,
        Language targetLanguage)
    {
        return StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(path.Path)
            .WithKnownMasters(knownMasters.ToArray())
            .WithTargetLanguage(targetLanguage)
            .Construct();
    }

    /// <inheritdoc />
    public IMod OpenExistingOutput(
        ModPath path,
        IReadOnlyList<IModMasterStyledGetter> knownMasters,
        Language targetLanguage)
    {
        return StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(path.Path)
            .WithKnownMasters(knownMasters.ToArray())
            .WithTargetLanguage(targetLanguage)
            .Mutable()
            .Construct();
    }

    /// <inheritdoc />
    public IMod CloneOutput(IModGetter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        return StarfieldModMixIn.DeepCopy((IStarfieldModGetter)output);
    }

    /// <inheritdoc />
    public ILinkCache CreateLinkCache(IReadOnlyList<IModGetter> sources, IMod output)
    {
        return sources
            .Cast<IStarfieldModGetter>()
            .ToMutableLinkCache<IStarfieldMod, IStarfieldModGetter>((IStarfieldMod)output);
    }

    /// <inheritdoc />
    public IModContext ResolveContextFromMod(ILinkCache cache, FormKey formKey, Type recordType, ModKey containingModKey)
    {
        return ((ILinkCache<IStarfieldMod, IStarfieldModGetter>)cache)
            .ResolveContextFromMod(formKey, recordType, containingModKey);
    }

    /// <inheritdoc />
    public IModContext ResolveWinningContext(ILinkCache cache, FormKey formKey, Type recordType)
    {
        return ((ILinkCache<IStarfieldMod, IStarfieldModGetter>)cache)
            .ResolveContext(formKey, recordType, ResolveTarget.Winner);
    }

    /// <inheritdoc />
    public IEnumerable<IModContext> EnumerateWinningContexts(ILinkCache cache, Type recordType)
    {
        return cache.PriorityOrder
            .Cast<IStarfieldModGetter>()
            .WinningContextOverrides<IStarfieldMod, IStarfieldModGetter>(cache, recordType)
            .Cast<IModContext>();
    }
}
