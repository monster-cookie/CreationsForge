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
            VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15, @16, @17, @18)
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
            faction.ModKey,
            faction.FormID,
            DbValue(faction.Name),
            DbValue(faction.KeywordFormKey),
            DbValue(faction.Flags),
            DbValue(faction.CrimeValuesArrest),
            DbValue(faction.CrimeValuesMurder),
            DbValue(faction.CrimeValuesAssault),
            DbValue(faction.CrimeValuesTrespass),
            DbValue(faction.CrimeValuesPickpocket),
            DbValue(faction.CrimeValuesStealMultiplier),
            DbValue(faction.CrimeValuesEscape),
            DbValue(faction.CrimeValuesPiracy),
            DbValue(faction.CrimeValuesSmuggleMultiplier),
            DbValue(faction.VendorValuesStartHour),
            DbValue(faction.VendorValuesEndHour),
            DbValue(faction.VendorValuesBuysStolenItems),
            DbValue(faction.VendorValuesBuysNonStolenItems),
            faction.ImportedAtUtc);
    }

    public void ReplaceRelations(IDatabase database, string modKey, string formId, IList<FactionRelationDTO> relations)
    {
        database.Execute(
            "DELETE FROM FactionRelation WHERE ModKey = @0 COLLATE NOCASE AND FormID = @1;",
            modKey,
            formId);

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
                VALUES (@0, @1, @2, @3, @4, @5);
                """,
                relation.ModKey,
                relation.FormID,
                relation.ItemIndex,
                relation.TargetFormKey,
                DbValue(relation.Reaction),
                relation.ImportedAtUtc);
        }
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
