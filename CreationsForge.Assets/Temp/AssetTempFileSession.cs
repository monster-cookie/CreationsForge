namespace CreationsForge.Assets.Temp;

public class AssetTempFileSession : IAssetTempFileSession
{
    public AssetTempFileSession()
    {
        RootDirectory = Path.Combine(Path.GetTempPath(), "CreationsForge", "AssetPreview", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(RootDirectory);
    }

    public string RootDirectory { get; }

    public string CreateExtractionDirectory(string scopeName)
    {
        var safeScopeName = string.Join("_", scopeName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        var directory = Path.Combine(RootDirectory, string.IsNullOrWhiteSpace(safeScopeName) ? "Asset" : safeScopeName);
        Directory.CreateDirectory(directory);
        return directory;
    }

    public void Dispose()
    {
        if (Directory.Exists(RootDirectory))
        {
            Directory.Delete(RootDirectory, recursive: true);
        }
    }
}
