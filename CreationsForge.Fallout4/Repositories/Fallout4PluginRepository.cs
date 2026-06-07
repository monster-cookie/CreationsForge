using CreationsForge.Fallout4.DTOs;
using CreationsForge.Fallout4.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Fallout4.Repositories;

public class Fallout4PluginRepository : IFallout4PluginRepository
{
    private readonly IDatabase Database;

    public Fallout4PluginRepository(IDatabase database)
    {
        Database = database;
    }

    public void Save(Fallout4PluginDTO dto)
    {
        Database.Execute(
            """
            INSERT INTO Fallout4Plugins (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, Incc)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @Incc)
            ON CONFLICT(Game, ModKey_Name, ModKey_Type, ModKey_FileName) DO UPDATE SET
                Incc = excluded.Incc;
            """,
            new
            {
                Game = dto.Game.ToString(),
                ModKeyName = dto.ModKey.Name,
                ModKeyType = dto.ModKey.Type,
                ModKeyFileName = dto.ModKey.FileName,
                dto.Incc
            });
    }
}
