using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class WorldspaceRepository : IWorldspaceRepository
{
    public void Upsert(IDatabase database, WorldspaceDTO worldspace)
    {
        database.Execute(
            """
            INSERT INTO Worldspace (
                ModKey,
                FormID,
                Name,
                ParentWorldspaceFormKey,
                ClimateFormKey,
                WaterFormKey,
                TopCellFormKey,
                WorldMapCellOffset,
                WorldMapOffsetScale,
                ImportedAtUtc
            )
            VALUES (@ModKey, @FormID, @Name, @ParentWorldspaceFormKey, @ClimateFormKey, @WaterFormKey, @TopCellFormKey, @WorldMapCellOffset, @WorldMapOffsetScale, @ImportedAtUtc)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                ParentWorldspaceFormKey = excluded.ParentWorldspaceFormKey,
                ClimateFormKey = excluded.ClimateFormKey,
                WaterFormKey = excluded.WaterFormKey,
                TopCellFormKey = excluded.TopCellFormKey,
                WorldMapCellOffset = excluded.WorldMapCellOffset,
                WorldMapOffsetScale = excluded.WorldMapOffsetScale,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            new
            {
                worldspace.ModKey,
                worldspace.FormID,
                Name = DbValue(worldspace.Name),
                ParentWorldspaceFormKey = DbValue(worldspace.ParentWorldspaceFormKey),
                ClimateFormKey = DbValue(worldspace.ClimateFormKey),
                WaterFormKey = DbValue(worldspace.WaterFormKey),
                TopCellFormKey = DbValue(worldspace.TopCellFormKey),
                WorldMapCellOffset = DbValue(worldspace.WorldMapCellOffset),
                WorldMapOffsetScale = DbValue(worldspace.WorldMapOffsetScale),
                worldspace.ImportedAtUtc
            });
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
