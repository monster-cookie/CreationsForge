using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories;
using CreationsForge.Core.Repositories.Interfaces;
using Moq;
using NPoco;
using Shouldly;

namespace CreationsForge.UnitTests.Repositories;

public class TypedRecordRepositoryBaseTests
{
    public static TheoryData<string> AllowedTableNames =>
        new()
        {
            RecordTypeCatalog.ActorValueInformation.TableName,
            RecordTypeCatalog.Book.TableName,
            RecordTypeCatalog.Container.TableName,
            RecordTypeCatalog.Door.TableName,
            RecordTypeCatalog.Keyword.TableName,
            RecordTypeCatalog.MagicEffect.TableName,
            RecordTypeCatalog.MiscObject.TableName,
            RecordTypeCatalog.NPC.TableName,
            RecordTypeCatalog.Perk.TableName,
            RecordTypeCatalog.Static.TableName,
            RecordTypeCatalog.Terminal.TableName
        };

    public static TheoryData<string> UnsafeIdentifiers =>
        new()
        {
            "MiscItems; DROP TABLE Plugins",
            "Name, Other",
            "Name --",
            "COUNT(*)",
            "Name AS Bad Alias",
            "CurrentRecord.Name",
            "Bad Alias",
            string.Empty
        };

    [Theory]
    [MemberData(nameof(AllowedTableNames))]
    public void GetValidatedTableName_AllowsKnownTypedRecordTables(string tableName)
    {
        var repository = new TestTypedRecordRepository(tableName);

        repository.ValidateTableName().ShouldBe(tableName);
    }

    [Theory]
    [MemberData(nameof(UnsafeIdentifiers))]
    public void GetValidatedTableName_RejectsUnsafeTableNames(string tableName)
    {
        var repository = new TestTypedRecordRepository(tableName);

        Should.Throw<InvalidOperationException>(() => repository.ValidateTableName());
    }

    [Fact]
    public void SelectColumn_RendersSimpleColumn()
    {
        var repository = new TestTypedRecordRepository(RecordTypeCatalog.MiscObject.TableName);

        repository.RenderColumn("Name").ShouldBe(", CurrentRecord.Name");
    }

    [Fact]
    public void SelectColumn_RendersAliasedColumn()
    {
        var repository = new TestTypedRecordRepository(RecordTypeCatalog.MiscObject.TableName);

        repository.RenderColumn("FeaturedItemMessage_FormKey_ID", "FeaturedItemMessageFormKeyId")
            .ShouldBe(", CurrentRecord.FeaturedItemMessage_FormKey_ID AS FeaturedItemMessageFormKeyId");
    }

    [Theory]
    [MemberData(nameof(UnsafeIdentifiers))]
    public void SelectColumn_RejectsUnsafeColumnNames(string columnName)
    {
        var repository = new TestTypedRecordRepository(RecordTypeCatalog.MiscObject.TableName);

        Should.Throw<ArgumentException>(() => repository.RenderColumn(columnName));
    }

    [Theory]
    [MemberData(nameof(UnsafeIdentifiers))]
    public void SelectColumn_RejectsUnsafeAliases(string alias)
    {
        var repository = new TestTypedRecordRepository(RecordTypeCatalog.MiscObject.TableName);

        Should.Throw<ArgumentException>(() => repository.RenderColumn("Name", alias));
    }

    private sealed class TestTypedRecordRepository : TypedRecordRepositoryBase
    {
        private readonly string TestTableName;

        public TestTypedRecordRepository(string tableName)
            : base(Mock.Of<IDatabase>(), Mock.Of<IRecordInstanceRepository>())
        {
            TestTableName = tableName;
        }

        public override string RecordType => "TEST";

        protected override string TableName => TestTableName;

        public string ValidateTableName()
        {
            return GetValidatedTableName();
        }

        public string RenderColumn(string columnName)
        {
            return SelectColumn(columnName).ToSqlFragment();
        }

        public string RenderColumn(string columnName, string alias)
        {
            return SelectColumn(columnName, alias).ToSqlFragment();
        }
    }
}
