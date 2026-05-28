using SFRecordCompareEngine.Core.Services;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class StarfieldPluginReaderServiceTests : IDisposable
{
    private readonly string TestDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(TestDirectory))
        {
            Directory.Delete(TestDirectory, true);
        }
    }

    [Fact]
    public void GetSourceInfo_WhenFileDoesNotExist_ReturnsMissingSourceInfo()
    {
        var sut = new StarfieldPluginReaderService();

        var result = sut.GetSourceInfo(Path.Combine(TestDirectory, "Missing.esm"));

        result.Exists.ShouldBeFalse();
        result.LastWriteUTCTicks.ShouldBe(0);
        result.FileSizeBytes.ShouldBe(0);
    }

    [Fact]
    public void GetSourceInfo_WhenFileExists_ReturnsSourceInfo()
    {
        Directory.CreateDirectory(TestDirectory);
        var pluginPath = Path.Combine(TestDirectory, "Example.esm");
        File.WriteAllText(pluginPath, "example");
        var lastWriteUTC = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        File.SetLastWriteTimeUtc(pluginPath, lastWriteUTC);
        var sut = new StarfieldPluginReaderService();

        var result = sut.GetSourceInfo(pluginPath);

        result.Exists.ShouldBeTrue();
        result.LastWriteUTCTicks.ShouldBe(File.GetLastWriteTimeUtc(pluginPath).Ticks);
        result.FileSizeBytes.ShouldBe(new FileInfo(pluginPath).Length);
    }

    [Fact]
    public void GetLoadOrder_ReturnsStarfieldLoadOrder()
    {
        var sut = new StarfieldPluginReaderService();

        var result = sut.GetLoadOrder();

        result.ShouldNotBeEmpty();
        result.ShouldContain(entry => string.Equals(entry.PluginFileName, "Starfield.esm", StringComparison.OrdinalIgnoreCase));
        result.ShouldAllBe(entry =>
            !string.IsNullOrWhiteSpace(entry.ModKey.FileName) &&
            !string.IsNullOrWhiteSpace(entry.PluginFileName) &&
            !string.IsNullOrWhiteSpace(entry.PluginPath) &&
            entry.LoadOrderIndex >= 0);
    }

    [Fact]
    public void GetMetadata_WhenStarfieldEsmExists_ReturnsMetadata()
    {
        var sut = new StarfieldPluginReaderService();
        var starfieldEntry = GetStarfieldEsmEntry(sut);

        var result = sut.GetMetadata(starfieldEntry.PluginPath);

        result.ModKey.FileName.String.ShouldBe("Starfield.esm");
        result.FormVersion.ShouldBeGreaterThan(0);
        result.Author.ShouldNotBeNullOrWhiteSpace();
        result.MasterReferences.ShouldNotBeNull();
    }

    private static Core.DTOs.Plugins.PluginLoadOrderEntryDTO GetStarfieldEsmEntry(StarfieldPluginReaderService sut)
    {
        return sut.GetLoadOrder().Single(entry => string.Equals(entry.PluginFileName, "Starfield.esm", StringComparison.OrdinalIgnoreCase));
    }
}
