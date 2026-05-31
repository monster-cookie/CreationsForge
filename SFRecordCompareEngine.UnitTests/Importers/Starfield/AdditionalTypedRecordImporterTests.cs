using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Moq;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Importers.Starfield;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Importers.Starfield;

public class AdditionalTypedRecordImporterTests
{
    [Fact]
    public void GlobalImporter_PropertiesReturnGlobalMetadata()
    {
        var sut = new GlobalImporter(Mock.Of<IGlobalRepository>());

        sut.GameRelease.ShouldBe(GameRelease.Starfield);
        sut.RecordType.ShouldBe(new RecordType(RecordTypeCatalog.Global.RecordID));
        sut.TableName.ShouldBe(RecordTypeCatalog.Global.TableName);
    }

    [Fact]
    public void GlobalImporter_ImportSavesGlobal()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var repository = new Mock<IGlobalRepository>();
        var record = new GlobalDTO
        {
            ModKey = modKey,
            FormKey = new FormKey(modKey, 123),
            EditorID = "Editor",
            FormVersion = 44,
            StarfieldMajorRecordFlags = 0,
            Version2 = 0,
            VersionControl = 0,
            ImportedAtUTC = DateTime.MinValue,
            Data = "1.5"
        };
        var result = new RecordTypeImportResultDTO
        {
            RecordType = "GLOB",
            HeaderImportSupported = true,
            TypedDetailImportSupported = true
        };
        var sut = new GlobalImporter(repository.Object);

        sut.Import(record, result);

        repository.Verify(x => x.Save(record), Times.Once);
        record.ImportedAtUTC.ShouldNotBe(DateTime.MinValue);
        result.DetailRowsImported.ShouldBe(1);
    }

    [Fact]
    public void AdditionalImporters_PropertiesReturnExpectedMetadata()
    {
        var importers = new (ITypedRecordDetailImporter Importer, string RecordID, string TableName)[]
        {
            (new MiscItemImporter(Mock.Of<IMiscItemRepository>()), "MISC", "MiscItem"),
            (new KeywordImporter(Mock.Of<IKeywordRepository>()), "KYWD", "Keyword"),
            (new NpcImporter(Mock.Of<INpcRepository>()), "NPC_", "Npc"),
            (new ActorValueInformationImporter(Mock.Of<IActorValueInformationRepository>()), "AVIF", "ActorValueInformation"),
            (new MagicEffectImporter(Mock.Of<IMagicEffectRepository>()), "MGEF", "MagicEffect"),
            (new PerkImporter(Mock.Of<IPerkRepository>()), "PERK", "Perk")
        };

        foreach (var (importer, recordID, tableName) in importers)
        {
            importer.GameRelease.ShouldBe(GameRelease.Starfield);
            importer.RecordType.ShouldBe(new RecordType(recordID));
            importer.TableName.ShouldBe(tableName);
        }
    }
}
