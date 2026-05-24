using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories;
using SFRecordCompareEngine.Core.Services;
using SFRecordCompareEngine.Migrations;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class SimpleMajorRecordImporterTests : IDisposable
{
    private readonly string DatabaseDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly PluginRepository PluginRepository = new();
    private readonly RecordHeaderRepository RecordHeaderRepository = new();

    public SimpleMajorRecordImporterTests()
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
    public void Import_WhenSimpleRecordsHaveFields_PersistsDetailRows()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPlugin(database);
        var importedAtUtc = DateTimeOffset.UtcNow.ToString("O");

        Import(database, "000001", "Keyword", new KeywordRecordImporter(new KeywordRepository()), new TestKeywordRecord("Ecliptic", "Faction"), importedAtUtc);
        Import(database, "000002", "Faction", new FactionRecordImporter(new FactionRepository()), new TestFactionRecord(), importedAtUtc);
        Import(database, "000003", "Message", new MessageRecordImporter(new MessageRepository()), new TestNamedRecord("Example Message"), importedAtUtc);
        Import(database, "000004", "GameplayOptionsGroup", new GameplayOptionsGroupRecordImporter(new GameplayOptionsGroupRepository()), new TestNamedRecord("Example Group"), importedAtUtc);
        Import(database, "000005", "Static", new StaticRecordImporter(new StaticRecordRepository()), new TestObjectRecord("Example Static"), importedAtUtc);
        Import(database, "000006", "StaticCollection", new StaticCollectionRecordImporter(new StaticCollectionRepository()), new TestObjectRecord("Example Collection"), importedAtUtc);

        var keyword = database.First<KeywordDTO>("SELECT * FROM Keyword WHERE FormID = @0;", "000001");
        keyword.Name.ShouldBe("Ecliptic");
        keyword.Color.ShouldBe("#00FFFFFF");
        keyword.KeywordType.ShouldBe("Faction");
        keyword.FNAM.ShouldBe("0x00000000");
        var faction = database.First<FactionDTO>("SELECT * FROM Faction WHERE FormID = @0;", "000002");
        faction.Name.ShouldBe("Crimson Fleet");
        faction.KeywordFormKey.ShouldBe("0546E0:Starfield.esm");
        faction.Flags.ShouldBe("HiddenFromPC, TrackCrime, IgnoreMurder");
        faction.CrimeValuesArrest.ShouldBe(1);
        faction.CrimeValuesMurder.ShouldBe(15000);
        faction.CrimeValuesAssault.ShouldBe(650);
        faction.CrimeValuesTrespass.ShouldBe(350);
        faction.CrimeValuesPickpocket.ShouldBe(500);
        faction.CrimeValuesStealMultiplier.ShouldBe(0.5);
        faction.CrimeValuesEscape.ShouldBe(1500);
        faction.CrimeValuesPiracy.ShouldBe(1500);
        faction.CrimeValuesSmuggleMultiplier.ShouldBe(999519420);
        faction.VendorValuesEndHour.ShouldBe(24);
        faction.VendorValuesBuysStolenItems.ShouldBe(1);
        faction.VendorValuesBuysNonStolenItems.ShouldBe(1);
        database.ExecuteScalar<int>("SELECT COUNT(*) FROM FactionRelation WHERE FormID = @0;", "000002").ShouldBe(3);
        database.ExecuteScalar<string>("SELECT Reaction FROM FactionRelation WHERE FormID = @0 AND TargetFormKey = @1;", "000002", "15923E:Starfield.esm").ShouldBeNull();
        database.ExecuteScalar<string>("SELECT Name FROM Message WHERE FormID = @0;", "000003").ShouldBe("Example Message");
        database.ExecuteScalar<string>("SELECT Name FROM GameplayOptionsGroup WHERE FormID = @0;", "000004").ShouldBe("Example Group");
        database.ExecuteScalar<string>("SELECT ObjectBounds FROM Static WHERE FormID = @0;", "000005").ShouldBe("Bounds");
        database.ExecuteScalar<string>("SELECT Model FROM StaticCollection WHERE FormID = @0;", "000006").ShouldBe("Model");
    }

    [Fact]
    public void Import_WhenKeywordRecordsHaveKeywords_PersistsChildRows()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPlugin(database);
        var importedAtUtc = DateTimeOffset.UtcNow.ToString("O");

        Import(database, "000007", "Activator", new ActivatorRecordImporter(new ActivatorRepository()), new TestKeywordObjectRecord("Example Activator"), importedAtUtc);
        Import(database, "000008", "MiscItem", new MiscItemRecordImporter(new MiscItemRepository()), new TestKeywordObjectRecord("Example MiscItem"), importedAtUtc);
        Import(database, "000009", "GameplayOption", new GameplayOptionRecordImporter(new GameplayOptionRepository()), new TestKeywordNamedRecord("Example Option"), importedAtUtc);
        Import(database, "00000A", "MagicEffect", new MagicEffectRecordImporter(new MagicEffectRepository()), new TestKeywordNamedRecord("Example Effect"), importedAtUtc);

        database.ExecuteScalar<string>("SELECT Destructible FROM Activator WHERE FormID = @0;", "000007").ShouldBe("Destructible");
        database.ExecuteScalar<string>("SELECT Destructible FROM MiscItem WHERE FormID = @0;", "000008").ShouldBe("Destructible");
        database.ExecuteScalar<string>("SELECT Name FROM GameplayOption WHERE FormID = @0;", "000009").ShouldBe("Example Option");
        database.ExecuteScalar<string>("SELECT Name FROM MagicEffect WHERE FormID = @0;", "00000A").ShouldBe("Example Effect");
        database.ExecuteScalar<int>("SELECT COUNT(*) FROM ActivatorKeyword WHERE FormID = @0;", "000007").ShouldBe(2);
        database.ExecuteScalar<int>("SELECT COUNT(*) FROM MiscItemKeyword WHERE FormID = @0;", "000008").ShouldBe(2);
        database.ExecuteScalar<int>("SELECT COUNT(*) FROM GameplayOptionKeyword WHERE FormID = @0;", "000009").ShouldBe(2);
        database.ExecuteScalar<int>("SELECT COUNT(*) FROM MagicEffectKeyword WHERE FormID = @0;", "00000A").ShouldBe(2);
    }

    [Fact]
    public void Import_WhenKeywordHasNoName_PersistsRemainingFields()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPlugin(database);
        var importedAtUtc = DateTimeOffset.UtcNow.ToString("O");

        Import(database, "00000B", "Keyword", new KeywordRecordImporter(new KeywordRepository()), new TestKeywordRecord(null, "None"), importedAtUtc);

        var keyword = database.First<KeywordDTO>("SELECT * FROM Keyword WHERE FormID = @0;", "00000B");
        keyword.Name.ShouldBeNull();
        keyword.Color.ShouldBe("#00FFFFFF");
        keyword.KeywordType.ShouldBe("None");
        keyword.FNAM.ShouldBe("0x00000000");
    }

    private void Import(NPoco.IDatabase database, string formId, string recordType, SFRecordCompareEngine.Core.Services.Interfaces.ITypedRecordDetailImporter importer, object record, string importedAtUtc)
    {
        InsertHeader(database, formId, recordType);
        importer.Import(database, "Example.esm", formId, new RecordEnumerationDTO
        {
            RecordType = recordType,
            Record = record
        }, importedAtUtc);
    }

    private void InsertPlugin(NPoco.IDatabase database)
    {
        PluginRepository.UpsertPlugin(database, new PluginMetadataDTO
        {
            ModKey = "Example.esm",
            GameRelease = "Starfield",
            LoadOrderIndex = 1,
            PluginFileName = "Example.esm",
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

    private class TestNamedRecord(string name)
    {
        public string Name => name;
    }

    private sealed class TestKeywordRecord(string? name, string type)
    {
        public string? Name => name;
        public string Color => "#00FFFFFF";
        public string Type => type;
        public string FNAM => "0x00000000";
    }

    private class TestObjectRecord(string name)
    {
        public string Name => name;
        public string ObjectBounds => "Bounds";
        public string Model => "Model";
    }

    private sealed class TestKeywordNamedRecord(string name) : TestNamedRecord(name)
    {
        public string[] Keywords => ["000100:Example.esm", "000101:Example.esm"];
    }

    private sealed class TestKeywordObjectRecord(string name) : TestObjectRecord(name)
    {
        public string Destructible => "Destructible";
        public string[] Keywords => ["000100:Example.esm", "000101:Example.esm"];
    }

    private sealed class TestFactionRecord
    {
        public string Name => "Crimson Fleet";
        public string Keyword => "0546E0:Starfield.esm";
        public string[] Flags => ["HiddenFromPC", "TrackCrime", "IgnoreMurder"];
        public TestCrimeValues CrimeValues => new();
        public TestVendorValues VendorValues => new();
        public TestFactionRelation[] Relations =>
        [
            new("056F08:Starfield.esm", "Enemy"),
            new("15923E:Starfield.esm", null),
            new("010B30:Starfield.esm", "Ally")
        ];
    }

    private sealed class TestCrimeValues
    {
        public bool Arrest => true;
        public int Murder => 15000;
        public int Assault => 650;
        public int Trespass => 350;
        public int Pickpocket => 500;
        public double StealMultiplier => 0.5;
        public int Escape => 1500;
        public int Piracy => 1500;
        public double SmuggleMultiplier => 999519420;
    }

    private sealed class TestVendorValues
    {
        public int EndHour => 24;
        public bool BuysStolenItems => true;
        public bool BuysNonStolenItems => true;
    }

    private sealed class TestFactionRelation(string target, string? reaction)
    {
        public string Target => target;
        public string? Reaction => reaction;
    }
}
