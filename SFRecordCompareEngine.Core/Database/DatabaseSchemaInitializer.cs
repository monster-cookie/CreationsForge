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

    public void Initialize()
    {
        Logger.Information("Initializing plugin database schema for {DatabasePath}", connectionFactory.DatabasePath);

        try
        {
            databaseMigrationRunner.Migrate(connectionFactory.DatabasePath);
            Logger.Information("Initialized plugin database schema for {DatabasePath}", connectionFactory.DatabasePath);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to initialize plugin database schema for {DatabasePath}", connectionFactory.DatabasePath);
            throw;
        }
    }
}
