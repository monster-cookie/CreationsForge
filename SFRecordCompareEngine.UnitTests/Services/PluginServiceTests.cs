using Moq;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;
using SFRecordCompareEngine.Core.Services.Interfaces;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class PluginServiceTests
{
    [Fact]
    public void GetLoadOrder_DelegatesToStarfieldPluginReaderService()
    {
        var expected = new List<PluginLoadOrderEntryDTO>
        {
            new()
            {
                ModKey = new ModKey("Example", ModType.Master),
                LoadOrderIndex = 0,
                Enabled = true
            }
        };
        var reader = new Mock<IStarfieldPluginReaderService>();
        reader.Setup(x => x.GetLoadOrder()).Returns(expected);
        var sut = new PluginService(reader.Object, Mock.Of<IPluginRepository>());

        var result = sut.GetLoadOrder();

        result.ShouldBeSameAs(expected);
    }

    [Fact]
    public void GetRecordTypes_ReturnsSortedRecordTypeNames()
    {
        var sut = new PluginService(Mock.Of<IStarfieldPluginReaderService>(), Mock.Of<IPluginRepository>());

        var result = sut.GetRecordTypes();

        result.ShouldNotBeEmpty();
        result.ShouldBe(result.OrderBy(x => x).ToList());
    }

    [Fact]
    public void GetImportedPlugins_DelegatesToPluginRepository()
    {
        var expected = new List<PluginDTO>();
        var repository = new Mock<IPluginRepository>();
        repository.Setup(x => x.GetImportedPlugins()).Returns(expected);
        var sut = new PluginService(Mock.Of<IStarfieldPluginReaderService>(), repository.Object);

        var result = sut.GetImportedPlugins();

        result.ShouldBeSameAs(expected);
    }

    [Fact]
    public void GetOpenablePlugins_DelegatesToPluginRepository()
    {
        var expected = new List<PluginDTO>();
        var repository = new Mock<IPluginRepository>();
        repository.Setup(x => x.GetOpenablePlugins()).Returns(expected);
        var sut = new PluginService(Mock.Of<IStarfieldPluginReaderService>(), repository.Object);

        var result = sut.GetOpenablePlugins();

        result.ShouldBeSameAs(expected);
    }

    [Fact]
    public void SearchOpenablePluginsByFilename_DelegatesToPluginRepository()
    {
        var expected = new List<PluginDTO>();
        var repository = new Mock<IPluginRepository>();
        repository.Setup(x => x.SearchOpenablePluginsByFilename("example")).Returns(expected);
        var sut = new PluginService(Mock.Of<IStarfieldPluginReaderService>(), repository.Object);

        var result = sut.SearchOpenablePluginsByFilename("example");

        result.ShouldBeSameAs(expected);
    }
}