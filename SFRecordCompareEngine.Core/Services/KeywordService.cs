using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class KeywordService : IKeywordService
{
    private readonly IKeywordRepository KeywordRepository;

    public KeywordService(IKeywordRepository repository)
    {
        KeywordRepository = repository;
    }

    public IList<KeywordDTO> GetByModKey(ModKey modKey) => KeywordRepository.GetByModKey(modKey);
    public IList<KeywordDTO> GetByFormKeyID(uint formKeyID) => KeywordRepository.GetByFormKeyID(formKeyID);
}
