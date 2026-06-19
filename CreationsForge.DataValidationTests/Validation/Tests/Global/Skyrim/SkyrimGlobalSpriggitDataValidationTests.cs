using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Global.Skyrim;

public class SkyrimGlobalSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "10636A:Skyrim.esm")]
    [Trait("EditorID", "1stPKillCam")]
    [Trait("SpriggitFile", "Globals/1stPKillCam - 10636A_Skyrim.esm.yaml")]
    public void Skyrim_GLOB_ShouldMatchSpriggitSample_1stPKillCam()
    {
        var spriggit = Helpers.GetSpriggit<GlobalSpriggitDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Global,
            "1stPKillCam");
        var dto = Helpers.GetDTO<GlobalDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Global,
            "10636A:Skyrim.esm");

        spriggit.FormKey.ShouldBe(Helpers.FormatFormKey(dto.FormKey));
        spriggit.MajorRecordFlagsRaw.ShouldBe(dto.MajorRecordFlags);
        spriggit.FormVersion.ShouldBe(dto.FormVersion);
        spriggit.Data.ShouldBe(dto.Data);

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, dto);
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto);
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "050765:Skyrim.esm")]
    [Trait("EditorID", "CarriageCost")]
    [Trait("SpriggitFile", "Globals/CarriageCost - 050765_Skyrim.esm.yaml")]
    public void Skyrim_GLOB_ShouldMatchSpriggitSample_CarriageCost()
    {
        var spriggit = Helpers.GetSpriggit<GlobalSpriggitDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Global,
            "CarriageCost");
        var dto = Helpers.GetDTO<GlobalDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Global,
            "050765:Skyrim.esm");

        spriggit.FormKey.ShouldBe(Helpers.FormatFormKey(dto.FormKey));
        spriggit.MajorRecordFlagsRaw.ShouldBe(dto.MajorRecordFlags);
        spriggit.FormVersion.ShouldBe(dto.FormVersion);
        spriggit.Data.ShouldBe(dto.Data);

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, dto);
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto);
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "107702:Skyrim.esm")]
    [Trait("EditorID", "CarriageCostSmall")]
    [Trait("SpriggitFile", "Globals/CarriageCostSmall - 107702_Skyrim.esm.yaml")]
    public void Skyrim_GLOB_ShouldMatchSpriggitSample_CarriageCostSmall()
    {
        var spriggit = Helpers.GetSpriggit<GlobalSpriggitDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Global,
            "CarriageCostSmall");
        var dto = Helpers.GetDTO<GlobalDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Global,
            "107702:Skyrim.esm");

        spriggit.FormKey.ShouldBe(Helpers.FormatFormKey(dto.FormKey));
        spriggit.MajorRecordFlagsRaw.ShouldBe(dto.MajorRecordFlags);
        spriggit.FormVersion.ShouldBe(dto.FormVersion);
        spriggit.Data.ShouldBe(dto.Data);

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, dto);
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto);
    }
}
