using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.ViewModels;

/// <summary>Presents one load-order plugin above its supported major-record groups.</summary>
public sealed class PluginGroupViewModel : ViewModelBase, IRecordTreeNodeViewModel
{
    /// <summary>Tracks whether the plugin group is expanded.</summary>
    private bool IsExpandedValue;

    /// <summary>Initializes one plugin group.</summary>
    /// <param name="plugin">The exact participating plugin.</param>
    /// <param name="children">The supported major-record groups contained by the plugin.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> or <paramref name="children"/> is <see langword="null"/>.</exception>
    public PluginGroupViewModel(
        PluginSummary plugin,
        IReadOnlyList<IRecordTreeNodeViewModel> children)
    {
        ArgumentNullException.ThrowIfNull(plugin);
        ArgumentNullException.ThrowIfNull(children);
        Plugin = plugin;
        Children = Array.AsReadOnly(children.ToArray());
    }

    /// <summary>Gets the exact participating plugin.</summary>
    public PluginSummary Plugin { get; }

    /// <inheritdoc />
    public string PrimaryText => Plugin.ModKey.FileName;

    /// <inheritdoc />
    public string EditorIdText => string.Empty;

    /// <inheritdoc />
    public string ContextText => $"[{Plugin.LoadOrderIndex}] {Plugin.Role}";

    /// <inheritdoc />
    public string OverrideCountText => string.Empty;

    /// <summary>Gets the supported major-record groups contained by this plugin.</summary>
    public IReadOnlyList<IRecordTreeNodeViewModel> Children { get; }

    /// <inheritdoc />
    public IReadOnlyList<IRecordTreeNodeViewModel> TreeChildren => Children;

    /// <inheritdoc />
    public bool HasChildren => Children.Count > 0;

    /// <inheritdoc />
    public bool IsExpanded
    {
        get => IsExpandedValue;
        set => SetProperty(ref IsExpandedValue, value);
    }
}
