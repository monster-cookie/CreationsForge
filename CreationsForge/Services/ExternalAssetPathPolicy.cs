namespace CreationsForge.Services;

public static class ExternalAssetPathPolicy
{
    private static readonly HashSet<string> AllowedAssetExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".dds",
        ".mat",
        ".mesh",
        ".nif"
    };

    public static bool IsSafeExistingAssetPath(string path)
    {
        return IsSafeExistingFilePath(path) &&
            AllowedAssetExtensions.Contains(Path.GetExtension(path));
    }

    public static bool IsSafeExistingExecutablePath(string path)
    {
        return IsSafeExistingFilePath(path) &&
            string.Equals(Path.GetExtension(path), ".exe", StringComparison.OrdinalIgnoreCase);
    }

    public static string GetSafeFileName(string path)
    {
        var fileName = Path.GetFileName(path);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = "Preview.nif";
        }

        var safeFileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        return string.IsNullOrWhiteSpace(Path.GetExtension(safeFileName))
            ? safeFileName + ".nif"
            : safeFileName;
    }

    private static bool IsSafeExistingFilePath(string path)
    {
        return !string.IsNullOrWhiteSpace(path) &&
            Path.IsPathRooted(path) &&
            (!Uri.TryCreate(path, UriKind.Absolute, out var uri) || uri.IsFile) &&
            File.Exists(path);
    }
}
