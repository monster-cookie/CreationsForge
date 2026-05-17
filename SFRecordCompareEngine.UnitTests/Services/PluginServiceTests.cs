using Moq;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.Services;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class PluginServiceTests
{
    [Fact]
    public void GetDatabases_WhenGameIsNotConfigured_ReturnsEmptyList()
    {
        var gameConfigurationStore = new Mock<IGameConfigurationStore>();
        gameConfigurationStore.SetupGet(store => store.Game).Returns(null as Mutagen.Bethesda.Environments.IGameEnvironment);
        var sut = new PluginService(gameConfigurationStore.Object);

        var result = sut.GetPlugins();

        result.ShouldBeEmpty();
    }

    [Fact]
    public void GetDatabases_WhenConfigurationStoreThrows_ReturnsEmptyList()
    {
        var gameConfigurationStore = new Mock<IGameConfigurationStore>();
        gameConfigurationStore.SetupGet(store => store.Game).Throws(new InvalidOperationException("Config failed."));
        var sut = new PluginService(gameConfigurationStore.Object);

        var result = sut.GetPlugins();

        result.ShouldBeEmpty();
    }

    [Fact]
    public void GetPluginHeader_WhenGameIsNotConfigured_ReturnsNull()
    {
        var gameConfigurationStore = new Mock<IGameConfigurationStore>();
        gameConfigurationStore.SetupGet(store => store.Game).Returns(null as Mutagen.Bethesda.Environments.IGameEnvironment);
        var sut = new PluginService(gameConfigurationStore.Object);

        var result = sut.GetPluginHeader("Example.esm");

        result.ShouldBeNull();
    }
}
