using Mutagen.Bethesda.Plugins;
using Moq;
using SFRecordCompareEngine.Core.DTOs.Plugins;
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
                PluginFileName = "Example.esm",
                PluginPath = "Example.esm",
                LoadOrderIndex = 0,
                Enabled = true
            }
        };
        var reader = new Mock<IStarfieldPluginReaderService>();
        reader.Setup(x => x.GetLoadOrder()).Returns(expected);
        var sut = new PluginService(reader.Object);

        var result = sut.GetLoadOrder();

        result.ShouldBeSameAs(expected);
    }

    [Fact]
    public void GetRecordTypes_ReturnsSortedRecordTypeNames()
    {
        var sut = new PluginService(Mock.Of<IStarfieldPluginReaderService>());

        var result = sut.GetRecordTypes();

        result.ShouldNotBeEmpty();
        result.ShouldBe(result.OrderBy(x => x).ToList());
    }
}
