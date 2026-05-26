using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class RecordImportResultDTO
{
    public RecordImportResultDTO()
    {}

    public RecordImportResultDTO(ModKey modKey)
    {
        ModKey = modKey;
    }

    public required ModKey ModKey { get; set; }

    public IList<RecordTypeImportResultDTO> RecordTypes { get; set; } = new List<RecordTypeImportResultDTO>();

    public int HeadersImported => RecordTypes.Sum(recordType => recordType.HeadersImported);
    
    public int DetailRowsImported => RecordTypes.Sum(recordType => recordType.DetailRowsImported);
    
    public int FormListItemsImported => RecordTypes.Sum(recordType => recordType.FormListItemsImported);
    
    public int RecordsFailed => RecordTypes.Sum(recordType => recordType.RecordsFailed);
    
    public int UnsupportedRecordTypes => RecordTypes.Count(recordType => !recordType.HeaderImportSupported);
}
