using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IStarfieldRecordReaderService
{
    IReadOnlyList<FormListDTO> GetFormLists(PluginDTO plugin);
    IReadOnlyList<GameSettingDTO> GetGameSettings(PluginDTO plugin);
}
