using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class GameRepository : IGameRepository
{
    private readonly IDatabase Database;

    public GameRepository(IDatabase database)
    {
        Database = database;
    }

    public void Save(GameDTO dto)
    {
        Database.Execute(
            """
            INSERT INTO Games (Game, DisplayName, InstallationFolder, DataFolder, ImportedAtUTC)
            VALUES (@Game, @DisplayName, @InstallationFolder, @DataFolder, @ImportedAtUTC)
            ON CONFLICT(Game) DO UPDATE SET
                DisplayName = excluded.DisplayName,
                InstallationFolder = excluded.InstallationFolder,
                DataFolder = excluded.DataFolder,
                ImportedAtUTC = excluded.ImportedAtUTC;
            """,
            new
            {
                Game = dto.Game.ToString(),
                dto.DisplayName,
                dto.InstallationFolder,
                dto.DataFolder,
                dto.ImportedAtUTC
            });
    }
}
