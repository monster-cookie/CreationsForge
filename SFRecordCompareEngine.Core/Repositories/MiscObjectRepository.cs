using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class MiscItemRepository : IMiscItemRepository
{
    private readonly IDatabase Database;

    public MiscItemRepository(IDatabase database)
    {
        Database = database;
    }

    public IList<MiscItemDTO> GetByModKey(ModKey modKey)
    {
        return HydrateChildren(Database.Fetch<MiscItem>("SELECT * FROM MiscItem WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName }).Select(x => new MiscItemDTO(x)).ToList());
    }

    public IList<RecordTreeEntryDTO> GetRecordTreeEntriesByModKey(ModKey modKey)
    {
        return Database.Fetch<MiscItem>("SELECT FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, EditorID FROM MiscItem WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName })
            .Select(x => new RecordTreeEntryDTO
            {
                FormKey = new FormKey(new ModKey(x.FormKeyModKeyName, (ModType)x.FormKeyModKeyType), (uint)x.FormKeyId),
                EditorID = x.EditorId
            })
            .ToList();
    }

    public IList<MiscItemDTO> GetByFormKey(FormKey formKey)
    {
        return HydrateChildren(Database
            .Fetch<MiscItem>("SELECT MiscItem.* FROM MiscItem INNER JOIN Plugins ON Plugins.ModKey_Name = MiscItem.ModKey_Name AND Plugins.ModKey_Type = MiscItem.ModKey_Type AND Plugins.ModKey_FileName = MiscItem.ModKey_FileName WHERE MiscItem.FormKey_ModKey_Name = @FormKeyModKeyName AND MiscItem.FormKey_ModKey_Type = @FormKeyModKeyType AND MiscItem.FormKey_ModKey_FileName = @FormKeyModKeyFileName AND MiscItem.FormKey_ID = @FormKeyID AND Plugins.Enabled = 1 AND Plugins.ExistsOnDisk = 1 AND Plugins.ImportState = @ImportState ORDER BY Plugins.LoadOrderIndex;",
                new { FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = (int)formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyID = formKey.ID, ImportState = nameof(PluginImportState.Current) }).Select(x => new MiscItemDTO(x)).ToList());
    }

    public void Save(MiscItemDTO dto)
    {
        Database.Save(new MiscItem(dto));
        DeleteChildren(dto);
        SaveOptionalChildren(dto);
        SaveCollections(dto);
    }

    private IList<MiscItemDTO> HydrateChildren(IList<MiscItemDTO> records)
    {
        foreach (var record in records)
        {
            var parameters = GetParameters(record);
            var bounds = Database.SingleOrDefault<MiscItemObjectBounds>("WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName AND FormKey_ModKey_Name = @FormKeyModKeyName AND FormKey_ModKey_Type = @FormKeyModKeyType AND FormKey_ModKey_FileName = @FormKeyModKeyFileName AND FormKey_ID = @FormKeyID", parameters);
            record.ObjectBounds = bounds == null ? null : new MiscItemObjectBoundsDTO { FirstX = bounds.FirstX, FirstY = bounds.FirstY, FirstZ = bounds.FirstZ, SecondX = bounds.SecondX, SecondY = bounds.SecondY, SecondZ = bounds.SecondZ };
            var palette = Database.SingleOrDefault<MiscItemObjectPaletteDefaults>("WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName AND FormKey_ModKey_Name = @FormKeyModKeyName AND FormKey_ModKey_Type = @FormKeyModKeyType AND FormKey_ModKey_FileName = @FormKeyModKeyFileName AND FormKey_ID = @FormKeyID", parameters);
            record.ObjectPaletteDefaults = palette == null ? null : new MiscItemObjectPaletteDefaultsDTO { Flags = palette.Flags, SinkMeters = palette.SinkMeters, SinkVariance = palette.SinkVariance, XYOffsetVariance = palette.XYOffsetVariance, FootprintSize = palette.FootprintSize, ScalePercent = palette.ScalePercent, ScaleVariance = palette.ScaleVariance, AngleXDegrees = palette.AngleXDegrees, AngleXVariance = palette.AngleXVariance, AngleYDegrees = palette.AngleYDegrees, AngleYVariance = palette.AngleYVariance, AngleZDegrees = palette.AngleZDegrees, AngleZVariance = palette.AngleZVariance, SlopePercent = palette.SlopePercent, SlopePercentVariance = palette.SlopePercentVariance, Density = palette.Density, FrequencyPercent = palette.FrequencyPercent, SlopeLimit = palette.SlopeLimit, DistanceBelowWater = palette.DistanceBelowWater, DistanceAboveWater = palette.DistanceAboveWater };
            record.Transforms = GetTransforms(parameters);
            record.Model = GetModel(parameters);
            record.CraftingSound = GetSound(parameters, "Crafting");
            record.PickupSound = GetSound(parameters, "Pickup");
            record.DropdownSound = GetSound(parameters, "Dropdown");
            record.Keywords = Database.Fetch<MiscItemKeyword>("SELECT * FROM MiscItemKeywords WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName AND FormKey_ModKey_Name = @FormKeyModKeyName AND FormKey_ModKey_Type = @FormKeyModKeyType AND FormKey_ModKey_FileName = @FormKeyModKeyFileName AND FormKey_ID = @FormKeyID ORDER BY Keyword_Index", parameters).Select(x => FormKey.Factory(x.KeywordFormKey)).ToList();
            record.Destructible = GetDestructible(parameters);
        }

        return records;
    }

    private MiscItemTransformsDTO? GetTransforms(object parameters)
    {
        var model = Database.SingleOrDefault<MiscItemTransforms>("WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName AND FormKey_ModKey_Name = @FormKeyModKeyName AND FormKey_ModKey_Type = @FormKeyModKeyType AND FormKey_ModKey_FileName = @FormKeyModKeyFileName AND FormKey_ID = @FormKeyID", parameters);
        return model == null ? null : new MiscItemTransformsDTO { InventoryIconFormKey = ParseFormKey(model.InventoryIconFormKey), OutpostFormKey = ParseFormKey(model.OutpostFormKey), ShipFormKey = ParseFormKey(model.ShipFormKey), PreviewFormKey = ParseFormKey(model.PreviewFormKey), InventoryFormKey = ParseFormKey(model.InventoryFormKey), WorkbenchFormKey = ParseFormKey(model.WorkbenchFormKey), MainGameUIFormKey = ParseFormKey(model.MainGameUIFormKey) };
    }

    private MiscItemModelDTO? GetModel(object parameters)
    {
        var model = Database.SingleOrDefault<MiscItemModel>("WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName AND FormKey_ModKey_Name = @FormKeyModKeyName AND FormKey_ModKey_Type = @FormKeyModKeyType AND FormKey_ModKey_FileName = @FormKeyModKeyFileName AND FormKey_ID = @FormKeyID", parameters);
        return model == null ? null : new MiscItemModelDTO { File = model.File, TextureFileHashes = model.TextureFileHashes, LightLayer = (uint?)model.LightLayer, Flags = model.Flags, ColorRemappingIndex = model.ColorRemappingIndex, FlagsVestigial = model.FlagsVestigial, MaterialSwaps = Database.Fetch<MiscItemModelMaterialSwap>("SELECT * FROM MiscItemModelMaterialSwaps WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName AND FormKey_ModKey_Name = @FormKeyModKeyName AND FormKey_ModKey_Type = @FormKeyModKeyType AND FormKey_ModKey_FileName = @FormKeyModKeyFileName AND FormKey_ID = @FormKeyID ORDER BY MaterialSwap_Index", parameters).Select(x => FormKey.Factory(x.MaterialSwapFormKey)).ToList() };
    }

    private MiscItemSoundDTO? GetSound(object parameters, string soundType)
    {
        var model = Database.SingleOrDefault<MiscItemSound>("WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName AND FormKey_ModKey_Name = @FormKeyModKeyName AND FormKey_ModKey_Type = @FormKeyModKeyType AND FormKey_ModKey_FileName = @FormKeyModKeyFileName AND FormKey_ID = @FormKeyID AND SoundType = @SoundType", MergeParameters(parameters, soundType));
        return model == null ? null : new MiscItemSoundDTO { Start = model.Start, Stop = model.Stop, ConditionFormKey = ParseFormKey(model.ConditionFormKey), EventMappingFormKey = ParseFormKey(model.EventMappingFormKey) };
    }

    private MiscItemDestructibleDTO? GetDestructible(object parameters)
    {
        var model = Database.SingleOrDefault<MiscItemDestructible>("WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName AND FormKey_ModKey_Name = @FormKeyModKeyName AND FormKey_ModKey_Type = @FormKeyModKeyType AND FormKey_ModKey_FileName = @FormKeyModKeyFileName AND FormKey_ID = @FormKeyID", parameters);
        if (model == null)
        {
            return null;
        }

        return new MiscItemDestructibleDTO
        {
            Health = model.Health,
            Count = (byte?)model.StageCount,
            Flags = model.Flags,
            Resistances = Database.Fetch<MiscItemDestructibleResistance>("SELECT * FROM MiscItemDestructibleResistances WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName AND FormKey_ModKey_Name = @FormKeyModKeyName AND FormKey_ModKey_Type = @FormKeyModKeyType AND FormKey_ModKey_FileName = @FormKeyModKeyFileName AND FormKey_ID = @FormKeyID ORDER BY Resistance_Index", parameters).Select(x => new MiscItemDestructibleResistanceDTO { DamageTypeFormKey = FormKey.Factory(x.DamageTypeFormKey), Value = (uint)x.Value, ResistanceIndex = x.ResistanceIndex }).ToList(),
            Stages = Database.Fetch<MiscItemDestructionStage>("SELECT * FROM MiscItemDestructionStages WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName AND FormKey_ModKey_Name = @FormKeyModKeyName AND FormKey_ModKey_Type = @FormKeyModKeyType AND FormKey_ModKey_FileName = @FormKeyModKeyFileName AND FormKey_ID = @FormKeyID ORDER BY Stage_Index", parameters).Select(x => new MiscItemDestructionStageDTO { StageIndex = x.StageIndex, HealthPercent = (byte?)x.HealthPercent, Index = (byte?)x.SourceIndex, ModelDamageStage = (byte?)x.ModelDamageStage, Flags = x.Flags, SelfDamagePerSecond = x.SelfDamagePerSecond, ExplosionFormKey = ParseFormKey(x.ExplosionFormKey), DebrisFormKey = ParseFormKey(x.DebrisFormKey), DebrisCount = x.DebrisCount, SequenceName = x.SequenceName, ModelFile = x.ModelFile, ModelLightLayer = (uint?)x.ModelLightLayer, ModelFlags = x.ModelFlags, ModelMaterialSwaps = Database.Fetch<MiscItemDestructionStageMaterialSwap>("SELECT * FROM MiscItemDestructionStageMaterialSwaps WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName AND FormKey_ModKey_Name = @FormKeyModKeyName AND FormKey_ModKey_Type = @FormKeyModKeyType AND FormKey_ModKey_FileName = @FormKeyModKeyFileName AND FormKey_ID = @FormKeyID AND Stage_Index = @StageIndex ORDER BY MaterialSwap_Index", MergeStageParameters(parameters, x.StageIndex)).Select(materialSwap => FormKey.Factory(materialSwap.MaterialSwapFormKey)).ToList() }).ToList()
        };
    }

    private void SaveOptionalChildren(MiscItemDTO dto)
    {
        var values = GetValues(dto);
        if (dto.ObjectBounds != null) Database.Save(new MiscItemObjectBounds(dto));
        if (dto.ObjectPaletteDefaults != null) Database.Save(new MiscItemObjectPaletteDefaults(dto));
        if (dto.Transforms != null) Database.Save(CreateTransforms(dto, values));
        if (dto.Model != null) Database.Save(CreateModel(dto, values));
        SaveSound(dto, values, "Crafting", dto.CraftingSound);
        SaveSound(dto, values, "Pickup", dto.PickupSound);
        SaveSound(dto, values, "Dropdown", dto.DropdownSound);
        if (dto.Destructible != null) Database.Save(new MiscItemDestructible { ModKeyName = values.ModKeyName, ModKeyType = values.ModKeyType, ModKeyFileName = values.ModKeyFileName, FormKeyModKeyName = values.FormKeyModKeyName, FormKeyModKeyType = values.FormKeyModKeyType, FormKeyModKeyFileName = values.FormKeyModKeyFileName, FormKeyId = values.FormKeyId, Health = dto.Destructible.Health, StageCount = dto.Destructible.Count, Flags = dto.Destructible.Flags, ImportedAtUTC = dto.ImportedAtUTC });
    }

    private void SaveCollections(MiscItemDTO dto)
    {
        var values = GetValues(dto);
        foreach (var item in dto.Keywords.Select((formKey, index) => (formKey, index))) Database.Save(new MiscItemKeyword { ModKeyName = values.ModKeyName, ModKeyType = values.ModKeyType, ModKeyFileName = values.ModKeyFileName, FormKeyModKeyName = values.FormKeyModKeyName, FormKeyModKeyType = values.FormKeyModKeyType, FormKeyModKeyFileName = values.FormKeyModKeyFileName, FormKeyId = values.FormKeyId, KeywordFormKey = item.formKey.ToString(), KeywordIndex = item.index, ImportedAtUTC = dto.ImportedAtUTC });
        foreach (var item in (dto.Model?.MaterialSwaps ?? new List<FormKey>()).Select((formKey, index) => (formKey, index))) Database.Save(new MiscItemModelMaterialSwap { ModKeyName = values.ModKeyName, ModKeyType = values.ModKeyType, ModKeyFileName = values.ModKeyFileName, FormKeyModKeyName = values.FormKeyModKeyName, FormKeyModKeyType = values.FormKeyModKeyType, FormKeyModKeyFileName = values.FormKeyModKeyFileName, FormKeyId = values.FormKeyId, MaterialSwapFormKey = item.formKey.ToString(), MaterialSwapIndex = item.index, ImportedAtUTC = dto.ImportedAtUTC });
        foreach (var item in dto.Destructible?.Resistances ?? new List<MiscItemDestructibleResistanceDTO>()) Database.Save(new MiscItemDestructibleResistance { ModKeyName = values.ModKeyName, ModKeyType = values.ModKeyType, ModKeyFileName = values.ModKeyFileName, FormKeyModKeyName = values.FormKeyModKeyName, FormKeyModKeyType = values.FormKeyModKeyType, FormKeyModKeyFileName = values.FormKeyModKeyFileName, FormKeyId = values.FormKeyId, DamageTypeFormKey = item.DamageTypeFormKey.ToString(), Value = item.Value, ResistanceIndex = item.ResistanceIndex, ImportedAtUTC = dto.ImportedAtUTC });
        foreach (var item in dto.Destructible?.Stages ?? new List<MiscItemDestructionStageDTO>())
        {
            Database.Save(new MiscItemDestructionStage { ModKeyName = values.ModKeyName, ModKeyType = values.ModKeyType, ModKeyFileName = values.ModKeyFileName, FormKeyModKeyName = values.FormKeyModKeyName, FormKeyModKeyType = values.FormKeyModKeyType, FormKeyModKeyFileName = values.FormKeyModKeyFileName, FormKeyId = values.FormKeyId, StageIndex = item.StageIndex, HealthPercent = item.HealthPercent, SourceIndex = item.Index, ModelDamageStage = item.ModelDamageStage, Flags = item.Flags, SelfDamagePerSecond = item.SelfDamagePerSecond, ExplosionFormKey = item.ExplosionFormKey?.ToString(), DebrisFormKey = item.DebrisFormKey?.ToString(), DebrisCount = item.DebrisCount, SequenceName = item.SequenceName, ModelFile = item.ModelFile, ModelLightLayer = item.ModelLightLayer, ModelFlags = item.ModelFlags, ImportedAtUTC = dto.ImportedAtUTC });
            foreach (var materialSwap in item.ModelMaterialSwaps.Select((formKey, index) => (formKey, index))) Database.Save(new MiscItemDestructionStageMaterialSwap { ModKeyName = values.ModKeyName, ModKeyType = values.ModKeyType, ModKeyFileName = values.ModKeyFileName, FormKeyModKeyName = values.FormKeyModKeyName, FormKeyModKeyType = values.FormKeyModKeyType, FormKeyModKeyFileName = values.FormKeyModKeyFileName, FormKeyId = values.FormKeyId, StageIndex = item.StageIndex, MaterialSwapFormKey = materialSwap.formKey.ToString(), MaterialSwapIndex = materialSwap.index, ImportedAtUTC = dto.ImportedAtUTC });
        }
    }

    private void DeleteChildren(MiscItemDTO dto)
    {
        var parameters = GetParameters(dto);
        foreach (var table in new[] { "MiscItemObjectBounds", "MiscItemObjectPaletteDefaults", "MiscItemTransforms", "MiscItemModels", "MiscItemSounds", "MiscItemKeywords", "MiscItemDestructibles" })
        {
            Database.Execute($"DELETE FROM {table} WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName AND FormKey_ModKey_Name = @FormKeyModKeyName AND FormKey_ModKey_Type = @FormKeyModKeyType AND FormKey_ModKey_FileName = @FormKeyModKeyFileName AND FormKey_ID = @FormKeyID", parameters);
        }
    }

    private static MiscItemTransforms CreateTransforms(MiscItemDTO dto, dynamic values) => new() { ModKeyName = values.ModKeyName, ModKeyType = values.ModKeyType, ModKeyFileName = values.ModKeyFileName, FormKeyModKeyName = values.FormKeyModKeyName, FormKeyModKeyType = values.FormKeyModKeyType, FormKeyModKeyFileName = values.FormKeyModKeyFileName, FormKeyId = values.FormKeyId, InventoryIconFormKey = dto.Transforms!.InventoryIconFormKey?.ToString(), OutpostFormKey = dto.Transforms.OutpostFormKey?.ToString(), ShipFormKey = dto.Transforms.ShipFormKey?.ToString(), PreviewFormKey = dto.Transforms.PreviewFormKey?.ToString(), InventoryFormKey = dto.Transforms.InventoryFormKey?.ToString(), WorkbenchFormKey = dto.Transforms.WorkbenchFormKey?.ToString(), MainGameUIFormKey = dto.Transforms.MainGameUIFormKey?.ToString(), ImportedAtUTC = dto.ImportedAtUTC };
    private static MiscItemModel CreateModel(MiscItemDTO dto, dynamic values) => new() { ModKeyName = values.ModKeyName, ModKeyType = values.ModKeyType, ModKeyFileName = values.ModKeyFileName, FormKeyModKeyName = values.FormKeyModKeyName, FormKeyModKeyType = values.FormKeyModKeyType, FormKeyModKeyFileName = values.FormKeyModKeyFileName, FormKeyId = values.FormKeyId, File = dto.Model!.File, TextureFileHashes = dto.Model.TextureFileHashes, LightLayer = dto.Model.LightLayer, Flags = dto.Model.Flags, ColorRemappingIndex = dto.Model.ColorRemappingIndex, FlagsVestigial = dto.Model.FlagsVestigial, ImportedAtUTC = dto.ImportedAtUTC };
    private void SaveSound(MiscItemDTO dto, dynamic values, string soundType, MiscItemSoundDTO? sound) { if (sound != null) Database.Save(new MiscItemSound { ModKeyName = values.ModKeyName, ModKeyType = values.ModKeyType, ModKeyFileName = values.ModKeyFileName, FormKeyModKeyName = values.FormKeyModKeyName, FormKeyModKeyType = values.FormKeyModKeyType, FormKeyModKeyFileName = values.FormKeyModKeyFileName, FormKeyId = values.FormKeyId, SoundType = soundType, Start = sound.Start, Stop = sound.Stop, ConditionFormKey = sound.ConditionFormKey?.ToString(), EventMappingFormKey = sound.EventMappingFormKey?.ToString(), ImportedAtUTC = dto.ImportedAtUTC }); }
    private static FormKey? ParseFormKey(string? value) => string.IsNullOrWhiteSpace(value) ? null : FormKey.Factory(value);
    private static object MergeParameters(object parameters, string soundType) { dynamic p = parameters; return new { p.ModKeyName, p.ModKeyType, p.ModKeyFileName, p.FormKeyModKeyName, p.FormKeyModKeyType, p.FormKeyModKeyFileName, p.FormKeyID, SoundType = soundType }; }
    private static object MergeStageParameters(object parameters, int stageIndex) { dynamic p = parameters; return new { p.ModKeyName, p.ModKeyType, p.ModKeyFileName, p.FormKeyModKeyName, p.FormKeyModKeyType, p.FormKeyModKeyFileName, p.FormKeyID, StageIndex = stageIndex }; }
    private static object GetParameters(MiscItemDTO dto) => new { ModKeyName = dto.ModKey.Name, ModKeyType = (int)dto.ModKey.Type, ModKeyFileName = dto.ModKey.FileName, FormKeyModKeyName = dto.FormKey.ModKey.Name, FormKeyModKeyType = (int)dto.FormKey.ModKey.Type, FormKeyModKeyFileName = dto.FormKey.ModKey.FileName, FormKeyID = dto.FormKey.ID };
    private static dynamic GetValues(MiscItemDTO dto) => new { ModKeyName = dto.ModKey.Name, ModKeyType = (int)dto.ModKey.Type, ModKeyFileName = dto.ModKey.FileName, FormKeyModKeyName = dto.FormKey.ModKey.Name, FormKeyModKeyType = (int)dto.FormKey.ModKey.Type, FormKeyModKeyFileName = dto.FormKey.ModKey.FileName, FormKeyId = (int)dto.FormKey.ID };
}
