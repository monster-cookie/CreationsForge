using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Records.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class ScriptingAdapterHydrationService : IScriptingAdapterHydrationService
{
    private readonly IScriptingAdapterPropertyListItemRepository ScriptingAdapterPropertyListItemRepository;
    private readonly IScriptingAdapterPropertyRepository ScriptingAdapterPropertyRepository;
    private readonly IScriptingAdapterRepository ScriptingAdapterRepository;

    public ScriptingAdapterHydrationService(
        IScriptingAdapterRepository scriptingAdapterRepository,
        IScriptingAdapterPropertyRepository scriptingAdapterPropertyRepository,
        IScriptingAdapterPropertyListItemRepository scriptingAdapterPropertyListItemRepository)
    {
        ScriptingAdapterRepository = scriptingAdapterRepository;
        ScriptingAdapterPropertyRepository = scriptingAdapterPropertyRepository;
        ScriptingAdapterPropertyListItemRepository = scriptingAdapterPropertyListItemRepository;
    }

    public IList<TRecord> Hydrate<TRecord>(IList<TRecord> records, string recordType) where TRecord : IHasScriptingAdaptersRecordDTO
    {
        foreach (var record in records)
        {
            var scriptingAdapters = ScriptingAdapterRepository.GetByRecord(record.ModKey, recordType, record.FormKey);
            var properties = ScriptingAdapterPropertyRepository.GetByRecord(record.ModKey, recordType, record.FormKey);
            var listItems = ScriptingAdapterPropertyListItemRepository.GetByRecord(record.ModKey, recordType, record.FormKey);

            foreach (var scriptingAdapter in scriptingAdapters)
            {
                scriptingAdapter.Properties = properties
                    .Where(property => string.Equals(property.ScriptingAdapterName, scriptingAdapter.Name, StringComparison.Ordinal))
                    .OrderBy(property => property.PropertyIndex)
                    .ToList();

                foreach (var property in scriptingAdapter.Properties)
                {
                    property.ListItems = listItems
                        .Where(listItem => string.Equals(listItem.ScriptingAdapterName, property.ScriptingAdapterName, StringComparison.Ordinal)
                                           && listItem.PropertyIndex == property.PropertyIndex)
                        .OrderBy(listItem => listItem.ListItemIndex)
                        .ToList();
                }
            }

            record.ScriptingAdapters = scriptingAdapters;
        }

        return records;
    }
}
