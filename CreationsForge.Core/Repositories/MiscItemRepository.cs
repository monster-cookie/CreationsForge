using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class MiscItemRepository : TypedRecordRepositoryBase, IMiscItemRepository
{
    private readonly IRecordLocalizedStringRepository RecordLocalizedStringRepository;
    private readonly IModelRepository ModelRepository;
    private readonly IKeywordMappingRepository KeywordMappingRepository;
    private readonly ISoundMappingRepository SoundMappingRepository;
    private readonly IScriptingAdapterRepository ScriptingAdapterRepository;
    private readonly IRawRecordPayloadRepository RawRecordPayloadRepository;

    public MiscItemRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        IRecordLocalizedStringRepository recordLocalizedStringRepository,
        IModelRepository modelRepository,
        IKeywordMappingRepository keywordMappingRepository,
        ISoundMappingRepository soundMappingRepository,
        IScriptingAdapterRepository scriptingAdapterRepository,
        IRawRecordPayloadRepository rawRecordPayloadRepository)
        : base(database, recordInstanceRepository)
    {
        RecordLocalizedStringRepository = recordLocalizedStringRepository;
        ModelRepository = modelRepository;
        KeywordMappingRepository = keywordMappingRepository;
        SoundMappingRepository = soundMappingRepository;
        ScriptingAdapterRepository = scriptingAdapterRepository;
        RawRecordPayloadRepository = rawRecordPayloadRepository;
    }

    public override string RecordType => RecordTypeCatalog.MiscItem.RecordID;

    protected override string TableName => RecordTypeCatalog.MiscItem.TableName;

    public IReadOnlyList<MiscItemDTO> GetByFormKey(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.FormKeyDTO formKey)
    {
        var records = FetchByFormKey<MiscItemRow>(
                game,
                formKey,
                [
                    SelectColumn("Name"),
                    SelectColumn("ShortName"),
                    SelectColumn("Version2"),
                    SelectColumn("VersionControl"),
                    SelectColumn("ObjectBounds_First", "ObjectBoundsFirst"),
                    SelectColumn("ObjectBounds_Second", "ObjectBoundsSecond"),
                    SelectColumn("Transforms_Inventory_ModKey_Name", "TransformsInventoryModKeyName"),
                    SelectColumn("Transforms_Inventory_ModKey_Type", "TransformsInventoryModKeyType"),
                    SelectColumn("Transforms_Inventory_ModKey_FileName", "TransformsInventoryModKeyFileName"),
                    SelectColumn("Transforms_Inventory_FormKey_ID", "TransformsInventoryFormKeyId"),
                    SelectColumn("PreviewTransform_ModKey_Name", "PreviewTransformModKeyName"),
                    SelectColumn("PreviewTransform_ModKey_Type", "PreviewTransformModKeyType"),
                    SelectColumn("PreviewTransform_ModKey_FileName", "PreviewTransformModKeyFileName"),
                    SelectColumn("PreviewTransform_FormKey_ID", "PreviewTransformFormKeyId"),
                    SelectColumn("Value"),
                    SelectColumn("Weight"),
                    SelectColumn("DirtinessScale"),
                    SelectColumn("FeaturedItemMessage_ModKey_Name", "FeaturedItemMessageModKeyName"),
                    SelectColumn("FeaturedItemMessage_ModKey_Type", "FeaturedItemMessageModKeyType"),
                    SelectColumn("FeaturedItemMessage_ModKey_FileName", "FeaturedItemMessageModKeyFileName"),
                    SelectColumn("FeaturedItemMessage_FormKey_ID", "FeaturedItemMessageFormKeyId"),
                    SelectColumn("FLAG", "Flag")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.MiscItem.RecordID, formKey);
        var models = ModelRepository.GetByFormKey(game, RecordTypeCatalog.MiscItem.RecordID, formKey);
        var keywords = KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.MiscItem.RecordID, formKey);
        var sounds = SoundMappingRepository.GetByFormKey(game, RecordTypeCatalog.MiscItem.RecordID, formKey);
        var scriptingAdapters = ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.MiscItem.RecordID, formKey);
        var rawPayloads = RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.MiscItem.RecordID, formKey);
        var components = GetComponentsByFormKey(game, formKey);
        var resources = GetResourcesByFormKey(game, formKey);
        var destructibles = GetDestructiblesByFormKey(game, formKey);
        foreach (var record in records)
        {
            ApplyLocalizedStrings(record, localizedStrings.Where(localizedString => RecordModKeysMatch(localizedString.ModKey, record.ModKey)).ToList());
            record.Models = models.Where(model => RecordModKeysMatch(model.ModKey, record.ModKey)).OrderBy(model => model.ModelSlot).ThenBy(model => model.ModelGender).ToList();
            record.Keywords = keywords.Where(keyword => RecordModKeysMatch(keyword.ModKey, record.ModKey)).OrderBy(keyword => keyword.KeywordIndex).ToList();
            record.Sounds = sounds.Where(sound => RecordModKeysMatch(sound.ModKey, record.ModKey)).OrderBy(sound => sound.SoundIndex).ToList();
            record.ScriptingAdapters = scriptingAdapters.Where(adapter => RecordModKeysMatch(adapter.ModKey, record.ModKey)).OrderBy(adapter => adapter.ScriptIndex).ToList();
            record.Components = components.Where(component => RecordModKeysMatch(component.ModKey, record.ModKey)).OrderBy(component => component.ComponentIndex).ToList();
            record.Resources = resources.Where(resource => RecordModKeysMatch(resource.ModKey, record.ModKey)).OrderBy(resource => resource.ResourceIndex).ToList();
            record.RawPayloads = rawPayloads.Where(payload => RecordModKeysMatch(payload.ModKey, record.ModKey)).OrderBy(payload => payload.PayloadSlot).ThenBy(payload => payload.PayloadIndex).ToList();
            record.Destructible = destructibles.FirstOrDefault(destructible => RecordModKeysMatch(destructible.ModKey, record.ModKey))?.Destructible;
        }

        return records;
    }

    public void Save(MiscItemDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO MiscItems (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, VersionControl, ObjectBounds_First, ObjectBounds_Second,
                Transforms_Inventory_ModKey_Name, Transforms_Inventory_ModKey_Type, Transforms_Inventory_ModKey_FileName, Transforms_Inventory_FormKey_ID,
                PreviewTransform_ModKey_Name, PreviewTransform_ModKey_Type, PreviewTransform_ModKey_FileName, PreviewTransform_FormKey_ID,
                Name, ShortName, Value, Weight, DirtinessScale,
                FeaturedItemMessage_ModKey_Name, FeaturedItemMessage_ModKey_Type, FeaturedItemMessage_ModKey_FileName, FeaturedItemMessage_FormKey_ID, FLAG)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @VersionControl, @ObjectBoundsFirst, @ObjectBoundsSecond,
                @TransformsInventoryModKeyName, @TransformsInventoryModKeyType, @TransformsInventoryModKeyFileName, @TransformsInventoryFormKeyId,
                @PreviewTransformModKeyName, @PreviewTransformModKeyType, @PreviewTransformModKeyFileName, @PreviewTransformFormKeyId,
                @Name, @ShortName, @Value, @Weight, @DirtinessScale,
                @FeaturedItemMessageModKeyName, @FeaturedItemMessageModKeyType, @FeaturedItemMessageModKeyFileName, @FeaturedItemMessageFormKeyId, @Flag);
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
                PreviewTransformModKeyName = dto.PreviewTransform?.ModKey.Name,
                PreviewTransformModKeyType = dto.PreviewTransform?.ModKey.Type,
                PreviewTransformModKeyFileName = dto.PreviewTransform?.ModKey.FileName,
                PreviewTransformFormKeyId = dto.PreviewTransform?.Id,
                Name = GetEnglishText(dto.Name),
                ShortName = GetEnglishText(dto.ShortName),
                dto.Value,
                dto.Weight,
                dto.DirtinessScale,
                FeaturedItemMessageModKeyName = dto.FeaturedItemMessage?.ModKey.Name,
                FeaturedItemMessageModKeyType = dto.FeaturedItemMessage?.ModKey.Type,
                FeaturedItemMessageModKeyFileName = dto.FeaturedItemMessage?.ModKey.FileName,
                FeaturedItemMessageFormKeyId = dto.FeaturedItemMessage?.Id,
                dto.Flag
            });
        ReplaceMiscItemComponents(dto);
        ReplaceMiscItemDestructible(dto);
        ReplaceMiscItemResources(dto);
    }

    public new void DeleteStaleByPlugin(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.ModKeyDTO modKey, DateTime importedAtUTC)
    {
        DeleteStaleComponentsByPlugin(game, modKey, importedAtUTC);
        DeleteStaleDestructiblesByPlugin(game, modKey, importedAtUTC);
        DeleteStaleResourcesByPlugin(game, modKey, importedAtUTC);
        base.DeleteStaleByPlugin(game, modKey, importedAtUTC);
    }

    private IReadOnlyList<MiscItemComponentDTO> GetComponentsByFormKey(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.FormKeyDTO formKey)
    {
        return Database.Fetch<MiscItemComponentRow>(
                """
                SELECT
                    ModKey_Name AS ModKeyName,
                    ModKey_Type AS ModKeyType,
                    ModKey_FileName AS ModKeyFileName,
                    FormKey_ModKey_Name AS FormKeyModKeyName,
                    FormKey_ModKey_Type AS FormKeyModKeyType,
                    FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                    FormKey_ID AS FormKeyId,
                    Component_ModKey_Name AS ComponentModKeyName,
                    Component_ModKey_Type AS ComponentModKeyType,
                    Component_ModKey_FileName AS ComponentModKeyFileName,
                    Component_FormKey_ID AS ComponentFormKeyId,
                    Component_Index AS ComponentIndex,
                    DisplayIndex,
                    Count,
                    ImportedAtUTC
                FROM MiscItemComponents
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, Component_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row => new MiscItemComponentDTO
            {
                Game = game,
                ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = row.ModKeyName, Type = row.ModKeyType, FileName = row.ModKeyFileName },
                FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
                Component = CreateFormKey(row.ComponentModKeyName, row.ComponentModKeyType, row.ComponentModKeyFileName, row.ComponentFormKeyId),
                ComponentIndex = row.ComponentIndex,
                DisplayIndex = row.DisplayIndex,
                Count = row.Count,
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private IReadOnlyList<MiscItemDestructibleRowAggregate> GetDestructiblesByFormKey(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.FormKeyDTO formKey)
    {
        var rows = Database.Fetch<MiscItemDestructibleRow>(
            """
            SELECT
                ModKey_Name AS ModKeyName,
                ModKey_Type AS ModKeyType,
                ModKey_FileName AS ModKeyFileName,
                FormKey_ModKey_Name AS FormKeyModKeyName,
                FormKey_ModKey_Type AS FormKeyModKeyType,
                FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                FormKey_ID AS FormKeyId,
                Health,
                DESTCount,
                ImportedAtUTC
            FROM MiscItemDestructibles
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE;
            """,
            new
            {
                Game = game.ToString(),
                FormKeyModKeyName = formKey.ModKey.Name,
                FormKeyModKeyType = formKey.ModKey.Type,
                FormKeyModKeyFileName = formKey.ModKey.FileName,
                FormKeyId = formKey.Id
            });
        var stages = GetDestructibleStagesByFormKey(game, formKey);

        return rows
            .Select(row => new MiscItemDestructibleRowAggregate
            {
                ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = row.ModKeyName, Type = row.ModKeyType, FileName = row.ModKeyFileName },
                Destructible = new MiscItemDestructibleDTO
                {
                    Data = new MiscItemDestructibleDataDTO
                    {
                        Health = row.Health,
                        DESTCount = row.DESTCount
                    },
                    Stages = stages
                        .Where(stage => string.Equals(stage.ModKey.FileName, row.ModKeyFileName, StringComparison.OrdinalIgnoreCase) &&
                                        string.Equals(stage.ModKey.Name, row.ModKeyName, StringComparison.OrdinalIgnoreCase) &&
                                        stage.ModKey.Type == row.ModKeyType)
                        .OrderBy(stage => stage.Stage.StageIndex)
                        .Select(stage => stage.Stage)
                        .ToList()
                }
            })
            .ToList();
    }

    private IReadOnlyList<MiscItemDestructibleStageRowAggregate> GetDestructibleStagesByFormKey(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.FormKeyDTO formKey)
    {
        return Database.Fetch<MiscItemDestructibleStageRow>(
                """
                SELECT
                    ModKey_Name AS ModKeyName,
                    ModKey_Type AS ModKeyType,
                    ModKey_FileName AS ModKeyFileName,
                    FormKey_ModKey_Name AS FormKeyModKeyName,
                    FormKey_ModKey_Type AS FormKeyModKeyType,
                    FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                    FormKey_ID AS FormKeyId,
                    Stage_Index AS StageIndex,
                    StageRecordIndex,
                    HealthPercent,
                    ModelDamageStage,
                    Flags,
                    SelfDamagePerSecond,
                    Explosion_ModKey_Name AS ExplosionModKeyName,
                    Explosion_ModKey_Type AS ExplosionModKeyType,
                    Explosion_ModKey_FileName AS ExplosionModKeyFileName,
                    Explosion_FormKey_ID AS ExplosionFormKeyId,
                    Model_File AS ModelFile,
                    Model_Data AS ModelData,
                    ImportedAtUTC
                FROM MiscItemDestructibleStages
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, Stage_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row => new MiscItemDestructibleStageRowAggregate
            {
                ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = row.ModKeyName, Type = row.ModKeyType, FileName = row.ModKeyFileName },
                Stage = new MiscItemDestructibleStageDTO
                {
                    StageIndex = row.StageIndex,
                    Index = row.StageRecordIndex,
                    HealthPercent = row.HealthPercent,
                    ModelDamageStage = row.ModelDamageStage,
                    Flags = row.Flags,
                    SelfDamagePerSecond = row.SelfDamagePerSecond,
                    Explosion = CreateNullableFormKey(row.ExplosionModKeyName, row.ExplosionModKeyType, row.ExplosionModKeyFileName, row.ExplosionFormKeyId),
                    Model = row.ModelFile == null && row.ModelData == null
                        ? null
                        : new MiscItemDestructibleStageModelDTO
                        {
                            File = row.ModelFile,
                            Data = row.ModelData
                        }
                }
            })
            .ToList();
    }

    private IReadOnlyList<MiscItemResourceDTO> GetResourcesByFormKey(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.FormKeyDTO formKey)
    {
        return Database.Fetch<MiscItemResourceRow>(
                """
                SELECT
                    ModKey_Name AS ModKeyName,
                    ModKey_Type AS ModKeyType,
                    ModKey_FileName AS ModKeyFileName,
                    FormKey_ModKey_Name AS FormKeyModKeyName,
                    FormKey_ModKey_Type AS FormKeyModKeyType,
                    FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                    FormKey_ID AS FormKeyId,
                    Resource_ModKey_Name AS ResourceModKeyName,
                    Resource_ModKey_Type AS ResourceModKeyType,
                    Resource_ModKey_FileName AS ResourceModKeyFileName,
                    Resource_FormKey_ID AS ResourceFormKeyId,
                    Resource_Index AS ResourceIndex,
                    Count,
                    ImportedAtUTC
                FROM MiscItemResources
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, Resource_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row => new MiscItemResourceDTO
            {
                Game = game,
                ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = row.ModKeyName, Type = row.ModKeyType, FileName = row.ModKeyFileName },
                FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
                Resource = CreateFormKey(row.ResourceModKeyName, row.ResourceModKeyType, row.ResourceModKeyFileName, row.ResourceFormKeyId),
                ResourceIndex = row.ResourceIndex,
                Count = row.Count,
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private void ReplaceMiscItemComponents(MiscItemDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM MiscItemComponents
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

        foreach (var component in dto.Components)
        {
            component.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO MiscItemComponents (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Component_ModKey_Name, Component_ModKey_Type, Component_ModKey_FileName, Component_FormKey_ID, Component_Index, DisplayIndex, Count, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @ComponentModKeyName, @ComponentModKeyType, @ComponentModKeyFileName, @ComponentFormKeyId, @ComponentIndex, @DisplayIndex, @Count, @ImportedAtUTC);
                """,
                new
                {
                    Game = component.Game.ToString(),
                    ModKeyName = component.ModKey.Name,
                    ModKeyType = component.ModKey.Type,
                    ModKeyFileName = component.ModKey.FileName,
                    FormKeyModKeyName = component.FormKey.ModKey.Name,
                    FormKeyModKeyType = component.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = component.FormKey.ModKey.FileName,
                    FormKeyId = component.FormKey.Id,
                    ComponentModKeyName = component.Component.ModKey.Name,
                    ComponentModKeyType = component.Component.ModKey.Type,
                    ComponentModKeyFileName = component.Component.ModKey.FileName,
                    ComponentFormKeyId = component.Component.Id,
                    component.ComponentIndex,
                    component.DisplayIndex,
                    component.Count,
                    component.ImportedAtUTC
                });
        }
    }

    private void ReplaceMiscItemDestructible(MiscItemDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM MiscItemDestructibleStages
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

        Database.Execute(
            """
            DELETE FROM MiscItemDestructibles
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

        if (dto.Destructible == null)
        {
            return;
        }

        Database.Execute(
            """
            INSERT OR REPLACE INTO MiscItemDestructibles (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                Health, DESTCount, ImportedAtUTC)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @Health, @DESTCount, @ImportedAtUTC);
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
                Health = dto.Destructible.Data?.Health,
                DESTCount = dto.Destructible.Data?.DESTCount,
                dto.ImportedAtUTC
            });

        foreach (var stage in dto.Destructible.Stages)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO MiscItemDestructibleStages (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Stage_Index, StageRecordIndex, HealthPercent, ModelDamageStage, Flags, SelfDamagePerSecond,
                    Explosion_ModKey_Name, Explosion_ModKey_Type, Explosion_ModKey_FileName, Explosion_FormKey_ID,
                    Model_File, Model_Data, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @StageIndex, @StageRecordIndex, @HealthPercent, @ModelDamageStage, @Flags, @SelfDamagePerSecond,
                    @ExplosionModKeyName, @ExplosionModKeyType, @ExplosionModKeyFileName, @ExplosionFormKeyId,
                    @ModelFile, @ModelData, @ImportedAtUTC);
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
                    stage.StageIndex,
                    StageRecordIndex = stage.Index,
                    stage.HealthPercent,
                    stage.ModelDamageStage,
                    stage.Flags,
                    stage.SelfDamagePerSecond,
                    ExplosionModKeyName = stage.Explosion?.ModKey.Name,
                    ExplosionModKeyType = stage.Explosion?.ModKey.Type,
                    ExplosionModKeyFileName = stage.Explosion?.ModKey.FileName,
                    ExplosionFormKeyId = stage.Explosion?.Id,
                    ModelFile = stage.Model?.File,
                    ModelData = stage.Model?.Data,
                    dto.ImportedAtUTC
                });
        }
    }

    private void ReplaceMiscItemResources(MiscItemDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM MiscItemResources
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

        foreach (var resource in dto.Resources)
        {
            resource.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO MiscItemResources (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Resource_ModKey_Name, Resource_ModKey_Type, Resource_ModKey_FileName, Resource_FormKey_ID, Resource_Index, Count, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @ResourceModKeyName, @ResourceModKeyType, @ResourceModKeyFileName, @ResourceFormKeyId, @ResourceIndex, @Count, @ImportedAtUTC);
                """,
                new
                {
                    Game = resource.Game.ToString(),
                    ModKeyName = resource.ModKey.Name,
                    ModKeyType = resource.ModKey.Type,
                    ModKeyFileName = resource.ModKey.FileName,
                    FormKeyModKeyName = resource.FormKey.ModKey.Name,
                    FormKeyModKeyType = resource.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = resource.FormKey.ModKey.FileName,
                    FormKeyId = resource.FormKey.Id,
                    ResourceModKeyName = resource.Resource.ModKey.Name,
                    ResourceModKeyType = resource.Resource.ModKey.Type,
                    ResourceModKeyFileName = resource.Resource.ModKey.FileName,
                    ResourceFormKeyId = resource.Resource.Id,
                    resource.ResourceIndex,
                    resource.Count,
                    resource.ImportedAtUTC
                });
        }
    }

    private void DeleteStaleComponentsByPlugin(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.ModKeyDTO modKey, DateTime importedAtUTC)
    {
        Database.Execute(
            """
            DELETE FROM MiscItemComponents
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND ImportedAtUTC <> @ImportedAtUTC;
            """,
            new { Game = game.ToString(), ModKeyName = modKey.Name, ModKeyType = modKey.Type, ModKeyFileName = modKey.FileName, ImportedAtUTC = importedAtUTC });
    }

    private void DeleteStaleResourcesByPlugin(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.ModKeyDTO modKey, DateTime importedAtUTC)
    {
        Database.Execute(
            """
            DELETE FROM MiscItemResources
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND ImportedAtUTC <> @ImportedAtUTC;
            """,
            new { Game = game.ToString(), ModKeyName = modKey.Name, ModKeyType = modKey.Type, ModKeyFileName = modKey.FileName, ImportedAtUTC = importedAtUTC });
    }

    private void DeleteStaleDestructiblesByPlugin(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.ModKeyDTO modKey, DateTime importedAtUTC)
    {
        Database.Execute(
            """
            DELETE FROM MiscItemDestructibles
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND ImportedAtUTC <> @ImportedAtUTC;
            """,
            new { Game = game.ToString(), ModKeyName = modKey.Name, ModKeyType = modKey.Type, ModKeyFileName = modKey.FileName, ImportedAtUTC = importedAtUTC });
    }

    private static MiscItemDTO ToDTO(MiscItemRow record, CreationsForge.Core.Enums.SupportedGame game)
    {
        var dto = new MiscItemDTO
        {
            Game = game,
            ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new CreationsForge.Core.DTOs.Plugins.FormKeyDTO { ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
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
            PreviewTransform = CreateNullableFormKey(record.PreviewTransformModKeyName, record.PreviewTransformModKeyType, record.PreviewTransformModKeyFileName, record.PreviewTransformFormKeyId),
            Name = FromEnglish(record.Name),
            ShortName = FromEnglish(record.ShortName),
            Value = record.Value,
            Weight = record.Weight,
            DirtinessScale = record.DirtinessScale,
            FeaturedItemMessage = CreateNullableFormKey(record.FeaturedItemMessageModKeyName, record.FeaturedItemMessageModKeyType, record.FeaturedItemMessageModKeyFileName, record.FeaturedItemMessageFormKeyId),
            Flag = record.Flag
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static void ApplyLocalizedStrings(MiscItemDTO record, IReadOnlyList<LocalizedStringDTO> localizedStrings)
    {
        record.LocalizedStrings = localizedStrings.ToList();
        record.Name = BuildTranslatedString(localizedStrings, nameof(MiscItemDTO.Name), record.Name);
        record.ShortName = BuildTranslatedString(localizedStrings, nameof(MiscItemDTO.ShortName), record.ShortName);
    }

    private static CreationsForge.Core.DTOs.Plugins.FormKeyDTO CreateFormKey(string modKeyName, int modKeyType, string modKeyFileName, long formKeyId)
    {
        return new CreationsForge.Core.DTOs.Plugins.FormKeyDTO
        {
            ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO
            {
                Name = modKeyName,
                Type = modKeyType,
                FileName = modKeyFileName
            },
            Id = (uint)formKeyId
        };
    }

    private sealed class MiscItemRow : RecordRow
    {
        public string? Name { get; set; }

        public string? ShortName { get; set; }

        public int? Version2 { get; set; }

        public int? VersionControl { get; set; }

        public string? ObjectBoundsFirst { get; set; }

        public string? ObjectBoundsSecond { get; set; }

        public string? TransformsInventoryModKeyName { get; set; }

        public int? TransformsInventoryModKeyType { get; set; }

        public string? TransformsInventoryModKeyFileName { get; set; }

        public long? TransformsInventoryFormKeyId { get; set; }

        public string? PreviewTransformModKeyName { get; set; }

        public int? PreviewTransformModKeyType { get; set; }

        public string? PreviewTransformModKeyFileName { get; set; }

        public long? PreviewTransformFormKeyId { get; set; }

        public int? Value { get; set; }

        public float? Weight { get; set; }

        public float? DirtinessScale { get; set; }

        public string? FeaturedItemMessageModKeyName { get; set; }

        public int? FeaturedItemMessageModKeyType { get; set; }

        public string? FeaturedItemMessageModKeyFileName { get; set; }

        public long? FeaturedItemMessageFormKeyId { get; set; }

        public string? Flag { get; set; }
    }

    private sealed class MiscItemComponentRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public string ComponentModKeyName { get; set; } = string.Empty;

        public int ComponentModKeyType { get; set; }

        public string ComponentModKeyFileName { get; set; } = string.Empty;

        public long ComponentFormKeyId { get; set; }

        public int ComponentIndex { get; set; }

        public int? DisplayIndex { get; set; }

        public int? Count { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class MiscItemDestructibleRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public int? Health { get; set; }

        public int? DESTCount { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class MiscItemDestructibleStageRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public int StageIndex { get; set; }

        public int? StageRecordIndex { get; set; }

        public int? HealthPercent { get; set; }

        public int? ModelDamageStage { get; set; }

        public string? Flags { get; set; }

        public int? SelfDamagePerSecond { get; set; }

        public string? ExplosionModKeyName { get; set; }

        public int? ExplosionModKeyType { get; set; }

        public string? ExplosionModKeyFileName { get; set; }

        public long? ExplosionFormKeyId { get; set; }

        public string? ModelFile { get; set; }

        public string? ModelData { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class MiscItemDestructibleRowAggregate
    {
        public required CreationsForge.Core.DTOs.Plugins.ModKeyDTO ModKey { get; set; }

        public required MiscItemDestructibleDTO Destructible { get; set; }
    }

    private sealed class MiscItemDestructibleStageRowAggregate
    {
        public required CreationsForge.Core.DTOs.Plugins.ModKeyDTO ModKey { get; set; }

        public required MiscItemDestructibleStageDTO Stage { get; set; }
    }

    private sealed class MiscItemResourceRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public string ResourceModKeyName { get; set; } = string.Empty;

        public int ResourceModKeyType { get; set; }

        public string ResourceModKeyFileName { get; set; } = string.Empty;

        public long ResourceFormKeyId { get; set; }

        public int ResourceIndex { get; set; }

        public int? Count { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }
}
