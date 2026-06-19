using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Global.Starfield;

public class StarfieldGlobalSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "20C81D:Starfield.esm")]
    [Trait("EditorID", "_UpdateShatteredSpaceMaster")]
    [Trait("SpriggitFile", "Globals/_UpdateShatteredSpaceMaster - 20C81D_Starfield.esm.yaml")]
    public void Starfield_GLOB_ShouldMatchSpriggitSample_UpdateShatteredSpaceMaster()
    {
        var spriggit = Helpers.GetSpriggit<GlobalSpriggitDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Global,
            "_UpdateShatteredSpaceMaster");
        var dto = Helpers.GetDTO<GlobalDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Global,
            "20C81D:Starfield.esm");

        spriggit.FormKey.ShouldBe(Helpers.FormatFormKey(dto.FormKey));
        spriggit.MajorRecordFlagsRaw.ShouldBe(dto.MajorRecordFlags);
        spriggit.FormVersion.ShouldBe(dto.FormVersion);
        spriggit.Data.ShouldBe(dto.Data);

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, dto);
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto);
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "2B7FBD:Starfield.esm")]
    [Trait("EditorID", "2B7FBD_Starfield.esm")]
    [Trait("SpriggitFile", "Globals/2B7FBD_Starfield.esm.yaml")]
    public void Starfield_GLOB_ShouldMatchSpriggitSample_2B7FBD_Starfield_esm()
    {
        var spriggit = Helpers.GetSpriggit<GlobalSpriggitDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Global,
            "2B7FBD_Starfield.esm");
        var dto = Helpers.GetDTO<GlobalDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Global,
            "2B7FBD:Starfield.esm");

        spriggit.FormKey.ShouldBe(Helpers.FormatFormKey(dto.FormKey));
        spriggit.MajorRecordFlagsRaw.ShouldBe(dto.MajorRecordFlags);
        spriggit.FormVersion.ShouldBe(dto.FormVersion);
        spriggit.Data.ShouldBe(dto.Data);

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, dto);
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto);
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "2B91E0:Starfield.esm")]
    [Trait("EditorID", "2B91E0_Starfield.esm")]
    [Trait("SpriggitFile", "Globals/2B91E0_Starfield.esm.yaml")]
    public void Starfield_GLOB_ShouldMatchSpriggitSample_2B91E0_Starfield_esm()
    {
        var spriggit = Helpers.GetSpriggit<GlobalSpriggitDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Global,
            "2B91E0_Starfield.esm");
        var dto = Helpers.GetDTO<GlobalDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Global,
            "2B91E0:Starfield.esm");

        spriggit.FormKey.ShouldBe(Helpers.FormatFormKey(dto.FormKey));
        spriggit.MajorRecordFlagsRaw.ShouldBe(dto.MajorRecordFlags);
        spriggit.FormVersion.ShouldBe(dto.FormVersion);
        spriggit.Data.ShouldBe(dto.Data);

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, dto);
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto);
    }
}
