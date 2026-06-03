using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class PerkRepository : IPerkRepository
{
    private readonly IDatabase Database;

    public PerkRepository(IDatabase database)
    {
        Database = database;
    }

    public IList<PerkDTO> GetByModKey(ModKey modKey)
    {
        return HydrateChildren(Database.Fetch<Perk>("SELECT * FROM Perk WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName }).Select(x => new PerkDTO(x)).ToList());
    }

    public IList<RecordTreeEntryDTO> GetRecordTreeEntriesByModKey(ModKey modKey)
    {
        return Database.Fetch<Perk>("SELECT FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, EditorID FROM Perk WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName })
            .Select(x => new RecordTreeEntryDTO
            {
                FormKey = new FormKey(new ModKey(x.FormKeyModKeyName, (ModType)x.FormKeyModKeyType), (uint)x.FormKeyId),
                EditorID = x.EditorId
            })
            .ToList();
    }

    public IList<PerkDTO> GetByFormKey(FormKey formKey)
    {
        return HydrateChildren(Database
            .Fetch<Perk>(
                "SELECT Perk.* FROM Perk INNER JOIN Plugins ON Plugins.ModKey_Name = Perk.ModKey_Name AND Plugins.ModKey_Type = Perk.ModKey_Type AND Plugins.ModKey_FileName = Perk.ModKey_FileName WHERE Perk.FormKey_ModKey_Name = @FormKeyModKeyName AND Perk.FormKey_ModKey_Type = @FormKeyModKeyType AND Perk.FormKey_ModKey_FileName = @FormKeyModKeyFileName AND Perk.FormKey_ID = @FormKeyID AND Plugins.Enabled = 1 AND Plugins.ExistsOnDisk = 1 AND Plugins.ImportState = @ImportState ORDER BY Plugins.LoadOrderIndex;",
                new { FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = (int)formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyID = formKey.ID, ImportState = nameof(PluginImportState.Current) }).Select(x => new PerkDTO(x)).ToList());
    }

    public void Save(PerkDTO dto)
    {
        Database.Save(new Perk(dto));
        DeleteChildren(dto.ModKey, dto.FormKey);
        foreach (var rank in dto.Ranks)
        {
            rank.ModKey = dto.ModKey;
            rank.FormKey = dto.FormKey;
            rank.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Save(new PerkRank(rank));
            foreach (var effect in rank.Effects)
            {
                effect.ModKey = dto.ModKey;
                effect.FormKey = dto.FormKey;
                effect.RankIndex = rank.RankIndex;
                effect.ImportedAtUTC = dto.ImportedAtUTC;
                Database.Save(new PerkRankEffect(effect));
            }
        }

        foreach (var backgroundSkill in dto.BackgroundSkills)
        {
            backgroundSkill.ModKey = dto.ModKey;
            backgroundSkill.FormKey = dto.FormKey;
            backgroundSkill.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Save(new PerkBackgroundSkill(backgroundSkill));
        }
    }

    private IList<PerkDTO> HydrateChildren(IList<PerkDTO> records)
    {
        foreach (var record in records)
        {
            var ranks = GetRanks(record.ModKey, record.FormKey);
            var effects = GetRankEffects(record.ModKey, record.FormKey);
            foreach (var rank in ranks)
            {
                rank.Effects = effects
                    .Where(effect => effect.RankIndex == rank.RankIndex)
                    .OrderBy(effect => effect.EffectIndex)
                    .ToList();
            }

            record.Ranks = ranks;
            record.BackgroundSkills = GetBackgroundSkills(record.ModKey, record.FormKey);
        }

        return records;
    }

    private IList<PerkRankDTO> GetRanks(ModKey modKey, FormKey formKey)
    {
        return Database.Fetch<PerkRank>(
                """
                SELECT *
                FROM PerkRanks
                WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                  AND FormKey_ModKey_Name = @FormKeyModKeyName
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
                  AND FormKey_ID = @FormKeyID
                ORDER BY Rank_Index;
                """,
                new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName, FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = (int)formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyID = formKey.ID })
            .Select(model => new PerkRankDTO(model))
            .ToList();
    }

    private IList<PerkRankEffectDTO> GetRankEffects(ModKey modKey, FormKey formKey)
    {
        return Database.Fetch<PerkRankEffect>(
                """
                SELECT *
                FROM PerkRankEffects
                WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                  AND FormKey_ModKey_Name = @FormKeyModKeyName
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
                  AND FormKey_ID = @FormKeyID
                ORDER BY Rank_Index, Effect_Index;
                """,
                new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName, FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = (int)formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyID = formKey.ID })
            .Select(model => new PerkRankEffectDTO(model))
            .ToList();
    }

    private IList<PerkBackgroundSkillDTO> GetBackgroundSkills(ModKey modKey, FormKey formKey)
    {
        return Database.Fetch<PerkBackgroundSkill>(
                """
                SELECT *
                FROM PerkBackgroundSkills
                WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                  AND FormKey_ModKey_Name = @FormKeyModKeyName
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
                  AND FormKey_ID = @FormKeyID
                ORDER BY Skill_Index;
                """,
                new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName, FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = (int)formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyID = formKey.ID })
            .Select(model => new PerkBackgroundSkillDTO(model))
            .ToList();
    }

    private void DeleteChildren(ModKey modKey, FormKey formKey)
    {
        var parameters = new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName, FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = (int)formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyID = formKey.ID };
        Database.Delete<PerkRank>(
            """
            WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyID
            """,
            parameters);
        Database.Delete<PerkBackgroundSkill>(
            """
            WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyID
            """,
            parameters);
    }
}