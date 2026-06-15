namespace CreationsForge.Core.DTOs.Records;

public class PluginRecordSetDTO
{
    public IReadOnlyList<FormListDTO> FormLists { get; set; } = [];

    public IReadOnlyList<GameSettingDTO> GameSettings { get; set; } = [];

    public IReadOnlyList<GlobalDTO> Globals { get; set; } = [];

    public IReadOnlyList<MiscObjectDTO> MiscObjects { get; set; } = [];

    public IReadOnlyList<KeywordDTO> Keywords { get; set; } = [];

    public IReadOnlyList<ActorValueInformationDTO> ActorValueInformation { get; set; } = [];

    public IReadOnlyList<NPCDTO> NPCs { get; set; } = [];

    public IReadOnlyList<MagicEffectDTO> MagicEffects { get; set; } = [];

    public IReadOnlyList<PerkDTO> Perks { get; set; } = [];

    public IReadOnlyList<StaticDTO> Statics { get; set; } = [];

    public IReadOnlyList<BookDTO> Books { get; set; } = [];

    public IReadOnlyList<DoorDTO> Doors { get; set; } = [];

    public IReadOnlyList<ContainerDTO> Containers { get; set; } = [];

    public IReadOnlyList<ConstructibleObjectDTO> ConstructibleObjects { get; set; } = [];

    public IReadOnlyList<ConditionFormDTO> ConditionForms { get; set; } = [];

    public IReadOnlyList<TerminalDTO> Terminals { get; set; } = [];
}
