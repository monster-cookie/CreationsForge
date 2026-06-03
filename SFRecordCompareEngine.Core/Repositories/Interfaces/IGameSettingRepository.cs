using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IGameSettingRepository
{
    /// <summary>
    ///     Gets game setting records owned by the given plugin.
    /// </summary>
    /// <param name="modKey">The owning plugin key.</param>
    /// <returns>The matching game setting records or an empty list if none are found.</returns>
    IList<GameSettingDTO> GetByModKey(ModKey modKey);

    IList<RecordTreeEntryDTO> GetRecordTreeEntriesByModKey(ModKey modKey);

    /// <summary>
    ///     Gets game setting records with the given form key.
    /// </summary>
    /// <param name="formKey">The origin form key to search for.</param>
    /// <returns>The matching game setting records in plugin load order or an empty list if none are found.</returns>
    IList<GameSettingDTO> GetByFormKey(FormKey formKey);

    /// <summary>
    ///     Saves a game setting record.
    /// </summary>
    void Save(GameSettingDTO dto);
}
