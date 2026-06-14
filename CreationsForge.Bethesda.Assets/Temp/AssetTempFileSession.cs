namespace CreationsForge.Bethesda.Assets.Temp;

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
        if (!IsSafeSessionDirectory() || !Directory.Exists(RootDirectory))
        {
            return;
        }

        try
        {
            Directory.Delete(RootDirectory, recursive: true);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
        }
    }

    private bool IsSafeSessionDirectory()
    {
        var root = Path.GetFullPath(RootDirectory);
        var expectedParent = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "CreationsForge", "AssetPreview"));
        return root.StartsWith(expectedParent, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(root, expectedParent, StringComparison.OrdinalIgnoreCase);
    }
}
