using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;
using ScriptingAdapterPropertyListItemModel = CreationsForge.Core.Models.Database.ScriptingAdapterPropertyListItem;

namespace CreationsForge.Core.Repositories;

public class ScriptingAdapterPropertyListItemRepository : IScriptingAdapterPropertyListItemRepository
{
    private readonly IDatabase Database;

    public ScriptingAdapterPropertyListItemRepository(IDatabase database)
    {
        Database = database;
    }

    public void Save(ScriptingAdapterPropertyListItemDTO dto)
    {
        Database.Save(new ScriptingAdapterPropertyListItemModel(dto));
    }
}
