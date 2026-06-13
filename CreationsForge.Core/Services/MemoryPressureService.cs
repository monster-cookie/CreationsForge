using System.Diagnostics;
using System.Runtime;
using CreationsForge.Core.Services.Interfaces;
using Serilog;

namespace CreationsForge.Core.Services;

public class MemoryPressureService : IMemoryPressureService
{
    private readonly ILogger Logger = Log.ForContext<MemoryPressureService>();

    public void CollectAfterBulkImportPhase(string phaseName)
    {
        var beforeManagedBytes = GC.GetTotalMemory(forceFullCollection: false);
        using var processBefore = Process.GetCurrentProcess();
        var beforeWorkingSetBytes = processBefore.WorkingSet64;
        var beforePrivateBytes = processBefore.PrivateMemorySize64;
        var beforeHandleCount = processBefore.HandleCount;
        var beforeThreadCount = processBefore.Threads.Count;

        Logger.Information(
            "Collecting memory after bulk import phase {PhaseName}; managed bytes before: {ManagedBytes}, working set bytes before: {WorkingSetBytes}, private bytes before: {PrivateBytes}, handle count before: {HandleCount}, thread count before: {ThreadCount}",
            phaseName,
            beforeManagedBytes,
            beforeWorkingSetBytes,
            beforePrivateBytes,
            beforeHandleCount,
            beforeThreadCount);

        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, blocking: true, compacting: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, blocking: true, compacting: true);

        var afterManagedBytes = GC.GetTotalMemory(forceFullCollection: false);
        using var processAfter = Process.GetCurrentProcess();
        Logger.Information(
            "Collected memory after bulk import phase {PhaseName}; managed bytes after: {ManagedBytes}, working set bytes after: {WorkingSetBytes}, private bytes after: {PrivateBytes}, handle count after: {HandleCount}, thread count after: {ThreadCount}",
            phaseName,
            afterManagedBytes,
            processAfter.WorkingSet64,
            processAfter.PrivateMemorySize64,
            processAfter.HandleCount,
            processAfter.Threads.Count);
    }
}
