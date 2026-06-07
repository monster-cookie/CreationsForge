using CreationsForge.Core.Configuration.Interfaces;

namespace CreationsForge.Core.Models.Database;

public class SqliteDatabaseOptions
{
    public SqliteDatabaseOptions(IApplicationConfigurationStore configurationStore)
    {
        DatabaseDirectory = configurationStore.Current.DatabaseDirectory;
        DatabasePath = Path.Combine(DatabaseDirectory, "CreationsForge.sqlite");
    }

    public string DatabaseDirectory { get; }

    public string DatabasePath { get; }
}
