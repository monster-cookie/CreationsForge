using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class ContainerRepository : TypedRecordRepositoryBase, IContainerRepository
{
    private readonly IModelRepository ModelRepository;
    private readonly IKeywordMappingRepository KeywordMappingRepository;
    private readonly ISoundMappingRepository SoundMappingRepository;
    private readonly IScriptingAdapterRepository ScriptingAdapterRepository;
    private readonly IRecordComponentRepository RecordComponentRepository;
    private readonly IReflectionRepository ReflectionRepository;
    private readonly IRecordLocalizedStringRepository RecordLocalizedStringRepository;

    public ContainerRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        IModelRepository modelRepository,
        IKeywordMappingRepository keywordMappingRepository,
        ISoundMappingRepository soundMappingRepository,
        IScriptingAdapterRepository scriptingAdapterRepository,
        IRecordComponentRepository recordComponentRepository,
        IReflectionRepository reflectionRepository,
        IRecordLocalizedStringRepository recordLocalizedStringRepository)
        : base(database, recordInstanceRepository)
    {
        ModelRepository = modelRepository;
        KeywordMappingRepository = keywordMappingRepository;
        SoundMappingRepository = soundMappingRepository;
        ScriptingAdapterRepository = scriptingAdapterRepository;
        RecordComponentRepository = recordComponentRepository;
        ReflectionRepository = reflectionRepository;
        RecordLocalizedStringRepository = recordLocalizedStringRepository;
    }

    public override string RecordType => RecordTypeCatalog.Container.RecordID;

    protected override string TableName => RecordTypeCatalog.Container.TableName;

    public IReadOnlyList<ContainerDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FetchByFormKey<ContainerRow>(
                game,
                formKey,
                [
                    SelectColumn("Version2"),
                    SelectColumn("VersionControl"),
                    SelectColumn("ObjectBounds_First", "ObjectBoundsFirst"),
                    SelectColumn("ObjectBounds_Second", "ObjectBoundsSecond"),
                    SelectColumn("Name"),
                    SelectColumn("Flags"),
                    SelectColumn("MajorFlags"),
                    SelectColumn("NativeTerminal_ModKey_Name", "NativeTerminalModKeyName"),
                    SelectColumn("NativeTerminal_ModKey_Type", "NativeTerminalModKeyType"),
                    SelectColumn("NativeTerminal_ModKey_FileName", "NativeTerminalModKeyFileName"),
                    SelectColumn("NativeTerminal_FormKey_ID", "NativeTerminalFormKeyId"),
                    SelectColumn("SnapTemplate_ModKey_Name", "SnapTemplateModKeyName"),
                    SelectColumn("SnapTemplate_ModKey_Type", "SnapTemplateModKeyType"),
                    SelectColumn("SnapTemplate_ModKey_FileName", "SnapTemplateModKeyFileName"),
                    SelectColumn("SnapTemplate_FormKey_ID", "SnapTemplateFormKeyId"),
                    SelectColumn("ContainsOnlyFilter_ModKey_Name", "ContainsOnlyFilterModKeyName"),
                    SelectColumn("ContainsOnlyFilter_ModKey_Type", "ContainsOnlyFilterModKeyType"),
                    SelectColumn("ContainsOnlyFilter_ModKey_FileName", "ContainsOnlyFilterModKeyFileName"),
                    SelectColumn("ContainsOnlyFilter_FormKey_ID", "ContainsOnlyFilterFormKeyId"),
                    SelectColumn("TransformOutpost_ModKey_Name", "TransformOutpostModKeyName"),
                    SelectColumn("TransformOutpost_ModKey_Type", "TransformOutpostModKeyType"),
                    SelectColumn("TransformOutpost_ModKey_FileName", "TransformOutpostModKeyFileName"),
                    SelectColumn("TransformOutpost_FormKey_ID", "TransformOutpostFormKeyId"),
                    SelectColumn("TransformPreview_ModKey_Name", "TransformPreviewModKeyName"),
                    SelectColumn("TransformPreview_ModKey_Type", "TransformPreviewModKeyType"),
                    SelectColumn("TransformPreview_ModKey_FileName", "TransformPreviewModKeyFileName"),
                    SelectColumn("TransformPreview_FormKey_ID", "TransformPreviewFormKeyId"),
                    SelectColumn("AnimationGraph"),
                    SelectColumn("AnimationSkeleton"),
                    SelectColumn("AnimationDirectory"),
                    SelectColumn("AnimationFile")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var items = FetchItemsByFormKey(game, formKey);
        var properties = FetchPropertiesByFormKey(game, formKey);
        var forcedLocations = FetchForcedLocationsByFormKey(game, formKey);
        var models = ModelRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey);
        var keywords = KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey);
        var sounds = SoundMappingRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey);
        var scriptingAdapters = ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey);
        var components = RecordComponentRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey);
        var reflections = ReflectionRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey);
        foreach (var record in records)
        {
            record.Items = items
                .Where(item => IsSameModKey(item.ModKey, record.ModKey))
                .OrderBy(item => item.ItemIndex)
                .ToList();
            record.Properties = properties.Where(property => IsSameModKey(property.ModKey, record.ModKey)).OrderBy(property => property.PropertyIndex).ToList();
            record.ForcedLocations = forcedLocations.Where(forcedLocation => IsSameModKey(forcedLocation.ModKey, record.ModKey)).OrderBy(forcedLocation => forcedLocation.ForcedLocationIndex).Select(forcedLocation => forcedLocation.ForcedLocation).ToList();
            ApplyLocalizedStrings(record, localizedStrings.Where(localizedString => IsSameModKey(localizedString.ModKey, record.ModKey)).ToList());
            record.Models = models.Where(model => IsSameModKey(model.ModKey, record.ModKey)).OrderBy(model => model.ModelSlot).ThenBy(model => model.ModelGender).ToList();
            record.Keywords = keywords.Where(keyword => IsSameModKey(keyword.ModKey, record.ModKey)).OrderBy(keyword => keyword.KeywordIndex).ToList();
            record.Sounds = sounds.Where(sound => IsSameModKey(sound.ModKey, record.ModKey)).OrderBy(sound => sound.SoundSlot).ThenBy(sound => sound.SoundIndex).ToList();
            record.ScriptingAdapters = scriptingAdapters.Where(adapter => IsSameModKey(adapter.ModKey, record.ModKey)).OrderBy(adapter => adapter.ScriptIndex).ToList();
            record.Components = components.Where(component => IsSameModKey(component.ModKey, record.ModKey)).OrderBy(component => component.ComponentIndex).ToList();
            record.Reflections = reflections.Where(reflection => IsSameModKey(reflection.ModKey, record.ModKey)).OrderBy(reflection => reflection.ComponentIndex).ToList();
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
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, VersionControl, ObjectBounds_First, ObjectBounds_Second, Name, Flags,
                MajorFlags, NativeTerminal_ModKey_Name, NativeTerminal_ModKey_Type, NativeTerminal_ModKey_FileName, NativeTerminal_FormKey_ID,
                SnapTemplate_ModKey_Name, SnapTemplate_ModKey_Type, SnapTemplate_ModKey_FileName, SnapTemplate_FormKey_ID,
                ContainsOnlyFilter_ModKey_Name, ContainsOnlyFilter_ModKey_Type, ContainsOnlyFilter_ModKey_FileName, ContainsOnlyFilter_FormKey_ID,
                TransformOutpost_ModKey_Name, TransformOutpost_ModKey_Type, TransformOutpost_ModKey_FileName, TransformOutpost_FormKey_ID,
                TransformPreview_ModKey_Name, TransformPreview_ModKey_Type, TransformPreview_ModKey_FileName, TransformPreview_FormKey_ID,
                AnimationGraph, AnimationSkeleton, AnimationDirectory, AnimationFile)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @VersionControl, @ObjectBoundsFirst, @ObjectBoundsSecond, @Name, @Flags,
                @MajorFlags, @NativeTerminalModKeyName, @NativeTerminalModKeyType, @NativeTerminalModKeyFileName, @NativeTerminalFormKeyId,
                @SnapTemplateModKeyName, @SnapTemplateModKeyType, @SnapTemplateModKeyFileName, @SnapTemplateFormKeyId,
                @ContainsOnlyFilterModKeyName, @ContainsOnlyFilterModKeyType, @ContainsOnlyFilterModKeyFileName, @ContainsOnlyFilterFormKeyId,
                @TransformOutpostModKeyName, @TransformOutpostModKeyType, @TransformOutpostModKeyFileName, @TransformOutpostFormKeyId,
                @TransformPreviewModKeyName, @TransformPreviewModKeyType, @TransformPreviewModKeyFileName, @TransformPreviewFormKeyId,
                @AnimationGraph, @AnimationSkeleton, @AnimationDirectory, @AnimationFile);
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
                dto.ObjectBoundsFirst,
                dto.ObjectBoundsSecond,
                Name = GetEnglishText(dto.Name),
                dto.Flags,
                dto.MajorFlags,
                NativeTerminalModKeyName = dto.NativeTerminalFormKey?.ModKey.Name,
                NativeTerminalModKeyType = dto.NativeTerminalFormKey?.ModKey.Type,
                NativeTerminalModKeyFileName = dto.NativeTerminalFormKey?.ModKey.FileName,
                NativeTerminalFormKeyId = dto.NativeTerminalFormKey?.Id,
                SnapTemplateModKeyName = dto.SnapTemplate?.ModKey.Name,
                SnapTemplateModKeyType = dto.SnapTemplate?.ModKey.Type,
                SnapTemplateModKeyFileName = dto.SnapTemplate?.ModKey.FileName,
                SnapTemplateFormKeyId = dto.SnapTemplate?.Id,
                ContainsOnlyFilterModKeyName = dto.ContainsOnlyFilter?.ModKey.Name,
                ContainsOnlyFilterModKeyType = dto.ContainsOnlyFilter?.ModKey.Type,
                ContainsOnlyFilterModKeyFileName = dto.ContainsOnlyFilter?.ModKey.FileName,
                ContainsOnlyFilterFormKeyId = dto.ContainsOnlyFilter?.Id,
                TransformOutpostModKeyName = dto.Transforms?.Outpost?.ModKey.Name,
                TransformOutpostModKeyType = dto.Transforms?.Outpost?.ModKey.Type,
                TransformOutpostModKeyFileName = dto.Transforms?.Outpost?.ModKey.FileName,
                TransformOutpostFormKeyId = dto.Transforms?.Outpost?.Id,
                TransformPreviewModKeyName = dto.Transforms?.Preview?.ModKey.Name,
                TransformPreviewModKeyType = dto.Transforms?.Preview?.ModKey.Type,
                TransformPreviewModKeyFileName = dto.Transforms?.Preview?.ModKey.FileName,
                TransformPreviewFormKeyId = dto.Transforms?.Preview?.Id,
                dto.AnimationGraph,
                dto.AnimationSkeleton,
                dto.AnimationDirectory,
                dto.AnimationFile
            });
        ReplaceItems(dto);
        ReplaceProperties(dto);
        ReplaceForcedLocations(dto);
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

    /// <summary>
    /// Reads actor-value property rows for a container form key.
    /// </summary>
    private IReadOnlyList<ContainerPropertyDTO> FetchPropertiesByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<ContainerPropertyRow>(
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
                FROM ContainerProperties
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
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
            .Select(row => ToDTO(row, game))
            .ToList();
    }

    /// <summary>
    /// Replaces all actor-value property rows for a container.
    /// </summary>
    private void ReplaceProperties(ContainerDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM ContainerProperties
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

        foreach (var property in dto.Properties)
        {
            property.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO ContainerProperties (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Property_Index, ActorValue_ModKey_Name, ActorValue_ModKey_Type, ActorValue_ModKey_FileName, ActorValue_FormKey_ID, Value, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @PropertyIndex, @ActorValueModKeyName, @ActorValueModKeyType, @ActorValueModKeyFileName, @ActorValueFormKeyId, @Value, @ImportedAtUTC);
                """,
                new
                {
                    Game = property.Game.ToString(),
                    ModKeyName = property.ModKey.Name,
                    ModKeyType = property.ModKey.Type,
                    ModKeyFileName = property.ModKey.FileName,
                    FormKeyModKeyName = property.FormKey.ModKey.Name,
                    FormKeyModKeyType = property.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = property.FormKey.ModKey.FileName,
                    FormKeyId = property.FormKey.Id,
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

    /// <summary>
    /// Reads forced location rows for a container form key.
    /// </summary>
    private IReadOnlyList<ContainerForcedLocationRow> FetchForcedLocationsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<ContainerForcedLocationRow>(
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
                ForcedLocation_Index AS ForcedLocationIndex,
                ForcedLocation_ModKey_Name AS ForcedLocationModKeyName,
                ForcedLocation_ModKey_Type AS ForcedLocationModKeyType,
                ForcedLocation_ModKey_FileName AS ForcedLocationModKeyFileName,
                ForcedLocation_FormKey_ID AS ForcedLocationFormKeyId,
                ImportedAtUTC
            FROM ContainerForcedLocations
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, ForcedLocation_Index;
            """,
            new
            {
                Game = game.ToString(),
                FormKeyModKeyName = formKey.ModKey.Name,
                FormKeyModKeyType = formKey.ModKey.Type,
                FormKeyModKeyFileName = formKey.ModKey.FileName,
                FormKeyId = formKey.Id
            });
    }

    /// <summary>
    /// Replaces all forced location rows for a container.
    /// </summary>
    private void ReplaceForcedLocations(ContainerDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM ContainerForcedLocations
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

        for (var forcedLocationIndex = 0; forcedLocationIndex < dto.ForcedLocations.Count; forcedLocationIndex++)
        {
            var forcedLocation = dto.ForcedLocations[forcedLocationIndex];
            Database.Execute(
                """
                INSERT OR REPLACE INTO ContainerForcedLocations (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    ForcedLocation_Index, ForcedLocation_ModKey_Name, ForcedLocation_ModKey_Type, ForcedLocation_ModKey_FileName, ForcedLocation_FormKey_ID, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @ForcedLocationIndex, @ForcedLocationModKeyName, @ForcedLocationModKeyType, @ForcedLocationModKeyFileName, @ForcedLocationFormKeyId, @ImportedAtUTC);
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
                    ForcedLocationIndex = forcedLocationIndex,
                    ForcedLocationModKeyName = forcedLocation.ModKey.Name,
                    ForcedLocationModKeyType = forcedLocation.ModKey.Type,
                    ForcedLocationModKeyFileName = forcedLocation.ModKey.FileName,
                    ForcedLocationFormKeyId = forcedLocation.Id,
                    dto.ImportedAtUTC
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
            VersionControl = record.VersionControl,
            ObjectBoundsFirst = record.ObjectBoundsFirst,
            ObjectBoundsSecond = record.ObjectBoundsSecond,
            Name = FromEnglish(record.Name),
            Flags = record.Flags,
            MajorFlags = record.MajorFlags,
            NativeTerminalFormKey = CreateNullableFormKey(record.NativeTerminalModKeyName, record.NativeTerminalModKeyType, record.NativeTerminalModKeyFileName, record.NativeTerminalFormKeyId),
            SnapTemplate = CreateNullableFormKey(record.SnapTemplateModKeyName, record.SnapTemplateModKeyType, record.SnapTemplateModKeyFileName, record.SnapTemplateFormKeyId),
            ContainsOnlyFilter = CreateNullableFormKey(record.ContainsOnlyFilterModKeyName, record.ContainsOnlyFilterModKeyType, record.ContainsOnlyFilterModKeyFileName, record.ContainsOnlyFilterFormKeyId),
            Transforms = new ContainerTransformsDTO
            {
                Outpost = CreateNullableFormKey(record.TransformOutpostModKeyName, record.TransformOutpostModKeyType, record.TransformOutpostModKeyFileName, record.TransformOutpostFormKeyId),
                Preview = CreateNullableFormKey(record.TransformPreviewModKeyName, record.TransformPreviewModKeyType, record.TransformPreviewModKeyFileName, record.TransformPreviewFormKeyId)
            },
            AnimationGraph = record.AnimationGraph,
            AnimationSkeleton = record.AnimationSkeleton,
            AnimationDirectory = record.AnimationDirectory,
            AnimationFile = record.AnimationFile
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static void ApplyLocalizedStrings(ContainerDTO record, IReadOnlyList<LocalizedStringDTO> localizedStrings)
    {
        record.LocalizedStrings = localizedStrings.ToList();
        record.Name = BuildTranslatedString(localizedStrings, nameof(ContainerDTO.Name), record.Name);
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

    /// <summary>
    /// Converts a database row into a container property DTO.
    /// </summary>
    private static ContainerPropertyDTO ToDTO(ContainerPropertyRow row, SupportedGame game)
    {
        return new ContainerPropertyDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            PropertyIndex = row.PropertyIndex,
            ActorValue = CreateNullableFormKey(row.ActorValueModKeyName, row.ActorValueModKeyType, row.ActorValueModKeyFileName, row.ActorValueFormKeyId),
            Value = row.Value,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    /// <summary>
    /// Creates a mod key DTO from database row parts.
    /// </summary>
    private static ModKeyDTO CreateModKey(string name, int type, string fileName)
    {
        return new ModKeyDTO { Name = name, Type = type, FileName = fileName };
    }

    /// <summary>
    /// Creates a form key DTO from database row parts.
    /// </summary>
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

    private sealed class ContainerRow : RecordRow
    {
        public int? Version2 { get; set; }

        public int? VersionControl { get; set; }

        public string? ObjectBoundsFirst { get; set; }

        public string? ObjectBoundsSecond { get; set; }

        public string? Name { get; set; }

        public string? Flags { get; set; }

        public string? MajorFlags { get; set; }

        public string? NativeTerminalModKeyName { get; set; }

        public int? NativeTerminalModKeyType { get; set; }

        public string? NativeTerminalModKeyFileName { get; set; }

        public long? NativeTerminalFormKeyId { get; set; }

        public string? SnapTemplateModKeyName { get; set; }

        public int? SnapTemplateModKeyType { get; set; }

        public string? SnapTemplateModKeyFileName { get; set; }

        public long? SnapTemplateFormKeyId { get; set; }

        public string? ContainsOnlyFilterModKeyName { get; set; }

        public int? ContainsOnlyFilterModKeyType { get; set; }

        public string? ContainsOnlyFilterModKeyFileName { get; set; }

        public long? ContainsOnlyFilterFormKeyId { get; set; }

        public string? TransformOutpostModKeyName { get; set; }

        public int? TransformOutpostModKeyType { get; set; }

        public string? TransformOutpostModKeyFileName { get; set; }

        public long? TransformOutpostFormKeyId { get; set; }

        public string? TransformPreviewModKeyName { get; set; }

        public int? TransformPreviewModKeyType { get; set; }

        public string? TransformPreviewModKeyFileName { get; set; }

        public long? TransformPreviewFormKeyId { get; set; }

        public string? AnimationGraph { get; set; }

        public string? AnimationSkeleton { get; set; }

        public string? AnimationDirectory { get; set; }

        public string? AnimationFile { get; set; }
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

    private sealed class ContainerPropertyRow
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

    private sealed class ContainerForcedLocationRow
    {
        public string Game { get; set; } = string.Empty;

        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public int ForcedLocationIndex { get; set; }

        public string ForcedLocationModKeyName { get; set; } = string.Empty;

        public int ForcedLocationModKeyType { get; set; }

        public string ForcedLocationModKeyFileName { get; set; } = string.Empty;

        public long ForcedLocationFormKeyId { get; set; }

        public DateTime ImportedAtUTC { get; set; }

        public ModKeyDTO ModKey => CreateModKey(ModKeyName, ModKeyType, ModKeyFileName);

        public FormKeyDTO ForcedLocation => CreateFormKey(ForcedLocationModKeyName, ForcedLocationModKeyType, ForcedLocationModKeyFileName, ForcedLocationFormKeyId);
    }
}
