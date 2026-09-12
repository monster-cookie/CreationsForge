using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Identifies the exact requested and selected native context for one FormList read.</summary>
public sealed class FormListContext
{
    /// <summary>Initializes immutable FormList read provenance.</summary>
    /// <param name="selection">The exact native identity, scope, and optional containing-plugin selection.</param>
    /// <param name="status">The outcome of selecting one native context.</param>
    /// <param name="containingModKey">The plugin containing the selected context, when singular.</param>
    /// <param name="path">The canonical path of the selected containing plugin, when singular.</param>
    /// <param name="loadOrderIndex">The explicit load-order position of the selected containing plugin, when singular.</param>
    /// <param name="role">The workspace role of the selected containing plugin, when singular.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="selection"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when containing-plugin provenance is incomplete or <paramref name="path"/> is not fully qualified.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="status"/> or <paramref name="role"/> is undefined, or <paramref name="loadOrderIndex"/> is negative.</exception>
    public FormListContext(
        ReferenceRequest selection,
        ReferenceResolutionStatus status,
        ModKey? containingModKey,
        string? path,
        int? loadOrderIndex,
        PluginRole? role)
    {
        ArgumentNullException.ThrowIfNull(selection);
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status));
        }

        if (loadOrderIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(loadOrderIndex));
        }

        if (role.HasValue && !Enum.IsDefined(role.Value))
        {
            throw new ArgumentOutOfRangeException(nameof(role));
        }

        var hasAnyProvenance = containingModKey.HasValue || path is not null || loadOrderIndex.HasValue || role.HasValue;
        var hasCompleteProvenance = containingModKey.HasValue && path is not null && loadOrderIndex.HasValue && role.HasValue;
        if (hasAnyProvenance != hasCompleteProvenance)
        {
            throw new ArgumentException("A FormList context requires complete containing-plugin provenance when any provenance is supplied.");
        }

        if (path is not null && !System.IO.Path.IsPathFullyQualified(path))
        {
            throw new ArgumentException("A FormList context path must be canonical and fully qualified.", nameof(path));
        }

        Selection = selection;
        Status = status;
        ContainingModKey = containingModKey;
        Path = path;
        LoadOrderIndex = loadOrderIndex;
        Role = role;
    }

    /// <summary>Gets the exact requested native identity, scope, and containing-plugin selection.</summary>
    public ReferenceRequest Selection { get; }

    /// <summary>Gets the native context-selection outcome.</summary>
    public ReferenceResolutionStatus Status { get; }

    /// <summary>Gets the plugin containing the selected context, or <see langword="null"/> when no singular context was selected.</summary>
    public ModKey? ContainingModKey { get; }

    /// <summary>Gets the canonical containing-plugin path, or <see langword="null"/> when no singular context was selected.</summary>
    public string? Path { get; }

    /// <summary>Gets the containing plugin's explicit load-order position, or <see langword="null"/> when no singular context was selected.</summary>
    public int? LoadOrderIndex { get; }

    /// <summary>Gets the containing plugin's workspace role, or <see langword="null"/> when no singular context was selected.</summary>
    public PluginRole? Role { get; }
}
