using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories;
using SFRecordCompareEngine.Migrations;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Repositories;

public class SimpleMajorRecordRepositoryTests : IDisposable
{
    private readonly string DatabaseDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly PluginRepository PluginRepository = new();
    private readonly RecordHeaderRepository RecordHeaderRepository = new();

    public SimpleMajorRecordRepositoryTests()
    {
        var options = new SqliteDatabaseOptions
        {
            DatabaseDirectory = DatabaseDirectory
        };

        ConnectionFactory = new SqliteConnectionFactory(options);
        new DatabaseSchemaInitializer(ConnectionFactory, new DatabaseMigrationRunner()).Initialize();
    }

    public void Dispose()
    {
        if (Directory.Exists(DatabaseDirectory))
        {
            Directory.Delete(DatabaseDirectory, true);
        }
    }

    [Fact]
    public void Upsert_WhenHeadersExist_InsertsSimpleDetailRows()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPlugin(database, "Example.esm");
        var importedAtUtc = DateTimeOffset.UtcNow.ToString("O");
        var name = "Name with 'quotes' and ; SQL";

        InsertHeader(database, "000001", "Keyword");
        new KeywordRepository().Upsert(database, new KeywordDTO
        {
            ModKey = "Example.esm",
            FormID = "000001",
            Name = name,
            Color = "#00FFFFFF",
            KeywordType = "Faction",
            FNAM = "0x00000000",
            ImportedAtUtc = importedAtUtc
        });
        InsertHeader(database, "000002", "Faction");
        new FactionRepository().Upsert(database, new FactionDTO
        {
            ModKey = "Example.esm",
            FormID = "000002",
            Name = name,
            KeywordFormKey = "000100:Example.esm",
            Flags = "HiddenFromPC, TrackCrime",
            CrimeValuesArrest = 1,
            CrimeValuesMurder = 15000,
            CrimeValuesAssault = 650,
            CrimeValuesTrespass = 350,
            CrimeValuesPickpocket = 500,
            CrimeValuesStealMultiplier = 0.5,
            CrimeValuesEscape = 1500,
            CrimeValuesPiracy = 1500,
            CrimeValuesSmuggleMultiplier = 999519420,
            VendorValuesEndHour = 24,
            VendorValuesBuysStolenItems = 1,
            VendorValuesBuysNonStolenItems = 1,
            ImportedAtUtc = importedAtUtc
        });
        InsertHeader(database, "000003", "Message");
        new MessageRepository().Upsert(database, new MessageDTO { ModKey = "Example.esm", FormID = "000003", Name = name, ImportedAtUtc = importedAtUtc });
        InsertHeader(database, "000004", "GameplayOptionsGroup");
        new GameplayOptionsGroupRepository().Upsert(database, new GameplayOptionsGroupDTO { ModKey = "Example.esm", FormID = "000004", Name = name, ImportedAtUtc = importedAtUtc });
        InsertHeader(database, "000005", "Static");
        new StaticRecordRepository().Upsert(database, new StaticRecordDTO { ModKey = "Example.esm", FormID = "000005", Name = name, ObjectBounds = "Bounds", Model = "Model", ImportedAtUtc = importedAtUtc });
        InsertHeader(database, "000006", "StaticCollection");
        new StaticCollectionRepository().Upsert(database, new StaticCollectionDTO { ModKey = "Example.esm", FormID = "000006", Name = name, ObjectBounds = "Bounds", Model = "Model", ImportedAtUtc = importedAtUtc });
        InsertHeader(database, "000007", "Activator");
        new ActivatorRepository().Upsert(database, new ActivatorDTO { ModKey = "Example.esm", FormID = "000007", Name = name, ObjectBounds = "Bounds", Model = "Model", Destructible = "Destructible", ImportedAtUtc = importedAtUtc });
        InsertHeader(database, "000008", "MiscItem");
        new MiscItemRepository().Upsert(database, new MiscItemDTO { ModKey = "Example.esm", FormID = "000008", Name = name, ObjectBounds = "Bounds", Model = "Model", Destructible = "Destructible", ImportedAtUtc = importedAtUtc });
        InsertHeader(database, "000009", "GameplayOption");
        new GameplayOptionRepository().Upsert(database, new GameplayOptionDTO { ModKey = "Example.esm", FormID = "000009", Name = name, ImportedAtUtc = importedAtUtc });
        InsertHeader(database, "00000A", "MagicEffect");
        new MagicEffectRepository().Upsert(database, new MagicEffectDTO { ModKey = "Example.esm", FormID = "00000A", Name = name, ImportedAtUtc = importedAtUtc });

        var keyword = database.First<KeywordDTO>("SELECT * FROM Keyword WHERE FormID = @0;", "000001");
        keyword.Name.ShouldBe(name);
        keyword.Color.ShouldBe("#00FFFFFF");
        keyword.KeywordType.ShouldBe("Faction");
        keyword.FNAM.ShouldBe("0x00000000");
        var faction = database.First<FactionDTO>("SELECT * FROM Faction WHERE FormID = @0;", "000002");
        faction.Name.ShouldBe(name);
        faction.KeywordFormKey.ShouldBe("000100:Example.esm");
        faction.Flags.ShouldBe("HiddenFromPC, TrackCrime");
        faction.CrimeValuesArrest.ShouldBe(1);
        faction.CrimeValuesMurder.ShouldBe(15000);
        faction.CrimeValuesStealMultiplier.ShouldBe(0.5);
        faction.CrimeValuesSmuggleMultiplier.ShouldBe(999519420);
        faction.VendorValuesEndHour.ShouldBe(24);
        faction.VendorValuesBuysStolenItems.ShouldBe(1);
        faction.VendorValuesBuysNonStolenItems.ShouldBe(1);
        database.ExecuteScalar<string>("SELECT Name FROM Message WHERE FormID = @0;", "000003").ShouldBe(name);
        database.ExecuteScalar<string>("SELECT Name FROM GameplayOptionsGroup WHERE FormID = @0;", "000004").ShouldBe(name);
        database.ExecuteScalar<string>("SELECT Model FROM Static WHERE FormID = @0;", "000005").ShouldBe("Model");
        database.ExecuteScalar<string>("SELECT Model FROM StaticCollection WHERE FormID = @0;", "000006").ShouldBe("Model");
        database.ExecuteScalar<string>("SELECT Destructible FROM Activator WHERE FormID = @0;", "000007").ShouldBe("Destructible");
        database.ExecuteScalar<string>("SELECT Destructible FROM MiscItem WHERE FormID = @0;", "000008").ShouldBe("Destructible");
        database.ExecuteScalar<string>("SELECT Name FROM GameplayOption WHERE FormID = @0;", "000009").ShouldBe(name);
        database.ExecuteScalar<string>("SELECT Name FROM MagicEffect WHERE FormID = @0;", "00000A").ShouldBe(name);
    }

    [Fact]
    public void ReplaceKeywords_WhenCalledAgain_ReplacesOnlySelectedRecordKeywords()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPlugin(database, "Example.esm");
        InsertHeader(database, "000001", "Activator");
        InsertHeader(database, "000002", "Activator");
        var importedAtUtc = DateTimeOffset.UtcNow.ToString("O");
        var sut = new ActivatorRepository();
        sut.Upsert(database, new ActivatorDTO { ModKey = "Example.esm", FormID = "000001", ImportedAtUtc = importedAtUtc });
        sut.Upsert(database, new ActivatorDTO { ModKey = "Example.esm", FormID = "000002", ImportedAtUtc = importedAtUtc });
        sut.ReplaceKeywords(database, "Example.esm", "000001", [CreateKeyword("000001", 0, "000100:Example.esm"), CreateKeyword("000001", 1, "000101:Example.esm")]);
        sut.ReplaceKeywords(database, "Example.esm", "000002", [CreateKeyword("000002", 0, "000200:Example.esm")]);

        sut.ReplaceKeywords(database, "Example.esm", "000001", [CreateKeyword("000001", 0, "000102:Example.esm")]);

        database.ExecuteScalar<int>("SELECT COUNT(*) FROM ActivatorKeyword WHERE FormID = @0;", "000001").ShouldBe(1);
        database.ExecuteScalar<string>("SELECT KeywordFormKey FROM ActivatorKeyword WHERE FormID = @0;", "000001").ShouldBe("000102:Example.esm");
        database.ExecuteScalar<string>("SELECT KeywordFormKey FROM ActivatorKeyword WHERE FormID = @0;", "000002").ShouldBe("000200:Example.esm");
    }

    [Fact]
    public void ReplaceRelations_WhenCalledAgain_ReplacesOnlySelectedFactionRelations()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPlugin(database, "Example.esm");
        InsertHeader(database, "000001", "Faction");
        InsertHeader(database, "000002", "Faction");
        var importedAtUtc = DateTimeOffset.UtcNow.ToString("O");
        var sut = new FactionRepository();
        sut.Upsert(database, new FactionDTO { ModKey = "Example.esm", FormID = "000001", ImportedAtUtc = importedAtUtc });
        sut.Upsert(database, new FactionDTO { ModKey = "Example.esm", FormID = "000002", ImportedAtUtc = importedAtUtc });
        sut.ReplaceRelations(database, "Example.esm", "000001", [CreateRelation("000001", 0, "000100:Example.esm", "Enemy"), CreateRelation("000001", 1, "000101:Example.esm", null)]);
        sut.ReplaceRelations(database, "Example.esm", "000002", [CreateRelation("000002", 0, "000200:Example.esm", "Friend")]);

        sut.ReplaceRelations(database, "Example.esm", "000001", [CreateRelation("000001", 0, "000102:Example.esm", "Ally")]);

        database.ExecuteScalar<int>("SELECT COUNT(*) FROM FactionRelation WHERE FormID = @0;", "000001").ShouldBe(1);
        database.ExecuteScalar<string>("SELECT TargetFormKey FROM FactionRelation WHERE FormID = @0;", "000001").ShouldBe("000102:Example.esm");
        database.ExecuteScalar<string>("SELECT Reaction FROM FactionRelation WHERE FormID = @0;", "000001").ShouldBe("Ally");
        database.ExecuteScalar<string>("SELECT TargetFormKey FROM FactionRelation WHERE FormID = @0;", "000002").ShouldBe("000200:Example.esm");
    }

    private void InsertPlugin(NPoco.IDatabase database, string modKey)
    {
        PluginRepository.UpsertPlugin(database, new PluginMetadataDTO
        {
            ModKey = modKey,
            GameRelease = "Starfield",
            LoadOrderIndex = 1,
            PluginFileName = modKey,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current.ToString(),
            LastCheckedUtc = DateTimeOffset.UtcNow.ToString("O")
        });
    }

    private void InsertHeader(NPoco.IDatabase database, string formId, string recordType)
    {
        RecordHeaderRepository.Upsert(database, new RecordHeaderDTO
        {
            ModKey = "Example.esm",
            FormID = formId,
            RecordType = recordType,
            FormKey = $"{formId}:Example.esm",
            PluginFileName = "Example.esm",
            ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
        });
    }

    private static RecordKeywordDTO CreateKeyword(string formId, int itemIndex, string keywordFormKey)
    {
        return new RecordKeywordDTO
        {
            ModKey = "Example.esm",
            FormID = formId,
            ItemIndex = itemIndex,
            KeywordFormKey = keywordFormKey,
            ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
        };
    }

    private static FactionRelationDTO CreateRelation(string formId, int itemIndex, string targetFormKey, string? reaction)
    {
        return new FactionRelationDTO
        {
            ModKey = "Example.esm",
            FormID = formId,
            ItemIndex = itemIndex,
            TargetFormKey = targetFormKey,
            Reaction = reaction,
            ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
        };
    }
}
