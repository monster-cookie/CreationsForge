using System.Globalization;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ActorValueInformation.Fallout4;

public class Fallout4ActorValueInformationSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0B287B:Fallout4.esm")]
    [Trait("EditorID", "SentryBotMaxHeatLevel")]
    [Trait("SpriggitFile", "ActorValueInformation/SentryBotMaxHeatLevel - 0B287B_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_SentryBotMaxHeatLevel()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "SentryBotMaxHeatLevel");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "0B287B:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["DefaultValue"].ShouldBe(dtoFields["DefaultValue"]);
        dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        AssertOptionalTranslatedField(spriggitFields, dtoFields, "Description");

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "00080F:Fallout4.esm")]
    [Trait("EditorID", "HC_Adrenaline")]
    [Trait("SpriggitFile", "ActorValueInformation/HC_Adrenaline - 00080F_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_HC_Adrenaline()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "HC_Adrenaline");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "00080F:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["DefaultValue"].ShouldBe(dtoFields["DefaultValue"]);
        dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        AssertOptionalTranslatedField(spriggitFields, dtoFields, "Description");

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "1B88D8:Fallout4.esm")]
    [Trait("EditorID", "Incendiary")]
    [Trait("SpriggitFile", "ActorValueInformation/Incendiary - 1B88D8_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_Incendiary()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "Incendiary");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "1B88D8:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["DefaultValue"].ShouldBe(dtoFields["DefaultValue"]);
        dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        AssertOptionalTranslatedField(spriggitFields, dtoFields, "Description");

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0002C7:Fallout4.esm")]
    [Trait("EditorID", "Agility")]
    [Trait("SpriggitFile", "ActorValueInformation/Agility - 0002C7_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_Agility()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "Agility");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "0002C7:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["DefaultValue"].ShouldBe(dtoFields["DefaultValue"]);
        dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        AssertOptionalTranslatedField(spriggitFields, dtoFields, "Description");

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "1EB998:Fallout4.esm")]
    [Trait("EditorID", "AddictionCount")]
    [Trait("SpriggitFile", "ActorValueInformation/AddictionCount - 1EB998_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_AddictionCount()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "AddictionCount");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "1EB998:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["DefaultValue"].ShouldBe(dtoFields["DefaultValue"]);
        dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
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
