using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class CellRepository : ICellRepository
{
    public void Upsert(IDatabase database, CellDTO cell)
    {
        database.Execute(
            """
            INSERT INTO Cell (
                ModKey,
                FormID,
                Name,
                Flags,
                MajorFlags,
                LightingTemplateFormKey,
                ImageSpaceFormKey,
                LocationFormKey,
                WaterFormKey,
                WaterHeight,
                IsLinkedRefTransient,
                ImportedAtUtc
            )
            VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                Flags = excluded.Flags,
                MajorFlags = excluded.MajorFlags,
                LightingTemplateFormKey = excluded.LightingTemplateFormKey,
                ImageSpaceFormKey = excluded.ImageSpaceFormKey,
                LocationFormKey = excluded.LocationFormKey,
                WaterFormKey = excluded.WaterFormKey,
                WaterHeight = excluded.WaterHeight,
                IsLinkedRefTransient = excluded.IsLinkedRefTransient,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            cell.ModKey,
            cell.FormID,
            DbValue(cell.Name),
            DbValue(cell.Flags),
            DbValue(cell.MajorFlags),
            DbValue(cell.LightingTemplateFormKey),
            DbValue(cell.ImageSpaceFormKey),
            DbValue(cell.LocationFormKey),
            DbValue(cell.WaterFormKey),
            DbValue(cell.WaterHeight),
            DbValue(cell.IsLinkedRefTransient),
            cell.ImportedAtUtc);
    }

    public void ReplaceGroupLocations(IDatabase database, string modKey, string cellFormId, IList<CellGroupLocationDTO> locations)
    {
        database.Execute(
            "DELETE FROM CellGroupLocation WHERE ModKey = @0 COLLATE NOCASE AND CellFormID = @1;",
            modKey,
            cellFormId);

        foreach (var location in locations)
        {
            database.Execute(
                """
                INSERT INTO CellGroupLocation (
                    ModKey,
                    CellFormID,
                    LocationIndex,
                    LocationKind,
                    WorldspaceFormID,
                    BlockNumber,
                    SubBlockNumber,
                    BlockX,
                    BlockY,
                    SubBlockX,
                    SubBlockY,
                    CellIndex,
                    BlockGroupType,
                    SubBlockGroupType,
                    BlockLastModified,
                    SubBlockLastModified,
                    BlockUnknown,
                    SubBlockUnknown,
                    ImportedAtUtc
                )
                VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15, @16, @17, @18);
                """,
                location.ModKey,
                location.CellFormID,
                location.LocationIndex,
                location.LocationKind,
                DbValue(location.WorldspaceFormID),
                DbValue(location.BlockNumber),
                DbValue(location.SubBlockNumber),
                DbValue(location.BlockX),
                DbValue(location.BlockY),
                DbValue(location.SubBlockX),
                DbValue(location.SubBlockY),
                DbValue(location.CellIndex),
                DbValue(location.BlockGroupType),
                DbValue(location.SubBlockGroupType),
                DbValue(location.BlockLastModified),
                DbValue(location.SubBlockLastModified),
                DbValue(location.BlockUnknown),
                DbValue(location.SubBlockUnknown),
                location.ImportedAtUtc);
        }
    }

    public void ReplacePlacedRecords(IDatabase database, string modKey, string cellFormId, IList<CellPlacedRecordDTO> placedRecords)
    {
        database.Execute(
            "DELETE FROM CellPlacedRecord WHERE ModKey = @0 COLLATE NOCASE AND CellFormID = @1;",
            modKey,
            cellFormId);

        foreach (var placedRecord in placedRecords)
        {
            database.Execute(
                """
                INSERT INTO CellPlacedRecord (
                    ModKey,
                    CellFormID,
                    PlacementGroup,
                    ItemIndex,
                    PlacedFormKey,
                    BaseFormKey,
                    EditorID,
                    Position,
                    Rotation,
                    IsDeleted,
                    ImportedAtUtc
                )
                VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10);
                """,
                placedRecord.ModKey,
                placedRecord.CellFormID,
                placedRecord.PlacementGroup,
                placedRecord.ItemIndex,
                DbValue(placedRecord.PlacedFormKey),
                DbValue(placedRecord.BaseFormKey),
                DbValue(placedRecord.EditorID),
                DbValue(placedRecord.Position),
                DbValue(placedRecord.Rotation),
                DbValue(placedRecord.IsDeleted),
                placedRecord.ImportedAtUtc);
        }
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
