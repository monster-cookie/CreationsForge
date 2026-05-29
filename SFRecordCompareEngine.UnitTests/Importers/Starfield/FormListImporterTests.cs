using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Moq;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Starfield;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Importers.Starfield;

public class FormListImporterTests
{
    [Fact]
    public void Properties_ReturnFormListMetadata()
    {
        var sut = new FormListImporter(
            Mock.Of<IFormListRepository>(),
            Mock.Of<IFormListItemRepository>());

        sut.GameRelease.ShouldBe(GameRelease.Starfield);
        sut.RecordType.ShouldBe(new RecordType(RecordTypeCatalog.FormList.RecordID));
        sut.TableName.ShouldBe(RecordTypeCatalog.FormList.TableName);
    }

    [Fact]
    public void Import_WhenFormListExists_SavesFormListAndItems()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var formKey = new FormKey(modKey, 123);
        var itemModKey = new ModKey("Item", ModType.Master);
        var itemFormKey = new FormKey(itemModKey, 456);
        var addToListFormKey = new FormKey(modKey, 789);
        var formListRepository = new Mock<IFormListRepository>();
        var formListItemRepository = new Mock<IFormListItemRepository>();
        var record = new FormListDTO
        {
            ModKey = modKey,
            FormKey = formKey,
            EditorID = "Editor",
            FormVersion = 44,
            StarfieldMajorRecordFlags = (StarfieldMajorRecord.StarfieldMajorRecordFlag)1,
            Version2 = 2,
            VersionControl = 3,
            ImportedAtUTC = DateTime.UtcNow,
            AddToListFormKey = addToListFormKey,
            Items = new List<FormListItemDataDTO>
            {
                new()
                {
                    ItemModKey = itemModKey,
                    ItemFormKey = itemFormKey
                }
            }
        };
        var sut = new FormListImporter(formListRepository.Object, formListItemRepository.Object);

        var result = new RecordTypeImportResultDTO
        {
            RecordType = "FLST",
            HeaderImportSupported = true,
            TypedDetailImportSupported = true
        };

        sut.Import(record, result);

        formListRepository.Verify(x => x.Save(It.Is<FormListDTO>(dto =>
            dto.ModKey == modKey &&
            dto.FormKey == formKey &&
            dto.EditorID == "Editor" &&
            dto.FormVersion == 44 &&
            dto.StarfieldMajorRecordFlags == (StarfieldMajorRecord.StarfieldMajorRecordFlag)1 &&
            dto.Version2 == 2 &&
            dto.VersionControl == 3 &&
            dto.AddToListFormKey == addToListFormKey)), Times.Once);
        formListItemRepository.Verify(x => x.Save(It.Is<FormListItemDTO>(dto =>
            dto.ModKey == modKey &&
            dto.FormKey == formKey &&
            dto.ItemModKey == itemModKey &&
            dto.ItemFormKey == itemFormKey)), Times.Once);
        result.DetailRowsImported.ShouldBe(1);
        result.FormListItemsImported.ShouldBe(1);
    }

}
