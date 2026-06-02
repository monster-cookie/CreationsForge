using SFRecordCompareEngine.Core.DTOs.Records.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class ScriptingAdapterImportService : IScriptingAdapterImportService
{
    private readonly IScriptingAdapterPropertyListItemRepository ScriptingAdapterPropertyListItemRepository;
    private readonly IScriptingAdapterPropertyRepository ScriptingAdapterPropertyRepository;
    private readonly IScriptingAdapterRepository ScriptingAdapterRepository;

    public ScriptingAdapterImportService(
        IScriptingAdapterRepository scriptingAdapterRepository,
        IScriptingAdapterPropertyRepository scriptingAdapterPropertyRepository,
        IScriptingAdapterPropertyListItemRepository scriptingAdapterPropertyListItemRepository)
    {
        ScriptingAdapterRepository = scriptingAdapterRepository;
        ScriptingAdapterPropertyRepository = scriptingAdapterPropertyRepository;
        ScriptingAdapterPropertyListItemRepository = scriptingAdapterPropertyListItemRepository;
    }

    public void ReplaceRecordScriptingAdapters(IHasScriptingAdaptersRecordDTO record, string recordType)
    {
        ScriptingAdapterRepository.DeleteByRecord(record.ModKey, recordType, record.FormKey);

        foreach (var scriptingAdapter in record.ScriptingAdapters)
        {
            scriptingAdapter.ModKey = record.ModKey;
            scriptingAdapter.RecordType = recordType;
            scriptingAdapter.FormKey = record.FormKey;
            ScriptingAdapterRepository.Save(scriptingAdapter);

            foreach (var property in scriptingAdapter.Properties)
            {
                property.ModKey = record.ModKey;
                property.RecordType = recordType;
                property.FormKey = record.FormKey;
                property.ScriptingAdapterName = scriptingAdapter.Name;
                ScriptingAdapterPropertyRepository.Save(property);

                foreach (var listItem in property.ListItems)
                {
                    listItem.ModKey = record.ModKey;
                    listItem.RecordType = recordType;
                    listItem.FormKey = record.FormKey;
                    listItem.ScriptingAdapterName = scriptingAdapter.Name;
                    listItem.PropertyIndex = property.PropertyIndex;
                    ScriptingAdapterPropertyListItemRepository.Save(listItem);
                }
            }
        }
    }
}
