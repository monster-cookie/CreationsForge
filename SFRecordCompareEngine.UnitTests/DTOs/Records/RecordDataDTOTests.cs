using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.DTOs.Records;

public class RecordDataDTOTests
{
    [Fact]
    public void FormListDTO_AssignsProperties()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var formKey = new FormKey(modKey, 123);
        var addToListFormKey = new FormKey(modKey, 456);
        var importedAtUTC = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var sut = new FormListDTO
        {
            ModKey = modKey,
            FormKey = formKey,
            EditorID = "Editor",
            FormVersion = 44,
            StarfieldMajorRecordFlags = (StarfieldMajorRecord.StarfieldMajorRecordFlag)1,
            Version2 = 2,
            VersionControl = 3,
            ImportedAtUTC = importedAtUTC,
            AddToListFormKey = addToListFormKey
        };

        sut.ModKey.ShouldBe(modKey);
        sut.FormKey.ShouldBe(formKey);
        sut.EditorID.ShouldBe("Editor");
        sut.FormVersion.ShouldBe(44);
        sut.StarfieldMajorRecordFlags.ShouldBe((StarfieldMajorRecord.StarfieldMajorRecordFlag)1);
        sut.Version2.ShouldBe(2);
        sut.VersionControl.ShouldBe(3);
        sut.ImportedAtUTC.ShouldBe(importedAtUTC);
        sut.AddToListFormKey.ShouldBe(addToListFormKey);
    }

    [Fact]
    public void FormListRecordDataDTO_DefaultsAndAssignsProperties()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var formKey = new FormKey(modKey, 123);
        var itemModKey = new ModKey("Item", ModType.Master);
        var itemFormKey = new FormKey(itemModKey, 456);
        var sut = new FormListRecordDataDTO
        {
            FormKey = formKey,
            EditorID = "Editor",
            FormVersion = 44,
            StarfieldMajorRecordFlags = (StarfieldMajorRecord.StarfieldMajorRecordFlag)1,
            Version2 = 2,
            VersionControl = 3,
            Items = new List<FormListItemDataDTO>
            {
                new()
                {
                    ItemModKey = itemModKey,
                    ItemFormKey = itemFormKey
                }
            }
        };

        sut.FormKey.ShouldBe(formKey);
        sut.EditorID.ShouldBe("Editor");
        sut.FormVersion.ShouldBe(44);
        sut.StarfieldMajorRecordFlags.ShouldBe((StarfieldMajorRecord.StarfieldMajorRecordFlag)1);
        sut.Version2.ShouldBe(2);
        sut.VersionControl.ShouldBe(3);
        sut.AddToListFormKey.ShouldBeNull();
        sut.Items.Single().ItemModKey.ShouldBe(itemModKey);
        sut.Items.Single().ItemFormKey.ShouldBe(itemFormKey);
    }

    [Fact]
    public void GameSettingDTO_AssignsProperties()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var sut = new GameSettingDTO
        {
            ModKey = modKey,
            FormID = "000001",
            SettingType = "String",
            TitleString = "Title",
            Data = "Data",
            RawData = 1.5,
            XALG = 2,
            IsCompressed = 0,
            IsDeleted = 1,
            ImportedAtUTC = "2026-01-02T03:04:05Z"
        };

        sut.ModKey.ShouldBe(modKey);
        sut.FormID.ShouldBe("000001");
        sut.SettingType.ShouldBe("String");
        sut.TitleString.ShouldBe("Title");
        sut.Data.ShouldBe("Data");
        sut.RawData.ShouldBe(1.5);
        sut.XALG.ShouldBe(2);
        sut.IsCompressed.ShouldBe(0);
        sut.IsDeleted.ShouldBe(1);
        sut.ImportedAtUTC.ShouldBe("2026-01-02T03:04:05Z");
    }

    [Fact]
    public void RecordComparisonFieldNodeDTO_DefaultsChildrenAndAssignsProperties()
    {
        var child = new RecordComparisonFieldNodeDTO
        {
            Name = "Child",
            Value = "Value"
        };
        var sut = new RecordComparisonFieldNodeDTO
        {
            Name = "Parent",
            Children = new List<RecordComparisonFieldNodeDTO> { child }
        };

        sut.Name.ShouldBe("Parent");
        sut.Value.ShouldBeNull();
        sut.Children.ShouldBe(new List<RecordComparisonFieldNodeDTO> { child });
    }
}
