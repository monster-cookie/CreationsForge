using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Container.Starfield;

public class StarfieldContainerSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "277A73:Starfield.esm")]
    [Trait("EditorID", "ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common")]
    [Trait("SpriggitFile", "Containers/ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common - 277A73_Starfield.esm.yaml")]
    public void Starfield_CONT_ShouldMatchSpriggitSample_ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "277A73:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "ANAM").ShouldBe(Helpers.GetDTOField(dto, "ANAM"));
        Helpers.GetSpriggitField(spriggit, "BNAM").ShouldBe(Helpers.GetDTOField(dto, "BNAM"));
        Helpers.GetSpriggitField(spriggit, "CloseSound.Start").ShouldBe(Helpers.GetDTOField(dto, "CloseSound.Start"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "NativeTerminal").ShouldBe(Helpers.GetDTOField(dto, "NativeTerminalFormKey"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound.Start").ShouldBe(Helpers.GetDTOField(dto, "OpenSound.Start"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Outpost").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Outpost"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Preview").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Preview"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Value[2]").ShouldBe(Helpers.GetDTOField(dto, "Value[2]"));
        Helpers.GetSpriggitField(spriggit, "Value[3]").ShouldBe(Helpers.GetDTOField(dto, "Value[3]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminal", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound.Start", "REFL", "Transforms.Outpost", "Transforms.Preview", "Value[0]", "Value[1]", "Value[2]", "Value[3]", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminalFormKey", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound.Start", "REFL", "Transforms.Outpost", "Transforms.Preview", "Value[0]", "Value[1]", "Value[2]", "Value[3]", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "277A81:Starfield.esm")]
    [Trait("EditorID", "ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare")]
    [Trait("SpriggitFile", "Containers/ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare - 277A81_Starfield.esm.yaml")]
    public void Starfield_CONT_ShouldMatchSpriggitSample_ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "277A81:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "ANAM").ShouldBe(Helpers.GetDTOField(dto, "ANAM"));
        Helpers.GetSpriggitField(spriggit, "BNAM").ShouldBe(Helpers.GetDTOField(dto, "BNAM"));
        Helpers.GetSpriggitField(spriggit, "CloseSound.Start").ShouldBe(Helpers.GetDTOField(dto, "CloseSound.Start"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "NativeTerminal").ShouldBe(Helpers.GetDTOField(dto, "NativeTerminalFormKey"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound.Start").ShouldBe(Helpers.GetDTOField(dto, "OpenSound.Start"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Outpost").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Outpost"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Preview").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Preview"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Value[2]").ShouldBe(Helpers.GetDTOField(dto, "Value[2]"));
        Helpers.GetSpriggitField(spriggit, "Value[3]").ShouldBe(Helpers.GetDTOField(dto, "Value[3]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminal", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound.Start", "REFL", "Transforms.Outpost", "Transforms.Preview", "Value[0]", "Value[1]", "Value[2]", "Value[3]", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminalFormKey", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound.Start", "REFL", "Transforms.Outpost", "Transforms.Preview", "Value[0]", "Value[1]", "Value[2]", "Value[3]", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "2779E9:Starfield.esm")]
    [Trait("EditorID", "ShipOutpost_Loot_Storage_BossChest_Industrial_Rare")]
    [Trait("SpriggitFile", "Containers/ShipOutpost_Loot_Storage_BossChest_Industrial_Rare - 2779E9_Starfield.esm.yaml")]
    public void Starfield_CONT_ShouldMatchSpriggitSample_ShipOutpost_Loot_Storage_BossChest_Industrial_Rare()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "ShipOutpost_Loot_Storage_BossChest_Industrial_Rare");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "2779E9:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "ANAM").ShouldBe(Helpers.GetDTOField(dto, "ANAM"));
        Helpers.GetSpriggitField(spriggit, "BNAM").ShouldBe(Helpers.GetDTOField(dto, "BNAM"));
        Helpers.GetSpriggitField(spriggit, "CloseSound.Start").ShouldBe(Helpers.GetDTOField(dto, "CloseSound.Start"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "NativeTerminal").ShouldBe(Helpers.GetDTOField(dto, "NativeTerminalFormKey"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound.Start").ShouldBe(Helpers.GetDTOField(dto, "OpenSound.Start"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Outpost").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Outpost"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Preview").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Preview"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Value[2]").ShouldBe(Helpers.GetDTOField(dto, "Value[2]"));
        Helpers.GetSpriggitField(spriggit, "Value[3]").ShouldBe(Helpers.GetDTOField(dto, "Value[3]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminal", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound.Start", "REFL", "Transforms.Outpost", "Transforms.Preview", "Value[0]", "Value[1]", "Value[2]", "Value[3]", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminalFormKey", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound.Start", "REFL", "Transforms.Outpost", "Transforms.Preview", "Value[0]", "Value[1]", "Value[2]", "Value[3]", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "1A23DF:Starfield.esm")]
    [Trait("EditorID", "Loot_Display_WeaponRack03_EMPTY")]
    [Trait("SpriggitFile", "Containers/Loot_Display_WeaponRack03_EMPTY - 1A23DF_Starfield.esm.yaml")]
    public void Starfield_CONT_ShouldMatchSpriggitSample_Loot_Display_WeaponRack03_EMPTY()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "Loot_Display_WeaponRack03_EMPTY");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "1A23DF:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "ANAM").ShouldBe(Helpers.GetDTOField(dto, "ANAM"));
        Helpers.GetSpriggitField(spriggit, "BNAM").ShouldBe(Helpers.GetDTOField(dto, "BNAM"));
        Helpers.GetSpriggitField(spriggit, "CloseSound.Start").ShouldBe(Helpers.GetDTOField(dto, "CloseSound.Start"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
        Helpers.GetSpriggitField(spriggit, "ContainsOnlyFilter").ShouldBe(Helpers.GetDTOField(dto, "ContainsOnlyFilter"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Index[0]").ShouldBe(Helpers.GetDTOField(dto, "Index[0]"));
        Helpers.GetSpriggitField(spriggit, "Index[1]").ShouldBe(Helpers.GetDTOField(dto, "Index[1]"));
        Helpers.GetSpriggitField(spriggit, "Index[10]").ShouldBe(Helpers.GetDTOField(dto, "Index[10]"));
        Helpers.GetSpriggitField(spriggit, "Index[2]").ShouldBe(Helpers.GetDTOField(dto, "Index[2]"));
        Helpers.GetSpriggitField(spriggit, "Index[3]").ShouldBe(Helpers.GetDTOField(dto, "Index[3]"));
        Helpers.GetSpriggitField(spriggit, "Index[4]").ShouldBe(Helpers.GetDTOField(dto, "Index[4]"));
        Helpers.GetSpriggitField(spriggit, "Index[5]").ShouldBe(Helpers.GetDTOField(dto, "Index[5]"));
        Helpers.GetSpriggitField(spriggit, "Index[6]").ShouldBe(Helpers.GetDTOField(dto, "Index[6]"));
        Helpers.GetSpriggitField(spriggit, "Index[7]").ShouldBe(Helpers.GetDTOField(dto, "Index[7]"));
        Helpers.GetSpriggitField(spriggit, "Index[8]").ShouldBe(Helpers.GetDTOField(dto, "Index[8]"));
        Helpers.GetSpriggitField(spriggit, "Index[9]").ShouldBe(Helpers.GetDTOField(dto, "Index[9]"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "NativeTerminal").ShouldBe(Helpers.GetDTOField(dto, "NativeTerminalFormKey"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound.Start").ShouldBe(Helpers.GetDTOField(dto, "OpenSound.Start"));
        Helpers.GetSpriggitField(spriggit, "SnapTemplate").ShouldBe(Helpers.GetDTOField(dto, "SnapTemplate"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[0]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[0]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[1]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[1]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[10]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[10]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[11]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[11]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[2]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[2]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[3]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[3]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[4]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[4]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[5]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[5]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[6]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[6]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[7]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[7]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[8]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[8]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[9]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[9]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "ContainsOnlyFilter", "EditorID", "FormKey", "FormVersion", "Index[0]", "Index[1]", "Index[10]", "Index[2]", "Index[3]", "Index[4]", "Index[5]", "Index[6]", "Index[7]", "Index[8]", "Index[9]", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminal", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound.Start", "SnapTemplate", "Unknown2[0]", "Unknown2[1]", "Unknown2[10]", "Unknown2[11]", "Unknown2[2]", "Unknown2[3]", "Unknown2[4]", "Unknown2[5]", "Unknown2[6]", "Unknown2[7]", "Unknown2[8]", "Unknown2[9]", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "ContainsOnlyFilter", "EditorID", "FormKey", "FormVersion", "Index[0]", "Index[1]", "Index[10]", "Index[2]", "Index[3]", "Index[4]", "Index[5]", "Index[6]", "Index[7]", "Index[8]", "Index[9]", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminalFormKey", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound.Start", "SnapTemplate", "Unknown2[0]", "Unknown2[1]", "Unknown2[10]", "Unknown2[11]", "Unknown2[2]", "Unknown2[3]", "Unknown2[4]", "Unknown2[5]", "Unknown2[6]", "Unknown2[7]", "Unknown2[8]", "Unknown2[9]", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "057C20:Starfield.esm")]
    [Trait("EditorID", "Loot_Display_ArboronWeaponRackPanel02")]
    [Trait("SpriggitFile", "Containers/Loot_Display_ArboronWeaponRackPanel02 - 057C20_Starfield.esm.yaml")]
    public void Starfield_CONT_ShouldMatchSpriggitSample_Loot_Display_ArboronWeaponRackPanel02()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "Loot_Display_ArboronWeaponRackPanel02");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "057C20:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "CloseSound.Start").ShouldBe(Helpers.GetDTOField(dto, "CloseSound.Start"));
        Helpers.GetSpriggitField(spriggit, "ContainsOnlyFilter").ShouldBe(Helpers.GetDTOField(dto, "ContainsOnlyFilter"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Index[0]").ShouldBe(Helpers.GetDTOField(dto, "Index[0]"));
        Helpers.GetSpriggitField(spriggit, "Index[1]").ShouldBe(Helpers.GetDTOField(dto, "Index[1]"));
        Helpers.GetSpriggitField(spriggit, "Index[2]").ShouldBe(Helpers.GetDTOField(dto, "Index[2]"));
        Helpers.GetSpriggitField(spriggit, "Index[3]").ShouldBe(Helpers.GetDTOField(dto, "Index[3]"));
        Helpers.GetSpriggitField(spriggit, "Index[4]").ShouldBe(Helpers.GetDTOField(dto, "Index[4]"));
        Helpers.GetSpriggitField(spriggit, "Index[5]").ShouldBe(Helpers.GetDTOField(dto, "Index[5]"));
        Helpers.GetSpriggitField(spriggit, "Index[6]").ShouldBe(Helpers.GetDTOField(dto, "Index[6]"));
        Helpers.GetSpriggitField(spriggit, "Index[7]").ShouldBe(Helpers.GetDTOField(dto, "Index[7]"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "NativeTerminal").ShouldBe(Helpers.GetDTOField(dto, "NativeTerminalFormKey"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound.Start").ShouldBe(Helpers.GetDTOField(dto, "OpenSound.Start"));
        Helpers.GetSpriggitField(spriggit, "SnapTemplate").ShouldBe(Helpers.GetDTOField(dto, "SnapTemplate"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[0]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[0]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[1]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[1]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[2]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[2]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[3]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[3]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[4]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[4]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[5]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[5]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[6]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[6]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[7]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[7]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[8]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[8]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CloseSound.Start", "ContainsOnlyFilter", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "EditorID", "FormKey", "FormVersion", "Index[0]", "Index[1]", "Index[2]", "Index[3]", "Index[4]", "Index[5]", "Index[6]", "Index[7]", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminal", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound.Start", "SnapTemplate", "Unknown2[0]", "Unknown2[1]", "Unknown2[2]", "Unknown2[3]", "Unknown2[4]", "Unknown2[5]", "Unknown2[6]", "Unknown2[7]", "Unknown2[8]", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CloseSound.Start", "ContainsOnlyFilter", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "EditorID", "FormKey", "FormVersion", "Index[0]", "Index[1]", "Index[2]", "Index[3]", "Index[4]", "Index[5]", "Index[6]", "Index[7]", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminalFormKey", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound.Start", "SnapTemplate", "Unknown2[0]", "Unknown2[1]", "Unknown2[2]", "Unknown2[3]", "Unknown2[4]", "Unknown2[5]", "Unknown2[6]", "Unknown2[7]", "Unknown2[8]", "Version2", "VersionControl");
    }
}