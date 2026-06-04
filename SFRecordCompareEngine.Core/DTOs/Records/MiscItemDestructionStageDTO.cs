using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class MiscItemDestructionStageDTO
{
    public required int StageIndex { get; set; }
    public byte? HealthPercent { get; set; }
    public byte? Index { get; set; }
    public byte? ModelDamageStage { get; set; }
    public string? Flags { get; set; }
    public int? SelfDamagePerSecond { get; set; }
    public FormKey? ExplosionFormKey { get; set; }
    public FormKey? DebrisFormKey { get; set; }
    public int? DebrisCount { get; set; }
    public string? SequenceName { get; set; }
    public string? ModelFile { get; set; }
    public uint? ModelLightLayer { get; set; }
    public string? ModelFlags { get; set; }
    public IList<FormKey> ModelMaterialSwaps { get; set; } = new List<FormKey>();
}
