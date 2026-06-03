using Moq;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;
using SFRecordCompareEngine.Core.Services.Interfaces;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class RecordTreeEntryServiceTests
{
    [Fact]
    public void FormListService_GetRecordTreeEntriesByModKey_DelegatesToRepository()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var expected = CreateEntries();
        var repository = new Mock<IFormListRepository>();
        repository.Setup(x => x.GetRecordTreeEntriesByModKey(modKey)).Returns(expected);
        var sut = new FormListService(repository.Object, Mock.Of<IFormListItemRepository>());

        var result = sut.GetRecordTreeEntriesByModKey(modKey);

        result.ShouldBeSameAs(expected);
    }

    [Fact]
    public void GameSettingService_GetRecordTreeEntriesByModKey_DelegatesToRepository()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var expected = CreateEntries();
        var repository = new Mock<IGameSettingRepository>();
        repository.Setup(x => x.GetRecordTreeEntriesByModKey(modKey)).Returns(expected);
        var sut = new GameSettingService(repository.Object);

        var result = sut.GetRecordTreeEntriesByModKey(modKey);

        result.ShouldBeSameAs(expected);
    }

    [Fact]
    public void GlobalService_GetRecordTreeEntriesByModKey_DelegatesToRepositoryWithoutHydrating()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var expected = CreateEntries();
        var repository = new Mock<IGlobalRepository>();
        var hydrationService = new Mock<IScriptingAdapterHydrationService>();
        repository.Setup(x => x.GetRecordTreeEntriesByModKey(modKey)).Returns(expected);
        var sut = new GlobalService(repository.Object, hydrationService.Object);

        var result = sut.GetRecordTreeEntriesByModKey(modKey);

        result.ShouldBeSameAs(expected);
        hydrationService.VerifyNoOtherCalls();
    }

    [Fact]
    public void MiscItemService_GetRecordTreeEntriesByModKey_DelegatesToRepositoryWithoutHydrating()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var expected = CreateEntries();
        var repository = new Mock<IMiscItemRepository>();
        var hydrationService = new Mock<IScriptingAdapterHydrationService>();
        repository.Setup(x => x.GetRecordTreeEntriesByModKey(modKey)).Returns(expected);
        var sut = new MiscItemService(repository.Object, hydrationService.Object);

        var result = sut.GetRecordTreeEntriesByModKey(modKey);

        result.ShouldBeSameAs(expected);
        hydrationService.VerifyNoOtherCalls();
    }

    [Fact]
    public void KeywordService_GetRecordTreeEntriesByModKey_DelegatesToRepositoryWithoutHydrating()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var expected = CreateEntries();
        var repository = new Mock<IKeywordRepository>();
        var hydrationService = new Mock<IScriptingAdapterHydrationService>();
        repository.Setup(x => x.GetRecordTreeEntriesByModKey(modKey)).Returns(expected);
        var sut = new KeywordService(repository.Object, hydrationService.Object);

        var result = sut.GetRecordTreeEntriesByModKey(modKey);

        result.ShouldBeSameAs(expected);
        hydrationService.VerifyNoOtherCalls();
    }

    [Fact]
    public void NPCService_GetRecordTreeEntriesByModKey_DelegatesToRepositoryWithoutHydrating()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var expected = CreateEntries();
        var repository = new Mock<INPCRepository>();
        var hydrationService = new Mock<IScriptingAdapterHydrationService>();
        repository.Setup(x => x.GetRecordTreeEntriesByModKey(modKey)).Returns(expected);
        var sut = new NPCService(repository.Object, hydrationService.Object);

        var result = sut.GetRecordTreeEntriesByModKey(modKey);

        result.ShouldBeSameAs(expected);
        hydrationService.VerifyNoOtherCalls();
    }

    [Fact]
    public void ActorValueInformationService_GetRecordTreeEntriesByModKey_DelegatesToRepositoryWithoutHydrating()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var expected = CreateEntries();
        var repository = new Mock<IActorValueInformationRepository>();
        var hydrationService = new Mock<IScriptingAdapterHydrationService>();
        repository.Setup(x => x.GetRecordTreeEntriesByModKey(modKey)).Returns(expected);
        var sut = new ActorValueInformationService(repository.Object, hydrationService.Object);

        var result = sut.GetRecordTreeEntriesByModKey(modKey);

        result.ShouldBeSameAs(expected);
        hydrationService.VerifyNoOtherCalls();
    }

    [Fact]
    public void MagicEffectService_GetRecordTreeEntriesByModKey_DelegatesToRepositoryWithoutHydrating()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var expected = CreateEntries();
        var repository = new Mock<IMagicEffectRepository>();
        var hydrationService = new Mock<IScriptingAdapterHydrationService>();
        repository.Setup(x => x.GetRecordTreeEntriesByModKey(modKey)).Returns(expected);
        var sut = new MagicEffectService(repository.Object, hydrationService.Object);

        var result = sut.GetRecordTreeEntriesByModKey(modKey);

        result.ShouldBeSameAs(expected);
        hydrationService.VerifyNoOtherCalls();
    }

    [Fact]
    public void PerkService_GetRecordTreeEntriesByModKey_DelegatesToRepositoryWithoutHydrating()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var expected = CreateEntries();
        var repository = new Mock<IPerkRepository>();
        var hydrationService = new Mock<IScriptingAdapterHydrationService>();
        repository.Setup(x => x.GetRecordTreeEntriesByModKey(modKey)).Returns(expected);
        var sut = new PerkService(repository.Object, hydrationService.Object);

        var result = sut.GetRecordTreeEntriesByModKey(modKey);

        result.ShouldBeSameAs(expected);
        hydrationService.VerifyNoOtherCalls();
    }

    private static IList<RecordTreeEntryDTO> CreateEntries()
    {
        var modKey = new ModKey("Example", ModType.Master);
        return new List<RecordTreeEntryDTO>
        {
            new()
            {
                FormKey = new FormKey(modKey, 123),
                EditorID = "ExampleRecord"
            }
        };
    }
}
