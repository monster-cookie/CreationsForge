using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Engine;

/// <summary>Preserves both an exact containing-plugin context and the winning native context for a record.</summary>
public sealed class NativeRecordResolution
{
    internal NativeRecordResolution(IModContext exactContext, IModContext winningContext)
    {
        ExactContext = exactContext;
        WinningContext = winningContext;
        if (exactContext.Record is not IMajorRecordGetter exactRecord)
        {
            throw new NativeWorkspaceException("The resolved native context did not contain a major record.");
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

    /// <summary>Gets the exact native Mutagen context, including its unflattened parent chain.</summary>
    public IModContext ExactContext { get; }

    /// <summary>Gets the winning native Mutagen context, including its unflattened parent chain.</summary>
    public IModContext WinningContext { get; }
}
