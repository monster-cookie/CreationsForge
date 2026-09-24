using System.Reflection;
using CreationsForge.Engine.Records;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Engine.Interfaces;

/// <summary>
/// Describes one game-specific Mutagen integration available to a CreationsForge host.
/// </summary>
public interface IGameIntegration
{
    /// <summary>Gets the concrete first-tranche record family registrations for this game.</summary>
    IReadOnlyList<RecordFamily> RecordFamilies { get; }

    /// <summary>Gets the exact Mutagen game release supported by this integration.</summary>
    GameRelease Release { get; }

    /// <summary>Gets the NuGet package identifier that supplies the game-specific Mutagen types.</summary>
    string MutagenPackageId { get; }

    /// <summary>Gets the loaded game-specific Mutagen assembly used by the integration.</summary>
    Assembly MutagenAssembly { get; }

    /// <summary>Validates that an output style is legal for this game and extension.</summary>
    /// <param name="modKey">The output identity, including its extension.</param>
    /// <param name="masterStyle">The requested plugin master style.</param>
    void ValidateOutputStyle(ModKey modKey, MasterStyle masterStyle);

    /// <summary>Gets required masters for a newly created output.</summary>
    /// <returns>Required masters in declared order.</returns>
    IReadOnlyList<ModKey> GetNewOutputMasters();

    /// <summary>Opens an immutable Mutagen plugin source with already discovered masters.</summary>
    /// <param name="path">The exact plugin path and expected identity.</param>
    /// <param name="knownMasters">Previously opened Mutagen master plugins in masters-first order.</param>
    /// <param name="targetLanguage">The active translated-string language to load.</param>
    /// <returns>An owned immutable Mutagen plugin source.</returns>
    IModDisposeGetter OpenSource(
        ModPath path,
        IReadOnlyList<IModMasterStyledGetter> knownMasters,
        Language targetLanguage);

    /// <summary>Opens a complete mutable Mutagen plugin output.</summary>
    /// <param name="path">The exact output path and expected identity.</param>
    /// <param name="knownMasters">Previously opened Mutagen master plugins in masters-first order.</param>
    /// <param name="targetLanguage">The active translated-string language to load.</param>
    /// <returns>The complete mutable output.</returns>
    IMod OpenExistingOutput(
        ModPath path,
        IReadOnlyList<IModMasterStyledGetter> knownMasters,
        Language targetLanguage);

    /// <summary>Creates a complete mutable clone of an output through the game's generated Mutagen translation.</summary>
    /// <param name="output">The output to clone.</param>
    /// <returns>An independently owned mutable output.</returns>
    IMod CloneOutput(IModGetter output);

    /// <summary>Creates the game's typed mutable link cache over immutable sources and one mutable output.</summary>
    /// <param name="sources">Immutable source plugins in low-to-high priority order.</param>
    /// <param name="output">The mutable Mutagen plugin output.</param>
    /// <returns>A typed Mutagen link cache exposed through the shared interface.</returns>
    ILinkCache CreateLinkCache(IReadOnlyList<IModGetter> sources, IMod output);

    /// <summary>Resolves a record context from an exact containing plugin.</summary>
    /// <param name="cache">The integration's typed link cache.</param>
    /// <param name="formKey">The record origin identity.</param>
    /// <param name="recordType">The Mutagen record getter type.</param>
    /// <param name="containingModKey">The exact plugin containing the requested record version.</param>
    /// <returns>The Mutagen record context with its parent chain.</returns>
    IModContext ResolveContextFromMod(ILinkCache cache, FormKey formKey, Type recordType, ModKey containingModKey);

    /// <summary>Resolves the winning record context for an origin identity.</summary>
    /// <param name="cache">The integration's typed link cache.</param>
    /// <param name="formKey">The record origin identity.</param>
    /// <param name="recordType">The Mutagen record getter type.</param>
    /// <returns>The winning Mutagen record context with its parent chain.</returns>
    IModContext ResolveWinningContext(ILinkCache cache, FormKey formKey, Type recordType);

    /// <summary>Enumerates winning Mutagen contexts from only the workspace's admitted source graph and output.</summary>
    /// <param name="cache">The integration's typed link cache.</param>
    /// <param name="recordType">The Mutagen record getter type to browse.</param>
    /// <returns>Winning Mutagen contexts with parent chains.</returns>
    IEnumerable<IModContext> EnumerateWinningContexts(ILinkCache cache, Type recordType);
}
