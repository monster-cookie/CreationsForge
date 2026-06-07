using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;
using ModelMaterialSwapDatabase = CreationsForge.Core.Models.Database.ModelMaterialSwap;

namespace CreationsForge.Core.Repositories;

public class ModelMaterialSwapRepository : IModelMaterialSwapRepository
{
    private readonly IDatabase Database;

    public ModelMaterialSwapRepository(IDatabase database)
    {
        Database = database;
    }

    public void Save(ModelMaterialSwapDTO dto)
    {
        Database.Save(new ModelMaterialSwapDatabase(dto));
    }
}
