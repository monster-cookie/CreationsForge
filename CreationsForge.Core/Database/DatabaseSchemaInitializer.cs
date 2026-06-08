using CreationsForge.Core.Database.Interfaces;
using CreationsForge.Migrations;
using Serilog;

namespace CreationsForge.Core.Database;

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
        Logger.Information("Initializing database schema for {DatabasePath}", ConnectionFactory.DatabasePath);

        try
        {
            var migrationsApplied = DatabaseMigrationRunner.Migrate(ConnectionFactory.DatabasePath);
            Logger.Information("Initialized database schema for {DatabasePath}; migrations applied: {MigrationsApplied}", ConnectionFactory.DatabasePath, migrationsApplied);
            return migrationsApplied;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to initialize database schema for {DatabasePath}", ConnectionFactory.DatabasePath);
            throw;
        }
    }
}
