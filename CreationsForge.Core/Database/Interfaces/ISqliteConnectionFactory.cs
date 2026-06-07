using NPoco;

namespace CreationsForge.Core.Database.Interfaces;

public interface ISqliteConnectionFactory
{
    string DatabasePath { get; }

    IDatabase OpenDatabase();
}
