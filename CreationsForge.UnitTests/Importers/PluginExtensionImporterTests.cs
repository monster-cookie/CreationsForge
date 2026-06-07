using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Fallout4.DTOs;
using CreationsForge.Fallout4.Importers;
using CreationsForge.Fallout4.Repositories.Interfaces;
using CreationsForge.Skyrim.DTOs;
using CreationsForge.Skyrim.Importers;
using CreationsForge.Skyrim.Repositories.Interfaces;
using CreationsForge.Starfield.DTOs;
using CreationsForge.Starfield.Importers;
using CreationsForge.Starfield.Repositories.Interfaces;
using Moq;
using Shouldly;

namespace CreationsForge.UnitTests.Importers;

public class PluginExtensionImporterTests
{
    [Fact]
    public void StarfieldImporter_CanImport_ReturnsTrueOnlyForStarfieldPlugin()
    {
        var importer = new StarfieldPluginExtensionImporter(Mock.Of<IStarfieldPluginRepository>());

        importer.CanImport(CreateStarfieldPlugin()).ShouldBeTrue();
        importer.CanImport(CreatePlugin(SupportedGame.Starfield)).ShouldBeFalse();
        importer.CanImport(CreateFallout4Plugin()).ShouldBeFalse();
    }

    [Fact]
    public void StarfieldImporter_Import_WithStarfieldPlugin_SavesPlugin()
    {
        var repository = new Mock<IStarfieldPluginRepository>();
        var importer = new StarfieldPluginExtensionImporter(repository.Object);
        var plugin = CreateStarfieldPlugin();

        importer.Import(plugin);

        repository.Verify(repo => repo.Save(plugin), Times.Once);
    }

    [Fact]
    public void StarfieldImporter_Import_WithWrongPluginType_Throws()
    {
        var importer = new StarfieldPluginExtensionImporter(Mock.Of<IStarfieldPluginRepository>());

        Should.Throw<ArgumentException>(() => importer.Import(CreatePlugin(SupportedGame.Starfield)))
            .Message.ShouldContain("Plugin must be a Starfield plugin.");
    }

    [Fact]
    public void Fallout4Importer_CanImport_ReturnsTrueOnlyForFallout4Plugin()
    {
        var importer = new Fallout4PluginExtensionImporter(Mock.Of<IFallout4PluginRepository>());

        importer.CanImport(CreateFallout4Plugin()).ShouldBeTrue();
        importer.CanImport(CreatePlugin(SupportedGame.Fallout4)).ShouldBeFalse();
        importer.CanImport(CreateSkyrimPlugin()).ShouldBeFalse();
    }

    [Fact]
    public void Fallout4Importer_Import_WithFallout4Plugin_SavesPlugin()
    {
        var repository = new Mock<IFallout4PluginRepository>();
        var importer = new Fallout4PluginExtensionImporter(repository.Object);
        var plugin = CreateFallout4Plugin();

        importer.Import(plugin);

        repository.Verify(repo => repo.Save(plugin), Times.Once);
    }

    [Fact]
    public void Fallout4Importer_Import_WithWrongPluginType_Throws()
    {
        var importer = new Fallout4PluginExtensionImporter(Mock.Of<IFallout4PluginRepository>());

        Should.Throw<ArgumentException>(() => importer.Import(CreatePlugin(SupportedGame.Fallout4)))
            .Message.ShouldContain("Plugin must be a Fallout 4 plugin.");
    }

    [Fact]
    public void SkyrimImporter_CanImport_ReturnsTrueOnlyForSkyrimPlugin()
    {
        var importer = new SkyrimPluginExtensionImporter(Mock.Of<ISkyrimPluginRepository>());

        importer.CanImport(CreateSkyrimPlugin()).ShouldBeTrue();
        importer.CanImport(CreatePlugin(SupportedGame.Skyrim)).ShouldBeFalse();
        importer.CanImport(CreateStarfieldPlugin()).ShouldBeFalse();
    }

    [Fact]
    public void SkyrimImporter_Import_WithSkyrimPlugin_SavesPlugin()
    {
        var repository = new Mock<ISkyrimPluginRepository>();
        var importer = new SkyrimPluginExtensionImporter(repository.Object);
        var plugin = CreateSkyrimPlugin();

        importer.Import(plugin);

        repository.Verify(repo => repo.Save(plugin), Times.Once);
    }

    [Fact]
    public void SkyrimImporter_Import_WithWrongPluginType_Throws()
    {
        var importer = new SkyrimPluginExtensionImporter(Mock.Of<ISkyrimPluginRepository>());

        Should.Throw<ArgumentException>(() => importer.Import(CreatePlugin(SupportedGame.Skyrim)))
            .Message.ShouldContain("Plugin must be a Skyrim plugin.");
    }

    private static StarfieldPluginDTO CreateStarfieldPlugin()
    {
        return new StarfieldPluginDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey("Starfield", "Starfield.esm"),
            LoadOrderIndex = 0,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current,
            HeaderFlags = 0,
            FormVersion = 1,
            RecordCount = 0,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow,
            Branch = string.Empty
        };
    }

    private static Fallout4PluginDTO CreateFallout4Plugin()
    {
        return new Fallout4PluginDTO
        {
            Game = SupportedGame.Fallout4,
            ModKey = CreateModKey("Fallout4", "Fallout4.esm"),
            LoadOrderIndex = 0,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current,
            HeaderFlags = 0,
            FormVersion = 1,
            RecordCount = 0,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow
        };
    }

    private static SkyrimPluginDTO CreateSkyrimPlugin()
    {
        return new SkyrimPluginDTO
        {
            Game = SupportedGame.Skyrim,
            ModKey = CreateModKey("Skyrim", "Skyrim.esm"),
            LoadOrderIndex = 0,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current,
            HeaderFlags = 0,
            FormVersion = 1,
            RecordCount = 0,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow
        };
    }

    private static PluginDTO CreatePlugin(SupportedGame game)
    {
        return new PluginDTO
        {
            Game = game,
            ModKey = CreateModKey(game.ToString(), $"{game}.esm"),
            LoadOrderIndex = 0,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current,
            HeaderFlags = 0,
            FormVersion = 1,
            RecordCount = 0,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow
        };
    }

    private static ModKeyDTO CreateModKey(string name, string fileName)
    {
        return new ModKeyDTO
        {
            Name = name,
            Type = 0,
            FileName = fileName
        };
    }
}
