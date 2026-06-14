using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class BookRepository : TypedRecordRepositoryBase, IBookRepository
{
    public BookRepository(IDatabase database, IRecordInstanceRepository recordInstanceRepository)
        : base(database, recordInstanceRepository)
    { }

    public override string RecordType => RecordTypeCatalog.Book.RecordID;

    protected override string TableName => RecordTypeCatalog.Book.TableName;

    public IReadOnlyList<BookDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return FetchByFormKey<BookRow>(
                game,
                formKey,
                [
                    SelectColumn("Version2"),
                    SelectColumn("ObjectBounds_First", "ObjectBoundsFirst"),
                    SelectColumn("ObjectBounds_Second", "ObjectBoundsSecond"),
                    SelectColumn("InventoryTransform_ModKey_Name", "InventoryTransformModKeyName"),
                    SelectColumn("InventoryTransform_ModKey_Type", "InventoryTransformModKeyType"),
                    SelectColumn("InventoryTransform_ModKey_FileName", "InventoryTransformModKeyFileName"),
                    SelectColumn("InventoryTransform_FormKey_ID", "InventoryTransformFormKeyId"),
                    SelectColumn("XALG"),
                    SelectColumn("Name"),
                    SelectColumn("Text"),
                    SelectColumn("Value"),
                    SelectColumn("Weight"),
                    SelectColumn("Flags"),
                    SelectColumn("TeachesType"),
                    SelectColumn("TeachesRawContent"),
                    SelectColumn("DataSlateType"),
                    SelectColumn("Description"),
                    SelectColumn("DataSlateHeaderLeft"),
                    SelectColumn("DataSlateHeaderRight")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
    }

    public void Save(BookDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO Books (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, ObjectBounds_First, ObjectBounds_Second,
                InventoryTransform_ModKey_Name, InventoryTransform_ModKey_Type, InventoryTransform_ModKey_FileName, InventoryTransform_FormKey_ID,
                XALG, Name, Text, Value, Weight, Flags, TeachesType, TeachesRawContent, DataSlateType, Description, DataSlateHeaderLeft, DataSlateHeaderRight)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @ObjectBoundsFirst, @ObjectBoundsSecond,
                @InventoryTransformModKeyName, @InventoryTransformModKeyType, @InventoryTransformModKeyFileName, @InventoryTransformFormKeyId,
                @Xalg, @Name, @Text, @Value, @Weight, @Flags, @TeachesType, @TeachesRawContent, @DataSlateType, @Description, @DataSlateHeaderLeft, @DataSlateHeaderRight);
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
                InventoryTransformModKeyName = dto.InventoryTransformFormKey?.ModKey.Name,
                InventoryTransformModKeyType = dto.InventoryTransformFormKey?.ModKey.Type,
                InventoryTransformModKeyFileName = dto.InventoryTransformFormKey?.ModKey.FileName,
                InventoryTransformFormKeyId = dto.InventoryTransformFormKey?.Id,
                dto.Xalg,
                dto.Name,
                dto.Text,
                dto.Value,
                dto.Weight,
                dto.Flags,
                dto.TeachesType,
                dto.TeachesRawContent,
                dto.DataSlateType,
                dto.Description,
                dto.DataSlateHeaderLeft,
                dto.DataSlateHeaderRight
            });
    }

    private static BookDTO ToDTO(BookRow record, SupportedGame game)
    {
        var dto = new BookDTO
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
            InventoryTransformFormKey = CreateNullableFormKey(record.InventoryTransformModKeyName, record.InventoryTransformModKeyType, record.InventoryTransformModKeyFileName, record.InventoryTransformFormKeyId),
            Xalg = record.Xalg,
            Name = record.Name,
            Text = record.Text,
            Value = record.Value,
            Weight = record.Weight,
            Flags = record.Flags,
            TeachesType = record.TeachesType,
            TeachesRawContent = record.TeachesRawContent,
            DataSlateType = record.DataSlateType,
            Description = record.Description,
            DataSlateHeaderLeft = record.DataSlateHeaderLeft,
            DataSlateHeaderRight = record.DataSlateHeaderRight
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private sealed class BookRow : RecordRow
    {
        public int? Version2 { get; set; }

        public string? ObjectBoundsFirst { get; set; }

        public string? ObjectBoundsSecond { get; set; }

        public string? InventoryTransformModKeyName { get; set; }

        public int? InventoryTransformModKeyType { get; set; }

        public string? InventoryTransformModKeyFileName { get; set; }

        public long? InventoryTransformFormKeyId { get; set; }

        public int? Xalg { get; set; }

        public string? Name { get; set; }

        public string? Text { get; set; }

        public int? Value { get; set; }

        public float? Weight { get; set; }

        public string? Flags { get; set; }

        public string? TeachesType { get; set; }

        public string? TeachesRawContent { get; set; }

        public string? DataSlateType { get; set; }

        public string? Description { get; set; }

        public string? DataSlateHeaderLeft { get; set; }

        public string? DataSlateHeaderRight { get; set; }
    }
}
