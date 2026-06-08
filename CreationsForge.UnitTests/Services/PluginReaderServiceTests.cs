using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Fallout4;
using CreationsForge.Skyrim;
using CreationsForge.Starfield;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class PluginReaderServiceTests : IDisposable
{
    private readonly string TestDirectory = Path.Combine(Path.GetTempPath(), "CreationsForgeTests", Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(TestDirectory)) Directory.Delete(TestDirectory, true);
    }

    [Fact]
    public void StarfieldReadSourceInfo_WhenFileDoesNotExist_ReturnsMissingSourceInfo()
    {
        var sut = new TestStarfieldPluginReaderService(TestDirectory);

        var result = sut.ReadSourceInfo(CreateModKey("Missing.esm"));

        AssertMissingSourceInfo(result);
    }

    [Fact]
    public void Fallout4ReadSourceInfo_WhenFileDoesNotExist_ReturnsMissingSourceInfo()
    {
        var sut = new TestFallout4PluginReaderService(TestDirectory);

        var result = sut.ReadSourceInfo(CreateModKey("Missing.esm"));

        AssertMissingSourceInfo(result);
    }

    [Fact]
    public void SkyrimReadSourceInfo_WhenFileDoesNotExist_ReturnsMissingSourceInfo()
    {
        var sut = new TestSkyrimPluginReaderService(TestDirectory);

        var result = sut.ReadSourceInfo(CreateModKey("Missing.esm"));

        AssertMissingSourceInfo(result);
    }

    [Fact]
    public void StarfieldReadSourceInfo_WhenFileExists_ReturnsSourceInfo()
    {
        var modKey = CreatePluginFile("Example.esm");
        var sut = new TestStarfieldPluginReaderService(TestDirectory);

        var result = sut.ReadSourceInfo(modKey);

        AssertExistingSourceInfo(result, modKey);
    }

    [Fact]
    public void Fallout4ReadSourceInfo_WhenFileExists_ReturnsSourceInfo()
    {
        var modKey = CreatePluginFile("Example.esm");
        var sut = new TestFallout4PluginReaderService(TestDirectory);

        var result = sut.ReadSourceInfo(modKey);

        AssertExistingSourceInfo(result, modKey);
    }

    [Fact]
    public void SkyrimReadSourceInfo_WhenFileExists_ReturnsSourceInfo()
    {
        var modKey = CreatePluginFile("Example.esm");
        var sut = new TestSkyrimPluginReaderService(TestDirectory);

        var result = sut.ReadSourceInfo(modKey);

        AssertExistingSourceInfo(result, modKey);
    }

    [Theory]
    [InlineData("BlueprintShips-Starfield.esm", true)]
    [InlineData("blueprintships-sfbgs050.esm", true)]
    [InlineData("BlueprintShips-Starfield.esp", false)]
    [InlineData("Starfield.esm", false)]
    public void StarfieldIsUnsupported_PreservesBlueprintShipsEsmRule(string fileName, bool expected)
    {
        var sut = new TestStarfieldPluginReaderService(TestDirectory);
        var loadOrderEntry = new PluginLoadOrderEntryDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            LoadOrderIndex = 0,
            Enabled = true
        };

        var result = sut.IsUnsupported(loadOrderEntry);

        result.ShouldBe(expected);
    }

    private ModKeyDTO CreatePluginFile(string fileName)
    {
        Directory.CreateDirectory(TestDirectory);
        var pluginPath = Path.Combine(TestDirectory, fileName);
        File.WriteAllText(pluginPath, "example");
        File.SetLastWriteTimeUtc(pluginPath, new DateTime(2026, 6, 5, 20, 0, 0, DateTimeKind.Utc));
        return CreateModKey(fileName);
    }

    private static ModKeyDTO CreateModKey(string fileName)
    {
        return new ModKeyDTO
        {
            Name = Path.GetFileNameWithoutExtension(fileName),
            Type = 0,
            FileName = fileName
        };
    }

    private static void AssertMissingSourceInfo(PluginSourceInfoDTO result)
    {
        result.Exists.ShouldBeFalse();
        result.LastWriteUTCTicks.ShouldBe(0);
        result.FileSizeBytes.ShouldBe(0);
    }

    private void AssertExistingSourceInfo(PluginSourceInfoDTO result, ModKeyDTO modKey)
    {
        var pluginPath = Path.Combine(TestDirectory, modKey.FileName);
        result.Exists.ShouldBeTrue();
        result.LastWriteUTCTicks.ShouldBe(File.GetLastWriteTimeUtc(pluginPath).Ticks);
        result.FileSizeBytes.ShouldBe(new FileInfo(pluginPath).Length);
    }

    private sealed class TestStarfieldPluginReaderService : StarfieldPluginReaderService
    {
        private readonly string DataFolderPath;

        public TestStarfieldPluginReaderService(string dataFolderPath)
            : base(new StarfieldGameMetadataService())
        {
            DataFolderPath = dataFolderPath;
        }

        protected override string GetDataFolderPath()
        {
            return DataFolderPath;
        }
    }

    private sealed class TestFallout4PluginReaderService : Fallout4PluginReaderService
    {
        private readonly string DataFolderPath;

        public TestFallout4PluginReaderService(string dataFolderPath)
            : base(new Fallout4GameMetadataService())
        {
            DataFolderPath = dataFolderPath;
        }

        protected override string GetDataFolderPath()
        {
            return DataFolderPath;
        }
    }

    private sealed class TestSkyrimPluginReaderService : SkyrimPluginReaderService
    {
        private readonly string DataFolderPath;

        public TestSkyrimPluginReaderService(string dataFolderPath)
            : base(new SkyrimGameMetadataService())
        {
            DataFolderPath = dataFolderPath;
        }

        protected override string GetDataFolderPath()
        {
            return DataFolderPath;
        }
    }
}
