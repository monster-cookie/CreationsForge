using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Engine.Workspaces;

/// <summary>Preserves both an exact containing-plugin context and the winning Mutagen context for a record.</summary>
public sealed class PluginRecordResolution
{
    internal PluginRecordResolution(IModContext exactContext, IModContext winningContext)
    {
        ExactContext = exactContext;
        WinningContext = winningContext;
        if (exactContext.Record is not IMajorRecordGetter exactRecord)
        {
            throw new PluginWorkspaceException("The resolved Mutagen context did not contain a major record.");
        }

        OriginFormKey = exactRecord.FormKey;
        ContainingModKey = exactContext.ModKey;
        WinningModKey = winningContext.ModKey;
    }

    /// <summary>Gets the record's origin identity. Its mod key may differ from the containing plugin.</summary>
    public FormKey OriginFormKey { get; }

    /// <summary>Gets the plugin containing the exact requested record version.</summary>
    public ModKey ContainingModKey { get; }

    /// <summary>Gets the plugin containing the winning record version.</summary>
    public ModKey WinningModKey { get; }

    /// <summary>Gets the exact Mutagen context, including its unflattened parent chain.</summary>
    public IModContext ExactContext { get; }

    /// <summary>Gets the winning Mutagen context, including its unflattened parent chain.</summary>
    public IModContext WinningContext { get; }
}
