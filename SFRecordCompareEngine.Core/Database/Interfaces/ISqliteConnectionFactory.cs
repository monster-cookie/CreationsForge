using NPoco;

namespace SFRecordCompareEngine.Core.Database.Interfaces;

public interface ISqliteConnectionFactory
{
    string DatabasePath { get; }

    IDatabase OpenDatabase();
}
