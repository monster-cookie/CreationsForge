using Serilog;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Migrations;

namespace SFRecordCompareEngine.Core.Database;

public class DatabaseSchemaInitializer : IDatabaseSchemaInitializer
{
    private readonly ILogger Logger = Log.ForContext<DatabaseSchemaInitializer>();

    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly IDatabaseMigrationRunner DatabaseMigrationRunner;
    
    public DatabaseSchemaInitializer(
        ISqliteConnectionFactory connectionFactory,
        IDatabaseMigrationRunner databaseMigrationRunner)
    {
        ConnectionFactory = connectionFactory;
        DatabaseMigrationRunner = databaseMigrationRunner;
    }
    
    public void Initialize()
    {
        Logger.Information("Initializing plugin database schema for {DatabasePath}", ConnectionFactory.DatabasePath);

        try
        {
            DatabaseMigrationRunner.Migrate(ConnectionFactory.DatabasePath);
            Logger.Information("Initialized plugin database schema for {DatabasePath}", ConnectionFactory.DatabasePath);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to initialize plugin database schema for {DatabasePath}", ConnectionFactory.DatabasePath);
            throw;
        }
    }
}
