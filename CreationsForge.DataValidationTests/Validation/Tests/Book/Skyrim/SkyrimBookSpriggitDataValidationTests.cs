using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.Book;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Book.Skyrim;

public class SkyrimBookSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "10F776:Skyrim.esm")]
    [Trait("EditorID", "AtrFrgDaedricRecipe00")]
    [Trait("SpriggitFile", "Books/AtrFrgDaedricRecipe00 - 10F776_Skyrim.esm.yaml")]
    public void Skyrim_BOOK_ShouldMatchSpriggitSample_AtrFrgDaedricRecipe00()
    {
        var spec = BookValidationSpecs.Skyrim_AtrFrgDaedricRecipe00();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "01AFD7:Skyrim.esm")]
    [Trait("EditorID", "Book0ArgonianAccountBook1")]
    [Trait("SpriggitFile", "Books/Book0ArgonianAccountBook1 - 01AFD7_Skyrim.esm.yaml")]
    public void Skyrim_BOOK_ShouldMatchSpriggitSample_Book0ArgonianAccountBook1()
    {
        var spec = BookValidationSpecs.Skyrim_Book0ArgonianAccountBook1();
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
