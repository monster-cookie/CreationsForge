using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class FactionRepository : IFactionRepository
{
    public void Upsert(IDatabase database, FactionDTO faction)
    {
        database.Execute(
            """
            INSERT INTO Faction (
                ModKey,
                FormID,
                Name,
                KeywordFormKey,
                Flags,
                CrimeValuesArrest,
                CrimeValuesMurder,
                CrimeValuesAssault,
                CrimeValuesTrespass,
                CrimeValuesPickpocket,
                CrimeValuesStealMultiplier,
                CrimeValuesEscape,
                CrimeValuesPiracy,
                CrimeValuesSmuggleMultiplier,
                VendorValuesStartHour,
                VendorValuesEndHour,
                VendorValuesBuysStolenItems,
                VendorValuesBuysNonStolenItems,
                ImportedAtUtc
            )
            VALUES (@ModKey, @FormID, @Name, @KeywordFormKey, @Flags, @CrimeValuesArrest, @CrimeValuesMurder, @CrimeValuesAssault, @CrimeValuesTrespass, @CrimeValuesPickpocket, @CrimeValuesStealMultiplier, @CrimeValuesEscape, @CrimeValuesPiracy, @CrimeValuesSmuggleMultiplier, @VendorValuesStartHour, @VendorValuesEndHour, @VendorValuesBuysStolenItems, @VendorValuesBuysNonStolenItems, @ImportedAtUtc)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                KeywordFormKey = excluded.KeywordFormKey,
                Flags = excluded.Flags,
                CrimeValuesArrest = excluded.CrimeValuesArrest,
                CrimeValuesMurder = excluded.CrimeValuesMurder,
                CrimeValuesAssault = excluded.CrimeValuesAssault,
                CrimeValuesTrespass = excluded.CrimeValuesTrespass,
                CrimeValuesPickpocket = excluded.CrimeValuesPickpocket,
                CrimeValuesStealMultiplier = excluded.CrimeValuesStealMultiplier,
                CrimeValuesEscape = excluded.CrimeValuesEscape,
                CrimeValuesPiracy = excluded.CrimeValuesPiracy,
                CrimeValuesSmuggleMultiplier = excluded.CrimeValuesSmuggleMultiplier,
                VendorValuesStartHour = excluded.VendorValuesStartHour,
                VendorValuesEndHour = excluded.VendorValuesEndHour,
                VendorValuesBuysStolenItems = excluded.VendorValuesBuysStolenItems,
                VendorValuesBuysNonStolenItems = excluded.VendorValuesBuysNonStolenItems,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            new
            {
                ModKey = faction.ModKey.FileName,
                faction.FormID,
                Name = DbValue(faction.Name),
                KeywordFormKey = DbValue(faction.KeywordFormKey),
                Flags = DbValue(faction.Flags),
                CrimeValuesArrest = DbValue(faction.CrimeValuesArrest),
                CrimeValuesMurder = DbValue(faction.CrimeValuesMurder),
                CrimeValuesAssault = DbValue(faction.CrimeValuesAssault),
                CrimeValuesTrespass = DbValue(faction.CrimeValuesTrespass),
                CrimeValuesPickpocket = DbValue(faction.CrimeValuesPickpocket),
                CrimeValuesStealMultiplier = DbValue(faction.CrimeValuesStealMultiplier),
                CrimeValuesEscape = DbValue(faction.CrimeValuesEscape),
                CrimeValuesPiracy = DbValue(faction.CrimeValuesPiracy),
                CrimeValuesSmuggleMultiplier = DbValue(faction.CrimeValuesSmuggleMultiplier),
                VendorValuesStartHour = DbValue(faction.VendorValuesStartHour),
                VendorValuesEndHour = DbValue(faction.VendorValuesEndHour),
                VendorValuesBuysStolenItems = DbValue(faction.VendorValuesBuysStolenItems),
                VendorValuesBuysNonStolenItems = DbValue(faction.VendorValuesBuysNonStolenItems),
                faction.ImportedAtUtc
            });
    }

    public void ReplaceRelations(IDatabase database, ModKey modKey, string formId, IList<FactionRelationDTO> relations)
    {
        database.Execute(
            "DELETE FROM FactionRelation WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;",
            new { ModKey = modKey.FileName, FormId = formId });

        foreach (var relation in relations)
        {
            database.Execute(
                """
                INSERT INTO FactionRelation (
                    ModKey,
                    FormID,
                    ItemIndex,
                    TargetFormKey,
                    Reaction,
                    ImportedAtUtc
                )
                VALUES (@ModKey, @FormID, @ItemIndex, @TargetFormKey, @Reaction, @ImportedAtUtc);
                """,
                new
                {
                    ModKey = relation.ModKey.FileName,
                    relation.FormID,
                    relation.ItemIndex,
                    relation.TargetFormKey,
                    Reaction = DbValue(relation.Reaction),
                    relation.ImportedAtUtc
                });
        }
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
