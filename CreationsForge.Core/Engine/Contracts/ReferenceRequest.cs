using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Requests resolution of one native FormKey without persisting a custom record model.</summary>
public sealed class ReferenceRequest
{
    /// <summary>Initializes a native reference request.</summary>
    /// <param name="formKey">The native identity to resolve.</param>
    /// <param name="scope">The native record contexts to consider.</param>
    /// <param name="containingModKey">An optional containing plugin used to select one source or output context.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="scope"/> is undefined.</exception>
    /// <exception cref="ArgumentException">Thrown when a containing plugin is supplied for the winning-override view.</exception>
    public ReferenceRequest(FormKey formKey, RecordScope scope, ModKey? containingModKey = null)
    {
        if (!Enum.IsDefined(scope))
        {
            throw new ArgumentOutOfRangeException(nameof(scope));
        }

        if (scope == RecordScope.WinningOverrides && containingModKey.HasValue)
        {
            throw new ArgumentException(
                "A containing plugin cannot be combined with the winning-override view.",
                nameof(containingModKey));
        }

        FormKey = formKey;
        Scope = scope;
        ContainingModKey = containingModKey;
    }

    /// <summary>Gets the native identity to resolve.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the native record contexts to consider.</summary>
    public RecordScope Scope { get; }

    /// <summary>Gets the containing plugin filter, or <see langword="null"/> when the scope selects it.</summary>
    public ModKey? ContainingModKey { get; }
}
