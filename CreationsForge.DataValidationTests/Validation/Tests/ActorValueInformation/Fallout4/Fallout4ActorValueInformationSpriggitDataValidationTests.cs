using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ActorValueInformation.Fallout4;

public class Fallout4ActorValueInformationSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    private static readonly string[] UnsupportedSpriggitFields =
    [
        "Description"
    ];

    private static readonly string[] UnsupportedDtoFields =
    [
        "FormVersion"
    ];

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0B287B:Fallout4.esm")]
    [Trait("EditorID", "SentryBotMaxHeatLevel")]
    [Trait("SpriggitFile", "ActorValueInformation/SentryBotMaxHeatLevel - 0B287B_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_SentryBotMaxHeatLevel()
    {
        AssertSharedFieldsMatch("SentryBotMaxHeatLevel", "0B287B:Fallout4.esm");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "00080F:Fallout4.esm")]
    [Trait("EditorID", "HC_Adrenaline")]
    [Trait("SpriggitFile", "ActorValueInformation/HC_Adrenaline - 00080F_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_HC_Adrenaline()
    {
        AssertSharedFieldsMatch("HC_Adrenaline", "00080F:Fallout4.esm");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "1B88D8:Fallout4.esm")]
    [Trait("EditorID", "Incendiary")]
    [Trait("SpriggitFile", "ActorValueInformation/Incendiary - 1B88D8_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_Incendiary()
    {
        AssertSharedFieldsMatch("Incendiary", "1B88D8:Fallout4.esm");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0002C7:Fallout4.esm")]
    [Trait("EditorID", "Agility")]
    [Trait("SpriggitFile", "ActorValueInformation/Agility - 0002C7_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_Agility()
    {
        AssertSharedFieldsMatch("Agility", "0002C7:Fallout4.esm");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "1EB998:Fallout4.esm")]
    [Trait("EditorID", "AddictionCount")]
    [Trait("SpriggitFile", "ActorValueInformation/AddictionCount - 1EB998_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_AddictionCount()
    {
        AssertSharedFieldsMatch("AddictionCount", "1EB998:Fallout4.esm");
    }

    private static void AssertSharedFieldsMatch(string spriggitSampleName, string formKey)
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            spriggitSampleName);
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            formKey);

        var dtoFields = Helpers.GetDTOFields(dto);

        foreach (var field in spriggit.Fields.OrderBy(field => field.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (dtoFields.TryGetValue(field.Key, out var dtoValue))
            {
                field.Value.ShouldBe(dtoValue);
            }
        }

        spriggit.Fields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggit.Fields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        if (Helpers.GetSpriggitListValues(spriggit, "Flags").Count > 0)
        {
            dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        }

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto, UnsupportedSpriggitFields).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto, UnsupportedDtoFields).ShouldBeEmpty();
    }
}
