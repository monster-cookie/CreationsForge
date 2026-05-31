using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class KeywordRepository : RecordHeaderRepository<Keyword, KeywordDTO>, IKeywordRepository
{
    public KeywordRepository(IDatabase database)
        : base(database, RecordTypeCatalog.Keyword.TableName)
    { }

    protected override KeywordDTO CreateDTO(Keyword model) => new(model);
    protected override Keyword CreateModel(KeywordDTO dto) => new(dto);
}
