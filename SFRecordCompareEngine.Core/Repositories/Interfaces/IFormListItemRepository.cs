using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IFormListItemRepository
{
    /// <summary>
    /// Gets ordered item rows for the given form list.
    /// </summary>
    /// <param name="modKey">The plugin containing the form list.</param>
    /// <param name="formKey">The form list key.</param>
    /// <returns>The form list items in source order or an empty list if none are found.</returns>
    IList<FormListItemDTO> GetByFormList(ModKey modKey, FormKey formKey);

    /// <summary>
    /// Deletes item rows for the given form list.
    /// </summary>
    /// <param name="modKey">The plugin containing the form list.</param>
    /// <param name="formKey">The form list key.</param>
    void DeleteByFormList(ModKey modKey, FormKey formKey);

    /// <summary>
    /// Saves a FormListItemDTO to the database.
    /// </summary>
    /// <param name="formListItemDTO">The form list item data to be saved</param>
    void Save(FormListItemDTO formListItemDTO);
}