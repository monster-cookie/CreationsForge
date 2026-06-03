using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class PerkRankEffectDTO
{
    public PerkRankEffectDTO()
    { }

    [SetsRequiredMembers]
    public PerkRankEffectDTO(PerkRankEffect model)
    {
        ModKey = new ModKey(model.ModKeyName, (ModType)model.ModKeyType);
        var formKeyModKey = new ModKey(model.FormKeyModKeyName, (ModType)model.FormKeyModKeyType);
        FormKey = new FormKey(formKeyModKey, (uint)model.FormKeyId);
        RankIndex = model.RankIndex;
        EffectIndex = model.EffectIndex;
        MutagenObjectType = model.MutagenObjectType;
        Rank = model.Rank;
        Priority = model.Priority;
        PerkEntryId = model.PerkEntryId;
        Flags = model.Flags;
        ButtonLabel = model.ButtonLabel;
        ConditionCount = model.ConditionCount;
        EntryPoint = model.EntryPoint;
        PerkConditionTabCount = model.PerkConditionTabCount;
        Modification = model.Modification;
        Value = model.Value;
        ImportedAtUTC = model.ImportedAtUTC;
    }

    public required ModKey ModKey { get; set; }
    public required FormKey FormKey { get; set; }
    public required int RankIndex { get; set; }
    public required int EffectIndex { get; set; }
    public required string MutagenObjectType { get; set; }
    public required int Rank { get; set; }
    public required int Priority { get; set; }
    public int? PerkEntryId { get; set; }
    public string? Flags { get; set; }
    public string? ButtonLabel { get; set; }
    public required int ConditionCount { get; set; }
    public string? EntryPoint { get; set; }
    public int? PerkConditionTabCount { get; set; }
    public string? Modification { get; set; }
    public float? Value { get; set; }
    public required DateTime ImportedAtUTC { get; set; }
}

