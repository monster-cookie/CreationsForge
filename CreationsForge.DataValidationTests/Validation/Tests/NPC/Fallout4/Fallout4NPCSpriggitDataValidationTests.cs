using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.NPC.Fallout4;

public class Fallout4NPCSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "0FB232:Fallout4.esm")]
    [Trait("EditorID", "BHExtBOSSoldier")]
    [Trait("SpriggitFile", "Npcs/BHExtBOSSoldier - 0FB232_Fallout4.esm.yaml")]
    public void Fallout4_NPC__ShouldMatchSpriggitSample_BHExtBOSSoldier()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.NPC,
            "BHExtBOSSoldier");
        var dto = Helpers.GetDTO<NPCDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.NPC,
            "0FB232:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Aggression"].ShouldBe(dtoFields["Aggression"]);
        spriggitFields["Assistance"].ShouldBe(dtoFields["Assistance"]);
        spriggitFields["AttackRace"].ShouldBe(dtoFields["AttackRace"]);
        spriggitFields["CalculatedActionPoints"].ShouldBe(dtoFields["CalculatedActionPoints"]);
        spriggitFields["CalculatedHealth"].ShouldBe(dtoFields["CalculatedHealth"]);
        spriggitFields["Class"].ShouldBe(dtoFields["Class"]);
        spriggitFields["CombatOverridePackageList"].ShouldBe(dtoFields["CombatOverridePackageList"]);
        spriggitFields["CombatStyle"].ShouldBe(dtoFields["CombatStyleFormKey"]);
        spriggitFields["Confidence"].ShouldBe(dtoFields["Confidence"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["CrimeFaction"].ShouldBe(dtoFields["CrimeFactionFormKey"]);
        spriggitFields["DeathItem"].ShouldBe(dtoFields["DeathItem"]);
        spriggitFields["DefaultOutfit"].ShouldBe(dtoFields["DefaultOutfit"]);
        spriggitFields["DefaultPackageList"].ShouldBe(dtoFields["DefaultPackageListFormKey"]);
        spriggitFields["DefaultTemplate"].ShouldBe(dtoFields["DefaultTemplate"]);
        spriggitFields["DispositionBase"].ShouldBe(dtoFields["DispositionBase"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["EnergyLevel"].ShouldBe(dtoFields["EnergyLevel"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["GearedUpWeapons"].ShouldBe(dtoFields["GearedUpWeapons"]);
        spriggitFields["HairColor"].ShouldBe(dtoFields["HairColor"]);
        spriggitFields["HeightMax"].ShouldBe(dtoFields["HeightMax"]);
        spriggitFields["HeightMin"].ShouldBe(dtoFields["HeightMin"]);
        spriggitFields["IsCompressed"].ShouldBe(dtoFields["IsCompressed"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Item[4]"].ShouldBe(dtoFields["Item[4]"]);
        spriggitFields["Level.Level"].ShouldBe(dtoFields["Level.Level"]);
        spriggitFields["Level.MutagenObjectType"].ShouldBe(dtoFields["Level.MutagenObjectType"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["NAM5"].ShouldBe(dtoFields["NAM5"]);
        spriggitFields["Race"].ShouldBe(dtoFields["RaceFormKey"]);
        spriggitFields["Responsibility"].ShouldBe(dtoFields["Responsibility"]);
        spriggitFields["SoundLevel"].ShouldBe(dtoFields["SoundLevel"]);
        spriggitFields["TemplateActors.AiDataTemplate"].ShouldBe(dtoFields["TemplateActors.AiDataTemplate"]);
        spriggitFields["TemplateActors.AttackDataTemplate"].ShouldBe(dtoFields["TemplateActors.AttackDataTemplate"]);
        spriggitFields["TemplateActors.BaseDataTemplate"].ShouldBe(dtoFields["TemplateActors.BaseDataTemplate"]);
        spriggitFields["TemplateActors.DefPackListTemplate"].ShouldBe(dtoFields["TemplateActors.DefPackListTemplate"]);
        spriggitFields["TemplateActors.InventoryTemplate"].ShouldBe(dtoFields["TemplateActors.InventoryTemplate"]);
        spriggitFields["TemplateActors.KeywordsTemplate"].ShouldBe(dtoFields["TemplateActors.KeywordsTemplate"]);
        spriggitFields["TemplateActors.SpellListTemplate"].ShouldBe(dtoFields["TemplateActors.SpellListTemplate"]);
        spriggitFields["TemplateActors.StatsTemplate"].ShouldBe(dtoFields["TemplateActors.StatsTemplate"]);
        spriggitFields["TemplateActors.TraitTemplate"].ShouldBe(dtoFields["TemplateActors.TraitTemplate"]);
        spriggitFields["TextureLighting"].ShouldBe(dtoFields["TextureLighting"]);
        spriggitFields["Unused"].ShouldBe(dtoFields["Unused"]);
        spriggitFields["UseTemplateActors"].ShouldBe(dtoFields["UseTemplateActors"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Value[2]"].ShouldBe(dtoFields["Value[2]"]);
        spriggitFields["Value[3]"].ShouldBe(dtoFields["Value[3]"]);
        spriggitFields["Value[4]"].ShouldBe(dtoFields["Value[4]"]);
        spriggitFields["Value[5]"].ShouldBe(dtoFields["Value[5]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Object"]);
        spriggitFields["Voice"].ShouldBe(dtoFields["VoiceFormKey"]);
        spriggitFields["Weight.Fat"].ShouldBe(dtoFields["Weight.Fat"]);
        spriggitFields["Weight.Muscular"].ShouldBe(dtoFields["Weight.Muscular"]);
        spriggitFields["Weight.Thin"].ShouldBe(dtoFields["Weight.Thin"]);
        spriggitFields["XpValueOffset"].ShouldBe(dtoFields["XpValueOffset"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "0FB22E:Fallout4.esm")]
    [Trait("EditorID", "BHExtBOSSoldier_PowerArmorAuto")]
    [Trait("SpriggitFile", "Npcs/BHExtBOSSoldier_PowerArmorAuto - 0FB22E_Fallout4.esm.yaml")]
    public void Fallout4_NPC__ShouldMatchSpriggitSample_BHExtBOSSoldier_PowerArmorAuto()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.NPC,
            "BHExtBOSSoldier_PowerArmorAuto");
        var dto = Helpers.GetDTO<NPCDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.NPC,
            "0FB22E:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Aggression"].ShouldBe(dtoFields["Aggression"]);
        spriggitFields["Assistance"].ShouldBe(dtoFields["Assistance"]);
        spriggitFields["AttackRace"].ShouldBe(dtoFields["AttackRace"]);
        spriggitFields["CalculatedActionPoints"].ShouldBe(dtoFields["CalculatedActionPoints"]);
        spriggitFields["CalculatedHealth"].ShouldBe(dtoFields["CalculatedHealth"]);
        spriggitFields["Class"].ShouldBe(dtoFields["Class"]);
        spriggitFields["CombatOverridePackageList"].ShouldBe(dtoFields["CombatOverridePackageList"]);
        spriggitFields["CombatStyle"].ShouldBe(dtoFields["CombatStyleFormKey"]);
        spriggitFields["Confidence"].ShouldBe(dtoFields["Confidence"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["CrimeFaction"].ShouldBe(dtoFields["CrimeFactionFormKey"]);
        spriggitFields["DeathItem"].ShouldBe(dtoFields["DeathItem"]);
        spriggitFields["DefaultOutfit"].ShouldBe(dtoFields["DefaultOutfit"]);
        spriggitFields["DefaultPackageList"].ShouldBe(dtoFields["DefaultPackageListFormKey"]);
        spriggitFields["DefaultTemplate"].ShouldBe(dtoFields["DefaultTemplate"]);
        spriggitFields["DispositionBase"].ShouldBe(dtoFields["DispositionBase"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["EnergyLevel"].ShouldBe(dtoFields["EnergyLevel"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["GearedUpWeapons"].ShouldBe(dtoFields["GearedUpWeapons"]);
        spriggitFields["HairColor"].ShouldBe(dtoFields["HairColor"]);
        spriggitFields["HeightMax"].ShouldBe(dtoFields["HeightMax"]);
        spriggitFields["HeightMin"].ShouldBe(dtoFields["HeightMin"]);
        spriggitFields["IsCompressed"].ShouldBe(dtoFields["IsCompressed"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Item[4]"].ShouldBe(dtoFields["Item[4]"]);
        spriggitFields["Level.Level"].ShouldBe(dtoFields["Level.Level"]);
        spriggitFields["Level.MutagenObjectType"].ShouldBe(dtoFields["Level.MutagenObjectType"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["NAM5"].ShouldBe(dtoFields["NAM5"]);
        spriggitFields["Position[0]"].ShouldBe(dtoFields["Position[0]"]);
        spriggitFields["Position[1]"].ShouldBe(dtoFields["Position[1]"]);
        spriggitFields["Position[2]"].ShouldBe(dtoFields["Position[2]"]);
        spriggitFields["Position[3]"].ShouldBe(dtoFields["Position[3]"]);
        spriggitFields["PowerArmorStand"].ShouldBe(dtoFields["PowerArmorStand"]);
        spriggitFields["Race"].ShouldBe(dtoFields["RaceFormKey"]);
        spriggitFields["Responsibility"].ShouldBe(dtoFields["Responsibility"]);
        spriggitFields["Scale[0]"].ShouldBe(dtoFields["Scale[0]"]);
        spriggitFields["Scale[1]"].ShouldBe(dtoFields["Scale[1]"]);
        spriggitFields["Scale[2]"].ShouldBe(dtoFields["Scale[2]"]);
        spriggitFields["Scale[3]"].ShouldBe(dtoFields["Scale[3]"]);
        spriggitFields["SoundLevel"].ShouldBe(dtoFields["SoundLevel"]);
        spriggitFields["TemplateActors.AiDataTemplate"].ShouldBe(dtoFields["TemplateActors.AiDataTemplate"]);
        spriggitFields["TemplateActors.AttackDataTemplate"].ShouldBe(dtoFields["TemplateActors.AttackDataTemplate"]);
        spriggitFields["TemplateActors.BaseDataTemplate"].ShouldBe(dtoFields["TemplateActors.BaseDataTemplate"]);
        spriggitFields["TemplateActors.DefPackListTemplate"].ShouldBe(dtoFields["TemplateActors.DefPackListTemplate"]);
        spriggitFields["TemplateActors.InventoryTemplate"].ShouldBe(dtoFields["TemplateActors.InventoryTemplate"]);
        spriggitFields["TemplateActors.KeywordsTemplate"].ShouldBe(dtoFields["TemplateActors.KeywordsTemplate"]);
        spriggitFields["TemplateActors.SpellListTemplate"].ShouldBe(dtoFields["TemplateActors.SpellListTemplate"]);
        spriggitFields["TemplateActors.StatsTemplate"].ShouldBe(dtoFields["TemplateActors.StatsTemplate"]);
        spriggitFields["TemplateActors.TraitTemplate"].ShouldBe(dtoFields["TemplateActors.TraitTemplate"]);
        spriggitFields["TextureLighting"].ShouldBe(dtoFields["TextureLighting"]);
        spriggitFields["Unknown"].ShouldBe(dtoFields["Unknown"]);
        spriggitFields["Unused"].ShouldBe(dtoFields["Unused"]);
        spriggitFields["UseTemplateActors"].ShouldBe(dtoFields["UseTemplateActors"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Value[2]"].ShouldBe(dtoFields["Value[2]"]);
        spriggitFields["Value[3]"].ShouldBe(dtoFields["Value[3]"]);
        spriggitFields["Value[4]"].ShouldBe(dtoFields["Value[4]"]);
        spriggitFields["Value[5]"].ShouldBe(dtoFields["Value[5]"]);
        spriggitFields["Value[6]"].ShouldBe(dtoFields["Value[6]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Object"]);
        spriggitFields["Voice"].ShouldBe(dtoFields["VoiceFormKey"]);
        spriggitFields["Weight.Fat"].ShouldBe(dtoFields["Weight.Fat"]);
        spriggitFields["Weight.Muscular"].ShouldBe(dtoFields["Weight.Muscular"]);
        spriggitFields["Weight.Thin"].ShouldBe(dtoFields["Weight.Thin"]);
        spriggitFields["XpValueOffset"].ShouldBe(dtoFields["XpValueOffset"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "1D58EA:Fallout4.esm")]
    [Trait("EditorID", "BHExtBOSSoldier_PowerArmorBigGun")]
    [Trait("SpriggitFile", "Npcs/BHExtBOSSoldier_PowerArmorBigGun - 1D58EA_Fallout4.esm.yaml")]
    public void Fallout4_NPC__ShouldMatchSpriggitSample_BHExtBOSSoldier_PowerArmorBigGun()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.NPC,
            "BHExtBOSSoldier_PowerArmorBigGun");
        var dto = Helpers.GetDTO<NPCDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.NPC,
            "1D58EA:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Aggression"].ShouldBe(dtoFields["Aggression"]);
        spriggitFields["Assistance"].ShouldBe(dtoFields["Assistance"]);
        spriggitFields["AttackRace"].ShouldBe(dtoFields["AttackRace"]);
        spriggitFields["CalculatedActionPoints"].ShouldBe(dtoFields["CalculatedActionPoints"]);
        spriggitFields["CalculatedHealth"].ShouldBe(dtoFields["CalculatedHealth"]);
        spriggitFields["Class"].ShouldBe(dtoFields["Class"]);
        spriggitFields["CombatOverridePackageList"].ShouldBe(dtoFields["CombatOverridePackageList"]);
        spriggitFields["CombatStyle"].ShouldBe(dtoFields["CombatStyleFormKey"]);
        spriggitFields["Confidence"].ShouldBe(dtoFields["Confidence"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["CrimeFaction"].ShouldBe(dtoFields["CrimeFactionFormKey"]);
        spriggitFields["DeathItem"].ShouldBe(dtoFields["DeathItem"]);
        spriggitFields["DefaultOutfit"].ShouldBe(dtoFields["DefaultOutfit"]);
        spriggitFields["DefaultPackageList"].ShouldBe(dtoFields["DefaultPackageListFormKey"]);
        spriggitFields["DefaultTemplate"].ShouldBe(dtoFields["DefaultTemplate"]);
        spriggitFields["DispositionBase"].ShouldBe(dtoFields["DispositionBase"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["EnergyLevel"].ShouldBe(dtoFields["EnergyLevel"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["GearedUpWeapons"].ShouldBe(dtoFields["GearedUpWeapons"]);
        spriggitFields["HairColor"].ShouldBe(dtoFields["HairColor"]);
        spriggitFields["HeadTexture"].ShouldBe(dtoFields["HeadTexture"]);
        spriggitFields["HeightMax"].ShouldBe(dtoFields["HeightMax"]);
        spriggitFields["HeightMin"].ShouldBe(dtoFields["HeightMin"]);
        spriggitFields["IsCompressed"].ShouldBe(dtoFields["IsCompressed"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Item[4]"].ShouldBe(dtoFields["Item[4]"]);
        spriggitFields["Level.Level"].ShouldBe(dtoFields["Level.Level"]);
        spriggitFields["Level.MutagenObjectType"].ShouldBe(dtoFields["Level.MutagenObjectType"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["NAM5"].ShouldBe(dtoFields["NAM5"]);
        spriggitFields["Position[0]"].ShouldBe(dtoFields["Position[0]"]);
        spriggitFields["Position[1]"].ShouldBe(dtoFields["Position[1]"]);
        spriggitFields["Position[10]"].ShouldBe(dtoFields["Position[10]"]);
        spriggitFields["Position[11]"].ShouldBe(dtoFields["Position[11]"]);
        spriggitFields["Position[12]"].ShouldBe(dtoFields["Position[12]"]);
        spriggitFields["Position[2]"].ShouldBe(dtoFields["Position[2]"]);
        spriggitFields["Position[3]"].ShouldBe(dtoFields["Position[3]"]);
        spriggitFields["Position[4]"].ShouldBe(dtoFields["Position[4]"]);
        spriggitFields["Position[5]"].ShouldBe(dtoFields["Position[5]"]);
        spriggitFields["Position[6]"].ShouldBe(dtoFields["Position[6]"]);
        spriggitFields["Position[7]"].ShouldBe(dtoFields["Position[7]"]);
        spriggitFields["Position[8]"].ShouldBe(dtoFields["Position[8]"]);
        spriggitFields["Position[9]"].ShouldBe(dtoFields["Position[9]"]);
        spriggitFields["PowerArmorStand"].ShouldBe(dtoFields["PowerArmorStand"]);
        spriggitFields["Race"].ShouldBe(dtoFields["RaceFormKey"]);
        spriggitFields["Responsibility"].ShouldBe(dtoFields["Responsibility"]);
        spriggitFields["Rotation[0]"].ShouldBe(dtoFields["Rotation[0]"]);
        spriggitFields["Rotation[1]"].ShouldBe(dtoFields["Rotation[1]"]);
        spriggitFields["Scale[0]"].ShouldBe(dtoFields["Scale[0]"]);
        spriggitFields["Scale[1]"].ShouldBe(dtoFields["Scale[1]"]);
        spriggitFields["Scale[2]"].ShouldBe(dtoFields["Scale[2]"]);
        spriggitFields["Scale[3]"].ShouldBe(dtoFields["Scale[3]"]);
        spriggitFields["Scale[4]"].ShouldBe(dtoFields["Scale[4]"]);
        spriggitFields["Scale[5]"].ShouldBe(dtoFields["Scale[5]"]);
        spriggitFields["SoundLevel"].ShouldBe(dtoFields["SoundLevel"]);
        spriggitFields["TemplateActors.AiDataTemplate"].ShouldBe(dtoFields["TemplateActors.AiDataTemplate"]);
        spriggitFields["TemplateActors.AttackDataTemplate"].ShouldBe(dtoFields["TemplateActors.AttackDataTemplate"]);
        spriggitFields["TemplateActors.BaseDataTemplate"].ShouldBe(dtoFields["TemplateActors.BaseDataTemplate"]);
        spriggitFields["TemplateActors.DefPackListTemplate"].ShouldBe(dtoFields["TemplateActors.DefPackListTemplate"]);
        spriggitFields["TemplateActors.InventoryTemplate"].ShouldBe(dtoFields["TemplateActors.InventoryTemplate"]);
        spriggitFields["TemplateActors.KeywordsTemplate"].ShouldBe(dtoFields["TemplateActors.KeywordsTemplate"]);
        spriggitFields["TemplateActors.SpellListTemplate"].ShouldBe(dtoFields["TemplateActors.SpellListTemplate"]);
        spriggitFields["TemplateActors.StatsTemplate"].ShouldBe(dtoFields["TemplateActors.StatsTemplate"]);
        spriggitFields["TemplateActors.TraitTemplate"].ShouldBe(dtoFields["TemplateActors.TraitTemplate"]);
        spriggitFields["TextureLighting"].ShouldBe(dtoFields["TextureLighting"]);
        spriggitFields["Unknown"].ShouldBe(dtoFields["Unknown"]);
        spriggitFields["Unused"].ShouldBe(dtoFields["Unused"]);
        spriggitFields["UseTemplateActors"].ShouldBe(dtoFields["UseTemplateActors"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Value[10]"].ShouldBe(dtoFields["Value[10]"]);
        spriggitFields["Value[11]"].ShouldBe(dtoFields["Value[11]"]);
        spriggitFields["Value[12]"].ShouldBe(dtoFields["Value[12]"]);
        spriggitFields["Value[13]"].ShouldBe(dtoFields["Value[13]"]);
        spriggitFields["Value[2]"].ShouldBe(dtoFields["Value[2]"]);
        spriggitFields["Value[3]"].ShouldBe(dtoFields["Value[3]"]);
        spriggitFields["Value[4]"].ShouldBe(dtoFields["Value[4]"]);
        spriggitFields["Value[5]"].ShouldBe(dtoFields["Value[5]"]);
        spriggitFields["Value[6]"].ShouldBe(dtoFields["Value[6]"]);
        spriggitFields["Value[7]"].ShouldBe(dtoFields["Value[7]"]);
        spriggitFields["Value[8]"].ShouldBe(dtoFields["Value[8]"]);
        spriggitFields["Value[9]"].ShouldBe(dtoFields["Value[9]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Object"]);
        spriggitFields["Voice"].ShouldBe(dtoFields["VoiceFormKey"]);
        spriggitFields["Weight.Fat"].ShouldBe(dtoFields["Weight.Fat"]);
        spriggitFields["Weight.Muscular"].ShouldBe(dtoFields["Weight.Muscular"]);
        spriggitFields["Weight.Thin"].ShouldBe(dtoFields["Weight.Thin"]);
        spriggitFields["XpValueOffset"].ShouldBe(dtoFields["XpValueOffset"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "05E557:Fallout4.esm")]
    [Trait("EditorID", "AllieFilmore")]
    [Trait("SpriggitFile", "Npcs/AllieFilmore - 05E557_Fallout4.esm.yaml")]
    public void Fallout4_NPC__ShouldMatchSpriggitSample_AllieFilmore()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.NPC,
            "AllieFilmore");
        var dto = Helpers.GetDTO<NPCDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.NPC,
            "05E557:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Aggression"].ShouldBe(dtoFields["Aggression"]);
        spriggitFields["Assistance"].ShouldBe(dtoFields["Assistance"]);
        spriggitFields["CalculatedActionPoints"].ShouldBe(dtoFields["CalculatedActionPoints"]);
        spriggitFields["CalculatedHealth"].ShouldBe(dtoFields["CalculatedHealth"]);
        spriggitFields["Class"].ShouldBe(dtoFields["Class"]);
        spriggitFields["Color[0]"].ShouldBe(dtoFields["Color[0]"]);
        spriggitFields["Color[1]"].ShouldBe(dtoFields["Color[1]"]);
        spriggitFields["Color[2]"].ShouldBe(dtoFields["Color[2]"]);
        spriggitFields["Color[3]"].ShouldBe(dtoFields["Color[3]"]);
        spriggitFields["Color[4]"].ShouldBe(dtoFields["Color[4]"]);
        spriggitFields["CombatStyle"].ShouldBe(dtoFields["CombatStyleFormKey"]);
        spriggitFields["Confidence"].ShouldBe(dtoFields["Confidence"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["CrimeFaction"].ShouldBe(dtoFields["CrimeFactionFormKey"]);
        spriggitFields["DefaultOutfit"].ShouldBe(dtoFields["DefaultOutfit"]);
        spriggitFields["DefaultTemplate"].ShouldBe(dtoFields["DefaultTemplate"]);
        spriggitFields["DispositionBase"].ShouldBe(dtoFields["DispositionBase"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["EnergyLevel"].ShouldBe(dtoFields["EnergyLevel"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["GearedUpWeapons"].ShouldBe(dtoFields["GearedUpWeapons"]);
        spriggitFields["HairColor"].ShouldBe(dtoFields["HairColor"]);
        spriggitFields["HeightMax"].ShouldBe(dtoFields["HeightMax"]);
        spriggitFields["HeightMin"].ShouldBe(dtoFields["HeightMin"]);
        spriggitFields["Index[0]"].ShouldBe(dtoFields["Index[0]"]);
        spriggitFields["Index[1]"].ShouldBe(dtoFields["Index[1]"]);
        spriggitFields["Index[2]"].ShouldBe(dtoFields["Index[2]"]);
        spriggitFields["Index[3]"].ShouldBe(dtoFields["Index[3]"]);
        spriggitFields["Index[4]"].ShouldBe(dtoFields["Index[4]"]);
        spriggitFields["Index[5]"].ShouldBe(dtoFields["Index[5]"]);
        spriggitFields["Index[6]"].ShouldBe(dtoFields["Index[6]"]);
        spriggitFields["IsCompressed"].ShouldBe(dtoFields["IsCompressed"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Level.Level"].ShouldBe(dtoFields["Level.Level"]);
        spriggitFields["Level.MutagenObjectType"].ShouldBe(dtoFields["Level.MutagenObjectType"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["NAM5"].ShouldBe(dtoFields["NAM5"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Name[3].Language"].ShouldBe(dtoFields["Name[3].Language"]);
        spriggitFields["Name[3].String"].ShouldBe(dtoFields["Name[3].String"]);
        spriggitFields["Name[4].Language"].ShouldBe(dtoFields["Name[4].Language"]);
        spriggitFields["Name[4].String"].ShouldBe(dtoFields["Name[4].String"]);
        spriggitFields["Name[5].Language"].ShouldBe(dtoFields["Name[5].Language"]);
        spriggitFields["Name[5].String"].ShouldBe(dtoFields["Name[5].String"]);
        spriggitFields["Name[6].Language"].ShouldBe(dtoFields["Name[6].Language"]);
        spriggitFields["Name[6].String"].ShouldBe(dtoFields["Name[6].String"]);
        spriggitFields["Name[7].Language"].ShouldBe(dtoFields["Name[7].Language"]);
        spriggitFields["Name[7].String"].ShouldBe(dtoFields["Name[7].String"]);
        spriggitFields["Name[8].Language"].ShouldBe(dtoFields["Name[8].Language"]);
        spriggitFields["Name[8].String"].ShouldBe(dtoFields["Name[8].String"]);
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["Position[0]"].ShouldBe(dtoFields["Position[0]"]);
        spriggitFields["Position[1]"].ShouldBe(dtoFields["Position[1]"]);
        spriggitFields["Position[10]"].ShouldBe(dtoFields["Position[10]"]);
        spriggitFields["Position[11]"].ShouldBe(dtoFields["Position[11]"]);
        spriggitFields["Position[12]"].ShouldBe(dtoFields["Position[12]"]);
        spriggitFields["Position[13]"].ShouldBe(dtoFields["Position[13]"]);
        spriggitFields["Position[14]"].ShouldBe(dtoFields["Position[14]"]);
        spriggitFields["Position[15]"].ShouldBe(dtoFields["Position[15]"]);
        spriggitFields["Position[16]"].ShouldBe(dtoFields["Position[16]"]);
        spriggitFields["Position[2]"].ShouldBe(dtoFields["Position[2]"]);
        spriggitFields["Position[3]"].ShouldBe(dtoFields["Position[3]"]);
        spriggitFields["Position[4]"].ShouldBe(dtoFields["Position[4]"]);
        spriggitFields["Position[5]"].ShouldBe(dtoFields["Position[5]"]);
        spriggitFields["Position[6]"].ShouldBe(dtoFields["Position[6]"]);
        spriggitFields["Position[7]"].ShouldBe(dtoFields["Position[7]"]);
        spriggitFields["Position[8]"].ShouldBe(dtoFields["Position[8]"]);
        spriggitFields["Position[9]"].ShouldBe(dtoFields["Position[9]"]);
        spriggitFields["Race"].ShouldBe(dtoFields["RaceFormKey"]);
        spriggitFields["Responsibility"].ShouldBe(dtoFields["Responsibility"]);
        spriggitFields["Rotation[0]"].ShouldBe(dtoFields["Rotation[0]"]);
        spriggitFields["Rotation[1]"].ShouldBe(dtoFields["Rotation[1]"]);
        spriggitFields["Scale[0]"].ShouldBe(dtoFields["Scale[0]"]);
        spriggitFields["Scale[1]"].ShouldBe(dtoFields["Scale[1]"]);
        spriggitFields["Scale[2]"].ShouldBe(dtoFields["Scale[2]"]);
        spriggitFields["Scale[3]"].ShouldBe(dtoFields["Scale[3]"]);
        spriggitFields["Scale[4]"].ShouldBe(dtoFields["Scale[4]"]);
        spriggitFields["Scale[5]"].ShouldBe(dtoFields["Scale[5]"]);
        spriggitFields["Scale[6]"].ShouldBe(dtoFields["Scale[6]"]);
        spriggitFields["Scale[7]"].ShouldBe(dtoFields["Scale[7]"]);
        spriggitFields["Scale[8]"].ShouldBe(dtoFields["Scale[8]"]);
        spriggitFields["ShortName.Count"].ShouldBe(dtoFields["ShortName.Count"]);
        spriggitFields["ShortName.TargetLanguage"].ShouldBe(dtoFields["ShortName.TargetLanguage"]);
        spriggitFields["ShortName[0].Language"].ShouldBe(dtoFields["ShortName[0].Language"]);
        spriggitFields["ShortName[0].String"].ShouldBe(dtoFields["ShortName[0].String"]);
        spriggitFields["ShortName[1].Language"].ShouldBe(dtoFields["ShortName[1].Language"]);
        spriggitFields["ShortName[1].String"].ShouldBe(dtoFields["ShortName[1].String"]);
        spriggitFields["ShortName[10].Language"].ShouldBe(dtoFields["ShortName[10].Language"]);
        spriggitFields["ShortName[10].String"].ShouldBe(dtoFields["ShortName[10].String"]);
        spriggitFields["ShortName[2].Language"].ShouldBe(dtoFields["ShortName[2].Language"]);
        spriggitFields["ShortName[2].String"].ShouldBe(dtoFields["ShortName[2].String"]);
        spriggitFields["ShortName[3].Language"].ShouldBe(dtoFields["ShortName[3].Language"]);
        spriggitFields["ShortName[3].String"].ShouldBe(dtoFields["ShortName[3].String"]);
        spriggitFields["ShortName[4].Language"].ShouldBe(dtoFields["ShortName[4].Language"]);
        spriggitFields["ShortName[4].String"].ShouldBe(dtoFields["ShortName[4].String"]);
        spriggitFields["ShortName[5].Language"].ShouldBe(dtoFields["ShortName[5].Language"]);
        spriggitFields["ShortName[5].String"].ShouldBe(dtoFields["ShortName[5].String"]);
        spriggitFields["ShortName[6].Language"].ShouldBe(dtoFields["ShortName[6].Language"]);
        spriggitFields["ShortName[6].String"].ShouldBe(dtoFields["ShortName[6].String"]);
        spriggitFields["ShortName[7].Language"].ShouldBe(dtoFields["ShortName[7].Language"]);
        spriggitFields["ShortName[7].String"].ShouldBe(dtoFields["ShortName[7].String"]);
        spriggitFields["ShortName[8].Language"].ShouldBe(dtoFields["ShortName[8].Language"]);
        spriggitFields["ShortName[8].String"].ShouldBe(dtoFields["ShortName[8].String"]);
        spriggitFields["ShortName[9].Language"].ShouldBe(dtoFields["ShortName[9].Language"]);
        spriggitFields["ShortName[9].String"].ShouldBe(dtoFields["ShortName[9].String"]);
        spriggitFields["SoundLevel"].ShouldBe(dtoFields["SoundLevel"]);
        spriggitFields["TemplateActors.StatsTemplate"].ShouldBe(dtoFields["TemplateActors.StatsTemplate"]);
        spriggitFields["TemplateColorIndex[0]"].ShouldBe(dtoFields["TemplateColorIndex[0]"]);
        spriggitFields["TemplateColorIndex[1]"].ShouldBe(dtoFields["TemplateColorIndex[1]"]);
        spriggitFields["TemplateColorIndex[2]"].ShouldBe(dtoFields["TemplateColorIndex[2]"]);
        spriggitFields["TemplateColorIndex[3]"].ShouldBe(dtoFields["TemplateColorIndex[3]"]);
        spriggitFields["TemplateColorIndex[4]"].ShouldBe(dtoFields["TemplateColorIndex[4]"]);
        spriggitFields["TextureLighting"].ShouldBe(dtoFields["TextureLighting"]);
        spriggitFields["Unknown"].ShouldBe(dtoFields["Unknown"]);
        spriggitFields["UseTemplateActors"].ShouldBe(dtoFields["UseTemplateActors"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Value[10]"].ShouldBe(dtoFields["Value[10]"]);
        spriggitFields["Value[11]"].ShouldBe(dtoFields["Value[11]"]);
        spriggitFields["Value[12]"].ShouldBe(dtoFields["Value[12]"]);
        spriggitFields["Value[13]"].ShouldBe(dtoFields["Value[13]"]);
        spriggitFields["Value[14]"].ShouldBe(dtoFields["Value[14]"]);
        spriggitFields["Value[15]"].ShouldBe(dtoFields["Value[15]"]);
        spriggitFields["Value[2]"].ShouldBe(dtoFields["Value[2]"]);
        spriggitFields["Value[3]"].ShouldBe(dtoFields["Value[3]"]);
        spriggitFields["Value[4]"].ShouldBe(dtoFields["Value[4]"]);
        spriggitFields["Value[5]"].ShouldBe(dtoFields["Value[5]"]);
        spriggitFields["Value[6]"].ShouldBe(dtoFields["Value[6]"]);
        spriggitFields["Value[7]"].ShouldBe(dtoFields["Value[7]"]);
        spriggitFields["Value[8]"].ShouldBe(dtoFields["Value[8]"]);
        spriggitFields["Value[9]"].ShouldBe(dtoFields["Value[9]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["Voice"].ShouldBe(dtoFields["VoiceFormKey"]);
        spriggitFields["Weight.Fat"].ShouldBe(dtoFields["Weight.Fat"]);
        spriggitFields["Weight.Muscular"].ShouldBe(dtoFields["Weight.Muscular"]);
        spriggitFields["Weight.Thin"].ShouldBe(dtoFields["Weight.Thin"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "240C21:Fallout4.esm")]
    [Trait("EditorID", "AudioTemplateSynthGen1")]
    [Trait("SpriggitFile", "Npcs/AudioTemplateSynthGen1 - 240C21_Fallout4.esm.yaml")]
    public void Fallout4_NPC__ShouldMatchSpriggitSample_AudioTemplateSynthGen1()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.NPC,
            "AudioTemplateSynthGen1");
        var dto = Helpers.GetDTO<NPCDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.NPC,
            "240C21:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Aggression"].ShouldBe(dtoFields["Aggression"]);
        spriggitFields["Assistance"].ShouldBe(dtoFields["Assistance"]);
        spriggitFields["AttackRace"].ShouldBe(dtoFields["AttackRace"]);
        spriggitFields["CalculatedActionPoints"].ShouldBe(dtoFields["CalculatedActionPoints"]);
        spriggitFields["CalculatedHealth"].ShouldBe(dtoFields["CalculatedHealth"]);
        spriggitFields["Class"].ShouldBe(dtoFields["Class"]);
        spriggitFields["CombatOverridePackageList"].ShouldBe(dtoFields["CombatOverridePackageList"]);
        spriggitFields["CombatStyle"].ShouldBe(dtoFields["CombatStyleFormKey"]);
        spriggitFields["Confidence"].ShouldBe(dtoFields["Confidence"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["Count[5]"].ShouldBe(dtoFields["Count[5]"]);
        spriggitFields["DefaultOutfit"].ShouldBe(dtoFields["DefaultOutfit"]);
        spriggitFields["DefaultPackageList"].ShouldBe(dtoFields["DefaultPackageListFormKey"]);
        spriggitFields["DispositionBase"].ShouldBe(dtoFields["DispositionBase"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["EnergyLevel"].ShouldBe(dtoFields["EnergyLevel"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["GearedUpWeapons"].ShouldBe(dtoFields["GearedUpWeapons"]);
        spriggitFields["HeightMax"].ShouldBe(dtoFields["HeightMax"]);
        spriggitFields["HeightMin"].ShouldBe(dtoFields["HeightMin"]);
        spriggitFields["IsCompressed"].ShouldBe(dtoFields["IsCompressed"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Item[4]"].ShouldBe(dtoFields["Item[4]"]);
        spriggitFields["Item[5]"].ShouldBe(dtoFields["Item[5]"]);
        spriggitFields["Level.Level"].ShouldBe(dtoFields["Level.Level"]);
        spriggitFields["Level.MutagenObjectType"].ShouldBe(dtoFields["Level.MutagenObjectType"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["NAM5"].ShouldBe(dtoFields["NAM5"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Name[3].Language"].ShouldBe(dtoFields["Name[3].Language"]);
        spriggitFields["Name[3].String"].ShouldBe(dtoFields["Name[3].String"]);
        spriggitFields["Name[4].Language"].ShouldBe(dtoFields["Name[4].Language"]);
        spriggitFields["Name[4].String"].ShouldBe(dtoFields["Name[4].String"]);
        spriggitFields["Name[5].Language"].ShouldBe(dtoFields["Name[5].Language"]);
        spriggitFields["Name[5].String"].ShouldBe(dtoFields["Name[5].String"]);
        spriggitFields["Name[6].Language"].ShouldBe(dtoFields["Name[6].Language"]);
        spriggitFields["Name[6].String"].ShouldBe(dtoFields["Name[6].String"]);
        spriggitFields["Name[7].Language"].ShouldBe(dtoFields["Name[7].Language"]);
        spriggitFields["Name[7].String"].ShouldBe(dtoFields["Name[7].String"]);
        spriggitFields["Name[8].Language"].ShouldBe(dtoFields["Name[8].Language"]);
        spriggitFields["Name[8].String"].ShouldBe(dtoFields["Name[8].String"]);
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["Race"].ShouldBe(dtoFields["RaceFormKey"]);
        spriggitFields["Rank"].ShouldBe(dtoFields["Rank"]);
        spriggitFields["Responsibility"].ShouldBe(dtoFields["Responsibility"]);
        spriggitFields["Skin"].ShouldBe(dtoFields["Skin"]);
        spriggitFields["SoundLevel"].ShouldBe(dtoFields["SoundLevel"]);
        spriggitFields["TextureLighting"].ShouldBe(dtoFields["TextureLighting"]);
        spriggitFields["Unknown"].ShouldBe(dtoFields["Unknown"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Value[2]"].ShouldBe(dtoFields["Value[2]"]);
        spriggitFields["Value[3]"].ShouldBe(dtoFields["Value[3]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["Voice"].ShouldBe(dtoFields["VoiceFormKey"]);
        spriggitFields["Weight.Fat"].ShouldBe(dtoFields["Weight.Fat"]);
        spriggitFields["Weight.Muscular"].ShouldBe(dtoFields["Weight.Muscular"]);
        spriggitFields["Weight.Thin"].ShouldBe(dtoFields["Weight.Thin"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
