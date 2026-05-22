using Serilog;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Migrations;

namespace SFRecordCompareEngine.Core.Database;

public class DatabaseSchemaInitializer(
    ISqliteConnectionFactory connectionFactory,
    IDatabaseMigrationRunner databaseMigrationRunner) : IDatabaseSchemaInitializer
{
    private readonly ILogger Logger = Log.ForContext<DatabaseSchemaInitializer>();

    public int Initialize()
    {
        Logger.Information("Initializing plugin database schema for {DatabasePath}", connectionFactory.DatabasePath);

        try
        {
            var migrationSchemaVersion = databaseMigrationRunner.Migrate(connectionFactory.DatabasePath);
            Logger.Information("Initialized plugin database schema for {DatabasePath} at schema version {SchemaVersion}", connectionFactory.DatabasePath, migrationSchemaVersion);

            return migrationSchemaVersion;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to initialize plugin database schema for {DatabasePath}", connectionFactory.DatabasePath);
            throw;
        }
    }
}
