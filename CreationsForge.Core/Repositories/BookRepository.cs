using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class BookRepository : TypedRecordRepositoryBase, IBookRepository
{
    private readonly IModelRepository ModelRepository;
    private readonly IKeywordMappingRepository KeywordMappingRepository;
    private readonly ISoundMappingRepository SoundMappingRepository;
    private readonly IScriptingAdapterRepository ScriptingAdapterRepository;
    private readonly IRecordComponentRepository RecordComponentRepository;
    private readonly IReflectionRepository ReflectionRepository;

    public BookRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        IModelRepository modelRepository,
        IKeywordMappingRepository keywordMappingRepository,
        ISoundMappingRepository soundMappingRepository,
        IScriptingAdapterRepository scriptingAdapterRepository,
        IRecordComponentRepository recordComponentRepository,
        IReflectionRepository reflectionRepository)
        : base(database, recordInstanceRepository)
    {
        ModelRepository = modelRepository;
        KeywordMappingRepository = keywordMappingRepository;
        SoundMappingRepository = soundMappingRepository;
        ScriptingAdapterRepository = scriptingAdapterRepository;
        RecordComponentRepository = recordComponentRepository;
        ReflectionRepository = reflectionRepository;
    }

    public override string RecordType => RecordTypeCatalog.Book.RecordID;

    protected override string TableName => RecordTypeCatalog.Book.TableName;

    public IReadOnlyList<BookDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FetchByFormKey<BookRow>(
                game,
                formKey,
                [
                    SelectColumn("Version2"),
                    SelectColumn("VersionControl"),
                    SelectColumn("ObjectBounds_First", "ObjectBoundsFirst"),
                    SelectColumn("ObjectBounds_Second", "ObjectBoundsSecond"),
                    SelectColumn("Transforms_Inventory_ModKey_Name", "TransformsInventoryModKeyName"),
                    SelectColumn("Transforms_Inventory_ModKey_Type", "TransformsInventoryModKeyType"),
                    SelectColumn("Transforms_Inventory_ModKey_FileName", "TransformsInventoryModKeyFileName"),
                    SelectColumn("Transforms_Inventory_FormKey_ID", "TransformsInventoryFormKeyId"),
                    SelectColumn("InventoryArt_ModKey_Name", "InventoryArtModKeyName"),
                    SelectColumn("InventoryArt_ModKey_Type", "InventoryArtModKeyType"),
                    SelectColumn("InventoryArt_ModKey_FileName", "InventoryArtModKeyFileName"),
                    SelectColumn("InventoryArt_FormKey_ID", "InventoryArtFormKeyId"),
                    SelectColumn("PreviewTransform_ModKey_Name", "PreviewTransformModKeyName"),
                    SelectColumn("PreviewTransform_ModKey_Type", "PreviewTransformModKeyType"),
                    SelectColumn("PreviewTransform_ModKey_FileName", "PreviewTransformModKeyFileName"),
                    SelectColumn("PreviewTransform_FormKey_ID", "PreviewTransformFormKeyId"),
                    SelectColumn("FeaturedItemMessage_ModKey_Name", "FeaturedItemMessageModKeyName"),
                    SelectColumn("FeaturedItemMessage_ModKey_Type", "FeaturedItemMessageModKeyType"),
                    SelectColumn("FeaturedItemMessage_ModKey_FileName", "FeaturedItemMessageModKeyFileName"),
                    SelectColumn("FeaturedItemMessage_FormKey_ID", "FeaturedItemMessageFormKeyId"),
                    SelectColumn("XALG"),
                    SelectColumn("Name"),
                    SelectColumn("Text"),
                    SelectColumn("Value"),
                    SelectColumn("Weight"),
                    SelectColumn("Flags"),
                    SelectColumn("Teaches_MutagenObjectType", "TeachesMutagenObjectType"),
                    SelectColumn("Teaches_Perk_ModKey_Name", "TeachesPerkModKeyName"),
                    SelectColumn("Teaches_Perk_ModKey_Type", "TeachesPerkModKeyType"),
                    SelectColumn("Teaches_Perk_ModKey_FileName", "TeachesPerkModKeyFileName"),
                    SelectColumn("Teaches_Perk_FormKey_ID", "TeachesPerkFormKeyId"),
                    SelectColumn("Teaches_RawContent", "TeachesRawContent"),
                    SelectColumn("DataSlateType"),
                    SelectColumn("Description"),
                    SelectColumn("DataSlateHeaderLeft"),
                    SelectColumn("DataSlateHeaderRight")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var models = ModelRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey);
        var keywords = KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey);
        var sounds = SoundMappingRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey);
        var scriptingAdapters = ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey);
        var components = RecordComponentRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey);
        var reflections = ReflectionRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey);
        foreach (var record in records)
        {
            record.Models = models.Where(model => IsSameModKey(model.ModKey, record.ModKey)).OrderBy(model => model.ModelSlot).ThenBy(model => model.ModelGender).ToList();
            record.Keywords = keywords.Where(keyword => IsSameModKey(keyword.ModKey, record.ModKey)).OrderBy(keyword => keyword.KeywordIndex).ToList();
            record.Sounds = sounds.Where(sound => IsSameModKey(sound.ModKey, record.ModKey)).OrderBy(sound => sound.SoundIndex).ToList();
            record.ScriptingAdapters = scriptingAdapters.Where(adapter => IsSameModKey(adapter.ModKey, record.ModKey)).OrderBy(adapter => adapter.ScriptIndex).ToList();
            record.Components = components.Where(component => IsSameModKey(component.ModKey, record.ModKey)).OrderBy(component => component.ComponentIndex).ToList();
            record.Reflections = reflections.Where(reflection => IsSameModKey(reflection.ModKey, record.ModKey)).OrderBy(reflection => reflection.ComponentIndex).ToList();
        }

        return records;
    }

    public void Save(BookDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO Books (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, VersionControl, ObjectBounds_First, ObjectBounds_Second,
                Transforms_Inventory_ModKey_Name, Transforms_Inventory_ModKey_Type, Transforms_Inventory_ModKey_FileName, Transforms_Inventory_FormKey_ID,
                InventoryArt_ModKey_Name, InventoryArt_ModKey_Type, InventoryArt_ModKey_FileName, InventoryArt_FormKey_ID,
                PreviewTransform_ModKey_Name, PreviewTransform_ModKey_Type, PreviewTransform_ModKey_FileName, PreviewTransform_FormKey_ID,
                FeaturedItemMessage_ModKey_Name, FeaturedItemMessage_ModKey_Type, FeaturedItemMessage_ModKey_FileName, FeaturedItemMessage_FormKey_ID,
                XALG, Name, Text, Value, Weight, Flags, Teaches_MutagenObjectType,
                Teaches_Perk_ModKey_Name, Teaches_Perk_ModKey_Type, Teaches_Perk_ModKey_FileName, Teaches_Perk_FormKey_ID,
                Teaches_RawContent, DataSlateType, Description, DataSlateHeaderLeft, DataSlateHeaderRight)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @VersionControl, @ObjectBoundsFirst, @ObjectBoundsSecond,
                @TransformsInventoryModKeyName, @TransformsInventoryModKeyType, @TransformsInventoryModKeyFileName, @TransformsInventoryFormKeyId,
                @InventoryArtModKeyName, @InventoryArtModKeyType, @InventoryArtModKeyFileName, @InventoryArtFormKeyId,
                @PreviewTransformModKeyName, @PreviewTransformModKeyType, @PreviewTransformModKeyFileName, @PreviewTransformFormKeyId,
                @FeaturedItemMessageModKeyName, @FeaturedItemMessageModKeyType, @FeaturedItemMessageModKeyFileName, @FeaturedItemMessageFormKeyId,
                @XALG, @Name, @Text, @Value, @Weight, @Flags, @TeachesMutagenObjectType,
                @TeachesPerkModKeyName, @TeachesPerkModKeyType, @TeachesPerkModKeyFileName, @TeachesPerkFormKeyId,
                @TeachesRawContent, @DataSlateType, @Description, @DataSlateHeaderLeft, @DataSlateHeaderRight);
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
                dto.VersionControl,
                ObjectBoundsFirst = dto.ObjectBounds?.First,
                ObjectBoundsSecond = dto.ObjectBounds?.Second,
                TransformsInventoryModKeyName = dto.Transforms?.Inventory?.ModKey.Name,
                TransformsInventoryModKeyType = dto.Transforms?.Inventory?.ModKey.Type,
                TransformsInventoryModKeyFileName = dto.Transforms?.Inventory?.ModKey.FileName,
                TransformsInventoryFormKeyId = dto.Transforms?.Inventory?.Id,
                InventoryArtModKeyName = dto.InventoryArt?.ModKey.Name,
                InventoryArtModKeyType = dto.InventoryArt?.ModKey.Type,
                InventoryArtModKeyFileName = dto.InventoryArt?.ModKey.FileName,
                InventoryArtFormKeyId = dto.InventoryArt?.Id,
                PreviewTransformModKeyName = dto.PreviewTransform?.ModKey.Name,
                PreviewTransformModKeyType = dto.PreviewTransform?.ModKey.Type,
                PreviewTransformModKeyFileName = dto.PreviewTransform?.ModKey.FileName,
                PreviewTransformFormKeyId = dto.PreviewTransform?.Id,
                FeaturedItemMessageModKeyName = dto.FeaturedItemMessage?.ModKey.Name,
                FeaturedItemMessageModKeyType = dto.FeaturedItemMessage?.ModKey.Type,
                FeaturedItemMessageModKeyFileName = dto.FeaturedItemMessage?.ModKey.FileName,
                FeaturedItemMessageFormKeyId = dto.FeaturedItemMessage?.Id,
                dto.XALG,
                Name = GetEnglishText(dto.Name),
                Text = GetEnglishText(dto.Text),
                dto.Value,
                dto.Weight,
                dto.Flags,
                TeachesMutagenObjectType = dto.Teaches?.MutagenObjectType,
                TeachesPerkModKeyName = dto.Teaches?.Perk?.ModKey.Name,
                TeachesPerkModKeyType = dto.Teaches?.Perk?.ModKey.Type,
                TeachesPerkModKeyFileName = dto.Teaches?.Perk?.ModKey.FileName,
                TeachesPerkFormKeyId = dto.Teaches?.Perk?.Id,
                TeachesRawContent = dto.Teaches?.RawContent,
                dto.DataSlateType,
                Description = GetEnglishText(dto.Description),
                DataSlateHeaderLeft = GetEnglishText(dto.DataSlateHeaderLeft),
                DataSlateHeaderRight = GetEnglishText(dto.DataSlateHeaderRight)
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
            VersionControl = record.VersionControl,
            ObjectBounds = new ObjectBoundsDTO
            {
                First = record.ObjectBoundsFirst,
                Second = record.ObjectBoundsSecond
            },
            Transforms = new BookTransformsDTO
            {
                Inventory = CreateNullableFormKey(record.TransformsInventoryModKeyName, record.TransformsInventoryModKeyType, record.TransformsInventoryModKeyFileName, record.TransformsInventoryFormKeyId)
            },
            InventoryArt = CreateNullableFormKey(record.InventoryArtModKeyName, record.InventoryArtModKeyType, record.InventoryArtModKeyFileName, record.InventoryArtFormKeyId),
            PreviewTransform = CreateNullableFormKey(record.PreviewTransformModKeyName, record.PreviewTransformModKeyType, record.PreviewTransformModKeyFileName, record.PreviewTransformFormKeyId),
            FeaturedItemMessage = CreateNullableFormKey(record.FeaturedItemMessageModKeyName, record.FeaturedItemMessageModKeyType, record.FeaturedItemMessageModKeyFileName, record.FeaturedItemMessageFormKeyId),
            XALG = record.XALG,
            Name = FromEnglish(record.Name),
            Text = FromEnglish(record.Text),
            Value = record.Value,
            Weight = record.Weight,
            Flags = record.Flags,
            Teaches = new BookTeachesDTO
            {
                MutagenObjectType = record.TeachesMutagenObjectType,
                Perk = CreateNullableFormKey(record.TeachesPerkModKeyName, record.TeachesPerkModKeyType, record.TeachesPerkModKeyFileName, record.TeachesPerkFormKeyId),
                RawContent = record.TeachesRawContent
            },
            DataSlateType = record.DataSlateType,
            Description = FromEnglish(record.Description),
            DataSlateHeaderLeft = FromEnglish(record.DataSlateHeaderLeft),
            DataSlateHeaderRight = FromEnglish(record.DataSlateHeaderRight)
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private sealed class BookRow : RecordRow
    {
        public int? Version2 { get; set; }

        public int? VersionControl { get; set; }

        public string? ObjectBoundsFirst { get; set; }

        public string? ObjectBoundsSecond { get; set; }

        public string? TransformsInventoryModKeyName { get; set; }

        public int? TransformsInventoryModKeyType { get; set; }

        public string? TransformsInventoryModKeyFileName { get; set; }

        public long? TransformsInventoryFormKeyId { get; set; }

        public string? InventoryArtModKeyName { get; set; }

        public int? InventoryArtModKeyType { get; set; }

        public string? InventoryArtModKeyFileName { get; set; }

        public long? InventoryArtFormKeyId { get; set; }

        public string? PreviewTransformModKeyName { get; set; }

        public int? PreviewTransformModKeyType { get; set; }

        public string? PreviewTransformModKeyFileName { get; set; }

        public long? PreviewTransformFormKeyId { get; set; }

        public string? FeaturedItemMessageModKeyName { get; set; }

        public int? FeaturedItemMessageModKeyType { get; set; }

        public string? FeaturedItemMessageModKeyFileName { get; set; }

        public long? FeaturedItemMessageFormKeyId { get; set; }

        public int? XALG { get; set; }

        public string? Name { get; set; }

        public string? Text { get; set; }

        public int? Value { get; set; }

        public float? Weight { get; set; }

        public string? Flags { get; set; }

        public string? TeachesMutagenObjectType { get; set; }

        public string? TeachesPerkModKeyName { get; set; }

        public int? TeachesPerkModKeyType { get; set; }

        public string? TeachesPerkModKeyFileName { get; set; }

        public long? TeachesPerkFormKeyId { get; set; }

        public string? TeachesRawContent { get; set; }

        public string? DataSlateType { get; set; }

        public string? Description { get; set; }

        public string? DataSlateHeaderLeft { get; set; }

        public string? DataSlateHeaderRight { get; set; }
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return string.Equals(first.Name, second.Name, StringComparison.Ordinal) &&
               first.Type == second.Type &&
               string.Equals(first.FileName, second.FileName, StringComparison.Ordinal);
    }
}
