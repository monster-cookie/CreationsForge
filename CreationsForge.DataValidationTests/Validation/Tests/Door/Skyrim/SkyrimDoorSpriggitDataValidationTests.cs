using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.Door;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Door.Skyrim;

public class SkyrimDoorSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "031897:Skyrim.esm")]
    [Trait("EditorID", "AutoLoadDoor01")]
    [Trait("SpriggitFile", "Doors/AutoLoadDoor01 - 031897_Skyrim.esm.yaml")]
    public void Skyrim_DOOR_ShouldMatchSpriggitSample_AutoLoadDoor01()
    {
        var spec = DoorValidationSpecs.Skyrim_AutoLoadDoor01();
        var dto = Helpers.GetDTO<DoorDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "022F44:Skyrim.esm")]
    [Trait("EditorID", "DBBlackDoor")]
    [Trait("SpriggitFile", "Doors/DBBlackDoor - 022F44_Skyrim.esm.yaml")]
    public void Skyrim_DOOR_ShouldMatchSpriggitSample_DBBlackDoor()
    {
        var spec = DoorValidationSpecs.Skyrim_DBBlackDoor();
        var dto = Helpers.GetDTO<DoorDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
