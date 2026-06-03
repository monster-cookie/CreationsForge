using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IFormListRepository
{
    /// <summary>
    ///     Gets form list records owned by the given plugin.
    /// </summary>
    /// <param name="modKey">The owning plugin key.</param>
    /// <returns>The matching form list records or an empty list if none are found.</returns>
    IList<FormListDTO> GetByModKey(ModKey modKey);

    IList<RecordTreeEntryDTO> GetRecordTreeEntriesByModKey(ModKey modKey);

    /// <summary>
    ///     Gets form list records with the given form key.
    /// </summary>
    /// <param name="formKey">The origin form key to search for.</param>
    /// <returns>The matching form list records in plugin load order or an empty list if none are found.</returns>
    IList<FormListDTO> GetByFormKey(FormKey formKey);

    /// <summary>
    ///     Saves a FormListDTO to the database.
    /// </summary>
    /// <param name="dto">The DTO with the FormList data to be saved.</param>
    void Save(FormListDTO dto);
}
