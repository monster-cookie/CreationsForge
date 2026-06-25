using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.NPC;
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
        var spec = NPCValidationSpecs.Fallout4_BHExtBOSSoldier();
        var dto = Helpers.GetDTO<NPCDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "0FB22E:Fallout4.esm")]
    [Trait("EditorID", "BHExtBOSSoldier_PowerArmorAuto")]
    [Trait("SpriggitFile", "Npcs/BHExtBOSSoldier_PowerArmorAuto - 0FB22E_Fallout4.esm.yaml")]
    public void Fallout4_NPC__ShouldMatchSpriggitSample_BHExtBOSSoldier_PowerArmorAuto()
    {
        var spec = NPCValidationSpecs.Fallout4_BHExtBOSSoldier_PowerArmorAuto();
        var dto = Helpers.GetDTO<NPCDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "1D58EA:Fallout4.esm")]
    [Trait("EditorID", "BHExtBOSSoldier_PowerArmorBigGun")]
    [Trait("SpriggitFile", "Npcs/BHExtBOSSoldier_PowerArmorBigGun - 1D58EA_Fallout4.esm.yaml")]
    public void Fallout4_NPC__ShouldMatchSpriggitSample_BHExtBOSSoldier_PowerArmorBigGun()
    {
        var spec = NPCValidationSpecs.Fallout4_BHExtBOSSoldier_PowerArmorBigGun();
        var dto = Helpers.GetDTO<NPCDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "05E557:Fallout4.esm")]
    [Trait("EditorID", "AllieFilmore")]
    [Trait("SpriggitFile", "Npcs/AllieFilmore - 05E557_Fallout4.esm.yaml")]
    public void Fallout4_NPC__ShouldMatchSpriggitSample_AllieFilmore()
    {
        var spec = NPCValidationSpecs.Fallout4_AllieFilmore();
        var dto = Helpers.GetDTO<NPCDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "240C21:Fallout4.esm")]
    [Trait("EditorID", "AudioTemplateSynthGen1")]
    [Trait("SpriggitFile", "Npcs/AudioTemplateSynthGen1 - 240C21_Fallout4.esm.yaml")]
    public void Fallout4_NPC__ShouldMatchSpriggitSample_AudioTemplateSynthGen1()
    {
        var spec = NPCValidationSpecs.Fallout4_AudioTemplateSynthGen1();
        var dto = Helpers.GetDTO<NPCDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
