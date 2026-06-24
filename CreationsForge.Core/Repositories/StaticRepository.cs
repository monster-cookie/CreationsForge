using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class StaticRepository : TypedRecordRepositoryBase, IStaticRepository
{
    private readonly IModelRepository ModelRepository;
    private readonly IKeywordMappingRepository KeywordMappingRepository;
    private readonly IRawRecordPayloadRepository RawRecordPayloadRepository;
    private readonly IRecordLocalizedStringRepository RecordLocalizedStringRepository;

    public StaticRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        IModelRepository modelRepository,
        IKeywordMappingRepository keywordMappingRepository,
        IRawRecordPayloadRepository rawRecordPayloadRepository,
        IRecordLocalizedStringRepository recordLocalizedStringRepository)
        : base(database, recordInstanceRepository)
    {
        ModelRepository = modelRepository;
        KeywordMappingRepository = keywordMappingRepository;
        RawRecordPayloadRepository = rawRecordPayloadRepository;
        RecordLocalizedStringRepository = recordLocalizedStringRepository;
    }

    public override string RecordType => RecordTypeCatalog.Static.RecordID;

    protected override string TableName => RecordTypeCatalog.Static.TableName;

    public IReadOnlyList<StaticDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FetchByFormKey<StaticRow>(
                game,
                formKey,
                [
                    SelectColumn("Name"),
                    SelectColumn("Version2"),
                    SelectColumn("ObjectBounds_First", "ObjectBoundsFirst"),
                    SelectColumn("ObjectBounds_Second", "ObjectBoundsSecond"),
                    SelectColumn("MaxAngle"),
                    SelectColumn("UnknownDNAMFloat"),
                    SelectColumn("LeafAmplitude"),
                    SelectColumn("LeafFrequency"),
                    SelectColumn("Unused"),
                    SelectColumn("DNAMDataTypeState"),
                    SelectColumn("DirtinessScale"),
                    SelectColumn("SnapTemplate_ModKey_Name", "SnapTemplateModKeyName"),
                    SelectColumn("SnapTemplate_ModKey_Type", "SnapTemplateModKeyType"),
                    SelectColumn("SnapTemplate_ModKey_FileName", "SnapTemplateModKeyFileName"),
                    SelectColumn("SnapTemplate_FormKey_ID", "SnapTemplateFormKeyId"),
                    SelectColumn("PreviewTransform_ModKey_Name", "PreviewTransformModKeyName"),
                    SelectColumn("PreviewTransform_ModKey_Type", "PreviewTransformModKeyType"),
                    SelectColumn("PreviewTransform_ModKey_FileName", "PreviewTransformModKeyFileName"),
                    SelectColumn("PreviewTransform_FormKey_ID", "PreviewTransformFormKeyId"),
                    SelectColumn("Material_ModKey_Name", "MaterialModKeyName"),
                    SelectColumn("Material_ModKey_Type", "MaterialModKeyType"),
                    SelectColumn("Material_ModKey_FileName", "MaterialModKeyFileName"),
                    SelectColumn("Material_FormKey_ID", "MaterialFormKeyId"),
                    SelectColumn("Lod_Level0", "LodLevel0"),
                    SelectColumn("Lod_Level1", "LodLevel1"),
                    SelectColumn("Lod_Level2", "LodLevel2"),
                    SelectColumn("Lod_Level3", "LodLevel3"),
                    SelectColumn("NavmeshGeometry")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var models = ModelRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey);
        var keywords = KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey);
        var rawPayloads = RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey);
        var properties = FetchPropertiesByFormKey(game, formKey);
        foreach (var record in records)
        {
            ApplyLocalizedStrings(record, localizedStrings.Where(localizedString => IsSameModKey(localizedString.ModKey, record.ModKey)).ToList());
            record.Models = models.Where(model => IsSameModKey(model.ModKey, record.ModKey)).OrderBy(model => model.ModelSlot).ThenBy(model => model.ModelGender).ToList();
            record.Keywords = keywords.Where(keyword => IsSameModKey(keyword.ModKey, record.ModKey)).OrderBy(keyword => keyword.KeywordIndex).ToList();
            record.Properties = properties.Where(property => IsSameModKey(property.ModKey, record.ModKey)).OrderBy(property => property.PropertyIndex).ToList();
            record.RawPayloads = rawPayloads.Where(payload => IsSameModKey(payload.ModKey, record.ModKey)).OrderBy(payload => payload.PayloadSlot).ThenBy(payload => payload.PayloadIndex).ToList();
        }

        return records;
    }

    public void Save(StaticDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO Statics (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Name, Version2, ObjectBounds_First, ObjectBounds_Second, MaxAngle,
                UnknownDNAMFloat, LeafAmplitude, LeafFrequency, Unused, DNAMDataTypeState, DirtinessScale,
                SnapTemplate_ModKey_Name, SnapTemplate_ModKey_Type, SnapTemplate_ModKey_FileName, SnapTemplate_FormKey_ID,
                PreviewTransform_ModKey_Name, PreviewTransform_ModKey_Type, PreviewTransform_ModKey_FileName, PreviewTransform_FormKey_ID,
                Material_ModKey_Name, Material_ModKey_Type, Material_ModKey_FileName, Material_FormKey_ID,
                Lod_Level0, Lod_Level1, Lod_Level2, Lod_Level3, NavmeshGeometry)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Name, @Version2, @ObjectBoundsFirst, @ObjectBoundsSecond, @MaxAngle,
                @UnknownDNAMFloat, @LeafAmplitude, @LeafFrequency, @Unused, @DNAMDataTypeState, @DirtinessScale,
                @SnapTemplateModKeyName, @SnapTemplateModKeyType, @SnapTemplateModKeyFileName, @SnapTemplateFormKeyId,
                @PreviewTransformModKeyName, @PreviewTransformModKeyType, @PreviewTransformModKeyFileName, @PreviewTransformFormKeyId,
                @MaterialModKeyName, @MaterialModKeyType, @MaterialModKeyFileName, @MaterialFormKeyId,
                @LodLevel0, @LodLevel1, @LodLevel2, @LodLevel3, @NavmeshGeometry);
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
                Name = GetEnglishText(dto.Name),
                dto.Version2,
                dto.ObjectBoundsFirst,
                dto.ObjectBoundsSecond,
                dto.MaxAngle,
                dto.UnknownDNAMFloat,
                dto.LeafAmplitude,
                dto.LeafFrequency,
                dto.Unused,
                dto.DNAMDataTypeState,
                dto.DirtinessScale,
                SnapTemplateModKeyName = dto.SnapTemplate?.ModKey.Name,
                SnapTemplateModKeyType = dto.SnapTemplate?.ModKey.Type,
                SnapTemplateModKeyFileName = dto.SnapTemplate?.ModKey.FileName,
                SnapTemplateFormKeyId = dto.SnapTemplate?.Id,
                PreviewTransformModKeyName = dto.PreviewTransform?.ModKey.Name,
                PreviewTransformModKeyType = dto.PreviewTransform?.ModKey.Type,
                PreviewTransformModKeyFileName = dto.PreviewTransform?.ModKey.FileName,
                PreviewTransformFormKeyId = dto.PreviewTransform?.Id,
                MaterialModKeyName = dto.Material?.ModKey.Name,
                MaterialModKeyType = dto.Material?.ModKey.Type,
                MaterialModKeyFileName = dto.Material?.ModKey.FileName,
                MaterialFormKeyId = dto.Material?.Id,
                dto.LodLevel0,
                dto.LodLevel1,
                dto.LodLevel2,
                dto.LodLevel3,
                dto.NavmeshGeometry
            });
        DeleteProperties(dto);
        SaveProperties(dto);
    }

    private static StaticDTO ToDTO(StaticRow record, SupportedGame game)
    {
        var dto = new StaticDTO
        {
            Game = game,
            ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new FormKeyDTO { ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Name = FromEnglish(record.Name),
            Version2 = record.Version2,
            ObjectBoundsFirst = record.ObjectBoundsFirst,
            ObjectBoundsSecond = record.ObjectBoundsSecond,
            MaxAngle = record.MaxAngle,
            UnknownDNAMFloat = record.UnknownDNAMFloat,
            LeafAmplitude = record.LeafAmplitude,
            LeafFrequency = record.LeafFrequency,
            Unused = record.Unused,
            DNAMDataTypeState = record.DNAMDataTypeState,
            DirtinessScale = record.DirtinessScale,
            SnapTemplate = CreateNullableFormKey(record.SnapTemplateModKeyName, record.SnapTemplateModKeyType, record.SnapTemplateModKeyFileName, record.SnapTemplateFormKeyId),
            PreviewTransform = CreateNullableFormKey(record.PreviewTransformModKeyName, record.PreviewTransformModKeyType, record.PreviewTransformModKeyFileName, record.PreviewTransformFormKeyId),
            Material = CreateNullableFormKey(record.MaterialModKeyName, record.MaterialModKeyType, record.MaterialModKeyFileName, record.MaterialFormKeyId),
            LodLevel0 = record.LodLevel0,
            LodLevel1 = record.LodLevel1,
            LodLevel2 = record.LodLevel2,
            LodLevel3 = record.LodLevel3,
            NavmeshGeometry = record.NavmeshGeometry
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static void ApplyLocalizedStrings(StaticDTO record, IReadOnlyList<LocalizedStringDTO> localizedStrings)
    {
        record.LocalizedStrings = localizedStrings.ToList();
        record.Name = BuildTranslatedString(localizedStrings, nameof(StaticDTO.Name), record.Name);
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return first.Type == second.Type &&
            string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private IReadOnlyList<StaticPropertyDTO> FetchPropertiesByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<StaticPropertyRow>(
            """
            SELECT
                Game,
                ModKey_Name AS ModKeyName,
                ModKey_Type AS ModKeyType,
                ModKey_FileName AS ModKeyFileName,
                FormKey_ModKey_Name AS FormKeyModKeyName,
                FormKey_ModKey_Type AS FormKeyModKeyType,
                FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                FormKey_ID AS FormKeyId,
                Property_Index AS PropertyIndex,
                ActorValue_ModKey_Name AS ActorValueModKeyName,
                ActorValue_ModKey_Type AS ActorValueModKeyType,
                ActorValue_ModKey_FileName AS ActorValueModKeyFileName,
                ActorValue_FormKey_ID AS ActorValueFormKeyId,
                Value,
                ImportedAtUTC
            FROM StaticProperties
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, Property_Index;
            """,
            new
            {
                Game = game.ToString(),
                FormKeyModKeyName = formKey.ModKey.Name,
                FormKeyModKeyType = formKey.ModKey.Type,
                FormKeyModKeyFileName = formKey.ModKey.FileName,
                FormKeyId = formKey.Id
            })
            .Select(ToDTO)
            .ToList();
    }

    private void DeleteProperties(StaticDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM StaticProperties
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
    }

    private void SaveProperties(StaticDTO dto)
    {
        foreach (var property in dto.Properties)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO StaticProperties (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Property_Index, ActorValue_ModKey_Name, ActorValue_ModKey_Type, ActorValue_ModKey_FileName, ActorValue_FormKey_ID, Value, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @PropertyIndex, @ActorValueModKeyName, @ActorValueModKeyType, @ActorValueModKeyFileName, @ActorValueFormKeyId, @Value, @ImportedAtUTC);
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
                    property.PropertyIndex,
                    ActorValueModKeyName = property.ActorValue?.ModKey.Name,
                    ActorValueModKeyType = property.ActorValue?.ModKey.Type,
                    ActorValueModKeyFileName = property.ActorValue?.ModKey.FileName,
                    ActorValueFormKeyId = property.ActorValue?.Id,
                    property.Value,
                    property.ImportedAtUTC
                });
        }
    }

    private static StaticPropertyDTO ToDTO(StaticPropertyRow row)
    {
        return new StaticPropertyDTO
        {
            Game = Enum.Parse<SupportedGame>(row.Game),
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
            PropertyIndex = row.PropertyIndex,
            ActorValue = CreateNullableFormKey(row.ActorValueModKeyName, row.ActorValueModKeyType, row.ActorValueModKeyFileName, row.ActorValueFormKeyId),
            Value = row.Value,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private sealed class StaticRow : RecordRow
    {
        public string? Name { get; set; }

        public int? Version2 { get; set; }

        public string? ObjectBoundsFirst { get; set; }

        public string? ObjectBoundsSecond { get; set; }

        public double? MaxAngle { get; set; }

        public double? UnknownDNAMFloat { get; set; }

        public double? LeafAmplitude { get; set; }

        public double? LeafFrequency { get; set; }

        public string? Unused { get; set; }

        public string? DNAMDataTypeState { get; set; }

        public double? DirtinessScale { get; set; }

        public string? SnapTemplateModKeyName { get; set; }

        public int? SnapTemplateModKeyType { get; set; }

        public string? SnapTemplateModKeyFileName { get; set; }

        public long? SnapTemplateFormKeyId { get; set; }

        public string? PreviewTransformModKeyName { get; set; }

        public int? PreviewTransformModKeyType { get; set; }

        public string? PreviewTransformModKeyFileName { get; set; }

        public long? PreviewTransformFormKeyId { get; set; }

        public string? MaterialModKeyName { get; set; }

        public int? MaterialModKeyType { get; set; }

        public string? MaterialModKeyFileName { get; set; }

        public long? MaterialFormKeyId { get; set; }

        public string? LodLevel0 { get; set; }

        public string? LodLevel1 { get; set; }

        public string? LodLevel2 { get; set; }

        public string? LodLevel3 { get; set; }

        public string? NavmeshGeometry { get; set; }
    }

    private sealed class StaticPropertyRow
    {
        public string Game { get; set; } = string.Empty;

        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public int PropertyIndex { get; set; }

        public string? ActorValueModKeyName { get; set; }

        public int? ActorValueModKeyType { get; set; }

        public string? ActorValueModKeyFileName { get; set; }

        public long? ActorValueFormKeyId { get; set; }

        public double? Value { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }
}
