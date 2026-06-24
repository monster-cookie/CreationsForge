using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Door;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Door.Starfield;

public class StarfieldDoorSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "144F85:Starfield.esm")]
    [Trait("EditorID", "ShipFloorLoadHatch")]
    [Trait("SpriggitFile", "Doors/ShipFloorLoadHatch - 144F85_Starfield.esm.yaml")]
    public void Starfield_DOOR_ShouldMatchSpriggitSample_ShipFloorLoadHatch()
    {
        var spec = DoorValidationSpecs.Starfield_ShipFloorLoadHatch();
        var dto = Helpers.GetDTO<DoorDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "205AA6:Starfield.esm")]
    [Trait("EditorID", "ShipDockingHatchFloor")]
    [Trait("SpriggitFile", "Doors/ShipDockingHatchFloor - 205AA6_Starfield.esm.yaml")]
    public void Starfield_DOOR_ShouldMatchSpriggitSample_ShipDockingHatchFloor()
    {
        var spec = DoorValidationSpecs.Starfield_ShipDockingHatchFloor();
        var dto = Helpers.GetDTO<DoorDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "19AFF6:Starfield.esm")]
    [Trait("EditorID", "SftIntRmSmWallMid_DoorA00")]
    [Trait("SpriggitFile", "Doors/SftIntRmSmWallMid_DoorA00 - 19AFF6_Starfield.esm.yaml")]
    public void Starfield_DOOR_ShouldMatchSpriggitSample_SftIntRmSmWallMid_DoorA00()
    {
        var spec = DoorValidationSpecs.Starfield_SftIntRmSmWallMid_DoorA00();
        var dto = Helpers.GetDTO<DoorDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "30D813:Starfield.esm")]
    [Trait("EditorID", "SftIntRmSmWallMid_DoorA00_Loud")]
    [Trait("SpriggitFile", "Doors/SftIntRmSmWallMid_DoorA00_Loud - 30D813_Starfield.esm.yaml")]
    public void Starfield_DOOR_ShouldMatchSpriggitSample_SftIntRmSmWallMid_DoorA00_Loud()
    {
        var spec = DoorValidationSpecs.Starfield_SftIntRmSmWallMid_DoorA00_Loud();
        var dto = Helpers.GetDTO<DoorDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "31D042:Starfield.esm")]
    [Trait("EditorID", "ShpGenIntPerSmWallMid_ExLg_DockingDoor02L_NonLoad")]
    [Trait("SpriggitFile", "Doors/ShpGenIntPerSmWallMid_ExLg_DockingDoor02L_NonLoad - 31D042_Starfield.esm.yaml")]
    public void Starfield_DOOR_ShouldMatchSpriggitSample_ShpGenIntPerSmWallMid_ExLg_DockingDoor02L_NonLoad()
    {
        var spec = DoorValidationSpecs.Starfield_ShpGenIntPerSmWallMid_ExLg_DockingDoor02L_NonLoad();
        var dto = Helpers.GetDTO<DoorDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
