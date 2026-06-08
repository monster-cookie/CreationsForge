using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
using Moq;
using Shouldly;

namespace CreationsForge.UnitTests.Importers;

public class TypedRecordImporterSupportedGamesTests
{
    [Theory]
    [MemberData(nameof(ExpandedRecordImporters))]
    public void ExpandedRecordImporters_SupportAllCurrentGames(ITypedRecordImporter importer)
    {
        importer.SupportedGames.ShouldBe([SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim], ignoreOrder: true);
    }

    public static IEnumerable<object[]> ExpandedRecordImporters()
    {
        yield return [new MiscObjectImporter(
            Mock.Of<IMiscObjectRepository>(),
            Mock.Of<IScriptingAdapterImportService>(),
            Mock.Of<IModelImportService>(),
            Mock.Of<IRecordKeywordImportService>(),
            Mock.Of<IRecordSoundImportService>())];
        yield return [new KeywordImporter(
            Mock.Of<IKeywordRepository>(),
            Mock.Of<IScriptingAdapterImportService>())];
        yield return [new ActorValueInformationImporter(
            Mock.Of<IActorValueInformationRepository>(),
            Mock.Of<IScriptingAdapterImportService>())];
        yield return [new NPCImporter(
            Mock.Of<INPCRepository>(),
            Mock.Of<IScriptingAdapterImportService>(),
            Mock.Of<IRecordKeywordImportService>())];
        yield return [new MagicEffectImporter(
            Mock.Of<IMagicEffectRepository>(),
            Mock.Of<IScriptingAdapterImportService>(),
            Mock.Of<IRecordKeywordImportService>(),
            Mock.Of<IRecordSoundImportService>())];
        yield return [new PerkImporter(
            Mock.Of<IPerkRepository>(),
            Mock.Of<IScriptingAdapterImportService>())];
    }
}
