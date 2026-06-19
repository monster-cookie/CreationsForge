using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Global.Fallout4;

public class Fallout4GlobalSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "18E889:Fallout4.esm")]
    [Trait("EditorID", "AO_Companion_Search_JunkThresholdValue")]
    [Trait("SpriggitFile", "Globals/AO_Companion_Search_JunkThresholdValue - 18E889_Fallout4.esm.yaml")]
    public void Fallout4_GLOB_ShouldMatchSpriggitSample_AO_Companion_Search_JunkThresholdValue()
    {
        var spriggit = Helpers.GetSpriggit<GlobalSpriggitDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Global,
            "AO_Companion_Search_JunkThresholdValue");
        var dto = Helpers.GetDTO<GlobalDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Global,
            "18E889:Fallout4.esm");

        spriggit.FormKey.ShouldBe(Helpers.FormatFormKey(dto.FormKey));
        spriggit.MajorRecordFlagsRaw.ShouldBe(dto.MajorRecordFlags);
        spriggit.FormVersion.ShouldBe(dto.FormVersion);
        spriggit.Data.ShouldBe(dto.Data);

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, dto);
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto);
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "176107:Fallout4.esm")]
    [Trait("EditorID", "AO_Companion_Search_NextAllowedDaysUntil")]
    [Trait("SpriggitFile", "Globals/AO_Companion_Search_NextAllowedDaysUntil - 176107_Fallout4.esm.yaml")]
    public void Fallout4_GLOB_ShouldMatchSpriggitSample_AO_Companion_Search_NextAllowedDaysUntil()
    {
        var spriggit = Helpers.GetSpriggit<GlobalSpriggitDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Global,
            "AO_Companion_Search_NextAllowedDaysUntil");
        var dto = Helpers.GetDTO<GlobalDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Global,
            "176107:Fallout4.esm");

        spriggit.FormKey.ShouldBe(Helpers.FormatFormKey(dto.FormKey));
        spriggit.MajorRecordFlagsRaw.ShouldBe(dto.MajorRecordFlags);
        spriggit.FormVersion.ShouldBe(dto.FormVersion);
        spriggit.Data.ShouldBe(dto.Data);

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, dto);
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto);
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "043F14:Fallout4.esm")]
    [Trait("EditorID", "AO_Dogmeat_Container_Bailout_Dist")]
    [Trait("SpriggitFile", "Globals/AO_Dogmeat_Container_Bailout_Dist - 043F14_Fallout4.esm.yaml")]
    public void Fallout4_GLOB_ShouldMatchSpriggitSample_AO_Dogmeat_Container_Bailout_Dist()
    {
        var spriggit = Helpers.GetSpriggit<GlobalSpriggitDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Global,
            "AO_Dogmeat_Container_Bailout_Dist");
        var dto = Helpers.GetDTO<GlobalDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Global,
            "043F14:Fallout4.esm");

        spriggit.FormKey.ShouldBe(Helpers.FormatFormKey(dto.FormKey));
        spriggit.MajorRecordFlagsRaw.ShouldBe(dto.MajorRecordFlags);
        spriggit.FormVersion.ShouldBe(dto.FormVersion);
        spriggit.Data.ShouldBe(dto.Data);

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, dto);
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto);
    }
}
