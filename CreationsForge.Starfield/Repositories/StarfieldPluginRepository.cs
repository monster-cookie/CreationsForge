using CreationsForge.Starfield.DTOs;
using CreationsForge.Starfield.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Starfield.Repositories;

public class StarfieldPluginRepository : IStarfieldPluginRepository
{
    private readonly IDatabase Database;

    public StarfieldPluginRepository(IDatabase database)
    {
        Database = database;
    }

    public void Save(StarfieldPluginDTO dto)
    {
        Database.Execute(
            """
            INSERT INTO StarfieldPlugins (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, Branch, InteriorCellCount, Intv)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @Branch, @InteriorCellCount, @Intv)
            ON CONFLICT(Game, ModKey_Name, ModKey_Type, ModKey_FileName) DO UPDATE SET
                Branch = excluded.Branch,
                InteriorCellCount = excluded.InteriorCellCount,
                Intv = excluded.Intv;
            """,
            new
            {
                Game = dto.Game.ToString(),
                ModKeyName = dto.ModKey.Name,
                ModKeyType = dto.ModKey.Type,
                ModKeyFileName = dto.ModKey.FileName,
                dto.Branch,
                dto.InteriorCellCount,
                dto.Intv
            });
    }
}
