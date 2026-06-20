using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class ContainerRepository : TypedRecordRepositoryBase, IContainerRepository
{
    public ContainerRepository(IDatabase database, IRecordInstanceRepository recordInstanceRepository)
        : base(database, recordInstanceRepository)
    { }

    public override string RecordType => RecordTypeCatalog.Container.RecordID;

    protected override string TableName => RecordTypeCatalog.Container.TableName;

    public IReadOnlyList<ContainerDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FetchByFormKey<ContainerRow>(
                game,
                formKey,
                [
                    SelectColumn("Version2"),
                    SelectColumn("ObjectBounds_First", "ObjectBoundsFirst"),
                    SelectColumn("ObjectBounds_Second", "ObjectBoundsSecond"),
                    SelectColumn("Name"),
                    SelectColumn("Flags"),
                    SelectColumn("MajorFlags"),
                    SelectColumn("NativeTerminal_ModKey_Name", "NativeTerminalModKeyName"),
                    SelectColumn("NativeTerminal_ModKey_Type", "NativeTerminalModKeyType"),
                    SelectColumn("NativeTerminal_ModKey_FileName", "NativeTerminalModKeyFileName"),
                    SelectColumn("NativeTerminal_FormKey_ID", "NativeTerminalFormKeyId")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var items = FetchItemsByFormKey(game, formKey);
        foreach (var record in records)
        {
            record.Items = items
                .Where(item => IsSameModKey(item.ModKey, record.ModKey))
                .OrderBy(item => item.ItemIndex)
                .ToList();
        }

        return records;
    }

    public void Save(ContainerDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO Containers (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, ObjectBounds_First, ObjectBounds_Second, Name, Flags,
                MajorFlags, NativeTerminal_ModKey_Name, NativeTerminal_ModKey_Type, NativeTerminal_ModKey_FileName, NativeTerminal_FormKey_ID)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @ObjectBoundsFirst, @ObjectBoundsSecond, @Name, @Flags,
                @MajorFlags, @NativeTerminalModKeyName, @NativeTerminalModKeyType, @NativeTerminalModKeyFileName, @NativeTerminalFormKeyId);
            """,
            new
            {
                Game = dto.Game.ToString(),
                ModKeyName = dto.ModKey.Name,
                ModKeyType = dto.ModKey.Type,
                ModKeyFileName = dto.ModKey.FileName,
                FormKeyModKeyName = dto.FormKey.ModKey.Name,
                FormKeyModKeyType = dto.FormKey.ModKey.Type,
                FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                FormKeyId = dto.FormKey.Id,
                EditorId = dto.EditorID,
                dto.FormVersion,
                dto.MajorRecordFlags,
                dto.ImportedAtUTC,
                dto.Version2,
                dto.ObjectBoundsFirst,
                dto.ObjectBoundsSecond,
                Name = GetEnglishText(dto.Name),
                dto.Flags,
                dto.MajorFlags,
                NativeTerminalModKeyName = dto.NativeTerminalFormKey?.ModKey.Name,
                NativeTerminalModKeyType = dto.NativeTerminalFormKey?.ModKey.Type,
                NativeTerminalModKeyFileName = dto.NativeTerminalFormKey?.ModKey.FileName,
                NativeTerminalFormKeyId = dto.NativeTerminalFormKey?.Id
            });
        ReplaceItems(dto);
    }

    private IReadOnlyList<ContainerItemDTO> FetchItemsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<ContainerItemRow>(
                """
                SELECT *
                FROM ContainerItems
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, Item_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row => ToDTO(row, game))
            .ToList();
    }

    private void ReplaceItems(ContainerDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM ContainerItems
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            new
            {
                Game = dto.Game.ToString(),
                ModKeyName = dto.ModKey.Name,
                ModKeyType = dto.ModKey.Type,
                ModKeyFileName = dto.ModKey.FileName,
                FormKeyModKeyName = dto.FormKey.ModKey.Name,
                FormKeyModKeyType = dto.FormKey.ModKey.Type,
                FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                FormKeyId = dto.FormKey.Id
            });

        foreach (var item in dto.Items)
        {
            item.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO ContainerItems (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Item_Index, Item_ModKey_Name, Item_ModKey_Type, Item_ModKey_FileName, Item_FormKey_ID, Count, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @ItemIndex, @ItemModKeyName, @ItemModKeyType, @ItemModKeyFileName, @ItemFormKeyId, @Count, @ImportedAtUTC);
                """,
                new
                {
                    Game = item.Game.ToString(),
                    ModKeyName = item.ModKey.Name,
                    ModKeyType = item.ModKey.Type,
                    ModKeyFileName = item.ModKey.FileName,
                    FormKeyModKeyName = item.FormKey.ModKey.Name,
                    FormKeyModKeyType = item.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = item.FormKey.ModKey.FileName,
                    FormKeyId = item.FormKey.Id,
                    item.ItemIndex,
                    ItemModKeyName = item.ItemFormKey.ModKey.Name,
                    ItemModKeyType = item.ItemFormKey.ModKey.Type,
                    ItemModKeyFileName = item.ItemFormKey.ModKey.FileName,
                    ItemFormKeyId = item.ItemFormKey.Id,
                    item.Count,
                    item.ImportedAtUTC
                });
        }
    }

    private static ContainerDTO ToDTO(ContainerRow record, SupportedGame game)
    {
        var dto = new ContainerDTO
        {
            Game = game,
            ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new FormKeyDTO { ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Version2 = record.Version2,
            ObjectBoundsFirst = record.ObjectBoundsFirst,
            ObjectBoundsSecond = record.ObjectBoundsSecond,
            Name = FromEnglish(record.Name),
            Flags = record.Flags,
            MajorFlags = record.MajorFlags,
            NativeTerminalFormKey = CreateNullableFormKey(record.NativeTerminalModKeyName, record.NativeTerminalModKeyType, record.NativeTerminalModKeyFileName, record.NativeTerminalFormKeyId)
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static ContainerItemDTO ToDTO(ContainerItemRow row, SupportedGame game)
    {
        return new ContainerItemDTO
        {
            Game = game,
            ModKey = new ModKeyDTO
            {
                Name = row.ModKeyName,
                Type = row.ModKeyType,
                FileName = row.ModKeyFileName
            },
            FormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = row.FormKeyModKeyName,
                    Type = row.FormKeyModKeyType,
                    FileName = row.FormKeyModKeyFileName
                },
                Id = (uint)row.FormKeyId
            },
            ItemIndex = row.ItemIndex,
            ItemFormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = row.ItemModKeyName,
                    Type = row.ItemModKeyType,
                    FileName = row.ItemModKeyFileName
                },
                Id = (uint)row.ItemFormKeyId
            },
            Count = row.Count,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return first.Type == second.Type &&
            string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class ContainerRow : RecordRow
    {
        public int? Version2 { get; set; }

        public string? ObjectBoundsFirst { get; set; }

        public string? ObjectBoundsSecond { get; set; }

        public string? Name { get; set; }

        public string? Flags { get; set; }

        public string? MajorFlags { get; set; }

        public string? NativeTerminalModKeyName { get; set; }

        public int? NativeTerminalModKeyType { get; set; }

        public string? NativeTerminalModKeyFileName { get; set; }

        public long? NativeTerminalFormKeyId { get; set; }
    }

    private sealed class ContainerItemRow
    {
        public string Game { get; set; } = string.Empty;

        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public int ItemIndex { get; set; }

        public string ItemModKeyName { get; set; } = string.Empty;

        public int ItemModKeyType { get; set; }

        public string ItemModKeyFileName { get; set; } = string.Empty;

        public long ItemFormKeyId { get; set; }

        public int? Count { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }
}
