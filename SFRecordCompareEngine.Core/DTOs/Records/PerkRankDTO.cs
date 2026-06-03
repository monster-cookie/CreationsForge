using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class PerkRankDTO
{
    public PerkRankDTO()
    { }

    [SetsRequiredMembers]
    public PerkRankDTO(PerkRank model)
    {
        ModKey = new ModKey(model.ModKeyName, (ModType)model.ModKeyType);
        var formKeyModKey = new ModKey(model.FormKeyModKeyName, (ModType)model.FormKeyModKeyType);
        FormKey = new FormKey(formKeyModKey, (uint)model.FormKeyId);
        RankIndex = model.RankIndex;
        Description = model.Description;
        UnknownStaticFormKey = CreateNullableFormKey(model.UnknownStaticModKeyName, model.UnknownStaticModKeyType, model.UnknownStaticFormKeyId);
        ConditionCount = model.ConditionCount;
        ActivityCount = model.ActivityCount;
        ImportedAtUTC = model.ImportedAtUTC;
    }

    public required ModKey ModKey { get; set; }
    public required FormKey FormKey { get; set; }
    public required int RankIndex { get; set; }
    public string? Description { get; set; }
    public FormKey? UnknownStaticFormKey { get; set; }
    public required int ConditionCount { get; set; }
    public required int ActivityCount { get; set; }
    public required DateTime ImportedAtUTC { get; set; }
    public IList<PerkRankEffectDTO> Effects { get; set; } = new List<PerkRankEffectDTO>();

    private static FormKey? CreateNullableFormKey(string? modKeyName, int? modKeyType, int? formKeyId)
    {
        return modKeyName == null || !modKeyType.HasValue || !formKeyId.HasValue
            ? null
            : new FormKey(new ModKey(modKeyName, (ModType)modKeyType.Value), (uint)formKeyId.Value);
    }
}
