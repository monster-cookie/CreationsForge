using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Importers.Starfield;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Importers.Starfield;

public class SupportedRecordImporterTests
{
    [Theory]
    [InlineData(typeof(GlobalImporter), "GLOB", "Global")]
    [InlineData(typeof(MiscItemImporter), "MISC", "MiscItem")]
    [InlineData(typeof(KeywordImporter), "KYWD", "Keyword")]
    [InlineData(typeof(NPCImporter), "NPC_", "NPC")]
    [InlineData(typeof(ActorValueInformationImporter), "AVIF", "ActorValueInformation")]
    [InlineData(typeof(MagicEffectImporter), "MGEF", "MagicEffect")]
    [InlineData(typeof(PerkImporter), "PERK", "Perk")]
    public void Properties_ReturnExpectedMetadata(Type importerType, string expectedRecordID, string expectedTableName)
    {
        var sut = (ITypedRecordDetailImporter)Activator.CreateInstance(importerType, new object?[] { null })!;

        sut.GameRelease.ShouldBe(GameRelease.Starfield);
        sut.RecordType.ShouldBe(new RecordType(expectedRecordID));
        sut.TableName.ShouldBe(expectedTableName);
    }
}