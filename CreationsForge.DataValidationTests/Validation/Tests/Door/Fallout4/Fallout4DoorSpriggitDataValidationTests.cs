using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Door;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Door.Fallout4;

public class Fallout4DoorSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "01ED77:Fallout4.esm")]
    [Trait("EditorID", "AutoloadDoor")]
    [Trait("SpriggitFile", "Doors/AutoloadDoor - 01ED77_Fallout4.esm.yaml")]
    public void Fallout4_DOOR_ShouldMatchSpriggitSample_AutoloadDoor()
    {
        var spec = DoorValidationSpecs.Fallout4_AutoloadDoor();
        var dto = Helpers.GetDTO<DoorDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "01D930:Fallout4.esm")]
    [Trait("EditorID", "BldWoodPDbDoor01")]
    [Trait("SpriggitFile", "Doors/BldWoodPDbDoor01 - 01D930_Fallout4.esm.yaml")]
    public void Fallout4_DOOR_ShouldMatchSpriggitSample_BldWoodPDbDoor01()
    {
        var spec = DoorValidationSpecs.Fallout4_BldWoodPDbDoor01();
        var dto = Helpers.GetDTO<DoorDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
