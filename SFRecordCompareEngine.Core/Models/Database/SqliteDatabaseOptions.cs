using SFRecordCompareEngine.Core.Configuration;

namespace SFRecordCompareEngine.Core.Models.Database;

public class SqliteDatabaseOptions
{
    public string DatabaseDirectory { get; init; } = ApplicationConfigurationStore.DefaultApplicationDataDirectory;

    public string DatabaseFileName { get; init; } = "SFRecordCompareEngine.sqlite";

    public string DatabasePath => Path.Combine(DatabaseDirectory, DatabaseFileName);

    public string LogDirectory => Path.Combine(DatabaseDirectory, "Logs");
}