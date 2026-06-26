using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Static;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Static.Fallout4;

public class Fallout4StaticSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "1B4AC0:Fallout4.esm")]
    [Trait("EditorID", "workshop_JunkWallDoor01")]
    [Trait("SpriggitFile", "Statics/workshop_JunkWallDoor01 - 1B4AC0_Fallout4.esm.yaml")]
    public void Fallout4_STAT_ShouldMatchSpriggitSample_workshop_JunkWallDoor01()
    {
        var spec = StaticValidationSpecs.Fallout4_workshop_JunkWallDoor01();
        var dto = Helpers.GetDTO<StaticDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "1B4AC1:Fallout4.esm")]
    [Trait("EditorID", "workshop_JunkWallDoor01A")]
    [Trait("SpriggitFile", "Statics/workshop_JunkWallDoor01A - 1B4AC1_Fallout4.esm.yaml")]
    public void Fallout4_STAT_ShouldMatchSpriggitSample_workshop_JunkWallDoor01A()
    {
        var spec = StaticValidationSpecs.Fallout4_workshop_JunkWallDoor01A();
        var dto = Helpers.GetDTO<StaticDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "0EC532:Fallout4.esm")]
    [Trait("EditorID", "workshop_ShackBalconyStairs01")]
    [Trait("SpriggitFile", "Statics/workshop_ShackBalconyStairs01 - 0EC532_Fallout4.esm.yaml")]
    public void Fallout4_STAT_ShouldMatchSpriggitSample_workshop_ShackBalconyStairs01()
    {
        var spec = StaticValidationSpecs.Fallout4_workshop_ShackBalconyStairs01();
        var dto = Helpers.GetDTO<StaticDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "000032:Fallout4.esm")]
    [Trait("EditorID", "COCMarkerHeading")]
    [Trait("SpriggitFile", "Statics/COCMarkerHeading - 000032_Fallout4.esm.yaml")]
    public void Fallout4_STAT_ShouldMatchSpriggitSample_COCMarkerHeading()
    {
        var spec = StaticValidationSpecs.Fallout4_COCMarkerHeading();
        var dto = Helpers.GetDTO<StaticDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "000021:Fallout4.esm")]
    [Trait("EditorID", "CollisionMarker")]
    [Trait("SpriggitFile", "Statics/CollisionMarker - 000021_Fallout4.esm.yaml")]
    public void Fallout4_STAT_ShouldMatchSpriggitSample_CollisionMarker()
    {
        var spec = StaticValidationSpecs.Fallout4_CollisionMarker();
        var dto = Helpers.GetDTO<StaticDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
