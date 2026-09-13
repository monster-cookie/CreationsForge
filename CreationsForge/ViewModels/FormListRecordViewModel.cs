using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

/// <summary>
/// Presents one FormList in navigation while retaining its exact ordered contexts for comparison selectors.
/// </summary>
public sealed class FormListRecordViewModel : ViewModelBase, IRecordTreeNodeViewModel
{
    /// <summary>Tracks the interface expansion value; FormList records expose no navigation children.</summary>
    private bool IsExpandedValue;

    /// <summary>Initializes one FormList tree node.</summary>
    /// <param name="formKey">The exact FormList identity.</param>
    /// <param name="containingModKey">The plugin containing this exact context, or the winning context for a root.</param>
    /// <param name="editorId">The EditorID observed for this root or context, or <see langword="null"/>.</param>
    /// <param name="overrideCount">The plugin override count reported for the FormList.</param>
    /// <param name="context">The exact winning or containing-plugin selection.</param>
    /// <param name="contexts">The exact ordered context records retained for selection and editing.</param>
    /// <param name="contextOptions">All valid comparison selections for the root, beginning with the winning selection.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="containingModKey"/> is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/>, <paramref name="contexts"/>, or <paramref name="contextOptions"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="overrideCount"/> is negative.</exception>
    public FormListRecordViewModel(
        FormKey formKey,
        ModKey containingModKey,
        string? editorId,
        int overrideCount,
        FormListContextOption context,
        IReadOnlyList<FormListRecordViewModel> contexts,
        IReadOnlyList<FormListContextOption> contextOptions)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(overrideCount);
        if (containingModKey == ModKey.Null)
        {
            throw new ArgumentException("A FormList presentation record requires a containing plugin.", nameof(containingModKey));
        }

        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(contexts);
        ArgumentNullException.ThrowIfNull(contextOptions);
        FormKey = formKey;
        ContainingModKey = containingModKey;
        EditorId = editorId;
        OverrideCount = overrideCount;
        Context = context;
        Contexts = Array.AsReadOnly(contexts.ToArray());
        ContextOptions = Array.AsReadOnly(contextOptions.ToArray());
    }

    /// <summary>Gets the exact FormList identity.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the plugin containing this exact context, or the winning context for a root.</summary>
    public ModKey ContainingModKey { get; }

    /// <summary>Gets the exact FormList identity as display text.</summary>
    public string FormKeyText => FormKey.ToString();

    /// <inheritdoc />
    public string PrimaryText => FormIdText;

    /// <summary>Gets the plugin FormID as an eight-digit hexadecimal value for display and filtering.</summary>
    public string FormIdText => FormKey.ID.ToString("X8");

    /// <summary>Gets the EditorID observed for this root or context, or <see langword="null"/>.</summary>
    public string? EditorId { get; }

    /// <summary>Gets the observed EditorID or an explicit absent-value label.</summary>
    public string EditorIdText => EditorId ?? "(no EditorID)";

    /// <summary>Gets the plugin override count reported for the FormList.</summary>
    public int OverrideCount { get; }

    /// <inheritdoc />
    public string OverrideCountText => OverrideCount.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Gets the exact winning or containing-plugin selection represented by this node.</summary>
    public FormListContextOption Context { get; }

    /// <summary>Gets the exact context records in engine enumeration order for comparison and editing.</summary>
    public IReadOnlyList<FormListRecordViewModel> Contexts { get; }

    /// <inheritdoc />
    public IReadOnlyList<IRecordTreeNodeViewModel> TreeChildren => Array.Empty<IRecordTreeNodeViewModel>();

    /// <summary>Gets the winning selector followed by all exact context selectors for this root.</summary>
    public IReadOnlyList<FormListContextOption> ContextOptions { get; }

    /// <summary>Gets whether the node is a winning-override root.</summary>
    public bool IsWinningOverride => Context.IsWinningOverride;

    /// <summary>Gets the winning or exact containing-plugin context label.</summary>
    public string ContextText => Context.Label;

    /// <summary>Gets whether the record exposes navigation children.</summary>
    public bool HasChildren => false;

    /// <summary>Gets or sets the interface expansion value; records remain navigation leaves.</summary>
    public bool IsExpanded
    {
        get => IsExpandedValue;
        set => SetProperty(ref IsExpandedValue, value);
    }
}
