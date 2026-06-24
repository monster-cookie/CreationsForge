using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class FactionRepository : TypedRecordRepositoryBase, IFactionRepository
{
    private readonly IRecordComponentRepository RecordComponentRepository;
    private readonly IConditionRuleRepository ConditionRuleRepository;

    public FactionRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        IRecordComponentRepository recordComponentRepository,
        IConditionRuleRepository conditionRuleRepository)
        : base(database, recordInstanceRepository)
    {
        RecordComponentRepository = recordComponentRepository;
        ConditionRuleRepository = conditionRuleRepository;
    }

    public override string RecordType => RecordTypeCatalog.Faction.RecordID;

    protected override string TableName => RecordTypeCatalog.Faction.TableName;

    public IReadOnlyList<FactionDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FetchByFormKey<FactionRow>(game, formKey, GetFactionSelectColumns())
            .Select(record => ToDTO(record, game))
            .ToList();
        var relations = FetchRelationsByFormKey(game, formKey);
        var ranks = FetchRanksByFormKey(game, formKey);
        var conditions = ConditionRuleRepository.GetByFormKey(game, RecordTypeCatalog.Faction.RecordID, formKey);
        var components = RecordComponentRepository.GetByFormKey(game, RecordTypeCatalog.Faction.RecordID, formKey);
        foreach (var record in records)
        {
            record.Relations = relations.Where(relation => IsSameModKey(relation.ModKey, record.ModKey)).OrderBy(relation => relation.RelationIndex).ToList();
            record.Ranks = ranks.Where(rank => IsSameModKey(rank.ModKey, record.ModKey)).OrderBy(rank => rank.RankIndex).ToList();
            record.Conditions = conditions
                .Where(condition => IsSameModKey(condition.ModKey, record.ModKey) && string.Equals(condition.ConditionSlot, "Conditions", StringComparison.Ordinal))
                .OrderBy(condition => condition.ConditionIndex)
                .ToList();
            record.Components = components.Where(component => IsSameModKey(component.ModKey, record.ModKey)).OrderBy(component => component.ComponentIndex).ToList();
        }

        return records;
    }

    public void Save(FactionDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO Factions (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, VersionControl, Name, Flags, FormationRadius,
                Keyword_ModKey_Name, Keyword_ModKey_Type, Keyword_ModKey_FileName, Keyword_FormKey_ID,
                Herd_ModKey_Name, Herd_ModKey_Type, Herd_ModKey_FileName, Herd_FormKey_ID,
                VoiceType_ModKey_Name, VoiceType_ModKey_Type, VoiceType_ModKey_FileName, VoiceType_FormKey_ID,
                SharedCrimeFactionList_ModKey_Name, SharedCrimeFactionList_ModKey_Type, SharedCrimeFactionList_ModKey_FileName, SharedCrimeFactionList_FormKey_ID,
                VendorBuySellList_ModKey_Name, VendorBuySellList_ModKey_Type, VendorBuySellList_ModKey_FileName, VendorBuySellList_FormKey_ID,
                MerchantContainer_ModKey_Name, MerchantContainer_ModKey_Type, MerchantContainer_ModKey_FileName, MerchantContainer_FormKey_ID,
                ExteriorJailMarker_ModKey_Name, ExteriorJailMarker_ModKey_Type, ExteriorJailMarker_ModKey_FileName, ExteriorJailMarker_FormKey_ID,
                FollowerWaitMarker_ModKey_Name, FollowerWaitMarker_ModKey_Type, FollowerWaitMarker_ModKey_FileName, FollowerWaitMarker_FormKey_ID,
                StolenGoodsContainer_ModKey_Name, StolenGoodsContainer_ModKey_Type, StolenGoodsContainer_ModKey_FileName, StolenGoodsContainer_FormKey_ID,
                PlayerInventoryContainer_ModKey_Name, PlayerInventoryContainer_ModKey_Type, PlayerInventoryContainer_ModKey_FileName, PlayerInventoryContainer_FormKey_ID,
                JailOutfit_ModKey_Name, JailOutfit_ModKey_Type, JailOutfit_ModKey_FileName, JailOutfit_FormKey_ID,
                CrimeValues_Arrest, CrimeValues_AttackOnSight, CrimeValues_Murder, CrimeValues_Assault, CrimeValues_Trespass, CrimeValues_Pickpocket,
                CrimeValues_Steal, CrimeValues_StealMult, CrimeValues_StealMultiplier, CrimeValues_Escape, CrimeValues_Werewolf, CrimeValues_WerewolfUnused,
                CrimeValues_Unknown, CrimeValues_Piracy, CrimeValues_SmuggleMultiplier, VendorValues_StartHour, VendorValues_EndHour, VendorValues_Radius,
                VendorValues_BuysStolenItems, VendorValues_BuysNonStolenItems, VendorValues_BuySellEverythingNotInList, VendorLocation_MutagenObjectType,
                VendorLocation_Target_MutagenObjectType, VendorLocation_Target_Type, VendorLocation_Target_Link_ModKey_Name, VendorLocation_Target_Link_ModKey_Type,
                VendorLocation_Target_Link_ModKey_FileName, VendorLocation_Target_Link_FormKey_ID)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @VersionControl, @Name, @Flags, @FormationRadius,
                @KeywordModKeyName, @KeywordModKeyType, @KeywordModKeyFileName, @KeywordFormKeyId,
                @HerdModKeyName, @HerdModKeyType, @HerdModKeyFileName, @HerdFormKeyId,
                @VoiceTypeModKeyName, @VoiceTypeModKeyType, @VoiceTypeModKeyFileName, @VoiceTypeFormKeyId,
                @SharedCrimeFactionListModKeyName, @SharedCrimeFactionListModKeyType, @SharedCrimeFactionListModKeyFileName, @SharedCrimeFactionListFormKeyId,
                @VendorBuySellListModKeyName, @VendorBuySellListModKeyType, @VendorBuySellListModKeyFileName, @VendorBuySellListFormKeyId,
                @MerchantContainerModKeyName, @MerchantContainerModKeyType, @MerchantContainerModKeyFileName, @MerchantContainerFormKeyId,
                @ExteriorJailMarkerModKeyName, @ExteriorJailMarkerModKeyType, @ExteriorJailMarkerModKeyFileName, @ExteriorJailMarkerFormKeyId,
                @FollowerWaitMarkerModKeyName, @FollowerWaitMarkerModKeyType, @FollowerWaitMarkerModKeyFileName, @FollowerWaitMarkerFormKeyId,
                @StolenGoodsContainerModKeyName, @StolenGoodsContainerModKeyType, @StolenGoodsContainerModKeyFileName, @StolenGoodsContainerFormKeyId,
                @PlayerInventoryContainerModKeyName, @PlayerInventoryContainerModKeyType, @PlayerInventoryContainerModKeyFileName, @PlayerInventoryContainerFormKeyId,
                @JailOutfitModKeyName, @JailOutfitModKeyType, @JailOutfitModKeyFileName, @JailOutfitFormKeyId,
                @CrimeValuesArrest, @CrimeValuesAttackOnSight, @CrimeValuesMurder, @CrimeValuesAssault, @CrimeValuesTrespass, @CrimeValuesPickpocket,
                @CrimeValuesSteal, @CrimeValuesStealMult, @CrimeValuesStealMultiplier, @CrimeValuesEscape, @CrimeValuesWerewolf, @CrimeValuesWerewolfUnused,
                @CrimeValuesUnknown, @CrimeValuesPiracy, @CrimeValuesSmuggleMultiplier, @VendorValuesStartHour, @VendorValuesEndHour, @VendorValuesRadius,
                @VendorValuesBuysStolenItems, @VendorValuesBuysNonStolenItems, @VendorValuesBuySellEverythingNotInList, @VendorLocationMutagenObjectType,
                @VendorLocationTargetMutagenObjectType, @VendorLocationTargetType, @VendorLocationTargetLinkModKeyName, @VendorLocationTargetLinkModKeyType,
                @VendorLocationTargetLinkModKeyFileName, @VendorLocationTargetLinkFormKeyId);
            """,
            CreateFactionParameters(dto));
        ReplaceRelations(dto);
        ReplaceRanks(dto);
    }

    private static IReadOnlyList<SelectColumnDefinition> GetFactionSelectColumns()
    {
        return
        [
            SelectColumn("Version2"),
            SelectColumn("VersionControl"),
            SelectColumn("Name"),
            SelectColumn("Flags"),
            SelectColumn("FormationRadius"),
            SelectColumn("Keyword_ModKey_Name", "KeywordModKeyName"),
            SelectColumn("Keyword_ModKey_Type", "KeywordModKeyType"),
            SelectColumn("Keyword_ModKey_FileName", "KeywordModKeyFileName"),
            SelectColumn("Keyword_FormKey_ID", "KeywordFormKeyId"),
            SelectColumn("Herd_ModKey_Name", "HerdModKeyName"),
            SelectColumn("Herd_ModKey_Type", "HerdModKeyType"),
            SelectColumn("Herd_ModKey_FileName", "HerdModKeyFileName"),
            SelectColumn("Herd_FormKey_ID", "HerdFormKeyId"),
            SelectColumn("VoiceType_ModKey_Name", "VoiceTypeModKeyName"),
            SelectColumn("VoiceType_ModKey_Type", "VoiceTypeModKeyType"),
            SelectColumn("VoiceType_ModKey_FileName", "VoiceTypeModKeyFileName"),
            SelectColumn("VoiceType_FormKey_ID", "VoiceTypeFormKeyId"),
            SelectColumn("SharedCrimeFactionList_ModKey_Name", "SharedCrimeFactionListModKeyName"),
            SelectColumn("SharedCrimeFactionList_ModKey_Type", "SharedCrimeFactionListModKeyType"),
            SelectColumn("SharedCrimeFactionList_ModKey_FileName", "SharedCrimeFactionListModKeyFileName"),
            SelectColumn("SharedCrimeFactionList_FormKey_ID", "SharedCrimeFactionListFormKeyId"),
            SelectColumn("VendorBuySellList_ModKey_Name", "VendorBuySellListModKeyName"),
            SelectColumn("VendorBuySellList_ModKey_Type", "VendorBuySellListModKeyType"),
            SelectColumn("VendorBuySellList_ModKey_FileName", "VendorBuySellListModKeyFileName"),
            SelectColumn("VendorBuySellList_FormKey_ID", "VendorBuySellListFormKeyId"),
            SelectColumn("MerchantContainer_ModKey_Name", "MerchantContainerModKeyName"),
            SelectColumn("MerchantContainer_ModKey_Type", "MerchantContainerModKeyType"),
            SelectColumn("MerchantContainer_ModKey_FileName", "MerchantContainerModKeyFileName"),
            SelectColumn("MerchantContainer_FormKey_ID", "MerchantContainerFormKeyId"),
            SelectColumn("ExteriorJailMarker_ModKey_Name", "ExteriorJailMarkerModKeyName"),
            SelectColumn("ExteriorJailMarker_ModKey_Type", "ExteriorJailMarkerModKeyType"),
            SelectColumn("ExteriorJailMarker_ModKey_FileName", "ExteriorJailMarkerModKeyFileName"),
            SelectColumn("ExteriorJailMarker_FormKey_ID", "ExteriorJailMarkerFormKeyId"),
            SelectColumn("FollowerWaitMarker_ModKey_Name", "FollowerWaitMarkerModKeyName"),
            SelectColumn("FollowerWaitMarker_ModKey_Type", "FollowerWaitMarkerModKeyType"),
            SelectColumn("FollowerWaitMarker_ModKey_FileName", "FollowerWaitMarkerModKeyFileName"),
            SelectColumn("FollowerWaitMarker_FormKey_ID", "FollowerWaitMarkerFormKeyId"),
            SelectColumn("StolenGoodsContainer_ModKey_Name", "StolenGoodsContainerModKeyName"),
            SelectColumn("StolenGoodsContainer_ModKey_Type", "StolenGoodsContainerModKeyType"),
            SelectColumn("StolenGoodsContainer_ModKey_FileName", "StolenGoodsContainerModKeyFileName"),
            SelectColumn("StolenGoodsContainer_FormKey_ID", "StolenGoodsContainerFormKeyId"),
            SelectColumn("PlayerInventoryContainer_ModKey_Name", "PlayerInventoryContainerModKeyName"),
            SelectColumn("PlayerInventoryContainer_ModKey_Type", "PlayerInventoryContainerModKeyType"),
            SelectColumn("PlayerInventoryContainer_ModKey_FileName", "PlayerInventoryContainerModKeyFileName"),
            SelectColumn("PlayerInventoryContainer_FormKey_ID", "PlayerInventoryContainerFormKeyId"),
            SelectColumn("JailOutfit_ModKey_Name", "JailOutfitModKeyName"),
            SelectColumn("JailOutfit_ModKey_Type", "JailOutfitModKeyType"),
            SelectColumn("JailOutfit_ModKey_FileName", "JailOutfitModKeyFileName"),
            SelectColumn("JailOutfit_FormKey_ID", "JailOutfitFormKeyId"),
            SelectColumn("CrimeValues_Arrest", "CrimeValuesArrest"),
            SelectColumn("CrimeValues_AttackOnSight", "CrimeValuesAttackOnSight"),
            SelectColumn("CrimeValues_Murder", "CrimeValuesMurder"),
            SelectColumn("CrimeValues_Assault", "CrimeValuesAssault"),
            SelectColumn("CrimeValues_Trespass", "CrimeValuesTrespass"),
            SelectColumn("CrimeValues_Pickpocket", "CrimeValuesPickpocket"),
            SelectColumn("CrimeValues_Steal", "CrimeValuesSteal"),
            SelectColumn("CrimeValues_StealMult", "CrimeValuesStealMult"),
            SelectColumn("CrimeValues_StealMultiplier", "CrimeValuesStealMultiplier"),
            SelectColumn("CrimeValues_Escape", "CrimeValuesEscape"),
            SelectColumn("CrimeValues_Werewolf", "CrimeValuesWerewolf"),
            SelectColumn("CrimeValues_WerewolfUnused", "CrimeValuesWerewolfUnused"),
            SelectColumn("CrimeValues_Unknown", "CrimeValuesUnknown"),
            SelectColumn("CrimeValues_Piracy", "CrimeValuesPiracy"),
            SelectColumn("CrimeValues_SmuggleMultiplier", "CrimeValuesSmuggleMultiplier"),
            SelectColumn("VendorValues_StartHour", "VendorValuesStartHour"),
            SelectColumn("VendorValues_EndHour", "VendorValuesEndHour"),
            SelectColumn("VendorValues_Radius", "VendorValuesRadius"),
            SelectColumn("VendorValues_BuysStolenItems", "VendorValuesBuysStolenItems"),
            SelectColumn("VendorValues_BuysNonStolenItems", "VendorValuesBuysNonStolenItems"),
            SelectColumn("VendorValues_BuySellEverythingNotInList", "VendorValuesBuySellEverythingNotInList"),
            SelectColumn("VendorLocation_MutagenObjectType", "VendorLocationMutagenObjectType"),
            SelectColumn("VendorLocation_Target_MutagenObjectType", "VendorLocationTargetMutagenObjectType"),
            SelectColumn("VendorLocation_Target_Type", "VendorLocationTargetType"),
            SelectColumn("VendorLocation_Target_Link_ModKey_Name", "VendorLocationTargetLinkModKeyName"),
            SelectColumn("VendorLocation_Target_Link_ModKey_Type", "VendorLocationTargetLinkModKeyType"),
            SelectColumn("VendorLocation_Target_Link_ModKey_FileName", "VendorLocationTargetLinkModKeyFileName"),
            SelectColumn("VendorLocation_Target_Link_FormKey_ID", "VendorLocationTargetLinkFormKeyId")
        ];
    }

    private IReadOnlyList<FactionRelationDTO> FetchRelationsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return FetchChildRows<FactionRelationRow>("FactionRelations", "Relation_Index", game, formKey)
            .Select(row => ToDTO(row, game))
            .ToList();
    }

    private IReadOnlyList<FactionRankDTO> FetchRanksByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return FetchChildRows<FactionRankRow>("FactionRanks", "Rank_Index", game, formKey)
            .Select(row => ToDTO(row, game))
            .ToList();
    }

    private IReadOnlyList<TRow> FetchChildRows<TRow>(string tableName, string orderBy, SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<TRow>(
            $"""
            SELECT *
            FROM {tableName}
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, {orderBy};
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

    private void ReplaceRelations(FactionDTO dto)
    {
        DeleteChildRows("FactionRelations", dto);
        foreach (var relation in dto.Relations)
        {
            relation.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO FactionRelations (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Relation_Index, Target_ModKey_Name, Target_ModKey_Type, Target_ModKey_FileName, Target_FormKey_ID, Reaction, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @RelationIndex, @TargetModKeyName, @TargetModKeyType, @TargetModKeyFileName, @TargetFormKeyId, @Reaction, @ImportedAtUTC);
                """,
                new
                {
                    Game = relation.Game.ToString(),
                    ModKeyName = relation.ModKey.Name,
                    ModKeyType = relation.ModKey.Type,
                    ModKeyFileName = relation.ModKey.FileName,
                    FormKeyModKeyName = relation.FormKey.ModKey.Name,
                    FormKeyModKeyType = relation.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = relation.FormKey.ModKey.FileName,
                    FormKeyId = relation.FormKey.Id,
                    relation.RelationIndex,
                    TargetModKeyName = relation.Target?.ModKey.Name,
                    TargetModKeyType = relation.Target?.ModKey.Type,
                    TargetModKeyFileName = relation.Target?.ModKey.FileName,
                    TargetFormKeyId = relation.Target?.Id,
                    relation.Reaction,
                    relation.ImportedAtUTC
                });
        }
    }

    private void ReplaceRanks(FactionDTO dto)
    {
        DeleteChildRows("FactionRanks", dto);
        foreach (var rank in dto.Ranks)
        {
            rank.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO FactionRanks (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Rank_Index, Number, Title_Male, Title_Female, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @RankIndex, @Number, @TitleMale, @TitleFemale, @ImportedAtUTC);
                """,
                new
                {
                    Game = rank.Game.ToString(),
                    ModKeyName = rank.ModKey.Name,
                    ModKeyType = rank.ModKey.Type,
                    ModKeyFileName = rank.ModKey.FileName,
                    FormKeyModKeyName = rank.FormKey.ModKey.Name,
                    FormKeyModKeyType = rank.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = rank.FormKey.ModKey.FileName,
                    FormKeyId = rank.FormKey.Id,
                    rank.RankIndex,
                    rank.Number,
                    TitleMale = GetEnglishText(rank.Title?.Male),
                    TitleFemale = GetEnglishText(rank.Title?.Female),
                    rank.ImportedAtUTC
                });
        }
    }

    private void DeleteChildRows(string tableName, FactionDTO dto)
    {
        Database.Execute(
            $"""
            DELETE FROM {tableName}
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            CommonParameters(dto));
    }

    private static object CreateFactionParameters(FactionDTO dto)
    {
        return new
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
            Name = GetEnglishText(dto.Name),
            dto.Flags,
            dto.FormationRadius,
            KeywordModKeyName = dto.Keyword?.ModKey.Name,
            KeywordModKeyType = dto.Keyword?.ModKey.Type,
            KeywordModKeyFileName = dto.Keyword?.ModKey.FileName,
            KeywordFormKeyId = dto.Keyword?.Id,
            HerdModKeyName = dto.Herd?.ModKey.Name,
            HerdModKeyType = dto.Herd?.ModKey.Type,
            HerdModKeyFileName = dto.Herd?.ModKey.FileName,
            HerdFormKeyId = dto.Herd?.Id,
            VoiceTypeModKeyName = dto.VoiceType?.ModKey.Name,
            VoiceTypeModKeyType = dto.VoiceType?.ModKey.Type,
            VoiceTypeModKeyFileName = dto.VoiceType?.ModKey.FileName,
            VoiceTypeFormKeyId = dto.VoiceType?.Id,
            SharedCrimeFactionListModKeyName = dto.SharedCrimeFactionList?.ModKey.Name,
            SharedCrimeFactionListModKeyType = dto.SharedCrimeFactionList?.ModKey.Type,
            SharedCrimeFactionListModKeyFileName = dto.SharedCrimeFactionList?.ModKey.FileName,
            SharedCrimeFactionListFormKeyId = dto.SharedCrimeFactionList?.Id,
            VendorBuySellListModKeyName = dto.VendorBuySellList?.ModKey.Name,
            VendorBuySellListModKeyType = dto.VendorBuySellList?.ModKey.Type,
            VendorBuySellListModKeyFileName = dto.VendorBuySellList?.ModKey.FileName,
            VendorBuySellListFormKeyId = dto.VendorBuySellList?.Id,
            MerchantContainerModKeyName = dto.MerchantContainer?.ModKey.Name,
            MerchantContainerModKeyType = dto.MerchantContainer?.ModKey.Type,
            MerchantContainerModKeyFileName = dto.MerchantContainer?.ModKey.FileName,
            MerchantContainerFormKeyId = dto.MerchantContainer?.Id,
            ExteriorJailMarkerModKeyName = dto.ExteriorJailMarker?.ModKey.Name,
            ExteriorJailMarkerModKeyType = dto.ExteriorJailMarker?.ModKey.Type,
            ExteriorJailMarkerModKeyFileName = dto.ExteriorJailMarker?.ModKey.FileName,
            ExteriorJailMarkerFormKeyId = dto.ExteriorJailMarker?.Id,
            FollowerWaitMarkerModKeyName = dto.FollowerWaitMarker?.ModKey.Name,
            FollowerWaitMarkerModKeyType = dto.FollowerWaitMarker?.ModKey.Type,
            FollowerWaitMarkerModKeyFileName = dto.FollowerWaitMarker?.ModKey.FileName,
            FollowerWaitMarkerFormKeyId = dto.FollowerWaitMarker?.Id,
            StolenGoodsContainerModKeyName = dto.StolenGoodsContainer?.ModKey.Name,
            StolenGoodsContainerModKeyType = dto.StolenGoodsContainer?.ModKey.Type,
            StolenGoodsContainerModKeyFileName = dto.StolenGoodsContainer?.ModKey.FileName,
            StolenGoodsContainerFormKeyId = dto.StolenGoodsContainer?.Id,
            PlayerInventoryContainerModKeyName = dto.PlayerInventoryContainer?.ModKey.Name,
            PlayerInventoryContainerModKeyType = dto.PlayerInventoryContainer?.ModKey.Type,
            PlayerInventoryContainerModKeyFileName = dto.PlayerInventoryContainer?.ModKey.FileName,
            PlayerInventoryContainerFormKeyId = dto.PlayerInventoryContainer?.Id,
            JailOutfitModKeyName = dto.JailOutfit?.ModKey.Name,
            JailOutfitModKeyType = dto.JailOutfit?.ModKey.Type,
            JailOutfitModKeyFileName = dto.JailOutfit?.ModKey.FileName,
            JailOutfitFormKeyId = dto.JailOutfit?.Id,
            CrimeValuesArrest = dto.CrimeValues?.Arrest,
            CrimeValuesAttackOnSight = dto.CrimeValues?.AttackOnSight,
            CrimeValuesMurder = dto.CrimeValues?.Murder,
            CrimeValuesAssault = dto.CrimeValues?.Assault,
            CrimeValuesTrespass = dto.CrimeValues?.Trespass,
            CrimeValuesPickpocket = dto.CrimeValues?.Pickpocket,
            CrimeValuesSteal = dto.CrimeValues?.Steal,
            CrimeValuesStealMult = dto.CrimeValues?.StealMult,
            CrimeValuesStealMultiplier = dto.CrimeValues?.StealMultiplier,
            CrimeValuesEscape = dto.CrimeValues?.Escape,
            CrimeValuesWerewolf = dto.CrimeValues?.Werewolf,
            CrimeValuesWerewolfUnused = dto.CrimeValues?.WerewolfUnused,
            CrimeValuesUnknown = dto.CrimeValues?.Unknown,
            CrimeValuesPiracy = dto.CrimeValues?.Piracy,
            CrimeValuesSmuggleMultiplier = dto.CrimeValues?.SmuggleMultiplier,
            VendorValuesStartHour = dto.VendorValues?.StartHour,
            VendorValuesEndHour = dto.VendorValues?.EndHour,
            VendorValuesRadius = dto.VendorValues?.Radius,
            VendorValuesBuysStolenItems = dto.VendorValues?.BuysStolenItems,
            VendorValuesBuysNonStolenItems = dto.VendorValues?.BuysNonStolenItems,
            VendorValuesBuySellEverythingNotInList = dto.VendorValues?.BuySellEverythingNotInList,
            VendorLocationMutagenObjectType = dto.VendorLocation?.MutagenObjectType,
            VendorLocationTargetMutagenObjectType = dto.VendorLocation?.Target?.MutagenObjectType,
            VendorLocationTargetType = dto.VendorLocation?.Target?.Type,
            VendorLocationTargetLinkModKeyName = dto.VendorLocation?.Target?.Link?.ModKey.Name,
            VendorLocationTargetLinkModKeyType = dto.VendorLocation?.Target?.Link?.ModKey.Type,
            VendorLocationTargetLinkModKeyFileName = dto.VendorLocation?.Target?.Link?.ModKey.FileName,
            VendorLocationTargetLinkFormKeyId = dto.VendorLocation?.Target?.Link?.Id
        };
    }

    private static FactionDTO ToDTO(FactionRow record, SupportedGame game)
    {
        var dto = new FactionDTO
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
            Name = FromEnglish(record.Name),
            Flags = record.Flags,
            FormationRadius = record.FormationRadius,
            Keyword = CreateNullableFormKey(record.KeywordModKeyName, record.KeywordModKeyType, record.KeywordModKeyFileName, record.KeywordFormKeyId),
            Herd = CreateNullableFormKey(record.HerdModKeyName, record.HerdModKeyType, record.HerdModKeyFileName, record.HerdFormKeyId),
            VoiceType = CreateNullableFormKey(record.VoiceTypeModKeyName, record.VoiceTypeModKeyType, record.VoiceTypeModKeyFileName, record.VoiceTypeFormKeyId),
            SharedCrimeFactionList = CreateNullableFormKey(record.SharedCrimeFactionListModKeyName, record.SharedCrimeFactionListModKeyType, record.SharedCrimeFactionListModKeyFileName, record.SharedCrimeFactionListFormKeyId),
            VendorBuySellList = CreateNullableFormKey(record.VendorBuySellListModKeyName, record.VendorBuySellListModKeyType, record.VendorBuySellListModKeyFileName, record.VendorBuySellListFormKeyId),
            MerchantContainer = CreateNullableFormKey(record.MerchantContainerModKeyName, record.MerchantContainerModKeyType, record.MerchantContainerModKeyFileName, record.MerchantContainerFormKeyId),
            ExteriorJailMarker = CreateNullableFormKey(record.ExteriorJailMarkerModKeyName, record.ExteriorJailMarkerModKeyType, record.ExteriorJailMarkerModKeyFileName, record.ExteriorJailMarkerFormKeyId),
            FollowerWaitMarker = CreateNullableFormKey(record.FollowerWaitMarkerModKeyName, record.FollowerWaitMarkerModKeyType, record.FollowerWaitMarkerModKeyFileName, record.FollowerWaitMarkerFormKeyId),
            StolenGoodsContainer = CreateNullableFormKey(record.StolenGoodsContainerModKeyName, record.StolenGoodsContainerModKeyType, record.StolenGoodsContainerModKeyFileName, record.StolenGoodsContainerFormKeyId),
            PlayerInventoryContainer = CreateNullableFormKey(record.PlayerInventoryContainerModKeyName, record.PlayerInventoryContainerModKeyType, record.PlayerInventoryContainerModKeyFileName, record.PlayerInventoryContainerFormKeyId),
            JailOutfit = CreateNullableFormKey(record.JailOutfitModKeyName, record.JailOutfitModKeyType, record.JailOutfitModKeyFileName, record.JailOutfitFormKeyId),
            CrimeValues = CreateCrimeValues(record),
            VendorValues = CreateVendorValues(record),
            VendorLocation = CreateVendorLocation(record)
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static FactionRelationDTO ToDTO(FactionRelationRow row, SupportedGame game)
    {
        return new FactionRelationDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            RelationIndex = row.RelationIndex,
            Target = CreateNullableFormKey(row.TargetModKeyName, row.TargetModKeyType, row.TargetModKeyFileName, row.TargetFormKeyId),
            Reaction = row.Reaction,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static FactionRankDTO ToDTO(FactionRankRow row, SupportedGame game)
    {
        return new FactionRankDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            RankIndex = row.RankIndex,
            Number = row.Number,
            Title = new FactionRankDTO.TitleDTO
            {
                Male = FromEnglish(row.TitleMale),
                Female = FromEnglish(row.TitleFemale)
            },
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static FactionDTO.CrimeValuesDTO? CreateCrimeValues(FactionRow record)
    {
        if (record.CrimeValuesArrest is null &&
            record.CrimeValuesAttackOnSight is null &&
            record.CrimeValuesMurder is null &&
            record.CrimeValuesAssault is null &&
            record.CrimeValuesTrespass is null &&
            record.CrimeValuesPickpocket is null &&
            record.CrimeValuesSteal is null &&
            record.CrimeValuesStealMult is null &&
            record.CrimeValuesStealMultiplier is null &&
            record.CrimeValuesEscape is null &&
            record.CrimeValuesWerewolf is null &&
            record.CrimeValuesWerewolfUnused is null &&
            record.CrimeValuesUnknown is null &&
            record.CrimeValuesPiracy is null &&
            record.CrimeValuesSmuggleMultiplier is null)
        {
            return null;
        }

        return new FactionDTO.CrimeValuesDTO
        {
            Arrest = ToBool(record.CrimeValuesArrest),
            AttackOnSight = ToBool(record.CrimeValuesAttackOnSight),
            Murder = record.CrimeValuesMurder,
            Assault = record.CrimeValuesAssault,
            Trespass = record.CrimeValuesTrespass,
            Pickpocket = record.CrimeValuesPickpocket,
            Steal = record.CrimeValuesSteal,
            StealMult = record.CrimeValuesStealMult,
            StealMultiplier = record.CrimeValuesStealMultiplier,
            Escape = record.CrimeValuesEscape,
            Werewolf = record.CrimeValuesWerewolf,
            WerewolfUnused = record.CrimeValuesWerewolfUnused,
            Unknown = record.CrimeValuesUnknown,
            Piracy = record.CrimeValuesPiracy,
            SmuggleMultiplier = record.CrimeValuesSmuggleMultiplier
        };
    }

    private static FactionDTO.VendorValuesDTO? CreateVendorValues(FactionRow record)
    {
        if (record.VendorValuesStartHour is null &&
            record.VendorValuesEndHour is null &&
            record.VendorValuesRadius is null &&
            record.VendorValuesBuysStolenItems is null &&
            record.VendorValuesBuysNonStolenItems is null &&
            record.VendorValuesBuySellEverythingNotInList is null)
        {
            return null;
        }

        return new FactionDTO.VendorValuesDTO
        {
            StartHour = record.VendorValuesStartHour,
            EndHour = record.VendorValuesEndHour,
            Radius = record.VendorValuesRadius,
            BuysStolenItems = ToBool(record.VendorValuesBuysStolenItems),
            BuysNonStolenItems = ToBool(record.VendorValuesBuysNonStolenItems),
            BuySellEverythingNotInList = ToBool(record.VendorValuesBuySellEverythingNotInList)
        };
    }

    private static FactionDTO.VendorLocationDTO? CreateVendorLocation(FactionRow record)
    {
        var targetLink = CreateNullableFormKey(
            record.VendorLocationTargetLinkModKeyName,
            record.VendorLocationTargetLinkModKeyType,
            record.VendorLocationTargetLinkModKeyFileName,
            record.VendorLocationTargetLinkFormKeyId);
        if (record.VendorLocationMutagenObjectType is null &&
            record.VendorLocationTargetMutagenObjectType is null &&
            record.VendorLocationTargetType is null &&
            targetLink is null)
        {
            return null;
        }

        return new FactionDTO.VendorLocationDTO
        {
            MutagenObjectType = record.VendorLocationMutagenObjectType,
            Target = record.VendorLocationTargetMutagenObjectType is null && record.VendorLocationTargetType is null && targetLink is null
                ? null
                : new FactionDTO.VendorLocationTargetDTO
                {
                    MutagenObjectType = record.VendorLocationTargetMutagenObjectType,
                    Type = record.VendorLocationTargetType,
                    Link = targetLink
                }
        };
    }

    private static bool? ToBool(int? value)
    {
        return value.HasValue ? value.Value != 0 : null;
    }

    private static ModKeyDTO CreateModKey(string name, int type, string fileName)
    {
        return new ModKeyDTO { Name = name, Type = type, FileName = fileName };
    }

    private static FormKeyDTO CreateFormKey(string modKeyName, int modKeyType, string modKeyFileName, long formKeyId)
    {
        return new FormKeyDTO { ModKey = CreateModKey(modKeyName, modKeyType, modKeyFileName), Id = (uint)formKeyId };
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return first.Type == second.Type &&
            string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class FactionRow : RecordRow
    {
        public int? Version2 { get; set; }
        public int? VersionControl { get; set; }
        public string? Name { get; set; }
        public string? Flags { get; set; }
        public double? FormationRadius { get; set; }
        public string? KeywordModKeyName { get; set; }
        public int? KeywordModKeyType { get; set; }
        public string? KeywordModKeyFileName { get; set; }
        public long? KeywordFormKeyId { get; set; }
        public string? HerdModKeyName { get; set; }
        public int? HerdModKeyType { get; set; }
        public string? HerdModKeyFileName { get; set; }
        public long? HerdFormKeyId { get; set; }
        public string? VoiceTypeModKeyName { get; set; }
        public int? VoiceTypeModKeyType { get; set; }
        public string? VoiceTypeModKeyFileName { get; set; }
        public long? VoiceTypeFormKeyId { get; set; }
        public string? SharedCrimeFactionListModKeyName { get; set; }
        public int? SharedCrimeFactionListModKeyType { get; set; }
        public string? SharedCrimeFactionListModKeyFileName { get; set; }
        public long? SharedCrimeFactionListFormKeyId { get; set; }
        public string? VendorBuySellListModKeyName { get; set; }
        public int? VendorBuySellListModKeyType { get; set; }
        public string? VendorBuySellListModKeyFileName { get; set; }
        public long? VendorBuySellListFormKeyId { get; set; }
        public string? MerchantContainerModKeyName { get; set; }
        public int? MerchantContainerModKeyType { get; set; }
        public string? MerchantContainerModKeyFileName { get; set; }
        public long? MerchantContainerFormKeyId { get; set; }
        public string? ExteriorJailMarkerModKeyName { get; set; }
        public int? ExteriorJailMarkerModKeyType { get; set; }
        public string? ExteriorJailMarkerModKeyFileName { get; set; }
        public long? ExteriorJailMarkerFormKeyId { get; set; }
        public string? FollowerWaitMarkerModKeyName { get; set; }
        public int? FollowerWaitMarkerModKeyType { get; set; }
        public string? FollowerWaitMarkerModKeyFileName { get; set; }
        public long? FollowerWaitMarkerFormKeyId { get; set; }
        public string? StolenGoodsContainerModKeyName { get; set; }
        public int? StolenGoodsContainerModKeyType { get; set; }
        public string? StolenGoodsContainerModKeyFileName { get; set; }
        public long? StolenGoodsContainerFormKeyId { get; set; }
        public string? PlayerInventoryContainerModKeyName { get; set; }
        public int? PlayerInventoryContainerModKeyType { get; set; }
        public string? PlayerInventoryContainerModKeyFileName { get; set; }
        public long? PlayerInventoryContainerFormKeyId { get; set; }
        public string? JailOutfitModKeyName { get; set; }
        public int? JailOutfitModKeyType { get; set; }
        public string? JailOutfitModKeyFileName { get; set; }
        public long? JailOutfitFormKeyId { get; set; }
        public int? CrimeValuesArrest { get; set; }
        public int? CrimeValuesAttackOnSight { get; set; }
        public int? CrimeValuesMurder { get; set; }
        public int? CrimeValuesAssault { get; set; }
        public int? CrimeValuesTrespass { get; set; }
        public int? CrimeValuesPickpocket { get; set; }
        public int? CrimeValuesSteal { get; set; }
        public double? CrimeValuesStealMult { get; set; }
        public double? CrimeValuesStealMultiplier { get; set; }
        public int? CrimeValuesEscape { get; set; }
        public int? CrimeValuesWerewolf { get; set; }
        public int? CrimeValuesWerewolfUnused { get; set; }
        public int? CrimeValuesUnknown { get; set; }
        public int? CrimeValuesPiracy { get; set; }
        public double? CrimeValuesSmuggleMultiplier { get; set; }
        public double? VendorValuesStartHour { get; set; }
        public double? VendorValuesEndHour { get; set; }
        public int? VendorValuesRadius { get; set; }
        public int? VendorValuesBuysStolenItems { get; set; }
        public int? VendorValuesBuysNonStolenItems { get; set; }
        public int? VendorValuesBuySellEverythingNotInList { get; set; }
        public string? VendorLocationMutagenObjectType { get; set; }
        public string? VendorLocationTargetMutagenObjectType { get; set; }
        public string? VendorLocationTargetType { get; set; }
        public string? VendorLocationTargetLinkModKeyName { get; set; }
        public int? VendorLocationTargetLinkModKeyType { get; set; }
        public string? VendorLocationTargetLinkModKeyFileName { get; set; }
        public long? VendorLocationTargetLinkFormKeyId { get; set; }
    }

    private sealed class FactionRelationRow
    {
        public string ModKeyName { get; set; } = string.Empty;
        public int ModKeyType { get; set; }
        public string ModKeyFileName { get; set; } = string.Empty;
        public string FormKeyModKeyName { get; set; } = string.Empty;
        public int FormKeyModKeyType { get; set; }
        public string FormKeyModKeyFileName { get; set; } = string.Empty;
        public long FormKeyId { get; set; }
        public int RelationIndex { get; set; }
        public string? TargetModKeyName { get; set; }
        public int? TargetModKeyType { get; set; }
        public string? TargetModKeyFileName { get; set; }
        public long? TargetFormKeyId { get; set; }
        public string? Reaction { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class FactionRankRow
    {
        public string ModKeyName { get; set; } = string.Empty;
        public int ModKeyType { get; set; }
        public string ModKeyFileName { get; set; } = string.Empty;
        public string FormKeyModKeyName { get; set; } = string.Empty;
        public int FormKeyModKeyType { get; set; }
        public string FormKeyModKeyFileName { get; set; } = string.Empty;
        public long FormKeyId { get; set; }
        public int RankIndex { get; set; }
        public int? Number { get; set; }
        public string? TitleMale { get; set; }
        public string? TitleFemale { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }

}
