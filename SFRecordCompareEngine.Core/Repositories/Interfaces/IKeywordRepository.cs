using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
namespace SFRecordCompareEngine.Core.Repositories.Interfaces;
public interface IKeywordRepository { IList<KeywordDTO> GetByModKey(ModKey modKey); IList<KeywordDTO> GetByFormKeyID(uint formKeyID); void Save(KeywordDTO dto); }
