using Moq;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class FormListServiceTests
{
    [Fact]
    public void GetByModKey_DelegatesToFormListRepository()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var expected = new List<FormListDTO>();
        var repository = new Mock<IFormListRepository>();
        repository.Setup(x => x.GetByModKey(modKey)).Returns(expected);
        var sut = new FormListService(repository.Object, Mock.Of<IFormListItemRepository>());

        var result = sut.GetByModKey(modKey);

        result.ShouldBeSameAs(expected);
    }

    [Fact]
    public void GetByFormKey_DelegatesToFormListRepository()
    {
        var formKey = new FormKey(new ModKey("Origin", ModType.Master), 123);
        var expected = new List<FormListDTO>();
        var repository = new Mock<IFormListRepository>();
        repository.Setup(x => x.GetByFormKey(formKey)).Returns(expected);
        var sut = new FormListService(repository.Object, Mock.Of<IFormListItemRepository>());

        var result = sut.GetByFormKey(formKey);

        result.ShouldBeSameAs(expected);
    }

    [Fact]
    public void GetItems_ReturnsOrderedDuplicateItemsFromRepository()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var formKey = new FormKey(modKey, 123);
        var itemModKey = new ModKey("Item", ModType.Master);
        var itemFormKey = new FormKey(itemModKey, 456);
        var expected = new List<FormListItemDTO>
        {
            CreateItem(modKey, formKey, itemModKey, itemFormKey, 0),
            CreateItem(modKey, formKey, itemModKey, itemFormKey, 1)
        };
        var repository = new Mock<IFormListItemRepository>();
        repository.Setup(x => x.GetByFormList(modKey, formKey)).Returns(expected);
        var sut = new FormListService(Mock.Of<IFormListRepository>(), repository.Object);

        var result = sut.GetItems(modKey, formKey);

        result.ShouldBeSameAs(expected);
        result.Select(item => item.ItemIndex).ShouldBe(new[] { 0, 1 });
    }

    private static FormListItemDTO CreateItem(ModKey modKey, FormKey formKey, ModKey itemModKey, FormKey itemFormKey, int itemIndex)
    {
        return new FormListItemDTO
        {
            ModKey = modKey,
            FormKey = formKey,
            ItemModKey = itemModKey,
            ItemFormKey = itemFormKey,
            ItemIndex = itemIndex,
            ImportedAtUTC = DateTime.UtcNow
        };
    }
}
