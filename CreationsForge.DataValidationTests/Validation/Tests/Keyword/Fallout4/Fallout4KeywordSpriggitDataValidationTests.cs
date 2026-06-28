using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.Keyword;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Keyword.Fallout4;

public class Fallout4KeywordSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "119B9B:Fallout4.esm")]
    [Trait("EditorID", "02Metal03Floor")]
    [Trait("SpriggitFile", "Keywords/02Metal03Floor - 119B9B_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample__02Metal03Floor()
    {
        var spec = KeywordValidationSpecs.Fallout4_02Metal03Floor();
        var dto = Helpers.GetDTO<KeywordDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "119B9C:Fallout4.esm")]
    [Trait("EditorID", "02Metal03Misc")]
    [Trait("SpriggitFile", "Keywords/02Metal03Misc - 119B9C_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample__02Metal03Misc()
    {
        var spec = KeywordValidationSpecs.Fallout4_02Metal03Misc();
        var dto = Helpers.GetDTO<KeywordDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "119B9D:Fallout4.esm")]
    [Trait("EditorID", "02Metal03Prefabs")]
    [Trait("SpriggitFile", "Keywords/02Metal03Prefabs - 119B9D_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample__02Metal03Prefabs()
    {
        var spec = KeywordValidationSpecs.Fallout4_02Metal03Prefabs();
        var dto = Helpers.GetDTO<KeywordDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "0CF43E:Fallout4.esm")]
    [Trait("EditorID", "AO_BoS_ScribeCollectData")]
    [Trait("SpriggitFile", "Keywords/AO_BoS_ScribeCollectData - 0CF43E_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample_AO_BoS_ScribeCollectData()
    {
        var spec = KeywordValidationSpecs.Fallout4_AO_BoS_ScribeCollectData();
        var dto = Helpers.GetDTO<KeywordDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "093BBE:Fallout4.esm")]
    [Trait("EditorID", "if_Armor_Combat_Freefall_Restricted")]
    [Trait("SpriggitFile", "Keywords/if_Armor_Combat_Freefall_Restricted - 093BBE_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample_if_Armor_Combat_Freefall_Restricted()
    {
        var spec = KeywordValidationSpecs.Fallout4_if_Armor_Combat_Freefall_Restricted();
        var dto = Helpers.GetDTO<KeywordDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "119BA0:Fallout4.esm")]
    [Trait("EditorID", "02Metal03Wall")]
    [Trait("SpriggitFile", "Keywords/02Metal03Wall - 119BA0_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample_02Metal03Wall()
    {
        var spec = KeywordValidationSpecs.Fallout4_02Metal03Wall();
        var dto = Helpers.GetDTO<KeywordDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "1157E8:Fallout4.esm")]
    [Trait("EditorID", "ActorTypeChild")]
    [Trait("SpriggitFile", "Keywords/ActorTypeChild - 1157E8_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample_ActorTypeChild()
    {
        var spec = KeywordValidationSpecs.Fallout4_ActorTypeChild();
        var dto = Helpers.GetDTO<KeywordDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "03D28F:Fallout4.esm")]
    [Trait("EditorID", "AnimArchetypeNervous")]
    [Trait("SpriggitFile", "Keywords/AnimArchetypeNervous - 03D28F_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample_AnimArchetypeNervous()
    {
        var spec = KeywordValidationSpecs.Fallout4_AnimArchetypeNervous();
        var dto = Helpers.GetDTO<KeywordDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "072B3F:Fallout4.esm")]
    [Trait("EditorID", "ap_Bot_ModLegsSlotB")]
    [Trait("SpriggitFile", "Keywords/ap_Bot_ModLegsSlotB - 072B3F_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample_ap_Bot_ModLegsSlotB()
    {
        var spec = KeywordValidationSpecs.Fallout4_ap_Bot_ModLegsSlotB();
        var dto = Helpers.GetDTO<KeywordDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
