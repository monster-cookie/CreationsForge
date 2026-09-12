using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

/// <summary>Presents one detached FormList comparison without retaining a native workspace or native record getter.</summary>
public sealed class NativeWorkspaceChangeItemViewModel
{
    /// <summary>Initializes one immutable detached FormList change item.</summary>
    /// <param name="comparison">The exact detached comparison returned by the engine.</param>
    /// <param name="beforeFields">The projected prior JSON hierarchy.</param>
    /// <param name="afterFields">The projected resulting JSON hierarchy.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required comparison or field collection is <see langword="null"/>.</exception>
    public NativeWorkspaceChangeItemViewModel(
        FormListComparison comparison,
        IReadOnlyList<NativeJsonFieldNodeViewModel> beforeFields,
        IReadOnlyList<NativeJsonFieldNodeViewModel> afterFields)
    {
        ArgumentNullException.ThrowIfNull(comparison);
        ArgumentNullException.ThrowIfNull(beforeFields);
        ArgumentNullException.ThrowIfNull(afterFields);
        FormKey = comparison.FormKey;
        BeforeContext = comparison.BeforeContext;
        AfterContext = comparison.AfterContext;
        BeforeFields = Array.AsReadOnly(beforeFields.ToArray());
        AfterFields = Array.AsReadOnly(afterFields.ToArray());
        SemanticChanges = Array.AsReadOnly(comparison.Changes.ToArray());
        Warnings = Array.AsReadOnly(comparison.Warnings.ToArray());
    }

    /// <summary>Gets the exact native identity of the changed FormList.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the exact prior native context and resolution result.</summary>
    public FormListContext BeforeContext { get; }

    /// <summary>Gets the exact resulting native context and resolution result.</summary>
    public FormListContext AfterContext { get; }

    /// <summary>Gets the complete projected prior JSON hierarchy.</summary>
    public IReadOnlyList<NativeJsonFieldNodeViewModel> BeforeFields { get; }

    /// <summary>Gets the complete projected resulting JSON hierarchy.</summary>
    public IReadOnlyList<NativeJsonFieldNodeViewModel> AfterFields { get; }

    /// <summary>Gets the engine-reported semantic change descriptors in their original order.</summary>
    public IReadOnlyList<SemanticChangeDescriptor> SemanticChanges { get; }

    /// <summary>Gets comparison-specific warnings in their original order.</summary>
    public IReadOnlyList<EngineWarning> Warnings { get; }
}
