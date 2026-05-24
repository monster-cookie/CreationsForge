using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class FormListRepository : IFormListRepository
{
    public void UpsertFormList(IDatabase database, FormListDTO formList)
    {
        database.Execute(
            """
            INSERT INTO FormList (
                ModKey,
                FormID,
                AddToListFormKey,
                ImportedAtUtc
            )
            VALUES (@ModKey, @FormID, @AddToListFormKey, @ImportedAtUtc)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                AddToListFormKey = excluded.AddToListFormKey,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            new
            {
                formList.ModKey,
                formList.FormID,
                AddToListFormKey = DbValue(formList.AddToListFormKey),
                formList.ImportedAtUtc
            });
    }

    public void ReplaceItems(IDatabase database, string modKey, string formId, IList<FormListItemDTO> items)
    {
        database.Execute(
            "DELETE FROM FormListItem WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;",
            new { ModKey = modKey, FormId = formId });

        foreach (var item in items)
        {
            database.Execute(
                """
                INSERT INTO FormListItem (
                    ModKey,
                    FormID,
                    ItemIndex,
                    ItemFormKey,
                    ImportedAtUtc
                )
                VALUES (@ModKey, @FormID, @ItemIndex, @ItemFormKey, @ImportedAtUtc);
                """,
                new { item.ModKey, item.FormID, item.ItemIndex, item.ItemFormKey, item.ImportedAtUtc });
        }
    }

    public IList<FormListRecordDTO> GetByModKey(IDatabase database, string modKey)
    {
        var records = database.Fetch<FormListJoinedRow>(
            """
            SELECT
                rh.*,
                fl.AddToListFormKey,
                fl.ImportedAtUtc AS FormListImportedAtUtc,
                p.LoadOrderIndex AS EffectiveLoadOrderIndex
            FROM FormList fl
            INNER JOIN RecordHeader rh
                ON rh.ModKey = fl.ModKey COLLATE NOCASE
               AND rh.FormID = fl.FormID
            INNER JOIN Plugins p
                ON p.ModKey = fl.ModKey COLLATE NOCASE
            WHERE fl.ModKey = @ModKey COLLATE NOCASE
              AND p.ImportState = @ImportState
            ORDER BY rh.FormID ASC;
            """,
            new { ModKey = modKey, ImportState = PluginImportState.Current.ToString() });

        return HydrateRecords(database, records);
    }

    public FormListRecordDTO? GetByModKeyAndFormId(IDatabase database, string modKey, string formId)
    {
        var records = database.Fetch<FormListJoinedRow>(
            """
            SELECT
                rh.*,
                fl.AddToListFormKey,
                fl.ImportedAtUtc AS FormListImportedAtUtc,
                p.LoadOrderIndex AS EffectiveLoadOrderIndex
            FROM FormList fl
            INNER JOIN RecordHeader rh
                ON rh.ModKey = fl.ModKey COLLATE NOCASE
               AND rh.FormID = fl.FormID
            INNER JOIN Plugins p
                ON p.ModKey = fl.ModKey COLLATE NOCASE
            WHERE fl.ModKey = @ModKey COLLATE NOCASE
              AND fl.FormID = @FormId
              AND p.ImportState = @ImportState;
            """,
            new { ModKey = modKey, FormId = formId, ImportState = PluginImportState.Current.ToString() });

        return HydrateRecords(database, records).FirstOrDefault();
    }

    public IList<FormListRecordDTO> GetByHierarchy(IDatabase database, string selectedModKey)
    {
        var records = FetchHierarchy(database, selectedModKey, null, null);
        return HydrateRecords(database, records);
    }

    public IList<FormListRecordDTO> GetByHierarchyAndFormId(IDatabase database, string selectedModKey, string formId)
    {
        var records = FetchHierarchy(database, selectedModKey, formId, null);
        return HydrateRecords(database, records);
    }

    public IList<FormListRecordDTO> SearchByEditorId(IDatabase database, string selectedModKey, string searchText)
    {
        var records = FetchHierarchy(database, selectedModKey, null, $"%{searchText}%");
        return HydrateRecords(database, records);
    }

    private static IList<FormListJoinedRow> FetchHierarchy(IDatabase database, string selectedModKey, string? formId, string? editorIdPattern)
    {
        return database.Fetch<FormListJoinedRow>(
            """
            SELECT
                rh.*,
                fl.AddToListFormKey,
                fl.ImportedAtUtc AS FormListImportedAtUtc,
                h.HierarchyLoadOrderIndex AS EffectiveLoadOrderIndex
            FROM PluginResolutionHierarchy h
            INNER JOIN Plugins p
                ON p.ModKey = h.HierarchyModKey COLLATE NOCASE
            INNER JOIN FormList fl
                ON fl.ModKey = h.HierarchyModKey COLLATE NOCASE
            INNER JOIN RecordHeader rh
                ON rh.ModKey = fl.ModKey COLLATE NOCASE
               AND rh.FormID = fl.FormID
            WHERE h.ChildModKey = @SelectedModKey COLLATE NOCASE
              AND p.ImportState = @ImportState
              AND (@FormId IS NULL OR fl.FormID = @FormId)
              AND (@EditorIdPattern IS NULL OR rh.EditorID LIKE @EditorIdPattern COLLATE NOCASE)
            ORDER BY
                h.HierarchyLoadOrderIndex IS NULL,
                h.HierarchyLoadOrderIndex ASC,
                h.IsChild ASC,
                rh.FormID ASC;
            """,
            new
            {
                SelectedModKey = selectedModKey,
                ImportState = PluginImportState.Current.ToString(),
                FormId = DbValue(formId),
                EditorIdPattern = DbValue(editorIdPattern)
            });
    }

    private static IList<FormListRecordDTO> HydrateRecords(IDatabase database, IList<FormListJoinedRow> rows)
    {
        var results = new List<FormListRecordDTO>();
        foreach (var row in rows)
        {
            var items = database.Fetch<FormListItemDTO>(
                """
                SELECT *
                FROM FormListItem
                WHERE ModKey = @ModKey COLLATE NOCASE
                  AND FormID = @FormID
                ORDER BY ItemIndex ASC;
                """,
                new { row.ModKey, row.FormID });

            results.Add(new FormListRecordDTO
            {
                Header = row.ToRecordHeader(),
                FormList = row.ToFormList(),
                Items = items,
                EffectiveLoadOrderIndex = row.EffectiveLoadOrderIndex
            });
        }

        return results;
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }

    private class FormListJoinedRow : RecordHeaderDTO
    {
        public string? AddToListFormKey { get; set; }
        public required string FormListImportedAtUtc { get; set; }
        public int? EffectiveLoadOrderIndex { get; set; }

        public RecordHeaderDTO ToRecordHeader()
        {
            return new RecordHeaderDTO
            {
                ModKey = ModKey,
                FormID = FormID,
                RecordType = RecordType,
                FormKey = FormKey,
                EditorID = EditorID,
                PluginFileName = PluginFileName,
                FormVersion = FormVersion,
                StarfieldMajorRecordFlags = StarfieldMajorRecordFlags,
                Version2 = Version2,
                VersionControl = VersionControl,
                ImportedAtUtc = ImportedAtUtc
            };
        }

        public FormListDTO ToFormList()
        {
            return new FormListDTO
            {
                ModKey = ModKey,
                FormID = FormID,
                AddToListFormKey = AddToListFormKey,
                ImportedAtUtc = FormListImportedAtUtc
            };
        }
    }
}
