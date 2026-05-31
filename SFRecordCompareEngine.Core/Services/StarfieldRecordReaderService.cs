using System.Globalization;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class StarfieldRecordReaderService : IStarfieldRecordReaderService
{
    #region FormList

    public IReadOnlyList<FormListDTO> GetFormLists(PluginDTO plugin)
    {
        var mod = LoadMod(plugin.ModKey);
        return mod.FormLists.Select(record => new FormListDTO
        {
            ModKey = plugin.ModKey,
            FormKey = record.FormKey,
            EditorID = record.EditorID ?? string.Empty,
            FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags,
            Version2 = record.Version2,
            VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow,
            AddToListFormKey = record.AddToList.FormKey,
            Items = record.Items.Select(item =>
            {
                item.TryGetModKey(out var itemModKey);
                return new FormListItemDataDTO
                {
                    ItemModKey = itemModKey,
                    ItemFormKey = item.FormKey
                };
            }).ToList()
        }).ToList();
    }

    #endregion

    #region GameSettings

    public IReadOnlyList<GameSettingDTO> GetGameSettings(PluginDTO plugin)
    {
        var mod = LoadMod(plugin.ModKey);
        return mod.GameSettings.Select(record => new GameSettingDTO
        {
            ModKey = plugin.ModKey,
            FormKey = record.FormKey,
            EditorID = record.EditorID ?? string.Empty,
            FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags,
            Version2 = record.Version2,
            VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow,
            SettingType = GetGameSettingType(record),
            Data = GetGameSettingData(record),
            RawData = GetGameSettingRawData(record),
            IsCompressed = 0,
            IsDeleted = 0
        }).ToList();
    }

    private static string GetGameSettingType(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingBoolGetter => "GameSettingBool",
            IGameSettingFloatGetter => "GameSettingFloat",
            IGameSettingIntGetter => "GameSettingInt",
            IGameSettingStringGetter => "GameSettingString",
            IGameSettingUIntGetter => "GameSettingUInt",
            _ => record.GetType().Name
        };
    }

    private static string? GetGameSettingData(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingBoolGetter gameSetting => Convert.ToString(gameSetting.Data, CultureInfo.InvariantCulture),
            IGameSettingFloatGetter gameSetting => Convert.ToString(gameSetting.Data, CultureInfo.InvariantCulture),
            IGameSettingIntGetter gameSetting => Convert.ToString(gameSetting.Data, CultureInfo.InvariantCulture),
            IGameSettingStringGetter gameSetting => gameSetting.Data?.ToString(),
            IGameSettingUIntGetter gameSetting => Convert.ToString(gameSetting.Data, CultureInfo.InvariantCulture),
            _ => null
        };
    }

    private static double? GetGameSettingRawData(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingBoolGetter gameSetting => gameSetting.Data == true ? 1 : 0,
            IGameSettingFloatGetter gameSetting => gameSetting.Data,
            IGameSettingIntGetter gameSetting => gameSetting.Data,
            IGameSettingUIntGetter gameSetting => gameSetting.Data,
            _ => null
        };
    }

    public IReadOnlyList<GlobalDTO> GetGlobals(PluginDTO plugin)
    {
        return LoadMod(plugin.ModKey).Globals.Select(record => new GlobalDTO
        {
            ModKey = plugin.ModKey, FormKey = record.FormKey, EditorID = record.EditorID ?? string.Empty, FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags, Version2 = record.Version2, VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow, Data = record.Data
        }).ToList();
    }

    public IReadOnlyList<MiscItemDTO> GetMiscItems(PluginDTO plugin)
    {
        return LoadMod(plugin.ModKey).MiscItems.Select(record => new MiscItemDTO
        {
            ModKey = plugin.ModKey, FormKey = record.FormKey, EditorID = record.EditorID ?? string.Empty, FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags, Version2 = record.Version2, VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow, Name = record.Name?.Lookup(Language.English), ShortName = record.ShortName?.Lookup(Language.English), Value = record.Value, Weight = record.Weight
        }).ToList();
    }

    public IReadOnlyList<KeywordDTO> GetKeywords(PluginDTO plugin)
    {
        return LoadMod(plugin.ModKey).Keywords.Select(record => new KeywordDTO
        {
            ModKey = plugin.ModKey, FormKey = record.FormKey, EditorID = record.EditorID ?? string.Empty, FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags, Version2 = record.Version2, VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow, Name = record.Name?.Lookup(Language.English), Color = record.Color.ToString() ?? string.Empty, Type = record.Type.ToString() ?? string.Empty,
            Notes = record.Notes, FlashLinkageName = record.FlashLinkageName, AttractionRuleFormKey = record.AttractionRule.FormKey
        }).ToList();
    }

    public IReadOnlyList<NPCDTO> GetNPCs(PluginDTO plugin)
    {
        return LoadMod(plugin.ModKey).Npcs.Select(record => new NPCDTO
        {
            ModKey = plugin.ModKey, FormKey = record.FormKey, EditorID = record.EditorID ?? string.Empty, FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags, Version2 = record.Version2, VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow, Name = record.Name?.Lookup(Language.English), ShortName = record.ShortName?.Lookup(Language.English), LongName = record.LongName?.Lookup(Language.English),
            DispositionBase = record.DispositionBase, Aggression = record.Aggression.ToString(), Confidence = record.Confidence.ToString(),
            EnergyLevel = record.EnergyLevel, Responsibility = record.Responsibility.ToString(), Assistance = record.Assistance.ToString(),
            GearedUpWeapons = record.GearedUpWeapons, HeightMin = record.HeightMin, HeightMax = record.HeightMax, SkinToneIndex = record.SkinToneIndex,
            Pronoun = record.Pronoun?.ToString(), VoiceFormKey = record.Voice.FormKey, RaceFormKey = record.Race.FormKey,
            CombatOverridePackageListFormKey = record.CombatOverridePackageList.FormKey, CombatStyleFormKey = record.CombatStyle.FormKey,
            DefaultPackageListFormKey = record.DefaultPackageList.FormKey, CrimeFactionFormKey = record.CrimeFaction.FormKey
        }).ToList();
    }

    public IReadOnlyList<ActorValueInformationDTO> GetActorValueInformation(PluginDTO plugin)
    {
        return LoadMod(plugin.ModKey).ActorValueInformation.Select(record => new ActorValueInformationDTO
        {
            ModKey = plugin.ModKey, FormKey = record.FormKey, EditorID = record.EditorID ?? string.Empty, FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags, Version2 = record.Version2, VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow, Name = record.Name?.Lookup(Language.English), Abbreviation = record.Abbreviation?.Lookup(Language.English), ContextNotes = record.ContextNotes,
            DefaultValue = record.DefaultValue, Flags = record.Flags.ToString(), Type = record.Type?.ToString(), Min = record.Min, Max = record.Max
        }).ToList();
    }

    public IReadOnlyList<MagicEffectDTO> GetMagicEffects(PluginDTO plugin)
    {
        return LoadMod(plugin.ModKey).MagicEffects.Select(record => new MagicEffectDTO
        {
            ModKey = plugin.ModKey, FormKey = record.FormKey, EditorID = record.EditorID ?? string.Empty, FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags, Version2 = record.Version2, VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow, Name = record.Name?.Lookup(Language.English), Description = record.Description?.Lookup(Language.English), Flags = record.Flags.ToString(),
            CastType = record.CastType.ToString(), TargetType = record.TargetType.ToString(), ActorValue2FormKey = record.ActorValue2.FormKey,
            ResistValueFormKey = record.ResistValue.FormKey, PerkToApplyFormKey = record.PerkToApply.FormKey, EquipAbilityFormKey = record.EquipAbility.FormKey,
            ExplosionFormKey = record.Explosion.FormKey, CastingArtFormKey = record.CastingArt.FormKey, HitEffectArtFormKey = record.HitEffectArt.FormKey,
            HitShaderFormKey = record.HitShader.FormKey, ImageSpaceModifierFormKey = record.ImageSpaceModifier.FormKey,
            ImpactDataFormKey = record.ImpactData.FormKey, ProjectileFormKey = record.Projectile.FormKey
        }).ToList();
    }

    public IReadOnlyList<PerkDTO> GetPerks(PluginDTO plugin)
    {
        return LoadMod(plugin.ModKey).Perks.Select(record => new PerkDTO
        {
            ModKey = plugin.ModKey, FormKey = record.FormKey, EditorID = record.EditorID ?? string.Empty, FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags, Version2 = record.Version2, VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow, Name = record.Name?.Lookup(Language.English), Description = record.Description?.Lookup(Language.English), Flags = record.Flags.ToString(),
            SkillGroup = record.SkillGroup.ToString(), CrewAssignment = record.CrewAssignment.ToString(), PerkIcon = record.PerkIcon
        }).ToList();
    }

    private static IStarfieldModGetter LoadMod(ModKey modKey)
    {
        var environment = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
        var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(Path.Join(environment.DataFolderPath, modKey.FileName))
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(environment.DataFolderPath)
            .Construct();
        return mod;
    }

    #endregion
}