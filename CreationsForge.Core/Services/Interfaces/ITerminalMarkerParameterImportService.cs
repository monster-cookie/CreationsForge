using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.Services.Interfaces;

public interface ITerminalMarkerParameterImportService
{
    void ReplaceRecordMarkerParameters(IHasTerminalMarkerParametersRecordDTO record);
}
