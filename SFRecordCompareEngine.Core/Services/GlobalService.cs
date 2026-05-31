using Mutagen.Bethesda.Plugins; using SFRecordCompareEngine.Core.DTOs.Records; using SFRecordCompareEngine.Core.Repositories.Interfaces; using SFRecordCompareEngine.Core.Services.Interfaces;
namespace SFRecordCompareEngine.Core.Services;
public class GlobalService : IGlobalService { private readonly IGlobalRepository Repository; public GlobalService(IGlobalRepository repository) { Repository = repository; } public IList<GlobalDTO> GetByModKey(ModKey modKey) => Repository.GetByModKey(modKey); public IList<GlobalDTO> GetByFormKeyID(uint formKeyID) => Repository.GetByFormKeyID(formKeyID); }
