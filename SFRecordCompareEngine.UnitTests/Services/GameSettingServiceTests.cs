using Moq;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class GameSettingServiceTests
{
    [Fact]
    public void GetByModKey_DelegatesToGameSettingRepository()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var expected = new List<GameSettingDTO>();
        var repository = new Mock<IGameSettingRepository>();
        repository.Setup(x => x.GetByModKey(modKey)).Returns(expected);
        var sut = new GameSettingService(repository.Object);

        var result = sut.GetByModKey(modKey);

        result.ShouldBeSameAs(expected);
    }

    [Fact]
    public void GetByFormKey_DelegatesToGameSettingRepository()
    {
        var formKey = new FormKey(new ModKey("Origin", ModType.Master), 123);
        var expected = new List<GameSettingDTO>();
        var repository = new Mock<IGameSettingRepository>();
        repository.Setup(x => x.GetByFormKey(formKey)).Returns(expected);
        var sut = new GameSettingService(repository.Object);

        var result = sut.GetByFormKey(formKey);

        result.ShouldBeSameAs(expected);
    }
}
