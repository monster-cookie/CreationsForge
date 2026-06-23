using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.DTOs.Records;

public class MiscItemDestructibleStageDTO
{
    public int StageIndex { get; set; }

    public int? Index { get; set; }

    public int? HealthPercent { get; set; }

    public int? ModelDamageStage { get; set; }

    public string? Flags { get; set; }

    public int? SelfDamagePerSecond { get; set; }

    public FormKeyDTO? Explosion { get; set; }

    public MiscItemDestructibleStageModelDTO? Model { get; set; }
}
