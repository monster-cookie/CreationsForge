using Mutagen.Bethesda.Plugins; using SFRecordCompareEngine.Core.DTOs.Records; using SFRecordCompareEngine.Core.Repositories.Interfaces; using SFRecordCompareEngine.Core.Services.Interfaces;
namespace SFRecordCompareEngine.Core.Services;
public class PerkService : IPerkService { private readonly IPerkRepository Repository; public PerkService(IPerkRepository repository) { Repository = repository; } public IList<PerkDTO> GetByModKey(ModKey modKey) => Repository.GetByModKey(modKey); public IList<PerkDTO> GetByFormKeyID(uint formKeyID) => Repository.GetByFormKeyID(formKeyID); }
