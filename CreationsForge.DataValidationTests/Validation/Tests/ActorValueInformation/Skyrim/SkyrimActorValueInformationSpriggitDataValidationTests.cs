using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ActorValueInformation.Skyrim;

public class SkyrimActorValueInformationSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    private static readonly string[] UnsupportedSpriggitFields =
    [
        "AssociatedSkill",
        "CNAM",
        "Description",
        "FNAM",
        "HorizontalPosition",
        "Index",
        "PerkTree",
        "PerkGridX",
        "PerkGridY",
        "Skill",
        "VerticalPosition"
    ];

    private static readonly string[] UnsupportedDtoFields =
    [
        "FormVersion",
        "Type"
    ];

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "000456:Skyrim.esm")]
    [Trait("EditorID", "AVAlchemy")]
    [Trait("SpriggitFile", "ActorValueInformation/AVAlchemy - 000456_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVAlchemy()
    {
        AssertSharedFieldsMatch("AVAlchemy", "000456:Skyrim.esm");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "000458:Skyrim.esm")]
    [Trait("EditorID", "AVAlteration")]
    [Trait("SpriggitFile", "ActorValueInformation/AVAlteration - 000458_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVAlteration()
    {
        AssertSharedFieldsMatch("AVAlteration", "000458:Skyrim.esm");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "00044F:Skyrim.esm")]
    [Trait("EditorID", "AVBlock")]
    [Trait("SpriggitFile", "ActorValueInformation/AVBlock - 00044F_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVBlock()
    {
        AssertSharedFieldsMatch("AVBlock", "00044F:Skyrim.esm");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0005F6:Skyrim.esm")]
    [Trait("EditorID", "AVFavorActive")]
    [Trait("SpriggitFile", "ActorValueInformation/AVFavorActive - 0005F6_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVFavorActive()
    {
        AssertSharedFieldsMatch("AVFavorActive", "0005F6:Skyrim.esm");
    }

    private static void AssertSharedFieldsMatch(string spriggitSampleName, string formKey)
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            spriggitSampleName);
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Skyrim,
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
