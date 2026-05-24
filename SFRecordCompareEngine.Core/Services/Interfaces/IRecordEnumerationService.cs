using System.Collections;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IRecordEnumerationService
{
    IEnumerable<RecordEnumerationDTO>? GetRecords(IStarfieldModGetter plugin, string recordType);
    IEnumerable? GetRawRecords(IStarfieldModGetter plugin, string recordType);
}
