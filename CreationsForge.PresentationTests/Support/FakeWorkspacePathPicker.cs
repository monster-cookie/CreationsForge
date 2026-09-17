using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.PresentationTests.Support;

/// <summary>
/// Returns configured paths while recording which plugin path pickers were requested.
/// </summary>
internal sealed class FakeWorkspacePathPicker : IWorkspacePathPicker
{
    /// <summary>Gets or sets the source path returned by the picker.</summary>
    public string? SourcePluginPath { get; set; }

    /// <summary>Gets or sets the ordered load-order paths returned by the picker.</summary>
    public IReadOnlyList<string> LoadOrderPluginPaths { get; set; } = [];

    /// <summary>Gets or sets the data directory returned by the picker.</summary>
    public string? DataDirectoryPath { get; set; }

    /// <summary>Gets or sets the string directories returned by the picker.</summary>
    public IReadOnlyList<string> StringDirectoryPaths { get; set; } = [];

    /// <summary>Gets or sets the output path returned by the picker.</summary>
    public string? OutputPluginPath { get; set; }

    /// <summary>Gets the output mode supplied to the most recent output picker.</summary>
    public OutputSelectionMode? RequestedOutputMode { get; private set; }

    /// <summary>Gets the suggested output directory supplied to the most recent output picker.</summary>
    public string? RequestedOutputDirectory { get; private set; }

    /// <summary>Gets the preferred extension supplied to the most recent output picker.</summary>
    public string? RequestedOutputExtension { get; private set; }

    /// <inheritdoc />
    public Task<string?> PickSourcePluginAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SourcePluginPath);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<string>> PickLoadOrderPluginsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(LoadOrderPluginPaths);
    }

    /// <inheritdoc />
    public Task<string?> PickDataDirectoryAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(DataDirectoryPath);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<string>> PickStringDirectoriesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(StringDirectoryPaths);
    }

    /// <inheritdoc />
    public Task<string?> PickOutputPluginAsync(
        OutputSelectionMode mode,
        string? suggestedDirectoryPath = null,
        string? preferredExtension = null,
        CancellationToken cancellationToken = default)
    {
        RequestedOutputMode = mode;
        RequestedOutputDirectory = suggestedDirectoryPath;
        RequestedOutputExtension = preferredExtension;
        return Task.FromResult(OutputPluginPath);
    }
}
