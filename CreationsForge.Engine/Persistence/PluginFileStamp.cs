namespace CreationsForge.Engine.Persistence;

/// <summary>Captures one associated file's bounded non-content identity.</summary>
internal sealed record PluginFileStamp(string RelativePath, long Length, DateTime LastWriteTimeUtc);
