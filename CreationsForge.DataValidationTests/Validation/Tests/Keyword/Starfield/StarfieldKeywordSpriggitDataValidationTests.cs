using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Keyword;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Keyword.Starfield;

public class StarfieldKeywordSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "200AEB:Starfield.esm")]
    [Trait("EditorID", "CCT_Enviro_AmbusherSurface")]
    [Trait("SpriggitFile", "Keywords/CCT_Enviro_AmbusherSurface - 200AEB_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_CCT_Enviro_AmbusherSurface()
    {
        var spec = KeywordValidationSpecs.Starfield_CCT_Enviro_AmbusherSurface();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "145388:Starfield.esm")]
    [Trait("EditorID", "CCT_Enviro_AmbusherUnderground")]
    [Trait("SpriggitFile", "Keywords/CCT_Enviro_AmbusherUnderground - 145388_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_CCT_Enviro_AmbusherUnderground()
    {
        var spec = KeywordValidationSpecs.Starfield_CCT_Enviro_AmbusherUnderground();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "200ADF:Starfield.esm")]
    [Trait("EditorID", "CCT_Enviro_Basking")]
    [Trait("SpriggitFile", "Keywords/CCT_Enviro_Basking - 200ADF_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_CCT_Enviro_Basking()
    {
        var spec = KeywordValidationSpecs.Starfield_CCT_Enviro_Basking();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "1C84DD:Starfield.esm")]
    [Trait("EditorID", "WeaponTypeDisplay_ElectromagneticRifle")]
    [Trait("SpriggitFile", "Keywords/WeaponTypeDisplay_ElectromagneticRifle - 1C84DD_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_WeaponTypeDisplay_ElectromagneticRifle()
    {
        var spec = KeywordValidationSpecs.Starfield_WeaponTypeDisplay_ElectromagneticRifle();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "200AE9:Starfield.esm")]
    [Trait("EditorID", "CCT_Enviro_Spook")]
    [Trait("SpriggitFile", "Keywords/CCT_Enviro_Spook - 200AE9_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_CCT_Enviro_Spook()
    {
        var spec = KeywordValidationSpecs.Starfield_CCT_Enviro_Spook();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "0345AE:Starfield.esm")]
    [Trait("EditorID", "ActorAttackInjuredLeft")]
    [Trait("SpriggitFile", "Keywords/ActorAttackInjuredLeft - 0345AE_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_ActorAttackInjuredLeft()
    {
        var spec = KeywordValidationSpecs.Starfield_ActorAttackInjuredLeft();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "1157E8:Starfield.esm")]
    [Trait("EditorID", "ActorTypeChild")]
    [Trait("SpriggitFile", "Keywords/ActorTypeChild - 1157E8_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_ActorTypeChild()
    {
        var spec = KeywordValidationSpecs.Starfield_ActorTypeChild();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "24E96F:Starfield.esm")]
    [Trait("EditorID", "AnimArchetypeEyeDown")]
    [Trait("SpriggitFile", "Keywords/AnimArchetypeEyeDown - 24E96F_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_AnimArchetypeEyeDown()
    {
        var spec = KeywordValidationSpecs.Starfield_AnimArchetypeEyeDown();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "157D41:Starfield.esm")]
    [Trait("EditorID", "ap_AVM_Armor_Skin")]
    [Trait("SpriggitFile", "Keywords/ap_AVM_Armor_Skin - 157D41_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_ap_AVM_Armor_Skin()
    {
        var spec = KeywordValidationSpecs.Starfield_ap_AVM_Armor_Skin();
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
