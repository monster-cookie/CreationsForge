namespace CreationsForge.Core.Services.Interfaces;

public interface IMemoryPressureService
{
    void CollectAfterBulkImportPhase(string phaseName);
}
