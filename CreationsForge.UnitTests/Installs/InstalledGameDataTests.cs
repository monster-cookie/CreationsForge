using CreationsForge.Engine.Installs;

namespace CreationsForge.UnitTests.Installs;

/// <summary>Verifies game Data lookup rejects names the MCP host does not accept.</summary>
public sealed class InstalledGameDataTests
{
    /// <summary>Returns no directory for a release name outside the supported set.</summary>
    [Fact]
    public void UnknownReleaseReturnsNull()
    {
        Assert.Null(InstalledGameData.TryGetDirectory("NotAGame"));
        Assert.Null(InstalledGameData.TryGetDirectory("Oblivion"));
        Assert.Null(InstalledGameData.TryGetDirectory("0"));
        Assert.Null(InstalledGameData.TryGetDirectory(string.Empty));
    }

    /// <summary>Requires the same release spelling the MCP host accepts.</summary>
    [Fact]
    public void ReleaseNamesAreCaseSensitive()
    {
        Assert.Null(InstalledGameData.TryGetDirectory("starfield"));
    }
}
