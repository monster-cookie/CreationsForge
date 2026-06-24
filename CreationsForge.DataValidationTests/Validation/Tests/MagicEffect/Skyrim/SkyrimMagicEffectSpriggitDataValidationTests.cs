using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.MagicEffect;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.MagicEffect.Skyrim;

public class SkyrimMagicEffectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "0D22FA:Skyrim.esm")]
    [Trait("EditorID", "ShockDamageMassConcAimed")]
    [Trait("SpriggitFile", "MagicEffects/ShockDamageMassConcAimed - 0D22FA_Skyrim.esm.yaml")]
    public void Skyrim_MGEF_ShouldMatchSpriggitSample_ShockDamageMassConcAimed()
    {
        AssertMagicEffect(MagicEffectValidationSpecs.Skyrim_ShockDamageMassConcAimed());
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "1019D6:Skyrim.esm")]
    [Trait("EditorID", "dunVolunruudPickaxeEffect")]
    [Trait("SpriggitFile", "MagicEffects/dunVolunruudPickaxeEffect - 1019D6_Skyrim.esm.yaml")]
    public void Skyrim_MGEF_ShouldMatchSpriggitSample_dunVolunruudPickaxeEffect()
    {
        AssertMagicEffect(MagicEffectValidationSpecs.Skyrim_dunVolunruudPickaxeEffect());
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "0CDB75:Skyrim.esm")]
    [Trait("EditorID", "ArmorFFSelf100")]
    [Trait("SpriggitFile", "MagicEffects/ArmorFFSelf100 - 0CDB75_Skyrim.esm.yaml")]
    public void Skyrim_MGEF_ShouldMatchSpriggitSample_ArmorFFSelf100()
    {
        AssertMagicEffect(MagicEffectValidationSpecs.Skyrim_ArmorFFSelf100());
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "09B246:Skyrim.esm")]
    [Trait("EditorID", "DA15WabbajackFF")]
    [Trait("SpriggitFile", "MagicEffects/DA15WabbajackFF - 09B246_Skyrim.esm.yaml")]
    public void Skyrim_MGEF_ShouldMatchSpriggitSample_DA15WabbajackFF()
    {
        AssertMagicEffect(MagicEffectValidationSpecs.Skyrim_DA15WabbajackFF());
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "0FB406:Skyrim.esm")]
    [Trait("EditorID", "dunHalldirAggDownFFAimedArea")]
    [Trait("SpriggitFile", "MagicEffects/dunHalldirAggDownFFAimedArea - 0FB406_Skyrim.esm.yaml")]
    public void Skyrim_MGEF_ShouldMatchSpriggitSample_dunHalldirAggDownFFAimedArea()
    {
        AssertMagicEffect(MagicEffectValidationSpecs.Skyrim_dunHalldirAggDownFFAimedArea());
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
