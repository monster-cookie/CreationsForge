using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class RecordComponentRepository : IRecordComponentRepository
{
    private readonly IDatabase Database;

    public RecordComponentRepository(IDatabase database)
    {
        Database = database;
    }

    public IReadOnlyList<RecordComponentDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        var components = Database.Fetch<RecordComponentRow>(
                """
                SELECT *
                FROM RecordComponents
                WHERE Game = @Game
                  AND RecordType = @RecordType
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, Component_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    RecordType = recordType,
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row => ToDTO(row, game))
            .ToList();
        var items = FetchItemsByFormKey(game, recordType, formKey);
        foreach (var component in components)
        {
            component.Items = items
                .Where(item => IsSameModKey(item.ModKey, component.ModKey) && item.ComponentIndex == component.ComponentIndex)
                .OrderBy(item => item.ItemIndex)
                .ToList();
        }

        return components;
    }

    public void ReplaceRecordComponents(IHasComponentsRecordDTO record, string recordType)
    {
        if (record is not RecordDTO recordDTO)
        {
            throw new ArgumentException($"Expected {nameof(RecordDTO)}.", nameof(record));
        }

        Database.Execute(
            """
            DELETE FROM RecordComponents
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND RecordType = @RecordType
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            new
            {
                Game = recordDTO.Game.ToString(),
                ModKeyName = recordDTO.ModKey.Name,
                ModKeyType = recordDTO.ModKey.Type,
                ModKeyFileName = recordDTO.ModKey.FileName,
                RecordType = recordType,
                FormKeyModKeyName = recordDTO.FormKey.ModKey.Name,
                FormKeyModKeyType = recordDTO.FormKey.ModKey.Type,
                FormKeyModKeyFileName = recordDTO.FormKey.ModKey.FileName,
                FormKeyId = recordDTO.FormKey.Id
            });

        foreach (var component in record.Components)
        {
            component.ImportedAtUTC = recordDTO.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO RecordComponents (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName,
                    FormKey_ID, Component_Index, MutagenObjectType, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @RecordType, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName,
                    @FormKeyId, @ComponentIndex, @MutagenObjectType, @ImportedAtUTC);
                """,
                new
                {
                    Game = component.Game.ToString(),
                    ModKeyName = component.ModKey.Name,
                    ModKeyType = component.ModKey.Type,
                    ModKeyFileName = component.ModKey.FileName,
                    component.RecordType,
                    FormKeyModKeyName = component.FormKey.ModKey.Name,
                    FormKeyModKeyType = component.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = component.FormKey.ModKey.FileName,
                    FormKeyId = component.FormKey.Id,
                    component.ComponentIndex,
                    component.MutagenObjectType,
                    component.ImportedAtUTC
                });

            foreach (var item in component.Items)
            {
                item.ImportedAtUTC = recordDTO.ImportedAtUTC;
                Database.Execute(
                    """
                    INSERT OR REPLACE INTO RecordComponentItems (
                        Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName,
                        FormKey_ID, Component_Index, Item_Index, Unknown1, Unknown2, Unknown3, Unknown4, Unknown5, ImportedAtUTC)
                    VALUES (
                        @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @RecordType, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName,
                        @FormKeyId, @ComponentIndex, @ItemIndex, @Unknown1, @Unknown2, @Unknown3, @Unknown4, @Unknown5, @ImportedAtUTC);
                    """,
                    new
                    {
                        Game = item.Game.ToString(),
                        ModKeyName = item.ModKey.Name,
                        ModKeyType = item.ModKey.Type,
                        ModKeyFileName = item.ModKey.FileName,
                        item.RecordType,
                        FormKeyModKeyName = item.FormKey.ModKey.Name,
                        FormKeyModKeyType = item.FormKey.ModKey.Type,
                        FormKeyModKeyFileName = item.FormKey.ModKey.FileName,
                        FormKeyId = item.FormKey.Id,
                        item.ComponentIndex,
                        item.ItemIndex,
                        item.Unknown1,
                        item.Unknown2,
                        item.Unknown3,
                        item.Unknown4,
                        item.Unknown5,
                        item.ImportedAtUTC
                    });
            }
        }
    }

    private IReadOnlyList<RecordComponentItemDTO> FetchItemsByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        return Database.Fetch<RecordComponentItemRow>(
                """
                SELECT *
                FROM RecordComponentItems
                WHERE Game = @Game
                  AND RecordType = @RecordType
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, Component_Index, Item_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    RecordType = recordType,
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row => ToDTO(row, game))
            .ToList();
    }

    private static RecordComponentDTO ToDTO(RecordComponentRow row, SupportedGame game)
    {
        return new RecordComponentDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            RecordType = row.RecordType,
            ComponentIndex = row.ComponentIndex,
            MutagenObjectType = row.MutagenObjectType,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static RecordComponentItemDTO ToDTO(RecordComponentItemRow row, SupportedGame game)
    {
        return new RecordComponentItemDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            RecordType = row.RecordType,
            ComponentIndex = row.ComponentIndex,
            ItemIndex = row.ItemIndex,
            Unknown1 = row.Unknown1,
            Unknown2 = row.Unknown2,
            Unknown3 = row.Unknown3,
            Unknown4 = row.Unknown4,
            Unknown5 = row.Unknown5,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static ModKeyDTO CreateModKey(string name, int type, string fileName)
    {
        return new ModKeyDTO { Name = name, Type = type, FileName = fileName };
    }

    private static FormKeyDTO CreateFormKey(string modKeyName, int modKeyType, string modKeyFileName, long formKeyId)
    {
        return new FormKeyDTO
        {
            ModKey = CreateModKey(modKeyName, modKeyType, modKeyFileName),
            Id = (uint)formKeyId
        };
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return first.Type == second.Type &&
            string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class RecordComponentRow
    {
        public string ModKeyName { get; set; } = string.Empty;
        public int ModKeyType { get; set; }
        public string ModKeyFileName { get; set; } = string.Empty;
        public string RecordType { get; set; } = string.Empty;
        public string FormKeyModKeyName { get; set; } = string.Empty;
        public int FormKeyModKeyType { get; set; }
        public string FormKeyModKeyFileName { get; set; } = string.Empty;
        public long FormKeyId { get; set; }
        public int ComponentIndex { get; set; }
        public string MutagenObjectType { get; set; } = string.Empty;
        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class RecordComponentItemRow
    {
        public string ModKeyName { get; set; } = string.Empty;
        public int ModKeyType { get; set; }
        public string ModKeyFileName { get; set; } = string.Empty;
        public string RecordType { get; set; } = string.Empty;
        public string FormKeyModKeyName { get; set; } = string.Empty;
        public int FormKeyModKeyType { get; set; }
        public string FormKeyModKeyFileName { get; set; } = string.Empty;
        public long FormKeyId { get; set; }
        public int ComponentIndex { get; set; }
        public int ItemIndex { get; set; }
        public double? Unknown1 { get; set; }
        public double? Unknown2 { get; set; }
        public double? Unknown3 { get; set; }
        public double? Unknown4 { get; set; }
        public double? Unknown5 { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }
}
