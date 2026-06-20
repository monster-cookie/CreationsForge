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
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, Name, Flags, FormationRadius,
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
                CrimeArrest, CrimeAttackOnSight, CrimeMurder, CrimeAssault, CrimeTrespass, CrimePickpocket, CrimeSteal, CrimeStealMult,
                CrimeEscape, CrimeWerewolf, CrimeUnknown, VendorStartHour, VendorEndHour, VendorRadius, VendorBuysStolenItems, VendorBuysNonStolenItems,
                VendorBuySellEverythingNotInList, VendorLocationMutagenObjectType, VendorLocationType, VendorLocationLink_ModKey_Name,
                VendorLocationLink_ModKey_Type, VendorLocationLink_ModKey_FileName, VendorLocationLink_FormKey_ID)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @Name, @Flags, @FormationRadius,
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
                @CrimeArrest, @CrimeAttackOnSight, @CrimeMurder, @CrimeAssault, @CrimeTrespass, @CrimePickpocket, @CrimeSteal, @CrimeStealMult,
                @CrimeEscape, @CrimeWerewolf, @CrimeUnknown, @VendorStartHour, @VendorEndHour, @VendorRadius, @VendorBuysStolenItems, @VendorBuysNonStolenItems,
                @VendorBuySellEverythingNotInList, @VendorLocationMutagenObjectType, @VendorLocationType, @VendorLocationLinkModKeyName,
                @VendorLocationLinkModKeyType, @VendorLocationLinkModKeyFileName, @VendorLocationLinkFormKeyId);
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
            SelectColumn("CrimeArrest"),
            SelectColumn("CrimeAttackOnSight"),
            SelectColumn("CrimeMurder"),
            SelectColumn("CrimeAssault"),
            SelectColumn("CrimeTrespass"),
            SelectColumn("CrimePickpocket"),
            SelectColumn("CrimeSteal"),
            SelectColumn("CrimeStealMult"),
            SelectColumn("CrimeEscape"),
            SelectColumn("CrimeWerewolf"),
            SelectColumn("CrimeUnknown"),
            SelectColumn("VendorStartHour"),
            SelectColumn("VendorEndHour"),
            SelectColumn("VendorRadius"),
            SelectColumn("VendorBuysStolenItems"),
            SelectColumn("VendorBuysNonStolenItems"),
            SelectColumn("VendorBuySellEverythingNotInList"),
            SelectColumn("VendorLocationMutagenObjectType"),
            SelectColumn("VendorLocationType"),
            SelectColumn("VendorLocationLink_ModKey_Name", "VendorLocationLinkModKeyName"),
            SelectColumn("VendorLocationLink_ModKey_Type", "VendorLocationLinkModKeyType"),
            SelectColumn("VendorLocationLink_ModKey_FileName", "VendorLocationLinkModKeyFileName"),
            SelectColumn("VendorLocationLink_FormKey_ID", "VendorLocationLinkFormKeyId")
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
                    TargetModKeyName = relation.TargetFormKey?.ModKey.Name,
                    TargetModKeyType = relation.TargetFormKey?.ModKey.Type,
                    TargetModKeyFileName = relation.TargetFormKey?.ModKey.FileName,
                    TargetFormKeyId = relation.TargetFormKey?.Id,
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
                    Rank_Index, RankNumber, MaleTitle, FemaleTitle, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @RankIndex, @RankNumber, @MaleTitle, @FemaleTitle, @ImportedAtUTC);
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
                    rank.RankNumber,
                    MaleTitle = GetEnglishText(rank.MaleTitle),
                    FemaleTitle = GetEnglishText(rank.FemaleTitle),
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
            Name = GetEnglishText(dto.Name),
            dto.Flags,
            dto.FormationRadius,
            KeywordModKeyName = dto.KeywordFormKey?.ModKey.Name,
            KeywordModKeyType = dto.KeywordFormKey?.ModKey.Type,
            KeywordModKeyFileName = dto.KeywordFormKey?.ModKey.FileName,
            KeywordFormKeyId = dto.KeywordFormKey?.Id,
            HerdModKeyName = dto.HerdFormKey?.ModKey.Name,
            HerdModKeyType = dto.HerdFormKey?.ModKey.Type,
            HerdModKeyFileName = dto.HerdFormKey?.ModKey.FileName,
            HerdFormKeyId = dto.HerdFormKey?.Id,
            VoiceTypeModKeyName = dto.VoiceTypeFormKey?.ModKey.Name,
            VoiceTypeModKeyType = dto.VoiceTypeFormKey?.ModKey.Type,
            VoiceTypeModKeyFileName = dto.VoiceTypeFormKey?.ModKey.FileName,
            VoiceTypeFormKeyId = dto.VoiceTypeFormKey?.Id,
            SharedCrimeFactionListModKeyName = dto.SharedCrimeFactionListFormKey?.ModKey.Name,
            SharedCrimeFactionListModKeyType = dto.SharedCrimeFactionListFormKey?.ModKey.Type,
            SharedCrimeFactionListModKeyFileName = dto.SharedCrimeFactionListFormKey?.ModKey.FileName,
            SharedCrimeFactionListFormKeyId = dto.SharedCrimeFactionListFormKey?.Id,
            VendorBuySellListModKeyName = dto.VendorBuySellListFormKey?.ModKey.Name,
            VendorBuySellListModKeyType = dto.VendorBuySellListFormKey?.ModKey.Type,
            VendorBuySellListModKeyFileName = dto.VendorBuySellListFormKey?.ModKey.FileName,
            VendorBuySellListFormKeyId = dto.VendorBuySellListFormKey?.Id,
            MerchantContainerModKeyName = dto.MerchantContainerFormKey?.ModKey.Name,
            MerchantContainerModKeyType = dto.MerchantContainerFormKey?.ModKey.Type,
            MerchantContainerModKeyFileName = dto.MerchantContainerFormKey?.ModKey.FileName,
            MerchantContainerFormKeyId = dto.MerchantContainerFormKey?.Id,
            ExteriorJailMarkerModKeyName = dto.ExteriorJailMarkerFormKey?.ModKey.Name,
            ExteriorJailMarkerModKeyType = dto.ExteriorJailMarkerFormKey?.ModKey.Type,
            ExteriorJailMarkerModKeyFileName = dto.ExteriorJailMarkerFormKey?.ModKey.FileName,
            ExteriorJailMarkerFormKeyId = dto.ExteriorJailMarkerFormKey?.Id,
            FollowerWaitMarkerModKeyName = dto.FollowerWaitMarkerFormKey?.ModKey.Name,
            FollowerWaitMarkerModKeyType = dto.FollowerWaitMarkerFormKey?.ModKey.Type,
            FollowerWaitMarkerModKeyFileName = dto.FollowerWaitMarkerFormKey?.ModKey.FileName,
            FollowerWaitMarkerFormKeyId = dto.FollowerWaitMarkerFormKey?.Id,
            StolenGoodsContainerModKeyName = dto.StolenGoodsContainerFormKey?.ModKey.Name,
            StolenGoodsContainerModKeyType = dto.StolenGoodsContainerFormKey?.ModKey.Type,
            StolenGoodsContainerModKeyFileName = dto.StolenGoodsContainerFormKey?.ModKey.FileName,
            StolenGoodsContainerFormKeyId = dto.StolenGoodsContainerFormKey?.Id,
            PlayerInventoryContainerModKeyName = dto.PlayerInventoryContainerFormKey?.ModKey.Name,
            PlayerInventoryContainerModKeyType = dto.PlayerInventoryContainerFormKey?.ModKey.Type,
            PlayerInventoryContainerModKeyFileName = dto.PlayerInventoryContainerFormKey?.ModKey.FileName,
            PlayerInventoryContainerFormKeyId = dto.PlayerInventoryContainerFormKey?.Id,
            JailOutfitModKeyName = dto.JailOutfitFormKey?.ModKey.Name,
            JailOutfitModKeyType = dto.JailOutfitFormKey?.ModKey.Type,
            JailOutfitModKeyFileName = dto.JailOutfitFormKey?.ModKey.FileName,
            JailOutfitFormKeyId = dto.JailOutfitFormKey?.Id,
            dto.CrimeArrest,
            dto.CrimeAttackOnSight,
            dto.CrimeMurder,
            dto.CrimeAssault,
            dto.CrimeTrespass,
            dto.CrimePickpocket,
            dto.CrimeSteal,
            dto.CrimeStealMult,
            dto.CrimeEscape,
            dto.CrimeWerewolf,
            dto.CrimeUnknown,
            dto.VendorStartHour,
            dto.VendorEndHour,
            dto.VendorRadius,
            dto.VendorBuysStolenItems,
            dto.VendorBuysNonStolenItems,
            dto.VendorBuySellEverythingNotInList,
            dto.VendorLocationMutagenObjectType,
            dto.VendorLocationType,
            VendorLocationLinkModKeyName = dto.VendorLocationLinkFormKey?.ModKey.Name,
            VendorLocationLinkModKeyType = dto.VendorLocationLinkFormKey?.ModKey.Type,
            VendorLocationLinkModKeyFileName = dto.VendorLocationLinkFormKey?.ModKey.FileName,
            VendorLocationLinkFormKeyId = dto.VendorLocationLinkFormKey?.Id
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
            Name = FromEnglish(record.Name),
            Flags = record.Flags,
            FormationRadius = record.FormationRadius,
            KeywordFormKey = CreateNullableFormKey(record.KeywordModKeyName, record.KeywordModKeyType, record.KeywordModKeyFileName, record.KeywordFormKeyId),
            HerdFormKey = CreateNullableFormKey(record.HerdModKeyName, record.HerdModKeyType, record.HerdModKeyFileName, record.HerdFormKeyId),
            VoiceTypeFormKey = CreateNullableFormKey(record.VoiceTypeModKeyName, record.VoiceTypeModKeyType, record.VoiceTypeModKeyFileName, record.VoiceTypeFormKeyId),
            SharedCrimeFactionListFormKey = CreateNullableFormKey(record.SharedCrimeFactionListModKeyName, record.SharedCrimeFactionListModKeyType, record.SharedCrimeFactionListModKeyFileName, record.SharedCrimeFactionListFormKeyId),
            VendorBuySellListFormKey = CreateNullableFormKey(record.VendorBuySellListModKeyName, record.VendorBuySellListModKeyType, record.VendorBuySellListModKeyFileName, record.VendorBuySellListFormKeyId),
            MerchantContainerFormKey = CreateNullableFormKey(record.MerchantContainerModKeyName, record.MerchantContainerModKeyType, record.MerchantContainerModKeyFileName, record.MerchantContainerFormKeyId),
            ExteriorJailMarkerFormKey = CreateNullableFormKey(record.ExteriorJailMarkerModKeyName, record.ExteriorJailMarkerModKeyType, record.ExteriorJailMarkerModKeyFileName, record.ExteriorJailMarkerFormKeyId),
            FollowerWaitMarkerFormKey = CreateNullableFormKey(record.FollowerWaitMarkerModKeyName, record.FollowerWaitMarkerModKeyType, record.FollowerWaitMarkerModKeyFileName, record.FollowerWaitMarkerFormKeyId),
            StolenGoodsContainerFormKey = CreateNullableFormKey(record.StolenGoodsContainerModKeyName, record.StolenGoodsContainerModKeyType, record.StolenGoodsContainerModKeyFileName, record.StolenGoodsContainerFormKeyId),
            PlayerInventoryContainerFormKey = CreateNullableFormKey(record.PlayerInventoryContainerModKeyName, record.PlayerInventoryContainerModKeyType, record.PlayerInventoryContainerModKeyFileName, record.PlayerInventoryContainerFormKeyId),
            JailOutfitFormKey = CreateNullableFormKey(record.JailOutfitModKeyName, record.JailOutfitModKeyType, record.JailOutfitModKeyFileName, record.JailOutfitFormKeyId),
            CrimeArrest = ToBool(record.CrimeArrest),
            CrimeAttackOnSight = ToBool(record.CrimeAttackOnSight),
            CrimeMurder = record.CrimeMurder,
            CrimeAssault = record.CrimeAssault,
            CrimeTrespass = record.CrimeTrespass,
            CrimePickpocket = record.CrimePickpocket,
            CrimeSteal = record.CrimeSteal,
            CrimeStealMult = record.CrimeStealMult,
            CrimeEscape = record.CrimeEscape,
            CrimeWerewolf = record.CrimeWerewolf,
            CrimeUnknown = record.CrimeUnknown,
            VendorStartHour = record.VendorStartHour,
            VendorEndHour = record.VendorEndHour,
            VendorRadius = record.VendorRadius,
            VendorBuysStolenItems = ToBool(record.VendorBuysStolenItems),
            VendorBuysNonStolenItems = ToBool(record.VendorBuysNonStolenItems),
            VendorBuySellEverythingNotInList = ToBool(record.VendorBuySellEverythingNotInList),
            VendorLocationMutagenObjectType = record.VendorLocationMutagenObjectType,
            VendorLocationType = record.VendorLocationType,
            VendorLocationLinkFormKey = CreateNullableFormKey(record.VendorLocationLinkModKeyName, record.VendorLocationLinkModKeyType, record.VendorLocationLinkModKeyFileName, record.VendorLocationLinkFormKeyId)
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
            TargetFormKey = CreateNullableFormKey(row.TargetModKeyName, row.TargetModKeyType, row.TargetModKeyFileName, row.TargetFormKeyId),
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
            RankNumber = row.RankNumber,
            MaleTitle = FromEnglish(row.MaleTitle),
            FemaleTitle = FromEnglish(row.FemaleTitle),
            ImportedAtUTC = row.ImportedAtUTC
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
        public int? CrimeArrest { get; set; }
        public int? CrimeAttackOnSight { get; set; }
        public int? CrimeMurder { get; set; }
        public int? CrimeAssault { get; set; }
        public int? CrimeTrespass { get; set; }
        public int? CrimePickpocket { get; set; }
        public int? CrimeSteal { get; set; }
        public double? CrimeStealMult { get; set; }
        public int? CrimeEscape { get; set; }
        public int? CrimeWerewolf { get; set; }
        public int? CrimeUnknown { get; set; }
        public double? VendorStartHour { get; set; }
        public double? VendorEndHour { get; set; }
        public int? VendorRadius { get; set; }
        public int? VendorBuysStolenItems { get; set; }
        public int? VendorBuysNonStolenItems { get; set; }
        public int? VendorBuySellEverythingNotInList { get; set; }
        public string? VendorLocationMutagenObjectType { get; set; }
        public string? VendorLocationType { get; set; }
        public string? VendorLocationLinkModKeyName { get; set; }
        public int? VendorLocationLinkModKeyType { get; set; }
        public string? VendorLocationLinkModKeyFileName { get; set; }
        public long? VendorLocationLinkFormKeyId { get; set; }
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
        public int? RankNumber { get; set; }
        public string? MaleTitle { get; set; }
        public string? FemaleTitle { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }

}
