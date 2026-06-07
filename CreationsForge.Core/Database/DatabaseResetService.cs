using CreationsForge.Core.Database.Interfaces;
using Serilog;

namespace CreationsForge.Core.Database;

public class DatabaseResetService : IDatabaseResetService
{
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly ILogger Logger = Log.ForContext<DatabaseResetService>();

    public DatabaseResetService(ISqliteConnectionFactory connectionFactory)
    {
        ConnectionFactory = connectionFactory;
    }

    public void Reset()
    {
        Logger.Information("Resetting application database at {DatabasePath}", ConnectionFactory.DatabasePath);
        DeleteIfExists(ConnectionFactory.DatabasePath);
        DeleteIfExists(ConnectionFactory.DatabasePath + "-wal");
        DeleteIfExists(ConnectionFactory.DatabasePath + "-shm");
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
