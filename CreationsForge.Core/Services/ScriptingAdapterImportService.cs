using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class ScriptingAdapterImportService : IScriptingAdapterImportService
{
    private readonly IScriptingAdapterPropertyListItemRepository ScriptingAdapterPropertyListItemRepository;
    private readonly IScriptingAdapterPropertyRepository ScriptingAdapterPropertyRepository;
    private readonly IScriptingAdapterPropertyStructMemberRepository ScriptingAdapterPropertyStructMemberRepository;
    private readonly IScriptingAdapterPropertyStructRepository ScriptingAdapterPropertyStructRepository;
    private readonly IScriptingAdapterRepository ScriptingAdapterRepository;
    private readonly IScriptFragmentRepository ScriptFragmentRepository;

    public ScriptingAdapterImportService(
        IScriptingAdapterRepository scriptingAdapterRepository,
        IScriptingAdapterPropertyRepository scriptingAdapterPropertyRepository,
        IScriptingAdapterPropertyListItemRepository scriptingAdapterPropertyListItemRepository,
        IScriptingAdapterPropertyStructRepository scriptingAdapterPropertyStructRepository,
        IScriptingAdapterPropertyStructMemberRepository scriptingAdapterPropertyStructMemberRepository,
        IScriptFragmentRepository scriptFragmentRepository)
    {
        ScriptingAdapterRepository = scriptingAdapterRepository;
        ScriptingAdapterPropertyRepository = scriptingAdapterPropertyRepository;
        ScriptingAdapterPropertyListItemRepository = scriptingAdapterPropertyListItemRepository;
        ScriptingAdapterPropertyStructRepository = scriptingAdapterPropertyStructRepository;
        ScriptingAdapterPropertyStructMemberRepository = scriptingAdapterPropertyStructMemberRepository;
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

                foreach (var propertyStruct in property.Structs)
                {
                    propertyStruct.Game = recordDTO.Game;
                    propertyStruct.ModKey = recordDTO.ModKey;
                    propertyStruct.RecordType = recordType;
                    propertyStruct.FormKey = recordDTO.FormKey;
                    propertyStruct.ScriptingAdapterName = scriptingAdapter.Name;
                    propertyStruct.PropertyIndex = property.PropertyIndex;
                    propertyStruct.ImportedAtUTC = recordDTO.ImportedAtUTC;
                    ScriptingAdapterPropertyStructRepository.Save(propertyStruct);

                    foreach (var member in propertyStruct.Members)
                    {
                        member.Game = recordDTO.Game;
                        member.ModKey = recordDTO.ModKey;
                        member.RecordType = recordType;
                        member.FormKey = recordDTO.FormKey;
                        member.ScriptingAdapterName = scriptingAdapter.Name;
                        member.PropertyIndex = property.PropertyIndex;
                        member.StructIndex = propertyStruct.StructIndex;
                        member.ImportedAtUTC = recordDTO.ImportedAtUTC;
                        ScriptingAdapterPropertyStructMemberRepository.Save(member);
                    }
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
