using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class ScriptingAdapterImportService : IScriptingAdapterImportService
{
    private readonly IScriptingAdapterPropertyListItemRepository ScriptingAdapterPropertyListItemRepository;
    private readonly IScriptingAdapterPropertyRepository ScriptingAdapterPropertyRepository;
    private readonly IScriptingAdapterRepository ScriptingAdapterRepository;
    private readonly IScriptFragmentRepository ScriptFragmentRepository;

    public ScriptingAdapterImportService(
        IScriptingAdapterRepository scriptingAdapterRepository,
        IScriptingAdapterPropertyRepository scriptingAdapterPropertyRepository,
        IScriptingAdapterPropertyListItemRepository scriptingAdapterPropertyListItemRepository,
        IScriptFragmentRepository scriptFragmentRepository)
    {
        ScriptingAdapterRepository = scriptingAdapterRepository;
        ScriptingAdapterPropertyRepository = scriptingAdapterPropertyRepository;
        ScriptingAdapterPropertyListItemRepository = scriptingAdapterPropertyListItemRepository;
        ScriptFragmentRepository = scriptFragmentRepository;
    }

    public void ReplaceRecordScriptingAdapters(IHasScriptingAdaptersDTO record, string recordType)
    {
        if (record is not RecordDTO recordDTO)
        {
            throw new ArgumentException($"Expected {nameof(RecordDTO)}.", nameof(record));
        }

        ScriptingAdapterRepository.DeleteByRecord(recordDTO.Game, recordDTO.ModKey, recordType, recordDTO.FormKey);

        foreach (var scriptingAdapter in record.ScriptingAdapters)
        {
            scriptingAdapter.Game = recordDTO.Game;
            scriptingAdapter.ModKey = recordDTO.ModKey;
            scriptingAdapter.RecordType = recordType;
            scriptingAdapter.FormKey = recordDTO.FormKey;
            scriptingAdapter.ImportedAtUTC = recordDTO.ImportedAtUTC;
            ScriptingAdapterRepository.Save(scriptingAdapter);

            foreach (var property in scriptingAdapter.Properties)
            {
                property.Game = recordDTO.Game;
                property.ModKey = recordDTO.ModKey;
                property.RecordType = recordType;
                property.FormKey = recordDTO.FormKey;
                property.ScriptingAdapterName = scriptingAdapter.Name;
                property.ImportedAtUTC = recordDTO.ImportedAtUTC;
                ScriptingAdapterPropertyRepository.Save(property);

                foreach (var listItem in property.ListItems)
                {
                    listItem.Game = recordDTO.Game;
                    listItem.ModKey = recordDTO.ModKey;
                    listItem.RecordType = recordType;
                    listItem.FormKey = recordDTO.FormKey;
                    listItem.ScriptingAdapterName = scriptingAdapter.Name;
                    listItem.PropertyIndex = property.PropertyIndex;
                    listItem.ImportedAtUTC = recordDTO.ImportedAtUTC;
                    ScriptingAdapterPropertyListItemRepository.Save(listItem);
                }
            }
        }

        if (record is not IHasScriptFragmentsDTO fragmentRecord)
        {
            return;
        }

        ScriptFragmentRepository.DeleteByRecord(recordDTO.Game, recordDTO.ModKey, recordType, recordDTO.FormKey);

        foreach (var scriptFragment in fragmentRecord.ScriptFragments)
        {
            scriptFragment.Game = recordDTO.Game;
            scriptFragment.ModKey = recordDTO.ModKey;
            scriptFragment.RecordType = recordType;
            scriptFragment.FormKey = recordDTO.FormKey;
            scriptFragment.ImportedAtUTC = recordDTO.ImportedAtUTC;
            ScriptFragmentRepository.Save(scriptFragment);
        }
    }
}
