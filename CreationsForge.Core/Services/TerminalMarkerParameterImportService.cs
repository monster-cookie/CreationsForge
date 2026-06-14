using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class TerminalMarkerParameterImportService : ITerminalMarkerParameterImportService
{
    private readonly ITerminalMarkerParameterRepository TerminalMarkerParameterRepository;

    public TerminalMarkerParameterImportService(ITerminalMarkerParameterRepository terminalMarkerParameterRepository)
    {
        TerminalMarkerParameterRepository = terminalMarkerParameterRepository;
    }

    public void ReplaceRecordMarkerParameters(IHasTerminalMarkerParametersRecordDTO record)
    {
        TerminalMarkerParameterRepository.ReplaceRecordMarkerParameters(record);
    }
}
