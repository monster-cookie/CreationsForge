using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.Book;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Book.Fallout4;

public class Fallout4BookSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "02B4DF:Fallout4.esm")]
    [Trait("EditorID", "BoS301ActuatorList")]
    [Trait("SpriggitFile", "Books/BoS301ActuatorList - 02B4DF_Fallout4.esm.yaml")]
    public void Fallout4_BOOK_ShouldMatchSpriggitSample_BoS301ActuatorList()
    {
        var spec = BookValidationSpecs.Fallout4_BoS301ActuatorList();
        var dto = Helpers.GetDTO<BookDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "23C675:Fallout4.esm")]
    [Trait("EditorID", "DN054PowerArmorPaintJobPurchaseItem")]
    [Trait("SpriggitFile", "Books/DN054PowerArmorPaintJobPurchaseItem - 23C675_Fallout4.esm.yaml")]
    public void Fallout4_BOOK_ShouldMatchSpriggitSample_DN054PowerArmorPaintJobPurchaseItem()
    {
        var spec = BookValidationSpecs.Fallout4_DN054PowerArmorPaintJobPurchaseItem();
        var dto = Helpers.GetDTO<BookDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "092A8C:Fallout4.esm")]
    [Trait("EditorID", "PerkMagGunsAndBullets07")]
    [Trait("SpriggitFile", "Books/PerkMagGunsAndBullets07 - 092A8C_Fallout4.esm.yaml")]
    public void Fallout4_BOOK_ShouldMatchSpriggitSample_PerkMagGunsAndBullets07()
    {
        var spec = BookValidationSpecs.Fallout4_PerkMagGunsAndBullets07();
        var dto = Helpers.GetDTO<BookDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
