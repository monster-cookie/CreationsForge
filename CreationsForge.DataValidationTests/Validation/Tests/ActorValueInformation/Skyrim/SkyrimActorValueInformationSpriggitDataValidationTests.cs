using System.Globalization;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ActorValueInformation.Skyrim;

public class SkyrimActorValueInformationSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "000456:Skyrim.esm")]
    [Trait("EditorID", "AVAlchemy")]
    [Trait("SpriggitFile", "ActorValueInformation/AVAlchemy - 000456_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVAlchemy()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "AVAlchemy");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "000456:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        AssertTranslatedField(spriggitFields, dtoFields, "Name");
        AssertOptionalTranslatedField(spriggitFields, dtoFields, "Description");

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "000458:Skyrim.esm")]
    [Trait("EditorID", "AVAlteration")]
    [Trait("SpriggitFile", "ActorValueInformation/AVAlteration - 000458_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVAlteration()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "AVAlteration");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "000458:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        AssertTranslatedField(spriggitFields, dtoFields, "Name");
        AssertOptionalTranslatedField(spriggitFields, dtoFields, "Description");

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "00044F:Skyrim.esm")]
    [Trait("EditorID", "AVBlock")]
    [Trait("SpriggitFile", "ActorValueInformation/AVBlock - 00044F_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVBlock()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "AVBlock");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "00044F:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        AssertTranslatedField(spriggitFields, dtoFields, "Name");
        AssertOptionalTranslatedField(spriggitFields, dtoFields, "Description");

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0005F6:Skyrim.esm")]
    [Trait("EditorID", "AVFavorActive")]
    [Trait("SpriggitFile", "ActorValueInformation/AVFavorActive - 0005F6_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVFavorActive()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "AVFavorActive");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "0005F6:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        AssertOptionalTranslatedField(spriggitFields, dtoFields, "Name");
        AssertOptionalTranslatedField(spriggitFields, dtoFields, "Description");

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    private static void AssertOptionalTranslatedField(
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields,
        string fieldName)
    {
        if (!spriggitFields.ContainsKey(fieldName + ".Count"))
        {
            return;
        }

        AssertTranslatedField(spriggitFields, dtoFields, fieldName);
    }

    private static void AssertTranslatedField(
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields,
        string fieldName)
    {
        spriggitFields[fieldName + ".Count"].ShouldBe(dtoFields[fieldName + ".Count"]);
        spriggitFields[fieldName + ".TargetLanguage"].ShouldBe(dtoFields[fieldName + ".TargetLanguage"]);
        var count = int.Parse(spriggitFields[fieldName + ".Count"], CultureInfo.InvariantCulture);
        for (var index = 0; index < count; index++)
        {
            var entryPath = fieldName + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
            spriggitFields[entryPath + ".Language"].ShouldBe(dtoFields[entryPath + ".Language"]);
            spriggitFields[entryPath + ".String"].ShouldBe(dtoFields[entryPath + ".String"]);
        }
    }
}
