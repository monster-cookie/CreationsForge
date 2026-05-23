namespace SFRecordCompareEngine.Core.DTOs.Records;

public class RecordTypeImportResultDTO
{
    public required string RecordType { get; set; }
    public bool HeaderImportSupported { get; set; }
    public bool TypedDetailImportSupported { get; set; }
    public string? DetailTableName { get; set; }
    public string? UnsupportedReason { get; set; }
    public int HeadersImported { get; set; }
    public int DetailRowsImported { get; set; }
    public int FormListItemsImported { get; set; }
    public int RecordsFailed { get; set; }
}
