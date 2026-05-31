using Moq;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class GlobalServiceTests
{
    [Fact]
    public void GetByModKey_DelegatesToGlobalRepository()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var expected = new List<GlobalDTO>();
        var repository = new Mock<IGlobalRepository>();
        repository.Setup(x => x.GetByModKey(modKey)).Returns(expected);
        var sut = new GlobalService(repository.Object);

        var result = sut.GetByModKey(modKey);

        result.ShouldBeSameAs(expected);
    }

    [Fact]
    public void GetByFormKeyID_DelegatesToGlobalRepository()
    {
        var expected = new List<GlobalDTO>();
        var repository = new Mock<IGlobalRepository>();
        repository.Setup(x => x.GetByFormKeyID(123)).Returns(expected);
        var sut = new GlobalService(repository.Object);

        var result = sut.GetByFormKeyID(123);

        result.ShouldBeSameAs(expected);
    }
}
