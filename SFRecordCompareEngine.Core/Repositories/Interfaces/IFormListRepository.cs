using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IFormListRepository
{
    /// <summary>
    /// Gets form list records owned by the given plugin.
    /// </summary>
    /// <param name="modKey">The owning plugin key.</param>
    /// <returns>The matching form list records or an empty list if none are found.</returns>
    IList<FormListDTO> GetByModKey(ModKey modKey);

    /// <summary>
    /// Saves a FormListDTO to the database.
    /// </summary>
    /// <param name="dto">The DTO with the FormList data to be saved.</param>
    void Save(FormListDTO dto);
}
