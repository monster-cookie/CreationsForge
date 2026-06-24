using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.MagicEffect;
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
        AssertMagicEffect(MagicEffectValidationSpecs.Starfield_ArtifactPowerLifeForced_Effect());
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "2C7789:Starfield.esm")]
    [Trait("EditorID", "ArtifactPowerParticleBeam_Effect")]
    [Trait("SpriggitFile", "MagicEffects/ArtifactPowerParticleBeam_Effect - 2C7789_Starfield.esm.yaml")]
    public void Starfield_MGEF_ShouldMatchSpriggitSample_ArtifactPowerParticleBeam_Effect()
    {
        AssertMagicEffect(MagicEffectValidationSpecs.Starfield_ArtifactPowerParticleBeam_Effect());
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "23AF01:Starfield.esm")]
    [Trait("EditorID", "ArtifactPowerSunlessSpace_AIUse")]
    [Trait("SpriggitFile", "MagicEffects/ArtifactPowerSunlessSpace_AIUse - 23AF01_Starfield.esm.yaml")]
    public void Starfield_MGEF_ShouldMatchSpriggitSample_ArtifactPowerSunlessSpace_AIUse()
    {
        AssertMagicEffect(MagicEffectValidationSpecs.Starfield_ArtifactPowerSunlessSpace_AIUse());
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "22AC10:Starfield.esm")]
    [Trait("EditorID", "ArtifactPowerSolarFlare_AIUse")]
    [Trait("SpriggitFile", "MagicEffects/ArtifactPowerSolarFlare_AIUse - 22AC10_Starfield.esm.yaml")]
    public void Starfield_MGEF_ShouldMatchSpriggitSample_ArtifactPowerSolarFlare_AIUse()
    {
        AssertMagicEffect(MagicEffectValidationSpecs.Starfield_ArtifactPowerSolarFlare_AIUse());
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "245B6F:Starfield.esm")]
    [Trait("EditorID", "ENV_DMG_Airborne_Hazard_Damage_Effect")]
    [Trait("SpriggitFile", "MagicEffects/ENV_DMG_Airborne_Hazard_Damage_Effect - 245B6F_Starfield.esm.yaml")]
    public void Starfield_MGEF_ShouldMatchSpriggitSample_ENV_DMG_Airborne_Hazard_Damage_Effect()
    {
        AssertMagicEffect(MagicEffectValidationSpecs.Starfield_ENV_DMG_Airborne_Hazard_Damage_Effect());
    }

    private void AssertMagicEffect(ValidationSpec spec)
    {
        var dto = Helpers.GetDTO<MagicEffectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
