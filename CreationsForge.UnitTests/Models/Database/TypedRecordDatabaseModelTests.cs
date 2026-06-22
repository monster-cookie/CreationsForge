using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Database;
using Shouldly;
using GlobalModel = CreationsForge.Core.Models.Database.Global;
using ModelDatabase = CreationsForge.Core.Models.Database.Model;
using ModelMaterialSwapDatabase = CreationsForge.Core.Models.Database.ModelMaterialSwap;

namespace CreationsForge.UnitTests.Models.Database;

public class TypedRecordDatabaseModelTests
{
    [Fact]
    public void FormList_MapsCommonRecordFieldsAndNullAddToListFormKey()
    {
        var dto = CreateFormListDTO(null);

        var model = new FormList(dto);

        AssertCommonRecordFields(model.Game, model.ModKeyName, model.ModKeyType, model.ModKeyFileName, model.FormKeyModKeyName, model.FormKeyModKeyType, model.FormKeyModKeyFileName, model.FormKeyId, model.EditorId, model.FormVersion, model.MajorRecordFlags, model.ImportedAtUTC, dto);
        model.AddToListModKeyName.ShouldBeNull();
        model.AddToListModKeyType.ShouldBeNull();
        model.AddToListModKeyFileName.ShouldBeNull();
        model.AddToListFormKeyId.ShouldBeNull();
    }

    [Fact]
    public void FormList_MapsPopulatedAddToListFormKey()
    {
        var addToListFormKey = CreateFormKey("AddToList", 2, "AddToList.esm", 5678);
        var dto = CreateFormListDTO(addToListFormKey);

        var model = new FormList(dto);

        model.AddToListModKeyName.ShouldBe("AddToList");
        model.AddToListModKeyType.ShouldBe(2);
        model.AddToListModKeyFileName.ShouldBe("AddToList.esm");
        model.AddToListFormKeyId.ShouldBe(5678);
    }

    [Fact]
    public void FormListItem_MapsParentAndItemKeys()
    {
        var dto = new FormListItemDTO
        {
            Game = SupportedGame.Fallout4,
            ModKey = CreateModKey("Container", 1, "Container.esm"),
            FormKey = CreateFormKey("Parent", 2, "Parent.esm", 100),
            ItemFormKey = CreateFormKey("Item", 3, "Item.esm", 200),
            ItemIndex = 7,
            ImportedAtUTC = new DateTime(2026, 6, 5, 18, 30, 0, DateTimeKind.Utc)
        };

        var model = new FormListItem(dto);

        model.Game.ShouldBe("Fallout4");
        model.ModKeyName.ShouldBe("Container");
        model.ModKeyType.ShouldBe(1);
        model.ModKeyFileName.ShouldBe("Container.esm");
        model.FormKeyModKeyName.ShouldBe("Parent");
        model.FormKeyModKeyType.ShouldBe(2);
        model.FormKeyModKeyFileName.ShouldBe("Parent.esm");
        model.FormKeyId.ShouldBe(100);
        model.ItemModKeyName.ShouldBe("Item");
        model.ItemModKeyType.ShouldBe(3);
        model.ItemModKeyFileName.ShouldBe("Item.esm");
        model.ItemFormKeyId.ShouldBe(200);
        model.ItemIndex.ShouldBe(7);
        model.ImportedAtUTC.ShouldBe(dto.ImportedAtUTC);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(false, 0)]
    [InlineData(true, 1)]
    public void GameSetting_MapsCommonFieldsAndBooleanData(bool? booleanData, int? expectedBooleanData)
    {
        var dto = new GameSettingDTO
        {
            Game = SupportedGame.Skyrim,
            ModKey = CreateModKey("Settings", 1, "Settings.esm"),
            FormKey = CreateFormKey("SettingForm", 2, "SettingForm.esm", 300),
            EditorID = "SettingEditor",
            FormVersion = 44,
            MajorRecordFlags = 55,
            ImportedAtUTC = new DateTime(2026, 6, 5, 19, 0, 0, DateTimeKind.Utc),
            Version2 = 66,
            VersionControl = 77,
            DataType = GameSettingDataType.Boolean,
            Data = new GameSettingDataDTO
            {
                DataType = GameSettingDataType.Boolean,
                Boolean = booleanData
            }
        };

        var model = new GameSetting(dto);

        AssertCommonRecordFields(model.Game, model.ModKeyName, model.ModKeyType, model.ModKeyFileName, model.FormKeyModKeyName, model.FormKeyModKeyType, model.FormKeyModKeyFileName, model.FormKeyId, model.EditorId, model.FormVersion, model.MajorRecordFlags, model.ImportedAtUTC, dto);
        model.Version2.ShouldBe(66);
        model.VersionControl.ShouldBe(77);
        model.DataType.ShouldBe(GameSettingDataType.Boolean.ToString());
        model.Data.ShouldBeNull();
        model.FloatData.ShouldBeNull();
        model.IntegerData.ShouldBeNull();
        model.UnsignedIntegerData.ShouldBeNull();
        model.BooleanData.ShouldBe(expectedBooleanData);
    }

    [Fact]
    public void Global_MapsCommonFieldsAndData()
    {
        var dto = new GlobalDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey("Globals", 1, "Globals.esm"),
            FormKey = CreateFormKey("GlobalForm", 2, "GlobalForm.esm", 400),
            EditorID = "GlobalEditor",
            FormVersion = 33,
            MajorRecordFlags = 44,
            ImportedAtUTC = new DateTime(2026, 6, 5, 19, 30, 0, DateTimeKind.Utc),
            Data = 42.5
        };

        var model = new GlobalModel(dto);

        AssertCommonRecordFields(model.Game, model.ModKeyName, model.ModKeyType, model.ModKeyFileName, model.FormKeyModKeyName, model.FormKeyModKeyType, model.FormKeyModKeyFileName, model.FormKeyId, model.EditorId, model.FormVersion, model.MajorRecordFlags, model.ImportedAtUTC, dto);
        model.Data.ShouldBe(42.5);
    }

    [Fact]
    public void Model_MapsRecordIdentityAndModelFields()
    {
        var dto = new ModelDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey("Container", 1, "Container.esm"),
            RecordType = "MISC",
            FormKey = CreateFormKey("Form", 2, "Form.esm", 1234),
            ModelSlot = "Model",
            ModelGender = string.Empty,
            File = "Meshes/Test.nif",
            TextureFileHashes = "AABBCC",
            LightLayer = 7,
            Flags = "HasDistantLOD",
            ColorRemappingIndex = 1.5f,
            FlagsVestigial = "HasDistantLOD",
            ImportedAtUTC = new DateTime(2026, 6, 7, 12, 0, 0, DateTimeKind.Utc)
        };

        var model = new ModelDatabase(dto);

        model.Game.ShouldBe("Starfield");
        model.ModKeyName.ShouldBe("Container");
        model.ModKeyType.ShouldBe(1);
        model.ModKeyFileName.ShouldBe("Container.esm");
        model.RecordType.ShouldBe("MISC");
        model.FormKeyModKeyName.ShouldBe("Form");
        model.FormKeyModKeyType.ShouldBe(2);
        model.FormKeyModKeyFileName.ShouldBe("Form.esm");
        model.FormKeyId.ShouldBe(1234);
        model.ModelSlot.ShouldBe("Model");
        model.ModelGender.ShouldBe(string.Empty);
        model.File.ShouldBe("Meshes/Test.nif");
        model.TextureFileHashes.ShouldBe("AABBCC");
        model.LightLayer.ShouldBe(7);
        model.Flags.ShouldBe("HasDistantLOD");
        model.ColorRemappingIndex.ShouldBe(1.5f);
        model.FlagsVestigial.ShouldBe("HasDistantLOD");
        model.ImportedAtUTC.ShouldBe(dto.ImportedAtUTC);
    }

    [Fact]
    public void ModelMaterialSwap_MapsParentModelKeyAndMaterialSwapKey()
    {
        var dto = new ModelMaterialSwapDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey("Container", 1, "Container.esm"),
            RecordType = "MISC",
            FormKey = CreateFormKey("Form", 2, "Form.esm", 1234),
            ModelSlot = "Model",
            ModelGender = string.Empty,
            MaterialSwapFormKey = CreateFormKey("Material", 3, "Material.esm", 5678),
            MaterialSwapIndex = 2,
            ImportedAtUTC = new DateTime(2026, 6, 7, 12, 30, 0, DateTimeKind.Utc)
        };

        var model = new ModelMaterialSwapDatabase(dto);

        model.Game.ShouldBe("Starfield");
        model.ModKeyName.ShouldBe("Container");
        model.ModKeyType.ShouldBe(1);
        model.ModKeyFileName.ShouldBe("Container.esm");
        model.RecordType.ShouldBe("MISC");
        model.FormKeyModKeyName.ShouldBe("Form");
        model.FormKeyModKeyType.ShouldBe(2);
        model.FormKeyModKeyFileName.ShouldBe("Form.esm");
        model.FormKeyId.ShouldBe(1234);
        model.ModelSlot.ShouldBe("Model");
        model.ModelGender.ShouldBe(string.Empty);
        model.MaterialSwapModKeyName.ShouldBe("Material");
        model.MaterialSwapModKeyType.ShouldBe(3);
        model.MaterialSwapModKeyFileName.ShouldBe("Material.esm");
        model.MaterialSwapFormKeyId.ShouldBe(5678);
        model.MaterialSwapIndex.ShouldBe(2);
        model.ImportedAtUTC.ShouldBe(dto.ImportedAtUTC);
    }

    private static FormListDTO CreateFormListDTO(FormKeyDTO? addToListFormKey)
    {
        return new FormListDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey("Container", 1, "Container.esm"),
            FormKey = CreateFormKey("Form", 2, "Form.esm", 1234),
            EditorID = "FormListEditor",
            FormVersion = 12,
            MajorRecordFlags = 34,
            ImportedAtUTC = new DateTime(2026, 6, 5, 18, 0, 0, DateTimeKind.Utc),
            AddToListFormKey = addToListFormKey
        };
    }

    private static ModKeyDTO CreateModKey(string name, int type, string fileName)
    {
        return new ModKeyDTO
        {
            Name = name,
            Type = type,
            FileName = fileName
        };
    }

    private static FormKeyDTO CreateFormKey(string modKeyName, int modKeyType, string modKeyFileName, uint id)
    {
        return new FormKeyDTO
        {
            ModKey = CreateModKey(modKeyName, modKeyType, modKeyFileName),
            Id = id
        };
    }

    private static void AssertCommonRecordFields(string game, string modKeyName, int modKeyType, string modKeyFileName, string formKeyModKeyName, int formKeyModKeyType, string formKeyModKeyFileName, long formKeyId, string editorId, int formVersion, int majorRecordFlags, DateTime importedAtUTC, RecordDTO dto)
    {
        game.ShouldBe(dto.Game.ToString());
        modKeyName.ShouldBe(dto.ModKey.Name);
        modKeyType.ShouldBe(dto.ModKey.Type);
        modKeyFileName.ShouldBe(dto.ModKey.FileName);
        formKeyModKeyName.ShouldBe(dto.FormKey.ModKey.Name);
        formKeyModKeyType.ShouldBe(dto.FormKey.ModKey.Type);
        formKeyModKeyFileName.ShouldBe(dto.FormKey.ModKey.FileName);
        formKeyId.ShouldBe(dto.FormKey.Id);
        editorId.ShouldBe(dto.EditorID);
        formVersion.ShouldBe(dto.FormVersion);
        majorRecordFlags.ShouldBe(dto.MajorRecordFlags);
        importedAtUTC.ShouldBe(dto.ImportedAtUTC);
    }
}
