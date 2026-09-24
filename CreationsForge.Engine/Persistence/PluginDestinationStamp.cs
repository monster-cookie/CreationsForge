namespace CreationsForge.Engine.Persistence;

/// <summary>Captures the complete associated file-set identity used to detect destination replacement.</summary>
internal sealed class PluginDestinationStamp : IEquatable<PluginDestinationStamp>
{
    private readonly IReadOnlyList<PluginFileStamp> _files;

    /// <summary>Initializes a normalized file-set identity.</summary>
    public PluginDestinationStamp(IEnumerable<PluginFileStamp> files)
    {
        ArgumentNullException.ThrowIfNull(files);
        _files = files.OrderBy(file => file.RelativePath, PathComparer).ToArray();
    }

    /// <inheritdoc />
    public bool Equals(PluginDestinationStamp? other)
    {
        return other is not null
            && _files.Count == other._files.Count
            && _files.Zip(other._files).All(pair =>
                PathComparer.Equals(pair.First.RelativePath, pair.Second.RelativePath)
                && pair.First.Length == pair.Second.Length
                && pair.First.LastWriteTimeUtc == pair.Second.LastWriteTimeUtc);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is PluginDestinationStamp other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var file in _files)
        {
            hash.Add(file.RelativePath, PathComparer);
            hash.Add(file.Length);
            hash.Add(file.LastWriteTimeUtc);
        }

        return hash.ToHashCode();
    }

    private static StringComparer PathComparer => OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;
}
