using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Services.Interfaces;

/// <summary>
/// Acquires explicit local workspace paths through Avalonia storage pickers without opening game records.
/// </summary>
public interface IWorkspacePathPicker
{
    /// <summary>Prompts for one read-only source plugin.</summary>
    /// <param name="cancellationToken">A token checked before and after the platform picker.</param>
    /// <returns>The selected local path, or <see langword="null"/> when canceled.</returns>
    Task<string?> PickSourcePluginAsync(CancellationToken cancellationToken = default);

    /// <summary>Prompts for explicit plugin paths whose returned order becomes the editable load order.</summary>
    /// <param name="cancellationToken">A token checked before and after the platform picker.</param>
    /// <returns>The selected local paths, or an empty collection when canceled.</returns>
    Task<IReadOnlyList<string>> PickLoadOrderPluginsAsync(CancellationToken cancellationToken = default);

    /// <summary>Prompts for the plugin data directory.</summary>
    /// <param name="cancellationToken">A token checked before and after the platform picker.</param>
    /// <returns>The selected local path, or <see langword="null"/> when canceled.</returns>
    Task<string?> PickDataDirectoryAsync(CancellationToken cancellationToken = default);

    /// <summary>Prompts for zero or more explicit localized-string directories.</summary>
    /// <param name="cancellationToken">A token checked before and after the platform picker.</param>
    /// <returns>The selected local paths, or an empty collection when canceled.</returns>
    Task<IReadOnlyList<string>> PickStringDirectoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>Prompts for an absent or existing output plugin according to the requested selection mode.</summary>
    /// <param name="mode">Whether the output must be newly named or already exist.</param>
    /// <param name="cancellationToken">A token checked before and after the platform picker.</param>
    /// <returns>The selected local path, or <see langword="null"/> when canceled.</returns>
    Task<string?> PickOutputPluginAsync(OutputSelectionMode mode, CancellationToken cancellationToken = default);
}
