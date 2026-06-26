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

    [Fact]
    public void ConditionFormImporter_SupportsOnlyStarfield()
    {
        var importer = new ConditionFormImporter(
            Mock.Of<IConditionFormRepository>(),
            Mock.Of<IRecordChildImportService>());

        importer.SupportedGames.ShouldBe([SupportedGame.Starfield], ignoreOrder: true);
    }

    public static IEnumerable<object[]> ExpandedRecordImporters()
    {
        yield return [new MiscItemImporter(
            Mock.Of<IMiscItemRepository>(),
            Mock.Of<IRecordChildImportService>())];
        yield return [new KeywordImporter(
            Mock.Of<IKeywordRepository>(),
            Mock.Of<IRecordChildImportService>())];
        yield return [new ActorValueInformationImporter(
            Mock.Of<IActorValueInformationRepository>(),
            Mock.Of<IRecordChildImportService>())];
        yield return [new NPCImporter(
            Mock.Of<INPCRepository>(),
            Mock.Of<IRecordChildImportService>())];
        yield return [new MagicEffectImporter(
            Mock.Of<IMagicEffectRepository>(),
            Mock.Of<IRecordChildImportService>())];
        yield return [new PerkImporter(
            Mock.Of<IPerkRepository>(),
            Mock.Of<IRecordChildImportService>())];
        yield return [new StaticImporter(
            Mock.Of<IStaticRepository>(),
            Mock.Of<IRecordChildImportService>())];
        yield return [new ContainerImporter(
            Mock.Of<IContainerRepository>(),
            Mock.Of<IRecordChildImportService>())];
    }
}
