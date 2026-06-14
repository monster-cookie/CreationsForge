using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface ITerminalMarkerParameterRepository
{
    void ReplaceRecordMarkerParameters(IHasTerminalMarkerParametersRecordDTO record);

    IReadOnlyList<TerminalMarkerParameterDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey);
}
