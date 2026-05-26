using Serilog;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class RecordService : IRecordService
{
    private readonly ILogger Logger = Log.ForContext<RecordService>();
}
