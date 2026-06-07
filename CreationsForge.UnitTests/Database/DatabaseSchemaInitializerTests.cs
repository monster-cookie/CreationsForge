using CreationsForge.Core.Database;
using CreationsForge.Core.Database.Interfaces;
using CreationsForge.Migrations;
using Moq;
using NPoco;
using Shouldly;

namespace CreationsForge.UnitTests.Database;

public class DatabaseSchemaInitializerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Initialize_ReturnsMigrationRunnerResult(bool migrationsApplied)
    {
        var connectionFactory = new TestSqliteConnectionFactory("Test.sqlite");
        var migrationRunner = new Mock<IDatabaseMigrationRunner>();
        migrationRunner
            .Setup(runner => runner.Migrate("Test.sqlite"))
            .Returns(migrationsApplied);
        var initializer = new DatabaseSchemaInitializer(connectionFactory, migrationRunner.Object);

        var result = initializer.Initialize();

        result.ShouldBe(migrationsApplied);
        migrationRunner.Verify(runner => runner.Migrate("Test.sqlite"), Times.Once);
    }

    [Fact]
    public void Initialize_WhenMigrationRunnerThrows_Rethrows()
    {
        var connectionFactory = new TestSqliteConnectionFactory("Broken.sqlite");
        var migrationRunner = new Mock<IDatabaseMigrationRunner>();
        migrationRunner
            .Setup(runner => runner.Migrate("Broken.sqlite"))
            .Throws(new InvalidOperationException("Migration failed."));
        var initializer = new DatabaseSchemaInitializer(connectionFactory, migrationRunner.Object);

        Should.Throw<InvalidOperationException>(() => initializer.Initialize())
            .Message.ShouldBe("Migration failed.");
    }

    private sealed class TestSqliteConnectionFactory : ISqliteConnectionFactory
    {
        public TestSqliteConnectionFactory(string databasePath)
        {
            DatabasePath = databasePath;
        }

        public string DatabasePath { get; }

        public IDatabase OpenDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
