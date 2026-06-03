using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class PerkBackgroundSkillDTO
{
    public PerkBackgroundSkillDTO()
    { }

    [SetsRequiredMembers]
    public PerkBackgroundSkillDTO(PerkBackgroundSkill model)
    {
        ModKey = new ModKey(model.ModKeyName, (ModType)model.ModKeyType);
        var formKeyModKey = new ModKey(model.FormKeyModKeyName, (ModType)model.FormKeyModKeyType);
        FormKey = new FormKey(formKeyModKey, (uint)model.FormKeyId);
        var skillModKey = new ModKey(model.SkillModKeyName, (ModType)model.SkillModKeyType);
        SkillFormKey = new FormKey(skillModKey, (uint)model.SkillFormKeyId);
        SkillIndex = model.SkillIndex;
        ImportedAtUTC = model.ImportedAtUTC;
    }

    public required ModKey ModKey { get; set; }
    public required FormKey FormKey { get; set; }
    public required FormKey SkillFormKey { get; set; }
    public required int SkillIndex { get; set; }
    public required DateTime ImportedAtUTC { get; set; }
}

