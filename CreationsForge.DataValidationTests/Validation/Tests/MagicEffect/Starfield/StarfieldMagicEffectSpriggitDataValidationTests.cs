using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.MagicEffect.Starfield;

public class StarfieldMagicEffectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "2C5392:Starfield.esm")]
    [Trait("EditorID", "ArtifactPowerLifeForced_Effect")]
    [Trait("SpriggitFile", "MagicEffects/ArtifactPowerLifeForced_Effect - 2C5392_Starfield.esm.yaml")]
    public void Starfield_MGEF_ShouldMatchSpriggitSample_ArtifactPowerLifeForced_Effect()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MagicEffect,
            "ArtifactPowerLifeForced_Effect");
        var dto = Helpers.GetDTO<MagicEffectDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MagicEffect,
            "2C5392:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ActorValue2"].ShouldBe(dtoFields["ActorValue2FormKey"]);
        spriggitFields["Archetype.MutagenObjectType"].ShouldBe(dtoFields["Archetype.MutagenObjectType"]);
        spriggitFields["Archetype.Type"].ShouldBe(dtoFields["Archetype.Type"]);
        spriggitFields["CastingArt"].ShouldBe(dtoFields["CastingArt"]);
        spriggitFields["CastType"].ShouldBe(dtoFields["CastType"]);
        spriggitFields["CompareOperator"].ShouldBe(dtoFields["CompareOperator"]);
        spriggitFields["ComparisonValue"].ShouldBe(dtoFields["ComparisonValue"]);
        spriggitFields["Data.FirstParameter"].ShouldBe(dtoFields["Data.FirstParameter"]);
        spriggitFields["Data.MutagenObjectType"].ShouldBe(dtoFields["Data.MutagenObjectType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["HitShader"].ShouldBe(dtoFields["HitShader"]);
        spriggitFields["ImpactData"].ShouldBe(dtoFields["ImpactData"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Name[3].Language"].ShouldBe(dtoFields["Name[3].Language"]);
        spriggitFields["Name[3].String"].ShouldBe(dtoFields["Name[3].String"]);
        spriggitFields["Name[4].Language"].ShouldBe(dtoFields["Name[4].Language"]);
        spriggitFields["Name[4].String"].ShouldBe(dtoFields["Name[4].String"]);
        spriggitFields["Name[5].Language"].ShouldBe(dtoFields["Name[5].Language"]);
        spriggitFields["Name[5].String"].ShouldBe(dtoFields["Name[5].String"]);
        spriggitFields["Name[6].Language"].ShouldBe(dtoFields["Name[6].Language"]);
        spriggitFields["Name[6].String"].ShouldBe(dtoFields["Name[6].String"]);
        spriggitFields["Name[7].Language"].ShouldBe(dtoFields["Name[7].Language"]);
        spriggitFields["Name[7].String"].ShouldBe(dtoFields["Name[7].String"]);
        spriggitFields["Name[8].Language"].ShouldBe(dtoFields["Name[8].Language"]);
        spriggitFields["Name[8].String"].ShouldBe(dtoFields["Name[8].String"]);
        spriggitFields["Projectile"].ShouldBe(dtoFields["Projectile"]);
        spriggitFields["Sound.Start[0]"].ShouldBe(dtoFields["Sound.Start[0]"]);
        spriggitFields["Sound.Start[1]"].ShouldBe(dtoFields["Sound.Start[1]"]);
        spriggitFields["Sound.Start[2]"].ShouldBe(dtoFields["Sound.Start[2]"]);
        spriggitFields["Sound.Stop"].ShouldBe(dtoFields["Sound.Stop"]);
        spriggitFields["TargetType"].ShouldBe(dtoFields["TargetType"]);
        spriggitFields["Type[0]"].ShouldBe(dtoFields["Type[0]"]);
        spriggitFields["Type[1]"].ShouldBe(dtoFields["Type[1]"]);
        spriggitFields["Type[2]"].ShouldBe(dtoFields["Type[2]"]);
        spriggitFields["Unknown[0]"].ShouldBe(dtoFields["Unknown[0]"]);
        spriggitFields["Unknown[1]"].ShouldBe(dtoFields["Unknown[1]"]);
        spriggitFields["Unknown[2]"].ShouldBe(dtoFields["Unknown[2]"]);
        spriggitFields["Unknown[3]"].ShouldBe(dtoFields["Unknown[3]"]);
        spriggitFields["Unknown2[0]"].ShouldBe(dtoFields["Unknown2[0]"]);
        spriggitFields["Unknown2[1]"].ShouldBe(dtoFields["Unknown2[1]"]);
        spriggitFields["UnknownFloat4"].ShouldBe(dtoFields["UnknownFloat4"]);
        spriggitFields["UnknownInt2"].ShouldBe(dtoFields["UnknownInt2"]);
        spriggitFields["UnknownInt3"].ShouldBe(dtoFields["UnknownInt3"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "2C7789:Starfield.esm")]
    [Trait("EditorID", "ArtifactPowerParticleBeam_Effect")]
    [Trait("SpriggitFile", "MagicEffects/ArtifactPowerParticleBeam_Effect - 2C7789_Starfield.esm.yaml")]
    public void Starfield_MGEF_ShouldMatchSpriggitSample_ArtifactPowerParticleBeam_Effect()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MagicEffect,
            "ArtifactPowerParticleBeam_Effect");
        var dto = Helpers.GetDTO<MagicEffectDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MagicEffect,
            "2C7789:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Archetype.Association"].ShouldBe(dtoFields["Archetype.Association"]);
        spriggitFields["Archetype.MutagenObjectType"].ShouldBe(dtoFields["Archetype.MutagenObjectType"]);
        spriggitFields["Archetype.Type"].ShouldBe(dtoFields["Archetype.Type"]);
        spriggitFields["CastingArt"].ShouldBe(dtoFields["CastingArt"]);
        spriggitFields["CastType"].ShouldBe(dtoFields["CastType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["HitShader"].ShouldBe(dtoFields["HitShader"]);
        spriggitFields["ImpactData"].ShouldBe(dtoFields["ImpactData"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Name[3].Language"].ShouldBe(dtoFields["Name[3].Language"]);
        spriggitFields["Name[3].String"].ShouldBe(dtoFields["Name[3].String"]);
        spriggitFields["Name[4].Language"].ShouldBe(dtoFields["Name[4].Language"]);
        spriggitFields["Name[4].String"].ShouldBe(dtoFields["Name[4].String"]);
        spriggitFields["Name[5].Language"].ShouldBe(dtoFields["Name[5].Language"]);
        spriggitFields["Name[5].String"].ShouldBe(dtoFields["Name[5].String"]);
        spriggitFields["Name[6].Language"].ShouldBe(dtoFields["Name[6].Language"]);
        spriggitFields["Name[6].String"].ShouldBe(dtoFields["Name[6].String"]);
        spriggitFields["Name[7].Language"].ShouldBe(dtoFields["Name[7].Language"]);
        spriggitFields["Name[7].String"].ShouldBe(dtoFields["Name[7].String"]);
        spriggitFields["Name[8].Language"].ShouldBe(dtoFields["Name[8].Language"]);
        spriggitFields["Name[8].String"].ShouldBe(dtoFields["Name[8].String"]);
        spriggitFields["Projectile"].ShouldBe(dtoFields["Projectile"]);
        spriggitFields["ResistValue"].ShouldBe(dtoFields["ResistValueFormKey"]);
        spriggitFields["Sound.Start[0]"].ShouldBe(dtoFields["Sound.Start[0]"]);
        spriggitFields["Sound.Start[1]"].ShouldBe(dtoFields["Sound.Start[1]"]);
        spriggitFields["Sound.Start[2]"].ShouldBe(dtoFields["Sound.Start[2]"]);
        spriggitFields["TargetType"].ShouldBe(dtoFields["TargetType"]);
        spriggitFields["Type[0]"].ShouldBe(dtoFields["Type[0]"]);
        spriggitFields["Type[1]"].ShouldBe(dtoFields["Type[1]"]);
        spriggitFields["Type[2]"].ShouldBe(dtoFields["Type[2]"]);
        spriggitFields["Unknown[0]"].ShouldBe(dtoFields["Unknown[0]"]);
        spriggitFields["Unknown[1]"].ShouldBe(dtoFields["Unknown[1]"]);
        spriggitFields["Unknown[2]"].ShouldBe(dtoFields["Unknown[2]"]);
        spriggitFields["Unknown[3]"].ShouldBe(dtoFields["Unknown[3]"]);
        spriggitFields["Unknown2"].ShouldBe(dtoFields["Unknown2"]);
        spriggitFields["UnknownFloat4"].ShouldBe(dtoFields["UnknownFloat4"]);
        spriggitFields["UnknownInt2"].ShouldBe(dtoFields["UnknownInt2"]);
        spriggitFields["UnknownInt3"].ShouldBe(dtoFields["UnknownInt3"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "23AF01:Starfield.esm")]
    [Trait("EditorID", "ArtifactPowerSunlessSpace_AIUse")]
    [Trait("SpriggitFile", "MagicEffects/ArtifactPowerSunlessSpace_AIUse - 23AF01_Starfield.esm.yaml")]
    public void Starfield_MGEF_ShouldMatchSpriggitSample_ArtifactPowerSunlessSpace_AIUse()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MagicEffect,
            "ArtifactPowerSunlessSpace_AIUse");
        var dto = Helpers.GetDTO<MagicEffectDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MagicEffect,
            "23AF01:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ActorValue2"].ShouldBe(dtoFields["ActorValue2FormKey"]);
        spriggitFields["Archetype.MutagenObjectType"].ShouldBe(dtoFields["Archetype.MutagenObjectType"]);
        spriggitFields["CastType"].ShouldBe(dtoFields["CastType"]);
        spriggitFields["Data.FirstParameter[0]"].ShouldBe(dtoFields["Data.FirstParameter[0]"]);
        spriggitFields["Data.FirstParameter[1]"].ShouldBe(dtoFields["Data.FirstParameter[1]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["Explosion"].ShouldBe(dtoFields["Explosion"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["ImageSpaceModifier"].ShouldBe(dtoFields["ImageSpaceModifier"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Name[3].Language"].ShouldBe(dtoFields["Name[3].Language"]);
        spriggitFields["Name[3].String"].ShouldBe(dtoFields["Name[3].String"]);
        spriggitFields["Name[4].Language"].ShouldBe(dtoFields["Name[4].Language"]);
        spriggitFields["Name[4].String"].ShouldBe(dtoFields["Name[4].String"]);
        spriggitFields["Name[5].Language"].ShouldBe(dtoFields["Name[5].Language"]);
        spriggitFields["Name[5].String"].ShouldBe(dtoFields["Name[5].String"]);
        spriggitFields["Name[6].Language"].ShouldBe(dtoFields["Name[6].Language"]);
        spriggitFields["Name[6].String"].ShouldBe(dtoFields["Name[6].String"]);
        spriggitFields["Name[7].Language"].ShouldBe(dtoFields["Name[7].Language"]);
        spriggitFields["Name[7].String"].ShouldBe(dtoFields["Name[7].String"]);
        spriggitFields["Name[8].Language"].ShouldBe(dtoFields["Name[8].Language"]);
        spriggitFields["Name[8].String"].ShouldBe(dtoFields["Name[8].String"]);
        spriggitFields["Projectile"].ShouldBe(dtoFields["Projectile"]);
        spriggitFields["ResistValue"].ShouldBe(dtoFields["ResistValueFormKey"]);
        spriggitFields["Sound.Start[0]"].ShouldBe(dtoFields["Sound.Start[0]"]);
        spriggitFields["Sound.Start[1]"].ShouldBe(dtoFields["Sound.Start[1]"]);
        spriggitFields["TargetType"].ShouldBe(dtoFields["TargetType"]);
        spriggitFields["Type[0]"].ShouldBe(dtoFields["Type[0]"]);
        spriggitFields["Type[1]"].ShouldBe(dtoFields["Type[1]"]);
        spriggitFields["Unknown[0]"].ShouldBe(dtoFields["Unknown[0]"]);
        spriggitFields["Unknown[1]"].ShouldBe(dtoFields["Unknown[1]"]);
        spriggitFields["Unknown[2]"].ShouldBe(dtoFields["Unknown[2]"]);
        spriggitFields["Unknown2[0]"].ShouldBe(dtoFields["Unknown2[0]"]);
        spriggitFields["Unknown2[1]"].ShouldBe(dtoFields["Unknown2[1]"]);
        spriggitFields["Unknown2[2]"].ShouldBe(dtoFields["Unknown2[2]"]);
        spriggitFields["UnknownFloat1"].ShouldBe(dtoFields["UnknownFloat1"]);
        spriggitFields["UnknownFloat4"].ShouldBe(dtoFields["UnknownFloat4"]);
        spriggitFields["UnknownInt2"].ShouldBe(dtoFields["UnknownInt2"]);
        spriggitFields["UnknownInt3"].ShouldBe(dtoFields["UnknownInt3"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "22AC10:Starfield.esm")]
    [Trait("EditorID", "ArtifactPowerSolarFlare_AIUse")]
    [Trait("SpriggitFile", "MagicEffects/ArtifactPowerSolarFlare_AIUse - 22AC10_Starfield.esm.yaml")]
    public void Starfield_MGEF_ShouldMatchSpriggitSample_ArtifactPowerSolarFlare_AIUse()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MagicEffect,
            "ArtifactPowerSolarFlare_AIUse");
        var dto = Helpers.GetDTO<MagicEffectDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MagicEffect,
            "22AC10:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ActorValue2"].ShouldBe(dtoFields["ActorValue2FormKey"]);
        spriggitFields["Archetype.MutagenObjectType"].ShouldBe(dtoFields["Archetype.MutagenObjectType"]);
        spriggitFields["CastingArt"].ShouldBe(dtoFields["CastingArt"]);
        spriggitFields["CastType"].ShouldBe(dtoFields["CastType"]);
        spriggitFields["Data.FirstParameter[0]"].ShouldBe(dtoFields["Data.FirstParameter[0]"]);
        spriggitFields["Data.FirstParameter[1]"].ShouldBe(dtoFields["Data.FirstParameter[1]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["Explosion"].ShouldBe(dtoFields["Explosion"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["HitShader"].ShouldBe(dtoFields["HitShader"]);
        spriggitFields["ImpactData"].ShouldBe(dtoFields["ImpactData"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Name[3].Language"].ShouldBe(dtoFields["Name[3].Language"]);
        spriggitFields["Name[3].String"].ShouldBe(dtoFields["Name[3].String"]);
        spriggitFields["Name[4].Language"].ShouldBe(dtoFields["Name[4].Language"]);
        spriggitFields["Name[4].String"].ShouldBe(dtoFields["Name[4].String"]);
        spriggitFields["Name[5].Language"].ShouldBe(dtoFields["Name[5].Language"]);
        spriggitFields["Name[5].String"].ShouldBe(dtoFields["Name[5].String"]);
        spriggitFields["Name[6].Language"].ShouldBe(dtoFields["Name[6].Language"]);
        spriggitFields["Name[6].String"].ShouldBe(dtoFields["Name[6].String"]);
        spriggitFields["Name[7].Language"].ShouldBe(dtoFields["Name[7].Language"]);
        spriggitFields["Name[7].String"].ShouldBe(dtoFields["Name[7].String"]);
        spriggitFields["Name[8].Language"].ShouldBe(dtoFields["Name[8].Language"]);
        spriggitFields["Name[8].String"].ShouldBe(dtoFields["Name[8].String"]);
        spriggitFields["Projectile"].ShouldBe(dtoFields["Projectile"]);
        spriggitFields["Sound.Start[0]"].ShouldBe(dtoFields["Sound.Start[0]"]);
        spriggitFields["Sound.Start[1]"].ShouldBe(dtoFields["Sound.Start[1]"]);
        spriggitFields["TargetType"].ShouldBe(dtoFields["TargetType"]);
        spriggitFields["Type[0]"].ShouldBe(dtoFields["Type[0]"]);
        spriggitFields["Type[1]"].ShouldBe(dtoFields["Type[1]"]);
        spriggitFields["Unknown[0]"].ShouldBe(dtoFields["Unknown[0]"]);
        spriggitFields["Unknown[1]"].ShouldBe(dtoFields["Unknown[1]"]);
        spriggitFields["Unknown[2]"].ShouldBe(dtoFields["Unknown[2]"]);
        spriggitFields["Unknown2[0]"].ShouldBe(dtoFields["Unknown2[0]"]);
        spriggitFields["Unknown2[1]"].ShouldBe(dtoFields["Unknown2[1]"]);
        spriggitFields["Unknown2[2]"].ShouldBe(dtoFields["Unknown2[2]"]);
        spriggitFields["UnknownFloat1"].ShouldBe(dtoFields["UnknownFloat1"]);
        spriggitFields["UnknownFloat4"].ShouldBe(dtoFields["UnknownFloat4"]);
        spriggitFields["UnknownInt2"].ShouldBe(dtoFields["UnknownInt2"]);
        spriggitFields["UnknownInt3"].ShouldBe(dtoFields["UnknownInt3"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "245B6F:Starfield.esm")]
    [Trait("EditorID", "ENV_DMG_Airborne_Hazard_Damage_Effect")]
    [Trait("SpriggitFile", "MagicEffects/ENV_DMG_Airborne_Hazard_Damage_Effect - 245B6F_Starfield.esm.yaml")]
    public void Starfield_MGEF_ShouldMatchSpriggitSample_ENV_DMG_Airborne_Hazard_Damage_Effect()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MagicEffect,
            "ENV_DMG_Airborne_Hazard_Damage_Effect");
        var dto = Helpers.GetDTO<MagicEffectDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MagicEffect,
            "245B6F:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ActorValue2"].ShouldBe(dtoFields["ActorValue2FormKey"]);
        spriggitFields["Archetype.MutagenObjectType"].ShouldBe(dtoFields["Archetype.MutagenObjectType"]);
        spriggitFields["CastType"].ShouldBe(dtoFields["CastType"]);
        spriggitFields["ComparisonValue"].ShouldBe(dtoFields["ComparisonValue"]);
        spriggitFields["Data.FirstParameter"].ShouldBe(dtoFields["Data.FirstParameter"]);
        spriggitFields["Data.MutagenObjectType"].ShouldBe(dtoFields["Data.MutagenObjectType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["ImageSpaceModifier"].ShouldBe(dtoFields["ImageSpaceModifier"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Name[3].Language"].ShouldBe(dtoFields["Name[3].Language"]);
        spriggitFields["Name[3].String"].ShouldBe(dtoFields["Name[3].String"]);
        spriggitFields["Name[4].Language"].ShouldBe(dtoFields["Name[4].Language"]);
        spriggitFields["Name[4].String"].ShouldBe(dtoFields["Name[4].String"]);
        spriggitFields["Name[5].Language"].ShouldBe(dtoFields["Name[5].Language"]);
        spriggitFields["Name[5].String"].ShouldBe(dtoFields["Name[5].String"]);
        spriggitFields["Name[6].Language"].ShouldBe(dtoFields["Name[6].Language"]);
        spriggitFields["Name[6].String"].ShouldBe(dtoFields["Name[6].String"]);
        spriggitFields["Name[7].Language"].ShouldBe(dtoFields["Name[7].Language"]);
        spriggitFields["Name[7].String"].ShouldBe(dtoFields["Name[7].String"]);
        spriggitFields["Name[8].Language"].ShouldBe(dtoFields["Name[8].Language"]);
        spriggitFields["Name[8].String"].ShouldBe(dtoFields["Name[8].String"]);
        spriggitFields["ResistValue"].ShouldBe(dtoFields["ResistValueFormKey"]);
        spriggitFields["Sound.Start"].ShouldBe(dtoFields["Sound.Start"]);
        spriggitFields["TargetType"].ShouldBe(dtoFields["TargetType"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Unknown[0]"].ShouldBe(dtoFields["Unknown[0]"]);
        spriggitFields["Unknown[1]"].ShouldBe(dtoFields["Unknown[1]"]);
        spriggitFields["Unknown2[0]"].ShouldBe(dtoFields["Unknown2[0]"]);
        spriggitFields["Unknown2[1]"].ShouldBe(dtoFields["Unknown2[1]"]);
        spriggitFields["UnknownInt2"].ShouldBe(dtoFields["UnknownInt2"]);
        spriggitFields["UnknownInt3"].ShouldBe(dtoFields["UnknownInt3"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
