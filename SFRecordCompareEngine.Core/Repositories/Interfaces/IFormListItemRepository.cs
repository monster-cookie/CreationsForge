using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IFormListItemRepository
{
    /// <summary>
    /// Saves a FormListItemDTO to the database.
    /// </summary>
    /// <param name="formListItemDTO">The form list item data to be saved</param>
    void Save(FormListItemDTO formListItemDTO);
}