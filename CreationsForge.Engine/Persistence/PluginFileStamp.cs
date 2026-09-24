namespace CreationsForge.Engine.Persistence;

/// <summary>Captures one associated output file's path, metadata, and content identity.</summary>
/// <param name="RelativePath">The path relative to the output plugin directory.</param>
/// <param name="Length">The observed file length in bytes.</param>
/// <param name="LastWriteTimeUtc">The observed last-write timestamp in UTC.</param>
/// <param name="ContentSha256">The uppercase SHA-256 digest of the observed file bytes.</param>
internal sealed record PluginFileStamp(
    string RelativePath,
    long Length,
    DateTime LastWriteTimeUtc,
    string ContentSha256);
