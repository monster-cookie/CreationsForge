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

        Helpers.GetSpriggitField(spriggit, "Aggression").ShouldBe(Helpers.GetDTOField(dto, "Aggression"));
        Helpers.GetSpriggitField(spriggit, "Assistance").ShouldBe(Helpers.GetDTOField(dto, "Assistance"));
        Helpers.GetSpriggitField(spriggit, "AttackRace").ShouldBe(Helpers.GetDTOField(dto, "AttackRace"));
        Helpers.GetSpriggitField(spriggit, "CalculatedActionPoints").ShouldBe(Helpers.GetDTOField(dto, "CalculatedActionPoints"));
        Helpers.GetSpriggitField(spriggit, "CalculatedHealth").ShouldBe(Helpers.GetDTOField(dto, "CalculatedHealth"));
        Helpers.GetSpriggitField(spriggit, "Class").ShouldBe(Helpers.GetDTOField(dto, "Class"));
        Helpers.GetSpriggitField(spriggit, "CombatOverridePackageList").ShouldBe(Helpers.GetDTOField(dto, "CombatOverridePackageList"));
        Helpers.GetSpriggitField(spriggit, "CombatStyle").ShouldBe(Helpers.GetDTOField(dto, "CombatStyleFormKey"));
        Helpers.GetSpriggitField(spriggit, "Confidence").ShouldBe(Helpers.GetDTOField(dto, "Confidence"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "CrimeFaction").ShouldBe(Helpers.GetDTOField(dto, "CrimeFactionFormKey"));
        Helpers.GetSpriggitField(spriggit, "DeathItem").ShouldBe(Helpers.GetDTOField(dto, "DeathItem"));
        Helpers.GetSpriggitField(spriggit, "DefaultOutfit").ShouldBe(Helpers.GetDTOField(dto, "DefaultOutfit"));
        Helpers.GetSpriggitField(spriggit, "DefaultPackageList").ShouldBe(Helpers.GetDTOField(dto, "DefaultPackageListFormKey"));
        Helpers.GetSpriggitField(spriggit, "DefaultTemplate").ShouldBe(Helpers.GetDTOField(dto, "DefaultTemplate"));
        Helpers.GetSpriggitField(spriggit, "DispositionBase").ShouldBe(Helpers.GetDTOField(dto, "DispositionBase"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "EnergyLevel").ShouldBe(Helpers.GetDTOField(dto, "EnergyLevel"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "GearedUpWeapons").ShouldBe(Helpers.GetDTOField(dto, "GearedUpWeapons"));
        Helpers.GetSpriggitField(spriggit, "HairColor").ShouldBe(Helpers.GetDTOField(dto, "HairColor"));
        Helpers.GetSpriggitField(spriggit, "HeightMax").ShouldBe(Helpers.GetDTOField(dto, "HeightMax"));
        Helpers.GetSpriggitField(spriggit, "HeightMin").ShouldBe(Helpers.GetDTOField(dto, "HeightMin"));
        Helpers.GetSpriggitField(spriggit, "IsCompressed").ShouldBe(Helpers.GetDTOField(dto, "IsCompressed"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Item[4]").ShouldBe(Helpers.GetDTOField(dto, "Item[4]"));
        Helpers.GetSpriggitField(spriggit, "Level.Level").ShouldBe(Helpers.GetDTOField(dto, "Level.Level"));
        Helpers.GetSpriggitField(spriggit, "Level.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Level.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "NAM5").ShouldBe(Helpers.GetDTOField(dto, "NAM5"));
        Helpers.GetSpriggitField(spriggit, "Race").ShouldBe(Helpers.GetDTOField(dto, "RaceFormKey"));
        Helpers.GetSpriggitField(spriggit, "Responsibility").ShouldBe(Helpers.GetDTOField(dto, "Responsibility"));
        Helpers.GetSpriggitField(spriggit, "SoundLevel").ShouldBe(Helpers.GetDTOField(dto, "SoundLevel"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.AiDataTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.AiDataTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.AttackDataTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.AttackDataTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.BaseDataTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.BaseDataTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.DefPackListTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.DefPackListTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.InventoryTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.InventoryTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.KeywordsTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.KeywordsTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.SpellListTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.SpellListTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.StatsTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.StatsTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.TraitTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.TraitTemplate"));
        Helpers.GetSpriggitField(spriggit, "TextureLighting").ShouldBe(Helpers.GetDTOField(dto, "TextureLighting"));
        Helpers.GetSpriggitField(spriggit, "Unused").ShouldBe(Helpers.GetDTOField(dto, "Unused"));
        Helpers.GetSpriggitField(spriggit, "UseTemplateActors").ShouldBe(Helpers.GetDTOField(dto, "UseTemplateActors"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Value[2]").ShouldBe(Helpers.GetDTOField(dto, "Value[2]"));
        Helpers.GetSpriggitField(spriggit, "Value[3]").ShouldBe(Helpers.GetDTOField(dto, "Value[3]"));
        Helpers.GetSpriggitField(spriggit, "Value[4]").ShouldBe(Helpers.GetDTOField(dto, "Value[4]"));
        Helpers.GetSpriggitField(spriggit, "Value[5]").ShouldBe(Helpers.GetDTOField(dto, "Value[5]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Object"));
        Helpers.GetSpriggitField(spriggit, "Voice").ShouldBe(Helpers.GetDTOField(dto, "VoiceFormKey"));
        Helpers.GetSpriggitField(spriggit, "Weight.Fat").ShouldBe(Helpers.GetDTOField(dto, "Weight.Fat"));
        Helpers.GetSpriggitField(spriggit, "Weight.Muscular").ShouldBe(Helpers.GetDTOField(dto, "Weight.Muscular"));
        Helpers.GetSpriggitField(spriggit, "Weight.Thin").ShouldBe(Helpers.GetDTOField(dto, "Weight.Thin"));
        Helpers.GetSpriggitField(spriggit, "XpValueOffset").ShouldBe(Helpers.GetDTOField(dto, "XpValueOffset"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Aggression", "Assistance", "AttackRace", "CalculatedActionPoints", "CalculatedHealth", "Class", "CombatOverridePackageList", "CombatStyle", "Confidence", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "CrimeFaction", "DeathItem", "DefaultOutfit", "DefaultPackageList", "DefaultTemplate", "DispositionBase", "EditorID", "EnergyLevel", "FormKey", "GearedUpWeapons", "HairColor", "HeightMax", "HeightMin", "IsCompressed", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Item[4]", "Level.Level", "Level.MutagenObjectType", "MajorRecordFlagsRaw", "NAM5", "Race", "Responsibility", "SoundLevel", "TemplateActors.AiDataTemplate", "TemplateActors.AttackDataTemplate", "TemplateActors.BaseDataTemplate", "TemplateActors.DefPackListTemplate", "TemplateActors.InventoryTemplate", "TemplateActors.KeywordsTemplate", "TemplateActors.SpellListTemplate", "TemplateActors.StatsTemplate", "TemplateActors.TraitTemplate", "TextureLighting", "Unused", "UseTemplateActors", "Value[0]", "Value[1]", "Value[2]", "Value[3]", "Value[4]", "Value[5]", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "Voice", "Weight.Fat", "Weight.Muscular", "Weight.Thin", "XpValueOffset");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Aggression", "Assistance", "AttackRace", "CalculatedActionPoints", "CalculatedHealth", "Class", "CombatOverridePackageList", "CombatStyleFormKey", "Confidence", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "CrimeFactionFormKey", "DeathItem", "DefaultOutfit", "DefaultPackageListFormKey", "DefaultTemplate", "DispositionBase", "EditorID", "EnergyLevel", "FormKey", "GearedUpWeapons", "HairColor", "HeightMax", "HeightMin", "IsCompressed", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Item[4]", "Level.Level", "Level.MutagenObjectType", "MajorRecordFlags", "NAM5", "RaceFormKey", "Responsibility", "SoundLevel", "TemplateActors.AiDataTemplate", "TemplateActors.AttackDataTemplate", "TemplateActors.BaseDataTemplate", "TemplateActors.DefPackListTemplate", "TemplateActors.InventoryTemplate", "TemplateActors.KeywordsTemplate", "TemplateActors.SpellListTemplate", "TemplateActors.StatsTemplate", "TemplateActors.TraitTemplate", "TextureLighting", "Unused", "UseTemplateActors", "Value[0]", "Value[1]", "Value[2]", "Value[3]", "Value[4]", "Value[5]", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VoiceFormKey", "Weight.Fat", "Weight.Muscular", "Weight.Thin", "XpValueOffset");
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

        Helpers.GetSpriggitField(spriggit, "Aggression").ShouldBe(Helpers.GetDTOField(dto, "Aggression"));
        Helpers.GetSpriggitField(spriggit, "Assistance").ShouldBe(Helpers.GetDTOField(dto, "Assistance"));
        Helpers.GetSpriggitField(spriggit, "AttackRace").ShouldBe(Helpers.GetDTOField(dto, "AttackRace"));
        Helpers.GetSpriggitField(spriggit, "CalculatedActionPoints").ShouldBe(Helpers.GetDTOField(dto, "CalculatedActionPoints"));
        Helpers.GetSpriggitField(spriggit, "CalculatedHealth").ShouldBe(Helpers.GetDTOField(dto, "CalculatedHealth"));
        Helpers.GetSpriggitField(spriggit, "Class").ShouldBe(Helpers.GetDTOField(dto, "Class"));
        Helpers.GetSpriggitField(spriggit, "CombatOverridePackageList").ShouldBe(Helpers.GetDTOField(dto, "CombatOverridePackageList"));
        Helpers.GetSpriggitField(spriggit, "CombatStyle").ShouldBe(Helpers.GetDTOField(dto, "CombatStyleFormKey"));
        Helpers.GetSpriggitField(spriggit, "Confidence").ShouldBe(Helpers.GetDTOField(dto, "Confidence"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "CrimeFaction").ShouldBe(Helpers.GetDTOField(dto, "CrimeFactionFormKey"));
        Helpers.GetSpriggitField(spriggit, "DeathItem").ShouldBe(Helpers.GetDTOField(dto, "DeathItem"));
        Helpers.GetSpriggitField(spriggit, "DefaultOutfit").ShouldBe(Helpers.GetDTOField(dto, "DefaultOutfit"));
        Helpers.GetSpriggitField(spriggit, "DefaultPackageList").ShouldBe(Helpers.GetDTOField(dto, "DefaultPackageListFormKey"));
        Helpers.GetSpriggitField(spriggit, "DefaultTemplate").ShouldBe(Helpers.GetDTOField(dto, "DefaultTemplate"));
        Helpers.GetSpriggitField(spriggit, "DispositionBase").ShouldBe(Helpers.GetDTOField(dto, "DispositionBase"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "EnergyLevel").ShouldBe(Helpers.GetDTOField(dto, "EnergyLevel"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "GearedUpWeapons").ShouldBe(Helpers.GetDTOField(dto, "GearedUpWeapons"));
        Helpers.GetSpriggitField(spriggit, "HairColor").ShouldBe(Helpers.GetDTOField(dto, "HairColor"));
        Helpers.GetSpriggitField(spriggit, "HeightMax").ShouldBe(Helpers.GetDTOField(dto, "HeightMax"));
        Helpers.GetSpriggitField(spriggit, "HeightMin").ShouldBe(Helpers.GetDTOField(dto, "HeightMin"));
        Helpers.GetSpriggitField(spriggit, "IsCompressed").ShouldBe(Helpers.GetDTOField(dto, "IsCompressed"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Item[4]").ShouldBe(Helpers.GetDTOField(dto, "Item[4]"));
        Helpers.GetSpriggitField(spriggit, "Level.Level").ShouldBe(Helpers.GetDTOField(dto, "Level.Level"));
        Helpers.GetSpriggitField(spriggit, "Level.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Level.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "NAM5").ShouldBe(Helpers.GetDTOField(dto, "NAM5"));
        Helpers.GetSpriggitField(spriggit, "Position[0]").ShouldBe(Helpers.GetDTOField(dto, "Position[0]"));
        Helpers.GetSpriggitField(spriggit, "Position[1]").ShouldBe(Helpers.GetDTOField(dto, "Position[1]"));
        Helpers.GetSpriggitField(spriggit, "Position[2]").ShouldBe(Helpers.GetDTOField(dto, "Position[2]"));
        Helpers.GetSpriggitField(spriggit, "Position[3]").ShouldBe(Helpers.GetDTOField(dto, "Position[3]"));
        Helpers.GetSpriggitField(spriggit, "PowerArmorStand").ShouldBe(Helpers.GetDTOField(dto, "PowerArmorStand"));
        Helpers.GetSpriggitField(spriggit, "Race").ShouldBe(Helpers.GetDTOField(dto, "RaceFormKey"));
        Helpers.GetSpriggitField(spriggit, "Responsibility").ShouldBe(Helpers.GetDTOField(dto, "Responsibility"));
        Helpers.GetSpriggitField(spriggit, "Scale[0]").ShouldBe(Helpers.GetDTOField(dto, "Scale[0]"));
        Helpers.GetSpriggitField(spriggit, "Scale[1]").ShouldBe(Helpers.GetDTOField(dto, "Scale[1]"));
        Helpers.GetSpriggitField(spriggit, "Scale[2]").ShouldBe(Helpers.GetDTOField(dto, "Scale[2]"));
        Helpers.GetSpriggitField(spriggit, "Scale[3]").ShouldBe(Helpers.GetDTOField(dto, "Scale[3]"));
        Helpers.GetSpriggitField(spriggit, "SoundLevel").ShouldBe(Helpers.GetDTOField(dto, "SoundLevel"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.AiDataTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.AiDataTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.AttackDataTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.AttackDataTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.BaseDataTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.BaseDataTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.DefPackListTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.DefPackListTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.InventoryTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.InventoryTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.KeywordsTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.KeywordsTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.SpellListTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.SpellListTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.StatsTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.StatsTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.TraitTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.TraitTemplate"));
        Helpers.GetSpriggitField(spriggit, "TextureLighting").ShouldBe(Helpers.GetDTOField(dto, "TextureLighting"));
        Helpers.GetSpriggitField(spriggit, "Unknown").ShouldBe(Helpers.GetDTOField(dto, "Unknown"));
        Helpers.GetSpriggitField(spriggit, "Unused").ShouldBe(Helpers.GetDTOField(dto, "Unused"));
        Helpers.GetSpriggitField(spriggit, "UseTemplateActors").ShouldBe(Helpers.GetDTOField(dto, "UseTemplateActors"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Value[2]").ShouldBe(Helpers.GetDTOField(dto, "Value[2]"));
        Helpers.GetSpriggitField(spriggit, "Value[3]").ShouldBe(Helpers.GetDTOField(dto, "Value[3]"));
        Helpers.GetSpriggitField(spriggit, "Value[4]").ShouldBe(Helpers.GetDTOField(dto, "Value[4]"));
        Helpers.GetSpriggitField(spriggit, "Value[5]").ShouldBe(Helpers.GetDTOField(dto, "Value[5]"));
        Helpers.GetSpriggitField(spriggit, "Value[6]").ShouldBe(Helpers.GetDTOField(dto, "Value[6]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Object"));
        Helpers.GetSpriggitField(spriggit, "Voice").ShouldBe(Helpers.GetDTOField(dto, "VoiceFormKey"));
        Helpers.GetSpriggitField(spriggit, "Weight.Fat").ShouldBe(Helpers.GetDTOField(dto, "Weight.Fat"));
        Helpers.GetSpriggitField(spriggit, "Weight.Muscular").ShouldBe(Helpers.GetDTOField(dto, "Weight.Muscular"));
        Helpers.GetSpriggitField(spriggit, "Weight.Thin").ShouldBe(Helpers.GetDTOField(dto, "Weight.Thin"));
        Helpers.GetSpriggitField(spriggit, "XpValueOffset").ShouldBe(Helpers.GetDTOField(dto, "XpValueOffset"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Aggression", "Assistance", "AttackRace", "CalculatedActionPoints", "CalculatedHealth", "Class", "CombatOverridePackageList", "CombatStyle", "Confidence", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "CrimeFaction", "DeathItem", "DefaultOutfit", "DefaultPackageList", "DefaultTemplate", "DispositionBase", "EditorID", "EnergyLevel", "FormKey", "GearedUpWeapons", "HairColor", "HeightMax", "HeightMin", "IsCompressed", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Item[4]", "Level.Level", "Level.MutagenObjectType", "MajorRecordFlagsRaw", "NAM5", "Position[0]", "Position[1]", "Position[2]", "Position[3]", "PowerArmorStand", "Race", "Responsibility", "Scale[0]", "Scale[1]", "Scale[2]", "Scale[3]", "SoundLevel", "TemplateActors.AiDataTemplate", "TemplateActors.AttackDataTemplate", "TemplateActors.BaseDataTemplate", "TemplateActors.DefPackListTemplate", "TemplateActors.InventoryTemplate", "TemplateActors.KeywordsTemplate", "TemplateActors.SpellListTemplate", "TemplateActors.StatsTemplate", "TemplateActors.TraitTemplate", "TextureLighting", "Unknown", "Unused", "UseTemplateActors", "Value[0]", "Value[1]", "Value[2]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "Voice", "Weight.Fat", "Weight.Muscular", "Weight.Thin", "XpValueOffset");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Aggression", "Assistance", "AttackRace", "CalculatedActionPoints", "CalculatedHealth", "Class", "CombatOverridePackageList", "CombatStyleFormKey", "Confidence", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "CrimeFactionFormKey", "DeathItem", "DefaultOutfit", "DefaultPackageListFormKey", "DefaultTemplate", "DispositionBase", "EditorID", "EnergyLevel", "FormKey", "GearedUpWeapons", "HairColor", "HeightMax", "HeightMin", "IsCompressed", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Item[4]", "Level.Level", "Level.MutagenObjectType", "MajorRecordFlags", "NAM5", "Position[0]", "Position[1]", "Position[2]", "Position[3]", "PowerArmorStand", "RaceFormKey", "Responsibility", "Scale[0]", "Scale[1]", "Scale[2]", "Scale[3]", "SoundLevel", "TemplateActors.AiDataTemplate", "TemplateActors.AttackDataTemplate", "TemplateActors.BaseDataTemplate", "TemplateActors.DefPackListTemplate", "TemplateActors.InventoryTemplate", "TemplateActors.KeywordsTemplate", "TemplateActors.SpellListTemplate", "TemplateActors.StatsTemplate", "TemplateActors.TraitTemplate", "TextureLighting", "Unknown", "Unused", "UseTemplateActors", "Value[0]", "Value[1]", "Value[2]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VoiceFormKey", "Weight.Fat", "Weight.Muscular", "Weight.Thin", "XpValueOffset");
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

        Helpers.GetSpriggitField(spriggit, "Aggression").ShouldBe(Helpers.GetDTOField(dto, "Aggression"));
        Helpers.GetSpriggitField(spriggit, "Assistance").ShouldBe(Helpers.GetDTOField(dto, "Assistance"));
        Helpers.GetSpriggitField(spriggit, "AttackRace").ShouldBe(Helpers.GetDTOField(dto, "AttackRace"));
        Helpers.GetSpriggitField(spriggit, "CalculatedActionPoints").ShouldBe(Helpers.GetDTOField(dto, "CalculatedActionPoints"));
        Helpers.GetSpriggitField(spriggit, "CalculatedHealth").ShouldBe(Helpers.GetDTOField(dto, "CalculatedHealth"));
        Helpers.GetSpriggitField(spriggit, "Class").ShouldBe(Helpers.GetDTOField(dto, "Class"));
        Helpers.GetSpriggitField(spriggit, "CombatOverridePackageList").ShouldBe(Helpers.GetDTOField(dto, "CombatOverridePackageList"));
        Helpers.GetSpriggitField(spriggit, "CombatStyle").ShouldBe(Helpers.GetDTOField(dto, "CombatStyleFormKey"));
        Helpers.GetSpriggitField(spriggit, "Confidence").ShouldBe(Helpers.GetDTOField(dto, "Confidence"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "CrimeFaction").ShouldBe(Helpers.GetDTOField(dto, "CrimeFactionFormKey"));
        Helpers.GetSpriggitField(spriggit, "DeathItem").ShouldBe(Helpers.GetDTOField(dto, "DeathItem"));
        Helpers.GetSpriggitField(spriggit, "DefaultOutfit").ShouldBe(Helpers.GetDTOField(dto, "DefaultOutfit"));
        Helpers.GetSpriggitField(spriggit, "DefaultPackageList").ShouldBe(Helpers.GetDTOField(dto, "DefaultPackageListFormKey"));
        Helpers.GetSpriggitField(spriggit, "DefaultTemplate").ShouldBe(Helpers.GetDTOField(dto, "DefaultTemplate"));
        Helpers.GetSpriggitField(spriggit, "DispositionBase").ShouldBe(Helpers.GetDTOField(dto, "DispositionBase"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "EnergyLevel").ShouldBe(Helpers.GetDTOField(dto, "EnergyLevel"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "GearedUpWeapons").ShouldBe(Helpers.GetDTOField(dto, "GearedUpWeapons"));
        Helpers.GetSpriggitField(spriggit, "HairColor").ShouldBe(Helpers.GetDTOField(dto, "HairColor"));
        Helpers.GetSpriggitField(spriggit, "HeadTexture").ShouldBe(Helpers.GetDTOField(dto, "HeadTexture"));
        Helpers.GetSpriggitField(spriggit, "HeightMax").ShouldBe(Helpers.GetDTOField(dto, "HeightMax"));
        Helpers.GetSpriggitField(spriggit, "HeightMin").ShouldBe(Helpers.GetDTOField(dto, "HeightMin"));
        Helpers.GetSpriggitField(spriggit, "IsCompressed").ShouldBe(Helpers.GetDTOField(dto, "IsCompressed"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Item[4]").ShouldBe(Helpers.GetDTOField(dto, "Item[4]"));
        Helpers.GetSpriggitField(spriggit, "Level.Level").ShouldBe(Helpers.GetDTOField(dto, "Level.Level"));
        Helpers.GetSpriggitField(spriggit, "Level.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Level.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "NAM5").ShouldBe(Helpers.GetDTOField(dto, "NAM5"));
        Helpers.GetSpriggitField(spriggit, "Position[0]").ShouldBe(Helpers.GetDTOField(dto, "Position[0]"));
        Helpers.GetSpriggitField(spriggit, "Position[1]").ShouldBe(Helpers.GetDTOField(dto, "Position[1]"));
        Helpers.GetSpriggitField(spriggit, "Position[10]").ShouldBe(Helpers.GetDTOField(dto, "Position[10]"));
        Helpers.GetSpriggitField(spriggit, "Position[11]").ShouldBe(Helpers.GetDTOField(dto, "Position[11]"));
        Helpers.GetSpriggitField(spriggit, "Position[12]").ShouldBe(Helpers.GetDTOField(dto, "Position[12]"));
        Helpers.GetSpriggitField(spriggit, "Position[2]").ShouldBe(Helpers.GetDTOField(dto, "Position[2]"));
        Helpers.GetSpriggitField(spriggit, "Position[3]").ShouldBe(Helpers.GetDTOField(dto, "Position[3]"));
        Helpers.GetSpriggitField(spriggit, "Position[4]").ShouldBe(Helpers.GetDTOField(dto, "Position[4]"));
        Helpers.GetSpriggitField(spriggit, "Position[5]").ShouldBe(Helpers.GetDTOField(dto, "Position[5]"));
        Helpers.GetSpriggitField(spriggit, "Position[6]").ShouldBe(Helpers.GetDTOField(dto, "Position[6]"));
        Helpers.GetSpriggitField(spriggit, "Position[7]").ShouldBe(Helpers.GetDTOField(dto, "Position[7]"));
        Helpers.GetSpriggitField(spriggit, "Position[8]").ShouldBe(Helpers.GetDTOField(dto, "Position[8]"));
        Helpers.GetSpriggitField(spriggit, "Position[9]").ShouldBe(Helpers.GetDTOField(dto, "Position[9]"));
        Helpers.GetSpriggitField(spriggit, "PowerArmorStand").ShouldBe(Helpers.GetDTOField(dto, "PowerArmorStand"));
        Helpers.GetSpriggitField(spriggit, "Race").ShouldBe(Helpers.GetDTOField(dto, "RaceFormKey"));
        Helpers.GetSpriggitField(spriggit, "Responsibility").ShouldBe(Helpers.GetDTOField(dto, "Responsibility"));
        Helpers.GetSpriggitField(spriggit, "Rotation[0]").ShouldBe(Helpers.GetDTOField(dto, "Rotation[0]"));
        Helpers.GetSpriggitField(spriggit, "Rotation[1]").ShouldBe(Helpers.GetDTOField(dto, "Rotation[1]"));
        Helpers.GetSpriggitField(spriggit, "Scale[0]").ShouldBe(Helpers.GetDTOField(dto, "Scale[0]"));
        Helpers.GetSpriggitField(spriggit, "Scale[1]").ShouldBe(Helpers.GetDTOField(dto, "Scale[1]"));
        Helpers.GetSpriggitField(spriggit, "Scale[2]").ShouldBe(Helpers.GetDTOField(dto, "Scale[2]"));
        Helpers.GetSpriggitField(spriggit, "Scale[3]").ShouldBe(Helpers.GetDTOField(dto, "Scale[3]"));
        Helpers.GetSpriggitField(spriggit, "Scale[4]").ShouldBe(Helpers.GetDTOField(dto, "Scale[4]"));
        Helpers.GetSpriggitField(spriggit, "Scale[5]").ShouldBe(Helpers.GetDTOField(dto, "Scale[5]"));
        Helpers.GetSpriggitField(spriggit, "SoundLevel").ShouldBe(Helpers.GetDTOField(dto, "SoundLevel"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.AiDataTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.AiDataTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.AttackDataTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.AttackDataTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.BaseDataTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.BaseDataTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.DefPackListTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.DefPackListTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.InventoryTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.InventoryTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.KeywordsTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.KeywordsTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.SpellListTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.SpellListTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.StatsTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.StatsTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.TraitTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.TraitTemplate"));
        Helpers.GetSpriggitField(spriggit, "TextureLighting").ShouldBe(Helpers.GetDTOField(dto, "TextureLighting"));
        Helpers.GetSpriggitField(spriggit, "Unknown").ShouldBe(Helpers.GetDTOField(dto, "Unknown"));
        Helpers.GetSpriggitField(spriggit, "Unused").ShouldBe(Helpers.GetDTOField(dto, "Unused"));
        Helpers.GetSpriggitField(spriggit, "UseTemplateActors").ShouldBe(Helpers.GetDTOField(dto, "UseTemplateActors"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Value[10]").ShouldBe(Helpers.GetDTOField(dto, "Value[10]"));
        Helpers.GetSpriggitField(spriggit, "Value[11]").ShouldBe(Helpers.GetDTOField(dto, "Value[11]"));
        Helpers.GetSpriggitField(spriggit, "Value[12]").ShouldBe(Helpers.GetDTOField(dto, "Value[12]"));
        Helpers.GetSpriggitField(spriggit, "Value[13]").ShouldBe(Helpers.GetDTOField(dto, "Value[13]"));
        Helpers.GetSpriggitField(spriggit, "Value[2]").ShouldBe(Helpers.GetDTOField(dto, "Value[2]"));
        Helpers.GetSpriggitField(spriggit, "Value[3]").ShouldBe(Helpers.GetDTOField(dto, "Value[3]"));
        Helpers.GetSpriggitField(spriggit, "Value[4]").ShouldBe(Helpers.GetDTOField(dto, "Value[4]"));
        Helpers.GetSpriggitField(spriggit, "Value[5]").ShouldBe(Helpers.GetDTOField(dto, "Value[5]"));
        Helpers.GetSpriggitField(spriggit, "Value[6]").ShouldBe(Helpers.GetDTOField(dto, "Value[6]"));
        Helpers.GetSpriggitField(spriggit, "Value[7]").ShouldBe(Helpers.GetDTOField(dto, "Value[7]"));
        Helpers.GetSpriggitField(spriggit, "Value[8]").ShouldBe(Helpers.GetDTOField(dto, "Value[8]"));
        Helpers.GetSpriggitField(spriggit, "Value[9]").ShouldBe(Helpers.GetDTOField(dto, "Value[9]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Object"));
        Helpers.GetSpriggitField(spriggit, "Voice").ShouldBe(Helpers.GetDTOField(dto, "VoiceFormKey"));
        Helpers.GetSpriggitField(spriggit, "Weight.Fat").ShouldBe(Helpers.GetDTOField(dto, "Weight.Fat"));
        Helpers.GetSpriggitField(spriggit, "Weight.Muscular").ShouldBe(Helpers.GetDTOField(dto, "Weight.Muscular"));
        Helpers.GetSpriggitField(spriggit, "Weight.Thin").ShouldBe(Helpers.GetDTOField(dto, "Weight.Thin"));
        Helpers.GetSpriggitField(spriggit, "XpValueOffset").ShouldBe(Helpers.GetDTOField(dto, "XpValueOffset"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Aggression", "Assistance", "AttackRace", "CalculatedActionPoints", "CalculatedHealth", "Class", "CombatOverridePackageList", "CombatStyle", "Confidence", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "CrimeFaction", "DeathItem", "DefaultOutfit", "DefaultPackageList", "DefaultTemplate", "DispositionBase", "EditorID", "EnergyLevel", "FormKey", "GearedUpWeapons", "HairColor", "HeadTexture", "HeightMax", "HeightMin", "IsCompressed", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Item[4]", "Level.Level", "Level.MutagenObjectType", "MajorRecordFlagsRaw", "NAM5", "Position[0]", "Position[1]", "Position[10]", "Position[11]", "Position[12]", "Position[2]", "Position[3]", "Position[4]", "Position[5]", "Position[6]", "Position[7]", "Position[8]", "Position[9]", "PowerArmorStand", "Race", "Responsibility", "Rotation[0]", "Rotation[1]", "Scale[0]", "Scale[1]", "Scale[2]", "Scale[3]", "Scale[4]", "Scale[5]", "SoundLevel", "TemplateActors.AiDataTemplate", "TemplateActors.AttackDataTemplate", "TemplateActors.BaseDataTemplate", "TemplateActors.DefPackListTemplate", "TemplateActors.InventoryTemplate", "TemplateActors.KeywordsTemplate", "TemplateActors.SpellListTemplate", "TemplateActors.StatsTemplate", "TemplateActors.TraitTemplate", "TextureLighting", "Unknown", "Unused", "UseTemplateActors", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[2]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "Voice", "Weight.Fat", "Weight.Muscular", "Weight.Thin", "XpValueOffset");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Aggression", "Assistance", "AttackRace", "CalculatedActionPoints", "CalculatedHealth", "Class", "CombatOverridePackageList", "CombatStyleFormKey", "Confidence", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "CrimeFactionFormKey", "DeathItem", "DefaultOutfit", "DefaultPackageListFormKey", "DefaultTemplate", "DispositionBase", "EditorID", "EnergyLevel", "FormKey", "GearedUpWeapons", "HairColor", "HeadTexture", "HeightMax", "HeightMin", "IsCompressed", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Item[4]", "Level.Level", "Level.MutagenObjectType", "MajorRecordFlags", "NAM5", "Position[0]", "Position[1]", "Position[10]", "Position[11]", "Position[12]", "Position[2]", "Position[3]", "Position[4]", "Position[5]", "Position[6]", "Position[7]", "Position[8]", "Position[9]", "PowerArmorStand", "RaceFormKey", "Responsibility", "Rotation[0]", "Rotation[1]", "Scale[0]", "Scale[1]", "Scale[2]", "Scale[3]", "Scale[4]", "Scale[5]", "SoundLevel", "TemplateActors.AiDataTemplate", "TemplateActors.AttackDataTemplate", "TemplateActors.BaseDataTemplate", "TemplateActors.DefPackListTemplate", "TemplateActors.InventoryTemplate", "TemplateActors.KeywordsTemplate", "TemplateActors.SpellListTemplate", "TemplateActors.StatsTemplate", "TemplateActors.TraitTemplate", "TextureLighting", "Unknown", "Unused", "UseTemplateActors", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[2]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VoiceFormKey", "Weight.Fat", "Weight.Muscular", "Weight.Thin", "XpValueOffset");
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

        Helpers.GetSpriggitField(spriggit, "Aggression").ShouldBe(Helpers.GetDTOField(dto, "Aggression"));
        Helpers.GetSpriggitField(spriggit, "Assistance").ShouldBe(Helpers.GetDTOField(dto, "Assistance"));
        Helpers.GetSpriggitField(spriggit, "CalculatedActionPoints").ShouldBe(Helpers.GetDTOField(dto, "CalculatedActionPoints"));
        Helpers.GetSpriggitField(spriggit, "CalculatedHealth").ShouldBe(Helpers.GetDTOField(dto, "CalculatedHealth"));
        Helpers.GetSpriggitField(spriggit, "Class").ShouldBe(Helpers.GetDTOField(dto, "Class"));
        Helpers.GetSpriggitField(spriggit, "Color[0]").ShouldBe(Helpers.GetDTOField(dto, "Color[0]"));
        Helpers.GetSpriggitField(spriggit, "Color[1]").ShouldBe(Helpers.GetDTOField(dto, "Color[1]"));
        Helpers.GetSpriggitField(spriggit, "Color[2]").ShouldBe(Helpers.GetDTOField(dto, "Color[2]"));
        Helpers.GetSpriggitField(spriggit, "Color[3]").ShouldBe(Helpers.GetDTOField(dto, "Color[3]"));
        Helpers.GetSpriggitField(spriggit, "Color[4]").ShouldBe(Helpers.GetDTOField(dto, "Color[4]"));
        Helpers.GetSpriggitField(spriggit, "CombatStyle").ShouldBe(Helpers.GetDTOField(dto, "CombatStyleFormKey"));
        Helpers.GetSpriggitField(spriggit, "Confidence").ShouldBe(Helpers.GetDTOField(dto, "Confidence"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "CrimeFaction").ShouldBe(Helpers.GetDTOField(dto, "CrimeFactionFormKey"));
        Helpers.GetSpriggitField(spriggit, "DefaultOutfit").ShouldBe(Helpers.GetDTOField(dto, "DefaultOutfit"));
        Helpers.GetSpriggitField(spriggit, "DefaultTemplate").ShouldBe(Helpers.GetDTOField(dto, "DefaultTemplate"));
        Helpers.GetSpriggitField(spriggit, "DispositionBase").ShouldBe(Helpers.GetDTOField(dto, "DispositionBase"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "EnergyLevel").ShouldBe(Helpers.GetDTOField(dto, "EnergyLevel"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "GearedUpWeapons").ShouldBe(Helpers.GetDTOField(dto, "GearedUpWeapons"));
        Helpers.GetSpriggitField(spriggit, "HairColor").ShouldBe(Helpers.GetDTOField(dto, "HairColor"));
        Helpers.GetSpriggitField(spriggit, "HeightMax").ShouldBe(Helpers.GetDTOField(dto, "HeightMax"));
        Helpers.GetSpriggitField(spriggit, "HeightMin").ShouldBe(Helpers.GetDTOField(dto, "HeightMin"));
        Helpers.GetSpriggitField(spriggit, "Index[0]").ShouldBe(Helpers.GetDTOField(dto, "Index[0]"));
        Helpers.GetSpriggitField(spriggit, "Index[1]").ShouldBe(Helpers.GetDTOField(dto, "Index[1]"));
        Helpers.GetSpriggitField(spriggit, "Index[2]").ShouldBe(Helpers.GetDTOField(dto, "Index[2]"));
        Helpers.GetSpriggitField(spriggit, "Index[3]").ShouldBe(Helpers.GetDTOField(dto, "Index[3]"));
        Helpers.GetSpriggitField(spriggit, "Index[4]").ShouldBe(Helpers.GetDTOField(dto, "Index[4]"));
        Helpers.GetSpriggitField(spriggit, "Index[5]").ShouldBe(Helpers.GetDTOField(dto, "Index[5]"));
        Helpers.GetSpriggitField(spriggit, "Index[6]").ShouldBe(Helpers.GetDTOField(dto, "Index[6]"));
        Helpers.GetSpriggitField(spriggit, "IsCompressed").ShouldBe(Helpers.GetDTOField(dto, "IsCompressed"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Level.Level").ShouldBe(Helpers.GetDTOField(dto, "Level.Level"));
        Helpers.GetSpriggitField(spriggit, "Level.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Level.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "NAM5").ShouldBe(Helpers.GetDTOField(dto, "NAM5"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[10].String").ShouldBe(Helpers.GetDTOField(dto, "Name[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Name[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[9].String").ShouldBe(Helpers.GetDTOField(dto, "Name[9].String"));
        Helpers.GetSpriggitField(spriggit, "Position[0]").ShouldBe(Helpers.GetDTOField(dto, "Position[0]"));
        Helpers.GetSpriggitField(spriggit, "Position[1]").ShouldBe(Helpers.GetDTOField(dto, "Position[1]"));
        Helpers.GetSpriggitField(spriggit, "Position[10]").ShouldBe(Helpers.GetDTOField(dto, "Position[10]"));
        Helpers.GetSpriggitField(spriggit, "Position[11]").ShouldBe(Helpers.GetDTOField(dto, "Position[11]"));
        Helpers.GetSpriggitField(spriggit, "Position[12]").ShouldBe(Helpers.GetDTOField(dto, "Position[12]"));
        Helpers.GetSpriggitField(spriggit, "Position[13]").ShouldBe(Helpers.GetDTOField(dto, "Position[13]"));
        Helpers.GetSpriggitField(spriggit, "Position[14]").ShouldBe(Helpers.GetDTOField(dto, "Position[14]"));
        Helpers.GetSpriggitField(spriggit, "Position[15]").ShouldBe(Helpers.GetDTOField(dto, "Position[15]"));
        Helpers.GetSpriggitField(spriggit, "Position[16]").ShouldBe(Helpers.GetDTOField(dto, "Position[16]"));
        Helpers.GetSpriggitField(spriggit, "Position[2]").ShouldBe(Helpers.GetDTOField(dto, "Position[2]"));
        Helpers.GetSpriggitField(spriggit, "Position[3]").ShouldBe(Helpers.GetDTOField(dto, "Position[3]"));
        Helpers.GetSpriggitField(spriggit, "Position[4]").ShouldBe(Helpers.GetDTOField(dto, "Position[4]"));
        Helpers.GetSpriggitField(spriggit, "Position[5]").ShouldBe(Helpers.GetDTOField(dto, "Position[5]"));
        Helpers.GetSpriggitField(spriggit, "Position[6]").ShouldBe(Helpers.GetDTOField(dto, "Position[6]"));
        Helpers.GetSpriggitField(spriggit, "Position[7]").ShouldBe(Helpers.GetDTOField(dto, "Position[7]"));
        Helpers.GetSpriggitField(spriggit, "Position[8]").ShouldBe(Helpers.GetDTOField(dto, "Position[8]"));
        Helpers.GetSpriggitField(spriggit, "Position[9]").ShouldBe(Helpers.GetDTOField(dto, "Position[9]"));
        Helpers.GetSpriggitField(spriggit, "Race").ShouldBe(Helpers.GetDTOField(dto, "RaceFormKey"));
        Helpers.GetSpriggitField(spriggit, "Responsibility").ShouldBe(Helpers.GetDTOField(dto, "Responsibility"));
        Helpers.GetSpriggitField(spriggit, "Rotation[0]").ShouldBe(Helpers.GetDTOField(dto, "Rotation[0]"));
        Helpers.GetSpriggitField(spriggit, "Rotation[1]").ShouldBe(Helpers.GetDTOField(dto, "Rotation[1]"));
        Helpers.GetSpriggitField(spriggit, "Scale[0]").ShouldBe(Helpers.GetDTOField(dto, "Scale[0]"));
        Helpers.GetSpriggitField(spriggit, "Scale[1]").ShouldBe(Helpers.GetDTOField(dto, "Scale[1]"));
        Helpers.GetSpriggitField(spriggit, "Scale[2]").ShouldBe(Helpers.GetDTOField(dto, "Scale[2]"));
        Helpers.GetSpriggitField(spriggit, "Scale[3]").ShouldBe(Helpers.GetDTOField(dto, "Scale[3]"));
        Helpers.GetSpriggitField(spriggit, "Scale[4]").ShouldBe(Helpers.GetDTOField(dto, "Scale[4]"));
        Helpers.GetSpriggitField(spriggit, "Scale[5]").ShouldBe(Helpers.GetDTOField(dto, "Scale[5]"));
        Helpers.GetSpriggitField(spriggit, "Scale[6]").ShouldBe(Helpers.GetDTOField(dto, "Scale[6]"));
        Helpers.GetSpriggitField(spriggit, "Scale[7]").ShouldBe(Helpers.GetDTOField(dto, "Scale[7]"));
        Helpers.GetSpriggitField(spriggit, "Scale[8]").ShouldBe(Helpers.GetDTOField(dto, "Scale[8]"));
        Helpers.GetSpriggitField(spriggit, "ShortName.Count").ShouldBe(Helpers.GetDTOField(dto, "ShortName.Count"));
        Helpers.GetSpriggitField(spriggit, "ShortName.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "ShortName.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "ShortName[0].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[0].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[0].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[0].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[1].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[1].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[1].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[1].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[10].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[10].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[10].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[10].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[2].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[2].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[2].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[2].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[3].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[3].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[3].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[3].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[4].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[4].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[4].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[4].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[5].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[5].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[5].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[5].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[6].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[6].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[6].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[6].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[7].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[7].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[7].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[7].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[8].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[8].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[8].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[8].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[9].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[9].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[9].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[9].String"));
        Helpers.GetSpriggitField(spriggit, "SoundLevel").ShouldBe(Helpers.GetDTOField(dto, "SoundLevel"));
        Helpers.GetSpriggitField(spriggit, "TemplateActors.StatsTemplate").ShouldBe(Helpers.GetDTOField(dto, "TemplateActors.StatsTemplate"));
        Helpers.GetSpriggitField(spriggit, "TemplateColorIndex[0]").ShouldBe(Helpers.GetDTOField(dto, "TemplateColorIndex[0]"));
        Helpers.GetSpriggitField(spriggit, "TemplateColorIndex[1]").ShouldBe(Helpers.GetDTOField(dto, "TemplateColorIndex[1]"));
        Helpers.GetSpriggitField(spriggit, "TemplateColorIndex[2]").ShouldBe(Helpers.GetDTOField(dto, "TemplateColorIndex[2]"));
        Helpers.GetSpriggitField(spriggit, "TemplateColorIndex[3]").ShouldBe(Helpers.GetDTOField(dto, "TemplateColorIndex[3]"));
        Helpers.GetSpriggitField(spriggit, "TemplateColorIndex[4]").ShouldBe(Helpers.GetDTOField(dto, "TemplateColorIndex[4]"));
        Helpers.GetSpriggitField(spriggit, "TextureLighting").ShouldBe(Helpers.GetDTOField(dto, "TextureLighting"));
        Helpers.GetSpriggitField(spriggit, "Unknown").ShouldBe(Helpers.GetDTOField(dto, "Unknown"));
        Helpers.GetSpriggitField(spriggit, "UseTemplateActors").ShouldBe(Helpers.GetDTOField(dto, "UseTemplateActors"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Value[10]").ShouldBe(Helpers.GetDTOField(dto, "Value[10]"));
        Helpers.GetSpriggitField(spriggit, "Value[11]").ShouldBe(Helpers.GetDTOField(dto, "Value[11]"));
        Helpers.GetSpriggitField(spriggit, "Value[12]").ShouldBe(Helpers.GetDTOField(dto, "Value[12]"));
        Helpers.GetSpriggitField(spriggit, "Value[13]").ShouldBe(Helpers.GetDTOField(dto, "Value[13]"));
        Helpers.GetSpriggitField(spriggit, "Value[14]").ShouldBe(Helpers.GetDTOField(dto, "Value[14]"));
        Helpers.GetSpriggitField(spriggit, "Value[15]").ShouldBe(Helpers.GetDTOField(dto, "Value[15]"));
        Helpers.GetSpriggitField(spriggit, "Value[2]").ShouldBe(Helpers.GetDTOField(dto, "Value[2]"));
        Helpers.GetSpriggitField(spriggit, "Value[3]").ShouldBe(Helpers.GetDTOField(dto, "Value[3]"));
        Helpers.GetSpriggitField(spriggit, "Value[4]").ShouldBe(Helpers.GetDTOField(dto, "Value[4]"));
        Helpers.GetSpriggitField(spriggit, "Value[5]").ShouldBe(Helpers.GetDTOField(dto, "Value[5]"));
        Helpers.GetSpriggitField(spriggit, "Value[6]").ShouldBe(Helpers.GetDTOField(dto, "Value[6]"));
        Helpers.GetSpriggitField(spriggit, "Value[7]").ShouldBe(Helpers.GetDTOField(dto, "Value[7]"));
        Helpers.GetSpriggitField(spriggit, "Value[8]").ShouldBe(Helpers.GetDTOField(dto, "Value[8]"));
        Helpers.GetSpriggitField(spriggit, "Value[9]").ShouldBe(Helpers.GetDTOField(dto, "Value[9]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "Voice").ShouldBe(Helpers.GetDTOField(dto, "VoiceFormKey"));
        Helpers.GetSpriggitField(spriggit, "Weight.Fat").ShouldBe(Helpers.GetDTOField(dto, "Weight.Fat"));
        Helpers.GetSpriggitField(spriggit, "Weight.Muscular").ShouldBe(Helpers.GetDTOField(dto, "Weight.Muscular"));
        Helpers.GetSpriggitField(spriggit, "Weight.Thin").ShouldBe(Helpers.GetDTOField(dto, "Weight.Thin"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Aggression", "Assistance", "CalculatedActionPoints", "CalculatedHealth", "Class", "Color[0]", "Color[1]", "Color[2]", "Color[3]", "Color[4]", "CombatStyle", "Confidence", "Count[0]", "Count[1]", "CrimeFaction", "DefaultOutfit", "DefaultTemplate", "DispositionBase", "EditorID", "EnergyLevel", "FormKey", "GearedUpWeapons", "HairColor", "HeightMax", "HeightMin", "Index[0]", "Index[1]", "Index[2]", "Index[3]", "Index[4]", "Index[5]", "Index[6]", "IsCompressed", "Item[0]", "Item[1]", "Level.Level", "Level.MutagenObjectType", "MajorRecordFlagsRaw", "NAM5", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Position[0]", "Position[1]", "Position[10]", "Position[11]", "Position[12]", "Position[13]", "Position[14]", "Position[15]", "Position[16]", "Position[2]", "Position[3]", "Position[4]", "Position[5]", "Position[6]", "Position[7]", "Position[8]", "Position[9]", "Race", "Responsibility", "Rotation[0]", "Rotation[1]", "Scale[0]", "Scale[1]", "Scale[2]", "Scale[3]", "Scale[4]", "Scale[5]", "Scale[6]", "Scale[7]", "Scale[8]", "ShortName.Count", "ShortName.TargetLanguage", "ShortName[0].Language", "ShortName[0].String", "ShortName[1].Language", "ShortName[1].String", "ShortName[10].Language", "ShortName[10].String", "ShortName[2].Language", "ShortName[2].String", "ShortName[3].Language", "ShortName[3].String", "ShortName[4].Language", "ShortName[4].String", "ShortName[5].Language", "ShortName[5].String", "ShortName[6].Language", "ShortName[6].String", "ShortName[7].Language", "ShortName[7].String", "ShortName[8].Language", "ShortName[8].String", "ShortName[9].Language", "ShortName[9].String", "SoundLevel", "TemplateActors.StatsTemplate", "TemplateColorIndex[0]", "TemplateColorIndex[1]", "TemplateColorIndex[2]", "TemplateColorIndex[3]", "TemplateColorIndex[4]", "TextureLighting", "Unknown", "UseTemplateActors", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[2]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl", "Voice", "Weight.Fat", "Weight.Muscular", "Weight.Thin");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Aggression", "Assistance", "CalculatedActionPoints", "CalculatedHealth", "Class", "Color[0]", "Color[1]", "Color[2]", "Color[3]", "Color[4]", "CombatStyleFormKey", "Confidence", "Count[0]", "Count[1]", "CrimeFactionFormKey", "DefaultOutfit", "DefaultTemplate", "DispositionBase", "EditorID", "EnergyLevel", "FormKey", "GearedUpWeapons", "HairColor", "HeightMax", "HeightMin", "Index[0]", "Index[1]", "Index[2]", "Index[3]", "Index[4]", "Index[5]", "Index[6]", "IsCompressed", "Item[0]", "Item[1]", "Level.Level", "Level.MutagenObjectType", "MajorRecordFlags", "NAM5", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Position[0]", "Position[1]", "Position[10]", "Position[11]", "Position[12]", "Position[13]", "Position[14]", "Position[15]", "Position[16]", "Position[2]", "Position[3]", "Position[4]", "Position[5]", "Position[6]", "Position[7]", "Position[8]", "Position[9]", "RaceFormKey", "Responsibility", "Rotation[0]", "Rotation[1]", "Scale[0]", "Scale[1]", "Scale[2]", "Scale[3]", "Scale[4]", "Scale[5]", "Scale[6]", "Scale[7]", "Scale[8]", "ShortName.Count", "ShortName.TargetLanguage", "ShortName[0].Language", "ShortName[0].String", "ShortName[1].Language", "ShortName[1].String", "ShortName[10].Language", "ShortName[10].String", "ShortName[2].Language", "ShortName[2].String", "ShortName[3].Language", "ShortName[3].String", "ShortName[4].Language", "ShortName[4].String", "ShortName[5].Language", "ShortName[5].String", "ShortName[6].Language", "ShortName[6].String", "ShortName[7].Language", "ShortName[7].String", "ShortName[8].Language", "ShortName[8].String", "ShortName[9].Language", "ShortName[9].String", "SoundLevel", "TemplateActors.StatsTemplate", "TemplateColorIndex[0]", "TemplateColorIndex[1]", "TemplateColorIndex[2]", "TemplateColorIndex[3]", "TemplateColorIndex[4]", "TextureLighting", "Unknown", "UseTemplateActors", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[2]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl", "VoiceFormKey", "Weight.Fat", "Weight.Muscular", "Weight.Thin");
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

        Helpers.GetSpriggitField(spriggit, "Aggression").ShouldBe(Helpers.GetDTOField(dto, "Aggression"));
        Helpers.GetSpriggitField(spriggit, "Assistance").ShouldBe(Helpers.GetDTOField(dto, "Assistance"));
        Helpers.GetSpriggitField(spriggit, "AttackRace").ShouldBe(Helpers.GetDTOField(dto, "AttackRace"));
        Helpers.GetSpriggitField(spriggit, "CalculatedActionPoints").ShouldBe(Helpers.GetDTOField(dto, "CalculatedActionPoints"));
        Helpers.GetSpriggitField(spriggit, "CalculatedHealth").ShouldBe(Helpers.GetDTOField(dto, "CalculatedHealth"));
        Helpers.GetSpriggitField(spriggit, "Class").ShouldBe(Helpers.GetDTOField(dto, "Class"));
        Helpers.GetSpriggitField(spriggit, "CombatOverridePackageList").ShouldBe(Helpers.GetDTOField(dto, "CombatOverridePackageList"));
        Helpers.GetSpriggitField(spriggit, "CombatStyle").ShouldBe(Helpers.GetDTOField(dto, "CombatStyleFormKey"));
        Helpers.GetSpriggitField(spriggit, "Confidence").ShouldBe(Helpers.GetDTOField(dto, "Confidence"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "Count[5]").ShouldBe(Helpers.GetDTOField(dto, "Count[5]"));
        Helpers.GetSpriggitField(spriggit, "DefaultOutfit").ShouldBe(Helpers.GetDTOField(dto, "DefaultOutfit"));
        Helpers.GetSpriggitField(spriggit, "DefaultPackageList").ShouldBe(Helpers.GetDTOField(dto, "DefaultPackageListFormKey"));
        Helpers.GetSpriggitField(spriggit, "DispositionBase").ShouldBe(Helpers.GetDTOField(dto, "DispositionBase"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "EnergyLevel").ShouldBe(Helpers.GetDTOField(dto, "EnergyLevel"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "GearedUpWeapons").ShouldBe(Helpers.GetDTOField(dto, "GearedUpWeapons"));
        Helpers.GetSpriggitField(spriggit, "HeightMax").ShouldBe(Helpers.GetDTOField(dto, "HeightMax"));
        Helpers.GetSpriggitField(spriggit, "HeightMin").ShouldBe(Helpers.GetDTOField(dto, "HeightMin"));
        Helpers.GetSpriggitField(spriggit, "IsCompressed").ShouldBe(Helpers.GetDTOField(dto, "IsCompressed"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Item[4]").ShouldBe(Helpers.GetDTOField(dto, "Item[4]"));
        Helpers.GetSpriggitField(spriggit, "Item[5]").ShouldBe(Helpers.GetDTOField(dto, "Item[5]"));
        Helpers.GetSpriggitField(spriggit, "Level.Level").ShouldBe(Helpers.GetDTOField(dto, "Level.Level"));
        Helpers.GetSpriggitField(spriggit, "Level.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Level.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "NAM5").ShouldBe(Helpers.GetDTOField(dto, "NAM5"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[10].String").ShouldBe(Helpers.GetDTOField(dto, "Name[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Name[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[9].String").ShouldBe(Helpers.GetDTOField(dto, "Name[9].String"));
        Helpers.GetSpriggitField(spriggit, "Race").ShouldBe(Helpers.GetDTOField(dto, "RaceFormKey"));
        Helpers.GetSpriggitField(spriggit, "Rank").ShouldBe(Helpers.GetDTOField(dto, "Rank"));
        Helpers.GetSpriggitField(spriggit, "Responsibility").ShouldBe(Helpers.GetDTOField(dto, "Responsibility"));
        Helpers.GetSpriggitField(spriggit, "Skin").ShouldBe(Helpers.GetDTOField(dto, "Skin"));
        Helpers.GetSpriggitField(spriggit, "SoundLevel").ShouldBe(Helpers.GetDTOField(dto, "SoundLevel"));
        Helpers.GetSpriggitField(spriggit, "TextureLighting").ShouldBe(Helpers.GetDTOField(dto, "TextureLighting"));
        Helpers.GetSpriggitField(spriggit, "Unknown").ShouldBe(Helpers.GetDTOField(dto, "Unknown"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Value[2]").ShouldBe(Helpers.GetDTOField(dto, "Value[2]"));
        Helpers.GetSpriggitField(spriggit, "Value[3]").ShouldBe(Helpers.GetDTOField(dto, "Value[3]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "Voice").ShouldBe(Helpers.GetDTOField(dto, "VoiceFormKey"));
        Helpers.GetSpriggitField(spriggit, "Weight.Fat").ShouldBe(Helpers.GetDTOField(dto, "Weight.Fat"));
        Helpers.GetSpriggitField(spriggit, "Weight.Muscular").ShouldBe(Helpers.GetDTOField(dto, "Weight.Muscular"));
        Helpers.GetSpriggitField(spriggit, "Weight.Thin").ShouldBe(Helpers.GetDTOField(dto, "Weight.Thin"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Aggression", "Assistance", "AttackRace", "CalculatedActionPoints", "CalculatedHealth", "Class", "CombatOverridePackageList", "CombatStyle", "Confidence", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "DefaultOutfit", "DefaultPackageList", "DispositionBase", "EditorID", "EnergyLevel", "FormKey", "GearedUpWeapons", "HeightMax", "HeightMin", "IsCompressed", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Item[4]", "Item[5]", "Level.Level", "Level.MutagenObjectType", "MajorRecordFlagsRaw", "NAM5", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Race", "Rank", "Responsibility", "Skin", "SoundLevel", "TextureLighting", "Unknown", "Value[0]", "Value[1]", "Value[2]", "Value[3]", "Version2", "VersionControl", "Voice", "Weight.Fat", "Weight.Muscular", "Weight.Thin");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Aggression", "Assistance", "AttackRace", "CalculatedActionPoints", "CalculatedHealth", "Class", "CombatOverridePackageList", "CombatStyleFormKey", "Confidence", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "DefaultOutfit", "DefaultPackageListFormKey", "DispositionBase", "EditorID", "EnergyLevel", "FormKey", "GearedUpWeapons", "HeightMax", "HeightMin", "IsCompressed", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Item[4]", "Item[5]", "Level.Level", "Level.MutagenObjectType", "MajorRecordFlags", "NAM5", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "RaceFormKey", "Rank", "Responsibility", "Skin", "SoundLevel", "TextureLighting", "Unknown", "Value[0]", "Value[1]", "Value[2]", "Value[3]", "Version2", "VersionControl", "VoiceFormKey", "Weight.Fat", "Weight.Muscular", "Weight.Thin");
    }
}