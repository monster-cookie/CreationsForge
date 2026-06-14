using CreationsForge.Services;
using Shouldly;

namespace CreationsForge.PresentationTests.Services;

public class ExternalAssetPathPolicyTests
{
    [Fact]
    public void IsSafeExistingAssetPath_AllowsSupportedAssetExtension()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var assetPath = Path.Combine(tempDirectory.FullName, "Preview.nif");
            File.WriteAllBytes(assetPath, [1]);

            ExternalAssetPathPolicy.IsSafeExistingAssetPath(assetPath).ShouldBeTrue();
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void IsSafeExistingAssetPath_RejectsUnsupportedExtension()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var assetPath = Path.Combine(tempDirectory.FullName, "Preview.cmd");
            File.WriteAllBytes(assetPath, [1]);

            ExternalAssetPathPolicy.IsSafeExistingAssetPath(assetPath).ShouldBeFalse();
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void IsSafeExistingExecutablePath_AllowsExeOnly()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var executablePath = Path.Combine(tempDirectory.FullName, "NifSkope.exe");
            var scriptPath = Path.Combine(tempDirectory.FullName, "NifSkope.cmd");
            File.WriteAllBytes(executablePath, [1]);
            File.WriteAllBytes(scriptPath, [1]);

            ExternalAssetPathPolicy.IsSafeExistingExecutablePath(executablePath).ShouldBeTrue();
            ExternalAssetPathPolicy.IsSafeExistingExecutablePath(scriptPath).ShouldBeFalse();
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }
}
