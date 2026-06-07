using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;
using ScriptingAdapterPropertyModel = CreationsForge.Core.Models.Database.ScriptingAdapterProperty;

namespace CreationsForge.Core.Repositories;

public class ScriptingAdapterPropertyRepository : IScriptingAdapterPropertyRepository
{
    private readonly IDatabase Database;

    public ScriptingAdapterPropertyRepository(IDatabase database)
    {
        Database = database;
    }

    public void Save(ScriptingAdapterPropertyDTO dto)
    {
        Database.Save(new ScriptingAdapterPropertyModel(dto));
    }
}
