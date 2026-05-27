using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IFormListRepository
{
    /// <summary>
    /// Saves a FormListDTO to the database.
    /// </summary>
    /// <param name="dto">The DTO with the FormList data to be saved.</param>
    void Save(FormListDTO dto);
}
