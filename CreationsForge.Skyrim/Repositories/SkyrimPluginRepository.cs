using CreationsForge.Skyrim.DTOs;
using CreationsForge.Skyrim.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Skyrim.Repositories;

public class SkyrimPluginRepository : ISkyrimPluginRepository
{
    private readonly IDatabase Database;

    public SkyrimPluginRepository(IDatabase database)
    {
        Database = database;
    }

    public void Save(SkyrimPluginDTO dto)
    {
        Database.Execute(
            """
            INSERT INTO SkyrimPlugins (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, Incc, Intv)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @Incc, @Intv)
            ON CONFLICT(Game, ModKey_Name, ModKey_Type, ModKey_FileName) DO UPDATE SET
                Incc = excluded.Incc,
                Intv = excluded.Intv;
            """,
            new
            {
                Game = dto.Game.ToString(),
                ModKeyName = dto.ModKey.Name,
                ModKeyType = dto.ModKey.Type,
                ModKeyFileName = dto.ModKey.FileName,
                dto.Incc,
                dto.Intv
            });
    }
}
