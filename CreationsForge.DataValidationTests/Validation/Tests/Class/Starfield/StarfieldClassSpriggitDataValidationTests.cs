using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Class;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Class.Starfield;

public class StarfieldClassSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "01326B:Starfield.esm")]
    [Trait("EditorID", "Citizen")]
    [Trait("SpriggitFile", "Classes/Citizen - 01326B_Starfield.esm.yaml")]
    public void Starfield_CLAS_ShouldMatchSpriggitSample_Citizen()
    {
        AssertClassSpec(ClassValidationSpecs.Starfield_Citizen());
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "20F487:Starfield.esm")]
    [Trait("EditorID", "CourserClass")]
    [Trait("SpriggitFile", "Classes/CourserClass - 20F487_Starfield.esm.yaml")]
    public void Starfield_CLAS_ShouldMatchSpriggitSample_CourserClass()
    {
        AssertClassSpec(ClassValidationSpecs.Starfield_CourserClass());
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "010B2F:Starfield.esm")]
    [Trait("EditorID", "CrimsonFleetClass")]
    [Trait("SpriggitFile", "Classes/CrimsonFleetClass - 010B2F_Starfield.esm.yaml")]
    public void Starfield_CLAS_ShouldMatchSpriggitSample_CrimsonFleetClass()
    {
        AssertClassSpec(ClassValidationSpecs.Starfield_CrimsonFleetClass());
    }

    private void AssertClassSpec(ValidationSpec spec)
    {
        var dto = Helpers.GetDTO<ClassDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
