using Mutagen.Bethesda.Plugins;
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
            VALUES (@ModKey, @FormID, @Name, @Flags, @MajorFlags, @LightingTemplateFormKey, @ImageSpaceFormKey, @LocationFormKey, @WaterFormKey, @WaterHeight, @IsLinkedRefTransient, @ImportedAtUtc)
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
            new
            {
                ModKey = cell.ModKey.FileName,
                cell.FormID,
                Name = DbValue(cell.Name),
                Flags = DbValue(cell.Flags),
                MajorFlags = DbValue(cell.MajorFlags),
                LightingTemplateFormKey = DbValue(cell.LightingTemplateFormKey),
                ImageSpaceFormKey = DbValue(cell.ImageSpaceFormKey),
                LocationFormKey = DbValue(cell.LocationFormKey),
                WaterFormKey = DbValue(cell.WaterFormKey),
                WaterHeight = DbValue(cell.WaterHeight),
                IsLinkedRefTransient = DbValue(cell.IsLinkedRefTransient),
                cell.ImportedAtUtc
            });
    }

    public void ReplaceGroupLocations(IDatabase database, ModKey modKey, string cellFormId, IList<CellGroupLocationDTO> locations)
    {
        database.Execute(
            "DELETE FROM CellGroupLocation WHERE ModKey = @ModKey COLLATE NOCASE AND CellFormID = @CellFormId;",
            new { ModKey = modKey.FileName, CellFormId = cellFormId });

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
                VALUES (@ModKey, @CellFormID, @LocationIndex, @LocationKind, @WorldspaceFormID, @BlockNumber, @SubBlockNumber, @BlockX, @BlockY, @SubBlockX, @SubBlockY, @CellIndex, @BlockGroupType, @SubBlockGroupType, @BlockLastModified, @SubBlockLastModified, @BlockUnknown, @SubBlockUnknown, @ImportedAtUtc);
                """,
                new
                {
                    ModKey = location.ModKey.FileName,
                    location.CellFormID,
                    location.LocationIndex,
                    location.LocationKind,
                    WorldspaceFormID = DbValue(location.WorldspaceFormID),
                    BlockNumber = DbValue(location.BlockNumber),
                    SubBlockNumber = DbValue(location.SubBlockNumber),
                    BlockX = DbValue(location.BlockX),
                    BlockY = DbValue(location.BlockY),
                    SubBlockX = DbValue(location.SubBlockX),
                    SubBlockY = DbValue(location.SubBlockY),
                    CellIndex = DbValue(location.CellIndex),
                    BlockGroupType = DbValue(location.BlockGroupType),
                    SubBlockGroupType = DbValue(location.SubBlockGroupType),
                    BlockLastModified = DbValue(location.BlockLastModified),
                    SubBlockLastModified = DbValue(location.SubBlockLastModified),
                    BlockUnknown = DbValue(location.BlockUnknown),
                    SubBlockUnknown = DbValue(location.SubBlockUnknown),
                    location.ImportedAtUtc
                });
        }
    }

    public void ReplacePlacedRecords(IDatabase database, ModKey modKey, string cellFormId, IList<CellPlacedRecordDTO> placedRecords)
    {
        database.Execute(
            "DELETE FROM CellPlacedRecord WHERE ModKey = @ModKey COLLATE NOCASE AND CellFormID = @CellFormId;",
            new { ModKey = modKey.FileName, CellFormId = cellFormId });

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
                VALUES (@ModKey, @CellFormID, @PlacementGroup, @ItemIndex, @PlacedFormKey, @BaseFormKey, @EditorID, @Position, @Rotation, @IsDeleted, @ImportedAtUtc);
                """,
                new
                {
                    ModKey = placedRecord.ModKey.FileName,
                    placedRecord.CellFormID,
                    placedRecord.PlacementGroup,
                    placedRecord.ItemIndex,
                    PlacedFormKey = DbValue(placedRecord.PlacedFormKey),
                    BaseFormKey = DbValue(placedRecord.BaseFormKey),
                    EditorID = DbValue(placedRecord.EditorID),
                    Position = DbValue(placedRecord.Position),
                    Rotation = DbValue(placedRecord.Rotation),
                    IsDeleted = DbValue(placedRecord.IsDeleted),
                    placedRecord.ImportedAtUtc
                });
        }
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
