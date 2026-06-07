namespace CreationsForge.Core.DTOs.Results;

public class RecordImportResultDTO
{
    public IList<RecordTypeImportResultDTO> RecordTypes { get; set; } = new List<RecordTypeImportResultDTO>();

    public int HeadersImported => RecordTypes.Sum(recordType => recordType.HeadersImported);

    public int DetailRowsImported => RecordTypes.Sum(recordType => recordType.DetailRowsImported);

    public int RecordsFailed => RecordTypes.Sum(recordType => recordType.RecordsFailed);

    public int UnsupportedRecordTypes => RecordTypes.Count(recordType => !recordType.HeaderImportSupported || !recordType.TypedDetailImportSupported);

    public int FormListsImported => RecordTypes.Where(recordType => recordType.RecordType == "FLST").Sum(recordType => recordType.DetailRowsImported);

    public int FormListItemsImported => RecordTypes.Sum(recordType => recordType.FormListItemsImported);

    public int GameSettingsImported => RecordTypes.Where(recordType => recordType.RecordType == "GMST").Sum(recordType => recordType.DetailRowsImported);

    public int GlobalsImported => RecordTypes.Where(recordType => recordType.RecordType == "GLOB").Sum(recordType => recordType.DetailRowsImported);

    public void Add(RecordImportResultDTO result)
    {
        foreach (var recordType in result.RecordTypes)
        {
            RecordTypes.Add(recordType);
        }
    }
}
