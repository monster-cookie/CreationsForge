using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class ModelImportService : IModelImportService
{
    private readonly IModelMaterialSwapRepository ModelMaterialSwapRepository;
    private readonly IModelRepository ModelRepository;

    public ModelImportService(
        IModelRepository modelRepository,
        IModelMaterialSwapRepository modelMaterialSwapRepository)
    {
        ModelRepository = modelRepository;
        ModelMaterialSwapRepository = modelMaterialSwapRepository;
    }

    public void ReplaceRecordModels(IHasModelsDTO record, string recordType)
    {
        if (record is not RecordDTO recordDTO)
        {
            throw new ArgumentException($"Expected {nameof(RecordDTO)}.", nameof(record));
        }

        ModelRepository.DeleteByRecord(recordDTO.Game, recordDTO.ModKey, recordType, recordDTO.FormKey);

        foreach (var model in record.Models)
        {
            model.Game = recordDTO.Game;
            model.ModKey = recordDTO.ModKey;
            model.RecordType = recordType;
            model.FormKey = recordDTO.FormKey;
            model.ImportedAtUTC = recordDTO.ImportedAtUTC;
            ModelRepository.Save(model);

            foreach (var materialSwap in model.MaterialSwaps)
            {
                materialSwap.Game = recordDTO.Game;
                materialSwap.ModKey = recordDTO.ModKey;
                materialSwap.RecordType = recordType;
                materialSwap.FormKey = recordDTO.FormKey;
                materialSwap.ModelSlot = model.ModelSlot;
                materialSwap.ModelGender = model.ModelGender;
                materialSwap.ImportedAtUTC = recordDTO.ImportedAtUTC;
                ModelMaterialSwapRepository.Save(materialSwap);
            }
        }
    }
}
