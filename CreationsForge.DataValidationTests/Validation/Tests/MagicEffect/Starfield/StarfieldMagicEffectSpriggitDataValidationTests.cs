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

        Helpers.GetSpriggitField(spriggit, "ActorValue2").ShouldBe(Helpers.GetDTOField(dto, "ActorValue2FormKey"));
        Helpers.GetSpriggitField(spriggit, "Archetype.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Archetype.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Archetype.Type").ShouldBe(Helpers.GetDTOField(dto, "Archetype.Type"));
        Helpers.GetSpriggitField(spriggit, "CastingArt").ShouldBe(Helpers.GetDTOField(dto, "CastingArt"));
        Helpers.GetSpriggitField(spriggit, "CastType").ShouldBe(Helpers.GetDTOField(dto, "CastType"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[2].String").ShouldBe(Helpers.GetDTOField(dto, "Description[2].String"));
        Helpers.GetSpriggitField(spriggit, "Description[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[3].String").ShouldBe(Helpers.GetDTOField(dto, "Description[3].String"));
        Helpers.GetSpriggitField(spriggit, "Description[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[4].String").ShouldBe(Helpers.GetDTOField(dto, "Description[4].String"));
        Helpers.GetSpriggitField(spriggit, "Description[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[5].String").ShouldBe(Helpers.GetDTOField(dto, "Description[5].String"));
        Helpers.GetSpriggitField(spriggit, "Description[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[6].String").ShouldBe(Helpers.GetDTOField(dto, "Description[6].String"));
        Helpers.GetSpriggitField(spriggit, "Description[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[7].String").ShouldBe(Helpers.GetDTOField(dto, "Description[7].String"));
        Helpers.GetSpriggitField(spriggit, "Description[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[8].String").ShouldBe(Helpers.GetDTOField(dto, "Description[8].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "HitShader").ShouldBe(Helpers.GetDTOField(dto, "HitShader"));
        Helpers.GetSpriggitField(spriggit, "ImpactData").ShouldBe(Helpers.GetDTOField(dto, "ImpactData"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "Projectile").ShouldBe(Helpers.GetDTOField(dto, "Projectile"));
        Helpers.GetSpriggitField(spriggit, "Sound.Start[0]").ShouldBe(Helpers.GetDTOField(dto, "Sound.Start[0]"));
        Helpers.GetSpriggitField(spriggit, "Sound.Start[1]").ShouldBe(Helpers.GetDTOField(dto, "Sound.Start[1]"));
        Helpers.GetSpriggitField(spriggit, "Sound.Start[2]").ShouldBe(Helpers.GetDTOField(dto, "Sound.Start[2]"));
        Helpers.GetSpriggitField(spriggit, "Sound.Stop").ShouldBe(Helpers.GetDTOField(dto, "Sound.Stop"));
        Helpers.GetSpriggitField(spriggit, "TargetType").ShouldBe(Helpers.GetDTOField(dto, "TargetType"));
        Helpers.GetSpriggitField(spriggit, "Type[0]").ShouldBe(Helpers.GetDTOField(dto, "Type[0]"));
        Helpers.GetSpriggitField(spriggit, "Type[1]").ShouldBe(Helpers.GetDTOField(dto, "Type[1]"));
        Helpers.GetSpriggitField(spriggit, "Type[2]").ShouldBe(Helpers.GetDTOField(dto, "Type[2]"));
        Helpers.GetSpriggitField(spriggit, "Unknown[0]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[0]"));
        Helpers.GetSpriggitField(spriggit, "Unknown[1]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[1]"));
        Helpers.GetSpriggitField(spriggit, "Unknown[2]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[2]"));
        Helpers.GetSpriggitField(spriggit, "Unknown[3]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[3]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[0]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[0]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[1]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[1]"));
        Helpers.GetSpriggitField(spriggit, "UnknownFloat4").ShouldBe(Helpers.GetDTOField(dto, "UnknownFloat4"));
        Helpers.GetSpriggitField(spriggit, "UnknownInt2").ShouldBe(Helpers.GetDTOField(dto, "UnknownInt2"));
        Helpers.GetSpriggitField(spriggit, "UnknownInt3").ShouldBe(Helpers.GetDTOField(dto, "UnknownInt3"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ActorValue2", "Archetype.MutagenObjectType", "Archetype.Type", "CastingArt", "CastType", "CompareOperator", "ComparisonValue", "Data.FirstParameter", "Data.MutagenObjectType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "FormKey", "FormVersion", "HitShader", "ImpactData", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Projectile", "Sound.Start[0]", "Sound.Start[1]", "Sound.Start[2]", "Sound.Stop", "TargetType", "Type[0]", "Type[1]", "Type[2]", "Unknown[0]", "Unknown[1]", "Unknown[2]", "Unknown[3]", "Unknown2[0]", "Unknown2[1]", "UnknownFloat4", "UnknownInt2", "UnknownInt3", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ActorValue2FormKey", "Archetype.MutagenObjectType", "Archetype.Type", "CastingArt", "CastType", "CompareOperator", "ComparisonValue", "Data.FirstParameter", "Data.MutagenObjectType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "FormKey", "FormVersion", "HitShader", "ImpactData", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Projectile", "Sound.Start[0]", "Sound.Start[1]", "Sound.Start[2]", "Sound.Stop", "TargetType", "Type[0]", "Type[1]", "Type[2]", "Unknown[0]", "Unknown[1]", "Unknown[2]", "Unknown[3]", "Unknown2[0]", "Unknown2[1]", "UnknownFloat4", "UnknownInt2", "UnknownInt3", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "Archetype.Association").ShouldBe(Helpers.GetDTOField(dto, "Archetype.Association"));
        Helpers.GetSpriggitField(spriggit, "Archetype.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Archetype.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Archetype.Type").ShouldBe(Helpers.GetDTOField(dto, "Archetype.Type"));
        Helpers.GetSpriggitField(spriggit, "CastingArt").ShouldBe(Helpers.GetDTOField(dto, "CastingArt"));
        Helpers.GetSpriggitField(spriggit, "CastType").ShouldBe(Helpers.GetDTOField(dto, "CastType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[2].String").ShouldBe(Helpers.GetDTOField(dto, "Description[2].String"));
        Helpers.GetSpriggitField(spriggit, "Description[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[3].String").ShouldBe(Helpers.GetDTOField(dto, "Description[3].String"));
        Helpers.GetSpriggitField(spriggit, "Description[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[4].String").ShouldBe(Helpers.GetDTOField(dto, "Description[4].String"));
        Helpers.GetSpriggitField(spriggit, "Description[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[5].String").ShouldBe(Helpers.GetDTOField(dto, "Description[5].String"));
        Helpers.GetSpriggitField(spriggit, "Description[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[6].String").ShouldBe(Helpers.GetDTOField(dto, "Description[6].String"));
        Helpers.GetSpriggitField(spriggit, "Description[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[7].String").ShouldBe(Helpers.GetDTOField(dto, "Description[7].String"));
        Helpers.GetSpriggitField(spriggit, "Description[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[8].String").ShouldBe(Helpers.GetDTOField(dto, "Description[8].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "HitShader").ShouldBe(Helpers.GetDTOField(dto, "HitShader"));
        Helpers.GetSpriggitField(spriggit, "ImpactData").ShouldBe(Helpers.GetDTOField(dto, "ImpactData"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "Projectile").ShouldBe(Helpers.GetDTOField(dto, "Projectile"));
        Helpers.GetSpriggitField(spriggit, "ResistValue").ShouldBe(Helpers.GetDTOField(dto, "ResistValueFormKey"));
        Helpers.GetSpriggitField(spriggit, "Sound.Start[0]").ShouldBe(Helpers.GetDTOField(dto, "Sound.Start[0]"));
        Helpers.GetSpriggitField(spriggit, "Sound.Start[1]").ShouldBe(Helpers.GetDTOField(dto, "Sound.Start[1]"));
        Helpers.GetSpriggitField(spriggit, "Sound.Start[2]").ShouldBe(Helpers.GetDTOField(dto, "Sound.Start[2]"));
        Helpers.GetSpriggitField(spriggit, "TargetType").ShouldBe(Helpers.GetDTOField(dto, "TargetType"));
        Helpers.GetSpriggitField(spriggit, "Type[0]").ShouldBe(Helpers.GetDTOField(dto, "Type[0]"));
        Helpers.GetSpriggitField(spriggit, "Type[1]").ShouldBe(Helpers.GetDTOField(dto, "Type[1]"));
        Helpers.GetSpriggitField(spriggit, "Type[2]").ShouldBe(Helpers.GetDTOField(dto, "Type[2]"));
        Helpers.GetSpriggitField(spriggit, "Unknown[0]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[0]"));
        Helpers.GetSpriggitField(spriggit, "Unknown[1]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[1]"));
        Helpers.GetSpriggitField(spriggit, "Unknown[2]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[2]"));
        Helpers.GetSpriggitField(spriggit, "Unknown[3]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[3]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2").ShouldBe(Helpers.GetDTOField(dto, "Unknown2"));
        Helpers.GetSpriggitField(spriggit, "UnknownFloat4").ShouldBe(Helpers.GetDTOField(dto, "UnknownFloat4"));
        Helpers.GetSpriggitField(spriggit, "UnknownInt2").ShouldBe(Helpers.GetDTOField(dto, "UnknownInt2"));
        Helpers.GetSpriggitField(spriggit, "UnknownInt3").ShouldBe(Helpers.GetDTOField(dto, "UnknownInt3"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Archetype.Association", "Archetype.MutagenObjectType", "Archetype.Type", "CastingArt", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "FormKey", "FormVersion", "HitShader", "ImpactData", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Projectile", "ResistValue", "Sound.Start[0]", "Sound.Start[1]", "Sound.Start[2]", "TargetType", "Type[0]", "Type[1]", "Type[2]", "Unknown[0]", "Unknown[1]", "Unknown[2]", "Unknown[3]", "Unknown2", "UnknownFloat4", "UnknownInt2", "UnknownInt3", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Archetype.Association", "Archetype.MutagenObjectType", "Archetype.Type", "CastingArt", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "FormKey", "FormVersion", "HitShader", "ImpactData", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Projectile", "ResistValueFormKey", "Sound.Start[0]", "Sound.Start[1]", "Sound.Start[2]", "TargetType", "Type[0]", "Type[1]", "Type[2]", "Unknown[0]", "Unknown[1]", "Unknown[2]", "Unknown[3]", "Unknown2", "UnknownFloat4", "UnknownInt2", "UnknownInt3", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "ActorValue2").ShouldBe(Helpers.GetDTOField(dto, "ActorValue2FormKey"));
        Helpers.GetSpriggitField(spriggit, "Archetype.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Archetype.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "CastType").ShouldBe(Helpers.GetDTOField(dto, "CastType"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[2].String").ShouldBe(Helpers.GetDTOField(dto, "Description[2].String"));
        Helpers.GetSpriggitField(spriggit, "Description[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[3].String").ShouldBe(Helpers.GetDTOField(dto, "Description[3].String"));
        Helpers.GetSpriggitField(spriggit, "Description[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[4].String").ShouldBe(Helpers.GetDTOField(dto, "Description[4].String"));
        Helpers.GetSpriggitField(spriggit, "Description[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[5].String").ShouldBe(Helpers.GetDTOField(dto, "Description[5].String"));
        Helpers.GetSpriggitField(spriggit, "Description[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[6].String").ShouldBe(Helpers.GetDTOField(dto, "Description[6].String"));
        Helpers.GetSpriggitField(spriggit, "Description[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[7].String").ShouldBe(Helpers.GetDTOField(dto, "Description[7].String"));
        Helpers.GetSpriggitField(spriggit, "Description[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[8].String").ShouldBe(Helpers.GetDTOField(dto, "Description[8].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "Explosion").ShouldBe(Helpers.GetDTOField(dto, "Explosion"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "ImageSpaceModifier").ShouldBe(Helpers.GetDTOField(dto, "ImageSpaceModifier"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "Projectile").ShouldBe(Helpers.GetDTOField(dto, "Projectile"));
        Helpers.GetSpriggitField(spriggit, "ResistValue").ShouldBe(Helpers.GetDTOField(dto, "ResistValueFormKey"));
        Helpers.GetSpriggitField(spriggit, "Sound.Start[0]").ShouldBe(Helpers.GetDTOField(dto, "Sound.Start[0]"));
        Helpers.GetSpriggitField(spriggit, "Sound.Start[1]").ShouldBe(Helpers.GetDTOField(dto, "Sound.Start[1]"));
        Helpers.GetSpriggitField(spriggit, "TargetType").ShouldBe(Helpers.GetDTOField(dto, "TargetType"));
        Helpers.GetSpriggitField(spriggit, "Type[0]").ShouldBe(Helpers.GetDTOField(dto, "Type[0]"));
        Helpers.GetSpriggitField(spriggit, "Type[1]").ShouldBe(Helpers.GetDTOField(dto, "Type[1]"));
        Helpers.GetSpriggitField(spriggit, "Unknown[0]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[0]"));
        Helpers.GetSpriggitField(spriggit, "Unknown[1]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[1]"));
        Helpers.GetSpriggitField(spriggit, "Unknown[2]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[2]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[0]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[0]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[1]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[1]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[2]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[2]"));
        Helpers.GetSpriggitField(spriggit, "UnknownFloat1").ShouldBe(Helpers.GetDTOField(dto, "UnknownFloat1"));
        Helpers.GetSpriggitField(spriggit, "UnknownFloat4").ShouldBe(Helpers.GetDTOField(dto, "UnknownFloat4"));
        Helpers.GetSpriggitField(spriggit, "UnknownInt2").ShouldBe(Helpers.GetDTOField(dto, "UnknownInt2"));
        Helpers.GetSpriggitField(spriggit, "UnknownInt3").ShouldBe(Helpers.GetDTOField(dto, "UnknownInt3"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ActorValue2", "Archetype.MutagenObjectType", "CastType", "Data.FirstParameter[0]", "Data.FirstParameter[1]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "Explosion", "FormKey", "FormVersion", "ImageSpaceModifier", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Projectile", "ResistValue", "Sound.Start[0]", "Sound.Start[1]", "TargetType", "Type[0]", "Type[1]", "Unknown[0]", "Unknown[1]", "Unknown[2]", "Unknown2[0]", "Unknown2[1]", "Unknown2[2]", "UnknownFloat1", "UnknownFloat4", "UnknownInt2", "UnknownInt3", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ActorValue2FormKey", "Archetype.MutagenObjectType", "CastType", "Data.FirstParameter[0]", "Data.FirstParameter[1]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "Explosion", "FormKey", "FormVersion", "ImageSpaceModifier", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Projectile", "ResistValueFormKey", "Sound.Start[0]", "Sound.Start[1]", "TargetType", "Type[0]", "Type[1]", "Unknown[0]", "Unknown[1]", "Unknown[2]", "Unknown2[0]", "Unknown2[1]", "Unknown2[2]", "UnknownFloat1", "UnknownFloat4", "UnknownInt2", "UnknownInt3", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "ActorValue2").ShouldBe(Helpers.GetDTOField(dto, "ActorValue2FormKey"));
        Helpers.GetSpriggitField(spriggit, "Archetype.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Archetype.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "CastingArt").ShouldBe(Helpers.GetDTOField(dto, "CastingArt"));
        Helpers.GetSpriggitField(spriggit, "CastType").ShouldBe(Helpers.GetDTOField(dto, "CastType"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[2].String").ShouldBe(Helpers.GetDTOField(dto, "Description[2].String"));
        Helpers.GetSpriggitField(spriggit, "Description[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[3].String").ShouldBe(Helpers.GetDTOField(dto, "Description[3].String"));
        Helpers.GetSpriggitField(spriggit, "Description[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[4].String").ShouldBe(Helpers.GetDTOField(dto, "Description[4].String"));
        Helpers.GetSpriggitField(spriggit, "Description[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[5].String").ShouldBe(Helpers.GetDTOField(dto, "Description[5].String"));
        Helpers.GetSpriggitField(spriggit, "Description[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[6].String").ShouldBe(Helpers.GetDTOField(dto, "Description[6].String"));
        Helpers.GetSpriggitField(spriggit, "Description[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[7].String").ShouldBe(Helpers.GetDTOField(dto, "Description[7].String"));
        Helpers.GetSpriggitField(spriggit, "Description[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[8].String").ShouldBe(Helpers.GetDTOField(dto, "Description[8].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "Explosion").ShouldBe(Helpers.GetDTOField(dto, "Explosion"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "HitShader").ShouldBe(Helpers.GetDTOField(dto, "HitShader"));
        Helpers.GetSpriggitField(spriggit, "ImpactData").ShouldBe(Helpers.GetDTOField(dto, "ImpactData"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "Projectile").ShouldBe(Helpers.GetDTOField(dto, "Projectile"));
        Helpers.GetSpriggitField(spriggit, "Sound.Start[0]").ShouldBe(Helpers.GetDTOField(dto, "Sound.Start[0]"));
        Helpers.GetSpriggitField(spriggit, "Sound.Start[1]").ShouldBe(Helpers.GetDTOField(dto, "Sound.Start[1]"));
        Helpers.GetSpriggitField(spriggit, "TargetType").ShouldBe(Helpers.GetDTOField(dto, "TargetType"));
        Helpers.GetSpriggitField(spriggit, "Type[0]").ShouldBe(Helpers.GetDTOField(dto, "Type[0]"));
        Helpers.GetSpriggitField(spriggit, "Type[1]").ShouldBe(Helpers.GetDTOField(dto, "Type[1]"));
        Helpers.GetSpriggitField(spriggit, "Unknown[0]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[0]"));
        Helpers.GetSpriggitField(spriggit, "Unknown[1]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[1]"));
        Helpers.GetSpriggitField(spriggit, "Unknown[2]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[2]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[0]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[0]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[1]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[1]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[2]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[2]"));
        Helpers.GetSpriggitField(spriggit, "UnknownFloat1").ShouldBe(Helpers.GetDTOField(dto, "UnknownFloat1"));
        Helpers.GetSpriggitField(spriggit, "UnknownFloat4").ShouldBe(Helpers.GetDTOField(dto, "UnknownFloat4"));
        Helpers.GetSpriggitField(spriggit, "UnknownInt2").ShouldBe(Helpers.GetDTOField(dto, "UnknownInt2"));
        Helpers.GetSpriggitField(spriggit, "UnknownInt3").ShouldBe(Helpers.GetDTOField(dto, "UnknownInt3"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ActorValue2", "Archetype.MutagenObjectType", "CastingArt", "CastType", "Data.FirstParameter[0]", "Data.FirstParameter[1]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "Explosion", "FormKey", "FormVersion", "HitShader", "ImpactData", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Projectile", "Sound.Start[0]", "Sound.Start[1]", "TargetType", "Type[0]", "Type[1]", "Unknown[0]", "Unknown[1]", "Unknown[2]", "Unknown2[0]", "Unknown2[1]", "Unknown2[2]", "UnknownFloat1", "UnknownFloat4", "UnknownInt2", "UnknownInt3", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ActorValue2FormKey", "Archetype.MutagenObjectType", "CastingArt", "CastType", "Data.FirstParameter[0]", "Data.FirstParameter[1]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "Explosion", "FormKey", "FormVersion", "HitShader", "ImpactData", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Projectile", "Sound.Start[0]", "Sound.Start[1]", "TargetType", "Type[0]", "Type[1]", "Unknown[0]", "Unknown[1]", "Unknown[2]", "Unknown2[0]", "Unknown2[1]", "Unknown2[2]", "UnknownFloat1", "UnknownFloat4", "UnknownInt2", "UnknownInt3", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "ActorValue2").ShouldBe(Helpers.GetDTOField(dto, "ActorValue2FormKey"));
        Helpers.GetSpriggitField(spriggit, "Archetype.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Archetype.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "CastType").ShouldBe(Helpers.GetDTOField(dto, "CastType"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[2].String").ShouldBe(Helpers.GetDTOField(dto, "Description[2].String"));
        Helpers.GetSpriggitField(spriggit, "Description[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[3].String").ShouldBe(Helpers.GetDTOField(dto, "Description[3].String"));
        Helpers.GetSpriggitField(spriggit, "Description[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[4].String").ShouldBe(Helpers.GetDTOField(dto, "Description[4].String"));
        Helpers.GetSpriggitField(spriggit, "Description[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[5].String").ShouldBe(Helpers.GetDTOField(dto, "Description[5].String"));
        Helpers.GetSpriggitField(spriggit, "Description[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[6].String").ShouldBe(Helpers.GetDTOField(dto, "Description[6].String"));
        Helpers.GetSpriggitField(spriggit, "Description[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[7].String").ShouldBe(Helpers.GetDTOField(dto, "Description[7].String"));
        Helpers.GetSpriggitField(spriggit, "Description[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[8].String").ShouldBe(Helpers.GetDTOField(dto, "Description[8].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "ImageSpaceModifier").ShouldBe(Helpers.GetDTOField(dto, "ImageSpaceModifier"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "ResistValue").ShouldBe(Helpers.GetDTOField(dto, "ResistValueFormKey"));
        Helpers.GetSpriggitField(spriggit, "Sound.Start").ShouldBe(Helpers.GetDTOField(dto, "Sound.Start"));
        Helpers.GetSpriggitField(spriggit, "TargetType").ShouldBe(Helpers.GetDTOField(dto, "TargetType"));
        Helpers.GetSpriggitField(spriggit, "Type").ShouldBe(Helpers.GetDTOField(dto, "Type"));
        Helpers.GetSpriggitField(spriggit, "Unknown[0]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[0]"));
        Helpers.GetSpriggitField(spriggit, "Unknown[1]").ShouldBe(Helpers.GetDTOField(dto, "Unknown[1]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[0]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[0]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[1]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[1]"));
        Helpers.GetSpriggitField(spriggit, "UnknownInt2").ShouldBe(Helpers.GetDTOField(dto, "UnknownInt2"));
        Helpers.GetSpriggitField(spriggit, "UnknownInt3").ShouldBe(Helpers.GetDTOField(dto, "UnknownInt3"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ActorValue2", "Archetype.MutagenObjectType", "CastType", "ComparisonValue", "Data.FirstParameter", "Data.MutagenObjectType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "FormKey", "FormVersion", "ImageSpaceModifier", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ResistValue", "Sound.Start", "TargetType", "Type", "Unknown[0]", "Unknown[1]", "Unknown2[0]", "Unknown2[1]", "UnknownInt2", "UnknownInt3", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ActorValue2FormKey", "Archetype.MutagenObjectType", "CastType", "ComparisonValue", "Data.FirstParameter", "Data.MutagenObjectType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "FormKey", "FormVersion", "ImageSpaceModifier", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ResistValueFormKey", "Sound.Start", "TargetType", "Type", "Unknown[0]", "Unknown[1]", "Unknown2[0]", "Unknown2[1]", "UnknownInt2", "UnknownInt3", "Version2", "VersionControl");
    }
}