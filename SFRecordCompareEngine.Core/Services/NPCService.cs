using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class NPCService : INPCService
{
    private readonly INPCRepository Repository;

    public NPCService(INPCRepository repository)
    {
        Repository = repository;
    }

    public IList<NPCDTO> GetByModKey(ModKey modKey)
    {
        return Repository.GetByModKey(modKey);
    }

    public IList<NPCDTO> GetByFormKeyID(uint formKeyID)
    {
        return Repository.GetByFormKeyID(formKeyID);
    }
}