using Serilog;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Migrations;

namespace SFRecordCompareEngine.Core.Database;

public class DatabaseSchemaInitializer : IDatabaseSchemaInitializer
{
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly IDatabaseMigrationRunner DatabaseMigrationRunner;
    private readonly ILogger Logger = Log.ForContext<DatabaseSchemaInitializer>();

    public DatabaseSchemaInitializer(
        ISqliteConnectionFactory connectionFactory,
        IDatabaseMigrationRunner databaseMigrationRunner)
    {
        ConnectionFactory = connectionFactory;
        DatabaseMigrationRunner = databaseMigrationRunner;
    }

    public bool Initialize()
    {
        Logger.Information("Initializing plugin database schema for {DatabasePath}", ConnectionFactory.DatabasePath);

        try
        {
            var migrationsApplied = DatabaseMigrationRunner.Migrate(ConnectionFactory.DatabasePath);
            Logger.Information("Initialized plugin database schema for {DatabasePath}; migrations applied: {MigrationsApplied}", ConnectionFactory.DatabasePath, migrationsApplied);
            return migrationsApplied;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to initialize plugin database schema for {DatabasePath}", ConnectionFactory.DatabasePath);
            throw;
        }
    }
}
