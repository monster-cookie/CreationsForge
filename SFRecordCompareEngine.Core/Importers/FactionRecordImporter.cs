using System.Collections;
using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;

namespace SFRecordCompareEngine.Core.Importers;

public class FactionRecordImporter(IFactionRepository factionRepository) : ITypedRecordDetailImporter
{
    public string RecordType => "Faction";
    public string TableName => "Faction";

    public void Import(IDatabase database, ModKey modKey, string formId, RecordEnumerationDTO record, string importedAtUtc)
    {
        factionRepository.Upsert(database, new FactionDTO
        {
            ModKey = modKey,
            FormID = formId,
            Name = RecordDetailValueMapper.GetTextValue(record.Record, "Name"),
            KeywordFormKey = RecordDetailValueMapper.ExtractReferenceText(RecordHeaderMapper.GetPropertyValue(record.Record, "Keyword")),
            Flags = RecordDetailValueMapper.GetCollectionTextValue(record.Record, "Flags"),
            CrimeValuesArrest = RecordDetailValueMapper.GetNestedBooleanIntValue(record.Record, "CrimeValues", "Arrest"),
            CrimeValuesMurder = RecordDetailValueMapper.GetNestedIntValue(record.Record, "CrimeValues", "Murder"),
            CrimeValuesAssault = RecordDetailValueMapper.GetNestedIntValue(record.Record, "CrimeValues", "Assault"),
            CrimeValuesTrespass = RecordDetailValueMapper.GetNestedIntValue(record.Record, "CrimeValues", "Trespass"),
            CrimeValuesPickpocket = RecordDetailValueMapper.GetNestedIntValue(record.Record, "CrimeValues", "Pickpocket"),
            CrimeValuesStealMultiplier = RecordDetailValueMapper.GetNestedDoubleValue(record.Record, "CrimeValues", "StealMultiplier"),
            CrimeValuesEscape = RecordDetailValueMapper.GetNestedIntValue(record.Record, "CrimeValues", "Escape"),
            CrimeValuesPiracy = RecordDetailValueMapper.GetNestedIntValue(record.Record, "CrimeValues", "Piracy"),
            CrimeValuesSmuggleMultiplier = RecordDetailValueMapper.GetNestedDoubleValue(record.Record, "CrimeValues", "SmuggleMultiplier"),
            VendorValuesStartHour = RecordDetailValueMapper.GetNestedIntValue(record.Record, "VendorValues", "StartHour"),
            VendorValuesEndHour = RecordDetailValueMapper.GetNestedIntValue(record.Record, "VendorValues", "EndHour"),
            VendorValuesBuysStolenItems = RecordDetailValueMapper.GetNestedBooleanIntValue(record.Record, "VendorValues", "BuysStolenItems"),
            VendorValuesBuysNonStolenItems = RecordDetailValueMapper.GetNestedBooleanIntValue(record.Record, "VendorValues", "BuysNonStolenItems"),
            ImportedAtUtc = importedAtUtc
        });

        factionRepository.ReplaceRelations(database, modKey, formId, GetRelations(record.Record, modKey, formId, importedAtUtc));
    }

    private static IList<FactionRelationDTO> GetRelations(object source, ModKey modKey, string formId, string importedAtUtc)
    {
        if (RecordHeaderMapper.GetPropertyValue(source, "Relations") is not IEnumerable relations) return [];

        return relations.Cast<object?>()
            .Where(relation => relation is not null)
            .Select(relation => new
            {
                Relation = relation!,
                TargetFormKey = RecordDetailValueMapper.ExtractReferenceText(RecordHeaderMapper.GetPropertyValue(relation!, "Target"))
            })
            .Where(relation => !string.IsNullOrWhiteSpace(relation.TargetFormKey))
            .Select((relation, index) => new FactionRelationDTO
            {
                ModKey = modKey,
                FormID = formId,
                ItemIndex = index,
                TargetFormKey = relation.TargetFormKey!,
                Reaction = RecordDetailValueMapper.GetTextValue(relation.Relation, "Reaction"),
                ImportedAtUtc = importedAtUtc
            })
            .ToList();
    }
}
