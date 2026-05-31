using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IStarfieldRecordReaderService
{
    IReadOnlyList<FormListDTO> GetFormLists(PluginDTO plugin);
    IReadOnlyList<GameSettingDTO> GetGameSettings(PluginDTO plugin);
    IReadOnlyList<GlobalDTO> GetGlobals(PluginDTO plugin);
    IReadOnlyList<MiscObjectDTO> GetMiscObjects(PluginDTO plugin);
    IReadOnlyList<KeywordDTO> GetKeywords(PluginDTO plugin);
    IReadOnlyList<NPCDTO> GetNPCs(PluginDTO plugin);
    IReadOnlyList<ActorValueInformationDTO> GetActorValueInformation(PluginDTO plugin);
    IReadOnlyList<MagicEffectDTO> GetMagicEffects(PluginDTO plugin);
    IReadOnlyList<PerkDTO> GetPerks(PluginDTO plugin);
}
